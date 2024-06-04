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
    public class FD_BaddebtAccrualODataProvider : OneWithManyQueryProvider<FD_BaddebtAccrualODataEntity, FD_BaddebtAccrualDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BaddebtAccrualODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BaddebtAccrualODataEntity> GetList(ODataQueryOptions<FD_BaddebtAccrualODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_BaddebtAccrualODataEntity> GetData(ODataQueryOptions<FD_BaddebtAccrualODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FD_BaddebtAccrualODataEntity> GetDatas(NoneQuery query = null)
        {
            var sql = GetHeadSql();
            sql+=string.Format(@"
                                WHERE A.EnterpriseID= {0} 
                                GROUP BY A.NumericalOrder,B.BusiType
                                ORDER BY A.DataDate DESC,A.Number DESC  ", _identityservice.EnterpriseId);
            return _context.FD_BaddebtAccrualDataSet.FromSqlRaw(sql); 
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
                                        CONVERT(A.NumericalOrderSetting USING utf8mb4) AS NumericalOrderSetting,
                                        A.Remarks, 
                                        -- ent.EnterpriseName EnterpriseName,
                                        HP1.Name OwnerName,
                                        t.`TicketedPointName`,
                                        IF(r2.`RecordID` IS NULL,0,1) AS CheckState,   
                                        IF(r2.`RecordID` IS NULL,'未审核','已审核') AS CheckStateName,
                                        B.BusiType,   
                                        CASE B.BusiType WHEN 0 THEN '应收账款'  WHEN 1 THEN '其他应收款' END AS BusiTypeName, 
                                        SUM(IFNULL(B.Amount,0.0))  Amount,     
                                        SUM(IFNULL(B.AccrualAmount,0.0))  AccrualAmount      
                                        -- ,CONVERT(B.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail
                                        FROM `nxin_qlw_business`.FD_BaddebtAccrual A 
                                        LEFT JOIN nxin_qlw_business.FD_BaddebtAccrualdetail B ON A.NumericalOrder=B.NumericalOrder
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=A.OwnerID
                                        LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16 
                                        LEFT JOIN nxin_qlw_business.`biz_ticketedpoint` t ON A.`TicketedPointID`=t.`TicketedPointID`
                                        -- LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON A.EnterpriseID=ent.EnterpriseID ");
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_BaddebtAccrualODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
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
                                        CONVERT(A.NumericalOrderSetting USING utf8mb4) AS NumericalOrderSetting,
                                        A.Remarks, 
                                        HP1.Name OwnerName,
                                        t.`TicketedPointName`,
                                        IF(r2.`RecordID` IS NULL,0,1) AS CheckState,   
                                        IF(r2.`RecordID` IS NULL,'未审核','已审核') AS CheckStateName,
                                        0 BusiType,   
                                        '' BusiTypeName, 
                                        0.0  Amount,
                                        0.0 AccrualAmount
                                        FROM `nxin_qlw_business`.FD_BaddebtAccrual A
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=A.OwnerID 
                                        LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16
                                        LEFT JOIN nxin_qlw_business.`biz_ticketedpoint` t ON A.`TicketedPointID`=t.`TicketedPointID`
                                        WHERE A.NumericalOrder = {manyQuery}";
           
            return _context.FD_BaddebtAccrualDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        public override Task<List<FD_BaddebtAccrualDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            var sql = GetDetailSQL();
            sql+=string.Format(@"                                   
                                    where FD.NumericalOrder = {0}", manyQuery);

            return _context.FD_BaddebtAccrualDetailDataSet.FromSqlRaw(sql).ToListAsync();
        }

        private string GetDetailSQL()
        {
            return @"SELECT 
                                    CONVERT(FD.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    FD.`BusiType` BusiType,
                                    CONVERT(FD.`TypeID` USING utf8mb4) TypeID,
                                    CONVERT(FD.`BusinessType` USING utf8mb4) BusinessType,
                                    CONVERT(FD.`CurrentUnit` USING utf8mb4) CurrentUnit,
                                    IFNULL(FD.Amount,0.0)  Amount,
                                    IFNULL(FD.AccrualAmount,0.0)  AccrualAmount,
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    dc.DataDictName BusinessTypeName,
                                    IFNULL(bc.CustomerName,hr.Name )   CurrentUnitName,
                                    fi.`TypeName`
                                    FROM  nxin_qlw_business.FD_BaddebtAccrualdetail FD 
                                    LEFT JOIN nxin_qlw_business.hr_person hr ON Fd.CurrentUnit = hr.PersonID
                                    LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CurrentUnit
                                    LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.`BusinessType` = dc.DataDictID
                                    LEFT JOIN nxin_qlw_business.fd_identificationtype fi ON fi.`TypeID`=FD.`TypeID` ";
        }

        public Task<List<FD_BaddebtAccrualExtODataEntity>> GetExtData(string id)
        {

            FormattableString sql = $@"
                                    SELECT RecordID, NAME, IFNULL(FD.Amount,0.0)  Amount,  
                                    FD.`BusiType` BusiType,
                                    CONVERT(FD.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail ,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate
                                    FROM   `nxin_qlw_business`.FD_BaddebtAccrualExt  FD
                                     where FD.NumericalOrderDetail =  {id};";

            return _context.FD_BaddebtAccrualExtDataSet.FromSqlInterpolated(sql).ToListAsync();

        }

    }
}
