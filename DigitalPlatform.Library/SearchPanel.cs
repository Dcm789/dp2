using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using System.Runtime.Serialization;

using System.Runtime.InteropServices;
using System.Collections.Specialized;

using System.Text;
using System.Web;
using System.Threading;
using System.Xml;

using DigitalPlatform;
using DigitalPlatform.rms.Client;
using DigitalPlatform.Xml;
using DigitalPlatform.GUI;
using DigitalPlatform.Text;

namespace DigitalPlatform.Library
{
	/// <summary>
	/// ���ڽ��в����ĳ��׻���
	/// </summary>
	public class SearchPanel
	{
        /// <summary>
        /// �����¼����
        /// </summary>
		public event BrowseRecordEventHandler BrowseRecord = null;

        /// <summary>
        /// Ӧ�ó�����Ϣ
        /// </summary>
		public ApplicationInfo ap = null;	// ����

        /// <summary>
        /// ��ap�б��洰�����״̬�ı����ַ���
        /// </summary>
		public string ApCfgTitle = "";

        /// <summary>
        /// ֹͣ������
        /// </summary>
		public DigitalPlatform.StopManager	stopManager = new DigitalPlatform.StopManager();

        /// <summary>
        /// �����ļ�����
        /// </summary>
		public CfgCache cfgCache = null;	// ����

        /// <summary>
        /// ��������Ϣ����
        /// </summary>
		public ServerCollection Servers = null;	// ����

        // TODO: ��Ҫ���� IDisposeable �ӿ�
        /// <summary>
        /// ͨ������
        /// </summary>
		public RmsChannelCollection Channels = new RmsChannelCollection();	// ӵ��

        /// <summary>
        /// ���ڹ���ֹͣ�����Ķ���
        /// </summary>
		DigitalPlatform.Stop stop = null;

        /// <summary>
        /// ͨ��
        /// </summary>
		RmsChannel channel = null;

		string m_strServerUrl = "";

        /// <summary>
        /// ���캯��
        /// </summary>
		public SearchPanel()
		{
		}

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="servers">��������Ϣ����</param>
        /// <param name="cfgcache">�����ļ�����</param>
		public void Initial(ServerCollection servers,
			CfgCache cfgcache)
		{
			this.Servers = servers;

            /*
			this.Channels.procAskAccountInfo = 
				new Delegate_AskAccountInfo(this.Servers.AskAccountInfo);
             */
            this.Channels.AskAccountInfo -= new AskAccountInfoEventHandle(this.Servers.OnAskAccountInfo);
            this.Channels.AskAccountInfo += new AskAccountInfoEventHandle(this.Servers.OnAskAccountInfo);

			this.cfgCache = cfgcache;
		}

        /// <summary>
        /// ��ʼ��ֹͣ������
        /// </summary>
        /// <param name="buttonStop">ֹͣ��ť</param>
        /// <param name="labelMessage">��Ϣ��ǩ</param>
		public void InitialStopManager(Button buttonStop,
			Label labelMessage)
		{
			stopManager.Initial(buttonStop,
				labelMessage,
                null);
			stop = new DigitalPlatform.Stop();
            stop.Register(this.stopManager, true);	// ����������
		}

        /// <summary>
        /// ��ʼ��ֹͣ������
        /// </summary>
        /// <param name="toolbarbuttonstop">�������ϵ�ֹͣ��ť</param>
        /// <param name="statusbar">״̬��</param>
		public void InitialStopManager(ToolBarButton toolbarbuttonstop,
			StatusBar statusbar)
		{
			stopManager.Initial(toolbarbuttonstop,
				statusbar,
                null);
			stop = new DigitalPlatform.Stop();
            stop.Register(this.stopManager, true);	// ����������
		}

        /// <summary>
        /// ��ֹͣ�������������
        /// </summary>
		public void FinishStopManager()
		{
			if (stop != null) // �������
			{
				stop.Unregister();	// ����������
				stop = null;
			}
		}

        /// <summary>
        /// ��ap��װ��Form״̬��Ϣ
        /// </summary>
        /// <param name="form">Form����</param>
		public void LoadFormStates(Form form)
		{
			if (ap != null) 
			{
				if (ApCfgTitle != "" && ApCfgTitle != null) 
				{
					ap.SaveFormStates(form,
						ApCfgTitle);
				}
				else 
				{
					Debug.Assert(true, "��Ҫ��ap����ͻָ��������״̬������������ApCfgTitle��Ա");
				}
			}
		}

