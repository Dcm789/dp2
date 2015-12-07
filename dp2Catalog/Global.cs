using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using DigitalPlatform.GUI;
using System.Xml;
using System.IO;
using DigitalPlatform.IO;
using DigitalPlatform.Text;
using System.Collections;
using DigitalPlatform.dp2.Statis;

namespace dp2Catalog
{
    public class Global
    {
        public static int ParsePathParam(string strPathParam,
    out int index,
    out string strPath,
    out string strDirection,
    out string strError)
        {
            strError = "";
            index = -1;
            strPath = "";
            strDirection = "";

            Hashtable param_table = StringUtil.ParseParameters(strPathParam, ',', ':');
            if (param_table["index"] != null)
            {
                string strIndex = (string)param_table["index"];
                if (Int32.TryParse(strIndex, out index) == false)
                {
                    strError = "strPath ����ֵ '" + strPathParam + "' ��ʽ����index Ӧ��Ϊ������";
                    return -1;
                }

            }
            if (param_table["path"] != null)
            {
                strPath = (string)param_table["path"];
            }
            if (param_table["direction"] != null)
            {
                strDirection = (string)param_table["direction"];
            }
            return 0;
        }

        // ��֧���첽����
        public static void WriteHtml(WebBrowser webBrowser,
    string strHtml)
        {

            HtmlDocument doc = webBrowser.Document;

            if (doc == null)
            {
                webBrowser.Navigate("about:blank");
                doc = webBrowser.Document;
            }

            // doc = doc.OpenNew(true);
            doc.Write(strHtml);

            // ����ĩ�пɼ�
            // ScrollToEnd(webBrowser);
        }

        // �� XML �ַ���װ��һ��Web������ؼ�
        // ��������ܹ���Ӧ"<root ... />"������û��prolog��XML����
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
                webBrowser.Navigate(strTargetFileName);
                return;
            }

