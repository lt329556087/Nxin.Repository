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

namespace FinanceManagement.ApiHost.Controllers.Nxin_DigitalIntelligenceMap
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class Nxin_DigitalIntelligenceMapApiController : ControllerBase
    {
        Nxin_DigitalIntelligenceMapODataProvider _prodiver;

        public Nxin_DigitalIntelligenceMapApiController(Nxin_DigitalIntelligenceMapODataProvider prodiver)
        {
            _prodiver = prodiver;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Get")]
        public List<Nxin_DigitalIntelligenceMapODataEntity> Get(Nxin_DigitalIntelligenceMapCommand model)
        {
            return _prodiver.Get(model).Result;
        }
        #endregion 

    }
}
