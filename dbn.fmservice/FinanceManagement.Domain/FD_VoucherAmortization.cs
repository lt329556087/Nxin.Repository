using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FD_VoucherAmortization : EntityBase
    {
        public FD_VoucherAmortization()
        {
            Details = new List<FD_VoucherAmortizationDetail>();
            PeriodDetails = new List<FD_VoucherAmortizationPeriodDetail>();
        }

        public FD_VoucherAmortizationDetail AddDetail(FD_VoucherAmortizationDetail detail)
        {
            Details.Add(detail);
            return detail;
        }
        public FD_VoucherAmortizationPeriodDetail AddPeriodDetail(FD_VoucherAmortizationPeriodDetail detail)
        {
            PeriodDetails.Add(detail);
            return detail;
        }

        public void Update(string AmortizationName, string TicketedPointID, string AbstractID,string Remarks,DateTime ModifiedDate,bool IsUse,string OperatorID)
        {
            this.AmortizationName = AmortizationName;
            this.TicketedPointID = TicketedPointID;
            this.AbstractID = AbstractID;
            this.Remarks = Remarks;
            this.ModifiedDate = ModifiedDate;
            this.IsUse = IsUse;
            this.OperatorID = OperatorID;
        }
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 摊销编码
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string AmortizationName { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 业务摘要
        /// </summary>
        public string AbstractID { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }
        public bool IsUse { get; set; }
        public string OperatorID { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public List<FD_VoucherAmortizationDetail> Details { get; set; }
        public List<FD_VoucherAmortizationPeriodDetail> PeriodDetails { get; set; }
    }

    public class FD_VoucherAmortizationDetail : EntityBase
    {
        public void Add() { }
        public void Update(string AccoSubjectCode, string AccoSubjectID, string PersonID, string CustomerID, string MarketID, string SupplierID, decimal ValueNumber,bool IsDebit,DateTime ModifiedDate)
        {
            this.AccoSubjectCode = AccoSubjectCode;
            this.AccoSubjectID = AccoSubjectID;
            this.PersonID =string.IsNullOrEmpty(PersonID)?"0":PersonID ;
            this.CustomerID = string.IsNullOrEmpty(CustomerID) ? "0" : CustomerID;
            this.MarketID = string.IsNullOrEmpty(MarketID) ? "0" : MarketID;
            this.SupplierID = string.IsNullOrEmpty(SupplierID) ? "0" : SupplierID;
            this.ValueNumber = ValueNumber;
            this.IsDebit = IsDebit;
            this.ModifiedDate = ModifiedDate;
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
        /// 会计科目Code
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectID { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>

        public string MarketID { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { get; set; }
        public decimal ValueNumber { get; set; }
        /// <summary>
        /// 是否待摊销
        /// </summary>
        public bool IsDebit { get; set; }
        public DateTime ModifiedDate { get; set; }
        public FD_VoucherAmortization FD_VoucherAmortization { get; set; }

    }
    public class FD_VoucherAmortizationPeriodDetail : EntityBase
    {
        public void Update(int RowNum, DateTime AccountDate, decimal AmortizationAmount, bool IsAmort, bool IsLast,DateTime ModifiedDate)
        {
            this.RowNum = RowNum;
            this.AccountDate = AccountDate;
            this.AmortizationAmount = AmortizationAmount;
            this.IsAmort = IsAmort;
            this.IsLast = IsLast;
            this.ModifiedDate = ModifiedDate;
        }


        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 明细流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 会计期间
        /// </summary>
        public DateTime AccountDate { get; set; }
        /// <summary>
        /// 摊销金额
        /// </summary>
        public decimal AmortizationAmount { get; set; }
        /// <summary>
        /// 是否摊销
        /// </summary>
        public bool IsAmort { get; set; }
        /// <summary>
        /// 是否最后期间
        /// </summary>
        public bool IsLast { get; set; }
        public DateTime ModifiedDate { get; set; }
        public FD_VoucherAmortization FD_VoucherAmortization { get; set; }

    }
}
