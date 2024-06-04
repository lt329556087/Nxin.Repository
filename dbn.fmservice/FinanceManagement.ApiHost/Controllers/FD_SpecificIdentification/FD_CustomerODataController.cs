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
    public class Biz_CustomerODataController : ODataController
    {
        FD_SpecificIdentificationODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public Biz_CustomerODataController(FD_SpecificIdentificationODataProvider prodiver, IIdentityService identityService)
        {
            _prodiver = prodiver;
            _identityService = identityService;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public IEnumerable<Biz_CustomerODataEntity> Get(ODataQueryOptions<Biz_CustomerODataEntity> odataqueryoptions, Uri uri)
        {
            var list = _prodiver.GetCustomerList(odataqueryoptions,uri).Result;

            return list;
        }
        #endregion 

    }
}
