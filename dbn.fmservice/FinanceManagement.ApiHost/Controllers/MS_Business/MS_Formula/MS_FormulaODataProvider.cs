using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class MS_FormulaODataProvider : OneWithManyQueryProvider<MS_FormulaODataEntity, MS_FormulaDetailODataEntity>
    {
        ILogger<MS_FormulaODataProvider> _logger;
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        private FD_MallPaymentODataProvider _biz_relatedODataProvider;

        public MS_FormulaODataProvider(IIdentityService identityservice, QlwCrossDbContext context, TreeModelODataProvider treeModel, ILogger<MS_FormulaODataProvider> logger, FD_MallPaymentODataProvider biz_relatedODataProvider)
        {
            _identityservice = identityservice;
            _context = context;
            _logger = logger;
            _treeModel = treeModel;
            _biz_relatedODataProvider = biz_relatedODataProvider;
        }

        public IEnumerable<MS_FormulaListODataEntity> GetList(ODataQueryOptions<MS_FormulaListODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<MS_FormulaListODataEntity> GetData(ODataQueryOptions<MS_FormulaListODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDataList();
        }
        public IQueryable<MS_FormulaListODataEntity> GetDataList(NoneQuery query = null)
        {

            string sql = $@" SELECT  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,
                                CONVERT(t1.Guid USING utf8mb4) Guid, 	
                                CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                CONVERT(t1.Number USING utf8mb4) Number,
                                t1.FormulaName,t1.IsUse,t1.`BaseQuantity`,t1.`Remarks`,t1.`PackageRemarks`,
                                CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                t1.`EarlyWarning`,
                                t1.CreatedDate,
                                t1.ModifiedDate,
                                CONVERT(t1.UseEnterprise USING utf8mb4) UseEnterprise,
                                usent.UseEnterpriseIds,usent.UseEnterpriseNames,
                                CONVERT(t1.UseProduct USING utf8mb4) UseProduct,
                                uspro.UseProductIds,uspro.UseProductNames,
                                t1.`IsGroup`,
                                (CASE WHEN t1.EffectiveBeginDate='1000-01-01' THEN NULL ELSE CONVERT(DATE_FORMAT( t1.EffectiveBeginDate,'%Y-%m-%d') USING utf8mb4) END ) AS  EffectiveBeginDate,
                                (CASE WHEN t1.EffectiveEndDate='9999-12-31' THEN NULL ELSE CONVERT(DATE_FORMAT( t1.EffectiveEndDate,'%Y-%m-%d') USING utf8mb4) END ) AS  EffectiveEndDate,
                                hr1.name OwnerName,
                                CONVERT(hr1.bo_id USING utf8mb4) OwnerID,
                                hr2.name ReviewName,
                                CONVERT(hr2.bo_id USING utf8mb4) ReviewID,
                                CASE WHEN hr2.bo_id IS NULL THEN 0 ELSE 1 END AS IsCheck
                                FROM 
                                nxin_qlw_business.ms_formula t1 
                                LEFT JOIN nxin_qlw_business.biz_reviwe r1 ON t1.NumericalOrder = r1.NumericalOrder AND r1.checkMark=65536
                                LEFT JOIN nxin_qlw_business.hr_person hr1 ON hr1.BO_ID = r1.CheckedByID
                                LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON t1.NumericalOrder = r2.NumericalOrder AND r2.checkMark=16
                                LEFT JOIN nxin_qlw_business.hr_person hr2 ON hr2.BO_ID = r2.CheckedByID
                                LEFT JOIN (
	                                SELECT ParentValue,GROUP_CONCAT(ChildValue) AS UseEnterpriseIds,GROUP_CONCAT(ent.`EnterpriseName`) AS UseEnterpriseNames FROM `nxin_qlw_business`.`biz_related` rel 
	                                LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON rel.ChildValue=ent.`EnterpriseID`
	                                WHERE RelatedType = '201610210104402102'AND  ParentType = '2004141510170000102'AND  ChildType = '1709301424110000101'
                                        GROUP BY ParentValue
                                ) usent ON t1.UseEnterprise=usent.ParentValue
                                LEFT JOIN (
	                                SELECT ParentValue,GROUP_CONCAT(ChildValue) AS UseProductIds,GROUP_CONCAT(ent.`ProductName`) AS UseProductNames FROM `nxin_qlw_business`.`biz_related` rel 
	                                LEFT JOIN `qlw_nxin_com`.`biz_product` ent ON rel.ChildValue=ent.`ProductID`
	                                WHERE RelatedType = '201610210104402102'AND  ParentType = '2004141510170000102'AND  ChildType = '1708231814060000101'
                                        GROUP BY ParentValue
                                ) uspro ON t1.UseProduct=uspro.ParentValue
                                WHERE  t1.EnterpriseID = {_identityservice.GroupId} AND t1.`IsGroup`=1
                                GROUP BY t1.NumericalOrder
                                ORDER BY t1.CreatedDate  ";
            var list = _context.MS_FormulaListDataList.FromSqlRaw(sql);
            return list;
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        public MS_FormulaODataEntity GetSingleData(long manyQuery)
        {
            string sql = $@"SELECT  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(t1.Guid USING utf8mb4) Guid, 	
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                        CONVERT(t1.Number USING utf8mb4) Number,
                                        t1.FormulaName,t1.IsUse,t1.`BaseQuantity`,t1.`Remarks`,t1.`PackageRemarks`,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP1.Name AS OwnerName,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        t1.`EarlyWarning`,
                                        t1.CreatedDate,
                                        t1.ModifiedDate,
                                        CONVERT(t1.UseEnterprise USING utf8mb4) UseEnterprise,
                                        CONVERT(t1.UseProduct USING utf8mb4) UseProduct,
                                        t1.`IsGroup`,
                                        (CASE WHEN t1.EffectiveBeginDate='1000-01-01' THEN NULL ELSE CONVERT(DATE_FORMAT( t1.EffectiveBeginDate,'%Y-%m-%d') USING utf8mb4) END ) AS  EffectiveBeginDate,
                                        (CASE WHEN t1.EffectiveEndDate='9999-12-31' THEN NULL ELSE CONVERT(DATE_FORMAT( t1.EffectiveEndDate,'%Y-%m-%d') USING utf8mb4) END ) AS  EffectiveEndDate
                                        FROM
                                        `nxin_qlw_business`.`MS_Formula` t1
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                        WHERE t1.NumericalOrder ={manyQuery}
                                        GROUP BY t1.`NumericalOrder`";
            var main = _context.MS_FormulaDataSet.FromSqlRaw(sql).FirstOrDefault();
            string relatedSql = @$"SELECT ParentValue,ChildValue AS Id,ent.`EnterpriseName` AS Name FROM `nxin_qlw_business`.`biz_related` rel 
                                    LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON rel.ChildValue = ent.`EnterpriseID`
                                    WHERE RelatedType = '201610210104402102'AND ParentType = '2004141510170000102'AND ChildType = '1709301424110000101' AND ParentValue ={main.UseEnterprise}
                                    UNION ALL
                                    SELECT ParentValue,ChildValue AS Id,ent.`ProductName` AS Name FROM `nxin_qlw_business`.`biz_related` rel
                                    LEFT JOIN `qlw_nxin_com`.`biz_product` ent ON rel.ChildValue = ent.`ProductID`
                                    WHERE RelatedType = '201610210104402102'AND ParentType = '2004141510170000102'AND ChildType = '1708231814060000101' AND ParentValue ={main.UseProduct} ";
            var value = _context.DynamicSqlQuery(relatedSql);
            var useEnterpriseList = value.Where(s => s.ParentValue == main.UseEnterprise).ToList();
            var useProductList = value.Where(s => s.ParentValue == main.UseProduct).ToList();
            foreach (var item in useEnterpriseList)
            {
                main.UseEnterpriseList.Add(new DictionaryData() { Id = item.Id, Name = item.Name });
            }
            foreach (var item in useProductList)
            {
                main.UseProductList.Add(new DictionaryData() { Id = item.Id, Name = item.Name });
            }
            if (main != null)
            {
                main.Lines = GetDetailDatas(manyQuery);
                main.Extends = GetExtendDatas(manyQuery);
            }
            return main;
        }

        public List<MS_FormulaDetailODataEntity> GetDetailDatas(long manyQuery)
        {
            FormattableString sql = $@"SELECT B.RecordID,CONVERT(B.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(B.Guid USING utf8mb4) Guid, 	
                                        CONVERT(B.ProductID USING utf8mb4) ProductID,
                                        pro.ProductName,
                                        pro.Specification,
				                        U.UnitName AS MeasureUnitName,
                                        CONVERT(B.StockType USING utf8mb4) StockType,
                                        CONVERT(B.FormulaTypeID USING utf8mb4) FormulaTypeID,
                                        B.ProportionQuantity,
                                        B.Quantity,
                                        B.RowNum,
                                        B.UnitCost,
                                        B.Cost,
                                        B.ModifiedDate
                                        FROM `nxin_qlw_business`.MS_FormulaDetail  B
                                        LEFT JOIN  `qlw_nxin_com`.`biz_product` pro ON B.`ProductID`=pro.ProductID  AND EnterpriseID={_identityservice.GroupId}
                                        LEFT JOIN  qlw_nxin_com.UnitMeasurement AS U ON pro.MeasureUnit = U.UnitID
                                        WHERE B.NumericalOrder ={manyQuery}";
            var result = _context.MS_FormulaDetailDataSet.FromSqlInterpolated(sql).ToList();
            return result;
        }
        public List<MS_FormulaExtendODataEntity> GetExtendDatas(long manyQuery)
        {
            FormattableString sql = $@"SELECT B.RecordID,CONVERT(B.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(B.Guid USING utf8mb4) Guid, 	
                                        CONVERT(B.ProductID USING utf8mb4) ProductID,
                                        pro.ProductName,
                                        pro.Specification,
                                        pro.StandardPack,
                                        CONVERT(B.PackingID USING utf8mb4) PackingID,
                                        pro1.ProductName AS PackingName,
                                        B.Quantity,
                                        B.RowNum,
                                        B.IsUse,
                                        B.ModifiedDate
                                        FROM `nxin_qlw_business`.ms_formulaextend  B
                                        LEFT JOIN  `qlw_nxin_com`.`biz_product` pro ON B.`ProductID`=pro.ProductID  AND pro.EnterpriseID={_identityservice.GroupId}
                                        LEFT JOIN  `qlw_nxin_com`.`biz_product` pro1 ON B.`PackingID`=pro1.ProductID  AND pro1.EnterpriseID={_identityservice.GroupId}
                                        WHERE B.NumericalOrder ={manyQuery} ";
            var result = _context.MS_FormulaExtendDataSet.FromSqlInterpolated(sql).ToList();
            return result;
        }

        public List<dynamic> GetFormulaByDate(MS_FormulaSearch model)
        {
            string sqlWhere = string.Empty;
            string dateWhere = string.Empty;
            if (!string.IsNullOrEmpty(model.UseProductIds))
            {
                sqlWhere += @$" AND  UseProductIds IN ({model.UseProductIds}) ";
            }
            if (!string.IsNullOrEmpty(model.EndDate))
            {
                dateWhere += @$" OR  '{model.EndDate}' BETWEEN EffectiveBeginDate AND EffectiveEndDate ";
            }
            if (!string.IsNullOrEmpty(model.NumericalOrder)&&model.NumericalOrder!="0")
            {
                dateWhere += @$" and t1.NumericalOrder != {model.NumericalOrder}";
            }
            sqlWhere += @$"  AND  ('{model.BeginDate}' BETWEEN EffectiveBeginDate AND EffectiveEndDate {dateWhere} )";
            string sql = @$"SELECT  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,
				                CONVERT(t1.Number USING utf8mb4) Number,
				                (CASE WHEN t1.EffectiveBeginDate='1000-01-01' THEN NULL ELSE CONVERT(DATE_FORMAT( t1.EffectiveBeginDate,'%Y-%m-%d') USING utf8mb4) END ) AS  EffectiveBeginDate,
				                (CASE WHEN t1.EffectiveEndDate='9999-12-31' THEN NULL ELSE CONVERT(DATE_FORMAT( t1.EffectiveEndDate,'%Y-%m-%d') USING utf8mb4) END ) AS  EffectiveEndDate,
				                B.RecordID,CONVERT(B.NumericalOrder USING utf8mb4) NumericalOrder,
				                CONVERT(B.Guid USING utf8mb4) Guid, 	
				                CONVERT(B.ProductID USING utf8mb4) ProductID,
                                t1.FormulaName,
				                pro.ProductName,
				                pro.Specification,
				                pro.StandardPack,
                                CONVERT(t1.UseEnterprise USING utf8mb4) UseEnterprise,
                                usent.UseEnterpriseIds,usent.UseEnterpriseNames,
                                CONVERT(t1.UseProduct USING utf8mb4) UseProduct,
                                uspro.UseProductIds,uspro.UseProductNames,
                                t1.BaseQuantity,
                                B.ProportionQuantity,
				                U.UnitName AS MeasureUnitName
                                FROM 
                                nxin_qlw_business.ms_formula t1 
                                LEFT JOIN nxin_qlw_business.biz_reviwe r1 ON t1.NumericalOrder = r1.NumericalOrder AND r1.checkMark=65536
                                LEFT JOIN nxin_qlw_business.hr_person hr1 ON hr1.BO_ID = r1.CheckedByID
                                LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON t1.NumericalOrder = r2.NumericalOrder AND r2.checkMark=16
                                LEFT JOIN nxin_qlw_business.hr_person hr2 ON hr2.BO_ID = r2.CheckedByID
                                LEFT JOIN (
	                                SELECT ParentValue,GROUP_CONCAT(ChildValue) AS UseEnterpriseIds,GROUP_CONCAT(ent.`EnterpriseName`) AS UseEnterpriseNames FROM `nxin_qlw_business`.`biz_related` rel 
	                                LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON rel.ChildValue=ent.`EnterpriseID`
	                                WHERE RelatedType = '201610210104402102'AND  ParentType = '2004141510170000102'AND  ChildType = '1709301424110000101'
                                        GROUP BY ParentValue
                                ) usent ON t1.UseEnterprise=usent.ParentValue
                                LEFT JOIN (
	                                SELECT ParentValue,GROUP_CONCAT(ChildValue) AS UseProductIds,GROUP_CONCAT(ent.`ProductName`) AS UseProductNames FROM `nxin_qlw_business`.`biz_related` rel 
	                                LEFT JOIN `qlw_nxin_com`.`biz_product` ent ON rel.ChildValue=ent.`ProductID`
	                                WHERE RelatedType = '201610210104402102'AND  ParentType = '2004141510170000102'AND  ChildType = '1708231814060000101'
                                        GROUP BY ParentValue
                                ) uspro ON t1.UseProduct=uspro.ParentValue
                                
                                LEFT JOIN `nxin_qlw_business`.MS_FormulaDetail  B ON t1.`NumericalOrder`=B.`NumericalOrder`
                                LEFT JOIN  `qlw_nxin_com`.`biz_product` pro ON B.`ProductID`=pro.ProductID  AND pro.EnterpriseID={model.GroupID}
                                LEFT JOIN  qlw_nxin_com.UnitMeasurement AS U ON pro.MeasureUnit = U.UnitID
                                WHERE  t1.EnterpriseID = {model.GroupID} AND t1.`IsGroup`=1
                                AND  UseEnterpriseIds IN ({model.UseEnterpriseIds})   
                               {sqlWhere}
                                GROUP BY t1.NumericalOrder,B.RecordID";
            var result = _context.DynamicSqlQuery(sql);
            if (result.Any())
            {
                string productIds = string.Join(',', result.Select(s => s.ProductID).ToArray());
                string priceSql = $@"SELECT temp2.* FROM (
                                        SELECT ProductID,
                                             MAX(t2.RecordID)                   AS RecordID,
                                             CAST(SUBSTRING_INDEX(
                                                     GROUP_CONCAT(t1.NumericalOrder ORDER BY t1.DataDate DESC, t1.CreatedDate DESC),
                                                     ',', 1) AS DECIMAL(20)) AS NumericalOrder FROM 
                                      `nxin_qlw_business`.`ms_formulaproductprice` t1
                                        LEFT JOIN nxin_qlw_business.MS_FormulaProductPricedetail t2 ON t1.NumericalOrder=t2.NumericalOrder
                                         LEFT JOIN  nxin_qlw_business.MS_FormulaProductPriceext t3 ON t1.NumericalOrder=t3.NumericalOrder
                                      WHERE t1.GroupID={model.GroupID} AND t2.ProductID in ({productIds}) AND t3.EnterpriseID in ({model.UseEnterpriseIds})
                                      GROUP BY  ProductID
                                      ) AS temp1
                                      LEFT JOIN nxin_qlw_business.MS_FormulaProductPricedetail temp2 ON temp1.NumericalOrder=temp2.NumericalOrder AND temp1.ProductID=temp2.ProductID";
                var priceResult = _context.DynamicSqlQuery(priceSql);
                foreach (var item in result)
                {
                    var data = priceResult.Where(s => s.ProductID == item.ProductID).FirstOrDefault();
                    if (data != null)
                    {
                        item.MarketPrice = data.MarketPrice;
                    }
                }
            }
            return result;

        }

        public List<MS_FormulaExprotODataEntity> ExportDataList(MS_FormulaSearch model)
        {
            string sqlWhere = @$" ";
            if (!string.IsNullOrEmpty(model.BeginDate))
            {
                sqlWhere = @$" and  t1.DataDate BETWEEN '{model.BeginDate}' AND  '{model.EndDate}' ";
            }
            if (!string.IsNullOrEmpty(model.UseEnterpriseIds))
            {
                sqlWhere += $" and usent.ParentValue in ({model.UseEnterpriseIds}) ";
            }
            if (!string.IsNullOrEmpty(model.Number))
            {
                sqlWhere += $" and t1.Number like %{model.Number}%  ";
            }
            if (!string.IsNullOrEmpty(model.OwnerID))
            {
                sqlWhere += $" and t1.OwnerID={model.OwnerID}  ";
            }
            if (model.CheckType == "1")
            {
                sqlWhere += $" and hr2.bo_id IS not NULL  ";
            }
            if (model.CheckType == "0")
            {
                sqlWhere += $" and hr2.bo_id IS  NULL  ";
            }
            if (!string.IsNullOrEmpty(model.NumericalOrder))
            {
                sqlWhere += $" and t1.NumericalOrder  =  " + model.NumericalOrder;
            }
            string sql = $@"SELECT  CONCAT(UUID()) AS PrimaryKey,CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,
                                CONVERT(t1.Number USING utf8mb4) Number,
                                CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,	
                                t1.FormulaName,t1.`BaseQuantity`,t1.`Remarks`,t1.`PackageRemarks`,
                                usent.UseEnterpriseNames,
                                uspro.UseProductNames,
                                (CASE WHEN t1.EffectiveBeginDate='1000-01-01' THEN NULL ELSE CONVERT(DATE_FORMAT( t1.EffectiveBeginDate,'%Y/%m/%d') USING utf8mb4) END ) AS  EffectiveBeginDate,
                                (CASE WHEN t1.EffectiveEndDate='9999-12-31' THEN NULL ELSE CONVERT(DATE_FORMAT( t1.EffectiveEndDate,'%Y/%m/%d') USING utf8mb4) END ) AS  EffectiveEndDate,
                                pro2.ProductName,
                                pro2.Specification AS Specification1,
                                U.UnitName AS MeasureUnitName,
                                B.ProportionQuantity,
                                B.Quantity,
                                B.UnitCost,
                                B.Cost,
                                pro.ProductName as ProductExtendName,
                                pro.Specification AS Specification2,
                                pro.StandardPack,
                                pro1.ProductName AS PackingName,
                                C.Quantity as QuantityExtend,
                                CASE WHEN    C.IsUse=TRUE THEN '是' ELSE '否' END AS IsUse
                                FROM 
                                nxin_qlw_business.ms_formula t1 
                                LEFT JOIN nxin_qlw_business.biz_reviwe r1 ON t1.NumericalOrder = r1.NumericalOrder AND r1.checkMark=65536
                                LEFT JOIN nxin_qlw_business.hr_person hr1 ON hr1.BO_ID = r1.CheckedByID
                                LEFT JOIN nxin_qlw_business.biz_reviwe r2 ON t1.NumericalOrder = r2.NumericalOrder AND r2.checkMark=16
                                LEFT JOIN nxin_qlw_business.hr_person hr2 ON hr2.BO_ID = r2.CheckedByID
                                LEFT JOIN (
	                                SELECT ParentValue,GROUP_CONCAT(ChildValue) AS UseEnterpriseIds,GROUP_CONCAT(ent.`EnterpriseName`) AS UseEnterpriseNames FROM `nxin_qlw_business`.`biz_related` rel 
	                                LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON rel.ChildValue=ent.`EnterpriseID`
	                                WHERE RelatedType = '201610210104402102'AND  ParentType = '2004141510170000102'AND  ChildType = '1709301424110000101'
                                        GROUP BY ParentValue
                                ) usent ON t1.UseEnterprise=usent.ParentValue
                                LEFT JOIN (
	                                SELECT ParentValue,GROUP_CONCAT(ChildValue) AS UseProductIds,GROUP_CONCAT(ent.`ProductName`) AS UseProductNames FROM `nxin_qlw_business`.`biz_related` rel 
	                                LEFT JOIN `qlw_nxin_com`.`biz_product` ent ON rel.ChildValue=ent.`ProductID`
	                                WHERE RelatedType = '201610210104402102'AND  ParentType = '2004141510170000102'AND  ChildType = '1708231814060000101'
                                        GROUP BY ParentValue
                                ) uspro ON t1.UseProduct=uspro.ParentValue
                                
                                
                                LEFT JOIN   `nxin_qlw_business`.MS_FormulaDetail  B ON t1.NumericalOrder=B.NumericalOrder
                                LEFT JOIN  `qlw_nxin_com`.`biz_product` pro2 ON B.`ProductID`=pro2.ProductID  AND pro2.EnterpriseID={model.GroupID}
                                LEFT JOIN  qlw_nxin_com.UnitMeasurement AS U ON pro2.MeasureUnit = U.UnitID
                                        
                                       
                                LEFT JOIN  `nxin_qlw_business`.ms_formulaextend  C ON t1.NumericalOrder=C.NumericalOrder
                                LEFT JOIN  `qlw_nxin_com`.`biz_product` pro ON C.`ProductID`=pro.ProductID  AND pro.EnterpriseID={model.GroupID}
                                LEFT JOIN  `qlw_nxin_com`.`biz_product` pro1 ON C.`PackingID`=pro1.ProductID  AND pro1.EnterpriseID={model.GroupID}
                                WHERE  t1.EnterpriseID = {model.GroupID} AND t1.`IsGroup`=1  {sqlWhere}
                                GROUP BY t1.NumericalOrder,B.RecordID,C.RecordID
                                ORDER BY t1.CreatedDate ";
            return _context.MS_FormulaExprotDataSet.FromSqlRaw(sql).ToList();
        }
        public override Task<MS_FormulaODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
        public override Task<List<MS_FormulaDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<MS_FormulaODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
    }
}
