using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class FD_CapitalBudgetODataProvider : OneWithManyQueryProvider<FD_CapitalBudgetODataEntity, FD_CapitalBudgetDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_CapitalBudgetODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_CapitalBudgetODataEntity> GetList(ODataQueryOptions<FD_CapitalBudgetODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_CapitalBudgetODataEntity> GetData(ODataQueryOptions<FD_CapitalBudgetODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FD_CapitalBudgetODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@"	SELECT  
                                CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                CONVERT(FD.Guid USING utf8mb4) Guid,
                                FD.RecordID,
                                date_format( FD.DataDate,'%Y-%m-%d') DataDate, 
                                CONVERT(FD.TicketedPointID USING utf8mb4) TicketedPointID,
                                CONVERT(FD.Number USING utf8mb4) Number,
                                CONVERT(FD.CapitalBudgetType USING utf8mb4) CapitalBudgetType, 
                                CONVERT(FD.CapitalBudgetAbstract USING utf8mb4) CapitalBudgetAbstract, 
                                date_format( FD.StartDate,'%Y-%m-%d')  StartDate, 
                                date_format( FD.EndDate,'%Y-%m-%d')  EndDate,
                                CONVERT(FD.OwnerID USING utf8mb4) OwnerID, 
                                FD.Remarks, 
                                hr1.Name  as  OwnerName, 
                                CONVERT(hr2.bo_id USING utf8mb4)  CheckedByID,
                                hr2.Name as  CheckedByName,
                                CONVERT(hr3.bo_id USING utf8mb4)  FinanceID,
                                hr3.Name as  FinanceName,
                                CONVERT(FD.EnterpriseID USING utf8mb4) EnterpriseID, 
                                FD.CreatedDate, 
                                '' Duration,
                                FD.ModifiedDate ,Ifnull(FD.Amount,0) Amount,
                                IFNULL(dc1.DataDictName,'')	AbstractName, 
                                IFNULL(BIZ_E.EnterpriseName,'') EnterpriseName,
                                CONVERT(FD.MarketID USING utf8mb4) MarketID, 
                                dc.DataDictName as MarketName
                                FROM qlw_nxin_com.FD_CapitalBudget FD
                                Left Join nxin_qlw_business.biz_datadict  dc1 ON FD.CapitalBudgetAbstract=dc1.DataDictID
                                Left join nxin_qlw_business.biz_datadict dc on dc.DataDictID = fd.MarketID
                                Left Join qlw_nxin_com.BIZ_Enterprise BIZ_E ON BIZ_E.EnterpriseID=FD.EnterpriseID
                                Left join nxin_qlw_business.biz_reviwe r1 on FD.NumericalOrder=r1.NumericalOrder and r1.CheckMark = 65536
                                Left join nxin_qlw_business.biz_reviwe r2 on FD.NumericalOrder=r2.NumericalOrder and r2.CheckMark = 16 
                                Left join nxin_qlw_business.biz_reviwe r3 on FD.NumericalOrder=r3.NumericalOrder and r3.CheckMark = 2048
                                Left join nxin_qlw_business.hr_person hr1 on hr1.bo_id = r1.CheckedByID 
                                Left join nxin_qlw_business.hr_person hr2 on hr2.bo_id= r2.CheckedByID
                                Left join nxin_qlw_business.hr_person hr3 on hr3.bo_id= r3.CheckedByID
                                WHERE CapitalBudgetAbstract=201612130104402203
                                and FD.EnterpriseID= {_identityservice.EnterpriseId}
                                GROUP BY FD.NumericalOrder";
            return _context.FD_CapitalBudgetDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_CapitalBudgetODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = @$" SELECT 
                                        CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                        FD.RecordID, 
                                        CONVERT(FD.Guid USING utf8mb4) Guid,
                                        date_format( FD.DataDate,'%Y-%m-%d') DataDate, 
                                        CONVERT(FD.TicketedPointID USING utf8mb4) TicketedPointID,
                                        CONVERT(FD.Number USING utf8mb4) Number,
                                        CONVERT(FD.CapitalBudgetType USING utf8mb4) CapitalBudgetType,
                                        CONVERT(FD.CapitalBudgetAbstract USING utf8mb4) CapitalBudgetAbstract,
                                        date_format( FD.StartDate,'%Y-%m-%d')  StartDate, 
                                        date_format(  FD.EndDate,'%Y-%m-%d')  EndDate, 
                                        FD. Remarks, '' as  OwnerName,
                                        CONVERT(FD.EnterpriseID USING utf8mb4) EnterpriseID,
                                       Ifnull(FD.Amount,0) Amount,
                                        IFNULL(dc1.DataDictName,'')	AbstractName, 
                                        IFNULL(BIZ_E.EnterpriseName,'') EnterpriseName,
                                        ''  CheckedByID,
                                        ''  FinanceID,
                                        ''   FinanceName,
                                        hr.Name CheckedByName, '' Duration,
                                        CONVERT(FD.MarketID USING utf8mb4) MarketID,
                                        dc2.DataDictName  as MarketName
                                        FROM qlw_nxin_com.FD_CapitalBudget FD
                                        Left Join qlw_nxin_com.BIZ_Enterprise BIZ_E ON BIZ_E.EnterpriseID=FD.EnterpriseID
                                        Left Join nxin_qlw_business.biz_datadict dc1 ON FD.CapitalBudgetAbstract=dc1.DataDictID
                                        Left join nxin_qlw_business.biz_datadict dc2 on FD.MarketID = dc2.DataDictID
                                        Left join nxin_qlw_business.biz_reviwe r1 on fd.Numericalorder = r1.Numericalorder and  r1.CheckMark =16 
                                        left join nxin_qlw_business.hr_person hr on r1.CheckedByID = hr.bo_id
                                        WHERE FD.NumericalOrder = {manyQuery}";

            return _context.FD_CapitalBudgetDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        public override Task<List<FD_CapitalBudgetDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"
                                    SELECT 
                                    FD.RecordID,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(FD.Guid USING utf8mb4) Guid,
                                    CONVERT(FD.ReceiptAbstractID USING utf8mb4) ReceiptAbstractID,
                                    CONVERT(FD.PaymentObjectID USING utf8mb4) PaymentObjectID,
                                    dc.DataDictName PaymentObjectName,
                                    IFNULL(FD.PayAmount,0.0)  PayAmount,
                                    IFNULL(FD.ReceiptAmount,0.0)  ReceiptAmount,
                                    FD.Remarks, 
                                    IFNULL(SettleSummaryName,'')  ReceiptAbstractName,
                                    IFNULL(bssg.SettleSummaryGroupCode,'')  SettleSummaryGroupCode
                                    FROM qlw_nxin_com.FD_CapitalBudgetDetail FD
                                    Left Join qlw_nxin_com.biz_settlesummary  bss ON bss.SettleSummaryID=FD.ReceiptAbstractID
                                    Left join nxin_qlw_business.biz_datadict dc on FD.PaymentObjectID = dc.DataDictID
                                    left join qlw_nxin_com.BIZ_SettleSummaryGroup as bssg on bssg.SettleSummaryGroupID=bss.SettleSummaryGroupID
                                    where FD.NumericalOrder = {manyQuery}";

            return _context.FD_CapitalBudgetDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }

        public Task<List<AuditODataEntity>> GetAudit(ODataQueryOptions<AuditODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = @$"	SELECT 
                                    CONVERT(f.Numericalorder USING utf8mb4) NumericalOrder,
                                    en.EnterpriseName,
                                    CONVERT(f.DataDate USING utf8mb4)  DataDate,
                                    CONVERT(dc.DictID  USING utf8mb4)  ExpenseType,
                                    CONVERT(dc.cDictName  USING utf8mb4)  ExpenseTypeName,
                                    bs.SettleSummaryName ReceiptAbstractName,
                                    sum(IFNull(fd.Amount,0)) Amount,
                                    0 ApprovalState, 
                                    '' ApprovalStateName,
                                    ifnull(hr2.Name,hr.Name)  OwnerName
                                    from qlw_nxin_com.fd_expense f
                                    left join qlw_nxin_com.fd_expensedetail fd on f.NumericalOrder = fd.Numericalorder
                                    left join qlw_nxin_com.bsdatadict dc on f.ExpenseType = dc.DictID
                                    left join qlw_nxin_com.biz_settlesummary bs on fd.ReceiptAbstractID = bs.SettleSummaryID
                                    left join qlw_nxin_com.biz_enterprise en on en.EnterpriseID = f.enterpriseId
                                    left join nxin_qlw_business.biz_reviwe r on f.NumericalOrder = r.NumericalOrder and CheckMark = 65536
                                    left join nxin_qlw_business.hr_person hr on hr.bo_id= r.CheckedByID
                                    left join nxin_qlw_business.hr_person hr2 on hr2.BO_ID=f.OwnerID
                                    where f.EnterpriseID={_identityservice.EnterpriseId}
                                    GROUP BY f.Numericalorder";
            return _context.AuditDataSet.FromSqlInterpolated(sql).ToListAsync();
        }




    }
}