            dom.Save(strTargetFileName);
            webBrowser.Navigate(strTargetFileName);
        }

        public static bool StopWebBrowser(WebBrowser webBrowser)
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


        public static void ClearHtmlPage(WebBrowser webBrowser,
            string strDataDir)
        {
            StopWebBrowser(webBrowser);

            if (String.IsNullOrEmpty(strDataDir) == true)
            {
                webBrowser.DocumentText = "(��)";
                return;
            }
            string strImageUrl = PathUtil.MergePath(strDataDir, "page_blank_128.png");
            string strHtml = "<html><body><img src='" + strImageUrl + "' width='64' height='64' alt='��'></body></html>";
            webBrowser.DocumentText = strHtml;
            /*
            string strTempFilename = strDataDir + "\\temp_blank_page.html";
            using (StreamWriter sw = new StreamWriter(strTempFilename, false, Encoding.UTF8))
            {
                sw.Write(strHtml);
            }
            webBrowser.Navigate(strTempFilename);
             * */
        }

        public static void SetHtmlString(WebBrowser webBrowser,
    string strHtml,
    string strDataDir,
    string strTempFileType)
        {
            StopWebBrowser(webBrowser);

            strHtml = strHtml.Replace("%datadir%", strDataDir);
            strHtml = strHtml.Replace("%mappeddir%", PathUtil.MergePath(strDataDir, "servermapped"));

            string strTempFilename = strDataDir + "\\temp_" + strTempFileType + ".html";
            using (StreamWriter sw = new StreamWriter(strTempFilename, false, Encoding.UTF8))
            {
                sw.Write(strHtml);
            }
            webBrowser.Navigate(strTempFilename);
        }

        // ��ListViewItem�ı����ݹ���Ϊtab�ַ��ָ���ַ���
        public static string BuildLine(ListViewItem item)
        {
            string strLine = "";
            for (int i = 0; i < item.SubItems.Count; i++)
            {
                if (i != 0)
                    strLine += "\t";
                strLine += item.SubItems[i].Text;
            }

            return strLine;
        }

        // �����ַ�������ListViewItem��
        // �ַ����ĸ�ʽΪ\t�����
        // parameters:
        //      list    ����Ϊnull�����Ϊnull����û���Զ���չ�б�����Ŀ�Ĺ���
        public static ListViewItem BuildListViewItem(
            ListView list,
            string strLine)
        {
            ListViewItem item = new ListViewItem();
            string[] parts = strLine.Split(new char[] { '\t' });
            for (int i = 0; i < parts.Length; i++)
            {
                ListViewUtil.ChangeItemText(item, i, parts[i]);

                // ȷ���б�����Ŀ��
                if (list != null)
                    ListViewUtil.EnsureColumns(list, parts.Length, 100);
            }

            return item;
        }

        // �Ӽ�������Paste�в��뵽ListView�е�ǰѡ����λ��
        // parameters:
        //      bInsertBefore   �Ƿ�ǰ��? ���==trueǰ�壬������
        public static void PasteLinesFromClipboard(Form form,
            string strFormatList,
            ListView list,
            bool bInsertBefore)
        {
            IDataObject ido = Clipboard.GetDataObject();
            if (ido.GetDataPresent(DataFormats.UnicodeText) == false
                && ido.GetDataPresent(typeof(MemLineCollection)) == false)
            {
                MessageBox.Show(form, "��������û������");
                return;
            }

            // ����ʹ��ר�ø�ʽ
            if (ido.GetDataPresent(typeof(MemLineCollection)) == true)
            {
                MemLineCollection mem_lines = (MemLineCollection)ido.GetData(typeof(MemLineCollection));

                if (StringUtil.IsInList(mem_lines.Format, strFormatList) == false)
                {
                    MessageBox.Show(form, "�������е����ݸ�ʽ�����ϵ�ǰ����Ҫ���޷�ճ��");
                    return;
                }

                int index = -1;

                if (list.SelectedIndices.Count > 0)
                    index = list.SelectedIndices[0];

                Cursor oldCursor = form.Cursor;
                form.Cursor = Cursors.WaitCursor;

                list.SelectedItems.Clear();

                foreach (MemLine line in mem_lines)
                {
                    /*
                    ListViewItem item = Global.BuildListViewItem(
                        list,
                        line.Line);
                    item.Tag = line.Tag;
                     * */
                    line.Item.Tag = line.Tag;

                    if (index == -1)
                        list.Items.Add(line.Item);
                    else
                    {
                        if (bInsertBefore == true)
                            list.Items.Insert(index, line.Item);
                        else
                            list.Items.Insert(index + 1, line.Item);

                        index++;
                    }

                    line.Item.Selected = true;
                }

                form.Cursor = oldCursor;
                return;
            }

            if (ido.GetDataPresent(DataFormats.UnicodeText) == true)
            {
                string strWhole = (string)ido.GetData(DataFormats.UnicodeText);

                int index = -1;

                if (list.SelectedIndices.Count > 0)
                    index = list.SelectedIndices[0];

                Cursor oldCursor = form.Cursor;
                form.Cursor = Cursors.WaitCursor;

                list.SelectedItems.Clear();

                string[] lines = strWhole.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    ListViewItem item = Global.BuildListViewItem(
                        list,
                        lines[i]);

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

                form.Cursor = oldCursor;
            }
        }

        public static void ExportLinesToExcel(
    Form form,
    ListView list)
        {
            // ѯ���ļ���
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Title = "��ָ��Ҫ����� Excel �ļ���";
            dlg.CreatePrompt = false;
            dlg.OverwritePrompt = true;
            // dlg.FileName = this.ExportExcelFilename;
            // dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Filter = "Excel �ļ� (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            ExcelDocument doc = null;
            // Sheet sheet = null;
            int _lineIndex = 0;

            doc = ExcelDocument.Create(dlg.FileName);

            doc.NewSheet("Sheet1");

            int nColIndex = 0;
            foreach (ColumnHeader header in list.Columns)
            {
                doc.WriteExcelCell(
                    _lineIndex,
                    nColIndex++,
                    header.Text,
                    true);
            }
            _lineIndex++;

            Cursor oldCursor = form.Cursor;
            form.Cursor = Cursors.WaitCursor;

            for (int i = 0; i < list.SelectedIndices.Count; i++)
            {
                int index = list.SelectedIndices[i];

                ListViewItem item = list.Items[index];

                List<CellData> cells = new List<CellData>();

                nColIndex = 0;
                foreach (ListViewItem.ListViewSubItem subitem in item.SubItems)
                {
                    cells.Add(
                        new CellData(
                                    nColIndex++,
                                    subitem.Text,
                                    true,
                                    0)
                    );
                }

                doc.WriteExcelLine(
    _lineIndex,
    cells,
    WriteExcelLineStyle.None);

                _lineIndex++;
            }

            doc.SaveWorksheet();
            if (doc != null)
            {
                doc.Close();
                doc = null;
            }

            form.Cursor = oldCursor;
        }

        // ���ƻ��߼���ListView��ѡ�������Clipboard
        // parameters:
        //      bCut    �Ƿ�Ϊ����
        public static void CopyLinesToClipboard(
            Form form,
            string strFormat,
            ListView list,
            bool bCut)
        {
            Cursor oldCursor = form.Cursor;
            form.Cursor = Cursors.WaitCursor;

            MemLineCollection mem_lines = new MemLineCollection();
            mem_lines.Format = strFormat;

            List<int> indices = new List<int>();
            string strTotal = "";
            for (int i = 0; i < list.SelectedIndices.Count; i++)
            {
                int index = list.SelectedIndices[i];

                ListViewItem item = list.Items[index];
                string strLine = Global.BuildLine(item);
                strTotal += strLine + "\r\n";

                MemLine mem_line = new MemLine();
                mem_line.Item = item;
                mem_line.Tag = item.Tag;
                mem_lines.Add(mem_line);

                if (bCut == true)
                    indices.Add(index);
            }

            // Clipboard.SetDataObject(strTotal, true);

            DataObject obj = new DataObject();
            obj.SetData(typeof(MemLineCollection), mem_lines);
            obj.SetData(strTotal);
            Clipboard.SetDataObject(obj, true);

            if (bCut == true)
            {
                for (int i = indices.Count - 1; i >= 0; i--)
                {
                    int index = indices[i];
                    list.Items.RemoveAt(index);
                }
            }

            form.Cursor = oldCursor;
        }

        // ���ƻ��߼���ListView��ѡ�������Clipboard
        // parameters:
        //      bCut    �Ƿ�Ϊ����
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

        // ����װ��dp2library·���任Ϊ��װ��̬
        // ���磺�ӡ����ط�����/����ͼ��/1���任Ϊ"����ͼ��/1@���ط�����"
        public static string GetBackStyleDp2Path(string strPath)
        {
            int nRet = strPath.IndexOf("/");
            if (nRet == -1)
                return strPath;

            string strServerName = strPath.Substring(0, nRet).Trim();
            string strPurePath = strPath.Substring(nRet + 1).Trim();

            return strPurePath + "@" + strServerName;
        }

        // �ѵ�װ��dp2library·���任Ϊ��װ��̬
        // ���磺��"����ͼ��/1@���ط�����"�任Ϊ�����ط�����/����ͼ��/1��
        public static string GetForwardStyleDp2Path(string strPath)
        {
            int nRet = strPath.IndexOf("@");
            if (nRet == -1)
                return strPath;

            string strServerName = strPath.Substring(nRet + 1).Trim();
            string strPurePath = strPath.Substring(0, nRet).Trim();

            return strServerName + "/" + strPurePath;
        }

