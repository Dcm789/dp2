using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using DigitalPlatform;
using DigitalPlatform.GUI;
using DigitalPlatform.Marc;
using DigitalPlatform.Xml;
using DigitalPlatform.Text;

using DigitalPlatform.CirculationClient;
using DigitalPlatform.LibraryClient.localhost;
using DigitalPlatform.LibraryClient;

namespace dp2Catalog
{
    public partial class dp2DupForm : Form, ISearchForm
    {
        bool m_bInSearch = false;

        // ��ǰȱʡ�ı��뷽ʽ
        Encoding CurrentEncoding = Encoding.UTF8;

        public LibraryChannelCollection Channels = null;
        LibraryChannel Channel = null;

        public string Lang = "zh";

        public MainForm MainForm = null;
        DigitalPlatform.Stop stop = null;

        // ����������к�����
        SortColumns SortColumns = new SortColumns();

        string m_strXmlRecord = "";

        /// <summary>
        /// ���������ź�
        /// </summary>
        public AutoResetEvent EventFinish = new AutoResetEvent(false);

        public bool AutoBeginSearch = false;

        const int WM_INITIAL = API.WM_USER + 201;

        const int ITEMTYPE_NORMAL = 0;  // ��ͨ����
        const int ITEMTYPE_OVERTHRESHOLD = 1; // Ȩֵ������ֵ������

        #region �ⲿ�ӿ�

        /// <summary>
        /// ���ط�����
        /// </summary>
        public string ProjectName
        {
            get
            {
                return this.comboBox_projectName.Text;
            }
            set
            {
                this.comboBox_projectName.Text = value;
            }
        }

        /// <summary>
        /// ������صļ�¼·����id����Ϊ?����Ҫ����ģ���keys
        /// </summary>
        public string RecordPath
        {
            get
            {
                return this.textBox_recordPath.Text;
            }
            set
            {
                this.textBox_recordPath.Text = value;
                this.Text = "����: " + value;
            }
        }

        /// <summary>
        /// ������ص�XML��¼
        /// </summary>
        public string XmlRecord
        {
            get
            {
                return m_strXmlRecord;
            }
            set
            {
                m_strXmlRecord = value;
            }
        }

        /// <summary>
        /// ��ò��ؽ���������е�Ȩֵ������ֵ�ļ�¼·���ļ���
        /// </summary>
        public string[] DupPaths
        {
            get
            {
                int i;
                List<string> aPath = new List<string>();
                for (i = 0; i < this.listView_browse.Items.Count; i++)
                {
                    ListViewItem item = this.listView_browse.Items[i];

                    if (item.ImageIndex == ITEMTYPE_OVERTHRESHOLD)
                    {
                        aPath.Add(item.Text);
                    }
                    else
                        break;  // �ٶ�������ֵ�������ǰ������������Ż��ж�
                }

                if (aPath.Count == 0)
                    return new string[0];

                string[] result = new string[aPath.Count];
                aPath.CopyTo(result);

                return result;
            }
        }

        #endregion

        #region ISearchForm �ӿں���

        // ���󡢴����Ƿ���Ч?
        public bool IsValid()
        {
            if (this.IsDisposed == true)
                return false;

            return true;
        }

        public string CurrentProtocol
        {
            get
            {
                return "dp2library";
            }
        }

        public string CurrentResultsetPath
        {
            get
            {
                return this.LibraryServerName
                    + "/" + this.RecordPath
                    + "/" + "searchdup"
                    + "/default";
            }
        }

        // ˢ��һ��MARC��¼
        // return:
        //      -2  ��֧��
        //      -1  error
        //      0   ��ش����Ѿ����٣�û�б�Ҫˢ��
        //      1   �Ѿ�ˢ��
        //      2   �ڽ������û���ҵ�Ҫˢ�µļ�¼
        public int RefreshOneRecord(
            string strPathParam,
            string strAction,
            out string strError)
        {
            strError = "��δʵ��";

            return -2;
        }


        // ɾ��һ��MARC/XML��¼
        // parameters:
        //      strSavePath ����Ϊ"����ͼ��/1@���ط�����"��û��Э�������֡�
        // return:
        //      -1  error
        //      0   suceed
        public int DeleteOneRecord(
            string strSavePath,
            byte[] baTimestamp,
            out byte[] baOutputTimestamp,
            out string strError)
        {
            baOutputTimestamp = null;
            strError = "��δʵ��";

            return -1;

        }

        public int SyncOneRecord(string strPath,
            ref long lVersion,
            ref string strSyntax,
            ref string strMARC,
            out string strError)
        {
            strError = "";
            return 0;
        }

