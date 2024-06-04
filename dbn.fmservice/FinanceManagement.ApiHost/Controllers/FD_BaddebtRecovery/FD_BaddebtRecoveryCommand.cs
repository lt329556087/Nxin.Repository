﻿using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BaddebtRecovery
{
    public class FD_BaddebtRecoveryDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
        public int RecordID { get; set; }

    }
    public class FD_BaddebtRecoveryDeleteDetailCommand : IRequest<Result>
    {
        public List<FD_BaddebtRecoveryDeleteCommand> Details { get; set; }
    }
    public class FD_BaddebtRecoveryAddCommand : FD_BaddebtRecoveryCommand, IRequest<Result>
    {
    }

    public class FD_BaddebtRecoveryModifyCommand : FD_BaddebtRecoveryCommand, IRequest<Result>
    {
    }

    /// <summary>
    /// 表头表体关联
    /// </summary>
    /// <typeparam name="TLineCommand"></typeparam>
    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }

    public class FD_BaddebtRecoveryCommand : MutilLineCommand<FD_BaddebtRecoveryDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }

        public string EnterpriseID { get; set; }
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }

    }

    public class FD_BaddebtRecoveryDetailCommand : CommonOperate
    {
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 业务类型（0：应收账款 1：其他应收款）
        /// </summary>
        public int BusiType { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CurrentUnit { get; set; }
        /// <summary>
        /// 期末余额
        /// </summary>

        public decimal Amount { get; set; }
        /// <summary>
        /// 坏账准备金额
        /// </summary>

        public decimal RecoveryAmount { get; set; }
        public string ModifiedDate { get; set; }
        public string RowStatus { get; set; }
    }
}
