using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_CostParamsSet
{
    #region Dto
    public class FM_CostParamsSetDto : MutilLineCommand<FmCostparamssetdetailDto, FmCostparamssetextendDto,FmCostparamssetextendDto>
    {
        
    }

    public class FmCostparamssetdetailDto : FmCostparamssetdetail
    {
        ///// <summary>
        ///// 流水号
        ///// </summary>
        //public new string NumericalOrder { get; set; }
        ///// <summary>
        ///// 猪场ID
        ///// </summary>
        //public new string PigFarmId { get; set; }
    }
    public class FmCostparamssetextendDto : FmCostparamssetextend
    {
        /////// <summary>
        /////// 流水号
        /////// </summary>
        ////public new string NumericalOrder { get; set; }
        /////// <summary>
        /////// 扩展类型ID（生成方式/总折旧月数）
        /////// </summary>
        ////public new string ExtendTypeId { get; set; }
        /////// <summary>
        /////// 来源类型ID
        /////// </summary>
        ////public new string SourceTypeId { get; set; }
        /////// <summary>
        /////// 生成方式/获取方式
        /////// </summary>
        ////public new string CreatedTypeId { get; set; }
    }

    public class MutilLineCommand<T1,T2,T3> : FmCostparamsset, IRequest<Result> where T1 : new() where T2 : new() where T3 : new()
    {
        //public new string NumericalOrder { get; set; }
        public List<T1> Details { get; set; }
        public List<T2> Extends { get; set; }
        public List<T3> DepreciationExtends { get; set; }
    }
    #endregion

    #region Command
    public class FM_CostParamsSetCommand : FM_CostParamsSetDto, IRequest<Result>
    { }
    public class FM_CostParamsSetAddCommand : FM_CostParamsSetDto, IRequest<Result>
    {

    }

    public class FM_CostParamsSetDeleteCommand : FM_CostParamsSetDto, IRequest<Result>
    {

    }

    public class FM_CostParamsSetModifyCommand : FM_CostParamsSetDto, IRequest<Result>
    {
    }
    #endregion
}
