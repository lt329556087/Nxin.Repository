using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class fd_auxiliaryprojectRepository : Repository<fd_auxiliaryproject, string, Nxin_Qlw_BusinessContext>, Ifd_auxiliaryprojectRepository
    {
        public fd_auxiliaryprojectRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        /// <summary>
        /// 检测当前编码是否重复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<fd_auxiliaryproject> IsExistCode(fd_auxiliaryproject model)
        {
            return this.DbContext.Set<fd_auxiliaryproject>().FromSqlRaw(@$" Select * from fd_auxiliaryproject where groupid = {model.GroupId} and ProjectType = {model.ProjectType}  and ProjectCode = {model.ProjectCode} AND ProjectID <> {model.ProjectId} ").ToList();
        }
        /// <summary>
        /// 检测凭证是否使用过当前数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<fd_auxiliaryproject> IsExistSettle(fd_auxiliaryproject model)
        {
            return this.DbContext.Set<fd_auxiliaryproject>().FromSqlRaw(@$" 
            SELECT Concat(v.NumericalOrder) ProjectId,'0' ProjectType,'0' ProjectName,'0' ProjectCode,0 Level,'0' Pid,'0' OwnerID,'0' GroupId,Now() CreatedDate,Now() ModifiedDate,'0' Remarks from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            inner join qlw_nxin_com.biz_enterprise en on en.enterpriseid = v.enterpriseid and en.PID = {model.GroupId} 
            where vd.Auxiliary1 = {model.ProjectId} or vd.Auxiliary2 = {model.ProjectId} or vd.Auxiliary3 = {model.ProjectId} or vd.Auxiliary4 = {model.ProjectId} or vd.Auxiliary5 = {model.ProjectId} OR 
            vd.Auxiliary6 = {model.ProjectId} or vd.Auxiliary7 = {model.ProjectId} or vd.Auxiliary8 = {model.ProjectId} or vd.Auxiliary9 = {model.ProjectId} or vd.Auxiliary10 = {model.ProjectId} 
            ").ToList();
        }
    }
    public class fd_auxiliarytypeRepository : Repository<fd_auxiliarytype, string, Nxin_Qlw_BusinessContext>, Ifd_auxiliarytypeRepository
    {
        public fd_auxiliarytypeRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public List<fd_auxiliarytype> GetAuxiliaryTypeByTag(fd_auxiliarytype model)
        {
            return this.DbContext.Set<fd_auxiliarytype>().FromSqlRaw(@$" Select * from fd_auxiliarytype where groupid = {model.GroupId} AND TypeTag = 2311151408380000101 ").ToList();
        }

        /// <summary>
        /// 检测当前类型编码是否重复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<fd_auxiliarytype> IsExistCode(fd_auxiliarytype model)
        {
            return this.DbContext.Set<fd_auxiliarytype>().FromSqlRaw(@$" Select * from fd_auxiliarytype where groupid = {model.GroupId} and TypeCode = {model.TypeCode} AND NumericalOrder <> {model.NumericalOrder} ").ToList();
        }

        public List<fd_auxiliaryproject> IsExistData(fd_auxiliaryproject model)
        {
            return this.DbContext.Set<fd_auxiliaryproject>().FromSqlRaw(@$" Select * from fd_auxiliaryproject where groupid = {model.GroupId} and ProjectType = {model.ProjectType} ").ToList();
        }
    }
}