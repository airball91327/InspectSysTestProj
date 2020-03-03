using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    /// <summary>
    /// API呼叫時，傳回的統一物件
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 執行成功與否
        /// </summary>
        public bool Succ { get; set; }
        /// <summary>
        /// 結果代碼(200=成功，其餘為錯誤代號)
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 資料時間
        /// </summary>
        public DateTime DataTime { get; set; }
        /// <summary>
        /// 資料本體
        /// </summary>
        public object Data { get; set; }

        public ApiResult()
        {
        }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        /// <param name="data"></param>
        public ApiResult(object data)
        {
            Code = "200";
            Succ = true;
            DataTime = DateTime.Now;
            Data = data;
        }

        /// <summary>
        /// 建立失敗結果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ApiResult(string code, string message)
        {
            Code = code;
            Succ = false;
            this.DataTime = DateTime.Now;
            Data = null;
            Message = message;
        }
    }
}