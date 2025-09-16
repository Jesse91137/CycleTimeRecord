using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using CycleTimeRecord.Models;
using CycleTimeRecord.ViewModels;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CycleTimeRecord.Controllers
{
    public class CycleTimeController : Controller
    {
        CycleTimeRecordEntities db = new CycleTimeRecordEntities();
        QueryServiceAMES.QueryServiceSoapClient AMES = new QueryServiceAMES.QueryServiceSoapClient();
        int pagesize = 10;
        // GET: CycleTime
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CTWork(int page = 1)
        {
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            int currentPage = page < 1 ? 1 : page;
            CTWorks tWorks = new CTWorks(db);
            // 呼叫 CT_LineDatas 方法，取得資料
            List<CTWorks.CT_DataList> ctDataList = tWorks.CT_LineDatas();
            double percent = tWorks.Optimization(null,null); //載入不帶日期
            ViewBag.PercentAge = percent;

            if (Request.IsAjaxRequest()) 
            {
                return PartialView("_CTWorkPartial", ctDataList.ToPagedList(currentPage, pagesize));
            }
            else
            {
                return View("CTWork", "_LayoutMember", ctDataList.ToPagedList(currentPage, pagesize));
            }                
        }

        [HttpGet]
        public ActionResult CTWorkPartial(int? page)
        {
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            int currentPage = page ??1;
            CTWorks tWorks = new CTWorks(db);
            // 呼叫 CT_LineDatas 方法，取得資料
            List<CTWorks.CT_DataList> ctDataList = tWorks.CT_LineDatas();
            // 傳回部分視圖
            return PartialView("_CTWorkPartial", ctDataList.ToPagedList(currentPage, pagesize));
        }
        [HttpPost]
        public ActionResult CTWorkAjax(string Line,string engSr,string TB)
        {
            CTWorks tWorks = new CTWorks(db);
            // 呼叫 CT_LineDatas 方法，取得資料
            List<CTWorks.CT_DataList> ctDataList = tWorks.CT_LineDataAj(Line, engSr,TB);
            return Json(ctDataList);
        }
        [HttpPost]
        public ActionResult CTWorkInHistory(string engSr, string Line, string TB, int BoardSum,int OPCnt, double Text1, double Text2, double Text3, decimal BottleneckHours, double Other, string Mark,string MEmark,int page = 1 )
        {
            string UserId = (Session["Member"] as MemberViewModels).fUserId;
            int currentPage = page < 1 ? 1 : page;
            CTWorks tWorks = new CTWorks(db);
            List<CTWorks.CT_DataList> ctDataList;
            // 呼叫 CT_LineDatas 方法，取得資料
            if (Line == "L1" || Line == "L3")
            {
                ctDataList = tWorks.CT_LineDatasInHistory(engSr, "L1", TB, BoardSum, Text1, Text2, Text3, BottleneckHours, Other, Mark, UserId,OPCnt, MEmark);
                ctDataList = tWorks.CT_LineDatasInHistory(engSr, "L3", TB, BoardSum, Text1, Text2, Text3, BottleneckHours, Other, Mark, UserId, OPCnt, MEmark);
            }
            else
            {
                ctDataList = tWorks.CT_LineDatasInHistory(engSr, Line, TB, BoardSum, Text1, Text2, Text3, BottleneckHours, Other, Mark, UserId, OPCnt, MEmark);
            }


            #region 更新AMES 1.0
            if (Line == "L1" || Line == "L3")
            {
                tWorks.TxStdTimeAsync(engSr, "L1", TB, BoardSum, BottleneckHours, Other, Mark, UserId, OPCnt).ConfigureAwait(false);
                tWorks.TxStdTimeAsync(engSr, "L3", TB, BoardSum, BottleneckHours, Other, Mark, UserId, OPCnt).ConfigureAwait(false);
            }
            else
            {
                tWorks.TxStdTimeAsync(engSr, Line, TB, BoardSum, BottleneckHours, Other, Mark, UserId, OPCnt).ConfigureAwait(false);
            }
            tWorks.TxStdTimeAsync(engSr, "L5", TB, UserId,0).ConfigureAwait(false);
            tWorks.TxStdTimeAsync(engSr, "L5", TB, UserId,1).ConfigureAwait(false);
            #endregion

            // 傳回部分視圖
            return PartialView("_CTWorkPartial", ctDataList.ToPagedList(currentPage, pagesize));
        }
        [HttpPost]
        public ActionResult CTWorkSearch(string engSr,string Line,string TB, DateTime? indate, DateTime? indate2, int? page)
        {
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }

            int currentPage = page ??1;
            CTWorks tWorks = new CTWorks(db);
            double percent = tWorks.Optimization(indate.ToString(), indate2.ToString()); //載入不帶日期
            
            // 呼叫 CT_LineDatas 方法，取得資料
            List<CTWorks.CT_DataList> ctDataList = tWorks.CT_LineDatas(engSr,Line, TB, indate,indate2);
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

        public ActionResult DeleteCT(int Sno, string reason) 
        {
            string UserId = (Session["Member"] as MemberViewModels).fUserId;
            CTWorks del = new CTWorks(db);

            del.DeleteCTrecord(Sno, reason, UserId);

            return RedirectToAction("CTWork");
        }

        #region Login 驗證相關Class
        public bool Login_Authentication()
            {
                if (Session["Member"] != null)
                {
                    string UserId = (Session["Member"] as MemberViewModels).fUserId;
                    string RoleId = (Session["Member"] as MemberViewModels).ROLE_ID;
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
    }
}