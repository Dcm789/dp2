using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Text;

using DigitalPlatform.GUI;

namespace DigitalPlatform.Marc
{
    internal class MyEdit : TextBox
    {
#if BIDI_SUPPORT
        const int WM_ADJUST_CARET = API.WM_USER + 200;
#endif
        public bool ContentIsNull = false;

        public int DisableFlush = 0;    // ��ʱ��ֹflush
        MarcEditor m_marcEditor = null;

        //�Ƿ����滻״̬
        public bool Overwrite = false;

        public int nInMenu = 0;

        public bool m_bChanged = false;

        /// <summary>
        /// �����Ƿ������޸�
        /// </summary>
        public bool Changed
        {
            get
            {
                return this.m_bChanged;
            }
            set
            {
                this.m_bChanged = value;
            }
        }

        /*
        protected override void CreateHandle()
        {
            this.TextChanged -= new EventHandler(MyEdit_TextChanged);
            this.TextChanged += new EventHandler(MyEdit_TextChanged);
        }*/

        void MyEdit_TextChanged(object sender, EventArgs e)
        {
            this.Changed = true;
        }

        public override bool Multiline
        {
            get
            {
                return base.Multiline;
            }
            set
            {
                this.DisableFlush++;
                base.Multiline = value;
                this.DisableFlush--;
            }
        }

        MarcEditor MarcEditor
        {
            get
            {
                if (this.m_marcEditor == null)
                    this.m_marcEditor = (MarcEditor)this.Parent;

                return this.m_marcEditor;
            }
        }

        // ����йز�������ڵ�ǰ���ֶε���Ϣ
        // return:
        //      0   �������ֶ���
        //      1   �����ֶ���
        public static int GetCurrentSubfieldCaretInfo(
            string strFieldValue,
            int nCaretPos,
            out string strSubfieldName,
            out string strSufieldContent,
            out int nStart,
            out int nContentStart,
            out int nContentLength)
        {
            nStart = 0;
            nContentStart = 0;
            nContentLength = 0;
            strSubfieldName = "";
            strSufieldContent = "";

            strFieldValue = strFieldValue.Replace(Record.KERNEL_SUBFLD, Record.SUBFLD);

            bool bFoundPrevDollar = false;

            int nEnd = strFieldValue.Length;

            // �����ҵ�$����
            for (int i = nCaretPos; i >= 0; i--)
            {
                if (nCaretPos > strFieldValue.Length - 1)
                    continue;

                char ch = strFieldValue[i];

                // �����ֶη���
                if (ch == Record.SUBFLD)
                {
                    bFoundPrevDollar = true;
                    nStart = i;
                    break;
                }
            }

            // �����ҵ�$����
            for (int i = nCaretPos + 1; i < strFieldValue.Length; i++)
            {
                char ch = strFieldValue[i];

                // �����ֶη���
                if (ch == Record.SUBFLD)
                {
                    nEnd = i;
                    break;
                }
            }

            if (bFoundPrevDollar == true)
            {
                nContentStart = nStart + 2;
                if (nContentStart > nEnd)
                    nContentStart = nEnd;
                strSubfieldName = strFieldValue.Substring(nStart, 1);
            }

            nContentLength = nEnd - nContentStart;
            strSufieldContent = strFieldValue.Substring(nContentStart, nContentLength);

            return 1;
        }

        public void DupCurrentSubfield()
        {
            if (this.MarcEditor.m_nFocusCol != 3)
                return;

            string strSubfieldName = "";
            string strSufieldContent = "";
            int nStart = 0;
            int nContentStart = 0;
            int nContentLength = 0;

            // ����йز�������ڵ�ǰ���ֶε���Ϣ
            // return:
            //      0   �������ֶ���
            //      1   �����ֶ���
            int nRet = GetCurrentSubfieldCaretInfo(
                this.Text,
                this.SelectionStart,
                out strSubfieldName,
                out strSufieldContent,
                out nStart,
                out nContentStart,
                out nContentLength);
            if (nRet == 0)
                return;

            if (nContentStart >= 2)
            {
                nContentStart -= 2;
                nContentLength += 2;
            }

            string strText = this.Text.Substring(nContentStart, nContentLength);
            int nInsertPos = nContentStart + nContentLength;
            this.Text = this.Text.Substring(0, nInsertPos) + strText + this.Text.Substring(nInsertPos);
            this.SelectionStart = nContentStart;
            this.SelectionLength = nContentLength;
        }


        // return:
        //      false   ��Ҫִ��ȱʡ���ڹ���
        //      true    ��Ҫִ��ȱʡ���ڹ��̡�����Ϣ�ӹ��ˡ�
        bool DoDoubleClick()
        {
            // OnMouseDoubleClick����ӹܲ��С���Ϊ����editԭ�ж����Ѿ�ִ��,this.SelectionStartֵ�Ѿ����ƻ�

            if (this.MarcEditor.m_nFocusCol != 3)
                return false;

            string strSubfieldName = "";
            string strSufieldContent = "";
            int nStart = 0;
            int nContentStart = 0;
            int nContentLength = 0;

            // ����йز�������ڵ�ǰ���ֶε���Ϣ
            // return:
            //      0   �������ֶ���
            //      1   �����ֶ���
            int nRet = GetCurrentSubfieldCaretInfo(
                this.Text,
                this.SelectionStart,
                out strSubfieldName,
                out strSufieldContent,
                out nStart,
                out nContentStart,
                out nContentLength);
            if (nRet == 0)
                return false;

            this.SelectionStart = nContentStart;
            this.SelectionLength = nContentLength;
            return true;
        }

        /// <summary>
        /// ȱʡ���ڹ���
        /// </summary>
        /// <param name="m">��Ϣ</param>
        protected override void DefWndProc(ref Message m)
        {

            switch (m.Msg)
            {
#if BIDI_SUPPORT
                case WM_ADJUST_CARET:
                    // ��������⴦�ڷ�����ź����ֶη���(31)֮��
                    if (this.SelectionLength == 0)
                    {
                        if (IsInForbiddenPos(this.SelectionStart) == true
                            && this.SelectionStart > 0)
                            this.SelectionStart--;
                    }
                    return;
#endif

                // ��ֹedit����Ĳ˵�
                case API.WM_RBUTTONDOWN:
                case API.WM_RBUTTONUP:
                    {
                        if (nInMenu > 0)
                            return;
                    }
                    break;
                case API.WM_LBUTTONDOWN:
                    {
                        uint nCurClickTime = API.GetMessageTime();
                        uint nDelta = nCurClickTime - this.MarcEditor.m_nLastClickTime;



                        int x = (int)((uint)m.LParam & 0x0000ffff);
                        int y = (int)(((uint)m.LParam >> 16) & 0x0000ffff);

                        bool bIn = true;
                        try
                        {
                            if (this.MarcEditor.m_rectLastClick.Contains(x + this.Location.X, y + this.Location.Y) == false)
                                bIn = false;
                        }
                        catch   // 2006/11/6 add
                        {
                        }



                        this.MarcEditor.m_nLastClickTime = nCurClickTime;
                        this.MarcEditor.m_rectLastClick = new Rectangle((int)x + this.Location.X, (int)y + this.Location.Y, 0, 0);
                        try
                        {
                            this.MarcEditor.m_rectLastClick.Inflate(
                                SystemInformation.DoubleClickSize.Width / 2,
                                SystemInformation.DoubleClickSize.Height / 2);
                        }
                        catch  // 2006/11/6 add
                        { }

                        if (bIn == false)
                            break;


                        if (nDelta < API.GetDoubleClickTime())
                        {
                            // ����˫��
                            if (DoDoubleClick() == true)
                                return;
                        }
                    }
                    break;
                case API.WM_LBUTTONDBLCLK:
                    {
                        if (DoDoubleClick() == true)
                            return;
                    }
                    break;
            }
            base.DefWndProc(ref m);
        }


