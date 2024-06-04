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

namespace FinanceManagement.ApiHost.Controllers.FA_PigAssetsReset
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_PigAssetsResetODataController : ControllerBase
    {
        FA_PigAssetsResetODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FA_PigAssetsResetODataController(FA_PigAssetsResetODataProvider prodiver)
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
        public IEnumerable<FA_PigAssetsResetListODataEntity> Get(ODataQueryOptions<FA_PigAssetsResetListODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 
    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_AssetsInspectMobileODataController : ControllerBase
    {
        FA_PigAssetsResetODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FA_AssetsInspectMobileODataController(FA_PigAssetsResetODataProvider prodiver)
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
        public IEnumerable<FA_AssetsInspectMobileODataEntity> Get(ODataQueryOptions<FA_AssetsInspectMobileODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetAssetsInspectMobileAsync(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); 
        }
        #endregion 
    }

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_AssetsCardMobileODataController : ControllerBase
    {
        FA_PigAssetsResetODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FA_AssetsCardMobileODataController(FA_PigAssetsResetODataProvider prodiver)
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
        public IEnumerable<FA_AssetsCardMobileODataEntity> Get(ODataQueryOptions<FA_AssetsCardMobileODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetAssetsCardMobileAsync(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 
    }
}
