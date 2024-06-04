using Architecture.Common.HttpClientUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Common
{
    public class ApplySubjectUtil
    {
        HttpClientUtil _httpClientUtil1;

        HostConfiguration _hostCongfiguration;

        public ApplySubjectUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public void AddSubject(SubjectInfo subjectInfo)
        {
            var url = $"{ _hostCongfiguration.QlwServiceHost}/api/FAAuditRecordExtend/AddSubject";
            _httpClientUtil1.PostJsonAsync(url, subjectInfo);
        }
    }

    public class SubjectInfo
    {
        public string NumericalOrder { get; set; }

        public string Subject { get; set; }

        public SubjectExtendInfo SubjectExtendInfo { get; set; }
    }

    public class SubjectExtendInfo
    {
        public decimal Amount { get; set; }

        public string ApplyObject { get; set; }

        public string Number { get; set; }

        public NameValue DateInfo { get; set; }
    }

    public class NameValue
    {
        public string Name { get; set; }

        public string Value { get; set; }

    }
}
