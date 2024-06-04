using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Util;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{

    public class MS_StockCostSearch
    {
        /// <summary>
        /// 查询期间
        /// </summary>
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        /// <summary>
        /// 上月期间
        /// </summary>
        public string LastBeginDate { get; set; }
        public string LastEndDate { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string EnterpriseIds { get; set; }
        /// <summary>
        /// 汇总方式
        /// </summary>
        public  List<string> SummaryType { get; set; }
        /// <summary>
        /// 商品代号
        /// </summary>
        public string ProductIds { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductGroupIds { get; set; }
        /// <summary>
        /// 商品类别
        /// </summary>
        public string ProductClassIds { get; set; }
        /// <summary>
        /// 存货分类
        /// </summary>
        public string StockTypes { get; set; }
        public string GroupId { get; set; }
    }

    public class MS_StockCostResultModel: DataResult
    {
        public string EnterPriseID { get; set; }
        public string EnterPriseName { get; set; }
        public string cAxis { get; set; }
        public string cFullClassName { get; set; }
        public string ProductGroupID { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ClassificationID1 { get; set; }
        public string ClassificationID2 { get; set; }
        public string ClassificationID3 { get; set; }
        public string ClassificationID4 { get; set; }
        public string ClassificationID5 { get; set; }
        public string ClassificationID6 { get; set; }
        public string ClassificationName1 { get; set; }
        public string ClassificationName2 { get; set; }
        public string ClassificationName3 { get; set; }
        public string ClassificationName4 { get; set; }
        public string ClassificationName5 { get; set; }
        public string ClassificationName6 { get; set; }
        public decimal qcQuantity { get; set; }
        public decimal qcUnitPrice { get; set; }
        public decimal qcAmount { get; set; }
        public decimal inQuantity { get; set; }
        public decimal inUnitPrice { get; set; }
        public decimal inAmount { get; set; }
        public decimal qmQuantity { get; set; }
        public decimal qmUnitPrice { get; set; }
        public decimal qmAmount { get; set; }
        
    }
    public class GroupModel
    {
        public bool EnterPriseID { get; set; }
        public bool ProductGroupID { get; set; }
        public bool ProductID { get; set; }
        public bool ClassificationID1 { get; set; }
        public bool ClassificationID2 { get; set; }
        public bool ClassificationID3 { get; set; }
        public bool ClassificationID4 { get; set; }
        public bool ClassificationID5 { get; set; }
        public bool ClassificationID6 { get; set; }
        public Group GroupBy(MS_StockCostResultModel dym)
        {
            var gro = new Group();
            if (EnterPriseID)
            {
                gro.EnterPriseID = dym.EnterPriseID;
            }
            if (ProductGroupID)
            {
                gro.ProductGroupID = dym.ProductGroupID;
            }
            if (ProductID)
            {
                gro.ProductID = dym.ProductID;
            }
            if (ClassificationID1)
            {
                gro.ClassificationID1 = dym.ClassificationID1;
            }
            if (ClassificationID2)
            {
                gro.ClassificationID2 = dym.ClassificationID2;
            }
            if (ClassificationID3)
            {
                gro.ClassificationID3 = dym.ClassificationID3;
            }
            if (ClassificationID4)
            {
                gro.ClassificationID4 = dym.ClassificationID4;
            }
            if (ClassificationID5)
            {
                gro.ClassificationID5 = dym.ClassificationID5;
            }
            if (ClassificationID6)
            {
                gro.ClassificationID6 = dym.ClassificationID6;
            }
            return gro;
        }
        public struct Group
        {
            public string EnterPriseID { get; set; }
            public string ProductGroupID { get; set; }
            public string ProductID { get; set; }
            public string ClassificationID1 { get; set; }
            public string ClassificationID2 { get; set; }
            public string ClassificationID3 { get; set; }
            public string ClassificationID4 { get; set; }
            public string ClassificationID5 { get; set; }
            public string ClassificationID6 { get; set; }
        }

    }

}