        // ���һ��MARC/XML��¼
        // return:
        //      -1  error ����not found
        //      0   found
        //      1   Ϊ��ϼ�¼
        public int GetOneRecord(
            string strStyle,
            int nTest,
            string strPathParam,
            string strParameters,   // bool bHilightBrowseLine,
            out string strSavePath,
            out string strRecord,
            out string strXmlFragment,
            out string strOutStyle,
            out byte[] baTimestamp,
            out long lVersion,
            out DigitalPlatform.Z3950.Record record,
            out Encoding currrentEncoding,
            out LoginInfo logininfo,
            out string strError)
        {
            strXmlFragment = "";
            strRecord = "";
            record = null;
            strError = "";
            currrentEncoding = this.CurrentEncoding;
            baTimestamp = null;
            strSavePath = "";
            strOutStyle = "marc";
            logininfo = new LoginInfo();
            lVersion = 0;

            // ��ֹ����
            if (m_bInSearch == true)
            {
                strError = "��ǰ�������ڱ�һ��δ�����ĳ�����ʹ�ã��޷���ü�¼�����Ժ����ԡ�";
                return -1;
            }

            if (strStyle != "marc" && strStyle != "xml")
            {
                strError = "DupFormֻ֧�ֻ�ȡMARC��ʽ��¼��xml��ʽ��¼����֧�� '" + strStyle + "' ��ʽ�ļ�¼";
                return -1;
            }
            int nRet = 0;

            int index = -1;
            string strPath = "";
            string strDirection = "";
            nRet = Global.ParsePathParam(strPathParam,
                out index,
                out strPath,
                out strDirection,
                out strError);
            if (nRet == -1)
                return -1;

            if (index == -1)
            {
                strError = "��ʱ��֧��û�� index ���÷�";
                return -1;
            }

            bool bHilightBrowseLine = StringUtil.IsInList("hilight_browse_line", strParameters);

            if (index >= this.listView_browse.Items.Count)
            {
                strError = "Խ�������β��";
                return -1;
            }
            ListViewItem curItem = this.listView_browse.Items[index];

            if (bHilightBrowseLine == true)
            {
                // �޸�listview�������ѡ��״̬
                for (int i = 0; i < this.listView_browse.SelectedItems.Count; i++)
                {
                    this.listView_browse.SelectedItems[i].Selected = false;
                }

                curItem.Selected = true;
                curItem.EnsureVisible();
            }

            string strPurePath = curItem.Text;
            string strServerName = this.LibraryServerName;

            strPath = strPurePath + "@" + this.LibraryServerName;

            strSavePath = this.CurrentProtocol + ":" + strPath;

            // ����һ��dp2���������ð���
            dp2SearchForm dp2_searchform = this.GetDp2SearchForm();

            if (dp2_searchform == null)
            {
                strError = "û�д򿪵�dp2���������޷�GetOneRecordSyntax()";
                return -1;
            }

            // ���server url
            string strServerUrl = dp2_searchform.GetServerUrl(strServerName);
            if (strServerUrl == null)
            {
                strError = "û���ҵ��������� '" + strServerName + "' ��Ӧ��URL";
                return -1;
            }

            this.Channel = this.Channels.GetChannel(strServerUrl);

            stop.OnStop += new StopEventHandler(this.DoStop);
            stop.Initial("���ڳ�ʼ���������� ...");
            stop.BeginLoop();

            this.Update();
            this.MainForm.Update();

            try
            {
                stop.SetMessage("����װ����Ŀ��¼ " + strPath + " ...");

                string[] formats = null;
                formats = new string[1];
                formats[0] = "xml";

                string[] results = null;

                long lRet = Channel.GetBiblioInfos(
                    stop,
                    strPurePath,
                    "",
                    formats,
                    out results,
                    out baTimestamp,
                    out strError);
                if (lRet == 0)
                {
                    strError = "·��Ϊ '" + strPath + "' ����Ŀ��¼û���ҵ� ...";
                    goto ERROR1;   // not found
                }

                if (lRet == -1)
                    goto ERROR1;

                // this.BiblioTimestamp = baTimestamp;

                if (results == null)
                {
                    strError = "results == null";
                    goto ERROR1;
                }
                if (results.Length != formats.Length)
                {
                    strError = "result.Length != formats.Length";
                    goto ERROR1;
                }

                string strXml = results[0];

                if (strStyle == "marc")
                {
                    string strMarcSyntax = "";
                    string strOutMarcSyntax = "";
                    // �����ݼ�¼�л��MARC��ʽ
                    nRet = MarcUtil.Xml2Marc(strXml,
                        true,
                        strMarcSyntax,
                        out strOutMarcSyntax,
                        out strRecord,
                        out strError);
                    if (nRet == -1)
                    {
                        strError = "XMLת����MARC��¼ʱ����: " + strError;
                        goto ERROR1;
                    }


                    // �����Ŀ���������XMLƬ��
                    nRet = dp2SearchForm.GetXmlFragment(strXml,
            out strXmlFragment,
            out strError);
                    if (nRet == -1)
                        goto ERROR1;
                }
                else
                {
                    strRecord = strXml;
                    strOutStyle = strStyle;
                }

            }
            finally
            {
                stop.EndLoop();
                stop.OnStop -= new StopEventHandler(this.DoStop);
                stop.Initial("");
            }
            return 0;
        ERROR1:
            return -1;
        }

        #endregion

        public dp2DupForm()
        {
            InitializeComponent();
        }

