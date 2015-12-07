using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DigitalPlatform;

namespace dp2Catalog
{
    public partial class EaccForm : Form
    {
        public MainForm MainForm = null;
        DigitalPlatform.Stop stop = null;

        public EaccForm()
        {
            InitializeComponent();
        }

        private void EaccForm_Load(object sender, EventArgs e)
        {
            stop = new DigitalPlatform.Stop();
            stop.Register(MainForm.stopManager, true);	// ����������


            this.textBox_unihanFilenames.Text = MainForm.AppInfo.GetString(
                "eacc_form",
                "unihan_filename",
                "");
            this.textBox_e2uFilename.Text = MainForm.AppInfo.GetString(
                "eacc_form",
                "e2u_filename",
                "");
            /*
            this.textBox_u2eFilename.Text = MainForm.applicationInfo.GetString(
                "eacc_form",
                "u2e_filename",
                "");
             * */

            Global.FillEncodingList(this.comboBox_codePage,
                false);

        }

        private void EaccForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (stop != null) // �������
            {
                stop.Style = StopStyle.None;    // ��Ҫǿ���ж�
                stop.DoStop();

                stop.Unregister();	// ����������
                stop = null;
            }

            if (this.MainForm != null && this.MainForm.AppInfo != null)
            {
                MainForm.AppInfo.SetString(
                    "eacc_form",
                    "unihan_filename",
                    this.textBox_unihanFilenames.Text);
                MainForm.AppInfo.SetString(
                    "eacc_form",
                    "e2u_filename",
                    this.textBox_e2uFilename.Text);
                /*
                MainForm.applicationInfo.SetString(
                    "eacc_form",
                    "u2e_filename",
                    this.textBox_u2eFilename.Text);
                 * */
            }
        }

        private void button_begin_Click(object sender, EventArgs e)
        {
            int nRet = 0;
            string strError = "";
            nRet = BuildCharsetTable(out strError);
            if (nRet == -1)
            {
                MessageBox.Show(this, strError);
            }
            else
            {
                MessageBox.Show(this, "OK");
            }
        }

