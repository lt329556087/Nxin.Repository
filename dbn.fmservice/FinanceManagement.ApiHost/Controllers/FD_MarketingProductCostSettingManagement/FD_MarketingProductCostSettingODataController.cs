using FinanceManagement.ApiHost.Controllers.FM_CostProject;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinanceManagement.Domain.MarketingProductCostSettingManagement;

namespace FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement
{
    /// <summary>
    /// 营销商品成本设定 OData
    /// </summary>
    [Authorize]
    public class FD_MarketingProductCostSettingODataController : ODataController
    {
        private readonly FD_MarketingProductCostSettingODataProvider _queryProvider;

        public FD_MarketingProductCostSettingODataController(FD_MarketingProductCostSettingODataProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        [HttpGet]
        [EnableQuery(EnsureStableOrdering =false)]
        public IQueryable<FD_MarketingProductCostSettingListQueryModel> Get()
        {
            return _queryProvider.GetList();
        }
    }
}
