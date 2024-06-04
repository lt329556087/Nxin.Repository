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
    public class TreeModelODataProvider
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public TreeModelODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }
        /// <summary>
        /// 获取集团商品分类
        /// </summary>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetProductGroupClassAsync(string groupid)
        {
            FormattableString sql = $@"SELECT CONVERT(ClassificationID USING utf8mb4) AS Id,
                                        ClassificationName AS cName,
                                       CONVERT(PID USING utf8mb4)  AS Pid,Rank
                                        FROM qlw_nxin_com.BIZ_ProductGroupClassification  WHERE EnterpriseID ={groupid}  AND IsUse=1 
                                        group by ClassificationID";
            List<TreeModelExtityODataEntity> list = _context.TreeModelExtityDataSet.FromSqlInterpolated(sql).ToList();
            return list.Select(s => new TreeModelODataEntity()
            {
                Id = s.Id,
                cName = s.cName,
                ExtendId = s.ExtendId,
                Pid = s.Pid,
                Rank = s.Rank
            }).ToList();
        }
        /// <summary>
        /// 获取当前单位的供应商
        /// </summary>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetSupplierAsync()
        {
            FormattableString sql = $@"SELECT CONVERT(SupplierId USING utf8mb4) AS Id,SupplierName AS cName,'0' AS Pid FROM NXin_Qlw_Business.PM_Supplier 
                                        WHERE EnterpriseID ={_identityservice.EnterpriseId} AND IsUse=1
                                       GROUP BY SupplierID,SupplierName ";
            return _context.TreeModelDataSet.FromSqlInterpolated(sql).ToList();
        }
        /// <summary>
        /// 获取当前单位的供应商
        /// </summary>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetSupplierAsync(string InEnterpriseID)
        {
            FormattableString sql = $@"SELECT CONVERT(SupplierId USING utf8mb4) AS Id,SupplierName AS cName,'0' AS Pid FROM NXin_Qlw_Business.PM_Supplier 
                                        WHERE EnterpriseID In ({InEnterpriseID}) AND IsUse=1
                                       GROUP BY SupplierID,SupplierName ";
            return _context.TreeModelDataSet.FromSqlInterpolated(sql).ToList();
        }
        /// <summary>
        /// 获取当前单位的物品分类
        /// </summary>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetSuppliesAsync(string EnterpriseId)
        {
            FormattableString sql = $@"SELECT CONVERT(SuppliesSortID USING utf8mb4) AS Id,cSuppliesSortName AS cName,CONVERT(PID USING utf8mb4)  AS Pid FROM `qlw_nxin_com`.`biz_suppliessort`
                                        WHERE EnterpriseID={EnterpriseId}
                                        GROUP BY SuppliesSortID ";
            return _context.TreeModelDataSet.FromSqlInterpolated(sql).ToList();
        }

        /// <summary>
        /// 获取当前单位的猪场
        /// </summary>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetPigFarmAsync()
        {
            FormattableString sql = $@"SELECT CONVERT(PigFarmID USING utf8mb4) AS Id,PigFarmFullName AS cName,''  AS Pid FROM nxin_qlw_zlw_group.biz_pigfarm
                                        WHERE EnterpriseID={_identityservice.EnterpriseId}
                                        GROUP BY PigFarmID ";
            return _context.TreeModelDataSet.FromSqlInterpolated(sql).ToList();
        }
        public TreeModelODataEntity GetBasicsData(string sqlStr)
        {
            if (!string.IsNullOrEmpty(sqlStr))
            {
                return _context.TreeModelDataSet.FromSqlRaw(sqlStr).FirstOrDefault();
            }
            else
            {
                return new TreeModelODataEntity();
            }
        }
        public List< TreeModelODataEntity> GetBasicsDatas(string sqlStr)
        {
            if (!string.IsNullOrEmpty(sqlStr))
            {
                return _context.TreeModelDataSet.FromSqlRaw(sqlStr).ToList();
            }
            else
            {
                return new List<TreeModelODataEntity>();
            }
        }

    }
}
