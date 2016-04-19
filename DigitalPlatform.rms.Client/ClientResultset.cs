using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Threading;

using DigitalPlatform.IO;
using DigitalPlatform.Text;
using DigitalPlatform.Xml;

namespace DigitalPlatform.rms.Client
{

    /// <summary>
    /// ���������
    /// </summary>
    public class ClientResultsetCollection
    {
        public ReaderWriterLock m_lock = new ReaderWriterLock();
        public static int m_nLockTimeout = 5000;	// 5000=5��


        public Hashtable hashtable = new Hashtable();

        public ClientResultsetCollection()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public ClientResultset NewResultset(string strName)
        {
            // ��д��
            this.m_lock.AcquireWriterLock(m_nLockTimeout);
            try
            {
                ClientResultset resultset = (ClientResultset)hashtable[strName];

                if (resultset == null)
                {
                    resultset = new ClientResultset();
                    resultset.Name = strName;

                    hashtable.Add(strName, resultset);

                    resultset.File.Open(true);
                }

                return resultset;

            }
            finally
            {
                this.m_lock.ReleaseWriterLock();
            }
        }

        public ClientResultset GetResultset(string strName)
        {
            // �Ӷ���
            this.m_lock.AcquireReaderLock(m_nLockTimeout);
            try
            {

                ClientResultset resultset = (ClientResultset)hashtable[strName];

                return resultset;
            }
            finally
            {
                this.m_lock.ReleaseReaderLock();
            }
        }

        // ����: �ϲ������
        // parameters:
        //		strStyle	������ OR , AND , SUB
        //		sourceLeft	Դ,��߽����
        //		sourceRight	Դ,�ұ߽����
        //		targetLeft	Ŀ��,��߽����
        //		targetMiddle	Ŀ��,�м�����
        //		targetRight	Ŀ��,�ұ߽����
        //		strDebugInfo	������Ϣ
        // return:
        //		-1	ʧ��
        //		0	�ɹ�
        public static int Merge(string strStyle,
            ClientResultset sourceLeft,
            ClientResultset sourceRight,
            ClientResultset targetLeft,
            ClientResultset targetMiddle,
            ClientResultset targetRight,
            bool bOutputDebugInfo,
            out string strDebugInfo,
            out string strError)
        {
            strDebugInfo = "";
            strError = "";
            if (bOutputDebugInfo == true)
            {
                strDebugInfo += "strStyleֵ:" + strStyle + "\r\n";
                strDebugInfo += "sourceLeft�����:\r\n" + sourceLeft.Dump() + "\r\n";
                strDebugInfo += "sourceRight�����:\r\n" + sourceRight.Dump() + "\r\n";
            }

            if (String.Compare(strStyle, "OR", true) == 0)
            {
                if (targetLeft != null || targetRight != null)
                {
                    Exception ex = new Exception("��strStyle����ֵΪ\"OR\"ʱ��targetLeft������targetRight��Ч��ֵӦΪnull");
                    throw (ex);
                }
            }

            if (sourceLeft != null)
            {
                if (sourceLeft.File.HasIndexed == false)
                {
                    strError = "Ϊ����������ٶ�, sourceLeftӦ�ڵ���Merge()ǰȷ��indexed";
                    return -1;
                }
            }

            if (sourceRight != null)
            {
                if (sourceRight.File.HasIndexed == false)
                {
                    strError = "Ϊ����������ٶ�, sourceRightӦ�ڵ���Merge()ǰȷ��indexed";
                    return -1;
                }
            }

            ClientRecordItem dpRecordLeft;
            ClientRecordItem dpRecordRight;
            int i = 0;
            int j = 0;
            int ret;
            while (true)
            {
                dpRecordLeft = null;
                dpRecordRight = null;
                if (i >= sourceLeft.Count)
                {
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "i���ڵ���sourceLeft�ĸ�������i��Ϊ-1\r\n";
                    i = -1;
                }
                else if (i != -1)
                {
                    try
                    {
                        dpRecordLeft = (ClientRecordItem)sourceLeft.File[i];
                        if (bOutputDebugInfo == true)
                            strDebugInfo += "ȡ��sourceLeft�����е� " + Convert.ToString(i) + " ��Ԫ�أ�PathΪ '" + dpRecordLeft.Path + "' \r\n";
                    }
                    catch
                    {
                        Exception ex = new Exception("sourceLeftȡԪ���쳣: i=" + Convert.ToString(i) + "----Count=" + Convert.ToString(sourceLeft.Count) + "");
                        throw (ex);
                    }
                }
                if (j >= sourceRight.Count)
                {
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "j���ڵ���sourceRight�ĸ�������j��Ϊ-1\r\n";
                    j = -1;
                }
                else if (j != -1)
                {
                    try
                    {
                        dpRecordRight = (ClientRecordItem)sourceRight.File[j];
                        if (bOutputDebugInfo == true)
                            strDebugInfo += "ȡ��sourceRight�����е� " + Convert.ToString(j) + " ��Ԫ�أ�PathΪ '" + dpRecordRight.Path + "'\r\n";
                    }
                    catch
                    {
                        Exception ex = new Exception("sourceRightȡԪ���쳣: j=" + Convert.ToString(j) + "----Count=" + Convert.ToString(sourceLeft.Count) + sourceRight.GetHashCode() + "<br/>");
                        throw (ex);
                    }
                }
                if (i == -1 && j == -1)
                {
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "i,j������-1����\r\n";
                    break;
                }

                if (dpRecordLeft == null)
                {
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "dpRecordLeftΪnull����ret����1\r\n";
                    ret = 1;
                }
                else if (dpRecordRight == null)
                {
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "dpRecordRightΪnull����ret����-1\r\n";
                    ret = -1;
                }
                else
                {
                    ret = dpRecordLeft.CompareTo(dpRecordRight);  //MyCompareTo(oldOneKey); //��CompareTO
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "dpRecordLeft��dpRecordRight����Ϊnull���Ƚ�������¼�õ�ret����" + Convert.ToString(ret) + "\r\n";
                }


                if (String.Compare(strStyle, "OR", true) == 0
                    && targetMiddle != null)
                {
                    if (ret == 0)
                    {
                        targetMiddle.File.Add(dpRecordLeft);
                        i++;
                        j++;
                    }
                    else if (ret < 0)
                    {
                        targetMiddle.File.Add(dpRecordLeft);
                        i++;
                    }
                    else if (ret > 0)
                    {
                        targetMiddle.File.Add(dpRecordRight);
                        j++;
                    }
                    continue;
                }

                if (ret == 0 && targetMiddle != null)
                {
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "ret����0,�ӵ�targetMiddle����\r\n";
                    targetMiddle.File.Add(dpRecordLeft);
                    i++;
                    j++;
                }

                if (ret < 0)
                {
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "retС��0,�ӵ�targetLeft����\r\n";

                    if (targetLeft != null && dpRecordLeft != null)
                        targetLeft.File.Add(dpRecordLeft);
                    i++;
                }

                if (ret > 0)
                {
                    if (bOutputDebugInfo == true)
                        strDebugInfo += "ret����0,�ӵ�targetRight����\r\n";

                    if (targetRight != null && dpRecordRight != null)
                        targetRight.File.Add(dpRecordRight);

                    j++;
                }
            }
            return 0;
        }


    }


