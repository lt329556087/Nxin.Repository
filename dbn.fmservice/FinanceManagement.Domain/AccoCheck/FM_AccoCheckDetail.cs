using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FM_AccoCheckDetail : EntityBase
    {
        public void Add() { }

        public void Update(string AccoCheckType, bool CheckMark, string OwnerID)
        {
            this.AccoCheckType = AccoCheckType;
            this.CheckMark = CheckMark;
            this.OwnerID = OwnerID;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 结账类型（字典表）
        /// </summary>
        public string AccoCheckType { get; set; }
        /// <summary>
        /// 是否结账（0：未结账，1：已结账）
        /// </summary>
        public bool CheckMark { get; set; }
        public bool IsNew { get; set; }
        /// <summary>
        /// 操作员ID
        /// </summary>
        public string OwnerID { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
