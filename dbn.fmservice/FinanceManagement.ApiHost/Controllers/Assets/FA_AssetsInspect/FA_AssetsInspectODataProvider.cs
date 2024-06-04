using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FA_AssetsInspect;
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
    public class FA_AssetsInspectODataProvider : OneWithManyQueryProvider<FA_AssetsInspectODataEntity, FA_AssetsInspectDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private BIZ_DataDictODataProvider _biz_DataDictODataProvider;
        private IBiz_Related _biz_RelatedRepository;
        public FA_AssetsInspectODataProvider(IIdentityService identityservice, QlwCrossDbContext context, BIZ_DataDictODataProvider biz_DataDictODataProvider, IBiz_Related biz_RelatedRepository)
        {
            _identityservice = identityservice;
            _context = context;
            _biz_DataDictODataProvider = biz_DataDictODataProvider;
            _biz_RelatedRepository = biz_RelatedRepository;
        }
        public IEnumerable<FA_AssetsInspectListODataEntity> GetList(ODataQueryOptions<FA_AssetsInspectListODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }
        public IQueryable<FA_AssetsInspectListODataEntity> GetData(ODataQueryOptions<FA_AssetsInspectListODataEntity> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FA_AssetsInspectListODataEntity> GetMainList(NoneQuery query = null)
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
                                        CONVERT(HP1.BO_ID USING utf8mb4) CheckValueID,
                                        HP1.Name AS CheckValueName,
                                        t2.AssetsName,	
                                        CONVERT(t2.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                        t2.Specification,
                                        CONVERT(t2.SupplierID USING utf8mb4) AS SupplierID,
                                        sup.SupplierName,
                                        CONVERT(t2.ProjectID USING utf8mb4) AS ProjectID,
                                        pro.ProjectName,
                                        CONVERT(t2.AssetsNatureId USING utf8mb4) AssetsNatureId,
                                        dict.DataDictName AS AssetsNatureName,
CASE WHEN (FIND_IN_SET(1, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '通过'
                                        WHEN (FIND_IN_SET(0, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '未审批'
                                        WHEN (FIND_IN_SET(0, faac.results)) THEN '审批中'
                                        WHEN (FIND_IN_SET(2, faac.results) AND !FIND_IN_SET(3, faac.results)) THEN '驳回'
                                        WHEN (FIND_IN_SET(3, faac.results)) THEN '拒绝'
                                        ELSE '未审批' END AS ResultsName

                                        FROM `nxin_qlw_business`.FA_AssetsInspect t1
                                        LEFT JOIN `nxin_qlw_business`.FA_AssetsInspectDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t2.AssetsTypeID=cl.ClassificationID
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON t2.SupplierID=sup.SupplierID
                                        LEFT JOIN `qlw_nxin_com`.ppm_project pro ON t2.ProjectID=pro.ProjectID
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=16
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
LEFT JOIN (
                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
                                        FROM  qlw_nxin_com.faauditrecord faac  
                                        INNER JOIN `nxin_qlw_business`.FA_AssetsInspect temp ON temp.NumericalOrder = faac.NumericalOrder
                                        WHERE faac.PersonID<>-1 
                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t1.NumericalOrder = faac.NumericalOrder

                                        LEFT JOIN `nxin_qlw_business`.biz_datadict dict ON t2.AssetsNatureId=dict.`DataDictID`
                                         WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId}
                                        GROUP BY t2.NumericalOrderDetail
                                        ORDER BY t1.`DataDate` DESC";
            return _context.FA_AssetsInspectListDataSet.FromSqlInterpolated(sql);
        }
        public override IQueryable<FA_AssetsInspectODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
        public override Task<FA_AssetsInspectODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,
                                        po.TicketedPointName,
                                        CONVERT(t1.Number USING utf8mb4) Number,	
                                        CONVERT(t1.MarketID USING utf8mb4) MarketID,
                                        mar.MarketName,
                                        CONVERT(t1.PersonID USING utf8mb4) PersonID,
                                        HP1.Name AS PersonName,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,	
                                        t1.Remarks,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate,
CASE WHEN (FIND_IN_SET(1, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '通过'
                                        WHEN (FIND_IN_SET(0, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '未审批'
                                        WHEN (FIND_IN_SET(0, faac.results)) THEN '审批中'
                                        WHEN (FIND_IN_SET(2, faac.results) AND !FIND_IN_SET(3, faac.results)) THEN '驳回'
                                        WHEN (FIND_IN_SET(3, faac.results)) THEN '拒绝'
                                        ELSE '未审批' END AS ResultsName
                                        FROM `nxin_qlw_business`.FA_AssetsInspect t1
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                        LEFT JOIN qlw_nxin_com.hr_person HP1 ON HP1.PersonID = t1.PersonID
LEFT JOIN (
                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
                                        FROM  qlw_nxin_com.faauditrecord faac  
                                        INNER JOIN `nxin_qlw_business`.FA_AssetsInspect temp ON temp.NumericalOrder = faac.NumericalOrder
                                        WHERE faac.PersonID<>-1 
                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t1.NumericalOrder = faac.NumericalOrder

                                        WHERE t1.NumericalOrder ={manyQuery}";
            var main = _context.FA_AssetsInspectDataSet.FromSqlInterpolated(sql).FirstOrDefault();
            var detail = this.GetDetaiDatasAsync(manyQuery).Result;
            if (detail.Count > 0)
            {
                var relateds = _biz_RelatedRepository.GetRelated(new BIZ_Related()
                {
                    RelatedType = "201610210104402102",
                    ParentType = "2205161654530000309",//资产验收单
                    ParentValue = string.Join(',', detail.Select(s => s.NumericalOrderDetail))
                }).Result;
                if (relateds != null && relateds.Count > 0)
                {
                    detail.ForEach(s =>
                    {
                        s.NumericalOrderInput = relateds.Where(n => n.ParentValue == s.NumericalOrderDetail).FirstOrDefault()?.ChildValue;
                    });
                    main.ApplyForms = string.Join(',', relateds.Where(s => s.ChildType == "2204151105080000109")?.Select(s => s.ChildValue)?.ToArray());
                    main.ContractForms = string.Join(',', relateds.Where(s => s.ChildType == "2204251425130000109")?.Select(s => s.ChildValue)?.ToArray());
                }
                main.Lines = detail;
            }
            return Task.FromResult(main);
        }
        public override Task<List<FA_AssetsInspectDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        t1.AssetsName,	
                                        CONVERT(t1.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                        t1.Specification,
                                        t1.Brand,
                                        CONVERT(t1.AssetsNatureId USING utf8mb4) AssetsNatureId,
                                        dict.DataDictName AS AssetsNatureName,
                                        CONVERT(t1.MeasureUnit USING utf8mb4) MeasureUnit,
                                        un.UnitName AS MeasureUnitName,
                                        t1.Quantity,t1.UnitPrice,t1.Amount,
                                        CONVERT(t1.SupplierID USING utf8mb4) AS SupplierID,
                                        sup.SupplierName,
                                        CONVERT(t1.ProjectID USING utf8mb4) AS ProjectID,
                                        pro.ProjectName,
                                        t1.Remarks,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_AssetsInspectDetail t1
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t1.AssetsTypeID=ClassificationID
                                        LEFT JOIN `qlw_nxin_com`.`unitmeasurement` un ON t1.MeasureUnit=un.UnitID
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON t1.SupplierID=sup.SupplierID
                                        LEFT JOIN `qlw_nxin_com`.ppm_project pro ON t1.ProjectID=pro.ProjectID
                                        LEFT JOIN `nxin_qlw_business`.biz_datadict dict ON t1.AssetsNatureId=dict.`DataDictID`
                                        WHERE t1.NumericalOrder={manyQuery}
                                        GROUP BY t1.NumericalOrderDetail";
            var result = _context.FA_AssetsInspectDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
            return result;
        }


    }
}
