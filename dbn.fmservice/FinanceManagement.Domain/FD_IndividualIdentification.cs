using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace FinanceManagement.Domain
{
    public class FD_IndividualIdentification: EntityBase
    {
        public FD_IndividualIdentification()
        {
            DataDate = CreatedDate =ModifiedDate = DateTime.Now;
            EnterpriseID = Number = NumericalOrder = TicketedPointID = OwnerID = "0";
            Lines = new List<FD_IndividualIdentificationDetail>();
        }

        public void AddDetail(FD_IndividualIdentificationDetail detail)
        {
            Lines.Add(detail);
        }


        public void Update(DateTime DataDate,string TicketedPointID,string Remarks)
        {
            this.DataDate = DataDate;
            this.TicketedPointID = TicketedPointID;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        //public string NumericalOrderSetting { get; set; }
        public List<FD_IndividualIdentificationDetail> Lines { get; set; }
        //[NotMapped]
        //public List<FD_IndividualIdentificationExt> Extends { get; set; }
    }

    public class FD_IndividualIdentificationDetail : EntityBase
    {
        public FD_IndividualIdentificationDetail()
        {
            NumericalOrder=IdentificationType = BusinessType = CurrentUnit = "0";
            Amount = 0;
            AgingList = new List<FD_IndividualIdentificationExt>();
        }

        public void Update(int BusiType, string IdentificationType,string BusinessType,string CurrentUnit, decimal Amount,int? DataSourceType,decimal AccrualAmount)
        {
            this.BusiType = BusiType;
            this.IdentificationType = IdentificationType;
            this.Amount = Amount;
            this.BusinessType = BusinessType;
            this.CurrentUnit = CurrentUnit;
            this.ModifiedDate = DateTime.Now;
            this.DataSourceType = DataSourceType;
            this.AccrualAmount = AccrualAmount;
        }

        [Key]
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 业务类型（0：应收账款 1：其他应收款）
        /// </summary>
        public int BusiType { get; set; }
        /// <summary>
        /// 认定类型
        /// </summary>
        public string IdentificationType { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CurrentUnit { get; set; }
        /// <summary>
        /// 期末余额
        /// </summary>

        public decimal Amount { get; set; }
        /// <summary>
        /// 坏账准备金额
        /// </summary>

        public decimal AccrualAmount { get; set; }
        public int? DataSourceType { get; set; }

        public DateTime ModifiedDate { get; set; }        

        public FD_IndividualIdentification FD_IndividualIdentification{ get; set; }

        //[ForeignKey(nameof(NumericalOrderDetail))]
        [NotMapped]
        public List<FD_IndividualIdentificationExt> AgingList { get; set; }
    }

    public class FD_IndividualIdentificationExt : EntityBase
    {
        public FD_IndividualIdentificationExt()
        {
            NumericalOrder  = "0";
        }
        public void Update(int BusiType, string Name,decimal Amount)
        {
            this.BusiType = BusiType;
            this.Name = Name;
            this.Amount = Amount;
            this.ModifiedDate = DateTime.Now;
        }

        [Key]
        public int RecordID { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 业务类型（0：坏账准备 1：账龄区间）
        /// </summary>
        public int BusiType { get; set; }
        /// <summary>
        /// 区间金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 区间名称
        /// </summary>
        public string Name { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
