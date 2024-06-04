using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using FinanceManagement.Common.NewMakeVoucherCommon;
using System.Net.Http.Headers;
using DBN.EncrypDecryp;
using FinanceManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Seedwork.Core;
namespace FinanceManagement.Common
{
    public class AssetsCommonUtil
    {
        HttpClientUtil _httpClientUtil1;

        HostConfiguration _hostCongfiguration;
        private QlwCrossDbContext _context;
        private readonly ValidateDataDateService _validateDataDateService;
        public AssetsCommonUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, QlwCrossDbContext context, ValidateDataDateService validateDataDateService)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _context = context;
            _validateDataDateService = validateDataDateService;
        }
        /// <summary>
        /// 是否已计提
        /// </summary>
        public async Task<ResultModel> GetIsAccrued(string enterID,string date)
        {
            var res = await _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/FA_AssetsCard/GetIsAccrued", new { EnterpriseID = enterID, DataDate=date });
            return res;
        }
        //调整单是否可以调整
        //status:A/M/D
        public async Task<Result> GetIsAjust(ValidateCheckOutInput checkReq,string status="A")
        {
            var result = new Result();
            var date = checkReq.DataDate;
            #region 是否已结账
            if (status != "D")
            {
                checkReq.DataDate = checkReq.DataDate.AddMonths(-1);
                var lastvalidDataDate = await _validateDataDateService.ValidateCheckOut(checkReq);

                if (!lastvalidDataDate.ResultState)
                {
                    result.msg = "当月不能调整，请结完上月账后才能调整！";
                    return result;
                }
            }
            checkReq.DataDate = date;
            var validDataDate = await _validateDataDateService.ValidateCheckOut(checkReq);

            if (validDataDate.ResultState)
            {
                result.msg = "当期已经结账不能操作，请先到资产结账取消结账";
                return result;
            }
            #endregion
            var IsAccruedResult = await GetIsAccrued(checkReq.EnterpriseID, checkReq.DataDate.ToString());
            if (IsAccruedResult.ResultState)
            {
                result.msg = "当期已经计提折旧,不能操作!";
                return result;
            }
            return result;
        }
    }
}
