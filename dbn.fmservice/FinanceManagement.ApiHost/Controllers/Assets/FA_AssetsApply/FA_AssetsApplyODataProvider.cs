using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FA_AssetsApply;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class FA_AssetsApplyODataProvider : OneWithManyQueryProvider<FA_AssetsApplyODataEntity, FA_AssetsApplyDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private BIZ_DataDictODataProvider _biz_DataDictODataProvider;
        private IBiz_Related _biz_RelatedRepository;

        public FA_AssetsApplyODataProvider(IIdentityService identityservice, QlwCrossDbContext context, BIZ_DataDictODataProvider biz_DataDictODataProvider, IBiz_Related biz_RelatedRepository)
        {
            _identityservice = identityservice;
            _context = context;
            _biz_DataDictODataProvider = biz_DataDictODataProvider;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public IEnumerable<FA_AssetsApplyListODataEntity> GetList(ODataQueryOptions<FA_AssetsApplyListODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }
        public IQueryable<FA_AssetsApplyListODataEntity> GetData(ODataQueryOptions<FA_AssetsApplyListODataEntity> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }
        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FA_AssetsApplyListODataEntity> GetMainList(NoneQuery query = null)
        {
            try
            {
                FormattableString sql = $@" SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(ifnull(t2.NumericalOrderDetail,'') USING utf8mb4) NumericalOrderDetail,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.Number USING utf8mb4) Number,	
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,
                                        po.TicketedPointName,
                                        CONVERT(t1.MarketID USING utf8mb4) MarketID,
                                        mar.MarketName,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,
                                        t2.AssetsName,	
                                        CONVERT(t2.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                        t2.Specification,
                                        CONVERT(t2.SupplierID USING utf8mb4) AS SupplierID,
                                        sup.SupplierName,
                                        CONVERT(t2.ProjectID USING utf8mb4) AS ProjectID,
                                        pro.ProjectName,
                                        CASE WHEN (FIND_IN_SET(1, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '通过'
                                        WHEN (FIND_IN_SET(0, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '未审批'
                                        WHEN (FIND_IN_SET(0, faac.results)) THEN '审批中'
                                        WHEN (FIND_IN_SET(2, faac.results) AND !FIND_IN_SET(3, faac.results)) THEN '驳回'
                                        WHEN (FIND_IN_SET(3, faac.results)) THEN '拒绝'
                                        ELSE '未审批' END AS ResultsName
                                        FROM `nxin_qlw_business`.FA_AssetsApply t1
                                        LEFT JOIN `nxin_qlw_business`.FA_AssetsApplyDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t2.AssetsTypeID=cl.ClassificationID
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON t2.SupplierID=sup.SupplierID
                                        LEFT JOIN `qlw_nxin_com`.ppm_project pro ON t2.ProjectID=pro.ProjectID
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                        LEFT JOIN (
                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
                                        FROM  qlw_nxin_com.faauditrecord faac  
                                        inner join `nxin_qlw_business`.FA_AssetsApply temp on temp.NumericalOrder = faac.NumericalOrder
                                        WHERE faac.PersonID<>-1 
                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t1.NumericalOrder = faac.NumericalOrder
                                         WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId}
                                        GROUP BY t2.NumericalOrderDetail
                                        ORDER BY t1.`DataDate` DESC";
                return _context.FA_AssetsApplyListDataSet.FromSqlInterpolated(sql);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
        public override IQueryable<FA_AssetsApplyODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
        public override Task<FA_AssetsApplyODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.Number USING utf8mb4) Number,	
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,
                                        po.TicketedPointName,
                                        CONVERT(t1.MarketID USING utf8mb4) MarketID,
                                        mar.MarketName,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,	
                                        t1.Remarks, t1.UpDataInfo,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate,
                                        CASE WHEN (FIND_IN_SET(1, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '通过'
                                        WHEN (FIND_IN_SET(0, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '未审批'
                                        WHEN (FIND_IN_SET(0, faac.results)) THEN '审批中'
                                        WHEN (FIND_IN_SET(2, faac.results) AND !FIND_IN_SET(3, faac.results)) THEN '驳回'
                                        WHEN (FIND_IN_SET(3, faac.results)) THEN '拒绝'
                                        ELSE '未审批' END AS ResultsName
                                        FROM `nxin_qlw_business`.FA_AssetsApply t1
                                        LEFT JOIN (
                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
                                        FROM  qlw_nxin_com.faauditrecord faac  
                                        WHERE faac.PersonID<>-1 and faac.NumericalOrder ={manyQuery}
                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t1.NumericalOrder = faac.NumericalOrder
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        WHERE t1.NumericalOrder ={manyQuery}";
            var main = _context.FA_AssetsApplyDataSet.FromSqlInterpolated(sql).FirstOrDefault();
            var detail = this.GetDetaiDatasAsync(manyQuery).Result;
            if (detail.Count > 0)
            {
                main.Lines = detail;
            }
            return Task.FromResult(main);
        }
        public override Task<List<FA_AssetsApplyDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        t1.AssetsName,	
                                        CONVERT(t1.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                        t1.Specification,
                                        t1.Brand,
                                        CONVERT(t1.MeasureUnit USING utf8mb4) MeasureUnit,
                                        un.UnitName AS MeasureUnitName,
                                        t1.Quantity,t1.TaxRate,t1.UnitPrice,t1.Amount,
                                        CONVERT(t1.SupplierID USING utf8mb4) AS SupplierID,
                                        sup.SupplierName,
                                        CONVERT(t1.ProjectID USING utf8mb4) AS ProjectID,
                                        pro.ProjectName,
                                        t1.Remarks
                                        FROM `nxin_qlw_business`.FA_AssetsApplyDetail t1
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t1.AssetsTypeID=ClassificationID
                                        LEFT JOIN `qlw_nxin_com`.`unitmeasurement` un ON t1.MeasureUnit=un.UnitID
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON t1.SupplierID=sup.SupplierID
                                        LEFT JOIN `qlw_nxin_com`.ppm_project pro ON t1.ProjectID=pro.ProjectID
                                        WHERE t1.NumericalOrder={manyQuery}
                                        GROUP BY t1.NumericalOrderDetail";
            var result = _context.FA_AssetsApplyDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
            return result;
        }

        /// <summary>
        /// 资产购置合同引用的资产购置申请
        /// </summary>
        public IQueryable<FA_AssetsApplyMobileListODataEntity> GetAssetsApplyMobileList(ODataQueryOptions<FA_AssetsApplyMobileListODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@" SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t2.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.Number USING utf8mb4) Number,	
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,
                                        po.TicketedPointName,
                                        CONVERT(t1.MarketID USING utf8mb4) MarketID,
                                        mar.MarketName,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,
                                        t2.AssetsName,	
                                        CONVERT(t2.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                        t2.Specification,t2.Brand,
                                         CONVERT(t2.MeasureUnit USING utf8mb4) MeasureUnit,
                                        un.UnitName AS MeasureUnitName,
                                        CONVERT(t2.SupplierID USING utf8mb4) AS SupplierID,
                                        sup.SupplierName,
                                        CONVERT(t2.ProjectID USING utf8mb4) AS ProjectID,
                                        pro.ProjectName,
                                        IFNULL(t2.UnitPrice,0) AS UnitPrice,
                                        IFNULL(t2.Quantity,0) AS Quantity,
                                        IFNULL(t2.Amount,0) AS Amount,
                                        IFNULL(temp.QuantityOut,0) AS QuantityOut,
                                        IFNULL(temp.AmountOut,0) AS AmountOut,
                                        IFNULL(t2.Quantity,0)-IFNULL(temp.QuantityOut,0) AS QuantityWait,
                                        IFNULL(t2.Amount,0)-IFNULL(temp.AmountOut,0) AS AmountWait
                                        FROM `nxin_qlw_business`.FA_AssetsApply t1
                                        LEFT JOIN `nxin_qlw_business`.FA_AssetsApplyDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t2.AssetsTypeID=cl.ClassificationID
                                        LEFT JOIN `qlw_nxin_com`.`unitmeasurement` un ON t2.MeasureUnit=un.UnitID
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON t2.SupplierID=sup.SupplierID
                                        LEFT JOIN `qlw_nxin_com`.ppm_project pro ON t2.ProjectID=pro.ProjectID
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                        LEFT JOIN (
						SELECT NumericalOrderInput,SUM(Quantity) AS QuantityOut,SUM(Amount) AS AmountOut FROM 	nxin_qlw_business.FA_AssetsContractDetail
						GROUP BY NumericalOrderInput
                                        ) AS temp ON t2.`NumericalOrderDetail`=temp.NumericalOrderInput
                                        LEFT JOIN (
                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
                                        FROM  qlw_nxin_com.faauditrecord faac  
                                        inner join `nxin_qlw_business`.FA_AssetsApply temp on temp.NumericalOrder = faac.NumericalOrder
                                        WHERE faac.PersonID<>-1 
                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t1.NumericalOrder = faac.NumericalOrder
                                         WHERE FIND_IN_SET(1, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0 and t1.EnterpriseID = {_identityservice.EnterpriseId}
                                        GROUP BY t2.NumericalOrderDetail
                                        ORDER BY t1.`DataDate` DESC";
            return _context.FA_AssetsApplyMobileListDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 资产验收单引用的资产购置申请
        /// </summary>
        public IQueryable<FA_AssetsApplyMobileByInspectListODataEntity> GetAssetsApplyMobileListByInspect(ODataQueryOptions<FA_AssetsApplyMobileByInspectListODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@" SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t2.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.Number USING utf8mb4) Number,	
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,
                                        po.TicketedPointName,
                                        CONVERT(t1.MarketID USING utf8mb4) MarketID,
                                        mar.MarketName,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,
                                        t2.AssetsName,	
                                        CONVERT(t2.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                        t2.Specification,t2.Brand,
                                         CONVERT(t2.MeasureUnit USING utf8mb4) MeasureUnit,
                                        un.UnitName AS MeasureUnitName,
                                        CONVERT(t2.SupplierID USING utf8mb4) AS SupplierID,
                                        sup.SupplierName,
                                        CONVERT(t2.ProjectID USING utf8mb4) AS ProjectID,
                                        pro.ProjectName,
                                        IFNULL(t2.UnitPrice,0) AS UnitPrice,
                                        IFNULL(t2.Quantity,0) AS Quantity,
                                        IFNULL(t2.Amount,0) AS Amount,
                                        IFNULL(temp.QuantityOut,0) AS QuantityOut,
                                        IFNULL(temp.AmountOut,0) AS AmountOut,
                                        IFNULL(t2.Quantity,0)-IFNULL(temp.QuantityOut,0) AS QuantityWait,
                                        IFNULL(t2.Amount,0)-IFNULL(temp.AmountOut,0) AS AmountWait
                                        FROM `nxin_qlw_business`.FA_AssetsApply t1
                                        LEFT JOIN `nxin_qlw_business`.FA_AssetsApplyDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                        LEFT JOIN `qlw_nxin_com`.`unitmeasurement` un ON t2.MeasureUnit=un.UnitID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                        LEFT JOIN `nxin_qlw_business`.`biz_related` re1 ON t2.NumericalOrderDetail=re1.`ChildValue` AND re1.`RelatedType`=201610210104402102 AND re1.`ParentType`=2205161654530000309 AND re1.`ChildType`=2204151105080000109
                                        LEFT JOIN  (
						SELECT NumericalOrderDetail,SUM(Quantity) AS QuantityOut,SUM(Amount) AS AmountOut FROM 	nxin_qlw_business.FA_AssetsInspectDetail
						GROUP BY NumericalOrderDetail
                                        ) AS temp ON re1.`ParentValue`=temp.NumericalOrderDetail
                                        LEFT JOIN (
                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
                                        FROM  qlw_nxin_com.faauditrecord faac  
                                        inner join `nxin_qlw_business`.FA_AssetsApply temp on temp.NumericalOrder = faac.NumericalOrder
                                        WHERE faac.PersonID<>-1 
                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t1.NumericalOrder = faac.NumericalOrder
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t2.AssetsTypeID=cl.ClassificationID
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON t2.SupplierID=sup.SupplierID
                                        LEFT JOIN `qlw_nxin_com`.ppm_project pro ON t2.ProjectID=pro.ProjectID
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                         WHERE  FIND_IN_SET(1, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0 and   t1.EnterpriseID = {_identityservice.EnterpriseId}  
                                        GROUP BY t2.NumericalOrderDetail
                                        ORDER BY t1.`DataDate` DESC";
            return _context.FA_AssetsApplyMobileByInspecListDataSet.FromSqlInterpolated(sql);
        }
    }
}
