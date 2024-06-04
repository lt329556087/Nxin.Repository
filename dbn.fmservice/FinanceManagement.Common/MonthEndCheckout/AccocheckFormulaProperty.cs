using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Common.MonthEndCheckout
{
    public class AccocheckFormulaProperty
    {
        public string FormulaID { get; set; }
        public string FormulaName { get; set; }
        public string FormulaValue { get; set; }
        public string FormulaPid { get; set; }
        public List<AccocheckFormulaProperty> GetFormulaProperty()
        {
            List<AccocheckFormulaProperty> list = new List<AccocheckFormulaProperty>();
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000102", FormulaName = "本期借方", FormulaPid = AccocheckDataSource.往来余额表.GetValue().ToString(), FormulaValue = "Debit" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000103", FormulaName = "本期贷方", FormulaPid = AccocheckDataSource.往来余额表.GetValue().ToString(), FormulaValue = "Credit" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000104", FormulaName = "期末余额", FormulaPid = AccocheckDataSource.往来余额表.GetValue().ToString(), FormulaValue = "LastBalance" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000105", FormulaName = "本期借方", FormulaPid = AccocheckDataSource.科目余额表.GetValue().ToString(), FormulaValue = "Debit" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000106", FormulaName = "本期贷方", FormulaPid = AccocheckDataSource.科目余额表.GetValue().ToString(), FormulaValue = "Credit" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000107", FormulaName = "期末余额", FormulaPid = AccocheckDataSource.科目余额表.GetValue().ToString(), FormulaValue = "LastBalance" });//IsLorR ? Show_LastCredit - Show_LastDebit : Show_LastDebit - Show_LastCredit
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000108", FormulaName = "销售数量", FormulaPid = AccocheckDataSource.销售汇总表.GetValue().ToString(), FormulaValue = "SalesAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000109", FormulaName = "销售成本", FormulaPid = AccocheckDataSource.销售汇总表.GetValue().ToString(), FormulaValue = "SalesCost" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000110", FormulaName = "销售净额", FormulaPid = AccocheckDataSource.销售汇总表.GetValue().ToString(), FormulaValue = "SalesAmountNet" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000111", FormulaName = "合计", FormulaPid = AccocheckDataSource.折扣汇总表.GetValue().ToString(), FormulaValue = "SubTotal" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000112", FormulaName = "入库数量", FormulaPid = AccocheckDataSource.采购汇总表.GetValue().ToString(), FormulaValue = "PurchaseQuantity" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000113", FormulaName = "不含税采购金额", FormulaPid = AccocheckDataSource.采购汇总表.GetValue().ToString(), FormulaValue = "AmountWithoutTax" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000114", FormulaName = "实际运费", FormulaPid = AccocheckDataSource.运费汇总表.GetValue().ToString(), FormulaValue = "Amount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000115", FormulaName = "本期入库数量", FormulaPid = AccocheckDataSource.存货汇总表.GetValue().ToString(), FormulaValue = "InboundDeliveryQuantity" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000116", FormulaName = "本期出库数量", FormulaPid = AccocheckDataSource.存货汇总表.GetValue().ToString(), FormulaValue = "OutboundDeliveryQuantity" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000117", FormulaName = "期末结存数量", FormulaPid = AccocheckDataSource.存货汇总表.GetValue().ToString(), FormulaValue = "EndingQuantity" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000118", FormulaName = "期末结存金额", FormulaPid = AccocheckDataSource.物品明细表.GetValue().ToString(), FormulaValue = "qcAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000119", FormulaName = "期末数量", FormulaPid = AccocheckDataSource.成本汇总表.GetValue().ToString(), FormulaValue = "qmQuantity" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000120", FormulaName = "销售成本", FormulaPid = AccocheckDataSource.成本汇总表.GetValue().ToString(), FormulaValue = "xsAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000121", FormulaName = "采购成本", FormulaPid = AccocheckDataSource.成本汇总表.GetValue().ToString(), FormulaValue = "cgAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000122", FormulaName = "原材料期末成本", FormulaPid = AccocheckDataSource.成本汇总表.GetValue().ToString(), FormulaValue = "qmAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000123", FormulaName = "包装物期末成本", FormulaPid = AccocheckDataSource.成本汇总表.GetValue().ToString(), FormulaValue = "qmAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000124", FormulaName = "半成品期末成本", FormulaPid = AccocheckDataSource.成本汇总表.GetValue().ToString(), FormulaValue = "qmAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000125", FormulaName = "产成品期末成本", FormulaPid = AccocheckDataSource.成本汇总表.GetValue().ToString(), FormulaValue = "qmAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000126", FormulaName = "本期折旧", FormulaPid = AccocheckDataSource.折旧汇总表.GetValue().ToString(), FormulaValue = "DepreciationMonthAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000127", FormulaName = "累计折旧", FormulaPid = AccocheckDataSource.固定资产卡片报表.GetValue().ToString(), FormulaValue = "DepreciationAccumulated" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000128", FormulaName = "资产原值", FormulaPid = AccocheckDataSource.固定资产卡片报表.GetValue().ToString(), FormulaValue = "OriginalValue" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000129", FormulaName = "期末金额", FormulaPid = AccocheckDataSource.资金汇总表.GetValue().ToString(), FormulaValue = "EnddingAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000130", FormulaName = "应收净额", FormulaPid = AccocheckDataSource.应收账款汇总表.GetValue().ToString(), FormulaValue = "NetReceivableAccount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000131", FormulaName = "应付账款", FormulaPid = AccocheckDataSource.应付账款汇总表.GetValue().ToString(), FormulaValue = "ReceiptAmount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000132", FormulaName = "期末原值", FormulaPid = AccocheckDataSource.成本变动汇总表.GetValue().ToString(), FormulaValue = "qmOrginalValue" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000133", FormulaName = "期末累计折旧", FormulaPid = AccocheckDataSource.成本变动汇总表.GetValue().ToString(), FormulaValue = "qmAccumulatedDepreciation" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000134", FormulaName = "期末待摊费用", FormulaPid = AccocheckDataSource.成本变动汇总表.GetValue().ToString(), FormulaValue = "qmPrepaidExpenses" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000135", FormulaName = "期末总成本", FormulaPid = AccocheckDataSource.猪场成本明细表.GetValue().ToString(), FormulaValue = "pigCostAmount" });//直接取数
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000136", FormulaName = "存货调整", FormulaPid = AccocheckDataSource.养户成本汇总表.GetValue().ToString(), FormulaValue = "AdjustCount" });
            list.Add(new AccocheckFormulaProperty() { FormulaID = "1905141116550000137", FormulaName = "期末总成本", FormulaPid = AccocheckDataSource.养户成本汇总表.GetValue().ToString(), FormulaValue = "EndTotalAmount" });
            return list;
        }
    }
}
