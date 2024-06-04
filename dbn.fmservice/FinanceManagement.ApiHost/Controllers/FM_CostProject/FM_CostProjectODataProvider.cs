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

namespace FinanceManagement.ApiHost.Controllers.FM_CostProject
{
    public class FM_CostProjectODataProvider : QueryProviderAbstraction<FM_CostProjectEntity>
    {
        private IIdentityService _identityService;
        private QlwCrossDbContext _context;

        public FM_CostProjectODataProvider(IIdentityService identityService, QlwCrossDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }

        /// <summary>
        /// OData 专用
        /// </summary>
        /// <returns></returns>
        public override IQueryable<FM_CostProjectEntity> GetDatas()
        {
            FormattableString sql = @$"SELECT 
              STR_TO_DATE(a.DataDate,'%Y-%m-%d') AS DataDate,
              CONCAT(a.EnterpriseID,'') AS EnterpriseID,
              CONCAT(a.CostProjectID,'') AS CostProjectID,
              CONCAT(a.CostProjectName,'') AS CostProjectName,
              CONCAT(a.CostProjectTypeID,'') AS CostProjectTypeID,
              b.cDictName AS CostProjectTypeName,
              a.IsUse,
              a.OrderNumber,
              a.Remarks,
              CONCAT(a.OwnerID,'') AS OwnerID,
              e.`Name` AS OwnerName,
              STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
              STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate,
              a.CostProjectCode,
              CONCAT(a.CollectionType,'') AS CollectionType,
              c.cDictName AS CollectionTypeName,
              CONCAT(a.AllocationType,'') AS AllocationType,
              d.cDictName AS AllocationTypeName,
              CONCAT(a.DataSource,'') AS DataSource,
              CONCAT(a.ModifiedOwnerID,'') AS ModifiedOwnerID,
              f.`Name` AS ModifiedOwnerName,
              CONCAT(a.PresetItem,'') AS PresetItem,
              g.cDictName AS PresetItemName 
            FROM
              nxin_qlw_business.fm_costproject a
              LEFT JOIN qlw_nxin_com.bsdatadict b ON a.CostProjectTypeID=b.DictID
              LEFT JOIN qlw_nxin_com.bsdatadict c ON a.CollectionType=c.DictID
              LEFT JOIN qlw_nxin_com.bsdatadict d ON a.AllocationType=d.DictID
              LEFT JOIN nxin_qlw_business.`hr_person` e ON a.OwnerID=e.`BO_ID`
              LEFT JOIN nxin_qlw_business.`hr_person` f ON a.ModifiedOwnerID=f.`BO_ID`
              LEFT JOIN qlw_nxin_com.bsdatadict g ON a.PresetItem=g.DictID
            WHERE a.EnterpriseID = {_identityService.EnterpriseId}";
            return _context.FM_CostProjectSet.FromSqlInterpolated(sql);
        }

