﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CycleTimeRecord.Models;
using CycleTimeRecord.ViewModels;

namespace CycleTimeRecord.Models
{
    public class CT_MemberLogin
    {
        private readonly CycleTimeRecordEntities db;
        public CT_MemberLogin(CycleTimeRecordEntities dbContext)
        {
            db = dbContext;
        }
        public string UId { get; set; }
        public string UName { get; set; }
        public string UPwd { get; set; }
        public string URole { get; set; }
        public string URoleDesc { get; set; }
        public class MemberResult
        {
            public string FName { get; set; }
            public MemberViewModels Member { get; set; }
        }
        public MemberResult MemberLogin()
        {
            try
            {
                //var member = db.E_Member.Where(m => m.fUserId == fUserId && m.fPwd == fPwd).FirstOrDefault();
                using (var context = new CycleTimeRecordEntities())
                {
                    var member = (from a in context.CT_Member
                                  join b in context.CT_MemberRole on a.fUserId equals b.USER_ID into roleJoin
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