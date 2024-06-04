using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_VoucherAmortization
{

    public class FD_VoucherAmortizationAddCommand : FD_VoucherAmortizationCommand, IRequest<Result>
    {

    }

    public class FD_VoucherAmortizationDeleteCommand : FD_VoucherAmortizationCommand, IRequest<Result>
    {

    }

    public class FD_VoucherAmortizationModifyCommand : FD_VoucherAmortizationCommand, IRequest<Result>
    {
    }
    public class FD_VoucherAmortizationListModifyCommand : FD_VoucherAmortizationCommand, IRequest<Result>
    {
    }
    public class VoucherSearch
    {
        /// <summary>
        /// 权限单位，后端自查
        /// </summary>
        public string EnterpriseIds { get; set; }
        /// <summary>
        /// 方案名称
        /// </summary>
        public string AmortizationName { get; set; }
        /// <summary>
        /// 单位组织
        /// </summary>
        public string EnterSortIds { get; set; }
        public string MarketSortIds { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string AccountDate { get; set; }

    }
    public class FD_VoucherAmortizationCommand : MutilLineCommand<FD_VoucherAmortizationDetailCommand, FD_VoucherAmortizationPeriodDetailCommand>
    {
        public FD_VoucherAmortizationCommand()
        {
            this.Lines = new List<FD_VoucherAmortizationDetailCommand>();
            this.PeriodLines = new List<FD_VoucherAmortizationPeriodDetailCommand>();
        }
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
        /// 执行状态
        /// </summary>
        public int ImpStateID { get; set; }
        /// <summary>
        /// 执行状态名称
        /// </summary>
        public string ImpState { get; set; }
        /// <summary>
        /// 今天状态
        /// </summary>
        public bool IsUse { get; set; }
        public string UseState { get; set; }
        /// <summary>
        /// 禁用人
        /// </summary>
        public string OperatorID { get; set; }
        public string OperatorName { get; set; }
    }

    public class FD_VoucherAmortizationDetailCommand : CommonOperate
    {
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

    public class FD_VoucherAmortizationPeriodDetailCommand : CommonOperate
    {
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        public int RowNum { get; set; }
        public DateTime AccountDate { get; set; }
        public decimal AmortizationAmount { get; set; }
        public bool IsAmort { get; set; }
        public bool IsLast { get; set; }
        public string ModifiedDate { get; set; }

    }

    public class MutilLineCommand<TLineCommand1, TLineCommand2> : CommonOperate where TLineCommand1 : CommonOperate
                                                                                where TLineCommand2 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
        public List<TLineCommand2> PeriodLines { get; set; }
    }

    

}
