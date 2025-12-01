// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.IotModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace IoTCenter.Data.Database
{
    public abstract class DatabaseRepository
    {
        public DbContext Db { get; set; }
        public DatabaseRepository()
        {
        }
        public void UpdateDataTable(DataTable dt)
        {
            using (var conn = Db.Database.GetDbConnection())
            {
                throw new NotImplementedException();
            }
        }
        public DataTable GetDataTable(string strSQL)
        {
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add("default");
            DataTable dt = dataSet.Tables[0];
            try
            {
                  var conn = Db.Database.GetDbConnection();
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    using (var cmd = Db.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        dataSet.EnforceConstraints = false;
                        using (var dataReader = cmd.ExecuteReader())
                        {
                            dt.Load(dataReader);
                        }
                    }
                    conn.Close();
            }
            catch (Exception ex)
            { 
                throw ex;
            }
            return dt;
        }
        public object ExecuteScalar(string strSQL)
        {
            object result = null;
            try
            {
                using (var conn = Db.Database.GetDbConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    using (var cmd = Db.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        result = cmd.ExecuteScalar();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            { 
                throw ex;
            }
            return result;
        }
        public void GetDataAdapter(string strSQL)
        {
            using (var conn = Db.Database.GetDbConnection())
            {
            }
        }
        public int ExecuteSql(string strSQL)
        {
            int rows = 0;
            using (var trans = Db.Database.BeginTransaction())
            {
                try
                {
                    rows = Db.Database.ExecuteSqlRaw(strSQL);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (Db.Database.GetDbConnection().State == ConnectionState.Open)
                        trans.Rollback(); 
                }

            }
            return rows;
        }
        public void ExecuteSql(string[] cmdtext)
        {
            using (var trans = Db.Database.BeginTransaction())
            {
                foreach (var strSql in cmdtext)
                {
                    Db.Database.ExecuteSqlRaw(strSql);
                }
                trans.Commit();
            }
        }

        public abstract DataTable SqlQueryDynamic(string sql, string sortField, bool isAsc, int pageSize, int pageIndex, out int total);
      
        public PaginationData GetDataByPagination(Pagination pagination)
        {
            int total;
            var list = this.SqlQueryDynamic(pagination.WhereCause, pagination.SortField, pagination.IsAsc, pagination.PageSize, pagination.PageIndex, out total);
            PaginationData paginationData = new PaginationData()
            {
                Data = list.ToJson(),
                Total = total,
            };
            return paginationData;
        }
    }
}