        private void DupForm_Load(object sender, EventArgs e)
        {
            this.Channels = new LibraryChannelCollection();
            this.Channels.BeforeLogin += new BeforeLoginEventHandle(Channels_BeforeLogin);
            this.Channels.AfterLogin += new AfterLoginEventHandle(Channels_AfterLogin);

            stop = new DigitalPlatform.Stop();
            stop.Register(MainForm.stopManager, true);	// ����������

            this.checkBox_includeLowCols.Checked = this.MainForm.AppInfo.GetBoolean(
    "dup_form",
    "include_low_cols",
    true);
            this.checkBox_returnAllRecords.Checked = this.MainForm.AppInfo.GetBoolean(
    "dup_form",
    "return_all_records",
    true);
            if (String.IsNullOrEmpty(this.comboBox_projectName.Text) == true)
            {
                this.comboBox_projectName.Text = this.MainForm.AppInfo.GetString(
                        "dup_form",
                        "projectname",
                        "");
            }

            string strWidths = this.MainForm.AppInfo.GetString(
"dup_form",
"browse_list_column_width",
"");
            if (String.IsNullOrEmpty(strWidths) == false)
            {
                ListViewUtil.SetColumnHeaderWidth(this.listView_browse,
                    strWidths,
                    true);
            }

            // �Զ���������
            if (this.AutoBeginSearch == true)
            {
                API.PostMessage(this.Handle, WM_INITIAL, 0, 0);
            }
        }

        void Channels_AfterLogin(object sender, AfterLoginEventArgs e)
        {
            LibraryChannel channel = (LibraryChannel)sender;

            dp2Server server = this.MainForm.Servers[channel.Url];
            if (server == null)
            {
                // e.ErrorInfo = "û���ҵ� URL Ϊ " + channel.Url + " �ķ���������";
                return;
            }

#if SN
            if (server.Verified == false && StringUtil.IsInList("serverlicensed", channel.Rights) == false)
            {
                string strError = "";
                string strTitle = "���ش���Ҫ���������кŲ��ܷ��ʷ����� " + server.Name + " " + server.Url;
                int nRet = this.MainForm.VerifySerialCode(strTitle,
                    "", 
                    true,
                    out strError);
                if (nRet == -1)
                {
                    channel.Close();
                    e.ErrorInfo = strTitle;
#if NO
                    MessageBox.Show(this.MainForm, "���ش���Ҫ���������кŲ���ʹ��");
                    API.PostMessage(this.Handle, API.WM_CLOSE, 0, 0);
#endif
                    return;
                }
            }
#endif

            server.Verified = true;
        }

        void Channels_BeforeLogin(object sender, BeforeLoginEventArgs e)
        {
            LibraryChannel channel = (LibraryChannel)sender;

            dp2Server server = this.MainForm.Servers[channel.Url];
            if (server == null)
            {
                e.ErrorInfo = "û���ҵ� URL Ϊ " + channel.Url + " �ķ���������";
                e.Failed = true;
                e.Cancel = true;
                return;
            }

            if (e.FirstTry == true)
            {
                e.UserName = server.DefaultUserName;
                e.Password = server.DefaultPassword;
                e.Parameters = "location=dp2Catalog,type=worker";
                /*
                e.IsReader = false;
                e.Location = "dp2Catalog";
                 * */
                // 2014/9/13
                e.Parameters += ",mac=" + StringUtil.MakePathList(SerialCodeForm.GetMacAddress(), "|");

#if SN
                // �����к��л�� expire= ����ֵ
                string strExpire = this.MainForm.GetExpireParam();
                if (string.IsNullOrEmpty(strExpire) == false)
                    e.Parameters += ",expire=" + strExpire;
#endif

                e.Parameters += ",client=dp2catalog|" + Program.ClientVersion;

                if (String.IsNullOrEmpty(e.UserName) == false)
                    return; // ��������, �Ա�����һ�� ������ �Ի�����Զ���¼
            }

            // 
            IWin32Window owner = this;

            ServerDlg dlg = SetDefaultAccount(
                e.LibraryServerUrl,
                null,
                e.ErrorInfo,
                owner);
            if (dlg == null)
            {
                e.Cancel = true;
                return;
            }


            e.UserName = dlg.UserName;
            e.Password = dlg.Password;
            e.SavePasswordShort = false;
            e.Parameters = "location=dp2Catalog,type=worker";

            e.Parameters += ",client=dp2catalog|" + Program.ClientVersion;

            /*
            e.IsReader = false;
            e.Location = "dp2Catalog";
             * */
            e.SavePasswordLong = true;
            e.LibraryServerUrl = dlg.ServerUrl;
        }

        ServerDlg SetDefaultAccount(
    string strServerUrl,
    string strTitle,
    string strComment,
    IWin32Window owner)
        {
            dp2Server server = this.MainForm.Servers[strServerUrl];

            ServerDlg dlg = new ServerDlg();
            GuiUtil.SetControlFont(dlg, this.Font);

            if (String.IsNullOrEmpty(strServerUrl) == true)
            {
            }
            else
            {
                dlg.ServerUrl = strServerUrl;
            }

            if (owner == null)
                owner = this;


            if (String.IsNullOrEmpty(strTitle) == false)
                dlg.Text = strTitle;

            dlg.Comment = strComment;
            dlg.UserName = server.DefaultUserName;

            this.MainForm.AppInfo.LinkFormState(dlg,
                "dp2_logindlg_state");

            dlg.ShowDialog(owner);

            this.MainForm.AppInfo.UnlinkFormState(dlg);


            if (dlg.DialogResult == DialogResult.Cancel)
            {
                return null;
            }

            server.DefaultUserName = dlg.UserName;
            server.DefaultPassword =
                (dlg.SavePassword == true) ?
                dlg.Password : "";

            server.SavePassword = dlg.SavePassword;

            server.Url = dlg.ServerUrl;
            return dlg;
        }

