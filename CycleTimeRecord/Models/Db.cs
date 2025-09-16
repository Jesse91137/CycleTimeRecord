using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace CycleTimeRecord.Models
{
    /// <summary>
    /// 資料庫連線
    /// </summary>
    public class Db
    {
        #region paramater method

        //開發測試
        //private static readonly String connStr = "server=localhost;database=CycleTimeRecord;uid=sa;pwd=A12345678;Connect Timeout = 480";
        //正式環境
        private static readonly String connStr = "server=192.168.7.39;database=CycleTimeRecord;uid=CT_Admin;pwd=CT12345678;Connect Timeout = 480";
        //ConfigurationManager.ConnectionStrings["conString"].ConnectionString;    
        //1. 執行insert/update/delete，回傳影響的資料列數

        /// <summary>
        /// 執行SQL語句，回傳影響的資料列數
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="cmdType">CommandType-SqlCommand 物件執行的是 SQL 語句</param>
        /// <param name="pms">SqlParameter[]參數陣列</param>
        /// <returns></returns>
        public static int ExecueNonQuery(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            // 創建SqlConnection物件，連接資料庫
            using (SqlConnection con = new SqlConnection(connStr))
            {
                //使用using語句，確保SqlCommand物件在使用完畢後自動釋放資源
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    //設置目前執行的是「存儲過程? 還是帶參數的sql 語句?」
                    cmd.CommandType = cmdType;

                    //如果參數陣列不為空，則將參數陣列添加到SqlCommand物件的Parameters屬性中
                    if (pms != null)
                    {
                        // 將參數陣列pms添加到命令物件cmd的參數集合中
                        cmd.Parameters.AddRange(pms);
                    }

                    con.Open();
                    //執行SQL語句，回傳影響的資料列數
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 執行SQL語句，返回SqlDataReader物件
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdType">CommandType-SqlCommand 物件執行的是 SQL 語句</param>
        /// <param name="pms">SqlParameter[]參數陣列</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            // 創建SqlConnection物件，連接資料庫
            SqlConnection con = new SqlConnection(connStr);
            // 使用using語句，確保SqlCommand物件在使用完畢後自動釋放資源
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                // 設置SqlCommand物件的CommandType屬性
                cmd.CommandType = cmdType;
                // 如果參數陣列不為空，則將參數陣列添加到SqlCommand物件的Parameters屬性中
                if (pms != null)
                {
                    // 將參數陣列pms添加到命令物件cmd的參數集合中
                    cmd.Parameters.AddRange(pms);
                }
                try
                {
                    // 打開資料庫連接
                    con.Open();
                    // 執行SQL語句，DataReader 物件關閉連線亦關閉，返回SqlDataReader物件
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch
                {
                    // 如果發生異常，關閉資料庫連接，釋放資源，並拋出異常
                    con.Close();
                    con.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// ExecuteDataTable方法，用於執行SQL語句並返回DataTable物件
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdType">CommandType-SqlCommand 物件執行的是 SQL 語句</param>
        /// <param name="pms">SqlParameter[]參數陣列</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            //創建DataTable對象
            DataTable dt = new DataTable();
            //使用SqlDataAdapter，它會建立Sql連接，所以不需要自己創建連接
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connStr))
            {
                //設置CommandType
                adapter.SelectCommand.CommandType = cmdType;
                //如果參數不為空，則添加參數
                if (pms != null)
                {
                    // 將參數陣列pms添加到命令物件cmd的參數集合中
                    adapter.SelectCommand.Parameters.AddRange(pms);

                }
                //將查詢結果填充DataTable
                adapter.Fill(dt);
                //返回DataTable
                return dt;
            }
        }
        /// <summary>
        /// ExecuteDataSet方法，用於執行SQL語句並返回DataSet物件
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdType">CommandType-SqlCommand 物件執行的是 SQL 語句</param>
        /// <param name="pms">SqlParameter[]參數陣列</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            //創建DataSet對象
            DataSet ds = new DataSet();
            //use SqlDataAdapter ,it will establish Sql connection.So ,it no need to create Connection by yourself.
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connStr))
            {
                //設置CommandType
                adapter.SelectCommand.CommandType = cmdType;
                //如果參數不為空，則添加參數
                if (pms != null)
                {
                    // 將參數陣列pms添加到命令物件cmd的參數集合中
                    adapter.SelectCommand.Parameters.AddRange(pms);
                }
                //將查詢結果填充DataSet
                adapter.Fill(ds);
                //返回DataSet
                return ds;
            }
        }
        /// <summary>
        /// ExecuteDataSetPmsList方法，用於執行SQL語句並返回DataSet物件
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdType">CommandType-SqlCommand 物件執行的是 SQL 語句</param>
        /// <param name="pms">SqlParameter[]參數陣列</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSetPmsList(string sql, CommandType cmdType, List<SqlParameter> pms)
        {
            DataSet ds = new DataSet();
            //use SqlDataAdapter ,it will establish Sql connection.So ,it no need to create Connection by yourself.
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connStr))
            {
                //設置CommandType
                adapter.SelectCommand.CommandType = cmdType;
                //如果參數不為空，則添加參數
                if (pms != null)
                {
                    // 將參數陣列pms添加到命令物件SelectCommand的參數集合中
                    adapter.SelectCommand.Parameters.AddRange(pms.ToArray<SqlParameter>());//paralist.ToArray<SqlParameter>()
                }
                //將查詢結果填充DataSet
                adapter.Fill(ds);
                return ds;
            }
        }
        /// <summary>
        /// ExecuteScalar方法，用於執行SQL語句並返回單個值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdType">CommandType-SqlCommand 物件執行的是 SQL 語句</param>
        /// <param name="pms">SqlParameter[]參數陣列</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReaderPmsList(string sql, CommandType cmdType, List<SqlParameter> pms)
        {
            //建立SQLConnection物件
            SqlConnection con = new SqlConnection(connStr);
            //建立SQLCommand物件
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                //設置CommandType
                cmd.CommandType = cmdType;
                if (pms != null)
                {
                    // 將參數陣列pms添加到命令物件SelectCommand的參數集合中
                    cmd.Parameters.AddRange(pms.ToArray<SqlParameter>());
                }
                try
                {
                    con.Open();
                    // 執行命令並返回SqlDataReader物件，關閉連接
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch
                {
                    // 如果發生異常，關閉資料庫連接，釋放資源，並拋出異常
                    con.Close();
                    con.Dispose();
                    throw;
                }
            }
        }
        #endregion

    }
}