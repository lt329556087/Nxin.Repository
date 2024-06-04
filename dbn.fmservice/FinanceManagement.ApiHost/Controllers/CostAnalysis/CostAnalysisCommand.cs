using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.PerformanceIncome
{

    public class CostAnalysisAddCommand : CostAnalysisCommand, IRequest<Result>
    {

    }

    public class CostAnalysisDeleteCommand : CostAnalysisCommand, IRequest<Result>
    {

    }

    public class CostAnalysisModifyCommand : CostAnalysisCommand, IRequest<Result>
    {
    }
    public class CostAnalysisCopyCommand : CostAnalysisCommand, IRequest<Result>
    {
        public List<string> EnterpriseIds { get; set; }
    }
    public class CostAnalysisCommand 
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
        public List<CostAnalysisCommand> List { get; set; } = new List<CostAnalysisCommand>();
    }
    public class CostAnalysisSearch
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
    /// <summary>
    /// 费用分析表
    /// </summary>
    public class CostAnalysisData
    {
        /// <summary>
        /// 列汇总方式数据项
        /// </summary>
        public dynamic ColumnList { get; set; }
        /// <summary>
        /// 行汇总数据项（横向列）
        /// </summary>
        public dynamic RowColumnList { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public dynamic DataList { get; set; }
    }
}
