using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;

using DigitalPlatform;
using DigitalPlatform.Xml;
using DigitalPlatform.Marc;
using DigitalPlatform.Script;
using DigitalPlatform.GcatClient;
using DigitalPlatform.Text;
using DigitalPlatform.IO;
using DigitalPlatform.CommonControl;

using DigitalPlatform.CirculationClient;
using DigitalPlatform.GUI;
using DigitalPlatform.LibraryClient;
using DigitalPlatform.LibraryClient.localhost;

namespace dp2Catalog
{
    /// <summary>
    /// Summary description for Host.
    /// </summary>
    public class MarcDetailHost : IDisposable
    {
        public MarcDetailForm DetailForm = null;
        public Assembly Assembly = null;
        public ScriptActionCollection ScriptActions = new ScriptActionCollection();

        public MarcDetailHost()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void Dispose()
        {
            if (this.GcatChannel != null)
                this.GcatChannel.Dispose();
        }

        public void Invoke(string strFuncName)
        {
            Type classType = this.GetType();

            // newһ��Host��������
            classType.InvokeMember(strFuncName,
                BindingFlags.DeclaredOnly |
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.InvokeMethod
                ,
                null,
                this,
                null);

        }

        public void Invoke(string strFuncName,
    object sender,
    GenerateDataEventArgs e)
        {
            Type classType = this.GetType();

            while (classType != null)
            {
                try
                {
                    // �����������ĳ�Ա����
                    classType.InvokeMember(strFuncName,
                        BindingFlags.DeclaredOnly |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.InvokeMethod
                        ,
                        null,
                        this,
                        new object[] { sender, e });
                    return;

                }
                catch (System.MissingMethodException/*ex*/)
                {
                    classType = classType.BaseType;
                    if (classType == null)
                        break;
                }
            }

            if (strFuncName == "Main")
            {
                classType = this.GetType();

                // �ϵ�HostEventArgs e ����
                while (classType != null)
                {
                    try
                    {
                        // �����������ĳ�Ա����
                        classType.InvokeMember(strFuncName,
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod
                            ,
                            null,
                            this,
                            new object[] { sender, new HostEventArgs() });
                        return;

                    }
                    catch (System.MissingMethodException/*ex*/)
                    {
                        classType = classType.BaseType;
                        if (classType == null)
                            break;
                    }
                }
            }


            classType = this.GetType();

            while (classType != null)
            {
                try
                {
                    // ������ǰ����д���� -- û�в���
                    classType.InvokeMember(strFuncName,
                        BindingFlags.DeclaredOnly |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.InvokeMethod
                        ,
                        null,
                        this,
                        null);
                    return;
                }
                catch (System.MissingMethodException/*ex*/)
                {
                    classType = classType.BaseType;
                }
            }

            throw new Exception("���� void " + strFuncName + "(object sender, GenerateDataEventArgs e) �� void " + strFuncName + "() û���ҵ�");
        }

        public virtual void Main(object sender, HostEventArgs e)
        {

        }

        // parameters:
        //      strIndicator    �ֶ�ָʾ���������null���ã����ʾ����ָʾ������ɸѡ
        // return:
        //      0   û���ҵ�ƥ�����������
        //      >=1 �ҵ��������ҵ��������������
        public static int GetPinyinCfgLine(XmlDocument cfg_dom,
            string strFieldName,
            string strIndicator,
            out List<PinyinCfgItem> cfg_items)
        {
            cfg_items = new List<PinyinCfgItem>();

            XmlNodeList nodes = cfg_dom.DocumentElement.SelectNodes("item");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];

                PinyinCfgItem item = new PinyinCfgItem(node);

                if (item.FieldName != strFieldName)
                    continue;

                if (string.IsNullOrEmpty(item.IndicatorMatchCase) == false
                    && string.IsNullOrEmpty(strIndicator) == false)
                {
                    if (MarcUtil.MatchIndicator(item.IndicatorMatchCase, strIndicator) == false)
                        continue;
                }

                cfg_items.Add(item);
            }