#if NO
        public static string ConvertSinglePinyinByStyle(string strPinyin,
    PinyinStyle style)
        {
            if (style == PinyinStyle.None)
                return strPinyin;
            if (style == PinyinStyle.Upper)
                return strPinyin.ToUpper();
            if (style == PinyinStyle.Lower)
                return strPinyin.ToLower();
            if (style == PinyinStyle.UpperFirst)
            {
                if (strPinyin.Length > 1)
                {
                    return strPinyin.Substring(0, 1).ToUpper() + strPinyin.Substring(1).ToLower();
                }

                return strPinyin;
            }

            Debug.Assert(false, "δ�����ƴ�����");
            return strPinyin;
        }
#endif

        // ��ISBN����ȡ�ó�����Ų���
        // �����������Զ���Ӧ��978ǰ׺������ISBN��
        // ע��ISBN���б����к��
        // parameters:
        //      strPublisherNumber  ��������롣������978-����
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

            if (strFirstPart == "978")
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

        /*
        // ����dp2library·��
        public static int ParseDp2Path(string strFullPath,
            out string strServerUrl,
            out string strLocalPath,
            out string strError)
        {
            strError = "";
            strServerUrl = "";
            strLocalPath = "";

            int nRet = strFullPath.IndexOf("@");
            if (nRet == -1)
            {
                strError = "ȱ��@";
                return -1;
            }

            strServerUrl = strFullPath.Substring(nRet + 1);

            strLocalPath = strFullPath.Substring(0, nRet);

            return 0;
        }*/

        // ����·��
        public static int ParsePath(string strFullPath,
            out string strProtocol,
            out string strPath,
            out string strError)
        {
            strError = "";
            strProtocol = "";
            strPath = "";

            int nRet = strFullPath.IndexOf(":");
            if (nRet == -1)
            {
                strError = "ȫ·�� '"+strFullPath+"' ��ȱ���ַ� ':'";
                return -1;
            }

            strProtocol = strFullPath.Substring(0, nRet).ToLower(); // Э�����淶ΪСд�ַ���̬
            // ȥ��":"
            strPath = strFullPath.Substring(nRet + 1);

            return 0;
        }


        public static void FillEncodingList(ComboBox list,
            bool bHasMarc8)
        {
            list.Items.Clear();

            List<string> encodings = Global.GetEncodingList(bHasMarc8);
            for (int i = 0; i < encodings.Count; i++)
            {
                list.Items.Add(encodings[i]);
            }

            /*
            EncodingInfo[] infos = Encoding.GetEncodings();
            for (int i = 0; i < infos.Length; i++)
            {
                list.Items.Add(infos[i].Name);
            }
             * */
        }

        // �г�encoding���б�
        // ��Ҫ��gb2312 utf-8�ȳ��õ���ǰ
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
    }

    public class WebBrowserInfo
    {
        public bool Cleared = false;    // �Ƿ�ʹ�ù�
    }

    [Serializable]
    public class MemLineCollection : List<MemLine>
    {
        public string Format = "";
    }

    [Serializable]
    public class MemLine
    {
        public ListViewItem Item = null;
        public object Tag = null;
    }
}
