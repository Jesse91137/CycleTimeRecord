using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using CycleTimeRecord.Models;
using CycleTimeRecord.ViewModels;
using System.Data;
using System.IO;

namespace CycleTimeRecord.Controllers
{
    /// <summary>
    ///  生產週期時間記錄系統
    /// </summary>
    public class CycleTimeController : Controller
    {
        /// <summary>
        /// DB連線
        /// </summary>
        CycleTimeRecordEntities db = new CycleTimeRecordEntities();
        /// <summary>
        ///  用於與Web服務進行通信，通過SOAP協定發送和接收消息
        /// </summary>
        QueryServiceAMES.QueryServiceSoapClient AMES = new QueryServiceAMES.QueryServiceSoapClient();
        /// <summary>
        /// 每頁顯示的記錄數
        /// </summary>
        int pagesize = 10;

        // GET: CycleTime
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 由作業特定人員依據線別、機種名稱填入設備時數；標準工時計算 = 瓶頸時間/60/板數
        /// </summary>
        /// <param name="page">當前頁碼</param>
        /// <returns></returns>
        public ActionResult CTWork(int page = 1)
        {
            // 驗證登入
            if (!Login_Authentication())
            {
                //導向 Login 頁面
                return RedirectToAction("Login", "Home");
            }
            int currentPage = page < 1 ? 1 : page;
            CTWorks tWorks = new CTWorks(db);
            // 呼叫 CT_LineDatas 方法，取得資料
            List<CTWorks.CT_DataList> ctDataList = tWorks.CT_LineDatas();
            double percent = tWorks.Optimization(null, null); //載入不帶日期
            ViewBag.PercentAge = percent;

            ViewBag.MEmarkOptions = MEmarkToSelectList();//取得下拉選單-ME 確認原因
            ViewBag.MarkOptions = MarkToSelectList();//取得下拉選單-工時原因

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CTWorkPartial", ctDataList.ToPagedList(currentPage, pagesize));//分別表示當前頁碼和每頁顯示的記錄數
            }
            else
            {
                return View("CTWork", "_LayoutMember", ctDataList.ToPagedList(currentPage, pagesize));//分別表示當前頁碼和每頁顯示的記錄數
            }


        }

        /// <summary>
        /// 載入CTWorkPartial頁面，取得標準工時作業下半部資料並分頁顯示。
        /// </summary>
        /// <param name="page">目前頁碼，預設為1。</param>
        /// <returns>回傳部分視圖 _CTWorkPartial，包含分頁後的標準工時資料。</returns>
        [HttpGet]
        public ActionResult CTWorkPartial(int? page)
        {
            // 驗證登入
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            int currentPage = page ?? 1;
            CTWorks tWorks = new CTWorks(db);
            // 呼叫 CT_LineDatas 方法，取得資料
            List<CTWorks.CT_DataList> ctDataList = tWorks.CT_LineDatas();
            // 傳回部分視圖
            return PartialView("_CTWorkPartial", ctDataList.ToPagedList(currentPage, pagesize));
        }

