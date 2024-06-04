using FinanceManagement.Common.MonthEndCheckout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;

namespace FinanceManagement.Common
{
    public static class SubTotalUtil
    {

        public static char splitChar = '~';

        public static char splitSummaryChar = '=';

        public static float maxSummaryTypeNum = 100f;

        public static string totalPlaceHolder = "小计";

        public static string placeHolder = "总计";

        public static string blankPlaceHolder = "";

        public static string blankValue = "[null]";

        public static IEnumerable<T> AddSubTotal<T>(this IEnumerable<T> data, Func<IEnumerable<T>, T> reduce) where T : RptDataResult
        {
            var t = InnerMakeSubTotal(data, reduce);
            //t.AddHeaderInfo(searchContext);
            t.FilterNull();
            return t;
        }
        //public static IEnumerable<T> AddSubTotal<T>(this IEnumerable<T> data, Func<IEnumerable<T>, T> reduce) where T : RptDataResult
        //{
        //    return InnerMakeSubTotal(data, reduce);
        //}

        //public static void AddHeaderInfo<T>(this IEnumerable<T> data, ReportSummaryModel searchContext) where T : RptDataResult
        //{
        //    var headerArray = searchContext.SummaryTypeName.Split(splitSummaryChar);
        //    foreach (T item in data)
        //    {
        //        item.SummaryTypeFieldNameList.Clear();
        //        for (int i = 0; i < headerArray.Length; i++)
        //        {
        //            item.SummaryTypeFieldNameList.Add(headerArray[i]);
        //        }
        //    }
        //}

        public static void FilterNull<T>(this IEnumerable<T> data) where T : RptDataResult
        {
            foreach (T item in data)
            {
                for (int i = 0; i < item.SummaryTypeList.Count; i++)
                {
                    item.SummaryTypeList[i] = item.SummaryTypeList[i] == blankValue ? string.Empty : item.SummaryTypeList[i];
                    item.SummaryTypeNameList[i] = item.SummaryTypeNameList[i] == blankValue ? string.Empty : item.SummaryTypeNameList[i];
                }
            }
        }

        /// <summary>
        /// 计算小计与合计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static IEnumerable<T> InnerMakeSubTotal<T>(IEnumerable<T> data, Func<IEnumerable<T>, T> reduce) where T : RptDataResult
        {
            data = data.OrderBy(item => item.SummaryType);
            var tempRes = SetOrderId(SplitSummaryFields(data));
            return ComputeSubTotal(tempRes, reduce);
        }

        public static IEnumerable<T> SplitSummaryFields<T>(IEnumerable<T> data) where T : RptDataResult
        {
            foreach (T item in data)
            {
                var summaryTypes = item.SummaryType.Split(splitChar);
                for (int i = 0; i < summaryTypes.Length; i++)
                    item.SummaryTypeList.Add(summaryTypes[i]);
                var summaryTypeNames = item.SummaryTypeName.Split(splitChar);
                var summaryTypeFieldNames = item.SummaryTypeFieldName.Split(splitChar);
                for (int i = 0; i < summaryTypeNames.Length; i++)
                {
                    item.SummaryTypeNameList.Add(summaryTypeNames[i]);
                    item.SummaryTypeFieldNameList.Add(summaryTypeFieldNames[i]);
                }
            }
            return data;
        }

        public static IEnumerable<T> SetOrderId<T>(IEnumerable<T> data) where T : RptDataResult
        {
            var step = 5;
            var index = 1;
            foreach (T item in data)
            {
                item.iOrder = index;
                index += step;
            }
            return data;
        }