        void DoStop(object sender, StopEventArgs e)
        {
            if (this.Channel != null)
                this.Channel.Abort();
        }

        private void dp2DupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (stop != null)
            {
                if (stop.State == 0)    // 0 ��ʾ���ڴ���
                {
                    MessageBox.Show(this, "���ڹرմ���ǰֹͣ���ڽ��еĳ�ʱ������");
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void DupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (stop != null) // �������
            {
                stop.Unregister();	// ����������
                stop = null;
            }

            if (this.MainForm != null && this.MainForm.AppInfo != null)
            {
                this.MainForm.AppInfo.SetBoolean(
    "dup_form",
    "include_low_cols",
    this.checkBox_includeLowCols.Checked);
                this.MainForm.AppInfo.SetBoolean(
        "dup_form",
        "return_all_records",
        this.checkBox_returnAllRecords.Checked);

                this.MainForm.AppInfo.SetString(
                    "dup_form",
                    "projectname",
                    this.comboBox_projectName.Text);

                string strWidths = ListViewUtil.GetColumnWidthListString(this.listView_browse);
                this.MainForm.AppInfo.SetString(
                    "dup_form",
                    "browse_list_column_width",
                    strWidths);
            }

            this.Channels.BeforeLogin -= new BeforeLoginEventHandle(Channels_BeforeLogin);

            EventFinish.Set();
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_INITIAL:
                    {
                        this.DoSearchDup();
                    }
                    return;
            }
            base.DefWndProc(ref m);
        }

        public void DoSearchDup()
        {
            string strError = "";
            string strUsedProjectName = "";

            this.EventFinish.Reset();

            this.m_bInSearch = true;
            try
            {

                int nRet = DoSearch(this.comboBox_projectName.Text,
                    this.textBox_recordPath.Text,
                    this.XmlRecord,
                    out strUsedProjectName,
                    out strError);
                if (nRet == -1)
                {
                    MessageBox.Show(this, strError);
                }

                if (String.IsNullOrEmpty(strUsedProjectName) == false)
                    this.ProjectName = strUsedProjectName;
            }
            finally
            {
                this.EventFinish.Set();

                this.m_bInSearch = false;
            }
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            DoSearchDup();
        }

        void EnableControls(bool bEnable)
        {
            this.button_search.Enabled = bEnable;
            this.button_findServerName.Enabled = bEnable;

            this.button_viewXmlRecord.Enabled = bEnable;

            this.comboBox_projectName.Enabled = bEnable;
            this.textBox_recordPath.Enabled = bEnable;

            this.textBox_serverName.Enabled = bEnable;
        }

        // ͼ��ݷ�������
        public string LibraryServerName
        {
            get
            {
                return this.textBox_serverName.Text;
            }
            set
            {
                this.textBox_serverName.Text = value;
            }
        }

