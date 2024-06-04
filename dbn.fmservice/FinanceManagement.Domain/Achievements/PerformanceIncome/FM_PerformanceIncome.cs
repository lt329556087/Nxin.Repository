using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FM_PerformanceIncome : EntityBase
    {
        public FM_PerformanceIncome()
        {
           
        }
        [Key]
        public int RecordID { get; set; }
        public string ProductGroupID { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductGroupTypeName { get; set; }
        public string IncomeTypeName { get; set; }
        public string ParentTypeName { get; set; }
        public string PropertyName { get; set; }
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
