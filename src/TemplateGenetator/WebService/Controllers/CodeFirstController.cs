using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;
using TemplateGenerator.GeneratorModel;
using TemplateGenerator.Util;

namespace WebService.Controllers
{
    public class CodeFirstController : Controller
    {
        static Random random = new Random();
        Data.MyDBContext context = new Data.MyDBContext();

        // GET: CodeFirst
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取预置模型及模板数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDataCodeFirst()
        {


            DataInfoForJson model = new DataInfoForJson();

            try
            {
                string nameSpace = AppSettingHelper.GetAppSetting("NameSpaceCodeFirst");
                var types = Assembly.GetExecutingAssembly().GetTypes().Where(u=>u.Namespace==nameSpace).ToList();

                model.DataTable = new List<DataItem>();
                model.Data = new List<DataItem>();

                var files = context.TempInfo.Where(u=>u.TP_Type==(int)Data.TemplateType.ModelFirst).OrderBy(u => u.TP_Order).ToList();


                foreach (var item in files)
                {
                    model.Data.Add(new DataItem()
                    {
                        Key = item.TP_ID.ToString(),
                        Name = item.TP_Name
                    });
                }

                foreach (var typeInfo in types)
                {
                    DataItem item = new DataItem();

                    item.Key = typeInfo.FullName;

                    var attribute = TemplateGenerator.CodeFirstGenerator.CodeFirstHelper.GetModelAttribute<TemplateGenerator.CodeFirstGenerator.ModelAttributes.ItemDisplayNameAttribute>(typeInfo);

                    if (attribute != null)
                    {
                        item.Name = attribute.DisplayName;
                    }
                    else
                    {
                        item.Name = typeInfo.Name;
                    }

                    model.DataTable.Add(item);
                }

                model.Status = 1;

            }
            catch (Exception ex)
            {
                model.Status = 0;
                model.Desc = ex.Message;
            }



            return Json(model);
        }

        /// <summary>
        /// 根据模板生成代码
        /// </summary>
        /// <param name="tempID"></param>
        /// <param name="tablename"></param>
        /// <param name="forceChange"></param>
        /// <returns></returns>
        public JsonResult GeneratorData(int tempID, string tablename, bool forceChange)
        {

            ModelForJson data = new ModelForJson();
            try
            {

                string localURL = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["tempLocation"]);

                string dllLocation = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["dllLocation"]);

                string nameSpace = AppSettingHelper.GetAppSetting("NameSpaceCodeFirst");

                var model = context.TempInfo.Find(tempID);

                if (model != null)
                {
                    var dataModel = Assembly.GetExecutingAssembly().GetTypes().Where(u => u.Namespace == nameSpace&&u.FullName==tablename).ToList();



                    if(dataModel.Count==1)
                    {

                        var modelData =TemplateGenerator.CodeFirstGenerator.CodeFirstHelper.GetModelData(dataModel[0]);

                        string dataString = JsonHelper.ToJson(modelData);

                        string result = TemplateGenerator.RazorGenerator.DomainHelper.GenetatorCodeFirst(dllLocation, Server.MapPath(model.TP_URL), forceChange, dataString);
                        data.Status = 1;
                        data.Result = result;
                    }
                    else
                    {
                        data.Status = 0;
                        data.Result = "错误的模型全称！";
                    }
                   
                }
                else
                {
                    data.Status = 0;
                    data.Desc = "模板信息不存在！";
                }
            }
            catch (Exception ex)
            {
                data.Status = 0;
                data.Desc = ex.Message;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量生成
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchGenerator()
        {
            return View();
        }

        /// <summary>
        /// 批量生成代码
        /// </summary>
        /// <param name="tempIDs"></param>
        /// <param name="tablenames"></param>
        /// <param name="forceChange"></param>
        /// <returns></returns>
        public ActionResult GeneratorDataBatch(int[] tempIDs, string[] tablenames, bool forceChange = false)
        {
            string newFolder = "";

            try
            {

                string dllLocation = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["dllLocation"]);
                string connectStr = System.Configuration.ConfigurationManager.AppSettings["DbConnString"];

                string TemporaryFolder = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["TemporaryFolder"]);

                string nameSpace = AppSettingHelper.GetAppSetting("NameSpaceCodeFirst");

                if (!FileHelper.IsExistDirectory(TemporaryFolder))
                {
                    FileHelper.CreateDirectory(TemporaryFolder);
                }

                newFolder = TemporaryFolder + "/" + (tempIDs.Length > 0 ? tempIDs[0].ToString() : "Temp") + "_" + (int)(random.NextDouble() * 10000);

                if (tablenames.Length > 0)
                {
                    var tempList = context.TempInfo.Where(u => tempIDs.Contains(u.TP_ID)).ToList();

                    FileHelper.CreateDirectory(newFolder);
                    foreach (var temp in tempList)
                    {
                        FileHelper.CreateDirectory(newFolder + "/" + temp.TP_FolderName);



                        foreach (string tableName in tablenames)
                        {


                            var dataModel = Assembly.GetExecutingAssembly().GetTypes().Where(u => u.Namespace == nameSpace && u.FullName == tableName).ToList();



                            if (dataModel.Count == 1)
                            {

                                var modelData = TemplateGenerator.CodeFirstGenerator.CodeFirstHelper.GetModelData(dataModel[0]);

                                string dataString = JsonHelper.ToJson(modelData);

                                string result = TemplateGenerator.RazorGenerator.DomainHelper.GenetatorCodeFirst(dllLocation, Server.MapPath(temp.TP_URL), forceChange, dataString);


                                string filePath = newFolder + "/" + temp.TP_FolderName + "/" + tableName + "." + temp.TP_Extention;

                                FileHelper.CreateFile(filePath);
                                FileHelper.WriteText(filePath, result);

                            }
                           
                        }
                    }

                    //生成压缩包
                    ICSharpCode.SharpZipLib.Zip.FastZip fz = new ICSharpCode.SharpZipLib.Zip.FastZip();
                    fz.CreateZip(newFolder + ".zip", newFolder, true, "");

                    #region 删除文件及文件夹
                    FileHelper.ClearDirectory(newFolder);
                    FileHelper.DeleteDirectory(newFolder);
                    #endregion

                    MemoryStream memStream = new MemoryStream();
                    using (FileStream fs = new FileStream(newFolder + ".zip", FileMode.Open))
                    {
                        memStream.SetLength(fs.Length);
                        fs.Read(memStream.GetBuffer(), 0, (int)fs.Length);
                    }

                    //删除压缩包
                    FileHelper.DeleteFile(newFolder + ".zip");

                    return File(memStream, "application/octet-stream", "批量生成代码" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip");
                }
                else
                {
                    Response.Write("没有数据");
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(newFolder))
                {
                    if (FileHelper.IsExistDirectory(newFolder))
                    {
                        #region 删除文件及文件夹
                        FileHelper.ClearDirectory(newFolder);
                        FileHelper.DeleteDirectory(newFolder);
                        FileHelper.DeleteFile(newFolder + ".zip");
                        #endregion
                    }
                }

                Response.Write(ex.Message);
                return null;
            }

        }
    }
}