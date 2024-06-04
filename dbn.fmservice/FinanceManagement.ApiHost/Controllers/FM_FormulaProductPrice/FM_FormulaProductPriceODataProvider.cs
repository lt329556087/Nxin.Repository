using Architecture.Common.Application.Query;
using Architecture.Common.Util;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.MS_FormulaProductPrice;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class MS_FormulaProductPriceODataProvider : OneWithManyQueryProvider<MS_FormulaProductPriceODataEntity, MS_FormulaProductPriceDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public MS_FormulaProductPriceODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IQueryable<MS_FormulaProductPriceODataEntity> GetList(ODataQueryOptions<MS_FormulaProductPriceODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetDatas();
            if (datas.Any())
            {
                foreach (var data in datas)
                {
                    var ext = GetExtData(data.NumericalOrder);
                    if (ext?.Count > 0)
                    {
                        var EnterpriseNameList = ext.Select(p => p.EnterpriseName).ToList();
                        data.EnterpriseName = string.Join(",", EnterpriseNameList);
                    }
                    if (!string.IsNullOrEmpty(data.CheckedByID)) { data.CheckState = 1; }
                }
            }
            return datas;
        }


        public override IQueryable<MS_FormulaProductPriceODataEntity> GetDatas(NoneQuery query = null)
        {
            var sql = GetHeadSql();
            sql += string.Format(@"
                                WHERE f.GroupID= {0} 
                                ORDER BY f.DataDate DESC,f.Number DESC  ", _identityservice.GroupId);
            return _context.MS_FormulaProductPriceDataSet.FromSqlRaw(sql);
        }
        private string GetHeadSql()
        {
            return string.Format(@"
            SELECT CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,CONVERT(f.GroupID USING utf8mb4) GroupID,CONVERT(DATE_FORMAT( f.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,CONVERT(f.Number USING utf8mb4) Number,CONVERT(f.OwnerID USING utf8mb4) OwnerID,f.CreatedDate,f.ModifiedDate
            ,h.Name OwnerName,CONVERT(bv.CheckedByID USING utf8mb4) AS CheckedByID,h1.Name CheckedByName,'' EnterpriseName,0 CheckState
            FROM nxin_qlw_business.MS_FormulaProductPrice f
            LEFT JOIN nxin_qlw_business.hr_person h ON f.OwnerID=h.BO_ID
            LEFT JOIN nxin_qlw_business.biz_reviwe bv ON f.NumericalOrder= bv.NumericalOrder AND bv.CheckMark=16 
            LEFT JOIN nxin_qlw_business.hr_person h1 ON bv.CheckedByID=h1.BO_ID  ");
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<MS_FormulaProductPriceODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            var sql = GetHeadSql();
            sql += $@"
                    WHERE f.NumericalOrder = {manyQuery}";

            return _context.MS_FormulaProductPriceDataSet.FromSqlRaw(sql).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public override Task<List<MS_FormulaProductPriceDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            var sql = @$"SELECT f.RecordID,CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,CONVERT(f.ProductID USING utf8mb4) ProductID,f.Specification,f.StandardPack,CONVERT(f.MeasureUnit USING utf8mb4) MeasureUnit,f.MarketPrice,f.Remarks,f.ModifiedDate
                                        ,p.ProductName,u.UnitName MeasureUnitName
                                         FROM nxin_qlw_business.MS_FormulaProductPricedetail f
                                        LEFT JOIN qlw_nxin_com.biz_product p ON f.ProductID=p.ProductID 
                                        LEFT JOIN qlw_nxin_com.unitmeasurement u ON f.MeasureUnit=u.UnitID
                                        WHERE f.NumericalOrder = {manyQuery}";

            return _context.MS_FormulaProductPriceDetailDataSet.FromSqlRaw(sql).ToListAsync();
        }

        /// <summary>
        /// 按流水号查询单位
        /// </summary>
        /// <param name="NumericalOrder"></param>
        /// <returns></returns>
        public List<MS_FormulaProductPriceExtODataEntity> GetExtData(string NumericalOrder)
        {
            var sql = $@"SELECT f.RecordID,CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,CONVERT(f.EnterpriseID USING utf8mb4) EnterpriseID,f.ModifiedDate,e.EnterpriseName
                                     FROM nxin_qlw_business.MS_FormulaProductPriceext f
                                    LEFT JOIN qlw_nxin_com.biz_enterprise e ON f.EnterpriseID=e.EnterpriseID
                                     where f.NumericalOrder =  {NumericalOrder};";

            return _context.MS_FormulaProductPriceExtDataSet.FromSqlRaw(sql).ToList();
        }

        /// <summary>
        /// 查询同一天、同一商品是否已存在
        /// </summary>
        /// <param name="req"></param>
        /// <param name="productIDs"></param>
        /// <returns></returns>
        public List<MS_FormulaProductPriceValidODataEntity> GetExistProductByCon(MS_FormulaProductPriceBaseCommand req, string productIDs,out string sql)
        {
            var sqlwhere = "";
            if (!string.IsNullOrEmpty(req.NumericalOrder))
            {
                sqlwhere += $@" AND f.NumericalOrder!={req.NumericalOrder} ";
            }
            sql = $@"SELECT UUID() Guid,CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,CONVERT(f.Number USING utf8mb4) Number,CONVERT(fd.ProductID USING utf8mb4) ProductID ,p.ProductName 
                        FROM nxin_qlw_business.MS_FormulaProductPrice f
                        INNER JOIN nxin_qlw_business.MS_FormulaProductPricedetail fd ON f.NumericalOrder=fd.NumericalOrder
                        LEFT JOIN qlw_nxin_com.biz_product p ON fd.ProductID=p.ProductID
                        WHERE f.GroupID={req.GroupID} AND  f.DataDate ='{req.DataDate.ToString("yyyy-MM-dd")}' AND fd.ProductID in({productIDs})  {sqlwhere}
                        GROUP BY f.NumericalOrder,fd.ProductID";
            return _context.MS_FormulaProductPriceValaidDataSet.FromSqlRaw(sql).ToList();
        }
        public List<MS_FormulaProductPriceExtODataEntity> GetExtDataByCon(string numericalOrders, string enterpriseIDs)
        {
            var sql = $@"SELECT f.RecordID,CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,CONVERT(f.EnterpriseID USING utf8mb4) EnterpriseID,f.ModifiedDate,e.EnterpriseName
                                     FROM nxin_qlw_business.MS_FormulaProductPriceext f
                                    LEFT JOIN qlw_nxin_com.biz_enterprise e ON f.EnterpriseID=e.EnterpriseID
                                     where f.NumericalOrder in({numericalOrders}) AND f.EnterpriseID in ({enterpriseIDs})";

            return _context.MS_FormulaProductPriceExtDataSet.FromSqlRaw(sql).ToList();
        }
        public Task<List<MS_FormulaProductPriceExportODataEntity>> GetExportDatalist(MS_FormulaProductPriceExportRequest req)
        {
            var sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(req.BeginDate))
            {
                sqlWhere.Append($@" AND f.DataDate >='{req.BeginDate}' ");
            }
            if (!string.IsNullOrEmpty(req.EndDate))
            {
                sqlWhere.Append($@" AND f.DataDate <= '{req.EndDate}' ");
            }
            if (!string.IsNullOrEmpty(req.Number))
            {
                sqlWhere.Append($@" AND f.Number LIKE '%{req.Number}%' ");
            }
            if (req.CheckState == 0)
            {
                sqlWhere.Append($@" AND bv.NumericalOrder is null  ");
            }
            if (req.CheckState == 1)
            {
                sqlWhere.Append($@" AND bv.NumericalOrder is not null  ");
            }
            if (!string.IsNullOrEmpty(req.NumericalOrder))
            {
                sqlWhere.Append($@" AND f.NumericalOrder={req.NumericalOrder} ");
            }
            var sql = $@"SELECT  CONVERT(f.Number USING utf8mb4) Number,'' EnterpriseName,DATE_FORMAT(f.DataDate,'%Y-%m-%d') DataDate,p.ProductName,fd.Specification,fd.StandardPack,u.UnitName MeasureUnitName,fd.MarketPrice,fd.Remarks,CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,UUID() Guid
                        -- ,fd.ProductID
                        FROM nxin_qlw_business.MS_FormulaProductPrice f
                        INNER JOIN nxin_qlw_business.MS_FormulaProductPricedetail fd ON f.NumericalOrder=fd.NumericalOrder
                        LEFT JOIN qlw_nxin_com.biz_product p ON fd.ProductID=p.ProductID
                        LEFT JOIN qlw_nxin_com.unitmeasurement u ON fd.MeasureUnit=u.UnitID
                        LEFT JOIN nxin_qlw_business.biz_reviwe bv ON f.NumericalOrder= bv.NumericalOrder AND bv.CheckMark=16 
                        WHERE f.GroupID={req.GroupID} {sqlWhere} ";

            return _context.MS_FormulaProductPriceExportDataSet.FromSqlRaw(sql).ToListAsync();
        }
    }
}
