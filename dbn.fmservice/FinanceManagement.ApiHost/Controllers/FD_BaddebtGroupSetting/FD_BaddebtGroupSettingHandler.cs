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

namespace FinanceManagement.ApiHost.Controllers.FD_BaddebtGroupSetting
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_BaddebtGroupSettingAddHandler : IRequestHandler<FD_BaddebtGroupSettingAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtGroupSettingRepository _repository;
        IFD_BaddebtGroupSettingDetailRepository _detailRepository;
        IFD_BaddebtGroupSettingExtendRepository _extendRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IFD_IdentificationTypeRepository _typeRepository;
        IFD_IdentificationTypeSubjectRepository _typeSubjectRepository;

        public FD_BaddebtGroupSettingAddHandler(IIdentityService identityService, IFD_BaddebtGroupSettingRepository repository, IFD_BaddebtGroupSettingDetailRepository detailRepository, IFD_BaddebtGroupSettingExtendRepository extendRepository, IBiz_ReviewRepository biz_ReviewRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator
            , IFD_IdentificationTypeRepository typeRepository, IFD_IdentificationTypeSubjectRepository typeSubjectRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extendRepository = extendRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _typeRepository = typeRepository;
            _typeSubjectRepository = typeSubjectRepository;
        }

        public async Task<Result> Handle(FD_BaddebtGroupSettingAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //if (string.IsNullOrEmpty(request.EnterpriseID))
                //{
                //    request.EnterpriseID = _identityService.GroupId;
                //}
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_BaddebtGroupSetting()
                {
                    DataDate=request.DataDate,
                    //Number = number.ToString(),
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Remarks=request.Remarks,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = request.OwnerID,
                    EnterpriseID = _identityService.GroupId
                };
                request.Lines?.ForEach(o =>
                {
                    domain.AddDetail(new FD_BaddebtGroupSettingDetail()
                    {
                        NumericalOrder = numericalOrder,
                        BusType =o.BusType,
                        IntervalType=o.IntervalType,
                        Name=o.Name,
                        DayNum=o.DayNum,
                        Serial=o.Serial,
                        ProvisionRatio = o.ProvisionRatio,
                        ModifiedDate = DateTime.Now,
                        CreatedDate = DateTime.Now
                    });
                });
                var dbTypeList = new List<FD_IdentificationType>();
                var typeSubjectList = new List<FD_IdentificationTypeSubject>();
                if (request.TypeAndSubjects?.Count > 0)
                {
                    foreach (var item in request.TypeAndSubjects)
                    {
                        #region 计提类型
                        var typeID = _numericalOrderCreator.Create();
                        dbTypeList.Add(new FD_IdentificationType()
                        {
                            TypeID = typeID,
                            TypeName = item.TypeName,
                            NumericalOrder = domain.NumericalOrder,
                            EnterpriseID = domain.EnterpriseID,
                            BusiType = item.BusiType,
                            AccrualType = item.AccrualType,
                            OwnerID = domain.OwnerID,
                            ModifiedDate = DateTime.Now,
                            CreatedDate = DateTime.Now
                        });
                        #endregion

                        #region 计提类型科目
                        typeSubjectList.Add(new FD_IdentificationTypeSubject()
                        {
                            TypeID= typeID,
                            AccoSubjectID=item.AccoSubjectID,
                            NumericalOrder=domain.NumericalOrder,
                            EnterpriseID=domain.EnterpriseID,
                            IsUse=true,
                            DataSourceType=item.DataSourceType,
                            OwnerID=domain.OwnerID,
                            ModifiedDate = DateTime.Now,
                            CreatedDate = DateTime.Now
                        });

                        #endregion
                    }
                }
                
                request.Extends?.ForEach(o =>
                {
                    domain.AddExtend(new FD_BaddebtGroupSettingExtend()
                    {
                        NumericalOrder = numericalOrder,
                        EnterpriseID = o.EnterpriseID,
                        ShowID=o.ShowID,
                        ModifiedDate = DateTime.Now
                    });
                });
                await _repository.AddAsync(domain);
                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, request.OwnerID).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _typeRepository.AddRangeAsync(dbTypeList);
                await _typeSubjectRepository.AddRangeAsync(typeSubjectList);
                await _typeRepository.UnitOfWork.SaveChangesAsync();
                await _typeSubjectRepository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extendRepository.UnitOfWork.SaveChangesAsync();
              
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
    public class FD_BaddebtGroupSettingDeleteHandler : IRequestHandler<FD_BaddebtGroupSettingDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtGroupSettingRepository _repository;
        IFD_BaddebtGroupSettingDetailRepository _detailRepository;
        IFD_BaddebtGroupSettingExtendRepository _extendRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IFD_IdentificationTypeRepository _typeRepository;
        IFD_IdentificationTypeSubjectRepository _typeSubjectRepository;
        public FD_BaddebtGroupSettingDeleteHandler(IIdentityService identityService, IFD_BaddebtGroupSettingRepository repository, IFD_BaddebtGroupSettingDetailRepository detailRepository, IFD_BaddebtGroupSettingExtendRepository extendRepository, IBiz_ReviewRepository biz_ReviewRepository
             , IFD_IdentificationTypeRepository typeRepository, IFD_IdentificationTypeSubjectRepository typeSubjectRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extendRepository = extendRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _typeRepository = typeRepository;
            _typeSubjectRepository = typeSubjectRepository;
        }

        public async Task<Result> Handle(FD_BaddebtGroupSettingDeleteCommand request, CancellationToken cancellationToken)
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
                    await _typeRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _typeSubjectRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder ==num);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _extendRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.GroupId && o.NumericalOrder == num);
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
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

    public class FD_BaddebtGroupSettingModifyHandler : IRequestHandler<FD_BaddebtGroupSettingModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtGroupSettingRepository _repository;
        IFD_BaddebtGroupSettingDetailRepository _detailRepository;
        IFD_BaddebtGroupSettingExtendRepository _extendRepository;
        IFD_IdentificationTypeRepository _typeRepository;
        IFD_IdentificationTypeSubjectRepository _typeSubjectRepository;
        NumericalOrderCreator _numericalOrderCreator;
        public FD_BaddebtGroupSettingModifyHandler(IIdentityService identityService, IFD_BaddebtGroupSettingRepository repository, IFD_BaddebtGroupSettingDetailRepository detailRepository, IFD_BaddebtGroupSettingExtendRepository extendRepository
             , IFD_IdentificationTypeRepository typeRepository, IFD_IdentificationTypeSubjectRepository typeSubjectRepository, NumericalOrderCreator numericalOrderCreator)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extendRepository = extendRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _typeRepository = typeRepository;
            _typeSubjectRepository = typeSubjectRepository;
        }

        public async Task<Result> Handle(FD_BaddebtGroupSettingModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                if (domain == null) { result.code = ErrorCode.Update.GetIntValue(); result.msg = "NumericalOrder查询空"; return result; }
                domain.Update(request.DataDate, request.StartDate, request.EndDate, request.Remarks);
               
                foreach (var item in request.Lines)
                {
                    if (item.RowStatus=="A"||item.IsCreate)
                    {
                        domain.AddDetail(new FD_BaddebtGroupSettingDetail()
                        {
                            NumericalOrder = request.NumericalOrder,
                            BusType = item.BusType,
                            ProvisionRatio = item.ProvisionRatio,
                            ModifiedDate = DateTime.Now,
                            IntervalType = item.IntervalType,
                            Name=item.Name,
                            DayNum=item.DayNum,
                            Serial=item.Serial
                        });
                    }
                    else if (item.RowStatus == "M"||item.IsUpdate)
                    {
                        var obj = domain.Details.Find(o => o.RecordID == item.RecordID);
                        obj?.Update(item.BusType, item.IntervalType,item.Name,item.DayNum,item.Serial, item.ProvisionRatio);
                    }
                    else if (item.RowStatus == "D" || item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }


                }
                //var opreateID = string.IsNullOrEmpty(_identityService.UserId) ? "0" : _identityService.UserId;
                var dbTypeList = new List<FD_IdentificationType>();
                var typeSubjectList = new List<FD_IdentificationTypeSubject>();
                if (request.TypeAndSubjects?.Count > 0)
                {
                    //查询计提类型和类型科目
                    var typelist = _typeRepository.GetTypeByID(request.NumericalOrder);
                    var typesubjelist = _typeSubjectRepository.GetTypeSubjectByID(request.NumericalOrder);
                    foreach (var item in request.TypeAndSubjects)
                    {
                        if (item.RowStatus == "A" || item.IsCreate)
                        {
                            #region 计提类型
                            var typeID = _numericalOrderCreator.Create();
                            dbTypeList.Add(new FD_IdentificationType()
                            {
                                TypeID = typeID,
                                TypeName = item.TypeName,
                                NumericalOrder = domain.NumericalOrder,
                                EnterpriseID = domain.EnterpriseID,
                                BusiType = item.BusiType,
                                AccrualType = item.AccrualType,
                                OwnerID = domain.OwnerID,
                                ModifiedDate = DateTime.Now,
                                CreatedDate = DateTime.Now
                            });
                            #endregion

                            #region 计提类型科目
                            typeSubjectList.Add(new FD_IdentificationTypeSubject()
                            {
                                TypeID = typeID,
                                AccoSubjectID = item.AccoSubjectID,
                                NumericalOrder = domain.NumericalOrder,
                                EnterpriseID = domain.EnterpriseID,
                                IsUse = true,
                                DataSourceType = item.DataSourceType,
                                OwnerID = domain.OwnerID,
                                ModifiedDate = DateTime.Now,
                                CreatedDate = DateTime.Now
                            });

                            #endregion
                        }
                        else if (item.RowStatus == "M" || item.IsUpdate)
                        {
                            #region 计提类型
                            var typeobj = typelist.Find(o => o.TypeID == item.OldTypeID);
                            typeobj?.Update(item.TypeName, item.BusiType, item.AccrualType);
                            #endregion

                            #region 计提类型科目
                            var subjectobj = typesubjelist.Find(o => o.RecordID == item.RecordID);
                            subjectobj?.Update(item.OldTypeID, item.AccoSubjectID,  item.DataSourceType);
                            #endregion
                        }
                        else if (item.RowStatus == "D" || item.IsDelete)
                        {
                            await _typeSubjectRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                            await _typeRepository.RemoveRangeAsync(o => o.TypeID == item.OldTypeID);
                            continue;
                        }


                    }
                }
               
                foreach (var item in request.Extends)
                {
                    if (item.RowStatus == "A" || item.IsCreate)
                    {
                        domain.AddExtend(new FD_BaddebtGroupSettingExtend()
                        {
                            NumericalOrder = request.NumericalOrder,
                            EnterpriseID = item.EnterpriseID,
                            ShowID=item.ShowID,
                            ModifiedDate=DateTime.Now
                        });
                    }
                    else if (item.RowStatus == "M" || item.IsUpdate)
                    {
                        var obj = domain.Extends.Find(o => o.EnterpriseID == item.EnterpriseID);
                        obj?.Update(item.EnterpriseID,item.ShowID);
                    }
                    else if (item.RowStatus == "D" || item.IsDelete)
                    {
                        await _extendRepository.RemoveRangeAsync(o => o.EnterpriseID == item.EnterpriseID);
                        continue;
                    }


                }
                await _typeRepository.AddRangeAsync(dbTypeList);
                await _typeSubjectRepository.AddRangeAsync(typeSubjectList);
                await _typeRepository.UnitOfWork.SaveChangesAsync();
                await _typeSubjectRepository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extendRepository.UnitOfWork.SaveChangesAsync();
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
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }
}