        bool _k = false;    // �Ƿ��� Ctrl+K ״̬

        // �ӹ�Ctrl+���ּ�
        /// <summary>
        /// ����Ի����
        /// </summary>
        /// <param name="keyData">System.Windows.Forms.Keys ֵ֮һ������ʾҪ����ļ���</param>
        /// <returns>����ؼ�����ʹ�û�������Ϊ true������Ϊ false���������һ������</returns>
        protected override bool ProcessDialogKey(
            Keys keyData)
        {
            // ȥ��Control/Shift/Alt �Ժ�Ĵ����ļ���
            // 2008/11/30 changed
            Keys pure_key = (keyData & (~(Keys.Control | Keys.Shift | Keys.Alt)));


            if (_k)
            {
                this.MarcEditor.DoCtrlK(pure_key);
                _k = false;
                return true;
            }

            // Ctrl + M
            if (Control.ModifierKeys == Keys.Control
                && pure_key == Keys.Enter)
            {
                // ��ֹ����س�����
                return true;
            }

            // Ctrl + M
            if (Control.ModifierKeys == Keys.Control
                && pure_key == Keys.M)
            {
                MarcEditor.EditControlTextToItem();

                // ��ģ��
                this.MarcEditor.GetValueFromTemplate();
                return true;
            }

            // Ctrl + C
            if ((keyData & Keys.Control) == Keys.Control
                && pure_key == Keys.C)
            {
                this.Menu_Copy(null, null);
                return true;
            }

            // Ctrl + X
            if ((keyData & Keys.Control) == Keys.Control
                && pure_key == Keys.X)
            {
                this.Menu_Cut(null, null);
                return true;
            }

            // Ctrl + V
            if ((keyData & Keys.Control) == Keys.Control
                && pure_key == Keys.V)
            {
                this.Menu_Paste(null, null);
                return true;
            }

            // Ctrl + K
            if ((keyData & Keys.Control) == Keys.Control
                && pure_key == Keys.K)
            {
                _k = true;
                return true;
            }

            /*
            // Ctrl + A �Զ�¼�빦��
            if ((keyData & Keys.Control) == Keys.Control
                && (keyData & Keys.A) == Keys.A)
            {
                if (this.m_marcEditor != null)
                {
                    GenerateDataEventArgs ea = new GenerateDataEventArgs();
                    this.m_marcEditor.OnGenerateData(ea);
                    return true;
                }
            }*/

            // Ctrl + A �Զ�¼�빦��
            // && (keyData & (~Keys.Control)) == Keys.A)   // 2007/5/15 �޸ģ�ԭ��������CTRL+C��CTRL+A�������ã�CTRL+C�Ǹ����á�

            if (keyData == (Keys.A | Keys.Control) // ��Ҳ��һ���취
                || keyData == (Keys.A | Keys.Control | Keys.Alt))
            {
                if (this.m_marcEditor != null)
                {
                    GenerateDataEventArgs ea = new GenerateDataEventArgs();
                    this.m_marcEditor.OnGenerateData(ea);
                    return true;
                }
            }

            if (keyData == (Keys.Y | Keys.Control)) // ��Ҳ��һ���취
            {
                if (this.m_marcEditor != null)
                {
                    GenerateDataEventArgs ea = new GenerateDataEventArgs();
                    this.m_marcEditor.OnVerifyData(ea);
                    return true;
                }
            }

            /*
            // Ctrl + T ����
            if ((keyData & Keys.Control) == Keys.Control
                && (keyData & (~Keys.Control)) == Keys.T)   // 2008/11/30 �޸ģ�ԭ��������CTRL+C��CTRL+A�������ã�CTRL+C�Ǹ����á�
                // && (keyData & Keys.T) == Keys.T)
            {
                if (this.m_marcEditor != null)
                {
                    this.m_marcEditor.Test();

                    return true;
                }
            }*/

            // ����δ����ļ�
            if ((keyData & Keys.Control) == Keys.Control)
            {
                bool bRet = this.MarcEditor.OnControlLetterKeyPress(pure_key);
                if (bRet == true)
                    return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        // ʧȥ����ʱ��Ӧ�����ݻ���ȥ
        protected override void OnLostFocus(EventArgs e)
        {
            // 2008/6/4
            this.MarcEditor.OldImeMode = this.ImeMode;

            if (this.DisableFlush > 0)
            {
                base.OnLostFocus(e);    //
                return;
            }

            this.MarcEditor.Flush();
            base.OnLostFocus(e);
        }

        // ��ý��㣬��ֹȫѡ
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            //this.SelectionLength = 0;

            // 2008/6/4
            if (this.ImeMode != this.MarcEditor.OldImeMode)
            {
                if (this.MarcEditor.ReadOnly == true)
                    this.ImeMode = this.MarcEditor.OldImeMode;
                else
                {   // 2009/11/20
                    if (this.MarcEditor.OldImeMode == ImeMode.Disable)
                        this.ImeMode = ImeMode.NoControl;
                    else
                        this.ImeMode = this.MarcEditor.OldImeMode;
                }
                API.SetImeHalfShape(this);
            }
        }

        // ���
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            int numberOfPixelsToMove = numberOfTextLinesToMove * (int)this.Font/*.DefaultTextFont*/.GetHeight();
            this.MarcEditor.DocumentOrgY += numberOfPixelsToMove;
        }

        #region �Ҽ��˵�

        // parameters:
        //      nActiveCol  ��ǰ�����
        public void PopupMenu(Control control,
            Point p)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem;

            // ȱʡֵ
            Cursor oldcursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;   // ����ɳ©

            List<string> macros = this.MarcEditor.SetDefaultValue(true, -1);

            this.Cursor = oldcursor;

            string strText = "";
            if (macros == null || macros.Count == 0)
                strText = "ȱʡֵ(��)";
            else if (macros.Count == 1)
            {
                Debug.Assert(macros != null, "");
                strText = "ȱʡֵ '" + macros[0].Replace(" ", "_").Replace(Record.SUBFLD, Record.KERNEL_SUBFLD) + "'";
            }
            else
                strText = "ȱʡֵ " + macros.Count.ToString() + " ��";

            menuItem = new MenuItem(strText);
            // menuItem.Click += new System.EventHandler(this.MarcEditor.SetCurFirstDefaultValue);

            if (macros != null && macros.Count == 1)
            {
                menuItem.Click += new System.EventHandler(this.MarcEditor.SetCurrentDefaultValue);
                menuItem.Tag = 0;
            }
            else if (macros != null && macros.Count > 1)
            {
                // �Ӳ˵�
                for (int i = 0; i < macros.Count; i++)
                {
                    string strMenuText = macros[i];

                    MenuItem subMenuItem = new MenuItem(strMenuText);
                    subMenuItem.Click += new System.EventHandler(this.MarcEditor.SetCurrentDefaultValue);
                    subMenuItem.Tag = i;
                    menuItem.MenuItems.Add(subMenuItem);
                }
            }
            contextMenu.MenuItems.Add(menuItem);
            if (macros == null || macros.Count == 0)
                menuItem.Enabled = false;

