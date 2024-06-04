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
using System.Text;

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class FA_UseStateTransferODataProvider : OneWithManyQueryProvider<FA_UseStateTransferODataEntity, FA_UseStateTransferDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FA_UseStateTransferODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FA_UseStateTransferODataEntity> GetList(ODataQueryOptions<FA_UseStateTransferODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FA_UseStateTransferODataEntity> GetData(ODataQueryOptions<FA_UseStateTransferODataEntity> odataqueryoptions, Uri uri)
        {
            //var sql = GetDataABasesql();
            //var whereSb = new StringBuilder();
            //var request = odataqueryoptions.Request;

            //if (!string.IsNullOrEmpty(request.Query["DataDate"]))
            //{
            //    whereSb.AppendFormat(@" AND f.`DataDate` >= '{0}'", request.Query["DataDate"]);
            //}
            //if (!string.IsNullOrEmpty(request.Query["CreatedDate"]))
            //{
            //    whereSb.AppendFormat(@" AND f.`DataDate` <= '{0}'", request.Query["CreatedDate"]);
            //}
            //if (!string.IsNullOrEmpty(request.Query["Number"]))
            //{
            //    whereSb.AppendFormat(@" AND A.Number like '%{0}%'", request.Query["Number"]);
            //}
            //if (!string.IsNullOrEmpty(request.Query["OwnerID"]))
            //{
            //    whereSb.AppendFormat(@" AND f.`OwnerID` ={0}", request.Query["OwnerID"]);
            //}
            //if (!string.IsNullOrEmpty(request.Query["AssetsName"]))
            //{
            //    whereSb.AppendFormat(@" AND fa.AssetsName like '%{0}%'", request.Query["AssetsName"]);
            //}
            //if (!string.IsNullOrEmpty(request.Query["AssetsCode"]))
            //{
            //    whereSb.AppendFormat(@" AND fa.`AssetsCode` like '%{0}%'", request.Query["AssetsCode"]);
            //}
            //sql += whereSb.ToString() + " ORDER BY A.NumericalOrder,A.DataDate DESC ";
            //return _context.FA_UseStateTransferDataSet.FromSqlRaw(sql);
            var data= GetDatas();
            if (data != null && data.Count() > 0)
            {
                data.ToList().ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.CheckedByName))
                    {
                        p.AuditStatus = "1";
                        p.AuditStatusName = "已审核";
                    }
                    else
                    {
                        p.AuditStatus = "2";
                        p.AuditStatusName = "未审核";
                    }
                });
            }
            return data;
        }

        public override IQueryable<FA_UseStateTransferODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(UUID() USING utf8mb4) Guid,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(A.Number USING utf8mb4) Number,
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName,
                                        B.`Remarks`,
                                        fas.`UseStateName` BeforeUseStateName,
                                        fas1.`UseStateName` AfterUseStateName,
                                        fa.`AssetsCode`,
                                        fa.AssetsName,
                                        HP1.Name CheckedByName,
                                        '' AuditStatus,
                                        '' AuditStatusName
                                        FROM nxin_qlw_business.FA_UseStateTransfer A
                                        INNER JOIN `nxin_qlw_business`.FA_UseStateTransferDetail B ON A.`NumericalOrder`=B.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.FA_AssetsUseState fas ON fas.`UseStateID`=B.`BeforeUseStateID`
					                    LEFT JOIN `nxin_qlw_business`.FA_AssetsUseState fas1 ON fas1.`UseStateID`=B.`AfterUseStateID`
					                    LEFT JOIN `nxin_qlw_business`.fa_assetscard fa ON fa.`CardID`=B.`CardID`
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                        LEFT JOIN nxin_qlw_business.BIZ_Reviwe BZ ON BZ.NumericalOrder=A.NumericalOrder   AND  BZ.CheckMark=16 -- AND  BZ.ReviweType=1
                                        LEFT JOIN nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID=BZ.CheckedByID
                                        WHERE A.EnterpriseID={_identityservice.EnterpriseId} ORDER BY A.NumericalOrder,A.DataDate DESC";
            return _context.FA_UseStateTransferDataSet.FromSqlInterpolated(sql);
        }
        private string GetDataABasesql()
        {
            return string.Format(@"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(A.Number USING utf8mb4) Number,
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName,
                                        B.`Remarks`,
                                        fas.`UseStateName` BeforeUseStateName,
                                        fas1.`UseStateName` AfterUseStateName,
                                        fa.`AssetsCode`,
                                        fa.AssetsName,
                                        HP1.Name CheckedByName
                                        FROM nxin_qlw_business.FA_UseStateTransfer A
                                        INNER JOIN `nxin_qlw_business`.FA_UseStateTransferDetail B ON A.`NumericalOrder`=B.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.FA_AssetsUseState fas ON fas.`UseStateID`=B.`BeforeUseStateID`
					                    LEFT JOIN `nxin_qlw_business`.FA_AssetsUseState fas1 ON fas1.`UseStateID`=B.`AfterUseStateID`
					                    LEFT JOIN `nxin_qlw_business`.fa_assetscard fa ON fa.`CardID`=B.`CardID`
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                        LEFT JOIN nxin_qlw_business.BIZ_Reviwe BZ ON BZ.NumericalOrder=A.NumericalOrder   AND  BZ.CheckMark=16 -- AND  BZ.ReviweType=1
                                        LEFT JOIN nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID=BZ.CheckedByID
                                        WHERE A.EnterpriseID={0} ", _identityservice.EnterpriseId);
        }
        
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FA_UseStateTransferODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = @$"SELECT CONVERT(UUID() USING utf8mb4) Guid,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(A.Number USING utf8mb4) Number,
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName,
                                        A.`Remarks`,
                                        '' BeforeUseStateName,
                                        '' AfterUseStateName,
                                        '' AssetsCode,
                                        '' AssetsName,
                                        HP1.Name CheckedByName,
                                        '' AuditStatus,
                                        '' AuditStatusName
                                        FROM nxin_qlw_business.FA_UseStateTransfer A                                       
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID 
                                        LEFT JOIN nxin_qlw_business.BIZ_Reviwe BZ ON BZ.NumericalOrder=A.NumericalOrder   AND  BZ.CheckMark=16 -- AND  BZ.ReviweType=1
                                        LEFT JOIN nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID=BZ.CheckedByID
                                    where A.NumericalOrder ={manyQuery}";

            return _context.FA_UseStateTransferDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        public override Task<List<FA_UseStateTransferDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            var sql = GetDetailSql(manyQuery);
            return _context.FA_UseStateTransferDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        private FormattableString GetDetailSql(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.`CardID` USING utf8mb4) CardID, 
                                    CONVERT(F.`BeforeUseStateID` USING utf8mb4) BeforeUseStateID,
                                    CONVERT(F.`AfterUseStateID` USING utf8mb4) `AfterUseStateID`,
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
                                    F.`Remarks`, 
                                    fas.`UseStateName` BeforeUseStateName,
                                    fas1.`UseStateName` AfterUseStateName,
                                    fa.CardCode,
                                    fa.`AssetsCode`,
                                    fa.AssetsName,
                                    fa.Specification,
                                    CONVERT(DATE_FORMAT( fa.`StartDate`,'%Y-%m-%d') USING utf8mb4) StartDate
                                    FROM nxin_qlw_business.FA_UseStateTransferDetail F 
                                    LEFT JOIN `nxin_qlw_business`.FA_AssetsUseState fas ON fas.`UseStateID`=F.`BeforeUseStateID`
				                    LEFT JOIN `nxin_qlw_business`.FA_AssetsUseState fas1 ON fas1.`UseStateID`=F.`AfterUseStateID`
				                    LEFT JOIN `nxin_qlw_business`.fa_assetscard fa ON fa.`CardID`=F.`CardID`                     
                                    where F.NumericalOrder = {manyQuery} ORDER BY f.RecordID";
            return sql;
        }
        public List<FA_UseStateTransferDetailODataEntity> GetDetaiData(long manyQuery)
        {
            var sql = GetDetailSql(manyQuery);
            return _context.FA_UseStateTransferDetailDataSet.FromSqlInterpolated(sql).ToList();
        }
          
    }
}
