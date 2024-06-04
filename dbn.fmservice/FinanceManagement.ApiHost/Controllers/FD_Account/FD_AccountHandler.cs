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
using FinanceManagement.Common;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
namespace FinanceManagement.ApiHost.Controllers.FD_Account
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_AccountAddHandler : IRequestHandler<FD_AccountAddCommand, Result>
    {
        IBiz_Related _biz_RelatedRepository;
        IIdentityService _identityService;
        IFD_AccountRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        HostConfiguration _hostCongfiguration;

        public FD_AccountAddHandler(IIdentityService identityService, IFD_AccountRepository repository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,IBiz_Related biz_RelatedRepository, HostConfiguration hostCongfiguration)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_RelatedRepository = biz_RelatedRepository;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<Result> Handle(FD_AccountAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                //加密
                EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                if (!string.IsNullOrEmpty(request.AccountNumber))
                {
                    var beforeAccountNumber = request.AccountNumber;
                    var resultRece = encryptAccount.AccountNumberEncrypt(request.AccountNumber);
                    request.AccountNumber = resultRece?.Item1 == true ? resultRece.Item2 : request.AccountNumber;
                    if (request.AccountNumber.Length < 30)
                    {
                        Serilog.Log.Information("FD_AccountAdd:beforeAccountNumber=" + beforeAccountNumber + ";AccountNumber=" + request.AccountNumber + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request) + ";resultRece=" + Newtonsoft.Json.JsonConvert.SerializeObject(resultRece));
                    }
                }
                
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    request.EnterpriseID = _identityService.EnterpriseId;
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                var domain = new Domain.FD_Account()
                {
                    AccountID = numericalOrder,
                    Guid = Guid.NewGuid(),
                    AccountName = request.AccountName,
                    AccountNumber =string.IsNullOrEmpty( request.AccountNumber)?"":request.AccountNumber,
                    AccountType = string.IsNullOrEmpty(request.AccountType)?"0": request.AccountType,
                    AccountFullName =string.IsNullOrEmpty( request.AccountFullName)?"":request.AccountFullName,
                    BankID = request.BankID,
                    BankAreaID = request.BankAreaID,
                    DepositBank = request.DepositBank,
                    Address = request.Address,
                    AccoSubjectID = request.AccoSubjectID,
                    ExpenseAccoSubjectID = request.ExpenseAccoSubjectID,
                    AccountUseType = request.AccountUseType,
                    IsUse = request.IsUse,
                    MarketID = request.MarketID,
                    BankNumber = request.BankNumber,
                    Remarks = request.Remarks,
                    OpenBankEnterConnect = request.OpenBankEnterConnect,
                    TubeAccountNumber = request.TubeAccountNumber,
                    CreatedDate =DateTime.Parse( request.CreatedDate),
                    ModifiedDate = DateTime.Now,                    
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID,
                    ResponsiblePerson=request.ResponsiblePerson
                };
                //结算方式 { RelatedType = 201610210104402102, ParentType = 201610140104402001, ChildType = 1611031642370000101, ParentValue = paymentTypeIDStr, ChildValue = account.ToString(), Remarks = "资金账户设置结算方式引用字典表" };
                relateds.Add(new BIZ_Related()
                {
                    RelatedType = "201610210104402102",
                    ParentType = "201610140104402001",                   
                    ChildType = "1611031642370000101",
                    ParentValue = request.PaymentTypeID,
                    ChildValue = numericalOrder,
                    Remarks= "资金账户设置结算方式引用字典表"
                });
                //账户用途 西煤支付 { RelatedType = 201610210104402102, ParentType = 1812271025360000100, ChildType = 1611031642370000101, ParentValue = i, ChildValue = account.ToString(), Remarks = "资金账户设置引用字典表" };
                request.PurposeList?.ForEach(p =>
                {
                    relateds.Add(new BIZ_Related()
                    {
                        RelatedType = "201610210104402102",
                        ParentType = "1812271025360000100",
                        ChildType = "1611031642370000101",
                        ParentValue = p,
                        ChildValue = numericalOrder,
                        Remarks = "资金账户设置引用字典表"
                    });
                });
                
                await _biz_RelatedRepository.AddRangeAsync(relateds);
                await _repository.AddAsync(domain);
                await _repository.UnitOfWork.SaveChangesAsync();

                result.data = new { AccountID = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
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

            return result;
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class FD_AccountDeleteHandler : IRequestHandler<FD_AccountDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_AccountRepository _repository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        public FD_AccountDeleteHandler(IIdentityService identityService, IFD_AccountRepository repository, IBiz_ReviewRepository biz_ReviewRepository, IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(FD_AccountDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList();
                }

                foreach (var num in list)
                {
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.EnterpriseId && o.AccountID == num);
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "201610140104402001" && o.ChildType == "1611031642370000101" && o.ChildValue == num);
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "1812271025360000100" && o.ChildType == "1611031642370000101" && o.ChildValue == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = new { AccountID = request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
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

    public class FD_AccountModifyHandler : IRequestHandler<FD_AccountModifyCommand, Result>
    {
        IBiz_Related _biz_RelatedRepository;
        Applications.Queries.BIZ_RelatedODataProvider _bizRelatedProvider;
        IIdentityService _identityService;
        IFD_AccountRepository _repository;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<FD_AccountModifyHandler> _logger;
        public FD_AccountModifyHandler(IIdentityService identityService, IFD_AccountRepository repository, IBiz_Related biz_RelatedRepository, Applications.Queries.BIZ_RelatedODataProvider bizRelatedProvider, HostConfiguration hostCongfiguration, ILogger<FD_AccountModifyHandler> logger)
        {
            _identityService = identityService;
            _repository = repository;
            _hostCongfiguration = hostCongfiguration;
            _biz_RelatedRepository = biz_RelatedRepository;
            _bizRelatedProvider = bizRelatedProvider;
            _logger = logger;
        }

        public async Task<Result> Handle(FD_AccountModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {                
                var domain = await _repository.GetAsync(request.AccountID);               
                if (domain == null) { result.code = ErrorCode.Update.GetIntValue(); result.msg = "AccountID查询空"; return result; }
                //加密
                EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                if (!string.IsNullOrEmpty(request.AccountNumber))
                {
                    var beforeAccountNumber = request.AccountNumber;
                    var resultRece = encryptAccount.AccountNumberEncrypt(request.AccountNumber);
                    request.AccountNumber = resultRece?.Item1 == true ? resultRece.Item2 : request.AccountNumber;
                    if (request.AccountNumber.Length < 30)
                    {
                        Serilog.Log.Information("FD_AccountModify:beforeAccountNumber=" + beforeAccountNumber + ";AccountNumber=" + request.AccountNumber+ ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request) + ";resultRece="+Newtonsoft.Json.JsonConvert.SerializeObject(resultRece));
                    }
                }
                domain?.Update(request.AccountName, request.AccountType, request.AccountFullName, request.BankID, request.BankAreaID, request.DepositBank, request.Address, request.AccoSubjectID, request.ExpenseAccoSubjectID, request.AccountUseType, request.ResponsiblePerson, 
                    request.IsUse, request.MarketID, request.BankNumber, request.Remarks, request.OpenBankEnterConnect,request.TubeAccountNumber,request.AccountNumber,DateTime.Parse( request.CreatedDate));
                //var sw = new Stopwatch();
                //sw.Start();
                List<BIZ_Related> addrelateds = new List<BIZ_Related>();
                
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "201610140104402001" && o.ChildType == "1611031642370000101" && o.ChildValue == request.AccountID);
                if (!string.IsNullOrEmpty(request.PaymentTypeID) && request.PaymentTypeID != "0")
                {
                    //结算方式 { RelatedType = 201610210104402102, ParentType = 201610140104402001, ChildType = 1611031642370000101, ParentValue = paymentTypeIDStr, ChildValue = account.ToString(), Remarks = "资金账户设置结算方式引用字典表" };
                    addrelateds.Add(new BIZ_Related()
                    {
                        RelatedType = "201610210104402102",
                        ParentType = "201610140104402001",
                        ChildType = "1611031642370000101",
                        ParentValue = request.PaymentTypeID,
                        ChildValue = request.AccountID,
                        Remarks = "资金账户设置结算方式引用字典表"
                    });
                }
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "1812271025360000100" && o.ChildType == "1611031642370000101" && o.ChildValue == request.AccountID);
                request.PurposeList?.ForEach(item => {
                    //账户用途 西煤支付 { RelatedType = 201610210104402102, ParentType = 1812271025360000100, ChildType = 1611031642370000101, ParentValue = i, ChildValue = account.ToString(), Remarks = "资金账户设置引用字典表" };
                    addrelateds.Add(new BIZ_Related()
                    {
                        RelatedType = "201610210104402102",
                        ParentType = "1812271025360000100",
                        ChildType = "1611031642370000101",
                        ParentValue = item,
                        ChildValue = request.AccountID,
                        Remarks = "资金账户设置引用字典表"
                    });
                });
                await _biz_RelatedRepository.AddRangeAsync(addrelateds);
                await _repository.UnitOfWork.SaveChangesAsync();
              
                result.data = new { AccountID = request.AccountID };
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
                Serilog.Log.Information("FD_AccountModify:errorCodeEx="+ errorCodeEx.ToString()+ ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存异常";
                Serilog.Log.Information("FD_AccountModify:ex=" + ex.ToString() + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }
            return result;
        }
    }
}
