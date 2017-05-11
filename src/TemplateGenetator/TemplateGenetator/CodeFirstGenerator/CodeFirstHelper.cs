using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateGenerator.GeneratorModel;

namespace TemplateGenerator.CodeFirstGenerator
{
    /// <summary>
    /// 获取模型信息帮助类
    /// </summary>
    public class CodeFirstHelper
    {
        /// <summary>
        /// 获取模型信息方法
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static CodeFirstModel GetModelData(Type modelType)
        {
            CodeFirstModel codeFirstModel = new CodeFirstModel();
            codeFirstModel.ClassName = modelType.Name;
            var attr = GetModelAttribute< ModelAttributes.ItemDisplayNameAttribute>(modelType);

            if (attr != null)
            {
                codeFirstModel.Title = attr.DisplayName;
            }

            codeFirstModel.CodeFirstItemList = CodeFirstAttributeHelper.GetModelAttributeInfo(modelType);

            return codeFirstModel;
        }

        /// <summary>
        /// 获取模型信息方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static CodeFirstModel GetModelData<T>() where T : class 
        {
            CodeFirstModel codeFirstModel = new CodeFirstModel();
            codeFirstModel.ClassName = typeof(T).Name;
            var attr = CodeFirstAttributeHelper.GetCustomAttributeForClass<T, ModelAttributes.ItemDisplayNameAttribute>();

            if (attr != null)
            {
                codeFirstModel.Title = attr.DisplayName;
            }

            codeFirstModel.CodeFirstItemList = CodeFirstAttributeHelper.GetModelAttributeInfo<T>();

            return codeFirstModel;
        }

        /// <summary>
        /// 获取单个Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public static T GetModelAttribute<T>(Type typeInfo) where T : class
        {
            var attribute = typeInfo.GetCustomAttribute(typeof(T),false) as T;

            if(attribute!=null)
            {
                return attribute;
            }

            return null;
        }
    }
}
