#if NO
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DigitalPlatform.Marc
{
	/// <summary>
	/// FieldNameDlg ��ժҪ˵����
    /// δ��ʹ��
	/// </summary>
	internal class FieldNameDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox textBox_fieldName;
		private System.Windows.Forms.Button button_ok;
		private System.Windows.Forms.Button button_cancel;

		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FieldNameDlg()
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();

			//
			// TODO: �� InitializeComponent ���ú�����κι��캯������
			//
		}

		/// <summary>
		/// ������������ʹ�õ���Դ��
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows ������������ɵĴ���
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FieldNameDlg));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_fieldName = new System.Windows.Forms.TextBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "�ֶ���:";
            // 
            // textBox_fieldName
            // 
            this.textBox_fieldName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_fieldName.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.textBox_fieldName.Location = new System.Drawing.Point(9, 25);
            this.textBox_fieldName.MaxLength = 3;
            this.textBox_fieldName.Name = "textBox_fieldName";
            this.textBox_fieldName.Size = new System.Drawing.Size(313, 21);
            this.textBox_fieldName.TabIndex = 1;
            // 
            // button_ok
            // 
            this.button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ok.Location = new System.Drawing.Point(168, 118);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 22);
            this.button_ok.TabIndex = 2;
            this.button_ok.Text = "ȷ��";
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(247, 118);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 22);
            this.button_cancel.TabIndex = 3;
            this.button_cancel.Text = "ȡ��";
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // FieldNameDlg
            // 
            this.AcceptButton = this.button_ok;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(331, 150);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.textBox_fieldName);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FieldNameDlg";
            this.ShowInTaskbar = false;
            this.Text = "FieldNameDlg";
            this.Closed += new System.EventHandler(this.FieldNameDlg_Closed);
            this.Load += new System.EventHandler(this.FieldNameDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FieldNameDlg_Load(object sender, System.EventArgs e)
		{
		
		}

		private void FieldNameDlg_Closed(object sender, System.EventArgs e)
		{
		
		}

		private void button_ok_Click(object sender, System.EventArgs e)
		{
			if (this.textBox_fieldName.Text == "")
			{
				MessageBox.Show (this,"��δ�����ֶ�����");
				return;
			}

			if (this.textBox_fieldName.Text.Length != 3)
			{
				MessageBox.Show(this,"�ֶ����ĳ���ֻ��Ϊ3λ��");
				return;
			}

			if (this.textBox_fieldName.Text.IndexOf(' ') != -1)
			{
				MessageBox.Show(this,"�ֶ����в��ܰ����ո�");
				return;
			}
		
			this.DialogResult = DialogResult.OK ;
			this.Close();
		}

		private void button_cancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel ;
			this.Close();
		}

	}
}

#endif