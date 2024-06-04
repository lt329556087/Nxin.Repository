using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_Expense;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Seedwork.Security;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using FinanceManagement.Common.MonthEndCheckout;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_ExpenseController : ControllerBase
    {
        IMediator _mediator;
        FD_ExpenseODataProvider _provider;
        IIdentityService _identityService;
        FMAPIService _fMAPIService;
        IFD_ExpenseRepository _repository;
        private readonly string PreexpenseAppid = "1803081320210000101";//预付款申请
        private readonly string PurexpenseAppid = "1612091318520000101";//采购付款申请
        private readonly string ContractAppid = "1612030929440000101";//采购合同
        public FD_ExpenseController(IMediator mediator, FD_ExpenseODataProvider provider, IIdentityService identityService, FMAPIService fMAPIService, IFD_ExpenseRepository repository)
        {
            _mediator = mediator;
            _provider = provider;
            _identityService = identityService;
            _fMAPIService = fMAPIService;
            _repository = repository;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        [PermissionAuthorize(Permission.Retrieve)]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            if (data != null)
            {
                //收方信息
                if (data.Lines != null)
                {
                    data.Lines.ForEach(line => {
                        if (!string.IsNullOrEmpty(line.AccountInformation))
                        {
                            List<string> array = new List<string>();
                            switch (data.ExpenseType)
                            {
                                case "1612091318520000101"://采购付款申请  菏泽和昌饲料有限公司^山东单县农村商业银行时楼支行^9170117054242050000216
                                    array = line.AccountInformation.Split('^').ToList();
                                    break;
                                case "1803081320210000101"://预付款申请   账户名称:希杰(上海)商贸有限公司,开户行:中国银行上海市静安新城支行,银行账号:435165569905
                                    var splitStrs = line.AccountInformation.Split(',');
                                    var index = -1;
                                    foreach (var splitStr in splitStrs)
                                    {
                                        index = splitStr.IndexOf(':');
                                        if (index >= 0)
                                        {
                                            var str = splitStr.Substring(index + 1, splitStr.Length - index - 1);
                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                array.Add(str);
                                            }
                                        }
                                        else if (!string.IsNullOrEmpty(splitStr))
                                        {
                                            array.Add(splitStr);
                                        }
                                    }
                                    break;
                            }
                            if (array.Count > 0)
                            {
                                line.AccountName = array[0];
                            }
                            if (array.Count > 1)
                            {
                                line.BankDeposit = array[1];
                            }
                            if (array.Count > 2)
                            {
                                line.BankAccount = array[2];
                            }
                        }

                    });
                }
                //合同信息
                if(data.ExpenseType == PreexpenseAppid)
                {
                   var relateList = _provider.GetRelatedDatasAsync(PreexpenseAppid, ContractAppid, data.NumericalOrder,"");
                    relateList?.ForEach(p => {
                        p.HavePaidAmount += p.Payment;
                    });
                    data.RelatedConList = relateList;
                }
                //采购单信息
                if (data.ExpenseType == PurexpenseAppid)
                {
                    var relateList = _provider.GetRelatedPurchaseDatasAsync(PurexpenseAppid, PurexpenseAppid, data.NumericalOrder, "");
                    relateList?.ForEach(p => {
                        p.HavePaidAmount += p.Payment;
                    });
                    data.RelatedPurList = relateList;
                }
            }
            result.code = ErrorCode.Success.GetIntValue();
            result.data = data;
            return result;
        }
        

        //增加
        [HttpPost]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_ExpenseAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Delete([FromBody] FD_ExpenseDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FD_ExpenseModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //增加
        [HttpPost]
        [Route("AddPre")]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> AddPre([FromBody] FD_ExpensePreAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //修改
        [HttpPut]
        [Route("ModifyPre")]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FD_ExpensePreModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        #region 页面
        [HttpPost]
        [Route("GetPayableSummaryAmount")]
        public Result GetPayableSummaryAmount([FromBody] PayableSummaryModel model)
        {
            decimal amount = 0;
            var result = new Result() { data = amount };
            
            if(model.EnteID < 1){ result.msg = "EnteID有误"; return result;}
            if (string.IsNullOrEmpty(model.BeginDate)) { result.msg = "BeginDate为空"; return result; }
            if (string.IsNullOrEmpty(model.EndDate)) { result.msg = "EndDate为空"; return result; }
            if (string.IsNullOrEmpty(model.SupplierName)) { result.msg = "SupplierName为空"; return result; }
            if (model.OwnerID<1) { model.OwnerID =long.Parse( _identityService.UserId); }
            if (model.GroupID < 1) { model.GroupID = long.Parse(_identityService.GroupId); }

            //应付账款汇总表 
            var payList = _fMAPIService.PayableSummaryByCon(model, "supplier");
            if (payList?.Count > 0)
            {
                amount= payList.Sum(s => s.ReceiptAmount);
            }
            result.code = ErrorCode.Success.GetIntValue();
            result.data = amount;
            return result;
        }

        /// <summary>
        /// 详情申请类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetExpenseType/{key}")]
        //[PermissionAuthorize(Permission.Retrieve)]
        public async Task<Result> GetExpenseType(string key)
        {
            var result = new Result();
            if (!string.IsNullOrEmpty(key))
            {
                var data = await _repository.GetAsync(key);
                result.data = data;
            }           
            
            result.code = ErrorCode.Success.GetIntValue();
           
            return result;
        }
        #endregion
    }
}
