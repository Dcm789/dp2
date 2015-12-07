using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using DigitalPlatform.IO;

namespace DigitalPlatform.MarcDom
{
    public class FilterCollection
    {
        Hashtable table = new Hashtable();

        public bool IgnoreCase = true;

        public ReaderWriterLock m_lock = new ReaderWriterLock();
        public static int m_nLockTimeout = 5000;	// 5000=5��

        public int Max = 100;   // ÿ��List�ж���������

        public FilterDocument GetFilter(string strName)
        {
            if (IgnoreCase == true)
                strName = strName.ToLower();

            FilterList filterlist = null;

            this.m_lock.AcquireWriterLock(m_nLockTimeout);
            try
            {

                filterlist = (FilterList)table[strName];


                if (filterlist == null)
                {
                    filterlist = new FilterList();
                    filterlist.Container = this;
                    table[strName] = filterlist;
                }

            }
            finally
            {
                this.m_lock.ReleaseWriterLock();
            }

            FilterDocument filter = filterlist.GetFilter();
            return filter;
        }

        // 2007/1/8
        public void Clear()
        {
            this.m_lock.AcquireWriterLock(m_nLockTimeout);
            try
            {
                this.table.Clear();
            }
            finally
            {
                this.m_lock.ReleaseWriterLock();
            }
        }


        // �Ӽ���������ض����ֵ�filterlist
        public void ClearFilter(string strName)
        {
            if (IgnoreCase == true)
                strName = strName.ToLower();

            FilterList filterlist = null;

            this.m_lock.AcquireWriterLock(m_nLockTimeout);
            try
            {

                filterlist = (FilterList)table[strName];

                if (filterlist != null)
                {
                    table.Remove(strName);
                }
            }
            finally
            {
                this.m_lock.ReleaseWriterLock();
            }
        }

        public void SetFilter(string strName,
            FilterDocument filter)
        {
            if (IgnoreCase == true)
                strName = strName.ToLower();
            FilterList filterlist = null;

            this.m_lock.AcquireWriterLock(m_nLockTimeout);
            try
            {
                filterlist = (FilterList)table[strName];

                if (filterlist == null)
                {
                    filterlist = new FilterList();
                    filterlist.Container = this;
                    table[strName] = filterlist;
                }

            }
            finally
            {
                this.m_lock.ReleaseWriterLock();
            }

            filterlist.SetFilter(filter);
        }

        public int Count
        {
            get 
            {
                return this.table.Count;
            }
        }

        public string Dump()
        {
            string strResult = "";

            strResult += "�������й���'" + Convert.ToString(this.table.Count)+ "'��FilterList����.\r\n";

            foreach (DictionaryEntry item in table)
            {
                strResult += "  " + item.Key + "\r\n";

                FilterList list = (FilterList)item.Value;

                strResult += "    " + list.Dump();
            }

            return strResult;
        }
    }

    public class FilterList
    {
        List<FilterHolder> list = new List<FilterHolder>();

        public ReaderWriterLock m_lock = new ReaderWriterLock();
        public static int m_nLockTimeout = 5000;	// 5000=5��

        public FilterCollection Container = null;

        public FilterDocument GetFilter()
        {
            this.m_lock.AcquireReaderLock(m_nLockTimeout);
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    FilterHolder item = this.list[i];

                    if (Interlocked.Increment(ref item.UsedCount) == 1)
                    {
                        return item.FilterDocument;
                    }

                    Interlocked.Decrement(ref item.UsedCount);
                }

                return null;
            }
            finally
            {
                this.m_lock.ReleaseReaderLock();
            }
        }

        public bool SetFilter(FilterDocument filter)
        {
            // string strMessage = "";

            this.m_lock.AcquireReaderLock(m_nLockTimeout);
            try
            {

                for (int i = 0; i < list.Count; i++)
                {
                    FilterHolder item = this.list[i];

                    if (item.FilterDocument == filter)
                    {
                        int nValue = Interlocked.Decrement(ref item.UsedCount);
                        if (nValue < 0)
                        {
                            throw new Exception("���غ�UsedCountС��0, ����");
                        }

                        return true;
                    }
                }
            }
            finally
            {
                this.m_lock.ReleaseReaderLock();
            }

            return NewFilter(filter);
        }

        public void ReturnFilter(FilterDocument filter)
        {
            this.m_lock.AcquireReaderLock(m_nLockTimeout);
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    FilterHolder item = this.list[i];

                    if (item.FilterDocument == filter)
                    {
                        int nValue = Interlocked.Decrement(ref item.UsedCount);
                        if (nValue < 0)
                        {
                            throw new Exception("���غ�UsedCountС��0, ����");
                        } 
                        return;
                    }
                }
            }
            finally
            {
                this.m_lock.ReleaseReaderLock();
            }

            throw new Exception("���صĶ�����������û���ҵ�");
        }

        // return:
        //      true    �Ѿ�����
        //      false   δ����
        public bool NewFilter(FilterDocument filter)
        {
            this.m_lock.AcquireWriterLock(m_nLockTimeout);
            try
            {


                if (this.list.Count >= this.Container.Max)
                    return false;


                FilterHolder item = new FilterHolder();
                item.FilterDocument = filter;

                this.list.Add(item);

                return true;
            }
            finally
            {
                this.m_lock.ReleaseWriterLock();
            }
        }

        public string Dump()
        {
            string strResult = "";

            strResult += "����'" + Convert.ToString(this.list.Count)+ "'��FilterDocumnet����.\r\n";

            return strResult;
        }
    }

    public class FilterHolder
    {
        public FilterDocument FilterDocument = null;
        public int UsedCount = 0;
    }
}
