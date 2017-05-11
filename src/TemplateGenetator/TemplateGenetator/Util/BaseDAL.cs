using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TemplateGenerator.Util
{
    /// <summary>
    /// DAL对象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseDAL<T> where T : class,new()
    {
        public static string TableName;
        public static string KeyName;
        public static string AddTxt;
        public static string UpdateTxt;
        public static PropertyInfo[] propArray;

        public BaseDAL()
        {
            SetParameter();
            SetAddUpdateTxt();
        }

        private void SetAddUpdateTxt()
        {
            if (string.IsNullOrEmpty(KeyName) || string.IsNullOrEmpty(AddTxt) || string.IsNullOrEmpty(UpdateTxt))
            {
                object o = new object();
                lock (o)
                {
                    if (string.IsNullOrEmpty(KeyName) || string.IsNullOrEmpty(AddTxt) || string.IsNullOrEmpty(UpdateTxt))
                    {
                        Type t = typeof(T);
                        PropertyInfo[] prop = t.GetProperties();

                        propArray = prop;
                        KeyName = prop[0].Name;

                        List<string> propList = new List<string>();
                        StringBuilder sb = new StringBuilder(10);
                        sb.Append("INSERT INTO " + TableName + "(");
                        for (int i = 1; i < prop.Length; i++)
                        {
                            propList.Add(prop[i].Name);
                        }
                        sb.Append(string.Join(",", propList.ToArray()) + ")VALUES(");
                        sb.Append(string.Join(",", propList.Select(u => "@" + u).ToArray()) + ")");
                        AddTxt = sb.ToString();

                        sb.Clear();
                        sb.Append("UPDATE " + TableName + " SET " + string.Join(",", propList.Select(u => u + "=" + "@" + u).ToArray()) + " WHERE " + KeyName + "=@" + KeyName);

                        UpdateTxt = sb.ToString();
                    }
                }
            }
        }

        public abstract void SetParameter();

        /// <summary>
        /// 根据ID返回填充后的实体类
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns>返回实体类</returns>
        public T GetModule(int ID)
        {
            //实例化实体类
            T TM = new T();
            //从数据库读取
            if (ID > 0)
            {
                DataTable dt = SqlHelper.Query("select * from " + TableName + " where " + KeyName + "=@" + KeyName, ParameterHelp.ParmsByWrite(new string[] { KeyName }, new object[] { ID })).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    object obj;
                    if (propArray != null)
                    {
                        obj = ModuleHelp.GetEntity
                       (
                           TM,
                           dt.Rows[0],
                           propArray
                       );
                    }
                    //读取的数据填充TM
                    else
                    {
                        obj = ModuleHelp.GetEntity
                        (
                            TM,
                            dt.Rows[0]
                        );
                    }
                    TM = (T)obj;
                }
                else
                {
                    TM = (T)ModuleHelp.GetEntity(TM);
                }
            }
            //不从数据取，自动补充值
            else
            {
                TM = (T)ModuleHelp.GetEntity(TM);
            }
            return TM;
        }

        /// <summary>
        /// 根据ID返回填充后的实体类
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>返回实体类</returns>
        public T GetModule(string where)
        {
            //实例化实体类
            T TM = new T();

            DataTable dt = SqlHelper.Query("select * from " + TableName + " where " + where).Tables[0];

            if (dt.Rows.Count > 0)
            {
                object obj;
                //读取的数据填充TM
                if (propArray != null)
                {
                    obj = ModuleHelp.GetEntity
                   (
                       TM,
                       dt.Rows[0],
                       propArray
                   );
                }
                else
                {
                    obj = ModuleHelp.GetEntity
                    (
                        TM,
                        dt.Rows[0]
                    );
                }
                TM = (T)obj;
            }
            else
            {
                TM = (T)ModuleHelp.GetEntity(TM);
            }
            return TM;
        }

        public List<T> GetModuleList(string where)
        {
            return ModuleHelp.GetModelByDT<T>(GetList(where).Tables[0],propArray);
        }

        #region 添加操作 + Add(T TM)
        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="TM">实体类</param>
        /// <returns>影响行数</returns>
        public int Add(T TM)
        {
            return SqlHelper.ExecuteSql(
                    AddTxt,
                    ParameterHelp.ParmsByModule(TM, propArray)
                );
        }
        #endregion

        #region 添加并返回标志ID + AddWithIdentity(T model)

        /// <summary>
        /// 添加并返回标志ID
        /// </summary>
        /// <param name="model">要添加的数据模型</param>
        /// <returns></returns>
        public int AddWithIdentity(T model)
        {
            object id = SqlHelper.ExecuteScalarText(AddTxt + " SELECT @@identity", ParameterHelp.ParmsByModule(model, propArray));
            return Convert.ToInt32(id);
        }

        #endregion 添加并返回标志ID + AddWithIdentity(T model)

        #region 得到数据集合 + GetList

        /// <summary>
        /// 得到数据集合
        /// </summary>
        /// <returns></returns>
        public DataSet GetList()
        {
            return SqlHelper.Query("select * from " + TableName);
        }

        /// <summary>
        /// 得到数据集合
        /// </summary>
        /// <returns></returns>
        public DataSet GetList(string where)
        {
            if (where == "")
            {
                return SqlHelper.Query("select * from " + TableName);
            }
            else
            {
                return SqlHelper.Query("select * from " + TableName + " Where " + where);
            }
        }

        /// <summary>
        /// 得到指定数量的数据集合
        /// </summary>
        /// <returns></returns>
        public DataSet GetTopList(int num, string where, string tableName, string orderField, string field = "*")
        {
            return SqlHelper.Query("select top " + num + " " + field + " from " + tableName + " Where " + where + " order by " + orderField);
        }

        /// <summary>
        /// 得到数据集合
        /// </summary>
        /// <returns></returns>
        public DataSet GetList(string table, string whereStr)
        {
            return SqlHelper.Query("select * from " + table + " Where " + whereStr);
        }

        /// <summary>
        /// 得到数据集合
        /// </summary>
        /// <returns></returns>
        public DataSet GetList(string table, string whereStr, string field)
        {
            return SqlHelper.Query("select " + field + " from " + table + " Where " + whereStr);
        }

        ///<summary>
        /// 分页获取数据列表 存储过程
        /// </summary>
        public DataSet GetList(string selectFileds, string orderFiled, int PageSize, int PageIndex, string strWhere, out int recordCount)
        {
            SqlParameter[] parameters = {
                                new SqlParameter("TableName", SqlDbType.NVarChar), //--表名,多表是请使用 tA a inner join tB b On a.AID = b.AID
                                new SqlParameter("PrimaryKey", SqlDbType.NVarChar),//--主键，可以带表头 a.AID
                                new SqlParameter("SelectFileds", SqlDbType.NVarChar),
                                new SqlParameter("WhereSQL", SqlDbType.NVarChar),
                                new SqlParameter("OrderFiled", SqlDbType.NVarChar),
                                new SqlParameter("PageIndex", SqlDbType.Int),
                                new SqlParameter("PageSize", SqlDbType.Int),
                                new SqlParameter("RecordCount", SqlDbType.Int)
                                        };
            parameters[0].Value = TableName;
            parameters[1].Value = KeyName;
            parameters[2].Value = selectFileds;
            parameters[3].Value = strWhere;
            parameters[4].Value = orderFiled;
            parameters[5].Value = PageIndex;
            parameters[6].Value = PageSize;
            parameters[7].Direction = ParameterDirection.Output;
            DataSet ds = SqlHelper.RunProcedure("CommonPagination", parameters, "ds");

            object obj = parameters[7].Value;
            if (!Convert.IsDBNull(obj))
            {
                recordCount = Convert.ToInt32(obj);
            }
            else
            {
                recordCount = 0;
            }
            return ds;
        }

        ///<summary>
        /// 分页获取数据列表 存储过程
        /// </summary>
        public DataSet GetList(string tableName, string keyName, string selectFileds, string orderFiled, int PageSize, int PageIndex, string strWhere, out int recordCount)
        {
            SqlParameter[] parameters = {
                                new SqlParameter("TableName", SqlDbType.NVarChar), //--表名,多表是请使用 tA a inner join tB b On a.AID = b.AID
                                new SqlParameter("PrimaryKey", SqlDbType.NVarChar),//--主键，可以带表头 a.AID
                                new SqlParameter("SelectFileds", SqlDbType.NVarChar),
                                new SqlParameter("WhereSQL", SqlDbType.NVarChar),
                                new SqlParameter("OrderFiled", SqlDbType.NVarChar),
                                new SqlParameter("PageIndex", SqlDbType.Int),
                                new SqlParameter("PageSize", SqlDbType.Int),
                                new SqlParameter("RecordCount", SqlDbType.Int)
                                        };
            parameters[0].Value = tableName;
            parameters[1].Value = keyName;
            parameters[2].Value = selectFileds;
            parameters[3].Value = strWhere;
            parameters[4].Value = orderFiled;
            parameters[5].Value = PageIndex;
            parameters[6].Value = PageSize;
            parameters[7].Direction = ParameterDirection.Output;
            DataSet ds = SqlHelper.RunProcedure("CommonPagination", parameters, "ds");

            object obj = parameters[7].Value;
            if (!Convert.IsDBNull(obj))
            {
                recordCount = Convert.ToInt32(obj);
            }
            else
            {
                recordCount = 0;
            }
            return ds;
        }

        #endregion 得到数据集合 + GetList

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="TM">实体类</param>
        /// <returns>影响行数</returns>
        public int Update(T TM)
        {
            return SqlHelper.ExecuteSql(
                    UpdateTxt,
                    ParameterHelp.ParmsByModule(TM, propArray)
                );
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int Delete(int keyValue)
        {
            return SqlHelper.ExecuteSql(
                    "delete from " + TableName + " where " + KeyName + "=@" + KeyName,
                     ParameterHelp.ParmsByWrite(new string[] { KeyName }, new object[] { keyValue })
                );
        }

        /// <summary>
        /// 筛选指定字段节约资源
        /// </summary>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public DataTable GetChooseField(string field, string where)
        {
            if (where == "")
            {
                return SqlHelper.Query("select " + field + " from " + TableName).Tables[0];
            }
            else
            {
                return SqlHelper.Query("select " + field + " from " + TableName + " where " + where).Tables[0];
            }
        }

        /// <summary>
        /// 执行SQL代码
        /// </summary>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public DataSet QuerySQL(string SQL, params SqlParameter[] parames)
        {
            return SqlHelper.Query(SQL, parames);
        }

        /// <summary>
        /// 筛选单个字段返回
        /// </summary>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public string GetChooseFieldTop(string field, string where)
        {
            var value = SqlHelper.ExecuteScalar(CommandType.Text, "select " + field + " from " + TableName + " where " + where);
            if (value == DBNull.Value || value == null)
            {
                return "";
            }
            return value.ToString();
        }

        /// <summary>
        /// 筛选制定字段转为 List<U>
        /// </summary>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<U> GetChooseFieldToList<U>(string where) where U : class,new()
        {
            string field = "";

            Type typeU = typeof(U);

            PropertyInfo[] prop = typeU.GetProperties();

            foreach (var item in prop)
            {
                if (field == "")
                {
                    field += item.Name;
                }
                else
                {
                    field += "," + item.Name;
                }
            }

            return ModuleHelp.GetModelByDT<U>(GetChooseField(field, where), prop);
        }

        /// <summary>
        /// 更新指定字段数值
        /// </summary>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int UpdateChooseField(string valuefield, string where)
        {
            return SqlHelper.ExecuteSql("UPDATE  " + TableName
+ " SET   " + valuefield + " WHERE  " + where);
        }

        /// <summary>
        ///  根据条件删除数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int DeleteWhere(string where)
        {
            return SqlHelper.ExecuteSql("delete from " + TableName + " where " + where);
        }

        /// <summary>
        ///  根据条件查询总数
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int CountWhere(string where)
        {
            return (int)SqlHelper.ExecuteScalar(CommandType.Text, "select count(*) from " + TableName + " where " + where);
        }
    }
}