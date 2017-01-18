using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using DigitalPlatform;
using DigitalPlatform.CirculationClient;
using DigitalPlatform.LibraryClient;
using DigitalPlatform.Text;

namespace dp2Catalog
{
    static class Program
    {
        /// <summary>
        /// ǰ�ˣ�Ҳ���� dp2catalog.exe �İ汾��
        /// </summary>
        public static string ClientVersion { get; set; }

        static bool bExiting = false;

        static MainForm _mainForm = null;
        // ������ _mainForm �洢���ڶ��󣬲���ȡ Form.ActiveForm ��ȡ�ķ�ʽ��ԭ������
        // http://stackoverflow.com/questions/17117372/form-activeform-occasionally-works
        // Form.ActiveForm occasionally works

        public static MainForm MainForm
        {
            get
            {
                return _mainForm;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ClientVersion = Assembly.GetAssembly(typeof(Program)).GetName().Version.ToString();
#if NO
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
#endif
            if (IsDevelopMode() == false)
                PrepareCatchException();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _mainForm = new MainForm();
            Application.Run(_mainForm);
        }

        public static bool IsDevelopMode()
        {
            string[] args = Environment.GetCommandLineArgs();
            int i = 0;
            foreach (string arg in args)
            {
                if (i > 0 && arg == "develop")
                    return true;
                i++;
            }

            return false;
        }

        // ׼���ӹ�δ������쳣
        static void PrepareCatchException()
        {
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        static void CurrentDomain_UnhandledException(object sender,
    UnhandledExceptionEventArgs e)
        {
            if (bExiting == true)
                return;

            Exception ex = (Exception)e.ExceptionObject;
            string strError = GetExceptionText(ex, "");

            // TODO: ����Ϣ�ṩ������ƽ̨�Ŀ�����Ա���Ա����
            // TODO: ��ʾΪ��ɫ���ڣ���ʾ�������˼
            bool bSendReport = true;
            DialogResult result = MessageDlg.Show(_mainForm,
    "dp2Catalog ����δ֪���쳣:\r\n\r\n" + strError + "\r\n---\r\n\r\n�㡰�رա����رճ���",
    "dp2Catalog ����δ֪���쳣",
    MessageBoxButtons.OK,
    MessageBoxDefaultButton.Button1,
    ref bSendReport,
    new string[] { "�ر�" },
    "����Ϣ���͸�������");
            // �����쳣����
            if (bSendReport)
                CrashReport(strError);
        }

        static string GetExceptionText(Exception ex, string strType)
        {
            // Exception ex = (Exception)e.Exception;
            string strError = "����δ�����" + strType + "�쳣: \r\n" + ExceptionUtil.GetDebugText(ex);
            Assembly myAssembly = Assembly.GetAssembly(typeof(Program));
            strError += "\r\ndp2Catalog �汾: " + myAssembly.FullName;
            strError += "\r\n����ϵͳ��" + Environment.OSVersion.ToString();
            strError += "\r\n���� MAC ��ַ: " + StringUtil.MakePathList(SerialCodeForm.GetMacAddress());

            // TODO: ��������ϵͳ��һ����Ϣ

            // MainForm main_form = Form.ActiveForm as MainForm;
            if (_mainForm != null)
            {
                try
                {
                    _mainForm.WriteErrorLog(strError);
                }
                catch
                {
                    WriteWindowsLog(strError, EventLogEntryType.Error);
                }
            }
            else
                WriteWindowsLog(strError, EventLogEntryType.Error);

            return strError;
        }

        static void Application_ThreadException(object sender,
            ThreadExceptionEventArgs e)
        {
            if (bExiting == true)
                return;

            Exception ex = (Exception)e.Exception;
            string strError = GetExceptionText(ex, "�����߳�");

            bool bSendReport = true;
            DialogResult result = MessageDlg.Show(_mainForm,
    "dp2Catalog ����δ֪���쳣:\r\n\r\n" + strError + "\r\n---\r\n\r\n�Ƿ�رճ���?",
    "dp2Catalog ����δ֪���쳣",
    MessageBoxButtons.YesNo,
    MessageBoxDefaultButton.Button2,
    ref bSendReport,
    new string[] { "�ر�", "����" },
    "����Ϣ���͸�������");
            {
                if (bSendReport)
                    CrashReport(strError);
            }
            if (result == DialogResult.Yes)
            {
                //End();
                bExiting = true;
                Application.Exit();
            }
        }

        public static string GetMacAddressString()
        {
            List<string> macs = SerialCodeForm.GetMacAddress();
            return StringUtil.MakePathList(macs);
        }

        public static void CrashReport(string strText)
        {
            // MainForm main_form = Form.ActiveForm as MainForm;

            MessageBar _messageBar = null;

            _messageBar = new MessageBar();
            _messageBar.TopMost = false;
            //_messageBar.BackColor = SystemColors.Info;
            //_messageBar.ForeColor = SystemColors.InfoText;
            _messageBar.Text = "dp2Catalog �����쳣";
            _messageBar.MessageText = "������ dp2003.com �����쳣���� ...";
            _messageBar.StartPosition = FormStartPosition.CenterScreen;
            _messageBar.Show(_mainForm);
            _messageBar.Update();

            int nRet = 0;
            string strError = "";
            try
            {
                string strSender = "";
                // if (_mainForm != null)
                    strSender = "@MAC:" + GetMacAddressString();

                // ��������
                nRet = LibraryChannel.CrashReport(
                    strSender,
                    "dp2catalog",
                    strText,
                    out strError);
            }
            catch (Exception ex)
            {
                strError = "CrashReport() ���̳����쳣: " + ExceptionUtil.GetDebugText(ex);
                nRet = -1;
            }
            finally
            {
                _messageBar.Close();
                _messageBar = null;
            }

            if (nRet == -1)
            {
                strError = "�� dp2003.com �����쳣����ʱ����δ�ܷ��ͳɹ�����ϸ���: " + strError;
                MessageBox.Show(_mainForm, strError);
                // д�������־
                if (_mainForm != null)
                    _mainForm.WriteErrorLog(strError);
                else
                    WriteWindowsLog(strError, EventLogEntryType.Error);
            }
        }

        // д��Windowsϵͳ��־
        public static void WriteWindowsLog(string strText,
            EventLogEntryType type)
        {
            EventLog Log = new EventLog("Application");
            Log.Source = "dp2Catalog";
            Log.WriteEntry(strText, type);
        }
    }
}