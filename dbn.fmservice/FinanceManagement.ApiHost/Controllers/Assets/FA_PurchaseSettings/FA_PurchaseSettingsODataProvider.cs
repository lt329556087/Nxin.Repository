using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FA_PurchaseSettings;
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
    public class FA_PurchaseSettingsODataProvider : OneWithManyQueryProvider<FA_PurchaseSettingsODataEntity, FA_PurchaseSettingsDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private BIZ_DataDictODataProvider _biz_DataDictODataProvider;
        private IBiz_Related _biz_RelatedRepository;

        public FA_PurchaseSettingsODataProvider(IIdentityService identityservice, QlwCrossDbContext context, BIZ_DataDictODataProvider biz_DataDictODataProvider, IBiz_Related biz_RelatedRepository)
        {
            _identityservice = identityservice;
            _context = context;
            _biz_DataDictODataProvider = biz_DataDictODataProvider;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public IEnumerable<FA_PurchaseSettingsODataEntity> GetList(ODataQueryOptions<FA_PurchaseSettingsODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FA_PurchaseSettingsODataEntity> GetData(ODataQueryOptions<FA_PurchaseSettingsODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }



        public override Task<FA_PurchaseSettingsODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.ModifyFieldID USING utf8mb4) ModifyFieldID,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP1.Name AS OwnerName,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_PurchaseSettings t1
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        WHERE t1.EnterpriseID={_identityservice.EnterpriseId}";
            var main = _context.FA_PurchaseSettingsDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
            var relateds = _biz_RelatedRepository.GetRelated(new BIZ_Related()
            {
                RelatedType = "201610210104402102",
                ParentType = "2205121429180000109",//资产购置设置Appid
                ParentValue = main.Result?.ModifyFieldID,
                ChildType = "2202271716580000115"
            });
            if (relateds.Result.Count > 0)
            {
                var dropList = _biz_DataDictODataProvider.GetDataDictAsync("2202271716580000115").Result;//获取修改字典下拉数据
                main.Result.ModifyFieldList = (from s in dropList
                                               join o in relateds.Result on s.DataDictID equals o.ChildValue
                                               select s).ToList();
            }
            var detail = this.GetDetaiDatasAsync(Convert.ToInt64(main.Result?.NumericalOrder)).Result;
            if (detail.Count > 0)
            {
                main.Result.Lines = detail;
            }
            return main;
        }
        public override Task<List<FA_PurchaseSettingsDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,	
                                        CONVERT(t1.AssetsTypeID USING utf8mb4) AssetsTypeID,
                                        t1.BeginRange,
                                        t1.EndRange,
                                        CONVERT(t1.FloatingDirectionID USING utf8mb4) AS FloatingDirectionID,
                                        dict1.DataDictName AS FloatingDirectionName,
                                        CONVERT(t1.FloatingTypeID USING utf8mb4) AS FloatingTypeID,
                                        dict2.DataDictName AS FloatingTypeName,
                                        t1.MaxFloatingValue,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_PurchaseSettingsDetail t1
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict1 ON t1.FloatingDirectionID=dict1.DataDictID  AND dict1.PID=2003031357470000115
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict2 ON t1.FloatingTypeID=dict2.DataDictID  AND dict2.PID=2003031357470000125
                                        WHERE t1.NumericalOrder={manyQuery} ";
            var result = _context.FA_PurchaseSettingsDetailDataSet.FromSqlInterpolated(sql).ToList();


            FormattableString assets = $@"SELECT CONVERT(t1.ClassificationID USING utf8mb4) ClassificationID,t1.ClassificationName,t1.ClassificationCode,
	CONVERT(t1.PID USING utf8mb4) PID,t1.Rank,t1.FixedYears,ifnull(t1.ResetFixedYears,0) as ResetFixedYears,t1.ResidualValue,
	CONVERT(t1.DepreciationMethodID USING utf8mb4) DepreciationMethodID,t1.`CodeRule`,CONVERT(ifnull(t1.AccruedRule,0) USING utf8mb4) AccruedRule,
	CONVERT(ifnull(t1.AssetsAccoSubjectID,0) USING utf8mb4) AssetsAccoSubjectID,CONVERT(ifnull(t1.DepreciationAccoSubjectID,0) USING utf8mb4) DepreciationAccoSubjectID,
	t1.Remarks,CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID
                                        FROM nxin_qlw_business.fa_assetsclassification t1
                                        WHERE t1.EnterpriseID={_identityservice.GroupId}";
            var dropList = _context.FA_AssetsClassificationDataSet.FromSqlInterpolated(assets).ToList();
            if (dropList?.Count > 0)
            {
                result.ForEach(s =>
                {
                    var relateds = _biz_RelatedRepository.GetRelated(new BIZ_Related()
                    {
                        RelatedType = "201610210104402102",
                        ParentType = "2205121429180000109",//资产购置设置Appid
                        ParentValue = s?.AssetsTypeID,
                        ChildType = "1712071843500000101"
                    }).Result;
                    if (s != null)
                    {
                        s.AssetsTypeList = (from d in dropList
                                            join o in relateds on d?.ClassificationID equals o?.ChildValue
                                            select new AssetsTypeModel()
                                            {
                                                AssetsTypeID = d.ClassificationID,
                                                AssetsTypeName = d.ClassificationName
                                            }).ToList();
                    }
                });
            }
            return Task.FromResult(result);
        }
        public override IQueryable<FA_PurchaseSettingsODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
    }
}