            return cfg_items.Count;
        }

        // ��װ��İ汾��������ǰ�ĵ���ϰ��
        public int AddPinyin(string strCfgXml,
    bool bUseCache = true,
    PinyinStyle style = PinyinStyle.None,
    string strPrefix = "",
        bool bAutoSel = false)
        {
            return AddPinyin(strCfgXml,
                bUseCache,
                style,
                strPrefix,
                bAutoSel ? "auto" : "");
        }

        /// <summary>
        /// Ϊ MARC �༭���ڵļ�¼��ƴ��
        /// </summary>
        /// <param name="strCfgXml">ƴ������ XML</param>
        /// <param name="bUseCache">�Ƿ�ʹ�ü�¼����ǰ����Ľ����</param>
        /// <param name="style">���</param>
        /// <param name="strPrefix">ǰ׺�ַ�����ȱʡΪ�� [��ʱû��ʹ�ñ�����]</param>
        /// <param name="strDuoyinStyle">�Ƿ��Զ�ѡ������֡�auto/first ֮һ�������</param>
        /// <returns>-1: ���������жϵ����; 0: ����</returns>
        public virtual int AddPinyin(string strCfgXml,
            bool bUseCache, //  = true,
            PinyinStyle style,  // = PinyinStyle.None,
            string strPrefix,
            string strDuoyinStyle)
        // bool bAutoSel = false)
        {
            string strError = "";
            XmlDocument cfg_dom = new XmlDocument();
            try
            {
                cfg_dom.LoadXml(strCfgXml);
            }
            catch (Exception ex)
            {
                strError = "strCfgXmlװ�ص�XMLDOMʱ����: " + ex.Message;
                goto ERROR1;
            }


            this.DetailForm.MarcEditor.Enabled = false;

            Hashtable old_selected = (bUseCache == true) ? this.DetailForm.GetSelectedPinyin() : new Hashtable();
            Hashtable new_selected = new Hashtable();

            try
            {
                // PinyinStyle style = PinyinStyle.None;	// �������޸�ƴ����Сд���

                for (int i = 0; i < DetailForm.MarcEditor.Record.Fields.Count; i++)
                {
                    Field field = DetailForm.MarcEditor.Record.Fields[i];

                    List<PinyinCfgItem> cfg_items = null;
                    int nRet = GetPinyinCfgLine(
                        cfg_dom,
                        field.Name,
                        field.Indicator,
                        out cfg_items);
                    if (nRet <= 0)
                        continue;

                    string strHanzi = "";
                    string strNextSubfieldName = "";

                    string strField = field.Text;

                    foreach (PinyinCfgItem item in cfg_items)
                    {
                        for (int k = 0; k < item.From.Length; k++)
                        {
                            if (item.From.Length != item.To.Length)
                            {
                                strError = "�������� fieldname='" + item.FieldName + "' from='" + item.From + "' to='" + item.To + "' ����from��to����ֵ���ַ�������";
                                goto ERROR1;
                            }

                            string from = new string(item.From[k], 1);
                            string to = new string(item.To[k], 1);
                            for (int j = 0; ; j++)
                            {

                                // return:
                                //		-1	error
                                //		0	not found
                                //		1	found

                                nRet = MarcUtil.GetSubfield(strField,
                                    ItemType.Field,
                                    from,
                                    j,
                                    out strHanzi,
                                    out strNextSubfieldName);
                                if (nRet != 1)
                                    break;
                                if (strHanzi.Length <= 1)
                                    break;

                                strHanzi = strHanzi.Substring(1);

                                // 2013/6/13
                                if (MarcDetailHost.ContainHanzi(strHanzi) == false)
                                    continue;

                                string strPinyin = "";

                                strPinyin = (string)old_selected[strHanzi];
                                if (string.IsNullOrEmpty(strPinyin) == true)
                                {

                                    // ���ַ����еĺ��ֺ�ƴ������
                                    // return:
                                    //      -1  ����
                                    //      0   �û�ϣ���ж�
                                    //      1   ����
                                    if (string.IsNullOrEmpty(this.DetailForm.MainForm.PinyinServerUrl) == true
                                       || this.DetailForm.MainForm.ForceUseLocalPinyinFunc == true)
                                    {
                                        nRet = this.DetailForm.MainForm.HanziTextToPinyin(
                                            this.DetailForm,
                                            true,	// ���أ�����
                                            strHanzi,
                                            style,
                                            strDuoyinStyle,
                                            out strPinyin,
                                            out strError);
                                    }
                                    else
                                    {
                                        // �����ַ���ת��Ϊƴ��
                                        // ����������Ѿ�MessageBox������strError��һ�ַ���Ϊ�ո�
                                        // return:
                                        //      -1  ����
                                        //      0   �û�ϣ���ж�
                                        //      1   ����
                                        //      2   ����ַ�������û���ҵ�ƴ���ĺ���
                                        nRet = this.DetailForm.MainForm.SmartHanziTextToPinyin(
                                            this.DetailForm,
                                            strHanzi,
                                            style,
                                            strDuoyinStyle,
                                            out strPinyin,
                                            out strError);
                                    }
                                    if (nRet == -1)
                                    {
                                        new_selected = null;
                                        goto ERROR1;
                                    }
                                    if (nRet == 0)
                                    {
                                        new_selected = null;
                                        strError = "�û��жϡ�ƴ�����ֶ����ݿ��ܲ�������";
                                        goto ERROR1;
                                    }
                                }

                                if (new_selected != null && nRet != 2)
                                    new_selected[strHanzi] = strPinyin;

                                nRet = MarcUtil.DeleteSubfield(
                                    ref strField,
                                    to,
                                    j);
                                nRet = MarcUtil.InsertSubfield(
                                    ref strField,
                                    from,
                                    j,
                                    new string(MarcUtil.SUBFLD, 1) + to + strPinyin,
                                    1);
                                field.Text = strField;
                            }
                        }
                    }
                }

                if (new_selected != null)
                    this.DetailForm.SetSelectedPinyin(new_selected);
            }
            finally
            {
                this.DetailForm.MarcEditor.Enabled = true;
                this.DetailForm.MarcEditor.Focus();
            }
            return 0;
        ERROR1:
            if (string.IsNullOrEmpty(strError) == false)
            {
                if (strError[0] != ' ')
                    MessageBox.Show(this.DetailForm, strError);
            }
            return -1;
        }

        public virtual void RemovePinyin(string strCfgXml)
        {
            string strError = "";
            XmlDocument cfg_dom = new XmlDocument();
            try
            {
                cfg_dom.LoadXml(strCfgXml);
            }
            catch (Exception ex)
            {
                strError = "strCfgXmlװ�ص�XMLDOMʱ����: " + ex.Message;
                goto ERROR1;
            }

            this.DetailForm.MarcEditor.Enabled = false;

            try
            {
                for (int i = 0; i < DetailForm.MarcEditor.Record.Fields.Count; i++)
                {
                    Field field = DetailForm.MarcEditor.Record.Fields[i];

                    List<PinyinCfgItem> cfg_items = null;
                    int nRet = GetPinyinCfgLine(
                        cfg_dom,
                        field.Name,
                        field.Indicator,    // TODO: ���Բ�����ָʾ�������������ɾ������Ѱ��Χ
                        out cfg_items);
                    if (nRet <= 0)
                        continue;

                    string strField = field.Text;
                    bool bChanged = false;
                    foreach (PinyinCfgItem item in cfg_items)
                    {
                        for (int k = 0; k < item.To.Length; k++)
                        {
                            string to = new string(item.To[k], 1);
                            for (; ; )
                            {
                                // ɾ��һ�����ֶ�
                                // ��ʵԭ����ReplaceSubfield()Ҳ���Ե���ɾ����ʹ��
                                // return:
                                //      -1  ����
                                //      0   û���ҵ����ֶ�
                                //      1   �ҵ���ɾ��
                                nRet = MarcUtil.DeleteSubfield(
                                    ref strField,
                                    to,
                                    0);
                                if (nRet != 1)
                                    break;
                                bChanged = true;
                            }
                        }
                    }
                    if (bChanged == true)
                        field.Text = strField;
                }
            }
            finally
            {
                this.DetailForm.MarcEditor.Enabled = true;
                this.DetailForm.MarcEditor.Focus();
            }
            return;
        ERROR1:
            MessageBox.Show(this.DetailForm, strError);
        }

        DigitalPlatform.GcatClient.Channel GcatChannel = null;

        // ������ߺ�
        // return:
        //      -1  error
        //      0   canceled
        //      1   succeed
        public int GetGcatAuthorNumber(string strGcatWebServiceUrl,
            string strAuthor,
            out string strAuthorNumber,
            out string strError)
        {
            strError = "";
            strAuthorNumber = "";

            if (String.IsNullOrEmpty(strGcatWebServiceUrl) == true)
                strGcatWebServiceUrl = "http://dp2003.com/dp2library/";  // "http://dp2003.com/gcatserver/" //  "http://dp2003.com/dp2libraryws/gcat.asmx";

            if (strGcatWebServiceUrl.IndexOf(".asmx") != -1)
            {

                if (this.GcatChannel == null)
                    this.GcatChannel = new DigitalPlatform.GcatClient.Channel();

                string strDebugInfo = "";

                BeginGcatLoop("���ڻ�ȡ '" + strAuthor + "' �����ߺţ��� " + strGcatWebServiceUrl + " ...");
                try
                {
                    // return:
                    //      -1  error
                    //      0   canceled
                    //      1   succeed
                    int nRet = this.GcatChannel.GetNumber(
                        this.DetailForm.stop,
                        this.DetailForm,
                        strGcatWebServiceUrl,
                        strAuthor,
                        true,	// bSelectPinyin
                        true,	// bSelectEntry
                        true,	// bOutputDebugInfo
                        new DigitalPlatform.GcatClient.BeforeLoginEventHandle(gcat_channel_BeforeLogin),
                        out strAuthorNumber,
                        out strDebugInfo,
                        out strError);
                    if (nRet == -1)
                    {
                        strError = "ȡ ���� '" + strAuthor + "' ֮����ʱ���� : " + strError;
                        return -1;
                    }

                    return nRet;
                }
                finally
                {
                    EndGcatLoop();
                }
            }
            else if (strGcatWebServiceUrl.Contains("gcat"))
            {
                // �µ�WebService

                string strID = this.DetailForm.MainForm.AppInfo.GetString("DetailHost", "gcat_id", "");
                bool bSaveID = this.DetailForm.MainForm.AppInfo.GetBoolean("DetailHost", "gcat_saveid", false);

                Hashtable question_table = (Hashtable)this.DetailForm.MainForm.ParamTable["question_table"];
                if (question_table == null)
                    question_table = new Hashtable();

            REDO_GETNUMBER:
                string strDebugInfo = "";

                BeginGcatLoop("���ڻ�ȡ '" + strAuthor + "' �����ߺţ��� " + strGcatWebServiceUrl + " ...");
                try
                {
                    // return:
                    //      -1  error
                    //      0   canceled
                    //      1   succeed
                    int nRet = GcatNew.GetNumber(
                        ref question_table,
                        this.DetailForm.stop,
                        this.DetailForm,
                        strGcatWebServiceUrl,
                        strID, // ID
                        strAuthor,
                        true,	// bSelectPinyin
                        true,	// bSelectEntry
                        true,	// bOutputDebugInfo
                        out strAuthorNumber,
                        out strDebugInfo,
                        out strError);
                    if (nRet == -1)
                    {
                        strError = "ȡ ���� '" + strAuthor + "' ֮����ʱ���� : " + strError;
                        return -1;
                    }
                    if (nRet == -2)
                    {
                        IdLoginDialog login_dlg = new IdLoginDialog();
                        GuiUtil.AutoSetDefaultFont(login_dlg);
                        login_dlg.Text = "������ߺ� -- "
                            + ((string.IsNullOrEmpty(strID) == true) ? "������ID" : strError);
                        login_dlg.ID = strID;
                        login_dlg.SaveID = bSaveID;
                        login_dlg.StartPosition = FormStartPosition.CenterScreen;
                        if (login_dlg.ShowDialog(this.DetailForm) == DialogResult.Cancel)
                        {
                            return -1;
                        }

                        strID = login_dlg.ID;
                        bSaveID = login_dlg.SaveID;
                        if (login_dlg.SaveID == true)
                        {
                            this.DetailForm.MainForm.AppInfo.SetString("DetailHost", "gcat_id", strID);
                        }
                        else
                        {
                            this.DetailForm.MainForm.AppInfo.SetString("DetailHost", "gcat_id", "");
                        }
                        this.DetailForm.MainForm.AppInfo.SetBoolean("DetailHost", "gcat_saveid", bSaveID);
                        goto REDO_GETNUMBER;
                    }

                    this.DetailForm.MainForm.ParamTable["question_table"] = question_table;

                    return nRet;
                }
                finally
                {
                    EndGcatLoop();
                }
            }
            else // dp2library ������
            {
                Hashtable question_table = (Hashtable)Program.MainForm.ParamTable["question_table"];
                if (question_table == null)
                    question_table = new Hashtable();

                string strDebugInfo = "";

                BeginGcatLoop("���ڻ�ȡ '" + strAuthor + "' �����ߺţ��� " + strGcatWebServiceUrl + " ...");
                try
                {
                    // return:
                    //      -1  error
                    //      0   canceled
                    //      1   succeed
                    long nRet = GetAuthorNumber(
                        ref question_table,
                        this.DetailForm.stop,
                        this.DetailForm,
                        strGcatWebServiceUrl,
                        strAuthor,
                        true,	// bSelectPinyin
                        true,	// bSelectEntry
                        true,	// bOutputDebugInfo
                        out strAuthorNumber,
                        out strDebugInfo,
                        out strError);
                    if (nRet == -1)
                    {
                        strError = "ȡ ���� '" + strAuthor + "' ֮����ʱ���� : " + strError;
                        return -1;
                    }
                    Program.MainForm.ParamTable["question_table"] = question_table;
                    return (int)nRet;
                }
                finally
                {
                    EndGcatLoop();
                }
            }

        }

        // �ⲿ����
        // ����汾�����л�������ʹ𰸵Ĺ���
        // return:
        //      -2  strID��֤ʧ��
        //      -1  error
        //      0   canceled
        //      1   succeed
        public static int GetAuthorNumber(
            ref Hashtable question_table,
            Stop stop,
            System.Windows.Forms.IWin32Window parent,
            string strUrl,
            string strAuthor,
            bool bSelectPinyin,
            bool bSelectEntry,
            bool bOutputDebugInfo,
            out string strNumber,
            out string strDebugInfo,
            out string strError)
        {
            strError = "";
            strDebugInfo = "";
            strNumber = "";

            long nRet = 0;

            Question[] questions = (Question[])question_table[strAuthor];
            if (questions == null)
                questions = new Question[0];

            for (; ; )
            {
                LibraryChannel channel = Program.MainForm.GetChannel(strUrl, "public");
                try
                {
                    // �����������catch ͨѶ�� exeption������
                    // return:
                    //		-3	��Ҫ�ش�����
                    //      -2  strID��֤ʧ��
                    //      -1  ����
                    //      0   �ɹ�
                    nRet = channel.GetAuthorNumber(
                        strAuthor,
                        bSelectPinyin,
                        bSelectEntry,
                        bOutputDebugInfo,
                        ref questions,
                        out strNumber,
                        out strDebugInfo,
                        out strError);
                    if (nRet != -3)
                        break;

                    Debug.Assert(nRet == -3, "");
                }
                finally
                {
                    Program.MainForm.ReturnChannel(channel);
                }

                string strTitle = strError;

                string strQuestion = questions[questions.Length - 1].Text;

                QuestionDlg dlg = new QuestionDlg();
                GuiUtil.AutoSetDefaultFont(dlg);
                dlg.StartPosition = FormStartPosition.CenterScreen;
                dlg.label_messageTitle.Text = strTitle;
                dlg.textBox_question.Text = strQuestion.Replace("\n", "\r\n");
                dlg.ShowDialog(parent);

                if (dlg.DialogResult != DialogResult.OK)
                {
                    strError = "����";
                    return 0;
                }

                questions[questions.Length - 1].Answer = dlg.textBox_result.Text;

                question_table[strAuthor] = questions;  // ����
            }

            if (nRet == -1)
                return -1;
            if (nRet == -2)
                return -2;  // strID��֤ʧ��

            return 1;
        }

        // GCATͨ����¼ �ɵķ�ʽ
        public void gcat_channel_BeforeLogin(object sender,
            DigitalPlatform.GcatClient.BeforeLoginEventArgs e)
        {
            string strUserName = (string)this.DetailForm.MainForm.ParamTable["author_number_account_username"];
            string strPassword = (string)this.DetailForm.MainForm.ParamTable["author_number_account_password"];

            if (String.IsNullOrEmpty(strUserName) == true)
            {
                strUserName = "test";
                strPassword = "";
            }

            // ֱ����̽
            if (!(e.UserName == strUserName && e.Failed == true)
                && strUserName != "")
            {
                e.UserName = strUserName;
                e.Password = strPassword;
                return;
            }

            LoginDlg dlg = new LoginDlg();
            GuiUtil.SetControlFont(dlg, this.DetailForm.MainForm.Font);

            if (e.Failed == true)
                dlg.textBox_comment.Text = "��¼ʧ�ܡ������ߺ��빦����Ҫ���µ�¼";
            else
                dlg.textBox_comment.Text = "�����ߺ��빦����Ҫ��¼";

            dlg.textBox_serverAddr.Text = e.GcatServerUrl;
            dlg.textBox_userName.Text = strUserName;
            dlg.textBox_password.Text = strPassword;
            dlg.checkBox_savePassword.Checked = true;

            dlg.textBox_serverAddr.Enabled = false;
            dlg.TopMost = true; // 2009/11/12 ��ΪShowDialog(null)��Ϊ�˷�ֹ�Ի��򱻷��ڷǶ���
            dlg.ShowDialog(null);
            if (dlg.DialogResult != DialogResult.OK)
            {
                e.Cancel = true;    // 2009/11/12 ���ȱ��һ�䣬�����Cancel����Ȼ���µ�����¼�Ի���
                return;
            }

            strUserName = dlg.textBox_userName.Text;
            strPassword = dlg.textBox_password.Text;

            e.UserName = strUserName;
            e.Password = strPassword;

            this.DetailForm.MainForm.ParamTable["author_number_account_username"] = strUserName;
            this.DetailForm.MainForm.ParamTable["author_number_account_password"] = strPassword;
        }

        void DoGcatStop(object sender, StopEventArgs e)
        {
            if (this.GcatChannel != null)
                this.GcatChannel.Abort();
        }

        bool bMarcEditorFocued = false;

        public void BeginGcatLoop(string strMessage)
        {
            bMarcEditorFocued = this.DetailForm.MarcEditor.Focused;
            this.DetailForm.EnableControls(false);

            Stop stop = this.DetailForm.stop;

            stop.OnStop += new StopEventHandler(this.DoGcatStop);
            stop.Initial(strMessage);
            stop.BeginLoop();

            this.DetailForm.Update();
            this.DetailForm.MainForm.Update();
        }

        public void EndGcatLoop()
        {
            Stop stop = this.DetailForm.stop;
            stop.EndLoop();
            stop.OnStop -= new StopEventHandler(this.DoGcatStop);
            stop.Initial("");

            this.DetailForm.EnableControls(true);
            if (bMarcEditorFocued == true)
                this.DetailForm.MarcEditor.Focus();
        }

        // ���˳��������ֵ��ַ���
        public List<string> ContainHanzi(List<string> authors)
        {
            List<string> results = new List<string>();
            foreach (string strAuthor in authors)
            {
                if (ContainHanzi(strAuthor) == true)
                    results.Add(strAuthor);
            }

            return results;
        }

        public static bool ContainHanzi(string strAuthor)
        {
            strAuthor = strAuthor.Trim();
            if (string.IsNullOrEmpty(strAuthor) == true)
                return false;

            string strError = "";
            string strResult = "";
            int nRet = PrepareSjhmAuthorString(strAuthor,
            out strResult,
            out strError);
            if (string.IsNullOrEmpty(strResult) == true)
                return false;
            return true;
        }

        // �Լ���ȡ�ĽǺ���������ַ�������Ԥ�ӹ�������ȥ�����зǺ����ַ�
        public static int PrepareSjhmAuthorString(string strAuthor,
            out string strResult,
            out string strError)
        {
            strResult = "";
            strError = "";

            // string strSpecialChars = "���������������������������������ۣݡ����������������ܣ�������������";

            for (int i = 0; i < strAuthor.Length; i++)
            {
                char ch = strAuthor[i];

                if (StringUtil.IsHanzi(ch) == false)
                    continue;

                // �����Ƿ��������
                if (StringUtil.SpecialChars.IndexOf(ch) != -1)
                {
                    continue;
                }

                // ����
                strResult += ch;
            }

            return 0;
        }

        // ������ߺ� -- �ĽǺ���
        // return:
        //      -1  error
        //      0   canceled
        //      1   succeed
        public virtual int GetSjhmAuthorNumber(string strAuthor,
            out string strAuthorNumber,
            out string strError)
        {
            strError = "";
            strAuthorNumber = "";

            string strResult = "";
            int nRet = PrepareSjhmAuthorString(strAuthor,
            out strResult,
            out strError);
            if (nRet == -1)
                return -1;
            if (String.IsNullOrEmpty(strResult) == true)
            {
                strError = "�����ַ��� '" + strAuthor + "' ����û�а�����Ч�ĺ����ַ�";
                return -1;
            }

            List<string> sjhms = null;
            // ���ַ����еĺ���ת��Ϊ�ĽǺ���
            // parameters:
            //      bLocal  �Ƿ�ӱ��ػ�ȡ�ĽǺ���
            // return:
            //      -1  ����
            //      0   �û�ϣ���ж�
            //      1   ����
            nRet = this.DetailForm.MainForm.HanziTextToSjhm(
                true,
            strResult,
            out sjhms,
            out strError);
            if (nRet != 1)
                return nRet;

            if (strResult.Length != sjhms.Count)
            {
                strError = "�����ַ��� '" + strResult + "' ������ַ���(" + strResult.Length.ToString() + ")��ȡ�ĽǺ����Ľ��������� " + sjhms.Count.ToString() + " ����";
                return -1;
            }

            // 1����������Ϊһ���ߣ�ȡ���ֵ��ĽǺ��롣�磺Ф=9022
            if (strResult.Length == 1)
            {
                strAuthorNumber = sjhms[0].Substring(0, 4);
                return 1;
            }
            // 2����������Ϊ�����ߣ��ֱ�ȡ�����ֵ����ϽǺ����Ͻǡ��磺����=0287
            if (strResult.Length == 2)
            {
                strAuthorNumber = sjhms[0].Substring(0, 2) + sjhms[1].Substring(0, 2);
                return 1;
            }

            // 3����������Ϊ�����ߣ�����ȡ�������ϡ��������Ǻͺ����ֵ����Ͻǡ��磺�޹���=6075
            if (strResult.Length == 3)
            {
                strAuthorNumber = sjhms[0].Substring(0, 2)
                    + sjhms[1].Substring(0, 1)
                    + sjhms[2].Substring(0, 1);
                return 1;
            }

            // 4����������Ϊ�����ߣ�����ȡ���ֵ����Ͻǡ��磺����Ӣ��=5645
            // 5�����ּ����������ߣ�����ǰ����ȡ�ţ�����ͬ�ϡ��磺��˹�����˹��=2423
            if (strResult.Length >= 4)
            {
                strAuthorNumber = sjhms[0].Substring(0, 1)
                    + sjhms[1].Substring(0, 1)
                    + sjhms[2].Substring(0, 1)
                    + sjhms[3].Substring(0, 1);
                return 1;
            }

            strError = "error end";
            return -1;
        }

        // ���ݱ���ǰ�Ĵ�����
        public virtual void BeforeSaveRecord(object sender,
            BeforeSaveRecordEventArgs e)
        {
            if (sender == null)
                return;

            int nRet = 0;
            string strError = "";
            bool bChanged = false;

            // ��MARC��¼���д���
            if (sender is MarcEditor)
            {
                // ��Ŀ���κ�
                string strBatchNo = this.GetFirstSubfield("998", "a");
                if (string.IsNullOrEmpty(strBatchNo) == true)
                {
                    string strValue = "";
                    // ��鱾�� %catalog_batchno% ���Ƿ����
                    // ��marceditor_macrotable.xml�ļ��н�����
                    // return:
                    //      -1  error
                    //      0   not found
                    //      1   found
                    nRet = MacroUtil.GetFromLocalMacroTable(PathUtil.MergePath(this.DetailForm.MainForm.UserDir, "marceditor_macrotable.xml"),
            "catalog_batchno",
            false,
            out strValue,
            out strError);
                    if (nRet == -1)
                    {
                        e.ErrorInfo = strError;
                        return;
                    }
                    if (nRet == 1 && string.IsNullOrEmpty(strValue) == false)
                    {
                        this.SetFirstSubfield("998", "a", strValue);
                        bChanged = true;
                    }
                }

                // ��¼����ʱ��
                string strCreateTime = this.GetFirstSubfield("998", "u");
                if (string.IsNullOrEmpty(strCreateTime) == true)
                {
                    DateTime now = DateTime.Now;
                    strCreateTime = now.ToString("u");
                    this.SetFirstSubfield("998", "u", strCreateTime);
                    bChanged = true;
                }

                // ��¼������
                string strCreator = this.GetFirstSubfield("998", "z");
                if (string.IsNullOrEmpty(strCreator) == true)
                {
                    strCreator = this.DetailForm.CurrentUserName;
                    if (string.IsNullOrEmpty(strCreator) == true)
                        strCreator = e.CurrentUserName;
                    this.SetFirstSubfield("998", "z", strCreator);
                    bChanged = true;
                }

                e.Changed = bChanged;
            }
        }

        // 2011/8/9
        public string GetFirstSubfield(string strFieldName,
            string strSubfieldName,
            string strIndicatorMatch = "**")
        {
            return this.DetailForm.MarcEditor.Record.Fields.GetFirstSubfield(
                    strFieldName,
                    strSubfieldName,
                    strIndicatorMatch);
        }

        public void SetFirstSubfield(string strFieldName,
            string strSubfieldName,
            string strSubfieldValue)
        {
            this.DetailForm.MarcEditor.Record.Fields.SetFirstSubfield(
                    strFieldName,
                    strSubfieldName,
                    strSubfieldValue);
        }

        // 2011/8/10
        public List<string> GetSubfields(string strFieldName,
            string strSubfieldName,
            string strIndicatorMatch = "**")
        {
            return this.DetailForm.MarcEditor.Record.Fields.GetSubfields(
                    strFieldName,
                    strSubfieldName,
                    strIndicatorMatch);
        }
    }

    public class HostEventArgs : EventArgs
    {
        /*
        // �Ӻδ�����? MarcEditor EntityEditForm
        public object StartFrom = null;
         * */

        // �������ݵ��¼�����
        public GenerateDataEventArgs e = null;
    }

    public class PinyinCfgItem
    {
        public string FieldName = "";
        public string IndicatorMatchCase = "";
        public string From = "";
        public string To = "";

        public PinyinCfgItem(XmlNode nodeItem)
        {
            this.FieldName = DomUtil.GetAttr(nodeItem, "name");
            this.IndicatorMatchCase = DomUtil.GetAttr(nodeItem, "indicator");
            this.From = DomUtil.GetAttr(nodeItem, "from");
            this.To = DomUtil.GetAttr(nodeItem, "to");
        }
    }
}
