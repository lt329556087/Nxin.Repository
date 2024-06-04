using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_UseStateTransfer
{
    public class FA_UseStateTransferDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FA_UseStateTransferAddCommand : FA_UseStateTransferCommand, IRequest<Result>
    {
    }

    public class FA_UseStateTransferModifyCommand : FA_UseStateTransferCommand, IRequest<Result>
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

    public class FA_UseStateTransferCommand : MutilLineCommand<FA_UseStateTransferDetailCommand>
    {

        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
        public string Number { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        public string Remarks { get; set; }
        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }       
    }

    public class FA_UseStateTransferDetailCommand : CommonOperate
    {
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }


        /// <summary>
        /// 卡片ID
        /// </summary>		
        public string CardID { get; set; }


        /// <summary>
        /// 变动前使用状态
        /// </summary>		
        public string BeforeUseStateID { get; set; }
        /// <summary>
        /// 变动后使用状态
        /// </summary>		
        public string AfterUseStateID { get; set; }
        public string Remarks { get; set; }

       
        public string ModifiedDate { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>		
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产编码
        /// </summary>		
        public string AssetsCode { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>		
        public string Specification { get; set; }
        /// <summary>
        /// 开始使用时间
        /// </summary>		
        public string StartDate { get; set; }


        /// <summary>
        /// 变动前使用状态名称
        /// </summary>		
        public string BeforeUseStateName { get; set; }
        /// <summary>
        /// 变动后使用状态名称
        /// </summary>		
        public string AfterUseStateName { get; set; }
        public string RowStatus { get; set; }
    }



}
