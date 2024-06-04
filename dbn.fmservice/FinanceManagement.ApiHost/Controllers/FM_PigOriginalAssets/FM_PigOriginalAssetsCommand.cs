using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_PigOriginalAssets
{
    #region Dto
    public class FM_PigOriginalAssetsDto : MutilLineCommand<FmPigoriginalassetsdetailDto>
    {

    }
    public class FmPigoriginalassetsdetailDto : FmPigoriginalassetsdetail
    {

    }
    public class MutilLineCommand<T1> : FmPigoriginalassets, IRequest<Result> where T1 : new()
    {
        //public new string NumericalOrder { get; set; }
        public List<T1> Details { get; set; }
    }
    #endregion

    #region Command
    public class FM_PigOriginalAssetsCommand : FM_PigOriginalAssetsDto, IRequest<Result>
    {
    }

    public class FM_PigOriginalAssetsAddCommand : FM_PigOriginalAssetsDto, IRequest<Result>
    {

    }

    public class FM_PigOriginalAssetsDeleteCommand : FM_PigOriginalAssetsDto, IRequest<Result>
    {

    }

    public class FM_PigOriginalAssetsModifyCommand : FM_PigOriginalAssetsDto, IRequest<Result>
    {
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    public class FM_PigOriginalAssetsBatchDelCommand : List<string>, IRequest<Result>
    {
         
    }
    #endregion
}