        /// <summary>
        /// ��Form״̬��Ϣ���浽ap��
        /// </summary>
        /// <param name="form">Form����</param>
		public void SaveFormStates(Form form)
		{
			if (ap != null) 
			{
				if (ApCfgTitle != "" && ApCfgTitle != null) 
				{
					ap.SaveFormStates(form,
						ApCfgTitle);
				}
				else 
				{
					Debug.Assert(true, "��Ҫ��ap����ͻָ��������״̬������������ApCfgTitle��Ա");
				}

			}
		}

        /// <summary>
        /// ȱʡ������URL
        /// </summary>
		public virtual string ServerUrl
		{
			get 
			{
				return m_strServerUrl;
			}
			set 
			{
				m_strServerUrl = value;
			}
		}

        /// <summary>
        /// ��������ļ�
        /// </summary>
        /// <param name="strServerUrl">������URL�����Ϊnull�����Զ�ʹ��this.ServerUrl</param>
        /// <param name="strCfgFilePath">�����ļ���·����������ServerUrl����</param>
        /// <param name="strContent">���������ļ�����</param>
        /// <param name="strError">���ش�����Ϣ</param>
        /// <returns>-1����;0û���ҵ�;1�ҵ�</returns>
		public int GetCfgFile(
			string strServerUrl,
			string strCfgFilePath,
			out string strContent,
			out string strError)
		{
			strError = "";
			strContent = "";

			if (strServerUrl == "" || strServerUrl == null)
			{
				strServerUrl = this.ServerUrl;
			}

			if (strServerUrl == "")
			{
				strError = "��δָ��������URL";
				return -1;
			}

			RmsChannel channelSave = channel;

			channel = Channels.GetChannel(strServerUrl);
			if (channel == null)
			{
				strError = "get channel error";
				return -1;
			}

			try 
			{

				this.BeginLoop("���������ļ�" + strCfgFilePath);

				byte[] baTimeStamp = null;
				string strMetaData;
				string strOutputPath;

				long lRet = channel.GetRes(
					this.cfgCache,
					strCfgFilePath,
					out strContent,
					out strMetaData,
					out baTimeStamp,
					out strOutputPath,
					out strError);

				this.EndLoop();



				if (lRet == -1) 
				{
					if (channel.ErrorCode == ChannelErrorCode.NotFound)
						return 0;	// not found
					return -1;
				}

				return 1;	// found
			}
			finally 
			{
				this.channel = channelSave;
			}
		}

        /// <summary>
        /// ��ֹͣ��ť����ʱ�����Ķ���
        /// </summary>
		public void DoStopClick()
		{
			if (stopManager != null)
				stopManager.DoStopActive();
		}

        /// <summary>
        /// ��ֹͨѶ����
        /// </summary>
		public void DoStop(object sender, StopEventArgs e)
		{
			if (this.channel != null)
				this.channel.Abort();
		}

		// 
		// return:
		//		-1	error
		//		0	not found
		//		1	found
        /// <summary>
        /// ��������ļ�
        /// </summary>
        /// <param name="strServerUrl">������URL�����Ϊnull�����Զ�ʹ��this.ServerUrl</param>
        /// <param name="strCfgFilePath">�����ļ���·����������ServerUrl����</param>
        /// <param name="dom">����װ���������ļ����ݵ�XmlDocument����</param>
        /// <param name="strError"></param>
        /// <returns>-1����;0û���ҵ�;1�ҵ�</returns>
		public int GetCfgFile(
			string strServerUrl,
			string strCfgFilePath,
			out XmlDocument dom,
			out string strError)
		{
			strError = "";
			dom = null;

			string strContent = "";

			int nRet = GetCfgFile(
				strServerUrl,
				strCfgFilePath,
				out strContent,
				out strError);
			if (nRet == -1)
				return -1;
			if (nRet == 0)
				return 0;

			dom = new XmlDocument();

			try 
			{
				dom.LoadXml(strContent);
			}
			catch (Exception ex)
			{
				dom = null;
				strError = "װ�������ļ� '"+strCfgFilePath+"' ���ݽ���domʧ��: " + ex.Message;
				return -1;
			}

			return 1;
		}

