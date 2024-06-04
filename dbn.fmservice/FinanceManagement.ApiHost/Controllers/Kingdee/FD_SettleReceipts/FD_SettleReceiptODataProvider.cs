using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceipt;
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
    public class FD_SettleReceiptODataProvider 
    {
        ILogger<FD_SettleReceiptODataProvider> _logger;
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        public FD_SettleReceiptODataProvider(IIdentityService identityservice, QlwCrossDbContext context, TreeModelODataProvider treeModel, ILogger<FD_SettleReceiptODataProvider> logger)
        {
            _identityservice = identityservice;
            _context = context;
            _logger = logger;
            _treeModel = treeModel;
        }

    }
}