    /// <summary>
    /// ǰ�˽����
    /// </summary>
    public class ClientResultset
    {
        public string Name = "";

        public ClientResultsetFile File = new ClientResultsetFile();

        public ReaderWriterLock m_lock = new ReaderWriterLock();
        public static int m_nLockTimeout = 5000;	// 5000=5��

        public long Count
        {
            get
            {
                return File.Count;
            }
        }

        // ��������
        // return:
        //		-1	error
        //		���� ������
        public int Search(
            string strServerUrl,
            string strQueryXml,
            RmsChannelCollection Channels,
            string strLang,
            out string strError)
        {
            strError = "";

            string strMessage = "";

            // ��д��
            this.m_lock.AcquireWriterLock(m_nLockTimeout);
            try
            {

                this.File.Clear();	// ��ռ���


                //if (page.Response.IsClientConnected == false)	// �����ж�
                //	return -1;

                RmsChannel channel = Channels.GetChannel(strServerUrl);
                Debug.Assert(channel != null, "Channels.GetChannel �쳣");

                strMessage += "--- begin search ...\r\n";

                DateTime time = DateTime.Now;

                //if (page.Response.IsClientConnected == false)	// �����ж�
                //	return -1;

                long nRet = channel.DoSearch(strQueryXml,
                    "default",
                    "", // strOuputStyle
                    out strError);
                if (nRet == -1)
                {
                    strError = "����ʱ����: " + strError;
                    return -1;
                }


                TimeSpan delta = DateTime.Now - time;
                strMessage += "search end. time=" + delta.ToString() + "\r\n";

                if (nRet == 0)
                    return 0;	// not found

                long lTotalCount = nRet;

                //if (page.Response.IsClientConnected == false)	// �����ж�
                //	return -1;

                strMessage += "--- begin get search result ...\r\n";

                time = DateTime.Now;

                long lStart = 0;
                long lPerCount = Math.Min(lTotalCount, 1000);


                for (; ; )
                {
                    //if (page.Response.IsClientConnected == false)	// �����ж�
                    //	return -1;


                    List<string> aPath = null;
                    lPerCount = Math.Min((lTotalCount - lStart), 1000);

                    nRet = channel.DoGetSearchResult(
                        "default",
                        lStart,
                        lPerCount,
                        strLang,
                        null,	// stop,
                        out aPath,
                        out strError);
                    if (nRet == -1)
                    {
                        strError = "������ʱ����: " + strError;
                        return -1;
                    }

                    delta = DateTime.Now - time;
                    strMessage += "get search result end. time=" + delta.ToString() + "\r\n";

                    if (aPath.Count == 0)
                    {
                        strError = "������ʱ��ȡ�ļ������Ϊ��";
                        return -1;
                    }

                    //if (page.Response.IsClientConnected == false)	// �����ж�
                    //	return -1;

                    strMessage += "--- begin build storage ...\r\n";

                    time = DateTime.Now;

                    int i;
                    // �������ж������ж����У�ֻ��ʼ����m_strRecID����
                    for (i = 0; i < aPath.Count; i++)
                    {
                        ClientRecordItem item = new ClientRecordItem();
                        item.Path = (string)aPath[i];
                        this.File.Add(item);

                        if ((i % 100) == 0)
                        {
                            strMessage += "process " + Convert.ToString(i) + "\r\n";
                        }

                    }

                    delta = DateTime.Now - time;
                    strMessage += "build storage end. time=" + delta.ToString() + "\r\n";

                    lStart += aPath.Count;
                    if (lStart >= lTotalCount)
                        break;

                }


                return 1;

            }
            finally
            {
                this.m_lock.ReleaseWriterLock();
            }
        }


