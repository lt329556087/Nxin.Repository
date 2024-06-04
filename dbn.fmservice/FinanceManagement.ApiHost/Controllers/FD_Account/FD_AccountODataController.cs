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
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.Repositories.Interfaces;

namespace FinanceManagement.ApiHost.Controllers.OData
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AccountODataController : ODataController
    {
        FD_AccountODataProvider _prodiver;
        //private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        HostConfiguration _hostCongfiguration;
        public FD_AccountODataController(FD_AccountODataProvider prodiver, IIdentityService identityService, IFD_BadDebtProvisionRepository iProvisionRepository, HostConfiguration hostCongfiguration)
        {
            _prodiver = prodiver;
            _iProvisionRepository = iProvisionRepository;
            _identityService = identityService;
            _hostCongfiguration = hostCongfiguration;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public IEnumerable<FD_AccountAODataEntity> Get(ODataQueryOptions<FD_AccountAODataEntity> odataqueryoptions, Uri uri)
        {
            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value))?.ToList();
            if(list==null)return new List<FD_AccountAODataEntity>().AsQueryable();
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.AccountNumber)) continue;
                EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                //解密银行卡号
                var resultNum = encryptAccount.AccountNumberDecrypt(item.AccountNumber);
                item.AccountNumber = resultNum?.Item1 == true ? resultNum.Item2 : item.AccountNumber;
            }           

            return list.AsQueryable();
        }

        #endregion 



    }
}
