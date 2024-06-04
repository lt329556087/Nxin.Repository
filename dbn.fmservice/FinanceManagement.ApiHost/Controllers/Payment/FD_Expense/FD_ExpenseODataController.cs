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
using FinanceManagement.Common;
using System.Linq;



namespace FinanceManagement.ApiHost.Controllers.OData
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_ExpenseODataController : ODataController
    {
        FD_ExpenseODataProvider _prodiver;
        //private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        FMBaseCommon _baseUnit;

        public FD_ExpenseODataController(FD_ExpenseODataProvider prodiver, IIdentityService identityService, FMBaseCommon baseUnit)
        {
            _baseUnit = baseUnit;
            _prodiver = prodiver;
            _identityService = identityService;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [PermissionAuthorize(Permission.Retrieve)]
        public IEnumerable<FD_ExpenseODataEntity> Get(string beginDate, string endDate, ODataQueryOptions<FD_ExpenseODataEntity> odataqueryoptions, Uri uri)
        {
            if (string.IsNullOrEmpty(beginDate))
            {
                beginDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            return _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value),beginDate,endDate);
        }
        #endregion 



    }
}