        // ����
        // return:
        //      -1  error
        //      0   succeed
        public int DoSearch(string strProjectName,
            string strRecPath,
            string strXml,
            out string strUsedProjectName,
            out string strError)
        {
            strError = "";
            strUsedProjectName = "";

            if (strProjectName == "<Ĭ��>"
                || strProjectName == "<default>")
                strProjectName = "";

            EventFinish.Reset();

            EnableControls(false);

            stop.OnStop += new StopEventHandler(this.DoStop);
            stop.Initial("���ڽ��в��� ...");
            stop.BeginLoop();

            this.Update();
            this.MainForm.Update();

            try
            {
                this.ClearDupState(true);
                this.listView_browse.Items.Clear();

                // ���server url
                if (String.IsNullOrEmpty(this.LibraryServerName) == true)
                {
                    strError = "��δָ����������";
                    goto ERROR1;
                }
                dp2Server server = this.MainForm.Servers.GetServerByName(this.LibraryServerName);
                if (server == null)
                {
                    strError = "��������Ϊ '" + this.LibraryServerName + "' �ķ�����������...";
                    goto ERROR1;
                }

                this.SortColumns.Clear();
                SortColumns.ClearColumnSortDisplay(this.listView_browse.Columns);

                string strBrowseStyle = "cols";
                if (this.checkBox_includeLowCols.Checked == false)
                    strBrowseStyle += ",excludecolsoflowthreshold";

                string strServerUrl = server.Url;

                this.Channel = this.Channels.GetChannel(strServerUrl);

                long lRet = Channel.SearchDup(
                    stop,
                    strRecPath,
                    strXml,
                    strProjectName,
                    "includeoriginrecord", // includeoriginrecord
                    out strUsedProjectName,
                    out strError);
                if (lRet == -1)
                    goto ERROR1;

                long lHitCount = lRet;

                if (lHitCount == 0)
                    goto END1;   // ���ط���û������


                long lStart = 0;
                long lPerCount = Math.Min(50, lHitCount);
                // װ�������ʽ
                for (; ; )
                {
                    Application.DoEvents();	// ���ý������Ȩ

                    if (stop != null)
                    {
                        if (stop.State != 0)
                        {
                            strError = "�û��ж�";
                            goto ERROR1;
                        }
                    }

                    stop.SetMessage("����װ�������Ϣ " + (lStart + 1).ToString() + " - " + (lStart + lPerCount).ToString() + " (���� " + lHitCount.ToString() + " ����¼) ...");

                    DupSearchResult[] searchresults = null;

                    lRet = Channel.GetDupSearchResult(
                        stop,
                        lStart,
                        lPerCount,
                        strBrowseStyle, // "cols,excludecolsoflowthreshold",
                        out searchresults,
                        out strError);
                    if (lRet == -1)
                        goto ERROR1;

                    if (lRet == 0)
                        break;

                    Debug.Assert(searchresults != null, "");

                    // ����������
                    for (int i = 0; i < searchresults.Length; i++)
                    {
                        DupSearchResult result = searchresults[i];

                        ListViewUtil.EnsureColumns(this.listView_browse,
    2 + (result.Cols == null ? 0 : result.Cols.Length),
    200);

                        if (this.checkBox_returnAllRecords.Checked == false)
                        {
                            // ������һ��Ȩֵ�ϵ͵ģ����ж�ȫ����ȡ�������
                            if (result.Weight < result.Threshold)
                                goto END1;
                        }

                        /*
                        if (result.Cols == null)
                        {
                            strError = "���صĽ���д��� result.Cols == null";
                            goto ERROR1;
                        }

                        ListViewUtil.EnsureColumns(this.listView_browse,
                            2 + result.Cols.Length,
                            200);
                         * */

                        ListViewItem item = new ListViewItem();
                        item.Text = result.Path;
                        item.SubItems.Add(result.Weight.ToString());
                        if (result.Cols != null)
                        {
                            for (int j = 0; j < result.Cols.Length; j++)
                            {
                                item.SubItems.Add(result.Cols[j]);
                            }
                        }
                        this.listView_browse.Items.Add(item);

                        if (item.Text == this.RecordPath)
                        {
                            // ������Ƿ����¼�Լ�  2008/2/29
                            item.ImageIndex = ITEMTYPE_OVERTHRESHOLD;
                            item.BackColor = Color.LightGoldenrodYellow;
                            item.ForeColor = SystemColors.GrayText; // ��ʾ���Ƿ����¼�Լ�
                        }
                        else if (result.Weight >= result.Threshold)
                        {
                            item.ImageIndex = ITEMTYPE_OVERTHRESHOLD;
                            item.BackColor = Color.LightYellow;
                            item.Font = new Font(item.Font, FontStyle.Bold);
                        }
                        else
                        {
                            item.ImageIndex = ITEMTYPE_NORMAL;
                        }

                    }

                    lStart += searchresults.Length;
                    if (lStart >= lHitCount || lPerCount <= 0)
                        break;

                }

            END1:
                this.SetDupState();

                return (int)lHitCount;
            }
            finally
            {
                stop.EndLoop();
                stop.OnStop -= new StopEventHandler(this.DoStop);
                stop.Initial("");

                EventFinish.Set();

                EnableControls(true);
            }


        ERROR1:
            return -1;
        }

        private void comboBox_projectName_DropDown(object sender, EventArgs e)
        {
            if (this.comboBox_projectName.Items.Count > 0)
                return;

            string strError = "";
            int nRet = 0;

            string[] projectnames = null;
            // �г����õĲ��ط�����
            nRet = ListProjectNames(this.RecordPath,
                out projectnames,
                out strError);
            if (nRet == -1)
                goto ERROR1;

            for (int i = 0; i < projectnames.Length; i++)
            {
                this.comboBox_projectName.Items.Add(projectnames[i]);
            }

            return;
        ERROR1:
            MessageBox.Show(this, strError);
        }

        // �г����õĲ��ط�����
        public int ListProjectNames(string strPureRecPath,
            out string[] projectnames,
            out string strError)
        {
            strError = "";
            projectnames = null;

            EnableControls(false);

            stop.OnStop += new StopEventHandler(this.DoStop);
            stop.Initial("���ڻ�ȡ���õĲ��ط����� ...");
            stop.BeginLoop();

            try
            {

                // ���server url
                if (String.IsNullOrEmpty(this.LibraryServerName) == true)
                {
                    strError = "��δָ����������";
                    goto ERROR1;
                }
                dp2Server server = this.MainForm.Servers.GetServerByName(this.LibraryServerName);
                if (server == null)
                {
                    strError = "��������Ϊ '" + this.LibraryServerName + "' �ķ�����������...";
                    goto ERROR1;
                }

                string strServerUrl = server.Url;

                this.Channel = this.Channels.GetChannel(strServerUrl);



                DupProjectInfo[] dpis = null;

                string strBiblioDbName = dp2SearchForm.GetDbName(strPureRecPath);

                long lRet = Channel.ListDupProjectInfos(
                    stop,
                    strBiblioDbName,
                    out dpis,
                    out strError);
                if (lRet == -1)
                    goto ERROR1;

                projectnames = new string[dpis.Length];
                for (int i = 0; i < projectnames.Length; i++)
                {
                    projectnames[i] = dpis[i].Name;
                }

                return (int)lRet;
            }
            finally
            {
                stop.EndLoop();
                stop.OnStop -= new StopEventHandler(this.DoStop);
                stop.Initial("");

                EnableControls(true);
            }


        ERROR1:
            return -1;
        }

