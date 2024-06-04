using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class FD_AuxiliaryODataProvider 
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_AuxiliaryODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<dynamic> GetList(string GroupId)
        {
            var datas = _context.DynamicSqlQuery($@" SELECT * FROM nxin_qlw_business.fd_auxiliaryproject where GroupId =  {(string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId)};").AsEnumerable();
            return datas;
        }
        public IEnumerable<dynamic> GetTypeList(string GroupId)
        {
            var datas = _context.DynamicSqlQuery($@" SELECT * FROM nxin_qlw_business.fd_auxiliarytype where GroupId = {(string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId)};").AsEnumerable();
            return datas;
        }
        public List<dynamic> IsExistSettle(fd_auxiliaryproject model)
        {
            var  list1 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary1 = {model.ProjectId} ");
            if (list1.Count > 0)
            {
                return list1;
            }
            var list2 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary2 = {model.ProjectId} ");
            if (list2.Count > 0)
            {
                return list2;
            }
            var list3 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary3 = {model.ProjectId} ");
            if (list3.Count > 0)
            {
                return list3;
            }
            var list4 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary4 = {model.ProjectId} ");
            if (list4.Count > 0)
            {
                return list4;
            }
            var list5 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary5 = {model.ProjectId} ");
            if (list5.Count > 0)
            {
                return list5;
            }
            var list6 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary6 = {model.ProjectId} ");
            if (list6.Count > 0)
            {
                return list6;
            }
            var list7 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary7 = {model.ProjectId} ");
            if (list7.Count > 0)
            {
                return list7;
            }
            var list8 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary8 = {model.ProjectId} ");
            if (list8.Count > 0)
            {
                return list8;
            }
            var list9 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary9 = {model.ProjectId} ");
            if (list9.Count > 0)
            {
                return list9;
            }
            var list10 = _context.DynamicSqlQuery($@"SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary10 = {model.ProjectId} ");
            if (list10.Count > 0)
            {
                return list10;
            }
            return new List<dynamic>();
        }
    }
}
