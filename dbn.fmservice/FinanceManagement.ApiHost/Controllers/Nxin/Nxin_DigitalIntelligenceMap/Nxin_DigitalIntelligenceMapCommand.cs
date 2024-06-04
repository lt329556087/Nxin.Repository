using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.Nxin_DigitalIntelligenceMap
{

    public class Nxin_DigitalIntelligenceMapAddCommand : Nxin_DigitalIntelligenceMapCommand, IRequest<Result>
    {

    }

    public class Nxin_DigitalIntelligenceMapDeleteCommand : Nxin_DigitalIntelligenceMapCommand, IRequest<Result>
    {

    }

    public class Nxin_DigitalIntelligenceMapModifyCommand : Nxin_DigitalIntelligenceMapCommand, IRequest<Result>
    {
    }
    public class Nxin_DigitalIntelligenceMapCommand 
    {
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        /// <summary>
        /// 地图类型
        /// </summary>
        public string MapType { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string BackgroundValue { get; set; }
        /// <summary>
        /// 区块配置信息
        /// </summary>
        public string BlockList { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

    }


    public class MutilLineCommand<TLineCommand1> : CommonOperate where TLineCommand1 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
    }

}
