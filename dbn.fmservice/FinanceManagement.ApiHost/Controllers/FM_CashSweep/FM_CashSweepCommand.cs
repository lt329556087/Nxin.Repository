using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_CashSweep
{
    public class FM_CashSweepDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FM_CashSweepAddCommand : FM_CashSweepCommand, IRequest<Result>
    {
    }

    public class FM_CashSweepModifyCommand : FM_CashSweepCommand, IRequest<Result>
    {
    }
    public class FM_CashSweepStateModifyCommand : FM_CashSweepCommand, IRequest<Result>
    {
        /// <summary>
        /// 自动归集时是否启用
        /// </summary>
        public bool IsUse { get; set; }
    }
    /// <summary>
    /// 表头表体关联
    /// </summary>
    /// <typeparam name="TLineCommand"></typeparam>
    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }

    public class FM_CashSweepCommand : MutilLineCommand<FM_CashSweepDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        public DateTime DataDate { get; set; }
        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }


        /// <summary>
        /// 归集方向
        /// </summary>		
        public string SweepDirectionID { get; set; }

        /// <summary>
        /// 归集类型
        /// </summary>
        public string SweepType { get; set; }


        /// <summary>
        /// Remarks 备注
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }



        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }


        public string ModifiedDate { get; set; }

        public DateTime? ExcuteDate { get; set; }

        public long? ExcuterID { get; set; }
        public string CollectionScheme { get; set; }
       
        public bool? IsUse { get; set; }
        /// <summary>
        /// 自动归集时间
        /// </summary>
        public DateTime? AutoTime { get; set; }
        /// <summary>
        /// 归集类型 对应coll_type
        /// </summary>
        public string SchemeType { get; set; }
        /// <summary>
        /// 方案金额 对应amount
        /// </summary>
        public decimal? SchemeAmount { get; set; }
        /// <summary>
        /// 方案比例 对应rate
        /// </summary>
        public decimal? Rate { get; set; }
        /// <summary>
        /// 资金计划类型 对应plan_type
        /// </summary>
        public string PlanType { get; set; }
        /// <summary>
        /// 归集公式
        /// </summary>
        public string SchemeFormula { get; set; }
        /// <summary>
        /// 归集方案类型名称
        /// </summary>
        public string SchemeTypeName { get; set; }
        public bool IsNew { get; set; }
        ///// <summary>
        ///// 定时任务ID
        ///// </summary>
        //public int JobID { get; set; }
        //附件
        public string UploadInfo { get; set; }
    }

    public class FM_CashSweepDetailCommand : CommonOperate
    {
        /// <summary>
        /// RecordID
        /// </summary>		
        public string RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// NumericalOrderDetail
        /// </summary>		
        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// EnterpriseID 
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }

        /// <summary>
        /// AccountBalance
        /// </summary>		
        public decimal? AccountBalance { get; set; }

        public decimal? OtherAccountBalance { get; set; }

        public decimal? TheoryBalance { get; set; }
        public decimal? TransformBalance { get; set; }
        /// <summary>
        /// AutoSweepBalance
        /// </summary>		
        public decimal? AutoSweepBalance { get; set; }

        public string AutoSweepBalance_Show { get; set; }
        public decimal? ManualSweepBalance { get; set; }

        /// <summary>
        /// Remarks 事由
        /// </summary>		
        public string Remark { get; set; }
        public int? Status { get; set; }
        public string ModifiedDate { get; set; }

        public string ExcuteMsg { get; set; }
        public string RowStatus { get; set; }
    }
}
