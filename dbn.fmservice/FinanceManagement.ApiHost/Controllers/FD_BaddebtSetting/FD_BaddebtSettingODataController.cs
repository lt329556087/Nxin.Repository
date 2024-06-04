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

namespace FinanceManagement.ApiHost.Controllers.OData
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BaddebtSettingODataController : ODataController
    {
        FD_BaddebtSettingODataProvider _prodiver;
        //private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        public FD_BaddebtSettingODataController(FD_BaddebtSettingODataProvider prodiver, IIdentityService identityService, IFD_BadDebtProvisionRepository iProvisionRepository)
        {
            _prodiver = prodiver;
            _iProvisionRepository = iProvisionRepository;
            _identityService = identityService;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [PermissionAuthorize(Permission.Retrieve)]
        public IEnumerable<FD_BaddebtSettingODataEntity> Get(ODataQueryOptions<FD_BaddebtSettingODataEntity> odataqueryoptions, Uri uri)
        {
            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList();
            foreach(var item in list)
            {
               item.IsProvision = _iProvisionRepository.IsSettingDataExist(item.NumericalOrder, _identityService.EnterpriseId);
            }
            return list.AsQueryable();
        }

        #endregion 



    }
}
