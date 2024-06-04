using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FA_AssetsMaintain;
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
    public class FA_AssetsMaintainODataProvider : OneWithManyQueryProvider<FA_AssetsMaintainODataEntity, FA_AssetsMaintainDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private BIZ_DataDictODataProvider _biz_DataDictODataProvider;
        private IBiz_Related _biz_RelatedRepository;

        public FA_AssetsMaintainODataProvider(IIdentityService identityservice, QlwCrossDbContext context, BIZ_DataDictODataProvider biz_DataDictODataProvider, IBiz_Related biz_RelatedRepository)
        {
            _identityservice = identityservice;
            _context = context;
            _biz_DataDictODataProvider = biz_DataDictODataProvider;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public IEnumerable<FA_AssetsMaintainListODataEntity> GetList(ODataQueryOptions<FA_AssetsMaintainListODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }
        public IQueryable<FA_AssetsMaintainListODataEntity> GetData(ODataQueryOptions<FA_AssetsMaintainListODataEntity> odataqueryoptions, Uri uri)
        {
            return GetMainList();
        }
        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FA_AssetsMaintainListODataEntity> GetMainList(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t2.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.Number USING utf8mb4) Number,	
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,
                                        t2.AssetsName,	 t2.AssetsCode,	
                                        CONVERT(t2.MaintainID USING utf8mb4) MaintainID,
                                        cl.DataDictName AS MaintainName,
                                        CONVERT(DATE_FORMAT( t2.MaintainDate,'%Y-%m-%d %H:%i') USING utf8mb4) MaintainDate,
                                        t2.Content,
                                        t2.Amount,
                                        CONVERT(t2.DepositID USING utf8mb4) DepositID,
                                        fix.`FAPlaceName` AS DepositName,
                                        t2.FileName,t2.FilePath,
                                        CONVERT(t2.PersonID USING utf8mb4) AS PersonID,
                                        HP1.Name AS PersonName
                                        FROM `nxin_qlw_business`.FA_AssetsMaintain t1
                                        LEFT JOIN `nxin_qlw_business`.FA_AssetsMaintainDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN nxin_qlw_business.biz_datadict  cl ON t2.MaintainID=DataDictID
                                        left join qlw_nxin_com.hr_person HP1 ON HP1.PersonID = t2.PersonID
                                        LEFT JOIN `nxin_qlw_business`.fa_fixedassetsplace fix ON t2.DepositID=fix.FAPlaceID
                                        WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId}
                                        GROUP BY t2.NumericalOrderDetail
                                        ORDER BY t1.`DataDate` DESC";
            return _context.FA_AssetsMaintainListDataSet.FromSqlInterpolated(sql);
        }
        public override IQueryable<FA_AssetsMaintainODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
        public override Task<FA_AssetsMaintainODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(DATE_FORMAT( t1.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(t1.Number USING utf8mb4) Number,	
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP.Name AS OwnerName,	
                                        t1.Remarks,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_AssetsMaintain t1
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        WHERE t1.NumericalOrder ={manyQuery}";
            var main = _context.FA_AssetsMaintainDataSet.FromSqlInterpolated(sql).FirstOrDefault();
            var detail = this.GetDetaiDatasAsync(manyQuery).Result;
            if (detail.Count > 0)
            {
                main.Lines = detail;
            }
            return Task.FromResult(main);
        }
        public override Task<List<FA_AssetsMaintainDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        t1.AssetsName,	 t1.AssetsCode,	
                                        CONVERT(t1.CardID USING utf8mb4) CardID,
                                        CONVERT(t1.MaintainID USING utf8mb4) MaintainID,
                                        cl.DataDictName AS MaintainName,
                                        CONVERT(DATE_FORMAT( t1.MaintainDate,'%Y-%m-%d %H:%i') USING utf8mb4) MaintainDate,
                                        t1.Content,
                                        t1.Amount,
                                        CONVERT(t1.DepositID USING utf8mb4) DepositID,
                                        fix.`FAPlaceName` AS DepositName,
                                        t1.FileName,t1.FilePath,
                                        CONVERT(t1.PersonID USING utf8mb4) AS PersonID,
                                        HP.Name AS PersonName,
                                        t1.Remarks,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FA_AssetsMaintainDetail t1
                                        LEFT JOIN nxin_qlw_business.biz_datadict  cl ON t1.MaintainID=DataDictID
                                        left join qlw_nxin_com.hr_person HP ON HP.PersonID = t1.PersonID
                                        LEFT JOIN `nxin_qlw_business`.fa_fixedassetsplace fix ON t1.DepositID=fix.FAPlaceID
                                        WHERE t1.NumericalOrder={manyQuery}
                                        GROUP BY t1.NumericalOrderDetail";
            var result = _context.FA_AssetsMaintainDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
            result.Result.ForEach(s =>
            {
                if (!string.IsNullOrEmpty(s.FileName) && !string.IsNullOrEmpty(s.FilePath))
                {
                    var filenames = s.FileName.Split(',');
                    var filepaths = s.FilePath.Split(',');
                    s.FileModels = new List<Infrastructure.QlwCrossDbEntities.FileModel>();
                    for (int i = 0; i < filenames.Length; i++)
                    {
                        s.FileModels.Add(new Infrastructure.QlwCrossDbEntities.FileModel()
                        {
                            FileName = filenames[i],
                            FilePath = filepaths[i],
                        }); ;
                    }
                }
            });
            return result;
        }

    }
}
