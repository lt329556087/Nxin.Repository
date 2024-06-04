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
    public class FD_SpecificIdentificationODataProvider : OneWithManyQueryProvider<FD_SpecificIdentificationODataEntity, FD_SpecificIdentificationDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_SpecificIdentificationODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public Task<List<Biz_CustomerODataEntity>> GetCustomerList(ODataQueryOptions<Biz_CustomerODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@" 
                                    SELECT CONVERT(CustomerID USING utf8mb4) CustomerID,CustomerName,IsUse from nxin_qlw_business.sa_customer where EnterpriseID = {_identityservice.EnterpriseId} 
                                    union 
                                    SELECT CONVERT(SupplierID USING utf8mb4) CustomerID, SupplierName CustomerName,IsUse from nxin_qlw_business.pm_supplier where EnterpriseID = {_identityservice.EnterpriseId} ";//and isuse=1
            return _context.Biz_CustomerDataSet.FromSqlInterpolated(sql).ToListAsync();
        }


        public IEnumerable<FD_SpecificIdentificationODataEntity> GetList(ODataQueryOptions<FD_SpecificIdentificationODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_SpecificIdentificationODataEntity> GetData(ODataQueryOptions<FD_SpecificIdentificationODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FD_SpecificIdentificationODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT  
                                        CONVERT(UUID() USING utf8mb4) Guid,	
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(A.Number USING utf8mb4) Number,
                                        CONVERT(HP1.OwnerID USING utf8mb4) AS OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                        ent.EnterpriseName EnterpriseName,
                                        HP1.Name OwnerName,
                                        CONVERT(HP2.BO_ID USING utf8mb4) AS CheckedByID,
                                        HP2.Name  CheckedByName,
                                        -- CONVERT(B.CustomerID USING utf8mb4) CustomerID,
                                        IFNULL(B.Amount,0.0)  Amount,
                                        B.Remarks, 
                                        dc.DataDictName ProvisionTypeName,
                                        IFNULL(bc.CustomerName,hr.Name) CustomerName,
                                        CONVERT(B.ProvisionType USING utf8mb4) ProvisionType,	
                                        CONVERT(B.CustomerID USING utf8mb4) CustomerID,	
                                        false  IsProvision,
                                        CONVERT(B.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,

                                        CONVERT(A.AccoSubjectID1 USING utf8mb4) AccoSubjectID1,	
                                        '' AccoSubjectName1,
                                        CONVERT(A.AccoSubjectID2 USING utf8mb4) AccoSubjectID2,	
                                        '' AccoSubjectName2,
                                        CONVERT(A.BusinessType USING utf8mb4) BusinessType,
                                        s.AccoSubjectCode,
                                        s.AccoSubjectName
                                        FROM `nxin_qlw_business`.fd_SpecificIdentification A 
                                        LEFT JOIN nxin_qlw_business.fd_specificidentificationdetail B ON A.NumericalOrder=B.NumericalOrder
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=A.OwnerID
                                        LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16 
                                        LEFT JOIN nxin_qlw_business.hr_person HP2 ON HP2.bo_id = r2.CheckedByID 
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON A.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=B.CustomerID
                                        Left Join nxin_qlw_business.hr_person hr on b.CustomerID = hr.PersonID
                                        LEFT JOIN nxin_qlw_business.biz_datadict dc ON B.ProvisionType = dc.DataDictID
                                        Left Join qlw_nxin_com.biz_accosubject s on  s.AccoSubjectID = B.AccoSubjectID 
                                WHERE A.EnterpriseID= {_identityservice.EnterpriseId} ORDER BY A.DataDate DESC";
            return _context.FD_SpecificIdentificationDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_SpecificIdentificationODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = @$"SELECT  
                                        CONVERT(UUID() USING utf8mb4) Guid,	
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        '0'   NumericalOrderDetail,
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(A.Number USING utf8mb4) Number,
                                        CONVERT(HP1.OwnerID USING utf8mb4) AS OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                        ent.EnterpriseName EnterpriseName,
                                        HP1.Name OwnerName,
                                        CONVERT(HP2.BO_ID USING utf8mb4) AS CheckedByID,
                                        HP2.Name  CheckedByName,
                                        0.0 Amount,
                                        '' Remarks, 
                                        '' ProvisionTypeName,
                                        '' CustomerName,
                                        '0' ProvisionType,	
                                        '0' CustomerID,	
                                        '' AccoSubjectCode,
                                        '' AccoSubjectName,
                                        false  IsProvision,
                                        CONVERT(A.AccoSubjectID1 USING utf8mb4) AccoSubjectID1,	
                                        CONVERT(A.AccoSubjectID2 USING utf8mb4) AccoSubjectID2,
                                        s1.AccoSubjectFullName   AccoSubjectName1,
                                        s2.AccoSubjectFullName AccoSubjectName2,
                                        CONVERT(A.BusinessType USING utf8mb4) BusinessType,
                                        CONVERT(A.NumericalOrderSetting USING utf8mb4) NumericalOrderSetting
                                        FROM `nxin_qlw_business`.fd_SpecificIdentification A 
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=A.OwnerID
                                        LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16 
                                        LEFT JOIN nxin_qlw_business.hr_person HP2 ON HP2.bo_id = r2.CheckedByID 
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON A.EnterpriseID=ent.EnterpriseID
                                        Left join qlw_nxin_com.biz_accosubject S1 on A.AccoSubjectID1 = S1.AccoSubjectID
                                        Left join qlw_nxin_com.biz_accosubject S2 on A.AccoSubjectID2 = S2.AccoSubjectID

                                        WHERE A.NumericalOrder = {manyQuery}";

            return _context.FD_SpecificIdentificationDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        public override Task<List<FD_SpecificIdentificationDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"
                                    SELECT 
                                    CONVERT(FD.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(FD.ProvisionType USING utf8mb4) ProvisionType,
                                    CONVERT(FD.CustomerID USING utf8mb4) CustomerID,
                                    CONVERT(FD.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                    IFNULL(FD.Amount,0.0)  Amount,
                                    IFNULL(FD.AccoAmount,0.0)  AccoAmount,
                                    FD.Remarks, 
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    dc.DataDictName ProvisionTypeName,
                                    IFNULL(bc.CustomerName,hr.Name )   CustomerName,
                                    false  IsProvision
                                    FROM 
                                    nxin_qlw_business.fd_SpecificIdentification A 
                                    left join nxin_qlw_business.fd_specificidentificationdetail FD on A.NumericalOrder = Fd.NumericalOrder
                                    Left join nxin_qlw_business.hr_person hr on Fd.CustomerID = hr.PersonID
                                    LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CustomerID
                                    LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.ProvisionType = dc.DataDictID
                                    where FD.NumericalOrder = {manyQuery}";

            return _context.FD_SpecificIdentificationDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }



        public Task<List<FD_SpecificIdentificationExtODataEntity>> GetExtData(string id)
        {

            FormattableString sql = $@"
                                    SELECT RecordID, Name, IFNULL(FD.Amount,0.0)  Amount,  
                                    CONVERT(FD.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail  ,
                                    ''  FD_SpecificIdentificationDetailODataEntityNumericalOrderDetail
                                    from   `nxin_qlw_business`.fd_SpecificIdentificationExt  FD
                                     where FD.NumericalOrderDetail =  {id};";

            return _context.FD_SpecificIdentificationExtDataSet.FromSqlInterpolated(sql).ToListAsync();

        }


        #region 获取个别认定
        public FD_SpecificIdentificationODataEntity GetDataByEnterId(long enterpriseId, string date, string customerId)
        {
            if (enterpriseId < 1)
            {
                return null;
            }
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            FormattableString sql = @$"	SELECT  
                                    CONVERT(UUID() USING utf8mb4) Guid,	
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                    CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                                    CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                    CONVERT(A.Number USING utf8mb4) Number,
                                    CONVERT(HP1.OwnerID USING utf8mb4) AS OwnerID,
                                    CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                    CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    ent.EnterpriseName EnterpriseName,
                                    HP1.Name OwnerName,
                                    CONVERT(HP2.BO_ID USING utf8mb4) AS CheckedByID,
                                    HP2.Name  CheckedByName,
                                    -- CONVERT(B.CustomerID USING utf8mb4) CustomerID,
                                    IFNULL(B.Amount,0.0)  Amount,
                                    B.Remarks, 
                                    dc.DataDictName ProvisionTypeName,
                                    bc.CustomerName,
                                    CONVERT(B.ProvisionType USING utf8mb4) ProvisionType,	
                                    CONVERT(B.CustomerID USING utf8mb4) CustomerID,	
                                    false  IsProvision
                                    FROM `nxin_qlw_business`.fd_SpecificIdentification A 
                                    LEFT JOIN nxin_qlw_business.fd_specificidentificationdetail B ON A.NumericalOrder=B.NumericalOrder
                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=A.OwnerID
                                    LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON A.NumericalOrder=r2.NumericalOrder AND r2.CheckMark = 16 
                                    LEFT JOIN nxin_qlw_business.hr_person HP2 ON HP2.bo_id = r2.CheckedByID 
                                    LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON A.EnterpriseID=ent.EnterpriseID
                                    LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=B.CustomerID
                                    LEFT JOIN nxin_qlw_business.biz_datadict dc ON B.ProvisionType = dc.DataDictID
                                WHERE A.EnterpriseID={enterpriseId} 
                                        AND A.DataDate=(SELECT IFNULL(MAX(DataDate),'{date}') DataDate FROM nxin_qlw_business.fd_SpecificIdentification A
                                        WHERE EnterpriseID={enterpriseId} AND DataDate<='{date}') ORDER BY A.CreatedDate DESC";
            var dataList = _context.FD_SpecificIdentificationDataSet.FromSqlInterpolated(sql);
            if (dataList != null && dataList.Count() > 0)
            {
                var dataA = dataList.FirstOrDefault();
                var dataB = GetDetaiByCon(dataA.NumericalOrder, customerId);
                dataA.Lines = dataB;
                return dataA;
            }
            return null;
        }

        public List<FD_SpecificIdentificationRangeODataEntity> GetIdentityRange(string startDate, string endDate)
        {

            FormattableString sql = $@"SELECT 
                                    CONVERT(id.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,	
                                    CONVERT(i.EnterpriseId USING utf8mb4) EnterpriseID,	
                                    CONVERT(i.Number USING utf8mb4) Number,	
                                    i.DataDate ,
                                    CONVERT(i.BusinessType USING utf8mb4) BusinessType,	
                                    CONVERT(id.AccoSubjectID USING utf8mb4) AccoSubjectID,	
                                    CONVERT(id.CustomerID USING utf8mb4) CustomerID,	
                                    ifnull(cus.CustomerName ,hr.Name)  CustomerName,
                                     CONVERT(id.ProvisionType USING utf8mb4) ProvisionType,	
                                     d.DataDictName as   ProvisionTypeName,
                                     id.Amount 
                                    from nxin_qlw_business.fd_specificidentification i 
                                    inner join nxin_qlw_business.fd_specificidentificationdetail id on i.NumericalOrder = id.NumericalOrder
                                    inner join (
                                    SELECT 
                                    SUBSTRING_INDEX( GROUP_CONCAT(i2.NumericalOrderDetail ORDER BY i1.DataDate,i2.ModifiedDate desc),',','1')  as NumericalOrderDetail
                                    from  nxin_qlw_business.fd_specificidentification i1 
                                    left join nxin_qlw_business.fd_specificidentificationdetail i2 on i1.NumericalOrder = i2.NumericalOrder
                                    GROUP BY EnterpriseID,customerId) as ide on id.NumericalOrderDetail = ide.NumericalOrderDetail
                                    left join nxin_qlw_business.biz_datadict d on id.ProvisionType=d.DataDictID 
                                    left join qlw_nxin_com.biz_customer cus on id.CustomerID = cus.CustomerID
                                    left join qlw_nxin_com.hr_person hr on id.CustomerID = hr.PersonID  
                                    where i.EnterpriseId = {_identityservice.EnterpriseId} and i.DataDate between {startDate} and {endDate}";

            return _context.FD_SpecificIdentificationRangeDataSet.FromSqlInterpolated(sql).ToList();
        }


        public List<FD_SpecificIdentificationDetailODataEntity> GetDetaiByCon(string key, string customerId)
        {
            FormattableString sql = null;
            if (string.IsNullOrEmpty(customerId))
            {
                sql = $@"
                                    SELECT 
                                    FD.NumericalOrderDetail,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(FD.ProvisionType USING utf8mb4) ProvisionType,
                                    CONVERT(FD.CustomerID USING utf8mb4) CustomerID,
                                    IFNULL(FD.Amount,0.0)  Amount,
                                    FD.Remarks, 
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    dc.DataDictName ProvisionTypeName,
                                    bc.CustomerName,
                                    false  IsProvision
                                    FROM nxin_qlw_business.fd_specificidentificationdetail FD
                                    LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CustomerID
                                    LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.ProvisionType = dc.DataDictID
                                    where FD.NumericalOrder = {key}";
            }
            else
            {
                sql = $@"
                                    SELECT 
                                    FD.NumericalOrderDetail,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(FD.ProvisionType USING utf8mb4) ProvisionType,
                                    CONVERT(FD.CustomerID USING utf8mb4) CustomerID,
                                    IFNULL(FD.Amount,0.0)  Amount,
                                    FD.Remarks, 
                                    CONVERT(FD.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    dc.DataDictName ProvisionTypeName,
                                    bc.CustomerName,
                                    false  IsProvision
                                    FROM nxin_qlw_business.fd_specificidentificationdetail FD
                                    LEFT JOIN qlw_nxin_com.biz_customer  bc ON bc.CustomerID=FD.CustomerID
                                    LEFT JOIN nxin_qlw_business.biz_datadict dc ON FD.ProvisionType = dc.DataDictID
                                    where FD.NumericalOrder = {key} AND FD.CustomerID={customerId}";
            }


            return _context.FD_SpecificIdentificationDetailDataSet.FromSqlInterpolated(sql).ToList();
        }
        #endregion
    }
}
