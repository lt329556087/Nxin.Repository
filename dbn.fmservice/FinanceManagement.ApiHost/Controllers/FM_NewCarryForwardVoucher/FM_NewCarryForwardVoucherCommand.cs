using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_NewCarryForwardVoucher
{

    public class FM_NewCarryForwardVoucherAddCommand : FM_NewCarryForwardVoucherCommand, IRequest<Result>
    {

    }

    public class FM_NewCarryForwardVoucherDeleteCommand : FM_NewCarryForwardVoucherCommand, IRequest<Result>
    {

    }

    public class FM_NewCarryForwardVoucherModifyCommand : FM_NewCarryForwardVoucherCommand, IRequest<Result>
    {
    }
    public class FM_NewCarryForwardVoucherCopyCommand : FM_NewCarryForwardVoucherCommand, IRequest<Result>
    {
        public List<string> EnterpriseIds { get; set; }
        /// <summary>
        /// 左上角账套日期
        /// </summary>
        public string LastDate { get; set; }
    }
    public class FM_NewCarryForwardVoucherCommand : MutilLineCommand<FM_NewCarryForwardVoucherDetailCommand>
    {

        public string NumericalOrder { get; set; }
        /// <summary>
        /// 结转类别
        /// </summary>
        public string TransferAccountsType { get; set; }
        public string TransferAccountsTypeName { get; set; }
        public string Number { get; set; }
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
        public string SettleNumber { get; set; }
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
        public DateTime TransactorDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class FM_NewCarryForwardVoucherDetailCommand : CommonOperate
    {
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
        public bool IsPigFram { get; set; }

        public bool IsProject { get; set; }

        public bool IsSum { get; set; }

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
        public DateTime ModifiedDate { get; set; }
        public List<FM_NewCarryForwardVoucherExtendCommand> Extends { get; set; }
        public List<FM_NewCarryForwardVoucherFormulaCommand> Formulas { get; set; }
    }

    public class FM_NewCarryForwardVoucherExtendCommand : CommonOperate
    {
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
    public class FM_NewCarryForwardVoucherFormulaCommand : CommonOperate
    {
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

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }


    public class FM_NewCarryForwardVoucherImport
    {
        /// <summary>
        /// 结转类别
        /// </summary>
        public string TransferAccountsType { get; set; }
        public string TransferAccountsTypeName { get; set; }
        public string Number { get; set; }
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
        public string TransferAccountsAbstract { get; set; } = "0";
        public string TransferAccountsAbstractName { get; set; } = "";
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
        public bool IsPigFram { get; set; }

        public bool IsProject { get; set; }

        public bool IsSum { get; set; }

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

        /// <summary>
        /// 辅助项目
        /// </summary>
        public string AuxiliaryProject { get; set; }
        /// <summary>
        /// 分录条件
        /// </summary>
        public string Condition { get; set; }
        public List<FM_NewCarryForwardVoucherExtendCommand> Extends { get; set; }
        public List<FM_NewCarryForwardVoucherFormulaCommand> Formulas { get; set; }
    }




}
