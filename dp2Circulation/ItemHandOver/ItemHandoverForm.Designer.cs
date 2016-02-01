namespace dp2Circulation
{
    partial class ItemHandoverForm
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

            if (this._fillThread != null)
                this._fillThread.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemHandoverForm));
            this.tabControl_main = new System.Windows.Forms.TabControl();
            this.tabPage_load = new System.Windows.Forms.TabPage();
            this.button_load_scanBarcode = new System.Windows.Forms.Button();
            this.button_load_loadFromRecPathFile = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_load_type = new System.Windows.Forms.ComboBox();
            this.button_load_loadFromBatchNo = new System.Windows.Forms.Button();
            this.button_load_loadFromBarcodeFile = new System.Windows.Forms.Button();
            this.tabPage_verify = new System.Windows.Forms.TabPage();
            this.checkBox_verify_autoUppercaseBarcode = new System.Windows.Forms.CheckBox();
            this.button_verify_load = new System.Windows.Forms.Button();
            this.textBox_verify_itemBarcode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage_move = new System.Windows.Forms.TabPage();
            this.button_move_notifyReader = new System.Windows.Forms.Button();
            this.button_move_changeStateAll = new System.Windows.Forms.Button();
            this.button_move_moveAll = new System.Windows.Forms.Button();
            this.tabPage_print = new System.Windows.Forms.TabPage();
            this.button_print_printNormalList = new System.Windows.Forms.Button();
            this.button_print_option = new System.Windows.Forms.Button();
            this.button_print_printCheckedList = new System.Windows.Forms.Button();
            this.button_next = new System.Windows.Forms.Button();
            this.listView_in = new DigitalPlatform.GUI.ListViewNF();
            this.imageList_lineType = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer_inAndOutof = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel_in = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel_out = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.listView_outof = new DigitalPlatform.GUI.ListViewNF();
            this.panel_up = new System.Windows.Forms.Panel();
            this.splitContainer_main = new System.Windows.Forms.SplitContainer();
            this.button_move_changeLocation = new System.Windows.Forms.Button();
            this.tabControl_main.SuspendLayout();
            this.tabPage_load.SuspendLayout();
            this.tabPage_verify.SuspendLayout();
            this.tabPage_move.SuspendLayout();
            this.tabPage_print.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_inAndOutof)).BeginInit();
            this.splitContainer_inAndOutof.Panel1.SuspendLayout();
            this.splitContainer_inAndOutof.Panel2.SuspendLayout();
            this.splitContainer_inAndOutof.SuspendLayout();
            this.tableLayoutPanel_in.SuspendLayout();
            this.tableLayoutPanel_out.SuspendLayout();
            this.panel_up.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_main)).BeginInit();
            this.splitContainer_main.Panel1.SuspendLayout();
            this.splitContainer_main.Panel2.SuspendLayout();
            this.splitContainer_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl_main
            // 
            this.tabControl_main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl_main.Controls.Add(this.tabPage_load);
            this.tabControl_main.Controls.Add(this.tabPage_verify);
            this.tabControl_main.Controls.Add(this.tabPage_move);
            this.tabControl_main.Controls.Add(this.tabPage_print);
            this.tabControl_main.Location = new System.Drawing.Point(0, 2);
            this.tabControl_main.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl_main.Name = "tabControl_main";
            this.tabControl_main.SelectedIndex = 0;
            this.tabControl_main.Size = new System.Drawing.Size(415, 109);
            this.tabControl_main.TabIndex = 0;
            this.tabControl_main.SelectedIndexChanged += new System.EventHandler(this.tabControl_main_SelectedIndexChanged);
            // 
            // tabPage_load
            // 
            this.tabPage_load.AutoScroll = true;
            this.tabPage_load.Controls.Add(this.button_load_scanBarcode);
            this.tabPage_load.Controls.Add(this.button_load_loadFromRecPathFile);
            this.tabPage_load.Controls.Add(this.label4);
            this.tabPage_load.Controls.Add(this.comboBox_load_type);
            this.tabPage_load.Controls.Add(this.button_load_loadFromBatchNo);
            this.tabPage_load.Controls.Add(this.button_load_loadFromBarcodeFile);
            this.tabPage_load.Location = new System.Drawing.Point(4, 22);
            this.tabPage_load.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_load.Name = "tabPage_load";
            this.tabPage_load.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_load.Size = new System.Drawing.Size(407, 83);
            this.tabPage_load.TabIndex = 0;
            this.tabPage_load.Text = "װ��";
            this.tabPage_load.UseVisualStyleBackColor = true;
            // 
            // button_load_scanBarcode
            // 
            this.button_load_scanBarcode.Location = new System.Drawing.Point(314, 30);
            this.button_load_scanBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.button_load_scanBarcode.Name = "button_load_scanBarcode";
            this.button_load_scanBarcode.Size = new System.Drawing.Size(170, 22);
            this.button_load_scanBarcode.TabIndex = 5;
            this.button_load_scanBarcode.Text = "ɨ�������(&S)...";
            this.button_load_scanBarcode.UseVisualStyleBackColor = true;
            this.button_load_scanBarcode.Click += new System.EventHandler(this.button_load_scanBarcode_Click);
            // 
            // button_load_loadFromRecPathFile
            // 
            this.button_load_loadFromRecPathFile.Location = new System.Drawing.Point(140, 31);
            this.button_load_loadFromRecPathFile.Margin = new System.Windows.Forms.Padding(2);
            this.button_load_loadFromRecPathFile.Name = "button_load_loadFromRecPathFile";
            this.button_load_loadFromRecPathFile.Size = new System.Drawing.Size(170, 22);
            this.button_load_loadFromRecPathFile.TabIndex = 3;
            this.button_load_loadFromRecPathFile.Text = "�Ӳ��¼·���ļ�(&R)...";
            this.button_load_loadFromRecPathFile.UseVisualStyleBackColor = true;
            this.button_load_loadFromRecPathFile.Click += new System.EventHandler(this.button_load_loadFromRecPathFile_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 5);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "����������(&T):";
            // 
            // comboBox_load_type
            // 
            this.comboBox_load_type.FormattingEnabled = true;
            this.comboBox_load_type.Items.AddRange(new object[] {
            "ͼ��",
            "����������"});
            this.comboBox_load_type.Location = new System.Drawing.Point(6, 19);
            this.comboBox_load_type.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_load_type.Name = "comboBox_load_type";
            this.comboBox_load_type.Size = new System.Drawing.Size(116, 20);
            this.comboBox_load_type.TabIndex = 1;
            this.comboBox_load_type.Text = "ͼ��";
            // 
            // button_load_loadFromBatchNo
            // 
            this.button_load_loadFromBatchNo.Location = new System.Drawing.Point(314, 4);
            this.button_load_loadFromBatchNo.Margin = new System.Windows.Forms.Padding(2);
            this.button_load_loadFromBatchNo.Name = "button_load_loadFromBatchNo";
            this.button_load_loadFromBatchNo.Size = new System.Drawing.Size(170, 22);
            this.button_load_loadFromBatchNo.TabIndex = 4;
            this.button_load_loadFromBatchNo.Text = "�������κż���(&B)...";
            this.button_load_loadFromBatchNo.UseVisualStyleBackColor = true;
            this.button_load_loadFromBatchNo.Click += new System.EventHandler(this.button_load_loadFromBatchNo_Click);
            // 
            // button_load_loadFromBarcodeFile
            // 
            this.button_load_loadFromBarcodeFile.Location = new System.Drawing.Point(140, 5);
            this.button_load_loadFromBarcodeFile.Margin = new System.Windows.Forms.Padding(2);
            this.button_load_loadFromBarcodeFile.Name = "button_load_loadFromBarcodeFile";
            this.button_load_loadFromBarcodeFile.Size = new System.Drawing.Size(170, 22);
            this.button_load_loadFromBarcodeFile.TabIndex = 2;
            this.button_load_loadFromBarcodeFile.Text = "��������ļ�(&F)...";
            this.button_load_loadFromBarcodeFile.UseVisualStyleBackColor = true;
            this.button_load_loadFromBarcodeFile.Click += new System.EventHandler(this.button_load_loadFromBarcodeFile_Click);
            // 
            // tabPage_verify
            // 
            this.tabPage_verify.AutoScroll = true;
            this.tabPage_verify.Controls.Add(this.checkBox_verify_autoUppercaseBarcode);
            this.tabPage_verify.Controls.Add(this.button_verify_load);
            this.tabPage_verify.Controls.Add(this.textBox_verify_itemBarcode);
            this.tabPage_verify.Controls.Add(this.label3);
            this.tabPage_verify.Location = new System.Drawing.Point(4, 22);
            this.tabPage_verify.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_verify.Name = "tabPage_verify";
            this.tabPage_verify.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_verify.Size = new System.Drawing.Size(407, 83);
            this.tabPage_verify.TabIndex = 1;
            this.tabPage_verify.Text = "��֤";
            this.tabPage_verify.UseVisualStyleBackColor = true;
            // 
            // checkBox_verify_autoUppercaseBarcode
            // 
            this.checkBox_verify_autoUppercaseBarcode.AutoSize = true;
            this.checkBox_verify_autoUppercaseBarcode.Location = new System.Drawing.Point(91, 33);
            this.checkBox_verify_autoUppercaseBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_verify_autoUppercaseBarcode.Name = "checkBox_verify_autoUppercaseBarcode";
            this.checkBox_verify_autoUppercaseBarcode.Size = new System.Drawing.Size(222, 16);
            this.checkBox_verify_autoUppercaseBarcode.TabIndex = 8;
            this.checkBox_verify_autoUppercaseBarcode.Text = "�Զ�������������ַ���תΪ��д(&U)";
            this.checkBox_verify_autoUppercaseBarcode.UseVisualStyleBackColor = true;
            // 
            // button_verify_load
            // 
            this.button_verify_load.Location = new System.Drawing.Point(227, 6);
            this.button_verify_load.Margin = new System.Windows.Forms.Padding(2);
            this.button_verify_load.Name = "button_verify_load";
            this.button_verify_load.Size = new System.Drawing.Size(56, 22);
            this.button_verify_load.TabIndex = 2;
            this.button_verify_load.Text = "�ύ(&S)";
            this.button_verify_load.UseVisualStyleBackColor = true;
            this.button_verify_load.Click += new System.EventHandler(this.button_verify_load_Click);
            // 
            // textBox_verify_itemBarcode
            // 
            this.textBox_verify_itemBarcode.Location = new System.Drawing.Point(91, 5);
            this.textBox_verify_itemBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_verify_itemBarcode.Name = "textBox_verify_itemBarcode";
            this.textBox_verify_itemBarcode.Size = new System.Drawing.Size(132, 21);
            this.textBox_verify_itemBarcode.TabIndex = 1;
            this.textBox_verify_itemBarcode.Enter += new System.EventHandler(this.textBox_verify_itemBarcode_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "�������(&B):";
            // 
            // tabPage_move
            // 
            this.tabPage_move.AutoScroll = true;
            this.tabPage_move.Controls.Add(this.button_move_changeLocation);
            this.tabPage_move.Controls.Add(this.button_move_notifyReader);
            this.tabPage_move.Controls.Add(this.button_move_changeStateAll);
            this.tabPage_move.Controls.Add(this.button_move_moveAll);
            this.tabPage_move.Location = new System.Drawing.Point(4, 22);
            this.tabPage_move.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_move.Name = "tabPage_move";
            this.tabPage_move.Size = new System.Drawing.Size(407, 83);
            this.tabPage_move.TabIndex = 3;
            this.tabPage_move.Text = "ת��";
            this.tabPage_move.UseVisualStyleBackColor = true;
            // 
            // button_move_notifyReader
            // 
            this.button_move_notifyReader.Location = new System.Drawing.Point(7, 32);
            this.button_move_notifyReader.Margin = new System.Windows.Forms.Padding(2);
            this.button_move_notifyReader.Name = "button_move_notifyReader";
            this.button_move_notifyReader.Size = new System.Drawing.Size(182, 22);
            this.button_move_notifyReader.TabIndex = 2;
            this.button_move_notifyReader.Text = "֪ͨ��������(&N)...";
            this.button_move_notifyReader.UseVisualStyleBackColor = true;
            this.button_move_notifyReader.Click += new System.EventHandler(this.button_move_notifyReader_Click);
            // 
            // button_move_changeStateAll
            // 
            this.button_move_changeStateAll.Location = new System.Drawing.Point(193, 6);
            this.button_move_changeStateAll.Margin = new System.Windows.Forms.Padding(2);
            this.button_move_changeStateAll.Name = "button_move_changeStateAll";
            this.button_move_changeStateAll.Size = new System.Drawing.Size(182, 22);
            this.button_move_changeStateAll.TabIndex = 1;
            this.button_move_changeStateAll.Text = "������ӹ��С�״̬(&C)...";
            this.button_move_changeStateAll.UseVisualStyleBackColor = true;
            this.button_move_changeStateAll.Click += new System.EventHandler(this.button_move_changeStateAll_Click);
            // 
            // button_move_moveAll
            // 
            this.button_move_moveAll.Location = new System.Drawing.Point(7, 6);
            this.button_move_moveAll.Margin = new System.Windows.Forms.Padding(2);
            this.button_move_moveAll.Name = "button_move_moveAll";
            this.button_move_moveAll.Size = new System.Drawing.Size(182, 22);
            this.button_move_moveAll.TabIndex = 0;
            this.button_move_moveAll.Text = "ת�Ƶ�Ŀ���(&M)...";
            this.button_move_moveAll.UseVisualStyleBackColor = true;
            this.button_move_moveAll.Click += new System.EventHandler(this.button_move_moveAll_Click);
            // 
            // tabPage_print
            // 
            this.tabPage_print.AutoScroll = true;
            this.tabPage_print.Controls.Add(this.button_print_printNormalList);
            this.tabPage_print.Controls.Add(this.button_print_option);
            this.tabPage_print.Controls.Add(this.button_print_printCheckedList);
            this.tabPage_print.Location = new System.Drawing.Point(4, 22);
            this.tabPage_print.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_print.Name = "tabPage_print";
            this.tabPage_print.Size = new System.Drawing.Size(407, 83);
            this.tabPage_print.TabIndex = 2;
            this.tabPage_print.Text = "��ӡ";
            this.tabPage_print.UseVisualStyleBackColor = true;
            // 
            // button_print_printNormalList
            // 
            this.button_print_printNormalList.Location = new System.Drawing.Point(5, 32);
            this.button_print_printNormalList.Margin = new System.Windows.Forms.Padding(2);
            this.button_print_printNormalList.Name = "button_print_printNormalList";
            this.button_print_printNormalList.Size = new System.Drawing.Size(152, 22);
            this.button_print_printNormalList.TabIndex = 1;
            this.button_print_printNormalList.Text = "��ӡȫ�������嵥(&N)...";
            this.button_print_printNormalList.UseVisualStyleBackColor = true;
            this.button_print_printNormalList.Click += new System.EventHandler(this.button_print_printNormalList_Click);
            // 
            // button_print_option
            // 
            this.button_print_option.Location = new System.Drawing.Point(162, 5);
            this.button_print_option.Margin = new System.Windows.Forms.Padding(2);
            this.button_print_option.Name = "button_print_option";
            this.button_print_option.Size = new System.Drawing.Size(152, 22);
            this.button_print_option.TabIndex = 2;
            this.button_print_option.Text = "��ӡ����(&O)...";
            this.button_print_option.UseVisualStyleBackColor = true;
            this.button_print_option.Click += new System.EventHandler(this.button_print_option_Click);
            // 
            // button_print_printCheckedList
            // 
            this.button_print_printCheckedList.Location = new System.Drawing.Point(5, 5);
            this.button_print_printCheckedList.Margin = new System.Windows.Forms.Padding(2);
            this.button_print_printCheckedList.Name = "button_print_printCheckedList";
            this.button_print_printCheckedList.Size = new System.Drawing.Size(152, 22);
            this.button_print_printCheckedList.TabIndex = 0;
            this.button_print_printCheckedList.Text = "��ӡ����֤�嵥(&P)...";
            this.button_print_printCheckedList.UseVisualStyleBackColor = true;
            this.button_print_printCheckedList.Click += new System.EventHandler(this.button_print_printCheckedList_Click);
            // 
            // button_next
            // 
            this.button_next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_next.Font = new System.Drawing.Font("����", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_next.Location = new System.Drawing.Point(331, 115);
            this.button_next.Margin = new System.Windows.Forms.Padding(2);
            this.button_next.Name = "button_next";
            this.button_next.Size = new System.Drawing.Size(83, 22);
            this.button_next.TabIndex = 0;
            this.button_next.Text = "��һ��(&N)";
            this.button_next.UseVisualStyleBackColor = true;
            this.button_next.Click += new System.EventHandler(this.button_next_Click);
            // 
            // listView_in
            // 
            this.listView_in.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_in.FullRowSelect = true;
            this.listView_in.HideSelection = false;
            this.listView_in.LargeImageList = this.imageList_lineType;
            this.listView_in.Location = new System.Drawing.Point(2, 14);
            this.listView_in.Margin = new System.Windows.Forms.Padding(2);
            this.listView_in.Name = "listView_in";
            this.listView_in.Size = new System.Drawing.Size(410, 50);
            this.listView_in.SmallImageList = this.imageList_lineType;
            this.listView_in.TabIndex = 2;
            this.listView_in.UseCompatibleStateImageBehavior = false;
            this.listView_in.View = System.Windows.Forms.View.Details;
            this.listView_in.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_in_ColumnClick);
            this.listView_in.DoubleClick += new System.EventHandler(this.listView_in_DoubleClick);
            this.listView_in.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView_in_MouseUp);
            // 
            // imageList_lineType
            // 
            this.imageList_lineType.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_lineType.ImageStream")));
            this.imageList_lineType.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_lineType.Images.SetKeyName(0, "WarningHS.png");
            this.imageList_lineType.Images.SetKeyName(1, "Book_angleHS.png");
            this.imageList_lineType.Images.SetKeyName(2, "Book_openHS.png");
            // 
            // splitContainer_inAndOutof
            // 
            this.splitContainer_inAndOutof.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_inAndOutof.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_inAndOutof.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer_inAndOutof.Name = "splitContainer_inAndOutof";
            this.splitContainer_inAndOutof.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_inAndOutof.Panel1
            // 
            this.splitContainer_inAndOutof.Panel1.Controls.Add(this.tableLayoutPanel_in);
            // 
            // splitContainer_inAndOutof.Panel2
            // 
            this.splitContainer_inAndOutof.Panel2.Controls.Add(this.tableLayoutPanel_out);
            this.splitContainer_inAndOutof.Size = new System.Drawing.Size(414, 141);
            this.splitContainer_inAndOutof.SplitterDistance = 66;
            this.splitContainer_inAndOutof.SplitterWidth = 3;
            this.splitContainer_inAndOutof.TabIndex = 3;
            // 
            // tableLayoutPanel_in
            // 
            this.tableLayoutPanel_in.ColumnCount = 1;
            this.tableLayoutPanel_in.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_in.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel_in.Controls.Add(this.listView_in, 0, 1);
            this.tableLayoutPanel_in.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_in.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_in.Name = "tableLayoutPanel_in";
            this.tableLayoutPanel_in.RowCount = 2;
            this.tableLayoutPanel_in.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_in.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_in.Size = new System.Drawing.Size(414, 66);
            this.tableLayoutPanel_in.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("����", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "������";
            // 
            // tableLayoutPanel_out
            // 
            this.tableLayoutPanel_out.ColumnCount = 1;
            this.tableLayoutPanel_out.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_out.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel_out.Controls.Add(this.listView_outof, 0, 1);
            this.tableLayoutPanel_out.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_out.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_out.Name = "tableLayoutPanel_out";
            this.tableLayoutPanel_out.RowCount = 2;
            this.tableLayoutPanel_out.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_out.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_out.Size = new System.Drawing.Size(414, 72);
            this.tableLayoutPanel_out.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("����", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(2, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "������";
            // 
            // listView_outof
            // 
            this.listView_outof.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_outof.FullRowSelect = true;
            this.listView_outof.HideSelection = false;
            this.listView_outof.LargeImageList = this.imageList_lineType;
            this.listView_outof.Location = new System.Drawing.Point(2, 14);
            this.listView_outof.Margin = new System.Windows.Forms.Padding(2);
            this.listView_outof.Name = "listView_outof";
            this.listView_outof.Size = new System.Drawing.Size(410, 56);
            this.listView_outof.SmallImageList = this.imageList_lineType;
            this.listView_outof.TabIndex = 1;
            this.listView_outof.UseCompatibleStateImageBehavior = false;
            this.listView_outof.View = System.Windows.Forms.View.Details;
            this.listView_outof.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_outof_ColumnClick);
            this.listView_outof.DoubleClick += new System.EventHandler(this.listView_outof_DoubleClick);
            this.listView_outof.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView_outof_MouseUp);
            // 
            // panel_up
            // 
            this.panel_up.Controls.Add(this.tabControl_main);
            this.panel_up.Controls.Add(this.button_next);
            this.panel_up.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_up.Location = new System.Drawing.Point(0, 12);
            this.panel_up.Name = "panel_up";
            this.panel_up.Size = new System.Drawing.Size(414, 137);
            this.panel_up.TabIndex = 4;
            // 
            // splitContainer_main
            // 
            this.splitContainer_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_main.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_main.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer_main.Name = "splitContainer_main";
            this.splitContainer_main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_main.Panel1
            // 
            this.splitContainer_main.Panel1.Controls.Add(this.panel_up);
            this.splitContainer_main.Panel1.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
            // 
            // splitContainer_main.Panel2
            // 
            this.splitContainer_main.Panel2.Controls.Add(this.splitContainer_inAndOutof);
            this.splitContainer_main.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.splitContainer_main.Size = new System.Drawing.Size(414, 310);
            this.splitContainer_main.SplitterDistance = 149;
            this.splitContainer_main.SplitterWidth = 8;
            this.splitContainer_main.TabIndex = 5;
            // 
            // button_move_changeLocation
            // 
            this.button_move_changeLocation.Location = new System.Drawing.Point(193, 32);
            this.button_move_changeLocation.Margin = new System.Windows.Forms.Padding(2);
            this.button_move_changeLocation.Name = "button_move_changeLocation";
            this.button_move_changeLocation.Size = new System.Drawing.Size(182, 22);
            this.button_move_changeLocation.TabIndex = 3;
            this.button_move_changeLocation.Text = "�޸Ĺݲص�(&L)...";
            this.button_move_changeLocation.UseVisualStyleBackColor = true;
            this.button_move_changeLocation.Click += new System.EventHandler(this.button_move_changeLocation_Click);
            // 
            // ItemHandoverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(414, 310);
            this.Controls.Add(this.splitContainer_main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ItemHandoverForm";
            this.Text = "����ƽ�";
            this.Activated += new System.EventHandler(this.ItemHandoverForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ItemHandoverForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ItemHandoverForm_FormClosed);
            this.Load += new System.EventHandler(this.ItemHandoverForm_Load);
            this.tabControl_main.ResumeLayout(false);
            this.tabPage_load.ResumeLayout(false);
            this.tabPage_load.PerformLayout();
            this.tabPage_verify.ResumeLayout(false);
            this.tabPage_verify.PerformLayout();
            this.tabPage_move.ResumeLayout(false);
            this.tabPage_print.ResumeLayout(false);
            this.splitContainer_inAndOutof.Panel1.ResumeLayout(false);
            this.splitContainer_inAndOutof.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_inAndOutof)).EndInit();
            this.splitContainer_inAndOutof.ResumeLayout(false);
            this.tableLayoutPanel_in.ResumeLayout(false);
            this.tableLayoutPanel_in.PerformLayout();
            this.tableLayoutPanel_out.ResumeLayout(false);
            this.tableLayoutPanel_out.PerformLayout();
            this.panel_up.ResumeLayout(false);
            this.splitContainer_main.Panel1.ResumeLayout(false);
            this.splitContainer_main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_main)).EndInit();
            this.splitContainer_main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl_main;
        private System.Windows.Forms.TabPage tabPage_load;
        private System.Windows.Forms.TabPage tabPage_verify;
        private System.Windows.Forms.Button button_next;
        private System.Windows.Forms.TabPage tabPage_print;
        private DigitalPlatform.GUI.ListViewNF listView_in;
        private System.Windows.Forms.SplitContainer splitContainer_inAndOutof;
        private System.Windows.Forms.Label label1;
        private DigitalPlatform.GUI.ListViewNF listView_outof;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_load_loadFromBatchNo;
        private System.Windows.Forms.Button button_load_loadFromBarcodeFile;
        private System.Windows.Forms.TextBox textBox_verify_itemBarcode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_verify_load;
        private System.Windows.Forms.ImageList imageList_lineType;
        private System.Windows.Forms.Button button_print_printCheckedList;
        private System.Windows.Forms.Button button_print_option;
        private System.Windows.Forms.Button button_print_printNormalList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_load_type;
        private System.Windows.Forms.TabPage tabPage_move;
        private System.Windows.Forms.Button button_move_moveAll;
        private System.Windows.Forms.Button button_move_changeStateAll;
        private System.Windows.Forms.CheckBox checkBox_verify_autoUppercaseBarcode;
        private System.Windows.Forms.Button button_move_notifyReader;
        private System.Windows.Forms.Panel panel_up;
        private System.Windows.Forms.SplitContainer splitContainer_main;
        private System.Windows.Forms.Button button_load_loadFromRecPathFile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_in;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_out;
        private System.Windows.Forms.Button button_load_scanBarcode;
        private System.Windows.Forms.Button button_move_changeLocation;
    }
}