        public FM_CostProjectEntity GetSingleData(string costprojectId)
        {
            var query = GetDatas();
            FM_CostProjectEntity main = query.Where(_=>_.CostProjectId==costprojectId).FirstOrDefault();
            if (main == null)
            {
                return new FM_CostProjectEntity();
            }

            #region 获取明细-有记录
            FormattableString sqldetail = $@"SELECT 
              a.RecordID,
              CONCAT(a.CostProjectID,'') AS CostProjectID,
              CONCAT(a.RelatedType,'') AS RelatedType,
              '' AS RelatedTypeName,
              CONCAT(RelatedID,'') AS RelatedID,
              c.`AccoSubjectFullName` AS RelatedName,
              a.SubsidiaryAccounting,
              CONCAT(a.DataFormula,'') AS DataFormula,
              b.cDictName AS DataFormulaStr
            FROM
              nxin_qlw_business.fm_costprojectdetail a
              LEFT JOIN qlw_nxin_com.bsdatadict b ON a.DataFormula=b.DictID
              LEFT JOIN qlw_nxin_com.`biz_accosubject` c ON a.RelatedID=c.`AccoSubjectID`
              WHERE a.CostProjectID={main.CostProjectId}";
            main.Details = _context.FM_CostProjectDetailSet.FromSqlInterpolated(sqldetail).ToList();

            #region 获取明细的辅助项
            main.Details.ForEach(_=> {
                #region sql副本
                //FormattableString sqlextend = $@"SELECT 
                //  a.RecordID,
                //  CONCAT(a.DetailID,'') AS DetailID,
                //  CONCAT(RelatedType,'') AS RelatedType,
                //  '' AS RelatedTypeName,
                //  CONCAT(RelatedID,'') AS RelatedID,
                //  CASE a.RelatedType WHEN 201904150104402122 THEN b.MarketName 
                //  WHEN 201904150104402123 THEN c.Name
                //  WHEN 201904150104402124 THEN d.`CustomerName`
                //  WHEN 201904150104402125 THEN d.`CustomerName` -- e.`SupplierName`
                //  WHEN 201904150104402126 THEN f.`ProjectName`
                //  ELSE ''
                //  END AS RelatedName
                //FROM
                //  nxin_qlw_business.fm_costprojectextend a
                //  LEFT JOIN qlw_nxin_com.`biz_market` b ON a.RelatedID=b.`MarketID`
                //  LEFT JOIN nxin_qlw_business.`hr_person` c ON a.RelatedID=c.`BO_ID`
                //  LEFT JOIN qlw_nxin_com.`biz_customer` d ON a.RelatedID=d.`CustomerID`
                //  -- LEFT JOIN nxin_qlw_business.`pm_supplier` e ON a.RelatedID=e.`SupplierID` and e.enterpriseid=634086739144001721
                //  LEFT JOIN qlw_nxin_com.`ppm_project` f ON a.RelatedID=f.`ProjectID`
                //  WHERE a.DetailID={_.RecordId}";
                //SELECT * FROM qlw_nxin_com.`biz_customerclassification` WHERE `ClassificationID`=202102030918539097
                #endregion
                FormattableString sqlextend = $@"SELECT 
                  a.RecordID,
                  CONCAT(a.DetailID,'') AS DetailID,
                  CONCAT(RelatedType,'') AS RelatedType,
                  '' AS RelatedTypeName,
                  CONCAT(RelatedID,'') AS RelatedID,
                  CASE a.RelatedType WHEN 201904150104402122 THEN b.MarketName 
                  WHEN 201904150104402123 THEN c.Name
                  WHEN 201904150104402124 THEN d.`CustomerName`
                  WHEN 201904150104402125 THEN d.`CustomerName`
                  WHEN 201904150104402126 THEN f.`ProjectName`
                  ELSE ''
                  END AS RelatedName
                FROM
                  nxin_qlw_business.fm_costprojectextend a
                  LEFT JOIN qlw_nxin_com.`biz_market` b ON a.RelatedID=b.`MarketID`
                  LEFT JOIN nxin_qlw_business.`hr_person` c ON a.RelatedID=c.`BO_ID`
                  LEFT JOIN qlw_nxin_com.`biz_customer` d ON a.RelatedID=d.`CustomerID`
                  LEFT JOIN qlw_nxin_com.`ppm_project` f ON a.RelatedID=f.`ProjectID`
                  WHERE a.DetailID={_.RecordId}";
                _.ExtendDetails = _context.FM_CostProjectExtendSet.FromSqlInterpolated(sqlextend).ToList();
            });
            #endregion

            #endregion

            return main;
        }

