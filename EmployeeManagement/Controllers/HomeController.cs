using EmployeeManagement.BusinessLogic;
using EmployeeManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Unauthorized()
        {
            return View("Unauthorized");
        }

        public ActionResult Tab()
        {
            var tabs = new MyTabs();
            //I store the json string in a local json file
            using (StreamReader sr = new StreamReader(Server.MapPath("~/Content/tabs.json")))
            {
                tabs = JsonConvert.DeserializeObject<MyTabs>(sr.ReadToEnd());
            }
            return View(tabs);
        }
    }
}