        /// <summary>
        /// ��ʼ����ѭ��
        /// </summary>
        /// <param name="strMessage">ѭ���ڼ�Ҫ��ʾ��״̬�е���ʾ��Ϣ</param>
		public void BeginLoop(string strMessage)
		{
			if (stop != null)
			{
                stop.OnStop += new StopEventHandler(this.DoStop);
				stop.Initial(strMessage);
				stop.BeginLoop();
			}
		}

        /// <summary>
        /// ��������ѭ��
        /// </summary>
		public void EndLoop()
		{
			if (stop != null)
			{
				stop.EndLoop();
                stop.OnStop -= new StopEventHandler(this.DoStop);
				stop.Initial("");
			}
		}


        /// <summary>
        /// ����һ�����н��
        /// </summary>
        /// <param name="strServerUrl">������URL</param>
        /// <param name="strQueryXml">����ʽXML</param>
        /// <param name="strPath">���صļ�¼·��</param>
        /// <param name="strError">���صĴ�����Ϣ</param>
        /// <returns>-1	һ�����;0	not found;1	found;>1	���ж���һ��</returns>
		public int SearchOnePath(
            string strServerUrl,
			string strQueryXml,
			out string strPath,
			out string strError)
		{
			strPath = "";
			strError = "";

            if (String.IsNullOrEmpty(strServerUrl) == true)
                strServerUrl = this.ServerUrl;

			RmsChannel channelSave = channel;

			channel = Channels.GetChannel(strServerUrl);
			if (channel == null)
			{
				strError = "get channel error";
				return -1;
			}

			try 
			{


				long lRet = channel.DoSearch(strQueryXml,
                    "default",
                    "", // strOuputStyle
                    out strError);
				if (lRet == -1) 
					return -1;

				if (lRet == 0) 
				{
					return 0;	// û���ҵ�
				}

				long lCount = lRet;

				if (lRet > 1)
				{
					strError = "���� " + Convert.ToString(lRet) + " ����";
				}

				List<string> aPath = null;
				lRet = channel.DoGetSearchResult(
                    "default",
                    0,
					1,
					"zh",
					this.stop,
					out aPath,
					out strError);
				if (lRet == -1) 
				{
					strError = "��ȡ�������ʱ����: " + strError;
					return -1;
				}


				strPath = (string)aPath[0];

				return (int)lCount;
			}
			finally 
			{
				channel = channelSave;
			}
		}


        /// <summary>
        /// �����õ��������н��
        /// </summary>
        /// <param name="strServerUrl">������URL</param>
        /// <param name="strQueryXml">����ʽXML</param>
        /// <param name="nMax">�������</param>
        /// <param name="aPath">���صļ�¼·������</param>
        /// <param name="strError">���صĴ�����Ϣ</param>
        /// <returns>-1	һ�����;0	not found;1	found;>1	���ж���һ��</returns>
        public int SearchMultiPath(
            string strServerUrl,
            string strQueryXml,
            int nMax,
            out List<string> aPath,
            out string strError)
        {
            aPath = null;
            strError = "";

            if (String.IsNullOrEmpty(strServerUrl) == true)
                strServerUrl = this.ServerUrl;

            RmsChannel channelSave = channel;

            channel = Channels.GetChannel(strServerUrl);
            if (channel == null)
            {
                strError = "get channel error";
                return -1;
            }

            try
            {


                long lRet = channel.DoSearch(strQueryXml,
                    "default",
                    "", // strOuputStyle
                    out strError);
                if (lRet == -1)
                    return -1;

                if (lRet == 0)
                {
                    return 0;	// û���ҵ�
                }

                long lCount = lRet;

                lCount = Math.Min(lCount, nMax);

                if (lRet > 1)
                {
                    strError = "���� " + Convert.ToString(lRet) + " ����";
                }

                lRet = channel.DoGetSearchResult(
                    "default",
                    0,
                    lCount,
                    "zh",
                    this.stop,
                    out aPath,
                    out strError);
                if (lRet == -1)
                {
                    strError = "��ȡ�������ʱ����: " + strError;
                    return -1;
                }

                return (int)lCount;
            }
            finally
            {
                channel = channelSave;
            }
        }


