using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Architecture.Common.HttpClientUtil;
using FinanceManagement.Common;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
namespace FinanceManagement.ApiHost.Controllers.FA_UseStateTransfer
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FA_UseStateTransferAddHandler : IRequestHandler<FA_UseStateTransferAddCommand, Result>
    {
        IIdentityService _identityService;
        IFA_UseStateTransferRepository _repository;
        IFA_UseStateTransferDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration ;
        private readonly AssetsCommonUtil _assetsCommonUtil;
        private readonly ILogger<Domain.FA_UseStateTransfer> _logger;
        public FA_UseStateTransferAddHandler(IIdentityService identityService, IFA_UseStateTransferRepository repository, IFA_UseStateTransferDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, HttpClientUtil httpClientUtil1, HostConfiguration hostCongfiguration, AssetsCommonUtil assetsCommonUtil, ILogger<Domain.FA_UseStateTransfer> logger)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _httpClientUtil1 = httpClientUtil1;
            _hostCongfiguration = hostCongfiguration;
            _assetsCommonUtil = assetsCommonUtil;
            _logger = logger;
        }

        public async Task<Result> Handle(FA_UseStateTransferAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                #region 是否可调整

                var checkReq = new ValidateCheckOutInput()
                {
                    AppID = _identityService.AppId,
                    DataDate = request.DataDate,
                    EnterpriseID = _identityService.EnterpriseId,
                    OwnerID = _identityService.UserId
                };
                result =await _assetsCommonUtil.GetIsAjust(checkReq, "A");
                if (!string.IsNullOrEmpty(result.msg)) { return result; }              
                #endregion
                long number = _numberCreator.Create<Domain.FA_UseStateTransfer>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                if (string.IsNullOrEmpty(request.EnterpriseID)|| request.EnterpriseID=="0")
                {
                    request.EnterpriseID = _identityService.EnterpriseId;
                }
                var domain = new Domain.FA_UseStateTransfer()
                {
                    DataDate=request.DataDate,
                    Number =  number.ToString(),
                    Remarks=request.Remarks,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID
                };
                request.Lines?.ForEach(o =>
                {
                    domain.AddDetail(new FA_UseStateTransferDetail()
                    {
                        NumericalOrder = numericalOrder,
                        CardID =o.CardID,
                        BeforeUseStateID = string.IsNullOrEmpty(o.BeforeUseStateID) ? "0" : o.BeforeUseStateID,
                        AfterUseStateID=o.AfterUseStateID,
                        Remarks=o.Remarks,
                        ModifiedDate = DateTime.Now
                    });
                });
                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, request.OwnerID).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.AddAsync(domain);
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                //_logger.LogInformation("FA_UseStateTransferAdd-1:");
                //新增后，修改卡片详情信息
                DateTime dt = request.DataDate;
                var DataDate = new DateTime(dt.Year, dt.Month, 1).ToString();
                var handleList = new List<FA_UseStateTransferHandle>();
                request.Lines.ForEach(a =>
                {
                    FA_UseStateTransferHandle obj = new FA_UseStateTransferHandle();
                    obj.DataDate = DataDate;
                    obj.CardID = a.CardID;
                    obj.UseStateID = a.AfterUseStateID;
                    handleList.Add(obj);
                });
                //_logger.LogInformation("FA_UseStateTransferAdd-2:"+handleList.Count);
                if (handleList.Count > 0)
                {
                    FA_AssetsCardInfos cardinfo = new FA_AssetsCardInfos();
                    cardinfo.lstCardDetailInfo = handleList;
                    cardinfo.OperateType = 33;
                    var url = $"{_hostCongfiguration.QlwServiceHost}/api/DBNFA_AssetsCard/SaveDataInfo";
                    var resultModel =await _httpClientUtil1.PostJsonAsync<ResultModel<object>>(url, cardinfo);
                    //_logger.LogInformation("FA_UseStateTransferAdd-3:"+JsonConvert.SerializeObject(resultModel)+"\n"+JsonConvert.SerializeObject(cardinfo)+"\n"+url);
                    //if (!resultModel.ResultState)
                    //{
                    //    result.code = ErrorCode.Create.GetIntValue();
                    //    result.msg = string.IsNullOrEmpty(resultModel.Msg)?"修改卡片详情信息失败":resultModel.Msg;
                    //}
                }
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Create.GetIntValue();
                result.msg = "保存异常";
            }
            //_logger.LogInformation("FA_UseStateTransferAdd-4:");
            return result;
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class FA_UseStateTransferDeleteHandler : IRequestHandler<FA_UseStateTransferDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFA_UseStateTransferRepository _repository;
        IFA_UseStateTransferDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration ;
        private readonly AssetsCommonUtil _assetsCommonUtil;
        private readonly ILogger<Domain.FA_UseStateTransfer> _logger;
        public FA_UseStateTransferDeleteHandler(IIdentityService identityService, IFA_UseStateTransferRepository repository, IFA_UseStateTransferDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository,  HttpClientUtil httpClientUtil1, HostConfiguration hostCongfiguration, AssetsCommonUtil assetsCommonUtil, ILogger<Domain.FA_UseStateTransfer> logger)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _httpClientUtil1 = httpClientUtil1;
            _hostCongfiguration = hostCongfiguration;
            _assetsCommonUtil = assetsCommonUtil;
            _logger = logger;
        }

        public async Task<Result> Handle(FA_UseStateTransferDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList();
                }
                var strNum = "";
                var saveFlag = false;//是否需要保存SaveChangesAsync
                //var successStr = "";
                foreach (var num in list)
                {
                    //_logger.LogInformation("FA_UseStateTransferDelete-1:");
                    //先获取变动单信息，为删除后修改卡片详情信息
                    var data = await _repository.GetAsync(num);
                    if (data == null)
                    {
                        strNum = num;
                        break;
                    }
                    #region 是否可调整

                    var checkReq = new ValidateCheckOutInput()
                    {
                        AppID = _identityService.AppId,
                        DataDate = data.DataDate,
                        EnterpriseID = _identityService.EnterpriseId,
                        OwnerID = _identityService.UserId
                    };
                    result = await _assetsCommonUtil.GetIsAjust(checkReq, "D");
                    //_logger.LogInformation("FA_UseStateTransferDelete-2:");
                    if (!string.IsNullOrEmpty(result.msg)) { break; }
                    #endregion
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.EnterpriseId && o.NumericalOrder == num);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    //删除成功后，还原卡片详情信息
                    DateTime dt = data.DataDate;
                    var DataDate = new DateTime(dt.Year, dt.Month, 1).ToString();
                    var handleList = new List<FA_UseStateTransferHandle>();
                    data.Details.ForEach(a =>
                    {
                        FA_UseStateTransferHandle obj = new FA_UseStateTransferHandle();
                        obj.DataDate = DataDate;
                        obj.CardID = a.CardID;
                        obj.UseStateID =  string.IsNullOrEmpty(a.BeforeUseStateID) ? "0" : a.BeforeUseStateID ;
                        handleList.Add(obj);
                    });
                    //_logger.LogInformation("FA_UseStateTransferDelete-3:");
                    if (handleList.Count > 0)
                    {
                        FA_AssetsCardInfos cardinfo = new FA_AssetsCardInfos();
                        cardinfo.lstCardDetailInfo = handleList;
                        cardinfo.OperateType = 33;
                        var param = JsonConvert.SerializeObject(cardinfo);
                        var url = $"{_hostCongfiguration.QlwServiceHost}/api/DBNFA_AssetsCard/SaveDataInfo";
                        var resultModel =await _httpClientUtil1.PostJsonAsync<ResultModel<object>>(url, cardinfo);
                        //_logger.LogInformation("FA_UseStateTransferDelete-4:"+JsonConvert.SerializeObject(result) + "\n" + JsonConvert.SerializeObject(cardinfo) + "\n" + url);
                        if (!resultModel.ResultState)
                        {
                            result.code = ErrorCode.Delete.GetIntValue();
                            result.msg = string.IsNullOrEmpty(resultModel.Msg) ? "还原卡片详情信息失败" : resultModel.Msg;
                            break;
                        }
                        else
                        {
                            saveFlag = true;
                        }
                    }                   
                    //successStr += num + ",";
                }
                if (saveFlag)
                {
                    await _repository.UnitOfWork.SaveChangesAsync();
                }
                if (!string.IsNullOrEmpty(strNum)) 
                {
                    result.msg = "未查询到" + strNum + "调整单信息";
                    result.code = ErrorCode.Delete.GetIntValue();
                }
                else if (!string.IsNullOrEmpty(result.msg))
                {
                    return result;
                }
                else
                {
                    //await _repository.UnitOfWork.SaveChangesAsync();
                    result.data = new { NumericalOrder = request.NumericalOrder };
                    result.code = ErrorCode.Success.GetIntValue();
                }    
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
            }

            return result;
        }


    }
    
    public class FA_UseStateTransferModifyHandler : IRequestHandler<FA_UseStateTransferModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFA_UseStateTransferRepository _repository;
        IFA_UseStateTransferDetailRepository _detailRepository;
        private readonly AssetsCommonUtil _assetsCommonUtil;
        public FA_UseStateTransferModifyHandler(IIdentityService identityService, IFA_UseStateTransferRepository repository, IFA_UseStateTransferDetailRepository detailRepository, AssetsCommonUtil assetsCommonUtil)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _assetsCommonUtil = assetsCommonUtil;
        }

        public async Task<Result> Handle(FA_UseStateTransferModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain?.Update(request.DataDate, request.Remarks, request.EnterpriseID);
                #region 是否可调整

                var checkReq = new ValidateCheckOutInput()
                {
                    AppID = _identityService.AppId,
                    DataDate = request.DataDate,
                    EnterpriseID = _identityService.EnterpriseId,
                    OwnerID = _identityService.UserId
                };
                result = await _assetsCommonUtil.GetIsAjust(checkReq, "M");
                if (!string.IsNullOrEmpty(result.msg)) { return result; }
                #endregion
                foreach (var item in request.Lines)
                {
                    if (item.RowStatus == "A" || item.IsCreate)
                    {
                        domain?.AddDetail(new FA_UseStateTransferDetail()
                        {
                            NumericalOrder = request.NumericalOrder,
                            CardID = item.CardID,
                            BeforeUseStateID =string.IsNullOrEmpty( item.BeforeUseStateID)?"0": item.BeforeUseStateID,
                            AfterUseStateID=item.AfterUseStateID,
                            Remarks=item.Remarks,
                            ModifiedDate = DateTime.Now
                        });
                    }
                    else if (item.RowStatus == "M" || item.IsUpdate)
                    {
                        item.BeforeUseStateID = string.IsNullOrEmpty(item.BeforeUseStateID) ? "0" : item.BeforeUseStateID;
                        var obj = domain?.Details?.Find(o => o.RecordID == item.RecordID);
                        if (obj == null) continue;
                        obj.Update(item.CardID, item.BeforeUseStateID,item.AfterUseStateID,item.Remarks);
                    }
                    else if (item.RowStatus == "D" || item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }


                }
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }
}
