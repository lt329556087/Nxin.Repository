using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.PerformanceIncome
{

    public class FM_PerformanceIncomeAddCommand : FM_PerformanceIncomeCommand, IRequest<Result>
    {

    }

    public class FM_PerformanceIncomeDeleteCommand : FM_PerformanceIncomeCommand, IRequest<Result>
    {

    }

    public class FM_PerformanceIncomeModifyCommand : FM_PerformanceIncomeCommand, IRequest<Result>
    {
    }
    public class FM_PerformanceIncomeCopyCommand : FM_PerformanceIncomeCommand, IRequest<Result>
    {
        public List<string> EnterpriseIds { get; set; }
    }
    public class FM_PerformanceIncomeCommand
    {
        public int RecordID { get; set; }
        public string ProductGroupID { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductGroupTypeName { get; set; }
        public string IncomeTypeName { get; set; }
        public string ParentTypeName { get; set; }
        public string PropertyName { get; set; }
        public string EnterpriseID { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
        public List<FM_PerformanceIncomeCommand> List { get; set; } = new List<FM_PerformanceIncomeCommand>();
    }

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }
    public class PerformanceIncomeSearch
    {
        public string EnterpriseID { get; set; }
        public string GroupID { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string ax1SortId { get; set; }
        public string ax2SortId { get; set; }
        public string ax3SortId { get; set; }
        public string ax1SortName { get; set; }
        public string ax2SortName { get; set; }
        public string ax3SortName { get; set; }
        public string PersonID { get; set; }
        public string PersonName { get; set; }
        public List<string> StortList { get; set; } = new List<string>();
        public List<string> SummaryType { get; set; } = new List<string>();
    }

    public class BusinessSearch
    {
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string BeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string Boids { get; set; }
        /// <summary>
        /// 人员汇总方式
        /// </summary>
        public bool IsGroupRank { get; set; }
        /// <summary>
        /// 集团商品分类 汇总方式
        /// </summary>
        public bool IsIncomeRank { get; set; }
        /// <summary>
        /// 单位汇总方式
        /// </summary>
        public bool IsEntentRank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEntentGroup { get; set; }
        /// <summary>
        /// 商品汇总方式
        /// </summary>
        public bool IsProductRank { get; set; }
        /// <summary>
        /// 客户汇总方式
        /// </summary>
        public bool IsCustomerRank { get; set; }
        /// <summary>
        /// 部门汇总方式
        /// </summary>
        public bool IsMarketRank { get; set; }
        /// <summary>
        /// 部门组织排名汇总
        /// </summary>
        public bool IsOrgRank { get; set; }
        /// <summary>
        /// 金额排序
        /// </summary>
        public bool IsOrderIncome { get; set; }
        /// <summary>
        /// 销量排序
        /// </summary>
        public bool IsOrderQuantity { get; set; }
        /// <summary>
        /// 净收入排序
        /// </summary>
        public bool IsOrderNetIncome { get; set; }
        /// <summary>
        /// 部门组织排名筛选
        /// </summary>
        public string OrgSortId { get; set; }
        /// <summary>
        /// 部门组织筛选
        /// </summary>
        public string SortId { get; set; }
        /// <summary>
        /// 筛选部门组织名称
        /// </summary>
        public string OrgSortName { get; set; }
        /// <summary>
        /// 筛选分类名称
        /// </summary>
        public string ClassificationName { get; set; }
        /// <summary>
        /// 当前筛选的分类级次(部门组织)
        /// </summary>
        public int ClassificationRank { get; set; }
        public int OrgRank { get; set; }
        /// <summary>
        /// 点击客户名称，钻取弹层到【商品代号，数值】
        /// </summary>
        public string CustomerName { get; set; }

    }
    public class SalesSummarySearch
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        /// <summary>
        /// 商品代号 逗号分隔 多选传参
        /// </summary>
        public string ProductIds { get; set; }
        /// <summary>
        /// 商品分类 逗号分隔 多选传参
        /// </summary>
        public string ProductGroupClassificationIds { get; set; }
        /// <summary>
        /// 商品名称 逗号分隔 多选传参
        /// </summary>
        public string ProductGroupIds { get; set; }
        /// <summary>
        /// 销售摘要 逗号分隔 多选传参
        /// </summary>
        public string SalesAbstracts { get; set; }
        /// <summary>
        /// 单位组织 逗号分隔 多选传参
        /// </summary>
        public string EnterSortIds { get; set; }
        /// <summary>
        /// 创业单元 逗号分隔 多选传参
        /// </summary>
        public string MarketSortIds { get; set; }
        /// <summary>
        /// 客户 逗号分隔 多选传参
        /// </summary>
        public string CustomerIds { get; set; }
        /// <summary>
        /// 销售类型 True:销售 False:赠送
        /// </summary>
        public bool? IsGift { get; set; }
        /// <summary>
        /// 获取权限单位
        /// </summary>
        public string EnterpriseIds { get; set; }
        /// <summary>
        /// 汇总方式 数组传参 对应实体名称 例子：单位组织一级，二级，三级["org_en1","org_en2","org_en3"]
        /// </summary>
        public List<string> SummaryType { get; set; } = new List<string>();
        /// <summary>
        /// 部门ID
        /// </summary>
        public string MarketId { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string AreaId { get; set; }
        /// <summary>
        /// 业务员ID
        /// </summary>
        public string SalesmanID { get; set; }
        public string EnterpriseId { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }

    }
}
