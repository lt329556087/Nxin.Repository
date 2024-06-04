using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IBiz_Related : IRepository<BIZ_Related, string>
    {
        Task<bool> ExistAsync();
        Task<List<BIZ_Related>> GetRelated(BIZ_Related model);
        Task<List<BIZ_Related>> GetRelatedList(BIZ_Related model);
        List<BIZ_Related> GetList(BIZ_Related model);
        /// <summary>
        /// 用于收款退回
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        List<BIZ_Related> GetListByNum(string Num);
        /// <summary>
        /// 金融专用 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<BIZ_Related> GetBIZ_Related(BIZ_Related model);
    }
    public interface IBiz_Related_FM : IRepository<BIZ_Related_FM, string>
    {
        Task<bool> ExistAsync();
        Task<List<BIZ_Related_FM>> GetRelated(BIZ_Related_FM model);
        Task<List<BIZ_Related_FM>> GetRelatedList(BIZ_Related_FM model);
        List<BIZ_Related_FM> GetList(BIZ_Related_FM model);
        /// <summary>
        /// 复核专用,通过流水，recordid 获取所有阶段流程数据 （驳回用到）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<BIZ_Related_FM> GetPayReviewList(BIZ_Related_FM model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        List<BIZ_Related_FM> GetListByNum(string Num);
        /// <summary>
        /// s.RelatedType == model.RelatedType && s.ParentType == model.ParentType && s.ChildType == model.ChildType && s.ChildValue == model.ChildValue
        /// `RelatedType` = 2205231634370000109
        /// `ParentType` = 总级次
        /// `ChildType` = 级次
        /// `ParentValue` = 付款流水
        /// `ChildValue` = 付款明细标识 extend RecordId
        /// `ParentValueDetail` = 复核人
        /// `ChildValueDetail` = 复核状态 复核/驳回
        /// `Remarks` = 记录操作时间 2023-04-11 15:02:34
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<BIZ_Related_FM> GetBIZ_Related(BIZ_Related_FM model);

    }
}
