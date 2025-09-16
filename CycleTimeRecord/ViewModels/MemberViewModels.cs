using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CycleTimeRecord.Models;
namespace CycleTimeRecord.ViewModels
{
    public class MemberViewModels
    {
        public int fId { get; set; }
        public string fUserId { get; set; }
        public string fName { get; set; }
        public string ROLE_DESC { get; set; }
        public string ROLE_ID { get; set; }
        public string fPwd { get; set; }
        public string fStatus { get; set; }
    }

    //人員名單建立使用
    public class MemberCreateViewModels
    {        
        public bool ErrMsg { get; set; }
        public string fUserId { get; set; }
        public string fName { get; set; }
        public string ROLE_DESC { get; set; }
        public string ROLE_ID { get; set; }
        public string fPwd { get; set; }
    }

}