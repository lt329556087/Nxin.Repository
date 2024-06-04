using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FM_CostProject;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FinanceManagement.ApiHost.Controllers.FM_Expensereport
{
    public class FM_ExpenseCalFactory
    {
        private IIdentityService _identityService;
        private QlwCrossDbContext _context;
        private EnterprisePeriodUtil _enterpriseperiodUtil;
        FM_CostProjectODataProvider _queryProvider;

        #region Properties
        /// <summary>
        /// 填报期间
        /// </summary>
        public string ReportPeriod { get; set; }
        /// <summary>
        /// 填报表体数据
        /// </summary>
        public fm_expensereportdetail repoertDetail { get; set; }
        #endregion

        public FM_ExpenseCalFactory(IIdentityService identityService, QlwCrossDbContext context,
                                        EnterprisePeriodUtil enterpriseperiodUtil,
                                        FM_CostProjectODataProvider queryProvider)
        {
            _identityService = identityService;
            _context = context;
            _enterpriseperiodUtil = enterpriseperiodUtil;
            _queryProvider = queryProvider;
        }

        #region Private Method
        /// <summary>
        /// 通过项目ID获取费用项目详细信息
        /// </summary>
        /// <returns></returns>
        private FM_CostProjectEntity GetCostProjectEntity()
        {
            FM_CostProjectEntity main = _queryProvider.GetSingleData(repoertDetail.CostProjectID);
            main.AllocationType = repoertDetail?.AllocationType;
            main.CollectionType = repoertDetail?.CollectionType;
            main.DataSource = repoertDetail?.DataSource;
            return main;
        }

        /// <summary>
        /// 根据归集类型，获取操作数据对象
        /// </summary>
        /// <param name="CollectionType"></param>
        /// <returns></returns>
        private FM_ExpenseBaseCalculate GetOperateModel(string CollectionType)
        {
            FM_ExpenseBaseCalculate opt = new FM_ExpenseOtherCal(_identityService,_context,_enterpriseperiodUtil);
            if (CollectionType == "202202111355001102")//部门
            {
                opt = new FM_ExpenseMarketCal(_identityService, _context, _enterpriseperiodUtil);
            }
            return opt;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 获取凭证发生数据
        /// </summary>
        /// <returns></returns>
        public List<FM_ExpenseCalEntity> GetFDSettlereceiptData()
        {
            List<FM_ExpenseCalEntity> result = new List<FM_ExpenseCalEntity>();
            FM_CostProjectEntity entity = GetCostProjectEntity();
            FM_ExpenseBaseCalculate optModel = GetOperateModel(entity.CollectionType);
            optModel.ReportPeriod = this.ReportPeriod;

            //多行取数逻辑
            entity.Details.ForEach(a=> {
                optModel.AccoSubjectID = a.RelatedId;
                optModel.ExtendDetails = a.ExtendDetails;
                optModel.DataFormula = a.DataFormula;
                result.AddRange(optModel.GetResults());
            });

            //汇总数据
            var groupresult = result.GroupBy(_ => new { _.MarketID, _.PigFarmID }).Select(_=>new FM_ExpenseCalEntity() { MarketID=_.Key.MarketID,PigFarmID=_.Key.PigFarmID,Amount=_.Sum(a=>a.Amount)}).ToList();
            return groupresult;
        }

        /// <summary>
        /// 获取猪联网汇总数据
        /// </summary>
        /// <returns></returns>
        public List<FM_PigGroupDataEntity> GetPigGroupData(string collectionType,string allocationType)
        {
            List<FM_PigGroupDataEntity> result = new List<FM_PigGroupDataEntity>();
            FM_ExpenseBaseCalculate optModel = GetOperateModel(collectionType);
            optModel.ReportPeriod = this.ReportPeriod;

            result = optModel.GetPigGroupData(collectionType, allocationType);
            return result;
        }

        public Dictionary<string, string> PackageEnterprisePeriod(string reportPeriod,string enterpriseId)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DateTime dt = DateTime.Parse(reportPeriod.Replace('.','-'));
            var BeginDate = new DateTime(dt.Year, dt.Month, 1).ToString("yyyy-MM-dd");
            var EndDate = new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            var result = _enterpriseperiodUtil.GetEnterperisePeriodList(enterpriseId, dt.Year, dt.Month);
            if (result != null && result.Count > 0)
            {
                BeginDate = result.First().StartDate.ToString("yyyy-MM-dd");
                EndDate = result.First().EndDate.ToString("yyyy-MM-dd");
            }
            dic.Add("BeginDate", BeginDate);
            dic.Add("EndDate", EndDate);
            return dic;
        }

        /// <summary>
        /// 获取凭证发生数据-问题追溯
        /// </summary>
        /// <returns></returns>
        public List<FM_ExpenseReportDetailLogsEntity> GetFDSettlereceiptErrorData()
        {
            List<FM_ExpenseReportDetailLogsEntity> result = new List<FM_ExpenseReportDetailLogsEntity>();
            FM_CostProjectEntity entity = GetCostProjectEntity();
            FM_ExpenseBaseCalculate optModel = GetOperateModel(entity.CollectionType);
            optModel.ReportPeriod = this.ReportPeriod;

            //多行取数逻辑
            entity.Details.ForEach(a => {
                optModel.AccoSubjectID = a.RelatedId;
                optModel.ExtendDetails = a.ExtendDetails;
                optModel.DataFormula = a.DataFormula;
                result.AddRange(optModel.DetailLogResults());
            });

            return result;
        }
        #endregion
    }
}
