using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public class FA_MarketSubject : EntityBase
    {
        public FA_MarketSubject()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = EnterpriseID = "0";
            Details = new List<FA_MarketSubjectDetail>();
        }

        public void Update(DateTime DataDate,  string Remarks, string EnterpriseID)
        {
            this.DataDate = DataDate;
            this.Remarks = Remarks;
            this.EnterpriseID = EnterpriseID;
            this.ModifiedDate = DateTime.Now;
        }

        public FA_MarketSubjectDetail AddDetail(FA_MarketSubjectDetail detail)
        {
            Details.Add(detail);
            return detail;
        }
               
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }


        public List<FA_MarketSubjectDetail> Details { get; set; }

    }

    public class FA_MarketSubjectDetail : EntityBase
    {

        public void Update(string MarketID,string AccoSubjectID)
        {
            this.MarketID = MarketID;
            this.AccoSubjectID = AccoSubjectID;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }


        /// <summary>
        /// 部门ID
        /// </summary>		
        public string MarketID { get; set; }


        /// <summary>
        /// 科目ID
        /// </summary>		
        public string AccoSubjectID { get; set; }

        public DateTime ModifiedDate { get; set; }


        [ForeignKey("NumericalOrder")]
        public FA_MarketSubject FA_MarketSubject { get; set; }
    }
}