        int BuildCharsetTable(out string strError)
        {
            strError = "";

            CharsetTable charsettable_e2u = new CharsetTable();
            charsettable_e2u.Open(true);
            if (this.textBox_unihanFilenames.Text == "")
            {
                strError = "��δָ�������ļ���";
                return -1;
            }

            stop.OnStop += new StopEventHandler(this.DoStop);
            stop.SetMessage("���ڴ�������ļ� ...");
            stop.BeginLoop();

            EnableControls(false);

            this.Update();
            this.MainForm.Update();

            try
            {
                int nRet = 0;

                for (int i = 0; i < this.textBox_unihanFilenames.Lines.Length; i++)
                {
                    string strSourceFileName = this.textBox_unihanFilenames.Lines[i];
                    if (String.IsNullOrEmpty(strSourceFileName) == true)
                        continue;

                    string strStyle = "";
                    nRet = strSourceFileName.IndexOf(" ");
                    if (nRet != -1)
                    {
                        strStyle = strSourceFileName.Substring(nRet + 1).Trim();
                        strSourceFileName = strSourceFileName.Substring(0, nRet).Trim();
                    }

                    StreamReader sr = null;
                    try
                    {
                        sr = new StreamReader(strSourceFileName);
                    }
                    catch (Exception ex)
                    {
                        strError = "�ļ� " + strSourceFileName + " ��ʧ��: " + ex.Message;
                        return -1;
                    }

                    this.MainForm.ToolStripProgressBar.Minimum = 0;
                    this.MainForm.ToolStripProgressBar.Maximum = (int)sr.BaseStream.Length;
                    this.MainForm.ToolStripProgressBar.Value = 0;

                    try
                    {
                        for (; ; )
                        {
                            Application.DoEvents();	// ���ý������Ȩ

                            if (stop != null)
                            {
                                if (stop.State != 0)
                                {
                                    strError = "�û��ж�";
                                    return -1;
                                }
                            }

                            string strLine = sr.ReadLine();
                            if (strLine == null)
                                break;

                            if (strLine.Length < 1)
                                goto CONTINUE;

                            // ע����
                            if (strLine[0] == '#')
                                goto CONTINUE;


                            if (String.IsNullOrEmpty(strStyle) == true)
                            {
                                nRet = strLine.IndexOf("\t", 0);
                                if (nRet == -1)
                                    goto CONTINUE;	// ��ʽ������

                                string strPart1 = strLine.Substring(0, nRet).Trim().ToUpper();

                                strLine = strLine.Substring(nRet + 1);

                                nRet = strLine.IndexOf("\t", 0);
                                if (nRet == -1)
                                    goto CONTINUE;	// ��ʽ������

                                string strPart2 = strLine.Substring(0, nRet).Trim();
                                string strPart3 = strLine.Substring(nRet + 1).Trim().ToUpper();

                                strPart1 = strPart1.Substring(2);	// ȥ��'U+'

                                if (strPart2 != "kEACC")
                                    goto CONTINUE;	// ����ص���

                                strLine = strPart1 + "\t" + strPart3;

                                CharsetItem item = new CharsetItem();
                                item.Content = strLine;
                                charsettable_e2u.Add(item); // charsettable_u2e.Add(item);

                                strLine = strPart3 + "\t" + strPart1;

                                item = new CharsetItem();
                                item.Content = strLine;
                                charsettable_e2u.Add(item); //charsettable_e2u.Add(item);	// ANSI�ַ���

                                stop.SetMessage(strLine.Replace("\t", "   "));
                            }

                            if (strStyle == "6+4")
                            {
                                nRet = strLine.IndexOfAny(new char[] { '\t', ' ' }, 0);
                                if (nRet == -1)
                                    goto CONTINUE;	// ��ʽ������

                                string strPart1 = strLine.Substring(0, nRet).Trim().ToUpper();

                                string strPart2 = strLine.Substring(nRet + 1).Trim().ToUpper();


                                CharsetItem item = new CharsetItem();
                                strLine = strPart1 + "\t" + strPart2;
                                item.Content = strLine;
                                charsettable_e2u.Add(item); // charsettable_u2e.Add(item);


                                item = new CharsetItem();
                                strLine = strPart2 + "\t" + strPart1;
                                item.Content = strLine;
                                charsettable_e2u.Add(item); //charsettable_e2u.Add(item);	// ANSI�ַ���

                                stop.SetMessage(strLine.Replace("\t", "   "));
                            }


                        CONTINUE:
                            // ��ʾ������
                            this.MainForm.ToolStripProgressBar.Value = (int)sr.BaseStream.Position;
                        }

                    }
                    finally
                    {
                        sr.Close();
                    }

                }

                stop.SetMessage("���ڸ��ƺ�����...");
                this.Update();
                this.MainForm.Update();

                string strDataFileName = "";
                string strIndexFileName = "";

                if (String.IsNullOrEmpty(this.textBox_e2uFilename.Text) == false)
                {
                    charsettable_e2u.Sort();
                    charsettable_e2u.Detach(out strDataFileName,
                        out strIndexFileName);

                    File.Delete(this.textBox_e2uFilename.Text);
                    File.Move(strDataFileName,
                        this.textBox_e2uFilename.Text);

                    File.Delete(this.textBox_e2uFilename.Text + ".index");
                    File.Move(strIndexFileName,
                        this.textBox_e2uFilename.Text + ".index");
                }

            }
            finally
            {
                stop.EndLoop();
                stop.OnStop -= new StopEventHandler(this.DoStop);
                stop.Initial("");

                EnableControls(true);
            }

            return 0;
        }

        void DoStop(object sender, StopEventArgs e)
        {
            /*
            if (this.Channel != null)
                this.Channel.Cancel();
             * */
        }

        void EnableControls(bool bEnable)
        {
            this.textBox_e2uFilename.Enabled = bEnable;
            //this.textBox_u2eFilename.Enabled = bEnable;
            this.textBox_unihanFilenames.Enabled = bEnable;

            this.button_findE2uFilename.Enabled = bEnable;
            this.button_findOriginFilename.Enabled = bEnable;
            //this.button_findU2eFilename.Enabled = bEnable;
            this.button_begin.Enabled = bEnable;
        }

