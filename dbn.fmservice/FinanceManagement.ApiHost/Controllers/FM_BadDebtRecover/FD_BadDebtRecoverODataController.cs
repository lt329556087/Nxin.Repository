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

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtRecover
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BadDebtRecoverODataController : ControllerBase
    {
        FD_BadDebtRecoverODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FD_BadDebtRecoverODataController(FD_BadDebtRecoverODataProvider prodiver)
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
        public IEnumerable<FD_BadDebtRecoverListOnly> Get(ODataQueryOptions<FD_BadDebtRecoverListOnly> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 
    }

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_PaymentReceivablesODataController : ControllerBase
    {
        FD_PaymentReceivablesODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FD_PaymentReceivablesODataController(FD_PaymentReceivablesODataProvider prodiver)
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
        public IEnumerable<FD_PaymentReceivablesODataEntity> Get(ODataQueryOptions<FD_PaymentReceivablesODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetMainList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 
    }


}
