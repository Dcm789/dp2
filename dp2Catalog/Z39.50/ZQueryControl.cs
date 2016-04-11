using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using DigitalPlatform.Xml;
using DigitalPlatform.Script;
using DigitalPlatform.Text;

namespace dp2Catalog
{
    public partial class ZQueryControl : UserControl
    {
        List<Line> Lines = new List<Line>();

        public ZQueryControl()
        {
            InitializeComponent();
        }

        private void QueryControl_Load(object sender, EventArgs e)
        {
            // this.AddLine(new string[] {"from1","from2" });
        }

        private void QueryControl_SizeChanged(object sender, EventArgs e)
        {
            tableLayoutPanel_main.Size = this.Size;
        }

        // ������һ��
        public void AddLine(string [] fromlist)
        {
            int nLastRow = this.tableLayoutPanel_main.RowCount;

            this.tableLayoutPanel_main.RowCount += 1;
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle());

            Line line = new Line(fromlist);

            line.AddToTable(this.tableLayoutPanel_main, nLastRow);

            this.Lines.Add(line);
        }

#if NOOOOOOOOOOOOOOOOOOOO
        // ��ü���ʽ
        public int GetQueryString(
            FromCollection Froms,
            out string strQueryString,
            out string strError)
        {
            strError = "";
            strQueryString = "";

            for (int i = 0; i < this.Lines.Count; i++)
            {
                Line line = this.Lines[i];

                string strLogic = line.comboBox_logicOperator.Text;
                string strWord = line.textBox_word.Text;
                string strFrom = line.comboBox_from.Text;

                if (strWord == "")
                    continue;

                if (strQueryString != "")
                    strQueryString += " " + strLogic + " ";

                int nRet = strFrom.IndexOf("-");
                if (nRet != -1)
                    strFrom = strFrom.Substring(0, nRet).Trim();

                string strValue = Froms.GetValue(strFrom);
                if (strValue == null)
                {
                    strError = "���� '" +strFrom+ "' ��use����û���ҵ���Ӧ�ı��";
                    return -1;
                }

                strWord.Replace("\"", "\\\"");
                strQueryString += "\""
                    + strWord + "\"" + "/1="
                    + strValue;

            }

            return 0;
        }

#endif

        static string GetLogicString(string strText)
        {
            int nRet = strText.IndexOf(" ");
            if (nRet != -1)
                return strText.Substring(0, nRet).Trim();

            return strText.Trim();
        }

        // �� XML ����ʽ�仯Ϊ������ʽ����ʽ
        public static int GetQueryString(
            FromCollection Froms,
            string strQueryXml,
            IsbnConvertInfo isbnconvertinfo,
            out string strQueryString,
            out string strError)
        {
            strError = "";
            strQueryString = "";

            if (String.IsNullOrEmpty(strQueryXml) == true)
                return 0;

            XmlDocument dom = new XmlDocument();
            try
            {
                dom.LoadXml(strQueryXml);
            }
            catch (Exception ex)
            {
                strError = "strQueryXmlװ��XMLDOMʱ����: " + ex.Message;
                return -1;
            }

            XmlNodeList nodes = dom.DocumentElement.SelectNodes("line");

            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];
                string strLogic = DomUtil.GetAttr(node, "logic");
                string strWord = DomUtil.GetAttr(node, "word");
                string strFrom = DomUtil.GetAttr(node, "from");

                if (strWord == "")
                    continue;

                strLogic = GetLogicString(strLogic);    // 2011/8/30

                if (strQueryString != "")
                    strQueryString += " " + strLogic + " ";

                int nRet = strFrom.IndexOf("-");
                if (nRet != -1)
                    strFrom = strFrom.Substring(0, nRet).Trim();

                string strValue = Froms.GetValue(strFrom);
                if (strValue == null)
                {
                    strError = "���� '" + strFrom + "' ��use����û���ҵ���Ӧ�ı��";
                    return -1;
                }

