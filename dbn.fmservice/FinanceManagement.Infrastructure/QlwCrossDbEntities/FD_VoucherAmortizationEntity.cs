using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_VoucherAmortizationODataEntity : OneWithManyQueryEntity<FD_VoucherAmortizationDetailODataEntity>
    {
        public FD_VoucherAmortizationODataEntity()
        {
            Lines = new List<FD_VoucherAmortizationDetailODataEntity>();
            PeriodLines = new List<FD_VoucherAmortizationPeriodDetailODataEntity>();
        }
        [NotMapped]
        public List<FD_VoucherAmortizationDetailODataEntity> LinesExtend { get; set; }
        [Key]
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
        public string AmortizationName { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 业务摘要
        /// </summary>
        public string AbstractID { get; set; }
        public string AbstractName { get; set; }
        
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string AccountDate { get; set; }
        /// <summary>
        /// 总摊销期间
        /// </summary>
        public int QuantityTotal { get; set; }
        /// <summary>
        /// 已摊销期间
        /// </summary>
        public int QuantityAlready { get; set; }
        /// <summary>
        /// 未摊销期间
        /// </summary>
        public int QuantityFuture { get; set; }
        /// <summary>
        /// 总摊销金额
        /// </summary>
        public decimal AmountTotal { get; set; }
        /// <summary>
        /// 已摊销金额
        /// </summary>
        public decimal AmountAlready { get; set; }
        /// <summary>
        /// 未摊销金额
        /// </summary>
        public decimal AmountFuture { get; set; }
        /// <summary>
        /// 本期摊销金额
        /// </summary>
        public decimal CurrentAmount { get; set; }
        /// <summary>
        /// 执行状态
        /// </summary>
        public int ImpStateID { get; set; }
        /// <summary>
        /// 执行状态名称
        /// </summary>
        public string ImpState { get; set; }
        /// <summary>
        /// 使用状态
        /// </summary>
        public bool IsUse { get; set; }
        public string UseState { get; set; }
        /// <summary>
        /// 禁用人
        /// </summary>
        public string OperatorID { get; set; }
        public string OperatorName { get; set; }
        [NotMapped]
        public List<FD_VoucherAmortizationPeriodDetailODataEntity> PeriodLines { get; set; }
    }

    public class FD_VoucherAmortizationDetailODataEntity
    {
      
        [Key]
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
        public string AccoSubjectID { get; set; }
        public string AccoSubjectName { get; set; }
        public bool IsPerson { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsMarket { get; set; }
        public bool IsSupplier { get; set; }
        public string PersonID { get; set; }
        public string CustomerID { get; set; }
        public string MarketID { get; set; }
        public string SupplierID { get; set; }
        public string PersonName { get; set; }
        public string CustomerName { get; set; }
        public string MarketName { get; set; }
        public string SupplierName { get; set; }
        public decimal ValueNumber { get; set; }
        public bool IsDebit { get; set; }
        public string ModifiedDate { get; set; }
    }
    public class FD_VoucherAmortizationPeriodDetailODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// 模板表体流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 模板表体流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        public int RowNum { get; set; }
        public string AccountDate { get; set; }
        public decimal AmortizationAmount { get; set; }
        public bool IsAmort { get; set; }
        public bool IsLast { get; set; }
        public string ModifiedDate { get; set; }
    }
    public class FM_VoucherAmortizationRelatedODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrderVoucher { get; set; }
        public string NumericalOrderSettl { get; set; }
        public string NumericalOrderStay { get; set; }
        public string NumericalOrderInto { get; set; }
        public decimal VoucherAmount { get; set; }
        public string ModifiedDate { get; set; }
        public string CreatedDate { get; set; }
    }


    public class FM_AccocheckoODataEntity
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string DataDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Remarks { get; set; }
    }

}
