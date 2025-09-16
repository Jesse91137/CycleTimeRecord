using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CycleTimeRecord.Models;
namespace CycleTimeRecord.ViewModels
{
    
    /// <summary>
    /// 員工ViewModel
    /// </summary>
    public class MemberViewModels
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        public int fId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string fUserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string fName { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        public string ROLE_DESC { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string ROLE_ID { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string fPwd { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public string fStatus { get; set; }
    }

    /// <summary>
    /// 人員名單建立使用
    /// </summary>
    public class MemberCreateViewModels
    {        
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public bool ErrMsg { get; set; }
        /// <summary>
        /// 員工ID
        /// </summary>
        public string fUserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string fName { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        public string ROLE_DESC { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string ROLE_ID { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string fPwd { get; set; }
    }

}