        /// <summary>
        /// 線別 & 機種名稱 & 板面 有都值才查詢
        /// </summary>
        /// <param name="Line">線別</param>
        /// <param name="engSr">機種名稱</param>
        /// <param name="TB">板面</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CTWorkAjax(string Line, string engSr, string TB)
        {
            CTWorks tWorks = new CTWorks(db);
            // 呼叫 CT_LineDatas 方法，取得資料
            List<CTWorks.CT_DataList> ctDataList = tWorks.CT_LineDataAj(Line, engSr, TB);
            return Json(ctDataList);
        }
        /// <summary>
        /// 瓶頸工時更新
        /// </summary>
        /// <param name="engSr">機種名稱</param>
        /// <param name="Line">線別</param>
        /// <param name="TB">板面</param>
        /// <param name="BoardSum">板數</param>
        /// <param name="OPCnt">標準人數</param>
        /// <param name="Text1">L3:NXT/L2:FX3/L4:NXT</param>
        /// <param name="Text2">L3:XPF1/L2:JUKI50/L4:XPF1</param>
        /// <param name="Text3">L3:AIMEX/L2:JUKI80/L4:XPF2</param>
        /// <param name="BottleneckHours">瓶頸工時</param>
        /// <param name="Other">其他瓶頸</param>
        /// <param name="Mark">工時原因</param>
        /// <param name="MEmark">ME標記</param>
        /// <param name="MEmarkAdd">ME確認原因補充</param>
        /// <param name="page">頁數</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CTWorkInHistory(string engSr, string Line, string TB, int BoardSum, int OPCnt, double Text1, double Text2
            , double Text3, decimal BottleneckHours, double Other, string Mark, string MEmark, string MEmarkAdd, int page = 1)
        {

            //字母轉大寫
            engSr = engSr.ToUpper();

            string UserId = (Session["Member"] as MemberViewModels).fUserId;
            int currentPage = page < 1 ? 1 : page;
            CTWorks tWorks = new CTWorks(db);
            List<CTWorks.CT_DataList> ctDataList;
            // 呼叫 CT_LineDatas 方法，取得資料
            if (Line == "L1" || Line == "L3")
            {
                ctDataList = tWorks.CT_LineDatasInHistory(engSr, "L1", TB, BoardSum, Text1, Text2, Text3, BottleneckHours, Other, Mark, UserId
                    , OPCnt, MEmark, MEmarkAdd);
                ctDataList = tWorks.CT_LineDatasInHistory(engSr, "L3", TB, BoardSum, Text1, Text2, Text3, BottleneckHours, Other, Mark, UserId
                    , OPCnt, MEmark, MEmarkAdd);
            }
            else
            {
                ctDataList = tWorks.CT_LineDatasInHistory(engSr, Line, TB, BoardSum, Text1, Text2, Text3, BottleneckHours, Other, Mark, UserId
                    , OPCnt, MEmark, MEmarkAdd);
            }


            #region 更新AMES 1.0
            /* ConfigureAwait(false) 異步操作 */
            if (Line == "L1" || Line == "L3")
            {
                tWorks.TxStdTimeAsync(engSr, "L1", TB, BoardSum, BottleneckHours, Other, Mark, UserId, OPCnt).ConfigureAwait(false);
                tWorks.TxStdTimeAsync(engSr, "L3", TB, BoardSum, BottleneckHours, Other, Mark, UserId, OPCnt).ConfigureAwait(false);
            }
            else
            {
                tWorks.TxStdTimeAsync(engSr, Line, TB, BoardSum, BottleneckHours, Other, Mark, UserId, OPCnt).ConfigureAwait(false);
            }
            tWorks.TxStdTimeAsync(engSr, "L5", TB, UserId, 0).ConfigureAwait(false);
            tWorks.TxStdTimeAsync(engSr, "L5", TB, UserId, 1).ConfigureAwait(false);
            #endregion

            // 傳回部分視圖
            return PartialView("_CTWorkPartial", ctDataList.ToPagedList(currentPage, pagesize));
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="engSr">機種名稱</param>
        /// <param name="Line">線別</param>
        /// <param name="TB">板面</param>
        /// <param name="indate">日期起</param>
        /// <param name="indate2">日期迄</param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        // 載入CTWorkSearch頁面
        public ActionResult CTWorkSearch(string engSr, string Line, string TB, DateTime? indate, DateTime? indate2, int? page)
        {
            // 驗證登入
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }

            //字母轉大寫
            engSr = engSr.ToUpper();

            int currentPage = page ?? 1;
            CTWorks tWorks = new CTWorks(db);
            double percent = tWorks.Optimization(indate.ToString(), indate2.ToString()); //載入不帶日期

            // 呼叫 CT_LineDatas 方法，取得資料
            List<CTWorks.CT_DataList> ctDataList = tWorks.CT_LineDatas(engSr, Line, TB, indate, indate2);
            var pagedList = ctDataList.ToPagedList(currentPage, pagesize);
            // 傳回部分視圖
            //return PartialView("_CTWorkPartial", pagedList);

            // 返回部分視圖 HTML 和 新的百分比數據
            return Json(new
            {
                success = true,
                partialView = RenderPartialViewToString("_CTWorkPartial", pagedList),
                percentage = percent.ToString("0.00") // 格式化數字
            });
        }

        /// <summary>
        /// 將指定的部分視圖渲染為 HTML 字串。
        /// 用於在 AJAX 請求時，將部分視圖內容以字串形式回傳給前端。
        /// </summary>
        /// <param name="viewName">部分視圖名稱。</param>
        /// <param name="model">要綁定到視圖的資料模型。</param>
        /// <returns>渲染後的 HTML 字串。</returns>
        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// 刪除指定的標準工時記錄。
        /// </summary>
        /// <param name="Sno">要刪除的記錄序號。</param>
        /// <param name="reason">刪除原因。</param>
        /// <returns>導向標準工時作業頁面。</returns>
        public ActionResult DeleteCT(int Sno, string reason)
        {
            // 取得目前使用者帳號
            string UserId = (Session["Member"] as MemberViewModels).fUserId;
            CTWorks del = new CTWorks(db);

            // 執行刪除記錄
            del.DeleteCTrecord(Sno, reason, UserId);

            // 刪除後導向標準工時作業頁面
            return RedirectToAction("CTWork");
        }

        #region Login 驗證相關Class
        /// <summary>
        /// 驗證登入
        /// </summary>
        /// <returns></returns>
        public bool Login_Authentication()
        {
            if (Session["Member"] != null)
            {
                string UserId = (Session["Member"] as MemberViewModels).fUserId;//取得帳號
                string RoleId = (Session["Member"] as MemberViewModels).ROLE_ID;//取得角色
                ViewBag.RoleId = RoleId;
                ViewBag.UserId = UserId;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region SELECT
        /// <summary>
        /// ME 確認原因 SELECT
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> MEmarkToSelectList()
        {
            return typeof(MEmark).GetProperties()
                          .Where(p => p.PropertyType == typeof(string))
                          .Select(p => new SelectListItem { Value = p.Name, Text = p.Name })
                          .ToList();
        }
        /// <summary>
        /// 工時原因
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> MarkToSelectList()
        {
            return typeof(Mark).GetProperties()
                          .Where(p => p.PropertyType == typeof(string))
                          .Select(p => new SelectListItem { Value = p.Name, Text = p.Name })
                          .ToList();
        }
        #endregion
    }
}