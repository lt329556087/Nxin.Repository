using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FA_PigAssetsReset;
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
    public class FA_PigAssetsResetODataProvider : OneWithManyQueryProvider<FA_PigAssetsResetODataEntity, FA_PigAssetsResetDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private BIZ_DataDictODataProvider _biz_DataDictODataProvider;
        private IBiz_Related _biz_RelatedRepository;
        public FA_PigAssetsResetODataProvider(IIdentityService identityservice, QlwCrossDbContext context, BIZ_DataDictODataProvider biz_DataDictODataProvider, IBiz_Related biz_RelatedRepository)
        {
            _identityservice = identityservice;
            _context = context;
            _biz_DataDictODataProvider = biz_DataDictODataProvider;
            _biz_RelatedRepository = biz_RelatedRepository;
        }
        public IEnumerable<FA_PigAssetsResetListODataEntity> GetList(ODataQueryOptions<FA_PigAssetsResetListODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }
        public IQueryable<FA_PigAssetsResetListODataEntity> GetData(ODataQueryOptions<FA_PigAssetsResetListODataEntity> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FA_PigAssetsResetListODataEntity> GetMainList(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.Number USING utf8mb4) Number,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,
                                        po.TicketedPointName,
                                        CONVERT(t1.PigfarmNatureID USING utf8mb4) PigfarmNatureID,
                                        dict.DataDictName AS PigfarmNatureName,
                                        CONVERT(t1.PigfarmID USING utf8mb4) PigfarmID,
                                        pig.PigFarmName AS PigfarmName,
                                        t1.PigOriginalValue,
                                        SUM(CASE WHEN t2.ContentType=1 THEN  t2.ResetOriginalValue ELSE 0 END ) AS EquipmentProportion,
                                        SUM(CASE WHEN t2.ContentType=2 THEN  t2.ResetOriginalValue ELSE 0 END) AS HouseProportion,
                                        CONVERT(DATE_FORMAT( t1.BeginAccountPeriodDate,'%Y-%m') USING utf8mb4) BeginAccountPeriodDate,
                                        t1.EndAccountPeriodDate,
                                         CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,	
                                        CASE WHEN (FIND_IN_SET(1, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '通过'
                                        WHEN (FIND_IN_SET(0, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) THEN '未审批'
                                        WHEN (FIND_IN_SET(0, faac.results)) THEN '审批中'
                                        WHEN (FIND_IN_SET(2, faac.results) AND !FIND_IN_SET(3, faac.results)) THEN '驳回'
                                        WHEN (FIND_IN_SET(3, faac.results)) THEN '拒绝'
                                        ELSE '未审批' END AS ResultsName
                                        FROM `nxin_qlw_business`.FA_PigAssetsReset t1
                                        LEFT JOIN `nxin_qlw_business`.FA_PigAssetsResetDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                         LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.biz_datadict dict ON t1.PigfarmNatureID=dict.`DataDictID`
                                        LEFT JOIN `nxin_qlw_zlw_group`.`biz_pigfarm` pig ON t1.PigfarmID=pig.PigfarmID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                        LEFT JOIN (
	                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
	                                        FROM  qlw_nxin_com.faauditrecord faac  
                                            inner join `nxin_qlw_business`.FA_PigAssetsReset temp on temp.NumericalOrder = faac.NumericalOrder
	                                        WHERE faac.PersonID<>-1 
	                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t1.NumericalOrder = faac.NumericalOrder
                                        WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId}
                                        GROUP BY t1.NumericalOrder
                                        ORDER BY t1.`DataDate` DESC";
            return _context.FA_PigAssetsResetListDataSet.FromSqlInterpolated(sql);
        }
        public override IQueryable<FA_PigAssetsResetODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
        public override Task<FA_PigAssetsResetODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.Number USING utf8mb4) Number,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,
                                        po.TicketedPointName,
                                        CONVERT(t1.PigfarmNatureID USING utf8mb4) PigfarmNatureID,
                                        dict.DataDictName AS PigfarmNatureName,
                                        CONVERT(t1.PigfarmID USING utf8mb4) PigfarmID,
                                        pig.PigFarmName as PigfarmName,
                                        CONVERT(t1.PigNumberTypeId USING utf8mb4) PigNumberTypeId,
                                        dict1.DataDictName AS PigNumberTypeName,
                                        t1.PigNumber,
                                        t1.PigPrice,
                                        t1.PigOriginalValue,
                                        CONVERT(DATE_FORMAT( t1.BeginAccountPeriodDate,'%Y-%m') USING utf8mb4) BeginAccountPeriodDate,
                                        t1.EndAccountPeriodDate,
                                        CONVERT(DATE_FORMAT( t1.ResetOriginalValueDate,'%Y-%m') USING utf8mb4) ResetOriginalValueDate,
                                        CONVERT(t1.ResetOriginalValueType USING utf8mb4) ResetOriginalValueType,
                                        dict2.DataDictName AS ResetOriginalValueTypeName,
                                        t1.EquipmentProportion,
                                        t1.HouseProportion,
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
                                        FROM `nxin_qlw_business`.FA_PigAssetsReset t1
                                        LEFT JOIN `nxin_qlw_zlw_group`.`biz_pigfarm` pig ON t1.PigfarmID=pig.PigfarmID
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                        LEFT JOIN `nxin_qlw_business`.biz_datadict dict ON t1.PigfarmNatureID=dict.`DataDictID` AND dict.`PID`=2204251432480000150
                                        LEFT JOIN `nxin_qlw_business`.biz_datadict dict1 ON t1.PigNumberTypeId=dict1.`DataDictID` AND dict1.`PID`=2204251504240000150
                                        LEFT JOIN `nxin_qlw_business`.biz_datadict dict2 ON t1.ResetOriginalValueType=dict2.`DataDictID` AND dict2.`PID` IN (2204291556090000150,2204291655220000150)
                                       LEFT JOIN (
	                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
	                                        FROM  qlw_nxin_com.faauditrecord faac  
                                            inner join `nxin_qlw_business`.FA_PigAssetsReset temp on temp.NumericalOrder = faac.NumericalOrder
	                                        WHERE faac.PersonID<>-1 
	                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t1.NumericalOrder = faac.NumericalOrder
                                        WHERE t1.NumericalOrder={manyQuery}
                                        GROUP BY t1.NumericalOrder";
            var main = _context.FA_PigAssetsResetDataSet.FromSqlInterpolated(sql).FirstOrDefault();
            var detail = this.GetDetaiDatasAsync(manyQuery).Result;
            if (detail.Count > 0)
            {
                var relateds = _biz_RelatedRepository.GetRelated(new BIZ_Related()
                {
                    RelatedType = "201610210104402102",
                    ParentType = "2205261535030000109",//资产验收单
                    ParentValue = string.Join(',', detail.Select(s => s.NumericalOrderDetail))
                }).Result;
                if (relateds != null && relateds.Count > 0)
                {
                    detail.ForEach(s =>
                    {
                        s.NumericalOrderInput = relateds.Where(n => n.ParentValue == s.NumericalOrderDetail).FirstOrDefault()?.ChildValue;
                    });
                }
                main.Lines = detail;
            }
            return Task.FromResult(main);
        }
        public override Task<List<FA_PigAssetsResetDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        t1.AssetsCode,
                                        CONVERT(t1.InspectNumber USING utf8mb4) InspectNumber,
                                        t1.AssetsName,	
                                        CONVERT(t1.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                        t1.Specification,
                                        t1.Brand,
                                        CONVERT(t1.MeasureUnit USING utf8mb4) MeasureUnit,
                                        un.UnitName AS MeasureUnitName,
                                        CONVERT(t1.MarketID USING utf8mb4) MarketID,
                                        mar.`MarketName` AS MarketName,
                                        t1.OriginalValue,t1.NetValue,t1.OriginalUseYear,
                                        t1.ResetBase,t1.ResetUseYear,t1.ResetOriginalValue,t1.ContentType,
                                        t1.Remarks,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_PigAssetsResetDetail t1
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t1.AssetsTypeID=ClassificationID
                                        LEFT JOIN `qlw_nxin_com`.`unitmeasurement` un ON t1.MeasureUnit=un.UnitID
                                         LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        WHERE t1.NumericalOrder={manyQuery}
                                        GROUP BY t1.NumericalOrderDetail";
            var result = _context.FA_PigAssetsResetDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
            return result;
        }
        public IQueryable<FA_AssetsInspectMobileODataEntity> GetAssetsInspectMobileAsync(ODataQueryOptions<FA_AssetsInspectMobileODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@"SELECT CONVERT(t2.NumericalOrder USING utf8mb4) NumericalOrder,	
                                    CONVERT(t2.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                    CONVERT(t1.Number USING utf8mb4) Number,
                                    CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,
                                    po.TicketedPointName,
                                    t2.AssetsName,	
                                    CONVERT(t2.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                    cl.ClassificationName AS AssetsTypeName,
                                    ifnull(cl.ResetFixedYears,0) as ResetFixedYears,
                                    t2.Specification,
                                    t2.Brand,
                                    CONVERT(t2.AssetsNatureId USING utf8mb4) AssetsNatureId,
                                    dict.DataDictName AS AssetsNatureName,
                                    CONVERT(t2.MeasureUnit USING utf8mb4) MeasureUnit,
                                    un.UnitName AS MeasureUnitName,
                                    t2.Quantity,t2.UnitPrice,t2.Amount,
                                    CONVERT(t1.MarketID USING utf8mb4) MarketID,
                                    mar.MarketName,
                                    CONVERT(t2.SupplierID USING utf8mb4) AS SupplierID,
                                    sup.SupplierName,
                                    CONVERT(t2.ProjectID USING utf8mb4) AS ProjectID,
                                    pro.ProjectName
                                    FROM `nxin_qlw_business`.FA_AssetsInspect t1
                                    LEFT JOIN `nxin_qlw_business`.FA_AssetsInspectDetail t2 ON t2.`NumericalOrder`=t2.`NumericalOrder`
                                    LEFT JOIN `nxin_qlw_business`.`biz_related` re1 ON t2.NumericalOrderDetail=re1.`ChildValue` AND re1.`RelatedType`=201610210104402102 AND re1.`ParentType`=2205261535030000109 AND re1.`ChildType`=2205161654530000309
                                    LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                    LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t2.AssetsTypeID=ClassificationID
                                    LEFT JOIN `qlw_nxin_com`.`unitmeasurement` un ON t2.MeasureUnit=un.UnitID
                                    LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` po ON t1.`TicketedPointID`=po.TicketedPointID
                                    LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON t2.SupplierID=sup.SupplierID
                                    LEFT JOIN `qlw_nxin_com`.ppm_project pro ON t2.ProjectID=pro.ProjectID
                                    LEFT JOIN `nxin_qlw_business`.biz_datadict dict ON t2.AssetsNatureId=dict.`DataDictID`
                                    WHERE  re1.ChildValue IS NULL AND  t2.AssetsNatureId IN (2204241600080000153,2204241600080000154) AND   t1.EnterpriseID = {_identityservice.EnterpriseId}
                                    GROUP BY t2.NumericalOrderDetail
                                    ORDER BY t1.`DataDate` DESC";
            var result = _context.FA_AssetsInspectMobileDataSet.FromSqlInterpolated(sql);
            return result;
        }

        public IQueryable<FA_AssetsCardMobileODataEntity> GetAssetsCardMobileAsync(ODataQueryOptions<FA_AssetsCardMobileODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@"SELECT 
CONVERT(fa.`CardID` USING utf8mb4) NumericalOrder,
	fa.`CardCode`, /* 卡片编码*/
	fa.`AssetsCode`,/* 资产编码*/
	fa.`AssetsName`,/* 资产名称*/
	CONVERT(fad.`ClassificationID` USING utf8mb4) AS AssetsTypeID,/*资产类别*/
	ff.`ClassificationName` as AssetsTypeName,/*资产类别名称*/
	fa.`Specification`,/* 规格型号*/
	CONVERT(DATE_FORMAT( fa.StartDate,'%Y-%m-%d') USING utf8mb4) as StartDate,/* 开始使用日期*/
	ifnull(fad.`OriginalValue`,0) as OriginalValue,/*原值*/
	IFNULL(fad.NetValue,0) as NetValue,/*净值*/
	IFNULL(fad.DepreciationAccumulated,0) as DepreciationAccumulated,/*累计折旧*/
	IFNULL(fad.ResidualValue,0) as ResidualValue,/*净残值*/
	IFNULL(fad.UseMonth,0) as UseMonth,/*使用年限*/
IFNULL(fad.Quantity,0) as Quantity,/*数量*/
	CONVERT(fa.`MeasureUnit` USING utf8mb4) as MeasureUnit,/*计量单位*/
	um.`UnitName` MeasureUnitName,/*计量单位名称*/
	CONVERT(mar.`MarketID` USING utf8mb4) as MarketID,
	mar.`MarketName`,
    IFNULL(ff.ResetFixedYears,0) as ResetFixedYears
FROM `nxin_qlw_business`.`fa_assetscard` fa
 LEFT JOIN `nxin_qlw_business`.`biz_related` re1 ON fa.`CardID`=re1.`ChildValue` AND re1.`RelatedType`=201610210104402102 AND re1.`ParentType`=2205261535030000109 AND re1.`ChildType`=1712201440170000101
LEFT JOIN (SELECT A.* FROM nxin_qlw_business.`fa_assetscarddetail` A
	INNER JOIN (SELECT B.CardID,MAX(DataDate) DataDate,B.`CardDetailID` FROM nxin_qlw_business.FA_AssetsCardDetail B GROUP BY B.CardID ) B ON   A.`CardDetailID`=B.CardDetailID
	INNER JOIN `nxin_qlw_business`.`fa_assetscard` C ON A.`CardID`=C.`CardID` WHERE C.`EnterpriseID`={_identityservice.EnterpriseId}) fad ON fa.CardID=fad.CardID 
LEFT JOIN `qlw_nxin_com`.`unitmeasurement` um ON um.`UnitID`=fa.`MeasureUnit`
left join 
(
  select CardID,DepartmentID from nxin_qlw_business.`fa_assetscardextend` group by CardID
) temp  on fa.`CardID`=temp.CardID
left join `qlw_nxin_com`.`biz_market` mar on mar.`MarketID`=temp.DepartmentID
LEFT JOIN `nxin_qlw_business`.`fa_assetsclassification` ff ON ff.`ClassificationID`=fad.`ClassificationID`
WHERE re1.ChildValue IS NULL  and fa.IsPigReset in (1,2) and  fa.`EnterpriseID`={_identityservice.EnterpriseId}";
            var result = _context.FA_AssetsCardMobileDataSet.FromSqlInterpolated(sql);
            return result;
        }

    }
}
