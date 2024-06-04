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
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsContract
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_AssetsContractODataController : ControllerBase
    {
        FA_AssetsContractODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FA_AssetsContractODataController(FA_AssetsContractODataProvider prodiver)
        {
            _prodiver = prodiver;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IEnumerable<FA_AssetsContractListODataEntity> Get(ODataQueryOptions<FA_AssetsContractListODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 

    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_AssetsContractMobileByInspectODataController : ControllerBase
    {
        FA_AssetsContractODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FA_AssetsContractMobileByInspectODataController(FA_AssetsContractODataProvider prodiver)
        {
            _prodiver = prodiver;
        }
        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IEnumerable<FA_AssetsContractMobileListODataEntity> Get(ODataQueryOptions<FA_AssetsContractMobileListODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetAssetsContractMobileListByInspect(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 

    }
}
