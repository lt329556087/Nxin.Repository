using Architecture.Seedwork.Core;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace FinanceManagement.Common
{
    public class ResultModel<TModel>
    {
        public bool ResultState { get; set; }
        public int CodeNo { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public List<TModel> Data { get; set; }
        public List<TModel> value { get; set; }
    }
    public class ResultModel
    {
        public bool ResultState { get; set; }
        public int CodeNo { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
        public string ErrMsg { get; set; }
    }
    public class IntermediaryRequest
    {
        public string type { get; set; }
        public string url { get; set; }
        public dynamic param { get; set; }
        public string cookie { get; set; }
    }
    public class CheckResult
    {
        public List<ResultModel> ResultList { get; set; }
        public Result Result { get; set; }
    }
}
