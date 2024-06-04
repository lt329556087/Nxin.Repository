using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables
{
    public class FD_PaymentReceivablesSummaryAddCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {
        public List<SummaryData> details { get; set; }
        public List<SummaryData> extend { get; set; }
    }
    public class FD_PaymentReceivablesSummaryModifyCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {
        public List<SummaryData> details { get; set; }
        public List<SummaryData> extend { get; set; }
    }
    public class FD_PaymentReceivablesAddCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {

    }

    public class FD_PaymentReceivablesDeleteCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {

    }

    public class FD_PaymentReceivablesModifyCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {
    }

    public class FD_ReceivablesAddCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {

    }

    public class FD_ReceivablesDeleteCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {

    }

    public class FD_ReceivablesModifyCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {
    }
    /// <summary>
    /// 上传附件专用
    /// </summary>
    public class FD_ReceivablesUpInfoCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {
    }
    public class FD_ReceivablesSummaryAddCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {
        public List<SummaryData> details { get; set; }
        public List<SummaryData> extend { get; set; }
    }
    public class FD_ReceivablesSummaryModifyCommand : FD_PaymentReceivablesCommand, IRequest<Result>
    {
        public List<SummaryData> details { get; set; }
        public List<SummaryData> extend { get; set; }
    }
    public class FD_PaymentReceivablesLogicCommand : IRequest<Result>
    {
        public string NumericalOrder { get; set; }
        public string failNum { get; set; }
        public string failInfo { get; set; }
        public string PayUse { get; set; }
        public int TransferType { get; set; }
        public bool IsRecheck { get; set; }
        public string RecheckId { get; set; }
        public string TradeNo { get; set; }
        public int RawLevel { get; set; }
        public int Level { get; set; }
        public List<AuditInfomation> AuditList { get; set; }
        public bool IsPay { get; set; } = false;
    }
    public class FD_PaymentReceivablesReviewLogicCommand : IRequest<Result>
    {
        public string RecordId { get; set; }
        public int RawLevel { get; set; }
        public int Level { get; set; }
        /// <summary>
        /// 1=复核，0=驳回
        /// </summary>
        public int Status { get; set; }
    }
    public class AuditInfomation
    {
        /// <summary>
        /// 最大审核级次
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 审核级次
        /// </summary>
        public int RawLevel { get; set; }
        /// <summary>
        /// 人员Id  = UserID
        /// </summary>
        public long PersonID { get; set; }
    }
    public class FD_PaymentReceivablesCancelLogicCommand : IRequest<Result>
    {
        public string NumericalOrder { get; set; }
        public string failNum { get; set; }
        public string failInfo { get; set; }
        public string PayUse { get; set; }
        public int TransferType { get; set; }
        public bool IsRecheck { get; set; }
        public string RecheckId { get; set; }
        public string TradeNo { get; set; }
        public int RawLevel { get; set; }
        public int Level { get; set; }
        public int Status { get; set; }
    }
    public class FD_PaymentReceivablesCommand 
    {
        /// <summary>
        /// 商城申请Appid  前端写死值
        /// 201612070104402204
        /// </summary>
        public string ApplyScAppId { get; set; }
        /// <summary>
        /// 申请Appid
        /// </summary>
        public string ApplyAppId { get; set; }
        /// <summary>
        /// 申请流水号
        /// </summary>
        public string ApplyNumericalOrder { get; set; }
        /// <summary>
        /// 银行流水
        /// </summary>
        public string BankNumericalOrder { get; set; }
        /// <summary>
        /// 排程流水号
        /// </summary>
        public string PlanNumericalOrder { get; set; }
        /// <summary>
        /// 付款单APPID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 凭证类别
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// 附单据张数
        /// </summary>
        public string AttachmentNum { get; set; }
        /// <summary>
        /// 是否推送
        /// </summary>
        public bool IsPush { get; set; }
        /// <summary>
        /// 借方科目
        /// </summary>
        public string DebitAccoSubjectID { get; set; }
        public string UploadInfo { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 申请关系
        /// </summary>
        public List<RelatedLists> RelatedList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FD_PaymentExtend> extend { get; set; }
        public List<FD_PaymentReceivablesDetailCommand> details { get; set; } = new List<FD_PaymentReceivablesDetailCommand>();
        /// <summary>
        /// 用于会计凭证单据号生成 （开始日期）
        /// </summary>
        public string BeginDate { get; set; }
        /// <summary>
        /// 用于会计凭证单据号生成 （结束日期）
        /// </summary>
        public string EndDate { get; set; }
        public string GroupId { get; set; }
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
    /// <summary>
    /// 付款汇总二表合一
    /// </summary>
    public class SummaryData : FD_PaymentReceivablesDetailCommand
    {
        public string CollectionId { get; set; }
        public string BankDeposit { get; set; }
        public string BankAccount { get; set; }
        public string SettleCustomerID { get;  set; }
        public List<RelatedLists> RelatedList { get; set; } = new List<RelatedLists>();
    }
    /// <summary>
    /// 销售单集合,收款退回
    /// </summary>
    public class RelatedLists
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApplyNumericalOrder { get; set; }
        /// <summary>
        /// 收款/收款汇总
        /// </summary>
        public string SettleReceipType { get; set; }
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
        public string RecordID { get; set; }
    }
    public class FD_PaymentReceivablesDetailCommand  
    {
        /// <summary>
        /// 商城支付单号
        /// </summary>
        public string ApplyScNumericalOrder { get; set; }
        /// <summary>
        /// 商城单号
        /// </summary>
        public string ApplyScOrderNo { get; set; }
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string ApplyNumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 往来类型(在表头实际是表体属性)
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 摘要(在表头实际是表体属性)
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 会计凭证-资金账户
        /// </summary>
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        /// <summary>
        /// 会计凭证-客户
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 所属员工
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 凭证专用
        /// </summary>
        public string SettlePersonID { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 凭证专用
        /// </summary>
        public string SettleMarketID { get; set; }
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// 支出内容
        /// </summary>
        public string Content { get; set; }
        public int? AttachCount { get; set; }
        public decimal? Charges { get; set; }
        public bool IsCreate { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        /// <summary>
        /// 费用科目ID（用于收款单）
        /// </summary>
        public string CostAccoSubjectID { get; set; }
        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID { get; set; }
        public string ExtendAccoSubjectID { get; set; }
        /// <summary>
        /// 科目编号
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 所属项目
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 所属商品
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 贷方金额
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// 借方金额
        /// </summary>
        public decimal Debit { get;  set; }
        /// <summary>
        /// 借贷方（0：借方，1贷方）
        /// </summary>
        public bool LorR { get;  set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int RowNum { get;  set; }
        /// <summary>
        /// 业务单元ID
        /// </summary>
        public string OrganizationSortID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get;  set; }
        /// <summary>
        /// 销售单已收款
        /// </summary>
        public decimal Paid { get; set; }
        /// <summary>
        /// 本次收款
        /// </summary>
        public decimal Payment { get; set; }
        /// <summary>
        /// 应收款
        /// </summary>
        public decimal Payable { get; set; }
        public List<RelatedLists> RelatedList { get; set; } = new List<RelatedLists>();
    }
    public class FD_PaymentExtend
    {
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string CollectionId { get; set; }
        public string AccountName { get; set; }
        public string BankDeposit { get; set; }
        public string BankAccount { get; set; }
        public decimal Amount { get; set; }
        public string PersonId { get; set; }
        public Guid Guid { get; set; }
    }
    public class AccountList
    {
        /// <summary>
        /// 付款账户名称
        /// </summary>
        public string accName { get; set; }
        /// <summary>
        /// 付款账户
        /// </summary>
        public string accNo { get; set; }
        /// <summary>
        /// 转账金额,单位:元
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 银行类型
        /// </summary>
        public string bankType { get; set; }
        /// <summary>
        /// 企业客户号
        /// </summary>
        public string clientId { get; set; }
        /// <summary>
        /// 付款人ID
        /// </summary>
        public string drawee { get; set; }
        /// <summary>
        /// 收款人类型1对私2对公（20200107）
        /// </summary>
        public string iscomm { get; set; }
        /// <summary>
        /// 汇路
        /// </summary>
        public string localFlag { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 业务支付订单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 收款人ID
        /// </summary>
        public string payee { get; set; }
        /// <summary>
        /// 测2200003220
        /// </summary>
        public string toAccName { get; set; }
        /// <summary>
        /// 收款账户
        /// </summary>
        public string toAccNo { get; set; }
        /// <summary>
        /// 收款人银行编码
        /// </summary>
        public string toBankCode { get; set; }
        /// <summary>
        /// 收款账户银行名称
        /// </summary>
        public string toBankName { get; set; }
        /// <summary>
        /// 付款单金额
        /// </summary>
        public string markingAmount { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string billName { get; set; }
        public string uniqueNo { get; set; }
    }
    public class PayType
    {
        public string NumericalOrder { get; set; }
        public string RecordId { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string PayUse { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string TransferType { get; set; }
        /// <summary>
        /// 针对付款汇总，多收方信息
        /// </summary>
        public string extendRecordID { get; set; }
        /// <summary>
        /// 是否复核（付款单 走提交 还是走支付）
        /// </summary>
        public bool IsRecheck { get; set; }
        public string RecheckId { get; set; }
        public string TradeNo { get; set; }
        /// <summary>
        /// 审批流 审批人（支付点提交 通知人） 例：1,2,3,4,5
        /// </summary>
        public string PersonIds { get; set; }
        /// <summary>
        /// 当前级次
        /// </summary>
        public int RawLevel { get; set; } = 0;
        /// <summary>
        /// 最大级次
        /// </summary>
        public int Level { get; set; } = 0;
        /// <summary>
        /// 1=复核，2=驳回 ，0=待复核
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 符合条件的审批流程信息
        /// </summary>
        public List<AuditInfomation> AuditList { get; set; } = new List<AuditInfomation>();


    }
    public class PayResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SerialNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PayCode { get; set; }
        /// <summary>
        /// 银企通
        /// </summary>
        public string PayTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 交易成功
        /// </summary>
        public string FailureMsg { get; set; }
    }
    public class UploadInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileHash { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 李七六
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PathUrl { get; set; }
    }
    /// <summary>
    /// 发起交易返回类
    /// </summary>
    public class PayConection
    {
        /// <summary>
        /// 
        /// </summary>
        public string failNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string successNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public dynamic failInfo { get; set; }
        public dynamic riskInfo { get; set; }
        public string resCode { get; set; }
        public string resMsg { get; set; }
        public string securityPCUrl { get; set; }
        public string securityAppUrl { get; set; }
    }
    /// <summary>
    /// 金融充值
    /// https://confluence.nxin.com/pages/viewpage.action?pageId=65055307
    /// </summary>
    public class ReCharge
    {
        public string resCode { get; set; }
        public string resMsg { get; set; }
        public string singMsg { get; set; }
        public string times { get; set; }
        public string resText { get; set; }
    }
    public class PayReturnResults
    {
        /// <summary>
        /// 业务订单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 业务支付订单号
        /// </summary>
        public string tradeNo { get; set; }
        /// <summary>
        /// 支付系统支付订单号
        /// </summary>
        public string serialNo { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 支付工具标识
        /// </summary>
        public string paymentType { get; set; }
        /// <summary>
        /// 支付工具编码
        /// </summary>
        public string payCode { get; set; }
        /// <summary>
        /// 支付工具描述
        /// </summary>
        public string payTypeName { get; set; }
        /// <summary>
        /// 支付结果
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 支付失败描述信息
        /// </summary>
        public string failureMsg { get; set; }
    }
    /// <summary>
    /// 预付款接口
    /// </summary>
    public class AdvanceCharge
    {
        /// <summary>
        /// 账户类型 预付款：YFK
        /// </summary>
        public string acctType { get; set; }
        /// <summary>
        /// 	充值金额， 两位小数 单位：元
        /// </summary>
        public string amt { get; set; }
        /// <summary>
        /// 业务来源 NX-I-005
        /// </summary>
        public string busiNo { get; set; }
        /// <summary>
        /// 订单号 唯一  如果充值订单号相同并且已经成功 和上一次订单号相同  档案id  单位id  金额一样成功 否则失败        
        /// </summary>
        public string busiOrderNo { get; set; }
        /// <summary>
        /// 档案id
        /// </summary>
        public string custId { get; set; }
        /// <summary>
        /// 单位id
        /// </summary>
        public string entId { get; set; }
        /// <summary>
        /// 操作人id
        /// </summary>
        public string operatorId { get; set; }
        /// <summary>
        ///平台编码：PLAT_NX
        /// </summary>
        public string platCode { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string signmsg { get; set; }
        /// <summary>
        /// 请求时间 yyyyMMddHHmm
        /// </summary>
        public string times { get; set; }
        /// <summary>
        /// 请求时间 yyyyMMddHHmmss
        /// </summary>
        public string transTime { get; set; }
        /// <summary>
        /// 充值:RECHARGE;充值回退:RECHARGEBACK
        /// </summary>
        public string transType { get; set; }
        /// <summary>
        /// 充值回退:RECHARGEBACK 时传原来充值的订单号：busiOrderNo
        /// </summary>
        public string sourceBusiOrderNo { get; set; }
    }
}
