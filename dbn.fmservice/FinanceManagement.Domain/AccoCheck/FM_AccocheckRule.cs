using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FM_AccoCheckRule : EntityBase
    {
        public FM_AccoCheckRule(string EnterpriseID, string AccoCheckType, string MasterDataSource, string MasterFormula, string MasterSecFormula, string FollowDataSource, string FollowFormula, string FollowSecFormula,
            string CheckValue, string OwnerID, bool IsUse)
        {
            this.EnterpriseID = EnterpriseID;
            this.AccoCheckType = AccoCheckType;
            this.MasterDataSource = string.IsNullOrEmpty(MasterDataSource) ? "0" : MasterDataSource;
            this.MasterFormula = MasterFormula;
            this.MasterSecFormula = MasterSecFormula;
            this.FollowDataSource = string.IsNullOrEmpty(FollowDataSource) ? "0" : FollowDataSource;
            this.FollowFormula = FollowFormula;
            this.FollowSecFormula = FollowSecFormula;
            this.CheckValue = CheckValue;
            this.OwnerID = OwnerID;
            this.IsUse = IsUse;
            this.ModifiedDate = DateTime.Now;
            this.CreatedDate = DateTime.Now;
        }
        public void Update(string EnterpriseID, string AccoCheckType, string MasterDataSource, string MasterFormula, string MasterSecFormula, string FollowDataSource, string FollowFormula, string FollowSecFormula,
            string CheckValue, string OwnerID, bool IsUse)
        {
            this.EnterpriseID = EnterpriseID;
            this.AccoCheckType = AccoCheckType;
            this.MasterDataSource = string.IsNullOrEmpty(MasterDataSource) ? "0" : MasterDataSource;
            this.MasterFormula = MasterFormula;
            this.MasterSecFormula = MasterSecFormula;
            this.FollowDataSource = string.IsNullOrEmpty(FollowDataSource) ? "0" : FollowDataSource;
            this.FollowFormula = FollowFormula;
            this.FollowSecFormula = FollowSecFormula;
            this.CheckValue = CheckValue;
            this.OwnerID = OwnerID;
            this.IsUse = IsUse;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public long RecordID { get; set; }
        public string EnterpriseID { get; set; }
        public string AccoCheckType { get; set; }
        public string MasterDataSource { get; set; }
        public string MasterFormula { get; set; }
        public string MasterSecFormula { get; set; }
        public string FollowDataSource { get; set; }
        public string FollowFormula { get; set; }
        public string FollowSecFormula { get; set; }
        public string CheckValue { get; set; }
        public string OwnerID { get; set; }
        public bool IsUse { get; set; }
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