        /// <summary>
        /// ��ȡ��¼
        /// </summary>
        /// <param name="strServerUrl">������URL�����==null����ʾ��SearchPannel�Լ���ServerUrl</param>
        /// <param name="strPath">��¼·��</param>
        /// <param name="dom">����װ���˼�¼���ݵ�XmlDocument����</param>
        /// <param name="baTimeStamp">����ʱ���</param>
        /// <param name="strError">���ش�����Ϣ</param>
        /// <returns>-1	error;0	not found;1	found</returns>
		public int GetRecord(
            string strServerUrl,
            string strPath,
			out XmlDocument dom,
			out byte[] baTimeStamp,
			out string strError)
		{
			dom = null;
			string strXml = "";

            int nRet = GetRecord(
                strServerUrl,
                strPath,
                out strXml,
                out baTimeStamp,
                out strError);
			if (nRet == -1 || nRet == 0)
				return nRet;

			dom = new XmlDocument();
			try 
			{
				dom.LoadXml(strXml);
			}
			catch(Exception ex)
			{
				strError = "װ��·��Ϊ '"+strPath+"' ��xml��¼ʱ����: " + ex.Message;
				return -1;
			}

			return 1;
		}


        /// <summary>
        /// ��ȡ�����¼�������¼��İ汾
        /// </summary>
        /// <param name="fullpaths">���ɼ�¼ȫ·��</param>
        /// <param name="bReverse">fullpaths��·���Ƿ�Ϊ��ת��ʽ</param>
        /// <param name="strStyle">�������</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1	error;0	not found;1	found</returns>
		public int GetBrowseRecord(string [] fullpaths,
			bool bReverse,
			string strStyle,
			out string strError)
		{
			strError = "";
			int nRet = 0;

			int nIndex = 0;

			ArrayList aTempPath = new ArrayList();
			ArrayList aTempRecord = new ArrayList();
			string strLastUrl = "";
			for(int i=0;i<=fullpaths.Length;i++)
			{
				bool bPush = false;
				ResPath respath = null;

				if (i<fullpaths.Length)
				{
					string strFullPath = fullpaths[i];

					if (bReverse == true)
						strFullPath = ResPath.GetRegularRecordPath(strFullPath);

					respath = new ResPath(strFullPath);

					if ( respath.Url != strLastUrl 
						|| aTempPath.Count >= 5)	// ���5��Ϊһ���������û��ȴ���
						bPush = true;

				}
				else 
				{
					bPush = true;
				}

				if ( bPush == true && aTempPath.Count > 0)
				{
							
					string [] temppaths = new string[aTempPath.Count];
					for(int j=0;j<temppaths.Length;j++)
					{
						temppaths[j] = (string)aTempPath[j];
					}
					nRet = GetBrowseRecord(
						strLastUrl,
						temppaths,
						strStyle,
						out aTempRecord,
						out strError);
					if (nRet == -1)
						return -1;

					// �����¼�
					if (this.BrowseRecord != null)
					{
						
						for(int j=0;j<aTempRecord.Count;j++)
						{
							BrowseRecordEventArgs e = new BrowseRecordEventArgs();
							e.SearchCount = 0;
							e.Index = nIndex ++;
							e.FullPath = strLastUrl + "?" + temppaths[j];
							e.Cols = (string[])aTempRecord[j];
							this.BrowseRecord(this, e);
							if (e.Cancel == true)
							{
								if (e.ErrorInfo == "")
									strError = "�û��ж�";
								else
									strError = e.ErrorInfo;
								return -2;
							}
						}
					}

					aTempRecord.Clear();

					aTempPath.Clear();
				}


				if (i<fullpaths.Length)
				{
					aTempPath.Add(respath.Path);

					strLastUrl = respath.Url;
				}

			} // end of for

			return 0;

		}

	


