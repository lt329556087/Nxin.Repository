using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_PaymentReceivablesTODataController : ControllerBase
    {
        FD_PaymentReceivablesTODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        HttpClientUtil _httpClientUtil;
        FMBaseCommon _fmBaseCommon;
        public FD_PaymentReceivablesTODataController(IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, HttpClientUtil httpClientUtil,FMBaseCommon fMBaseCommon)
        {
            _prodiver = prodiver;
            _identityService = identityService;
            _httpClientUtil = httpClientUtil;
            _fmBaseCommon = fMBaseCommon;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery(MaxNodeCount = 10000)]
        [HttpGet]
        public IEnumerable<FD_PaymentReceivablesEntity> Get(ODataQueryOptions<FD_PaymentReceivablesEntity> odataqueryoptions, Uri uri)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return null;
            }
            var entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            return _prodiver.GetList(entes).ToList().AsQueryable(); ;
        }
        #endregion 

    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_PaymentReceivablesSummaryODataController : ControllerBase
    {
        FD_PaymentReceivablesTODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _fmBaseCommon;

        public FD_PaymentReceivablesSummaryODataController(
            FMBaseCommon fmBaseCommon,
            IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, HostConfiguration hostCongfiguration)
        {
            _prodiver = prodiver;
            _hostCongfiguration = hostCongfiguration;
            _identityService = identityService;
            _fmBaseCommon = fmBaseCommon;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery(MaxNodeCount = 10000)]
        [HttpGet]
        public IEnumerable<FD_PaymentReceivablesEntity> Get(ODataQueryOptions<FD_PaymentReceivablesEntity> odataqueryoptions, Uri uri)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return null;
            }
            var entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            return _prodiver.GetSummaryDatas(entes).ToList().AsQueryable();
        }
        #endregion 
        
    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_PaymentReceivablesMergeODataController : ControllerBase
    {
        FD_PaymentReceivablesTODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _fmBaseCommon;
        ILogger<FD_PaymentReceivablesMergeODataController> _logger;

        public FD_PaymentReceivablesMergeODataController(
            FMBaseCommon fmBaseCommon,
            ILogger<FD_PaymentReceivablesMergeODataController> logger,
            IIdentityService identityService,FD_PaymentReceivablesTODataProvider prodiver, HostConfiguration hostCongfiguration)
        {
            _prodiver = prodiver;
            _hostCongfiguration = hostCongfiguration;
            _identityService = identityService;
            _fmBaseCommon = fmBaseCommon;
            _logger = logger;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<FD_PaymentReceivablesEntity> Get(ODataQueryOptions<FD_PaymentReceivablesHeadEntity> odataqueryoptions, Uri uri)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return null;
            }
            // 判断是否存在收款名称过滤条件
            var IsColection = odataqueryoptions.Filter == null ? false : odataqueryoptions.Filter.RawValue.IndexOf("CollectionName") != -1;
            var entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            if (odataqueryoptions.Filter != null)
            {
                if (odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge ") > -1)
                {
                    string begindate = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge "), 24).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge "), 24).IndexOf("'")).Replace("'", "");
                    string enddate = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate le "), 24).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate le "), 24).IndexOf("'")).Replace("'", "");
                    var data =odataqueryoptions.ApplyTo(_prodiver.GetMergeDatasFilterNums(entes,begindate,enddate, IsColection).Distinct().AsQueryable()) as IQueryable<FD_PaymentReceivablesHeadEntity>;
                    if (data == null)
                    {
                        return new List<FD_PaymentReceivablesEntity>();
                    }
                    var finaly = _prodiver.GetMergeDatasList(string.Join(',', data.Select(m => m.NumericalOrder))).ToList();
                    return finaly.OrderByDescending(m=>m.DataDate).ToList();
                }
            }
            var list = odataqueryoptions.ApplyTo(_prodiver.GetMergeDatasFilterNums(entes,"","", IsColection).Distinct().AsQueryable()) as IQueryable<FD_PaymentReceivablesHeadEntity>;
            var listFinaly = _prodiver.GetMergeDatasList(string.Join(',', list.Select(m => m.NumericalOrder))).ToList();
            return listFinaly.OrderByDescending(m => m.DataDate).ToList();
        }
        #endregion

    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_ReceivablesMergeODataController : ControllerBase
    {
        FD_PaymentReceivablesTODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        FMBaseCommon _fmBaseCommon;
        public FD_ReceivablesMergeODataController(FMBaseCommon fmBaseCommon,IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil)
        {
            _prodiver = prodiver;
            _hostCongfiguration = hostCongfiguration;
            _identityService = identityService;
            _httpClientUtil = httpClientUtil;
            _fmBaseCommon = fmBaseCommon;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<FD_PaymentReceivablesEntity> Get(ODataQueryOptions<FD_PaymentReceivablesHeadEntity> odataqueryoptions, Uri uri)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return null;
            }
            //权限单位集合
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            // 判断是否存在收款名称过滤条件
            var IsColection = odataqueryoptions.Filter == null ? false : odataqueryoptions.Filter.RawValue.IndexOf("CollectionName") != -1;
            if (odataqueryoptions.Filter != null)
            {
                if (odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge ") > -1)
                {
                    string begindate = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge "), 24).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge "), 24).IndexOf("'")).Replace("'", "");
                    string enddate = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate le "), 24).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate le "), 24).IndexOf("'")).Replace("'", "");
                    var data = odataqueryoptions.ApplyTo(_prodiver.GetMergeDatasFilterNums(entes, begindate, enddate, IsColection, false).Distinct().AsQueryable()) as IQueryable<FD_PaymentReceivablesHeadEntity>;
                    if (data == null)
                    {
                        return new List<FD_PaymentReceivablesEntity>();
                    }
                    var finaly = _prodiver.GetReceivablesMergeDatas(string.Join(',', data.Select(m => m.NumericalOrder))).ToList();
                    var Amounts = _prodiver.GetReceiptAmouts(string.Join(',', data.Select(m => m.NumericalOrder)));
                    foreach (var item in finaly)
                    {
                        if (string.IsNullOrEmpty(item.CollectionName))
                        {
                            continue;
                        }
                        item.Amount = Amounts.Where(m => m.NumericalOrder == item.NumericalOrder).FirstOrDefault()?.Amount;
                        var temp = item.CollectionName.Split(',');
                        temp = temp.Distinct().ToArray();
                        item.CollectionName = string.Join(',', temp);
                    }
                    return finaly.OrderByDescending(m => m.DataDate).ToList();
                }
            }
            else
            {
                var list = odataqueryoptions.ApplyTo(_prodiver.GetMergeDatasFilterNums(entes, "", "", IsColection, false).Distinct().AsQueryable()) as IQueryable<FD_PaymentReceivablesHeadEntity>;
                var listFinaly = _prodiver.GetReceivablesMergeDatas(string.Join(',', list.Select(m => m.NumericalOrder))).ToList();
                var Amounts = _prodiver.GetReceiptAmouts(string.Join(',', list.Select(m => m.NumericalOrder)));
                foreach (var item in listFinaly)
                {
                    if (string.IsNullOrEmpty(item.CollectionName))
                    {
                        continue;
                    }
                    item.Amount = Amounts.Where(m => m.NumericalOrder == item.NumericalOrder).FirstOrDefault()?.Amount;
                    var temp = item.CollectionName.Split(',');
                    temp = temp.Distinct().ToArray();
                    item.CollectionName = string.Join(',', temp);
                }
                return listFinaly.OrderByDescending(m => m.DataDate).ToList();
            }
            return new List<FD_PaymentReceivablesEntity>();
        }
        #endregion

    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BankReceivablesODataController : ControllerBase
    {
        FD_PaymentReceivablesTODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _fmBaseCommon;
        public FD_BankReceivablesODataController(IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, HostConfiguration hostCongfiguration,FMBaseCommon fmBaseCommon)
        {
            _prodiver = prodiver;
            _hostCongfiguration = hostCongfiguration;
            _identityService = identityService;
            _fmBaseCommon = fmBaseCommon;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery(MaxNodeCount = 10000)]
        [HttpGet]
        public IEnumerable<BankReceivablesEntity> Get(ODataQueryOptions<FD_PaymentReceivablesEntity> odataqueryoptions, Uri uri)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return null;
            }
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            var list = _prodiver.GetBankReceivables(entes).ToList();
            var accountList = _prodiver.GetBankAccountInfo(entes).Where(m=> list.Select(m=>m.entId).Contains(m.EnterpriseID)).ToList();
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            foreach (var item in accountList)
            {
                var resultRece2 = encryptAccount.AccountNumberDecrypt(item.AccountNumber);
                item.AccountNumber = resultRece2?.Item1 == true ? resultRece2.Item2 : item.AccountNumber;
            }
            foreach (var item in list)
            {
                var temp = accountList.Where(m => m.AccountNumber == item.acctNo && m.EnterpriseID == item.entId).FirstOrDefault();
                if (temp != null)
                {
                    item.AccountName = temp.AccountName;
                    item.AccountId = temp.AccountId;
                }
                if (!string.IsNullOrEmpty(item.custList))
                {
                    var cList = JsonConvert.DeserializeObject<List<custList>>(item.custList);
                    item.custName = string.Join(",", cList.Select(m => m.custName).ToList());
                }
            }
            return list.AsQueryable();
        }
        #endregion

    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_PaymentReceivablesRecheckODataController : ControllerBase
    {
        FD_PaymentReceivablesTODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _fmBaseCommon;
        public FD_PaymentReceivablesRecheckODataController(IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, HostConfiguration hostCongfiguration,FMBaseCommon fmBaseCommon)
        {
            _prodiver = prodiver;
            _hostCongfiguration = hostCongfiguration;
            _identityService = identityService;
            _fmBaseCommon = fmBaseCommon;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<RecheckPaymentList> Get(ODataQueryOptions<RecheckPaymentList> odataqueryoptions, Uri uri)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return null;
            }
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            //后期性能慢 可以加日期过滤
            //查询未复核数据
            var isExitRstatus = odataqueryoptions.Filter.RawValue.IndexOf("RStatus eq 0");
            if (odataqueryoptions.Filter != null)
            {
                if (odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge ") > -1)
                {
                    string begindate = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge "), 24).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate ge "), 24).IndexOf("'")).Replace("'", "");
                    string enddate = odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate le "), 24).Substring(odataqueryoptions.Filter.RawValue.Substring(odataqueryoptions.Filter.RawValue.IndexOf("DataDate le "), 24).IndexOf("'")).Replace("'", "");
                
                    var querybale = odataqueryoptions.ApplyTo(_prodiver.GetRecheckPaymentList(entes, isExitRstatus,_identityService.UserId,begindate,enddate).AsQueryable()) as IQueryable<RecheckPaymentList>;
                    var data = querybale.ToList();
                    foreach (var item in data)
                    {
                        //解密银行卡号
                        var resultRece = encryptAccount.AccountNumberDecrypt(item.BankAccount);
                        item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;

                        var resultRece2 = encryptAccount.AccountNumberDecrypt(item.AccountNumber);
                        item.AccountNumber = resultRece2?.Item1 == true ? resultRece2.Item2 : item.AccountNumber;

                        //没有制单权限 数据脱敏
                        if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Report))
                        {
                            try
                            {
                                var temp = resultRece.Item1 ? item.BankAccount.Substring(4, resultRece.Item2.Length - 4) : "";
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    temp = item.BankAccount.Substring(0, item.BankAccount.Length - 4);
                                    item.BankAccount = item.BankAccount.Replace(temp, "****");
                                }

                                temp = resultRece2.Item1 ? item.AccountNumber.Substring(4, resultRece2.Item2.Length - 4) : "";
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    temp = item.AccountNumber.Substring(0, item.AccountNumber.Length - 4);
                                    item.AccountNumber = item.AccountNumber.Replace(temp, "****");
                                }
                            }
                            catch (Exception e)
                            {
                                LogHelper.LogInformation("加密失败"+e + JsonConvert.SerializeObject(item));
                            }
                        }
                    }
                    return data.AsQueryable();
                }
            }
            else
            {
                return new List<RecheckPaymentList>();
            }
            return new List<RecheckPaymentList>();
        }
        #endregion 
    }
}
