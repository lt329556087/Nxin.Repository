using Architecture.Common.Util.Extensions;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MailKit.Search;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using MP.MiddlePlatform.Integration.Integaration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_SettleReceiptNewODataController : ControllerBase
    {
        FD_SettleReceiptNewODataProvider _provider;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        FMBaseCommon _fmBaseCommon;

        public FD_SettleReceiptNewODataController(FD_SettleReceiptNewODataProvider provider,IIdentityService identityService,FMBaseCommon fmBaseCommon)
        {
            _provider = provider;
            _identityService = identityService;
            _fmBaseCommon = fmBaseCommon;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [PermissionAuthorize(Permission.Retrieve)]
        [EnableQuery(EnsureStableOrdering = false)]
        [HttpGet]
        public IQueryable<FD_SettleReceiptEntity> Get(ODataQueryOptions<FD_SettleReceiptEntity> odataqueryoptions, Uri uri)
        {
            if (odataqueryoptions.Filter != null)
            {
                if (odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge ") > -1)
                {
                    var entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
                    string begindate = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge "), 24).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge "), 24).IndexOf("'")).Replace("'", "");
                    string enddate = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate le "), 24).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate le "), 24).IndexOf("'")).Replace("'", "");
                    string EnterpriseID = "";
                    if (odataqueryoptions.Filter.RawValue.IndexOf("EnterpriseID eq ") > -1)
                    {
                        EnterpriseID = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("EnterpriseID eq "), odataqueryoptions.Filter.RawValue.IndexOf(")) and ((DataDate ge") - odataqueryoptions.Filter.RawValue.IndexOf("EnterpriseID eq ")).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("EnterpriseID eq "), odataqueryoptions.Filter.RawValue.IndexOf(")) and ((DataDate ge") - odataqueryoptions.Filter.RawValue.IndexOf("EnterpriseID eq ")).IndexOf("'")).Replace("'", "");
                    }
                    //兼容 会计凭证集团级 多选单位异常 （odata 会 增加 or 条件 多选）
                    if (odataqueryoptions.Filter.RawValue.IndexOf(") or (EnterpriseID eq ") > -1) 
                    {
                        EnterpriseID = EnterpriseID.Replace(") or (EnterpriseID eq ", ",");
                    }
                    var finaly = _provider.GetDataList(begindate, enddate, string.IsNullOrEmpty(EnterpriseID) ? entes : EnterpriseID, "");
                    return finaly.AsQueryable();
                }
            }
            var NullList = new List<FD_SettleReceiptEntity>();
            return NullList.AsQueryable();
        }
        #endregion 

    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherHandleODataController : ControllerBase
    {
        FD_SettleReceiptNewODataProvider  _provider;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        FMBaseCommon _fmBaseCommon;
        public VoucherHandleODataController(FD_SettleReceiptNewODataProvider provider,IIdentityService identityService,FMBaseCommon fmBaseCommon)
        {
            _provider = provider;
            _identityService = identityService;
            _fmBaseCommon = fmBaseCommon;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery(MaxNodeCount = 10000)]
        [PermissionAuthorize(Permission.Retrieve)]
        [HttpGet]
        public IQueryable<VoucherHandleInfoEntity> Get(ODataQueryOptions<VoucherHandleInfoEntity> odataqueryoptions, Uri uri)
        {
            var entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            return _provider.GetVoucherHandleInfoEntities(entes).ToList().AsQueryable();
        }
        #endregion 

    }
}
