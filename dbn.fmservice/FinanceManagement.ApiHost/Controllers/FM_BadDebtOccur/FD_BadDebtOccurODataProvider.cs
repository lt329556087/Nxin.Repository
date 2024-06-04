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

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtOccur
{
    public class FD_BadDebtOccurODataProvider : OneWithManyQueryProvider<FD_BadDebtOccurODataEntity, FD_BadDebtOccurDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BadDebtOccurODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BadDebtOccurListOnly> GetList(ODataQueryOptions<FD_BadDebtOccurListOnly> odataqueryoptions, Uri uri)
        {
            var datas = GetMainList(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_BadDebtOccurListOnly> GetMainList(ODataQueryOptions<FD_BadDebtOccurListOnly> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FD_BadDebtOccurListOnly> GetMainList(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT 
                                        CONVERT(A.CreateDate USING utf8mb4) CreateDate,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(a.Number USING utf8mb4) Number,
                                        sum(b.CurrentOccurAmount) CurrentOccurAmount,
                                        hr1.name OwnerName,
                                        CONVERT(hr1.bo_id USING utf8mb4) OwnerID,
                                        hr2.name ReviewName,
                                        CONVERT(hr2.bo_id USING utf8mb4) ReviewID,
                                        CONVERT(a.CustomerID USING utf8mb4) CustomerID,
                                        c.customerName
                                        from 
                                        nxin_qlw_business.fd_baddebtoccur a inner join nxin_qlw_business.fd_baddebtoccurdetail b on a.NumericalOrder = b.NumericalOrder
                                        Left join (select CustomerID,CustomerName from  nxin_qlw_business.sa_customer  where EnterpriseID =  {_identityservice.EnterpriseId} GROUP BY CustomerID) c on A.CustomerID = c.CustomerID
                                        left join nxin_qlw_business.biz_reviwe r1 on a.NumericalOrder = r1.NumericalOrder and r1.checkMark=65536
                                        left join nxin_qlw_business.hr_person hr1 on hr1.BO_ID = r1.CheckedByID
                                        left join nxin_qlw_business.biz_reviwe r2 on a.NumericalOrder = r2.NumericalOrder and r2.checkMark=16
                                        left join nxin_qlw_business.hr_person hr2 on hr2.BO_ID = r2.CheckedByID
                                        WHERE  a.EnterpriseID = {_identityservice.EnterpriseId}
                                        GROUP BY a.NumericalOrder
                                        order by a.CreateDate ";
            return _context.FD_BadDebtOccurListOnlyDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_BadDebtOccurODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT   
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.NumericalOrderSetting USING utf8mb4) NumericalOrderSetting,
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
                                        FROM nxin_qlw_business.FD_BadDebtOccur A
                                        Left join qlw_nxin_com.biz_accosubject S1 on A.AccoSubjectID1 = S1.AccoSubjectID
                                        Left join qlw_nxin_com.biz_accosubject S2 on A.AccoSubjectID2 = S2.AccoSubjectID
                                          Left join qlw_nxin_com.biz_accosubject s3 on A.CAccoSubjectID = S3.AccoSubjectID
                                    where A.NumericalOrder ={manyQuery}";
            return _context.FD_BadDebtOccurDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<FD_BadDebtOccurODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public override Task<List<FD_BadDebtOccurDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
                                    RecordID,
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CurrentOccurAmount,
                                    Amount,
                                    CONVERT(A.PersonID USING utf8mb4) PersonID,
                                    hr.`Name` PersonName,
                                    CONVERT(A.MarketID USING utf8mb4) MarketID,
                                    CONVERT(A.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                    m.MarketName 
                                    from nxin_qlw_business.fd_baddebtoccurdetail A
                                    left join qlw_nxin_com.biz_market m on  A.MarketID = m.MarketID
                                    left join nxin_qlw_business.hr_person hr on A.PersonID = hr.PersonID 
                                    where A.NumericalOrder = {manyQuery}";
            return _context.FD_BadDebtOccurDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }

     
    }
}
