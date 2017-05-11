using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateGenerator.GeneratorModel.EnumData;

namespace TemplateGenerator.Util
{
    /// <summary>
    /// 枚举信息帮助类
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 获取枚举集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<EnumItem> GetEnumList<T>(int maxValue = -1, int minValue = -1)
        {
            List<EnumItem> enumList = new List<EnumItem>();
            Type enumType = typeof(T);
            string[] names = Enum.GetNames(enumType);
            int[] values = (int[])Enum.GetValues(enumType);
            if (maxValue == -1)
            {
                for (int i = 0; i < values.Length; i++)
                {

                    object[] objs = enumType.GetField(names[i]).GetCustomAttributes(typeof(EnumNameAttribute), false);
                    if (objs == null || objs.Length == 0)
                    {
                    }
                    else
                    {
                        EnumNameAttribute attr = objs[0] as EnumNameAttribute;
                        string strName = attr.EnumName;
                        int value = values[i];
                        enumList.Add(new EnumItem() { Key = value, Name = strName });
                    }
                }
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] <= maxValue && values[i] >= minValue)
                    {
                        object[] objs = enumType.GetField(names[i]).GetCustomAttributes(typeof(EnumNameAttribute), false);
                        if (objs == null || objs.Length == 0)
                        {

                        }
                        else
                        {
                            EnumNameAttribute attr = objs[0] as EnumNameAttribute;
                            string strName = attr.EnumName;
                            int value = values[i];
                            enumList.Add(new EnumItem() { Key = value, Name = strName });
                        }
                    }
                }
            }
            return enumList;
        }

        /// <summary>
        /// 获取某个枚举某个值的EnumName
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(TEnum enumValue)
        {
            object[] objs = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttributes(typeof(EnumNameAttribute), false);

            if (objs.Length > 0)
            {
                EnumNameAttribute attr = objs[0] as EnumNameAttribute;
                return attr.EnumName;
            }
            return "";
        }

        public class EnumItem
        {
            public int Key { get; set; }
            public string Name { get; set; }
        }
    }
}
