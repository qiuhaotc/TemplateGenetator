using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebService.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestSQLlite()
        {
            Data.MyDBContext context = new Data.MyDBContext();

            var data = context.TempInfo.Where(u => true).ToList();
            
            return View(data);
        }

        public ActionResult TestCodeFirst()
        {
            var model=TemplateGenerator.CodeFirstGenerator.CodeFirstHelper.GetModelData<TemporaryFolder.CodeModel.TestModel>();

            return View(model);
        }
    }
}