using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_Account;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Seedwork.Security;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.ApiHost.Controllers.FD_Auxiliary;
using FinanceManagement.Domain;

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AuxiliaryController : ControllerBase
    {
        IMediator _mediator;
        FD_AuxiliaryODataProvider _provider;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _baseUnit;
        Ifd_auxiliarytypeRepository _auxiliarytypeRepository;
        public FD_AuxiliaryController(IMediator mediator, FD_AuxiliaryODataProvider provider, HostConfiguration hostCongfiguration, FMBaseCommon baseUnit, Ifd_auxiliarytypeRepository auxiliarytypeRepository)
        {
            _mediator = mediator;
            _provider = provider;
            _hostCongfiguration = hostCongfiguration;
            _baseUnit = baseUnit;
            _auxiliarytypeRepository = auxiliarytypeRepository;
        }

        //增加
        [HttpPost]
        [Route("AddProject")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_AuxiliaryAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        [Route("DeleteProject")]
        public async Task<Result> Delete([FromBody] FD_AuxiliaryDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPost]
        [Route("ModifyProject")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FD_AuxiliaryModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }


        //增加
        [HttpPost]
        [Route("AddType")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_AuxiliaryTypeAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        [Route("DeleteType")]
        public async Task<Result> Delete([FromBody] FD_AuxiliaryTypeDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPost]
        [Route("ModifyType")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FD_AuxiliaryTypeModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("GetAuxiliaryTypeByTag")]
        public Result<List<fd_auxiliarytype>> GetAuxiliaryTypeByTag(fd_auxiliarytype model)
        {
            if (model == null)
            {
                return new Result<List<fd_auxiliarytype>>() { code = 500,msg = "参数不可为空" };
            }
            if (string.IsNullOrEmpty(model.GroupId))
            {
                return new Result<List<fd_auxiliarytype>>() { code = 500, msg = "集团ID不可为空" };
            }
            if (string.IsNullOrEmpty(model.TypeTag))
            {
                model.TypeTag = "2311151408380000101";
            }
            var data = _auxiliarytypeRepository.GetAuxiliaryTypeByTag(model);
            return new Result<List<fd_auxiliarytype>>()
            {
                code = ErrorCode.Success.GetIntValue(),
                data = data,
                msg = "OK"
            };
        }
    }
}
