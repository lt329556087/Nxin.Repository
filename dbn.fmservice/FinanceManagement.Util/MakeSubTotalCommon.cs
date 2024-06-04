using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Util
{



    public static class SubTotalContainer
    {

        public static char splitChar = '~';

        public static char splitSummaryChar = '=';

        public static float maxSummaryTypeNum = 100f;

        public static string totalPlaceHolder = "小计";

        public static string placeHolder = "总计";

        public static string blankPlaceHolder = "";

        public static string blankValue = "[null]";

        public static IEnumerable<T> AddSubTotal<T>(this IEnumerable<T> data, Func<IEnumerable<T>, T> reduce, ViewModel searchContext) where T : DataResult
        {
            var t = InnerMakeSubTotal(data, reduce);
             t.AddHeaderInfo(searchContext);
            t.FilterNull();
            return t;
        }

        public static IEnumerable<T> AddSubTotal<T>(this IEnumerable<T> data, Func<IEnumerable<T>, T> reduce) where T : DataResult
        {
            return InnerMakeSubTotal(data, reduce);
        }

        public static void AddHeaderInfo<T>(this IEnumerable<T> data, ViewModel searchContext) where T : DataResult
        {
            var headerArray = searchContext.SummaryTypeName.Split(splitSummaryChar);
            foreach (T item in data)
            {
                item.SummaryTypeFieldNameList.Clear();
                for (int i = 0; i < headerArray.Length; i++)
                {
                    item.SummaryTypeFieldNameList.Add(headerArray[i]);
                }
            }
        }

        public static void FilterNull<T>(this IEnumerable<T> data) where T : DataResult
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
        public static IEnumerable<T> InnerMakeSubTotal<T>(IEnumerable<T> data, Func<IEnumerable<T>, T> reduce) where T : DataResult
        {
            data = data.OrderBy(item => item.SummaryType);
            var tempRes = SetOrderId(SplitSummaryFields(data));
            return ComputeSubTotal(tempRes, reduce);
        }

        public static IEnumerable<T> SplitSummaryFields<T>(IEnumerable<T> data) where T : DataResult
        {
            foreach (T item in data)
            {
                var summaryTypes = item.SummaryType.Split(splitChar);
                for (int i = 0; i < summaryTypes.Length; i++)
                    item.SummaryTypeList.Add(summaryTypes[i]);
                var summaryTypeNames = item.SummaryTypeName.Split(splitChar);
                for (int i = 0; i < summaryTypeNames.Length; i++)
                {
                    item.SummaryTypeNameList.Add(summaryTypeNames[i]);
                    item.SummaryTypeFieldNameList.Add("dd");
                }
            }
            return data;
        }

        public static IEnumerable<T> SetOrderId<T>(IEnumerable<T> data) where T : DataResult
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

        public static IEnumerable<T> ComputeSubTotal<T>(IEnumerable<T> tempRes, Func<IEnumerable<T>, T> total) where T : DataResult
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

        public static string GetRowKey<T>(T item, int num) where T : DataResult
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
            bool isSubTotal = true) where T : DataResult
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
    public class ViewModel
    {
        public ViewModel()
        {
            ToAutoReprot = false;
            IsNoValidate = false;
        }

        public string SummaryType { get; set; }

        public string SummaryTypeName { get; set; }
        /// <summary>
        /// 数据来源
        /// 1-结帐数据
        /// 2-实时查询
        /// 3-高级查询
        /// </summary>
        public int DataSource { get; set; }
        /// <summary>
        /// 0为业务系统进入，1为OA菜单进入
        /// </summary>
        public string MenuParttern { get; set; }

        public bool ToAutoReprot { get; set; }
        public string WindModelValue { get; set; }

        #region 配置化报表
        public string index { get; set; }
        public string appId { get; set; }
        /// <summary>
        /// 这个configged是判断2个
        /// 1.当前系统是否启用该报表配置，
        /// 2.当前单位是否启用该报表配置。
        /// </summary>
        public bool IsConfigged { get; set; }
        public List<NameValueDataResult> TableHeadLst { get; set; }
        /// <summary>
        /// 命名空间FULLNAME
        /// </summary>
        public string FullName { get; set; }

        public bool IsNoValidate { get; set; }
        #endregion

    }
    public class NameValueDataResult
    {
        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        public string PID { get; set; }

        public int Level { get; set; }
        public bool IsEnd { get; set; }
    }
    public class DataResult : IComparable<DataResult>
    {
        #region 小计专用字段
        public double iOrder { get; set; }
        public bool IsSubTotal { get; set; }
        #endregion

        public string SummaryType { get; set; }
        public string SummaryTypeName { get; set; }
        public string SummaryTypeFieldName { get; set; }

        public List<string> SummaryTypeList = new List<string>();

        public List<string> SummaryTypeNameList = new List<string>();

        public List<string> SummaryTypeFieldNameList = new List<string>();

        public int CompareTo(DataResult other)
        {
            if (this.iOrder > other.iOrder)
                return 1;
            else if (this.iOrder == other.iOrder)
                return 0;
            return -1;
        }
    }
}
