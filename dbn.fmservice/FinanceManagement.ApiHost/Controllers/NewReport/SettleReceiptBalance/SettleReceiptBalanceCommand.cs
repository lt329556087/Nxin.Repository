using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Codeless.Report;
using System.Text.RegularExpressions;
namespace FinanceManagement.ApiHost.Controllers.SettleReceiptBalance
{
    public class SettleReceiptBalanceQueryCommand : IRequest<GatewayResultModel>
    {
        /// <summary>
        /// 校验参数信息
        /// </summary>
        /// <returns></returns>
        public string CheckModel()
        {
            var validationMsg = string.Empty;
            //if (boid.IsNullOrEmpty())
            //{
            //    validationMsg += $"{nameof(boid)}必填；";
            //}
            //if (groupID.IsNullOrEmpty())
            //{
            //    validationMsg += $"{nameof(groupID)}必填；";
            //}
            if (DataList == null || DataList.Count < 2)
            {
                validationMsg += $"{nameof(DataList)}必填，包含开始截止日期；";
            }
            else
            {
                Begindate = DataList[0];
                Enddate = DataList[1];
                if (Begindate.IsNullOrEmpty())// || (Begindate.IsNotNullOrEmpty() && !Regex.IsMatch(Begindate, @"^\d{4}(-|/)((0([1-9]))|(1(0|1|2)))(-|/)((0([1-9]))|((1|2)([1-9]))|((3)(0|1)))$")))
                {
                    validationMsg += $"{nameof(DataList)}校验失败；";
                }
                else
                {
                    try
                    {
                        Begindate = DateTime.Parse(Begindate).ToString("yyyy-MM-dd");
                    } catch (Exception ex)
                    {
                        validationMsg += $"{nameof(DataList)}校验失败；";
                    }
                }
                if (Enddate.IsNullOrEmpty())// || (Enddate.IsNotNullOrEmpty() && !Regex.IsMatch(Enddate, @"^\d{4}(-|/)((0([1-9]))|(1(0|1|2)))(-|/)((0([1-9]))|((1|2)([1-9]))|((3)(0|1)))$")))
                {
                    validationMsg += $"{nameof(DataList)}校验失败；";
                }
                else
                {
                    try
                    {
                        Enddate = DateTime.Parse(Enddate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex)
                    {
                        validationMsg += $"{nameof(DataList)}校验失败；";
                    }
                }
            }
            AccountingSubjectsList?.RemoveAll(p => string.IsNullOrEmpty(p));
            if (AccountingSubjectsList?.Count > 0)
            {
                for (var i = 0; i < AccountingSubjectsList.Count; i++)
                {
                    //查看是否有上级科目
                    var subList = AccountingSubjectsList.Where(p => p.Length <= AccountingSubjectsList[i].Length && AccountingSubjectsList[i].StartsWith(p) && p != AccountingSubjectsList[i]);
                    if (subList?.Count() > 0)
                    {
                        AccountingSubjectsList.Remove(AccountingSubjectsList[i]);
                        i--;
                    }
                }
                //AccountingSubjectsRadio = string.Join(',', AccountingSubjectsList);
            }
            if (string.IsNullOrEmpty(SubjectLevel))
            {
                SubjectLevel = "1";
            }
            return validationMsg;
        }
        public List<string> DataList { get; set; }
        public string Begindate { get; set; }
        public string Enddate { get; set; }
        public bool? OnlyCombineEnte { get; set; }
        /// <summary>
        /// 权限单位
        /// </summary>
        public List<string> Perm_EnterList { get; set; }//同OwnEntes
        /// <summary>
        /// 选择单位
        /// </summary>
        public List<string> EnterpriseList { get; set; }//同EnterpriseList_id
        ///// <summary>
        ///// 往来类型
        ///// </summary>
        public string AccountingType { get; set; }
        public List<string> AccountingTypeList { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CurrentUnitIDs { get; set; }
        public List<string> AccountingSubjectsList { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public List<string> CurrentUnitIDList { get; set; }
        /// <summary>
        /// 不显示期末余额为0数据
        /// </summary>

        public bool? HideEndZero { get; set; }


        public string groupID { get; set; }
        public string boid { get; set; }
        public string SubjectLevel { get; set; } = "1";
        ///// <summary>
        ///// 汇总方式 单位：EnterpriseID，科目：AccoSubjectCode，往来单位：CustomerID 其他暂不支持
        ///// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; } = 50;
        /// <summary>
        /// 页码 从1开始
        /// </summary>
        public int PageIndex { get; set; } = 1;
    }

    public class OrgEnterRequest
    {
        public string groupId { get; set; }
        //使用单位
        public string enterpriseId { get; set; }
        // 指定参数，用于选中返回数据
        public string checklist { get; set; }
        //组织父级
        public string pid { get; set; } = "-1";
        //过滤条件
        public string name { get; set; }
        //是否组织树
        public string istree { get; set; } = "1";
        public string boid { get; set; }
        public string isuse { get; set; } = "-1";// -1 全部 0停用 1启用
    }
    public class OrgEnterResult
    {
        public string Type { get; set; }
        public string cAxis { get; set; }
        public string Abled { get; set; }
        public string Check { get; set; }
        public string pid { get; set; }
        public int IsEnter { get; set; }
        public string id { get; set; }
        public string Name { get; set; }
        public bool IsUse { get; set; }
    }
    public class OrgResult
    {
        public string sortId { get; set; }
        public string sortName { get; set; }
        public string pid { get; set; }
        public string cFullName { get; set; }
        public string sortRank { get; set; }
        public string isEnd { get; set; }
        public bool isUsed { get; set; }
        public string cAxis { get; set; }
        public int type { get; set; }
    }
    public class EnterResult
    {
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string SortId { get; set; }
        public string PID { get; set; }
        public bool IsUse { get; set; }
        public string EnterpriseFullName { get; set; }       
        public string Level { get; set; }
        
}
    public class SubjectLevelRequest
    {
        /// <summary>
        /// BO_ID
        /// </summary>
        public string enterpriseId { get; set; }
        /// <summary>
        /// GroupID
        /// </summary>
        public string groupId { get; set; }
    }
    public class DropResult
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class DropODataResult
    {
        [Key]
        public string id { get; set; }
        public string name { get; set; }
    }
    public class ODataResultModel
    {
        public object value { get; set; }
    }
    public class PersonResult
    {
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
    public class GatewayResultModel
    {
        /// <summary>
        /// 消息编号
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; }
        public int totalcount { get; set; }
    }
}