        private void textBox_recordPath_TextChanged(object sender, EventArgs e)
        {
            // ��¼·����Ӱ�쵽�������б�
            // �޸ļ�¼·����ʱ����ʹ�����������б���գ��������õ������б��ʱ����Զ�ȥ��ȡ������
            this.comboBox_projectName.Items.Clear();

        }

        /// <summary>
        /// �ȴ���������
        /// </summary>
        public void WaitSearchFinish()
        {
            for (; ; )
            {
                Application.DoEvents();
                bool bRet = this.EventFinish.WaitOne(10, true);
                if (bRet == true)
                    break;
            }
        }

        private void button_viewXmlRecord_Click(object sender, EventArgs e)
        {
            XmlViewerForm dlg = new XmlViewerForm();
            GuiUtil.SetControlFont(dlg, this.Font);

            dlg.Text = "��ǰXML����";
            dlg.MainForm = this.MainForm;
            dlg.XmlString = this.XmlRecord;
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.ShowDialog();
            return;
        }

        void ClearDupState(bool bSearching)
        {
            if (bSearching == true)
                this.label_dupMessage.Text = "���ڲ���...";
            else
                this.label_dupMessage.Text = "��δ����";
        }

        // ���ò���״̬
        void SetDupState()
        {
            int nCount = 0;
            for (int i = 0; i < this.listView_browse.Items.Count; i++)
            {
                ListViewItem item = this.listView_browse.Items[i];

                if (item.Text == this.RecordPath)
                    continue;   // �����������¼�Լ� 2008/2/29


                if (item.ImageIndex == ITEMTYPE_OVERTHRESHOLD)
                    nCount++;
                else
                    break;  // �ٶ�����Ȩֵ�������ǰ����һ������һ�����ǵ����ѭ���ͽ���
            }

            if (nCount > 0)
                this.label_dupMessage.Text = "�� " + Convert.ToString(nCount) + " ���ظ���¼��";
            else
                this.label_dupMessage.Text = "û���ظ���¼��";

        }

        private void listView_browse_DoubleClick(object sender, EventArgs e)
        {
            /*
            string strPurePath = this.listView_browse.SelectedItems[0].SubItems[0].Text;

            EntityForm form = new EntityForm();

            form.MdiParent = this.MainForm;

            form.MainForm = this.MainForm;
            form.Show();
            form.LoadRecord(strPath);
             * */
            int nIndex = -1;
            if (this.listView_browse.SelectedIndices.Count > 0)
                nIndex = this.listView_browse.SelectedIndices[0];
            else
            {
                if (this.listView_browse.FocusedItem == null)
                {
                    MessageBox.Show(this, "��δѡ��Ҫװ����ϸ��������");
                    return;
                }
                nIndex = this.listView_browse.Items.IndexOf(this.listView_browse.FocusedItem);
            }

            LoadDetail(nIndex);
        }

        void LoadDetail(int index)
        {
            // ȡ����¼·����������Ŀ������Ȼ�������Ŀ���syntax
            // ����װ��MARC��DC���ֲ�ͬ�Ĵ���
            string strError = "";

            // ��ֹ����
            if (m_bInSearch == true)
            {
                strError = "��ǰ�������ڱ�һ��δ�����ĳ�����ʹ�ã��޷�װ�ؼ�¼�����Ժ����ԡ�";
                goto ERROR1;
            }

            string strSyntax = "";
            int nRet = GetOneRecordSyntax(index,
                out strSyntax,
                out strError);
            if (nRet == -1)
                goto ERROR1;

            if (strSyntax == "" // default = unimarc
                || strSyntax.ToLower() == "unimarc"
                || strSyntax.ToLower() == "usmarc")
            {

                MarcDetailForm form = new MarcDetailForm();

                form.MdiParent = this.MainForm;
                form.MainForm = this.MainForm;

                // MARC Syntax OID
                // ��Ҫ�������ݿ����ò��������еõ�MARC��ʽ
                //// form.AutoDetectedMarcSyntaxOID = "1.2.840.10003.5.1";   // UNIMARC

                form.Show();

                form.LoadRecord(this, index);
            }
            else if (strSyntax.ToLower() == "dc")
            {

                DcForm form = new DcForm();

                form.MdiParent = this.MainForm;
                form.MainForm = this.MainForm;

                form.Show();

                form.LoadRecord(this, index);
            }
            else
            {
                strError = "δ֪��syntax '" + strSyntax + "'";
                goto ERROR1;
            }

            return;
        ERROR1:
            MessageBox.Show(this, strError);
        }

