using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebService.Data
{
    [Table("TempInfo")]
    public class TempInfo
    {
        [Key]
        public int TP_ID { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TP_Name { get; set; }
        public string TP_URL { get; set; }
        public string TP_Desc { get; set; }
        public string TP_AddDate { get; set; }
        public int TP_Order { get; set; }
        public int TP_IsSysTemp { get; set; }
        /// <summary>
        /// 生成的文件夹名称
        /// </summary>
        public string TP_FolderName { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string TP_NameSpace { get; internal set; }
        public string TP_Extention { get; internal set; }
        /// <summary>
        /// 模板类型
        /// </summary>
        public int TP_Type { get; set; }
        public string TP_FileName { get; set; }
    }
}