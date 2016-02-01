using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;

using DigitalPlatform.Text;
using DigitalPlatform.Xml;

// using DigitalPlatform.LibraryClient.localhost;
using DigitalPlatform.GUI;
using DigitalPlatform.LibraryClient;
using DigitalPlatform.LibraryClient.localhost;

namespace DigitalPlatform.CirculationClient
{
    /// <summary>
    /// dp2��Դ���ؼ�
    /// </summary>
    public partial class dp2ResTree : System.Windows.Forms.TreeView
    {
        public CfgCache cfgCache = null;

        public bool SortTableChanged = false;
        public Hashtable sort_tables = new Hashtable();

        DigitalPlatform.Stop m_current_stop = null;

        public string Lang = "zh";

        public dp2ServerCollection Servers = null;	// ����
        public LibraryChannelCollection Channels = null;    // ����

        public bool TestMode = false;   // �Ƿ�Ϊ����ģʽ

        public DigitalPlatform.StopManager stopManager = null;

        /// <summary>
        /// ͨѶͨ������������Ķ���
        /// </summary>
        LibraryChannel channel = null;

        public int[] EnabledIndices = null;	// null��ʾȫ�����ڡ����������ڣ�����Ԫ�ظ���Ϊ0����ʾȫ������

        #region	��Դ���͡�����Icon�±���

        public const int RESTYPE_SERVER = 2;
        public const int RESTYPE_DB = 0;
        public const int RESTYPE_FROM = 1;
        public const int RESTYPE_LOADING = 3;
        public const int RESTYPE_FOLDER = 4;
        public const int RESTYPE_FILE = 5;

        #endregion

        public dp2ResTree()
        {
            InitializeComponent();
        }

        // 2011/11/23
        // ��ǰ�û���
        // ���һ��ʹ�ù�Channel�ĵ�ǰ�û���
        public string CurrentUserName
        {
            get
            {
                if (this.channel == null)
                    return "";
                return this.channel.UserName;
            }
        }

        // ˢ�·��
        [Flags]
        public enum RefreshStyle
        {
            All = 0xffff,
            Servers = 0x01,
            Selected = 0x02,
        }

        void Rearrange(List<string> order_table,
            ref List<NormalDbProperty> properties)
        {
            List<NormalDbProperty> result = new List<NormalDbProperty>();

            foreach (string strName in order_table)
            {
                foreach (NormalDbProperty property in properties)
                {
                    if (property.DbName == strName)
                    {
                        result.Add(property);
                        properties.Remove(property);
                        break;
                    }
                }
            }

            // ʣ�µ�
            result.AddRange(properties);
            properties = result;
        }

        void Rearrange(List<string> order_table,
        ref List<string> froms,
        ref List<String> styles)
        {
            List<string> result_froms = new List<string>();
            List<string> result_styles = new List<string>();

            foreach (string strName in order_table)
            {
                foreach (string from in froms)
                {
                    if (strName == from)
                    {
                        int pos = froms.IndexOf(from);

                        result_froms.Add(from);
                        froms.RemoveAt(pos);

                        result_styles.Add(styles[pos]);
                        styles.RemoveAt(pos);
                        break;
                    }
                }
            }

            // ʣ�µ�
            result_froms.AddRange(froms);
            result_styles.AddRange(styles);

            froms = result_froms;
            styles = result_styles;
        }

        public int Fill(TreeNode node)
        {
            string strError = "";
            try
            {
                int nRet = Fill(node, out strError);
                if (nRet == -1)
                {
                    try
                    {
                        MessageBox.Show(this,
                            strError);
                    }
                    catch
                    {
                        // this�����Ѿ�������
                    }
                    return -1;
                }
                return nRet;
            }
            catch (ObjectDisposedException)
            {
                return 0;
            }
        }

        // �ݹ�
        public int Fill(TreeNode node,
            out string strError)
        {
            strError = "";
            int nRet = 0;

            if (this.Servers == null)
            {
                strError = "this.Servers == null";
                return -1;
            }

            this.Enabled = false;
            this.Update();
            try
            {
                TreeNodeCollection children = null;

                if (node == null)
                {
                    children = this.Nodes;
                }
                else
                {
                    children = node.Nodes;
                }

                // ����
                if (node == null)
                {
                    children.Clear();

                    for (int i = 0; i < Servers.Count; i++)
                    {
                        Application.DoEvents();	// ���ý������Ȩ

                        dp2Server server = (dp2Server)Servers[i];
                        TreeNode nodeNew = new TreeNode(server.Name,
                            RESTYPE_SERVER,
                            RESTYPE_SERVER);
                        SetLoading(nodeNew);

                        if (EnabledIndices != null
                            && StringUtil.IsInList(nodeNew.ImageIndex, EnabledIndices) == false)
                            nodeNew.ForeColor = ControlPaint.LightLight(nodeNew.ForeColor);

                        children.Add(nodeNew);

                        dp2ServerNodeInfo info = new dp2ServerNodeInfo();
                        info.Name = server.Name;
                        info.Url = server.Url;

                        nodeNew.Tag = info;
                    }

                    return 0;
                }

                // *** �����µĽڵ�����

                // ������
                if (node.ImageIndex == RESTYPE_SERVER)
                {
                    Application.DoEvents();	// ���ý������Ȩ

                    ResPath respath = new ResPath(node);

                    List<NormalDbProperty> properties = null;
                    // string[] dbnames = null;
                    nRet = GetDbNames(
                        respath.Url,
                        // out dbnames,
                        out properties,
                        out strError);
                    if (nRet == -1)
                    {
                        if (node != null)
                        {
                            SetLoading(node);	// ������ƺ������³���+��
                            node.Collapse();
                        }
                        goto ERROR1;
                    }

                    children.Clear();

                    List<string> order_table = (List<string>)this.sort_tables[respath.FullPath];

                    if (order_table != null)
                    {
                        Rearrange(order_table,
        ref properties);
                    }

                    for (int i = 0; i < properties.Count; i++)
                    {
                        NormalDbProperty prop = properties[i];

                        string strDbName = prop.DbName;
                        TreeNode nodeNew = new TreeNode(strDbName,
                            RESTYPE_DB, RESTYPE_DB);

                        // nodeNew.Tag = items[i].TypeString;  // �����ַ���

                        SetLoading(nodeNew);

                        if (EnabledIndices != null
                            && StringUtil.IsInList(nodeNew.ImageIndex, EnabledIndices) == false)
                            nodeNew.ForeColor = ControlPaint.LightLight(nodeNew.ForeColor);

                        children.Add(nodeNew);

                        nodeNew.Tag = prop;
                    }

                }

                // ���ݿ�
                if (node.ImageIndex == RESTYPE_DB)
                {
                    ResPath respath = new ResPath(node);

                    List<string> froms = null;
                    List<string> styles = null;
                    nRet = GetFroms(
                        respath.Url,
                        respath.Path,
                        out froms,
                        out styles,
                        out strError);
                    if (nRet == -1)
                    {
                        if (node != null)
                        {
                            SetLoading(node);	// ������ƺ������³���+��
                            node.Collapse();
                        }
                        goto ERROR1;
                    }

                    Debug.Assert(froms.Count == styles.Count, "");

                    children.Clear();

                    List<string> order_table = (List<string>)this.sort_tables[respath.FullPath];

                    if (order_table != null)
                    {
                        Rearrange(order_table,
        ref froms,
        ref styles);
                    }


                    for (int i = 0; i < froms.Count; i++)
                    {
                        Application.DoEvents();	// ���ý������Ȩ

                        string strFrom = froms[i];
                        TreeNode nodeNew = new TreeNode(strFrom,
                            RESTYPE_FROM, RESTYPE_FROM);

                        dp2FromInfo info = new dp2FromInfo();
                        info.Caption = strFrom;
                        info.Style = styles[i];

                        nodeNew.Tag = info;  // dp2FromInfo��

                        // SetLoading(nodeNew);

                        if (EnabledIndices != null
                            && StringUtil.IsInList(nodeNew.ImageIndex, EnabledIndices) == false)
                            nodeNew.ForeColor = ControlPaint.LightLight(nodeNew.ForeColor);

                        children.Add(nodeNew);
                    }
                }
            }
            finally
            {
                this.Enabled = true;
            }
            return 0;
        ERROR1:
            return -1;
        }

