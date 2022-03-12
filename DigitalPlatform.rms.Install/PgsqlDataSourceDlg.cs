using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;

using DigitalPlatform;
using DigitalPlatform.GUI;
using DigitalPlatform.Install;
using Npgsql;

namespace DigitalPlatform.rms
{
    public partial class PgsqlDataSourceDlg : Form
    {
        public PgsqlDataSourceDlg()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ������Ϣ��ѡ�� SQL Server �ʹ�����¼���Ĺ�����Ϣ
        /// </summary>
        public string DebugInfo
        {
            get;
            set;
        }

        public string SqlServerName
        {
            get
            {
                return this.textBox_sqlServerName.Text;
            }
            set 
            {
                this.textBox_sqlServerName.Text = value;
            }
        }

        public string KernelLoginName
        {
            get
            {
                return this.textBox_loginName.Text;
            }
            set
            {
                this.textBox_loginName.Text = value;
            }
        }

        public string KernelLoginPassword
        {
            get
            {
                return this.textBox_loginPassword.Text;
            }
            set
            {
                this.textBox_loginPassword.Text = value;
                this.textBox_confirmLoginPassword.Text = value;
            }
        }

        /*
        public bool SSPI
        {
            get
            {
                return this.radioButton_SSPI.Checked;
            }
            set
            {
                this.radioButton_SSPI.Checked = value;
            }
        }*/

        /*
        public string SqlUserName
        {
            get
            {
                return this.textBox_sqlUserName.Text;
            }
            set
            {
                this.textBox_sqlUserName.Text = value;
            }
        }

        public string SqlPassword
        {
            get
            {
                return this.textBox_sqlPassword.Text;
            }
            set
            {
                this.textBox_sqlPassword.Text = value;
            }
        }
         * */

