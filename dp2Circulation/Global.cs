using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Web;

using Microsoft.Win32;

using DigitalPlatform;
using DigitalPlatform.Xml;
using DigitalPlatform.GUI;

using DigitalPlatform.IO;
using DigitalPlatform.Text;
using DigitalPlatform.Marc;
using DigitalPlatform.Drawing;
using DigitalPlatform.CirculationClient;
using DigitalPlatform.LibraryClient;
using DigitalPlatform.CommonControl;

namespace dp2Circulation
{
    /// <summary>
    /// ȫ�־�̬����
    /// </summary>
    public class Global
    {
        // ���Ӧ����ʱ���Ƿ񳬹���ǰʱ��
        // parameters:
        //      end Ӧ����ʱ�䡣GMT ʱ��
        //      now ��ǰʱ�䡣GMT ʱ��
        public static int IsOver(string strUnit,
            DateTime end,
            DateTime now,
            out string strError)
        {
            strError = "";
            int nRet = DateTimeUtil.RoundTime(strUnit, ref now, out strError);
            if (nRet == -1)
                return -1;

            if (now > end)
                return 1;   // ����
            return 0;   // û�г���
        }

        // �������Ƿ��Ѿ�����
        public static int IsOverdue(string strBorrowDate,
            string strPeriod,
            out string strError)
        {
            strError = "";

            DateTime now = DateTime.UtcNow;
            DateTime timeEnd = new DateTime(0);
            int nRet = GetReturnDay(
                strBorrowDate,
                strPeriod,
            out timeEnd,
            out strError);
            if (nRet == -1)
                return -1;

            string strPeriodUnit = "";
            long lPeriodValue = 0;

            nRet = DateTimeUtil.ParsePeriodUnit(strPeriod,
                "day",
                out lPeriodValue,
                out strPeriodUnit,
                out strError);
            if (nRet == -1)
            {
                strError = "��������ֵ '" + strPeriod + "' ��ʽ����: " + strError;
                return -1;
            }

            return IsOver(strPeriodUnit, timeEnd, now, out strError);
        }

        // ����Ӧ����ʱ��
        public static int GetReturnDay(
            string strBorrowDate,
            string strPeriod,
            out DateTime timeEnd,
            out string strError)
        {
            timeEnd = new DateTime(0);
            string strPeriodUnit = "";
            long lPeriodValue = 0;

            int nRet = DateTimeUtil.ParsePeriodUnit(strPeriod,
                "day",
                out lPeriodValue,
                out strPeriodUnit,
                out strError);
            if (nRet == -1)
            {
                strError = "��������ֵ '" + strPeriod + "' ��ʽ����: " + strError;
                return -1;
            }

            DateTime borrowdate = new DateTime((long)0);

            try
            {
                borrowdate = DateTimeUtil.FromRfc1123DateTimeString(strBorrowDate);
            }
            catch
            {
                strError = "���������ַ��� '" + strBorrowDate + "' ��ʽ����";
                return -1;
            }

            return GetReturnDay(
                borrowdate,
                lPeriodValue,
                strPeriodUnit,
                out timeEnd,
                out strError);
        }

        // ����Ӧ��������
        // �������ۻ���ʱ�䣬�����ǿ��������
        // parameters:
        //      timeStart   ���Ŀ�ʼʱ�䡣GMTʱ��
        //      timeEnd     ����Ӧ���ص����ʱ�䡣GMTʱ��
        // return:
        //      -1  ����
        //      0   �ɹ�
        public static int GetReturnDay(
            DateTime timeStart,
            long lPeriod,
            string strUnit,
            out DateTime timeEnd,
            out string strError)
        {
            strError = "";
            timeEnd = DateTime.MinValue;

            // ���滯ʱ��
            int nRet = DateTimeUtil.RoundTime(strUnit,
                ref timeStart,
                out strError);
            if (nRet == -1)
                return -1;

            TimeSpan delta;

            if (strUnit == "day")
                delta = new TimeSpan((int)lPeriod, 0, 0, 0);
            else if (strUnit == "hour")
                delta = new TimeSpan((int)lPeriod, 0, 0);
            else
            {
                strError = "δ֪��ʱ�䵥λ '" + strUnit + "'";
                return -1;
            }

            timeEnd = timeStart + delta;

            // ���滯ʱ��
            nRet = DateTimeUtil.RoundTime(strUnit,
                ref timeEnd,
                out strError);
            if (nRet == -1)
                return -1;

            return 0;
        }