        dp2SearchForm GetDp2SearchForm()
        {
            dp2SearchForm dp2_searchform = this.MainForm.TopDp2SearchForm;

            if (dp2_searchform == null)
            {
                // �¿�һ��dp2������
                dp2_searchform = new dp2SearchForm();
                dp2_searchform.MainForm = this.MainForm;
                dp2_searchform.MdiParent = this.MainForm;
                dp2_searchform.WindowState = FormWindowState.Minimized;
                dp2_searchform.Show();
            }


            return dp2_searchform;
        }

        // return:
        //      -1  error
        //      0   not found
        //      1   found
        int GetOneRecordSyntax(int index,
            out string strSyntax,
            out string strError)
        {
            strError = "";
            strSyntax = "";

            if (index >= this.listView_browse.Items.Count)
            {
                strError = "Խ�������β��";
                return -1;
            }

            ListViewItem curItem = this.listView_browse.Items[index];

            string strServerName = this.LibraryServerName;
            string strPurePath = curItem.Text;

            // ����һ��dp2���������ð���
            dp2SearchForm dp2_searchform = this.GetDp2SearchForm();

            if (dp2_searchform == null)
            {
                strError = "û�д򿪵�dp2���������޷�GetOneRecordSyntax()";
                return -1;
            }

            /*
            // ���server url
            dp2Server server = this.dp2ResTree1.Servers.GetServerByName(strServerName);
            string strServerUrl = server.Url;
             * */
            string strBiblioDbName = dp2SearchForm.GetDbName(strPurePath);

            // ���һ�����ݿ������syntax
            // parameters:
            //      stop    ���!=null����ʾʹ�����stop�����Ѿ�OnStop +=
            //              ���==null����ʾ���Զ�ʹ��this.stop�����Զ�OnStop+=
            // return:
            //      -1  error
            //      0   not found
            //      1   found
            int nRet = dp2_searchform.GetDbSyntax(
                null,
                strServerName,
                strBiblioDbName,
                out strSyntax,
                out strError);

            if (nRet == -1)
                return -1;

            return nRet;
        }

        private void listView_browse_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int nClickColumn = e.Column;

            ColumnSortStyle sortStyle = ColumnSortStyle.LeftAlign;

            // ��һ��Ϊ��¼·��������������
            if (nClickColumn == 0)
                sortStyle = ColumnSortStyle.RecPath;

            this.SortColumns.SetFirstColumn(nClickColumn,
                sortStyle,
                this.listView_browse.Columns,
                true);

            // ����
            this.listView_browse.ListViewItemSorter = new SortColumnsComparer(this.SortColumns);

            this.listView_browse.ListViewItemSorter = null;
        }

        private void button_findServerName_Click(object sender, EventArgs e)
        {
            GetDp2ResDlg dlg = new GetDp2ResDlg();
            GuiUtil.SetControlFont(dlg, this.Font);

            dlg.Text = "��ָ��һ����Ϊ����Ŀ��� dp2library ������";
#if OLD_CHANNEL
            dlg.dp2Channels = this.Channels;
#endif
            dlg.ChannelManager = Program.MainForm;

            dlg.Servers = this.MainForm.Servers;
            dlg.EnabledIndices = new int[] { dp2ResTree.RESTYPE_SERVER };
            dlg.Path = this.textBox_serverName.Text;

            dlg.ShowDialog(this);

            if (dlg.DialogResult != DialogResult.OK)
                return;

            this.textBox_serverName.Text = dlg.Path;
        }

        private void DupForm_Activated(object sender, EventArgs e)
        {
            if (stop != null)
                MainForm.stopManager.Active(this.stop);

            MainForm.SetMenuItemState();

            /*
            // �˵�
            MainForm.MenuItem_saveOriginRecordToIso2709.Enabled = true;
            MainForm.MenuItem_font.Enabled = true;

            // ��������ť
            MainForm.toolButton_search.Enabled = false;
            MainForm.toolButton_prev.Enabled = true;
            MainForm.toolButton_next.Enabled = true;
            MainForm.toolButton_nextBatch.Enabled = false;

            MainForm.toolButton_getAllRecords.Enabled = false;
            MainForm.toolButton_saveTo.Enabled = true;
            MainForm.toolButton_save.Enabled = true;
            MainForm.toolButton_delete.Enabled = true;

            MainForm.toolButton_loadTemplate.Enabled = true;
             * */

            MainForm.toolButton_dup.Enabled = false;
            MainForm.toolButton_verify.Enabled = false;
        }

