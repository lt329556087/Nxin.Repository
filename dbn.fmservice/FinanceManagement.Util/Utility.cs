using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Util
{
    public static class Utility
    {
       /// <summary>
       /// 测试
       /// </summary>
       /// <param name="curDay"></param>
       /// <returns></returns>














        #region 计算为第几周：-WeekOfYear(DateTime curDay)
        public static int WeekOfYear(DateTime curDay)
        {
            System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(curDay, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }
        #endregion

        #region 计算每周第一天：CalculateFirstDateOfWeek(DateTime someDate)
        public static DateTime CalculateFirstDateOfWeek(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        #endregion

        #region 计算每周最后一天：CalculateFirstDateOfWeek(DateTime someDate)
        public static DateTime CalculateLastDateOfWeek(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;// 因为枚举原因，Sunday排在最前，相减间隔要被7减。
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts).AddDays(1).Date.AddSeconds(-1);
        }
        #endregion
    }
}
