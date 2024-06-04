using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtExecution;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtProvision;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtProvision
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BadDebtProvisionController : ControllerBase
    {
        IMediator _mediator;
        FD_BadDebtProvisionODataProvider _provider;
        AgingDataUtil _agingDataUtil;
        SettleReceiptUtil _settleReceiptUtil;
        FD_BaddebtSettingODataProvider _settingProvider;
        IIdentityService _identityService;
        FD_SpecificIdentificationODataProvider _specificProvider;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        IFD_BadDebtProvisionDetailRepository _iProvisionDetailRepository;
        FMBaseCommon _baseUtil;
        IFD_BadDebtExecutionRepository _exeRepository;
        IFD_SpecificIdentificationRepository _specificIdentificationRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        SubjectBalanceUtil _subjectBalanceUtil;
        ReceiptExecutionUtil _exeUtil;
        RptAgingReclassificationUtil _agingReclassificationUtil;
        EnterprisePeriodUtil _enterprisePeriodUtil;



        public FD_BadDebtProvisionController(IMediator mediator, FD_BadDebtProvisionODataProvider provider,
            FD_BaddebtSettingODataProvider settingProvider,
            AgingDataUtil agingDataUtil,
            SettleReceiptUtil settleReceiptUtil,
            IIdentityService identityService,
            FD_SpecificIdentificationODataProvider specificProvider,
            IFD_BadDebtProvisionExtRepository extRepository,
            FMBaseCommon baseUtil,
            SubjectBalanceUtil subjectBalanceUtil,
            IFD_BadDebtExecutionRepository exeRepository,
            IFD_BadDebtProvisionRepository iProvisionRepository,
            IFD_BadDebtProvisionDetailRepository iProvisionDetailRepository,
            IFD_SpecificIdentificationRepository specificIdentificationRepository,
            IBiz_ReviewRepository biz_ReviewRepository,
            ReceiptExecutionUtil exeUtil,
             RptAgingReclassificationUtil agingReclassificationUtil,
                   EnterprisePeriodUtil enterprisePeriodUtil
            )
        {
            _mediator = mediator;
            _provider = provider;
            _settingProvider = settingProvider;
            _agingDataUtil = agingDataUtil;
            _identityService = identityService;
            _specificProvider = specificProvider;
            _baseUtil = baseUtil;
            _exeRepository = exeRepository;
            _settleReceiptUtil = settleReceiptUtil;
            _iProvisionRepository = iProvisionRepository;
            _iProvisionDetailRepository = iProvisionDetailRepository;
            _specificIdentificationRepository = specificIdentificationRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _subjectBalanceUtil = subjectBalanceUtil;
            _exeUtil = exeUtil;
            _agingReclassificationUtil = agingReclassificationUtil;
            _enterprisePeriodUtil = enterprisePeriodUtil;
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
            var data = _provider.GetSingleDataAsync(key).Result;
            if (data != null)
            {
                data?.Lines.ForEach(o =>
                {
                    o.AgingList = _provider.GetExtDatasAsync(Convert.ToInt64(o.NumericalOrderDetail)).ToList();
                });

                data.Lines1 = data.Lines.Where(o => data.AccoSubjectID1 == o.AccoSubjectID).ToList();
                data.SumProvisionAmount1 = data.Lines1.Sum(o => o.ProvisionAmount);//本期末应计提总金额
                data.DiffAmount1 = data.SumProvisionAmount1 - data.HaveProvisionAmount1;//本期应计提差额

                data.Lines2 = data.Lines.Where(o => data.AccoSubjectID2 == o.AccoSubjectID).ToList();
                data.SumProvisionAmount2 = data.Lines2.Sum(o => o.ProvisionAmount);//本期末应计提总金额
                data.DiffAmount2 = data.SumProvisionAmount2 - data.HaveProvisionAmount2;//本期应计提差额
            }

            result.data = data;
            return result;
        }

        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost, Route("IsSettingDataExist")]
        public Result IsSettingDataExist([FromBody] FD_BadDebtProvisionAddCommand request)
        {
            var result = new Result();
            result.data = _iProvisionRepository.IsSettingDataExist(request.NumericalOrder, _identityService.EnterpriseId);
            return result;
        }

        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("IsSpecificDataExist")]
        public Result IsSpecificDataExist([FromBody] FD_BadDebtProvisionAddCommand request)
        {
            var result = new Result();
            var specific = _specificIdentificationRepository.GetAsync(request.NumericalOrder).Result;
            var list = new List<SpeicificResult>();
            specific?.Lines?.ForEach(o =>
            {
                var result = new SpeicificResult();
                result.RecordID = o.NumericalOrderDetail;
                result.State = _iProvisionDetailRepository.IsSpecificDataExist(o.NumericalOrderDetail.ToString(), _identityService.EnterpriseId);
                list.Add(result);
            });
            result.data = list;
            return result;
        }

        public class SpeicificResult
        {
            public string RecordID { get; set; }
            public bool State { get; set; }
        }

        //增加
        [HttpPost, Route("GetEmptyModel")]
        public Result GetEmptyModel([FromBody] FD_BadDebtProvisionAddCommand request)
        {
            var result = new Result();
            try
            {
                var provisionModel = new FD_BadDebtProvisionODataEntity();
                var enterpriseId = Convert.ToInt64(_identityService.EnterpriseId);
                var settingModel = _settingProvider.GetDataByEnterId(enterpriseId, request.DataDate);

                var subjectCodeList = _baseUtil.GetEnterSubjectList(0, enterpriseId, request.DataDate).Result;
                //应收账款
                provisionModel.AccoSubjectID1 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1122")?.AccoSubjectID;
                provisionModel.AccoSubjectName1 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1122")?.cAccoSubjectFullName;
                //其他应收款
                provisionModel.AccoSubjectID2 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1221")?.AccoSubjectID;
                provisionModel.AccoSubjectName2 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1221")?.cAccoSubjectFullName;

                var dataDate = request.DataDate;//单据日期


                var listReclass = PackageReclassification(request.DataDate);
                var list1 = listReclass.Where(o => o.AccoSubjectID == provisionModel.AccoSubjectID1)?.ToList();
                if (list1?.Count > 0)
                {
                    list1.ForEach(o => { o.AccoSubjectID = provisionModel.AccoSubjectID1; });
                    provisionModel.Lines1.AddRange(list1);
                }
                var list2 = listReclass.Where(o => o.AccoSubjectID == provisionModel.AccoSubjectID2)?.ToList();
                if (list2?.Count > 0)
                {
                    list2.ForEach(o => { o.AccoSubjectID = provisionModel.AccoSubjectID2; });
                    provisionModel.Lines2.AddRange(list2);
                }

                //拼装个别认定相关信息
                if (provisionModel.Lines1.Count > 0) provisionModel.Lines1 = PackageIdentity(provisionModel.Lines1, request.DataDate);
                if (provisionModel.Lines2.Count > 0) provisionModel.Lines2 = PackageIdentity(provisionModel.Lines2, request.DataDate);

                //拼装参数设置相关信息
                if (provisionModel.Lines1.Count > 0) provisionModel.Lines1 = PackageSetting(provisionModel.Lines1, settingModel);
                if (provisionModel.Lines2.Count > 0) provisionModel.Lines2 = PackageSetting(provisionModel.Lines2, settingModel);

                //var specificModel = _specificProvider.GetDataByEnterId(enterpriseId, request.DataDate, "");

                ////个别认定 移除全额不计提
                //specificModel.Lines.Where(o => o.ProvisionType == "2110251443470000109").ToList().ForEach(specific =>
                //{
                //    var p = provisionModel.Lines1.Find(o => specific.CustomerID == o.CustomerID);
                //    if (p != null)
                //    {
                //        provisionModel.Lines1.Remove(p);
                //    }
                //});

                ////个别认定 盖掉部分计提
                //provisionModel.Lines1.ForEach(line =>
                //{
                //    var specific = specificModel.Lines.Find(o => o.CustomerID == line.CustomerID);
                //    if (specific?.ProvisionType == "2110251444240000109" || specific?.ProvisionType == "2110251444240000109")//全额计提 or 部分计提
                //    {
                //        line.CurrentDebtPrepareAmount = specific.Amount;
                //        line.NumericalOrderSpecific = specific.NumericalOrderDetail.ToString();
                //    }
                //});

                //provisionModel.Lines1.ForEach(o => o.AccoSubjectID = provisionModel.AccoSubjectID1);

                //本次应计提金额处理
                provisionModel.HaveProvisionAmount1 = GetSubjectBalanceAmount(request.DataDate, settingModel?.BadAccoSubjectOne);//已计提总金额
                if (provisionModel?.Lines1.Count > 0)
                {
                    provisionModel.SumProvisionAmount1 = provisionModel.Lines1.Sum(o => o.ProvisionAmount);//本期末应计提总金额
                }
                provisionModel.DiffAmount1 = provisionModel.SumProvisionAmount1 - provisionModel.HaveProvisionAmount1;//本期应计提差额

                //var ageData2 = GetAgingData(request.DataDate, settingModel.BadAccoSubjectTwo).Where(o => o.Amount > 0).ToList();
                //provisionModel.Lines2 = BuildDetail(ageData2, provisionModel, settingModel);

                ////个别认定 移除全额不计提
                //specificModel.Lines.Where(o => o.ProvisionType == "2110251443470000109").ToList().ForEach(specific =>
                //{
                //    var p = provisionModel.Lines2.Find(o => specific.CustomerID == o.CustomerID);
                //    if (p != null)
                //    {
                //        provisionModel.Lines2.Remove(p);
                //    }
                //});

                ////个别认定 盖掉部分计提
                //provisionModel.Lines2.ForEach(line =>
                //{
                //    var specific = specificModel.Lines.Find(o => o.CustomerID == line.CustomerID);
                //    if (specific?.ProvisionType == "2110251444240000109" || specific?.ProvisionType == "2110251444240000109")//全额计提 or 部分计提
                //    {
                //        line.CurrentDebtPrepareAmount = specific.Amount;
                //        line.NumericalOrderSpecific = specific.NumericalOrderDetail.ToString();
                //    }
                //});
                //provisionModel.Lines2.ForEach(o => o.AccoSubjectID = provisionModel.AccoSubjectID2);

                //本次应计提金额处理
                provisionModel.HaveProvisionAmount2 = GetSubjectBalanceAmount(request.DataDate, settingModel?.BadAccoSubjectTwo);//已计提总金额
                if (provisionModel?.Lines2.Count > 0)
                {
                    provisionModel.SumProvisionAmount2 = provisionModel.Lines2.Sum(o => o.ProvisionAmount);//本期末应计提总金额
                }
                provisionModel.DiffAmount2 = provisionModel.SumProvisionAmount2 - provisionModel.HaveProvisionAmount2;//本期应计提差额
                provisionModel.DataDate = request.DataDate;
                provisionModel.NumericalOrderSetting = settingModel?.NumericalOrder;
                result.data = provisionModel;
                return result;
            }
            catch (Exception ex)
            {
                result.code = 0;
                result.msg = "保存异常";
                return result;
            }
        }
        [NonAction]
        public List<FD_BadDebtProvisionDetailODataEntity> PackageIdentity(List<FD_BadDebtProvisionDetailODataEntity> lines, string dataDate)
        {

            var provision = _iProvisionRepository.GetLastest(_identityService.EnterpriseId).Result;
            if (provision != null)
            {
                var periodLast = _enterprisePeriodUtil.GetEnterperisePeriod(_identityService.EnterpriseId, provision.DataDate.ToString("yyyy-MM-dd"));
                var periodCurrent = _enterprisePeriodUtil.GetEnterperisePeriod(_identityService.EnterpriseId, dataDate);

                var startDate = periodLast.EndDate.AddDays(1).ToString("yyyy-MM-dd");//上一期的计提准备会计期间最后一天作为开始时间
                var endDate = periodCurrent.EndDate.ToString("yyyy-MM-dd");//当期单据日期所在会计期间的最后一天作为结束时间
                //获取开始时间到结束时间之间的所有计提准备
                var listIdentity = _specificProvider.GetIdentityRange(startDate, endDate);
                lines.ForEach(o =>
                {
                    var item = listIdentity.Find(i => o.AccoSubjectID == i.AccoSubjectID && i.CustomerID == o.CustomerID);
                    if (item != null)
                    {
                        if (item.ProvisionType == ((long)ProvisionTypeEnum.FullProvision).ToString() || item.ProvisionType == ((long)ProvisionTypeEnum.PartProvision).ToString())
                        {
                            o.ProvisionAmount = item.Amount;
                            o.ProvisionType = item.ProvisionType;
                            o.ProvisionTypeName = item.ProvisionTypeName;
                            o.NumericalOrderSpecific = item.NumericalOrderDetail;
                        }
                        else if (item.ProvisionType == ((long)ProvisionTypeEnum.FullNOProvision).ToString())//全额不计提
                        {
                            o.ProvisionAmount = 0;
                            o.ProvisionType = item.ProvisionType;
                            o.ProvisionTypeName = item.ProvisionTypeName;
                            o.NumericalOrderSpecific = item.NumericalOrderDetail;
                        }


                    }
                });
            }
            return lines;
        }

        private List<FD_BadDebtProvisionDetailODataEntity> PackageSetting(List<FD_BadDebtProvisionDetailODataEntity> lines, FD_BaddebtSettingODataEntity settingModel)
        {
            lines.ForEach(o =>
            {
                //if (string.IsNullOrEmpty(o.ProvisionType))
                //{
                //    o.ProvisionType = ((long)ProvisionTypeEnum.AgingProvision).ToString();
                //    o.ProvisionTypeName = "按账龄计提";
                //    decimal amount = 0M;
                //    o.AgingList.ForEach(a =>
                //    {
                //        var s = settingModel?.Lines.Find(i => a.Name == i.Name);
                //        if (s != null)
                //        {
                //            a.Ratio = s.ProvisionRatio;
                //            amount += a.Amount * a.Ratio;
                //        }
                //    });
                //    if (amount > 0)
                //    {
                //        o.ProvisionAmount = amount;
                //    }
                //}
            });

            return lines;
        }
        [NonAction]
        public List<FD_BadDebtProvisionDetailODataEntity> PackageReclassification(string dataDate)
        {
            var requestCustomer1 = new RptAgingReclassificationRequest();
            requestCustomer1.CustomerType = "1";
            requestCustomer1.Enddate = dataDate;
            //requestCustomer1.AccountingSubjectsID = subjectCode;
            var listCustomer1 = GetReclassification(requestCustomer1);

            var requestPerson1 = new RptAgingReclassificationRequest();
            requestPerson1.CustomerType = "2";
            requestPerson1.Enddate = dataDate;
            //requestPerson1.AccountingSubjectsID = subjectCode;
            var listPerson1 = GetReclassification(requestPerson1);
            if (listPerson1?.Count > 0)
            {
                listCustomer1?.AddRange(listPerson1);
            }
            return listCustomer1;
        }

        /// <summary>
        /// 构建数据
        /// </summary>
        /// <param name="listAge"></param>
        /// <param name="provisionModel"></param>
        /// <param name="settinModel"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        private List<FD_BadDebtProvisionDetailODataEntity> BuildDetail(List<DealingOccurDataResult> listAge, FD_BadDebtProvisionODataEntity provisionModel, FD_BaddebtSettingODataEntity settinModel)
        {
            var lines = new List<FD_BadDebtProvisionDetailODataEntity>();
            listAge?.ForEach(o =>
            {
                var detail = new FD_BadDebtProvisionDetailODataEntity()
                {
                    NoReceiveAmount = o.Amount,
                    ProvisionAmount = o.Amount,
                    CustomerID = o.SummaryType1,
                    CustomerName = o.SummaryType1Name
                };
                o?.AgingintervalDatas?.ForEach(a =>
                {
                    var age = new FD_BadDebtProvisionExtODataEntity();
                    age.Name = a.Name;
                    //settinModel.Lines.ForEach(i =>
                    //{
                    //    if (i.IntervalTypeName == a.Name)
                    //    {
                    //        a.Amount = a.Amount * i.ProvisionRatio;
                    //    }
                    //});
                    age.Amount = a.Amount;
                    detail.CurrentDebtPrepareAmount = o.AgingintervalDatas.Sum(ss => ss.Amount);//本期末坏账准备金额
                    detail.AgingList.Add(age);
                });
                lines.Add(detail);
            });

            return lines;
        }

        //增加
        [HttpPost, Route("CreateSettleReceipt")]
        public async Task<RpcResult<ReceiptResult>> CreateSettleReceipt([FromBody] FD_BadDebtProvisionAddCommand request)
        {
            try
            {
                var result = new Result();
                var enterpriseId = Convert.ToInt64(_identityService.EnterpriseId);
                var provisionModel = _provider.GetSingleDataAsync(Convert.ToInt64(request.NumericalOrder)).Result;
                var settingModel = new FD_BaddebtSettingODataEntity();// await _settingProvider.GetSingleDataAsync(Convert.ToInt64(provisionModel.NumericalOrderSetting));

                //凭证1：
                var resultReceipt = new RpcResult<ReceiptResult>();
                var sumProvisonAmount1 = provisionModel.Lines.Where(o => o.AccoSubjectID == provisionModel.AccoSubjectID1).Sum(o => o.ProvisionAmount);
                var diffAmount1 = sumProvisonAmount1 - provisionModel.HaveProvisionAmount1;
                if (diffAmount1 > 0)
                {
                    await SaveReceipt(provisionModel, settingModel, settingModel.OtherAccoSubjectOne, settingModel.BadAccoSubjectOne, diffAmount1);
                }

                //凭证2
                var sumProvisonAmount2 = provisionModel.Lines.Where(o => o.AccoSubjectID == provisionModel.AccoSubjectID2).Sum(o => o.ProvisionAmount);
                var diffAmount2 = sumProvisonAmount2 - provisionModel.HaveProvisionAmount2;
                if (diffAmount2 > 0)
                {
                    resultReceipt = await SaveReceipt(provisionModel, settingModel, settingModel.OtherAccoSubjectTwo, settingModel.BadAccoSubjectTwo, diffAmount2);
                }

                return resultReceipt;
            }
            catch (Exception ex)
            {
                return new RpcResult<ReceiptResult>() { code = 0 };
            }
        }
        [NonAction]
        private async Task<RpcResult<ReceiptResult>> SaveReceipt(FD_BadDebtProvisionODataEntity provisionModel, FD_BaddebtSettingODataEntity settingModel, string debitSubjectId, string creditSubjectId, decimal Amount)
        {
            var recetiptData = new ReceiptData();
            recetiptData.DataA.DataDate = provisionModel.DataDate;
            recetiptData.DataA.TicketedPointID = provisionModel.TicketedPointID;
            recetiptData.DataA.SettleReceipType = ((long)SettleReceiptType.转账凭证).ToString();
            recetiptData.DataA.EnterpriseID = provisionModel.EnterpriseID;
            var receiptDebit = new SettleReceiptDetail();
            receiptDebit.LorR = 0;//借方
            receiptDebit.ReceiptAbstractID = settingModel.ProvisionReceiptAbstractID;
            receiptDebit.Debit = Amount;
            receiptDebit.AccoSubjectID = debitSubjectId;//对方科目一

            var subjectCodeDebit = _baseUtil.GetEnterSubjectList(Convert.ToInt64(debitSubjectId), Convert.ToInt64(_identityService.EnterpriseId), provisionModel.DataDate).Result;
            if (subjectCodeDebit != null)
            {
                var code = subjectCodeDebit.FirstOrDefault(o => o.AccoSubjectID == receiptDebit.AccoSubjectID);
                receiptDebit.AccoSubjectCode = code?.cAccoSubjectCode;
                //if (code != null && code.bDept)
                //{
                //    receiptDebit.MarketID = provisionModel.MarketID;
                //}
                //if (code != null && code.bPerson)
                //{
                //    receiptDebit.PersonID = provisionModel.PersonID;
                //}
            }
            var receiptCredit = new SettleReceiptDetail();
            receiptCredit.LorR = 1;//贷方
            receiptCredit.ReceiptAbstractID = settingModel.ProvisionReceiptAbstractID;
            receiptCredit.Credit = Amount;
            receiptCredit.AccoSubjectID = creditSubjectId;//坏账科目一
            var subjectCodeCredit = _baseUtil.GetEnterSubjectList(Convert.ToInt64(creditSubjectId), Convert.ToInt64(_identityService.EnterpriseId), provisionModel.DataDate).Result;
            if (subjectCodeCredit != null)
            {
                var code = subjectCodeCredit.FirstOrDefault(o => o.AccoSubjectID == receiptCredit.AccoSubjectID);
                receiptCredit.AccoSubjectCode = code?.cAccoSubjectCode;
                //if (code != null && code.bDept)
                //{
                //    receiptCredit.MarketID = provisionModel.MarketID;
                //}
                //if (code != null && code.bPerson)
                //{
                //    receiptCredit.PersonID = provisionModel.PersonID;
                //}
            }
            recetiptData.DataA.Remarks = "计提坏账准备";
            recetiptData.DataB.Add(receiptDebit);
            recetiptData.DataB.Add(receiptCredit);
            var resultReceipt = await _exeUtil.AfterSaveReceipt(recetiptData, provisionModel.NumericalOrder, _identityService.AppId, _identityService.EnterpriseId, _identityService.UserId);
            return resultReceipt;
        }


        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_BadDebtProvisionAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }


        /// <summary>
        /// 获取账龄重分类
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private List<FD_BadDebtProvisionDetailODataEntity> GetReclassification(RptAgingReclassificationRequest param)
        {
            //param.Enddate = request.DataDate;
            param.Boid = _identityService.UserId;
            param.EnteID = _identityService.EnterpriseId;
            param.GroupID = _identityService.GroupId;
            param.EnterpriseList = $"{_identityService.EnterpriseId}";

            param.MenuParttern = "0";//单位
            if (string.IsNullOrEmpty(param.IntervalType) || param.IntervalType == "0")
            {
                param.IntervalType = "1803300944570000101";//默认按年
            }
            param.OwnEntes = new List<string>() { _identityService.EnterpriseId };
            param.CanWatchEntes = new List<string>() { _identityService.EnterpriseId };
            var data = _agingReclassificationUtil.GetData(param).Result?.Where(o => o.AdjustAmount > 0)?.ToList();
            var list = data?.Select(o => new FD_BadDebtProvisionDetailODataEntity()
            {
                BusinessType = param.CustomerType == "1" ? "201611160104402101" : "201611160104402103",//客户:员工
                NoReceiveAmount = o.AdjustAmount,
                ReclassAmount = o.ReclassAmount,//重分类金额
                ProvisionAmount = o.AdjustAmount,
                EndAmount = o.Amount,//期末金额
                AgingList = o.AgingintervalDatas.Select(a => new FD_BadDebtProvisionExtODataEntity { Amount = a.Amount, Name = a.Name }).ToList(),
                CustomerID = o.SummaryType1,
                CustomerName = o.SummaryType1Name,
                AccoSubjectID = o.SummaryType2
            }).ToList();

            return list;
        }

        private List<DealingOccurDataResult> GetAgingData(string dataDate, string accoSubjectId)
        {
            var param = new DealingOccurRequest()
            {
                Boid = _identityService.UserId,//    "1799425",
                Enddate = dataDate,
                EnteID = _identityService.EnterpriseId,//634086739144001721,
                GroupID = _identityService.GroupId,//957025251000000,
                IntervalType = "1803300944570000101",
                EnterpriseList = $"|{_identityService.EnterpriseId}",
                OwnEntes = new List<string>() { _identityService.EnterpriseId },
                CustomerType = "1",
                MenuParttern = "1",
                AccountingSubjectsID = accoSubjectId, //应收科目一   1702241401310000101
            };
            var result = _agingDataUtil.GetAgingData(param).Result;//账龄报表默认只有一条
            return result;
        }

        /// <summary>
        /// 获取科目余额表期末余额
        /// </summary>
        /// <param name="dataDate"></param>
        /// <param name="accoSubjectId"></param>
        /// <returns></returns>
        private decimal GetSubjectBalanceAmount(string dataDate, string accoSubjectId)
        {

            var request = new SubjectBalanceRequest()
            {
                Begindate = Convert.ToDateTime(dataDate).AddDays(-1).ToString("yyyy-MM-dd"),
                Enddate = dataDate,
                AccountingSubjectsRadio = accoSubjectId,
                AccountingSubjectsRadio2 = accoSubjectId,
                AccountingType = "-1",
                OnlyCombineEnte = false,
                EnterpriseList = _identityService.EnterpriseId,
                SummaryType1 = "enterName",
                OwnEntes = new List<string>() { _identityService.EnterpriseId },
                CanWatchEntes = new List<string>() { _identityService.EnterpriseId },
                EnteCateSummary = "",
                GroupID = _identityService.GroupId,
                EnteID = _identityService.EnterpriseId,
                Boid = _identityService.UserId,
                MenuParttern = "0"
            };

            var result = _subjectBalanceUtil.GetData(request).Result;
            var debitYear = result.Sum(o => o.DebitYear);

            return debitYear;
        }


        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_BadDebtProvisionDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_BadDebtProvisionModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