                // ��ISBN�����ʽ���Ԥ����
                if (strFrom == "ISBN"
                    && isbnconvertinfo != null)
                {
                    /*
                    // return:
                    //      -1  ����
                    //      0   û�б�Ҫת��
                    //      1   �Ѿ�ת��
                    nRet = isbnconvertinfo.ConvertISBN(ref strWord,
                out strError);
                    if (nRet == -1)
                    {
                        strError = "�ڴ���ISBN�ַ��� '" + strWord + "' �����г���: " + strError;
                        return -1;
                    }
                     * */
                    List<string> isbns = null;
                    // return:
                    //      -1  ����
                    //      0   û�б�Ҫת��
                    //      1   �Ѿ�ת��
                    nRet = isbnconvertinfo.ConvertISBN(strWord,
                        out isbns,
                        out strError);
                    if (nRet == -1)
                    {
                        strError = "�ڴ���ISBN�ַ��� '" + strWord + "' �����г���: " + strError;
                        return -1;
                    }

                    int j = 0;
                    foreach (string isbn in isbns)
                    {
                        if (j > 0)
                            strQueryString += " OR ";
                        // string strIsbn = isbn.Replace("\"", "\\\"");    // �ַ� " �滻Ϊ \"
                        string strIsbn = StringUtil.EscapeString(isbn, "\"/=");    // eacape �����ַ�
                        strQueryString += "\""
                            + strIsbn + "\"" + "/1="
                            + strValue;
                        j++;
                    }
                    continue;
                }

