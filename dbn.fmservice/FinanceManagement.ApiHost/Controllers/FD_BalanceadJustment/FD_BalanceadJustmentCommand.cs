using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BalanceadJustment
{

    public class FD_BalanceadJustmentAddCommand : FD_BalanceadJustmentCommand, IRequest<Result>
    {

    }

    public class FD_BalanceadJustmentDeleteCommand : FD_BalanceadJustmentCommand, IRequest<Result>
    {

    }

    public class FD_BalanceadJustmentModifyCommand : FD_BalanceadJustmentCommand, IRequest<Result>
    {
    }
    public class FD_BalanceadJustmentSearchCommand : R2PRequest
    {
        public string Account_id { get; set; }
        public string Begindate { get; set; }
        public string Enddate { get; set; }
        public string Reviewed { get; set; }
        public string DataDate { get; set; }
        /// <summary>
        /// 资金账户ID
        /// </summary>		
        public string AccountID { get; set; }
        public string EnteId { get; set; }
    }
    public class FD_BalanceadJustmentCommand : MutilLineCommand<FD_BalanceadJustmentDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        public string DataDate { get; set; }
        
        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// 资金账户ID
        /// </summary>		
        public string AccountID { get; set; }

        /// <summary>
        /// 资金账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
    }

    public class FD_BalanceadJustmentDetailCommand : CommonOperate
    {
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        public string EnterProjectID { get; set; }

        public string EnterProjectName { get; set; }

        public string BankProjectID { get; set; }
        public string BankProjectName { get; set; }
        /// <summary>
        /// EnterProjectAmount
        /// </summary>		
        public decimal EnterProjectAmount { get; set; }

        /// <summary>
        /// BankProjectAmount
        /// </summary>		
        public decimal BankProjectAmount { get; set; }
    }

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }



    public interface IR2PRequest
    {
    }
    //R:report 报表端
    //F:front 网站端
    //P:plantform 服务端
    /// <summary>
    /// 报表到服务端的请求
    /// </summary>
    public class R2PRequest : IR2PRequest
    {
        /// <summary>
        ///  //是否查询结账数据
        ///  0为实时
        ///  1为结账
        /// </summary>
        public int DataSource { get; set; }
        /// <summary>
        /// 汇总方式1，2，3
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        public List<string> OwnEntes { get; set; }
        public List<string> GetZBOwnEntes()
        {
            List<string> strLst = new List<string>() {
            "634086739144000981","634086739144000461","634086739144001161"};
            return strLst;
        }
        public List<string> CanWatchEntes { get; set; }
        /// <summary>
        /// 用于产业汇总用
        /// </summary>
        public string SummaryT1Rank { get; set; }
        public string SummaryT2Rank { get; set; }
        public string SummaryT3Rank { get; set; }
        /// <summary>
        /// 判断是否用的是产业/事业部/核算单元的方式汇总
        /// </summary>
        public bool IsGroupByEnteCate { get; set; }
        /// <summary>
        /// 专门针对 2019年3月25-28日提出来的 核算单元 创业单位 organizationID 进行判断的
        /// </summary>
        public bool[] IsGroupByChuangYeDanYuan { get; set; }
        public string EnteCateSummary
        {
            get
            {
                var sum = string.Empty;
                if (!string.IsNullOrEmpty(SummaryType1) && this.SummaryType1.Contains("EnteCate_"))
                {
                    var split = SummaryType1.Split('_')[2];
                    sum += split + "_";
                }
                if (!string.IsNullOrEmpty(SummaryType2) && SummaryType2.Contains("EnteCate_"))
                {
                    var split = SummaryType2.Split('_')[2];
                    sum += split + "_";
                }
                if (!string.IsNullOrEmpty(SummaryType3) && SummaryType3.Contains("EnteCate_"))
                {
                    var split = SummaryType3.Split('_')[2];
                    sum += split + "_";
                }
                return sum;
            }
        }
        public long GroupID { get; set; }
        public long EnteID { get; set; }
        public string Boid { get; set; }
        /// <summary>
        /// 0为业务系统进入用enteid，1为OA菜单进入用groupid
        /// </summary>
        public string MenuParttern { get; set; }
        //public string CustomerField { get; set; }

        #region 权限使用
        /// <summary>
        /// 某一用户下负责的部门。 第一类人
        /// </summary>
        public string MarketsUnderUser { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public string Orgs { get; set; }

        /// <summary>
        /// 是否是公司负责人 第二类人
        /// </summary>
        public bool IsCompanyManager { get; set; }
        /// <summary>
        /// 是否有报表权限， 第三类人
        /// </summary>
        public bool HasReportAuth { get; set; }
        #endregion

    }
}
