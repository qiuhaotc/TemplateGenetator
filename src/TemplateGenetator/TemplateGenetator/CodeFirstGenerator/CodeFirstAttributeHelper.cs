using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateGenerator.CodeFirstGenerator.ModelAttributes;
using TemplateGenerator.GeneratorModel;

namespace TemplateGenerator.CodeFirstGenerator
{
    class CodeFirstAttributeHelper
    {
        /// <summary>
        /// 获取 CodeFirstItem 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static List<CodeFirstItem> GetModelAttributeInfo<T>()
        {
            Type tModel = typeof(T);

            var props = tModel.GetProperties();

            List<CodeFirstItem> itemsList = new List<CodeFirstItem>();

            foreach (var prop in props)
            {
                CodeFirstItem itemModel = new CodeFirstItem();

                var attributes = prop.GetCustomAttributes(false);

                foreach (var attribute in attributes)
                {
                    InitItemModel(itemModel, attribute);
                }

                itemModel.ItemName = prop.Name;


                itemsList.Add(itemModel);
            }

            return itemsList;
        }

        /// <summary>
        /// 获取 CodeFirstItem 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelType"></param>
        /// <returns></returns>
        internal static List<CodeFirstItem> GetModelAttributeInfo(Type modelType)
        {
            Type tModel = modelType;

            var props = tModel.GetProperties();

            List<CodeFirstItem> itemsList = new List<CodeFirstItem>();

            foreach (var prop in props)
            {
                CodeFirstItem itemModel = new CodeFirstItem();

                var attributes = prop.GetCustomAttributes(false);

                foreach (var attribute in attributes)
                {
                    InitItemModel(itemModel, attribute);
                }

                itemModel.ItemName = prop.Name;


                itemsList.Add(itemModel);
            }

            return itemsList;
        }

        /// <summary>
        /// 获取单个Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        internal static U GetCustomAttributeForClass<T,U>() where T:class where U:class
        {
            Type t = typeof(T);
            var attr = t.GetCustomAttribute(typeof(U),false) as U;
            if(attr!=null)
            {
                return attr;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Attribute赋值
        /// </summary>
        /// <param name="itemModel"></param>
        /// <param name="attribute"></param>
        private static void InitItemModel(CodeFirstItem itemModel, object attribute)
        {
            Type t = attribute.GetType();

            switch (t.Name)
            {
                case "DisplayTypeAttribute":

                    DisplayTypeAttribute attr1 = attribute as DisplayTypeAttribute;

                    if(attr1!=null)
                    {
                        itemModel.DisplayType = attr1.DisplayType;
                    }

                    break;
                case "ItemDisplayNameAttribute":

                    ItemDisplayNameAttribute attr2 = attribute as ItemDisplayNameAttribute;

                    if (attr2 != null)
                    {
                        itemModel.ItemDisplayName = attr2.DisplayName;
                    }

                    break;
                case "KeyPropertyAttribute":

                    KeyPropertyAttribute attr3 = attribute as KeyPropertyAttribute;

                    if (attr3 != null)
                    {
                        itemModel.IsKey = true;
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
