using Architecture.Common.Application.Query;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class FA_InventoryODataProvider : OneWithManyQueryProvider<FA_InventoryODataEntity, FA_InventoryDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private readonly HttpClientUtil _httpClientUtil;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HostConfiguration _hostConfiguration;
        public FA_InventoryODataProvider(IIdentityService identityservice, QlwCrossDbContext context, HttpClientUtil httpClientUtil, IHttpContextAccessor httpContextAccessor, HostConfiguration hostConfiguration)
        {
            _identityservice = identityservice;
            _context = context;
            _httpClientUtil = httpClientUtil;
            _httpContextAccessor = httpContextAccessor;
            _hostConfiguration = hostConfiguration;
        }

    
        public IQueryable<FA_InventoryODataEntity> GetList(ODataQueryOptions<FA_InventoryODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri);
            return datas;
        }
        public IQueryable<FA_InventoryODataEntity> GetData(ODataQueryOptions<FA_InventoryODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FA_InventoryODataEntity> GetDatas(NoneQuery query = null)
        {
            var sql = string.Format(@$"SELECT CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,
                                    	CONVERT(f.EnterpriseID USING utf8mb4) EnterpriseID,
                                    	CONVERT(f.Number USING utf8mb4) Number,
                                    	DATE_FORMAT(f.DataDate, '%Y-%m-%d') DataDate, 
                                    	f.FAPlaceID,f.UseStateID,f.Remarks,
	                                    CONVERT(f.OwnerID USING utf8mb4) OwnerID,
	                                    CONVERT(v.CheckedByID USING utf8mb4) AuditID,
                                    	CONVERT(f.CreatedDate USING utf8mb4) CreatedDate,
                                            CONVERT(f.ModifiedDate USING utf8mb4) ModifiedDate,
                                            e.EnterpriseName, h1.Name OwnerName,h.Name AuditName,
                                            (SELECT GROUP_CONCAT(fp.FAPlaceName) FROM nxin_qlw_business.fa_fixedassetsplace fp WHERE FIND_IN_SET(fp.FAPlaceID,f.FAPlaceID)) AS FAPlaceName,
                                            (SELECT GROUP_CONCAT(fu.UseStateName) FROM nxin_qlw_business.fa_assetsusestate fu WHERE FIND_IN_SET(fu.UseStateID,f.UseStateID)) AS UseStateName,
                                            SUM(fd.Quantity) Quantity,SUM(fd.InventoryQuantity) InventoryQuantity,0.0 QuantityDiff        
                                    	FROM nxin_qlw_business.fa_inventory f
                                    LEFT JOIN nxin_qlw_business.fa_inventorydetail  fd ON f.NumericalOrder=fd.NumericalOrder	
                                    LEFT JOIN qlw_nxin_com.biz_enterprise e ON f.EnterpriseID=e.EnterpriseID
                                    LEFT JOIN nxin_qlw_business.biz_reviwe v ON v.NumericalOrder=f.NumericalOrder AND v.CheckMark=16
                                    LEFT JOIN nxin_qlw_business.hr_person h ON h.BO_ID=v.CheckedByID
                                    LEFT JOIN nxin_qlw_business.hr_person h1 ON h1.BO_ID=f.OwnerID
                                    WHERE f.EnterpriseID={_identityservice.EnterpriseId} 
                                    GROUP BY f.NumericalOrder
                                ORDER BY f.DataDate DESC,f.Number DESC  ");
            return _context.FA_InventoryEntityODataSet.FromSqlRaw(sql);
        }
        

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FA_InventoryODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = GetDataSql(manyQuery);

            return _context.FA_InventoryEntityODataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }      
        private FormattableString GetDataSql(long manyQuery)
        {
            return @$"SELECT CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,
                        	CONVERT(f.EnterpriseID USING utf8mb4) EnterpriseID,
                        	CONVERT(f.Number USING utf8mb4) Number,
                        	DATE_FORMAT(f.DataDate, '%Y-%m-%d') DataDate, 
                        	f.FAPlaceID,f.UseStateID,f.Remarks,
	                        CONVERT(f.OwnerID USING utf8mb4) OwnerID,
	                        CONVERT(v.CheckedByID USING utf8mb4) AuditID,
                        	CONVERT(f.CreatedDate USING utf8mb4) CreatedDate,
                                CONVERT(f.ModifiedDate USING utf8mb4) ModifiedDate,
                                e.EnterpriseName, h1.Name OwnerName,h.Name AuditName,
                                (SELECT GROUP_CONCAT(fp.FAPlaceName) FROM nxin_qlw_business.fa_fixedassetsplace fp WHERE FIND_IN_SET(fp.FAPlaceID,f.FAPlaceID)) AS FAPlaceName,
                                (SELECT GROUP_CONCAT(fu.UseStateName) FROM nxin_qlw_business.fa_assetsusestate fu WHERE FIND_IN_SET(fu.UseStateID,f.UseStateID)) AS UseStateName,
                            0.0 Quantity,0.0 InventoryQuantity,0.0 QuantityDiff
                        	FROM nxin_qlw_business.fa_inventory f
                        LEFT JOIN qlw_nxin_com.biz_enterprise e ON f.EnterpriseID=e.EnterpriseID
                        LEFT JOIN nxin_qlw_business.biz_reviwe v ON v.NumericalOrder=f.NumericalOrder AND v.CheckMark=16
                        LEFT JOIN nxin_qlw_business.hr_person h ON h.BO_ID=v.CheckedByID
                        LEFT JOIN nxin_qlw_business.hr_person h1 ON h1.BO_ID=f.OwnerID
                        WHERE f.NumericalOrder= {manyQuery}";
        }

        public override Task<List<FA_InventoryDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = GetDetailSql(manyQuery);
            return _context.FA_InventoryDetailEntityODataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        private FormattableString GetDetailSql(long manyQuery)
        {
            return @$" SELECT CONVERT(fd.RecordID USING utf8mb4) RecordID,
                              CONVERT(fd.NumericalOrder USING utf8mb4) NumericalOrder,
                              CONVERT(fd.CardID USING utf8mb4) CardID,
                              fd.Quantity,fd.InventoryQuantity,0.0 QuantityDiff,fd.Remarks,fd.FileName,fd.PathUrl,
                             -- CONVERT(fad.ClassificationID USING utf8mb4) ClassificationID,
                              CONVERT(IFNULL(fad.UseStateID,a.UseStateID) USING utf8mb4) UseStateID,
                              CONVERT(IFNULL(fad.FAPlaceID,a.FAPlaceID) USING utf8mb4) FAPlaceID,
                              a.AssetsCode,a.AssetsName,a.CardCode,a.Specification,fc.ClassificationName,IFNULL(fu.UseStateName,fu1.UseStateName) UseStateName,
                              IFNULL(fp.FAPlaceName,fp1.FAPlaceName) FAPlaceName,um.UnitName MeasureUnitName,bm.MarketFullName
                       FROM nxin_qlw_business.fa_inventorydetail fd
                       LEFT JOIN nxin_qlw_business.fa_assetscard a ON a.CardID=fd.CardID
                       LEFT JOIN (SELECT a.CardID,MAX(ad.DataDate) DataDate FROM nxin_qlw_business.fa_inventorydetail a
                       INNER JOIN nxin_qlw_business.fa_assetscarddetail ad ON a.CardID =ad.CardID
                       WHERE a.NumericalOrder={manyQuery} 
                       GROUP BY a.CardID) ad ON ad.CardID=fd.CardID
                       LEFT JOIN nxin_qlw_business.fa_assetscarddetail fad ON fad.CardID=ad.CardID AND fad.DataDate=ad.DataDate
                       LEFT JOIN nxin_qlw_business.fa_assetsclassification fc ON fc.ClassificationID=fad.ClassificationID
                       LEFT JOIN nxin_qlw_business.fa_assetsusestate fu ON fu.UseStateID=fad.UseStateID
                       LEFT JOIN nxin_qlw_business.fa_assetsusestate fu1 ON fu1.UseStateID=a.UseStateID
                       LEFT JOIN nxin_qlw_business.fa_fixedassetsplace fp ON fp.FAPlaceID=fad.FAPlaceID 
                       LEFT JOIN nxin_qlw_business.fa_fixedassetsplace fp1 ON fp1.FAPlaceID=a.FAPlaceID
                       LEFT JOIN qlw_nxin_com.unitmeasurement um ON um.UnitID=a.MeasureUnit
                       LEFT JOIN (SELECT ade.CardID,GROUP_CONCAT(bm.cFullName) MarketFullName FROM 
                       (SELECT a.CardID,MAX(ade.DataDate) DataDate FROM nxin_qlw_business.fa_inventorydetail a
                       INNER JOIN nxin_qlw_business.fa_assetscardextend ade ON a.CardID =ade.CardID
                       WHERE a.NumericalOrder={manyQuery}
                       GROUP BY a.CardID) ade 
                       INNER JOIN nxin_qlw_business.fa_assetscardextend fade ON fade.CardID =ade.CardID AND fade.DataDate=ade.DataDate
                       INNER JOIN qlw_nxin_com.biz_market bm ON bm.MarketID=fade.DepartmentID GROUP BY ade.CardID)  bm ON bm.CardID=fd.CardID
                       WHERE fd.NumericalOrder={manyQuery}";
        }
        public List<FA_InventoryDetailODataEntity> GetAssetscardList(AssetscardSearch param)
        {
            var sqlWhere = new StringBuilder();
            if (string.IsNullOrEmpty(param.EnterpriseID))
            {
                param.EnterpriseID = _identityservice.EnterpriseId;
            }            
            if (string.IsNullOrEmpty(param.DateDate))
            {
                param.DateDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            sqlWhere.AppendFormat("  WHERE a.EnterpriseID={0} AND a.DataDate<='{1}' ",param.EnterpriseID,param.DateDate);
            if (!string.IsNullOrEmpty(param.FAPlaceID))
            {
                sqlWhere.AppendLine(string.Format(@" AND (fd.FAPlaceID IN({0}) ) ", param.FAPlaceID));
                //sqlWhere.AppendLine(string.Format(@" AND (a.FAPlaceID IN({0}) OR fd.FAPlaceID IN({0}) ) ", param.FAPlaceID));
            }
            if (!string.IsNullOrEmpty(param.UseStateID))
            {
                sqlWhere.AppendLine(string.Format(@" AND (a.UseStateID IN({0}) OR fd.UseStateID IN({0}) ) ", param.UseStateID));
            }
            var sql = $@"SELECT  UUID() RecordID, CONVERT(a.CardID USING utf8mb4) CardID 
                              , '' NumericalOrder,fd.Quantity Quantity,fd.Quantity InventoryQuantity,0.0 QuantityDiff,'' Remarks,'' FileName,''PathUrl,
                              CONVERT(IFNULL(fd.UseStateID,a.UseStateID) USING utf8mb4) UseStateID,
                              CONVERT(fd.FAPlaceID USING utf8mb4) FAPlaceID,
                              a.AssetsCode,a.AssetsName,a.CardCode,a.Specification,fc.ClassificationName,IFNULL(fu.UseStateName,fu1.UseStateName) UseStateName,
                             fp.FAPlaceName FAPlaceName,um.UnitName MeasureUnitName,bm.MarketFullName
                        FROM nxin_qlw_business.fa_assetscard a
                        INNER JOIN(                              
                        SELECT a.CardID,MAX(fd.DataDate) DataDate FROM nxin_qlw_business.fa_assetscard a
                        INNER JOIN nxin_qlw_business.fa_assetscarddetail fd ON a.CardID=fd.CardID
                        {sqlWhere}
                        GROUP BY a.CardID) fdt ON a.CardID=fdt.CardID
                        INNER JOIN nxin_qlw_business.fa_assetscarddetail fd ON a.CardID=fd.CardID AND fd.DataDate=fdt.DataDate
                        LEFT JOIN nxin_qlw_business.fa_assetsclassification fc ON fc.ClassificationID=fd.ClassificationID
                        LEFT JOIN nxin_qlw_business.fa_assetsusestate fu ON fu.UseStateID=fd.UseStateID
                        LEFT JOIN nxin_qlw_business.fa_assetsusestate fu1 ON fu1.UseStateID=a.UseStateID
                        LEFT JOIN nxin_qlw_business.fa_fixedassetsplace fp ON fp.FAPlaceID=fd.FAPlaceID 
                        -- LEFT JOIN nxin_qlw_business.fa_fixedassetsplace fp1 ON fp1.FAPlaceID=a.FAPlaceID
                        LEFT JOIN qlw_nxin_com.unitmeasurement um ON um.UnitID=a.MeasureUnit
                        LEFT JOIN (SELECT ade.CardID,GROUP_CONCAT(bm.cFullName) MarketFullName FROM 
                        (SELECT a.CardID,MAX(ade.DataDate) DataDate FROM nxin_qlw_business.fa_assetscard a
                        INNER JOIN nxin_qlw_business.fa_assetscardextend ade ON a.CardID =ade.CardID
                        WHERE a.EnterpriseID={param.EnterpriseID} AND a.DataDate<='{param.DateDate}'
                        GROUP BY a.CardID) ade 
                        INNER JOIN nxin_qlw_business.fa_assetscardextend fade ON fade.CardID =ade.CardID AND fade.DataDate=ade.DataDate
                        INNER JOIN qlw_nxin_com.biz_market bm ON bm.MarketID=fade.DepartmentID GROUP BY ade.CardID)  bm ON bm.CardID=fd.CardID
                         {sqlWhere} ";
            return _context.FA_InventoryDetailEntityODataSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 获取当天的盘点单卡片
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<DropODataEntity> GetInventoryDetailByDate(AssetscardSearch param)
        {
            if (string.IsNullOrEmpty(param.EnterpriseID))
            {
                param.EnterpriseID = _identityservice.EnterpriseId;
            }
            var sql = $@"SELECT CONVERT(fd.CardID USING utf8mb4) id,
                              CONVERT(fd.NumericalOrder USING utf8mb4) NAME,
                              CONVERT(f.Number USING utf8mb4) curtype                               
                       FROM nxin_qlw_business.fa_inventory f
                       INNER JOIN nxin_qlw_business.fa_inventorydetail fd ON f.NumericalOrder=fd.NumericalOrder
                       WHERE f.EnterpriseID={param.EnterpriseID} AND f.DataDate='{param.DateDate}' ";
            if (!string.IsNullOrEmpty(param.NumericalOrder))
            {
                sql +=string.Format(" AND f.NumericalOrder!={0} ",param.NumericalOrder) ;
            }
            return _context.DropODataSet.FromSqlRaw(sql).ToList();
        }
    }
}