        public IQueryable<FM_CostProjectEntity> GetDatasByEnterPriseID(string enterpriseId)
        {
            FormattableString sql = @$"SELECT 
              STR_TO_DATE(a.DataDate,'%Y-%m-%d') AS DataDate,
              CONCAT(a.EnterpriseID,'') AS EnterpriseID,
              CONCAT(a.CostProjectID,'') AS CostProjectID,
              CONCAT(a.CostProjectName,'') AS CostProjectName,
              CONCAT(a.CostProjectTypeID,'') AS CostProjectTypeID,
              b.cDictName AS CostProjectTypeName,
              a.IsUse,
              a.OrderNumber,
              a.Remarks,
              CONCAT(a.OwnerID,'') AS OwnerID,
              e.`Name` AS OwnerName,
              STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
              STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate,
              a.CostProjectCode,
              CONCAT(a.CollectionType,'') AS CollectionType,
              c.cDictName AS CollectionTypeName,
              CONCAT(a.AllocationType,'') AS AllocationType,
              d.cDictName AS AllocationTypeName,
              CONCAT(a.DataSource,'') AS DataSource,
              CONCAT(a.ModifiedOwnerID,'') AS ModifiedOwnerID,
              f.`Name` AS ModifiedOwnerName,
              CONCAT(a.PresetItem,'') AS PresetItem,
              g.cDictName AS PresetItemName 
            FROM
              nxin_qlw_business.fm_costproject a
              LEFT JOIN qlw_nxin_com.bsdatadict b ON a.CostProjectTypeID=b.DictID
              LEFT JOIN qlw_nxin_com.bsdatadict c ON a.CollectionType=c.DictID
              LEFT JOIN qlw_nxin_com.bsdatadict d ON a.AllocationType=d.DictID
              LEFT JOIN nxin_qlw_business.`hr_person` e ON a.OwnerID=e.`BO_ID`
              LEFT JOIN nxin_qlw_business.`hr_person` f ON a.ModifiedOwnerID=f.`BO_ID`
              LEFT JOIN qlw_nxin_com.bsdatadict g ON a.PresetItem=g.DictID
            WHERE a.EnterpriseID = {enterpriseId}";
            return _context.FM_CostProjectSet.FromSqlInterpolated(sql);
        }

        public IQueryable<FM_CostProjectValidateEntity> GetDataExistsUse(string costprojectid)
        {
            FormattableString sql = @$"SELECT DISTINCT CONCAT(a.CostProjectID,'') AS CostProjectID,a.CostProjectName,CONCAT(a.CostProjectCode,'') AS CostProjectCode FROM nxin_qlw_business.`fm_costproject` a
            INNER JOIN nxin_qlw_business.`fm_expensereportdetail` b ON a.CostProjectID=b.CostProjectID
            WHERE a.CostProjectID IN ({costprojectid})
            UNION 
            SELECT DISTINCT CONCAT(C.CostProjectID,'') AS CostProjectID,C.CostProjectName,CONCAT(C.CostProjectCode,'') AS CostProjectCode FROM nxin_qlw_zlw.ms_factoryoverhead A 
            INNER JOIN nxin_qlw_zlw.ms_factoryoverheaddetail B ON A.NumericalOrder=B.NumericalOrder
            INNER JOIN nxin_qlw_business.`fm_costproject` C ON B.ProjectType=c.`PresetItem`
            WHERE C.CostprojectID IN ({costprojectid}) AND A.EnterpriseID={_identityService.EnterpriseId}";
            return _context.FM_CostProjectValidateSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 初始化数据用(获取所有有猪场的单位ID和制单人集合)
        /// </summary>
        /// <returns></returns>
        public IQueryable<FM_EnterpriseEntity> GetAllPigFarmID()
        {
            FormattableString sql = @$"SELECT CONCAT(EnterpriseID,'') AS EnterpriseID,CONCAT(OwnerID,'') AS OwnerID FROM nxin_qlw_zlw.biz_pigfarm GROUP BY EnterpriseID";
            return _context.FM_EnterpriseSet.FromSqlInterpolated(sql);
        }

        #region private method

        #endregion
    }
}
