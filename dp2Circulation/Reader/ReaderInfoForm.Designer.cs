namespace dp2Circulation
{
    partial class ReaderInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReaderInfoForm));
            this.splitContainer_normal = new System.Windows.Forms.SplitContainer();
            this.readerEditControl1 = new dp2Circulation.ReaderEditControl();
            this.webBrowser_readerInfo = new System.Windows.Forms.WebBrowser();
            this.tabControl_readerInfo = new System.Windows.Forms.TabControl();
            this.tabPage_normal = new System.Windows.Forms.TabPage();
            this.tabPage_xml = new System.Windows.Forms.TabPage();
            this.webBrowser_xml = new System.Windows.Forms.WebBrowser();
            this.tabPage_objects = new System.Windows.Forms.TabPage();
            this.binaryResControl1 = new DigitalPlatform.CirculationClient.BinaryResControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_delete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_loadFromIdcard = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_loadBlank = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton_loadBlank = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem_loadBlankFromLocal = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_loadBlankFromServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_webCamera = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_pasteCardPhoto = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_registerFingerprint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_createMoneyRecord = new System.Windows.Forms.ToolStripDropDownButton();
            this.ToolStripMenuItem_hire = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_foregift = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_returnForegift = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_saveTo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_save = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_next = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_prev = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_stopSummaryLoop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_addFriends = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_clearOutofReservationCount = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton_otherFunc = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripButton_saveTemplate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_exportDetailToExcelFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_exportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_exportBorrowingBarcode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_moveRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_clearFingerprint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_option = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_load = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox_barcode = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton_load = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel_main = new System.Windows.Forms.TableLayoutPanel();
            this.tabPage_borrowHistory = new System.Windows.Forms.TabPage();
            this.webBrowser_borrowHistory = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_normal)).BeginInit();
            this.splitContainer_normal.Panel1.SuspendLayout();
            this.splitContainer_normal.Panel2.SuspendLayout();
            this.splitContainer_normal.SuspendLayout();
            this.tabControl_readerInfo.SuspendLayout();
            this.tabPage_normal.SuspendLayout();
            this.tabPage_xml.SuspendLayout();
            this.tabPage_objects.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip_load.SuspendLayout();
            this.tableLayoutPanel_main.SuspendLayout();
            this.tabPage_borrowHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer_normal
            // 
            this.splitContainer_normal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_normal.Location = new System.Drawing.Point(2, 2);
            this.splitContainer_normal.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer_normal.Name = "splitContainer_normal";
            // 
            // splitContainer_normal.Panel1
            // 
            this.splitContainer_normal.Panel1.Controls.Add(this.readerEditControl1);
            // 
            // splitContainer_normal.Panel2
            // 
            this.splitContainer_normal.Panel2.Controls.Add(this.webBrowser_readerInfo);
            this.splitContainer_normal.Size = new System.Drawing.Size(594, 253);
            this.splitContainer_normal.SplitterDistance = 318;
            this.splitContainer_normal.SplitterWidth = 3;
            this.splitContainer_normal.TabIndex = 5;
            // 
            // readerEditControl1
            // 
            this.readerEditControl1.Access = "";
            this.readerEditControl1.Address = "";
            this.readerEditControl1.Barcode = "";
            this.readerEditControl1.CardNumber = "";
            this.readerEditControl1.Changed = false;
            this.readerEditControl1.Comment = "";
            this.readerEditControl1.CreateDate = "";
            this.readerEditControl1.CreateState = dp2Circulation.ItemDisplayState.Normal;
            this.readerEditControl1.DateOfBirth = "";
            this.readerEditControl1.Department = "";
            this.readerEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.readerEditControl1.Email = "";
            this.readerEditControl1.ExpireDate = "";
            this.readerEditControl1.Fingerprint = "";
            this.readerEditControl1.FingerprintVersion = "";
            this.readerEditControl1.Foregift = "";
            this.readerEditControl1.Friends = "";
            this.readerEditControl1.Gender = "";
            this.readerEditControl1.HireExpireDate = "";
            this.readerEditControl1.HirePeriod = "";
            this.readerEditControl1.IdCardNumber = "";
            this.readerEditControl1.Initializing = true;
            this.readerEditControl1.Location = new System.Drawing.Point(0, 0);
            this.readerEditControl1.Margin = new System.Windows.Forms.Padding(2);
            this.readerEditControl1.Name = "readerEditControl1";
            this.readerEditControl1.NamePinyin = "";
            this.readerEditControl1.NameString = "";
            this.readerEditControl1.ParentId = "";
            this.readerEditControl1.PersonalLibrary = "";
            this.readerEditControl1.Post = "";
            this.readerEditControl1.ReaderType = "";
            this.readerEditControl1.RecPath = "";
            this.readerEditControl1.RefID = "";
            this.readerEditControl1.Rights = "";
            this.readerEditControl1.Size = new System.Drawing.Size(318, 253);
            this.readerEditControl1.State = "";
            this.readerEditControl1.TabIndex = 0;
            this.readerEditControl1.Tel = "";
            this.readerEditControl1.GetLibraryCode += new dp2Circulation.GetLibraryCodeEventHandler(this.readerEditControl1_GetLibraryCode);
            this.readerEditControl1.CreatePinyin += new System.EventHandler(this.readerEditControl1_CreatePinyin);
            this.readerEditControl1.EditRights += new System.EventHandler(this.readerEditControl1_EditRights);
            // 
            // webBrowser_readerInfo
            // 
            this.webBrowser_readerInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser_readerInfo.Location = new System.Drawing.Point(0, 0);
            this.webBrowser_readerInfo.Margin = new System.Windows.Forms.Padding(2);
            this.webBrowser_readerInfo.MinimumSize = new System.Drawing.Size(15, 16);
            this.webBrowser_readerInfo.Name = "webBrowser_readerInfo";
            this.webBrowser_readerInfo.Size = new System.Drawing.Size(273, 253);
            this.webBrowser_readerInfo.TabIndex = 0;
            this.webBrowser_readerInfo.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_readerInfo_DocumentCompleted);
            // 
            // tabControl_readerInfo
            // 
            this.tabControl_readerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl_readerInfo.Controls.Add(this.tabPage_normal);
            this.tabControl_readerInfo.Controls.Add(this.tabPage_borrowHistory);
            this.tabControl_readerInfo.Controls.Add(this.tabPage_xml);
            this.tabControl_readerInfo.Controls.Add(this.tabPage_objects);
            this.tabControl_readerInfo.Location = new System.Drawing.Point(2, 27);
            this.tabControl_readerInfo.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl_readerInfo.Name = "tabControl_readerInfo";
            this.tabControl_readerInfo.SelectedIndex = 0;
            this.tabControl_readerInfo.Size = new System.Drawing.Size(606, 283);
            this.tabControl_readerInfo.TabIndex = 3;
            // 
            // tabPage_normal
            // 
            this.tabPage_normal.Controls.Add(this.splitContainer_normal);
            this.tabPage_normal.Location = new System.Drawing.Point(4, 22);
            this.tabPage_normal.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_normal.Name = "tabPage_normal";
            this.tabPage_normal.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_normal.Size = new System.Drawing.Size(598, 257);
            this.tabPage_normal.TabIndex = 0;
            this.tabPage_normal.Text = "����";
            this.tabPage_normal.UseVisualStyleBackColor = true;
            // 
            // tabPage_xml
            // 
            this.tabPage_xml.Controls.Add(this.webBrowser_xml);
            this.tabPage_xml.Location = new System.Drawing.Point(4, 22);
            this.tabPage_xml.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_xml.Name = "tabPage_xml";
            this.tabPage_xml.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_xml.Size = new System.Drawing.Size(598, 257);
            this.tabPage_xml.TabIndex = 1;
            this.tabPage_xml.Text = "XML";
            this.tabPage_xml.UseVisualStyleBackColor = true;
            // 
            // webBrowser_xml
            // 
            this.webBrowser_xml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser_xml.Location = new System.Drawing.Point(2, 2);
            this.webBrowser_xml.Margin = new System.Windows.Forms.Padding(2);
            this.webBrowser_xml.MinimumSize = new System.Drawing.Size(15, 16);
            this.webBrowser_xml.Name = "webBrowser_xml";
            this.webBrowser_xml.Size = new System.Drawing.Size(594, 253);
            this.webBrowser_xml.TabIndex = 0;
            // 
            // tabPage_objects
            // 
            this.tabPage_objects.Controls.Add(this.binaryResControl1);
            this.tabPage_objects.Location = new System.Drawing.Point(4, 22);
            this.tabPage_objects.Name = "tabPage_objects";
            this.tabPage_objects.Size = new System.Drawing.Size(598, 257);
            this.tabPage_objects.TabIndex = 2;
            this.tabPage_objects.Text = "����";
            this.tabPage_objects.UseVisualStyleBackColor = true;
            // 
            // binaryResControl1
            // 
            this.binaryResControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.binaryResControl1.BiblioRecPath = "";
            this.binaryResControl1.Changed = false;
            this.binaryResControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.binaryResControl1.ErrorInfo = "";
            this.binaryResControl1.Location = new System.Drawing.Point(0, 0);
            this.binaryResControl1.Margin = new System.Windows.Forms.Padding(2);
            this.binaryResControl1.Name = "binaryResControl1";
            this.binaryResControl1.RightsCfgFileName = null;
            this.binaryResControl1.Size = new System.Drawing.Size(598, 257);
            this.binaryResControl1.TabIndex = 1;
            this.binaryResControl1.TempDir = null;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_delete,
            this.toolStripButton_loadFromIdcard,
            this.toolStripButton_loadBlank,
            this.toolStripDropDownButton_loadBlank,
            this.toolStripButton_webCamera,
            this.toolStripButton_pasteCardPhoto,
            this.toolStripButton_registerFingerprint,
            this.toolStripSeparator1,
            this.toolStripButton_createMoneyRecord,
            this.toolStripSeparator3,
            this.toolStripButton_saveTo,
            this.toolStripButton_save,
            this.toolStripButton_next,
            this.toolStripButton_prev,
            this.toolStripSeparator4,
            this.toolStripButton_stopSummaryLoop,
            this.toolStripSeparator5,
            this.toolStripButton_addFriends,
            this.toolStripButton_clearOutofReservationCount,
            this.toolStripSeparator2,
            this.toolStripDropDownButton_otherFunc});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 312);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(610, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_delete
            // 
            this.toolStripButton_delete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_delete.Image")));
            this.toolStripButton_delete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_delete.Name = "toolStripButton_delete";
            this.toolStripButton_delete.Size = new System.Drawing.Size(52, 22);
            this.toolStripButton_delete.Text = "ɾ��";
            this.toolStripButton_delete.Click += new System.EventHandler(this.toolStripButton_delete_Click);
            // 
            // toolStripButton_loadFromIdcard
            // 
            this.toolStripButton_loadFromIdcard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_loadFromIdcard.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_loadFromIdcard.Image")));
            this.toolStripButton_loadFromIdcard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_loadFromIdcard.Name = "toolStripButton_loadFromIdcard";
            this.toolStripButton_loadFromIdcard.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_loadFromIdcard.Text = "�����֤������Ϣ";
            this.toolStripButton_loadFromIdcard.ToolTipText = "�����֤������Ϣ (��ס Ctrl �������ԭ������)";
            this.toolStripButton_loadFromIdcard.Click += new System.EventHandler(this.toolStripButton_loadFromIdcard_Click);
            // 
            // toolStripButton_loadBlank
            // 
            this.toolStripButton_loadBlank.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_loadBlank.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_loadBlank.Image")));
            this.toolStripButton_loadBlank.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_loadBlank.Name = "toolStripButton_loadBlank";
            this.toolStripButton_loadBlank.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_loadBlank.Text = "�ӱ���װ��հ׼�¼";
            this.toolStripButton_loadBlank.Click += new System.EventHandler(this.toolStripButton_loadBlank_Click);
            // 
            // toolStripDropDownButton_loadBlank
            // 
            this.toolStripDropDownButton_loadBlank.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripDropDownButton_loadBlank.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_loadBlankFromLocal,
            this.ToolStripMenuItem_loadBlankFromServer});
            this.toolStripDropDownButton_loadBlank.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton_loadBlank.Image")));
            this.toolStripDropDownButton_loadBlank.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton_loadBlank.Name = "toolStripDropDownButton_loadBlank";
            this.toolStripDropDownButton_loadBlank.Size = new System.Drawing.Size(13, 22);
            this.toolStripDropDownButton_loadBlank.Text = "װ��հ׼�¼";
            // 
            // toolStripMenuItem_loadBlankFromLocal
            // 
            this.toolStripMenuItem_loadBlankFromLocal.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem_loadBlankFromLocal.Image")));
            this.toolStripMenuItem_loadBlankFromLocal.Name = "toolStripMenuItem_loadBlankFromLocal";
            this.toolStripMenuItem_loadBlankFromLocal.Size = new System.Drawing.Size(196, 22);
            this.toolStripMenuItem_loadBlankFromLocal.Text = "�ӱ���װ��հ׼�¼";
            this.toolStripMenuItem_loadBlankFromLocal.ToolTipText = "�ӱ���װ��հ׼�¼";
            this.toolStripMenuItem_loadBlankFromLocal.Click += new System.EventHandler(this.toolStripMenuItem_loadBlankFromLocal_Click);
            // 
            // ToolStripMenuItem_loadBlankFromServer
            // 
            this.ToolStripMenuItem_loadBlankFromServer.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripMenuItem_loadBlankFromServer.Image")));
            this.ToolStripMenuItem_loadBlankFromServer.Name = "ToolStripMenuItem_loadBlankFromServer";
            this.ToolStripMenuItem_loadBlankFromServer.Size = new System.Drawing.Size(196, 22);
            this.ToolStripMenuItem_loadBlankFromServer.Text = "�ӷ�����װ��հ׼�¼";
            this.ToolStripMenuItem_loadBlankFromServer.ToolTipText = "�ӷ�����װ��հ׼�¼";
            this.ToolStripMenuItem_loadBlankFromServer.Click += new System.EventHandler(this.ToolStripMenuItem_loadBlankFromServer_Click);
            // 
            // toolStripButton_webCamera
            // 
            this.toolStripButton_webCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_webCamera.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_webCamera.Image")));
            this.toolStripButton_webCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_webCamera.Name = "toolStripButton_webCamera";
            this.toolStripButton_webCamera.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_webCamera.Text = "������ͷ��ȡͼ��";
            this.toolStripButton_webCamera.Click += new System.EventHandler(this.toolStripButton_webCamera_Click);
            // 
            // toolStripButton_pasteCardPhoto
            // 
            this.toolStripButton_pasteCardPhoto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_pasteCardPhoto.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_pasteCardPhoto.Image")));
            this.toolStripButton_pasteCardPhoto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_pasteCardPhoto.Name = "toolStripButton_pasteCardPhoto";
            this.toolStripButton_pasteCardPhoto.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_pasteCardPhoto.Text = "(�Ӽ�����)ճ��֤����";
            this.toolStripButton_pasteCardPhoto.Click += new System.EventHandler(this.toolStripButton_pasteCardPhoto_Click);
            // 
            // toolStripButton_registerFingerprint
            // 
            this.toolStripButton_registerFingerprint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_registerFingerprint.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_registerFingerprint.Image")));
            this.toolStripButton_registerFingerprint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_registerFingerprint.Name = "toolStripButton_registerFingerprint";
            this.toolStripButton_registerFingerprint.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_registerFingerprint.Text = "�Ǽ�ָ��";
            this.toolStripButton_registerFingerprint.Click += new System.EventHandler(this.toolStripButton_registerFingerprint_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton_createMoneyRecord
            // 
            this.toolStripButton_createMoneyRecord.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_hire,
            this.ToolStripMenuItem_foregift,
            this.ToolStripMenuItem_returnForegift});
            this.toolStripButton_createMoneyRecord.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_createMoneyRecord.Image")));
            this.toolStripButton_createMoneyRecord.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(193)))));
            this.toolStripButton_createMoneyRecord.Name = "toolStripButton_createMoneyRecord";
            this.toolStripButton_createMoneyRecord.Size = new System.Drawing.Size(109, 22);
            this.toolStripButton_createMoneyRecord.Text = "������������";
            // 
            // ToolStripMenuItem_hire
            // 
            this.ToolStripMenuItem_hire.Name = "ToolStripMenuItem_hire";
            this.ToolStripMenuItem_hire.Size = new System.Drawing.Size(112, 22);
            this.ToolStripMenuItem_hire.Text = "�����";
            this.ToolStripMenuItem_hire.Click += new System.EventHandler(this.ToolStripMenuItem_hire_Click);
            // 
            // ToolStripMenuItem_foregift
            // 
            this.ToolStripMenuItem_foregift.Name = "ToolStripMenuItem_foregift";
            this.ToolStripMenuItem_foregift.Size = new System.Drawing.Size(112, 22);
            this.ToolStripMenuItem_foregift.Text = "��Ѻ��";
            this.ToolStripMenuItem_foregift.Click += new System.EventHandler(this.ToolStripMenuItem_foregift_Click);
            // 
            // ToolStripMenuItem_returnForegift
            // 
            this.ToolStripMenuItem_returnForegift.Name = "ToolStripMenuItem_returnForegift";
            this.ToolStripMenuItem_returnForegift.Size = new System.Drawing.Size(112, 22);
            this.ToolStripMenuItem_returnForegift.Text = "��Ѻ��";
            this.ToolStripMenuItem_returnForegift.Click += new System.EventHandler(this.ToolStripMenuItem_returnForegift_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton_saveTo
            // 
            this.toolStripButton_saveTo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_saveTo.Image")));
            this.toolStripButton_saveTo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_saveTo.Name = "toolStripButton_saveTo";
            this.toolStripButton_saveTo.Size = new System.Drawing.Size(52, 22);
            this.toolStripButton_saveTo.Text = "����";
            this.toolStripButton_saveTo.ToolTipText = "����¼����Ϊһ���µļ�¼";
            this.toolStripButton_saveTo.Click += new System.EventHandler(this.toolStripButton_saveTo_Click);
            // 
            // toolStripButton_save
            // 
            this.toolStripButton_save.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_save.Image")));
            this.toolStripButton_save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_save.Name = "toolStripButton_save";
            this.toolStripButton_save.Size = new System.Drawing.Size(52, 22);
            this.toolStripButton_save.Text = "����";
            this.toolStripButton_save.ToolTipText = "����ǰ�޸ĸ��Ǳ��浽���ݿ�";
            this.toolStripButton_save.Click += new System.EventHandler(this.toolStripButton_save_Click);
            // 
            // toolStripButton_next
            // 
            this.toolStripButton_next.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton_next.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_next.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_next.Image")));
            this.toolStripButton_next.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(193)))));
            this.toolStripButton_next.Name = "toolStripButton_next";
            this.toolStripButton_next.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_next.Text = "��һ��¼";
            this.toolStripButton_next.Click += new System.EventHandler(this.toolStripButton_next_Click);
            // 
            // toolStripButton_prev
            // 
            this.toolStripButton_prev.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton_prev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_prev.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_prev.Image")));
            this.toolStripButton_prev.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(193)))));
            this.toolStripButton_prev.Name = "toolStripButton_prev";
            this.toolStripButton_prev.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_prev.Text = "ǰһ��¼";
            this.toolStripButton_prev.Click += new System.EventHandler(this.toolStripButton_prev_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton_stopSummaryLoop
            // 
            this.toolStripButton_stopSummaryLoop.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton_stopSummaryLoop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_stopSummaryLoop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_stopSummaryLoop.Image")));
            this.toolStripButton_stopSummaryLoop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_stopSummaryLoop.Name = "toolStripButton_stopSummaryLoop";
            this.toolStripButton_stopSummaryLoop.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_stopSummaryLoop.Text = "ֹͣװ����ĿժҪ";
            this.toolStripButton_stopSummaryLoop.Click += new System.EventHandler(this.toolStripButton_stopSummaryLoop_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton_addFriends
            // 
            this.toolStripButton_addFriends.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_addFriends.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_addFriends.Image")));
            this.toolStripButton_addFriends.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_addFriends.Name = "toolStripButton_addFriends";
            this.toolStripButton_addFriends.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_addFriends.Text = "�Ӻ���";
            this.toolStripButton_addFriends.Click += new System.EventHandler(this.toolStripButton_addFriends_Click);
            // 
            // toolStripButton_clearOutofReservationCount
            // 
            this.toolStripButton_clearOutofReservationCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_clearOutofReservationCount.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_clearOutofReservationCount.Image")));
            this.toolStripButton_clearOutofReservationCount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_clearOutofReservationCount.Name = "toolStripButton_clearOutofReservationCount";
            this.toolStripButton_clearOutofReservationCount.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_clearOutofReservationCount.Text = "���ԤԼ�����δȡ����";
            this.toolStripButton_clearOutofReservationCount.Click += new System.EventHandler(this.toolStripButton_clearOutofReservationCount_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButton_otherFunc
            // 
            this.toolStripDropDownButton_otherFunc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton_otherFunc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_saveTemplate,
            this.toolStripSeparator8,
            this.toolStripMenuItem_exportDetailToExcelFile,
            this.toolStripMenuItem_exportExcel,
            this.ToolStripMenuItem_exportBorrowingBarcode,
            this.toolStripSeparator7,
            this.toolStripMenuItem_moveRecord,
            this.toolStripMenuItem_clearFingerprint,
            this.toolStripSeparator6,
            this.toolStripButton_option});
            this.toolStripDropDownButton_otherFunc.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton_otherFunc.Image")));
            this.toolStripDropDownButton_otherFunc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton_otherFunc.Name = "toolStripDropDownButton_otherFunc";
            this.toolStripDropDownButton_otherFunc.Size = new System.Drawing.Size(30, 22);
            this.toolStripDropDownButton_otherFunc.Text = "...";
            this.toolStripDropDownButton_otherFunc.ToolTipText = "��������...";
            // 
            // toolStripButton_saveTemplate
            // 
            this.toolStripButton_saveTemplate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_saveTemplate.Image")));
            this.toolStripButton_saveTemplate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_saveTemplate.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(193)))));
            this.toolStripButton_saveTemplate.Name = "toolStripButton_saveTemplate";
            this.toolStripButton_saveTemplate.Size = new System.Drawing.Size(142, 26);
            this.toolStripButton_saveTemplate.Text = "������߼�¼��ģ��";
            this.toolStripButton_saveTemplate.Click += new System.EventHandler(this.toolStripButton_saveTemplate_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(253, 6);
            // 
            // toolStripMenuItem_exportDetailToExcelFile
            // 
            this.toolStripMenuItem_exportDetailToExcelFile.Name = "toolStripMenuItem_exportDetailToExcelFile";
            this.toolStripMenuItem_exportDetailToExcelFile.Size = new System.Drawing.Size(256, 22);
            this.toolStripMenuItem_exportDetailToExcelFile.Text = "����������Ϣ�� Excel �ļ�(&E)...";
            this.toolStripMenuItem_exportDetailToExcelFile.Click += new System.EventHandler(this.toolStripMenuItem_exportDetailToExcelFile_Click);
            // 
            // toolStripMenuItem_exportExcel
            // 
            this.toolStripMenuItem_exportExcel.Name = "toolStripMenuItem_exportExcel";
            this.toolStripMenuItem_exportExcel.Size = new System.Drawing.Size(256, 22);
            this.toolStripMenuItem_exportExcel.Text = "������ Excel �ļ�(&X)...";
            this.toolStripMenuItem_exportExcel.Visible = false;
            this.toolStripMenuItem_exportExcel.Click += new System.EventHandler(this.toolStripMenuItem_exportExcel_Click);
            // 
            // ToolStripMenuItem_exportBorrowingBarcode
            // 
            this.ToolStripMenuItem_exportBorrowingBarcode.Name = "ToolStripMenuItem_exportBorrowingBarcode";
            this.ToolStripMenuItem_exportBorrowingBarcode.Size = new System.Drawing.Size(256, 22);
            this.ToolStripMenuItem_exportBorrowingBarcode.Text = "�����ڽ������ŵ��ı��ļ�(&E)...";
            this.ToolStripMenuItem_exportBorrowingBarcode.Click += new System.EventHandler(this.ToolStripMenuItem_exportBorrowingBarcode_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(253, 6);
            // 
            // toolStripMenuItem_moveRecord
            // 
            this.toolStripMenuItem_moveRecord.Name = "toolStripMenuItem_moveRecord";
            this.toolStripMenuItem_moveRecord.Size = new System.Drawing.Size(256, 22);
            this.toolStripMenuItem_moveRecord.Text = "�ƶ����߼�¼(&M)";
            this.toolStripMenuItem_moveRecord.ToolTipText = "�ڶ��߿�֮���ƶ���¼";
            this.toolStripMenuItem_moveRecord.Click += new System.EventHandler(this.toolStripMenuItem_moveRecord_Click);
            // 
            // toolStripMenuItem_clearFingerprint
            // 
            this.toolStripMenuItem_clearFingerprint.Name = "toolStripMenuItem_clearFingerprint";
            this.toolStripMenuItem_clearFingerprint.Size = new System.Drawing.Size(256, 22);
            this.toolStripMenuItem_clearFingerprint.Text = "���ָ����Ϣ(&C)";
            this.toolStripMenuItem_clearFingerprint.Click += new System.EventHandler(this.toolStripMenuItem_clearFingerprint_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(253, 6);
            // 
            // toolStripButton_option
            // 
            this.toolStripButton_option.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_option.Image")));
            this.toolStripButton_option.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_option.Name = "toolStripButton_option";
            this.toolStripButton_option.Size = new System.Drawing.Size(52, 21);
            this.toolStripButton_option.Text = "ѡ��";
            this.toolStripButton_option.Click += new System.EventHandler(this.toolStripButton_option_Click);
            // 
            // toolStrip_load
            // 
            this.toolStrip_load.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip_load.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip_load.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBox_barcode,
            this.toolStripButton_load});
            this.toolStrip_load.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_load.Name = "toolStrip_load";
            this.toolStrip_load.Size = new System.Drawing.Size(250, 25);
            this.toolStrip_load.TabIndex = 5;
            this.toolStrip_load.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(59, 22);
            this.toolStripLabel1.Text = "֤�����:";
            // 
            // toolStripTextBox_barcode
            // 
            this.toolStripTextBox_barcode.Name = "toolStripTextBox_barcode";
            this.toolStripTextBox_barcode.Size = new System.Drawing.Size(150, 25);
            this.toolStripTextBox_barcode.Enter += new System.EventHandler(this.toolStripTextBox_barcode_Enter);
            this.toolStripTextBox_barcode.Leave += new System.EventHandler(this.toolStripTextBox_barcode_Leave);
            this.toolStripTextBox_barcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox_barcode_KeyDown);
            this.toolStripTextBox_barcode.TextChanged += new System.EventHandler(this.toolStripTextBox_barcode_TextChanged);
            // 
            // toolStripButton_load
            // 
            this.toolStripButton_load.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_load.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_load.Image")));
            this.toolStripButton_load.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_load.Name = "toolStripButton_load";
            this.toolStripButton_load.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton_load.Text = "װ��";
            this.toolStripButton_load.Click += new System.EventHandler(this.toolStripButton_load_Click);
            // 
            // tableLayoutPanel_main
            // 
            this.tableLayoutPanel_main.ColumnCount = 1;
            this.tableLayoutPanel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_main.Controls.Add(this.toolStrip_load, 0, 0);
            this.tableLayoutPanel_main.Controls.Add(this.toolStrip1, 0, 2);
            this.tableLayoutPanel_main.Controls.Add(this.tabControl_readerInfo, 0, 1);
            this.tableLayoutPanel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_main.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_main.Name = "tableLayoutPanel_main";
            this.tableLayoutPanel_main.RowCount = 3;
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_main.Size = new System.Drawing.Size(610, 337);
            this.tableLayoutPanel_main.TabIndex = 6;
            // 
            // tabPage_borrowHistory
            // 
            this.tabPage_borrowHistory.Controls.Add(this.webBrowser_borrowHistory);
            this.tabPage_borrowHistory.Location = new System.Drawing.Point(4, 22);
            this.tabPage_borrowHistory.Name = "tabPage_borrowHistory";
            this.tabPage_borrowHistory.Size = new System.Drawing.Size(598, 257);
            this.tabPage_borrowHistory.TabIndex = 3;
            this.tabPage_borrowHistory.Text = "������ʷ";
            this.tabPage_borrowHistory.UseVisualStyleBackColor = true;
            // 
            // webBrowser_borrowHistory
            // 
            this.webBrowser_borrowHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser_borrowHistory.Location = new System.Drawing.Point(0, 0);
            this.webBrowser_borrowHistory.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser_borrowHistory.Name = "webBrowser_borrowHistory";
            this.webBrowser_borrowHistory.Size = new System.Drawing.Size(598, 257);
            this.webBrowser_borrowHistory.TabIndex = 0;
            // 
            // ReaderInfoForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 337);
            this.Controls.Add(this.tableLayoutPanel_main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReaderInfoForm";
            this.ShowInTaskbar = false;
            this.Text = "����";
            this.Activated += new System.EventHandler(this.ReaderInfoForm_Activated);
            this.Deactivate += new System.EventHandler(this.ReaderInfoForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReaderInfoForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReaderInfoForm_FormClosed);
            this.Load += new System.EventHandler(this.ReaderInfoForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ReaderInfoForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ReaderInfoForm_DragEnter);
            this.Enter += new System.EventHandler(this.ReaderInfoForm_Enter);
            this.Leave += new System.EventHandler(this.ReaderInfoForm_Leave);
            this.splitContainer_normal.Panel1.ResumeLayout(false);
            this.splitContainer_normal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_normal)).EndInit();
            this.splitContainer_normal.ResumeLayout(false);
            this.tabControl_readerInfo.ResumeLayout(false);
            this.tabPage_normal.ResumeLayout(false);
            this.tabPage_xml.ResumeLayout(false);
            this.tabPage_objects.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip_load.ResumeLayout(false);
            this.toolStrip_load.PerformLayout();
            this.tableLayoutPanel_main.ResumeLayout(false);
            this.tableLayoutPanel_main.PerformLayout();
            this.tabPage_borrowHistory.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer_normal;
        private ReaderEditControl readerEditControl1;
        private System.Windows.Forms.WebBrowser webBrowser_readerInfo;
        private System.Windows.Forms.TabControl tabControl_readerInfo;
        private System.Windows.Forms.TabPage tabPage_normal;
        private System.Windows.Forms.TabPage tabPage_xml;
        private System.Windows.Forms.WebBrowser webBrowser_xml;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_save;
        private System.Windows.Forms.ToolStripButton toolStripButton_saveTo;
        private System.Windows.Forms.ToolStripButton toolStripButton_delete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton_prev;
        private System.Windows.Forms.ToolStripButton toolStripButton_next;
        private System.Windows.Forms.ToolStripButton toolStripButton_stopSummaryLoop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton_createMoneyRecord;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_hire;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_foregift;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_returnForegift;
        private System.Windows.Forms.ToolStrip toolStrip_load;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_barcode;
        private System.Windows.Forms.ToolStripButton toolStripButton_load;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_main;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripButton_clearOutofReservationCount;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton_otherFunc;
        private System.Windows.Forms.ToolStripButton toolStripButton_saveTemplate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton toolStripButton_option;
        private System.Windows.Forms.TabPage tabPage_objects;
        private DigitalPlatform.CirculationClient.BinaryResControl binaryResControl1;
        private System.Windows.Forms.ToolStripButton toolStripButton_pasteCardPhoto;
        private System.Windows.Forms.ToolStripButton toolStripButton_webCamera;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton_loadBlank;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_loadBlankFromServer;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_loadBlankFromLocal;
        private System.Windows.Forms.ToolStripButton toolStripButton_loadFromIdcard;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_moveRecord;
        private System.Windows.Forms.ToolStripButton toolStripButton_registerFingerprint;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_clearFingerprint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_exportBorrowingBarcode;
        private System.Windows.Forms.ToolStripButton toolStripButton_loadBlank;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_exportExcel;
        private System.Windows.Forms.ToolStripButton toolStripButton_addFriends;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_exportDetailToExcelFile;
        private System.Windows.Forms.TabPage tabPage_borrowHistory;
        private System.Windows.Forms.WebBrowser webBrowser_borrowHistory;

    }
}