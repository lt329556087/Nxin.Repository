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

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsApply
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_AssetsApplyODataController : ControllerBase
    {
        FA_AssetsApplyODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FA_AssetsApplyODataController(FA_AssetsApplyODataProvider prodiver)
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
        public IEnumerable<FA_AssetsApplyListODataEntity> Get(ODataQueryOptions<FA_AssetsApplyListODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetList(odataqueryoptions, null).ToList().AsQueryable(); 
        }
        #endregion 

    }
        [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_AssetsApplyMobileODataController : ControllerBase
    {
        FA_AssetsApplyODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FA_AssetsApplyMobileODataController(FA_AssetsApplyODataProvider prodiver)
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
        public IEnumerable<FA_AssetsApplyMobileListODataEntity> Get(ODataQueryOptions<FA_AssetsApplyMobileListODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetAssetsApplyMobileList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 

    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_AssetsApplyMobileByInspectODataController : ControllerBase
    {
        FA_AssetsApplyODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FA_AssetsApplyMobileByInspectODataController(FA_AssetsApplyODataProvider prodiver)
        {
            _prodiver = prodiver;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public IEnumerable<FA_AssetsApplyMobileByInspectListODataEntity> Get(ODataQueryOptions<FA_AssetsApplyMobileByInspectListODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetAssetsApplyMobileListByInspect(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 

    }
}