        public static IEnumerable<T> ComputeSubTotal<T>(IEnumerable<T> tempRes, Func<IEnumerable<T>, T> total) where T : RptDataResult
        {
            var dataCount = tempRes.Count();
            Action<List<T>, int, T, Dictionary<string, Dictionary<string, List<T>>>, string, string, int> EndRowHandler =
                (totalRows, index, item, cache, cacheIndex, key, i) =>
                {
                    if (index == dataCount - 1)
                        totalRows.Add(GetSubTotalRowFromCacheShardList(item, total, cache, cacheIndex, key, i, true));
                };
            if (dataCount > 0 && tempRes.FirstOrDefault().SummaryTypeList.Count > 0)
            {
                var firstEle = tempRes.FirstOrDefault();
                var totalRows = new List<T>();
                var subTotalCount = firstEle.SummaryTypeList.Count - 1;
                var cache = new Dictionary<string, Dictionary<string, List<T>>>();
                T lastItem = null;
                for (int i = 0; i <= subTotalCount; i++)
                    cache.Add(i.ToString(), new Dictionary<string, List<T>>());
                cache["0"] = new Dictionary<string, List<T>>();
                cache["0"].Add("total", new List<T>());
                var index = -1;
                foreach (T item in tempRes)
                {
                    index++;
                    cache["0"]["total"].Add(item);

                    for (int i = 0; i < subTotalCount; i++)
                    {
                        var cacheIndex = (i + 1).ToString();
                        var key = GetRowKey(item, i);
                        if (cache[cacheIndex].ContainsKey(key))
                        {
                            cache[cacheIndex][key].Add(item);
                            EndRowHandler(totalRows, index, item, cache, cacheIndex, key, i);
                        }
                        else
                        {
                            if (cache[cacheIndex].Keys.Count == 0)
                            {
                                cache[cacheIndex].Add(key, new List<T>() { item });
                                EndRowHandler(totalRows, index, item, cache, cacheIndex, key, i);
                            }
                            else
                            {
                                string lastkey = lastItem == null ? "" : GetRowKey(lastItem, i);
                                totalRows.Add(GetSubTotalRowFromCacheShardList(item, total, cache, cacheIndex, lastkey, i, false));
                                cache[cacheIndex].Clear();
                                cache[cacheIndex].Add(key, new List<T>() { item });
                                EndRowHandler(totalRows, index, item, cache, cacheIndex, key, i);
                            }
                        }

                    }
                    lastItem = item;
                }
                totalRows.Add(GetSubTotalRowFromCacheShardList(tempRes.LastOrDefault(), total, cache, "0", "total", -1, true, false));
                var t = tempRes.Concat(totalRows).ToList();
                t.Sort();
                tempRes = t;
            }
            return tempRes;
        }

        public static string GetRowKey<T>(T item, int num) where T : RptDataResult
        {
            var key = new StringBuilder();
            for (int j = 0; j <= num; j++)
            {
                key.Append(item.SummaryTypeList[j]);
            }
            return key.ToString();
        }

        public static T GetSubTotalRowFromCacheShardList<T>(
            T item,
            Func<IEnumerable<T>, T> total,
            Dictionary<string, Dictionary<string, List<T>>> cache,
            string cacheIndex,
            string key,
            int shardIndex,
            bool isLast,
            bool isSubTotal = true) where T : RptDataResult
        {
            var tempList = cache[cacheIndex][key];
            var _totalPlaceHolder = isSubTotal ? totalPlaceHolder : placeHolder;
            T totalRow = total(tempList);
            totalRow.iOrder = isLast ? item.iOrder + (maxSummaryTypeNum - 1 - shardIndex) / maxSummaryTypeNum + 1 / maxSummaryTypeNum : item.iOrder - shardIndex / maxSummaryTypeNum - 1 / maxSummaryTypeNum;
            var firstOfCache = cache[cacheIndex][key].FirstOrDefault();
            int j = 0;
            for (; j <= shardIndex; j++)
            {
                totalRow.SummaryTypeList.Add(firstOfCache.SummaryTypeList[j]);
                totalRow.SummaryTypeFieldNameList.Add(firstOfCache.SummaryTypeFieldNameList[j]);
                totalRow.SummaryTypeNameList.Add(firstOfCache.SummaryTypeNameList[j]);
            }
            if (j < firstOfCache.SummaryTypeList.Count)
            {
                totalRow.SummaryTypeList.Add(firstOfCache.SummaryTypeList[j]);
                totalRow.SummaryTypeFieldNameList.Add(firstOfCache.SummaryTypeFieldNameList[j]);
                totalRow.SummaryTypeNameList.Add(_totalPlaceHolder);
                j++;
                for (; j < firstOfCache.SummaryTypeList.Count; j++)
                {
                    totalRow.SummaryTypeList.Add(firstOfCache.SummaryTypeList[j]);
                    totalRow.SummaryTypeFieldNameList.Add(firstOfCache.SummaryTypeFieldNameList[j]);
                    totalRow.SummaryTypeNameList.Add(blankPlaceHolder);
                }
            }
            totalRow.IsSubTotal = true;
            return totalRow;
        }
    }   
}
