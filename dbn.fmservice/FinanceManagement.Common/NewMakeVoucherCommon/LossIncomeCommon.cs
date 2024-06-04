using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using FinanceManagement.Common.MonthEndCheckout;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    /// <summary>
    /// 损益结转
    /// </summary>
    public class LossIncomeCommon : MakeVoucherBase
    {

        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public LossIncomeCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
            #region 构造销售汇总表查询条件  别问我为什么。产品非得要
            List<RptSearchModel> rpts = new List<RptSearchModel>() {
             new RptSearchModel(){Text="查询时间",Value=request.Begindate+"-"+request.Enddate},
             new RptSearchModel(){Text="单位筛选",Value=model.EnterpriseName},
            };
            #endregion
            FD_SettleReceipt domain = new FD_SettleReceipt()
            {
                SettleReceipType = request.SettleReceipType,
                TicketedPointID = model.TicketedPointID,
                TransBeginDate = request.Begindate,
                TransEndDate = request.Enddate
            };
            List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
            //reqUrl = _hostCongfiguration.ReportService + "/api/RetSubjectBalance/GetBalanceReport";
            reqUrl = _hostCongfiguration.ReportService + "/api/RptSettleReceipt/GetSettleReceiptReport";
            Dictionary<string, string> dictList = GetDict();
            bool isBreak = true;
            var noProfitSettleList =new List<MySettleReceiptDataResult>();
            foreach (var item in model.Lines)
            {
                if (item.AccoSubjectCode == "4103") continue;
                item.DebitSecFormula = item.DebitSecFormula ?? "0";
                item.CreditSecFormula = item.CreditSecFormula ?? "0";
                if (item.DebitSecFormula.Contains("2208081518000000151") || item.CreditSecFormula.Contains("2208081518000000152")) continue;
                bool isDebit = false;//借贷方标识  false为借，true为贷
                string formular = string.Empty;
                if (!string.IsNullOrEmpty(item.DebitSecFormula) && item.DebitSecFormula != "0") { isDebit = false; formular = item.DebitSecFormula; }
                if (!string.IsNullOrEmpty(item.CreditSecFormula) && item.CreditSecFormula != "0") { isDebit = true; formular = item.CreditSecFormula; }
                List<MySettleReceiptDataResult> lossIncomes = GetLossIncomeDataList(model, request, item, domain, isBreak);//获取会计辅助账数据
                noProfitSettleList.AddRange(lossIncomes);
                lossIncomes?.ForEach(summary =>
                {
                    var detail=SetResultModel(summary, item, isDebit, formular, dictList, request);
                    //GetDetailByFormula(detail, formular);
                    Lines.Add(detail);
                });
                isBreak = false;
            }
            var profitResultList = new List<FD_SettleReceiptDetailCommand>();
            var profitList = model.Lines?.Where(p => p.AccoSubjectCode == "4103")?.ToList();
            //本年利润:4103
            if (profitList?.Count > 0)
            {
                foreach(var item in profitList)
                {
                    item.DebitSecFormula = item.DebitSecFormula ?? "0";
                    item.CreditSecFormula = item.CreditSecFormula ?? "0";
                    if (item.DebitSecFormula.Contains("2208081518000000151") || item.CreditSecFormula.Contains("2208081518000000152")) continue;
                    bool isDebit = false;//借贷方标识  false为借，true为贷
                    string formular = string.Empty;
                    if (!string.IsNullOrEmpty(item.DebitSecFormula) && item.DebitSecFormula != "0") { isDebit = false; formular = item.DebitSecFormula; }
                    if (!string.IsNullOrEmpty(item.CreditSecFormula) && item.CreditSecFormula != "0") { isDebit = true; formular = item.CreditSecFormula; }
                    List<MySettleReceiptDataResult> lossIncomes = new List<MySettleReceiptDataResult>();
                    if (item.IsSum)
                    {
                        lossIncomes = new List<MySettleReceiptDataResult> {new MySettleReceiptDataResult()
                        {
                            Credit = (decimal)Lines?.Sum(s => s.Credit),
                            Debit = (decimal)Lines?.Sum(s => s.Debit)
                        } };
                    }
                    else
                    {
                        lossIncomes = noProfitSettleList;
                    }
                    lossIncomes?.ForEach(summary =>
                    {
                        var credit = summary.Credit;
                        var debit = summary.Debit;
                        summary.Credit = debit;
                        summary.Debit = credit;
                        var detail = SetResultModel(summary, item, isDebit, formular, dictList, request);
                        profitResultList.Add(detail);
                    });
                }
                if(profitResultList.Count > 0)
                {
                    Lines.AddRange(profitResultList);
                }
            }
            rpts.Add(new RptSearchModel() { Text = "汇总方式", Value = domain.SummaryTypeName });
            domain.RptSearchText = JsonConvert.SerializeObject(rpts);
            #region 解析借方合计金额和贷方合计金额
            var debit = model.Lines.LastOrDefault(s => s.DebitSecFormula.Contains("2208081518000000151"));
            var credit = model.Lines.LastOrDefault(s => s.CreditSecFormula.Contains("2208081518000000152"));            
            if (debit != null)
            {
                var sumLine = new FD_SettleReceiptDetailCommand()
                {
                    AccoSubjectCode = debit.AccoSubjectCode,
                    AccoSubjectID = debit.AccoSubjectID,
                    ReceiptAbstractID = debit.ReceiptAbstractID,
                    ReceiptAbstractName = debit.ReceiptAbstractName,
                    Debit = Lines.Sum(s => s.Credit),
                    Credit = Lines.Sum(s => s.Debit),
                    LorR = false,
                };
                if (GetCalFormula(debit.DebitSecFormula))
                {
                    sumLine=GetDetailByFormula(sumLine,true);
                }
                else
                {
                    sumLine.Credit = 0;
                }
                Lines.Add(sumLine);
                domain.Lines = Lines;
                return domain;
            }
            if (credit != null)
            {
                var sumLine = new FD_SettleReceiptDetailCommand()
                {
                    AccoSubjectCode = credit.AccoSubjectCode,
                    AccoSubjectID = credit.AccoSubjectID,
                    ReceiptAbstractID = credit.ReceiptAbstractID,
                    ReceiptAbstractName = credit.ReceiptAbstractName,
                    Credit = Lines.Sum(s => s.Debit),
                    Debit = Lines.Sum(s => s.Credit),
                    LorR = true,
                };
                if (GetCalFormula(credit.CreditSecFormula))
                {
                    sumLine = GetDetailByFormula(sumLine,false);
                }
                else
                {
                    sumLine.Debit = 0;
                }
                Lines.Add(sumLine);
                domain.Lines = Lines;
                return domain;
            }
            #endregion
            domain.Lines = Lines;
            return domain;
        }
        private List<MySettleReceiptDataResult> GetLossIncomeDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak)
        {
            try
            {
                #region 构造接口参数
                //参数：单位 日期 单据字 会计科目-明细表
                var data = new MySettleReceiptRequest()
                {
                    Begindate = request.Begindate,
                    Enddate = request.Enddate,
                    EnterpriseList = model.EnterpriseID,                   
                    TicketedPointID = model.TicketedPointID,
                    AccoSubjectIDLst =item.AccoSubjectCode,
                    reportType = "1",
                    SettleReceipType = "-1",
                    PaymentTypeID = "-1",                    
                    FirstProject = "-1",
                    SecondProject = "-1",                    
                    OnlyCombineEnte = false,
                    DataSource = 0,
                    SummaryType1 = "sg1.SettleSummaryGroupName",
                    IsGroupByEnteCate = false,
                    SummaryT1Rank = "-1",
                    SummaryT2Rank = "-1",
                    SummaryT3Rank = "-1",
                    OwnEntes = new List<string>() { model.EnterpriseID },
                    CanWatchEntes = new List<string>() { model.EnterpriseID },
                    GroupID = Convert.ToInt64(request.GroupID),
                    EnteID = Convert.ToInt64(request.EnterpriseID),
                    MenuParttern = "0",
                    Boid = model.OwnerID.ToString()
                };
                #endregion
                ResultModel<MySettleReceiptDataResult> result = base.postActionByUrl<ResultModel<MySettleReceiptDataResult>, MySettleReceiptRequest>(data);
                List<MySettleReceiptDataResult> resultList = result.ResultState ? result.Data : new List<MySettleReceiptDataResult>();

                if (item.IsSum)
                {
                    return new List<MySettleReceiptDataResult>()
                    {
                        new MySettleReceiptDataResult()
                        {
                             Credit=(decimal)resultList?.Sum(s=>s.Credit),
                             Debit=(decimal)resultList?.Sum(s=>s.Debit)
                        }
                    };
                }
                LossGroupModel groupModel = new LossGroupModel();

                if (item.IsProduct)
                {
                    groupModel.ProductId = true;
                }
                if (item.IsProject)
                {
                    groupModel.ProjectId = true;
                }
                if (item.IsPerson)
                {
                    groupModel.PersonId = true;
                }
                if (item.IsCustomer)
                {
                    groupModel.CustomerId = true;
                }
                if (item.IsMarket)
                {
                    groupModel.DeptId = true;
                }
                foreach (var item2 in resultList)
                {
                    if (string.IsNullOrEmpty(item2.ProductID))
                    {
                        item2.ProductID = "0";
                    }
                    if (string.IsNullOrEmpty(item2.ProjectID))
                    {
                        item2.ProjectID = "0";
                    }
                    if (string.IsNullOrEmpty(item2.PersonID))
                    {
                        item2.PersonID = "0";
                    }
                    if (string.IsNullOrEmpty(item2.CustomerID))
                    {
                        item2.CustomerID = "0";
                    }
                    if (string.IsNullOrEmpty(item2.MarketID))
                    {
                        item2.MarketID = "0";
                    }
                }
                resultList = dynamicGroupbySummary(resultList, groupModel);
                return resultList;
            }
            catch (Exception ex)
            {
                return new List<MySettleReceiptDataResult>();
            }
        }
        public FD_SettleReceiptDetailCommand SetResultModel(MySettleReceiptDataResult summary, FM_NewCarryForwardVoucherDetailODataEntity item, bool isDebit,string formular, Dictionary<string, string> dictList, FM_CarryForwardVoucherSearchCommand request)
        {
            FD_SettleReceiptDetailCommand detail = base.NewObject<MySettleReceiptDataResult>(summary, item, isDebit, formular, dictList);
            if (item.IsPerson) detail.PersonID = summary.PersonID;
            if (item.IsMarket) detail.MarketID = summary.MarketID;
            if (item.IsCustomer) detail.CustomerID = summary.CustomerID;
            if (item.IsProject) detail.ProjectID = summary.ProjectID;
            if (item.IsProduct) detail.ProductID = summary.ProductID;
            if (item.IsSum) detail.EnterpriseID = request.EnterpriseID;
            return detail;
        }
        public List<MySettleReceiptDataResult> dynamicGroupbySummary(List<MySettleReceiptDataResult> summaryList, LossGroupModel groupModel)
        {
            summaryList = summaryList.GroupBy(s => groupModel.GroupBy(s)).Select(s => new MySettleReceiptDataResult()
            {
                MarketID = s.Key.DeptId,
                CustomerID =s.Key.CustomerId,
                PersonID =s.Key.PersonId,
                ProjectID = s.Key.ProjectId,
                ProductID = s.Key.ProductId,
                AccoSubjectCode= s.FirstOrDefault()?.AccoSubjectCode,
                //SummaryType1 = s.FirstOrDefault()?.SummaryType1,
                //SummaryType2 = s.FirstOrDefault()?.SummaryType2,
                //SummaryType3 = s.FirstOrDefault()?.SummaryType3,
                //NumericalOrder = s.FirstOrDefault()?.NumericalOrder,
                //DataDate = s.FirstOrDefault()?.DataDate,
                //Number = s.FirstOrDefault()?.Number,
                //TicketedPointName = s.FirstOrDefault()?.TicketedPointName,
                //ANumber = s.FirstOrDefault()?.ANumber,
                //AccountNo = s.FirstOrDefault()?.AccountNo,
                //SettleSummaryName = s.FirstOrDefault()?.SettleSummaryName,
                //Content = s.FirstOrDefault()?.Content,
                //AccoSubjectName = s.FirstOrDefault()?.AccoSubjectName,
                //CustomerName = s.FirstOrDefault()?.CustomerName,
                //Cname = s.FirstOrDefault()?.Cname,
                //MarketName = s.FirstOrDefault()?.MarketName,
                //HName = s.FirstOrDefault()?.HName,
                //SettleReceipType = s.FirstOrDefault()?.SettleReceipType,
                //twoMarketName = s.FirstOrDefault()?.twoMarketName,
                //ProjectName = s.FirstOrDefault()?.ProjectName,
                //SuoShuDanWei = s.FirstOrDefault()?.SuoShuDanWei,
                //Cashflowproject = s.FirstOrDefault()?.Cashflowproject,
                //OrgMarketName = s.FirstOrDefault()?.OrgMarketName,
                //Remark = s.FirstOrDefault()?.Remark,
                //TicketedPointID = s.FirstOrDefault()?.TicketedPointID,                
                //AccoSubjectID = s.FirstOrDefault()?.AccoSubjectID,
                //IsCus = s.FirstOrDefault()?.IsCus,
                //IsSup = s.FirstOrDefault()?.IsSup,               
                Debit = s.Sum(n => n.Debit),
                Credit = s.Sum(n => n.Credit),
                Balance = s.Sum(n => n.Balance),
            }).ToList();
            return summaryList;
        }
        #region old 科目余额表
        //public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        //{
        //    #region 构造销售汇总表查询条件  别问我为什么。产品非得要
        //    List<RptSearchModel> rpts = new List<RptSearchModel>() {
        //     new RptSearchModel(){Text="查询时间",Value=request.Begindate+"-"+request.Enddate},
        //     new RptSearchModel(){Text="单位筛选",Value=model.EnterpriseName},
        //    };
        //    #endregion
        //    FD_SettleReceipt domain = new FD_SettleReceipt()
        //    {
        //        SettleReceipType = request.SettleReceipType,
        //        TicketedPointID = model.TicketedPointID,
        //        TransBeginDate = request.Begindate,
        //        TransEndDate = request.Enddate
        //    };
        //    List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
        //    reqUrl = _hostCongfiguration.ReportService + "/api/RetSubjectBalance/GetBalanceReport";
        //    Dictionary<string, string> dictList = GetDict();
        //    bool isBreak = true;
        //    foreach (var item in model.Lines)
        //    {
        //        item.DebitSecFormula = item.DebitSecFormula ?? "0";
        //        item.CreditSecFormula = item.CreditSecFormula ?? "0";
        //        if (item.DebitSecFormula.Contains("2208081518000000151") || item.CreditSecFormula.Contains("2208081518000000152")) continue;
        //        bool isDebit = false;//借贷方标识  false为借，true为贷
        //        string formular = string.Empty;
        //        if (!string.IsNullOrEmpty(item.DebitSecFormula) && item.DebitSecFormula != "0") { isDebit = false; formular = item.DebitSecFormula; }
        //        if (!string.IsNullOrEmpty(item.CreditSecFormula) && item.CreditSecFormula != "0") { isDebit = true; formular = item.CreditSecFormula; }
        //        List<SubjectBalance> lossIncomes = GetLossIncomeDataList(model, request, item, domain, isBreak);//获取科目余额表数据
        //        lossIncomes?.ForEach(summary =>
        //        {
        //            FD_SettleReceiptDetailCommand detail = base.NewObject<SubjectBalance>(summary, item, isDebit, formular, dictList);
        //            Lines.Add(detail);
        //        });
        //        isBreak = false;
        //    }
        //    rpts.Add(new RptSearchModel() { Text = "汇总方式", Value = domain.SummaryTypeName });
        //    domain.RptSearchText = JsonConvert.SerializeObject(rpts);
        //    #region 解析借方合计金额和贷方合计金额
        //    var debit = model.Lines.LastOrDefault(s => s.DebitSecFormula.Contains("2208081518000000151"));
        //    var credit = model.Lines.LastOrDefault(s => s.CreditSecFormula.Contains("2208081518000000152"));
        //    if (debit != null)
        //    {
        //        Lines.Add(new FD_SettleReceiptDetailCommand()
        //        {
        //            AccoSubjectCode = debit.AccoSubjectCode,
        //            AccoSubjectID = debit.AccoSubjectID,
        //            ReceiptAbstractID = debit.ReceiptAbstractID,
        //            ReceiptAbstractName = debit.ReceiptAbstractName,
        //            Debit = Lines.Sum(s => s.Credit),
        //            LorR = false,
        //        });
        //        domain.Lines = Lines;
        //        return domain;
        //    }
        //    if (credit != null)
        //    {
        //        Lines.Add(new FD_SettleReceiptDetailCommand()
        //        {
        //            AccoSubjectCode = credit.AccoSubjectCode,
        //            AccoSubjectID = credit.AccoSubjectID,
        //            ReceiptAbstractID = credit.ReceiptAbstractID,
        //            ReceiptAbstractName = credit.ReceiptAbstractName,
        //            Credit = Lines.Sum(s => s.Debit),
        //            LorR = true,
        //        });
        //        domain.Lines = Lines;
        //        return domain;
        //    }
        //    #endregion
        //    domain.Lines = Lines;
        //    return domain;
        //}
        //private List<SubjectBalance> GetLossIncomeDataList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, FD_SettleReceipt domain, bool isBreak)
        //{
        //    try
        //    {
        //        #region 构造接口参数
        //        int rank = _httpClientUtil.GetJsonAsync<int>(_hostCongfiguration.ReportService + $@"/api/RetSubjectBalance/GetSubjectMaxLevelByEnterId?groupId={request.GroupID}&enterId={model.EnterpriseID}").Result;
        //        var data = new SubjectBalanceRequset()
        //        {
        //            Begindate = request.Begindate,
        //            EnterpriseList_id = model.EnterpriseID.ToString(),
        //            Enddate = request.Enddate,
        //            AccountingSubjects = "",
        //            AccountingSubjectsRadio = "",
        //            AccountingSubjects2 = "",
        //            AccountingSubjectsRadio2 = "",
        //            AccountingType = "-1",
        //            OnlyCombineEnte = false,
        //            EnterpriseList = model.EnterpriseID.ToString(),
        //            SubjectLevel = 1,
        //            SubjectLevel2 = rank,
        //            DataSource = 0,
        //            SummaryType1 = "enterName",
        //            IsGroupByEnteCate = false,
        //            SummaryT1Rank = "-1",
        //            SummaryT2Rank = "-1",
        //            SummaryT3Rank = "-1",
        //            OwnEntes = new List<string>() { model.EnterpriseID.ToString() },
        //            CanWatchEntes = new List<string>() { model.EnterpriseID.ToString() },
        //            GroupID = Convert.ToInt64(request.GroupID),
        //            EnteID = Convert.ToInt64(request.EnterpriseID),
        //            MenuParttern = "0",
        //            Boid = model.OwnerID.ToString()
        //        };
        //        #endregion
        //        ResultModel<SubjectBalance> result = base.postActionByUrl<ResultModel<SubjectBalance>, SubjectBalanceRequset>(data);
        //        List<SubjectBalance> resultList= result.ResultState? result.Data : new List<SubjectBalance>();
        //        if (item.IsSum)
        //        {
        //            return new List<SubjectBalance>()
        //            {
        //                new SubjectBalance()
        //                {
        //                     Credit=(decimal)resultList?.Sum(s=>s.Credit),
        //                     Debit=(decimal)resultList?.Sum(s=>s.Debit)
        //                }
        //            };
        //        }
        //        else
        //        {
        //            return resultList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<SubjectBalance>();
        //    }
        //}
        #endregion
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("2203161432070000102", "Debit");//本期借方
            value.Add("2203161432070000103", "Credit");//本期贷方
            return value;
        }
        public bool GetCalFormula(string formular)
        {
            var list=new List<string>();
            list.Add("[2208081518000000151]-[2208081518000000152]");
            list.Add("[2208081518000000152]-[2208081518000000151]");
            list.Add("([2208081518000000151]-[2208081518000000152])");
            list.Add("([2208081518000000152]-[2208081518000000151])");
            if (list.Where(p => p == formular)?.Count() > 0) { return true; }
            return false;
        }

        public FD_SettleReceiptDetailCommand GetDetailByFormula(FD_SettleReceiptDetailCommand detail,bool isDebit)
        {
            var credit = detail.Credit;
            var debit = detail.Debit;
            var diff = credit - debit;
            if (diff < 0)
            {
                detail.Credit = 0;
                detail.Debit = -diff;
            }
            else
            {
                detail.Credit = diff;
                detail.Debit = 0;
            }
            return detail;
        }
    }

}