            //--------------
            menuItem = new MenuItem("-");
            contextMenu.MenuItems.Add(menuItem);

            // ����
            menuItem = new MenuItem("����(&U)");
            menuItem.Click += new System.EventHandler(this.Menu_Undo);
            contextMenu.MenuItems.Add(menuItem);
            if (this.CanUndo == true)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;

            //--------------
            menuItem = new MenuItem("-");
            contextMenu.MenuItems.Add(menuItem);

            // ����
            menuItem = new MenuItem("����(&I)");
            menuItem.Click += new System.EventHandler(this.Menu_Cut);
            contextMenu.MenuItems.Add(menuItem);
            if (this.SelectionLength > 0)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;

            // ����
            menuItem = new MenuItem("����(&C)");
            menuItem.Click += new System.EventHandler(this.Menu_Copy);
            contextMenu.MenuItems.Add(menuItem);
            if (this.SelectionLength > 0)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;

            // ճ��
            menuItem = new MenuItem("ճ��(&P)");
            menuItem.Click += new System.EventHandler(this.Menu_Paste);
            contextMenu.MenuItems.Add(menuItem);
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;

            // ɾ��
            menuItem = new MenuItem("ɾ��(&D)");
            menuItem.Click += new System.EventHandler(this.Menu_Delete);
            contextMenu.MenuItems.Add(menuItem);
            if (this.SelectionLength > 0)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;

            //--------------
            menuItem = new MenuItem("-");
            contextMenu.MenuItems.Add(menuItem);

            // ȫѡ
            menuItem = new MenuItem("ȫѡ(&A)");
            menuItem.Click += new System.EventHandler(this.Menu_SelectAll);
            contextMenu.MenuItems.Add(menuItem);
            if (this.Text != "")
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;

            //--------------
            menuItem = new MenuItem("-");
            contextMenu.MenuItems.Add(menuItem);

            // ����ģ��
            string strCurName = "";
            bool bEnable = this.MarcEditor.HasTemplateOrValueListDef(
                "template",
                out strCurName);

            menuItem = new MenuItem("����ģ��(Ctrl+M) " + strCurName);
            menuItem.Click += new System.EventHandler(this.MarcEditor.GetValueFromTemplate);
            contextMenu.MenuItems.Add(menuItem);
            if (this.MarcEditor.SelectedFieldIndices.Count == 1
                && bEnable == true)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;

            // ֵ�б�
            bEnable = this.MarcEditor.HasTemplateOrValueListDef(
                "valuelist",
                out strCurName);

            menuItem = new MenuItem("ֵ�б� " + strCurName);
            menuItem.Click += new System.EventHandler(this.MarcEditor.GetValueFromValueList);
            contextMenu.MenuItems.Add(menuItem);
            if (this.MarcEditor.SelectedFieldIndices.Count == 1
                && bEnable == true)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;

#if NO
            // ����
            menuItem = new MenuItem("IMEMODE");
            menuItem.Click += new System.EventHandler(this.ShowImeMode);
            contextMenu.MenuItems.Add(menuItem);
#endif

            /*
            //--------------
            menuItem = new MenuItem("-");
            contextMenu.MenuItems.Add(menuItem);

            // ɾ���ֶ�
            menuItem = new MenuItem("ɾ���ֶ�");
            menuItem.Click += new System.EventHandler(this.MarcEditor.DeleteFieldWithDlg);
            contextMenu.MenuItems.Add(menuItem);
            if (this.MarcEditor.m_nFocusCol == 1 || this.MarcEditor.m_nFocusCol == 2)
                menuItem.Enabled = true;
            else
                menuItem.Enabled = false;
             */

            // ׷�������˵���
            if (this.MarcEditor != null)
            {
                //--------------
                menuItem = new MenuItem("-");
                contextMenu.MenuItems.Add(menuItem);

                this.MarcEditor.AppendMenu(contextMenu, false);
            }

            contextMenu.Show(control, p);
        }

        void ShowImeMode(System.Object sender, System.EventArgs e)
        {
            MessageBox.Show(this, this.ImeMode.ToString());
        }

        private void Menu_Copy(System.Object sender, System.EventArgs e)
        {
            if (this.SelectionLength > 0)
            {
                string strText = this.SelectedText;
                strText = strText.Replace(Record.KERNEL_SUBFLD, Record.SUBFLD);
                DigitalPlatform.Marc.MarcEditor.TextToClipboard(strText);

                //this.Copy();
            }
        }

        private void Menu_Cut(System.Object sender, System.EventArgs e)
        {
            if (this.SelectedText != "")
            {
                string strText = this.SelectedText;
                strText = strText.Replace(Record.KERNEL_SUBFLD, Record.SUBFLD);

                this.Cut();

                DigitalPlatform.Marc.MarcEditor.TextToClipboard(strText);

                // �ֶ�����ȷ��3�ַ�
                if (this.MarcEditor.m_nFocusCol == 1)
                {
                    if (this.Text.Length < 3)
                        this.Text = this.Text.PadRight(3, ' ');
                }
                // �ֶ�ָʾ����ȷ��2�ַ�
                else if (this.MarcEditor.m_nFocusCol == 2)
                {
                    if (this.Text.Length < 2)
                        this.Text = this.Text.PadRight(2, ' ');
                }

                this.MarcEditor.Flush();
            }
        }

        public void PasteToCurrent(string strText)
        {
            if (string.IsNullOrEmpty(strText) == false)
            {
                int nPos = this.SelectionStart;
                if (this.SelectionLength != 0)
                    this.Text = this.Text.Remove(nPos, this.SelectionLength);
                this.Text = this.Text.Insert(nPos, strText);
                this.SelectionStart = nPos;
                this.SelectionLength = strText.Length;
            }
        }

        // ���� $a �����һ���ո�
        static string RemoveBlankChar(string strText)
        {
            int step = -1;  // ��ʾ��ǰ char ���� $ �ַ��Ĳ�����0 ��ʾ������ $a �� $ �ϣ�1 ��ʾ�� $a �� a �ϣ�2����ʾ�� a ����һ���ַ���
            StringBuilder result = new StringBuilder();
            foreach (char ch in strText)
            {
                if (ch == Record.KERNEL_SUBFLD)
                    step = 0;
                else if (step != -1)
                    step++;

                if (step == 2 && ch == ' ')
                {
                    // Խ��
                }
                else
                    result.Append(ch);
            }

            // ȥ�� $ ǰ���һ���ո�
            for (int i = result.Length - 1; i >= 0; i--)
            {
                if (i - 1 >= 0
                    && result[i] == Record.KERNEL_SUBFLD
                    && result[i - 1] == ' ')
                {
                    result.Remove(i - 1, 1);
                    // i--;
                }
            }

            return result.ToString();
        }

