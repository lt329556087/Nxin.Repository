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
    public class FD_BaddebtGroupSettingODataController : ODataController
    {
        FD_BaddebtGroupSettingODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        public FD_BaddebtGroupSettingODataController(FD_BaddebtGroupSettingODataProvider prodiver, IIdentityService identityService)//, IFD_BadDebtProvisionRepository iProvisionRepository)
        {
            _prodiver = prodiver;
           // _iProvisionRepository = iProvisionRepository;
            _identityService = identityService;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [PermissionAuthorize(Permission.Retrieve)]
        public IEnumerable<FD_BaddebtGroupSettingODataEntity> Get(ODataQueryOptions<FD_BaddebtGroupSettingODataEntity> odataqueryoptions, Uri uri)
        {
            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList();
            foreach (var item in list)
            {
                var extends = _prodiver.GetExtendDatasAsync(long.Parse(item.NumericalOrder))?.Result;
                if (extends != null &&extends.Count > 0)
                {
                    var arrayid = extends.Select(p => p.EnterpriseID);
                    item.EnterpriseIDs = string.Join(',', arrayid);
                    var arrayname = extends.Select(p => p.EnterpriseName);
                    item.EnterpriseNames = string.Join(',', arrayname);
                }
            }
            return list.AsQueryable();
        }

        #endregion 



    }
}
