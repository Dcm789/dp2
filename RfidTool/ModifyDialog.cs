﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.IO;

using DigitalPlatform.GUI;
using DigitalPlatform.Core;
using DigitalPlatform.CommonControl;
using DigitalPlatform.CirculationClient;
using DigitalPlatform;
using DigitalPlatform.RFID;

namespace RfidTool
{
    public partial class ModifyDialog : Form
    {
        public event WriteCompleteEventHandler WriteComplete = null;

        public ModifyDialog()
        {
            InitializeComponent();

            _errorTable = new ErrorTable((s) =>
            {
                try
                {
                    this.Invoke((Action)(() =>
                    {

                    }));
                }
                catch (ObjectDisposedException)
                {

                }
            });
        }

        private void ModifyDialog_Load(object sender, EventArgs e)
        {

        }

        private void ModifyDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancel?.Cancel();
        }

        private void ModifyDialog_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        ActionInfo _action = null;

        // 开始修改
        private void toolStripButton_begin_Click(object sender, EventArgs e)
        {
            using (BeginModifyDialog dlg = new BeginModifyDialog())
            {
                GuiUtil.SetControlFont(dlg, this.Font);
                ClientInfo.MemoryState(dlg, "beginModifyDialog", "state");

                dlg.UiState = ClientInfo.Config.Get("BeginModifyDialog", "uiState", "");
                dlg.ShowDialog(this);
                ClientInfo.Config.Set("BeginModifyDialog", "uiState", dlg.UiState);

                if (dlg.DialogResult == DialogResult.Cancel)
                    return;

                _action = new ActionInfo
                {
                    FilterTU = dlg.FilterTU,
                    OI = dlg.OiString,
                    AOI = dlg.AoiString,
                    LinkUID = dlg.LinkUID,
                };
            }

            // TODO: 开始前用一个概要对话框显示确认一下本次批处理要进行的修改操作
            _cancel?.Dispose();
            _cancel = new CancellationTokenSource();
            BeginModify(_cancel.Token);
        }

        private void toolStripButton_stop_Click(object sender, EventArgs e)
        {
            _cancel?.Cancel();
        }

        // 导出 UID --> PII 对照关系
        private async void toolStripButton_exportUidPiiMap_Click(object sender, EventArgs e)
        {
            using (MessageBar bar = MessageBar.Create(this, "正在导出 UID --> PII 对照关系文件 ..."))
            {
                await ExportUidPiiMapFile();
            }
        }

        async Task ExportUidPiiMapFile()
        {
            string filename = "";
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Title = "请指定要保存的对照文件名";
                dlg.CreatePrompt = false;
                // dlg.FileName = "";
                dlg.InitialDirectory = Environment.CurrentDirectory;
                dlg.Filter = "对照文件 (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                filename = dlg.FileName;
            }

