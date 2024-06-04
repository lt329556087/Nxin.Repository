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


namespace FinanceManagement.ApiHost.Controllers.OData
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_CapitalBudgetODataController : ODataController
    {
        FD_CapitalBudgetODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;

        public FD_CapitalBudgetODataController(FD_CapitalBudgetODataProvider prodiver)
        {
            _prodiver = prodiver;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public IEnumerable<FD_CapitalBudgetODataEntity> Get(ODataQueryOptions<FD_CapitalBudgetODataEntity> odataqueryoptions, Uri uri)
        {
            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable();

            list.ToList().ForEach(o=>{
                if (DateTime.TryParse(o.EndDate,out DateTime et))
                {
                    o.Duration = "第" + Utility.WeekOfYear(et) + "周";
                }
            });
            return list;
        }
        #endregion 



    }
}
