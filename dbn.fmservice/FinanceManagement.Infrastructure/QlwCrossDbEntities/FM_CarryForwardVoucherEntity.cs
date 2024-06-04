using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_CarryForwardVoucherODataEntity : OneWithManyQueryEntity<FM_CarryForwardVoucherDetailODataEntity>
    {
        public FM_CarryForwardVoucherODataEntity()
        {
            Lines = new List<FM_CarryForwardVoucherDetailODataEntity>();
        }
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 结转类别
        /// </summary>
        public string TransferAccountsType { get; set; }
        public string TransferAccountsTypeName { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 来源数据
        /// </summary>
        public string DataSource { get; set; }
        public string DataSourceName { get; set; }
        /// <summary>
        /// 业务摘要
        /// </summary>
        public string TransferAccountsAbstract { get; set; }
        public string TransferAccountsAbstractName { get; set; }
        /// <summary>
        /// 凭证方案
        /// </summary>
        public string TransferAccountsSort { get; set; }
        public string TransferAccountsSortName { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        public string TransactorID { get; set; }
        public string TransactorName { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public string TransactorDate { get; set; }	
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
    }

    public class FM_CarryForwardVoucherDetailODataEntity
    {
        public FM_CarryForwardVoucherDetailODataEntity()
        {
            Extends = new List<FM_CarryForwardVoucherExtendODataEntity>();
            Formulas = new List<FM_CarryForwardVoucherFormulaODataEntity>();
        }
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 结算摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        public string ReceiptAbstractName { get; set; }
        /// <summary>
        /// 会计科目Code
        /// </summary>
        public string AccoSubjectCode { get; set; }

        public string AccoSubjectID { get; set; }
        public string AccoSubjectName { get; set; }

        public bool IsPerson { get; set; }

        public bool IsCustomer { get; set; }

        public bool IsMarket { get; set; }

        public bool IsProduct { get; set; }
        /// <summary>
        /// 借方公式
        /// </summary>

        public string DebitFormula { get; set; }

        public string DebitSecFormula { get; set; }
        /// <summary>
        /// 贷方公式
        /// </summary>
        public string CreditFormula { get; set; }
        public string CreditSecFormula { get; set; }
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 合并分录集合
        /// </summary>
        /// 
        [NotMapped]
        public List<FM_CarryForwardVoucherExtendODataEntity> Extends { get; set; }
        [NotMapped]
        public List<FM_CarryForwardVoucherFormulaODataEntity> Formulas { get; set; }
    }
    public class FM_CarryForwardVoucherExtendODataEntity
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
        /// <summary>
        /// 分类类别
        /// </summary>
        public int Sort { get; set; }
        public string SortName { get; set; }
        public string Symbol { get; set; }
        /// <summary>
        /// 分录类别ID
        /// </summary>
        public string Object { get; set; }
        public string ObjectName { get; set; }
        public string ModifiedDate { get; set; }

    }

    public class FM_CarryForwardVoucherFormulaODataEntity
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
        /// <summary>
        /// 括号标识
        /// </summary>
        public string Bracket { get; set; }
        /// <summary>
        /// 取数来源
        /// </summary>
        public string FormulaID { get; set; }
        /// <summary>
        /// 符号标识
        /// </summary>
        public string Operator { get; set; }
        public string ModifiedDate { get; set; }

    }
}
