using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    //参数
    public class RptAssistRecordRequest
    {
        public string CheckModel()
        {
            var validationMsg = string.Empty;
            if (BeginDate.IsNullOrEmpty())// || (Begindate.IsNotNullOrEmpty() && !Regex.IsMatch(Begindate, @"^\d{4}(-|/)((0([1-9]))|(1(0|1|2)))(-|/)((0([1-9]))|((1|2)([1-9]))|((3)(0|1)))$")))
            {
                BeginDate = DateTime.Now.ToString("yyyy-MM-01");
            }
            else
            {
                try
                {
                    BeginDate = DateTime.Parse(BeginDate).ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    validationMsg += $"{nameof(BeginDate)}校验失败；";
                }
            }
            if (EndDate.IsNullOrEmpty())// || (Begindate.IsNotNullOrEmpty() && !Regex.IsMatch(Begindate, @"^\d{4}(-|/)((0([1-9]))|(1(0|1|2)))(-|/)((0([1-9]))|((1|2)([1-9]))|((3)(0|1)))$")))
            {
                EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                try
                {
                    EndDate = DateTime.Parse(EndDate).ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    validationMsg += $"{nameof(EndDate)}校验失败；";
                }
            }
            //显示方式为汇总时汇总方式不能为空 
            if (ReportType == 1)
            {
                if (SummaryTypeList == null || SummaryTypeList.Count() == 0)
                {
                    validationMsg += $"{nameof(SummaryTypeList)}不能为空；";
                }
            }
            return validationMsg;
        }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 选择单位
        /// </summary>
        public List<string> EnterpriseList { get; set; }
        /// <summary>
        /// 合并单位
        /// </summary>
        public bool? OnlyCombineEnte { get; set; }
        /// <summary>
        /// 显示方式 0:明细 1：汇总
        /// </summary>
        public int ReportType { get; set; }
        ///// <summary>
        ///// 科目编码
        ///// </summary>        
        //public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 科目编码
        /// </summary>        
        public List<string> AccoSubjectCodeList { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>        
        public string SettleSummaryID { get; set; }
        /// <summary>
        /// 员工
        /// </summary>        
        public string PersonID { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 客商
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 凭证类别
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 凭证字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 汇总方式
        /// </summary>
        public List<string> SummaryTypeList { get; set; } = new List<string>();
        ///// <summary>
        ///// 权限单位
        ///// </summary>
        //public List<string> Perm_EnterList { get; set; }
        /// <summary>
        /// 有效单位
        /// </summary>
        public List<string> EnableEnterList { get; set; }
        #region 自定义辅助项
        public string Auxiliary1 { get; set; }
        public string Auxiliary2 { get; set; }
        public string Auxiliary3 { get; set; }
        public string Auxiliary4 { get; set; }
        public string Auxiliary5 { get; set; }
        public string Auxiliary6 { get; set; }
        public string Auxiliary7 { get; set; }
        public string Auxiliary8 { get; set; }
        public string Auxiliary9 { get; set; }
        public string Auxiliary10 { get; set; }
        public string AuxiliaryName1 { get; set; }
        public string AuxiliaryName2 { get; set; }
        public string AuxiliaryName3 { get; set; }
        public string AuxiliaryName4 { get; set; }
        public string AuxiliaryName5 { get; set; }
        public string AuxiliaryName6 { get; set; }
        public string AuxiliaryName7 { get; set; }
        public string AuxiliaryName8 { get; set; }
        public string AuxiliaryName9 { get; set; }
        public string AuxiliaryName10 { get; set; }

        #endregion
    }

    /// <summary>
    /// 汇总方式
    /// </summary>
    public class SummaryDataType
    {
        /// <summary>
        /// 汇总方式名称SummaryName
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 汇总方式的值SummaryValue
        /// </summary>
        public string SV { get; set; }

        ///// <summary>
        ///// 是否默认
        ///// </summary>
        //public bool DV { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int Order { get; set; }
        //级次
        public int Rank { get; set; }
    }

    public class SummaryTypeRequest
    {
        public string DataDate { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 选择单位
        /// </summary>
        public List<string> EnterpriseList { get; set; }
    }

    public class AssistDorpRequest
    {
        /// <summary>
        /// 集团ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 选择单位
        /// </summary>
        public List<string> EnterpriseList { get; set; }
        public string DataDate { get; set; }
    }
    public class OptionRequest
    {
        public string OptionId { get; set; }
        public string EnterpriseID { get; set; }
        //1系统2集团4单位 8个人16场
        public string ScopeCode { get; set; }
    }
}
