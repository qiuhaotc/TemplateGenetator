using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;

namespace TemplateGenerator.Util
{
    internal class ParameterHelp
    {
        /// <summary>
        /// 根据Model生成SQL新增语句
        /// </summary>
        /// <param name="mouob"></param>
        /// <returns></returns>
        public static string CreateAddStr(object mouob)
        {
            PropertyInfo[] Fields = mouob.GetType().GetProperties();
            foreach (PropertyInfo PI in Fields)
            {
               // "@" + PI.Name;
            }
            return "";
        }

        /// <summary>
        /// 根据输入的数组生成SqlParameter，用于少量参数
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="val">字段值</param>
        /// <returns>SqlParameter[]</returns>
        public static SqlParameter[] ParmsByWrite(string[] name, object[] val)
        {
            int Len = name.Length;
            SqlParameter[] parms = new SqlParameter[Len];
            for (int i = 0; i < Len; i++)
            {
                parms[i] = new SqlParameter("@" + name[i], val[i]);
            }
            return parms;
        }

        /// <summary>
        /// 根据实体类生成SqlParameter集合,用于大量参数
        /// </summary>
        /// <param name="mouob">实体类</param>
        /// <returns>SqlParameter[]</returns>
        public static SqlParameter[] ParmsByModule(object mouob)
        {
            PropertyInfo[] Fields = mouob.GetType().GetProperties();
            SqlParameter[] parms = new SqlParameter[Fields.Length];
            int i = 0;
            foreach (PropertyInfo PI in Fields)
            {
                //获取实体类属性值
                object val = PI.GetValue(mouob, null);
                parms[i] = new SqlParameter();
                parms[i].ParameterName = "@" + PI.Name;
                if (PI.PropertyType == typeof(System.Int32))
                {
                    parms[i].SqlDbType = SqlDbType.Int;
                    if (val == null) { val = 0; }
                }
                else if (PI.PropertyType == typeof(System.String))
                {
                    parms[i].SqlDbType = SqlDbType.VarChar;
                    if (val == null) { val = ""; }
                    else
                    {
                        string valStr = val.ToString();
                        if (valStr.Length > 4000 && valStr.Length < 8000)
                        {
                            parms[i].Size = -1;
                        }
                    }
                }
                else if (PI.PropertyType == typeof(System.DateTime))
                {
                    parms[i].SqlDbType = SqlDbType.DateTime;
                    if (val == null) { val = DateTime.Now; }
                }
                else if (PI.PropertyType == typeof(System.Double))
                {
                    parms[i].SqlDbType = SqlDbType.Float;
                    if (val == null) { val = 0; }
                }
                else if (PI.PropertyType == typeof(System.Boolean))
                {
                    parms[i].SqlDbType = SqlDbType.Bit;
                    if (val == null) { val = false; }
                }
                else if (PI.PropertyType == typeof(Nullable<DateTime>))
                {
                    parms[i].SqlDbType = SqlDbType.DateTime;
                    if (val == null) { val = DBNull.Value; }
                }
                else if (PI.PropertyType == typeof(Nullable<int>))
                {
                    parms[i].SqlDbType = SqlDbType.Int;
                    if (val == null) { val = DBNull.Value; }
                }
                parms[i].Value = val;
                i++;
            }
            return parms;
        }

        /// <summary>
        /// 根据实体类生成SqlParameter集合,用于大量参数
        /// </summary>
        /// <param name="mouob">实体类</param>
        /// <returns>SqlParameter[]</returns>
        public static SqlParameter[] ParmsByModule(object mouob, PropertyInfo[] fields = null)
        {
            PropertyInfo[] Fields;

            if (fields != null)
            {
                Fields = fields;
            }
            else
            {
                Fields = mouob.GetType().GetProperties();
            }

            SqlParameter[] parms = new SqlParameter[Fields.Length];
            int i = 0;
            foreach (PropertyInfo PI in Fields)
            {
                //获取实体类属性值
                object val = PI.GetValue(mouob, null);
                parms[i] = new SqlParameter();
                parms[i].ParameterName = "@" + PI.Name;
                if (PI.PropertyType == typeof(System.Int32))
                {
                    parms[i].SqlDbType = SqlDbType.Int;
                    if (val == null) { val = 0; }
                }
                else if (PI.PropertyType == typeof(System.String))
                {
                    parms[i].SqlDbType = SqlDbType.VarChar;
                    if (val == null) { val = ""; }
                    else
                    {
                        string valStr = val.ToString();
                        if (valStr.Length > 4000 && valStr.Length < 8000)
                        {
                            parms[i].Size = -1;
                        }
                    }
                }
                else if (PI.PropertyType == typeof(System.DateTime))
                {
                    parms[i].SqlDbType = SqlDbType.DateTime;
                    if (val == null) { val = DateTime.Now; }
                }
                else if (PI.PropertyType == typeof(System.Double))
                {
                    parms[i].SqlDbType = SqlDbType.Float;
                    if (val == null) { val = 0; }
                }
                else if (PI.PropertyType == typeof(System.Boolean))
                {
                    parms[i].SqlDbType = SqlDbType.Bit;
                    if (val == null) { val = false; }
                }
                else if (PI.PropertyType == typeof(Nullable<DateTime>))
                {
                    parms[i].SqlDbType = SqlDbType.DateTime;
                    if (val == null) { val = DBNull.Value; }
                }
                else if (PI.PropertyType == typeof(Nullable<int>))
                {
                    parms[i].SqlDbType = SqlDbType.Int;
                    if (val == null) { val = DBNull.Value; }
                }
                parms[i].Value = val;
                i++;
            }
            return parms;
        }

        /// <summary>
        /// 根据实体类生成SqlParameter集合,用于大量参数(返回自增后的主键时使用)
        /// </summary>
        /// <param name="mouob">实体类</param>
        /// <returns>SqlParameter[]</returns>
        public static SqlParameter[] ParmsByModuleAddId(object mouob)
        {
            PropertyInfo[] Fields = mouob.GetType().GetProperties();
            SqlParameter[] parms = new SqlParameter[Fields.Length+1];
            int i = 0;
            foreach (PropertyInfo PI in Fields)
            {
                parms[i] = new SqlParameter();
                parms[i].ParameterName = "@" + PI.Name;
                if (PI.PropertyType == typeof(System.Int32))
                {
                    parms[i].SqlDbType = SqlDbType.Int;
                }
                else if (PI.PropertyType == typeof(System.String))
                {
                    parms[i].SqlDbType = SqlDbType.VarChar;
                }
                else if (PI.PropertyType == typeof(System.DateTime))
                {
                    parms[i].SqlDbType = SqlDbType.DateTime;
                }
                else if (PI.PropertyType == typeof(System.Double))
                {
                    parms[i].SqlDbType = SqlDbType.Float;
                }
                else if (PI.PropertyType == typeof(System.Boolean))
                {
                    parms[i].SqlDbType = SqlDbType.Bit;
                }
                parms[i].Value = PI.GetValue(mouob, null);
                i++;
            }
            //插入后自增ID
            parms[i] = new SqlParameter();
            parms[i].ParameterName = "@InsertId";
            parms[i].SqlDbType = SqlDbType.Int;
            parms[i].Value =0;
            parms[i].Direction = ParameterDirection.Output;
            return parms;
        }
    }
}
