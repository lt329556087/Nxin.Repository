using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FinanceManagement.Common
{
    public class EnterprisePeriodUtil
    {
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;

        public EnterprisePeriodUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public EnterperisePeriod GetEnterperisePeriod(string enterpriseId, int year, int month)
        {
            string url = $"{_hostCongfiguration.QlwServiceHost}/api/BIZEnterprise/getEnterprisePeriod";
            var list = _httpClientUtil1.PostJsonAsync<ResultModel<EnterperisePeriod>>(url, new { enterpriseId, year, month }).Result;
            return list.Data.FirstOrDefault();
        }

        public EnterperisePeriod GetEnterperisePeriod(string enterpriseId, string Datadate)
        {
            string url = $"{_hostCongfiguration.QlwServiceHost}/api/BIZEnterprise/getEnterprisePeriod";
            var result = _httpClientUtil1.PostJsonAsync<ResultModel<EnterperisePeriod>>(url, new { enterpriseId, Datadate }).Result;
            return result.Data.FirstOrDefault();
        }

        public EnterperisePeriod GetPreEnterprisePeriod(string enterpriseId, int year, int month)
        {
            int preYear, preMonth;
            if (month == 1)
            {
                preYear = year - 1;
                preMonth = 12;
            }
            else
            {
                preYear = year;
                preMonth = month - 1;
            }
            var period = GetEnterperisePeriod(enterpriseId, preYear, preMonth);
            return period;
        }

        public List<EnterperisePeriod> GetEnterperisePeriodList(string enterpriseId, int year, int month)
        {
            string url = $"{_hostCongfiguration.QlwServiceHost}/api/BIZEnterprise/getEnterprisePeriod";
            var list = _httpClientUtil1.PostJsonAsync<ResultModel<EnterperisePeriod>>(url, new { enterpriseId, year, month }).Result.Data;
            return list;
        }
    }

    public class EnterperisePeriod
    {
        /// <summary>
        /// 单位组织ID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 年
        /// </summary>		
        public int Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>		
        public int Month { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>		
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>		
        public DateTime EndDate { get; set; }

    }

}
