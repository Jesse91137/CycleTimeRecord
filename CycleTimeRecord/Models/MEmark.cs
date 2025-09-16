using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CycleTimeRecord.Models
{
    /// <summary>
    /// ME 確認原因
    /// </summary>
    public class MEmark
    {
        
        public string 試產 { get; set; }
        public string 延伸試產 { get; set; }
        public string 印刷 { get; set; }
        public string 爐子 { get; set; }
        public string 手放 { get; set; }
        public string 點紅膠 { get; set; }
        public string 載具過爐 { get; set; }
        public string 左右架鋁條 { get; set; }
        public string 線內貼條碼 { get; set; }
    }
}