        // parameters:
        //      strName ���磬"KB2544514"
        public static bool IsKbInstalled(string strName)
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Updates\Microsoft .NET Framework 4 Extended\" + strName))
                {
                    return (string)baseKey.GetValue("ThisVersionInstalled") == "Y";
                }
            }
            catch
            {
                return false;
            }
        }

        // �ڵ�һ��ǰ�����һ���հ���
        public static string[] InsertBlankColumn(string[] cols,
            int nDelta = 1)
        {
            string[] results = new string[cols == null ? nDelta : cols.Length + nDelta];
            for (int i = 0; i < nDelta; i++)
            {
                results[i] = "";
            }
            if (results.Length > 1)
                Array.Copy(cols, 0, results, nDelta, results.Length - nDelta);
            return results;
        }

        public static Control FindFocusedControl(Control control)
        {
            var container = control as IContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as IContainerControl;
            }
            return control;
        }

        /// <summary>
        /// Activate һ�� Form�������˴�����С��ʱ��ָ���ʾ�Ĺ���
        /// </summary>
        /// <param name="form">Form</param>
        public static void Activate(Form form)
        {
            if (form != null)
            {
                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;
                form.Activate();
            }
        }

        /// <summary>
        /// �ж�һ�������ַ����Ƿ�Ϊ�������������
        /// </summary>
        /// <param name="strFontString">�����ַ���������������������壬��������ʱ���Ѿ����޸�Ϊ�������������������</param>
        /// <returns>�Ƿ�Ϊ������������塣true ��ʾ��</returns>
        public static bool IsVirtualBarcodeFont(ref string strFontString)
        {
            if (string.IsNullOrEmpty(strFontString) == true)
                return false;

            string strFontName = "";
            string strOther = "";
            StringUtil.ParseTwoPart(strFontString,
                ",",
                out strFontName,
                out strOther);
            if (strFontName == "barcode")
            {
                strFontString = "C39HrP24DhTt," + strOther;
                return true;
            }

            return false;
        }

        public static string GetBarcodeFontString(Font font)
        {
            if (font == null)
                return "";

            return "barcode, " + font.SizeInPoints.ToString() + "pt";
        }

        /// <summary>
        /// �����ַ������幹�� Font ����
        /// �������������� C39HrP24DhTt
        /// </summary>
        /// <param name="strFontString"></param>
        /// <returns></returns>
        public static Font BuildFont(string strFontString)
        {
            if (String.IsNullOrEmpty(strFontString) == true)
                return Control.DefaultFont;

            // Create the FontConverter.
            System.ComponentModel.TypeConverter converter =
                System.ComponentModel.TypeDescriptor.GetConverter(typeof(Font));

            Font font = (Font)converter.ConvertFromString(strFontString);

            // GDI+ �°�װ����������ֵ� BUG �ƹ�����
            if (string.IsNullOrEmpty(font.OriginalFontName) == false
                && font.OriginalFontName != font.Name)
            {
                List<FontFamily> families = new List<FontFamily>(GlobalVars.PrivateFonts.Families);

                FontFamily t = families.Find(f => string.Compare(f.Name, font.OriginalFontName, true) == 0);

                // if (families.Exists(f => string.Compare(f.Name, font.OriginalFontName, true) == 0) == true)
                if (t != null)
                {
                    Font new_font = new Font(t, font.Size, font.Style);
                    font.Dispose(); // 2017/2/27
                    return new_font;
                }
            }

            return font;
        }

        // ���Զ��� PrivateFonts ��Ѱ��
        public static Font BuildFont(string font_name, float height, GraphicsUnit unit)
        {
            Font font = new Font(
               font_name,    // "OCR-B 10 BT", 
               height, unit);
            if (string.IsNullOrEmpty(font.OriginalFontName) == false
    && font.OriginalFontName != font.Name)
            {
                List<FontFamily> families = new List<FontFamily>(GlobalVars.PrivateFonts.Families);

                FontFamily t = families.Find(f => string.Compare(f.Name, font.OriginalFontName, true) == 0);
                if (t != null)
                {
                    font.Dispose();
                    // return new Font(t, font.Size, font.Style, unit);    // unit 2017/10/19
                    return new Font(t, height, unit);   // �޸ĳ������۲�һ��Ч�� 2017/10/19
                }
            }
            return font;
        }

        /// <summary>
        /// ����һ���ı��༭��ĳ�е�����
        /// </summary>
        /// <param name="textbox">�ı��༭��</param>
        /// <param name="nLine">�� index</param>
        /// <param name="strValue">Ҫ���õ�����</param>
        public static void SetLineText(TextBox textbox,
    int nLine,
    string strValue)
        {
            string strText = textbox.Text.Replace("\r\n", "\r");
            string[] lines = strText.Split(new char[] { '\r' });

            strText = "";
            for (int i = 0; i < Math.Max(nLine, lines.Length); i++)
            {
                if (i != 0)
                    strText += "\r\n";

                if (i == nLine)
                    strText += strValue;
                else
                {
                    if (i < lines.Length)
                        strText += lines[i];
                    else
                        strText += "";
                }

            }

            textbox.Text = strText;
        }

        // ���û���ˢ��һ����������
        internal static int SetOperation(
            ref XmlDocument dom,
            string strOperName,
            string strOperator,
            string strComment,
            bool bAppend,
            out string strError)
        {
            strError = "";

            if (dom.DocumentElement == null)
            {
                strError = "dom.DocumentElement == null";
                return -1;
            }

            XmlNode nodeOperations = dom.DocumentElement.SelectSingleNode("operations");
            if (nodeOperations == null)
            {
                nodeOperations = dom.CreateElement("operations");
                dom.DocumentElement.AppendChild(nodeOperations);
            }

            XmlNodeList nodes = nodeOperations.SelectNodes("operation[@name='" + strOperName + "']");
            if (bAppend == true)
            {
                // ɾ������9����
                if (nodes.Count > 9)
                {
                    for (int i = 0; i < nodes.Count - 9; i++)
                    {
                        XmlNode node = nodes[i];
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }
            else
            {
                if (nodes.Count > 1)
                {
                    for (int i = 0; i < nodes.Count - 1; i++)
                    {
                        XmlNode node = nodes[i];
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }

            {
                XmlNode node = null;
                if (bAppend == true)
                {
                }
                else
                {
                    node = nodeOperations.SelectSingleNode("operation[@name='" + strOperName + "']");
                }


                if (node == null)
                {
                    node = dom.CreateElement("operation");
                    nodeOperations.AppendChild(node);
                    DomUtil.SetAttr(node, "name", strOperName);
                }


                string strTime = DateTimeUtil.Rfc1123DateTimeString(DateTime.UtcNow);// app.Clock.GetClock();

                DomUtil.SetAttr(node, "time", strTime);
                DomUtil.SetAttr(node, "operator", strOperator);
                if (String.IsNullOrEmpty(strComment) == false)
                    DomUtil.SetAttr(node, "comment", strComment);
            }

            return 0;
        }

        /// <summary>
        /// �޸�һ��״̬�ַ���
        /// </summary>
        /// <param name="strState">Ҫ�޸ĵ��ַ���</param>
        /// <param name="strAddList">Ҫ�����ֵ�б�</param>
        /// <param name="strRemoveList">Ҫ�Ƴ���ֵ�б�</param>
        public static void ModifyStateString(ref string strState,
            string strAddList,
            string strRemoveList)
        {
            string[] adds = strAddList.Split(new char[] { ',' });
            for (int i = 0; i < adds.Length; i++)
            {
                StringUtil.SetInList(ref strState, adds[i], true);
            }
            string[] removes = strRemoveList.Split(new char[] { ',' });
            for (int i = 0; i < removes.Length; i++)
            {
                StringUtil.SetInList(ref strState, removes[i], false);
            }
        }

        /// <summary>
        /// ��� MARC ��¼�е�ȫ����Ŀ�����ַ���
        /// </summary>
        /// <param name="strMARC">MARC�ַ��������ڸ�ʽ</param>
        /// <returns>�ַ�������</returns>
        public static List<string> GetExistCatalogingRules(string strMARC)
        {
            int nRet = 0;
            List<string> results = new List<string>();
            MarcRecord record = new MarcRecord(strMARC);

            MarcNodeList subfields = record.select("field/subfield");
            foreach (MarcSubfield subfield in subfields)
            {
                if (subfield.Name == "*")
                    results.Add(subfield.Content.Trim());
                else
                {
                    string strCmd = StringUtil.GetLeadingCommand(subfield.Content);
                    if (string.IsNullOrEmpty(strCmd) == false
                        && StringUtil.HasHead(strCmd, "cr:") == true)
                    {
                        results.Add(strCmd.Substring(3));
                    }
                }
            }

            foreach (MarcField field in record.ChildNodes)
            {
                string strField = field.Text;

                if (string.IsNullOrEmpty(strField) == true)
                    continue;
                if (strField.Length < 3)
                    continue;

                {
#if NO
                    // �ֶ�����(�ֶ�ָʾ����)�͵�һ�����ֶη���֮��Ŀհ�Ƭ��
                    string strIndicator = "";
                    string strContent = "";
                    if (MarcUtil.IsControlFieldName(strField.Substring(0, 3)) == true)
                    {
                        strContent = strField.Substring(3);
                    }
                    else
                    {
                        if (strField.Length >= 5)
                        {
                            strIndicator = strField.Substring(3, 2);
                            strContent = strField.Substring(3 + 2);
                        }
                        else
                            strIndicator = strField.Substring(3, 1);
                    }
#endif

                    string strBlank = field.Content;   // .Trim();
                    nRet = strBlank.IndexOf((char)MarcUtil.SUBFLD);
                    if (nRet != -1)
                        strBlank = strBlank.Substring(0, nRet); // .Trim();

                    string strCmd = StringUtil.GetLeadingCommand(strBlank);
                    if (string.IsNullOrEmpty(strCmd) == false
                        && StringUtil.HasHead(strCmd, "cr:") == true)
                    {
                        results.Add(strCmd.Substring(3));
                    }
                }
            }
            results.Sort();
            StringUtil.RemoveDup(ref results);
            return results;
        }

#if NO
        /// <summary>
        /// ��� MARC ��¼�е�ȫ����Ŀ�����ַ���
        /// </summary>
        /// <param name="strMARC">MARC�ַ��������ڸ�ʽ</param>
        /// <returns>�ַ�������</returns>
        public static List<string> GetExistCatalogingRules(string strMARC)
        {
            int nRet = 0;
            List<string> results = new List<string>();

            for (int i = 1; ; i++)
            {
                string strField = "";
                string strNextFieldName = "";
                // return:
                //		-1	����
                //		0	��ָ�����ֶ�û���ҵ�
                //		1	�ҵ����ҵ����ֶη�����strField������
                nRet = MarcUtil.GetField(strMARC,
                    null,
                    i,
                    out strField,
                    out strNextFieldName);
                if (nRet != 1)
                    break;

                if (string.IsNullOrEmpty(strField) == true)
                    continue;
                if (strField.Length < 3)
                    continue;

                {
#if NO
                    string strFieldName = strField.Substring(0, 3);
#endif

                    // �ֶ�����(�ֶ�ָʾ����)�͵�һ�����ֶη���֮��Ŀհ�Ƭ��
                    string strIndicator = "";
                    string strContent = "";
                    if (MarcUtil.IsControlFieldName(strField.Substring(0, 3)) == true)
                    {
                        strContent = strField.Substring(3);
                    }
                    else
                    {
                        if (strField.Length >= 5)
                        {
                            strIndicator = strField.Substring(3, 2);
                            strContent = strField.Substring(3 + 2);
                        }
                        else
                            strIndicator = strField.Substring(3, 1);
                    }

                    string strBlank = strContent;   // .Trim();
                    nRet = strBlank.IndexOf((char)MarcUtil.SUBFLD);
                    if (nRet != -1)
                        strBlank = strBlank.Substring(0, nRet); // .Trim();

                    string strCmd = StringUtil.GetLeadingCommand(strBlank);
                    if (string.IsNullOrEmpty(strCmd) == false
                        && StringUtil.HasHead(strCmd, "cr:") == true)
                    {
                        results.Add(strCmd.Substring(3));
                    }

#if NO
                    // ���滹��Ҫ����������strField��ȥ���� {...} һ��
                    if (string.IsNullOrEmpty(strCmd) == false)
                    {
                        strContent = strContent.Substring(strCmd.Length + 2);
                        strField = strFieldName + strIndicator + strContent;
                    }
#endif
                }

                // 2012/11/6
                // �۲� $* ���ֶ�
                {
                    //
                    string strSubfield = "";
                    string strNextSubfieldName1 = "";
                    // return:
                    //		-1	����
                    //		0	��ָ�������ֶ�û���ҵ�
                    //		1	�ҵ����ҵ������ֶη�����strSubfield������
                    nRet = MarcUtil.GetSubfield(strField,
                        ItemType.Field,
                        "*",    // "*",
                        0,
                        out strSubfield,
                        out strNextSubfieldName1);
                    if (nRet == 1)
                    {
                        string strCurRule = strSubfield.Substring(1);
                        if (string.IsNullOrEmpty(strCurRule) == false)
                            results.Add(strCurRule);
                    }
                }

                for (int j = 0; ; j++)
                {
                    string strSubfield = "";
                    string strNextSubfieldName = "";
                    // return:
                    //		-1	error
                    //		0	not found
                    //		1	found
                    nRet = MarcUtil.GetSubfield(strField,
                        ItemType.Field,
                        null,
                        j,
                        out strSubfield,
                        out strNextSubfieldName);
                    if (nRet != 1)
                        break;
                    if (strSubfield.Length <= 1)
                        continue;

                    string strSubfieldName = strSubfield.Substring(0, 1);
                    string strContent = strSubfield.Substring(1);

                    if (strSubfieldName == "*")
                        results.Add(strContent);
                    else
                    {
                        string strCmd = StringUtil.GetLeadingCommand(strContent);

                        if (string.IsNullOrEmpty(strCmd) == false
                            && StringUtil.HasHead(strCmd, "cr:") == true)
                            results.Add(strCmd.Substring(3));
                    }
                }
            }
            results.Sort();
            StringUtil.RemoveDup(ref results);
            return results;
        }

#endif

        #region ˢ���б�

        delegate void Delegate_filterValue(Control control);

        // ����ȫ�汾
        // ���˵� {} ��Χ�Ĳ���
        static void __FilterValue(Control control)
        {
            string strText = StringUtil.GetPureSelectedValue(control.Text);
            if (control.Text != strText)
                control.Text = strText;
        }

#if NO
        // ��ȫ�汾
        public static void FilterValue(Control owner, 
            Control control)
        {
            if (owner.InvokeRequired == true)
            {
                Delegate_filterValue d = new Delegate_filterValue(__FilterValue);
                owner.BeginInvoke(d, new object[] { control });
            }
            else
            {
                __FilterValue((Control)control);
            }
        }
#endif
        // ��ȫ�汾
        /// <summary>
        /// ���˿ؼ��е��ı�ֵ
        /// </summary>
        /// <param name="owner">�ؼ��������ؼ�</param>
        /// <param name="control">�ؼ�</param>
        public static void FilterValue(Control owner,
            Control control)
        {
            Delegate_filterValue d = new Delegate_filterValue(__FilterValue);

            if (owner.Created == false)
                __FilterValue((Control)control);
            else
                owner.BeginInvoke(d, new object[] { control });
        }

        // ����ȫ�汾
        // ���˵� {} ��Χ�Ĳ���
        // �����б�ֵȥ�صĹ���
        static void __FilterValueList(Control control)
        {
            List<string> results = StringUtil.FromListString(StringUtil.GetPureSelectedValue(control.Text));
            StringUtil.RemoveDupNoSort(ref results);
            string strText = StringUtil.MakePathList(results);
            if (control.Text != strText)
                control.Text = strText;
        }

#if NO
        // ��ȫ�汾
        public static void FilterValueList(Control owner, Control control)
        {
            if (owner.InvokeRequired == true)
            {
                Delegate_filterValue d = new Delegate_filterValue(__FilterValueList);
                owner.BeginInvoke(d, new object[] { control });
            }
            else
            {
                __FilterValueList((Control)control);
            }
        }
#endif
        // ��ȫ�汾
        /// <summary>
        /// ���˿ؼ��е��б�ֵ
        /// </summary>
        /// <param name="owner">�ؼ��������ؼ�</param>
        /// <param name="control">�ؼ�</param>
        public static void FilterValueList(Control owner, Control control)
        {

            Delegate_filterValue d = new Delegate_filterValue(__FilterValueList);

            if (owner.Created == false)
                __FilterValueList((Control)control);
            else
                owner.BeginInvoke(d, new object[] { control });

        }

        #endregion

        // 
        // return:
        //      -1  �����������������Ȼ�����
        //      0   �ɹ�
        /// <summary>
        /// ɾ��һ��Ŀ¼
        /// </summary>
        /// <param name="owner">�������� MessageBox Ҫ�õ��Ĵ���</param>
        /// <param name="strDataDir">Ҫɾ����Ŀ¼</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����������������; 0: �ɹ�</returns>
        public static int DeleteDataDir(
            IWin32Window owner,
            string strDataDir,
            out string strError)
        {
            strError = "";
        REDO_DELETE_DATADIR:
            try
            {
                Directory.Delete(strDataDir, true);
                return 0;
            }
            catch (Exception ex)
            {
                strError = "ɾ��Ŀ¼ '" + strDataDir + "' ʱ����: " + ex.Message;
            }

            if (owner != null)
            {
                DialogResult temp_result = MessageBox.Show(owner,
        strError + "\r\n\r\n�Ƿ�����?",
        "ɾ��Ŀ¼ '" + strDataDir + "'",
        MessageBoxButtons.RetryCancel,
        MessageBoxIcon.Question,
        MessageBoxDefaultButton.Button1);
                if (temp_result == DialogResult.Retry)
                    goto REDO_DELETE_DATADIR;
            }

            return -1;
        }

        // 
        /// <summary>
        /// ����ݴ����ַ������ϡ����ַ���Ҳ��������
        /// </summary>
        /// <param name="strList">�б��ַ��������ŷָ��Ķ���ݴ���</param>
        /// <returns>�ַ�������</returns>
        public static List<string> FromLibraryCodeList(string strList)
        {
            List<string> results = new List<string>();
            string[] parts = strList.Split(new char[] { ',' });
            foreach (string s in parts)
            {
                string strText = s.Trim();
                results.Add(strText);
            }

            return results;
        }
#if NO
        // ���һ���ݲط����ַ�����������йݴ���
        public static int GetDistributeLibraryCodes(string strDistribute,
            out List<string> library_codes,
            out string strError)
        {
            strError = "";
            library_codes = new List<string>();

            LocationColletion locations = new LocationColletion();
            int nRet = locations.Build(strDistribute,
                out strError);
            if (nRet == -1)
                return -1;

            foreach (Location location in locations)
            {
                if (string.IsNullOrEmpty(location.Name) == true)
                    continue;



                string[] parts = location.RefID.Split(new char[] { '|' });
                foreach (string text in parts)
                {
                    string strRefID = text.Trim();
                    if (string.IsNullOrEmpty(strRefID) == true)
                        continue;
                    library_codes.Add(strRefID);
                }
            }

            return 0;
        }
#endif

        // 
        /// <summary>
        /// ���һ���ݲط����ַ������������ �ο� ID
        /// </summary>
        /// <param name="strDistribute">�ݲط����ַ���</param>
        /// <param name="refids">���زο� ID �ַ�������</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����; 0: �ɹ�</returns>
        public static int GetRefIDs(string strDistribute,
            out List<string> refids,
            out string strError)
        {
            strError = "";
            refids = new List<string>();

            LocationCollection locations = new LocationCollection();
            int nRet = locations.Build(strDistribute,
                out strError);
            if (nRet == -1)
                return -1;

            foreach (Location location in locations)
            {
                if (string.IsNullOrEmpty(location.RefID) == true)
                    continue;

                string[] parts = location.RefID.Split(new char[] { '|' });
                foreach (string text in parts)
                {
                    string strRefID = text.Trim();
                    if (string.IsNullOrEmpty(strRefID) == true)
                        continue;
                    refids.Add(strRefID);
                }
            }

            return 0;
        }

        /// <summary>
        /// ����һ���ݴ����б��ַ������ж�����ַ����Ƿ������ȫ���û�
        /// </summary>
        /// <param name="strLibraryCodeList">�ݴ����б��ַ���</param>
        /// <returns>�Ƿ�</returns>
        public static bool IsGlobalUser(string strLibraryCodeList)
        {
            if (strLibraryCodeList == "*" || string.IsNullOrEmpty(strLibraryCodeList) == true)
                return true;
            return false;
        }

        // 
        // return:
        //      -1  ����
        //      0   ������Ͻ��Χ��strError���н���
        //      1   �ڹ�Ͻ��Χ��
        /// <summary>
        /// �۲�һ���ݲط����ַ����������Ƿ���ȫ�ڵ�ǰ�û���Ͻ��Χ��
        /// </summary>
        /// <param name="strDistribute">�ݲط����ַ���</param>
        /// <param name="strLibraryCodeList">��ǰ�û��Ĺݴ����б��ַ���</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����; 0: ������Ͻ��Χ��strError ���н���; 1: �ڹ�Ͻ��Χ��</returns>
        public static int DistributeInControlled(string strDistribute,
            string strLibraryCodeList,
            out string strError)
        {
            strError = "";

            if (IsGlobalUser(strLibraryCodeList) == true)
                return 1;

            LocationCollection locations = new LocationCollection();
            int nRet = locations.Build(strDistribute, out strError);
            if (nRet == -1)
            {
                strError = "�ݲط����ַ��� '" + strDistribute + "' ��ʽ����ȷ";
                return -1;
            }

            foreach (Location location in locations)
            {
                // �յĹݲصص㱻��Ϊ���ڷֹ��û���Ͻ��Χ��
                if (string.IsNullOrEmpty(location.Name) == true)
                {
                    strError = "�ݴ��� '' ���ڷ�Χ '" + strLibraryCodeList + "' ��";
                    return 0;
                }

                string strLibraryCode = "";
                string strPureName = "";

                // ����
                ParseCalendarName(location.Name,
            out strLibraryCode,
            out strPureName);

                if (StringUtil.IsInList(strLibraryCode, strLibraryCodeList) == false)
                {
                    strError = "�ݴ��� '" + strLibraryCode + "' ���ڷ�Χ '" + strLibraryCodeList + "' ��";
                    return 0;
                }
            }

            return 1;
        }

        // 
        // return:
        //      -1  ����
        //      0   û���κβ����ڹ�Ͻ��Χ
        //      1   ���ٲ����ڹ�Ͻ��Χ��
        /// <summary>
        /// �۲�һ���ݲط����ַ����������Ƿ񲿷��ڵ�ǰ�û���Ͻ��Χ��
        /// </summary>
        /// <param name="strDistribute">�ݲط����ַ���</param>
        /// <param name="strLibraryCodeList">��ǰ�û��Ĺݴ����б��ַ���</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����; 0: û���κβ����ڹ�Ͻ��Χ; 1: ���ٲ����ڹ�Ͻ��Χ��</returns>
        public static int DistributeCross(string strDistribute,
            string strLibraryCodeList,
            out string strError)
        {
            strError = "";

            if (IsGlobalUser(strLibraryCodeList) == true)
                return 1;

            LocationCollection locations = new LocationCollection();
            int nRet = locations.Build(strDistribute, out strError);
            if (nRet == -1)
            {
                strError = "�ݲط����ַ��� '" + strDistribute + "' ��ʽ����ȷ";
                return -1;
            }

            foreach (Location location in locations)
            {
                // �յĹݲصص㱻��Ϊ���ڷֹ��û���Ͻ��Χ��
                if (string.IsNullOrEmpty(location.Name) == true)
                    continue;

                string strLibraryCode = "";
                string strPureName = "";

                // ����
                ParseCalendarName(location.Name,
            out strLibraryCode,
            out strPureName);

                if (StringUtil.IsInList(strLibraryCode, strLibraryCodeList) == true)
                    return 1;
            }

            return 0;
        }

        // ������ǰ�Ľű����µĽű�����Ҫʹ��StringUtil.GetPureLocation()
        /// <summary>
        /// ��ô����Ĺݲصص��ַ�����ȥ�������е� #reservation ����
        /// �뷽��Ϊ�˼�����ǰ�Ľű����µĽű�����Ҫʹ��StringUtil.GetPureLocation()
        /// </summary>
        /// <param name="strLocation">���ӹ��Ĺݲصص��ַ���</param>
        /// <returns>�ӹ���Ĺݲصص��ַ���</returns>
        public static string GetPureLocation(string strLocation)
        {
            return StringUtil.GetPureLocation(strLocation);
        }

        /// <summary>
        /// ��ùݴ����б��еĵ�һ���ݴ���
        /// </summary>
        /// <param name="strLibraryCodeList">�ܴ����б����ŷָ����ַ����б�</param>
        /// <returns>��һ���ݴ���</returns>
        public static string GetFirstLibraryCode(string strLibraryCodeList)
        {
            if (string.IsNullOrEmpty(strLibraryCodeList) == true)
                return "";

            List<string> librarycodes = StringUtil.SplitList(strLibraryCodeList);

            if (librarycodes.Count > 0)
                return librarycodes[0];

            return "";
        }

        /// <summary>
        /// ��һ���ݲصص��ַ����н������ݴ��벿�֡����� "����ֹ�/������" ������ "����ֹ�"
        /// </summary>
        /// <param name="strLocationString">�ݲصص��ַ���</param>
        /// <returns>���عݴ���</returns>
        public static string GetLibraryCode(string strLocationString)
        {
            string strLibraryCode = "";
            string strPureName = "";

            // ����
            ParseCalendarName(strLocationString,
        out strLibraryCode,
        out strPureName);

            return strLibraryCode;
        }

        // 2016/5/5
        public static string GetLocationRoom(string strLocationString)
        {
            string strLibraryCode = "";
            string strPureName = "";

            // ����
            ParseCalendarName(strLocationString,
        out strLibraryCode,
        out strPureName);

            return strPureName;
        }

        /// <summary>
        /// �ϳ�������
        /// </summary>
        /// <param name="strLibraryCode">�ݴ���</param>
        /// <param name="strPureName">������������</param>
        /// <returns></returns>
        public static string BuildCalendarName(string strLibraryCode,
            string strPureName)
        {
            if (string.IsNullOrEmpty(strLibraryCode) == true)
                return strPureName;
            else
                return strLibraryCode + "/" + strPureName;
        }

        // 
        /// <summary>
        /// ���������������� "����ֹ�/��������"
        /// </summary>
        /// <param name="strName">������������</param>
        /// <param name="strLibraryCode">���عݴ��벿��</param>
        /// <param name="strPureName">���ش�������������</param>
        public static void ParseCalendarName(string strName,
            out string strLibraryCode,
            out string strPureName)
        {
            strLibraryCode = "";
            strPureName = "";
            int nRet = strName.IndexOf("/");
            if (nRet == -1)
            {
                strPureName = strName;
                return;
            }
            strLibraryCode = strName.Substring(0, nRet).Trim();
            strPureName = strName.Substring(nRet + 1).Trim();
        }

        /// <summary>
        /// ���˳����Ϲݴ����б����Щ�ݲص��ַ���
        /// </summary>
        /// <param name="strLibraryCodeList">�ݴ����б��ַ���</param>
        /// <param name="values">Ҫ���й��˵��ַ�������</param>
        /// <returns>���˺�õ����ַ�������</returns>
        public static List<string> FilterLocationsWithLibraryCodeList(string strLibraryCodeList,
    List<string> values)
        {
            List<string> results = new List<string>();
            foreach (string v in values)
            {
                string strLibraryCode = "";
                string strPureValue = "";

                // ����һ���ݲصص��ַ���
                // ����ֹ�/��ʦ
                ParseCalendarName(v,
                    out strLibraryCode,
                    out strPureValue);

                if (StringUtil.IsInList(strLibraryCode, strLibraryCodeList) == true)
                    results.Add(v);
            }

            return results;
        }

#if NO
        // ���˳����Ϲݴ����б����Щֵ�ַ���
        public static List<string> FilterValuesWithLibraryCodeList(string strLibraryCodeList,
    List<string> values)
        {
            List<string> results = new List<string>();
            foreach (string v in values)
            {
                string strCode = "";
                string strPureValue = "";

                // ����һ������dp2library���б�ֵ�ַ���
                // {����ֹ�} ��ʦ
                ParseValueString(v,
                    out strCode,
                    out strPureValue);

                if (StringUtil.IsInList(strCode, strLibraryCodeList) == true)
                    results.Add(v);
            }

            return results;
        }
#endif

        // 
        /// <summary>
        /// ���˳����Ϲݴ������Щֵ�ַ���
        /// ֵ�ַ����ĸ�ʽΪ��{����ֹ�} ��ʦ
        /// </summary>
        /// <param name="strLibraryCode">�ݴ����б��ַ���</param>
        /// <param name="values">Ҫ���й��˵�ֵ�ַ�������</param>
        /// <returns>���˺�õ���ֵ�ַ�������</returns>
        public static List<string> FilterValuesWithLibraryCode(string strLibraryCode,
            List<string> values)
        {
            List<string> results = new List<string>();
            foreach (string v in values)
            {
                string strCode = "";
                string strPureValue = "";

                // ����һ������dp2library���б�ֵ�ַ���
                // {����ֹ�} ��ʦ
                ParseValueString(v,
                    out strCode,
                    out strPureValue);

                if (strCode == strLibraryCode)
                    results.Add(v);
            }

            return results;
        }

        // ����һ������dp2library���б�ֵ�ַ���
        // {����ֹ�} ��ʦ
        /// <summary>
        /// ����һ��ֵ�ַ���
        /// ֵ�ַ����ĸ�ʽ��{����ֹ�} ��ʦ
        /// </summary>
        /// <param name="strText">���������ַ���</param>
        /// <param name="strLibraryCode">���عݴ��벿��</param>
        /// <param name="strPureValue">���ش����ֵ����</param>
        public static void ParseValueString(string strText,
            out string strLibraryCode,
            out string strPureValue)
        {
            strLibraryCode = "";
            strPureValue = "";

            int nRet = strText.IndexOf("{");
            if (nRet == -1)
            {
                strPureValue = strText;
                return;
            }
            int nStart = nRet;
            nRet = strText.IndexOf("}", nStart + 1);
            if (nRet == -1)
            {
                strPureValue = strText;
                return;
            }
            int nEnd = nRet;

            strLibraryCode = strText.Substring(nStart + 1, nEnd - nStart - 1).Trim();
            strPureValue = strText.Remove(nStart, nEnd - nStart + 1).Trim();
        }


        // 
        // return:
        //      -1  error
        //      0   succeed
        /// <summary>
        /// ��� Encoding ���󡣱�������֧��MARC-8������
        /// </summary>
        /// <param name="strName">�������ơ������Ǵ���ҳ������ʽ</param>
        /// <param name="encoding">Encoding ����</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����; 0: �ɹ�</returns>
        public static int GetEncoding(string strName,
            out Encoding encoding,
            out string strError)
        {
            strError = "";
            encoding = null;

            try
            {

                if (StringUtil.IsNumber(strName) == true)
                {
                    try
                    {
                        Int32 nCodePage = Convert.ToInt32(strName);
                        encoding = Encoding.GetEncoding(nCodePage);
                    }
                    catch (Exception ex)
                    {
                        strError = "������뷽ʽ���̳���: " + ex.Message;
                        return -1;
                    }
                }
                else
                {
                    encoding = Encoding.GetEncoding(strName);
                }
            }
            catch (Exception ex)
            {
                strError = "GetEncoding() exception: " + ExceptionUtil.GetAutoText(ex);
                return -1;
            }

            return 0;
        }

        // 
        /// <summary>
        /// ���һ���������Ϣ��
        /// ע�⣬���������ܴ��������Marc8Encoding��
        /// </summary>
        /// <param name="encoding">Encoding ����</param>
        /// <returns>EncodingInfo �������û���ҵ����򷵻� null</returns>
        static EncodingInfo GetEncodingInfo(Encoding encoding)
        {
            EncodingInfo[] infos = Encoding.GetEncodings();
            for (int i = 0; i < infos.Length; i++)
            {
                if (encoding.Equals(infos[i].GetEncoding()))
                    return infos[i];
            }

            return null;    // not found
        }

        // 
        /// <summary>
        /// ���encoding����ʽ���֡�����������ʶ��Marc8Encoding��
        /// </summary>
        /// <param name="encoding">Encoding ����</param>
        /// <returns>��ʽ����</returns>
        public static string GetEncodingName(Encoding encoding)
        {
            EncodingInfo info = GetEncodingInfo(encoding);
            if (info != null)
            {
                return info.Name;
            }
            else
            {
                return "Unknown encoding";
            }
        }

        // �г�encoding���б�
        // ��Ҫ��gb2312 utf-8�ȳ��õ���ǰ
        /// <summary>
        /// ���ȫ�����õı������б�
        /// </summary>
        /// <param name="bHasMarc8">�Ƿ���� MARC-8</param>
        /// <returns>�ַ�������</returns>
        public static List<string> GetEncodingList(bool bHasMarc8)
        {
            List<string> result = new List<string>();

            EncodingInfo[] infos = Encoding.GetEncodings();
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].GetEncoding().Equals(Encoding.GetEncoding(936)) == true)
                    result.Insert(0, infos[i].Name);
                else if (infos[i].GetEncoding().Equals(Encoding.UTF8) == true)
                    result.Insert(0, infos[i].Name);
                else
                    result.Add(infos[i].Name);
            }

            if (bHasMarc8 == true)
                result.Add("MARC-8");

            return result;
        }

        #region ������غ���

        // ����֮���Ƿ�Ϊ�����Ĺ�ϵ
        static bool IsNextNo(string strPrevNo,
            string strNextNo)
        {
            int nPrevNo = 0;
            try
            {
                nPrevNo = Convert.ToInt32(strPrevNo);
            }
            catch
            {
                return false;
            }
            int nNextNo = 0;
            try
            {
                nNextNo = Convert.ToInt32(strNextNo);
            }
            catch
            {
                return false;
            }

            if (nPrevNo + 1 == nNextNo)
                return true;

            return false;
        }

        // return:
        //      -1  ���
        //      0   δ��
        //      1   ����
        static int GetNumberListStyle(List<string> strings)
        {
            if (strings.Count <= 1)
                return 0;
            Debug.Assert(strings.Count >= 2, "");
            if (strings[0] != strings[1])
                return 1;
            return -1;
        }

        static string OutputNumberList(List<string> strings)
        {
            Debug.Assert(strings != null, "");
            Debug.Assert(strings.Count >= 1, "");
            if (strings.Count == 1)
            {
                string strHead = strings[0];
                if (String.IsNullOrEmpty(strHead) == true)
                    strHead = "(��)";
                return strHead;
            }
            if (strings.Count >= 2)
            {
                string strHead = strings[0];
                string strTail = strings[strings.Count - 1];
                if (strHead == strTail)
                {
                    if (String.IsNullOrEmpty(strHead) == true)
                        strHead = "(��)";
                    return strHead + "*" + strings.Count.ToString();
                }

                return strHead + "-" + strTail;
            }
            Debug.Assert(false, "");
            return null;
        }

        // �Ƿ�Ϊȫ�����ַ���������?
        static bool IsNullList(List<string> parts)
        {
            if (parts.Count == 0)
                return true;
            for (int i = 0; i < parts.Count; i++)
            {
                if (String.IsNullOrEmpty(parts[i]) == false)
                    return false;
            }

            return true;
        }

        // 
        /// <summary>
        /// ������������ַ������Ϊһ�������ķ�Χ�ַ���
        /// </summary>
        /// <param name="parts">����ַ�������</param>
        /// <returns>��Ϻ���ַ���</returns>
        public static string BuildNumberRangeString(List<string> parts)
        {
            if (IsNullList(parts) == true)
                return "";

            string strResult = "";

            List<string> temp_list = new List<string>();

            string strNo = null;
            for (int i = 0; i < parts.Count; i++)
            {
                strNo = parts[i];

                if (temp_list.Count == 0)
                {
                    // ���뵱ǰ
                    temp_list.Add(strNo);
                    continue;
                }

                Debug.Assert(temp_list.Count > 0, "");

                // return:
                //      -1  ���
                //      0   δ��
                //      1   ����
                int nNumberStyle = GetNumberListStyle(temp_list);

                string strPrevNo = temp_list[temp_list.Count - 1];

                if (nNumberStyle == 1)
                {
                    if (IsNextNo(strPrevNo, strNo) == false)
                    {
                        // ���
                        goto OUTPUT;
                    }
                    // ���뵱ǰ
                    temp_list.Add(strNo);
                    continue;
                }

                if (nNumberStyle == -1)
                {
                    if (strPrevNo != strNo)
                    {
                        // ���
                        goto OUTPUT;
                    }
                    // ���뵱ǰ
                    temp_list.Add(strNo);
                    continue;
                }

                Debug.Assert(nNumberStyle == 0, "");

                if (IsNextNo(strPrevNo, strNo) == true
                    || strPrevNo == strNo)
                {
                    // ���뵱ǰ
                    temp_list.Add(strNo);
                    continue;
                }
                else
                {
                    goto OUTPUT;
                }

            OUTPUT:
                // �����Ȼ�����뵱ǰ
                if (String.IsNullOrEmpty(strResult) == false)
                    strResult += ",";
                strResult += OutputNumberList(temp_list);
                temp_list.Clear();
                temp_list.Add(strNo);
            }

            if (temp_list.Count > 0)
            {
                // ���
                if (String.IsNullOrEmpty(strResult) == false)
                    strResult += ",";
                strResult += OutputNumberList(temp_list);
            }

            return strResult;
        }

        #endregion
        /*
        // һ���ַ�����ͷ��?
        public static bool HasHead(string strText,
            string strHead)
        {
            if (strText.Length < strHead.Length)
                return false;
            string strPart = strText.Substring(0, strHead.Length);
            if (strPart == strHead)
                return true;
            return false;
        }
         * */

        /// <summary>
        /// ǿ�� GC ������������
        /// </summary>
        public static void ForceGarbageCollection()
        {
            //Force garbage collection.
            GC.Collect();

            // Wait for all finalizers to complete before continuing.
            // Without this call to GC.WaitForPendingFinalizers, 
            // the worker loop below might execute at the same time 
            // as the finalizers.
            // With this call, the worker loop executes only after
            // all finalizers have been called.
            GC.WaitForPendingFinalizers();
        }

        // 
        /// <summary>
        /// ����״̬�ַ����Ƿ�����ˡ��ӹ��С�
        /// </summary>
        /// <param name="strState">״̬�ַ���</param>
        /// <returns>�Ƿ�����ˡ��ӹ��С�����</returns>
        public static bool IncludeStateProcessing(string strState)
        {
            if (StringUtil.IsInList("�ӹ���", strState) == true)
                return true;
            return false;
        }

        // 
        /// <summary>
        /// Ϊ״̬�ַ�������(��)ֵ���ӹ��С�
        /// </summary>
        /// <param name="strState">��������ַ���</param>
        /// <returns>���ش������ַ���</returns>
        public static string AddStateProcessing(string strState)
        {
            string strResult = strState;
            StringUtil.SetInList(ref strResult, "�ӹ���", true);
            return strResult;
        }

        // 
        /// <summary>
        /// Ϊ״̬�ַ���ȥ��(��)ֵ���ӹ��С�
        /// </summary>
        /// <param name="strState">��������ַ���</param>
        /// <returns>���ش������ַ���</returns>
        public static string RemoveStateProcessing(string strState)
        {
            string strResult = strState;
            StringUtil.SetInList(ref strResult, "�ӹ���", false);
            return strResult;
        }

        // ����ʱ�䷶Χ�ַ���
        // ���ص�ʱ�䷶Χ�ַ�����ʽ����̬Ϊ ��19980101-19991231��
        /// <summary>
        /// ����ʱ�䷶Χ�ַ���
        /// </summary>
        /// <param name="start">��ʼʱ��</param>
        /// <param name="end">����ʱ��</param>
        /// <returns>���ص�ʱ�䷶Χ�ַ�����ʽ����̬Ϊ ��19980101-19991231��</returns>
        public static string MakeTimeRangeString(DateTime start,
            DateTime end)
        {
            string strStart = "";
            if (start != new DateTime(0))
                strStart = DateTimeUtil.DateTimeToString8(start);
            string strEnd = "";
            if (end != new DateTime(0))
                strEnd = DateTimeUtil.DateTimeToString8(end);

            return strStart + " - " + strEnd;
        }

        // ����ʱ�䷶Χ�ַ���
        // ע�����end == new DateTime(0)��ʾ���޿����ʱ�䡣
        // parameters:
        //      bAdjustEnd  �Ƿ����ĩβʱ�䡣������ָ����һ��
        //      strText ���ڷ�Χ�ַ�������̬Ϊ ��19980101-19991231��
        /// <summary>
        /// ����ʱ�䷶Χ�ַ���
        /// ע�����end == new DateTime(0)��ʾ���޿����ʱ��
        /// </summary>
        /// <param name="strText">ʱ�䷶Χ�ַ���</param>
        /// <param name="bAdjustEnd">�Ƿ����ĩβʱ�䡣������ָ����һ��</param>
        /// <param name="start">���ؿ�ʼʱ��</param>
        /// <param name="end">���ؽ���ʱ��</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����; 0: �ɹ�</returns>
        public static int ParseTimeRangeString(string strText,
            bool bAdjustEnd,
            out DateTime start,
            out DateTime end,
            out string strError)
        {
            strError = "";
            start = new DateTime((long)0);
            end = new DateTime((long)0);

            int nRet = strText.IndexOf("-");
            if (nRet == -1)
            {
                strError = "'" + strText + "' ��ȱ�����ۺ� '-'";
                return -1;
            }

            string strStart = strText.Substring(0, nRet).Trim();
            string strEnd = strText.Substring(nRet + 1).Trim();

            if (String.IsNullOrEmpty(strStart) == true)
                start = new DateTime(0);
            else
            {
                if (strStart.Length != 8)
                {
                    strError = "���ۺ���ߵĲ��� '" + strStart + "' ����8�ַ�";
                    return -1;
                }
                start = DateTimeUtil.Long8ToDateTime(strStart);
            }

            if (String.IsNullOrEmpty(strEnd) == true)
                end = new DateTime(0);
            else
            {
                if (strEnd.Length != 8)
                {
                    strError = "���ۺ��ұߵĲ��� '" + strEnd + "' ����8�ַ�";
                    return -1;
                }
                end = DateTimeUtil.Long8ToDateTime(strEnd);

                if (bAdjustEnd == true)
                {
                    // ����һ��
                    end += new TimeSpan(24, 0, 0);
                }
            }

            return 0;
        }

        // 
        /// <summary>
        /// ����һ��ʱ�䷶Χ�ڰ���������
        /// </summary>
        /// <param name="strRange">ʱ�䷶Χ�ַ���</param>
        /// <returns>�������֣���ʾ����</returns>
        public static float Years(string strRange)
        {
            int nRet = strRange.IndexOf("-");
            if (nRet == -1)
                return 0;
            string strStart = strRange.Substring(0, nRet).Trim();
            string strEnd = strRange.Substring(nRet + 1).Trim();

            if (strStart.Length != 8)
                return 0;
            if (strEnd.Length != 8)
                return 0;

            // 2012/5/9
            // ���������
            if (strStart.Substring(0, 4) == strEnd.Substring(0, 4)
                && strStart.Substring(4, 4) == "0101"
                && strEnd.Substring(4, 4) == "1231")
                return 1;

            DateTime start = DateTimeUtil.Long8ToDateTime(strStart);
            DateTime end = DateTimeUtil.Long8ToDateTime(strEnd);

            // �����е�С���⣬ĩβ����Ӧ������һ�յ�ǰһ��
            // 7���Ժ�
            end += new TimeSpan(1, 0, 0, 0);
            end -= new TimeSpan(0, 0, 0, 1);

            TimeSpan delta = end - start;

            return ((float)delta.TotalDays / (float)365);
        }

        // ���һ������ʱ���Ƿ����ض���ʱ�䷶Χ��?
        // Exception: �п����׳��쳣
        // parameters:
        //      strPublishTime  4/6/8�ַ�
        //      strRange    ��ʽΪ"20080101-20081231"
        /// <summary>
        /// ���һ������ʱ���Ƿ����ض���ʱ�䷶Χ��?
        /// ���ܻ��׳��쳣
        /// </summary>
        /// <param name="strPublishTime">����ʱ���ַ���������Ϊ 4/6/8 �ַ�</param>
        /// <param name="strRange">ʱ�䷶Χ�ַ�������ʽΪ"20080101-20081231"</param>
        /// <returns>�Ƿ��ڷ�Χ��</returns>
        public static bool InRange(string strPublishTime,
            string strRange)
        {
            if (strPublishTime.Length == 4)
                strPublishTime += "0101";
            else if (strPublishTime.Length == 6)
                strPublishTime += "01";

            int nRet = strRange.IndexOf("-");

            string strStart = strRange.Substring(0, nRet).Trim();
            string strEnd = strRange.Substring(nRet + 1).Trim();

            if (strStart.Length != 8)
                throw new Exception("ʱ�䷶Χ�ַ��� '" + strRange + "' ����߲��� '" + strStart + "' ��ʽ����ӦΪ8�ַ�");

            if (strEnd.Length != 8)
                throw new Exception("ʱ�䷶Χ�ַ��� '" + strRange + "' ���ұ߲��� '" + strEnd + "' ��ʽ����ӦΪ8�ַ�");

            if (String.Compare(strPublishTime, strStart) < 0)
                return false;

            if (String.Compare(strPublishTime, strEnd) > 0)
                return false;

            return true;
        }

        // �Ӽ�������Paste�в��뵽ListView�е�ǰѡ����λ��
        // ע������������ɾ������ǰ�Ѿ�ѡ����������
        // parameters:
        //      bInsertBefore   �Ƿ�ǰ��? ���==trueǰ�壬������
        /// <summary>
        /// �Ӽ�������Paste�в��뵽ListView�е�ǰѡ����λ��
        /// ע������������ɾ������ǰ�Ѿ�ѡ����������
        /// </summary>
        /// <param name="form">���� Form</param>
        /// <param name="list">ListView</param>
        /// <param name="bInsertBefore">�Ƿ�ǰ��? ���==trueǰ�壬������</param>
        public static void PasteLinesFromClipboard(Form form,
            ListView list,
            bool bInsertBefore)
        {
            IDataObject ido = Clipboard.GetDataObject();
            if (ido.GetDataPresent(DataFormats.UnicodeText) == false)
            {
                MessageBox.Show(form, "��������û������");
                return;
            }
            string strWhole = (string)ido.GetData(DataFormats.UnicodeText);

            int index = -1;

            if (list.SelectedIndices.Count > 0)
                index = list.SelectedIndices[0];

            Cursor oldCursor = form.Cursor;
            form.Cursor = Cursors.WaitCursor;

            list.SelectedItems.Clear();

            int nMaxColumns = 0;
            string[] lines = strWhole.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            list.BeginUpdate();
            for (int i = 0; i < lines.Length; i++)
            {
                ListViewItem item = Global.BuildListViewItem(
                    list,
                    lines[i],
                    false);
                // ���ﵥ����������ٶ�Ҫ��Щ
                if (item.SubItems.Count > nMaxColumns)
                    nMaxColumns = item.SubItems.Count;

                if (index == -1)
                    list.Items.Add(item);
                else
                {
                    if (bInsertBefore == true)
                        list.Items.Insert(index, item);
                    else
                        list.Items.Insert(index + 1, item);

                    index++;
                }

                item.Selected = true;
            }
            // ȷ���б�����Ŀ��
            ListViewUtil.EnsureColumns(list, nMaxColumns, 100);

            list.EndUpdate();

            form.Cursor = oldCursor;
        }

        // ���ƻ��߼���ListView��ѡ�������Clipboard
        // parameters:
        //      bCut    �Ƿ�Ϊ����
        /// <summary>
        /// ���ƻ��߼��� ListView ��ѡ���������ĳ�е� Clipboard
        /// </summary>
        /// <param name="form">���� Form</param>
        /// <param name="nColumnIndex">Ҫ���Ƶ��е��±�</param>
        /// <param name="list">ListView</param>
        /// <param name="bCut">�Ƿ�Ϊ����</param>
        public static void CopyLinesToClipboard(
            Form form,
            int nColumnIndex,
            ListView list,
            bool bCut)
        {
            Cursor oldCursor = form.Cursor;
            form.Cursor = Cursors.WaitCursor;

            StringBuilder strTotal = new StringBuilder(4096);

            foreach (ListViewItem item in list.SelectedItems)
            {
                string strLine = nColumnIndex >= item.SubItems.Count ? "" : item.SubItems[nColumnIndex].Text;
                strLine = strLine.Replace("\r", "\\r").Replace("\n", "\\n");    // ���������еĻس����и��� paste ���� Excel �ȵ�����
                strTotal.Append(strLine + "\r\n");
            }

            Clipboard.SetDataObject(strTotal.ToString(), true);

            if (bCut == true)
            {
                list.BeginUpdate();
                foreach (ListViewItem item in list.SelectedItems)
                {
                    list.Items.Remove(item);
                }
                /*
                for (int i = indices.Count - 1; i >= 0; i--)
                {
                    int index = indices[i];
                    list.Items.RemoveAt(index);
                }
                 * */
                list.EndUpdate();
            }

            form.Cursor = oldCursor;
        }

#if NO
        // ���ƻ��߼���ListView��ѡ�������Clipboard
        // parameters:
        //      bCut    �Ƿ�Ϊ����
        public static void CopyLinesToClipboard(
            Form form,
            ListView list,
            bool bCut)
        {
            Cursor oldCursor = form.Cursor;
            form.Cursor = Cursors.WaitCursor;

            StringBuilder strTotal = new StringBuilder(4096);

            foreach (ListViewItem item in list.SelectedItems)
            {
                string strLine = Global.BuildLine(item);
                strTotal.Append(strLine + "\r\n");
            }

            Clipboard.SetDataObject(strTotal.ToString(), true);

            if (bCut == true)
            {
                list.BeginUpdate();

                foreach (ListViewItem item in list.SelectedItems)
                {
                    // list.Items.Remove(item);
                    item.Remove();
                }
                /*
                for (int i = indices.Count - 1; i >= 0; i--)
                {
                    int index = indices[i];
                    list.Items.RemoveAt(index);
                }
                 * */

                list.EndUpdate();
            }

            form.Cursor = oldCursor;
        }
#endif

#if NO
        // ���ƻ��߼���ListView��ѡ�������Clipboard
        // parameters:
        //      bCut    �Ƿ�Ϊ����
        public static void CopyLinesToClipboard(
            Form form,
            ListView list,
            bool bCut)
        {
            Cursor oldCursor = form.Cursor;
            form.Cursor = Cursors.WaitCursor;

            StringBuilder strTotal = new StringBuilder(4096);

            if (bCut == true)
                list.BeginUpdate();

            foreach (ListViewItem item in list.SelectedItems)
            {
                string strLine = Global.BuildLine(item);
                strTotal.Append(strLine + "\r\n");

                if (bCut == true)
                    item.Remove();
            }

            if (bCut == true)
                list.EndUpdate();

            Clipboard.SetDataObject(strTotal.ToString(), true);

            form.Cursor = oldCursor;
        }
#endif

#if NO
        // ���ƻ��߼���ListView��ѡ�������Clipboard
        // parameters:
        //      bCut    �Ƿ�Ϊ����
        public static void CopyLinesToClipboard(
            Form form,
            ListView list,
            bool bCut)
        {
            Cursor oldCursor = form.Cursor;
            form.Cursor = Cursors.WaitCursor;

            StringBuilder strTotal = new StringBuilder(4096);

            foreach (ListViewItem item in list.SelectedItems)
            {
                string strLine = Global.BuildLine(item);
                strTotal.Append(strLine + "\r\n");
            }

            Clipboard.SetDataObject(strTotal.ToString(), true);

            if (bCut == true)
            {
                ListViewItem [] items = new ListViewItem[list.SelectedItems.Count];
                list.SelectedItems.CopyTo(items, 0);

                list.BeginUpdate();

                foreach (ListViewItem item in items)
                {
                    // list.Items.Remove(item);
                    item.Remove();
                }
                /*
                for (int i = indices.Count - 1; i >= 0; i--)
                {
                    int index = indices[i];
                    list.Items.RemoveAt(index);
                }
                 * */

                list.EndUpdate();
            }

            form.Cursor = oldCursor;
        }
#endif

        // ���ƻ��߼���ListView��ѡ�������Clipboard
        // parameters:
        //      bCut    �Ƿ�Ϊ����
        /// <summary>
        /// ���ƻ��߼��� ListView ��ѡ������� Clipboard
        /// </summary>
        /// <param name="form">���� Form</param>
        /// <param name="list">ListView</param>
        /// <param name="bCut">�Ƿ�Ϊ����</param>
        public static void CopyLinesToClipboard(
            Form form,
            ListView list,
            bool bCut)
        {
            Cursor oldCursor = form.Cursor;
            form.Cursor = Cursors.WaitCursor;

            StringBuilder strTotal = new StringBuilder(4096);

            foreach (ListViewItem item in list.SelectedItems)
            {
                string strLine = Global.BuildLine(item);
                strTotal.Append(strLine + "\r\n");
            }

            Clipboard.SetDataObject(strTotal.ToString(), true);

            if (bCut == true)
            {
                int[] indices = new int[list.SelectedItems.Count];
                list.SelectedIndices.CopyTo(indices, 0);

                list.BeginUpdate();

                for (int i = indices.Length - 1; i >= 0; i--)
                {
                    list.Items.RemoveAt(indices[i]);
                }

                list.EndUpdate();
            }

            form.Cursor = oldCursor;
        }

        /*
        // ��һ���ַ�������ȥ�ء�����ǰ��Ӧ���Ѿ�����
        public static void RemoveDup(ref List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                string strItem = list[i];
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (strItem == list[j])
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                    else
                    {
                        i = j - 1;
                        break;
                    }
                }
            }

        }
         * */

        // 
        /// <summary>
        /// �� ListViewItem �ı����ݹ���Ϊ tab �ַ��ָ���ַ���
        /// </summary>
        /// <param name="item">ListViewItem</param>
        /// <returns>�ַ���</returns>
        public static string BuildLine(ListViewItem item)
        {
            StringBuilder strLine = new StringBuilder(4096);
            for (int i = 0; i < item.SubItems.Count; i++)
            {
                if (i != 0)
                    strLine.Append("\t");

                string strText = item.SubItems[i].Text.Replace("\r", "\\r").Replace("\n", "\\n");    // ���������еĻس����и��� paste ���� Excel �ȵ�����
                strLine.Append(strText);
            }

            return strLine.ToString();
        }

        // �����ַ�������ListViewItem��
        // �ַ����ĸ�ʽΪ\t�����
        // parameters:
        //      list    ����Ϊnull�����Ϊnull����û���Զ���չ�б�����Ŀ�Ĺ���
        /// <summary>
        /// �����ַ�������ListViewItem
        /// �ַ�����ʽΪ tab �ַ��ָ���ַ���
        /// </summary>
        /// <param name="list">ListView������Ϊnull�����Ϊnull����û���Զ���չ�б�����Ŀ�Ĺ���</param>
        /// <param name="strLine">�ַ���</param>
        /// <param name="AutoExpandColumnCount">�Ƿ��Զ���չ����</param>
        /// <returns>����õ� ListViewItem ����</returns>
        public static ListViewItem BuildListViewItem(
            ListView list,
            string strLine,
            bool AutoExpandColumnCount = true)
        {
            ListViewItem item = new ListViewItem();
            string[] parts = strLine.Split(new char[] { '\t' });
            for (int i = 0; i < parts.Length; i++)
            {
                ListViewUtil.ChangeItemText(item, i, parts[i]);
            }

            // ȷ���б�����Ŀ��
            if (AutoExpandColumnCount == true)
            {
                if (list != null)
                    ListViewUtil.EnsureColumns(list, parts.Length, 100);
            }

            return item;
        }

        // ��¼·���Ƿ�Ϊ׷���ͣ�
        // ��ν׷���ͣ����Ǽ�¼ID����Ϊ'?'������û�м�¼ID����
        /// <summary>
        /// ��¼·���Ƿ�Ϊ׷���ͣ�
        /// ��ν׷���ͣ����Ǽ�¼ID����Ϊ'?'������û�м�¼ID����
        /// </summary>
        /// <param name="strPath">��¼·���ַ��������� "����ͼ��/120"</param>
        /// <returns>�Ƿ�Ϊ׷����</returns>
        public static bool IsAppendRecPath(string strPath)
        {
            if (String.IsNullOrEmpty(strPath) == true)
                return true;

            string strRecordID = Global.GetRecordID(strPath);
            if (String.IsNullOrEmpty(strRecordID) == true
                || strRecordID == "?")
                return true;

            return false;
        }

#if NO
        // �Ƿ�Ϊ������¼��·��
        /// <summary>
        /// ��¼·���Ƿ�Ϊ׷���͡������ֹ�˺���
        /// </summary>
        /// <param name="strPath">��¼·���ַ��������� "����ͼ��/120"</param>
        /// <returns>�Ƿ�Ϊ׷����</returns>
        public static bool IsNewPath(string strPath)
        {
            if (String.IsNullOrEmpty(strPath) == true)
                return true;    //???? ��·��������·��?

            string strID = Global.GetRecordID(strPath);

            if (strID == "?"
                || String.IsNullOrEmpty(strID) == true) // 2008/11/28 
                return true;

            return false;
        }
#endif

        /// <summary>
        /// ���ı��ļ��ж�������
        /// </summary>
        /// <param name="strFilePath">�ļ�ȫ·��</param>
        /// <param name="strContent">��������</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����; 0: �ɹ�</returns>
        public static int ReadTextFileContent(string strFilePath,
    out string strContent,
    out string strError)
        {
            return ReadTextFileContent(strFilePath,
                -1,
                out strContent,
                out strError);
        }

        /// <summary>
        /// ���ı��ļ��ж�������
        /// </summary>
        /// <param name="strFilePath">�ļ�ȫ·��</param>
        /// <param name="lMaxLength">�޶�����ַ�����-1 Ϊ������</param>
        /// <param name="strContent">��������</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����; 0: �ɹ�</returns>
        public static int ReadTextFileContent(string strFilePath,
    long lMaxLength,
    out string strContent,
    out string strError)
        {
            Encoding encoding = null;
            return FileUtil.ReadTextFileContent(strFilePath,
                lMaxLength,
                out strContent,
                out encoding,
                out strError);
        }

        // ������κű�
        // parameters:
        //      strPubType  ���������͡�Ϊ ͼ��/����������/(��) ֮һ
        internal static void GetBatchNoTable(GetKeyCountListEventArgs e,
            IWin32Window owner,
            string strPubType,  // ����������
            string strType,
            Stop stop,
            LibraryChannel channel)
        {
            string strError = "";
            long lRet = 0;


            if (e.KeyCounts == null)
                e.KeyCounts = new List<KeyCount>();

            string strName = "";
            if (strType == "order")
                strName = "����";
            else if (strType == "item")
                strName = "��";
            else if (strType == "biblio")
                strName = "��Ŀ";
            else
                throw new Exception("δ֪��strType '" + strType + "' ֵ");

            TimeSpan old_timeout = channel.Timeout;
            channel.Timeout = TimeSpan.FromMinutes(5);

            // EnableControls(false);
            stop.OnStop += new StopEventHandler(channel.DoStop);
            stop.Initial("�����г�ȫ��" + strName + "���κ� ...");
            stop.BeginLoop();

            try
            {
                int nPerMax = 2000; // һ�μ������е������������
                string strLang = "zh";

                string strDbName = "<all>";
                if (strPubType == "ͼ��")
                    strDbName = "<all book>";
                else if (strPubType == "����������")
                    strDbName = "<all series>";
                else
                    strDbName = "<all>";

                if (strType == "order")
                {
                    lRet = channel.SearchOrder(
                        stop,
                        strDbName,  // "<all>",
                        "", // strBatchNo
                        nPerMax,   // -1,
                        "���κ�",
                        "left",
                        strLang,
                        "batchno",   // strResultSetName
                        "desc",
                        "keycount", // strOutputStyle
                        out strError);
                }
                else if (strType == "biblio")
                {
                    string strQueryXml = "";

                    lRet = channel.SearchBiblio(
                        stop,
                        strDbName,  // "<all>",    // ���ܿ����� this.comboBox_inputBiblioDbName.Text, �Ա��ú�������Ŀ����ص����κ�ʵ�����������������᣺��Ϊ���ݿ����б�ˢ�º�����ȴ����ˢ�£�
                        "", // strBatchNo,
                        nPerMax,   // -1,    // nPerMax
                        "batchno",
                        "left",
                        strLang,
                        "batchno",   // strResultSetName
                        "desc",
                        "keycount", // strOutputStyle
                        "",
                        out strQueryXml,
                        out strError);
                }
                else if (strType == "item")
                {

                    lRet = channel.SearchItem(
                        stop,
                        strDbName,   // "<all>",
                        "", // strBatchNo
                        nPerMax,  // -1,
                        "���κ�",
                        "left",
                        strLang,
                        "batchno",   // strResultSetName
                        "desc",
                        "keycount", // strOutputStyle
                        out strError);
                }
                else
                {
                    Debug.Assert(false, "");
                }


                if (lRet == -1)
                    goto ERROR1;

                if (lRet == 0)
                {
                    strError = "û���ҵ��κ�" + strName + "���κż�����";
                    return;
                }

                long lHitCount = lRet;

                long lStart = 0;
                long lCount = lHitCount;
                DigitalPlatform.LibraryClient.localhost.Record[] searchresults = null;

                // װ�������ʽ
                for (; ; )
                {
                    Application.DoEvents();	// ���ý������Ȩ

                    if (stop != null && stop.State != 0)
                    {
                        strError = "�û��ж�";
                        goto ERROR1;
                    }

                    lRet = channel.GetSearchResult(
                        stop,
                        "batchno",   // strResultSetName
                        lStart,
                        lCount,
                        "keycount",
                        strLang,
                        out searchresults,
                        out strError);
                    if (lRet == -1)
                    {
                        strError = "GetSearchResult() error: " + strError;
                        goto ERROR1;
                    }

                    if (lRet == 0)
                    {
                        // MessageBox.Show(this, "δ����");
                        return;
                    }

                    // ����������
                    for (int i = 0; i < searchresults.Length; i++)
                    {
                        if (searchresults[i].Cols == null)
                        {
                            strError = "�����Ӧ�÷����������ݿ��ں˵����°汾������ʹ���г�" + strName + "���κŵĹ���";
                            goto ERROR1;
                        }

                        KeyCount keycount = new KeyCount();
                        keycount.Key = searchresults[i].Path;
                        keycount.Count = Convert.ToInt32(searchresults[i].Cols[0]);
                        e.KeyCounts.Add(keycount);
                    }

                    lStart += searchresults.Length;
                    lCount -= searchresults.Length;

                    stop.SetMessage("������ " + lHitCount.ToString() + " ������װ�� " + lStart.ToString() + " ��");

                    if (lStart >= lHitCount || lCount <= 0)
                        break;
                }
            }
            finally
            {
                stop.EndLoop();
                stop.OnStop -= new StopEventHandler(channel.DoStop);
                stop.Initial("");

                // EnableControls(true);

                channel.Timeout = old_timeout;
            }
            return;
        ERROR1:
            MessageBox.Show(owner, strError);
        }

        /// <summary>
        /// ��һ����ɫ���ձ����䰵
        /// </summary>
        /// <param name="color">ԭʼ��ɫ</param>
        /// <param name="percent">����</param>
        /// <returns>���ر仯�����ɫ</returns>
        public static Color Dark(Color color, float percent)
        {
            int r = color.R - (int)((float)color.R * percent);
            int g = color.G - (int)((float)color.G * percent);
            int b = color.B - (int)((float)color.B * percent);

            if (r < 0)
                r = 0;
            if (g < 0)
                g = 0;
            if (b < 0)
                b = 0;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// ��һ����ɫ���ձ�������
        /// </summary>
        /// <param name="color">ԭʼ��ɫ</param>
        /// <param name="percent">����</param>
        /// <returns>���ر仯�����ɫ</returns>
        public static Color Light(Color color, float percent)
        {
            int r = color.R + (int)((float)color.R * percent);
            int g = color.G + (int)((float)color.G * percent);
            int b = color.B + (int)((float)color.B * percent);

            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// �ݹ� Invalidate һ�� Control ������ȫ���� Control
        /// </summary>
        /// <param name="control">Control ����</param>
        public static void InvalidateAllControls(Control control)
        {
            control.Invalidate();
            for (int i = 0; i < control.Controls.Count; i++)
            {
                InvalidateAllControls(control.Controls[i]);    // �ݹ�
            }
        }

        // ������ݿ���
        // return:
        //      -1  �д�
        //      0   �޴�
        internal static int CheckDbName(string strDbName,
            out string strError)
        {
            strError = "";
            if (strDbName.IndexOf("#") != -1)
            {
                strError = "���ݿ��� '" + strDbName + "' ��ʽ���󡣲�����#��";
                return -1;
            }

            return 0;
        }

        // ��listviewcontrol��ǰ�����һ��
        /// <summary>
        /// �� ListViewControl1 ��ǰ�����һ��
        /// </summary>
        /// <param name="list">ListViewControl1 ����</param>
        /// <param name="strID">��ߵ�һ������</param>
        /// <param name="others">����������</param>
        /// <returns>�´����� ListViewItem ����</returns>
        public static ListViewItem InsertNewLine(
            ListViewControl1 list,
            string strID,
            string[] others)
        {
            if (others != null)
                ListViewUtil.EnsureColumns(list, others.Length + 1);

            ListViewItem item = new ListViewItem(strID, 0);

            list.Items.Insert(0, item);

            if (others != null)
            {
                for (int i = 0; i < others.Length; i++)
                {
                    item.SubItems.Add(others[i]);
                }
            }

            list.UpdateItem(0);

            return item;
        }

        // ��listviewcontrol���׷��һ��
        /// <summary>
        /// �� ListViewControl1 ���׷��һ��
        /// </summary>
        /// <param name="list">ListViewControl1 ����</param>
        /// <param name="strID">��ߵ�һ������</param>
        /// <param name="others">����������</param>
        /// <returns>�´����� ListViewItem ����</returns>
        public static ListViewItem AppendNewLine(
            ListViewControl1 list,
            string strID,
            string[] others)
        {
            if (others != null)
                ListViewUtil.EnsureColumns(list, others.Length + 1);

            ListViewItem item = new ListViewItem(strID, 0);

            list.Items.Add(item);

            if (others != null)
            {
                for (int i = 0; i < others.Length; i++)
                {
                    item.SubItems.Add(others[i]);
                }
            }

            list.UpdateItem(list.Items.Count - 1);

            return item;
        }


        // ��listview��ǰ�����һ��
        /// <summary>
        /// �� ListView ��ǰ�����һ��
        /// </summary>
        /// <param name="list">ListView ����</param>
        /// <param name="strID">��ߵ�һ������</param>
        /// <param name="others">����������</param>
        /// <returns>�´����� ListViewItem ����</returns>
        public static ListViewItem InsertNewLine(
            ListView list,
            string strID,
            string[] others)
        {
            if (others != null)
                ListViewUtil.EnsureColumns(list, others.Length + 1);

            ListViewItem item = new ListViewItem(strID, 0);

            list.Items.Insert(0, item);

            if (others != null)
            {
                for (int i = 0; i < others.Length; i++)
                {
                    item.SubItems.Add(others[i]);
                }
            }

            return item;
        }


        // ��listview���׷��һ��
        /// <summary>
        /// �� ListView ���׷��һ��
        /// </summary>
        /// <param name="list">ListView ����</param>
        /// <param name="strID">��ߵ�һ������</param>
        /// <param name="others">����������</param>
        /// <returns>�´����� ListViewItem ����</returns>
        public static ListViewItem AppendNewLine(
            ListView list,
            string strID,
            string[] others)
        {
            if (others != null)
                ListViewUtil.EnsureColumns(list, others.Length + 1);

            ListViewItem item = new ListViewItem(strID, 0);

            list.Items.Add(item);

            if (others != null)
            {
                for (int i = 0; i < others.Length; i++)
                {
                    item.SubItems.Add(others[i]);
                }
            }

            return item;
        }

        /*
        // ȷ���б��������㹻
        public static void EnsureColumns(ListView list,
            int nCount)
        {
            if (list.Columns.Count >= nCount)
                return;

            for (int i = list.Columns.Count; i < nCount; i++)
            {
                string strText = "";
                if (i == 0)
                {
                    strText = "��¼·��";
                }
                else
                {
                    strText = Convert.ToString(i);
                }

                ColumnHeader col = new ColumnHeader();
                col.Text = strText;
                col.Width = 200;
                list.Columns.Add(col);
            }

        }
         * */

        // 
        /// <summary>
        /// ��ö���ժҪ��Ϣ
        /// </summary>
        /// <param name="strReaderXml">���߼�¼ XML</param>
        /// <returns>����ժҪ</returns>
        public static string GetReaderSummary(string strReaderXml)
        {
            XmlDocument dom = new XmlDocument();
            try
            {
                dom.LoadXml(strReaderXml);
            }
            catch (Exception ex)
            {
                return "���߼�¼XMLװ��DOMʱ����: " + ex.Message;
            }

            return DomUtil.GetElementText(dom.DocumentElement,
                "name");
        }

        /// <summary>
        /// ��������ؼ����� HTML �ַ���
        /// </summary>
        /// <param name="webBrowser">������ؼ�</param>
        /// <param name="strHtml">HTML �ַ���</param>
        /// <param name="strDataDir">����Ŀ¼���������������д���һ����ʱ�ļ�</param>
        /// <param name="strTempFileType">��ʱ�ļ����͡����ڹ�����ʱ�ļ���</param>
        public static void SetHtmlString(WebBrowser webBrowser,
            string strHtml,
            string strDataDir,
            string strTempFileType)
        {
            StopWebBrowser(webBrowser);

            strHtml = strHtml.Replace("%datadir%", strDataDir);
            strHtml = strHtml.Replace("%mappeddir%", PathUtil.MergePath(strDataDir, "servermapped"));

            string strTempFilename = Path.Combine(strDataDir, "~temp_" + strTempFileType + ".html");
            using (StreamWriter sw = new StreamWriter(strTempFilename, false, Encoding.UTF8))
            {
                sw.Write(strHtml);
            }
            // webBrowser.Navigate(strTempFilename);
            Navigate(webBrowser, strTempFilename);  // 2015/7/28
        }

        // 2015/7/28 
        // �ܴ����쳣�� Navigate
        internal static void Navigate(WebBrowser webBrowser, string urlString)
        {
            int nRedoCount = 0;
        REDO:
            try
            {
                webBrowser.Navigate(urlString);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                /*
System.Runtime.InteropServices.COMException (0x800700AA): �������Դ��ʹ���С� (�쳣���� HRESULT:0x800700AA)
   �� System.Windows.Forms.UnsafeNativeMethods.IWebBrowser2.Navigate2(Object& URL, Object& flags, Object& targetFrameName, Object& postData, Object& headers)
   �� System.Windows.Forms.WebBrowser.PerformNavigate2(Object& URL, Object& flags, Object& targetFrameName, Object& postData, Object& headers)
   �� System.Windows.Forms.WebBrowser.Navigate(String urlString)
   �� dp2Circulation.QuickChargingForm._setReaderRenderString(String strText) λ�� F:\cs4.0\dp2Circulation\Charging\QuickChargingForm.cs:�к� 394
                 * */
                if ((uint)ex.ErrorCode == 0x800700AA)
                {
                    nRedoCount++;
                    if (nRedoCount < 5)
                    {
                        Application.DoEvents(); // 2015/8/13
                        Thread.Sleep(200);
                        goto REDO;
                    }
                }

                throw ex;
            }
        }

        // �����⣬��Ҫ��
        internal static void SetXmlString(WebBrowser webBrowser,
    string strHtml,
    string strDataDir,
    string strTempFileType)
        {
            strHtml = strHtml.Replace("%datadir%", strDataDir);
            strHtml = strHtml.Replace("%mappeddir%", PathUtil.MergePath(strDataDir, "servermapped"));

            string strTempFilename = Path.Combine(strDataDir, "~temp_" + strTempFileType + ".xml");

            // TODO: Ҫ����Ӧ"<root ... />"������û��prolog��XML����
            using (StreamWriter sw = new StreamWriter(strTempFilename, false, Encoding.UTF8))
            {
                sw.Write(strHtml);
            }
            // webBrowser.Navigate(strTempFilename);
            Navigate(webBrowser, strTempFilename);  // 2015/7/28
        }

        // �� XML �ַ���װ��һ��Web������ؼ�
        // ��������ܹ���Ӧ"<root ... />"������û��prolog��XML����
        /// <summary>
        /// �� XML �ַ���װ��һ��Web������ؼ�
        /// �������ܹ���Ӧ"&lt;root ... /&gt;"������û�� prolog �� XML ����
        /// </summary>
        /// <param name="webBrowser">������ؼ�</param>
        /// <param name="strDataDir">����Ŀ¼���������������д���һ����ʱ�ļ�</param>
        /// <param name="strTempFileType">��ʱ�ļ����͡����ڹ�����ʱ�ļ���</param>
        /// <param name="strXml">XML �ַ���</param>
        public static void SetXmlToWebbrowser(WebBrowser webBrowser,
            string strDataDir,
            string strTempFileType,
            string strXml)
        {
            if (string.IsNullOrEmpty(strXml) == true)
            {
                ClearHtmlPage(webBrowser,
                    strDataDir);
                return;
            }

            string strTargetFileName = strDataDir + "\\temp_" + strTempFileType + ".xml";

            XmlDocument dom = new XmlDocument();
            try
            {
                dom.LoadXml(strXml);
            }
            catch (Exception ex)
            {
                strTargetFileName = Path.Combine(strDataDir, "xml.txt");
                using (StreamWriter sw = new StreamWriter(strTargetFileName,
    false,	// append
    System.Text.Encoding.UTF8))
                {
                    sw.Write("XML����װ��DOMʱ����: " + ex.Message + "\r\n\r\n" + strXml);
                }
                // webBrowser.Navigate(strTargetFileName);
                Navigate(webBrowser, strTargetFileName);  // 2015/7/28
                return;
            }

            dom.Save(strTargetFileName);
            // webBrowser.Navigate(strTargetFileName);
            Navigate(webBrowser, strTargetFileName);  // 2015/7/28
        }

        internal static void PrepareStop(WebBrowser webBrowser)
        {
            webBrowser.Tag = new WebBrowserInfo();
        }

        internal static bool StopWebBrowser(WebBrowser webBrowser)
        {
            WebBrowserInfo info = null;
            if (webBrowser.Tag is WebBrowserInfo)
            {
                info = (WebBrowserInfo)webBrowser.Tag;
                if (info != null)
                {
                    if (info.Cleared == true)
                    {
                        webBrowser.Stop();
                        return true;
                    }
                    else
                    {
                        info.Cleared = true;
                        return false;
                    }
                }
            }

            return false;
        }

        // ��װ��İ汾
        public static void ClearHtmlPage(WebBrowser webBrowser,
    string strDataDir)
        {
            ClearHtmlPage(webBrowser, strDataDir, SystemColors.Window);
        }

        /// <summary>
        /// ���������ؼ�����
        /// </summary>
        /// <param name="webBrowser">������ؼ�</param>
        /// <param name="strDataDir">����Ŀ¼���������������д���һ����ʱ�ļ�</param>
        /// <param name="backColor">����ɫ</param>
        public static void ClearHtmlPage(WebBrowser webBrowser,
            string strDataDir,
            Color backColor)
        {
            StopWebBrowser(webBrowser);

            if (String.IsNullOrEmpty(strDataDir) == true)
            {
                webBrowser.DocumentText = "(��)";
                return;
            }
            string strImageUrl = PathUtil.MergePath(strDataDir, "page_blank_128.png");
            string strHtml = "<html><body style='background-color:" + ColorUtil.Color2String(backColor) + ";'><img src='" + strImageUrl + "' width='64' height='64' alt='��'></body></html>";
            webBrowser.DocumentText = strHtml;
        }

        /// <summary>
        /// ��������ؼ����� HTML �ַ���
        /// </summary>
        /// <param name="webBrowser">������ؼ�</param>
        /// <param name="strHtml">HTML �ַ���</param>
        public static void SetHtmlString(WebBrowser webBrowser,
    string strHtml)
        {
            /*
            // ���� �������ã������Զ�<body onload='...'>�¼�
            HtmlDocument doc = webBrowser.Document;

            if (doc == null)
            {
                webBrowser.Navigate("about:blank");
                doc = webBrowser.Document;
                Debug.Assert(doc != null, "doc��Ӧ��Ϊnull");
            }

            doc = doc.OpenNew(true);
            doc.Write(strHtml);
             * */

            webBrowser.DocumentText = strHtml;
        }

        // ��֧���첽����
        /// <summary>
        /// ��һ��������ؼ���׷��д�� HTML �ַ���
        /// ��֧���첽����
        /// </summary>
        /// <param name="webBrowser">������ؼ�</param>
        /// <param name="strHtml">HTML �ַ���</param>
        public static void WriteHtml(WebBrowser webBrowser,
    string strHtml)
        {

            HtmlDocument doc = webBrowser.Document;

            if (doc == null)
            {
                // webBrowser.Navigate("about:blank");
                Navigate(webBrowser, "about:blank");  // 2015/7/28

                doc = webBrowser.Document;
#if NO
                webBrowser.DocumentText = "<h1>hello</h1>";
                doc = webBrowser.Document;
                Debug.Assert(doc != null, "");
#endif
            }

            // doc = doc.OpenNew(true);
            doc.Write(strHtml);

            // ����ĩ�пɼ�
            // ScrollToEnd(webBrowser);
        }

        // �� WebBrowserExtension �е�ScrollToEnd() ���
        /// <summary>
        /// ��������ؼ����ݾ����β��
        /// </summary>
        /// <param name="webBrowser">������ؼ�����</param>
        public static void ScrollToEnd(WebBrowser webBrowser)
        {
#if NO
            /*
            API.SendMessage(window.Handle,
                API.WM_VSCROLL,
                API.SB_BOTTOM,  // (int)API.MakeLParam(API.SB_BOTTOM, 0),
                0);
             * */
            HtmlDocument doc = webBrowser.Document;
            doc.Window.ScrollTo(0, 0x7fffffff);

            /*
            webBrowser.Invalidate();
            webBrowser.Update();
             * */
#endif
            webBrowser.ScrollToEnd();   // 2016/4/22
        }

#if NO
        public static void ScrollToEnd(WebBrowser webBrowser)
        {
            HtmlDocument doc = webBrowser.Document;
            doc.Body.ScrollIntoView(false);
        }
#endif

        // 
        /// <summary>
        /// ����ַ����Ƿ�Ϊ������(������'-','.'��)
        /// </summary>
        /// <param name="s">�ַ���</param>
        /// <returns>�Ƿ�Ϊ������</returns>
        public static bool IsPureNumber(string s)
        {
            if (s == null)
                return false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] > '9' || s[i] < '0')
                    return false;
            }
            return true;
        }

        static string source_chars = "������������������������������������������������������������£ãģţƣǣȣɣʣˣ̣ͣΣϣУѣңӣԣգ֣ףأ٣�";
        static string target_chars = "0123456789..abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// ���ַ��������ȫ���ַ�ת��Ϊ��Ӧ�İ���ַ�
        /// </summary>
        /// <param name="strText">Ҫ������ַ���</param>
        /// <returns>�������ַ���</returns>
        public static string ConvertQuanjiaoToBanjiao(string strText)
        {
            Debug.Assert(source_chars.Length == target_chars.Length, "");
            string strTarget = "";
            for (int i = 0; i < strText.Length; i++)
            {
                char ch = strText[i];

                int nRet = source_chars.IndexOf(ch);
                if (nRet != -1)
                    ch = target_chars[nRet];

                strTarget += ch;
            }

            return strTarget;
        }

        // 
        /// <summary>
        /// ���һ���ַ����Ƿ������ȫ���ַ�
        /// </summary>
        /// <param name="strText">Ҫ�����ַ���</param>
        /// <returns>�Ƿ������ȫ���ַ�</returns>
        public static bool HasQuanjiaoChars(string strText)
        {
            for (int i = 0; i < strText.Length; i++)
            {
                char ch = strText[i];

                int nRet = source_chars.IndexOf(ch);
                if (nRet != -1)
                    return true;
            }

            return false;
        }

        // 
        /// <summary>
        /// ��������ؼ������е������������Ϊ��������Ĵ��ı���ʾ����׼��
        /// </summary>
        /// <param name="webBrowser">������ؼ�</param>
        public static void ClearForPureTextOutputing(WebBrowser webBrowser)
        {
            HtmlDocument doc = webBrowser.Document;

            if (doc == null)
            {
                // webBrowser.Navigate("about:blank");
                Navigate(webBrowser, "about:blank");  // 2015/7/28
                doc = webBrowser.Document;
            }

            doc = doc.OpenNew(true);
            doc.Write("<pre>");
        }

        /// <summary>
        /// ɾ�������ļ�
        /// </summary>
        /// <param name="filenames">�ļ�������</param>
        public static void DeleteFiles(List<string> filenames)
        {
            if (filenames == null)
                return;

            for (int i = 0; i < filenames.Count; i++)
            {
                try
                {
                    File.Delete(filenames[i]);
                }
                catch
                {
                }
            }
        }

        // 
        // parammeters:
        //      strPath ·��������"����ͼ��/3"
        /// <summary>
        /// ��·����ȡ����������
        /// </summary>
        /// <param name="strPath">·��������"����ͼ��/3"</param>
        /// <returns>���ؿ�������</returns>
        public static string GetDbName(string strPath)
        {
            int nRet = strPath.LastIndexOf("/");
            if (nRet == -1)
                return strPath;

            return strPath.Substring(0, nRet).Trim();
        }

        // 
        // parammeters:
        //      strPath ·��������"����ͼ��/3"
        /// <summary>
        /// ��·����ȡ����¼�Ų���
        /// </summary>
        /// <param name="strPath">·��������"����ͼ��/3"</param>
        /// <returns>���ؼ�¼�Ų���</returns>
        public static string GetRecordID(string strPath)
        {
            int nRet = strPath.LastIndexOf("/");
            if (nRet == -1)
                return "";

            return strPath.Substring(nRet + 1).Trim();
        }

