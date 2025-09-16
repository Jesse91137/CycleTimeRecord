using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CycleTimeRecord.Models;
using CycleTimeRecord.ViewModels;

namespace CycleTimeRecord.Models
{
    /// <summary>
    /// 員工登入
    /// </summary>
    public class CT_MemberLogin
    {
        private readonly CycleTimeRecordEntities db;
        /// <summary>
        /// 員工登入
        /// </summary>
        /// <param name="dbContext"></param>
        public CT_MemberLogin(CycleTimeRecordEntities dbContext)
        {
            db = dbContext;
        }
        /// <summary>
        /// 帳號
        /// </summary>
        public string UId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UName { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string UPwd { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string URole { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        public string URoleDesc { get; set; }
        /// <summary>
        /// 員工狀態
        /// </summary>
        public class MemberResult
        {
            /// <summary>
            /// 姓名
            /// </summary>
            public string FName { get; set; }
            /// <summary>
            /// 員工ViewModel
            /// </summary>
            public MemberViewModels Member { get; set; }
        }
        /// <summary>
        /// 員工登入
        /// </summary>
        /// <returns></returns>
        public MemberResult MemberLogin()
        {
            try
            {
                //var member = db.E_Member.Where(m => m.fUserId == fUserId && m.fPwd == fPwd).FirstOrDefault();
                using (var context = new CycleTimeRecordEntities())
                {
                    var member = (from a in context.CT_Member /*員工*/
                                  join b in context.CT_MemberRole on a.fUserId equals b.USER_ID into roleJoin /*角色*/
                                  from c in roleJoin.DefaultIfEmpty()
                                  where a.fUserId == UId && a.fPwd == UPwd && a.fStatus == true
                                  orderby a.fUserId
                                  select new MemberViewModels
                                  {
                                      fUserId = a.fUserId,
                                      fName = a.fName,
                                      fId=a.fId,
                                      ROLE_ID = c != null ? c.ROLE_ID : null                                      
                                  }).FirstOrDefault();

                    //若member為null表示尚未註冊
                    //if (member == null)
                    //{
                    //    ViewBag.Message = "帳號密碼錯誤，請重新登入";
                    //    return View();
                    //}
                    ////使用session變數記錄歡迎詞
                    //Session["WelCome"] = "員工 : " + member.fName;
                    ////使用session變數紀錄登入會員物件
                    //Session["Member"] = member;

                    return new MemberResult { Member = member };
                }
            }
            catch (Exception m)
            {
                return new MemberResult { Member = null };
            }
        }
    }
}