        // ����Eacc������Unicode����
        private void button_searchE2u_Click(object sender, EventArgs e)
        {
            if (this.textBox_e2uFilename.Text == "")
            {
                MessageBox.Show(this, "��δָ�� e2u ����ļ�");
                return;
            }

            this.textBox_unicodeCode.Text = "";

            CharsetTable charsettable_e2u = new CharsetTable();

            charsettable_e2u.Attach(this.textBox_e2uFilename.Text,
                this.textBox_e2uFilename.Text + ".index");

            try
            {

                string strValue = "";
                // return:
                //      -1  not found
                int nRet = charsettable_e2u.Search(
                    this.textBox_eaccCode.Text.ToUpper(),
                    out strValue);

                if (nRet == -1)
                {
                    MessageBox.Show(this, "û���ҵ�");
                }
                else
                {
                    this.textBox_unicodeCode.Text = strValue;
                }
            }
            finally
            {
                string strTemp1;
                string strTemp2;

                charsettable_e2u.Detach(out strTemp1,
                    out strTemp2);
            }


        }

        // ����Unicode������Eacc����
        private void button_searchU2e_Click(object sender, EventArgs e)
        {
            if (this.textBox_e2uFilename.Text == "")
            {
                MessageBox.Show(this, "��δָ�� ����ļ�");
                return;
            }

            this.textBox_eaccCode.Text = "";

            CharsetTable charsettable_u2e = new CharsetTable();

            charsettable_u2e.Attach(this.textBox_e2uFilename.Text,
                this.textBox_e2uFilename.Text + ".index");

            try
            {

                string strValue = "";
                // return:
                //      -1  not found
                int nRet = charsettable_u2e.Search(
                    this.textBox_unicodeCode.Text.ToUpper(),
                    out strValue);

                if (nRet == -1)
                {
                    MessageBox.Show(this, "û���ҵ�");
                }
                else
                {
                    this.textBox_eaccCode.Text = strValue;
                }
            }
            finally
            {
                string strTemp1;
                string strTemp2;

                charsettable_u2e.Detach(out strTemp1,
                    out strTemp2);
            }
        }

        private void button_e2uStringConvert_Click(object sender, EventArgs e)
        {
            if (this.textBox_e2uFilename.Text == "")
            {
                MessageBox.Show(this, "��δָ�� e2u ����ļ�");
                return;
            }

            this.textBox_unicodeCode.Text = "";

            CharsetTable charsettable_e2u = new CharsetTable();

            charsettable_e2u.Attach(this.textBox_e2uFilename.Text,
                this.textBox_e2uFilename.Text + ".index");

            Marc8Encoding encoding = new Marc8Encoding(charsettable_e2u,
                this.MainForm.DataDir + "\\asciicodetables.xml");

            try
            {
                string strText = "";

                if (this.textBox_field066value.Text != "")
                    encoding.SetDefaultCodePage(this.textBox_field066value.Text.Replace('|', (char)31));

                encoding.Marc8_to_Unicode(
                    Encoding.ASCII.GetBytes(
                    this.textBox_eaccString.Text),
                    out strText);

                this.textBox_unicodeString.Text = strText;
            }
            finally
            {
                string strTemp1;
                string strTemp2;

                charsettable_e2u.Detach(out strTemp1,
                    out strTemp2);
            }


        }

        private void button_u2eStringConvert_Click(object sender, EventArgs e)
        {

        }

        private void button_codePage_8tou_Click(object sender, EventArgs e)
        {
            int nValue = Convert.ToInt32(this.textBox_codePage_8bitCode.Text, 16);

            byte[] data = new byte[1];
            data[0] = (byte)nValue;

            Encoding encoding = Encoding.GetEncoding(this.comboBox_codePage.Text);

            char[] chars = encoding.GetChars(data);

            this.textBox_codePage_unicodeCode.Text = Convert.ToString((int)chars[0], 16).PadLeft(4, '0');
        }



    }
}