        /// <summary>
        ///  ��ȡ�����¼
        /// </summary>
        /// <param name="fullpaths">���ɼ�¼ȫ·��</param>
        /// <param name="bReverse">fullpaths��·���Ƿ�Ϊ��ת��ʽ</param>
        /// <param name="strStyle">�������</param>
        /// <param name="records">���ؼ�¼����</param>
        /// <param name="strError">���س�����Ϣ</param>
        /// <returns>-1	error;0	not found;1	found</returns>
		public int GetBrowseRecord(string [] fullpaths,
			bool bReverse,
			string strStyle,
			out ArrayList records,
			out string strError)
		{
			strError = "";
			records = new ArrayList();
			int nRet = 0;


			ArrayList aTempPath = new ArrayList();
			ArrayList aTempRecord = new ArrayList();
			string strLastUrl = "";
			for(int i=0;i<=fullpaths.Length;i++)
			{
				bool bPush = false;
				ResPath respath = null;

				if (i<fullpaths.Length)
				{
					string strFullPath = fullpaths[i];

					if (bReverse == true)
						strFullPath = ResPath.GetRegularRecordPath(strFullPath);

					respath = new ResPath(strFullPath);

					if ( respath.Url != strLastUrl )
						bPush = true;

				}
				else 
				{
					bPush = true;
				}

				if ( bPush == true && aTempPath.Count > 0)
				{
							
					string [] temppaths = new string[aTempPath.Count];
					for(int j=0;j<temppaths.Length;j++)
					{
						temppaths[j] = (string)aTempPath[j];
					}
					nRet = GetBrowseRecord(
						strLastUrl,
						temppaths,
						strStyle,
						out aTempRecord,
						out strError);
					if (nRet == -1)
						return -1;

					records.AddRange(aTempRecord);
					aTempRecord.Clear();

					aTempPath.Clear();
				}


				if (i<fullpaths.Length)
				{
					aTempPath.Add(respath.Path);

					strLastUrl = respath.Url;
				}

			} // end of for

			return 0;

		}

        /// <summary>
        /// ��ȡ�����¼
        /// </summary>
        /// <param name="strServerUrl"></param>
        /// <param name="paths"></param>
        /// <param name="strStyle"></param>
        /// <param name="records"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
		public int GetBrowseRecord(
			string strServerUrl,
			string [] paths,
			string strStyle,
			out ArrayList records,
			out string strError)
		{
			strError = "";
			records = null;

			if (String.IsNullOrEmpty(strServerUrl) == true)
				strServerUrl = this.ServerUrl;

			RmsChannel channelSave = channel;
			
			channel = Channels.GetChannel(strServerUrl);
			if (channel == null)
			{
				strError = "get channel error";
				return -1;
			}

			try 
			{
				// �����ƶ��ļ�¼·����������ʽ��¼
				// parameter:
				//		aRecord	���ص������¼��Ϣ��һ��ArrayList���顣ÿ��Ԫ��Ϊһ��string[]��������������
				//				����strStyle���������strStyle����id����aRecordÿ��Ԫ���е�string[]��һ���ַ�������id�������Ǹ������ݡ�
				return channel.GetBrowseRecords(paths,
					strStyle,
					out records,
					out strError);
			}

			finally 
			{
				channel = channelSave;
			}

		}


        /// <summary>
        /// ��ȡ��¼
        /// </summary>
        /// <param name="strServerUrl">������URL�����==null����ʾ��SearchPannel�Լ���ServerUrl</param>
        /// <param name="strPath"></param>
        /// <param name="strXml"></param>
        /// <param name="baTimeStamp"></param>
        /// <param name="strError"></param>
        /// <returns>-1	error;0	not found;1	found</returns>
		public int GetRecord(
            string strServerUrl,
            string strPath,
			out string strXml,
			out byte[] baTimeStamp,
			out string strError)
		{
			strError = "";
			baTimeStamp = null;
			strXml = "";

            if (String.IsNullOrEmpty(strServerUrl) == true)
                strServerUrl = this.ServerUrl;

			RmsChannel channelSave = channel;
			
			channel = Channels.GetChannel(strServerUrl);
			if (channel == null)
			{
				strError = "get channel error";
				return -1;
			}

			try 
			{

				// ȡ��¼
				string strStyle = "content,data,timestamp";

				string strMetaData;
				string strOutputPath;


				long lRet = channel.GetRes(strPath,
					strStyle,
					out strXml,
					out strMetaData,
					out baTimeStamp,
					out strOutputPath,
					out strError);
				if (lRet == -1) 
				{
					strError = "��ȡ '" + strPath + "' ��¼��ʱ����: " + strError;
					if (channel.ErrorCode == ChannelErrorCode.NotFound)
					{
						return 0;
					}

					return -1;
				}


				return 1;
			}

			finally 
			{
				channel = channelSave;
			}

		}


