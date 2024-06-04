using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManagement.Util;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Domain;

namespace FinanceManagement.ApiHost.Controllers.OData
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AuxiliaryODataController : ODataController
    {
        FD_AuxiliaryODataProvider _prodiver;
        //private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        HostConfiguration _hostCongfiguration;
        public FD_AuxiliaryODataController(FD_AuxiliaryODataProvider prodiver, IIdentityService identityService, IFD_BadDebtProvisionRepository iProvisionRepository, HostConfiguration hostCongfiguration)
        {
            _prodiver = prodiver;
            _iProvisionRepository = iProvisionRepository;
            _identityService = identityService;
            _hostCongfiguration = hostCongfiguration;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public List<dynamic> Get(string TypeId, string GroupId)
        {
            var list = _prodiver.GetList(GroupId)?.ToList();
            if(list==null)return new List<dynamic>();
            if (!string.IsNullOrEmpty(TypeId))
            {
                return list.Where(m => m.ProjectType == TypeId && m.IsUse == "1").ToList();
            }
            return list.Where(m => m.IsUse == "1").ToList();
        }

        #endregion 

    }
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AuxiliaryTypeODataController : ODataController
    {
        FD_AuxiliaryODataProvider _prodiver;
        //private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        HostConfiguration _hostCongfiguration;
        public FD_AuxiliaryTypeODataController(FD_AuxiliaryODataProvider prodiver, IIdentityService identityService, IFD_BadDebtProvisionRepository iProvisionRepository, HostConfiguration hostCongfiguration)
        {
            _prodiver = prodiver;
            _iProvisionRepository = iProvisionRepository;
            _identityService = identityService;
            _hostCongfiguration = hostCongfiguration;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public List<dynamic> Get(string TypeId,string GroupId)
        {
            var list = _prodiver.GetTypeList(GroupId)?.ToList();
            if (list == null) return new List<dynamic>();
            if (!string.IsNullOrEmpty(TypeId))
            {
                return list.Where(m => m.TypeTag == TypeId).ToList();
            }
            return list;
        }
        #endregion 

    }
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AuxiliaryUnAuthorizeODataController : ODataController
    {
        FD_AuxiliaryODataProvider _prodiver;
        //private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        HostConfiguration _hostCongfiguration;
        public FD_AuxiliaryUnAuthorizeODataController(FD_AuxiliaryODataProvider prodiver, IIdentityService identityService, IFD_BadDebtProvisionRepository iProvisionRepository, HostConfiguration hostCongfiguration)
        {
            _prodiver = prodiver;
            _iProvisionRepository = iProvisionRepository;
            _identityService = identityService;
            _hostCongfiguration = hostCongfiguration;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public List<dynamic> Get(string TypeId, string GroupId)
        {
            var list = _prodiver.GetList(GroupId)?.ToList();
            if (list == null) return new List<dynamic>();
            if (!string.IsNullOrEmpty(TypeId))
            {
                list = list.Where(m => m.ProjectType == TypeId).OrderBy(m=>m.ProjectCode).ToList();
            }
            return list;
        }
        #endregion 

    }
}
