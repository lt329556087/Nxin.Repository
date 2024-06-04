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
    public class FD_BaddebtGroupSettingODataProvider : OneWithManyQueryProvider<FD_BaddebtGroupSettingODataEntity, FD_BaddebtGroupSettingDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BaddebtGroupSettingODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BaddebtGroupSettingODataEntity> GetList(ODataQueryOptions<FD_BaddebtGroupSettingODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_BaddebtGroupSettingODataEntity> GetData(ODataQueryOptions<FD_BaddebtGroupSettingODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FD_BaddebtGroupSettingODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        -- CONVERT(A.Number USING utf8mb4) Number, 
                                        CONVERT(DATE_FORMAT(A.StartDate,'%Y-%m') USING utf8mb4) StartDate, 
                                        CASE A.EndDate WHEN NULL THEN '' ELSE CONVERT(DATE_FORMAT(A.EndDate,'%Y-%m') USING utf8mb4) END AS EndDate, 
                                        CONVERT(A.Remarks USING utf8mb4) Remarks,  
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,                                        
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName,
                                        NULL Extends,
                                        '' EnterpriseIDs,
                                        '' EnterpriseNames
                                        -- 'U' DataStatus
                                        FROM nxin_qlw_business.FD_BaddebtGroupSetting A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                        WHERE A.EnterpriseID={_identityservice.GroupId} ORDER BY A.DataDate DESC";
            return _context.FD_BaddebtGroupSettingDataSet.FromSqlInterpolated(sql);
        }
        
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_BaddebtGroupSettingODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            if (manyQuery < 1)
            {
                long.TryParse(_identityservice.GroupId, out manyQuery);
            }
            FormattableString sql = @$"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        -- CONVERT(A.Number USING utf8mb4) Number, 
                                        CONVERT(DATE_FORMAT(A.StartDate,'%Y-%m') USING utf8mb4) StartDate, 
                                        CASE A.EndDate WHEN NULL THEN '' ELSE CONVERT(DATE_FORMAT(A.EndDate,'%Y-%m') USING utf8mb4) END AS EndDate, 
                                        A.Remarks Remarks,  
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,                                        
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName,
                                        NULL Extends,
                                        '' EnterpriseIDs,
                                        '' EnterpriseNames
                                        -- 'U' DataStatus
                                        FROM nxin_qlw_business.FD_BaddebtGroupSetting A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                    where A.NumericalOrder ={manyQuery}";

            return _context.FD_BaddebtGroupSettingDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
       
        public override Task<List<FD_BaddebtGroupSettingDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            if (manyQuery < 1)
            {
                long.TryParse(_identityservice.GroupId, out manyQuery);
            }
            FormattableString sql = $@"SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.BusType ,SIGNED) BusType, 
                                    CONVERT(F.IntervalType USING utf8mb4) IntervalType, 
                                    F.Name,
                                    IFNULL(F.DayNum,0) DayNum,
                                    F.Serial,
                                    IFNULL(F.ProvisionRatio,0) ProvisionRatio,
                                    CONVERT( F.CreatedDate USING utf8mb4) CreatedDate,
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,                                   
                                    d.cDictName IntervalTypeName,                                  
                                    'U' RowStatus
                                    FROM nxin_qlw_business.FD_BaddebtGroupSettingDetail F 
                                    LEFT JOIN qlw_nxin_com.bsdatadict d ON d.DictID=F.IntervalType                             
                                    where F.NumericalOrder = {manyQuery} ORDER BY F.Serial";

            return _context.FD_BaddebtGroupSettingDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        public Task<List<FD_BaddebtGroupSettingExtendODataEntity>> GetExtendDatasAsync(long manyQuery)
        {
            FormattableString sql = $@" SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.EnterpriseID USING utf8mb4) EnterpriseID,                                    
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
                                    en.EnterpriseName AS EnterpriseName,
                                    ShowID
                                    FROM nxin_qlw_business.FD_BaddebtGroupSettingExtend F 
                                    LEFT JOIN qlw_nxin_com.biz_enterprise en ON F.EnterpriseID = en.EnterpriseID                                
                                    where F.NumericalOrder = {manyQuery}";

            return _context.FD_BaddebtGroupSettingExtendDataSet.FromSqlInterpolated(sql).ToListAsync();

        }
        /// <summary>
        /// 根据流水号获取计提类型
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public Task<List<FD_IdentificationTypeODataEntity>> GetIdentificationTypeAsync(long manyQuery)
        {
            var sql = GetIdentificationTypeBaseSql();
            sql += string.Format(@"                              
                                    where F.NumericalOrder = {0}", manyQuery);
            return _context.FD_IdentificationTypeDataSet.FromSqlRaw(sql).ToListAsync();
        }
        /// <summary>
        /// 根据流水号获取计提类型科目
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public Task<List<FD_IdentificationTypeSubjectODataEntity>> GetIdentificationTypeSubjectAsync(long manyQuery)
        {
            var sql = GetIdentificationTypeSubjectBaseSql();
            sql += string.Format(@"                              
                                    where F.NumericalOrder = {0}", manyQuery);
            return _context.FD_IdentificationTypeSubjectDataSet.FromSqlRaw(sql).ToListAsync();
        }

        /// <summary>
        /// 根据流水号获取计提类型和科目
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public Task<List<FD_IdentificationTypeAndSubjectODataEntity>> GetIdentificationTypeAndSubjectAsync(long manyQuery)
        {
            var sql = string.Format(@" SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID,
                                    CONVERT(F.TypeID USING utf8mb4) TypeID, 
                                    CONVERT(F.TypeID USING utf8mb4) OldTypeID, 
                                    CONVERT(F.AccoSubjectID USING utf8mb4) AccoSubjectID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder,                               
                                    fi.BusiType,
                                    fi.AccrualType,
                                    F.IsUse,
                                    F.DataSourceType,
                                    CASE F.DataSourceType WHEN 0 THEN '账龄重分类表' ELSE '' END DataSourceTypeName,
                                    fi.TypeName,
                                    a.AccoSubjectFullName
                                    FROM nxin_qlw_business.fd_identificationtypesubject F 
                                    LEFT JOIN  nxin_qlw_business.fd_identificationtype fi ON fi.TypeID=F.TypeID
                                    LEFT JOIN qlw_nxin_com.biz_accosubject a ON a.AccoSubjectID=F.AccoSubjectID
                                    where F.NumericalOrder = {0}", manyQuery);
            return _context.FD_IdentificationTypeAndSubjectDataSet.FromSqlRaw(sql).ToListAsync();
        }

        //public List<FD_BaddebtGroupSettingExtendODataEntity> GetExtendDatas(long manyQuery)
        //{
        //    FormattableString sql = GetExtendSql(manyQuery);
        //    return _context.FD_BaddebtGroupSettingExtendDataSet.FromSqlInterpolated(sql)?.ToList();

        //}
        //public FormattableString GetExtendSql(long manyQuery)
        //{
        //    return  $@" SELECT 
        //                            CONVERT(F.RecordID ,SIGNED) RecordID, 
        //                            CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
        //                            CONVERT(F.EnterpriseID USING utf8mb4) EnterpriseID,                                    
        //                            CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
        //                            en.EnterpriseName AS EnterpriseName,
        //                            ShowID
        //                            FROM nxin_qlw_business.FD_BaddebtGroupSettingExtend F 
        //                            LEFT JOIN qlw_nxin_com.biz_enterprise en ON F.EnterpriseID = en.EnterpriseID                                
        //                            where F.NumericalOrder = {manyQuery}";

        //}
        public Task<List<FD_BaddebtGroupSettingExtendODataEntity>> GetExtendsByDate(string startDate, string endDate)
        {
            var sql = string.Format(@" SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.EnterpriseID USING utf8mb4) EnterpriseID,                                    
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
                                    en.EnterpriseName AS EnterpriseName,
                                    ShowID
                                    FROM nxin_qlw_business.FD_BaddebtGroupSettingExtend F 
                                    LEFT JOIN nxin_qlw_business.FD_BaddebtGroupSetting A ON A.NumericalOrder=F.NumericalOrder
                                    LEFT JOIN qlw_nxin_com.biz_enterprise en ON F.EnterpriseID = en.EnterpriseID                                
                                    where  A.EnterpriseID={0} ", _identityservice.GroupId);
            var where = string.Format(" ");
            if (!string.IsNullOrEmpty(startDate))
            {
                if (!string.IsNullOrEmpty(endDate))
                {
                    where += string.Format(" AND (('{0}' BETWEEN A.StartDate AND A.EndDate) OR ('{1}' BETWEEN A.StartDate AND A.EndDate)  OR ('{0}'<= A.StartDate AND {1}>=A.EndDate))", startDate, endDate);
                }
                else
                {
                    where += string.Format(" AND ( A.EndDate is null OR ('{0}' BETWEEN A.StartDate AND A.EndDate)) ", startDate);
                }
            }
            sql += where;
            return _context.FD_BaddebtGroupSettingExtendDataSet.FromSqlRaw(sql).ToListAsync();

        }

        /// <summary>
        /// 根据日期、单位获取集团坏账区间设置
        /// </summary>
        /// <param name="date"></param>
        /// <param name="enterpriseID"></param>
        /// <returns></returns>      
        public Task<List<FD_BaddebtGroupSettingODataEntity>> GetGroupSetting(string date, string enterpriseID)
        {
           
            var sql = GetHeadSql();
            sql += string.Format(" WHERE  B.EnterpriseID={0} ", enterpriseID);
            if (!string.IsNullOrEmpty(date))
            {
                sql += string.Format(" AND ( A.EndDate is null OR ('{0}' BETWEEN A.StartDate AND A.EndDate))", date);
            }
            
            sql += " ORDER BY A.DataDate DESC,A.CreatedDate DESC ";
            return _context.FD_BaddebtGroupSettingDataSet.FromSqlRaw(sql)?.ToListAsync();
        }
        private string GetHeadSql()
        {
            return @"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        -- CONVERT(A.Number USING utf8mb4) Number, 
                                        CONVERT(DATE_FORMAT(A.StartDate,'%Y-%m') USING utf8mb4) StartDate, 
                                        CASE A.EndDate WHEN NULL THEN '' ELSE CONVERT(DATE_FORMAT(A.EndDate,'%Y-%m') USING utf8mb4) END AS EndDate, 
                                        A.Remarks Remarks,  
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,                                        
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName,
                                        NULL Extends,
                                        '' EnterpriseIDs,
                                        '' EnterpriseNames
                                        -- 'U' DataStatus
                                        FROM nxin_qlw_business.FD_BaddebtGroupSetting A
                                        INNER JOIN nxin_qlw_business.FD_BaddebtGroupSettingExtend B  ON A.NumericalOrder=B.NumericalOrder
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID";
        }
        /// <summary>
        /// 获取最新的区间设置
        /// </summary>
        /// <param name="date"></param>
        /// <param name="enterpriseID"></param>
        ///  <param name="BusType"></param>
        /// <returns></returns>
        public Task<List<FD_BaddebtGroupSettingDetailODataEntity>> GetGroupSettingDetailByDate(string date, string enterpriseID,int BusType)
        {
            var sql = string.Format(@"         SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.BusType ,SIGNED) BusType, 
                                    CONVERT(F.IntervalType USING utf8mb4) IntervalType, 
                                    F.Name,
                                    IFNULL(F.DayNum,0) DayNum,
                                    F.Serial,
                                    IFNULL(F.ProvisionRatio,0) ProvisionRatio,
                                    CONVERT( F.CreatedDate USING utf8mb4) CreatedDate,
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,                                   
                                    d.cDictName IntervalTypeName,                                  
                                    'U' RowStatus  
                                    FROM nxin_qlw_business.FD_BaddebtGroupSettingDetail F                                    
                                    LEFT JOIN qlw_nxin_com.bsdatadict d ON d.DictID=F.IntervalType   
                                    INNER JOIN (SELECT B.NumericalOrder FROM nxin_qlw_business.FD_BaddebtGroupSetting B 
                                    INNER JOIN nxin_qlw_business.FD_BaddebtGroupSettingExtend A  ON A.NumericalOrder=B.NumericalOrder 
                                    WHERE  A.EnterpriseID={0}  AND ( B.EndDate IS NULL OR ('{1}' BETWEEN B.StartDate AND B.EndDate))
                                    ORDER BY B.DataDate DESC,B.NumericalOrder DESC  LIMIT 1) B ON F.NumericalOrder=B.NumericalOrder 
                                    WHERE  F.BusType={2} order by Serial asc", enterpriseID,date,BusType);

            return _context.FD_BaddebtGroupSettingDetailDataSet.FromSqlRaw(sql)?.ToListAsync();

        }
        /// <summary>
        /// 获取账龄区间设置
        /// </summary>
        /// <param name="enterpriseId">集团id</param>
        /// <returns></returns>
        public List<FD_AgingDetaiODataEntity> GetAgingDetaiDatasAsync(long enterpriseId)
        {
            var groupid = _identityservice.GroupId;
            if (enterpriseId > 0)
            {
                groupid = enterpriseId.ToString();
            }
            FormattableString sql = $@"SELECT 
                                    CONVERT(fad.RecordID ,SIGNED) RecordID,
                                    CONVERT(fad.IntervalType USING utf8mb4) IntervalType, 
                                    d.cDictName IntervalTypeName,
                                    fad.Name,
                                    IFNULL(fad.DayNum,0) DayNum,
                                    fad.Serial
                                    FROM nxin_qlw_business.fd_aginginterval F 
                                    INNER JOIN nxin_qlw_business.fd_agingintervaldetail fad ON fad.NumericalOrder=F.NumericalOrder
                                    LEFT JOIN qlw_nxin_com.bsdatadict d ON d.DictID=fad.IntervalType                             
                                    where F.EnterpriseID = {groupid} ORDER BY fad.Serial";

            return _context.FD_AgingDetaiODataEntity.FromSqlInterpolated(sql).ToList();
        }

        public Task<List<FD_IdentificationTypeODataEntity>> GetIdentificationTypeList(IdentificationTypeSearchModel model)
        {
            var sql = GetIdentificationTypeSql();

            sql += string.Format(@"
                                        WHERE A.EnterpriseID = {0} AND A.AccrualType={1} AND c.EnterpriseID={2}", model.EnterpriseID, model.AccrualType, _identityservice.EnterpriseId);
            if (!string.IsNullOrEmpty(model.DataDate))
            {
                sql += string.Format(@"  AND (('{0}' BETWEEN B.StartDate AND B.EndDate) OR (B.EndDate IS NULL AND '{0}'>=B.StartDate)) ", model.DataDate);
            }
            return _context.FD_IdentificationTypeDataSet.FromSqlRaw(sql).ToListAsync();
        }
        private string GetIdentificationTypeSql()
        {
            return @"SELECT  
                                        CONVERT(A.TypeID USING utf8mb4) TypeID,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,  
                                        CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                        A.TypeName,
                                        A.BusiType,
                                        A.AccrualType,
                                        -- A.DataSourceType,
                                        A.Remarks
                                        FROM nxin_qlw_business.fd_identificationtype A
                                        LEFT JOIN nxin_qlw_business.fd_baddebtgroupsetting B ON A.NumericalOrder=B.NumericalOrder
                                        LEFT JOIN nxin_qlw_business.fd_baddebtgroupsettingextend C ON C.NumericalOrder=B.NumericalOrder";
        }
        private string GetIdentificationTypeBaseSql()
        {
            return @"SELECT  
                                        CONVERT(A.TypeID USING utf8mb4) TypeID,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,  
                                        CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                        A.TypeName,
                                        A.BusiType,
                                        A.AccrualType,
                                        A.Remarks
                                        FROM nxin_qlw_business.fd_identificationtype A ";
        }
        public Task<List<FD_IdentificationTypeSubjectODataEntity>> GetIdentificationTypeSubjectList(FD_BaddebtGroupSettingODataEntity model,int? AccrualType,int? BusiType,string EnteID)
        {
            var sql = string.Format(@"SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID,
                                    CONVERT(F.TypeID USING utf8mb4) TypeID, 
                                    CONVERT(F.AccoSubjectID USING utf8mb4) AccoSubjectID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.EnterpriseID USING utf8mb4) EnterpriseID, 
                                    CONVERT(F.OwnerID USING utf8mb4) OwnerID, 
                                    CONVERT(F.CreatedDate USING utf8mb4) AS CreatedDate,
                                    CONVERT(F.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    F.IsUse,
                                    F.DataSourceType,
                                    fi.TypeName,
                                    a.AccoSubjectCode,
                                    a.Rank,
                                    a.IsPerson
                                    FROM nxin_qlw_business.fd_identificationtypesubject F 
                                    INNER JOIN nxin_qlw_business.fd_baddebtgroupsetting B ON F.NumericalOrder=B.NumericalOrder    
                                    LEFT JOIN  nxin_qlw_business.fd_identificationtype fi ON fi.TypeID=F.TypeID
                                    LEFT JOIN qlw_nxin_com.biz_accosubject a ON a.AccoSubjectID=F.AccoSubjectID
                                    LEFT JOIN nxin_qlw_business.fd_baddebtgroupsettingextend C ON C.NumericalOrder=B.NumericalOrder 
                                    WHERE F.EnterpriseID = {0} AND C.EnterpriseID={1}", model.EnterpriseID,EnteID);
            if (!string.IsNullOrEmpty(model.DataDate))
            {
                sql += string.Format(@"  AND (('{0}' BETWEEN B.StartDate AND B.EndDate) OR (B.EndDate IS NULL AND '{0}'>=B.StartDate)) ", model.DataDate);
            }
            if (model.IsFiltersubject)
            {
                sql += string.Format(@"  AND F.AccoSubjectID<>0 AND F.AccoSubjectID is not null ", model.DataDate);
            }
            if (AccrualType != null)
            {
                sql += string.Format(@"  AND fi.AccrualType={0} ",AccrualType);
            }
            if (BusiType != null)
            {
                sql += string.Format(@"  AND fi.BusiType={0} ", BusiType);
            }
            return _context.FD_IdentificationTypeSubjectDataSet.FromSqlRaw(sql).ToListAsync();
        }
        private string GetIdentificationTypeSubjectBaseSql()
        {
            return @"SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID,
                                    CONVERT(F.TypeID USING utf8mb4) TypeID, 
                                    CONVERT(F.AccoSubjectID USING utf8mb4) AccoSubjectID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.EnterpriseID USING utf8mb4) EnterpriseID, 
                                    CONVERT(F.OwnerID USING utf8mb4) OwnerID, 
                                    CONVERT(F.CreatedDate USING utf8mb4) AS CreatedDate,
                                    CONVERT(F.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                    F.IsUse,
                                    F.DataSourceType,
                                    fi.TypeName,
                                    a.AccoSubjectCode,
                                    a.Rank,
                                    a.IsPerson
                                    FROM nxin_qlw_business.fd_identificationtypesubject F 
                                    LEFT JOIN  nxin_qlw_business.fd_identificationtype fi ON fi.TypeID=F.TypeID
                                    LEFT JOIN qlw_nxin_com.biz_accosubject a ON a.AccoSubjectID=F.AccoSubjectID";
        }
        /// <summary>
        /// 根据集团、业务类型获取计提类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<List<FD_IdentificationTypeODataEntity>> GetIdenTypeList(IdentificationTypeSearchModel model)
        {
            var sql = GetIdentificationTypeSql();
            sql += string.Format(@"
                                        WHERE A.EnterpriseID = {0} AND A.BusiType={1} 
                                        GROUP BY A.EnterpriseID,A.TypeName ", model.EnterpriseID, model.BusiType);//集团
           
            return _context.FD_IdentificationTypeDataSet.FromSqlRaw(sql).ToListAsync();
        }
        public Task<List<DropSubject>> GetSubjectList(IdentificationTypeSearchModel model)
        {
            var sql = string.Format(@" SELECT CONVERT(a.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                        a.AccoSubjectName cAccoSubjectName,
                                        a.AccoSubjectFullName cAccoSubjectFullName,
                                        a.AccoSubjectCode cAccoSubjectCode,
                                        a.IsLorR bLorR
                                        FROM qlw_nxin_com.biz_accosubject a
                                        INNER JOIN qlw_nxin_com.biz_versionsetting v ON a.VersionID=v.VersionID
                                        WHERE a.EnterpriseID={0} AND a.Rank<=2 AND '{1}' BETWEEN v.dBegin AND v.dEnd ",model.EnterpriseID,model.DataDate);
            return _context.FD_SubjectDataSet.FromSqlRaw(sql).ToListAsync();
        }
    }
}