        // ��Hashtable����ת��Ϊ���ڴ洢���ַ���
        public static string SaveSortTables(Hashtable sort_table)
        {
            if (sort_table == null)
                return "";

            StringBuilder strResult = new StringBuilder(4096);
            foreach (string key in sort_table.Keys)
            {
                List<string> values = (List<string>)sort_table[key];
                if (values == null)
                    continue;
                string strValues = StringUtil.MakePathList(values);
                if (strResult.Length > 0)
                    strResult.Append(";");
                strResult.Append(key + "=" + strValues);
            }

            return strResult.ToString();
        }

        // ���ַ����ָ�ΪHashtable����
        public static Hashtable RestoreSortTables(string strText)
        {
            Hashtable table = new Hashtable();
            if (String.IsNullOrEmpty(strText) == true)
                return table;

            string[] parts = strText.Split(new char[] {';'});
            foreach (string part in parts)
            {
                string strName = "";
                string strValues = "";
                int nRet = part.IndexOf("=");
                if (nRet == -1)
                    strName = part;
                else
                {
                    strName = part.Substring(0, nRet);
                    strValues = part.Substring(nRet + 1);
                }

                table[strName] = StringUtil.SplitList(strValues);
            }

            return table;
        }

        void DoStop(object sender, StopEventArgs e)
        {
            if (this.channel != null)
                this.channel.Abort();
        }

        // ����·����չ��
        // ע�⣺respath��Url���Ƿ�������URL����TreeView�ĵ�һ��Ϊ������������һ��
        public void ExpandPath(ResPath respath)
        {

            string[] aName = respath.Path.Split(new Char[] { '/' });

            TreeNode node = null;
            TreeNode nodeThis = null;


            string[] temp = new string[aName.Length + 1];
            Array.Copy(aName, 0, temp, 1, aName.Length);
            temp[0] = respath.Url;

            aName = temp;

            for (int i = 0; i < aName.Length; i++)
            {
                TreeNodeCollection nodes = null;

                if (node == null)
                    nodes = this.Nodes;
                else
                    nodes = node.Nodes;

                bool bFound = false;
                for (int j = 0; j < nodes.Count; j++)
                {
                    TreeNode currrent_node = nodes[j];

                    string strName = "";
                    if (currrent_node.Tag is dp2ServerNodeInfo)
                    {
                        // �����server�ڵ㣬����Ҫȡ��URL
                        // 2008/3/9
                        dp2ServerNodeInfo info = (dp2ServerNodeInfo)currrent_node.Tag;
                        if (info == null)
                        {
                            // ϣ����������������
                            strName = currrent_node.Text;
                            Debug.Assert(false, "server���͵�node TagΪ��");
                        }
                        else
                            strName = info.Url;
                    }
                    else
                    {
                        // ����������ڵ㣬ȡ�ڵ����ļ���
                        strName = currrent_node.Text;
                    }

                    if (aName[i] == strName)
                    {
                        bFound = true;
                        nodeThis = nodes[j];
                        break;
                    }
                }
                if (bFound == false)
                    break;

                node = nodeThis;

                // ��Ҫչ��
                if (IsLoading(node) == true)
                {
                    Fill(node);
                }
                node.Expand();  // �������ղ��û���ҵ���ҲҪչ���м���
            }

            if (nodeThis != null && nodeThis.Parent != null)
                nodeThis.Parent.Expand();

            this.SelectedNode = nodeThis;
        }


        // ������¼·�����ĸ�����
        // ��¼·����������ʽ������͵���
        // ���� ���ط�����/����ͼ��/1
        // ���� ����ͼ��/1@���ط�����
        public static void ParseRecPath(string strFullPath,
            out string strServerName,
            out string strPath)
        {
            int nRet = strFullPath.IndexOf("@");
            if (nRet == -1)
            {
                // ����������
                nRet = strFullPath.IndexOf("/");
                if (nRet == -1)
                {
                    strServerName = strFullPath;
                    strPath = "";
                    return;
                }

                strServerName = strFullPath.Substring(0, nRet);
                strPath = strFullPath.Substring(nRet + 1);
                return;
            }

            // �����ǵ���
            strPath = strFullPath.Substring(0, nRet).Trim();
            strServerName = strFullPath.Substring(nRet + 1).Trim();

        }

        // ����ȫ·����չ��
        // ·����������ʽ������͵���
        // ���� ���ط�����/����ͼ��/1
        // ���� ����ͼ��/1@���ط�����
        public void ExpandPath(string strFullPath)
        {
            string strServerName = "";
            string strPath = "";

            ParseRecPath(strFullPath,
                out strServerName,
                out strPath);

            string[] aName = strPath.Split(new Char[] { '/' });

            TreeNode node = null;
            TreeNode nodeThis = null;

            string[] temp = new string[aName.Length + 1];
            Array.Copy(aName, 0, temp, 1, aName.Length);
            temp[0] = strServerName;

            aName = temp;

            for (int i = 0; i < aName.Length; i++)
            {
                TreeNodeCollection nodes = null;

                if (node == null)
                    nodes = this.Nodes;
                else
                    nodes = node.Nodes;

                bool bFound = false;
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (aName[i] == nodes[j].Text)
                    {
                        bFound = true;
                        nodeThis = nodes[j];
                        break;
                    }
                }
                if (bFound == false)
                    break;

                node = nodeThis;

                // ��Ҫչ��
                if (IsLoading(node) == true)
                {
                    Fill(node);
                }
                node.Expand();  // �������ղ��û���ҵ���ҲҪչ���м���
            }

            if (nodeThis != null && nodeThis.Parent != null)
                nodeThis.Parent.Expand();

            this.SelectedNode = nodeThis;
        }

        // ��һ���ڵ��¼�����"loading..."���Ա����+��
        public static void SetLoading(TreeNode node)
        {
            if (node == null)
                return;

            // ��node
            TreeNode nodeNew = new TreeNode("loading...", RESTYPE_LOADING, RESTYPE_LOADING);

            node.Nodes.Clear();
            node.Nodes.Add(nodeNew);
        }

