using CycleTimeRecord.Models;
using CycleTimeRecord.ViewModels;
using PagedList;
using System.Web.Mvc;

namespace CycleTimeRecord.Controllers
{
    /// <summary>
    /// 權限階層管理 (操作員/組長/管理員)
    /// </summary>
    public class MaintainController : Controller
    {
        /// <summary>
        /// DB 操作
        /// </summary>
        CycleTimeRecordEntities db = new CycleTimeRecordEntities();
        /// <summary>
        /// 一頁顯示50筆資料
        /// </summary>
        int pageSize = 50;
        // GET: Maintain
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 管理者權限全部資料
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult MemberMaintain(int page = 1)
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            int currentPage = page < 1 ? 1 : page;
            MemberMaintain MM = new MemberMaintain(db);
            /* A:管理者,不可刪除
             * D:管理者,可刪除
             */
            if (ViewBag.RoleId != "A" && ViewBag.RoleId != "D")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("MemberMaintain", "_LayoutMember", MM.CT_Member().ToPagedList(currentPage, pageSize));
            }
        }


        /// <summary>
        /// 管理者權限編輯使用者畫面
        /// </summary>
        /// <param name="fId"></param>
        /// <returns></returns>
        public ActionResult Edit_Member(int fId)
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            MemberMaintain editMember = new MemberMaintain(db);

            return View(editMember.Edit(fId));
        }

        /// <summary>
        /// 管理者權限編輯使用者
        /// </summary>
        /// <param name="fUserId">員工編號</param>
        /// <param name="fName">姓名</param>
        /// <param name="ROLE_ID">角色ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit_Member(string fUserId, string fName, string ROLE_ID)
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            MemberMaintain editMember = new MemberMaintain(db);

            editMember.Edit(fUserId, fName, ROLE_ID);

            return RedirectToAction("MemberMaintain");
        }

        /// <summary>
        /// 管理者權限停用使用者-變更狀態
        /// </summary>
        /// <param name="fId"></param>
        /// <returns></returns>
        public ActionResult Del_Member(int fId)
        {
            MemberMaintain delMember = new MemberMaintain(db);

            delMember.Del(fId);

            return RedirectToAction("MemberMaintain");
        }

        /// <summary>
        /// 導向變更密碼頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit_Password()
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            MemberMaintain editMember = new MemberMaintain(db);

            int fId = (Session["Member"] as MemberViewModels).fId;

            return View(editMember.Password(fId));
        }

        /// <summary>
        /// 管理者權限變更密碼畫面
        /// </summary>
        /// <param name="fId">KEY</param>
        /// <param name="fPwd">舊密碼</param>
        /// <param name="fNPwd">新密碼</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit_Password(int fId, string fPwd, string fNPwd)
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            MemberMaintain editMember = new MemberMaintain(db);

            if (editMember.Password(fId, fPwd, fNPwd))
            {
                ViewBag.msg = "密碼變更完成!!";
                return View("Edit_Password", editMember.Password(fId));
            }
            else
            {
                ViewBag.msg = "新舊密碼輸入錯誤，請再次確認!!";
                return View("Edit_Password", editMember.Password(fId));
            }
        }

        /// <summary>
        /// 導向建立人員頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateMember()
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            return View("CreateMember", "_LayoutMember");
        }

        /// <summary>
        /// 管理者權限建立人員
        /// </summary>
        /// <param name="fUserId">員工編號</param>
        /// <param name="fName">姓名</param>
        /// <param name="ROLE_ID">角色ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateMember(string fUserId, string fName, string ROLE_ID)
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            MemberMaintain create = new MemberMaintain(db);
            if (!create.Create(fUserId, fName, ROLE_ID).ErrMsg)
            {
                ViewBag.msg = "該使用者代號已存在，請輸入不同的代號";
                return View("CreateMember", "_LayoutMember");
            }
            return RedirectToAction("MemberMaintain"); //導向主頁
        }

        #region Login 驗證相關Class
        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <returns></returns>
        public bool Login_Authentication()
        {
            if (Session["Member"] != null)
            {
                string UserId = (Session["Member"] as MemberViewModels).fUserId;//員工編號
                string RoleId = (Session["Member"] as MemberViewModels).ROLE_ID;//角色ID
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