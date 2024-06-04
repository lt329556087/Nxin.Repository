using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class fd_schedulesetODataController : ControllerBase
    {
        fd_schedulesetODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IMediator _mediator;

        public fd_schedulesetODataController(IMediator mediator, fd_schedulesetODataProvider prodiver)
        {
            _mediator = mediator;
            _prodiver = prodiver;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// 初始设置默认值
        /// </summary>
        /// <returns></returns>
        [EnableQuery(MaxNodeCount = 10000)]
        [HttpGet]
        public async Task<IEnumerable<fd_scheduleset>> GetAsync(ODataQueryOptions<fd_scheduleset> odataqueryoptions, Uri uri)
        {
            var setList = _prodiver.GetList().ToList();
            if (setList.Count == 0)
            {
                fd_schedulesetSaveCommand data = new fd_schedulesetSaveCommand();
                for (int i = 1; i <= 3; i++)
                {
                    data.Add(new fd_scheduleset() { Level = i, StayDay = i });
                }
                await _mediator.Send(data, HttpContext.RequestAborted);
                return await GetAsync(null,null);
            }
            else
            {
                return setList.OrderBy(m=>m.Level).AsQueryable();
            }
        }
        #endregion 

    }
}
