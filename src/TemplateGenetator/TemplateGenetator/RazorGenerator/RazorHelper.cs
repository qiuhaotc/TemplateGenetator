using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.RazorGenerator
{
    public class RazorHelper
    {
        /// <summary>
        /// 根据数据库类型获取C#对应类型
        /// </summary>
        /// <param name="items"></param>
        /// <param name="emptyToDefaultValue"></param>
        /// <returns></returns>
        public static string GetItemType(GeneratorModel.TableInfoModel items,bool emptyToDefaultValue=false)
        {
            string typeStr = "";

            switch (items.ColType)
            {
                case "int":
                    typeStr = "Int32";
                    break;
                    
                case "nvarchar":
                case "varchar":
                    typeStr = "String";
                    break;
                case "datetime":
                    typeStr = "DateTime";
                    break;
                case "bigint":
                    typeStr = "Int64";
                    break;
                case "bit":
                    typeStr = "Boolean";
                    break;
                case "decimal":
                    typeStr = "Decimal";
                    break;
                case "float":
                    typeStr = "Float";
                    break;
                case "uniqueidentifier":
                    typeStr = "Guid";
                    break;
                default:
                    typeStr= items.ColType;
                    break;
            }

            if(emptyToDefaultValue|| items.AllowEmpty==0)
            {
                return typeStr;
            }
            else
            {
                if(items.ColType!= "nvarchar"&&items.ColType!= "varchar")
                {
                    return typeStr + "?";
                }
                else
                {
                    return typeStr;
                }
            }
        }
        
        /// <summary>
        /// 根据数据库类型获取C#对应类型
        /// </summary>
        /// <param name="items"></param>
        /// <param name="emptyToDefaultValue"></param>
        /// <returns></returns>
        public static string GetItemTypeJava(GeneratorModel.TableInfoModel items,bool emptyToDefaultValue=false)
        {
            string typeStr = "";

            switch (items.ColType)
            {
                case "int":
                case "integer":
                    typeStr = "int";
                    break;
                    
                case "nvarchar":
                case "varchar":
                    typeStr = "String";
                    break;
                case "date":
                case "datetime":
                    typeStr = "Date";
                    break;
                case "bigint":
                    typeStr = "long";
                    break;
                case "bit":
                    typeStr = "bool";
                    break;
                case "decimal":
                    typeStr = "BigDecimal";
                    break;
                case "float":
                    typeStr = "Float";
                    break;
                case "uniqueidentifier":
                    typeStr = "Guid";
                    break;
                default:
                    typeStr= items.ColType;
                    break;
            }

            if(emptyToDefaultValue|| items.AllowEmpty==0)
            {
                return typeStr;
            }
            else
            {
                if(items.ColType!= "nvarchar"&&items.ColType!= "varchar")
                {
                    return typeStr + "?";
                }
                else
                {
                    return typeStr;
                }
            }
        }

        /// <summary>
        /// 获取首字母大写的名称
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string UpperFirstLetter(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return content.Substring(0,1).ToUpper() + content.Substring(1, content.Length - 1);
            }
            return content;
        }

        /// <summary>
        /// 获取Java版本的主键类型
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string GetPrimaryKeyTypeJava(List<GeneratorModel.TableInfoModel> items)
        {
            foreach (var item in items)
            {
                if (item.IsPrimaryKey==1)
                {
                    switch (item.ColType)
                    {
                        case "int":
                            return "Integer";
                        case "varchar":
                            return "String";
                    }
                    return item.ColType;
                }
            }
            return "Integer";
        }
    }
}
