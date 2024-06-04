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

namespace FinanceManagement.ApiHost.Controllers.FM_NewCarryForwardVoucher
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_NewCarryForwardVoucherRecordODataController : ControllerBase
    {
        FM_NewCarryForwardVoucherODataProvider _prodiver;

        public FM_NewCarryForwardVoucherRecordODataController(FM_NewCarryForwardVoucherODataProvider prodiver)
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
        public IEnumerable<FM_NewCarryForwardVoucherRecordODataEntity> Get(ODataQueryOptions<FM_NewCarryForwardVoucherRecordODataEntity> odataqueryoptions, Uri uri)
        {
            return _prodiver.GetRecordList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList().AsQueryable(); ;
        }
        #endregion 

    }
}
