using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateGenerator.GeneratorModel;

namespace TemplateGenerator.Util
{
    public class DataBaseInfo
    {

        static TableInfoData dataTable = new TableInfoData();
        static String sqlTableInfo = @"SELECT   
(case when a.colorder=1 then d.name else '' end) as TableName,--如果表名相同就返回空 
     a.colorder as ColOrder,  
     a.name as ColName,  
     (case when COLUMNPROPERTY( a.id,a.name, 'IsIdentity' )=1 then 1 else 0 end) as IsIdentity,  
     (case when (SELECT count(*) FROM sysobjects--查询主键 
                     WHERE (name in   
                             (SELECT name FROM sysindexes   
                             WHERE (id = a.id)   AND (indid in   
                                     (SELECT indid FROM sysindexkeys  
                                       WHERE (id = a.id) AND (colid in   
                                         (SELECT colid FROM syscolumns  
                                         WHERE (id = a.id) AND (name = a.name))  
                         )))))   
         AND (xtype = 'PK' ))>0 then 1 else 0 end) as IsPrimaryKey,--查询主键END  
b.name as ColType,  
a.length as TypeLength,  
COLUMNPROPERTY(a.id,a.name,'PRECISION' ) as    ColLength,  
isnull(COLUMNPROPERTY(a.id,a.name,'Scale' ),0) as ColScale,  
(case when a.isnullable=1 then 1 else 0 end) as AllowEmpty,  
isnull(e.text,'' ) as DefaultValue,  
isnull(g.[value],'' ) AS ColDesc  
FROM syscolumns a left join systypes b   
on a.xtype=b.xusertype  
inner join sysobjects d   
on a.id=d.id and (d.xtype='U' OR d.xtype='V') and d.name<> 'dtproperties'   
left join syscomments e  
on a.cdefault=e.id  
left join sys.extended_properties g  
on a.id=g.major_id AND a.colid = g.minor_id   
     where d.name='{0}' --所要查询的表 
order by a.id,a.colorder  ";

        /// <summary>
        /// 根据数据库名获取所有表和视图
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static List<DataItem> GetDatabaseTablesInfo(string databaseName,string connectStr,string dbType)
        {
            

            DataSet ds = null;

            if (IsMySQL(dbType))
            {
                MySQLHelper.connectionString = connectStr;
                ds = MySQLHelper.GetDataSet(MySQLHelper.connectionString,CommandType.Text, " select TABLE_NAME from information_schema.tables where TABLE_SCHEMA=@tableSchema", new MySql.Data.MySqlClient.MySqlParameter("@tableSchema",databaseName));
            }
            else
            {
                SqlHelper.connectionString = connectStr;
                ds = SqlHelper.Query(" SELECT Name FROM SysObjects Where XType='U' OR XType='V' ORDER BY Name ");
            }

            List<DataItem> itemsList = new List<DataItem>();

            foreach (DataRow item in ds.Tables[0].Rows)
            {
                itemsList.Add(new DataItem() {
                    Key = item[0].ToString(),
                    Name=item[0].ToString()
                });
            }

            return itemsList;
        }

        /// <summary>
        /// 判断是否因为MySQL数据库
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private static bool IsMySQL(string dbType)
        {
            return dbType == "MySQL";
        }

