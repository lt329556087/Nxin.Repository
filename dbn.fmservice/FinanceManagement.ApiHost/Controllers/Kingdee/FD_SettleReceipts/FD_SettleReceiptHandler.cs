using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_SettleReceipt
{
    public class FD_SettleReceiptAddHandler : IRequestHandler<FD_SettleReceiptAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        private TreeModelODataProvider _treeModel;

        public FD_SettleReceiptAddHandler(IIdentityService identityService, TreeModelODataProvider treeModel, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _treeModel = treeModel;
        }
        public async Task<Result> Handle(FD_SettleReceiptAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                result= VerificationMethod(request);
                if (result.code>0)
                {
                    return result;
                }
                long number = _numberCreator.Create<Domain.FD_SettleReceipt>(o => o.DataDate, o => o.Number, DateTime.Now, o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_SettleReceipt()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    SettleReceipType = request.SettleReceipType,
                    DataDate = Convert.ToDateTime(request.DataDate),
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                    Number = number.ToString(),
                    AccountNo = request.AccountNo,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = _identityService.EnterpriseId,
                    OwnerID = _identityService.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    details = new List<FD_SettleReceiptDetail>()
                };
                int i = 1;
                request.Lines?.ForEach(o =>
                {
                    domain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        EnterpriseID = _identityService.EnterpriseId,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        AccoSubjectID = o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        PaymentTypeID = o.PaymentTypeID,
                        AccountID = o.AccountID,
                        LorR = o.LorR,
                        Credit = o.Credit,
                        Debit = o.Debit,
                        Content = o.Content,
                        RowNum = i++,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        IsCharges = false
                    });
                });
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                Biz_Review review = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.msg = "保存成功";
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }
        private Result VerificationMethod(FD_SettleReceiptAddCommand model)
        {
            Result result = new Result() { code = 1 };
            if (model == null)
            {
                result.msg = "参数有误";
                return result;
            }
            #region 表头表体必填字段验证
            if (string.IsNullOrEmpty(model.SettleReceipTypeName))
            {
                result.msg = "凭证类型不能为空";
                return result;
            }
            else
            {
                var settleType= _treeModel.GetBasicsData("SELECT CONVERT(DictID USING utf8mb4) AS Id ,cDictName AS cName,CONVERT(Pid USING utf8mb4) AS Pid  FROM qlw_nxin_com.bsdatadict WHERE pid=201610220104402002 and cDictName='" + model.SettleReceipTypeName+"'");
                if (settleType!=null)
                {
                    model.SettleReceipType = settleType.Id;
                }
                else
                {
                    result.msg = "凭证类型未找到匹配项";
                    return result;
                }
            }
            if (string.IsNullOrEmpty(model.TicketedPointName))
            {
                result.msg = "单据字不能为空";
                return result;
            }
            else
            {
                var ticket = _treeModel.GetBasicsData(@$"SELECT CONVERT(TicketedPointID USING utf8mb4) AS Id ,TicketedPointName AS cName,'' AS Pid  FROM nxin_qlw_business.biz_ticketedpoint WHERE EnterpriseID={_identityService.EnterpriseId} and TicketedPointName='" + model.TicketedPointName+"'");
                if (ticket != null)
                {
                    model.TicketedPointID = ticket.Id;
                }
                else
                {
                    result.msg = "单据字未找到匹配项";
                    return result;
                }
            }
            if (string.IsNullOrEmpty(model.DataDate))
            {
                result.msg = "凭证日期不能为空";
                return result;
            }
            #endregion
            List<TreeModelODataEntity> abstracts = _treeModel.GetBasicsDatas("SELECT CONVERT(SettleSummaryID  USING utf8mb4) AS Id ,SettleSummaryName AS cName,'' AS Pid  FROM qlw_nxin_com.`biz_settlesummary` WHERE   EnterpriseID=" + _identityService.EnterpriseId);
            List<TreeModelODataEntity> subjects = _treeModel.GetBasicsDatas("SELECT CONVERT(AccoSubjectID  USING utf8mb4) AS Id ,AccoSubjectName AS cName,CONVERT(AccoSubjectCode USING utf8mb4) AS Pid  FROM qlw_nxin_com.`biz_accosubject` WHERE   EnterpriseID=" + _identityService.EnterpriseId + " or  EnterpriseID="+_identityService.GroupId);
            var isCustomer = (from s in model.Lines
                              where !string.IsNullOrEmpty(s.CustomerName) && s.CustomerName != "0"
                              select s.CustomerName).Count() > 0;
            List<TreeModelODataEntity> suppliers = isCustomer ? _treeModel.GetBasicsDatas("SELECT CONVERT(SupplierID  USING utf8mb4) AS Id ,SupplierName AS cName,'' AS Pid  FROM nxin_qlw_business.pm_supplier WHERE EnterpriseID=" + _identityService.EnterpriseId) : new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> customers = isCustomer ? _treeModel.GetBasicsDatas("SELECT CONVERT(CustomerID  USING utf8mb4) AS Id ,CustomerName AS cName,'' AS Pid  FROM nxin_qlw_business.sa_customer WHERE EnterpriseID=" + _identityService.EnterpriseId) : new List<TreeModelODataEntity>();
            var isPerson = (from s in model.Lines
                            where !string.IsNullOrEmpty(s.PersonName) && s.PersonName != "0"
                            select s.PersonName).Count() > 0;
            List<TreeModelODataEntity> persons = isPerson ? _treeModel.GetBasicsDatas("SELECT CONVERT(PersonID  USING utf8mb4) AS Id ,NAME AS cName,'' AS Pid  FROM nxin_qlw_business.hr_person ") : new List<TreeModelODataEntity>();
            var isMarket = (from s in model.Lines
                            where !string.IsNullOrEmpty(s.MarketName) && s.MarketName != "0"
                            select s.MarketName).Count() > 0;
            List<TreeModelODataEntity> markets = isMarket ? _treeModel.GetBasicsDatas("SELECT CONVERT(MarketID  USING utf8mb4) AS Id ,MarketName AS cName,'' AS Pid  FROM qlw_nxin_com.biz_market WHERE EnterpriseID=" + _identityService.EnterpriseId) : new List<TreeModelODataEntity>();
            var isProject = (from s in model.Lines
                             where !string.IsNullOrEmpty(s.ProjectName) && s.ProjectName != "0"
                             select s.ProjectName).Count() > 0;
            List<TreeModelODataEntity> projects = isProject ? _treeModel.GetBasicsDatas("SELECT CONVERT(ProjectID  USING utf8mb4) AS Id ,ProjectName AS cName,'' AS Pid  FROM qlw_nxin_com.ppm_project WHERE EnterpriseID=" + _identityService.EnterpriseId) : new List<TreeModelODataEntity>();
            var isProduct = (from s in model.Lines
                             where !string.IsNullOrEmpty(s.ProductName) && s.ProductName != "0"
                             select s.ProductName).Count() > 0;
            List<TreeModelODataEntity> products = isProduct ? _treeModel.GetBasicsDatas("SELECT CONVERT(ProductID  USING utf8mb4) AS Id ,ProductName AS cName,'' AS Pid  FROM qlw_nxin_com.biz_product  WHERE EnterpriseID=" + _identityService.EnterpriseId) : new List<TreeModelODataEntity>();
            var isAccount = (from s in model.Lines
                             where !string.IsNullOrEmpty(s.AccountName) && s.AccountName != "0"
                             select s.AccountName).Count() > 0;
            List<TreeModelODataEntity> accounts = isAccount ? _treeModel.GetBasicsDatas("SELECT CONVERT(AccountID  USING utf8mb4) AS Id ,AccountName AS cName,'' AS Pid  FROM nxin_qlw_business.fd_account WHERE EnterpriseID=" + _identityService.EnterpriseId) : new List<TreeModelODataEntity>();
            var isPaymentType = (from s in model.Lines
                                 where !string.IsNullOrEmpty(s.PaymentTypeName) && s.PaymentTypeName != "0"
                                 select s.PaymentTypeName).Count() > 0;
            List<TreeModelODataEntity> payTypes = isPaymentType ? _treeModel.GetBasicsDatas("SELECT CONVERT(DictID  USING utf8mb4) AS Id ,cDictName AS cName,CONVERT(Pid USING utf8mb4) AS Pid  FROM qlw_nxin_com.bsdatadict WHERE pid=201610140104402001") : new List<TreeModelODataEntity>();
            int i = 1;
            foreach (var item in model.Lines)
            {
                if (string.IsNullOrEmpty(item.ReceiptAbstractName))
                {
                    result.msg = "第" + i + "行摘要不能为空";
                    break;
                }
                else
                {
                    var abstract1 = abstracts.Where(s => s.cName == item.ReceiptAbstractName).FirstOrDefault();
                    if (abstract1 != null)
                    {
                        item.ReceiptAbstractID = abstract1.Id;
                    }
                    else
                    {
                        result.msg = "第" + i + "行摘要未找到匹配项";
                        break;
                    }
                }
                if (string.IsNullOrEmpty(item.AccoSubjectName))
                {
                    result.msg = "第" + i + "行会计科目不能为空";
                    break;
                }
                else
                {
                    var subject = subjects.Where(s => s.cName == item.AccoSubjectName).FirstOrDefault();
                    if (subject != null)
                    {
                        item.AccoSubjectID = subject.Id;
                        item.AccoSubjectCode = subject.Pid;
                    }
                    else
                    {
                        result.msg = "第" + i + "行会计科目未找到匹配项";
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(item.CustomerName))
                {
                   var  customer= customers.Where(s => s.cName == item.CustomerName).FirstOrDefault();
                    var supplier = suppliers.Where(s => s.cName == item.CustomerName).FirstOrDefault();
                    if (customer!=null||supplier!=null)
                    {
                        item.CustomerID = customer!=null?customer.Id:supplier.Id;
                    }
                    else
                    {
                        result.msg = "第" + i + "行客户/供应商未找到匹配项";
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(item.PersonName))
                {
                    var person = persons.Where(s => s.cName == item.PersonName).FirstOrDefault();
                    if (person != null)
                    {
                        item.PersonID = person.Id;
                    }
                    else
                    {
                        result.msg = "第" + i + "行人员未找到匹配项";
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(item.MarketName))
                {
                    var market = markets.Where(s => s.cName == item.MarketName).FirstOrDefault();
                    if (market != null)
                    {
                        item.MarketID = market.Id;
                    }
                    else
                    {
                        result.msg = "第" + i + "行部门未找到匹配项";
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(item.ProjectName))
                {
                    var project = projects.Where(s => s.cName == item.ProjectName).FirstOrDefault();
                    if (project != null)
                    {
                        item.ProjectID = project.Id;
                    }
                    else
                    {
                        result.msg = "第" + i + "行项目未找到匹配项";
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(item.ProductName))
                {
                    var product = products.Where(s => s.cName == item.ProductName).FirstOrDefault();
                    if (product != null)
                    {
                        item.ProductID = product.Id;
                    }
                    else
                    {
                        result.msg = "第" + i + "行商品未找到匹配项";
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(item.AccountName))
                {
                    var account = accounts.Where(s => s.cName == item.AccountName).FirstOrDefault();
                    if (account != null)
                    {
                        item.AccountID = account.Id;
                    }
                    else
                    {
                        result.msg = "第" + i + "行资金账户未找到匹配项";
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(item.PaymentTypeName))
                {
                    var payType = payTypes.Where(s => s.cName == item.PaymentTypeName).FirstOrDefault();
                    if (payType != null)
                    {
                        item.PaymentTypeID = payType.Id;
                    }
                    else
                    {
                        result.msg = "第" + i + "行结算方式未找到匹配项";
                        break;
                    }
                }
                i++;
            }
            if (result.msg!=null)
            {
                return result;
            }
            else
            {
                result.code = 0;
                return result;
            }
        }
    }

    


}
