using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TemplateGenerator.GeneratorModel;
using TemplateGenerator.Util;

namespace WebService.Controllers
{
    public class RazorController : Controller
    {
        static Random random = new Random();
        Data.MyDBContext context = new Data.MyDBContext();

        /// <summary>
        /// 生成器首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
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
        /// 获取数据信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetData()
        {
            DataInfoForJson model = new DataInfoForJson();
            model.Data = new List<DataItem>();
            model.DataTable = new List<DataItem>();

            try
            {
                var files = context.TempInfo.Where(u => u.TP_Type == (int)Data.TemplateType.DataFirst).OrderBy(u=>u.TP_Order).ToList();


                foreach (var item in files)
                {
                    model.Data.Add(new DataItem()
                    {
                        Key = item.TP_ID.ToString(),
                        Name = item.TP_Name
                    });
                }

                //TODO 自选数据库
                string connectStr = System.Configuration.ConfigurationManager.AppSettings["DbConnString"];
                string databaseName = System.Configuration.ConfigurationManager.AppSettings["DBName"];
                string dbType= System.Configuration.ConfigurationManager.AppSettings["DBType"];
                var tables = DataBaseInfo.GetDatabaseTablesInfo(databaseName, connectStr, dbType);

                foreach (var item in tables)
                {
                    model.DataTable.Add(new DataItem()
                    {
                        Key = item.Name,
                        Name = item.Name
                    });
                }

                model.Status = 1;
            }
            catch (Exception e)
            {
                model.Status = 0;
                model.Desc = e.Message;
            }

            return Json(model);
        }

       
        /// <summary>
        /// 获取所有表列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTable()
        {
            string connectStr = System.Configuration.ConfigurationManager.AppSettings["DbConnString"];

            
            string databaseName = System.Configuration.ConfigurationManager.AppSettings["DBName"];

            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            DataInfoForJson model = new DataInfoForJson();
            model.Data = new List<DataItem>();

            try
            {
                var tables = DataBaseInfo.GetDatabaseTablesInfo(databaseName, connectStr, dbType);

                foreach (var item in tables)
                {
                    model.Data.Add(new DataItem()
                    {
                        Key = item.Name,
                        Name = item.Name
                    });
                }
                model.Status = 1;
            }
            catch (Exception e)
            {
                model.Status = 0;
                model.Desc = e.Message;
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
                string connectStr = System.Configuration.ConfigurationManager.AppSettings["DbConnString"];
                string dbType= System.Configuration.ConfigurationManager.AppSettings["DBType"];

                var model = context.TempInfo.Find(tempID);

                if (model != null)
                {
                    string result = TemplateGenerator.RazorGenerator.DomainHelper.GenetatorTemp(forceChange,dllLocation, Server.MapPath(model.TP_URL), tablename,connectStr, model.TP_NameSpace,dbType);

                    data.Status = 1;
                    data.Result = result;
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

                string localURL = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["tempLocation"]);
                string dllLocation = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["dllLocation"]);
                string connectStr = System.Configuration.ConfigurationManager.AppSettings["DbConnString"];

                string TemporaryFolder = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["TemporaryFolder"]);

                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

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

                            string result = TemplateGenerator.RazorGenerator.DomainHelper.GenetatorTemp(forceChange,dllLocation, Server.MapPath(temp.TP_URL), tableName, connectStr, temp.TP_NameSpace,dbType);

                            string fileName = string.IsNullOrEmpty(temp.TP_FileName) ? tableName : string.Format(temp.TP_FileName, TemplateGenerator.RazorGenerator.RazorHelper.UpperFirstLetter(tableName));

                            string filePath = newFolder + "/" + temp.TP_FolderName + "/" + fileName + "." + temp.TP_Extention;

                            FileHelper.CreateFile(filePath);
                            FileHelper.WriteText(filePath, result);
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

        /// <summary>
        /// 模板管理
        /// </summary>
        /// <returns></returns>
        public ActionResult TemplateMgr()
        {
            var files = context.TempInfo.OrderBy(u => u.TP_Order).ToList();
            return View(files);
        }

