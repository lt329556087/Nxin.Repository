using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtExecution
{

    public class FD_BadDebtExecutionAddCommand : FD_BadDebtExecutionCommand, IRequest<Result>
    {

    }

    public class FD_BadDebtExecutionDeleteCommand : FD_BadDebtExecutionCommand, IRequest<Result>
    {

    }

    public class FD_BadDebtExecutionCommand 
    {

        public int RecordID { get; set; }
        public string AppID { get; set; }
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 凭证流水号
        /// </summary>
        public string NumericalReceipt { get; set; }
        public bool State { get; set; }
        /// <summary>
        /// 生成凭证时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 详情信息
        /// </summary>
        public string Remarks { get; set; }
    }

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }
}
