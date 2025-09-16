using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CycleTimeRecord.ViewModels;

namespace CycleTimeRecord.Models
{
    public class MemberMaintain
    {
        /// <summary>
        /// 資料庫
        /// </summary>
        private readonly CycleTimeRecordEntities db;
        /// <summary>
        /// 資料庫
        /// </summary>
        /// <param name="dbContext"></param>
        public MemberMaintain(CycleTimeRecordEntities dbContext)
        {
            db = dbContext;
        }
        /// <summary>
        /// 人員結果
        /// </summary>
        public class MemberResult
        {
            /// <summary>
            /// 錯誤訊息
            /// </summary>
            public bool ErrMsg { get; set; }
            /// <summary>
            /// 員工編號
            /// </summary>
            public string fUserId { get; set; }
            /// <summary>
            /// 人員名單建立使用
            /// </summary>
            public MemberCreateViewModels Member { get; set; }
        }
        /// <summary>
        /// 取得人員資料
        /// </summary>
        /// <returns></returns>
        public List<MemberViewModels> CT_Member()
        {
            using (var context = new CycleTimeRecordEntities())
            {

                var query = from a in context.CT_Member
                            join b in context.CT_MemberRole on a.fUserId equals b.USER_ID into roleJoin
                            from c in roleJoin.DefaultIfEmpty()
                            join d in context.CT_CycleTimeRecord_Role on c.ROLE_ID equals d.ROLE_ID into roleDescJoin
                            from e in roleDescJoin.DefaultIfEmpty()
                            orderby a.fUserId
                            select new MemberViewModels
                            {
                                fId = a.fId,
                                fUserId = a.fUserId,
                                fName = a.fName,
                                ROLE_DESC = e != null ? e.ROLE_DESC : null,
                                ROLE_ID = c != null ? c.ROLE_ID : null,
                                fStatus = (bool)a.fStatus ? "啟用" : "停用"
                            };
                List<MemberViewModels> models = query.ToList();
                return models;
            }
        }

        /// <summary>
        /// 編輯畫面
        /// </summary>
        /// <param name="fId"></param>
        /// <returns></returns>        
        public List<MemberViewModels> Edit(int fId)
        {
            using (var context = new CycleTimeRecordEntities())
            {
                var query = from a in context.CT_Member
                            join b in context.CT_MemberRole on a.fUserId equals b.USER_ID into roleJoin
                            from c in roleJoin.DefaultIfEmpty()
                            join d in context.CT_CycleTimeRecord_Role on c.ROLE_ID equals d.ROLE_ID into roleDescJoin
                            from e in roleDescJoin.DefaultIfEmpty()
                            where a.fId == fId
                            orderby a.fUserId
                            select new MemberViewModels
                            {
                                fId = a.fId,
                                fUserId = a.fUserId,
                                fName = a.fName,
                                ROLE_DESC = e != null ? e.ROLE_DESC : null,
                                ROLE_ID = c != null ? c.ROLE_ID : null
                            };
                List<MemberViewModels> models = query.ToList();
                return models;
            }
        }
        //進入編輯程序
        /// <summary>
        /// 編輯
        /// </summary>
        /// <param name="fUserId"></param>
        /// <param name="fName"></param>
        /// <param name="ROLE_ID"></param>
        public void Edit(string fUserId, string fName, string ROLE_ID)
        {
            var member = db.CT_Member.Where(m => m.fUserId == fUserId).FirstOrDefault();
            //member.fUserId = fUserId;
            member.fName = fName;
            db.SaveChanges();

            //人員權限名單
            var member_Role = db.CT_MemberRole.Where(r => r.USER_ID == fUserId).FirstOrDefault();
            member_Role.USER_ID = fUserId;
            member_Role.ROLE_ID = ROLE_ID;
            member_Role.EXPIRED_DATE = DateTime.Now;
            db.SaveChanges();
        }

        /// <summary>
        /// 取得員工資料
        /// </summary>
        /// <param name="fId">KE</param>
        /// <returns></returns>
        public List<MemberViewModels> Password(int fId)
        {
            using (var context = new CycleTimeRecordEntities())
            {
                var query = from a in context.CT_Member
                            join b in context.CT_MemberRole on a.fUserId equals b.USER_ID into roleJoin
                            from c in roleJoin.DefaultIfEmpty()
                            join d in context.CT_CycleTimeRecord_Role on c.ROLE_ID equals d.ROLE_ID into roleDescJoin
                            from e in roleDescJoin.DefaultIfEmpty()
                            where a.fId == fId
                            orderby a.fUserId
                            select new MemberViewModels
                            {
                                fPwd = a.fPwd,
                                fId = a.fId,
                                fUserId = a.fUserId,
                                fName = a.fName,
                            };
                List<MemberViewModels> models = query.ToList();
                return models;
            }
        }
        //進入密碼變更程序
        public bool Password(int fId, string fPwd, string fNPwd)
        {
            var result = string.CompareOrdinal(fPwd, fNPwd);
            if (result != 0)
            {
                return false;
            }
            else
            {
                var member = db.CT_Member.Where(m => m.fId == fId).FirstOrDefault();
                //member.fUserId = fUserId;
                member.fPwd = fNPwd;
                db.SaveChanges();
                return true;
            }
        }

        //人員新增畫面沒有, 直接進入新增人員程序
        /// <summary>
        /// 新增人員
        /// </summary>
        /// <param name="fUserId">員工編號</param>
        /// <param name="fName">姓名</param>
        /// <param name="ROLE_ID">角色ID</param>
        /// <returns></returns>
        public MemberResult Create(string fUserId, string fName, string ROLE_ID)
        {
            //檢查是否重複
            var repeat = db.CT_Member.Where(u => u.fUserId == fUserId).FirstOrDefault();
            MemberCreateViewModels result = new MemberCreateViewModels();
            if (repeat != null)
            {
                return new MemberResult { ErrMsg = false };
            }
            else
            {
                //人員名單
                CT_Member member = new CT_Member();
                member.fUserId = fUserId;
                member.fPwd = fUserId;
                member.fName = fName;
                member.fStatus = true;
                db.CT_Member.Add(member);
                db.SaveChanges();

                //人員權限名單
                CT_MemberRole member_Role = new CT_MemberRole();
                member_Role.USER_ID = fUserId;
                member_Role.ROLE_ID = ROLE_ID;
                member_Role.EXPIRED_DATE = DateTime.Now;
                db.CT_MemberRole.Add(member_Role);
                db.SaveChanges();
                return new MemberResult { ErrMsg = true };
            }
        }

        //人員刪除
        /// <summary>
        /// 變更狀態
        /// </summary>
        /// <param name="fId"></param>
        public void Del(int fId)
        {
            var member = db.CT_Member.Where(m => m.fId == fId).FirstOrDefault();

            member.fStatus = (member.fStatus == true) ? false : true;//以 true為啟用, false為停用

            db.SaveChanges();
        }
    }
}