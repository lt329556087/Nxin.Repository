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
using Architecture.Common.Application.Commands;
using Newtonsoft.Json;
using FinanceManagement.Common;
namespace FinanceManagement.ApiHost.Controllers.FM_CashSweep
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FM_CashSweepAddHandler : IRequestHandler<FM_CashSweepAddCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CashSweepRepository _repository;
        IFM_CashSweepDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IbsfileRepository _ibsfileRepository;
        ApplySubjectUtil _applySubjectUtil;
        private string CashSweepAppID = "2210111522080000109";//资金归集菜单
        public FM_CashSweepAddHandler(IIdentityService identityService, IFM_CashSweepRepository repository, IFM_CashSweepDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository
            ,IbsfileRepository ibsfileRepository
            , ApplySubjectUtil applySubjectUtil)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _ibsfileRepository = ibsfileRepository;
            _applySubjectUtil = applySubjectUtil;
        }

        public async Task<Result> Handle(FM_CashSweepAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    result.msg = "EnterpriseID不能为空";
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    return result;
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                //long number = _numberCreator.Create<Domain.FM_CashSweep>(o => o.DataDate, o => o.Number, Convert.ToDateTime(Convert.ToDateTime(request.DataDate).Format("yyyy-MM-dd")), o => o.EnterpriseID == request.EnterpriseID);
                var number = _numberCreator.Create<Domain.FM_CashSweep>(request.DataDate, o => o.Number, o => o.Number.StartsWith(request.DataDate.ToString("yyyyMMdd")), o => o.EnterpriseID == request.EnterpriseID); 
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                
                if (request.Lines?.Count > 0)
                {
                    //自动归集 归集方案为自定义 归集公式=归集金额；归集金额=0；
                    if (request.SweepType == "1811191754180000202")
                    {
                        var firstLine = request.Lines[0];
                        request.SchemeFormula = firstLine.AutoSweepBalance_Show;
                        request.Lines.ForEach(p => p.AutoSweepBalance = 0);
                    }
                    else
                    {
                        try
                        {
                            request.Lines.ForEach(p => p.AutoSweepBalance = string.IsNullOrEmpty(p.AutoSweepBalance_Show) ? 0 : decimal.Parse(p.AutoSweepBalance_Show));
                        }
                        catch(Exception ex)
                        {
                            result.msg = "AutoSweepBalance_Show归集金额有误";
                            result.code = ErrorCode.RequestArgumentError.GetIntValue();
                            return result;
                        }                          
                    }                    
                }

                var domain = new Domain.FM_CashSweep()
                {
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID,
                    AccountID = request.AccountID,
                    SweepDirectionID = request.SweepDirectionID,
                    SweepType = request.SweepType,
                    ExcuteDate = request.ExcuteDate,
                    ExcuterID = request.ExcuterID,
                    Remarks = request.Remarks,
                    Number = number.ToString(),
                    CollectionScheme = request.CollectionScheme,
                    SchemeType = request.SchemeType,
                    AutoTime = request.AutoTime,
                    SchemeAmount = request.SchemeAmount,
                    Rate = request.Rate,
                    PlanType = request.PlanType,
                    SchemeFormula = request.SchemeFormula,
                    SchemeTypeName = request.SchemeTypeName,
                    IsUse = false,
                    IsNew=true
                };
                if (request.Lines?.Count > 0)
                {
                    foreach (var item in request.Lines)
                    {
                        var detailNumericalOrder = await _numericalOrderCreator.CreateAsync();
                        var detial = new FM_CashSweepDetail()
                        {
                            NumericalOrder = numericalOrder,
                            NumericalOrderDetail = detailNumericalOrder,
                            EnterpriseID = item.EnterpriseID,
                            AccountID = item.AccountID,
                            AccountBalance = item.AccountBalance,
                            OtherAccountBalance = item.OtherAccountBalance,
                            TheoryBalance = item.TheoryBalance,
                            TransformBalance = item.TransformBalance,
                            AutoSweepBalance = item.AutoSweepBalance,
                            ManualSweepBalance = item.ManualSweepBalance,
                            Remark = item.Remark,
                            Status = item.Status,
                            ExcuteMsg = item.ExcuteMsg,
                            ModifiedDate = DateTime.Now
                        };
                        domain.AddDetail(detial);
                    }
                }
                Biz_Review review = new Biz_Review(numericalOrder, CashSweepAppID, request.OwnerID).SetMaking();
                List<FD_PaymentReceivables.UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<FD_PaymentReceivables.UploadInfo>>(request.UploadInfo);
                }
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = request.EnterpriseID,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = request.OwnerID,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }                
                //添加申请主题 
                var sweepTypeName = (request.SweepType == "1811191754180000202" ? "自动归集" : "手动归集");
                var sweepDirectionName = (request.SweepDirectionID == "1811191754180000101" ? "向上归集" : "向下归集");
                
                var amountStr = "";
                if (request.SweepType == "1811191754180000201")//手动归集
                {
                    var sumamount = request.Lines?.Sum(p => p.AutoSweepBalance);
                    amountStr = "，归集金额 : " + sumamount;
                }
                else
                {
                    amountStr = "，归集金额公式:" + request.SchemeFormula;
                }
                var subject = new SubjectInfo()
                {
                    NumericalOrder = numericalOrder,
                    Subject = "资金集中归集：归集类型：" + sweepTypeName+ "，归集方向："+ sweepDirectionName+amountStr
                };
                _applySubjectUtil.AddSubject(subject);
                await _repository.AddAsync(domain);
                await _biz_ReviewRepository.AddAsync(review);

                await _repository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
               
                result.data = new { NumericalOrder = numericalOrder };
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
    public class FM_CashSweepDeleteHandler : IRequestHandler<FM_CashSweepDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CashSweepRepository _repository;
        IFM_CashSweepDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IbsfileRepository _ibsfileRepository;
        public FM_CashSweepDeleteHandler(IIdentityService identityService, IFM_CashSweepRepository repository, IFM_CashSweepDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository, IbsfileRepository ibsfileRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _ibsfileRepository = ibsfileRepository;
        }

        public async Task<Result> Handle(FM_CashSweepDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList<string>();
                }

                foreach (var num in list)
                {                    
                    await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
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

    public class FM_CashSweepModifyHandler : IRequestHandler<FM_CashSweepModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CashSweepRepository _repository;
        IFM_CashSweepDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        IbsfileRepository _ibsfileRepository;
        public FM_CashSweepModifyHandler(IIdentityService identityService, IFM_CashSweepRepository repository, IFM_CashSweepDetailRepository detailRepository,
            NumericalOrderCreator numericalOrderCreator,
             IbsfileRepository ibsfileRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _ibsfileRepository = ibsfileRepository;
        }

        public async Task<Result> Handle(FM_CashSweepModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                if (string.IsNullOrEmpty(_identityService.EnterpriseId) || _identityService.EnterpriseId == "0")
                {
                    result.msg = "登录单位空";
                    result.code = ErrorCode.Update.GetIntValue();
                    return result;
                }

                if (domain == null)
                {
                    result.msg = "未查询到数据";
                    result.code = ErrorCode.NoContent.GetIntValue();
                    return result;
                }
                if (request.Lines?.Count > 0)
                {
                    //自动归集 归集方案类型为自定义 归集公式=归集金额；归集金额=0；
                    if (request.SweepType == "1811191754180000202")
                    {
                        var firstLine = request.Lines[0];
                        request.SchemeFormula = firstLine.AutoSweepBalance_Show;
                        request.Lines.ForEach(p => p.AutoSweepBalance = 0);
                    }
                    else
                    {
                        try
                        {
                            request.Lines.ForEach(p => p.AutoSweepBalance = string.IsNullOrEmpty(p.AutoSweepBalance_Show) ? 0 : decimal.Parse(p.AutoSweepBalance_Show));
                        }
                        catch(Exception ex)
                        {
                            result.msg = "AutoSweepBalance_Show归集金额有误";
                            result.code = ErrorCode.RequestArgumentError.GetIntValue();
                            return result;
                        }                       
                    }
                }
                if (!string.IsNullOrEmpty(request.SweepDirectionID))
                {
                    domain?.Update(request.DataDate, request.AccountID, request.SweepDirectionID, request.SweepType, request.Remarks, request.ExcuteDate, request.ExcuterID
                    , request.CollectionScheme, request.SchemeType, request.AutoTime, request.SchemeAmount, request.Rate, request.PlanType, request.SchemeFormula, request.SchemeTypeName);
                }

                if (request.Lines?.Count > 0)
                {
                    foreach (var item in request.Lines)
                    {
                        if (item.RowStatus == "A" || item.IsCreate || item.Target == TargetType.Create)
                        {
                            var detailNumericalOrder = await _numericalOrderCreator.CreateAsync();
                            var detial = new FM_CashSweepDetail()
                            {
                                NumericalOrder = request.NumericalOrder,
                                NumericalOrderDetail = detailNumericalOrder,
                                EnterpriseID = item.EnterpriseID,
                                AccountID = item.AccountID,
                                AccountBalance = item.AccountBalance,
                                OtherAccountBalance = item.OtherAccountBalance,
                                TheoryBalance = item.TheoryBalance,
                                TransformBalance = item.TransformBalance,
                                AutoSweepBalance = item.AutoSweepBalance,
                                ManualSweepBalance = item.ManualSweepBalance,
                                Remark = item.Remark,
                                Status = item.Status,
                                ExcuteMsg = item.ExcuteMsg,
                                ModifiedDate = DateTime.Now
                            };
                            _detailRepository.Add(detial);
                            domain.AddDetail(detial);
                            continue;
                        }
                        if (item.RowStatus == "D" || item.IsDelete || item.Target == TargetType.Delete)
                        {
                            await _detailRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                            continue;
                        }
                        if (item.RowStatus == "M" || item.IsUpdate || item.Target == TargetType.Update)
                        {
                            var obj = domain.Lines.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                            obj?.Update(item.EnterpriseID, item.AccountID, item.AccountBalance, item.OtherAccountBalance, item.TheoryBalance, item.TransformBalance, item.AutoSweepBalance, item.ManualSweepBalance, item.Remark, item.Status, item.ExcuteMsg);

                            continue;
                        }
                    }
                }
               
                await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                List<FD_PaymentReceivables.UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<FD_PaymentReceivables.UploadInfo>>(request.UploadInfo);
                }
                if (up != null)
                {
                    var EnterpriseID = request.EnterpriseID;
                    if (string.IsNullOrEmpty(request.EnterpriseID))
                    {
                        EnterpriseID = _identityService.EnterpriseId;
                    }
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId =  EnterpriseID,
                            NumericalOrder = request.NumericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID =string.IsNullOrEmpty( _identityService.UserId)?"0" : _identityService.UserId,
                            Remarks = request.UploadInfo
                        });
                    }                    
                }
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = domain.NumericalOrder;
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }

    
}
