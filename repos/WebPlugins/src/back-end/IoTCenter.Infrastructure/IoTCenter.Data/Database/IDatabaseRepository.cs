// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.IotModels;
using System.Data;

namespace IoTCenter.Data.Database
{
    public interface IDatabaseRepository
    {
        bool IsOpen();
        DataTable SqlQueryDynamic(string sql, string sortField, bool isAsc, int pageSize, int pageIndex, out int total);
        void UpdateDataTable(DataTable dt);
        DataTable GetDataTable(string strSQL);
        object ExecuteScalar(string strSQL);
        void GetDataAdapter(string strSQL);
        int ExecuteSql(string strSQL);
        void ExecuteSql(string[] cmdtext);
        int ExecuteSqlWithRecordId(string strSql, string tableName);

        PaginationData GetDataByPagination(Pagination pagination);

        bool IsOpen(string connectionString);
    }
}
