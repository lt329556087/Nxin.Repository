using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FA_PigAssetsAccrual;
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
    public class FA_PigAssetsAccrualODataProvider : OneWithManyQueryProvider<FA_PigAssetsAccrualODataEntity, FA_PigAssetsAccrualDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private BIZ_DataDictODataProvider _biz_DataDictODataProvider;
        private IBiz_Related _biz_RelatedRepository;
        public FA_PigAssetsAccrualODataProvider(IIdentityService identityservice, QlwCrossDbContext context, BIZ_DataDictODataProvider biz_DataDictODataProvider, IBiz_Related biz_RelatedRepository)
        {
            _identityservice = identityservice;
            _context = context;
            _biz_DataDictODataProvider = biz_DataDictODataProvider;
            _biz_RelatedRepository = biz_RelatedRepository;
        }
        public IEnumerable<FA_PigAssetsAccrualListODataEntity> GetList(ODataQueryOptions<FA_PigAssetsAccrualListODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }
        public IQueryable<FA_PigAssetsAccrualListODataEntity> GetData(ODataQueryOptions<FA_PigAssetsAccrualListODataEntity> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FA_PigAssetsAccrualListODataEntity> GetMainList(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
        public override IQueryable<FA_PigAssetsAccrualODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
        public override Task<FA_PigAssetsAccrualODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
        public FA_PigAssetsAccrualODataEntity GetData(long manyQuery = 0, string dataDate = "")
        {
            string sqlwhere = string.Empty;
            if (manyQuery != 0)
            {
                sqlwhere += $@" and t1.`NumericalOrder`={manyQuery} ";
            }
            if (!string.IsNullOrEmpty(dataDate))
            {
                sqlwhere += $@" and DATE_FORMAT(t1.DataDate,'%Y-%m-%d')='{dataDate}' ";
            }
            string sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,	
                                        t1.Remarks,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_PigAssetsAccrual t1
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        WHERE   t1.EnterpriseID={_identityservice.EnterpriseId}  {sqlwhere}
                                        GROUP BY t1.NumericalOrder";
            var main = _context.FA_PigAssetsAccrualDataSet.FromSqlRaw(sql).FirstOrDefault();
            if (main!=null)
            {
                var detail = this.GetDetaiDatasAsync(Convert.ToInt64(main.NumericalOrder)).Result;
                main.Lines = detail;
            }
            return main;
        }
        public override Task<List<FA_PigAssetsAccrualDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        CONVERT(t1.NumericalOrderInput USING utf8mb4) NumericalOrderInput,
                                        CONVERT(t1.PigfarmNatureID USING utf8mb4) PigfarmNatureID,
                                        dict.DataDictName AS PigfarmNatureName,
                                        CONVERT(t1.PigfarmID USING utf8mb4) PigfarmID,
                                        pig.PigFarmName AS PigfarmName,
                                        CONVERT(t1.MarketID USING utf8mb4) MarketID,
                                        mar.`MarketName` AS MarketName,
                                        t1.`CardCode`,t1.`AssetsCode`,
                                        t1.AssetsName,
                                        CONVERT(t1.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                        t1.OriginalValue,t1.DepreciationMonthAmount,t1.DepreciationMonthRate,
                                        t1.DepreciationAccumulated,t1.NetValue,t1.UseMonth,t1.AlreadyAccruedMonth,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_PigAssetsAccrualDetail t1
                                        LEFT JOIN `nxin_qlw_zlw`.biz_pigfarm pig ON t1.PigfarmID=pig.PigfarmID
                                        LEFT JOIN `nxin_qlw_business`.biz_datadict dict ON t1.PigfarmNatureID=dict.`DataDictID` AND dict.`PID`=2204251432480000150
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t1.AssetsTypeID=ClassificationID
                                        LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        WHERE t1.NumericalOrder={manyQuery}
                                        GROUP BY t1.NumericalOrderDetail";
            var result = _context.FA_PigAssetsAccrualDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
            return result;
        }

        public IQueryable<FA_PigAssetsResetByAccrualODataEntity> GetPigAssetsResetByAccrual(string beginDate, string endDate)
        {
            string sql = @$"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        IFNULL(fa.`CardCode`,temp.CardCode)  AS CardCode, /* 卡片编码*/
					                    IFNULL(fa.`AssetsCode`,temp.AssetsCode) AS AssetsCode,/* 资产编码*/
                                        CONVERT(t1.InspectNumber USING utf8mb4) InspectNumber,
                                        t1.AssetsName,	
                                        CONVERT(t1.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        cl.ClassificationName AS AssetsTypeName,
                                         CONVERT(t2.PigfarmNatureID USING utf8mb4) PigfarmNatureID,
                                        dict.DataDictName AS PigfarmNatureName,
                                        CONVERT(t2.PigfarmID USING utf8mb4) PigfarmID,
                                        pig.PigFarmName AS PigfarmName,
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
                                        LEFT JOIN `nxin_qlw_business`.FA_PigAssetsReset t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_zlw`.biz_pigfarm pig ON t2.PigfarmID=pig.PigfarmID
                                        LEFT JOIN `nxin_qlw_business`.biz_datadict dict ON t2.PigfarmNatureID=dict.`DataDictID` AND dict.`PID`=2204251432480000150
                                        LEFT JOIN nxin_qlw_business.fa_assetsclassification  cl ON t1.AssetsTypeID=ClassificationID
                                        LEFT JOIN `qlw_nxin_com`.`unitmeasurement` un ON t1.MeasureUnit=un.UnitID
                                        LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON t1.MarketID=mar.MarketID
                                        LEFT JOIN `nxin_qlw_business`.`biz_related` re1 ON t1.`NumericalOrderDetail`=re1.ParentValue AND re1.RelatedType=201610210104402102 AND re1.ParentType = 2205261535030000109 AND re1.ChildType=1712201440170000101  #关联资产卡片
                                        LEFT JOIN `nxin_qlw_business`.`fa_assetscard` fa ON re1.`ChildValue`=fa.`CardID`
                                        LEFT JOIN `nxin_qlw_business`.`biz_related` re2 ON t1.`NumericalOrderDetail`=re2.ParentValue AND re2.RelatedType=201610210104402102 AND re2.ParentType = 2205261535030000109 AND re2.ChildType=2205161654530000309  #关联资产验收单
                                        LEFT JOIN 
                                        (
						SELECT CONVERT(ins.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,fa.`CardCode`,fa.`AssetsCode` 
						FROM `nxin_qlw_business`.FA_AssetsInspectDetail ins 
						LEFT JOIN `nxin_qlw_business`.`biz_related` re3 ON ins.`NumericalOrderDetail`=re3.ParentValue AND re3.RelatedType=201610210104402122 AND re3.ParentType = 2205161654530000309 AND re3.ChildType=2204081706020000109  #关联新增资产
					        LEFT JOIN `nxin_qlw_business`.`fa_assetscard` fa ON re3.`ChildValue`=fa.`CardID`
					        WHERE fa.`EnterpriseID`={_identityservice.EnterpriseId}
					        GROUP BY ins.NumericalOrderDetail
                                        ) AS temp  ON re2.`ChildValue`=temp.NumericalOrderDetail
                                        LEFT JOIN (
	                                        SELECT faac.NumericalOrder,GROUP_CONCAT(DISTINCT faac.results) results 
	                                        FROM  qlw_nxin_com.faauditrecord faac  
                                            inner join `nxin_qlw_business`.FA_PigAssetsReset temp on temp.NumericalOrder = faac.NumericalOrder
	                                        WHERE faac.PersonID<>-1 
	                                        GROUP BY faac.NumericalOrder
                                        ) faac ON t2.NumericalOrder = faac.NumericalOrder
                                        
                                         WHERE t2.`EnterpriseID`={_identityservice.EnterpriseId} 
                                          AND  t2.`BeginAccountPeriodDate` BETWEEN '{beginDate}' AND '{endDate}'
                                         # and (FIND_IN_SET(1, faac.results) AND (LENGTH(results) - LENGTH(REPLACE( results,',','' ))) = 0) 
                                        GROUP BY t1.NumericalOrderDetail ";
            return _context.FA_PigAssetsResetByAccrualDataSet.FromSqlRaw(sql);
        }
        public Task<List<FA_PigAssetsAccrualODataEntity>> GetMains()
        {
            string sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,	
                                        t1.Remarks,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_PigAssetsAccrual t1
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        WHERE  t1.`EnterpriseID`={_identityservice.EnterpriseId}   
                                        GROUP BY t1.NumericalOrder";
            var mains = _context.FA_PigAssetsAccrualDataSet.FromSqlRaw(sql).ToListAsync();
            return mains;
        }

    }
}
