using Architecture.Common.Util;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_AccountInventory;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FinanceManagement.ApiHost.Applications.Queries.FM_CashSweepODataProvider;

namespace FinanceManagement.ApiHost.Controllers.FD_AccountInventory
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AccountInventoryController : ControllerBase
    {
        IMediator _mediator;
        FD_AccountInventoryODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;
        FD_AccountODataProvider _accountProvider;
        FMBaseCommon _fmbaseCommon;
        FinanceTradeUtil _financeutil;
        HostConfiguration _hostCongfiguration;

        public FD_AccountInventoryController(IMediator mediator, FD_AccountInventoryODataProvider provider, FundSummaryUtil fundSummaryUtil, IIdentityService identityService, FD_AccountODataProvider accountProvider, FMBaseCommon fmbaseCommon, FinanceTradeUtil financeutil, HostConfiguration hostCongfiguration)
        {
            _mediator = mediator;
            _provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
            _accountProvider = accountProvider;
            _fmbaseCommon = fmbaseCommon;
            _financeutil = financeutil;
            _hostCongfiguration = hostCongfiguration;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_AccountInventoryAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_AccountInventoryDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_AccountInventoryModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }


        //增加
        [HttpPost, Route("GetFundSummary")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> GetFundSummary([FromBody] FD_AccountInventoryAddCommand request)
        {
            var result = new Result();
            var param = new FundSummaryRequest()
            {
                GroupId = _identityService.GroupId,
                EnterpriseId = _identityService.EnterpriseId,
                BoId = _identityService.UserId,
                OwnEntes = new List<string>() { _identityService.EnterpriseId },
                EndDate = request.DataDate,
                BeginDate = request.DataDate
            };

            var data = await _fundSummaryUtil.GetData(param);
            data.ForEach(o => { o.SummaryType = o.SummaryType?.Split('~')[0]; });
            if (!string.IsNullOrEmpty(request.NumericalOrder))
            {
                //var enddingAmount = data.Find(o => o.SummaryType == request.NumericalOrder)?.EnddingAmount;
                data= data.Where(o => o.SummaryType == request.NumericalOrder).ToList();
            }

            result.data = data;
            //result.data = enddingAmount;
            return result;
        }


        /// <summary>
        /// 查询负责人账户 金融历史余额
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost, Route("GetResponsiblePersonAccount")]
        public Result GetResponsiblePersonAccount([FromBody] FDAccountSearch req)
        {
            var result = new Result();
            var list = new List<FD_AccountInventoryDetailODataEntity>();
            if (req == null)
            {
                result.msg = "参数不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.DataDate))
            {
                result.msg = "DataDate不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(req.ResponsiblePerson))
            {
                result.msg = "ResponsiblePerson不能为空";
                return result;
            }
            req.EnterpriseID = _identityService.EnterpriseId;
            var accountList = _accountProvider.GetList(null,null);
            if(accountList?.Count()> 0)
            {
                var personaccountList=accountList.Where(p=>p.ResponsiblePerson==req.ResponsiblePerson&&p.IsUse==1)?.ToList();
                if(personaccountList==null||personaccountList.Count==0) { return SetResult(result,list);  }
                //查询历史余额
                if (req.IsSearchBal)
                {
                    var searchList=new List<FD_AccountAODataEntity>();
                    var nobankaccountList = personaccountList.Where(p => p.OpenBankEnterConnect == false);
                    foreach (var item in nobankaccountList)
                    {
                        list.Add(SetDetailValue(item, 0, ""));
                    }
                    var bankaccountList = personaccountList.Where(p => p.OpenBankEnterConnect==true);
                    foreach(var group in bankaccountList.GroupBy(p=>p.BankID))
                    {
                        var bankType = _fmbaseCommon.ConvertBankType(group.Key);
                        if (string.IsNullOrEmpty(bankType))
                        {
                            foreach(var item in group)
                            {
                                list.Add(SetDetailValue(item, 0, ""));
                            }
                        }
                        else
                        {
                            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                            foreach (var item in group)
                            {
                                //解密银行卡号
                                var resultNum = encryptAccount.AccountNumberDecrypt(item.AccountNumber);
                                item.AccountNumber = resultNum?.Item1 == true ? resultNum.Item2 : item.AccountNumber;
                                item.BankType = bankType;
                                var remark = ValidData(item);
                                if (string.IsNullOrEmpty(remark))
                                {
                                    item.BankType = bankType;
                                    searchList.Add(item);
                                }
                                else
                                {
                                    list.Add(SetDetailValue(item, 0, remark));
                                }
                            }                                
                        }
                    }
                    if (searchList.Any())
                    {
                        var banklist = GetBalValue(req, searchList);
                        list.AddRange(banklist);
                    }
                }
                else
                {
                    foreach(var item in personaccountList)
                    {
                        list.Add(SetDetailValue(item,0,""));
                    }
                }
            }

            return SetResult(result, list); 
        }
        private List<FD_AccountInventoryDetailODataEntity> GetBalValue(FDAccountSearch req,List<FD_AccountAODataEntity> banklist)
        {
            var list = new List<FD_AccountInventoryDetailODataEntity>();
            var responseStr=_financeutil.GetYqtAccountBalByDate(req,banklist);
            var remark = "";
            var nobal = false;
            if (!string.IsNullOrEmpty(responseStr))
            {
                var accountBalModel = JsonConvert.DeserializeObject<RestfulResult>(responseStr);

                if (accountBalModel == null|| accountBalModel.code != 0|| accountBalModel.data == null) { 
                    nobal = true;                    
                }
                else
                {
                    var balList = JsonConvert.DeserializeObject<List<BalDataModel>>(accountBalModel.data.ToString());
                    foreach(var item in banklist)
                    {
                        var newItem=SetDetailValue(item, 0, "");
                        var filterData = balList.Where(p => p.accNo == item.AccountNumber).ToList();
                        if (filterData != null && filterData.Count() > 0)
                        {
                            var result = filterData.FirstOrDefault();
                            if (result.resCode == "1")//成功
                            {
                                decimal acctBal = 0;
                                decimal.TryParse(string.IsNullOrEmpty(result.accBal) ? "0" : result.accBal, out acctBal);
                                newItem.FlowAmount = acctBal;// result.accBal;
                                newItem.Remarks = result.resMsg;
                            }
                            else if (result.detailCode == "66600007")//在限定时间内查询无记录 查询前一天的盘点数据
                            {
                                req.AccountID = item.AccountID;
                                req.DataDate = DateTime.Parse(req.DataDate).AddDays(-1).ToString("yyyy-MM-dd");
                                var flowamount =_provider.GetAccountinventory(req);
                                newItem.FlowAmount = flowamount;
                                newItem.Remarks = "";
                            }
                            else
                            {
                                newItem.FlowAmount = 0;
                                newItem.Remarks = result.resMsg + ";" + result.resCode + "-" + result.detailCode;
                            }
                        }
                        list.Add(newItem);
                    }                    
                }
            }
            else
            {
                nobal = true;
            }
            if(nobal)
            {
                foreach (var item in banklist)
                {
                    list.Add(SetDetailValue(item, 0, remark));
                }
            }
            return list;
        }
        private FD_AccountInventoryDetailODataEntity SetDetailValue(FD_AccountAODataEntity detail,decimal amount,string remarks)
        {
            var row = new FD_AccountInventoryDetailODataEntity();
            row.AccountID = detail.AccountID;
            row.AccountName = detail.AccountName;
            row.AccoSubjectID = detail.AccoSubjectID;
            row.AccoSubjectCode = detail.AccoSubjectCode;
            row.cAccoSubjectFullName = detail.AccoSubjectName;
            row.cAccoSubjectName = detail.AccoSubjectName;
            row.FlowAmount = amount;
            row.DepositAmount = 0;
            row.FrozeAmount = 0;
            row.Remarks = remarks;
            return row;
        }
        private Result SetResult(Result result,List<FD_AccountInventoryDetailODataEntity> list)
        {
            result.code= ErrorCode.Success.GetIntValue();
            result.data= list;
            return result;
        }
        private string ValidData(FD_AccountAODataEntity item)
        {
            var msg = "";
            if (string.IsNullOrEmpty(item.AccountNumber) || item.AccountNumber == "0")
            {
                msg += "资金账户设置[银行账号]不能为空!";
            }
            if (string.IsNullOrEmpty(item.BankType))
            {
                msg += "[银行类型]为空!";
            }
            if ((string.IsNullOrEmpty(item.BankNumber) || item.BankNumber == "0"))
            {
                msg += "资金账户设置[网银客户号]不能为空!";
            }
            return msg;
        }



    }
}
