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

namespace FinanceManagement.ApiHost.Controllers.FM_CostProject
{
    [Authorize]
    public class FM_CostProjectODataController : ODataController
    {
        FM_CostProjectODataProvider _queryProvider;

        public FM_CostProjectODataController(FM_CostProjectODataProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<FM_CostProjectEntity> Get()
        {
            return _queryProvider.GetDatas();
        }

        [HttpGet]
        [Route("GetSingleData")]
        [EnableQuery]
        public FM_CostProjectEntity Get(string key)
        {
            return _queryProvider.GetSingleData(key);
        }
    }
}
