using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization;

using DigitalPlatform;
using DigitalPlatform.GUI;
using DigitalPlatform.Marc;
using DigitalPlatform.Xml;
using DigitalPlatform.Text;

using DigitalPlatform.CirculationClient;
using DigitalPlatform.LibraryClient.localhost;
using DigitalPlatform.LibraryClient;

namespace dp2Catalog
{
    public partial class ChangePasswordForm : Form
    {
        public LibraryChannelCollection Channels = null;
        LibraryChannel Channel = null;

        public string Lang = "zh";

        public MainForm MainForm = null;
        DigitalPlatform.Stop stop = null;

        public ChangePasswordForm()
        {
            InitializeComponent();
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {
            this.Channels = new LibraryChannelCollection();
            this.Channels.BeforeLogin += new BeforeLoginEventHandle(Channels_BeforeLogin);
            this.Channels.AfterLogin += new AfterLoginEventHandle(Channels_AfterLogin);

            stop = new DigitalPlatform.Stop();
            stop.Register(MainForm.stopManager, true);	// ����������

        }

        void Channels_AfterLogin(object sender, AfterLoginEventArgs e)
        {

            LibraryChannel channel = (LibraryChannel)sender;

            dp2Server server = this.MainForm.Servers[channel.Url];
            if (server == null)
            {
                // e.ErrorInfo = "û���ҵ� URL Ϊ " + channel.Url + " �ķ���������";
                return;
            }

#if SN
            if (server.Verified == false && StringUtil.IsInList("serverlicensed", channel.Rights) == false)
            {
                string strError = "";
                string strTitle = "�޸����봰��Ҫ���������кŲ��ܷ��ʷ����� " + server.Name + " " + server.Url;
                int nRet = this.MainForm.VerifySerialCode(strTitle,
                    "",
                    true,
                    out strError);
                if (nRet == -1)
                {
                    channel.Close();
                    e.ErrorInfo = strTitle;
#if NO
                    MessageBox.Show(this.MainForm, "�޸����봰��Ҫ���������кŲ���ʹ��");
                    API.PostMessage(this.Handle, API.WM_CLOSE, 0, 0);
#endif
                    return;
                }
            }
            server.Verified = true;
#else

            server.Verified = true;
#endif
        }

        void Channels_BeforeLogin(object sender, BeforeLoginEventArgs e)
        {
            LibraryChannel channel = (LibraryChannel)sender;

            dp2Server server = this.MainForm.Servers[channel.Url];
            if (server == null)
            {
                e.ErrorInfo = "û���ҵ� URL Ϊ " + channel.Url + " �ķ���������";
                e.Failed = true;
                e.Cancel = true;
                return;
            }

            if (e.FirstTry == true)
            {
                e.UserName = server.DefaultUserName;
                e.Password = server.DefaultPassword;
                e.Parameters = "location=dp2Catalog,type=worker";
                /*
                e.IsReader = false;
                e.Location = "dp2Catalog";
                 * */

                // 2014/9/13
                e.Parameters += ",mac=" + StringUtil.MakePathList(SerialCodeForm.GetMacAddress(), "|");

#if SN
                // �����к��л�� expire= ����ֵ
                string strExpire = this.MainForm.GetExpireParam();
                if (string.IsNullOrEmpty(strExpire) == false)
                    e.Parameters += ",expire=" + strExpire;
#endif
                e.Parameters += ",client=dp2catalog|" + Program.ClientVersion;

                if (String.IsNullOrEmpty(e.UserName) == false)
                    return; // ��������, �Ա�����һ�� ������ �Ի�����Զ���¼
            }

            // 
            IWin32Window owner = this;

            ServerDlg dlg = SetDefaultAccount(
                e.LibraryServerUrl,
                null,
                e.ErrorInfo,
                owner);
            if (dlg == null)
            {
                e.Cancel = true;
                return;
            }


            e.UserName = dlg.UserName;
            e.Password = dlg.Password;
            e.SavePasswordShort = false;
            e.Parameters = "location=dp2Catalog,type=worker";

            e.Parameters += ",client=dp2catalog|" + Program.ClientVersion;

            /*
            e.IsReader = false;
            e.Location = "dp2Catalog";
             * */
            e.SavePasswordLong = true;
            e.LibraryServerUrl = dlg.ServerUrl;
        }

        ServerDlg SetDefaultAccount(
    string strServerUrl,
    string strTitle,
    string strComment,
    IWin32Window owner)
        {
            dp2Server server = this.MainForm.Servers[strServerUrl];

            ServerDlg dlg = new ServerDlg();
            GuiUtil.SetControlFont(dlg, this.Font);

            if (String.IsNullOrEmpty(strServerUrl) == true)
            {
            }
            else
            {
                dlg.ServerUrl = strServerUrl;
            }

            if (owner == null)
                owner = this;


            if (String.IsNullOrEmpty(strTitle) == false)
                dlg.Text = strTitle;

            dlg.Comment = strComment;
            dlg.UserName = server.DefaultUserName;

            this.MainForm.AppInfo.LinkFormState(dlg,
                "dp2_logindlg_state");

            dlg.ShowDialog(owner);

            this.MainForm.AppInfo.UnlinkFormState(dlg);


            if (dlg.DialogResult == DialogResult.Cancel)
            {
                return null;
            }

            server.DefaultUserName = dlg.UserName;
            server.DefaultPassword =
                (dlg.SavePassword == true) ?
                dlg.Password : "";

            server.SavePassword = dlg.SavePassword;

            server.Url = dlg.ServerUrl;
            return dlg;
        }

        void DoStop(object sender, StopEventArgs e)
        {
            if (this.Channel != null)
                this.Channel.Abort();
        }

        // ͼ��ݷ�������
        public string LibraryServerName
        {
            get
            {
                return this.textBox_dp2library_serverName.Text;
            }
            set
            {
                this.textBox_dp2library_serverName.Text = value;
            }
        }


        private void ChangePasswordForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (stop != null) // �������
            {
                stop.Unregister();	// ����������
                stop = null;
            }

            this.Channels.BeforeLogin -= new BeforeLoginEventHandle(Channels_BeforeLogin);
        }