        private void Menu_Paste(System.Object sender, System.EventArgs e)
        {
            bool bControl = Control.ModifierKeys == Keys.Control;

            // Determine if there is any text in the Clipboard to paste into the text box.
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                // �����ֶη��Ż�һ��
                string strText = DigitalPlatform.Marc.MarcEditor.ClipboardToText();
                if (strText == null)
                    strText = "";

                // ȥ���س����з���
                strText = strText.Replace("\r\n", "\r");
                strText = strText.Replace("\r", "*");
                strText = strText.Replace("\n", "*");
                strText = strText.Replace("\t", "*");

                // 2017/2/20
                strText = strText.Replace(Record.SUBFLD, Record.KERNEL_SUBFLD);

                if (bControl)
                {
                    strText = strText.Replace('|', Record.KERNEL_SUBFLD);
                    // strText = strText.Replace(" ", "");
                    strText = RemoveBlankChar(strText);
                }

                Debug.Assert(this.MarcEditor.SelectedFieldIndices.Count == 1, "Menu_Paste(),MarcEditor.SelectedFieldIndices����Ϊ1��");

                string strFieldsMarc = strText;
                // strFieldsMarc = strFieldsMarc.Replace(Record.SUBFLD, Record.KERNEL_SUBFLD);

                // ���ҵ��м����ֶ�

                List<string> fields = Record.GetFields(strFieldsMarc);
                if (fields == null || fields.Count == 0)
                    return;

                // ճ������
                if (fields.Count == 1)
                {
                    string strThisText = fields[0];
                    int nOldSelectionStart = this.SelectionStart;
                    if (this.SelectedText == "")
                    {
                        this.Text = this.Text.Insert(this.SelectionStart, strThisText);
                    }
                    else
                    {
                        string strTempText = this.Text;
                        strTempText = strTempText.Remove(nOldSelectionStart, this.SelectedText.Length);
                        strTempText = strTempText.Insert(nOldSelectionStart, strThisText);

                        this.Text = strTempText;
                    }

                    this.SelectionStart = nOldSelectionStart + strThisText.Length;

                    // �ֶ�����ȷ��3�ַ�
                    if (this.MarcEditor.m_nFocusCol == 1)
                    {
                        if (this.Text.Length > 3)
                            this.Text = this.Text.Substring(0, 3);
                    }
                    // �ֶ�ָʾ����ȷ��2�ַ�
                    else if (this.MarcEditor.m_nFocusCol == 2)
                    {
                        if (this.Text.Length > 2)
                            this.Text = this.Text.Substring(0, 2);
                    }

                    this.MarcEditor.Flush();    // ��ʹ֪ͨ���
                }
                else if (fields.Count > 1)
                {
                    List<string> addFields = new List<string>();
                    // ˦����һ�� i = 1
                    for (int i = 0; i < fields.Count; i++)
                    {
                        addFields.Add(fields[i]);
                    }

                    int nIndex = this.MarcEditor.FocusedFieldIndex;
                    Debug.Assert(nIndex >= 0 && nIndex < this.MarcEditor.Record.Fields.Count, "Menu_Paste()��FocusFieldIndexԽ�硣");
                    int nStartIndex = nIndex + 1;
                    int nNewFieldsCount = addFields.Count;

                    this.MarcEditor.Record.Fields.InsertInternal(nStartIndex,
                        addFields);

                    // �ѽ�����Ϊ���һ����
                    Debug.Assert(nStartIndex + nNewFieldsCount <= this.MarcEditor.Record.Fields.Count, "�����ܵ����");

                    // �����ֶ��е����һ���ֶ���Ϊ��ǰ�ֶ�
                    this.MarcEditor.SetActiveField(nStartIndex + nNewFieldsCount - 1, 3);

                    InvalidateRect iRect = new InvalidateRect();
                    iRect.bAll = false;
                    iRect.rect = this.MarcEditor.GetItemBounds(nIndex, //nStartIndex,
                        -1,
                        BoundsPortion.FieldAndBottom);
                    this.MarcEditor.AfterDocumentChanged(ScrollBarMember.Both,
                        iRect);
                }
            }
        }

        private void Menu_Delete(System.Object sender, System.EventArgs e)
        {
            if (this.SelectedText != "")
            {
                int nStart = this.SelectionStart;
                this.Text = this.Text.Remove(this.SelectionStart, this.SelectionLength);
                this.SelectionStart = nStart;

                // �ֶ�����ȷ��3�ַ�
                if (this.MarcEditor.m_nFocusCol == 1)
                {
                    if (this.Text.Length < 3)
                        this.Text = this.Text.PadRight(3, ' ');
                }
                // �ֶ�ָʾ����ȷ��2�ַ�
                else if (this.MarcEditor.m_nFocusCol == 2)
                {
                    if (this.Text.Length < 2)
                        this.Text = this.Text.PadRight(2, ' ');
                }
            }
        }

        private void Menu_Undo(System.Object sender, System.EventArgs e)
        {
            // Determine if last operation can be undone in text box.   
            if (this.CanUndo == true)
            {
                // Undo the last operation.
                this.Undo();
                // Clear the undo buffer to prevent last action from being redone.
                this.ClearUndo();
            }
        }

        private void Menu_SelectAll(System.Object sender, System.EventArgs e)
        {
            this.SelectAll();
        }

        #endregion

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (nInMenu > 0)
                return;

            if (e.Button == MouseButtons.Right)
            {
                nInMenu++;
                PopupMenu(this, new Point(e.X, e.Y));
                nInMenu--;
                return;
            }

            API.PostMessage(this.MarcEditor.Handle,
MarcEditor.WM_LEFTRIGHT_MOVED,
0,
0);

            base.OnMouseDown(e);

            /*
             * ��ʱע�͵� xietao

            POINT point = new POINT();
            point.x = 0;
            point.y = 0;
            bool bRet = API.GetCaretPos(ref point);
            Rectangle rect = new Rectangle(point.x - 10,
                point.y,
                20,
                30);
            // parameter:
            //		nCol	�к� 
            //				0 �ֶ�˵��;
            //				1 �ֶ���;
            //				2 �ֶ�ָʾ�� 
            //				3 �ֶ��ڲ�
            this.MarcEditor.EnsureVisible(this.MarcEditor.FocusedFieldIndex,
                3,
                rect);
             */
        }

        /*
���� ����δ����Ľ����߳��쳣: 
Type: System.ArgumentOutOfRangeException
Message: �����ͼ����������ø��ַ����ڵ�λ�á�
������: count
Stack:
�� System.String.RemoveInternal(Int32 startIndex, Int32 count)
�� System.String.Remove(Int32 startIndex, Int32 count)
�� DigitalPlatform.Marc.MyEdit.OnKeyPress(KeyPressEventArgs e)
�� System.Windows.Forms.Control.ProcessKeyEventArgs(Message& m)
�� System.Windows.Forms.Control.ProcessKeyMessage(Message& m)
�� System.Windows.Forms.Control.WmKeyChar(Message& m)
�� System.Windows.Forms.Control.WndProc(Message& m)
�� System.Windows.Forms.TextBoxBase.WndProc(Message& m)
�� System.Windows.Forms.TextBox.WndProc(Message& m)
�� System.Windows.Forms.Control.ControlNativeWindow.OnMessage(Message& m)
�� System.Windows.Forms.Control.ControlNativeWindow.WndProc(Message& m)
�� System.Windows.Forms.NativeWindow.Callback(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)


dp2Circulation �汾: dp2Circulation, Version=2.30.6506.29202, Culture=neutral, PublicKeyToken=null
����ϵͳ��Microsoft Windows NT 6.1.7601 Service Pack 1
���� MAC ��ַ: xxx 
����ʱ�� 2017/10/26 9:28:53 (Thu, 26 Oct 2017 09:28:53 +0800) 
         * * */
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            _k = false;
            switch (e.KeyChar)
            {
                case '#':
                    if (this.m_marcEditor.m_nFocusCol == 1)
                    {
                        e.Handled = true;
                        Console.Beep(); // ��ʾ�ܾ���������ַ�
                        return;
                    }
                    break;
                case '\\':
                    {
                        if (this.m_marcEditor.m_nFocusCol != 3)
                            break;

                        // Ϊ�β�������?
                        // Ctrl + \ �������� \
                        if (Control.ModifierKeys == Keys.Control)
                            break;

                        int nOldStart = this.SelectionStart;

                        string strTemp = this.Text;
                        /*
                        if (nOldStart < strTemp.Length)
                            strTemp = strTemp.Remove(nOldStart, 1);
                         */
#if BIDI_SUPPORT

                        strTemp = strTemp.Insert(nOldStart, "\x200e" + new string((char)Record.KERNEL_SUBFLD, 1));
#else
                        strTemp = strTemp.Insert(nOldStart, new string((char)Record.KERNEL_SUBFLD, 1));
#endif

                        this.Text = strTemp;
                        this.SelectionStart = nOldStart;
                        e.Handled = true;

#if BIDI_SUPPORT
                        this.SelectionStart += 2;

#else
                        this.SelectionStart++;
#endif
                        return;
                    }
                    break;
                case (char)Keys.Enter:
                    {
                        if (this.MarcEditor.EnterAsAutoGenerate == true)
                        {
                            GenerateDataEventArgs ea = new GenerateDataEventArgs();
                            this.m_marcEditor.OnGenerateData(ea);
                        }
                        else
                        {
                            // ����ֶ�
                            // this.MarcEditor.InsertAfterFieldWithoutDlg();

                            // parameters:
                            //      nAutoComplate   0: false; 1: true; -1:���ֵ�ǰ����״̬
                            this.MarcEditor.InsertField(this.MarcEditor.FocusedFieldIndex, 0, 1);   // false, true
                        }
                        e.Handled = true;
                        return;
                    }
                case (char)Keys.Back:
                    {
                        if (this.Overwrite == true)
                        {
                            e.Handled = true;
                            Console.Beep();
                            return;

                            /* ������ܱ����������Ǳ���ֹ��
                            int nOldSelectionStart = this.SelectionStart;
                            if (nOldSelectionStart > 0)
                            {
                                this.Text = this.Text.Remove(nOldSelectionStart - 1, 1);

                                this.Text = this.Text.Insert(nOldSelectionStart - 1, " ");
                                this.SelectionStart = nOldSelectionStart - 1;
                                return;
                            }
                             * */
                        }

#if BIDI_SUPPORT
                        int nStart = this.SelectionStart - 1;
                        if (this.SelectionLength == 0
                            && nStart > 0)
                        {
                            // Ϊ����׼������Ϣ
                            string strDebugInfo = "ɾ��ǰ�� text [" + this.Text + "] hex[" + GetTextHex(this.Text) + "], nStart=" + nStart + ", this.Text.Length=" + this.Text.Length;

                            try
                            {
                                // ���ɾ���������Ƿ����ַ�����ôҲҪ׷��ɾ������һ����ͨ�ַ�
                                if (this.Text[nStart] == 0x200e)    // && this.Text.Length >= nStart + 1 + 1
                                {
                                    // һͬɾ��
                                    this.Text = this.Text.Remove(
                                        nStart - 1,
                                        2);
                                    this.SelectionStart = nStart - 1;
                                    // 2011/12/5 ��������������BUG
                                    e.Handled = true;
                                }
                                // ���ɾ��λ�õ��������Ƿ����ַ���ҲҪһ��ɾ��
                                else if (nStart > 0
                                    && this.Text[nStart - 1] == 0x200e)
                                {
                                    // һͬɾ��
                                    this.Text = this.Text.Remove(nStart - 1, 2);
                                    this.SelectionStart = nStart - 1;
                                    e.Handled = true;
                                }
                            }
                            catch (ArgumentOutOfRangeException ex)
                            {
                                throw new Exception("Backspace �����쳣:" + strDebugInfo, ex);
                            }
                        }
#endif

                    }
                    break;
                default:
                    {
                        if (this.Overwrite == true)
                        {
                            if ((Control.ModifierKeys == Keys.Control)
                                // || Control.ModifierKeys == Keys.Shift
                                || Control.ModifierKeys == Keys.Alt)
                            {
                                break;
                            }
                            int nOldSelectionStart = this.SelectionStart;
                            if (nOldSelectionStart < this.Text.Length)
                            {
                                if (this.Text.Length >= this.MaxLength) // 2009/3/6 changed
                                {
                                    // Ϊ����׼������Ϣ
                                    string strDebugInfo = "ɾ��ǰ�� text [" + this.Text + "] hex[" + GetTextHex(this.Text) + "], this.MaxLength=" + this.MaxLength + ", this.Text.Length=" + this.Text.Length;

                                    try
                                    {
                                        this.Text = this.Text.Remove(this.SelectionStart, 1 + (this.Text.Length - this.MaxLength));
                                        this.SelectionStart = nOldSelectionStart;
                                    }
                                    catch (ArgumentOutOfRangeException ex)
                                    {
                                        throw new Exception("default overwrite �����쳣:" + strDebugInfo, ex);
                                    }
                                }
                                this.ContentIsNull = false; // 2017/1/15 ��ֹ�״��� MyEdit �������޷����ֵ��ڴ�
                            }
                            else
                            {
                                Console.Beep(); // ��ʾ�ܾ���������ַ�
                            }
                        }
                    }
                    break;
            }

            base.OnKeyPress(e);
        }

        static string GetTextHex(string strText)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;
            foreach (char ch in strText)
            {
                string strHex = Convert.ToString(ch, 16).PadLeft(2, '0');

                result.Append(i.ToString() + ":" + strHex + ",");
                i++;
            }

            return result.ToString();
        }

