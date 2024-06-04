using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using FinanceManagement.Common;
using System.Threading.Tasks;
using FinanceManagement.Util;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Architecture.Common.HttpClientUtil;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FinanceManagement.ApiHost.Controllers.FA_Inventory
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_InventoryODataController : ODataController
    {
        FA_InventoryODataProvider _prodiver;
        private IIdentityService _identityService;
        //HttpClientUtil _httpClientUtil1;
        //HostConfiguration _hostCongfiguration;
        private readonly ILogger<FA_InventoryODataController> _logger;

        public FA_InventoryODataController(FA_InventoryODataProvider prodiver, IIdentityService identityService, ILogger<FA_InventoryODataController> logger)
        {
            _prodiver = prodiver;
            _identityService = identityService;
            _logger = logger;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public IQueryable<FA_InventoryODataEntity> Get(ODataQueryOptions<FA_InventoryODataEntity> odataqueryoptions, Uri uri)
        {
            if (string.IsNullOrEmpty(_identityService.EnterpriseId))
            {
                return new List<FA_InventoryODataEntity>().AsQueryable();  
            }
            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value))?.ToList();
            list?.ForEach(item => { item.QuantityDiff=item.Quantity-item.InventoryQuantity; });
            return list?.AsQueryable();
        }
        #endregion


    }
}
