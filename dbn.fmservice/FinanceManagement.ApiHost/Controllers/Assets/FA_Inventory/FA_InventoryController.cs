using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FA_Inventory;
using FinanceManagement.Common;
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
using Microsoft.AspNet.OData.Query;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Serilog.Core;
using Microsoft.Extensions.Logging;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;
namespace FinanceManagement.ApiHost.Controllers.Api
{
    /*资产盘点*/
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_InventoryController : ControllerBase
    {
        IMediator _mediator;
        FA_InventoryODataProvider _provider;
        private readonly ILogger<FA_InventoryController> _logger;
        private IIdentityService _identityService;
        //private IFA_InventoryRepository _repository;
        //private FinanceTradeUtil _financeTradeUtil;
        //private FundSummaryUtil _fundSummaryUtil;
        //HostConfiguration _hostCongfiguration;
        public FA_InventoryController(IMediator mediator, FA_InventoryODataProvider provider, ILogger<FA_InventoryController> logger, IIdentityService identityService
            )
        {
            _mediator = mediator;
            _provider = provider;
            _logger = logger;
            _identityService = identityService;           
        }


        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        //[PermissionAuthorize(Permission.Retrieve)]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            if (data?.Lines?.Count > 0)
            {
                data.Lines.ForEach(line =>
                {
                    line.QuantityDiff = line.Quantity - line.InventoryQuantity;
                    var filenames = line.FileName.Split(',');
                    var PathUrls = line.PathUrl.Split(',');
                    line.FileModels = new List<FMFileModel>();
                    for (int i = 0; i < filenames.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(filenames[i]))
                        {
                            line.FileModels.Add(new FMFileModel()
                            {
                                FileName = filenames[i],
                                PathUrl = PathUrls[i],
                            });
                        }                        
                    }
                });
            }
            
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }

        //增加
        [HttpPost]
        [AllowAnonymous]
        //[PermissionAuthorize(Permission.Create)]
        public async Task<Result> Add([FromBody] FA_InventoryAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FA_InventoryDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        //[PermissionAuthorize(Permission.Update)]
        public async Task<Result> Modify([FromBody] FA_InventoryModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        
        [HttpPost("GetAssetscardList")]
        public CardResult GetAssetscardList(AssetscardSearch param)
        {
            var result = new CardResult() { code = -1 };
            if (param == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            //if (string.IsNullOrEmpty(param.FAPlaceID))
            //{
            //    result.msg = "FAPlaceID不能为空";
            //    return result;
            //}
            var list =_provider.GetAssetscardList(param);
            result.data = list;
            if (list==null||list.Count==0)
            {
                result.code = 1;
                result.msg = "无可盘点资产，请检查是否有资金卡片信息";
                return result;
            }
            //排除当天做过盘点单的卡片
            var existCardIDList = _provider.GetInventoryDetailByDate(param);
            if (existCardIDList?.Count > 0)
            {
                foreach(var item in  existCardIDList)
                {
                    list.RemoveAll(p => p.CardID == item.id);
                }
                if (list == null || list.Count == 0)
                {
                    var existmodelList = existCardIDList.GroupBy(p=>p.name);
                    result.code = 2;
                    result.msg = "";
                    result.errorinfo = existmodelList.Select(p=> new { NumericalOrder=p.Key,Number=p.FirstOrDefault().curtype });
                    return result;
                }
            }
            result.code = 0;           
            return result;
        }
    }
}
