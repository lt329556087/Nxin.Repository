using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifd_auxiliaryprojectRepository : IRepository<fd_auxiliaryproject, string>
    {
        /// <summary>
        /// 监测是否有重复编码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<fd_auxiliaryproject> IsExistCode(fd_auxiliaryproject model);
        /// <summary>
        /// 检测凭证是否使用过当前数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<fd_auxiliaryproject> IsExistSettle(fd_auxiliaryproject model);
    }
    public interface Ifd_auxiliarytypeRepository : IRepository<fd_auxiliarytype, string>
    {
        /// <summary>
        /// 监测是否有重复类型编码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<fd_auxiliarytype> IsExistCode(fd_auxiliarytype model);
        /// <summary>
        /// 根据类型标签获取当前集团此标签下的 所有类型数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<fd_auxiliarytype> GetAuxiliaryTypeByTag(fd_auxiliarytype model);
        /// <summary>
        /// 监测当前数据是否已被使用过
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<fd_auxiliaryproject> IsExistData(fd_auxiliaryproject model);
    }
}
