using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_AccoCheck
{

    public class FM_AccoCheckAddCommand : FM_AccoCheckCommand, IRequest<Result>
    {

    }

    public class FM_AccoCheckDeleteCommand : FM_AccoCheckCommand, IRequest<Result>
    {

    }

    public class FM_AccoCheckModifyCommand : FM_AccoCheckCommand, IRequest<Result>
    {
    }
    public class FM_AccoCheckCopyCommand : FM_AccoCheckCommand, IRequest<Result>
    {
        public List<string> EnterpriseIds { get; set; }
    }
    public class FM_AccoCheckCommand : MutilLineCommand<FM_AccoCheckDetailCommand>
    {
        public string PigFarmIds { get; set; }
        public bool IsSinge { get; set; }
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 结账标识
        /// </summary>
        public bool CheckMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        /// <summary>
        /// 所属单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
    }

    public class FM_AccoCheckDetailCommand : CommonOperate
    {
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 结算摘要
        /// </summary>
        public string AccoCheckType { get; set; }

        public bool CheckMark { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string ModifiedDate { get; set; }
        public bool IsNew { get; set; }
        public List<FM_AccoCheckExtendCommand> Extends { get; set; }
    }

    public class FM_AccoCheckExtendCommand : CommonOperate
    {
        public int RecordID { get; set; }
        /// <summary>
        /// 模板表体流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 模板表体流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        public string AccoCheckType { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string MenuID { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public bool CheckMark { get; set; }
        public string ModifiedDate { get; set; }

    }

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }

    public class FM_AccoCheckRuleAddCommand : FM_AccoCheckRuleCommand, IRequest<Result>
    {

    }

    public class FM_AccoCheckRuleDeleteCommand : FM_AccoCheckRuleCommand, IRequest<Result>
    {

    }

    public class FM_AccoCheckRuleModifyCommand : FM_AccoCheckRuleCommand, IRequest<Result>
    {
        public bool IsSave { get; set; }
    }

    public class FM_AccoCheckRuleCommand 
    {
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string AccoCheckType { get; set; }
        public string MasterDataSource { get; set; }
        public string MasterFormula { get; set; }
        public string MasterSecFormula { get; set; }
        public string FollowDataSource { get; set; }
        public string FollowFormula { get; set; }
        public string FollowSecFormula { get; set; }
        public string CheckValue { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public bool IsUse { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
    }



}