#if BIDI_SUPPORT
        bool IsForbiddenPos()
        {
            if (this.Text.Length <= this.SelectionStart)
                return false;
            char current = this.Text[this.SelectionStart];
            if (current == 0x200e)
                return true;
            return false;
        }

        bool IsInForbiddenPos(int index)
        {
            if (index <= 0)
                return false;

            if (index >= this.Text.Length)
                return false;

            char left = this.Text[index - 1];
            char current = this.Text[index];

            // ��������⴦�ڷ�����ź����ֶη���(31)
            if (left == 0x200e && current == Record.KERNEL_SUBFLD)
                return true;

            return false;

        }
#endif

        // ���¼�
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        int x;
                        int y;
                        API.GetEditCurrentCaretPos(this,
                            out x,
                            out y);
                        if (y == 0 && this.MarcEditor.FocusedFieldIndex != 0)		// ��Ȼ������0���������ϻ���-1
                        {
                            this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex - 1,
                                this.MarcEditor.m_nFocusCol);

                            POINT point = new POINT();
                            point.x = 0;
                            point.y = 0;
                            bool bRet = API.GetCaretPos(ref point);
                            point.y = this.ClientSize.Height - 2;
                            API.SendMessage(this.Handle,
                                API.WM_LBUTTONDOWN,
                                new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
                                API.MakeLParam(point.x, point.y));

                            API.SendMessage(this.Handle,
                                API.WM_LBUTTONUP,
                                new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
                                API.MakeLParam(point.x, point.y));

                            e.Handled = true;
                        }
