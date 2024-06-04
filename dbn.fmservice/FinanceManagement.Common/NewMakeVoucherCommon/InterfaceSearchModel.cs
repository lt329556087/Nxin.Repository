using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    /// <summary>
    /// 请求参数类
    /// </summary>
    public class RequestModel
    {
        /// <summary>
        /// 汇总方式 SummaryType 集合的形式
        /// </summary>
        private List<string> summaryTypeList;

        /// <summary>
        /// 汇总方式用"="分割
        /// </summary>
        public string SummaryType { get; set; }
        public string SummaryTypeName { get; set; }

        /// <summary>
        /// 集团ID
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public long EnterpriseId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long BoId { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public int MenuParttern { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { get; set; }



        /// <summary>
        /// 是否合并单位
        /// </summary>
        public bool OnlyCombineEnte { get; set; }

        /// <summary>
        /// 逗号分割的单位ID
        /// </summary>
        public string EnterpriseList { get; set; }

        /// <summary>
        /// 业务单元/部门组织
        /// </summary>
        public string MarketOrgs { get; set; }

        /// <summary>
        /// 逗号分割的部门ID
        /// </summary>
        public string Markets { get; set; }

        /// <summary>
        /// 员工
        /// </summary>
        public string Personnels { get; set; }

        /// <summary>
        /// 已拥用的权限单位
        /// </summary>
        public List<string> OwnEntes { get; set; }

        /// <summary>
        /// 菜单Id
        /// </summary>
        public long APPID { get; set; }

        /// <summary>
        ///  是否查询结账数据 0为实时 1为结账
        /// </summary>
        public int DataSource { get; set; }

        /// <summary>
        ///  企店 0不是 1是
        /// </summary>
        public int QiDian { get; set; }

        /// <summary>
        /// 会计期间开始日期
        /// </summary>
        public DateTime APBeginDate { get; set; }

        /// <summary>
        /// 会计期间结束日期
        /// </summary>
        public DateTime APEndDate { get; set; }

        
    }
    public class SaleSummaryRequest : RequestModel
    {
        public string EnteID { get; set; }
        public string EnteIdForOtherReport { get; set; }
        /// <summary>
        /// //销售摘要
        /// </summary>
        public string SalesAbstract { get; set; }

        /// <summary>
        /// 商品分类 商品名称 商品代号
        /// </summary>
        public string ProductLst { get; set; }

        /// <summary>
        /// 客户类别
        /// </summary>
        public string CustomerCateId { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerCategory_id { get; set; }

        /// <summary>
        /// 上级客户Id
        /// </summary>
        public string SuperiorCustomer_id { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public string MarketID { get; set; }

        /// <summary>
        /// 计量单位: 0-本地计量; 1-标准计量单位
        /// </summary>
        public string MeasurementUnit { get; set; }

        /// <summary>
        /// 销售类型
        /// 1-销售
        /// 2-赠送
        /// 3-全部
        /// </summary>
        public string IsGift { get; set; }

        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPoint { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public string ReceiptType { get; set; }

        /// <summary>
        /// 仓库
        /// </summary>
        public string Warehouse_id { get; set; }

        /// <summary>
        /// 业务人员ID
        /// </summary>
        public string BussinessMan_id { get; set; }

        /// <summary>
        /// 销量及单价按照件数显示  1 为显示，其他的不启用
        /// </summary>
        public string JianShuXianShi { get; set; }

        /// <summary>
        /// 猪联网
        /// </summary>
        public bool IsPig { get; set; }

        /// <summary>
        /// 是否使用标吨
        /// </summary>
        public string IsUseStandardTons { get; set; }

        #region 手机端 销售战报分页用

        /// <summary>
        /// 是否指定商品
        /// </summary>
        public bool IsSpecialProuct { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string SecialProductID { get; set; }

        #endregion
    }
    public class FM_CarryForwardVoucherSearchCommand
    {
        public string SettleReceipType { get; set; }
        public string CurrentEnterDate { get; set; }
        public string Begindate { get; set; }
        public string Enddate { get; set; }
        public string NumericalOrderList { get; set; }
        public string EnterpriseID { get; set; }
        public string GroupID { get; set; }
        public string Boid { get; set; }
        public string NumericalOrder { get; set; }
        public string DataDate { get; set; }
        /// <summary>
        /// 是否当前操作日期生成凭证
        /// </summary>
        public bool IsCurrentData { get; set; }
    }

    public class MakeVoucherModel: FM_CarryForwardVoucherSearchCommand
    {
        /// <summary>
        /// 多个凭证方案生成一张凭证
        /// </summary>
        public bool IsMultiple { get; set; }

        public bool IsSettleCheckOut { get; set; }
        /// <summary>
        /// 会计期间结束日期
        /// </summary>
        public DateTime PeriodEndDate { get; set; }
    }

    public class RptSearchModel {
        public string Text { get; set; }
        public string Value { get; set; }

    }
    public class PurchaseSummaryRequest : R2PRequest
    {

        public string EnteIdFromOtherReport { get; set; }
        public string Begindate { get; set; }
        public string Enddate { get; set; }
        public bool OnlyCombineEnte { get; set; }
        /// <summary>
        /// 判断是否选得是组织
        /// </summary>
        public bool IsEnterCate { get; set; }
        public string EnterpriseList { get; set; }
        //销售摘要

        /// <summary>
        /// 商品分类
        /// </summary>
        public string ProductCode { get; set; }
        public string ProductLst { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }
        public string SupplierName_id { get; set; }
        //public long GroupID { get;  set; }
        /// <summary>
        /// 单据摘要
        /// </summary>
        public string PurchaseAbstract { get; set; }
        /// <summary>
        /// 只能为0本地计量 1标准计量 默认本地
        /// </summary>
        public int MeasurementUnit { get; set; }
        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPoint { get; set; }
        /// <summary>
        /// 日期规则
        /// </summary>
        public string DateRules { get; set; }
        /// <summary>
        /// 单据类型
        /// </summary>
        public string ReceiptType { get; set; }

        public int QiDian { get; set; }
        /// <summary>
        /// 0显示 1 不显示
        /// </summary>
        public string isShowFaPiao { get; set; }
        /// <summary>
        /// 入库仓库
        /// </summary>
        public string ReceiveWarehouse_id { get; set; }
        #region 手机端 采购战报分页用
        public bool IsSpecialProuct { get; set; }
        public string secialProductID { get; set; }
        #endregion
    }

    public class CheckenSummaryRequest : R2PRequest
    {

        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string SummaryType { get; set; }
        public string SummaryTypeName { get; set; }
        public string TableShow { get; set; }
        public string EnterpriseID { get; set; }
        public string BatchID { get; set; }
        public string BreedingID { get; set; }
        public string ChickenHouseID { get; set; }
       
    }
    public class SuppliesModelForRequeset : R2PRequest
    {
        /// <summary>
        /// 是否财务审核
        /// </summary>
        public bool IsCwAudit { get; set; }
        /// <summary>
        /// 起始时间
        /// </summary>
        public string Begindate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string Enddate { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnteNameLst { get; set; }
        /// <summary>
        /// 是否合并单位，此实体中用不到该属性
        /// </summary>
        public bool OnlyCombineEnte { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public string ProductTreeLst { get; set; }


        public bool IsCompanyManager { get; set; }
        public string MarketsUnderUser { get; set; }
        public bool HasReportAuth { get; set; }

        /// <summary>
        /// 判断是否选得是组织
        /// </summary>
        public bool IsEnterCate { get; set; }
        /// <summary>
        /// 单位集合
        /// </summary>
        public string EnterpriseList { get; set; }

        /// <summary>
        /// 商品分类
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// 商品 集合
        /// </summary>
        public string ProductLst { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTreeName { get; set; }

        /// <summary>
        /// 存货分类
        /// </summary>
        public string CunHuoFenLei { get; set; }
        /// <summary>
        /// 客户类别 CustomerCategory
        /// </summary>
        public string CustomerCategory { get; set; }
        public string CustomerCategory_id { get; set; }

        /// <summary>
        /// 审核 
        /// 已审核1，
        /// 未审核2，
        /// 全部3
        /// </summary>
        public int IsAudit { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public string SummarySortField { get; set; }
        /// <summary>
        /// 排序字段是否降序
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// 日期规则
        /// </summary>
        public string DateRules { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        public string warehouse_id { get; set; }
        public string warehouseName_id { get; set; }
        /// <summary>
        /// 业务人员
        /// </summary>
        public string BussinessMan_Name { get; set; }
        public string BussinessMan_id { get; set; }


        /// <summary>
        /// 领用部门
        /// </summary>
        public string MarketName { get; set; }
        /// <summary>
        /// 领用部门
        /// </summary>
        public string MarketName_id { get; set; }


        public string IsDepartTreeCate { get; set; }
        public string DepartList { get; set; }
        public string DepartTreeInputName { get; set; }


        public string StoreType { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string GiveCustomer { get; set; }

        /// <summary>
        /// 物资地址
        /// </summary>
        public string WarehouseIdAddress { get; set; }
        /// <summary>
        /// 出库方式
        /// </summary>
        public string OutInWarehouseType { get; set; }
        /// <summary>
        /// 是否去详细列表页
        /// </summary>
        public bool IsToDetail { get; set; }


        /// <summary>
        /// 供应商idList
        /// </summary>
        public string SupplierName_id { get; set; }


        /// <summary>
        /// 一级项目
        /// </summary>
        public string OutInprojectSelectListType { get; set; }

        /// <summary>
        /// 二级项目
        /// </summary>
        public string OutInprojectSelectListTypeTwo { get; set; }
        /// <summary>
        /// 是否开启审批
        /// </summary>
        public bool IsOpenFavor { get; set; }


        /// <summary>
        /// 猪场
        /// </summary>
        public string pigAddressTreeName { get; set; }
        /// <summary>
        /// 猪场
        /// </summary>
        public string pigAddressLst { get; set; }


        /// <summary>
        /// 养户
        /// </summary>
        public string breedManTreeName { get; set; }
        /// <summary>
        /// 养户
        /// </summary>
        public string breedManLst { get; set; }

        public string batchLst { get; set; }
        public string batchTreeName { get; set; }


        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPointSelectList { get; set; }

        /// <summary>
        /// 查询新方法 是否启用结账数据
        /// </summary>
        public bool IsUseNewMethod { get; set; }
        public bool IsNotUseNewMethodAndQc { get; set; }
        /// <summary>
        /// 查询新方法  会计期间起始时间
        /// </summary>
        public string SelectNewMethodUseStartDate { get; set; }
        /// <summary>
        /// 查询新方法 会计期间结束时间
        /// </summary>
        public string SelectNewMethodUseEndDate { get; set; }
        /// <summary>
        /// 查询新方法 请求的开始时间临时 
        /// </summary>
        public DateTime SelectNewMethodUseTempBeginDate { get; set; }
        /// <summary>
        /// 查询新方法  是否跨月
        /// </summary>
        public bool SelectNewMethodIsStepMonth { get; set; }
        /// <summary>
        /// 查询新方法  跨月起始时间
        /// </summary>
        public string SelectNewMethodStepMonthUseStartDate { get; set; }
        /// <summary>
        /// 查询新方法 跨月结束时间
        /// </summary>
        public string SelectNewMethodStepMonthUseEndDate { get; set; }


        /// <summary>
        /// 系统选项：启用物品出库金额用“全月加权平均法”核算       20190319132324
        /// </summary>
        private string _IsOpenAddWeight { get; set; }
        public string IsOpenAddWeight
        {
            get
            {
                if (string.IsNullOrEmpty(_IsOpenAddWeight))
                {
                    return "0";
                }
                return _IsOpenAddWeight;
            }
            set
            {
                _IsOpenAddWeight = value;
            }
        }

        /// <summary>
        /// 系统选项：启用物品结账    ID：20181213092354
        /// </summary>
        private string _IsOpenEndMoneyOption { get; set; }
        public string IsOpenEndMoneyOption
        {
            get
            {
                if (string.IsNullOrEmpty(_IsOpenEndMoneyOption))
                {
                    return "0";
                }
                return _IsOpenEndMoneyOption;
            }
            set
            {
                _IsOpenEndMoneyOption = value;
            }
        }


        public string groupByStr { get; set; }
        /// <summary>
        /// 是否结账数操作
        /// </summary>
        public bool IsMonthFinish { get; set; }

        /// <summary>
        /// 期初查询的是否结账数据
        /// </summary>
        public bool IsQueryMonthFinishData { get; set; }

        /// <summary>
        /// 制单人集合
        /// </summary>
        public string Owners { get; set; }
    }
    public interface IR2PRequest
    {
    }
    public class R2PRequest : IR2PRequest
    {
        /// <summary>
        ///  //是否查询结账数据
        ///  0为实时
        ///  1为结账
        /// </summary>
        public int DataSource { get; set; }
        /// <summary>
        /// 汇总方式1，2，3
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        public List<string> OwnEntes { get; set; }
        public List<string> GetZBOwnEntes()
        {
            List<string> strLst = new List<string>() {
            "634086739144000981","634086739144000461","634086739144001161"};
            return strLst;
        }
        public List<string> CanWatchEntes { get; set; }
        /// <summary>
        /// 用于产业汇总用
        /// </summary>
        public string SummaryT1Rank { get; set; }
        public string SummaryT2Rank { get; set; }
        public string SummaryT3Rank { get; set; }
        /// <summary>
        /// 判断是否用的是产业/事业部/核算单元的方式汇总
        /// </summary>
        public bool IsGroupByEnteCate { get; set; }
        /// <summary>
        /// 专门针对 2019年3月25-28日提出来的 核算单元 创业单位 organizationID 进行判断的
        /// </summary>
        public bool[] IsGroupByChuangYeDanYuan { get; set; }
        public string EnteCateSummary
        {
            get
            {
                var sum = string.Empty;
                if (!string.IsNullOrEmpty(SummaryType1) && this.SummaryType1.Contains("EnteCate_"))
                {
                    var split = SummaryType1.Split('_')[2];
                    sum += split + "_";
                }
                if (!string.IsNullOrEmpty(SummaryType2) && SummaryType2.Contains("EnteCate_"))
                {
                    var split = SummaryType2.Split('_')[2];
                    sum += split + "_";
                }
                if (!string.IsNullOrEmpty(SummaryType3) && SummaryType3.Contains("EnteCate_"))
                {
                    var split = SummaryType3.Split('_')[2];
                    sum += split + "_";
                }
                return sum;
            }
        }
        public long GroupID { get; set; }
        public long EnteID { get; set; }
        public string Boid { get; set; }
        /// <summary>
        /// 0为业务系统进入用enteid，1为OA菜单进入用groupid
        /// </summary>
        public string MenuParttern { get; set; }
        //public string CustomerField { get; set; }

        #region 权限使用
        /// <summary>
        /// 某一用户下负责的部门。 第一类人
        /// </summary>
        public string MarketsUnderUser { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public string Orgs { get; set; }

        /// <summary>
        /// 是否是公司负责人 第二类人
        /// </summary>
        public bool IsCompanyManager { get; set; }
        /// <summary>
        /// 是否有报表权限， 第三类人
        /// </summary>
        public bool HasReportAuth { get; set; }
        #endregion

    }

    public  class FM_VoucherAmortizationRecord
    {
        public string NumericalOrderVoucher { get; set; }
        public string NumericalOrderSettl { get; set; }
        public string AmortizationName { get; set; }
        public string OwnerID { get; set; }
        public string ImplementResult { get; set; }
        public bool ResultState { get; set; }
        public string EnterpriseID { get; set; }

    }

    public class FM_VoucherAmortizationRelated
    {
        public string NumericalOrderVoucher { get; set; }
        public string NumericalOrderSettl { get; set; }
        public string NumericalOrderStay { get; set; }
        public string NumericalOrderInto { get; set; }
        public decimal VoucherAmount { get; set; }

    }
   
}