        private void listView_browse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView_browse.SelectedItems.Count == 1)
            {
                ListViewItem item = this.listView_browse.SelectedItems[0];
                int nLineNo = this.listView_browse.SelectedIndices[0] + 1;
                if (item.ImageIndex == ITEMTYPE_OVERTHRESHOLD)
                {
                    if (item.Text == this.RecordPath)
                    {
                        this.label_message.Text = "��� " + nLineNo.ToString() + ": ������صļ�¼(�Լ�)";
                    }
                    else
                    {
                        this.label_message.Text = "��� " + nLineNo.ToString() + ": �ظ��ļ�¼";
                    }
                }
                else
                {
                    this.label_message.Text = "��� " + nLineNo.ToString();
                }
            }
            else
            {
                this.label_message.Text = "";
            }

            // װ��(δװ���)�����
            if (this.listView_browse.SelectedItems.Count > 0)
            {
                List<string> pathlist = new List<string>();
                List<ListViewItem> itemlist = new List<ListViewItem>();
                for (int i = 0; i < this.listView_browse.SelectedItems.Count; i++)
                {
                    ListViewItem item = this.listView_browse.SelectedItems[i];
                    string strFirstCol = ListViewUtil.GetItemText(item, 2);
                    if (string.IsNullOrEmpty(strFirstCol) == false)
                        continue;
                    pathlist.Add(item.Text);
                    itemlist.Add(item);
                }

                if (pathlist.Count > 0)
                {
                    string strError = "";
                    int nRet = GetBrowseCols(pathlist,
                        itemlist,
                        out strError);
                    if (nRet == -1)
                        MessageBox.Show(this, strError);
                }
            }

        }

        private void listView_browse_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            menuItem = new MenuItem("���ȫ�������(&F)");
            menuItem.Click += new System.EventHandler(this.menu_fillBrowseCols_Click);
            /*
            if (this.listView_browse.SelectedItems.Count == 0)
                menuItem.Enabled = false;
             * */
            contextMenu.MenuItems.Add(menuItem);

            contextMenu.Show(this.listView_browse, new Point(e.X, e.Y));
        }

        void menu_fillBrowseCols_Click(object sender, EventArgs e)
        {
            List<string> pathlist = new List<string>();
            List<ListViewItem> itemlist = new List<ListViewItem>();
            for (int i = 0; i < this.listView_browse.Items.Count; i++)
            {
                ListViewItem item = this.listView_browse.Items[i];
                string strFirstCol = ListViewUtil.GetItemText(item, 2);
                if (string.IsNullOrEmpty(strFirstCol) == false)
                    continue;
                pathlist.Add(item.Text);
                itemlist.Add(item);
            }

            if (pathlist.Count > 0)
            {
                string strError = "";
                int nRet = GetBrowseCols(pathlist,
                    itemlist,
                    out strError);
                if (nRet == -1)
                    MessageBox.Show(this, strError);
            }
        }

        int GetBrowseCols(List<string> pathlist,
            List<ListViewItem> itemlist,
            out string strError)
        {
            strError = "";

            // ���server url
            if (String.IsNullOrEmpty(this.LibraryServerName) == true)
            {
                strError = "��δָ����������";
                return -1;
            }
            dp2Server server = this.MainForm.Servers.GetServerByName(this.LibraryServerName);
            if (server == null)
            {
                strError = "��������Ϊ '" + this.LibraryServerName + "' �ķ�����������...";
                return -1;
            }

            string strServerUrl = server.Url;

            this.Channel = this.Channels.GetChannel(strServerUrl);

            EnableControls(false);

            stop.OnStop += new StopEventHandler(this.DoStop);
            stop.Initial("������������ ...");
            stop.BeginLoop();

            this.Update();
            this.MainForm.Update();

            try
            {
                int nStart = 0;
                int nCount = 0;
                for (; ; )
                {
                    nCount = pathlist.Count - nStart;
                    if (nCount > 100)
                        nCount = 100;
                    if (nCount <= 0)
                        break;

                    Application.DoEvents();	// ���ý������Ȩ

                    if (stop != null)
                    {
                        if (stop.State != 0)
                        {
                            strError = "�û��ж�";
                            return -1;
                        }
                    }

                    stop.SetMessage("����װ�������Ϣ " + (nStart + 1).ToString() + " - " + (nStart + nCount).ToString());

                    string[] paths = new string[nCount];
                    pathlist.CopyTo(nStart, paths, 0, nCount);

                    DigitalPlatform.LibraryClient.localhost.Record[] searchresults = null;

                    long lRet = this.Channel.GetBrowseRecords(
                        this.stop,
                        paths,
                        "id,cols",
                        out searchresults,
                        out strError);
                    if (lRet == -1)
                        return -1;

                    if (searchresults == null || searchresults.Length == 0)
                    {
                        strError = "searchresults == null || searchresults.Length == 0";
                        return -1;
                    }

                    for (int i = 0; i < searchresults.Length; i++)
                    {
                        DigitalPlatform.LibraryClient.localhost.Record record = searchresults[i];

                        ListViewUtil.EnsureColumns(this.listView_browse,
                            2 + (record.Cols == null ? 0 : record.Cols.Length),
                            200);

                        ListViewItem item = itemlist[nStart + i];
                        item.Text = record.Path;
                        if (record.Cols != null)
                        {
                            for (int j = 0; j < record.Cols.Length; j++)
                            {
                                item.SubItems.Add(record.Cols[j]);
                            }
                        }
                    }


                    nStart += searchresults.Length;
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


    }
}