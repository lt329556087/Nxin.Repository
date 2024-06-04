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
namespace FinanceManagement.ApiHost.Controllers.BaddebtAccrualDraft
{
    public class BaddebtAccrualDraftQueryCommand : IRequest<RptResult>
    {
        /// <summary>
        /// 校验参数信息
        /// </summary>
        /// <returns></returns>
        public string CheckModel()
        {
            var validationMsg = string.Empty;
            if (Boid.IsNullOrEmpty())
            {
                validationMsg += $"{nameof(Boid)}必填；";
            }
            if (GroupID.IsNullOrEmpty())
            {
                validationMsg += $"{nameof(GroupID)}必填；";
            }
            if (Begindate.IsNullOrEmpty())
            {
                validationMsg += $"{nameof(Begindate)}必填；";
            }

            return validationMsg;
        }
        //public List<string> DataList { get; set; }
        public string Begindate { get; set; }
        //public string Enddate { get; set; }
        //public bool OnlyCombineEnte { get; set; }
        /// <summary>
        /// 权限单位
        /// </summary>
        public List<string> Perm_EnterList { get; set; }//同OwnEntes
        /// <summary>
        /// 选择单位
        /// </summary>
        public List<string> EnterpriseList { get; set; }//同EnterpriseList_id
        /// <summary>
        /// 选择的合并单位
        /// </summary>
        //public List<string> Merge_EnterList { get; set; }   

        public long EnteID { get; set; }
        /// <summary>
        /// 0为业务系统进入用enteid，1为OA菜单进入用groupid
        /// </summary>
        public string MenuParttern { get; set; }

        public string GroupID { get; set; }
        public string Boid { get; set; }

    }
}
