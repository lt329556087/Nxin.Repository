using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_SettleReceiptODataEntity : OneWithManyQueryEntity<FD_SettleReceiptDetailODataEntity>
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
    }

    public class FD_SettleReceiptDetailODataEntity
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
    }
   
}
