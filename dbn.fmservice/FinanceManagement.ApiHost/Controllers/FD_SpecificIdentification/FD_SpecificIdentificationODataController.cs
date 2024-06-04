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

namespace FinanceManagement.ApiHost.Controllers.FD_SpecificIdentification
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_SpecificIdentificationODataController : ODataController
    {
        FD_SpecificIdentificationODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionDetailRepository _iProvisionDetailRepository;

        public FD_SpecificIdentificationODataController(FD_SpecificIdentificationODataProvider prodiver, IIdentityService identityService, IFD_BadDebtProvisionDetailRepository iProvisionDetailRepository)
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
        public IEnumerable<FD_SpecificIdentificationODataEntity> Get(ODataQueryOptions<FD_SpecificIdentificationODataEntity> odataqueryoptions, Uri uri)
        {
            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable();
            foreach (var item in list)
            {
                item.IsProvision = _iProvisionDetailRepository.IsSpecificDataExist(item.NumericalOrderDetail, _identityService.EnterpriseId);
            }
            return list;
        }
        #endregion 



    }
}