        static String sqlTableInfo2 = @"SELECT   
(case when a.colorder=1 then d.name else '' end) as TableName,--如果表名相同就返回空 
     a.colorder as ColOrder,  
     a.name as ColName,  
     (case when COLUMNPROPERTY( a.id,a.name, 'IsIdentity' )=1 then 1 else 0 end) as IsIdentity,  
     (case when (SELECT count(*) FROM sysobjects--查询主键 
                     WHERE (name in   
                             (SELECT name FROM sysindexes   
                             WHERE (id = a.id)   AND (indid in   
                                     (SELECT indid FROM sysindexkeys  
                                       WHERE (id = a.id) AND (colid in   
                                         (SELECT colid FROM syscolumns  
                                         WHERE (id = a.id) AND (name = a.name))  
                         )))))   
         AND (xtype = 'PK' ))>0 then 1 else 0 end) as IsPrimaryKey,--查询主键END  
b.name as ColType,  
a.length as TypeLength,  
COLUMNPROPERTY(a.id,a.name,'PRECISION' ) as    ColLength,  
isnull(COLUMNPROPERTY(a.id,a.name,'Scale' ),0) as ColScale,  
(case when a.isnullable=1 then 1 else 0 end) as AllowEmpty,  
isnull(e.text,'' ) as DefaultValue,  
isnull(g.[value],'' ) AS ColDesc  
FROM syscolumns a left join systypes b   
on a.xtype=b.xusertype  
inner join sysobjects d   
on a.id=d.id and (d.xtype='U' OR d.xtype='V') and d.name<> 'dtproperties'   
left join syscomments e  
on a.cdefault=e.id  
left join sys.extended_properties g  
on a.id=g.major_id AND a.colid = g.minor_id   
     where d.name=@TableName --所要查询的表 
order by a.id,a.colorder  ";

        static String sqlTableInfoMySQL = @"use information_schema;  
select 
TABLE_Name as TableName
,Cast(ORDINAL_POSITION as Signed) as ColOrder
,COLUMN_Name as ColName
,case when EXTRA='auto_increment' then 1 else 0 end as IsIdentity
,case when COLUMN_KEY='PRI' then 1 else 0 END as IsPrimaryKey
,Data_Type as ColType
,10 as TypeLength
,case when (substring_index(substring_index(COLUMN_Type,'(',-1),')',1) REGEXP '[0-9]')=1 then Cast(substring_index(substring_index(COLUMN_Type,'(',-1),')',1) as Signed) else 100 end as ColLength
,0 as ColScale
,case when IS_NULLABLE='YES' then 1 else 0 end as AllowEmpty
,'' as DefaultValue
,'' as ColDesc
 from columns where table_name=@TableName;  ";

        /// <summary>
        /// 获取表所有相关数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static IList<List<TableInfoModel>> GetTableInfo(List<string> tableNameList)
        {
            StringBuilder sb = new StringBuilder(tableNameList.Count * (sqlTableInfo.Length + 20));

            foreach (var item in tableNameList)
            {
                sb.Append(string.Format(sqlTableInfo, item));
            }

            DataSet dtTableInfo = dataTable.QuerySQL(sb.ToString());

            IList<List<TableInfoModel>> listThis = new List<List<TableInfoModel>>();

            PropertyInfo[] propInfo = typeof(TableInfoModel).GetProperties();

            foreach (DataTable item in dtTableInfo.Tables)
            {
                List<TableInfoModel> list = ModuleHelp.GetModelByDT<TableInfoModel>(item, propInfo);
                listThis.Add(list);
            }
            

            return listThis;
        }

        /// <summary>
        /// 获取单个数据表数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static IList<TableInfoModel> GetOneTableInfo(string tableName,string dbType,string connectionString)
        {
            DataSet dtTableInfo = null;

            if (IsMySQL(dbType))
            {
                MySQLHelper.connectionString = connectionString;

                dtTableInfo = MySQLHelper.GetDataSet(MySQLHelper.connectionString, CommandType.Text, sqlTableInfoMySQL, new MySql.Data.MySqlClient.MySqlParameter("TableName", MySql.Data.MySqlClient.MySqlDbType.VarChar) { Value = tableName });
            }
            else
            {
                SqlHelper.connectionString = connectionString;

                dtTableInfo = dataTable.QuerySQL(sqlTableInfo2, new System.Data.SqlClient.SqlParameter("TableName", SqlDbType.NVarChar) { Value = tableName });
            }

            IList<TableInfoModel> listDataColomn = new List<TableInfoModel>();

            PropertyInfo[] propInfo = typeof(TableInfoModel).GetProperties();

            if (dtTableInfo.Tables.Count > 0)
            {
                listDataColomn = ModuleHelp.GetModelByDT<TableInfoModel>(dtTableInfo.Tables[0], propInfo);
            }

            return listDataColomn;
        }
    }
}