        /// <summary>
        /// �����¼
        /// </summary>
        /// <param name="strServerUrl">������URL</param>
        /// <param name="strPath"></param>
        /// <param name="strXml"></param>
        /// <param name="baTimestamp"></param>
        /// <param name="bForceSaveOnTimestampMismatch"></param>
        /// <param name="baOutputTimestamp"></param>
        /// <param name="strError"></param>
        /// <returns>-2	ʱ�����ƥ��;-1	һ�����;0	����</returns>
		public int SaveRecord(
            string strServerUrl,
			string strPath,
			string strXml,
			byte [] baTimestamp,
			bool bForceSaveOnTimestampMismatch,
			out byte [] baOutputTimestamp,
			out string strError)
		{
			strError = "";
			baOutputTimestamp = null;

            if (String.IsNullOrEmpty(strServerUrl) == true)
                strServerUrl = this.ServerUrl;

			RmsChannel channelSave = channel;

			channel = Channels.GetChannel(strServerUrl);
			if (channel == null)
			{
				strError = "get channel error";
				return -1;
			}

			try 
			{
				string strOutputPath = "";

			REDO:

				long lRet = channel.DoSaveTextRes(strPath,
					strXml,
					false,	// bInlucdePreamble
					"",	// style
					baTimestamp,
					out baOutputTimestamp,
					out strOutputPath,
					out strError);
				if (lRet == -1)
				{
					if (channel.ErrorCode == ChannelErrorCode.TimestampMismatch)
					{
						if (bForceSaveOnTimestampMismatch == true)
						{
							baTimestamp = baOutputTimestamp;
							goto REDO;
						}
						else
							return -2;

					}

					return -1;
				}

				strError = channel.ErrorInfo;	// ����API���

				return 0;
			}
			finally 
			{
				channel = channelSave;
			}
		}


        /// <summary>
        /// ģ�ⴴ��������
        /// </summary>
        /// <param name="strServerUrl">������URL��ΪʲôҪ���������URL����������this.ServerUrl? ��ΪҪģ�������ļ�¼���������URL����������������URL��ͬ��</param>
        /// <param name="strPath"></param>
        /// <param name="strXml"></param>
        /// <param name="aLine"></param>
        /// <param name="strError"></param>
        /// <returns>-1	һ�����;0	����</returns>
		public int GetKeys(
			string strServerUrl,
			string strPath,
			string strXml,
            out List<AccessKeyInfo> aLine,
			out string strError)
		{
			strError = "";
			aLine = null;

            if (String.IsNullOrEmpty(strServerUrl) == true)
                strServerUrl = this.ServerUrl;

			
			RmsChannel channelSave = channel;

			channel = Channels.GetChannel(strServerUrl);
			if (channel == null)
			{
				strError = "get channel error";
				return -1;
			}

			try 
			{
				long lRet = channel.DoGetKeys(
                    strPath,
					strXml,
					"zh",	// strLang
					// "",	// strStyle
					null,	// this.stop,
					out aLine,
					out strError);
				if (lRet == -1)
				{
					return -1;
				}
				return 0;
			}
			finally 
			{
				channel = channelSave;
			}
		}

        /// <summary>
        /// ����ʵ�ÿ�
        /// </summary>
        /// <param name="strDbName"></param>
        /// <param name="strFrom"></param>
        /// <param name="strKey"></param>
        /// <param name="dom"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
		public int SearchUtilDb(
			string strDbName,
			string strFrom,
			string strKey,
			out XmlDocument dom,
			out string strError)
		{
			strError = "";
			dom = null;

            // 2007/4/5 ���� ������ GetXmlStringSimple()
			string strQueryXml = "<target list='"
                + StringUtil.GetXmlStringSimple(strDbName + ":" + strFrom)       // 2007/9/14
                + "'><item><word>"
				+ StringUtil.GetXmlStringSimple(strKey)
                + "</word><match>exact</match><relation>=</relation><dataType>string</dataType><maxCount>-1</maxCount></item><lang>zh</lang></target>";

			string strPath = "";

			// ����һ�����н��
			// return:
			//		-1	һ�����
			//		0	not found
			//		1	found
			//		>1	���ж���һ��
			int nRet = SearchOnePath(
                null,
				strQueryXml,
				out strPath,
				out strError);
			if (nRet == -1)
				return -1;
			if (nRet == 0)
				return 0;

			byte [] baTimeStamp = null;
			// ��ȡ��¼
			// return:
			//		-1	error
			//		0	not found
			//		1	found
			nRet = this.GetRecord(
                null,
                strPath,
				out dom,
				out baTimeStamp,
				out strError);
			if (nRet == -1)
				return -1;
			if (nRet == 0)
				return 0;

			return 1;
		}

