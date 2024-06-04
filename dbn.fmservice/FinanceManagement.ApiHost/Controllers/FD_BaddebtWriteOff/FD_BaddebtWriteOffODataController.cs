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
using FinanceManagement.Util;
using FinanceManagement.Infrastructure.Repositories.Interfaces;

namespace FinanceManagement.ApiHost.Controllers.FD_BaddebtWriteOff
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BaddebtWriteOffODataController : ODataController
    {
        FD_BaddebtWriteOffODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionDetailRepository _iProvisionDetailRepository;

        public FD_BaddebtWriteOffODataController(FD_BaddebtWriteOffODataProvider prodiver, IIdentityService identityService, IFD_BadDebtProvisionDetailRepository iProvisionDetailRepository)
        {
            _prodiver = prodiver;
            _iProvisionDetailRepository = iProvisionDetailRepository;
            _identityService = identityService;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [PermissionAuthorize(Permission.Retrieve)]
        public IEnumerable<FD_BaddebtWriteOffODataEntity> Get(ODataQueryOptions<FD_BaddebtWriteOffODataEntity> odataqueryoptions, Uri uri)
        {
            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value))?.ToList().AsQueryable();
            return list;
        }
        #endregion 



    }
}
