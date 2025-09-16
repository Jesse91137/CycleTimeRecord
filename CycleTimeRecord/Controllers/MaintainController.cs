using CycleTimeRecord.Models;
using CycleTimeRecord.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CycleTimeRecord.Controllers
{
    public class MaintainController : Controller
    {
        CycleTimeRecordEntities db = new CycleTimeRecordEntities();
        int pageSize = 50;
        // GET: Maintain
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MemberMaintain(int page = 1)
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            int currentPage = page < 1 ? 1 : page;
            MemberMaintain MM = new MemberMaintain(db);
            if (ViewBag.RoleId != "A" && ViewBag.RoleId != "D")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("MemberMaintain", "_LayoutMember", MM.CT_Member().ToPagedList(currentPage, pageSize));
            }            
        }

        //管理者權限編輯使用者
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
        
        //停用使用者
        public ActionResult Del_Member(int fId)
        {
            MemberMaintain delMember = new MemberMaintain(db);
            
            delMember.Del(fId);

            return RedirectToAction("MemberMaintain");
        }

        //變更密碼
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

        [HttpPost]
        public ActionResult Edit_Password(int fId,string fPwd,string fNPwd)
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

        public ActionResult CreateMember()
        {
            //取得會員帳號指定fUserId
            if (!Login_Authentication())
            {
                return RedirectToAction("Login", "Home");
            }
            return View("CreateMember", "_LayoutMember");
        }
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
            return RedirectToAction("MemberMaintain");
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