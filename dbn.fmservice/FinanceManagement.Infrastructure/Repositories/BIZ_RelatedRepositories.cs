using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class BIZ_RelatedRepositories : Repository<BIZ_Related, string, Nxin_Qlw_BusinessContext>, IBiz_Related
    {
        public BIZ_RelatedRepositories(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public Task<bool> ExistAsync()
        {
            return Set.AnyAsync();
        }
        /// <summary>
        /// 根据父级子级流水号 获取关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<BIZ_Related> GetList(BIZ_Related model)
        {
            return Set.Where(m => m.ChildValue == model.ChildValue && m.ParentValue == model.ParentValue && m.ParentValueDetail == model.ParentValueDetail).ToList();
        }

        public List<BIZ_Related> GetListByNum(string Num)
        {
            var data = Set.Where(m => m.ParentValue == Num && m.ChildType == "201611180104402201").ToList();
            if (data != null && data.Count > 0)
            {
                return data;
            }
            else
            {
                return Set.Where(m => m.ParentValue == Num && m.ChildType == "201611180104402203").ToList();
            }
        }
        /// <summary>
        /// 金融预收款使用专用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<BIZ_Related> GetBIZ_Related(BIZ_Related model)
        {
            if (model == null)
            {
                return new List<BIZ_Related>();
            }
            return Set.Where(s => s.RelatedType == model.RelatedType && s.ParentType == model.ParentType && s.ChildType == model.ChildType && s.ChildValue == model.ChildValue).ToList();
        }
        public Task<List<BIZ_Related>> GetRelated(BIZ_Related model)
        {
            if (model == null)
            {
                return Task.FromResult(new List<BIZ_Related>());
            }
            var data = Set.Where(s => s.RelatedType == model.RelatedType && s.ParentType == model.ParentType).ToList();
            if (!string.IsNullOrEmpty(model.ChildType))
            {
                data = data.Where(s => s.ChildType == model.ChildType).ToList();
            }
            if (!string.IsNullOrEmpty(model.ParentValue))
            {
                data = data.Where( s => model.ParentValue.Contains(s.ParentValue)).ToList();
            }
            if (!string.IsNullOrEmpty(model.ChildValue))
            {
                data = data.Where(s => model.ChildValue.Contains(s.ChildValue)).ToList();
            }
            if (!string.IsNullOrEmpty(model.ParentValueDetail))
            {
                data = data.Where(s => s.ParentValueDetail == model.ParentValueDetail).ToList();
            }
            if (!string.IsNullOrEmpty(model.ChildValueDetail))
            {
                data = data.Where(s => s.ChildValueDetail == model.ChildValueDetail).ToList();
            }
            return Task.FromResult(data);
        }
        public Task<List<BIZ_Related>> GetRelatedList(BIZ_Related model)
        {
            if (model == null)
            {
                return Task.FromResult(new List<BIZ_Related>());
            }
            var data = Set.Where(s => s.RelatedType == model.RelatedType && s.ChildType == model.ChildType).ToList();
            if (!string.IsNullOrEmpty(model.ParentType))
            {
                data = data.Where(s => s.ParentType == model.ParentType).ToList();
            }
            //if (!string.IsNullOrEmpty(model.ChildType))
            //{
            //    data = data.Where(s => s.ChildType == model.ChildType).ToList();
            //}
            if (!string.IsNullOrEmpty(model.ParentValue))
            {
                data = data.Where(s => model.ParentValue==s.ParentValue).ToList();
            }
            if (!string.IsNullOrEmpty(model.ChildValue))
            {
                data = data.Where(s => model.ChildValue==s.ChildValue).ToList();
            }
            if (!string.IsNullOrEmpty(model.ParentValueDetail))
            {
                data = data.Where(s => s.ParentValueDetail == model.ParentValueDetail).ToList();
            }
            if (!string.IsNullOrEmpty(model.ChildValueDetail))
            {
                data = data.Where(s => s.ChildValueDetail == model.ChildValueDetail).ToList();
            }
            if (!string.IsNullOrEmpty(model.Remarks))
            {
                data = data.Where(s => s.Remarks == model.Remarks).ToList();
            }
            return Task.FromResult(data);
        }
    }

    public class BIZ_Related_FMRepositories : Repository<BIZ_Related_FM, string, Nxin_Qlw_BusinessContext>, IBiz_Related_FM
    {
        public BIZ_Related_FMRepositories(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public Task<bool> ExistAsync()
        {
            return Set.AnyAsync();
        }
        /// <summary>
        /// 根据父级子级流水号 获取关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<BIZ_Related_FM> GetList(BIZ_Related_FM model)
        {
            return Set.Where(m => m.ChildValue == model.ChildValue && m.ParentValue == model.ParentValue && m.ParentValueDetail == model.ParentValueDetail).ToList();
        }

        public List<BIZ_Related_FM> GetListByNum(string Num)
        {
            var data = Set.Where(m => m.ParentValue == Num && m.ChildType == "201611180104402201").ToList();
            if (data != null && data.Count > 0)
            {
                return data;
            }
            else
            {
                return Set.Where(m => m.ParentValue == Num && m.ChildType == "201611180104402203").ToList();
            }
        }
        /// <summary>
        /// 复核支付专用
        /// s.RelatedType == model.RelatedType && s.ParentType == model.ParentType && s.ChildType == model.ChildType && s.ChildValue == model.ChildValue
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<BIZ_Related_FM> GetBIZ_Related(BIZ_Related_FM model)
        {
            if (model == null)
            {
                return new List<BIZ_Related_FM>();
            }
            //最大级次+级次+recordId  初步筛选
            var data = Set.Where(s => s.RelatedType == model.RelatedType && s.ParentType == model.ParentType && s.ChildType == model.ChildType && s.ChildValue == model.ChildValue).ToList();
            //锁定到人
            if (!string.IsNullOrEmpty(model.ParentValueDetail))
            {
                data = data.Where(m => m.ParentValueDetail == model.ParentValueDetail).ToList();
            }
            return data;
        }
        public Task<List<BIZ_Related_FM>> GetRelated(BIZ_Related_FM model)
        {
            if (model == null)
            {
                return Task.FromResult(new List<BIZ_Related_FM>());
            }
            var data = Set.Where(s => s.RelatedType == model.RelatedType && s.ParentType == model.ParentType).ToList();
            if (!string.IsNullOrEmpty(model.ChildType))
            {
                data = data.Where(s => s.ChildType == model.ChildType).ToList();
            }
            if (!string.IsNullOrEmpty(model.ParentValue))
            {
                data = data.Where(s => model.ParentValue.Contains(s.ParentValue)).ToList();
            }
            if (!string.IsNullOrEmpty(model.ChildValue))
            {
                data = data.Where(s => model.ChildValue.Contains(s.ChildValue)).ToList();
            }
            if (!string.IsNullOrEmpty(model.ParentValueDetail))
            {
                data = data.Where(s => s.ParentValueDetail == model.ParentValueDetail).ToList();
            }
            if (!string.IsNullOrEmpty(model.ChildValueDetail))
            {
                data = data.Where(s => s.ChildValueDetail == model.ChildValueDetail).ToList();
            }
            return Task.FromResult(data);
        }
        public Task<List<BIZ_Related_FM>> GetRelatedList(BIZ_Related_FM model)
        {
            if (model == null)
            {
                return Task.FromResult(new List<BIZ_Related_FM>());
            }
            var data = Set.Where(s => s.RelatedType == model.RelatedType && s.ChildType == model.ChildType).ToList();
            if (!string.IsNullOrEmpty(model.ParentType))
            {
                data = data.Where(s => s.ParentType == model.ParentType).ToList();
            }
            //if (!string.IsNullOrEmpty(model.ChildType))
            //{
            //    data = data.Where(s => s.ChildType == model.ChildType).ToList();
            //}
            if (!string.IsNullOrEmpty(model.ParentValue))
            {
                data = data.Where(s => model.ParentValue == s.ParentValue).ToList();
            }
            if (!string.IsNullOrEmpty(model.ChildValue))
            {
                data = data.Where(s => model.ChildValue == s.ChildValue).ToList();
            }
            if (!string.IsNullOrEmpty(model.ParentValueDetail))
            {
                data = data.Where(s => s.ParentValueDetail == model.ParentValueDetail).ToList();
            }
            if (!string.IsNullOrEmpty(model.ChildValueDetail))
            {
                data = data.Where(s => s.ChildValueDetail == model.ChildValueDetail).ToList();
            }
            if (!string.IsNullOrEmpty(model.Remarks))
            {
                data = data.Where(s => s.Remarks == model.Remarks).ToList();
            }
            return Task.FromResult(data);
        }
        /// <summary>
        /// 软删除查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<BIZ_Related_FM> GetPayReviewList(BIZ_Related_FM model)
        {
            return Set.Where(m => m.RelatedType == model.RelatedType && m.ParentValue == model.ParentValue && m.ChildValue == model.ChildValue).ToList();
        }
    }
}