#if BIDI_SUPPORT
                        // ��������⴦�ڷ�����ź����ֶη���(31)֮��
                        API.PostMessage(this.Handle, WM_ADJUST_CARET, 0, 0);
#endif
                    }
                    break;
                case Keys.Down:
                    {
                        int x, y;
                        API.GetEditCurrentCaretPos(this,
                            out x,
                            out y);
                        int nTemp = API.GetEditLines(this);
                        if (y >= nTemp - 1
                            && this.MarcEditor.FocusedFieldIndex < this.MarcEditor.Record.Fields.Count - 1)
                        {
                            this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex + 1, this.MarcEditor.m_nFocusCol);

                            POINT point = new POINT();
                            point.x = 0;
                            point.y = 0;
                            bool bRet = API.GetCaretPos(ref point);

                            {
                                point.y = 1;
                                API.SendMessage(this.Handle,
                                    API.WM_LBUTTONDOWN,
                                    new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
                                    API.MakeLParam(point.x, point.y));

                                API.SendMessage(this.Handle,
                                    API.WM_LBUTTONUP,
                                    new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
                                    API.MakeLParam(point.x, point.y));
                            }
                            e.Handled = true;


                        }
#if BIDI_SUPPORT
                        // ��������⴦�ڷ�����ź����ֶη���(31)֮��
                        API.PostMessage(this.Handle, WM_ADJUST_CARET, 0, 0);
#endif
                    }
                    break;
                case Keys.Left:
                    {
                        API.PostMessage(this.MarcEditor.Handle,
    MarcEditor.WM_LEFTRIGHT_MOVED,
    0,
    0);


                        if (this.SelectionStart != 0)
                        {
#if BIDI_SUPPORT
                            // ��������⴦�ڷ�����ź����ֶη���(31)֮��
                            if (this.SelectionLength == 0)
                            {
                                if (IsInForbiddenPos(this.SelectionStart - 1) == true
                                    || IsInForbiddenPos(this.SelectionStart) == true)
                                    this.SelectionStart--;
                            }
#endif
                            break;
                        }

                        if (this.MarcEditor.m_nFocusCol == 1)
                        {
                            if (this.MarcEditor.FocusedFieldIndex > 0)
                            {
                                // ������һ�е�ĩβ
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex - 1, 3);
                                this.SelectionStart = this.Text.Length;
                                e.Handled = true;
                                return;
                            }
                        }
                        else if (this.MarcEditor.m_nFocusCol == 2)
                        {
                            // ��ָʾ�����ֶ���
                            this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 1);
                            this.SelectionStart = 2;
                            e.Handled = true;
                            return;
                        }
                        else if (this.MarcEditor.m_nFocusCol == 3)
                        {
                            // �����ݵ�ָʾ��

                            // һ���ֶ�
                            if (Record.IsControlFieldName(this.MarcEditor.FocusedField.Name) == false)
                            {
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 2);
                                this.SelectionStart = 1;
                                e.Handled = true;
                                return;
                            }
                            else
                            {
                                // ͷ����
                                if (this.MarcEditor.FocusedField.Name != "###")
                                {
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 1);
                                    this.SelectionStart = 2;
                                    e.Handled = true;
                                    return;
                                }
                            }
                        }
                    }
                    break;
                case Keys.Right:    // �ҷ����
                    {
                        API.PostMessage(this.MarcEditor.Handle,
MarcEditor.WM_LEFTRIGHT_MOVED,
0,
0);

                        if (this.MarcEditor.m_nFocusCol == 1)
                        {
                            // ���ֶ�����ָʾ��
                            if (this.SelectionStart >= 2)
                            {
                                // �����ֶ�û��ָʾ��, ֱ�ӵ�����
                                if (Record.IsControlFieldName(this.MarcEditor.FocusedField.Name) == true)
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 3);
                                else
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 2);

                                this.SelectionStart = 0;
                                e.Handled = true;
                            }
                        }
                        else if (this.MarcEditor.m_nFocusCol == 2)
                        {
                            // ��ָʾ��������
                            if (this.SelectionStart == 1 || this.SelectionStart == 2)
                            {
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 3);
                                this.SelectionStart = 0;
                                e.Handled = true;
                            }
                        }
                        else if (this.MarcEditor.m_nFocusCol == 3)
                        {
                            // ������ĩβ����һ���ײ�
                            if (this.SelectionStart == this.Text.Length
                                && this.MarcEditor.FocusedFieldIndex < this.MarcEditor.Record.Fields.Count - 1)
                            {
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex + 1, 1);
                                this.SelectionStart = 0;
                                e.Handled = true;
                                break;
                            }

#if BIDI_SUPPORT
                            // ��������⴦�ڷ�����ź����ֶη���(31)֮��
                            if (this.SelectionLength == 0)
                            {
                                if (IsInForbiddenPos(this.SelectionStart + 1) == true)
                                    this.SelectionStart++;
                            }