        public string InstanceName
        {
            get
            {
                return this.textBox_instanceName.Text;
            }
            set
            {
                this.textBox_instanceName.Text = value;
            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            string strError = "";
            int nRet = 0;

            if (this.textBox_sqlServerName.Text == "")
            {
                strError = "��δָ�� PostgreSQL ������";
                goto ERROR1;
            }

            if (string.Compare(this.textBox_sqlServerName.Text.Trim(), "~sqlite") == 0)
            {
                strError = "PostgreSQL ������������Ϊ '~sqlite'����Ϊ������ֱ������� SQLite �������ݿ�����";
                goto ERROR1;
            }


            {
                if (this.textBox_loginName.Text == "")
                {
                    strError = "��δָ�� dp2Kernel ��¼��";
                    goto ERROR1;
                }

                if (this.textBox_loginPassword.Text != this.textBox_confirmLoginPassword.Text)
                {
                    strError = "dp2Kernel ��¼���������ȷ�����벻һ��";
                    goto ERROR1;
                }
            }

            this.button_OK.Enabled = false;
            try
            {
                // ���� Pgsql �����ݿ⡣�������ݿ�ʵ������һ��ʵ���ڵĹ����ռ䣬���� MS SQL Server �������ݿ����
                nRet = CreateDatabase(
                    this.SqlServerName,
                    this.textBox_loginName.Text,
                    this.textBox_loginPassword.Text,
                    this.textBox_adminDatabaseName.Text,
                    this.textBox_instanceName.Text,
                    out strError);
                if (nRet == -1)
                {
                    DialogResult result = MessageBox.Show(this,
    "���Զ��������ݿ�Ĺ����з�������: \r\n\r\n"
    + strError
    + "\r\n\r\n�Ƿ���Ȼ������Щ����������ɰ�װ?",
    "PgsqlServerDataSourceDlg",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question,
    MessageBoxDefaultButton.Button2);
                    if (result == System.Windows.Forms.DialogResult.No)
                    {
                        MessageBox.Show(this, "���޸ķ���������");
                        return;
                    }
                }

                // ������������
                nRet = VerifySqlServer(
                    this.SqlServerName,
                    this.textBox_loginName.Text,
                    this.textBox_loginPassword.Text,
                    this.textBox_instanceName.Text,
                    out strError);
                if (nRet == -1)
                {
                    DialogResult result = MessageBox.Show(this,
    "�ڼ������������Ĺ����з�������: \r\n\r\n"
    + strError
    + "\r\n\r\n�Ƿ���Ȼ������Щ����������ɰ�װ?",
    "PgsqlServerDataSourceDlg",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question,
    MessageBoxDefaultButton.Button2);
                    if (result == System.Windows.Forms.DialogResult.No)
                    {
                        MessageBox.Show(this, "���޸ķ���������");
                        return;
                    }
                }
            }
            finally
            {
                this.button_OK.Enabled = true;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        ERROR1:
            MessageBox.Show(this, strError);
            // MessageBox.Show(this, "��Ȼ�ղŵĴ�����¼������ʧ���ˣ�����Ҳ����������ָ����¼����������ٴΰ���ȷ������ť������¼�����������а�װ");
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public static int DeleteDatabase(
string strSqlServerName,
string strSqlUserName,
string strSqlUserPassword,
string strAdminDatabase,
string strDatabaseName,
out string strError)
        {
            strError = "";

            if (strSqlServerName.Contains("="))
            {
                strError = $"strSqlServerName ���� '{strSqlServerName}' �в���������Ⱥ�";
                return -1;
            }
            string strConnection = $"Host={strSqlServerName};Username={strSqlUserName};Password={strSqlUserPassword};Database={strAdminDatabase};"; // Database={strDatabaseName}

            try
            {
                using (var connection = new NpgsqlConnection(strConnection))
                {
                    try
                    {
                        connection.Open();
                        string strCommand = $"DROP DATABASE \"{strDatabaseName}\"";
                        using (var command = new NpgsqlCommand(strCommand, connection))
                        {
                            var count = command.ExecuteNonQuery();
                        }
                    }
                    catch (PostgresException ex)
                    {
                        // https://www.postgresql.org/docs/current/errcodes-appendix.html
                        // 3D000	invalid_catalog_name
                        if (ex.SqlState == "3D000")
                        {
                            strError = $"���ݿ� '{strDatabaseName}' ������";
                            return -1;
                        }
                        strError = $"ɾ�����ݿ� {strDatabaseName} ����: { ex.Message }";
                        return -1;
                    }
                    catch (NpgsqlException sqlEx)
                    {
                        strError = $"ɾ�����ݿ� {strDatabaseName} ����: { sqlEx.Message }";
                        int nError = (int)sqlEx.ErrorCode;
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        strError = $"ɾ�����ݿ� {strDatabaseName} ����: { ex.Message }";
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                strError = $"ɾ�����ݿ� {strDatabaseName} ����: { ex.Message }";
                return -1;
            }
            return 0;
        }


        public static int CreateDatabase(
string strSqlServerName,
string strSqlUserName,
string strSqlUserPassword,
string strAdminDatabase,
string strDatabaseName,
out string strError)
        {
            strError = "";

            if (strSqlServerName.Contains("="))
            {
                strError = $"strSqlServerName ���� '{strSqlServerName}' �в���������Ⱥ�";
                return -1;
            }
            string strConnection = $"Host={strSqlServerName};Username={strSqlUserName};Password={strSqlUserPassword};Database={strAdminDatabase};"; // Database={strDatabaseName}

            try
            {
                using (var connection = new NpgsqlConnection(strConnection))
                {
                    try
                    {
                        connection.Open();
                        string strCommand = $"CREATE DATABASE \"{strDatabaseName}\"";
                        using(var command = new NpgsqlCommand(strCommand, connection))
                        {
                            var count = command.ExecuteNonQuery();
                        }
                    }
                    catch(PostgresException ex)
                    {
                        // https://www.postgresql.org/docs/current/errcodes-appendix.html
                        // 42P04	duplicate_database
                        if (ex.SqlState == "42P04")
                            return 0;
                        strError = $"�������ݿ� {strDatabaseName} ����: { ex.Message }";
                        return -1;
                    }
                    catch (NpgsqlException sqlEx)
                    {
                        strError = $"�������ݿ� {strDatabaseName} ����: { sqlEx.Message }";
                        int nError = (int)sqlEx.ErrorCode;
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        // strError = "���� SQL ���ݿ���� " + ex.Message + " ����:" + ex.GetType().ToString();
                        strError = $"�������ݿ� {strDatabaseName} ����: { ex.Message }";
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                strError = "�������ӳ���" + ex.Message + " ����:" + ex.GetType().ToString();
                return -1;
            }
            return 0;
        }

        public static int VerifySqlServer(
string strSqlServerName,
string strSqlUserName,
string strSqlUserPassword,
string strDatabaseName,
out string strError)
        {
            strError = "";

            if (strSqlServerName.Contains("="))
            {
                strError = $"strSqlServerName ���� '{strSqlServerName}' �в���������Ⱥ�";
                return -1;
            }
            // strSqlServerName ������һ��Ϊ "localhost;Database=postgres" ��̬�����ڰ����� database ����
            string strConnection = $"Host={strSqlServerName};Username={strSqlUserName};Password={strSqlUserPassword};Database={strDatabaseName};"; // Database={strDatabaseName}

            try
            {
                using (var connection = new NpgsqlConnection(strConnection))
                {
                    try
                    {
                        connection.Open();
                        NpgsqlConnection.ClearPool(connection);
                    }
                    catch (NpgsqlException sqlEx)
                    {
                        strError = "���� SQL ���ݿ���� " + sqlEx.Message + "��";
                        int nError = (int)sqlEx.ErrorCode;
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        strError = "���� SQL ���ݿ���� " + ex.Message + " ����:" + ex.GetType().ToString();
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                strError = "�������ӳ���" + ex.Message + " ����:" + ex.GetType().ToString();
                return -1;
            }
            return 0;
        }

        private void button_getSqlServerName_Click(object sender, EventArgs e)
        {
            /*
            GetSqlServerDlg dlg = new GetSqlServerDlg();
            GuiUtil.AutoSetDefaultFont(dlg);

            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.ShowDialog(this);

            if (dlg.DialogResult != DialogResult.OK)
                return;

            this.textBox_sqlServerName.Text = dlg.SqlServerName;
            */
        }

        public void EnableControls(bool bEnable)
        {
            this.textBox_sqlServerName.Enabled = bEnable;

            /*
            if (this.radioButton_SSPI.Checked == true)
            {
                this.textBox_sqlUserName.Enabled = false;
                this.textBox_sqlPassword.Enabled = false;
            }
            else
            {
                this.textBox_sqlUserName.Enabled = bEnable;
                this.textBox_sqlPassword.Enabled = bEnable;
            }*/

            this.textBox_instanceName.Enabled = bEnable;

            this.button_getSqlServerName.Enabled = bEnable;

            // this.button_detect.Enabled = bEnable;

            this.button_OK.Enabled = bEnable;
            this.button_Cancel.Enabled = bEnable;            
        }

        private void DataSourceDlg_Load(object sender, EventArgs e)
        {
            // radioButton_SSPI_CheckedChanged(null, null);
        }

        public string Comment
        {
            get
            {
                return this.textBox_message.Text;
            }
            set
            {
                this.textBox_message.Text = value;
            }
        }

        private void textBox_sqlServerName_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_loginName_TextChanged(object sender, EventArgs e)
        {
            this.textBox_adminDatabaseName.Text = this.textBox_loginName.Text;
        }

        private void checkBox_enableModifyAdminDatabaseName_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox_adminDatabaseName.ReadOnly = !this.checkBox_enableModifyAdminDatabaseName.Checked;
        }

        private void button_deleteDatabase_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this,
$"ȷʵҪɾ�����ݿ� '{this.InstanceName}'?",
"PgsqlServerDataSourceDlg",
MessageBoxButtons.YesNo,
MessageBoxIcon.Question,
MessageBoxDefaultButton.Button2);
            if (result == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            this.button_OK.Enabled = false;
            try
            {
                // ɾ�� Pgsql �����ݿ⡣�������ݿ�ʵ������һ��ʵ���ڵĹ����ռ䣬���� MS SQL Server �������ݿ����
                int nRet = DeleteDatabase(
                    this.SqlServerName,
                    this.textBox_loginName.Text,
                    this.textBox_loginPassword.Text,
                    this.textBox_adminDatabaseName.Text,
                    this.textBox_instanceName.Text,
                    out string strError);
                if (nRet == -1)
                {
                    MessageBox.Show(this,
    $"��ɾ�����ݿ� '{this.InstanceName}' �Ĺ����з�������: \r\n\r\n"
    + strError);
                    return;
                }

                MessageBox.Show(this, $"���ݿ� '{this.InstanceName}' ɾ���ɹ�");
            }
            finally
            {
                this.button_OK.Enabled = true;
            }
        }
    }
}