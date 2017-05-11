using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RazorEngine.Templating;
using TemplateGenerator.Util;

namespace TemplateGenerator.RazorGenerator
{
    internal class GeneratorHelper:MarshalByRefObject
    {
        /// <summary>
        /// 根据模板生成代码
        /// </summary>
        /// <param name="tempURL"></param>
        /// <param name="tablename"></param>
        /// <param name="connectionStr"></param>
        /// <param name="nameSpaceStr"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        internal string GenetatorTemp(string tempURL, string tablename,string connectionStr,string nameSpaceStr,string dbType)
        {

            DynamicViewBag viewdata = new DynamicViewBag();

            string result = "";

            viewdata.AddValue("Data", DataBaseInfo.GetOneTableInfo(tablename, dbType, connectionStr));

            viewdata.AddValue("NameSpaceStr", nameSpaceStr);

            viewdata.AddValue("ClassName", tablename);

            try
            {
                ParseHelper helper = new ParseHelper();

                result = helper.ParseData(tempURL, tempURL, null, viewdata);
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 根据定义的模型类生成代码
        /// </summary>
        /// <param name="tempURL"></param>
        /// <param name="dataString"></param>
        /// <returns></returns>
        internal string GenetatorCodeFirst(string tempURL, string dataString)
        {

            string result = "";

            DynamicViewBag viewdata = new DynamicViewBag();

            try
            {
                ParseHelper helper = new ParseHelper();

                GeneratorModel.CodeFirstModel modelData = JsonHelper.ToObject<GeneratorModel.CodeFirstModel>(dataString);

                result = helper.ParseData(tempURL, tempURL, modelData, viewdata);
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public override object InitializeLifetimeService()
        {
            //Remoting对象 无限生存期
            return null;
        }
    }
}