#endif
                        }

                    }
                    break;
                case Keys.End:
                    {
                        if (this.MarcEditor.m_nFocusCol == 3)
                        {
                            // Ŀǰ����������
                            break;
                        }

                        // �ȵ�������
                        this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 3);

                        // ͷ����
                        if (this.MarcEditor.FocusedField.Name == "###")
                            this.SelectionStart = this.Text.Length - 1;
                        else
                            this.SelectionStart = this.Text.Length;

                        this.SelectionLength = 0;

                        e.Handled = true;
                        return;
                    }
                    break;
                case Keys.Home:
                    {
                        if (this.SelectionStart == 0)
                        {
                            if (this.MarcEditor.m_nFocusCol == 3)
                            {
                                // Ŀǰ����������
                                if (Record.IsControlFieldName(this.MarcEditor.FocusedField.Name) == true)
                                {
                                    // �����ֶ�,���ֶ�����
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 1);
                                }
                                else
                                {
                                    // һ���ֶ�,��ָʾ����
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 2);
                                }
                                this.SelectionStart = 0;
                                e.Handled = true;
                                return;
                            }
                            else if (this.MarcEditor.m_nFocusCol == 2)
                            {
                                // Ŀǰ����ָʾ����, �Ǿ͵��ֶ�����
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 1);
                                this.SelectionStart = 0;
                                e.Handled = true;
                                return;
                            }
                            else if (this.MarcEditor.m_nFocusCol == 1)
                            {
                                // Ŀǰ�����ֶ�����, �Ǿ͵�������
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 3);
                                this.SelectionStart = 0;
                                e.Handled = true;
                                return;
                            }
                        }
                    }
                    break;
                case Keys.PageUp:
                    {
#if NO
                        if (this.MarcEditor.Record.Fields.Count > 0)
                        {
                            this.MarcEditor.SetActiveField(0, 3);
                            this.SelectionStart = 0;
                            e.Handled = true;
                            return;
                        }
#endif

                        this.MarcEditor.DocumentOrgY += this.MarcEditor.ClientRectangle.Height;

                        int x = this.MarcEditor.Record.NameCaptionTotalWidth + this.MarcEditor.Record.NameTotalWidth + this.MarcEditor.Record.IndicatorTotalWidth + 8 - this.MarcEditor.DocumentOrgX;

                        POINT point = new POINT();
                        point.x = 0;
                        point.y = 0;
                        bool bRet = API.GetCaretPos(ref point);


                        if (point.x + this.Location.X - this.MarcEditor.DocumentOrgX > x)
                            x = point.x + this.Location.X - this.MarcEditor.DocumentOrgX;

                        int y = this.MarcEditor.Font.Height / 2;

                        API.PostMessage(this.MarcEditor.Handle,
                            API.WM_LBUTTONDOWN,
                            new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
                            API.MakeLParam(x, y));
                        API.PostMessage(this.MarcEditor.Handle,
API.WM_LBUTTONUP,
new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
API.MakeLParam(x, y));

                        API.PostMessage(this.MarcEditor.Handle,
    API.WM_LBUTTONDOWN,
    new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
    API.MakeLParam(x, y));
                        API.PostMessage(this.MarcEditor.Handle,
API.WM_LBUTTONUP,
new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
API.MakeLParam(x, y));
                        e.Handled = true;
                    }
                    break;
                case Keys.PageDown:
                    {
                        this.MarcEditor.DocumentOrgY -= this.MarcEditor.ClientRectangle.Height;
                        /*
                        {
                            if (this.MarcEditor.Record.Fields.Count > 0)
                            {
                                this.MarcEditor.SetActiveField(this.MarcEditor.Record.Fields.Count - 1, 3);
                                this.SelectionStart = this.Text.Length;
                                e.Handled = true;
                                return;
                            }
                        }
                         * */
                        int x = this.MarcEditor.Record.NameCaptionTotalWidth + this.MarcEditor.Record.NameTotalWidth + this.MarcEditor.Record.IndicatorTotalWidth + 8 - this.MarcEditor.DocumentOrgX;

                        POINT point = new POINT();
                        point.x = 0;
                        point.y = 0;
                        bool bRet = API.GetCaretPos(ref point);


                        if (point.x + this.Location.X - this.MarcEditor.DocumentOrgX > x)
                            x = point.x + this.Location.X - this.MarcEditor.DocumentOrgX;

                        int y = this.MarcEditor.Font.Height / 2;


                        API.PostMessage(this.MarcEditor.Handle,
                            API.WM_LBUTTONDOWN,
                            new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
                            API.MakeLParam(x, y));
                        API.PostMessage(this.MarcEditor.Handle,
API.WM_LBUTTONUP,
new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
API.MakeLParam(x, y));


                        API.PostMessage(this.MarcEditor.Handle,
    API.WM_LBUTTONDOWN,
    new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
    API.MakeLParam(x, y));
                        API.PostMessage(this.MarcEditor.Handle,
API.WM_LBUTTONUP,
new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
API.MakeLParam(x, y));

                        e.Handled = true;
                    }
                    break;
                case Keys.Insert:
                    {
                        if (this.MarcEditor.m_nFocusCol == 1
                            || this.MarcEditor.m_nFocusCol == 2)
                        {
                            if (this.MarcEditor.ReadOnly == true)
                            {
                                Console.Beep();
                                break;
                            }
                            // parameters:
                            //      nAutoComplate   0: false; 1: true; -1:���ֵ�ǰ����״̬
                            bool bDone = this.MarcEditor.InsertField(this.MarcEditor.FocusedFieldIndex, -1/*ԭ����1*/, -1);    // true, false
                            if (bDone == false)
                                break;
                        }
                    }
                    break;
                case Keys.Delete:
                    {
                        if (this.MarcEditor.m_nFocusCol == 1
                            || this.MarcEditor.m_nFocusCol == 2)
                        {
                            if (this.MarcEditor.ReadOnly == true)
                            {
                                Console.Beep();
                                break;
                            }
                            // �� �ֶ��� �� ָʾ�� λ��
                            int nStart = this.SelectionStart;
                            bool bRemoved = this.MarcEditor.DeleteFieldWithDlg();
                            if (bRemoved == false)
                            {
                                /*
                                if (nStart < this.Text.Length)
                                {
                                    string strOneChar = this.Text.Substring(nStart, 1);
                                    this.Text = this.Text.Insert(this.Text.Length, strOneChar);
                                }
                                this.SelectionStart = nStart;
                                 */
                                e.Handled = true;
                                return;
                            }
                            else
                            {
                                /*
                                if (nStart < this.Text.Length)
                                {
                                    string strOneChar = this.Text.Substring(nStart, 1);
                                    this.Text = this.Text.Insert(nStart, strOneChar);
                                }
                                 */
                                this.SelectionStart = nStart;
                                e.Handled = true;
                                return;
                            }
                        }
                        else
                        {
                            if (this.Overwrite == true)
                            {
                                int nStart = this.SelectionStart;
                                // this.Text = this.Text.Insert(this.Text.Length, " ");
                                if (nStart >= this.Text.Length)
                                {
                                    e.Handled = true;
                                    Console.Beep();
                                    return;
                                }


                                this.Text = this.Text.Insert(nStart, " ");
#if BIDI_SUPPORT
                                if (this.Text[nStart + 1] == 0x200e
                                    && this.Text.Length >= nStart + 2 + 1)
                                    this.Text = this.Text.Remove(nStart + 1, 2);
                                else
                                    this.Text = this.Text.Remove(nStart + 1, 1);
#else

                                this.Text = this.Text.Remove(nStart + 1, 1);
#endif

                                this.SelectionStart = nStart;
                                e.Handled = true;
                                return;
                            }

#if BIDI_SUPPORT
                            if (this.SelectionLength == 0
                                && this.SelectionStart < this.Text.Length)
                            {
                                // ���ɾ���������Ƿ����ַ�
                                if (this.Text[this.SelectionStart] == 0x200e
                                    && this.Text.Length >= this.SelectionStart + 1 + 1)
                                {
                                    // һͬɾ��
                                    int nStart = this.SelectionStart;
                                    this.Text = this.Text.Remove(this.SelectionStart, 2);
                                    this.SelectionStart = nStart;
                                    e.Handled = true;
                                }
                                // ���ɾ��λ�õ��������Ƿ����ַ�
                                else if (this.SelectionStart > 0
                                    && this.Text[this.SelectionStart - 1] == 0x200e)
                                {
                                    // һͬɾ��
                                    int nStart = this.SelectionStart;
                                    this.Text = this.Text.Remove(this.SelectionStart - 1, 2);
                                    this.SelectionStart = nStart - 1;
                                    e.Handled = true;
                                }
                            }
#endif

                        }
                    }
                    break;
                default:
                    break;
            }

            base.OnKeyDown(e);
        }

        void SendKeyUp()
        {
            POINT point = new POINT();
            point.x = 0;
            point.y = 0;
            API.GetCaretPos(ref point);
            API.SendMessage(this.Handle,
                API.WM_LBUTTONUP,
                new UIntPtr(API.MK_LBUTTON),	//	UIntPtr wParam,
                API.MakeLParam(point.x, point.y));
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageUp:
                case Keys.PageDown:
                    e.Handled = true;
                    return;
            }

            //������Shiftʱ����MarcEditor��Shiftѡ����ʼλ����Ϊ0
            if (e.Shift == false)
                this.MarcEditor.nStartFieldIndex = -1;

            base.OnKeyUp(e);
            this.MarcEditor.Flush();    // 2011/8/8

            switch (e.KeyCode)
            {
                case Keys.PageUp:
                case Keys.PageDown:
                    e.Handled = true;
                    return;
                case Keys.OemPipe:
                    {
                        /*
                        if (this.m_marcEditor.m_nFocusCol != 3)
                            break;

                        int nOldStart = this.SelectionStart;

                        string strTemp = this.Text;
                        strTemp = strTemp.Remove(nOldStart-1, 1);
                        strTemp = strTemp.Insert(nOldStart - 1, new string((char)Record.KERNEL_SUBFLD,1) );

                        this.Text = strTemp;
                        this.SelectionStart = nOldStart;
                         */

                    }
                    break;

                // �������Ҽ�ʱ
                case Keys.Left:
                    {
                        if (this.SelectionStart != 0)
                            break;

                        /*
                        if (this.MarcEditor.m_nFocusCol == 1)
                        {
                        }
                        else if (this.MarcEditor.m_nFocusCol == 2)
                        {
                            this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 1);
                            this.SelectionStart = 3;
                        }
                        else if (this.MarcEditor.m_nFocusCol == 3)
                        {

                            if (Record.IsControlFieldName(this.MarcEditor.FocusedField.Name) == false)
                            {
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 2);
                                this.SelectionStart = 2;
                            }
                            else
                            {
                                if (this.MarcEditor.FocusedField.Name != "###")
                                {
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 1);
                                    this.SelectionStart = 3;
                                }
                            }
                        }
                         */
                    }
                    break;
                case Keys.Right:
                    {
                        /*
                        if (this.MarcEditor.m_nFocusCol == 1)
                        {
                            if (this.SelectionStart == 3)
                            {
                                if (Record.IsControlFieldName(this.MarcEditor.FocusedField.Name) == true)
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 3);
                                else
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 2);
                                this.SelectionStart = 0;
                            }
                        }
                        else if (this.MarcEditor.m_nFocusCol == 2)
                        {
                            if (this.SelectionStart == 2)
                            {
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 3);
                                this.SelectionStart = 0;
                            }
                        }
                        else if (this.MarcEditor.m_nFocusCol == 3)
                        {
                            if (this.SelectionStart == this.Text.Length
                                && this.MarcEditor.FocusedFieldIndex < this.MarcEditor.Record.Fields.Count - 1)
                            {
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex + 1, 1);
                                this.SelectionStart = 0;
                            }
                        }
                         */
                    }
                    break;
                case Keys.Delete:
                    {
                        /*
                        if (this.MarcEditor.m_nFocusCol == 1
                            || this.MarcEditor.m_nFocusCol == 2)
                        {
                            int nStart = this.SelectionStart;
                            bool bRemove = this.MarcEditor.DeleteFieldWithDlg();
                            if (bRemove == false)
                            {
                                if (nStart < this.Text.Length)
                                {
                                    string strOneChar = this.Text.Substring(nStart, 1);
                                    this.Text = this.Text.Insert(this.Text.Length, strOneChar);
                                }
                                this.SelectionStart = nStart;
                            }
                            else
                            {

                                this.SelectionStart = nStart;
                            }
                        }
                        else
                        {
                            if (this.Overwrite == true)
                            {
                                int nStart = this.SelectionStart;
                                this.Text = this.Text.Insert(this.Text.Length, " ");
                                this.SelectionStart = nStart;
                            }
                        }
                         */

                    }
                    break;
                case Keys.Up:
                case Keys.Down:
                    break;

                // ���������������KeyUpʱ�������value������������
                // ������ֶ����������3������λ�ö�λ���ֶ�ָʾ��
                // �����ָʾ���������2������λ�ö�λ���ֶ�ֵ��
                default:
                    {
                        this.MarcEditor.Flush();

                        if (this.m_marcEditor.m_nFocusCol == 1)
                        {
                            // ��Ŀǰ��������ֶ���ʱ
                            if (this.SelectionStart == 3)
                            {
                                if (Record.IsControlFieldName(this.MarcEditor.FocusedField.Name) == true)
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 3);
                                else
                                    this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 2);

                                // this.SelectionStart = 0;
                                // return; 
                            }
                        }
                        if (this.m_marcEditor.m_nFocusCol == 2)
                        {
                            // ��Ŀǰ��������ֶ�ָʾ��ʱ
                            if (this.SelectionStart == 2)
                            {
                                Debug.Assert(Record.IsControlFieldName(this.MarcEditor.FocusedField.Name) == false, "�к�Ϊ2ʱ,��ӦΪ�����ֶ�");
                                this.MarcEditor.SetActiveField(this.MarcEditor.FocusedFieldIndex, 3);
                            }
                        }
                        else if (this.m_marcEditor.m_nFocusCol == 3)
                        {
                            // ��Ŀǰ��������ֶ�ֵʱ
                            bool bChangedHeight = false;
                            API.SendMessage(this.Handle,
                                API.EM_LINESCROLL,
                                0,
                                (int)1000);	// 0x7fffffff
                            while (true)
                            {
                                int nFirstLine = API.GetEditFirstVisibleLine(this);
                                if (nFirstLine != 0)
                                {
                                    bChangedHeight = true;
                                    this.Size = new Size(this.Size.Width, this.Size.Height + 10);
                                }
                                else
                                    break;
                            }
                            if (bChangedHeight)
                            {
                                this.MarcEditor.AfterItemHeightChanged(this.MarcEditor.FocusedFieldIndex,
                                    -1);
                            }
                        }
                    }
                    break;
            }

            // �ò������λ�ÿɼ�
            // this.Focus();
            int nHeight = 20;

            /*
            if (this.MarcEditor.curEdit != null)
                nHeight = Math.Max(20, this.MarcEditor.curEdit.Height + 8);
             * */
            nHeight = Math.Max(20, this.MarcEditor.Font.Height);

            POINT point = new POINT();
            point.x = 0;
            point.y = 0;
            bool bRet = API.GetCaretPos(ref point);
            Rectangle rect = new Rectangle(point.x - 10,
                point.y,
                20,
                nHeight);
            // parameter:
            //		nCol	�к� 
            //				0 �ֶ�˵��;
            //				1 �ֶ���;
            //				2 �ֶ�ָʾ��
            //				3 �ֶ��ڲ�
            this.MarcEditor.EnsureVisible(this.MarcEditor.FocusedFieldIndex,
                3,
                rect);
        }

#if NO
        public override void OnHeightChanged()
        {
            this.MarcEditor.AfterItemHeightChanged(this.MarcEditor.FocusedFieldIndex,
    -1);

            base.OnHeightChanged();
        }
#endif

        public void EnsureVisible()
        {
            // �ò������λ�ÿɼ�
            int nHeight = 20;

            /*
            if (this.MarcEditor.curEdit != null)
                nHeight = Math.Max(20, this.MarcEditor.curEdit.Height + 8);
             * */
            nHeight = Math.Max(20, this.MarcEditor.Font.Height);

            POINT point = new POINT();
            point.x = 0;
            point.y = 0;
            bool bRet = API.GetCaretPos(ref point);
            Rectangle rect = new Rectangle(point.x - 10,
                point.y,
                20,
                nHeight);
            // parameter:
            //		nCol	�к� 
            //				0 �ֶ�˵��;
            //				1 �ֶ���;
            //				2 �ֶ�ָʾ��
            //				3 �ֶ��ڲ�
            this.MarcEditor.EnsureVisible(this.MarcEditor.FocusedFieldIndex,
                3,
                rect);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            this.SelectionLength = 0;

            Debug.WriteLine("little edit Enter");
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            Debug.WriteLine("little edit leave");
        }
    }
}