        /// <summary>
        /// ����ʵ�ÿ�
        /// </summary>
        /// <param name="strDbName"></param>
        /// <param name="strFrom"></param>
        /// <param name="strKey"></param>
        /// <param name="strValueAttrName"></param>
        /// <param name="strValue"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
		public int SearchUtilDb(
			string strDbName,
			string strFrom,
			string strKey,
			string strValueAttrName,
			out string strValue,
			out string strError)
		{
			strError = "";
			strValue = "";

            // 2007/4/5 ���� ������ GetXmlStringSimple()
			string strQueryXml = "<target list='"
                + StringUtil.GetXmlStringSimple(strDbName + ":" + strFrom)       // 2007/9/14
                + "'><item><word>"
				+ StringUtil.GetXmlStringSimple(strKey)
                + "</word><match>exact</match><relation>=</relation><dataType>string</dataType><maxCount>-1</maxCount></item><lang>zh</lang></target>";

			string strPath = "";

			// ����һ�����н��
		// return:
		//		-1	һ�����
		//		0	not found
		//		1	found
		//		>1	���ж���һ��
			int nRet = SearchOnePath(
                null,
				strQueryXml,
				out strPath,
				out strError);
			if (nRet == -1)
				return -1;
			if (nRet == 0)
				return 0;

			byte [] baTimeStamp = null;
			XmlDocument domRecord = null;
			// ��ȡ��¼
			// return:
			//		-1	error
			//		0	not found
			//		1	found
			nRet = this.GetRecord(
                null,
                strPath,
				out domRecord,
				out baTimeStamp,
				out strError);
			if (nRet == -1)
				return -1;
			if (nRet == 0)
				return 0;

			strValue = DomUtil.GetAttr(domRecord.DocumentElement, strValueAttrName);

			return 1;
		}



        /// <summary>
        /// ��������ȡ������
        /// </summary>
        /// <param name="strServerUrl">������URL�����==""����==null����ʾ��this.ServerUrl</param>
        /// <param name="strQueryXml"></param>
        /// <param name="bGetBrowseCols">�Ƿ�Ҫ��������</param>
        /// <param name="strError"></param>
        /// <returns>-2	�û��ж�;-1	һ�����;0	δ����;	>=1	����������������������</returns>
		public long SearchAndBrowse(
			string strServerUrl,
			string strQueryXml,
            bool bGetBrowseCols,
			out string strError)
		{
			strError = "";
			
			if (strServerUrl == null || strServerUrl == null)
				strServerUrl = this.ServerUrl;

			RmsChannel channelSave = channel;


			channel = Channels.GetChannel(strServerUrl);
			if (channel == null)
			{
				strError = "get channel error";
				return -1;
			}

			try 
			{

				// ����
				long lRet = channel.DoSearch(strQueryXml,
                    "default",
                    "", // strOuputStyle
                    out strError);
				if (lRet == -1) 
					return -1;

				if (lRet == 0) 
					return 0;

				// ѭ����ȡ���
				long nHitCount = lRet;
				long nStart = 0;
				long nCount = 10;
				long nIndex = 0;

				for(;;)
				{
					Application.DoEvents();	// ���ý������Ȩ

					if (stop != null) 
					{
						if (stop.State != 0)
						{
							strError = "�û��ж�";
							return -2;
						}
					}

					List<string> aPath = null;
                    ArrayList aLine = null;
                    if (bGetBrowseCols == false)
                    {
                        lRet = channel.DoGetSearchResult(
                    "default",
                            nStart,
                            nCount,
                            "zh",
                            this.stop,
                            out aPath,
                            out strError);
                    }
                    else
                    {
                        lRet = channel.DoGetSearchFullResult(
                    "default",
                            nStart,
                            nCount,
                            "zh",
                            this.stop,
                            out aLine,
                            out strError);
                    }
					if (lRet == -1) 
					{
						strError = "��ȡ�������ʱ����: " + strError;
						return -1;
					}

                    if (bGetBrowseCols == false)
					    nStart += aPath.Count;
                    else
                        nStart += aLine.Count;


					// �����¼�
					if (this.BrowseRecord != null)
					{
                        int nThisCount = 0;

                        if (bGetBrowseCols == false)
                            nThisCount = aPath.Count;
                        else
                            nThisCount = aLine.Count;


                        for (int j = 0; j < nThisCount; j++)
						{
							BrowseRecordEventArgs e = new BrowseRecordEventArgs();
							e.SearchCount = nHitCount;
							e.Index = nIndex ++;
                            if (bGetBrowseCols == false)
                            {
                                e.FullPath = strServerUrl + "?" + (string)aPath[j];
                            }
                            else
                            {
                                string[] cols = (string[])aLine[j];
                                e.FullPath = strServerUrl + "?" + cols[0];
                                // ������һ��
                                e.Cols = new string[Math.Max(cols.Length - 1, 0)];
                                Array.Copy(cols, 1, e.Cols, 0, cols.Length - 1);
                            }
							this.BrowseRecord(this, e);
							if (e.Cancel == true)
							{
								if (e.ErrorInfo == "")
									strError = "�û��ж�";
								else
									strError = e.ErrorInfo;
								return -2;
							}
						}
					}



					if (nStart >= nHitCount)
						break;

                    // 2006/9/24 add ��ֹnStart + nCountԽ��
                    if (nStart + nCount > nHitCount)
                        nCount = nHitCount - nStart;
                    else
                        nCount = 10;

				}


				return nHitCount;
			}
			finally 
			{
				channel = channelSave;
			}

		}



