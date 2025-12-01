// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;

namespace GWModbusStandard.STD;

public partial class ClientSession
{
    #region 批量读取数据
    private const int PageSize = 1; // 每页50个元素
    private int offlineCount = 0;
    private async Task ReadAllNodesValues()
    {
        try
        {
            Session.EnsureConnected();
            offlineCount = 0;
            SetOnline();
        }
        catch
        {
            offlineCount++;

            //设置10次连接不了就离线
            if (offlineCount > 5)
            {
                offlineCount = 0;
                SetOffline();
            }
        }
        if (Status == false)
        {
            await Task.CompletedTask;
            return;
        }

        int totalItems = AddressInputs.Values.Count;
        if (totalItems == 0)
        {
            await Task.CompletedTask;
            return;
        }

        int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
        for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
        {
            var pagedResults = AddressInputs.Values
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            try
            {
                var resultList = await Session.BatchRead(pagedResults);
                if (!resultList.IsSucceed)
                {
                    DataCenter.WriteLogFile($"读取数据出现失败，连接地址:{Config.ServerUrl}，Page：{pageNumber}，Err：{resultList.Err}，ErrCode：{resultList.ErrCode}，Exception：{resultList.Exception}");
                    continue;
                }

                foreach (var item in resultList.Value)
                {
                    CurrentValues[item.DisplayNameKey] = item.Value;
                }
            }
            catch (Exception ex)
            {
                DataCenter.WriteLogFile($"分页读取异常，连接地址:{Config.ServerUrl}，Page：{pageNumber}，,操作命令： {string.Join('|', pagedResults.Select(m => m.NodeStr))}，Exception：{ex}");
            }
        }
    }
    #endregion
}
