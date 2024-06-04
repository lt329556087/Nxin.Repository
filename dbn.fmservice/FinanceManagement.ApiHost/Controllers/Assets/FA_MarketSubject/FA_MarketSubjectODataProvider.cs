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
    public class FA_MarketSubjectODataProvider : OneWithManyQueryProvider<FA_MarketSubjectODataEntity, FA_MarketSubjectDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FA_MarketSubjectODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FA_MarketSubjectODataEntity> GetList(ODataQueryOptions<FA_MarketSubjectODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FA_MarketSubjectODataEntity> GetData(ODataQueryOptions<FA_MarketSubjectODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FA_MarketSubjectODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        A.Remarks,
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName
                                        FROM nxin_qlw_business.FA_MarketSubject A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                        WHERE A.EnterpriseID={_identityservice.EnterpriseId} ORDER BY A.DataDate DESC";
            return _context.FA_MarketSubjectDataSet.FromSqlInterpolated(sql);
        }
        public IQueryable<FA_MarketSubjectODataEntity> GetDataByDate(string date)
        {
            FormattableString sql = $@"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        A.Remarks,
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName
                                        FROM nxin_qlw_business.FA_MarketSubject A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                        WHERE A.EnterpriseID={_identityservice.EnterpriseId} and A.DataDate<='{date}' ORDER BY A.DataDate DESC";
            return _context.FA_MarketSubjectDataSet.FromSqlInterpolated(sql);
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FA_MarketSubjectODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = @$"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        A.Remarks,
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,                
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName
                                        FROM nxin_qlw_business.FA_MarketSubject A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                    where A.NumericalOrder ={manyQuery}";

            return _context.FA_MarketSubjectDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        public override Task<List<FA_MarketSubjectDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            //FormattableString sql = $@"SELECT 
            //                        CONVERT(F.RecordID ,SIGNED) RecordID, 
            //                        CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
            //                        CONVERT(F.MarketID USING utf8mb4) MarketID, 
            //                        CONVERT(F.AccoSubjectID USING utf8mb4) AccoSubjectID,
            //                        CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
            //                        m.MarketName,
            //                        a.AccoSubjectFullName
            //                        FROM nxin_qlw_business.FA_MarketSubjectDetail F 
            //                        LEFT JOIN qlw_nxin_com.biz_market m ON F.MarketID=m.MarketID
            //                        LEFT JOIN qlw_nxin_com.biz_accosubject a ON F.AccoSubjectID =a.AccoSubjectID                         
            //                        where F.NumericalOrder = {manyQuery} ORDER BY f.RecordID";
            var sql = GetDetailSql(manyQuery);
            return _context.FA_MarketSubjectDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        private FormattableString GetDetailSql(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.MarketID USING utf8mb4) MarketID, 
                                    CONVERT(F.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
                                    m.MarketName,
                                    a.AccoSubjectFullName,
                                    a.Rank
                                    FROM nxin_qlw_business.FA_MarketSubjectDetail F 
                                    LEFT JOIN qlw_nxin_com.biz_market m ON F.MarketID=m.MarketID
                                    LEFT JOIN qlw_nxin_com.biz_accosubject a ON F.AccoSubjectID =a.AccoSubjectID                         
                                    where F.NumericalOrder = {manyQuery} ORDER BY f.RecordID";
            return sql;
        }
        public List<FA_MarketSubjectDetailODataEntity> GetDetaiData(long manyQuery)
        {
            var sql = GetDetailSql(manyQuery);
            return _context.FA_MarketSubjectDetailDataSet.FromSqlInterpolated(sql).ToList();
        }
        ///// <summary>
        ///// 根据流水号获取详情
        ///// </summary>
        ///// <param name="manyQuery"></param>
        ///// <returns></returns>
        //public Task<List<FA_MarketSubjectDetailODataEntity>> GetDetaiListAsync(string key)
        //{
        //    FormattableString sql = $@"SELECT 
        //                            CONVERT(F.RecordID ,SIGNED) RecordID, 
        //                            CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
        //                            CONVERT(F.AgingIntervalID ,SIGNED) AgingIntervalID, 
        //                            IFNULL(F.ProvisionRatio,0) ProvisionRatio, 
        //                             F.Remarks, 
        //                            CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
        //                            CONVERT(fad.IntervalType USING utf8mb4) IntervalType, 
        //                            d.cDictName IntervalTypeName,
        //                            fad.Name,
        //                            IFNULL(fad.DayNum,0) DayNum,
        //                            fad.Serial,
        //                            'U' RowStatus
        //                            FROM nxin_qlw_business.FA_MarketSubjectDetail F 
        //                            LEFT JOIN nxin_qlw_business.fd_agingintervaldetail fad ON fad.RecordID=F.AgingIntervalID
        //                            LEFT JOIN qlw_nxin_com.bsdatadict d ON d.DictID=fad.IntervalType                             
        //                            where F.NumericalOrder = {key} ORDER BY fad.Serial";

        //    return _context.FA_MarketSubjectDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        //}       
    }
}
