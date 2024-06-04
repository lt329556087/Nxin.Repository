using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FM_CarryForwardVoucher : EntityBase
    {
        public FM_CarryForwardVoucher()
        {
            Details = new List<FM_CarryForwardVoucherDetail>();
        }

        public FM_CarryForwardVoucherDetail AddDetail(FM_CarryForwardVoucherDetail detail)
        {
            Details.Add(detail);
            return detail;
        }

        public void Update(string TransferAccountsType, string TicketedPointID, string DataSource,
             string TransferAccountsAbstract, string TransferAccountsSort, string Remarks, string TransactorID, DateTime TransactorDate,string SettleNumber)
        {
            this.TransferAccountsType = TransferAccountsType;
            this.TicketedPointID = TicketedPointID;
            this.DataSource = DataSource;
            this.TransferAccountsAbstract = TransferAccountsAbstract;
            this.TransferAccountsSort = TransferAccountsSort;
            this.Remarks = Remarks;
            this.SettleNumber = SettleNumber;
            this.TransactorID = TransactorID;
            this.TransactorDate = TransactorDate;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
        /// <summary>
        /// 结转类别
        /// </summary>
        public string TransferAccountsType { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 来源数据
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 业务摘要
        /// </summary>
        public string TransferAccountsAbstract { get; set; }
        /// <summary>
        /// 凭证方案
        /// </summary>
        public string TransferAccountsSort { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }
        public string SettleNumber { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        public string TransactorID { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime TransactorDate { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public List<FM_CarryForwardVoucherDetail> Details { get; set; }
    }

    public class FM_CarryForwardVoucherDetail : EntityBase
    {
        public void Add() { }

        public void Update(string ReceiptAbstractID, string AccoSubjectCode, string AccoSubjectID, bool IsPerson,
            bool IsCustomer, bool IsMarket, bool IsProduct, bool IsPigFram, bool IsProject, bool IsSum, string DebitFormula,
            string DebitSecFormula, string CreditFormula, string CreditSecFormula)
        {
            this.ReceiptAbstractID = ReceiptAbstractID;
            this.AccoSubjectCode = AccoSubjectCode;
            this.AccoSubjectID = AccoSubjectID;
            this.IsPerson = IsPerson;
            this.IsCustomer = IsCustomer;
            this.IsMarket = IsMarket;
            this.IsProduct = IsProduct;
            this.IsPigFram = IsPigFram;
            this.IsProject = IsProject;
            this.IsSum = IsSum;
            this.DebitFormula = DebitFormula;
            this.DebitSecFormula = DebitSecFormula;
            this.CreditFormula = CreditFormula;
            this.CreditSecFormula = CreditSecFormula;
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
        /// 结算摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 会计科目Code
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectID { get; set; }

        public bool IsPerson { get; set; }

        public bool IsCustomer { get; set; }

        public bool IsMarket { get; set; }
        public bool IsPigFram { get; set; }

        public bool IsProject { get; set; }

        public bool IsSum { get; set; }

        public bool IsProduct { get; set; }
        /// <summary>
        /// 借方公式
        /// </summary>
        public string DebitFormula { get; set; }
        /// <summary>
        /// 借方公式信息
        /// </summary>
        public string DebitSecFormula { get; set; }
        /// <summary>
        /// 贷方公式
        /// </summary>
        public string CreditFormula { get; set; }
        /// <summary>
        /// 贷方公式信息
        /// </summary>
        public string CreditSecFormula { get; set; }
        public DateTime ModifiedDate { get; set; }
        public FM_CarryForwardVoucher FM_CarryForwardVoucher { get; set; }

    }
    public class FM_CarryForwardVoucherExtend : EntityBase
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
        /// <summary>
        /// 符号
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// 分录类别ID
        /// </summary>
        public string Object { get; set; }
        public DateTime ModifiedDate { get; set; }

    }

    public class FM_CarryForwardVoucherFormula : EntityBase
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
        public DateTime ModifiedDate { get; set; }

    }
}
