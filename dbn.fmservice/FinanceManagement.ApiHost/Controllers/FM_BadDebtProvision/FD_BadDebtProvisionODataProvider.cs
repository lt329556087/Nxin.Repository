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

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtProvision
{
    public class FD_BadDebtProvisionODataProvider : OneWithManyQueryProvider<FD_BadDebtProvisionODataEntity, FD_BadDebtProvisionDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BadDebtProvisionODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BadDebtProvisionListOnlyODataEntity> GetList(ODataQueryOptions<FD_BadDebtProvisionListOnlyODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetMainList(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_BadDebtProvisionListOnlyODataEntity> GetMainList(ODataQueryOptions<FD_BadDebtProvisionListOnlyODataEntity> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_BadDebtProvisionODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT 
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.NumericalOrderSetting USING utf8mb4) NumericalOrderSetting,
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,
                                        CONVERT(A.MarketID USING utf8mb4) MarketID,
                                        CONVERT(A.PersonID USING utf8mb4) PersonID,
                                        CONVERT(A.DataDate USING utf8mb4) DataDate,
                                        CONVERT(A.TicketedPointID USING utf8mb4) TicketedPointID,
                                        CONVERT(A.Number USING utf8mb4) Number,
                                        CONVERT(A.AccoSubjectID1 USING utf8mb4) AccoSubjectID1,
                                        s1.AccoSubjectFullName   AccoSubjectName1,
                                        CONVERT(A.AccoSubjectID2 USING utf8mb4) AccoSubjectID2,
                                        s2.AccoSubjectFullName AccoSubjectName2,
                                        CONVERT(A.CreateDate USING utf8mb4) CreateDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,
                                        A.Remarks,
                                        A.HaveProvisionAmount1,
                                        A.HaveProvisionAmount2
                                        from nxin_qlw_business.FD_BadDebtProvision A 
                                        Left join qlw_nxin_com.biz_accosubject S1 on A.AccoSubjectID1 = S1.AccoSubjectID
                                        Left join qlw_nxin_com.biz_accosubject S2 on A.AccoSubjectID2 = S2.AccoSubjectID
                                    where A.NumericalOrder ={manyQuery}";
            return _context.FD_BadDebtProvisionDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }


        public IQueryable<FD_BadDebtProvisionExtODataEntity> GetExtDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
                                        RecordID,	
                                        CONVERT(A.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        AgingID,	
                                        Name,	
                                        Amount,
                                        Ratio
                                        from  nxin_qlw_business.fd_baddebtprovisionext A
                                        where A.NumericalOrderDetail = {manyQuery} ";

            return _context.FD_BadDebtProvisionExtDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FD_BadDebtProvisionListOnlyODataEntity> GetMainList(NoneQuery query = null)
        {
            FormattableString sql = $@"select 
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(A.Number USING utf8mb4) Number,
                                    CONVERT(A.DataDate USING utf8mb4) DataDate,
                                    sum( IFNULL(ProvisionAmount,0)) ProvisionAmount,
                                    hr1.name OwnerName,
                                    CONVERT(hr1.bo_id USING utf8mb4) OwnerID,
                                    hr2.name ReviewName,
                                    CONVERT(hr2.bo_id USING utf8mb4) ReviewID
                                    from nxin_qlw_business.FD_BadDebtProvision A 
                                    Left join nxin_qlw_business.fd_baddebtprovisiondetail B on A.NumericalOrder = B.NumericalOrder
                                    left join nxin_qlw_business.biz_reviwe r1 on a.NumericalOrder = r1.NumericalOrder and r1.checkMark=65536
                                    left join nxin_qlw_business.hr_person hr1 on hr1.BO_ID = r1.CheckedByID
                                    left join nxin_qlw_business.biz_reviwe r2 on a.NumericalOrder = r2.NumericalOrder and r2.checkMark=16
                                    left join nxin_qlw_business.hr_person hr2 on hr2.BO_ID = r2.CheckedByID
                                    WHERE  A.EnterpriseID = {_identityservice.EnterpriseId}
                                    GROUP BY A.NumericalOrder Order by A.CreateDate desc";

            return _context.FD_BadDebtProvisionListOnlyDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<FD_BadDebtProvisionODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@"";

            return _context.FD_BadDebtProvisionDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public override Task<List<FD_BadDebtProvisionDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        CONVERT(A.NumericalOrderSpecific USING utf8mb4) NumericalOrderSpecific,
                                        CONVERT(A.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                        s1.AccoSubjectFullName   AccoSubjectName,
                                        CONVERT(A.CustomerID USING utf8mb4) CustomerID,
                                        CONVERT(A.BusinessType USING utf8mb4) BusinessType,
                                        C.CustomerName,
                                        NoReceiveAmount,	
                                        CurrentDebtPrepareAmount,	
                                        LastDebtPrepareAmount	,
                                        TransferAmount	,
                                        CONVERT(A.ProvisionType USING utf8mb4) ProvisionType,
                                        d.DataDictName ProvisionTypeName,
                                        ProvisionAmount,
                                        ReclassAmount,
                                        EndAmount
                                        from nxin_qlw_business.fd_baddebtprovisiondetail A
                                        Left join qlw_nxin_com.biz_customer  c  on A.CustomerID = c.CustomerID
                                        Left join qlw_nxin_com.biz_accosubject S1 on A.AccoSubjectID = S1.AccoSubjectID
                                        left join nxin_qlw_business.biz_datadict d on d.DataDictID = A.ProvisionType
                                    where A.NumericalOrder = {manyQuery}";
            return _context.FD_BadDebtProvisionDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }


       

    }
}
