using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtProvision
{

    public class FD_BadDebtProvisionAddCommand : FD_BadDebtOccurCommand, IRequest<Result>
    {

    }

    public class FD_BadDebtProvisionDeleteCommand : FD_BadDebtOccurCommand, IRequest<Result>
    {

    }

    public class FD_BadDebtProvisionModifyCommand : FD_BadDebtOccurCommand, IRequest<Result>
    {
    }

    public class FD_BadDebtOccurCommand : MutilLineCommand<FD_BadDebtProvisionDetailCommand>
    {
        public FD_BadDebtOccurCommand()
        {
            Lines = new List<FD_BadDebtProvisionDetailCommand>();
            Lines1 = new List<FD_BadDebtProvisionDetailCommand>();
            Lines2 = new List<FD_BadDebtProvisionDetailCommand>();
        }

        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPointID { get; set; }


        public string EnterpriseID { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        public string AccoSubjectID1 { get; set; }
        public string AccoSubjectID2 { get; set; }

        public decimal SumProvisionAmount1 { get; set; }

        public decimal SumProvisionAmount2 { get; set; }

        /// <summary>
        /// 已计提总金额1
        /// </summary>
        public decimal HaveProvisionAmount1 { get; set; }

        /// <summary>
        /// 已计提总金额2
        /// </summary>
        public decimal HaveProvisionAmount2 { get; set; }

        /// <summary>
        /// 计提差额1
        /// </summary>
        public decimal DiffAmount1 { get; set; }

        /// <summary>
        /// 计提差额2
        /// </summary>
        public decimal DiffAmount2 { get; set; }

        public string NumericalOrderSetting { get; set; } = "0";

        public List<FD_BadDebtProvisionDetailCommand> Lines1 { get; set; }

        public List<FD_BadDebtProvisionDetailCommand> Lines2 { get; set; }

    }

    public class FD_BadDebtProvisionDetailCommand : CommonOperate
    {


        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }

        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// 科目id
        /// </summary>
        public string AccoSubjectID { get; set; }


        /// <summary>
        /// 科目名称
        /// </summary>
        public string AccoSubjectName { get; set; }

        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CustomerID { get; set; }


        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 未收回金额
        /// </summary>
        public decimal NoReceiveAmount { get; set; }
        /// <summary>
        /// 本期坏账计提准备金额
        /// </summary>
        public decimal CurrentDebtPrepareAmount { get; set; }
        /// <summary>
        /// 上期坏账计提准备金额
        /// </summary>
        public decimal LastDebtPrepareAmount { get; set; }

        /// <summary>
        /// 调整金额
        /// </summary>
        public decimal TransferAmount { get; set; }

        /// <summary>
        /// 本期计提金额
        /// </summary>
        public decimal ProvisionAmount { get; set; }

        public string NumericalOrderSpecific { get; set; }

        public string ProvisionType { get; set; }

        public string ProvisionTypeName { get; set; }


        public decimal ReclassAmount { get; set; }

        public decimal EndAmount { get; set; }

        public string BusinessType { get; set; }

        [NotMapped]
        public List<FD_BadDebtProvisionExtCommand> AgingList { get; set; }

    }

    public class FD_BadDebtProvisionExtCommand : CommonOperate
    {
        public int RecordID { get; set; }
        /// <summary>
        /// 计提表体ID
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 账龄ID
        /// </summary>
        public int AgingID { get; set; }

        /// <summary>
        /// 账龄名称
        /// </summary>
        public string AgingName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        public string Name { get; set; }

        public decimal Ratio { get; set; }
    }

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }
}
