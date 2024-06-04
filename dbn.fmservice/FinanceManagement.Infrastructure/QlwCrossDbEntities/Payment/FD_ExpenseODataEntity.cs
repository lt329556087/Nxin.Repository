using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_ExpenseODataEntity : OneWithManyQueryEntity<FD_ExpenseDetailODataEntity>
    {


        #region Model
        
        //public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>	        
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public string CreatedDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>		
        public string ModifiedDate { get; set; }
        
        public string Guid { get; set; }
        /// <summary>
        /// 报销类型（应用ID） 预付款申请：1803081320210000101 采购付款申请：1612091318520000101
        /// </summary>
        public string ExpenseType { get; set; }
        /// <summary>
        /// 报销摘要
        /// </summary>
        public string ExpenseAbstract { get; set; }
        /// <summary>
        /// 单据分类（字典表）
        /// </summary>
        public string ExpenseSort { get; set; }
        /// <summary>
        /// 负责人ID
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 应付款日期（到期日）
        /// </summary>
        public string HouldPayDate { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public string PayDate { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string DraweeID { get; set; }
        /// <summary>
        /// 本次核销金额
        /// </summary>
        public decimal CurrentVerificationAmount { get; set; }
        /// <summary>
        /// 已核销金额
        /// </summary>
        public decimal HadVerificationAmount { get; set; }
        /// <summary>
        /// 紧急 默认:0 0否 1是
        /// </summary>
        public int Pressing { get; set; }
        /// <summary>
        /// 审批进度
        /// </summary>
        public string Progress { get; set; }
        /// <summary>
        /// 付款情况
        /// </summary>
        public string PayContent { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        #endregion
        /// <summary>
        /// 制单人名称
        /// </summary>

        public string OwnerName { get; set; }

        /// <summary>
        /// 申请单位名称
        /// </summary>		
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 单据类型名称
        /// </summary>
        public string ExpenseTypeName { get; set; }
        public string ExpenseAbstractName { get; set; }
        public string ExpenseSortName { get; set; }
        public string PersonName { get; set; }
        public string DraweeName { get; set; }  
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public string AuditResultName { get; set; }
        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 核销状态
        /// </summary>
        public string VerificationStateName { get; set; }
        /// <summary>
        /// 收款单位
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 关联采购单
        /// </summary>
        [NotMapped]
        public List<RelatedODataEntity> RelatedPurList { get; set; }
        /// <summary>
        /// 关联采购合同
        /// </summary>
        [NotMapped]
        public List<RelatedODataEntity> RelatedConList { get; set; }
        //public string PayerName { get; set; }
    }

    public class FD_ExpenseDetailODataEntity
    {
        [Key]
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrder { get; set; }
       
        public string Guid { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 往来类型 201611160104402104：供应商
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 收款人类型 201611160104402101：客户 201611160104402102：部门 201611160104402103：员工 201611160104402104：供应商  201611160104402105：集团单位 201611160104402106：其他
        /// </summary>
        public string SettleBusinessType { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>		
        public string MarketID { get; set; }
        public string ProjectID { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string SettlePayerID { get; set; }
        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 支出内容
        /// </summary>
        public string Content { get; set; }
        public string AccountInformation { get; set; }
        public string ReceiptAbstractDetail { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractName { get; set; }
        /// <summary>
        /// 往来类型 供应商
        /// </summary>
        public string BusinessTypeName { get; set; }
        /// <summary>
        /// 收款人类型 201611160104402101：客户 201611160104402102：部门 201611160104402103：员工 201611160104402104：供应商  201611160104402105：集团单位 201611160104402106：其他
        /// </summary>
        public string SettleBusinessTypeName { get; set; }
        /// <summary>
        /// 收款单位
        /// </summary>
        public string CustomerName { get; set; }        //PayerName { get; set; }
        
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string SettlePayerName { get; set; }
        public string ReceiptAbstractDetailName { get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        [NotMapped]
        public string AccountName { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        [NotMapped]
        public string BankDeposit { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        [NotMapped]
        public string BankAccount { get; set; }
        
        //public string CustomerName { get; set; }
        //public string PersonName { get; set; }
        ///// <summary>
        ///// 部门ID
        ///// </summary>		
        //public string MarketName { get; set; }
    }
    public class FD_ExpenseExtODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string CollectionId { get; set; }
        public string PersonId { get; set; }
        public string AccountName { get; set; }
        public string BankDeposit { get; set; }
        public string BankAccount { get; set; }
        public decimal Amount { get; set; }
        public string CollectionName{ get; set; }
        public string PersonName { get; set; }
    }
    /// <summary>
    /// 关联信息
    /// </summary>
    public class RelatedODataEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public string Guid { get; set; }
        public string RelatedNumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Payment { get; set; }
        public int RelatedID { get; set; }
        public decimal Amount { get; set; }
        public decimal HavePaidAmount { get; set; }
        public string DataDate { get; set; }
        public string ContractType { get; set; }
        public string ContractNumber { get; set; }
        public string ContractNumericalOrder { get; set; }
        public string ContractTypeName { get; set; }
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
    }
    public static class FMEnumType
    {
        /// <summary>
        /// 客户类型
        /// </summary>
        public static string CustomerType = "201611160104402101";
        /// <summary>
        /// 部门类型
        /// </summary>
        public static string MarketType = "201611160104402102";

        /// <summary>
        /// 员工类型
        /// </summary>
        public static string PersonType = "201611160104402103";
        /// <summary>
        /// 供应商类型
        /// </summary>
        public static string SupplierType = "201611160104402104";
        /// <summary>
        /// 集团单位
        /// </summary>
        public static string GroupEnteType = "201611160104402105";
        //其他
        public static string OtherType = "201611160104402106";

    }
    public enum FMExpenseType : long
    {
        支出申请 = 1612052003560000101,//已更新
        借款申请 = 1612061850170000101,//已更新
        支出汇总申请 = 1612081533020000101,
        采购付款申请 = 1612091318520000101,
        差旅报销申请 = 1612101828540000101,//已更新
        差旅汇总申请 = 1612122002230000101,
        项目支出申请 = 1612122008320000101,
        薪资支出申请 = 1612121954010000101,
        保证金退款申请 = 1904180946310000100,
    }
}
