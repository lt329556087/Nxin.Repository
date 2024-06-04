using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_SettleReceiptExtODataEntity : OneWithManyQueryEntity<FD_SettleReceiptSubjectDetailODataEntity>
    {
        [Key]
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string SettleReceipType { get; set; }
        public string SettleReceipTypeName { get; set; }
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
        public string AttachmentNum { get; set; }
        public string AccountNo { get; set; }
        public string OwnerName { get; set; }

    }

    public class FD_SettleReceiptSubjectDetailODataEntity
    {
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 所属单位
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectID { get; set; }
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 客商
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 人员
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 资金账户
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// 借贷方（0：借方，1贷方）
        /// </summary>
        public bool? LorR { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 账龄
        /// </summary>
        public DateTime? AgingDate { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 业务单元
        /// </summary>
        public string OrganizationSortID { get; set; }
        /// <summary>
        /// 是否费用
        /// </summary>
        public bool? IsCharges { get; set; }
        public string OrganizationSortName { get; set; }
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractName { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectName { get; set; }

        public string AccoSubjectFullName { get; set; }
        /// <summary>
        /// 客商
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 人员
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketName { get; set; }

        public string MarketFullName { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// 资金账户
        /// </summary>
        public string AccountName { get; set; }
        
        /// <summary>
        /// 开户行（支行名称）
        /// </summary>
        public string DepositBank { get; set; }
        #region 科目

        /// <summary>
        /// bProject
        /// </summary>		
        public bool? bProject { get; set; }

        /// <summary>
        /// bCus
        /// </summary>		
        public bool? bCus { get; set; }

        /// <summary>
        /// bPerson
        /// </summary>		
        public bool? bPerson { get; set; }

        /// <summary>
        /// bSup
        /// </summary>		
        public bool? bSup { get; set; }

        /// <summary>
        /// bDept
        /// </summary>		
        public bool? bDept { get; set; }
        ///// <summary>
        ///// bTorF
        ///// </summary>		
        //public bool bTorF { get; set; }

        ///// <summary>
        ///// bLorR
        ///// </summary>		
        //public bool bLorR { get; set; }

        ///// <summary>
        ///// bEnd
        ///// </summary>		
        //public bool bEnd { get; set; }

        ///// <summary>
        ///// bItem
        ///// </summary>		
        //public bool bItem { get; set; }

        ///// <summary>
        ///// bCash
        ///// </summary>		
        //public bool bCash { get; set; }

        ///// <summary>
        ///// bBank
        ///// </summary>		
        //public bool bBank { get; set; }
        #endregion
    }


    public class FD_SettleReceiptEntity : OneWithManyQueryEntity<FD_SettleReceiptDetailEntity>
    {
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 上传附件信息
        /// </summary>
        public string UploadInfo { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 收付款（类型区分）用于 原始单据跳转
        /// </summary>
        public string PayReceType { get; set; }
        public string SettleReceipTypeName { get; set; }
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
        public string TicketedPointNumber { get; set; }
        public string line { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public long? Number { get; set; }
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
        public string AttachmentNum { get; set; }
        public string AccountNo { get; set; }
        public string OwnerName { get; set; }
        public string AuditName { get; set; }
        public decimal? Amount { get; set; }
        public string ReceiptAbstractID { get; set; }
        public string ReceiptAbstractName { get; set; }
        public string ApplyNumericalOrder { get; set; }
        public string ApplySettleReceipType { get; set; }
        /// <summary>
        /// 标识是否集中支付  false：否 true：是
        /// </summary>
        public bool? IsGroupPay { get; set; }
        /// <summary>
        /// 新版会计凭证关联收付（汇总）单单号显示改为显示：单据类型+单号 ，例如 收款单 2 ，单击可跳转打开收付款单
        /// </summary>
        public string ApplyNumber { get; set; }
        /// <summary>
        /// 采购发票与会计凭证关联的采购发票单据号
        /// </summary>
        public string InvocieNumber{ get; set; }
        /// <summary>
        /// 采购发票 流水号
        /// </summary>
        public string InvocieNumericalOrder { get; set; }
        public int? SummaryIsEnd { get; set; }
        public int? AccIsEnd { get; set; }
        public int? MarIsEnd { get; set; }

    }
    /// <summary>
    /// 会计凭证列表查询扩展头
    /// </summary>
    public class FD_SettleReceiptHeadExtend
    {
        [Key]
        public long? NumericalOrder { get; set; }
        public decimal Amount { get; set; }
        public long? ReceiptAbstractID { get; set; }
        public string ReceiptAbstractName { get; set; }
        /// <summary>
        /// 摘要是否末级 总数量 > 0 就拦截
        /// </summary>
        public int? SummaryIsEnd { get; set; }
        /// <summary>
        /// 科目是否末级 总数量 > 0 就拦截
        /// </summary>
        public int? AccIsEnd { get; set; }
        /// <summary>
        /// 部门是否末级 总数量 > 0 就拦截
        /// </summary>
        public int? MarIsEnd { get; set; }
    }
    public class FD_SettleReceiptDetailEntity
    {
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 现金流量项目名称
        /// </summary>
        public string FinancialStatementName { get; set; }
        /// <summary>
        /// 所属单位
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectID { get; set; }
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 客商
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 人员
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 资金账户
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// true=供应商，false=客户
        /// </summary>
        public bool? IsCus { get; set; }
        /// <summary>
        /// 借贷方（0：借方，1贷方）
        /// </summary>
        public bool LorR { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 账龄
        /// </summary>
        public DateTime? AgingDate { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 业务单元
        /// </summary>
        public string OrganizationSortID { get; set; }
        /// <summary>
        /// 是否费用
        /// </summary>
        public bool? IsCharges { get; set; }
        public string OrganizationSortName { get; set; }
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractName { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectName { get; set; }

        public string AccoSubjectFullName { get; set; }
        /// <summary>
        /// 客商
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 人员
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketName { get; set; }

        public string MarketFullName { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// 资金账户
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 开户行（支行名称）
        /// </summary>
        public string DepositBank { get; set; }
        public bool? SummaryIsEnd { get; set; }
        public bool? AccIsEnd { get; set; }
        public bool? MarIsEnd { get; set; }
        #region 科目

        /// <summary>
        /// bProject
        /// </summary>		
        public bool? bProject { get; set; }

        /// <summary>
        /// bCus
        /// </summary>		
        public bool? bCus { get; set; }

        /// <summary>
        /// bPerson
        /// </summary>		
        public bool? bPerson { get; set; }

        /// <summary>
        /// bSup
        /// </summary>		
        public bool? bSup { get; set; }
        /// <summary>
        /// 是否资金科目
        /// </summary>
        public bool? IsTorF { get; set; }

        /// <summary>
        /// bDept
        /// </summary>		
        public bool? bDept { get; set; }
        public string ClassificationID { get; set; }
        public string ClassificationName { get; set; }
        public string ProductGroupID { get; set; }
        public string ProductGroupName { get; set; }
        /// <summary>
        /// 1,2,3,4,5
        /// </summary>
        public string Auxiliary { get; set; }

        ///// <summary>
        ///// bTorF
        ///// </summary>		
        //public bool bTorF { get; set; }

        ///// <summary>
        ///// bLorR
        ///// </summary>		
        //public bool bLorR { get; set; }

        ///// <summary>
        ///// bEnd
        ///// </summary>		
        //public bool bEnd { get; set; }

        ///// <summary>
        ///// bItem
        ///// </summary>		
        //public bool bItem { get; set; }

        ///// <summary>
        ///// bCash
        ///// </summary>		
        //public bool bCash { get; set; }

        ///// <summary>
        ///// bBank
        ///// </summary>		
        //public bool bBank { get; set; }
        #endregion
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
        public string AuxiliaryName1 { get; set; }
        public string AuxiliaryName2 { get; set; }
        public string AuxiliaryName3 { get; set; }
        public string AuxiliaryName4 { get; set; }
        public string AuxiliaryName5 { get; set; }
        public string AuxiliaryName6 { get; set; }
        public string AuxiliaryName7 { get; set; }
        public string AuxiliaryName8 { get; set; }
        public string AuxiliaryName9 { get; set; }
        public string AuxiliaryName10 { get; set; }

        #endregion
    }
    /// <summary>
    /// 会计凭证生成单据号(默认最大单据号)
    /// </summary>
    public class Number
    {
        [Key]
        public string MaxNumber { get; set; }
    }
    public class SearchKM
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string accCode { get; set; }
        public string accType { get; set; }
        public string EnterpriseId { get; set; }
    }
    public class SearchVoucher
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string EnterpriseIds { get; set; }
    }
    public class KMData
    {
        [Key]
        public bool? IsLorR { get; set; }
        public decimal? fsDebit { get; set; }
        public decimal? fsCredit { get; set; }
        public decimal? qcDebit { get; set; }
        public decimal? qcCredit { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
    }
    /// <summary>
    /// 凭证处理列表实体
    /// </summary>
    public class VoucherHandleInfoEntity
    {
        /// <summary>
        /// 凭证处理流水号
        /// </summary>
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 账务单位
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 单据类型（收付款）
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 单据类型名称
        /// </summary>
        public string SettleReceipTypeName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public long Number { get; set; }
        /// <summary>
        /// 单据字+单据号
        /// </summary>
        public string TicketedPointNumber { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 往来类型名称
        /// </summary>
        public string BusinessTypeName { get; set; }
        /// <summary>
        /// 往来单位名称 （包含 五种往来数据匹配ID）
        /// </summary>
        public string CollectionName { get; set; }
        /// <summary>
        /// 往来单位ID
        /// </summary>
        public string CollectionId { get; set; }
        /// <summary>
        /// 金额（汇总）
        /// </summary>
        public decimal Amount { get; set; }
        public string EnterpriseID { get; set; }
        public string TicketedPointID { get; set; }
    }
}
