﻿using DigitalPlatform.CommonControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace dp2Circulation
{
    public partial class VerifyEntityDialog : Form
    {
        public VerifyEntityDialog()
        {
            InitializeComponent();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        public string UiState
        {
            get
            {
                List<object> controls = new List<object>();
                controls.Add(this.checkBox_autoModify);
                controls.Add(this.checkBox_verifyItemBarcode);
                return GuiState.GetUiState(controls);
            }
            set
            {
                List<object> controls = new List<object>();
                controls.Add(this.checkBox_autoModify);
                controls.Add(this.checkBox_verifyItemBarcode);
                GuiState.SetUiState(controls, value);
            }
        }

        public bool VerifyItemBarcode
        {
            get
            {
                return this.checkBox_verifyItemBarcode.Checked;
            }
            set
            {
                this.checkBox_verifyItemBarcode.Checked = value;
            }
        }

        public bool AutoModify
        {
            get
            {
                return this.checkBox_autoModify.Checked;
            }
            set
            {
                this.checkBox_autoModify.Checked = value;
            }
        }
    }
}