        void EnableControls(bool bEnable)
        {
            this.textBox_dp2library_serverName.Enabled = bEnable;
            this.button_dp2library_findServerName.Enabled = bEnable;

            this.button_dp2library_changePassword.Enabled = bEnable;
            this.textBox_dp2library_userName.Enabled = bEnable;
            if (this.checkBox_dp2library_force.Checked == true)
                this.textBox_dp2library_oldPassword.Enabled = false;
            else
                this.textBox_dp2library_oldPassword.Enabled = bEnable;
            this.textBox_dp2library_newPassword.Enabled = bEnable;
            this.textBox_dp2library_confirmNewPassword.Enabled = bEnable;
            this.checkBox_dp2library_force.Enabled = bEnable;

        }

        private void button_dp2library_changePassword_Click(object sender, EventArgs e)
        {
            string strError = "";

            if (this.textBox_dp2library_userName.Text == "")
            {
                MessageBox.Show(this, "��δ�����û�����");
                this.textBox_dp2library_userName.Focus();
                return;
            }

            if (this.textBox_dp2library_newPassword.Text != this.textBox_dp2library_confirmNewPassword.Text)
            {
                MessageBox.Show(this, "������ �� ȷ�������벻һ�¡����������롣");
                this.textBox_dp2library_newPassword.Focus();
                return;
            }

            stop.OnStop += new StopEventHandler(this.DoStop);
            stop.Initial("�����޸� dp2library �û����� ...");
            stop.BeginLoop();

            this.EnableControls(false);

            this.Update();
            this.MainForm.Update();


            try
            {
                long lRet = 0;

                // ���server url
                if (String.IsNullOrEmpty(this.LibraryServerName) == true)
                {
                    strError = "��δָ����������";
                    goto ERROR1;
                }
                dp2Server server = this.MainForm.Servers.GetServerByName(this.LibraryServerName);
                if (server == null)
                {
                    strError = "��������Ϊ '" + this.LibraryServerName + "' �ķ�����������...";
                    goto ERROR1;
                }

                string strServerUrl = server.Url;

                this.Channel = this.Channels.GetChannel(strServerUrl);


                // ��ǿ���޸����룬�������޸�
                if (this.checkBox_dp2library_force.Checked == false)
                {

                    // return:
                    //      -1  error
                    //      0   ��¼δ�ɹ�
                    //      1   ��¼�ɹ�
                    lRet = Channel.Login(this.textBox_dp2library_userName.Text,
                        this.textBox_dp2library_oldPassword.Text,
                        "location=dp2Catalog,type=worker,client=dp2catalog|" + Program.ClientVersion,
                        /*
                        "",
                        false,
                         * */
                        out strError);
                    if (lRet == -1)
                    {
                        goto ERROR1;
                    }

                    if (lRet == 0)
                    {
                        strError = "�����벻��ȷ";
                        goto ERROR1;
                    }

                    try
                    {

                        lRet = Channel.ChangeUserPassword(
                            stop,
                            this.textBox_dp2library_userName.Text,
                            this.textBox_dp2library_oldPassword.Text,
                            this.textBox_dp2library_newPassword.Text,
                            out strError);
                        if (lRet == -1)
                            goto ERROR1;
                    }
                    finally
                    {
                        string strError_1 = "";
                        Channel.Logout(out strError_1);
                    }
                }

                // ǿ���޸�����
                if (this.checkBox_dp2library_force.Checked == true)
                {
                    UserInfo info = new UserInfo();
                    info.UserName = this.textBox_dp2library_userName.Text;
                    info.Password = this.textBox_dp2library_newPassword.Text;
                    // ��actionΪ"resetpassword"ʱ����info.ResetPassword״̬�������ã�����������Ҫ�޸����롣resetpassword�����޸�������Ϣ��Ҳ����˵info�г���Password/UserName����������Ա��ֵ��Ч��
                    lRet = Channel.SetUser(
                        stop,
                        "resetpassword",
                        info,
                        out strError);
                    if (lRet == -1)
                        goto ERROR1;

                }


            }
            finally
            {
                this.EnableControls(true);

                stop.EndLoop();
                stop.OnStop -= new StopEventHandler(this.DoStop);
                stop.Initial("");
            }

            MessageBox.Show(this, "dp2library �û� '" + this.textBox_dp2library_userName.Text + "' �����Ѿ����ɹ��޸ġ�");

            this.textBox_dp2library_userName.SelectAll();
            this.textBox_dp2library_userName.Focus();
            return;
        ERROR1:
            MessageBox.Show(this, strError);

            // �������¶�λ������������
            this.textBox_dp2library_oldPassword.Focus();
            this.textBox_dp2library_oldPassword.SelectAll();
        }

        private void checkBox_dp2library_force_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_dp2library_force.Checked == true)
                this.textBox_dp2library_oldPassword.Enabled = false;
            else
                this.textBox_dp2library_oldPassword.Enabled = true;
        }

        private void ChangePasswordForm_Activated(object sender, EventArgs e)
        {
            if (stop != null)
                MainForm.stopManager.Active(this.stop);

            MainForm.SetMenuItemState();
        }

        private void button_dp2library_findServerName_Click(object sender, EventArgs e)
        {
            GetDp2ResDlg dlg = new GetDp2ResDlg();
            GuiUtil.SetControlFont(dlg, this.Font);

            dlg.dp2Channels = this.Channels;
            dlg.Servers = this.MainForm.Servers;
            dlg.EnabledIndices = new int[] { dp2ResTree.RESTYPE_SERVER };
            dlg.Path = this.textBox_dp2library_serverName.Text;

            dlg.ShowDialog(this);

            if (dlg.DialogResult != DialogResult.OK)
                return;

            this.textBox_dp2library_serverName.Text = dlg.Path;
        }
    }
}