#if NO
        // ��·����ȡ��id����
        // ԭ����entityform.cs��
        // parammeters:
        //      strPath ·��������"����ͼ��/3"
        public static string GetID(string strPath)
        {
            int nRet = strPath.LastIndexOf("/");
            if (nRet == -1)
                return strPath;

            return strPath.Substring(nRet + 1).Trim();
        }
#endif


        // ��ISBN����ȡ�ó�����Ų���
        // �����������Զ���Ӧ��978ǰ׺������ISBN��
        // ע��ISBN���б����к��
        // parameters:
        //      strPublisherNumber  ��������롣������978-����
        /// <summary>
        /// �� ISBN ����ȡ�ó�����Ų���
        /// �����������Զ���Ӧ�� 978 ǰ׺������ ISBN ��
        /// ע�� ISBN ���б����к��
        /// </summary>
        /// <param name="strISBN">ISBN ���ַ���</param>
        /// <param name="strPublisherNumber">���س�������벿�֡������� 978- ����</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1: ����; 1: �ɹ�</returns>
        public static int GetPublisherNumber(string strISBN,
            out string strPublisherNumber,
            out string strError)
        {
            strError = "";
            strPublisherNumber = "";
            int nRet = 0;

            if (strISBN == null)
            {
                strError = "ISBNΪ��";
                return -1;
            }

            strISBN = strISBN.Trim();

            if (String.IsNullOrEmpty(strISBN) == true)
            {
                strError = "ISBNΪ��";
                return -1;
            }

            // ��̽ǰ���ǲ���978
            nRet = strISBN.IndexOf("-");
            if (nRet == -1)
            {
                strError = "ISBN�ַ��� '" + strISBN + "' ��û�к�ܷ��ţ� ����޷���ȡ��������벿��";
                return -1;
            }

            int nStart = 0; // ��ʼȡ�ŵ�λ��
            string strFirstPart = strISBN.Substring(0, nRet);

            if (strFirstPart == "978" || strFirstPart == "979")
            {
                nStart = nRet + 1;

                nRet = strISBN.IndexOf("-", nStart);

                if (nRet == -1)
                {
                    strError = "ISBN����ȱ���ڶ�����ܣ�����޷���ȡ�������";
                    return -1;
                }

                // ��ʱnRet��978-7-�ĵڶ����������
            }
            else
            {
                nStart = 0;

                // ��ʱnRet��7-�ĺ������
            }

            nRet = strISBN.IndexOf("-", nRet + 1);
            if (nRet != -1)
            {
                strPublisherNumber = strISBN.Substring(nStart, nRet - nStart).Trim();
            }
            else
            {
                strPublisherNumber = strISBN.Substring(nStart).Trim();
            }

            return 1;
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    public enum PublicationType
    {
        Book = 0,
        Series = 1,
    }

    /// <summary>
    /// ������ؼ���Ϣ
    /// </summary>
    internal class WebBrowserInfo
    {
        /// <summary>
        /// �Ƿ�����ʹ�ù�һ��
        /// </summary>
        public bool Cleared = false;    // �Ƿ�ʹ�ù�
    }
}
