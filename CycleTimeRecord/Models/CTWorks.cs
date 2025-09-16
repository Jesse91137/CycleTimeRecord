using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using System.Data.SqlClient;

namespace CycleTimeRecord.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class CTWorks
    {
        /// <summary>
        /// DB操作
        /// </summary>
        private readonly CycleTimeRecordEntities db;
        /// <summary>
        /// 創建一個QueryServiceAMES.QueryServiceSoapClient對象
        /// 用於與Web服務進行通信，通過SOAP協定發送和接收消息
        /// </summary>        
        QueryServiceAMES.QueryServiceSoapClient Rv = new QueryServiceAMES.QueryServiceSoapClient();
        /// <summary>
        /// 用於與一個Web服務進行通信，該Web服務是通過SOAP協定實現的
        /// </summary>
        TransferServiceAMES.TransferServiceSoapClient Tx = new TransferServiceAMES.TransferServiceSoapClient();
        /// <summary>
        /// 用於與Web服務進行通信，通過SOAP協定發送和接收消息
        /// </summary>
        ProcessServiceAMES.ProcessServiceSoapClient Proc = new ProcessServiceAMES.ProcessServiceSoapClient();

        /// <summary>
        /// 用於與Web服務進行通信，通過SOAP協定發送和接收消息
        /// </summary>
        /// <param name="dbContext"></param>
        public CTWorks(CycleTimeRecordEntities dbContext)
        {
            db = dbContext;
            CT_DataLists = new List<CT_DataList>();
        }
        /// <summary>
        /// 線別
        /// </summary>
        public string LNX { get; set; }
        /// <summary>
        /// 取得CT_DataList資料
        /// </summary>
        public List<CT_DataList> CT_DataLists { get; set; }
        /// <summary>
        /// CT_DataList
        /// </summary>
        public class CT_DataList
        {
            /// <summary>
            /// 
            /// </summary>
            public int Sno { get; set; }
            /// <summary>
            /// 線別
            /// </summary>
            public string Line { get; set; }
            /// <summary>
            /// 機種名稱
            /// </summary>
            public string EngSr { get; set; }
            /// <summary>
            /// 板面
            /// </summary>
            public string T_B { get; set; }
            /// <summary>
            /// 板數
            /// </summary>
            public int BoardSum { get; set; }
            /// <summary>
            /// 瓶頸工時
            /// </summary>
            public decimal BottleneckHours { get; set; }
            /// <summary>
            /// 日期
            /// </summary>
            public DateTime Date { get; set; }
            public double Other { get; set; }
            /// <summary>
            /// 原因
            /// </summary>
            public string Mark { get; set; }
            public double Text1 { get; set; }
            public double Text2 { get; set; }
            public double Text3 { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string fName { get; set; }
            /// <summary>
            /// ME確認原因標記
            /// </summary>
            public string ME_mark { get; set; }
            /// <summary>
            /// ME確認原因補充
            /// </summary>
            public string MEmarkAdd { get; set; }
            /// <summary>
            /// 標準人數
            /// </summary>
            public int OP_Cnt { get; set; }
        }
        public class StdWTimeData
        {
            public string ITEM_NO { get; set; }
            public double CT { get; set; }
            public double TOTAL_CT { get; set; }
            public double FIRST_TIME { get; set; }
            public int OP_CNT { get; set; }
            public double MACHINE_CNT { get; set; }
            public string MEMO { get; set; }
            public string UNIT_NO { get; set; }
            public int LINE_ID { get; set; }
            public int STATION_ID { get; set; }
            public string SIDE { get; set; }
        }
        #region 頁面顯示
        /// <summary>
        /// 標準工時作業下半部資料取得
        /// </summary>
        /// <returns></returns>
        public List<CT_DataList> CT_LineDatas()
        {
            var dataList = from lineWork in db.CT_LineWork
                           join member in db.CT_Member on lineWork.UserID equals member.fUserId into memberJoin
                           from member in memberJoin.DefaultIfEmpty()
                           where lineWork.flag == false
                           orderby lineWork.Date descending
                           select new CT_DataList
                           {
                               Sno = lineWork.sno,
                               Line = lineWork.Line,
                               EngSr = lineWork.EngSr,
                               T_B = lineWork.T_B,
                               BoardSum = (int)lineWork.BoardSum,
                               BottleneckHours = (decimal)lineWork.BottleneckHours,
                               Date = (DateTime)lineWork.Date,
                               Mark = lineWork.Mark,
                               fName = member != null ? member.fName : null,
                               OP_Cnt = (int)lineWork.OP_Cnt,
                               ME_mark = lineWork.MEmark+ lineWork.MEmarkAdd??"",// ME確認原因+ME確認原因補充 20250527 By Jesse 
                               MEmarkAdd =lineWork.MEmarkAdd                               
                           };

            //CT_DataLists.AddRange(dataList.ToList());
            // 建立新 List，不累積之前的資料
            List<CT_DataList> result = dataList.ToList();
            return result;
        }
        #endregion

        #region AJAXQuery 隨查隨顯
        /// <summary>
        /// 線別 & 機種名稱 & 板面 有都值才查詢 - 隨查隨顯
        /// </summary>
        /// <param name="LNX">線別</param>
        /// <param name="engSr">機種名稱</param>
        /// <param name="TB">板面</param>
        /// <returns></returns>
        public List<CT_DataList> CT_LineDataAj(string LNX, string engSr, string TB)
        {
            var datas = db.CT_LineWork.Where(o => o.Line == LNX && o.EngSr == engSr && o.T_B == TB && o.flag == false)
                .OrderByDescending(x => x.Date).ToList();

            foreach (var data in datas)
            {
                CT_DataList ctData = new CT_DataList
                {
                    Line = data.Line,
                    EngSr = data.EngSr,
                    T_B = data.T_B,
                    BoardSum = (int)data.BoardSum,
                    BottleneckHours = (decimal)data.BottleneckHours,
                    Date = (DateTime)data.Date,
                    Text1 = (double)data.Text1,
                    Text2 = (double)data.Text2,
                    Text3 = (double)data.Text3,
                    Other = (double)data.Other,
                    Mark = data.Mark,
                    OP_Cnt = (int)data.OP_Cnt,
                    ME_mark = data.MEmark,
                    MEmarkAdd=data.MEmarkAdd // ME確認原因補充 20250527 By Jesse
                };
                CT_DataLists.Add(ctData);
            }
            return CT_DataLists;
        }
        #endregion

        #region 一般查詢
        /// <summary>
        /// 一般查詢
        /// </summary>
        /// <param name="engSr">機種名稱</param>
        /// <param name="line">線別</param>
        /// <param name="TB">板面</param>
        /// <param name="indate">日期起</param>
        /// <param name="indate2">日期迄</param>
        /// <returns></returns>
        public List<CT_DataList> CT_LineDatas(string engSr, string line, string TB, DateTime? indate, DateTime? indate2)
        {
            var query = db.CT_LineWork
                                .Where(lineWork => lineWork.flag == false);
          
            if (!string.IsNullOrEmpty(engSr))
            {
                query = query.Where(lineWork => lineWork.EngSr == engSr);
            }

            if (!string.IsNullOrEmpty(TB))
            {
                query = query.Where(lineWork => lineWork.T_B == TB);
            }

            if (!string.IsNullOrEmpty(line))
            {
                query = query.Where(lineWork => lineWork.Line == line);
            }

            if (indate.HasValue && indate2.HasValue)
            {
                query = query.Where(lineWork => lineWork.Date >= indate.Value && lineWork.Date <= indate2.Value);                        
            }
         

            var dataList = (from lineWork in query
                            join member in db.CT_Member on lineWork.UserID equals member.fUserId into memberJoin
                            from member in memberJoin.DefaultIfEmpty()
                            orderby lineWork.Date descending
                            select new CT_DataList
                            {
                                Sno = lineWork.sno,
                                Line = lineWork.Line,
                                EngSr = lineWork.EngSr,
                                T_B = lineWork.T_B,
                                BoardSum = (int)lineWork.BoardSum,
                                BottleneckHours = (decimal)lineWork.BottleneckHours,
                                Date = (DateTime)lineWork.Date,
                                Mark = lineWork.Mark,
                                fName = member != null ? member.fName : null,
                                OP_Cnt = (int)lineWork.OP_Cnt,
                                ME_mark = lineWork.MEmark??""+lineWork.MEmarkAdd??"",// ME確認原因+ME確認原因補充 20250527 By Jesse
                            }).ToList();

            //CT_DataLists.AddRange(dataList.ToList());

            return dataList;
        }
        #endregion

        #region CT_Web都用新增變成歷史紀錄
        /// <summary>
        /// 
        /// </summary>
        /// <param name="engsr">機種名稱</param>
        /// <param name="Lnx">線別</param>
        /// <param name="TB">板面</param>
        /// <param name="b_sum">板數</param>
        /// <param name="t1">L3:NXT/L2:FX3/L4:NXT</param>
        /// <param name="t2">L3:XPF1/L2:JUKI50/L4:XPF1</param>
        /// <param name="t3">L3:AIMEX/L2:JUKI80/L4:XPF2</param>
        /// <param name="neckHours">瓶頸工時</param>
        /// <param name="other">其他瓶頸</param>
        /// <param name="mark">工時原因</param>
        /// <param name="Uid">使用者UID</param>
        /// <param name="opcnt">標準人數</param>
        /// <param name="memark">ME確認原因</param>
        /// <param name="MEmarkAdd">ME確認原因補充</param>
        /// <returns></returns>
        public List<CT_DataList> CT_LineDatasInHistory(string engsr, string Lnx, string TB, int b_sum, double t1, double t2, double t3
                                                        , decimal neckHours, double other, string mark, string Uid, int opcnt, string memark, string MEmarkAdd)
        {
            try
            {
                CT_DataList ctDate = new CT_DataList();
                ctDate.Date = DateTime.Now;
                CT_LineWork lineWork = new CT_LineWork();

                lineWork.EngSr = engsr.ToUpper();
                lineWork.Line = Lnx;
                lineWork.T_B = TB;
                lineWork.BoardSum = b_sum;
                lineWork.Text1 = t1;
                lineWork.Text2 = t2;
                lineWork.Text3 = t3;
                lineWork.BottleneckHours = neckHours;
                lineWork.Other = other;
                lineWork.Mark = mark;
                lineWork.Date = ctDate.Date;
                lineWork.UserID = Uid;
                lineWork.OP_Cnt = opcnt;
                lineWork.MEmark = memark;
                lineWork.MEmarkAdd = MEmarkAdd;
                lineWork.flag = false;
                db.CT_LineWork.Add(lineWork);
                db.SaveChanges();

                try
                {
                    var dataList = from lineData in db.CT_LineWork
                                   join member in db.CT_Member on lineWork.UserID equals member.fUserId into memberJoin
                                   from member in memberJoin.DefaultIfEmpty()
                                   where lineData.EngSr == engsr && lineData.T_B == TB
                                   && lineData.Line == Lnx && lineData.flag == false
                                   orderby lineData.Date descending
                                   select new CT_DataList
                                   {
                                       Sno = lineData.sno,
                                       Line = lineData.Line,
                                       EngSr = lineData.EngSr,
                                       T_B = lineData.T_B,
                                       BoardSum = (int)lineData.BoardSum,
                                       BottleneckHours = (decimal)lineData.BottleneckHours,
                                       Date = (DateTime)lineData.Date,
                                       Mark = lineData.Mark,
                                       fName = member != null ? member.fName : null,
                                       OP_Cnt = (int)lineData.OP_Cnt,
                                       ME_mark = lineData.MEmark
                                   };

                    CT_DataLists.AddRange(dataList);

                }
                catch (Exception err)
                {
                    //查詢失敗
                }
            }
            catch (Exception errX)
            {
                //寫入失敗
            }
            return CT_DataLists;
        }
        #endregion

        #region 刪除本地紀錄,
        /// <summary>
        /// 刪除本地紀錄
        /// </summary>
        /// <param name="sno"></param>
        /// <param name="reason"></param>
        /// <param name="Uid"></param>
        public void DeleteCTrecord(int sno, string reason, string Uid)
        {
            var record = db.CT_LineWork.Find(sno);
            if (record != null)
            {
                record.flag = true; /*判斷刪除*/
                record.reason = reason;/*刪除原因*/

                record.UserID = Uid;
                record.Date = DateTime.Now;

                db.SaveChanges();
            }
        }
        #endregion

        #region 更新AMES 1.0 (WHM002)
        /// <summary>
        /// 更新AMES 1.0 (WHM002)
        /// </summary>
        /// <param name="engsr"></param>
        /// <param name="Lnx">線別</param>
        /// <param name="TB"></param>
        /// <returns></returns>
        public DataSet RvStandardID(string engsr, string Lnx, string TB)
        {
            //取得線別對應取得標準工時
            LnxToLxMap.TryGetValue(Lnx, out int Lx);
            TBToSideNameMap.TryGetValue(TB, out string Sidename);
            DataSet ds = Rv.QueryStd_WorkTime(Lx, Sidename, engsr);
            return ds;
        }
        /// <summary>
        /// 更新AMES 1.0
        /// </summary>
        /// <param name="engsr">機種名稱</param>
        /// <param name="Lnx">線別</param>
        /// <param name="TB">板面</param>
        /// <param name="b_sum">板數</param>
        /// <param name="neckHours">瓶頸工時</param>
        /// <param name="other">其他瓶頸工時</param>
        /// <param name="mark">工時原因</param>
        /// <param name="Uid">使用者UID</param>
        /// <param name="op_cnt">標準人數</param>
        /// <returns></returns>
        public async Task TxStdTimeAsync(string engsr, string Lnx, string TB, int b_sum, decimal neckHours, double other, string mark, string Uid, int op_cnt)
        {
            DataSet ds = RvStandardID(engsr, Lnx, TB);
            if (ds.Tables[0].Rows.Count > 0)//Update
            {
                int StdID = Convert.ToInt32(ds.Tables[0].Rows[0]["STANDARD_ID"]);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    StdWTimeData stdWTime = new StdWTimeData
                    {
                        ITEM_NO = row["ITEM_NO"].ToString(),
                        CT = (double)neckHours / 60 / b_sum,
                        TOTAL_CT = row["TOTAL_CT"] is DBNull ? 0.0 : Convert.ToDouble(row["TOTAL_CT"]),
                        FIRST_TIME = row["FIRST_TIME"] is DBNull ? 0.0 : Convert.ToDouble(row["FIRST_TIME"]),
                        OP_CNT = op_cnt,//Convert.ToInt32(row["OP_CNT"]) == 1 ? 4 : Convert.ToInt32(row["OP_CNT"]),
                        MACHINE_CNT = row["MACHINE_CNT"] is DBNull ? 0.0 : Convert.ToDouble(row["MACHINE_CNT"]),
                        MEMO = mark.Trim(),//row["MEMO"].ToString(),
                        LINE_ID = row["LINE_ID"] is DBNull ? 0 : Convert.ToInt32(row["LINE_ID"]),
                        STATION_ID = row["STATION_ID"] is DBNull ? 0 : Convert.ToInt32(row["STATION_ID"]),
                        SIDE = row["SIDE"].ToString(),
                    };

                    #region 範本
                    /*
                     (string txtype,int? stdID, string UNIT_NO, int? LINE_ID, string ITEM_NO, int? OP_CNT, 
                    double? FIRST_TIME, double? TOTAL_CT, double? CT, double? MACHINE_CNT, string SIDE, 
                    int? STATION_ID, string MEMO,string uID)
                     */
                    #endregion
                    try
                    {//TxStandardTime
                        var result = await Tx.TxStandardTimeAsync("Update", StdID, "S", stdWTime.LINE_ID, stdWTime.ITEM_NO, stdWTime.OP_CNT, stdWTime.FIRST_TIME
                        , stdWTime.TOTAL_CT, stdWTime.CT, stdWTime.MACHINE_CNT, stdWTime.SIDE, stdWTime.STATION_ID, stdWTime.MEMO, Uid);
                    }
                    catch (SoapException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }
                }
            }
            else//Insert
            {
                try
                {
                    int stdID = Proc.TxStandardID("JH_STANDARD_WORKTIME");
                    double CT = (double)neckHours / 60 / b_sum;
                    LnxToLxMap.TryGetValue(Lnx, out int Lx);
                    TBToStationIDMap.TryGetValue(TB, out int stationID);

                    var result = await Tx.TxStandardTimeAsync("Insert", stdID, "S", Lx, engsr, op_cnt, 0.0, 0.0, CT, 0.0, TB, stationID, mark, Uid);
                }
                catch (SoapException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }

        //SMD接料帶        266     0.6
        //SMD線外備料    239     0.98
        /// <summary>
        /// 更新AMES 1.0
        /// </summary>
        /// <param name="engsr">機種名稱</param>
        /// <param name="Lnx">線別</param>
        /// <param name="TB">板面</param>
        /// <param name="Uid">板數</param>
        /// <param name="cnt">瓶頸工時</param>
        /// <returns></returns>
        public async Task TxStdTimeAsync(string engsr, string Lnx, string TB, string Uid, int cnt)
        {
            DataSet ds = RvStandardID(engsr, Lnx, TB);
            if (ds.Tables[0].Rows.Count > 0)//Update
            {
            }
            else//Insert
            {
                try
                {
                    double[] CT = { 0.6, 0.98 };
                    LnxToLxMap.TryGetValue("L5", out int Lx);
                    int[] stationID = { 266, 239 };

                    int stdID = Proc.TxStandardID("JH_STANDARD_WORKTIME");
                    await Tx.TxStandardTimeAsync("Insert", stdID, "S", Lx, engsr, 1, 0.0, 0.0, CT[cnt], 1.0, TB, stationID[cnt], "", Uid);

                }
                catch (Exception r)
                {
                    Console.WriteLine(r.Message);
                    throw;
                }
            }
        }

        #endregion

        #region 優化比例
        /// <summary>
        /// 日期起~日期迄 中檢可抽百分比
        /// </summary>
        /// <param name="startDate">日期起</param>
        /// <param name="endDate">日期迄</param>
        /// <returns></returns>
        public double Optimization(string startDate, string endDate)
        {
            double percentage = 0;

            string query = @"
    WITH FilteredData AS (
    SELECT EngSr, line, T_B, OP_Cnt, Date
    FROM (
        SELECT EngSr, line, T_B, OP_Cnt, Date, 
               ROW_NUMBER() OVER (PARTITION BY EngSr, line, T_B ORDER BY Date DESC) AS rn
        FROM CT_LineWork
        WHERE 
            (@StartDate IS NULL AND @EndDate IS NULL)  
            OR (@StartDate IS NULL AND @EndDate IS NOT NULL AND Date = @EndDate)  
            OR (@StartDate IS NOT NULL AND @EndDate IS NULL AND Date = @StartDate)  
            OR (@StartDate IS NOT NULL AND @EndDate IS NOT NULL AND Date BETWEEN @StartDate AND @EndDate)
    ) AS subquery
    WHERE rn = 1  -- 取每組 (EngSr, line, T_B) 的最新記錄
)

SELECT 
    FLOOR((CAST(querySon AS FLOAT) / NULLIF(CAST(queryMonther AS FLOAT), 0)) * 100 * 100) / 100 AS Percentage
FROM 
(
    SELECT 
        (SELECT COUNT(*) FROM FilteredData WHERE OP_Cnt = 3) AS querySon,
        (SELECT COUNT(*) FROM FilteredData WHERE OP_Cnt >= 3) AS queryMonther
) AS subquery";

            try
            {
                // 處理 NULL 日期
                SqlParameter startDateParam = new SqlParameter("@StartDate", SqlDbType.Date);
                SqlParameter endDateParam = new SqlParameter("@EndDate", SqlDbType.Date);

                if (string.IsNullOrEmpty(startDate))
                    startDateParam.Value = DBNull.Value;
                else
                    startDateParam.Value = DateTime.Parse(startDate);

                if (string.IsNullOrEmpty(endDate))
                    endDateParam.Value = DBNull.Value;
                else
                    endDateParam.Value = DateTime.Parse(endDate);

                SqlParameter[] parameters = { startDateParam, endDateParam };

                using (SqlDataReader dr = Db.ExecuteReader(query, CommandType.Text, parameters))
                {
                    if (dr.Read()) // 確保讀取資料
                    {
                        percentage = dr.IsDBNull(0) ? 0.0 : dr.GetDouble(0); // 避免 NULL 例外
                    }
                }

                return percentage;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 0.0; // 或拋出異常
            }
        }

        #endregion

        #region DictionaryMAP
        /// <summary>
        /// 板面
        /// </summary>
        Dictionary<string, string> TBToSideNameMap = new Dictionary<string, string>
        {{ "TOP", "SMT_TOP" },{ "BOT", "SMT_BOT" },};

        //站別
        Dictionary<string, int> TBToStationIDMap = new Dictionary<string, int>
        {{ "TOP", 12 },{ "BOT", 13 },};

        /// <summary>
        /// 線別
        /// </summary>
        Dictionary<string, int> LnxToLxMap = new Dictionary<string, int>
        {{ "L1", 1 },{ "L2", 101 },{ "L3", 102 },{ "L4", 103 },{ "L5",104} };
        #endregion

    }
}