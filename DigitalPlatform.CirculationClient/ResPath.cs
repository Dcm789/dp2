using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace DigitalPlatform.CirculationClient
{
    /// <summary>
    /// ����һ����Դ���ڵ��·���� ������ �� �¼�·�� 2����
    /// </summary>
    public class ResPath
    {
        public string Url = "";	// ������URL����
        public string Path = "";	// �������ڵ���Դ�ڵ�·������һ���ǿ���

        public ResPath()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public ResPath Clone()
        {
            ResPath newobj = new ResPath();
            newobj.Url = this.Url;
            newobj.Path = this.Path;
            return newobj;
        }

        // �������ڵ㹹�죬�ѵ�һ����ΪUrl���ݣ������������ϲ���������Path�������'/'
        public ResPath(TreeNode node)
        {
            string strFullPath = "";

            while (node != null)
            {
                if (node.Parent == null)
                {
                    dp2ServerNodeInfo info = (dp2ServerNodeInfo)node.Tag;
                    Debug.Assert(info != null, "");
                    Url = info.Url;
                    break;
                }
                else
                {
                    if (strFullPath != "")
                        strFullPath = "/" + strFullPath;
                    strFullPath = node.Text + strFullPath;
                }
                node = node.Parent;
            }
            Path = strFullPath;
        }

        // ��ȫ·������
        public ResPath(string strFullPath)
        {
            SetFullPath(strFullPath);
        }

        // ֻ�������ݿ�������
        public void MakeDbName()
        {
            this.Path = GetDbName(this.Path);
        }

        // ��һ����·��(����url����)�н�ȡ��������
        public static string GetDbName(string strLongPath)
        {
            int nRet = strLongPath.IndexOf("/");
            if (nRet == -1)
                return strLongPath;
            else
                return strLongPath.Substring(0, nRet);
        }

        // ��һ����·��(����url����)�н�ȡ��¼id����
        public static string GetRecordId(string strLongPath)
        {
            int nRet = strLongPath.IndexOf("/");
            if (nRet == -1)
                return strLongPath;
            else
                return strLongPath.Substring(nRet + 1).Trim();
        }

        // ��ȡ����·���е�id���֡����� this.PathΪ"���ݿ�/1"��Ӧ��ȡ��"1"
        public string GetRecordId()
        {
            string[] aPart = this.Path.Split(new char[] { '/' });

            if (aPart.Length < 2)
                return null;
            return aPart[1];
        }

        // ��ȡ����·���е�object id���֡����� this.PathΪ"���ݿ�/1/object/0"��Ӧ��ȡ��"1"
        public string GetObjectId()
        {
            string[] aPart = this.Path.Split(new char[] { '/' });

            if (aPart.Length < 4)
                return null;
            return aPart[3];
        }

        // parameters:
        //		strPath	���Ƿ�����URL�Ϳ��Լ�����·���ϳɵ�,�м���'?'
        public void SetFullPath(string strPath)
        {
            int nRet = strPath.IndexOf('?');

            if (nRet == -1)
            {
                Url = strPath.Trim();
                Path = "";
                return;
            }

            Url = strPath.Substring(0, nRet).Trim();
            Path = strPath.Substring(nRet + 1).Trim();
        }

        // �������ȫ·�����뱾����
        // parameters:
        //		strPath	���ǿ��Լ�����·�� �� ������URL �ϳɵ�,�м���'@'
        public void SetReverseFullPath(string strPath)
        {
            int nRet = strPath.IndexOf('@');

            if (nRet == -1)
            {
                Path = strPath.Trim();
                Url = "";
                return;
            }

            Path = strPath.Substring(0, nRet).Trim();
            Url = strPath.Substring(nRet + 1).Trim();
        }

        // ȫ·�������Ƿ�����URL�Ϳ��Լ�����·���ϳɵ�,�м���'?'
        public string FullPath
        {
            get
            {
                if (this.Path != "")
                    return this.Url + "?" + this.Path;
                return this.Url;
            }
            set
            {
                SetFullPath(value);
            }
        }

        public string ReverseFullPath
        {
            get
            {
                return this.Path + " @" + this.Url;
            }
            set
            {
                SetReverseFullPath(value);
            }
        }


        // �ѷ���ȫ·���ӹ�Ϊ����ȫ·����̬
        public static string GetRegularRecordPath(string strReverseRecordPath)
        {
            int nRet = strReverseRecordPath.IndexOf("@");
            if (nRet == -1)
                return strReverseRecordPath;
            return strReverseRecordPath.Substring(nRet + 1).Trim() + "?" + strReverseRecordPath.Substring(0, nRet).Trim();
        }

        // ������ȫ·����̬�ӹ�Ϊ����ȫ·����̬
        public static string GetReverseRecordPath(string strRegularRecordPath)
        {
            int nRet = strRegularRecordPath.IndexOf("?");
            if (nRet == -1)
                return strRegularRecordPath;
            return strRegularRecordPath.Substring(nRet + 1).Trim() + " @" + strRegularRecordPath.Substring(0, nRet).Trim();
        }
    }
}
