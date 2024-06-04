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
    public class FD_IndividualIdentificationODataProvider : OneWithManyQueryProvider<FD_IndividualIdentificationODataEntity, FD_IndividualIdentificationDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_IndividualIdentificationODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_IndividualIdentificationODataEntity> GetList(ODataQueryOptions<FD_IndividualIdentificationODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_IndividualIdentificationODataEntity> GetData(ODataQueryOptions<FD_IndividualIdentificationODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FD_IndividualIdentificationODataEntity> GetDatas(NoneQuery query = null)
        {
            var sql = GetHeadSql();
            sql+=string.Format(@"
                                WHERE A.EnterpriseID= {0} 
                                GROUP BY A.NumericalOrder,B.BusiType
                                ORDER BY A.DataDate DESC,A.Number DESC  ", _identityservice.EnterpriseId);
            return _context.FD_IndividualIdentificationDataSet.FromSqlRaw(sql); 
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
                                        FROM `nxin_qlw_business`.fd_IndividualIdentification A 
                                        LEFT JOIN nxin_qlw_business.fd_IndividualIdentificationdetail B ON A.NumericalOrder=B.NumericalOrder
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
        public override Task<FD_IndividualIdentificationODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
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
                                        t.`TicketedPointName`,
                                        IF(r2.`RecordID` IS NULL,0,1) AS CheckState,   
                                        IF(r2.`RecordID` IS NULL,'未审核','已审核') AS CheckStateName,
                                        0 BusiType,   
                                        '' BusiTypeName, 
                                        0.0  Amount,
                                        0.0 AccrualAmount
                                        FROM `nxin_qlw_business`.fd_IndividualIdentification A
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=A.OwnerID 
                                        LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16
                                        LEFT JOIN nxin_qlw_business.`biz_ticketedpoint` t ON A.`TicketedPointID`=t.`TicketedPointID`
                                        WHERE A.NumericalOrder = {manyQuery}";

            return _context.FD_IndividualIdentificationDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        public override Task<List<FD_IndividualIdentificationDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            var sql = GetDetailSQL();
            sql+=string.Format(@"                                   
                                    where FD.NumericalOrder = {0}", manyQuery);

            return _context.FD_IndividualIdentificationDetailDataSet.FromSqlRaw(sql).ToListAsync();
        }

        private string GetDetailSQL()
        {
            return @"SELECT 
                                    CONVERT(FD.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    FD.`BusiType` BusiType,
                                    CONVERT(FD.`IdentificationType` USING utf8mb4) IdentificationType,
                                    CONVERT(FD.`BusinessType` USING utf8mb4) BusinessType,
                                    CONVERT(FD.`CurrentUnit` USING utf8mb4) CurrentUnit,
                                    IFNULL(FD.Amount,0.0)  Amount,
                                    IFNULL(FD.AccrualAmount,0.0)  AccrualAmount,
                                    FD.DataSourceType,
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    dc.DataDictName BusinessTypeName,
                                    IFNULL(bc.CustomerName,hr.Name )   CurrentUnitName,
                                    fi.`TypeName` IdentificationTypeName
                                    FROM  nxin_qlw_business.fd_IndividualIdentificationdetail FD 
                                    LEFT JOIN nxin_qlw_business.hr_person hr ON Fd.CurrentUnit = hr.PersonID
                                    LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CurrentUnit
                                    LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.`BusinessType` = dc.DataDictID
                                    LEFT JOIN nxin_qlw_business.fd_identificationtype fi ON fi.`TypeID`=FD.`IdentificationType` ";
        }
        /// <summary>
        ///根据日期 单位获取距离查询日期最近的个别认定信息 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<List<FD_IndividualIdentificationDetailODataEntity>> GetDetaiByConAsync(AgingReclass model)
        {
            var sql = string.Format(@"SELECT 
                                    CONVERT(FD.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    FD.`BusiType` BusiType,
                                    CONVERT(FD.`IdentificationType` USING utf8mb4) IdentificationType,
                                    CONVERT(FD.`BusinessType` USING utf8mb4) BusinessType,
                                    CONVERT(FD.`CurrentUnit` USING utf8mb4) CurrentUnit,
                                    IFNULL(FD.Amount,0.0)  Amount,
                                    IFNULL(FD.AccrualAmount,0.0)  AccrualAmount,
                                    FD.DataSourceType,
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    dc.DataDictName BusinessTypeName,
                                    IFNULL(bc.CustomerName,hr.Name )   CurrentUnitName,
                                    fi.`TypeName` IdentificationTypeName
                                    FROM  nxin_qlw_business.fd_IndividualIdentificationdetail FD 
                                    INNER JOIN nxin_qlw_business.fd_IndividualIdentification F ON F.`NumericalOrder`=FD.`NumericalOrder`
                                    LEFT JOIN nxin_qlw_business.hr_person hr ON Fd.CurrentUnit = hr.PersonID
                                    LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CurrentUnit
                                    LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.`BusinessType` = dc.DataDictID
                                    LEFT JOIN nxin_qlw_business.fd_identificationtype fi ON fi.`TypeID`=FD.`IdentificationType`
                                    INNER JOIN(SELECT NumericalOrder FROM nxin_qlw_business.fd_IndividualIdentification F
                                     WHERE F.`EnterpriseID`={0} AND F.DataDate<='{1}' ORDER BY F.`DataDate` DESC,F.CreatedDate DESC LIMIT 1)  fii ON fii.NumericalOrder=F.NumericalOrder
                                    WHERE F.`EnterpriseID`={0} AND F.NumericalOrder=fii.NumericalOrder ", model.EnterpriseID,model.DataDate);

            return _context.FD_IndividualIdentificationDetailDataSet.FromSqlRaw(sql).ToListAsync();
        }
        public Task<List<FD_IndividualIdentificationExtODataEntity>> GetExtData(string id)
        {

            FormattableString sql = $@"
                                    SELECT RecordID, NAME, IFNULL(FD.Amount,0.0)  Amount,  
                                    FD.`BusiType` BusiType,
                                    CONVERT(FD.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail ,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate
                                    FROM   `nxin_qlw_business`.fd_IndividualIdentificationExt  FD
                                     where FD.NumericalOrderDetail =  {id};";

            return _context.FD_IndividualIdentificationExtDataSet.FromSqlInterpolated(sql).ToListAsync();

        }
        /// <summary>
        /// 获取账龄设置
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="intervalType"></param>
        /// <returns></returns>
        public IQueryable<AgingintervalDataODataEntity> GetAgingintervals(string groupId, string intervalType)
        {
            string sql =string.Format(@"SELECT RecordID, b.Name,b.Serial FROM nxin_qlw_business.fd_aginginterval a LEFT JOIN nxin_qlw_business.fd_agingintervaldetail b ON a.NumericalOrder = b.NumericalOrder WHERE a.EnterpriseID={0} AND b.IntervalType = {1}", groupId, intervalType);
            return _context.AgingintervalDataDataSet.FromSqlRaw(sql);
        }


        #region 获取个别认定
        //public FD_IndividualIdentificationODataEntity GetDataByEnterId(long enterpriseId, string date, string customerId)
        //{
        //    if (enterpriseId < 1)
        //    {
        //        return null;
        //    }
        //    if (string.IsNullOrEmpty(date))
        //    {
        //        return null;
        //    }

        //    FormattableString sql = @$"	SELECT  
        //                            CONVERT(UUID() USING utf8mb4) Guid,	
        //                            CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
        //                            CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
        //                            CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
        //                            CONVERT(A.Number USING utf8mb4) Number,
        //                            CONVERT(HP1.OwnerID USING utf8mb4) AS OwnerID,
        //                            CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
        //                            CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
        //                            ent.EnterpriseName EnterpriseName,
        //                            HP1.Name OwnerName,
        //                            CONVERT(HP2.BO_ID USING utf8mb4) AS CheckedByID,
        //                            HP2.Name  CheckedByName,
        //                            -- CONVERT(B.CustomerID USING utf8mb4) CustomerID,
        //                            IFNULL(B.Amount,0.0)  Amount,
        //                            B.Remarks, 
        //                            dc.DataDictName ProvisionTypeName,
        //                            bc.CustomerName,
        //                            CONVERT(B.ProvisionType USING utf8mb4) ProvisionType,	
        //                            CONVERT(B.CustomerID USING utf8mb4) CustomerID,	
        //                            false  IsProvision
        //                            FROM `nxin_qlw_business`.fd_IndividualIdentification A 
        //                            LEFT JOIN nxin_qlw_business.fd_IndividualIdentificationdetail B ON A.NumericalOrder=B.NumericalOrder
        //                            LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=A.OwnerID
        //                            LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16 
        //                            LEFT JOIN nxin_qlw_business.hr_person HP2 ON HP2.bo_id = r2.CheckedByID 
        //                            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON A.EnterpriseID=ent.EnterpriseID
        //                            LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=B.CustomerID
        //                            LEFT JOIN nxin_qlw_business.biz_datadict dc ON B.ProvisionType = dc.DataDictID
        //                        WHERE A.EnterpriseID={enterpriseId} 
        //                                AND A.DataDate=(SELECT IFNULL(MAX(DataDate),'{date}') DataDate FROM nxin_qlw_business.fd_IndividualIdentification A
        //                                WHERE EnterpriseID={enterpriseId} AND DataDate<='{date}') ORDER BY A.CreatedDate DESC";
        //    var dataList = _context.FD_IndividualIdentificationDataSet.FromSqlInterpolated(sql);
        //    if (dataList != null && dataList.Count() > 0)
        //    {
        //        var dataA = dataList.FirstOrDefault();
        //        var dataB = GetDetaiByCon(dataA.NumericalOrder, customerId);
        //        dataA.Lines = dataB;
        //        return dataA;
        //    }
        //    return null;
        //}

     
        //public List<FD_IndividualIdentificationDetailODataEntity> GetDetaiByCon(string key, string customerId)
        //{
        //    FormattableString sql = null;
        //    if (string.IsNullOrEmpty(customerId))
        //    {
        //        sql = $@"
        //                            SELECT 
        //                            FD.NumericalOrderDetail,
        //                            CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
        //                            CONVERT(FD.ProvisionType USING utf8mb4) ProvisionType,
        //                            CONVERT(FD.CustomerID USING utf8mb4) CustomerID,
        //                            IFNULL(FD.Amount,0.0)  Amount,
        //                            FD.Remarks, 
        //                            CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
        //                            dc.DataDictName ProvisionTypeName,
        //                            bc.CustomerName,
        //                            false  IsProvision
        //                            FROM nxin_qlw_business.fd_IndividualIdentificationdetail FD
        //                            LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CustomerID
        //                            LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.ProvisionType = dc.DataDictID
        //                            where FD.NumericalOrder = {key}";
        //    }
        //    else
        //    {
        //        sql = $@"
        //                            SELECT 
        //                            FD.NumericalOrderDetail,
        //                            CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
        //                            CONVERT(FD.ProvisionType USING utf8mb4) ProvisionType,
        //                            CONVERT(FD.CustomerID USING utf8mb4) CustomerID,
        //                            IFNULL(FD.Amount,0.0)  Amount,
        //                            FD.Remarks, 
        //                            CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
        //                            dc.DataDictName ProvisionTypeName,
        //                            bc.CustomerName,
        //                            false  IsProvision
        //                            FROM nxin_qlw_business.fd_IndividualIdentificationdetail FD
        //                            LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CustomerID
        //                            LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.ProvisionType = dc.DataDictID
        //                            where FD.NumericalOrder = {key} AND FD.CustomerID={customerId}";
        //    }


        //    return _context.FD_IndividualIdentificationDetailDataSet.FromSqlInterpolated(sql).ToList();
        //}
        #endregion
    }
}