            await Task.Run(() =>
            {
                using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    lock (_uidTable.SyncRoot)
                    {
                        foreach (string key in _uidTable.Keys)
                        {
                            writer.WriteLine($"{key}\t{(string)_uidTable[key]}");
                        }
                    }
                }
            });
        }

        public string UiState
        {
            get
            {
                List<object> controls = new List<object>
                {
                    this.listView_tags,
                };
                return GuiState.GetUiState(controls);
            }
            set
            {
                List<object> controls = new List<object>
                {
                    this.listView_tags,
                };
                GuiState.SetUiState(controls, value);
            }
        }

        CancellationTokenSource _cancel = new CancellationTokenSource();

        void BeginModify(CancellationToken token)
        {
            this.toolStripButton_begin.Enabled = false;
            this.toolStripButton_stop.Enabled = true;
            ClearUidTable();
            _ = Task.Factory.StartNew(
                async () =>
                {
                    // 暂停基本循环
                    DataModel.PauseLoop();
                    try
                    {
                        while (token.IsCancellationRequested == false)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), token);

                            // 语音提示倒计时开始盘点
                            await SpeakCounter(token);

                            string readerNameList = "*";
                            var result = DataModel.ListTags(readerNameList, "");
                            if (result.Value == -1)
                                ShowMessageBox("inventory", result.ErrorInfo);
                            else
                            {
                                ShowMessageBox("inventory", null);
                            }

                            this.Invoke((Action)(() =>
                            {
                                FillTags(result.Results);
                                // TODO: 语音念出交叉的事项个数
                            }));

                            // testing
                            // await SpeakPauseCounter(token);

                            using (var cancel = CancellationTokenSource.CreateLinkedTokenSource(token))
                            {
                                var current_token = cancel.Token;
                                EnableSkipButton(true, cancel);
                                try
                                {
                                    while (current_token.IsCancellationRequested == false)
                                    {
                                        // result.Value:
                                        //      -1  表示遇到了严重出错，要停止循环调用本函数
                                        //      0   表示全部完成，没有遇到出错的情况
                                        //      >0  表示处理过程中有事项出错。后继需要调主循环调用本函数
                                        var process_result = await ProcessTags(result.Results, current_token);
                                        if (process_result.Value == 0 || process_result.Value == -1)
                                            break;

                                        if (current_token.IsCancellationRequested)
                                            break;

                                        await SpeakAdjust($"有 {process_result.Value} 项出错。请调整天线位置", token/*注意这里不能用 current_token(用了会在“跳过”时停止全部循环)*/);
                                    }
                                }
                                finally
                                {
                                    EnableSkipButton(false);
                                }
                            }

                            // 语音或音乐提示正在处理，不要移动天线
                            await Task.Delay(TimeSpan.FromSeconds(5), token);
                        }
                    }
                    catch (TaskCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                        ClientInfo.WriteErrorLog($"修改循环出现异常: {ExceptionUtil.GetDebugText(ex)}");
                        ShowMessageBox("inventory", $"修改循环因为异常已终止: {ex.Message}");
                    }
                    finally
                    {
                        FormClientInfo.Speak("停止修改", false, false);
                        
                        this.Invoke((Action)(() =>
                        {
                            // 把按钮状态复原到未启动状态
                            this.toolStripButton_begin.Enabled = true;
                            this.toolStripButton_stop.Enabled = false;
                        }));

                        // 恢复基本循环
                        DataModel.ContinueLoop();
                    }
                },
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        void EnableSkipButton(bool enable, CancellationTokenSource cancel = null)
        {
            this.Invoke((Action)(() =>
            {
                this.toolStripButton_nextScan.Tag = cancel;
                this.toolStripButton_nextScan.Enabled = enable;
            }));
        }

        // 语音提示倒计时
        async Task SpeakCounter(CancellationToken token)
        {
            for (int i = 5; i > 0; i--)
            {
                FormClientInfo.Speak($"{i}", false, true);
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
            FormClientInfo.Speak($"开始扫描", false, true);
        }

        // 语音提示间隙时间，方便拿走从读写器上标签
        async Task SpeakPauseCounter(CancellationToken token)
        {
            // 让上一句话说完
            await Task.Delay(TimeSpan.FromSeconds(3), token);

            FormClientInfo.Speak($"暂停开始", false, true);
            await Task.Delay(TimeSpan.FromSeconds(3), token);
            for (int i = 5; i > 0; i--)
            {
                FormClientInfo.Speak($"{i}", false, true);
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
            FormClientInfo.Speak($"暂停结束", false, true);
            await Task.Delay(TimeSpan.FromSeconds(3), token);
        }

        async Task SpeakAdjust(string text, CancellationToken token)
        {
            FormClientInfo.Speak(text, false, true);
            await Task.Delay(TimeSpan.FromSeconds(4), token);
        }

        #region Error Dialog

        ErrorTable _errorTable = null;

        FloatingErrorDialog _errorDialog = null;

        void CreateErrorDialog()
        {
            if (_errorDialog == null)
            {
                _errorDialog = new FloatingErrorDialog();

                _errorDialog.FormClosing += _errorDialog_FormClosing;

                /*
                GuiUtil.SetControlFont(_errorDialog, this.Font);
                ClientInfo.MemoryState(_errorDialog, "scanDialog", "state");
                _errorDialog.UiState = ClientInfo.Config.Get("scanDialog", "uiState", null);
                */
            }
        }
        private void _errorDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dialog = sender as Form;

            // 将关闭改为隐藏
            dialog.Visible = false;
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        void ShowMessageBox(string type, string text)
        {
            // 语音提示出错
            if (text != null)
                FormClientInfo.Speak(text, false, false);

            this.Invoke((Action)(() =>
            {
                CreateErrorDialog();
                if (text == null)
                    _errorDialog.Hide();
                else
                {
                    if (_errorDialog.Visible == false)
                    {
                        _errorDialog.Show(this);
                    }
                }

                _errorTable.SetError(type, text);
                _errorDialog.Message = _errorTable.GetError(false);
            }));
        }

        #endregion

        const int COLUMN_UID = 0;
        const int COLUMN_ERRORINFO = 1;
        const int COLUMN_PII = 2;
        const int COLUMN_TU = 3;
        const int COLUMN_OI = 4;
        const int COLUMN_AOI = 5;
        const int COLUMN_EAS = 6;
        const int COLUMN_READERNAME = 7;
        const int COLUMN_ANTENNA = 8;
        const int COLUMN_PROTOCOL = 9;

        class FillResult : NormalResult
        {
            public int CrossCount { get; set; }
        }

        // 初始填充 tags 列表
        FillResult FillTags(List<OneTag> tags)
        {
            this.listView_tags.Items.Clear();
            int cross_count = 0;
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (tag.Protocol != InventoryInfo.ISO15693)
                        continue;

                    ListViewItem item = ListViewUtil.FindItem(this.listView_tags, tag.UID, COLUMN_UID);
                    if (item == null)
                    {
                        item = new ListViewItem();
                        item.Tag = new ItemInfo { Tag = tag };
                        ListViewUtil.ChangeItemText(item, COLUMN_PII, "(尚未填充)");
                        this.listView_tags.Items.Add(item);
                    }
                    RefreshItem(item, tag);

                    var iteminfo = item.Tag as ItemInfo;
                    if (iteminfo.State == "cross")
                        cross_count++;
                }
            }

            return new FillResult { CrossCount = cross_count };
        }

        public class ItemInfo
        {
            public OneTag Tag { get; set; }
            public TagInfo TagInfo { get; set; }

            // 是否被过滤掉
            // public bool Disabled { get; set; }
            public string State { get; set; }   // disable/succeed

            public string ErrorInfo { get; set; }
        }

        // 刷新一个 ListViewItem 的所有列显示
        void RefreshItem(ListViewItem item, OneTag tag)
        {
            var iteminfo = item.Tag as ItemInfo;
            if (iteminfo == null)
                item.Tag = new ItemInfo { Tag = tag };

            if (IsProcessed(tag.UID))
            {
                iteminfo.State = "cross";   // 表示以前处理过这个标签
                ListViewUtil.ChangeItemText(item, COLUMN_ERRORINFO, "刚处理过");
                SetItemColor(item, "cross");
            }

            ListViewUtil.ChangeItemText(item, COLUMN_UID, tag.UID);
            ListViewUtil.ChangeItemText(item, COLUMN_ANTENNA, tag.AntennaID.ToString());
            ListViewUtil.ChangeItemText(item, COLUMN_READERNAME, tag.ReaderName);
            ListViewUtil.ChangeItemText(item, COLUMN_PROTOCOL, tag.Protocol);

            ListViewUtil.ChangeItemText(item, COLUMN_PII, "(尚未填充)");
        }

        class ProcessResult : NormalResult
        {
            // 出错的个数
            public int ErrorCount { get; set; }
            // 处理个数(包含出错的个数)
            public int ProcessCount { get; set; }
            // 被过滤掉(不符合 TU)的个数
            public int FilteredCount { get; set; }
        }

        // 之前处理过的事项的集合。用于计算本次和以前处理过的交叉部分
        // TODO: 这里面存储所有的已经处理过的，还是仅仅只保留前一轮的？
        // UID --> ItemInfo
        Hashtable _processedTable = new Hashtable();

        // 先前是否已经处理过？
        bool IsProcessed(string uid)
        {
            lock (_processedTable.SyncRoot)
            {
                return _processedTable.ContainsKey(uid);
            }
        }

        void AddToProcessed(string uid, ItemInfo iteminfo)
        {
            lock (_processedTable.SyncRoot)
            {
                _processedTable[uid] = iteminfo;
            }
        }

        // 处理每一个标签的修改动作
        // result.Value:
        //      -1  表示遇到了严重出错，要停止循环调用本函数
        //      0   表示全部完成，没有遇到出错的情况
        //      >0  表示处理过程中有事项出错。后继需要调主循环调用本函数
        async Task<ProcessResult> ProcessTags(List<OneTag> tags,
            CancellationToken token)
        {
            if (tags == null)
                return new ProcessResult();

            int process_count = 0;
            int error_count = 0;
            int filtered_count = 0;
            foreach (var tag in tags)
            {
                if (token.IsCancellationRequested)
                    return new ProcessResult
                    {
                        Value = -1,
                        ErrorInfo = "中断",
                        ErrorCode = "cancel"
                    };

                if (tag.Protocol != InventoryInfo.ISO15693)
                    continue;

                ListViewItem item = (ListViewItem)this.Invoke((Func<ListViewItem>)(() =>
                {
                    return ListViewUtil.FindItem(this.listView_tags, tag.UID, COLUMN_UID);
                }));

                Debug.Assert(item != null);

                var iteminfo = item.Tag as ItemInfo;

                if (iteminfo.State == "disable")
                {
                    filtered_count++;
                    continue;
                }

                // 跳过已经成功的项目
                if (iteminfo.State == "succeed")
                    continue;

                if (iteminfo.TagInfo == null)
                {
                    // 第一步，获得标签详细信息
                    InventoryInfo info = new InventoryInfo
                    {
                        Protocol = tag.Protocol,
                        UID = tag.UID,
                        AntennaID = tag.AntennaID
                    };
                    var get_result = DataModel.GetTagInfo(tag.ReaderName, info, "");
                    if (get_result.Value == -1)
                    {
                        SetErrorInfo(item, get_result.ErrorInfo);
                        error_count++;
                        continue;
                    }

                    iteminfo.TagInfo = get_result.TagInfo;
                }

                // 第二步，刷新 PII 等栏目
                this.Invoke((Action)(() =>
                {
                    RefreshItem(item);
                }));

                if (iteminfo.State == "disable")
                {
                    filtered_count++;
                    continue;
                }

                if (iteminfo.State == "succeed")
                    continue;

                // 跳过以前已经处理过的项目
                if (iteminfo.State == "cross")
                    continue;

                // 第三步，执行修改动作
                var action_result = DoAction(item);
                if (action_result.Value == -1)
                    error_count++;

                process_count++;
            }

            // 返回 0 表示全部完成，没有遇到出错的情况
            return new ProcessResult
            {
                Value = error_count,
                ErrorCount = error_count,
                ProcessCount = process_count
            };
        }

        // 执行修改动作
        NormalResult DoAction(ListViewItem item)
        {
            var iteminfo = item.Tag as ItemInfo;

            string pii = "";
            string tou = "";
            string oi = "";
            string aoi = "";
            try
            {
                var taginfo = iteminfo.TagInfo;
                if (taginfo == null)
                {
                    // TODO: 显示出错信息
                    return new NormalResult
                    {
                        Value = -1,
                        ErrorInfo = "taginfo == null"
                    };
                }

                LogicChip chip = null;

                if (taginfo.Protocol == InventoryInfo.ISO18000P6C)
                {
                    throw new NotImplementedException("暂不支持");
                }
                else
                {
                    // *** ISO15693 HF
                    if (taginfo.Bytes != null)
                    {
                        // Exception:
                        //      可能会抛出异常 ArgumentException TagDataException
                        chip = LogicChip.From(taginfo.Bytes,
            (int)taginfo.BlockSize,
            "");
                        pii = ScanDialog.GetPIICaption(chip.FindElement(ElementOID.PII)?.Text);
                    }
                    else
                    {
                        return new NormalResult
                        {
                            Value = -1,
                            ErrorInfo = $"标签 {taginfo.UID} 没有数据体"
                        };
                    }
                }

                tou = chip?.FindElement(ElementOID.TypeOfUsage)?.Text;

                if (string.IsNullOrEmpty(oi))
                {
                    oi = chip?.FindElement(ElementOID.OI)?.Text;
                    aoi = chip?.FindElement(ElementOID.AOI)?.Text;
                }

                bool changed = false;

                if (_action.OI != null && oi != _action.OI)
                {
                    chip?.RemoveElement(ElementOID.AOI);
                    chip?.SetElement(ElementOID.OI, _action.OI);
                    changed = true;
                }
                if (_action.AOI != null && aoi != _action.AOI)
                {
                    chip?.RemoveElement(ElementOID.OI);
                    chip?.SetElement(ElementOID.AOI, _action.AOI);
                    changed = true;
                }
                if (_action.LinkUID)
                {
                    AddUidEntry(iteminfo.Tag.UID, pii);
                }

                // 写回标签
                if (changed)
                {
                    var tag = iteminfo.Tag;
                    var new_tag_info = GetTagInfo(taginfo, chip);
                    // 写入标签
                    var write_result = DataModel.WriteTagInfo(tag.ReaderName,
                        taginfo,
                        new_tag_info);
                    if (write_result.Value == -1)
                    {
                        string error = $"写入标签出错: {write_result.ErrorInfo}";
                        SetErrorInfo(item, error);
                        return new NormalResult
                        {
                            Value = -1,
                            ErrorInfo = error
                        };
                    }

                    iteminfo.State = "succeed";
                    this.Invoke((Action)(() =>
                    {
                        SetItemColor(item, "changed");
                    }));

                    AddToProcessed(iteminfo.Tag.UID, iteminfo);
                }
                else
                {
                    // 没有发生实质性修改，显示为特定状态

                    iteminfo.State = "succeed";
                    this.Invoke((Action)(() =>
                    {
                        SetItemColor(item, "notchanged");
                        ListViewUtil.ChangeItemText(item, COLUMN_ERRORINFO, "没有发生修改");
                    }));

                    AddToProcessed(iteminfo.Tag.UID, iteminfo);
                }

                return new NormalResult();
            }
            catch (Exception ex)
            {
                string error = "exception:" + ex.Message;
                SetErrorInfo(item, error);
                return new NormalResult
                {
                    Value = -1,
                    ErrorInfo = error
                };
            }

        }

        public static TagInfo GetTagInfo(TagInfo existing,
LogicChip chip/*,
bool eas*/)
        {
            if (existing.Protocol == InventoryInfo.ISO15693)
            {
                TagInfo new_tag_info = existing.Clone();
                new_tag_info.Bytes = chip.GetBytes(
                    (int)(new_tag_info.MaxBlockCount * new_tag_info.BlockSize),
                    (int)new_tag_info.BlockSize,
                    LogicChip.GetBytesStyle.None,
                    out string block_map);
                new_tag_info.LockStatus = block_map;

                // new_tag_info.DSFID = LogicChip.DefaultDSFID;  // 图书

                // new_tag_info.SetEas(eas);
                return new_tag_info;
            }

            throw new ArgumentException($"目前暂不支持 {existing.Protocol} 协议标签的写入操作");
        }


        Hashtable _uidTable = new Hashtable();

        void AddUidEntry(string uid, string pii)
        {
            lock (_uidTable.SyncRoot)
            {
                _uidTable[uid] = pii;
            }
        }

        void ClearUidTable()
        {
            lock (_uidTable.SyncRoot)
            {
                _uidTable.Clear();
            }
        }

        // 刷新一个 ListViewItem 的所有列显示
        void RefreshItem(ListViewItem item)
        {
            string pii = "(尚未填充)";
            string tou = "";
            string eas = "";
            string oi = "";
            string aoi = "";

            var iteminfo = item.Tag as ItemInfo;

            try
            {
                var taginfo = iteminfo.TagInfo;
                if (taginfo != null)
                {
                    LogicChip chip = null;

                    if (taginfo.Protocol == InventoryInfo.ISO18000P6C)
                    {
                        throw new NotImplementedException("暂不支持");
                    }
                    else
                    {
                        // *** ISO15693 HF
                        if (taginfo.Bytes != null)
                        {
                            // Exception:
                            //      可能会抛出异常 ArgumentException TagDataException
                            chip = LogicChip.From(taginfo.Bytes,
                (int)taginfo.BlockSize,
                "");
                            pii = ScanDialog.GetPIICaption(chip.FindElement(ElementOID.PII)?.Text);
                        }
                    }

                    tou = chip?.FindElement(ElementOID.TypeOfUsage)?.Text;
                    eas = taginfo.EAS ? "On" : "Off";

                    if (string.IsNullOrEmpty(oi))
                    {
                        oi = chip?.FindElement(ElementOID.OI)?.Text;
                        aoi = chip?.FindElement(ElementOID.AOI)?.Text;
                    }
                }

                ListViewUtil.ChangeItemText(item, COLUMN_PII, pii);

                ListViewUtil.ChangeItemText(item, COLUMN_TU, tou);
                ListViewUtil.ChangeItemText(item, COLUMN_EAS, eas);
                ListViewUtil.ChangeItemText(item, COLUMN_OI, oi);
                ListViewUtil.ChangeItemText(item, COLUMN_AOI, aoi);


                // 过滤 TU
                if (FilterTU(_action.FilterTU, tou))
                {
                    // iteminfo.Disabled = false;
                }
                else
                {
                    // 被过滤掉的
                    iteminfo.State = "disable";
                    iteminfo.ErrorInfo = "被过滤掉";
                    ListViewUtil.ChangeItemText(item, COLUMN_ERRORINFO, iteminfo.ErrorInfo);
                    SetItemColor(item, "disable");
                }
            }
            catch (Exception ex)
            {
                SetErrorInfo(item, "exception:" + ex.Message);
            }
        }

        /*
图书
读者证
层架标
所有类别
* */
        static bool FilterTU(string filter, string tu)
        {
            if (filter == null || filter == "所有类别")
                return true;

            if (tu == filter)
                return true;
            // 10 图书; 80 读者证; 30 层架标
            if (tu.StartsWith("1") && filter == "图书")
                return true;
            if (tu.StartsWith("8") && filter == "读者证")
                return true;
            if (tu == "30" && filter == "层架标")
                return true;

            return false;
        }

        // 设置指定行的错误信息栏，并改变背景色
        void SetErrorInfo(ListViewItem item, string error)
        {
            var iteminfo = item.Tag as ItemInfo;
            if (iteminfo != null)
                iteminfo.ErrorInfo = error;

            this.Invoke((Action)(() =>
            {
                ListViewUtil.ChangeItemText(item, COLUMN_ERRORINFO, error);
                SetItemColor(item, "error");
            }));
        }

        static void SetItemColor(ListViewItem item, string state)
        {
            if (state == "normal")
            {
                if (item.ListView != null)
                {
                    item.BackColor = item.ListView.BackColor;
                    item.ForeColor = item.ListView.ForeColor;
                }
                return;
            }

            // 处理过了，并且发生了实质性修改
            if (state == "changed")
            {
                item.BackColor = Color.DarkGreen;
                item.ForeColor = Color.White;
                return;
            }

            // 处理过了但没有发生实质性修改
            if (state == "notchanged")
            {
                item.BackColor = Color.Black;
                item.ForeColor = Color.White;
                return;
            }

            if (state == "error")
            {
                item.BackColor = Color.DarkRed;
                item.ForeColor = Color.White;
                return;
            }

            if (state == "disable")
            {
                item.BackColor = Color.DarkGray;
                item.ForeColor = Color.White;
                return;
            }

            if (state == "cross")
            {
                item.BackColor = Color.White;
                item.ForeColor = Color.DarkGray;
                return;
            }

            throw new ArgumentException($"未知的 state '{state}'");
        }

        // 清空列表
        private void toolStripButton_clearList_Click(object sender, EventArgs e)
        {
            this.listView_tags.Items.Clear();
        }

        // 跳过本次扫描内循环(即语音不断提示“调整天线位置”的循环)
        private void toolStripButton_nextScan_Click(object sender, EventArgs e)
        {
            var button = sender as ToolStripButton;
            var cancel = button.Tag as CancellationTokenSource;
            cancel?.Cancel();
        }
    }

    // 修改动作
    class ActionInfo
    {
        // Type of usage 过滤
        public string FilterTU { get; set; }

        public string OI { get; set; }
        public string AOI { get; set; }

        public bool LinkUID { get; set; }
    }
}
