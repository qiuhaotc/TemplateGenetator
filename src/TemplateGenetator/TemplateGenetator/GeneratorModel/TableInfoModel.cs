using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.GeneratorModel
{
    [Serializable]
    public class TableInfoModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 列排序值
        /// </summary>
        public long  ColOrder { get; set; }
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 是否为标识 0:不是 1:是
        /// </summary>
        public long IsIdentity { get; set; }
        /// <summary>
        /// 是否为主键 0:不是 1:是
        /// </summary>
        public long IsPrimaryKey { get; set; }
        /// <summary>
        /// 列数据类型
        /// </summary>
        public string ColType { get; set; }
        /// <summary>
        /// 列类型长度
        /// </summary>
        public long TypeLength { get; set; }
        /// <summary>
        /// 列字段长度
        /// </summary>
        public long ColLength { get; set; }
        /// <summary>
        /// 列小数位
        /// </summary>
        public long ColScale { get; set; }
        /// <summary>
        /// 是否允许空值 0:不允许 1:允许
        /// </summary>
        public long AllowEmpty { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 列备注信息
        /// </summary>
        public string ColDesc { get; set; }
    }
}
