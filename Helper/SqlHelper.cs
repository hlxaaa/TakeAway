using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Common.Helper;
using System.Text;

namespace takeAwayWebApi.Helper
{
    public class SqlHelperHere
    {
        public static string connStr = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="str"></param>
        public static void ExcuteNon(string str)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand com = new SqlCommand(str, conn);
                com.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 防注入
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dict"></param>
        public static void ExcuteNon(string str, Dictionary<string, string> dict)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand sqlCom = new SqlCommand(str, conn);
                foreach (var item in dict)
                {
                    sqlCom.Parameters.AddWithValue(item.Key, item.Value);
                }
                conn.Open();
                sqlCom.Connection = conn;
                sqlCom.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 防止sql注入，要传com的参数
        /// </summary>
        /// <param name="sqlCom"></param>
        public static void ExcuteNon(SqlCommand sqlCom)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                sqlCom.Connection = conn;
                sqlCom.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 执行sql语句，返回结果
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public static string ExecuteScalar(string sqlStr)
        {
            string r = "";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand sqlCom = new SqlCommand(sqlStr, conn);
                var t = sqlCom.ExecuteScalar();
                if (t != null)
                    r = t.ToString();
                else
                    r = null;
                conn.Close();
            }
            return r;
        }

        /// <summary>
        /// 防注入
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ExecuteScalar(string sqlStr, Dictionary<string, string> dict)
        {
            string r = "";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand sqlCom = new SqlCommand(sqlStr, conn);
                foreach (var item in dict)
                {
                    sqlCom.Parameters.AddWithValue(item.Key, item.Value);
                }
                var t = sqlCom.ExecuteScalar();
                if (t != null)
                    r = t.ToString();
                else
                    r = null;
                conn.Close();
            }
            return r;
        }

        /// <summary>
        /// 防止sql注入，要传com的参数
        /// </summary>
        /// <param name="sqlCom"></param>
        /// <returns></returns>
        public static string ExecuteScalar(SqlCommand sqlCom)
        {
            string r = "";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                sqlCom.Connection = conn;
                var t = sqlCom.ExecuteScalar();
                if (t != null)
                    r = t.ToString();
                else
                    r = null;
                conn.Close();
            }
            return r;
        }

        /// <summary>
        /// 执行sql得DataTable
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public static DataTable ExecuteGetDt(string sqlStr)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(sqlStr, conn);
                da.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// 执行sql得DataTable,防注入
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public static DataTable ExecuteGetDt(string sqlStr, Dictionary<string, string> dict)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand sqlCom = new SqlCommand(sqlStr, conn);
                foreach (var item in dict)
                {
                    sqlCom.Parameters.AddWithValue(item.Key, item.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(sqlCom);
                da.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// 执行sql得List,防注入
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public static List<T> ExecuteGetList<T>(string sqlStr, Dictionary<string, string> dict) where T : class, new()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand sqlCom = new SqlCommand(sqlStr, conn);
                foreach (var item in dict)
                {
                    sqlCom.Parameters.AddWithValue(item.Key, item.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(sqlCom);
                da.Fill(dt);
            }
            return dt.ConvertToList<T>();
        }

        /// <summary>
        /// 执行sql得List
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public static List<T> ExecuteGetList<T>(string sqlStr) where T : class, new()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand sqlCom = new SqlCommand(sqlStr, conn);
                SqlDataAdapter da = new SqlDataAdapter(sqlCom);
                da.Fill(dt);
            }
            return dt.ConvertToList<T>();
        }

        /// <summary>
        /// 事务执行sql
        /// </summary>
        /// <param name="sqls">几组sql一起</param>
        /// <param name="transactionName">事务名称</param>
        /// <param name="dicts">几组参数一起，要与sqls对应</param>
        /// <returns></returns>
        public static bool ExecuteInTransaction(List<string> sqls, string transactionName, List<Dictionary<string, string>> dicts)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction(transactionName);
                try
                {
                    for (int i = 0; i < sqls.Count; i++)
                    {
                        SqlCommand com = new SqlCommand();
                        com.Connection = conn;
                        com.Transaction = transaction;
                        com.CommandText = sqls[i];
                        foreach (var item in dicts[i])
                        {
                            com.Parameters.AddWithValue(item.Key, item.Value);
                        }
                        com.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// 条件查询类似orderTryJoin这种一对多（当然也支持一对一）的视图。排序，分页    select自己写
        /// </summary>
        /// <returns></returns>
        public static List<T> GetViewPaging<T>(string tableName, string select, string condition, int index, int size, string orderStr, Dictionary<string, string> dict) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(select + " where " + condition);

                int x = (index - 1) * size;
                string str = "select o.* from " + tableName + " as o,( ";
                str += "select top " + size + " * from (" + sb.ToString() + ") r where id not in (select top " + x + " id from (" + sb.ToString() + ") r " + orderStr + ") " + orderStr + "";
                str += " ) as r where r.id=o.id";
                SqlCommand sqlCom = new SqlCommand(str, conn);
                foreach (var item in dict)
                {
                    sqlCom.Parameters.AddWithValue(item.Key, item.Value);
                }
                SqlDataAdapter da = new SqlDataAdapter(sqlCom);
                DataTable table = new DataTable();
                da.Fill(table);
                return table.ConvertToList<T>();
            }
        }

        /// <summary>
        /// 条件查询类似orderTryJoin这种一对多（当然也支持一对一）的视图，分页  ,id倒序
        /// </summary>
        /// <returns></returns>
        public static List<T> GetViewPaging<T>(string tableName, string condition, int index, int size, Dictionary<string, string> dict) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select DISTINCT id from " + tableName + " where " + condition);

                int x = (index - 1) * size;
                string str = "select o.* from " + tableName + " as o,( ";
                str += "select top " + size + " * from (" + sb.ToString() + ") r where id not in (select top " + x + " id from (" + sb.ToString() + ") r order by id desc) order by id desc";
                str += " ) as r where r.id=o.id";
                SqlCommand sqlCom = new SqlCommand(str, conn);
                foreach (var item in dict)
                {
                    sqlCom.Parameters.AddWithValue(item.Key, item.Value);
                }
                SqlDataAdapter da = new SqlDataAdapter(sqlCom);
                DataTable table = new DataTable();
                da.Fill(table);
                return table.ConvertToList<T>();
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="ids"></param>
        public static void DeleteByIds(string tableName, int[] ids)
        {
            var idsStr = StringHelperHere.Instance.ArrJoin(ids);
            string str = "update " + tableName + " set isdeleted=1 where id in (" + idsStr + ")";
            ExcuteNon(str);
        }

    }
}