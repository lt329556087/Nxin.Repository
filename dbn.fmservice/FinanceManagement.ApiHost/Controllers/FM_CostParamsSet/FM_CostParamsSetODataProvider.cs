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

namespace FinanceManagement.ApiHost.Controllers.FM_CostParamsSet
{
    public class FM_CostParamsSetODataProvider : QueryProviderAbstraction<FM_CostParamsSetEntity>
    {
        private IIdentityService _identityService;
        private QlwCrossDbContext _context;

        public FM_CostParamsSetODataProvider(IIdentityService identityService, QlwCrossDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }

        /// <summary>
        /// OData 专用
        /// </summary>
        /// <returns></returns>
        public override IQueryable<FM_CostParamsSetEntity> GetDatas()
        {
            FormattableString sql = @$"SELECT 
              CONCAT(NumericalOrder,'') AS NumericalOrder,
              CONCAT(EnterpriseID,'') AS EnterpriseID,
              CONCAT(GenerationMode,'') AS GenerationMode,
              TotalDepreciationMonths,
              CONCAT(ResidualValueCalMethod,'') AS ResidualValueCalMethod,
              ResidualValueRate,
              ResidualValue,
              Remarks,
              CONCAT(OwnerID,'') AS OwnerID,
              STR_TO_DATE(CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
              STR_TO_DATE(ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate 
            FROM
              nxin_qlw_business.fm_costparamsset 
            WHERE EnterpriseID = {_identityService.EnterpriseId}";
            return _context.FM_CostParamsSet.FromSqlInterpolated(sql);
        }

        public FM_CostParamsSetEntity GetSingleData()
        {
            var query = GetDatas();
            FM_CostParamsSetEntity main = query.FirstOrDefault();
            if (main == null)
            {
                return CreateSingleData();
            }

            #region 获取明细-有记录
            FormattableString sqldetail = $@"SELECT 
              a.RecordID,
              CONCAT(a.NumericalOrder,'') AS NumericalOrder,
              CONCAT(a.PigFarmID,'') AS PigFarmID,
              b.`PigFarmName`,
              a.BeginPeriod AS BeginPeriod,
              STR_TO_DATE(a.BeginDate,'%Y-%m-%d') AS BeginDate,
              a.EnablePeriod AS EnablePeriod,
              STR_TO_DATE(a.EnableDate,'%Y-%m-%d') AS EnableDate,
              STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
              STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate
            FROM
              nxin_qlw_business.fm_costparamssetdetail a
              LEFT JOIN nxin_qlw_zlw_group.biz_pigfarm b ON a.`PigFarmID`=b.`PigFarmID`
              WHERE a.NumericalOrder={main.NumericalOrder}";
            main.Details = _context.FM_CostParamsSetDetailSet.FromSqlInterpolated(sqldetail).ToList();

            FormattableString sqlextend = $@"SELECT 
                a.RecordID,
                CONCAT(a.NumericalOrder,'') AS NumericalOrder,
                CONCAT(a.ExtendTypeID,'') AS ExtendTypeID,
                b.cDictName AS ExtendTypeName,
                CONCAT(a.SourceTypeID,'') AS SourceTypeID,
                c.cDictName AS SourceTypeName,
                CONCAT(a.CreatedTypeID,'') AS CreatedTypeID,
                d.cDictName AS CreatedTypeName,
                a.TotalDepreciationMonths,
                STR_TO_DATE(IFNULL(a.CreatedDate,NOW()),'%Y-%m-%d %H:%i:%s') AS CreatedDate,
                STR_TO_DATE(IFNULL(a.ModifiedDate,NOW()),'%Y-%m-%d %H:%i:%s') AS ModifiedDate
            FROM
                nxin_qlw_business.fm_costparamssetextend a
                LEFT JOIN qlw_nxin_com.`bsdatadict` b ON a.ExtendTypeID=b.DictID
                LEFT JOIN qlw_nxin_com.`bsdatadict` c ON a.SourceTypeID=c.DictID
                LEFT JOIN qlw_nxin_com.`bsdatadict` d ON a.CreatedTypeID=d.DictID
            WHERE a.NumericalOrder={main.NumericalOrder}";
            var extendList = _context.FM_CostParamsSetExtendSet.FromSqlInterpolated(sqlextend).ToList();
            //生成方式
            main.Extends = extendList.Where(_ => _.ExtendTypeId == "202201051355001201").ToList();

            //总折旧月数
            main.DepreciationExtends = extendList.Where(_ => _.ExtendTypeId == "202201051355001301").ToList();
            #endregion

            return main;
        }

        #region private method
        private FM_CostParamsSetEntity CreateSingleData()
        {
            FM_CostParamsSetEntity main = new FM_CostParamsSetEntity() { EnterpriseId = _identityService.EnterpriseId, NumericalOrder = "0",GenerationMode= "202201051355001202", TotalDepreciationMonths=36, ResidualValueCalMethod = "202201051355001402", ResidualValueRate=30 };

            #region 获取明细-无记录
            FormattableString sqldetail = $@"SELECT 
              CONVERT(FLOOR(1 + (RAND() * 1000000)), UNSIGNED INTEGER) AS RecordId,
              '0' AS NumericalOrder,
              CONCAT(a.PigFarmID,'') AS PigFarmID,
              a.PigFarmName,
              a.EnablePeriod AS BeginPeriod,
              STR_TO_DATE(a.Begindate,'%Y-%m-%d') AS BeginDate,
              a.EnablePeriod AS EnablePeriod,
              STR_TO_DATE(CONCAT(a.EnablePeriod,'-01'),'%Y-%m-%d') AS EnableDate,
              STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
              STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate
            FROM
              nxin_qlw_zlw_group.biz_pigfarm a
              WHERE a.`EnterpriseID`= {_identityService.EnterpriseId} AND a.IsUse=1";
            main.Details = _context.FM_CostParamsSetDetailSet.FromSqlInterpolated(sqldetail).ToList();

            FormattableString sqlextend = $@"SELECT 
                        CONVERT(FLOOR(1 + (RAND() * 1000000)), UNSIGNED INTEGER) AS RecordId,
                        '' AS NumericalOrder,
                        CONCAT(a.DictID,'') AS ExtendTypeID,
a.cDictName AS ExtendTypeName,
                        CONCAT(c.DictID,'') AS SourceTypeID,
                        c.cDictName AS SourceTypeName,
                        CONCAT(CASE WHEN a.DictID=202201051355001201 THEN 202201051355001202 ELSE 202201051355001302 END,'') AS CreatedTypeID,
CASE WHEN a.DictID=202201051355001201 THEN '自动生成' ELSE '取剩余折旧月数' END AS CreatedTypeName,
                        36 AS TotalDepreciationMonths,
                        STR_TO_DATE(NOW(),'%Y-%m-%d %H:%i:%s') AS CreatedDate,
                        STR_TO_DATE(NOW(),'%Y-%m-%d %H:%i:%s') AS ModifiedDate
                    FROM
		        qlw_nxin_com.`bsdatadict` a
		        LEFT JOIN qlw_nxin_com.`bsdatadict` c ON 1=1 AND c.pid=202201051355001101
                WHERE a.DictID IN (202201051355001201,202201051355001301)";
            var extendList = _context.FM_CostParamsSetExtendSet.FromSqlInterpolated(sqlextend).ToList();
            //生成方式
            main.Extends = extendList.Where(_ => _.ExtendTypeId == "202201051355001201").ToList();

            //总折旧月数
            main.DepreciationExtends = extendList.Where(_ => _.ExtendTypeId == "202201051355001301").ToList();
            #endregion

            return main;
        }
        #endregion
    }
}