        //����:�г������е�������
        //����ֵ: ���ؼ��ϳ�Ա��ɵı���ַ���
        public string Dump()
        {
            string strText = "";

            foreach (ClientRecordItem eachRecord in this.File)
            {
                strText += eachRecord.Path + "\r\n";
            }
            return strText;
        }

    }


    public class ClientRecordItem : DigitalPlatform.IO.Item
    {
        int m_nLength = 0;
        string m_strPath = null;

        byte[] m_buffer = null;


        public string Path
        {
            get
            {
                return m_strPath;
            }
            set
            {
                m_strPath = value;

                m_buffer = Encoding.Unicode.GetBytes(m_strPath); ;

                this.Length = m_buffer.Length;
            }
        }

        public override int Length
        {
            get
            {
                return m_nLength;
            }
            set
            {
                m_nLength = value;
            }
        }

        public override void ReadData(Stream stream)
        {
            if (this.Length == 0)
                throw new Exception("length��δ��ʼ��");


            // ����Length��bytes������
            m_buffer = new byte[this.Length];
            stream.Read(m_buffer, 0, m_buffer.Length);

            // ��ԭ�ڴ����
            m_strPath = Encoding.Unicode.GetString(m_buffer);
        }


        public override void ReadCompareData(Stream stream)
        {
            if (this.Length == 0)
                throw new Exception("length��δ��ʼ��");


            ReadData(stream);
        }

        public override void WriteData(Stream stream)
        {
            if (m_strPath == null)
            {
                throw (new Exception("m_strPath��δ��ʼ��"));
            }

            if (m_buffer == null)
            {
                throw (new Exception("m_buffer��δ��ʼ��"));
            }


            // д��Length��bytes������
            stream.Write(m_buffer, 0, this.Length);
        }

        // ʵ��IComparable�ӿڵ�CompareTo()����,
        // ����ID�Ƚ���������Ĵ�С���Ա�����
        // ���Ҷ��뷽ʽ�Ƚ�
        // obj: An object to compare with this instance
        // ����ֵ A 32-bit signed integer that indicates the relative order of the comparands. The return value has these meanings:
        // Less than zero: This instance is less than obj.
        // Zero: This instance is equal to obj.
        // Greater than zero: This instance is greater than obj.
        // �쳣: ArgumentException,obj is not the same type as this instance.
        public override int CompareTo(object obj)
        {
            ClientRecordItem item = (ClientRecordItem)obj;

            return String.Compare(this.Path, item.Path, true);
        }
    }



    /// <summary>
    /// һ��������Ĵ�������洢�ṹ
    /// </summary>
    public class ClientResultsetFile : ItemFileBase
    {

        public ClientResultsetFile()
        {

        }


        public override DigitalPlatform.IO.Item NewItem()
        {
            return new ClientRecordItem();
        }

    }




}