                // strWord = strWord.Replace("\"", "\\\""); // �ַ� " �滻Ϊ \"
                strWord = StringUtil.EscapeString(strWord, "\"/=");    // eacape �����ַ�
                strQueryString += "\""
                    + strWord + "\"" + "/1="
                    + strValue;
            }

            return 1;
        }

        // ��� XML ����ʽ
        // paramers:
        //      bOptimize   �Ƿ��Ż���
        public string GetContent(bool bOptimize)
        {
            bool bAllEmpty = true;  // �Ƿ�ÿ�м����ʶ�Ϊ��?

            XmlDocument dom = new XmlDocument();
            dom.LoadXml("<root />");
            for (int i = 0; i < this.Lines.Count; i++)
            {
                Line line = this.Lines[i];

                XmlNode node = dom.CreateElement("line");
                dom.DocumentElement.AppendChild(node);

                string strLogic = line.comboBox_logicOperator.Text;
                string strWord = line.textBox_word.Text;
                string strFrom = line.comboBox_from.Text;

                DomUtil.SetAttr(node, "logic", strLogic);
                DomUtil.SetAttr(node, "word", strWord);
                DomUtil.SetAttr(node, "from", strFrom);

                if (String.IsNullOrEmpty(strWord) == false)
                    bAllEmpty = false;
            }

            if (bOptimize == true
                && bAllEmpty == true)
                return null;

            return dom.OuterXml;
        }

        public void Clear()
        {
            for (int i = 0; i < this.Lines.Count; i++)
            {
                Line line = this.Lines[i];

                line.textBox_word.Text = "";

                // TODO: ��Ҫ���߼��������from�ָ���ȱʡ״̬
                // line.comboBox_from.Text = DomUtil.GetAttr(node, "from");
                // line.comboBox_logicOperator.Text = DomUtil.GetAttr(node, "logic");
            }
        }

        public void SetContent(string strUse, string strWord)
        {
            this.Clear();
            {
                Line line = this.Lines[0];

                // line.comboBox_logicOperator.Text = "";
                line.textBox_word.Text = strWord;
                SelectComboBoxValue(line.comboBox_from, strUse);
            }
        }

        static void SelectComboBoxValue(ComboBox combobox, string strValue)
        {
            strValue = strValue.ToLower();
            foreach(string s in combobox.Items)
            {
                string strLeft;
                string strRight;
                StringUtil.ParseTwoPart(s, "-", out strLeft, out strRight);
                strLeft = strLeft.Trim();
                strRight = strRight.Trim();
                if (strLeft.ToLower() == strValue)
                {
                    combobox.Text = s;
                    return;
                }
            }
        }

        // �� XML ����ʽ���õ��ؼ���
        // ���ܻ��׳��쳣
        public void SetContent(string strContentXml)
        {
            this.Clear();

            if (String.IsNullOrEmpty(strContentXml) == true)
            {
                return;
            }

            XmlDocument dom = new XmlDocument();
            dom.LoadXml(strContentXml);

            XmlNodeList nodes = dom.DocumentElement.SelectNodes("line");

            for (int i = 0; i < Math.Min(this.Lines.Count, nodes.Count); i++)
            {
                Line line = this.Lines[i];

                XmlNode node = nodes[i];

                line.comboBox_logicOperator.Text = DomUtil.GetAttr(node, "logic");
                line.textBox_word.Text = DomUtil.GetAttr(node, "word");
                line.comboBox_from.Text = DomUtil.GetAttr(node, "from");
            }
        }

        public override bool Focused
        {
            get
            {
                if (base.Focused == true)
                    return true;

                for (int i = 0; i < this.Lines.Count; i++)
                {
                    Line line = this.Lines[i];

                    if (line.textBox_word.Focused == true)
                        return true;
                    if (line.comboBox_from.Focused == true)
                        return true;
                    if (line.comboBox_logicOperator.Focused == true)
                        return true;
                }

                return false;
            }
        }
    }

    public class Line
    {
        public ComboBox comboBox_logicOperator = null;
        public TextBox textBox_word = null;
        public ComboBox comboBox_from = null;

        public Line(string [] fromlist)
        {
            comboBox_logicOperator  = new ComboBox();
            comboBox_logicOperator.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_logicOperator.FlatStyle = FlatStyle.Flat;
            comboBox_logicOperator.Dock = DockStyle.Fill;
            comboBox_logicOperator.MaximumSize = new Size(150, 28);
            comboBox_logicOperator.Size = new Size(80, 28);
            comboBox_logicOperator.MinimumSize = new Size(50, 28);
            comboBox_logicOperator.Items.AddRange(new object[] {
                "AND ��",
                "OR  ��",
                "NOT ��",
            });
            comboBox_logicOperator.Text = "AND ��";

            textBox_word = new TextBox();
            textBox_word.BorderStyle = BorderStyle.FixedSingle;
            textBox_word.Dock = DockStyle.Fill;

            comboBox_from = new ComboBox();
            comboBox_from.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_from.FlatStyle = FlatStyle.Flat;
            comboBox_from.DropDownHeight = 300;
            comboBox_from.DropDownWidth = 300;
            comboBox_from.Dock = DockStyle.Fill;
            comboBox_from.MaximumSize = new Size(200, 28);
            comboBox_from.Size = new Size(150, 28);
            comboBox_from.MinimumSize = new Size(100, 28);

            if (fromlist != null)
            {
                comboBox_from.Items.AddRange(fromlist);
                if (fromlist.Length > 0)
                    comboBox_from.Text = fromlist[0];
            }

        }

        public void AddToTable(TableLayoutPanel table, 
            int nRow)
        {
            table.Controls.Add(this.comboBox_logicOperator, 0, nRow);
            table.Controls.Add(this.textBox_word, 1, nRow);
            table.Controls.Add(this.comboBox_from, 2, nRow);

            if (nRow == 1)
            {
                this.comboBox_logicOperator.Enabled = false;
            }
        }


    }

    public class IsbnConvertInfo
    {
        public IsbnSplitter IsbnSplitter = null;
        public string ConvertStyle = "";    // force13 force10 addhyphen removehyphen wild

#if NO
        // return:
        //      -1  ����
        //      0   û�б�Ҫת��
        //      1   �Ѿ�ת��
        public int ConvertISBN(ref string strISBN,
            out string strError)
        {
            strError = "";

            if (string.IsNullOrEmpty(this.ConvertStyle) == true)
                return 0;

            bool bForce13 = StringUtil.IsInList("force13", this.ConvertStyle);
            bool bForce10 = StringUtil.IsInList("force10", this.ConvertStyle);
            bool bAddHyphen = StringUtil.IsInList("addhyphen", this.ConvertStyle);
            bool bRemoveHyphen = StringUtil.IsInList("removehyphen", this.ConvertStyle);
            int nRet = 0;

            string strStyle = "remainverifychar";
            if (bAddHyphen == true)
            {
                if (bForce10 == false && bForce13 == false)
                    strStyle += ",auto";
                else if (bForce13 == true)
                    strStyle += ",force13";
                else if (bForce10 == true)
                    strStyle += ",force10";
                else
                {
                    strError = "force10��force13��Ӧͬʱ�߱�";
                    return -1;
                }

                string strTarget = "";
                nRet = this.IsbnSplitter.IsbnInsertHyphen(
                   strISBN,
                   strStyle,
       out strTarget,
       out strError);
                if (nRet == -1)
                    return -1;
                strISBN = strTarget;
                return 1;
            }

            if (bRemoveHyphen == true)
            {
                if (bForce10 == false && bForce13 == false)
                {
                    strISBN = strISBN.Replace("-", "");
                    return 1;
                }
                else if (bForce13 == true)
                    strStyle += ",force13";
                else if (bForce10 == true)
                    strStyle += ",force10";
                else
                {
                    strError = "force10��force13��Ӧͬʱ�߱�";
                    return -1;
                }

                string strTarget = "";
                nRet = this.IsbnSplitter.IsbnInsertHyphen(
                   strISBN,
                   strStyle,
       out strTarget,
       out strError);
                if (nRet == -1)
                    return -1;
                strISBN = strTarget.Replace("-", "");
                return 1;
            }

            return 0;
        }
#endif
        // return:
        //      -1  ����
        //      0   û�б�Ҫת��
        //      1   �Ѿ�ת��
        public int ConvertISBN(string strISBN,
            out List<string> isbns,
            out string strError)
        {
            strError = "";
            isbns = new List<string>();

            if (string.IsNullOrEmpty(this.ConvertStyle) == true)
            {
                isbns.Add(strISBN);
                return 0;
            }

            bool bForce13 = StringUtil.IsInList("force13", this.ConvertStyle);
            bool bForce10 = StringUtil.IsInList("force10", this.ConvertStyle);
            bool bAddHyphen = StringUtil.IsInList("addhyphen", this.ConvertStyle);
            bool bRemoveHyphen = StringUtil.IsInList("removehyphen", this.ConvertStyle);
            bool bWildMatch = StringUtil.IsInList("wild", this.ConvertStyle);

            int nRet = 0;

            if (bWildMatch == true)
            {
                List<string> styles = new List<string>();
                styles.Add("remainverifychar,auto");
                styles.Add("remainverifychar,force13");
                styles.Add("remainverifychar,force10");

                foreach (string style in styles)
                {
                    string strTarget = "";
                    nRet = this.IsbnSplitter.IsbnInsertHyphen(
                        strISBN,
                        style,
                        out strTarget,
                        out strError);
                    if (nRet == -1)
                        continue;
                    isbns.Add(strTarget);
                }
                isbns.Add(strISBN); // ��ԭʼ��

                styles = new List<string>();
                styles.Add("remainverifychar,force13");
                styles.Add("remainverifychar,force10");

                foreach (string style in styles)
                {
                    string strTarget = "";
                    nRet = this.IsbnSplitter.IsbnInsertHyphen(
                        strISBN,
                        style,
                        out strTarget,
                        out strError);
                    if (nRet == -1)
                        continue;
                    isbns.Add(strTarget.Replace("-", ""));
                }
                isbns.Add(strISBN.Replace("-", ""));    // ��ԭʼ��ȥ�����ߵ�

                // TODO: �Ƿ�Ҫ����10λ13λȥ��У��λ�ģ�Ȼ��ָ��ǰ��һ�µ�?

                StringUtil.RemoveDupNoSort(ref isbns);
                return 1;
            }

            string strStyle = "remainverifychar";

            // ��� bAddHyphen �� bRemoveHyphen ��û�й�ѡ����ô��Ҫ���ַ������汾���Ƿ��к�ܣ��оͱ�����û��Ҳ��Ҫ����
            if (bAddHyphen == false && bRemoveHyphen == false
                && (bForce13 == true || bForce10 == true))
            {
                if (strISBN.IndexOf("-") == -1)
                    bRemoveHyphen = true;
                else
                    bAddHyphen = true;
            }

            if (bAddHyphen == true)
            {
                if (bForce10 == false && bForce13 == false)
                    strStyle += ",auto";
                else if (bForce13 == true)
                    strStyle += ",force13";
                else if (bForce10 == true)
                    strStyle += ",force10";
                else
                {
                    strError = "force10��force13��Ӧͬʱ�߱�";
                    return -1;
                }

                string strTarget = "";
                nRet = this.IsbnSplitter.IsbnInsertHyphen(
                   strISBN,
                   strStyle,
       out strTarget,
       out strError);
                if (nRet == -1)
                    return -1;
                isbns.Add(strTarget);
                return 1;
            }

            if (bRemoveHyphen == true)
            {
                if (bForce10 == false && bForce13 == false)
                {
                    strISBN = strISBN.Replace("-", "");
                    return 1;
                }
                else if (bForce13 == true)
                    strStyle += ",force13";
                else if (bForce10 == true)
                    strStyle += ",force10";
                else
                {
                    strError = "force10��force13��Ӧͬʱ�߱�";
                    return -1;
                }

                string strTarget = "";
                nRet = this.IsbnSplitter.IsbnInsertHyphen(
                   strISBN,
                   strStyle,
       out strTarget,
       out strError);
                if (nRet == -1)
                    return -1;
                isbns.Add(strTarget.Replace("-", ""));
                return 1;
            }

            return 0;
        }
    }
}
