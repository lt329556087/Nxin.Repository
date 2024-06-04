using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FM_AccoCheck : EntityBase
    {
        public FM_AccoCheck()
        {
           
        }


        public void Update(DateTime DataDate, DateTime StartDate, DateTime EndDate,string Remarks)
        {
            this.DataDate = DataDate;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.Remarks = Remarks;
            this.ModifiedDate=DateTime.Now;
        }
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 结账标识
        /// </summary>
        public bool CheckMark { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
    }
}
