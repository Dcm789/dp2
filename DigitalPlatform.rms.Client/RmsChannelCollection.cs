using System;
using System.Collections;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.IO;

// using Microsoft.Web.Services3;
// using Microsoft.Web.Services3.Attachments;

using System.Web.Services.Protocols;	// Ϊ��WebClientAsyncResult

using DigitalPlatform;
using DigitalPlatform.IO;
using DigitalPlatform.Text;
using DigitalPlatform.Range;
using DigitalPlatform.rms.Client.rmsws_localhost;

namespace DigitalPlatform.rms.Client
{
	/// <summary>
	/// ͨѶͨ������
	/// </summary>
	public class RmsChannelCollection : Hashtable, IDisposable
	{
		// ���ȱʡ�˻�������Ϣ�Ļص�������ַ
		// public Delegate_AskAccountInfo procAskAccountInfo = null;

        public event AskAccountInfoEventHandle AskAccountInfo = null;

		public bool GUI = true;

		public RmsChannelCollection()
		{
			//
			// TODO: Add constructor logic here
			//
		}

         // 2011/1/19
        public void Dispose()
        {
            foreach (string key in this.Keys)
            {
                RmsChannel channel = (RmsChannel)this[key];
                if (channel != null)
                {
                    try
                    {
                        channel.Close();
                    }
                    catch
                    {
                    }
                }
            }

            this.Clear();
            this.AskAccountInfo = null;
        }

        public void OnAskAccountInfo(object sender, AskAccountInfoEventArgs e)
        {
            if (this.AskAccountInfo == null)
            {
                e.ErrorInfo = "AskAccountInfo�¼�����δ����";
                e.Result = -1;
                return;
            }

            if (this.AskAccountInfo != null)
                this.AskAccountInfo(sender, e);
        }

		// ���һ��Channel����
		// ����������Ѿ��������������ֱ�ӷ��أ����򴴽�һ���¶���
		public RmsChannel GetChannel(string strUrl)
		{
			string strRegularUrl = strUrl.ToUpper();

			RmsChannel channel = (RmsChannel)this[strRegularUrl];

			if (channel != null)
				return channel;

			// ����
			channel = new RmsChannel();
			channel.Url = strUrl;
			channel.Container = this;

			this.Add(strRegularUrl, channel);
			return channel;
		}
 
        // TPPD: ��Ҫ����Щ��ʱ��������������ڱ�Ҫ��ʱ�����Close()
        // ����һ����ʱChannel����
        public RmsChannel CreateTempChannel(string strUrl)
        {
            string strRegularUrl = strUrl.ToUpper();

            // ����
            RmsChannel channel = new RmsChannel();
            channel.Url = strUrl;
            channel.Container = this;

            // this.Add(strRegularUrl, channel);

            return channel;
        }

        // 2011/1/19
        public void Close()
        {
            this.Dispose();
        }
	}

    /*
	// ���ȱʡ�ʻ���Ϣ
	// return:
	//		2	already login succeed
	//		1	dialog return OK
	//		0	dialog return Cancel
	//		-1	other error
	public delegate int Delegate_AskAccountInfo(
	ChannelCollection Channels, 
	string strComment,
	string strUrl,
	string strPath,
	LoginStyle loginStyle,
	out IWin32Window owner,	// �����Ҫ���ֶԻ������ﷵ�ضԻ��������Form
	out string strUserName,
	out string strPassword);
     */

	public class AccessKeyInfo
	{
		public string Key = "";
		public string KeyNoProcess = "";
		public string Num = "";
		public string FromName = "";	// ����;����
		public string FromValue = "";	// key��from�ֶ�ֵ
		public string ID = "";
	}

    // �¼�: ѯ���ʻ���Ϣ
    public delegate void AskAccountInfoEventHandle(object sender,
    AskAccountInfoEventArgs e);

    public class AskAccountInfoEventArgs : EventArgs
    {
        // �������
	    public RmsChannelCollection Channels = null;
        public RmsChannel Channel = null;   // [in] �����Channel��  ��� == null���Ŵ� Channels������� Url ���� 2013/2/14
	    public string Comment = "";
	    public string Url = "";
	    public string Path = "";
	    public LoginStyle LoginStyle;
        // �������
	    public IWin32Window Owner = null;	// �����Ҫ���ֶԻ������ﷵ�ضԻ��������Form
	    public string UserName = "";
	    public string Password = "";

        public int Result = 0;
        public string ErrorInfo = "";
        // return:
        //		2	already login succeed
        //		1	dialog return OK
        //		0	dialog return Cancel
        //		-1	other error
    }


}
