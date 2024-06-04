using Architecture.Common.Application.Query;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_PaymentReceivablesEntity : OneWithManyQueryEntity<FD_PaymentReceivablesDetailEntity>
    {

        [Key]
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 审核人ID
        /// </summary>
        public string CheckedByID { get; set; }
        /// <summary>
        /// 审核人姓名
        /// </summary>
        public string AuditName { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 单据字号名称
        /// </summary>
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 会计凭证 单据号（收付款专用）
        /// </summary>
        public string VoucherNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 附件数
        /// </summary>
        public long? AttachmentNum { get; set; }
        /// <summary>
        /// 摘要名
        /// </summary>
        public string ReceiptAbstractName { get; set; }
        public string ReceiptAbstractId { get; set; }
        /// <summary>
        /// 支出内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 收款单位
        /// </summary>
        public string CollectionName { get; set; }
        public string CollectionId { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerName { get; set; }
        public decimal? Amount { get; set; }

        /// <summary>
        /// 借方科目
        /// </summary>
        public string DebitAccoSubjectID { get; set; }
        public bool? IsGroupPay { get; set; }
        public string ProjectID { get; set; }
        public string ProductID { get; set; }
        public string UploadInfo { get; set; }
        public string BusinessType { get; set; }
        public string BusinessTypeName { get; set; }
        public string ApplyNumericalOrder { get; set; }
        [NotMapped]
        /// <summary>
        /// 商城关系 前端利用该属性控制
        /// </summary>
        public string ApplyScNumericalOrder { get; set; }
        public string ApplyAppId { get; set; }
        /// <summary>
        /// 可发起支付的数量
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 总发起支付数量
        /// </summary>
        public int? PayCount { get; set; }
        /// <summary>
        /// 收款单回单地址
        /// </summary>
        public string BankUrl { get; set; }
        [NotMapped]
        public List<RelatedList> RelatedList { get; set; }
        [NotMapped]
        public dynamic Detail { get; set; }
        /// <summary>
        /// true=退回，false=正常业务单据
        /// </summary>
        public bool? IsPayBack { get; set; }
        /// <summary>
        /// 支付状态 0：未发起，1成功，2失败，3处理中
        /// </summary>
        public int PayStatus { get; set; }
        /// <summary>
        /// 列表增加 是否复核属性
        /// </summary>
        public int IsRecheck { get; set; }
        #region 自定义辅助项
        public string Auxiliary1 { get; set; }
        public string Auxiliary2 { get; set; }
        public string Auxiliary3 { get; set; }
        public string Auxiliary4 { get; set; }
        public string Auxiliary5 { get; set; }
        public string Auxiliary6 { get; set; }
        public string Auxiliary7 { get; set; }
        public string Auxiliary8 { get; set; }
        public string Auxiliary9 { get; set; }
        public string Auxiliary10 { get; set; }
        #endregion
    }
    public class FD_ReceivablesEntity
    {

        [Key]
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 审核人ID
        /// </summary>
        public string CheckedByID { get; set; }
        /// <summary>
        /// 审核人姓名
        /// </summary>
        public string AuditName { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 单据字号名称
        /// </summary>
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 会计凭证 单据号（收付款专用）
        /// </summary>
        public string VoucherNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 附件数
        /// </summary>
        public long? AttachmentNum { get; set; }
        /// <summary>
        /// 摘要名
        /// </summary>
        public string ReceiptAbstractName { get; set; }
        public string ReceiptAbstractId { get; set; }
        /// <summary>
        /// 支出内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 收款单位
        /// </summary>
        public string CollectionName { get; set; }
        public string CollectionId { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerName { get; set; }
        public decimal? Amount { get; set; }

        /// <summary>
        /// 借方科目
        /// </summary>
        public string DebitAccoSubjectID { get; set; }
        public bool? IsGroupPay { get; set; }
        public string ProjectID { get; set; }
        public string ProductID { get; set; }
        public string UploadInfo { get; set; }
        public string BusinessType { get; set; }
        public string BusinessTypeName { get; set; }
        public string ApplyNumericalOrder { get; set; }
        [NotMapped]
        /// <summary>
        /// 商城关系 前端利用该属性控制
        /// </summary>
        public string ApplyScNumericalOrder { get; set; }
        public string ApplyAppId { get; set; }
        /// <summary>
        /// 可发起支付的数量
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 总发起支付数量
        /// </summary>
        public int? PayCount { get; set; }
        /// <summary>
        /// 收款单回单地址
        /// </summary>
        public string BankUrl { get; set; }
        [NotMapped]
        public List<RelatedList> RelatedList { get; set; }
        [NotMapped]
        public dynamic Detail { get; set; }
        /// <summary>
        /// true=退回，false=正常业务单据
        /// </summary>
        public bool? IsPayBack { get; set; }
        /// <summary>
        /// 支付状态 0：未发起，1成功，2失败，3处理中
        /// </summary>
        public int PayStatus { get; set; }
        /// <summary>
        /// 列表增加 是否复核属性
        /// </summary>
        public int IsRecheck { get; set; }
        #region 自定义辅助项
        public string Auxiliary1 { get; set; }
        public string Auxiliary2 { get; set; }
        public string Auxiliary3 { get; set; }
        public string Auxiliary4 { get; set; }
        public string Auxiliary5 { get; set; }
        public string Auxiliary6 { get; set; }
        public string Auxiliary7 { get; set; }
        public string Auxiliary8 { get; set; }
        public string Auxiliary9 { get; set; }
        public string Auxiliary10 { get; set; }
        public string Auxiliary { get; set; }
        #endregion
    }
    /// <summary>
    /// 只用于移动支付查询
    /// </summary>
    public class FD_PaymentReceivablesMobileEntity
    {

        [Key]
        public string NumericalOrder { get; set; }

        public Guid Guid { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 单据字号名称
        /// </summary>
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        public string VoucherNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 附件数
        /// </summary>
        public long? AttachmentNum { get; set; }
        /// <summary>
        /// 摘要名
        /// </summary>
        public string ReceiptAbstractName { get; set; }
        public string ReceiptAbstractId { get; set; }
        /// <summary>
        /// 支出内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 收款单位
        /// </summary>
        public string CollectionName { get; set; }
        public string CollectionId { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerName { get; set; }
        public decimal? Amount { get; set; }

        /// <summary>
        /// 借方科目
        /// </summary>
        public string DebitAccoSubjectID { get; set; }
        public bool IsGroupPay { get; set; }
        public string ProjectID { get; set; }
        public string ProductID { get; set; }
        public string UploadInfo { get; set; }
        public string BusinessType { get; set; }
        public string BusinessTypeName { get; set; }
        public string ApplyNumericalOrder { get; set; }
        [NotMapped]
        /// <summary>
        /// 商城流水号 前端利用此属性 判断是否锁定单据
        /// </summary>
        public string ApplyScNumericalOrder { get; set; }
        public string ApplyAppId { get; set; }
        /// <summary>
        /// 可发起支付的数量
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 总发起支付数量
        /// </summary>
        public int PayCount { get; set; }
        /// <summary>
        /// 收款单回单地址
        /// </summary>
        public string BankUrl { get; set; }
        [NotMapped]
        public List<RelatedList> RelatedList { get; set; }
        [NotMapped]
        public dynamic Detail { get; set; }
        /// <summary>
        /// true=退回，false=正常业务单据
        /// </summary>
        public bool? IsPayBack { get; set; }
        /// <summary>
        /// 支付状态 0：未发起，1成功，2失败，3处理中
        /// </summary>
        public int PayStatus { get; set; }
        public string ProjectName { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// 复核数量（汇总数）
        /// </summary>
        public int IsRecheck { get; set; }
        #region 自定义辅助项
        public string Auxiliary1 { get; set; }
        public string Auxiliary2 { get; set; }
        public string Auxiliary3 { get; set; }
        public string Auxiliary4 { get; set; }
        public string Auxiliary5 { get; set; }
        public string Auxiliary6 { get; set; }
        public string Auxiliary7 { get; set; }
        public string Auxiliary8 { get; set; }
        public string Auxiliary9 { get; set; }
        public string Auxiliary10 { get; set; }
        #endregion
        /// <summary>
        /// 科目 判断是否必填 辅助项
        /// </summary>
        public string Auxiliary { get; set; }
    }
    /// <summary>
    /// 销售单集合
    /// </summary>
    public class RelatedList
    {
        [Key]
        public string RelatedID { get; set; }
        public string RecordId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApplyNumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Paid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Payable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Payment { get; set; }
        [NotMapped]
        public string SettleReceipType { get; set; }
    }
    public class FD_PaymentReceivablesDetailEntity
    {
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 关系流水
        /// </summary>
        public string ApplyNumericalOrder { get; set; }
        /// <summary>
        /// 费用科目
        /// </summary>
        [NotMapped]
        public string CostAccoSubjectID { get; set; }
        public Guid Guid { get; set; }
        public string BusinessType { get; set; }
        public string ReceiptAbstractID { get; set; }
        public string PaymentTypeID { get; set; }
        public string AccountID { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        public string MarketID { get; set; }
        public string EnterpriseID { get; set; }
        public decimal? Amount { get; set; }
        public string Content { get; set; }
        public int? AttachCount { get; set; }
        public decimal? Charges { get; set; }
        public string AccoSubjectID { get; set; }
        public string OrganizationSortID { get; set; }
        [NotMapped]
        public List<RelatedList> RelatedList { get; set; } = new List<RelatedList>();
        public string PersonName { get; set; }
        public string MarketName { get; set; }
        public string AccountName { get; set; }
    }
    /// <summary>
    /// 获取费用科目专用()
    /// </summary>
    public class FD_ReceivablesDetailEntity
    {
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 费用科目
        /// </summary>
        [NotMapped]
        public string CostAccoSubjectID { get; set; }
        public Guid Guid { get; set; }
        public string BusinessType { get; set; }
        public string ReceiptAbstractID { get; set; }
        public string PaymentTypeID { get; set; }
        public string AccountID { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        public string MarketID { get; set; }
        public string EnterpriseID { get; set; }
        public decimal? Amount { get; set; }
        public string Content { get; set; }
        public int? AttachCount { get; set; }
        public decimal? Charges { get; set; }
        public string AccoSubjectID { get; set; }
        public string OrganizationSortID { get; set; }
    }
    public class FD_PaymentReceivablesSummaryDetailEntity
    {
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public Guid? Guid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CollectionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccountName { get; set; }
        public string BankDeposit { get; set; }
        public string BankAccount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Amount { get; set; }
        public decimal? Charges { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrganizationSortID { get; set; }
        [NotMapped]
        public List<RelatedList> RelatedList { get; set; } = new List<RelatedList>();
        public string CollectionName { get; set; }
        public string ProductName { get; set; }
        public string ProjectName { get; set; }
        public string ReceiptAbstractName { get; set; }

    }
    public partial class FD_PaymentExtendEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string CollectionName { get; set; }
        public string NumericalOrder { get; set; }
        public string PersonID { get; set; }
        public string CollectionId { get; set; }
        public string AccountName { get; set; }
        public string BankDeposit { get; set; }
        public string BankAccount { get; set; }
        public decimal Amount { get; set; }
        public string BankNumber { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BO_ID { get; set; }
        public string PayResult { get; set; }
        public int Status { get; set; }
        /// <summary>
        /// 账户属性（0：个人，1：公司）
        /// </summary>
        public bool? AccountNature { get; set; }
        public bool? IsRecheck { get; set; }
        public string Number { get; set; }
        public string TicketedPointName { get; set; }
        public string RecheckId { get; set; }
        public string RecheckName { get; set; }
        public string TradeNo { get; set; }
    }
    public partial class FD_PaymentExtendSummaryEntity
    {
        [Key]
        public int RecordID { get; set; }
        public Guid Guid { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Amount { get; set; }
        [NotMapped]
        public decimal? Charges { get; set; }
        public string AccountID { get; set; }
        public int Status { get; set; }
        public bool? IsRecheck { get; set; }
        [NotMapped]
        public string CollectionId { get; set; }
        /// <summary>
        /// 费用科目
        /// </summary>
        /// 
        [NotMapped]
        public string CostAccoSubjectID { get; set; }
        public string AccountName { get; set; }
        public string TradeNo { get; set; }
    }

    /// <summary>
    /// 获取真实金额
    /// </summary>
    public partial class FD_PaymentExtendSummaryEntityByAmount
    {
        [Key]
        public int RecordID { get; set; }
        public Guid Guid { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Amount { get; set; }
        public decimal? Charges { get; set; }
        public string AccountID { get; set; }
        public int Status { get; set; }
        public string CollectionId { get; set; }
    }
    public class FD_PaymentDoMain:FD_PaymentReceivablesEntity
    {
        public string ProductName { get; set; }
        public string ProjectName { get; set; }
        #region 自定义辅助项
        public string Auxiliary1 { get; set; }
        public string Auxiliary2 { get; set; }
        public string Auxiliary3 { get; set; }
        public string Auxiliary4 { get; set; }
        public string Auxiliary5 { get; set; }
        public string Auxiliary6 { get; set; }
        public string Auxiliary7 { get; set; }
        public string Auxiliary8 { get; set; }
        public string Auxiliary9 { get; set; }
        public string Auxiliary10 { get; set; }
        #endregion
        public string Auxiliary { get; set; }
        public List<FD_PaymentReceivablesDetailEntity> details { get; set; }
        public List<FD_PaymentExtendEntity> extend { get; set; }
    }
    public class PaymentList
    {
        [Key]
        public int RecordId { get; set; }
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string PayBank { get; set; }
        public string BankNumber { get; set; }
        public decimal Amount { get; set; }
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string CollectionBank { get; set; }
        /// <summary>
        /// 支付结果
        /// </summary>
        public string PayResult { get; set; }
        /// <summary>
        /// 收方银行卡号
        /// </summary>
        public string BankAccount { get; set; }
        public int Status { get; set; }
        /// <summary>
        /// 收方id
        /// </summary>
        public int extendRecordID { get; set; }
        public string PayUse { get; set; }
        public int TransferType { get; set; }
        /// <summary>
        /// 是否复核
        /// </summary>
        public bool IsRecheck { get; set; }
        /// <summary>
        /// 统计是否汇总流水号（薪资支出专用）
        /// </summary>
        [NotMapped]
        public string ExtendRecordIdCount { get; set; }
    }
    /// <summary>
    /// 对外查询支付结果
    /// </summary>
    public class PaymentOpenList
    {
        [Key]
        public int RecordId { get; set; }
        public bool IsGroupPay { get; set; }
        public string NumericalOrder { get; set; }
        public string TradeNo { get; set; }
        public string ffAccountID { get; set; }
        public string ffAccountName { get; set; }
        public string ffDepositBank { get; set; }
        public string sfBankNumber { get; set; }
        public string sfAccountName { get; set; }
        public string sfPersonName { get; set; }
        public decimal Amount { get; set; }
        public string PayResult { get; set; }
        public string sfStatus { get; set; }
        public string PayUse { get; set; }
    }
    /// <summary>
    /// 人员信息
    /// </summary>
    public class HrInfo
    {
        [Key]
        public string PersonID { get; set; }
        public string Name { get; set; }
        public string BO_ID { get; set; }
    }
    public class RecheckPaymentList
    {
        [Key]
        public string rrid { get; set; }
        public int RecordId { get; set; }
        public string DataDate { get; set; }
        public string Number { get; set; }
        public string EnterpriseName { get; set; }
        public string EnterpriseID { get; set; }
        public string RecheckName { get; set; }
        public string RecheckPhoto { get; set; }
        public string RecheckId { get; set; }
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string PayBank { get; set; }
        public string AccountNumber { get; set; }
        public decimal? Amount { get; set; }
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string CollectionBank { get; set; }
        /// <summary>
        /// 支付结果
        /// </summary>
        public string PayResult { get; set; }
        /// <summary>
        /// 收方银行卡号
        /// </summary>
        public string BankAccount { get; set; }
        public int Status { get; set; }
        /// <summary>
        /// 收方id
        /// </summary>
        public int extendRecordID { get; set; }
        public string PayUse { get; set; }
        public int TransferType { get; set; }
        /// <summary>
        /// 201611180104402202 = 付款单，201611180104402204=汇总单
        /// </summary>
        public string SettleReceipType { get; set; }
        public string NumericalOrder { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        public int RawLevel { get; set; }
        /// <summary>
        /// 复核 驳回 状态
        /// </summary>
        public int RStatus { get; set; }
        /// <summary>
        /// 通过人员id判断是否为阶段流程数据  不是阶段流程数据 则走 老逻辑处理数据
        /// </summary>
        public string rPersonId { get; set; }
        /// <summary>
        /// 通过人员名称，复核人为空时候 显示这个，复核人显示正常来说 是最重付款人
        /// </summary>
        public string rPersonName { get; set; }
        /// <summary>
        /// 查看未复核人数 == 1 时 点击复核=支付
        /// </summary>
        public int NoReviewPerson { get; set; }
    }
    public class Enterprise
    {
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
    }
    /// <summary>
    /// 获取权限单位专用
    /// </summary>
    public class EnterpriseByPower
    {
        public string EnterpriseID { get; set; }
        public string EnterpriseFullName { get; set; }
        public string EnterpriseName { get; set; }
        public int IsUse { get; set; }
    }
    public class ApprovalSetDetail
    {
        [Key]
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 审批条件
        /// </summary>
        public string ApprovalCondition { get; set; }
    }
    /// <summary>
    /// 获取申请人信息发送消息通知
    /// </summary>
    public class ApplyData
    {
        [Key]
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public string cText { get; set; }
        public string MenuID { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string DataDate { get; set; }
        public string EnterpriseName { get; set; }
        public string NumericalOrder { get; set; }
    }
    public class BankReceivablesEntity
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string transIndex { get; set; }
        public string dataSource { get; set; }
        public string bankSerial { get; set; }
        public decimal? amount { get; set; }
        public string receiveDay { get; set; }
        public string entId { get; set; }
        public string entName { get; set; }
        public string acctIndex { get; set; }
        public string AccountName { get; set; }
        public string AccountId { get; set; }
        public string acctNo { get; set; }
        public string otherSideName { get; set; }
        public string otherSideAcctIndex { get; set; }
        public string otherSideAcct { get; set; }
        public decimal fee { get; set; }
        public string msgCode { get; set; }
        public string msg { get; set; }
        public string custList { get; set; }
        /// <summary>
        /// 1:已生成,2:未生成,3:自动生成
        /// </summary>
        public int? IsGenerate { get; set; }
        public string Remarks { get; set; }
        public string SourceNum { get; set; }
        public DateTime CreateTime { get; set; }
        public string custName { get; set; }
    }
    public class BankAccountInfo
    {
        /// <summary>
        /// 随机GUID 作为主键
        /// </summary>
        [Key]
        public Guid Guid { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountId { get; set; }
        public string EnterpriseID { get; set; }
    }
    public class custList
    {
        /// <summary>
        /// 
        /// </summary>
        public string custId { get; set; }
        /// <summary>
        /// 曾桐俭
        /// </summary>
        public string custName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string marketId { get; set; }
        /// <summary>
        /// 助农一车间
        /// </summary>
        public string marketName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string boId { get; set; }
        /// <summary>
        /// 猪小智
        /// </summary>
        public string userName { get; set; }
    }
    /// <summary>
    /// 资金主页 收款管理
    /// </summary>
    public class FundReceivablesData
    {
        [Key]
        public Guid Guid { get; set; }
        public string SettleSummaryGroupName { get; set; }
        public string SettleSummaryGroupId { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// 占比
        /// </summary>
        [NotMapped]
        public decimal Ratio { get; set; }
    }
    /// <summary>
    /// 资金主页 收款管理（客户）取自 一级摘要编码 0101
    /// </summary>
    public class FundReceivablesCusData
    {
        [Key]
        public Guid Guid { get; set; }
        public string CustomerName { get; set; }
        public string EnterpriseName { get; set; }
        public decimal Amount { get; set; }
    }
    public class PayResultCount
    {
        [Key]
        public Guid Guid { get; set; }
        public string PayResult { get; set; }
        public int Count { get; set; }
    }
    public class TemplateDataDrop
    {
        [Key]
        /// <summary>
        /// 计划期间
        /// </summary>
        public string PlanPeriod { get; set; }
        public string EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string NumericalOrder { get; set; }
    }
    public class TemplateData
    {
        [Key]
        public string EnterpriseList { get; set; }
    }
    public class TemplateEnterData
    {
        [Key]
        public string EnterpriseName { get; set; }
    }
    /// <summary>
    /// 资金主页-账户个数
    /// </summary>
    public class BankInfoCount
    {
        /// <summary>
        /// 账户id
        /// </summary>
        [Key]
        public string BankId { get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 账户个数
        /// </summary>
        public int BankCount { get; set; }
        /// <summary>
        /// 银行图片地址
        /// </summary>
        public string ImgUrl { get; set; }
    }
    public class CashSweepInfo
    {
        [Key]
        public Guid Guid { get; set; }
        public string DataDate { get; set; }
        public string EnterpriseName { get; set; }
        public string EnterpriseNameDetial { get; set; }
        public string TradeResult { get; set; }
        public string SweepTypeName { get; set; }
        public decimal AutoSweepBalance { get; set; }
        public string AutoTime { get; set; }
        public string SweepDirectionID { get; set; }
    }
    /// <summary>
    /// 付款单 收款退回列表
    /// </summary>
    public class RefundList
    {
        public string NumericalOrder { get; set; }
        public string DataDate { get; set; }
        public string SettleReceipTypeName { get; set; }
        public string Number { get; set; }
        public decimal? Paid { get; set; }
        public decimal? Payment { get; set; }
        public string CollectionIds { get; set; }
        public string CollectionNames { get; set; }
        public string SettleReceipType { get; set; }
        public string EnterpriseID { get; set; }
        [Key]
        public string RecordID { get; set; }
    }
    /// <summary>
    /// 资金主页 查询属性
    /// </summary>
    public class FundsParameter
    {
        public string EnterpriseID { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }
    public class ReceiptAmout
    {
        [Key]
        public string NumericalOrder { get; set; }
        public decimal? Amount { get; set; }
    }
    /// <summary>
    /// 销售单收款单关联
    /// </summary>
    public class SaSalseUnionReceivables
    {
        /// <summary>
        /// biz_relateddetail  主键标识
        /// </summary>
        [Key]
        public string RecordId { get; set; }
        /// <summary>
        /// 销售单单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 总共应收金额
        /// </summary>
        public decimal Payable { get; set; }
        /// <summary>
        /// 已付金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 收款方式ID
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 收款方式名称
        /// </summary>
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankAccount { get; set; }
    }
    /// <summary>
    /// 利用索引 进行SQL查询
    /// </summary>
    public class NumericalOrderData
    {
        [Key]
        public string NumericalOrder { get; set; }
        public bool IsGroupPay { get; set; }
    }
    /// <summary>
    /// 利用索引 进行SQL查询
    /// 会计凭证定制化
    /// </summary>
    public class NumericalOrderVoucherData
    {
        [Key]
        public string NumericalOrder { get; set; }
        public bool IsGroupPay { get; set; }
        public decimal? Amount { get; set; }
        public string ReceiptAbstractName { get; set; }
    }
    /// <summary>
    /// 特殊处理的实体类（列表查询专用）
    /// </summary>
    public class FD_PaymentReceivablesHeadEntity
    {
        /// <summary>
        /// 企业id
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 数字顺序
        /// </summary>
        /// 
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 收款方名称
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// 内部对比使用
        /// </summary>
        public long CollectionId { get; set; }
        public string TicketedPointID { get; set; }
        public decimal? Amount { get; set; }

    }
    /// <summary>
    /// 复核阶段查询实体
    /// -- MaxLevel = 2205231634370000109 && RawLevel = 0  = 提交人信息
    /// MaxLevel = 2205231634370000109 && RawLevel = 999  = 交易结果信息
    /// 当前只支持一个级次一个复核人
    /// </summary>
    public class ReviewFlowPath
    {
        [Key]
        public string rrid { get; set; }
        /// <summary>
        /// 制单人=出纳人
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 最大级次（阶段流程）0=提交人首级，1-x(数字级别) = 复核阶段流程，999=交易末级
        /// </summary>
        public long MaxLevel { get; set; }
        /// <summary>
        /// 当前级次
        /// </summary>
        public long RawLevel { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 复核人名称
        /// </summary>
        public string ReviweName { get; set; }
        
        /// <summary>
        /// 提交了多少/交易成功
        /// </summary>
        public int SuccessCount { get; set; }
        /// <summary>
        /// 驳回了多少/交易失败
        /// </summary>
        public int FailCount { get; set; }
        /// <summary>
        /// 交易中/未复核（复核支付列表点复核）
        /// </summary>
        public int ProcessingCount { get; set; }
        /// <summary>
        /// SuccessCount 最新时间
        /// </summary>
        public string SuccessTime { get; set; }
        /// <summary>
        /// FailCount 最新时间
        /// </summary>
        public string FailTime { get; set; }
        /// <summary>
        /// Processing 最新时间
        /// </summary>
        public string ProcessingTime { get; set; }
        public decimal? SuccessAmount { get; set; }
        public decimal? FailAmount { get; set; }
        public decimal? ProcessingAmount { get; set; }
    }
}
