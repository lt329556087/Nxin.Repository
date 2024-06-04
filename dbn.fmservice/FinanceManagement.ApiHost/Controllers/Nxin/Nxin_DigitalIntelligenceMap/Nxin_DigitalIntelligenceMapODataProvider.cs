using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.Nxin_DigitalIntelligenceMap;
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
    public class Nxin_DigitalIntelligenceMapODataProvider
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private BIZ_DataDictODataProvider _biz_DataDictODataProvider;
        private IBiz_Related _biz_RelatedRepository;

        public Nxin_DigitalIntelligenceMapODataProvider(IIdentityService identityservice, QlwCrossDbContext context, BIZ_DataDictODataProvider biz_DataDictODataProvider, IBiz_Related biz_RelatedRepository)
        {
            _identityservice = identityservice;
            _context = context;
            _biz_DataDictODataProvider = biz_DataDictODataProvider;
            _biz_RelatedRepository = biz_RelatedRepository;
        }
        public Task< List<Nxin_DigitalIntelligenceMapODataEntity>> Get(Nxin_DigitalIntelligenceMapCommand model)
        {
            string wheresql = string.Empty;
            if (!string.IsNullOrEmpty(model.NumericalOrder) )
            {
                wheresql += " and  NumericalOrder=" + model.NumericalOrder;
            }
            if (!string.IsNullOrEmpty(model.MapType))
            {
                wheresql += " and  MapType=" + model.MapType;
            }
            if (!string.IsNullOrEmpty(model.GroupID))
            {
                wheresql += " and  t1.GroupID = "+ model.GroupID;
            }
            string sql = $@"SELECT CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                            CONVERT(t1.GroupID USING utf8mb4) GroupID,
                            ent.EnterpriseName AS 	GroupName,
                            CONVERT(t1.MapType USING utf8mb4) MapType,	
                            t1.BackgroundValue,
                            t1.BlockList,
                            CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                            HP.Name AS OwnerName,
                            t1.Remarks,
                            CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                            CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                            FROM `nxin_qlw_business`.Nxin_DigitalIntelligenceMap t1
                            LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = t1.OwnerID
                            LEFT JOIN qlw_nxin_com.biz_enterprise ent ON t1.GroupID=ent.EnterpriseID
                            WHERE  1=1 "+wheresql;
            return _context.Nxin_DigitalIntelligenceMapDataSet.FromSqlRaw(sql).ToListAsync();
        }
    }
}