        /// <summary>
        /// ��marcdef�����ļ��л��marc��ʽ����
        /// </summary>
        /// <param name="strDbFullPath"></param>
        /// <param name="strMarcSyntax"></param>
        /// <param name="strError"></param>
        /// <returns>-1	����;0	û���ҵ�;1	�ҵ�</returns>
		public int GetMarcSyntax(string strDbFullPath,
			out string strMarcSyntax,
			out string strError)
		{
			strError = "";
			strMarcSyntax = "";

			ResPath respath = new ResPath(strDbFullPath);

			string strCfgFilePath = respath.Path + "/cfgs/marcdef";

			XmlDocument tempdom = null;
			// ��������ļ�
			// return:
			//		-1	error
			//		0	not found
			//		1	found
			int nRet = this.GetCfgFile(
				respath.Url,
				strCfgFilePath,
				out tempdom,
				out strError);
			if (nRet == -1)
				return -1;
			if (nRet == 0) 
			{
				strError = "�����ļ� '" + strCfgFilePath + "' û���ҵ�...";
				return 0;
			}

			XmlNode node = tempdom.DocumentElement.SelectSingleNode("//MARCSyntax");
			if (node == null)
			{
				strError = "marcdef�ļ� "+strCfgFilePath+" ��û��<MARCSyntax>Ԫ��";
				return 0;
			}

			strMarcSyntax = DomUtil.GetNodeText(node);

			strMarcSyntax = strMarcSyntax.ToLower();

			return 1;
		}


	}



    /// <summary>
    /// �����¼����
    /// </summary>
    /// <param name="sender">������</param>
    /// <param name="e">�¼�����</param>
	public delegate void BrowseRecordEventHandler(object sender,
	BrowseRecordEventArgs e);

    /// <summary>
    /// �����¼�����¼�����
    /// </summary>
	public class BrowseRecordEventArgs: EventArgs
	{
        /// <summary>
        /// ��������
        /// </summary>
		public long SearchCount = 0;	// 

        /// <summary>
        /// ��ǰ��¼����ƫ��
        /// </summary>
		public long Index = 0;	// 

        /// <summary>
        /// ��¼·��������ȫ·�������� http://dp2003.com/rmsservice/rmsservice.asmx?��Ŀ��/1
        /// </summary>
		public string FullPath = "";	// 

        /// <summary>
        /// ���������Ϣ
        /// </summary>
		public string [] Cols = null;	// 

        /// <summary>
        /// �Ƿ���Ҫ�ж�
        /// </summary>
		public bool Cancel = false;	// 

        /// <summary>
        /// �ص��ڼ䷢���Ĵ�����Ϣ
        /// </summary>
		public string ErrorInfo = "";	// 
	}

}