        // �¼��Ƿ����loading...?
        public static bool IsLoading(TreeNode node)
        {
            if (node.Nodes.Count == 0)
                return false;

            if (node.Nodes[0].Text == "loading...")
                return true;

            return false;
        }

        public void Refresh(RefreshStyle style)
        {
            ResPath OldPath = null;
            bool bExpanded = false;

            bool bFocused = this.Focused;

            // ����
            if (this.SelectedNode != null)
            {
                OldPath = new ResPath(this.SelectedNode);
                bExpanded = this.SelectedNode.IsExpanded;
            }

            // ˢ�·�������
            if ((style & RefreshStyle.Servers) == RefreshStyle.Servers)
            {
                this.Fill(null);
            }

            // ˢ�µ�ǰѡ��Ľڵ�
            if (OldPath != null
                && (style & RefreshStyle.Selected) == RefreshStyle.Selected)
            {
                ResPath respath = OldPath.Clone();

                // ˢ��

                respath.Path = "";
                ExpandPath(respath);	// ѡ�з����������½ڵ����
                SetLoading(this.SelectedNode);

                ExpandPath(OldPath);
                if (bExpanded == true && this.SelectedNode != null)
                    this.SelectedNode.Expand();
            }

            if (this.Focused != bFocused)
                this.Focus();
        }

        delegate int Delegate_Fill(TreeNode node);

        private void dp2ResTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            if (node == null)
                return;

