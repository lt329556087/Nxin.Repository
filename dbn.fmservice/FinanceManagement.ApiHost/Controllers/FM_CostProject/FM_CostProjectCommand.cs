using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_CostProject
{
    #region Dto
    public class FM_CostProjectDto : MutilLineCommand<FmCostprojectdetailDto>
    {

    }

    public class FmCostprojectdetailDto : FmCostprojectdetail
    {
        ///// <summary>
        ///// 流水号
        ///// </summary>
        //public new string NumericalOrder { get; set; }
        ///// <summary>
        ///// 猪场ID
        ///// </summary>
        //public new string PigFarmId { get; set; }

        //public List<FmCostprojectExtendDto> ExtendDetails { get; set; }
    }
    public class FmCostprojectExtendDto : FmCostprojectExtend
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

    public class MutilLineCommand<T1> : FmCostproject, IRequest<Result> where T1 : new()
    {
        //public new string NumericalOrder { get; set; }
        public List<T1> Details { get; set; }
    }
    #endregion

    #region Command
    public class FM_CostProjectCommand : FM_CostProjectDto, IRequest<Result>
    {
    }
    public class FM_CostProjectAddCommand : FM_CostProjectDto, IRequest<Result>
    {

    }

    public class FM_CostProjectDeleteCommand : FM_CostProjectDto, IRequest<Result>
    {

    }

    public class FM_CostProjectModifyCommand : FM_CostProjectDto, IRequest<Result>
    {
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    public class FM_CostProjectBatchDelCommand : List<string>, IRequest<Result>
    {

    }

    /// <summary>
    /// 批量插入
    /// </summary>
    public class FM_CostProjectBatchAddCommand : FM_CostProjectDto, IRequest<Result>
    {
    }
    #endregion
}
