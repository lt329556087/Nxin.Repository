using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Auxiliary
{
    public class FD_AuxiliaryDeleteCommand : IRequest<Result>
    {

        [Key]
        public string ProjectId { get; set; }
    }

    public class FD_AuxiliaryAddCommand : FD_AuxiliaryCommand, IRequest<Result>
    {
    }

    public class FD_AuxiliaryModifyCommand : FD_AuxiliaryCommand, IRequest<Result>
    {
    }

    public class FD_AuxiliaryCommand : fd_auxiliaryproject
    {

        
    }
    #region 自定义类型
    public class FD_AuxiliaryTypeDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FD_AuxiliaryTypeAddCommand : FD_AuxiliaryTypeCommand, IRequest<Result>
    {
    }

    public class FD_AuxiliaryTypeModifyCommand : FD_AuxiliaryTypeCommand, IRequest<Result>
    {
    }

    public class FD_AuxiliaryTypeCommand : fd_auxiliarytype
    {


    }
    #endregion
}