            // ��Ҫչ��
            if (IsLoading(node) == true)
            {
                // this.Fill(node);

                this.Update();

                object[] pList = new object[] { node };
                this.BeginInvoke(new Delegate_Fill(this.Fill), pList);

            }
        }

        public void Stop()
        {
            if (this.m_current_stop != null)
                this.m_current_stop.DoStop();
        }

        // ���һ���������µ�ȫ�����ݿ���
        int GetDbNames(
            string strServerUrl,
            // out string [] dbnames,
            out List<NormalDbProperty> properties,
            out string strError)
        {
            strError = "";
            int nRet = 0;
            properties = new List<NormalDbProperty>();

            if (this.Channels == null)
            {
                strError = "this.Channels == null";
                return -1;
            }

            this.channel = this.Channels.GetChannel(strServerUrl);
            if (this.channel == null)
            {
                strError = "GetChannel() error. strServerUrl '"+strServerUrl+"'";
                return -1;
            }

            DigitalPlatform.Stop stop = null;
            if (this.stopManager != null)
            {
                stop = new DigitalPlatform.Stop();
                stop.Register(this.stopManager, true);	// ����������

                stop.OnStop += new StopEventHandler(this.DoStop);
                stop.Initial("���ڻ�÷����� "+strServerUrl+" �Ŀ����б� ...");
                stop.BeginLoop();

                this.m_current_stop = stop;
            }

            try
            {
#if NO
                string strValue = "";
                long lRet = this.channel.GetSystemParameter(stop,
                    "biblio",
                    "dbnames",
                    out strValue,
                    out strError);
                if (lRet == -1)
                {
                    strError = "��Է����� " + this.channel.Url + " ��ñ�Ŀ�����б���̷�������" + strError;
                    return -1;
                }

                string [] dbnames = strValue.Split(new char [] {','});
                for (int i = 0; i < dbnames.Length; i++)
                {
                    string strDbName = dbnames[i].Trim();
                    if (String.IsNullOrEmpty(strDbName) == true)
                        continue;
                    NormalDbProperty prop = new NormalDbProperty();
                    prop.DbName = strDbName;
                    properties.Add(prop);
                }
#endif

                string version = "0.0";
                // return:
                //      -1  error
                //      0   dp2Library�İ汾�Ź��͡�������Ϣ��strError��
                //      1   dp2Library�汾�ŷ���Ҫ��
                nRet = LibraryChannel.GetServerVersion(
                    this.channel,
                    stop,
                    out version,
                    out strError);
                if (nRet != 1)
                    return -1;

                if (this.TestMode == true && StringUtil.CompareVersion(version, "2.34") < 0)
                {
                    strError = "dp2 ǰ�˵�����ģʽֻ���������ӵ� dp2library �汾Ϊ 2.34 ����ʱ����ʹ�� (��ǰ dp2library �汾Ϊ " + version.ToString() + ")";
                    return -1;
                }

                string base_version = "2.60";

                // ��� dp2library ��Ͱ汾Ҫ�� 2.60
                if (StringUtil.CompareVersion(version, base_version) < 0) // 2.48
                {
                    strError = "dp2 ǰ�������ӵ� dp2library �汾��������Ϊ " + base_version + " ����ʱ����ʹ�� (��ǰ dp2library �汾Ϊ " + version.ToString() + ")";
                    return -1;
                }

                string strValue = "";
                long lRet = this.channel.GetSystemParameter(stop,
                    "system",
                    "biblioDbGroup",
                    out strValue,
                    out strError);
                if (lRet == -1)
                {
                    strError = "��Է����� " + this.channel.Url + " ��ñ�Ŀ��������Ϣ�Ĺ��̷�������" + strError;
                    return -1;
                }

                XmlDocument dom = new XmlDocument();
                dom.LoadXml("<root />");
                try
                {
                    dom.DocumentElement.InnerXml = strValue;
                }
                catch (Exception ex)
                {
                    strError = "category=system,name=biblioDbGroup�����ص�XMLƬ����װ��InnerXmlʱ����: " + ex.Message;
                    return -1;
                }

                XmlNodeList nodes = dom.DocumentElement.SelectNodes("database");
                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlNode node = nodes[i];

                    string strDbName = DomUtil.GetAttr(node, "biblioDbName");
                    if (String.IsNullOrEmpty(strDbName) == true)
                        continue;

                    NormalDbProperty prop = new NormalDbProperty();
                    properties.Add(prop);
                    prop.DbName = strDbName;

                    prop.ItemDbName = DomUtil.GetAttr(node, "itemDbName");
                    prop.Syntax = DomUtil.GetAttr(node, "syntax");
                    prop.IssueDbName = DomUtil.GetAttr(node, "issueDbName");
                    prop.OrderDbName = DomUtil.GetAttr(node, "orderDbName");
                    prop.CommentDbName = DomUtil.GetAttr(node, "commentDbName");
                    prop.Role = DomUtil.GetAttr(node, "role");

                    bool bValue = true;
                    nRet = DomUtil.GetBooleanParam(node,
                        "inCirculation",
                        true,
                        out bValue,
                        out strError);
                    prop.InCirculation = bValue;
                }

                if (properties.Count > 0)
                {
                    // return:
                    //      -1  ������ϣ�������Ժ�Ĳ���
                    //      0   �ɹ�
                    nRet = GetBrowseColumns(
                        this.channel,
                        stop,
                        ref properties,
                        out strError);
                    if (nRet == -1)
                        return -1;
                }
            }
            finally
            {
                if (this.stopManager != null)
                {
                    this.m_current_stop = null;

                    stop.EndLoop();
                    stop.OnStop -= new StopEventHandler(this.DoStop);
                    stop.Initial("");

                    stop.Unregister();	// �������������
                }
            }

            // this.channel = null;
            return 0;
        }

        // return:
        //      -1  ������ϣ�������Ժ�Ĳ���
        //      0   �ɹ�
        public int GetBrowseColumns(
            LibraryChannel Channel,
            Stop stop,
            ref List<NormalDbProperty> properties,
            out string strError)
        {
            strError = "";

            // Stop.Initial("���ڻ����ͨ�������б� ...");


            // ���browse�����ļ�
            for (int i = 0; i < properties.Count; i++)
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

                NormalDbProperty normal = properties[i];

                // normal.ColumnNames = new List<string>();
                normal.ColumnNames = new ColumnPropertyCollection();

                string strContent = "";
                byte[] baCfgOutputTimestamp = null;
                int nRet = GetCfgFile(
                    Channel,
                    stop,   // stop
                    normal.DbName,
                    "browse",
                    out strContent,
                    out baCfgOutputTimestamp,
                    out strError);
                if (nRet == -1)
                    return -1;

                XmlDocument dom = new XmlDocument();
                try
                {
                    dom.LoadXml(strContent);
                }
                catch (Exception ex)
                {
                    strError = "���ݿ� " + normal.DbName + " ��browse�����ļ�����װ��XMLDOMʱ����: " + ex.Message;
                    return -1;
                }

                XmlNodeList nodes = dom.DocumentElement.SelectNodes("//col");
                foreach (XmlNode node in nodes)
                {
                    string strColumnType = DomUtil.GetAttr(node, "type");
                    // 2013/10/23
                    string strColumnTitle = GetColumnTitle(node,
                        this.Lang); 
                    // normal.ColumnNames.Add(strColumnTitle);
                    normal.ColumnNames.Add(strColumnTitle, strColumnType);
                }
            }

            return 0;
        }

        // ��� col Ԫ�ص� title ����ֵ������������������ص� title Ԫ��ֵ
        /*
<col>
	<title>
		<caption lang='zh-CN'>����</caption>
		<caption lang='en'>Title</caption>
	</title>
         * */
        public static string GetColumnTitle(XmlNode nodeCol,
            string strLang = "zh")
        {
            string strColumnTitle = DomUtil.GetAttr(nodeCol, "title");
            if (string.IsNullOrEmpty(strColumnTitle) == false)
                return strColumnTitle;
            XmlNode nodeTitle = nodeCol.SelectSingleNode("title");
            if (nodeTitle == null)
                return "";
            return DomUtil.GetCaption(strLang, nodeTitle);
        }

        // �г��������ڵ�����
        public List<string> GetServerNames()
        {
            // ��Ҫչ��
            if (this.Nodes.Count == 0)
            {
                string strError = "";
                int nRet = Fill(null, out strError);
                if (nRet == -1)
                {
                    throw new Exception(strError);
                }
            }

            List<string> results = new List<string>();
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                TreeNode currrent_node = this.Nodes[i];
                Debug.Assert(currrent_node.Tag is dp2ServerNodeInfo, "");
                dp2ServerNodeInfo info = (dp2ServerNodeInfo)currrent_node.Tag;

                results.Add(info.Name);
            }

            return results;
        }

        public List<string> GetDbNames(string strServerName)
        {
            TreeNode server_node = FindServer(strServerName);
            if (server_node == null)
            {
                throw new Exception("û���ҵ���Ϊ '"+strServerName+"' �ķ������ڵ�");
            }

            // ��Ҫչ��
            if (IsLoading(server_node) == true)
            {
                string strError = "";
                int nRet = Fill(server_node,out strError);
                if (nRet == -1)
                {
                    throw new Exception(strError);
                }
            } 
            
            List<string> results = new List<string>();

            Debug.Assert(server_node != null, "");
            for (int i = 0; i < server_node.Nodes.Count; i++)
            {
                TreeNode node = server_node.Nodes[i];
                results.Add(node.Text);
            }

            return results;
        }

        public NormalDbProperty GetDbProperty(string strServerName,
    string strDbName)
        {
            TreeNode server_node = FindServer(strServerName);
            if (server_node == null)
            {
                throw new Exception("û���ҵ���Ϊ '" + strServerName + "' �ķ������ڵ�");
            }

            string strError = "";
            // ��Ҫչ��
            if (IsLoading(server_node) == true)
            {
                int nRet = Fill(server_node, out strError);
                if (nRet == -1)
                {
                    throw new Exception(strError);
                }
            }

            TreeNode db_node = FindDb(server_node, strDbName);
            if (db_node == null)
            {
                throw new Exception("�ڷ����� '" + strServerName + "' ��û���ҵ���Ϊ '" + strDbName + "' �����ݿ�ڵ�");
            }

            return (NormalDbProperty)db_node.Tag;
        }

        public List<string> GetFromNames(string strServerName,
            string strDbName)
        {
            TreeNode server_node = FindServer(strServerName);
            if (server_node == null)
            {
                throw new Exception("û���ҵ���Ϊ '" + strServerName + "' �ķ������ڵ�");
            }

            string strError = "";
            // ��Ҫչ��
            if (IsLoading(server_node) == true)
            {
                int nRet = Fill(server_node, out strError);
                if (nRet == -1)
                {
                    throw new Exception(strError);
                }
            }

            TreeNode db_node = FindDb(server_node, strDbName);
            if (db_node == null)
            {
                throw new Exception("�ڷ����� '" + strServerName + "' ��û���ҵ���Ϊ '" + strDbName + "' �����ݿ�ڵ�");
            }

            // ��Ҫչ��
            if (IsLoading(db_node) == true)
            {
                int nRet = Fill(db_node, out strError);
                if (nRet == -1)
                {
                    throw new Exception(strError);
                }
            }

            List<string> results = new List<string>();

            Debug.Assert(db_node != null, "");
            for (int i = 0; i < db_node.Nodes.Count; i++)
            {
                TreeNode node = db_node.Nodes[i];
                results.Add(node.Text);
            }

            return results;
        }

        // �ҵ��������ڵ�
        TreeNode FindServer(string strServerUrlOrName)
        {
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                TreeNode currrent_node = this.Nodes[i];
                Debug.Assert(currrent_node.Tag is dp2ServerNodeInfo, "");
                dp2ServerNodeInfo info = (dp2ServerNodeInfo)currrent_node.Tag;

                if (info.Url == strServerUrlOrName
                    || info.Name == strServerUrlOrName)
                    return currrent_node;
            }

            return null;
        }

        static TreeNode FindDb(TreeNode server_node, string strDbName)
        {
            Debug.Assert(server_node != null, "");
            for (int i = 0; i < server_node.Nodes.Count; i++)
            {
                TreeNode node = server_node.Nodes[i];
                if (node.Text == strDbName)
                    return node;
            }

            return null;
        }

        public ColumnPropertyCollection GetBrowseColumnNames(string strServerUrlOrName,
            string strDbName)
        {
            // �ҵ��������ڵ�
            TreeNode node_server = this.FindServer(strServerUrlOrName);
            if (node_server == null)
                return null;    // not found server

            // �ҵ����ݿ�ڵ�
            TreeNode node_db = FindDb(node_server, strDbName);
            if (node_db == null)
                return null;    // not found db

            NormalDbProperty prop = (NormalDbProperty)node_db.Tag;

            return prop.ColumnNames;
        }

        // ��������ļ�
        public int GetCfgFile(
            LibraryChannel Channel,
            Stop stop,
            string strDbName,
            string strCfgFileName,
            out string strContent,
            out byte[] baOutputTimestamp,
            out string strError)
        {
            baOutputTimestamp = null;
            strError = "";
            strContent = "";

            /*
            if (m_nInGetCfgFile > 0)
            {
                strError = "GetCfgFile() ������";
                return -1;
            }*/

            /*
            if (stop != null)
            {
                stop.OnStop += new StopEventHandler(this.DoStop);
                stop.Initial("�������������ļ� ...");
                stop.BeginLoop();
            }*/

            // m_nInGetCfgFile++;

            try
            {
                string strPath = strDbName + "/cfgs/" + strCfgFileName;

                if (stop != null)   // 2006/12/16
                    stop.SetMessage("�������������ļ� " + strPath + " ...");

                string strMetaData = "";
                string strOutputPath = "";

                string strStyle = "content,data,metadata,timestamp,outputpath";

                long lRet = 0;

                if (this.cfgCache != null)
                lRet = Channel.GetRes(stop,
                    this.cfgCache,
                    strPath,
                    strStyle,
                    null,
                    out strContent,
                    out strMetaData,
                    out baOutputTimestamp,
                    out strOutputPath,
                    out strError);
                else
                lRet = Channel.GetRes(stop,
    strPath,
    strStyle,
    out strContent,
    out strMetaData,
    out baOutputTimestamp,
    out strOutputPath,
    out strError);
                if (lRet == -1)
                    goto ERROR1;

            }
            finally
            {
                /*
                if (stop != null)
                {
                    stop.EndLoop();
                    stop.OnStop -= new StopEventHandler(this.DoStop);
                    stop.Initial("");
                }*/

                // m_nInGetCfgFile--;
            }

            return 1;
        ERROR1:
            return -1;
        }

        // ���һ�����ݿ��µļ���;��
        int GetFroms(
            string strServerUrl,
            string strDbName,
            out List<string> froms,
            out List<string> styles,
            out string strError)
        {
            froms = new List<string>();
            styles = new List<string>();

            strError = "";

            // Debug.Assert(false, "");

            this.channel = this.Channels.GetChannel(strServerUrl);
            Debug.Assert(this.channel != null, "");

            DigitalPlatform.Stop stop = null;
            if (this.stopManager != null)
            {
                stop = new DigitalPlatform.Stop();
                stop.Register(this.stopManager, true);	// ����������

                stop.OnStop += new StopEventHandler(this.DoStop);
                stop.Initial("���ڻ�ÿ� "+strDbName+" �ļ���;���б� ...");
                stop.BeginLoop();
                this.m_current_stop = stop;
            }

            try
            {
                BiblioDbFromInfo[] infos = null;

                long lRet = this.channel.ListDbFroms(stop,
                    "biblio",
                    this.Lang,
                    out infos,
                    out strError);
                if (lRet == -1)
                {
                    strError = "��Է����� " + this.channel.Url + " �г���Ŀ�����;�����̷�������" + strError;
                    return -1;
                }

                for (int i = 0; i < infos.Length; i++)
                {
                    froms.Add(infos[i].Caption);
                    styles.Add(infos[i].Style);
                }
            }
            finally
            {
                if (this.stopManager != null)
                {
                    this.m_current_stop = null;
                    stop.EndLoop();
                    stop.OnStop -= new StopEventHandler(this.DoStop);
                    stop.Initial("");

                    stop.Unregister();	// �������������
                }
            }

            // this.channel = null;

            return 0;
        }

        // ���˵� _ ��ͷ����Щstyle�Ӵ�
        // parameters:
        //      bRemove2    �Ƿ��˳� __ ǰ׺��
        //      bRemove1    �Ƿ��˳� _ ǰ׺��
        public static string GetDisplayFromStyle(string strStyles,
            bool bRemove2,
            bool bRemove1)
        {
            string[] parts = strStyles.Split(new char[] { ',' });
            List<string> results = new List<string>();
            foreach (string part in parts)
            {
                string strText = part.Trim();
                if (String.IsNullOrEmpty(strText) == true)
                    continue;

                if (strText[0] == '_')
                {
                    if (bRemove1 == true)
                    {
                        if (strText.Length >= 2 && /*strText[0] == '_' &&*/ strText[1] != '_')
                            continue;
#if NO
                        if (strText[0] == '_')
                            continue;
#endif
                        if (strText.Length == 1)
                            continue;
                    }

                    if (bRemove2 == true && strText.Length >= 2)
                    {
                        if (/*strText[0] == '_' && */ strText[1] == '_')
                            continue;
                    }
                }


                results.Add(strText);
            }

            return StringUtil.MakePathList(results, ",");
        }

        public static int GetNodeInfo(TreeNode node,
            out string strServerName,
            out string strServerUrl,
            out string strDbName,
            out string strFrom,
            out string strFromStyle,
            out string strError)
        {
            strError = "";
            strServerName = "";
            strServerUrl = "";
            strDbName = "";
            strFrom = "";
            strFromStyle = "";

            List<TreeNode> node_path = new List<TreeNode>();
            while (true)
            {
                if (node == null)
                    break;
                node_path.Insert(0, node);
                node = node.Parent;
            }

            if (node_path.Count > 0)
            {
                dp2ServerNodeInfo server_info = (dp2ServerNodeInfo)node_path[0].Tag;
                strServerName = server_info.Name;
                strServerUrl = server_info.Url;
            }

            if (node_path.Count > 1)
            {
                strDbName = node_path[1].Text;
            }

            if (node_path.Count > 2)
            {
                dp2FromInfo from_info = (dp2FromInfo)node_path[2].Tag;
                strFrom = from_info.Caption;
                strFromStyle = from_info.Style;
            }


            return 0;
        }

        static bool IsFirstChild(TreeNode node)
        {
            if (node == null)
                return false;
            TreeNodeCollection nodes = null;
            if (node.Parent != null)
                nodes = node.Parent.Nodes;
            else
                nodes = node.TreeView.Nodes;

            if (nodes.IndexOf(node) == 0)
                return true;

            return false;
        }

        static bool IsLastChild(TreeNode node)
        {
            if (node == null)
                return false;

            TreeNodeCollection nodes = null;
            if (node.Parent != null)
                nodes = node.Parent.Nodes;
            else
                nodes = node.TreeView.Nodes;

            if (nodes.IndexOf(node) == nodes.Count - 1)
                return true;

            return false;
        }

        private void dp2ResTree_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            TreeNode node = this.SelectedNode;

            //
            menuItem = new MenuItem("����(&U)");
            menuItem.Click += new System.EventHandler(this.button_moveUp_Click);
            if (node == null || IsFirstChild(node) == true)
                menuItem.Enabled = false;
            contextMenu.MenuItems.Add(menuItem);

            //
            menuItem = new MenuItem("����(&D)");
            menuItem.Click += new System.EventHandler(this.button_moveDown_Click);
            if (node == null || IsLastChild(node) == true)
                menuItem.Enabled = false;
            contextMenu.MenuItems.Add(menuItem);

            // ---
            menuItem = new MenuItem("-");
            contextMenu.MenuItems.Add(menuItem);


            // 
            menuItem = new MenuItem("����ѡ(&M)");
            menuItem.Click += new System.EventHandler(this.menu_toggleCheckBoxes);
            if (this.CheckBoxes == true)
                menuItem.Checked = true;
            else
                menuItem.Checked = false;
            contextMenu.MenuItems.Add(menuItem);

            menuItem = new MenuItem("���ȫ����ѡ(&C)");
            menuItem.Click += new System.EventHandler(this.menu_clearCheckBoxes);
            if (this.CheckBoxes == true)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;
            contextMenu.MenuItems.Add(menuItem);

            // ---
            menuItem = new MenuItem("-");
            contextMenu.MenuItems.Add(menuItem);

            menuItem = new MenuItem("ˢ��(&R)");
            menuItem.Click += new System.EventHandler(this.menu_refresh);
            contextMenu.MenuItems.Add(menuItem);


            contextMenu.Show(this, new Point(e.X, e.Y));	
        }

        // ˢ��
        public void menu_refresh(object sender, System.EventArgs e)
        {
            this.Refresh(RefreshStyle.All);
        }

        void menu_clearCheckBoxes(object sender, System.EventArgs e)
        {
            this.ClearChildrenCheck(null);
        }

        // ����¼����е�ѡ�е���(�������Լ�)
        // parameters:
        //      nodeStart   ���node�����Ϊnull, ��ʾ�Ӹ��㿪ʼ�����ȫ��
        public void ClearChildrenCheck(TreeNode nodeStart)
        {
            TreeNodeCollection nodes = null;
            if (nodeStart == null)
            {
                nodes = this.Nodes;
            }
            else
                nodes = nodeStart.Nodes;

            foreach (TreeNode node in nodes)
            {
                node.Checked = false;
                ClearChildrenCheck(node);	// �ݹ�
            }
        }

        void menu_toggleCheckBoxes(object sender, System.EventArgs e)
        {
            if (this.CheckBoxes == true)
            {
                this.ClearChildrenCheck(null);
                this.CheckBoxes = false;
            }
            else
                this.CheckBoxes = true;
        }

        void button_moveUp_Click(object sender, EventArgs e)
        {
            MoveUpOrDown(this.SelectedNode,
    true);

        }

        void button_moveDown_Click(object sender, EventArgs e)
        {
            MoveUpOrDown(this.SelectedNode,
false);

        }

        bool MoveUpOrDown(TreeNode node,
            bool bUp)
        {
            TreeNodeCollection nodes = null;
            if (node.Parent != null)
                nodes = node.Parent.Nodes;
            else
                nodes = node.TreeView.Nodes;

            int nOldIndex = nodes.IndexOf(node);
            int nNewIndex = -1;
            if (bUp == true)
            {
                nNewIndex = nOldIndex - 1;
                if (nNewIndex < 0)
                    return false;
            }
            else
            {
                nNewIndex = nOldIndex + 1;
                if (nNewIndex >= nodes.Count)
                    return false;
            }

            nodes.RemoveAt(nOldIndex);
            nodes.Insert(nNewIndex, node);

            if (node.Tag is dp2ServerNodeInfo)
            {
                dp2Server server = (dp2Server)this.Servers[nOldIndex];
                this.Servers.RemoveAt(nOldIndex);
                this.Servers.Insert(nNewIndex, server);
                this.Servers.Changed = true;
            }
            else
            {
                // ����sort_tables
                if (node.Tag is NormalDbProperty
                    || node.Tag is dp2FromInfo)
                {
                    ResPath respath = new ResPath(node.Parent);
                    List<string> values = new List<string>();
                    foreach (TreeNode temp in node.Parent.Nodes)
                    {
                        values.Add(temp.Text);
                    }
                    this.sort_tables[respath.FullPath] = values;
                    this.SortTableChanged = true;
                }
            }

            this.SelectedNode = node;

            return true;
        }

        // ���¼�
        protected override void OnKeyDown(KeyEventArgs e)
        {
            Debug.WriteLine(e.KeyCode.ToString());

            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        if (e.Modifiers == Keys.Control
                            && this.SelectedNode != null)
                        {
                            MoveUpOrDown(this.SelectedNode,
                true);
                        }
                    }
                    break;
                case Keys.Down:
                    {
                        if (e.Modifiers == Keys.Control
                            && this.SelectedNode != null)
                        {
                            MoveUpOrDown(this.SelectedNode,
                false);
                        }
                    }
                    break;
                default:
                    break;
            }

            base.OnKeyDown(e);
        }

        // ����¼����е�ѡ�е���(�������Լ�)
        public static void ClearOneLevelChildrenCheck(TreeNode nodeStart)
        {
            if (nodeStart == null)
                return;
            foreach (TreeNode node in nodeStart.Nodes)
            {
                node.Checked = false;
                // ClearChildrenCheck(node);	// ��ʱ���ݹ�
            }
        }

        private void dp2ResTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.Checked == true)
            {
                e.Node.BackColor = Color.Yellow;
                e.Node.ForeColor = Color.Black;

                e.Node.NodeFont = new Font(this.Font, FontStyle.Bold);
            }
            else
            {
                e.Node.BackColor = this.BackColor;
                e.Node.ForeColor = this.ForeColor;

                e.Node.NodeFont = null;
            }

            if (e.Node.Checked == false)
            {
                ClearOneLevelChildrenCheck(e.Node);
            }
            else
            {
                if (e.Node.Parent != null)
                    e.Node.Parent.Checked = true;
            }

            // ע���¼��Լ���ݹ�
        }

        // ��һ�׶Ρ��������TargetItem��Url��Target����
        // �������ϵ�ѡ��״̬���ɼ���Ŀ���ַ���
        // ��ͬ�ķ������е��ַ����ֿ���
        // return:
        //      -1  ����
        //      0   ��δѡ������Ŀ��
        //      1   �ɹ�
        public int GetSearchTarget(out TargetItemCollection result_items,
            out string strError)
        {
            strError = "";
            result_items = new TargetItemCollection();

            if (this.CheckBoxes == false)
            {
                List<TreeNode> aNode = new List<TreeNode>();
                TreeNode node = this.SelectedNode;
                if (node == null)
                {
                    strError = "��δѡ������Ŀ��";
                    return 0;
                }

                for (; node != null; )
                {
                    aNode.Insert(0, node);
                    node = node.Parent;
                }

                if (aNode.Count == 0)
                    goto END1;


                TargetItem item = new TargetItem();
                item.Lang = this.Lang;

                result_items.Add(item);

                item.ServerName = ((TreeNode)aNode[0]).Text;
                // ���server url
                dp2Server server = this.Servers.GetServerByName(item.ServerName);
                if (server == null)
                {
                    strError = "��Ϊ '" + item.ServerName + "' �ķ������ڼ���������δ����...";
                    return -1;
                }
                item.Url = server.Url;

                if (aNode.Count == 1)
                    goto END1;

                item.Target = ((TreeNode)aNode[1]).Text;


                if (aNode.Count == 2)
                    goto END1;

                item.Target += ":" + ((TreeNode)aNode[2]).Text;
            END1:
                return 1;
            }

            // ��ѡ�еķ�����
            foreach (TreeNode nodeServer in this.Nodes)
            {
                if (nodeServer.Checked == false)
                    continue;

                int nDbCount = 0;   // checked�����ݿ�������
                string strTargetList = "";

                // ��ѡ�е����ݿ�
                foreach (TreeNode nodeDb in nodeServer.Nodes)
                {
                    if (nodeDb.Checked == false)
                        continue;

                    if (nodeDb.ImageIndex != RESTYPE_DB)
                        continue;   // ��Ϊ�����������ļ�Ŀ¼�����ļ�������Ҫ����

                    nDbCount++;

                    if (strTargetList != "")
                        strTargetList += ";";
                    strTargetList += nodeDb.Text + ":";

                    // ��һ��strFrom�±��������Ժܺõش�����
                    string strFrom = "";
                    // ��ѡ�е�from
                    foreach (TreeNode nodeFrom in nodeDb.Nodes)
                    {
                        if (nodeFrom.Checked == true)
                        {
                            if (strFrom != "")
                                strFrom += ",";
                            strFrom += nodeFrom.Text;
                        }
                    }
                    strTargetList += strFrom;
                }

                if (nDbCount == 0)
                {
                    strError = "���ڷ����� '" + nodeServer .Text+ "' �ڵ��¼���ѡһ�����߶�����ݿ�ڵ㣬�������ɽ��С������ϣ�������˷������ڵ㣬��������乴ѡ״̬��";
                    return -1;
                }

                TargetItem item = new TargetItem();
                item.ServerName = nodeServer.Text;

                // ���server url
                dp2Server server = this.Servers.GetServerByName(item.ServerName);
                if (server == null)
                {
                    strError = "��Ϊ '" + item.ServerName + "' �ķ������ڼ���������δ����...";
                    return -1;
                }

                item.Url = server.Url;
                item.Target = strTargetList;
                item.Lang = this.Lang;

                result_items.Add(item);

                strTargetList = "";
            }

            if (result_items.Count == 0)
            {
                strError = "��δѡ������Ŀ��";
                return 0;
            }

            return 1;
        }
    }

    // ����Ŀ������
    public class TargetItem
    {
        public string Lang = "";

        public string ServerName = "";  // ��������������
        public string Url = "";         // ������URL

        public string Target = "";	// ����Ŀ���ַ���������"��1:from1,from2;��2:from1,from2"

        public string Words = "";	// ԭʼ̬�ļ�����,��δ�и�
        public string[] aWord = null;	// MakeWordPhrases()�ӹ�����ַ���
        public string Xml = "";
        public int MaxCount = -1;	// �������������
    }

    // ����Ŀ������
    public class TargetItemCollection : List<TargetItem>
    {
        // ������ݿ����б�
        // ÿ�����ݿ����ĸ�ʽΪ "���ݿ���@��������"
        public int GetDbNameList(out List<string> dbnames,
            out string strError)
        {
            strError = "";
            dbnames = new List<string>();

            foreach (TargetItem item in this)
            {
                List<string> current_dbnames = ParseDbNamesInTargetString(item.Target);
                StringUtil.RemoveDupNoSort(ref current_dbnames);
                foreach (string strDbName in current_dbnames)
                {
                    dbnames.Add(strDbName + "@" + item.ServerName);
                }
            }

            StringUtil.RemoveDupNoSort(ref dbnames);
            return 0;
        }

        // ��Target�ַ��������������ݿ���
        public static List<string> ParseDbNamesInTargetString(string strTargetString)
        {
            List<string> results = new List<string>();
            string[] parts = strTargetString.Split(new char[] {';'});
            foreach (string strPart in parts)
            {
                string strText = strPart.Trim();
                string strDbName = "";
                int nRet = strText.IndexOf(":");
                if (nRet != -1)
                    strDbName = strText.Substring(0, nRet);
                else
                    strDbName = strText;

                results.Add(strDbName);
            }

            return results;
        }

        // �ڶ��׶�: ����ÿ��TargetItem��Words��ԭʼ��̬�ļ����ʣ��и�Ϊstring[] aWord
        // ���ñ�����ǰ��Ӧ��Ϊÿ��TargetItem�������ú�Words��Աֵ
        // �ڶ��׶κ͵�һ�׶��Ⱥ�˳����Ҫ��
        public int MakeWordPhrases(
            string strDefaultMatchStyle = "left",
            bool bSplitWords = true,
            bool bAutoDetectRange = true,
            bool bAutoDetectRelation = true)
        {
            for (int i = 0; i < this.Count; i++)
            {
                TargetItem item = this[i];
                item.aWord = MakeWordPhrases(item.Words,
                    strDefaultMatchStyle,
                    bSplitWords,
                    bAutoDetectRange,
                    bAutoDetectRelation);
            }

            return 0;
        }


        // �����׶Σ�����ÿ��TargetItem�е�Target��aWord�������Xml����
        public int MakeXml()
        {
            string strText = "";
            for (int i = 0; i < this.Count; i++)
            {
                TargetItem item = this[i];

                strText = "";

                string strCount = "";

                if (item.MaxCount != -1)
                    strCount = "<maxCount>" + Convert.ToString(item.MaxCount) + "</maxCount>";

                for (int j = 0; j < item.aWord.Length; j++)
                {
                    if (j != 0)
                    {
                        strText += "<operator value='OR' />";
                    }

                    strText += "<item>" + item.aWord[j] + strCount + "</item>";
                }

                strText = "<target list='"
                    + StringUtil.GetXmlStringSimple(item.Target)       // 2007/9/14
                    + "'>" + strText
                    + "<lang>" + item.Lang + "</lang></target>";

                item.Xml = strText;
            }

            return 0;
        }

        // ƥ����������
        static bool MatchTailQuote(char left, char right)
        {
            if (left == '��' && right == '��')
                return true;
            if (left == '��' && right == '��')
                return true;

            if (left == '\'' && right == '\'')
                return true;

            if (left == '"' && right == '"')
                return true;

            return false;
        }

        // ���տո��и��������
        static List<string> SplitWords(string strWords)
        {
            List<string> results = new List<string>();
            string strWord = "";
            bool bInQuote = false;
            char chQuote = '\'';
            for (int i = 0; i < strWords.Length; i++)
            {
                if ("\'\"��������".IndexOf(strWords[i]) != -1)
                {
                    if (bInQuote == true
                        && MatchTailQuote(chQuote, strWords[i]) == true)
                    {
                        bInQuote = false;
                        continue;   // �ڽ���к����������
                    }
                    else if (bInQuote == false)
                    {
                        bInQuote = true;
                        chQuote = strWords[i];
                        continue;   // �ڽ���к����������
                    }
                }

                if ((strWords[i] == ' ' || strWords[i] == '��')
                    && bInQuote == false
                    && String.IsNullOrEmpty(strWord) == false)
                {
                    results.Add(strWord);
                    strWord = "";
                }
                else
                {
                    strWord += strWords[i];
                }
            }

            if (String.IsNullOrEmpty(strWord) == false)
            {
                results.Add(strWord);
                strWord = "";
            }


            return results;
        }

        //��"***-***"��ֳ�������
        public static int SplitRangeID(string strRange,
            out string strID1,
            out string strID2)
        {
            int nPosition;
            nPosition = strRange.IndexOf("-");
            strID1 = "";
            strID2 = "";
            if (nPosition > 0)
            {
                strID1 = strRange.Substring(0, nPosition).Trim();
                strID2 = strRange.Substring(nPosition + 1).Trim();
                if (strID2 == "")
                    strID2 = "9999999999";
            }
            if (nPosition == 0)
            {
                strID1 = "0";
                strID2 = strRange.Substring(1).Trim();
            }
            if (nPosition < 0)
            {
                strID1 = strRange.Trim();
                strID2 = strRange.Trim();
            }
            return 0;
        }

        // ����һ���������ַ��������տհ��и�ɵ��������ʣ�
        // ���Ҹ��ݼ������Ƿ�Ϊ���ֵȵȲ²���������������������
        // ��<item>�ڲ���Ԫ�ص��ַ�����������Ȼ�������<target>��Ԫ�أ�
        // ���չ���������<item>�ַ���
        public static string[] MakeWordPhrases(string strWords,
            string strDefaultMatchStyle = "left",
            bool bSplitWords = true,
            bool bAutoDetectRange = true,
            bool bAutoDetectRelation = true)
        {
            /*
			string[] aWord;
			aWord = strWords.Split(new Char [] {' '});
             */
            List<string> aWord = null;

            if (bSplitWords == true)
                aWord = SplitWords(strWords);

            if (aWord == null || aWord.Count == 0)
            {
                aWord = new List<string>();
                aWord.Add(strWords);
            }

            string strXml = "";
            string strWord = "";
            string strMatch = "";
            string strRelation = "";
            string strDataType = "";

            ArrayList aResult = new ArrayList();

            foreach (string strOneWord in aWord)
            {
                if (bAutoDetectRange == true)
                {
                    string strID1;
                    string strID2;

                    SplitRangeID(strOneWord, out strID1, out strID2);
                    if (StringUtil.IsNum(strID1) == true
                        && StringUtil.IsNum(strID2) && strOneWord != "")
                    {
                        strWord = strOneWord;
                        strMatch = "exact";
                        strRelation = "draw";
                        strDataType = "number";
                        goto CONTINUE;
                    }
                }


                if (bAutoDetectRelation == true)
                {
                    string strOperatorTemp;
                    string strRealText;

                    int ret;
                    ret = GetPartCondition(strOneWord,
                        out strOperatorTemp,
                        out strRealText);

                    if (ret == 0 && strOneWord != "")
                    {
                        strWord = strRealText;
                        strMatch = "exact";
                        strRelation = strOperatorTemp;
                        if (StringUtil.IsNum(strRealText) == true)
                            strDataType = "number";
                        else
                            strDataType = "string";
                        goto CONTINUE;
                    }
                }

                strWord = strOneWord;
                strMatch = strDefaultMatchStyle;
                strRelation = "=";
                strDataType = "string";
            CONTINUE:

                // 2007/4/5 ���� ������ GetXmlStringSimple()
                strXml += "<word>"
                    + StringUtil.GetXmlStringSimple(strWord)
                    + "</word>"
                    + "<match>" + strMatch + "</match>"
                    + "<relation>" + strRelation + "</relation>"
                    + "<dataType>" + strDataType + "</dataType>";

                aResult.Add(strXml);

                strXml = "";
            }
            string[] array = new string[aResult.Count];
            aResult.CopyTo(array);

            return array;

            // return ConvertUtil.GetStringArray(0, aResult);
        }

        // ���ݱ�ʾʽ���õ���������ֵ
        // return:
        //		0	�й�ϵ������
        //		-1	�޹�ϵ������				
        public static int GetPartCondition(string strText,
            out string strOperator,
            out string strRealText)
        {
            strText = strText.Trim();
            strOperator = "=";
            strRealText = strText;
            int nPosition;
            nPosition = strText.IndexOf(">=");
            if (nPosition >= 0)
            {
                strRealText = strText.Substring(nPosition + 2);

                strOperator = ">=";
                return 0;
            }
            nPosition = strText.IndexOf("<=");
            if (nPosition >= 0)
            {
                strRealText = strText.Substring(nPosition + 2);
                strOperator = "<=";
                return 0;
            }
            nPosition = strText.IndexOf("<>");
            if (nPosition >= 0)
            {
                strRealText = strText.Substring(nPosition + 2);
                strOperator = "<>";
                return 0;
            }

            nPosition = strText.IndexOf("><");
            if (nPosition >= 0)
            {
                strRealText = strText.Substring(nPosition + 2);
                strOperator = "<>";
                return 0;
            }
            nPosition = strText.IndexOf("!=");
            if (nPosition >= 0)
            {
                strRealText = strText.Substring(nPosition + 2);
                strOperator = "<>";
                return 0;
            }
            nPosition = strText.IndexOf(">");
            int nPosition2 = strText.IndexOf(">=");
            if (nPosition2 < 0 && nPosition >= 0)
            {
                strRealText = strText.Substring(nPosition + 1);
                strOperator = ">";
                return 0;
            }
            nPosition = strText.IndexOf("<");
            nPosition2 = strText.IndexOf("<=");
            if (nPosition2 < 0 && nPosition >= 0)
            {
                strRealText = strText.Substring(nPosition + 1);
                strOperator = "<";
                return 0;
            }
            return -1;
        }
    }

    // ���οؼ�Server���ͽڵ�Tag����Ϣ�ṹ
    public class dp2ServerNodeInfo
    {
        public string Name = "";    // ��ʾ����������
        public string Url = ""; // dp2library URL������http://localhost:8001/dp2library
    }

    /*
    public class dp2ServerInfo
    {
        public string Url = "";
        public string Name = "";
        public List<dp2DbInfo> DbInfos = null;
    }

    public class dp2DbInfo
    {
        public string DbName = "";
        public dp2FromInfo FromInfo = null;
    }
     * */

    // From�ڵ�Tag����Ϣ
    public class dp2FromInfo
    {
        public string Caption = "";
        public string Style = "";

    }

    // ��ͨ�������
    public class NormalDbProperty
    {
        public string DbName = "";
        // public List<string> ColumnNames = new List<string>();
        public ColumnPropertyCollection ColumnNames = new ColumnPropertyCollection();
        
        public string Syntax = "";  // ��ʽ�﷨
        public string ItemDbName = "";  // ��Ӧ��ʵ�����

        public string IssueDbName = ""; // ��Ӧ���ڿ���
        public string OrderDbName = ""; // ��Ӧ�Ķ�������

        public string CommentDbName = "";   // ��Ӧ����ע����
        public string Role = "";    // ��ɫ
        public bool InCirculation = true;  // �Ƿ������ͨ
    }
}
