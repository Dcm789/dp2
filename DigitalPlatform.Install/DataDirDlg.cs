using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DigitalPlatform.IO;

namespace DigitalPlatform.Install
{
    public partial class DataDirDlg : Form
    {
        public string MessageBoxTitle = "setup";

        public DataDirDlg()
        {
            InitializeComponent();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            if (this.DataDir == "")
            {
                MessageBox.Show(this, "��δָ������Ŀ¼��");
                return;
            }

        REDO:
            if (Directory.Exists(this.DataDir) == false)
            {
                string strText = "����Ŀ¼ '" + this.DataDir + "' �����ڡ�\r\n\r\n�Ƿ񴴽���Ŀ¼?";
                DialogResult result = MessageBox.Show(this,
                    strText,
                    this.MessageBoxTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);
                if (result == DialogResult.No)
                {
                    MessageBox.Show(this, "���ֶ���������Ŀ¼ " + this.DataDir);
                    return;
                }

                PathUtil.TryCreateDir(this.DataDir);

                if (Directory.Exists(this.DataDir) == false)
                {
                    MessageBox.Show(this, "����Ŀ¼ " + this.DataDir + "����ʧ�ܡ�");
                    return;
                }
                goto REDO;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public string DataDir
        {
            get
            {
                return this.textBox_dataDir.Text;
            }
            set
            {
                this.textBox_dataDir.Text = value;
            }
        }

        // ע��
        public string Comment
        {
            get
            {
                return this.textBox_message.Text;
            }
            set
            {
                this.textBox_message.Text = value;
            }
        }
    }
}