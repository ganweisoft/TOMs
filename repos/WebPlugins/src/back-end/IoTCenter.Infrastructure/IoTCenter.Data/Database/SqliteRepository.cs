// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.Common;

namespace IoTCenter.Data.Database
{
    public class SqliteRepository : DatabaseRepository, IDatabaseRepository
    {
        private IServiceScopeFactory serviceScopeFactory;

        public SqliteRepository(IServiceScopeFactory serviceScope)
        {
            serviceScopeFactory = serviceScope;
            Db = serviceScopeFactory.CreateScope().ServiceProvider.GetService<GWDbContext>();
        }
        public override DataTable SqlQueryDynamic(string sql, string sortField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("请输入查询语句");
            }
            sql = sql.Replace("\t", " ").Replace("\r  ", " ").Replace("\n", " ").Replace("\r\r", " ");

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add("default");
            DataTable dataTable = dataSet.Tables[0];
            total = Convert.ToInt32(ExecuteScalar($" select count(*) from ({sql})"));
            using (var cmd = Db.Database.GetDbConnection().CreateCommand())
            {
                string orderCause = "";
                if (!string.IsNullOrEmpty(sortField))
                    if (isAsc)
                    {
                        orderCause = $" order by {sortField} asc";
                    }
                    else
                    {
                        orderCause = $" order by {sortField} desc";
                    }
                sql = $"select * from ({sql}) {orderCause} limit {pageSize} offset  {pageSize * (pageIndex - 1)} ";
                cmd.CommandText = sql;

                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                using (var dataReader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    dataSet.EnforceConstraints = false;
                    dataTable.Load(dataReader);
                }
            }
            return dataTable;
        }
        public int ExecuteSqlWithRecordId(string strSql, string tableName)
        {
            int rowId = 0;
            var conn = Db.Database.GetDbConnection();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    using (var cmd = Db.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = $"{strSql};select last_insert_rowid() from {tableName}";
                        cmd.Transaction = trans;
                        DbDataReader reader = cmd.ExecuteReader();
                        trans.Commit();
                        if (reader.Read())
                        {
                            rowId = int.Parse(reader[0].ToString());
                            reader.Close();
                        }
                        else
                            rowId = 0;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
                finally
                {
                    conn.Close();
                }

                return rowId;
            }
        }

        public bool IsOpen()
        {
            bool isOpen = false;
            var conn = Db.Database.GetDbConnection();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                isOpen = true;
                conn.Close();
                return true;
            }
            else
                return false;
        }

        public bool IsOpen(string str)
        {
            throw new NotImplementedException();
        }
    }
}
