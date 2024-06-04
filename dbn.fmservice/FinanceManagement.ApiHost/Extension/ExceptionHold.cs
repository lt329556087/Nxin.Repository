using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Extensions
{
    public class ExceptionHold : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = new { ResultState = false, CodeNo = -3, Msg = context.Exception.Message };
            context.ExceptionHandled = true;
            context.Result = new Microsoft.AspNetCore.Mvc.OkObjectResult(result);
        }
    }

}
