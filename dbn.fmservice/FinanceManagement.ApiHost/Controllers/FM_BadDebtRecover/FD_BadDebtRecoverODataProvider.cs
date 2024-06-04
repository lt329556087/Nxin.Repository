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

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtRecover
{
    public class FD_BadDebtRecoverODataProvider : OneWithManyQueryProvider<FD_BadDebtRecoverODataEntity, FD_BadDebtRecoverDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BadDebtRecoverODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BadDebtRecoverListOnly> GetList(ODataQueryOptions<FD_BadDebtRecoverListOnly> odataqueryoptions, Uri uri)
        {
            var datas = GetMainList(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public  IQueryable<FD_BadDebtRecoverListOnly> GetMainList(ODataQueryOptions<FD_BadDebtRecoverListOnly> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_BadDebtRecoverODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT   
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.NumericalOrderReceive USING utf8mb4) NumericalOrderReceive,
                                          CONVERT(A.NumericalOrderSetting USING utf8mb4) NumericalOrderSetting,
                                        CONVERT(r.Number USING utf8mb4) NumberReceive,
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,
                                        CONVERT(A.DataDate USING utf8mb4) DataDate,
                                        CONVERT(A.TicketedPointID USING utf8mb4) TicketedPointID,
                                        CONVERT(A.Number USING utf8mb4) Number,
                                        CONVERT(A.CustomerID USING utf8mb4) CustomerID,
                                        CONVERT(A.BusinessType USING utf8mb4) BusinessType,
                                        CONVERT(A.PersonID USING utf8mb4) PersonID,
                                        CONVERT(A.CAccoSubjectID USING utf8mb4) CAccoSubjectID,
                                        s3.AccoSubjectFullName    CAccoSubjectName,
                                        CONVERT(A.AccoSubjectID1 USING utf8mb4) AccoSubjectID1,
                                        s1.AccoSubjectFullName   AccoSubjectName1,
                                        CONVERT(A.AccoSubjectID2 USING utf8mb4) AccoSubjectID2,
                                        s2.AccoSubjectFullName AccoSubjectName2,
                                        CONVERT(A.CreateDate USING utf8mb4) CreateDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate 
                                        FROM nxin_qlw_business.FD_BadDebtReCover A
                                        Left join qlw_nxin_com.biz_accosubject S1 on A.AccoSubjectID1 = S1.AccoSubjectID
                                        Left join qlw_nxin_com.biz_accosubject S2 on A.AccoSubjectID2 = S2.AccoSubjectID
                                        Left join qlw_nxin_com.biz_accosubject s3 on A.CAccoSubjectID = S3.AccoSubjectID
                                        Left join nxin_qlw_business.fd_settlereceipt r on A.NumericalOrderReceive = r.NumericalOrder
                                        where A.NumericalOrder ={manyQuery}";
            return _context.FD_BadDebtRecoverDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public  IQueryable<FD_BadDebtRecoverListOnly> GetMainList(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT 
                                        CONVERT(A.CreateDate USING utf8mb4) CreateDate,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(a.Number USING utf8mb4) Number,
                                        sum(b.CurrentRecoverAmount) CurrentRecoverAmount,
                                        hr1.name OwnerName,
                                        CONVERT(hr1.bo_id USING utf8mb4) OwnerID,
                                        hr2.name ReviewName,
                                        CONVERT(hr2.bo_id USING utf8mb4) ReviewID,
                                        CONVERT(a.CustomerID USING utf8mb4) CustomerID,
                                        c.customerName
                                        from 
                                        nxin_qlw_business.fd_baddebtrecover a inner join nxin_qlw_business.fd_baddebtrecoverdetail b on a.NumericalOrder = b.NumericalOrder
                                        Left join (select CustomerID,CustomerName from  nxin_qlw_business.sa_customer  where EnterpriseID ={_identityservice.EnterpriseId}   GROUP BY CustomerID) c on A.CustomerID = c.CustomerID
                                        left join nxin_qlw_business.biz_reviwe r1 on a.NumericalOrder = r1.NumericalOrder and r1.checkMark=65536
                                        left join nxin_qlw_business.hr_person hr1 on hr1.BO_ID = r1.CheckedByID
                                        left join nxin_qlw_business.biz_reviwe r2 on a.NumericalOrder = r2.NumericalOrder and r2.checkMark=16
                                        left join nxin_qlw_business.hr_person hr2 on hr2.BO_ID = r2.CheckedByID
                                        WHERE  a.EnterpriseID = {_identityservice.EnterpriseId}
                                        GROUP BY a.NumericalOrder
                                        order by a.CreateDate";

            return _context.FD_BadDebtRecoverListOnlyDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<FD_BadDebtRecoverODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public override Task<List<FD_BadDebtRecoverDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
                                    RecordID,
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CurrentRecoverAmount,
                                    Amount,
                                    CONVERT(A.PersonID USING utf8mb4) PersonID,
                                    hr.`Name` PersonName,
                                    CONVERT(A.MarketID USING utf8mb4) MarketID,
                                    CONVERT(A.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                    m.MarketName 
                                    from nxin_qlw_business.fd_baddebtRecOverdetail A
                                    left join qlw_nxin_com.biz_market m on  A.MarketID = m.MarketID
                                    left join nxin_qlw_business.hr_person hr on A.PersonID = hr.PersonID 
                                    where A.NumericalOrder = {manyQuery}";
            return _context.FD_BadDebtRecoverDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
    }


    public class FD_PaymentReceivablesODataProvider
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_PaymentReceivablesODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_PaymentReceivablesODataEntity> GetList(ODataQueryOptions<FD_PaymentReceivablesODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetMainList(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_PaymentReceivablesODataEntity> GetMainList(ODataQueryOptions<FD_PaymentReceivablesODataEntity> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FD_PaymentReceivablesODataEntity> GetMainList(NoneQuery query = null)
        {
            FormattableString sql = $@" 
                                    SELECT 
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(A.DataDate USING utf8mb4) DataDate,
                                    CONVERT(A.Number USING utf8mb4) Number,
                                    CONVERT(B.CustomerID USING utf8mb4) CustomerID,
                                    C.CustomerName,
                                    s.SettleSummaryName ReceiptAbstractName,
                                    IFNULL(B.Amount,0) Amount,
                                    CONVERT(B.PersonID USING utf8mb4) PersonID,
                                    hr.Name PersonName,
                                    CONVERT(B.MarketID USING utf8mb4) MarketID,
                                    m.MarketName,
                                    D.AccoSubjectCode
                                    from nxin_qlw_business.fd_paymentreceivables A 
                                    Left join  nxin_qlw_business.fd_paymentreceivablesdetail B  on A.NumericalOrder = B.NumericalOrder 
                                    left join nxin_qlw_business.fd_settlereceiptdetail D  on A.NumericalOrder = D.NumericalOrder and D.LorR = 1
                                    Left join (select CustomerID,CustomerName from  nxin_qlw_business.sa_customer  where EnterpriseID ={_identityservice.EnterpriseId}
                                    GROUP BY CustomerID) c on B.CustomerID = c.CustomerID
                                    Left join qlw_nxin_com.biz_settlesummary s on B.ReceiptAbstractID  =S.SettleSummaryID 
                                    left join qlw_nxin_com.biz_market m on  B.MarketID = m.MarketID
                                    left join nxin_qlw_business.hr_person hr on B.PersonID = hr.PersonID 
                                    WHERE  A.EnterpriseID = {_identityservice.EnterpriseId}
                                    and A.SettleReceipType = 201611180104402201
                                    and (D.AccoSubjectCode like '1122%' or   D.AccoSubjectCode like '1231%'  )
                                    GROUP BY a.NumericalOrder
                                    order by a.DataDate desc";

            return _context.FD_PaymentReceivablesDataSet.FromSqlInterpolated(sql);
        }
    }


}