        /// <summary>
        /// 添加模板
        /// </summary>
        /// <returns></returns>
        public ActionResult TempAdd()
        {
            Data.TempInfo model = new Data.TempInfo()
            {
                TP_Order = 100,
                TP_FolderName = "FolderName",
                TP_Type=(int)Data.TemplateType.DataFirst
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult TempAdd(int OrderID, string TempDesc, string FolderName, string TempName, string Content, string extention, string namespaceStr,int TempType,string fileName)
        {
            ModelForJson data = new ModelForJson();

            try
            {
                string localURL = System.Configuration.ConfigurationManager.AppSettings["tempLocation"];

                if (!string.IsNullOrEmpty(Content) && !string.IsNullOrEmpty(TempName))
                {
                    string tempURL = localURL + "\\" + TempName + ".cshtml";

                    string path = Server.MapPath(tempURL);
                    if (FileHelper.IsExistFile(path))
                    {
                        data.Status = 0;
                        data.Desc = "已存在相同模板，无法添加！";
                    }
                    else
                    {
                        FileHelper.CreateFile(path);

                        FileHelper.WriteText(path, Content);

                        Data.TempInfo infoModel = new Data.TempInfo();
                        infoModel.TP_Order = OrderID;
                        infoModel.TP_Desc = TempDesc;
                        infoModel.TP_FolderName = FolderName;
                        infoModel.TP_Name = TempName;
                        infoModel.TP_AddDate = DateTime.Now.ToString();
                        infoModel.TP_URL = tempURL;
                        infoModel.TP_IsSysTemp = 0;
                        infoModel.TP_Extention = extention;
                        infoModel.TP_NameSpace = namespaceStr;
                        infoModel.TP_Type = TempType;
                        infoModel.TP_FileName = fileName;

                        context.TempInfo.Add(infoModel);
                        context.SaveChanges();

                        data.Status = 1;
                        data.Desc = "保存成功！";
                    }
                }
                else
                {
                    data.Status = 0;
                    data.Desc = "模板名或模板内容不能为空！";
                }
            }
            catch (Exception ex)
            {
                data.Status = 0;
                data.Desc = ex.Message;
            }

            return Json(data);
        }

        /// <summary>
        /// 编辑模板
        /// </summary>
        /// <param name="tempname"></param>
        /// <returns></returns>
        public ActionResult TempEdit(int tempID)
        {
            var model = context.TempInfo.Find(tempID);

            if (model == null)
            {
                return RedirectToAction("/Razor/TemplateMgr");
            }
            else
            {
                string path = Server.MapPath(model.TP_URL);

                string content = System.IO.File.ReadAllText(path);

                ViewBag.Content = content;

                return View(model);
            }

        }

        /// <summary>
        /// 模板编辑保存
        /// </summary>
        /// <param name="TempID"></param>
        /// <param name="OrderID"></param>
        /// <param name="TempDesc"></param>
        /// <param name="FolderName"></param>
        /// <param name="TempName"></param>
        /// <param name="Content"></param>
        /// <param name="extention"></param>
        /// <param name="namespaceStr"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult TempEdit(int TempID, int OrderID, string TempDesc, string FolderName, string TempName, string Content, string extention, string namespaceStr,int TempType,string fileName)
        {
            ModelForJson data = new ModelForJson();

            try
            {
                var model = context.TempInfo.Find(TempID);

                if (model != null)
                {
                    if (!string.IsNullOrEmpty(Content) && !string.IsNullOrEmpty(TempName))
                    {
                        string path = Server.MapPath(model.TP_URL);
                        if (FileHelper.IsExistFile(path))
                        {
                            model.TP_Order = OrderID;
                            model.TP_Desc = TempDesc;
                            model.TP_FolderName = FolderName;
                            model.TP_Name = TempName;
                            model.TP_AddDate = DateTime.Now.ToString();
                            model.TP_Extention = extention;
                            model.TP_NameSpace = namespaceStr;
                            model.TP_Type = TempType;
                            model.TP_FileName = fileName;

                            context.SaveChanges();

                            FileHelper.WriteText(path, Content);
                            data.Status = 1;
                            data.Desc = "保存成功！";
                        }
                        else
                        {
                            data.Status = 0;
                            data.Desc = "保存错误，模板文件不存在！";
                        }
                    }
                    else
                    {
                        data.Status = 0;
                        data.Desc = "模板名或模板内容不能为空！";
                    }
                }
                else
                {
                    data.Status = 0;
                    data.Desc = "模板信息错误！";
                }
            }
            catch (Exception ex)
            {
                data.Status = 0;
                data.Desc = ex.Message;
            }

            return Json(data);
        }

        public ActionResult TempDelete(int tempID)
        {
            ModelForJson data = new ModelForJson();

            try
            {
                var model = context.TempInfo.Find(tempID);

                if (model != null)
                {

                    string path = Server.MapPath(model.TP_URL);

                    if (FileHelper.IsExistFile(path))
                    {
                        if (model.TP_IsSysTemp == 1)
                        {
                            data.Status = 0;
                            data.Desc = "预置模板不允许删除！";
                        }
                        else
                        {
                            context.TempInfo.Remove(model);

                            FileHelper.DeleteFile(path);
                            data.Status = 1;
                            data.Desc = "删除成功！";

                            context.SaveChanges();
                        }

                    }
                    else
                    {
                        data.Status = 0;
                        data.Desc = "模板文件不存在！";
                    }
                }
                else
                {
                    data.Status = 0;
                    data.Desc = "模板文件不存在！";
                }
            }
            catch (Exception ex)
            {
                data.Status = 0;
                data.Desc = ex.Message;
            }

            return Json(data);
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        /// <returns></returns>
        public ActionResult DatabaseConfig()
        {
            ViewBag.SQLStr = System.Configuration.ConfigurationManager.AppSettings["DbConnString"];
            ViewBag.DbType= System.Configuration.ConfigurationManager.AppSettings["DBType"];
            ViewBag.DBName = System.Configuration.ConfigurationManager.AppSettings["DBName"];
            return View();
        }


        /// <summary>
        /// 数据库配置
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DatabaseConfig(string sqlStr, string dbType, string dbName)
        {
            ModelForJson data = new ModelForJson();
            try
            {
                TemplateGenerator.Util.AppSettingHelper.UpdateWebConfigAppSetting("DbConnString", sqlStr);

                TemplateGenerator.Util.AppSettingHelper.UpdateWebConfigAppSetting("DBType", dbType);

                TemplateGenerator.Util.AppSettingHelper.UpdateWebConfigAppSetting("DBName", dbName);

                data.Status = 1;
                data.Desc = "配置成功";
            }
            catch (Exception e)
            {
                data.Status = 0;
                data.Desc = e.Message;
            }


            return Json(data);
        }

    }

}