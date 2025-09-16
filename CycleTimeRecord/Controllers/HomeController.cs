using System.Web.Mvc;
using CycleTimeRecord.Models;

namespace CycleTimeRecord.Controllers
{
    /// <summary>
    /// 負責處理首頁、登入、登出等相關功能的控制器
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// DB 連線物件
        /// </summary>
        CycleTimeRecordEntities db = new CycleTimeRecordEntities();

        /// <summary>
        /// 首頁顯示，依據是否有登入會員切換不同的版型
        /// </summary>
        /// <returns>回傳首頁 View</returns>
        public ActionResult Index()
        {
            if (Session["Member"] == null)
            {
                // 指定 Index.cshtml 套用 _Layout.cshtml
                return View("Index", "_Layout");
            }
            // 指定 Index.cshtml 套用 _MaterialLayout.cshtml
            return View("Index", "_MaterialLayout");
        }

        /// <summary>
        /// 顯示登入頁面
        /// </summary>
        /// <returns>回傳登入頁面 View</returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 處理登入請求
        /// </summary>
        /// <param name="fUserId">帳號</param>
        /// <param name="fPwd">密碼</param>
        /// <returns>登入成功導向 CycleTime/CTWork，失敗則回登入頁面</returns>
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
            // 使用 session 變數記錄歡迎詞
            Session["WelCome"] = "員工 : " + CT_ML.MemberLogin().Member.fName;
            // 使用 session 變數紀錄登入會員物件
            Session["Member"] = CT_ML.MemberLogin().Member;
            return RedirectToAction("CTWork", "CycleTime");
        }

        /// <summary>
        /// 登出，清除 Session 並導回首頁
        /// </summary>
        /// <returns>回首頁</returns>
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}