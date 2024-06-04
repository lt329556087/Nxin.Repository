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

namespace FinanceManagement.ApiHost.Controllers
{
    public class FM_PigOriginalAssetsODataProvider : QueryProviderAbstraction<FM_PigOriginalAssetsEntity>
    {
        private IIdentityService _identityService;
        private QlwCrossDbContext _context;

        public FM_PigOriginalAssetsODataProvider(IIdentityService identityService, QlwCrossDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }

        /// <summary>
        /// OData 专用
        /// </summary>
        /// <returns></returns>
        public override IQueryable<FM_PigOriginalAssetsEntity> GetDatas()
        {
            FormattableString sql = @$"SELECT 
                  CONCAT(a.NumericalOrder,b.EarNumber) AS PrimaryKey,
                  CONCAT(a.NumericalOrder,'') AS NumericalOrder,
                  CONCAT(a.Number,'') AS Number,
                  STR_TO_DATE(a.DataDate,'%Y-%m-%d') AS DataDate,
                  CONCAT(a.PigFarmID,'') AS PigFarmID,
                  CONCAT(a.EnterpriseID,'') AS EnterpriseID,
                  CONCAT(a.SourceType,'') AS SourceType,
                  e.cDictName AS SourceTypeName,
                  a.Remarks,
                  CONCAT(a.OwnerID,'') AS OwnerID,
                  c.Name AS OwnerName,
                  CONCAT(a.ModifiedOwnerID,'') AS ModifiedOwnerID,
                  d.Name AS ModifiedOwnerName,
                  HP1.Name CheckedByName,
                  STR_TO_DATE(HP1.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS CheckedDate,
                  STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
                  STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate,
                  CONCAT(b.EarNumber,'') AS EarNumber,
                  zbp.EarNumber AS EarNumberName,
                  CONCAT(b.PigType,'') AS PigType,
                  zd.DictName AS PigTypeName,
                  b.OriginalValue,
                  b.DepreciationUseMonth,
                  b.ResidualValueRate,
                  b.ResidualValue,
                  b.StartDate,
                  b.AccruedMonth,
                  b.DepreciationAccumulated
                FROM
                  nxin_qlw_business.fm_pigoriginalassets a
                  LEFT JOIN nxin_qlw_business.fm_pigoriginalassetsdetail b ON a.NumericalOrder=b.NumericalOrder
                  LEFT JOIN nxin_qlw_business.hr_person c ON a.OwnerID=c.BO_ID
                  LEFT JOIN nxin_qlw_business.hr_person d ON a.ModifiedOwnerID=d.BO_ID
                  LEFT JOIN qlw_nxin_com.bsdatadict e ON a.SourceType=e.DictID
                  LEFT JOIN nxin_qlw_business.BIZ_Reviwe BZ ON BZ.NumericalOrder=a.NumericalOrder AND BZ.CheckMark=16 
                  LEFT JOIN nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID=BZ.CheckedByID
                  LEFT JOIN nxin_qlw_zlw.biz_pigunit zbp ON b.EarNumber=zbp.PigID
                  LEFT JOIN nxin_qlw_zlw.`biz_datadict` zd ON zbp.`PigType`=zd.DictID
            WHERE a.EnterpriseID = {_identityService.EnterpriseId}";
            return _context.FM_PigOriginalAssetsSet.FromSqlInterpolated(sql);
        }

        public FM_PigOriginalAssetsEntity GetSingleData(string numericalOrder)
        {
            var query = GetDatas();
            FM_PigOriginalAssetsEntity main = query.Where(_ => _.NumericalOrder == numericalOrder).FirstOrDefault();
            if (main == null)
            {
                return new FM_PigOriginalAssetsEntity();
            }

            #region 获取明细-有记录
            FormattableString sqldetail = $@"SELECT 
                  CONCAT(b.NumericalOrder,'') AS NumericalOrder,
                  CONCAT(b.EarNumber,'') AS EarNumber,
                  zbp.EarNumber AS EarNumberName,
                  CONCAT(b.PigType,'') AS PigType,
                  zd.DictName AS PigTypeName,
                  b.OriginalValue,
                  b.DepreciationUseMonth,
                  b.ResidualValueRate,
                  b.ResidualValue,
                  STR_TO_DATE(b.StartDate,'%Y-%m-%d') AS StartDate,
                  b.AccruedMonth,
                  b.DepreciationAccumulated,
                  STR_TO_DATE(b.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
                  STR_TO_DATE(b.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate 
                FROM
                  nxin_qlw_business.fm_pigoriginalassetsdetail b
                  LEFT JOIN nxin_qlw_zlw.biz_pigunit zbp ON b.EarNumber=zbp.PigID
                  LEFT JOIN nxin_qlw_zlw.`biz_datadict` zd ON zbp.`PigType`=zd.DictID
              WHERE b.NumericalOrder={main.NumericalOrder}";
            main.Details = _context.FM_PigOriginalAssetsDetailSet.FromSqlInterpolated(sqldetail).ToList();

            #endregion

            #region 扩展表记录
            string sqldetaillist = @"SELECT 
                  a.RecordID,
                  STR_TO_DATE(a.DataDate,'%Y-%m-%d') AS DataDate,
                  CONCAT(a.EarNumber,'') AS EarNumber,
                  zbp.EarNumber AS EarNumberName,
                  CONCAT(a.PigType,'') AS PigType,
                  zd.DictName AS PigTypeName,
                  CONCAT(a.PigFarmID,'') AS PigFarmID,
                  CONCAT(a.EnterpriseID,'') AS EnterpriseID,
                  a.OriginalValue,
                  a.DepreciationUseMonth,
                  a.ResidualValueRate,
                  a.ResidualValue,
                  STR_TO_DATE(a.StartDate,'%Y-%m-%d') AS StartDate,
                  a.AccruedMonth,
                  a.DepreciationAccumulated,
                  a.DepreciationMonthAmount,
                  a.DepreciationMonthRate,
                  a.NetValue,
                  STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
                  STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate
                FROM
                  nxin_qlw_business.fm_pigoriginalassetsdetaillist a
                  LEFT JOIN nxin_qlw_zlw.biz_pigunit zbp ON a.EarNumber=zbp.PigID
                  LEFT JOIN nxin_qlw_zlw.`biz_datadict` zd ON zbp.`PigType`=zd.DictID";
            #endregion

            return main;
        }
    }
}
