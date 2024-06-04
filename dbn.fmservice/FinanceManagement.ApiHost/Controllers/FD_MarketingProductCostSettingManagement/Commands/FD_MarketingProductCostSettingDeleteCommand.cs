using Architecture.Seedwork.Core;
using MediatR;

namespace FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Commands
{
    public class FD_MarketingProductCostSettingDeleteCommand : IRequest<Result>
    {
        public string NumericalOrder { get; set; }
    }
}
