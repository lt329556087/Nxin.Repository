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
    public class FD_BaddebtRecoveryODataProvider : OneWithManyQueryProvider<FD_BaddebtRecoveryODataEntity, FD_BaddebtRecoveryDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BaddebtRecoveryODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BaddebtRecoveryODataEntity> GetList(ODataQueryOptions<FD_BaddebtRecoveryODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_BaddebtRecoveryODataEntity> GetData(ODataQueryOptions<FD_BaddebtRecoveryODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FD_BaddebtRecoveryODataEntity> GetDatas(NoneQuery query = null)
        {
            var sql = GetHeadSql();
            sql+=string.Format(@"
                                WHERE A.EnterpriseID= {0} 
                                ORDER BY A.DataDate DESC,A.Number DESC  ", _identityservice.EnterpriseId);
            return _context.FD_BaddebtRecoveryDataSet.FromSqlRaw(sql); 
        }
        private string GetHeadSql()
        {
            return string.Format(@"SELECT  
                                        CONVERT(UUID() USING utf8mb4) Guid,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(A.Number USING utf8mb4) Number,	
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(A.TicketedPointID USING utf8mb4) TicketedPointID,
                                        CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                        A.Remarks, 
                                        HP1.Name OwnerName,
                                        t.TicketedPointName,
                                        IF(r2.RecordID IS NULL,0,1) AS CheckState,   
                                        IF(r2.RecordID IS NULL,'未审核','已审核') AS CheckStateName,
                                        B.BusiType,   
                                        CASE B.BusiType WHEN 0 THEN '应收账款'  WHEN 1 THEN '其他应收款' END AS BusiTypeName, 
                                        B.RecordID,
                                        IFNULL(B.Amount,0.0)  Amount,     
                                        IFNULL(B.RecoveryAmount,0.0)  RecoveryAmount,
                                        CONVERT(B.BusinessType USING utf8mb4) BusinessType,
                    					CONVERT(B.CurrentUnit USING utf8mb4) CurrentUnit, 
                    					dc.DataDictName BusinessTypeName,
                    					IFNULL(bc.CustomerName,hr.Name) CurrentUnitName     
                                        FROM nxin_qlw_business.FD_BaddebtRecovery A 
                                        LEFT JOIN nxin_qlw_business.FD_BaddebtRecoverydetail B ON A.NumericalOrder=B.NumericalOrder
                                        LEFT JOIN nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID=A.OwnerID
                                        LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16 
                                        LEFT JOIN nxin_qlw_business.biz_ticketedpoint t ON A.TicketedPointID=t.TicketedPointID 
                                        LEFT JOIN nxin_qlw_business.hr_person hr ON B.CurrentUnit = hr.PersonID
                    					LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=B.CurrentUnit
                    					LEFT JOIN nxin_qlw_business.biz_datadict dc ON B.BusinessType = dc.DataDictID ");
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_BaddebtRecoveryODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = @$"SELECT  
                                        CONVERT(UUID() USING utf8mb4) Guid,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(A.Number USING utf8mb4) Number,	
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(A.TicketedPointID USING utf8mb4) TicketedPointID,
                                        CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                        A.Remarks, 
                                        HP1.Name OwnerName,
                                        t.TicketedPointName,
                                        IF(r2.RecordID IS NULL,0,1) AS CheckState,   
                                        IF(r2.RecordID IS NULL,'未审核','已审核') AS CheckStateName,
                                        0 BusiType,   
                                        '' BusiTypeName, 
                                        0 RecordID,
                                        0.0  Amount,
                                        0.0 RecoveryAmount,
                                        '' BusinessType,
                    					'' CurrentUnit, 
                    					'' BusinessTypeName,
                    					'' CurrentUnitName  
                                        FROM nxin_qlw_business.FD_BaddebtRecovery A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID=A.OwnerID 
                                        LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16
                                        LEFT JOIN nxin_qlw_business.biz_ticketedpoint t ON A.TicketedPointID=t.TicketedPointID
                                        WHERE A.NumericalOrder = {manyQuery}";
           
            return _context.FD_BaddebtRecoveryDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        public override Task<List<FD_BaddebtRecoveryDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            var sql = GetDetailSQL();
            sql+=string.Format(@"                                   
                                    where FD.NumericalOrder = {0}", manyQuery);

            return _context.FD_BaddebtRecoveryDetailDataSet.FromSqlRaw(sql).ToListAsync();
        }

        private string GetDetailSQL()
        {
            return @"SELECT 
                                    FD.RecordID,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    FD.BusiType BusiType,
                                    CONVERT(FD.BusinessType USING utf8mb4) BusinessType,
                                    CONVERT(FD.CurrentUnit USING utf8mb4) CurrentUnit,
                                    IFNULL(FD.Amount,0.0)  Amount,
                                    IFNULL(FD.RecoveryAmount,0.0)  RecoveryAmount,
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    dc.DataDictName BusinessTypeName,
                                    IFNULL(bc.CustomerName,hr.Name )   CurrentUnitName
                                    FROM  nxin_qlw_business.FD_BaddebtRecoverydetail FD 
                                    LEFT JOIN nxin_qlw_business.hr_person hr ON Fd.CurrentUnit = hr.PersonID
                                    LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CurrentUnit
                                    LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.BusinessType = dc.DataDictID ";
        }
    }
}
