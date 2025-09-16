using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CycleTimeRecord.Models;

namespace CycleTimeRecord.Controllers
{
    public class HomeController : Controller
    {
        CycleTimeRecordEntities db = new CycleTimeRecordEntities();
        public ActionResult Index()
        {
            if (Session["Member"] == null)
            {
                //指定Index.cshtml套用_layout.cshtml, View 使用products                
                return View("Index", "_Layout");
            }
            return View("Index", "_MaterialLayout");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd) 
        {
            CT_MemberLogin CT_ML = new CT_MemberLogin(db) 
            {
                UId = fUserId,
                UPwd = fPwd,
            };
                        
            CT_ML.MemberLogin();
            if (CT_ML.MemberLogin().Member == null)
            {
                ViewBag.Message = "帳號密碼錯誤，請重新登入";
                return View();
            }
            //使用session變數記錄歡迎詞
            Session["WelCome"] = "員工 : " + CT_ML.MemberLogin().Member.fName;
            //使用session變數紀錄登入會員物件
            Session["Member"] = CT_ML.MemberLogin().Member;
            return RedirectToAction("CTWork", "CycleTime");
        }
        //GET:Home/Logout
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
        
    }
}