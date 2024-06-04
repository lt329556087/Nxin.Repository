using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FM_TicketedPoint
    {

        /// <summary>
        /// 字号（开票点）ID
        /// </summary>		
        public string TicketedPointID { get; set; }

        /// <summary>
        /// 字号（开票点）名称
        /// </summary>		
        public string TicketedPointName { get; set; }

        /// <summary>
        /// 字号（开票点）编号
        /// </summary>		
        public string TicketedPointCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// 组织/单位ID
        /// </summary>		
        public long EnterpriseID { get; set; }
    }
    public class Biz_EnterpriseInfo
    {
        public string Pid { get; set; }
        [Key]
        public string EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
    }
    public class BIZ_OptionConfig
    {

        public long OptionID { get; set; }
        public string OptionName { get; set; }
        public string OptionValue { get; set; }
        public string Remarks { get; set; }
        public long EnterpriseID { get; set; }
        public long OwnerID { get; set; }
    }
    /// <summary>
    /// 新版2023年6月20日 11:26:33  系统选项json
    /// </summary>
    public class OptionConfigNew
    {
        /// <summary>
        /// 
        /// </summary>
        public int OptionSwitch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OptionText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsForce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OptionValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PForce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OptionType { get; set; }
    }
    public class ReceiptAbstract
    {
        public string ReceiptAbstractID { get; set; }
        public string ReceiptAbstractIDs { get; set; }
        public string ReceiptAbstractName { get; set; }
        public string SettleSummaryGroupCode { get; set; }
        public int SettleSummaryID { get; set; }
        public string SettleSummaryName { get; set; }
        public string AccoSubjectID { get; set; }
        public string AccoSubjectCode { get; set; }
        public string EnterpriseID { get; set; }
    }
    public class Biz_Settlesummary
    {
        public string id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public string pid { get; set; }
        public int Rank { get; set; }
        public string SettleSummaryGroupAbstract { get; set; }
        public string SettleSummaryGroupType { get; set; }
        public int IsGroup { get;set; }
        public string IdKey { get; set; }
        public string PidKey { get; set; }
    }
    public class Subject
    {
        public Subject()
        {
            Rank = 1;
        }
        public string AccoSubjectID { get; set; }
        //实体类和接口的科目编号
        public string cAccoSubjectCode { get; set; }
        //表的科目编号
        public string AccoSubjectCode { get; set; }
        public string cAccoSubjectName { get; set; }
        public string cAccoSubjectFullName { get; set; }
        public long AccoSubjectSystemID { get; set; }
        public bool bTorF { get; set; }
        public bool bLorR { get; set; }
        public bool bEnd { get; set; }
        public bool bProject { get; set; }
        public bool bCus { get; set; }
        public bool bPerson { get; set; }
        public bool bSup { get; set; }
        public bool bDept { get; set; }
        public bool bItem { get; set; }
        public bool bCash { get; set; }
        public int Rank { get; set; }
        public bool bBank { get; set; }
        public string PID { get; set; }

        public string AccoSubjectType { get; set; }
        public string cAxis { get; set; }
        /// <summary>
        /// 辅助核算
        /// </summary>
        public string SubsidiaryAccounting { get { 
                string str = "";
                str = (bDept ? "部门/" : "") + (bPerson ? "人员/" : "") + (bCus ? "客户/" : "") + (bSup ? "供应商/" : "") + (bProject ? "项目/" : "");
                if (!string.IsNullOrEmpty(str))
                {
                    str = str.Substring(0,str.Length-1);
                }
                return str; } }
    }
    public class Biz_Subject
    {
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectId { get; set; }
        /// <summary>
        /// 资产
        /// </summary>
        public string AccoSubjectName { get; set; }
        /// <summary>
        /// 资产
        /// </summary>
        public string AccoSubjectFullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectSystemId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectClassId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectClassName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExpenseType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Rank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsTorF { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsLorR { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsEnd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsProject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsCus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsSup { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsDept { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsItem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsCash { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsBank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsUse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int BDel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CAxis { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VersionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsGroup { get; set; }
        public string IdKey { get; set; }
        public string PidKey { get; set; }
    }
    /// <summary>
    /// 单位
    /// </summary>
    public class Biz_Enterprise
    {
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public int IsUse { get; set; }
    }
    public class SortType
    {
        public int  SortID { get; set; }
        public string SortName { get; set; }
        public string Type { get; set; }
    }
    public class DropSelectSearch:SortType//可任意增加字段
    {
        public string DataDate { get; set; }
        public string relationId { get; set; }
        public string Boid { get; set; }
        public string EnterpriseID { get; set; }
        public string CurrentEnterDate { get; set; }
        /// <summary>
        /// 入参参数ID
        /// </summary>
        public string InEnterpriseID { get; set; }
        public int? Rank { get; set; }
        public string GroupID { get; set; }
        public string PID { get; set; }
        public bool? IsEnd { get; set; } = false;
        public string TransferAccountsType { get; set; }

    }
    public enum SortTypeEnum
    {
        商品分类=1001,
        供应商 = 1002,
        物品分类=1003,
        部门 = 1004,
        人员 = 1005,
        鸡场 = 1007,
        品种 = 1008,
        批次 = 1009,
        厂区 = 1010,
        鸡舍 = 1011,
        销售摘要 =1013,
        猪场 = 1014,
        商品代号 = 1015,
        耳号或批次 = 1016,
        费用性质=1017,
        采购摘要 = 1018,
        猪只类型 = 1019,
        资产类别=1020,
        养殖场 = 1021,
        出入库方式 = 1022,
        发票类型 = 1023,
        运费摘要 = 1024,
        存货分类 = 1025,
    }
    public enum TransferAccountsTypeEnum:long
    {
        销售结转 = 1911081429200099101,
        薪资计提 = 1911081429200099102,
        福利计提 = 1911081429200099103,
        猪成本结转 = 1911081429200099104,
        养户成本结转 = 1911081429200099107,
        费用分摊结转 = 1911081429200099108,
        损益结转 = 1911081429200099109,
        折旧计提 = 1911081429200099110,
        禽成本蛋鸡结转 = 1911081429200099105,
        禽成本结转 = 1911081429200099106,
        采购结转 = 1911081429200099111,
        物品结转 = 1911081429200099112,
        预收款核销 = 1911081429200099113,
        税费抵扣 = 1911081429200099114,
        运费结转 = 1911081429200099115,
        生产成本结转 = 1911081429200099116,
    }
    public class MM_Markets
    {

        /// <summary>
        /// 部门ID
        /// </summary>		
        public string MarketID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>		
        public string MarketName { get; set; }
        /// <summary>
        /// 部门全称
        /// </summary>		
        public string MarketFullName { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>		
        public long EnterpriseID { get; set; }
        /// <summary>
        /// 级次
        /// </summary>		
        public int Rank { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public long PID { get; set; }

        /// <summary>
        /// 是否末级
        /// </summary>
        public bool IsEnd { get; set; }
    }
    public class Biz_Market
    {
        /// <summary>
        /// 
        /// </summary>
        public int isUse { get; set; }
        /// <summary>
        /// 昌乐大北农食品
        /// </summary>
        public string marketName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string groupId { get; set; }
        /// <summary>
        /// 昌乐大北农食品
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int rank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string enterpriseId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string axis { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int isEnd { get; set; }
        /// <summary>
        /// 昌乐大北农
        /// </summary>
        public string enterpriseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string marketType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string marketId { get; set; }
    }
    public class PM_Person
    {
        /// <summary>
        /// 员工ID
        /// </summary>		
        public string PersonID { get; set; }


        /// <summary>
        /// 用户ID
        /// </summary>		
        public long UserID { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>		
        public string PersonName { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>		
        public long MarketID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>		
        public string MarketName { get; set; }
        public string PersonState { get; set; }
    }
    public class FM_Customer
    {

        /// <summary>
        /// 客户ID
        /// </summary>		
        public string CustomerID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>		
        public string CustomerName { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>		
        public long EnterpriseID { get; set; }
        public string CustomerName_Show { get; set; }
        public string MnemonicCode { get; set; }

    }
    
    public class UnitMeasurement
    {

        /// <summary>
        /// auto_increment
        /// </summary>		
        public string UnitID { get; set; }

        /// <summary>
        /// UnitName
        /// </summary>		
        public string UnitName { get; set; }

        /// <summary>
        /// Abbreviation
        /// </summary>		
        public string Abbreviation { get; set; }

        /// <summary>
        /// cUnitCode
        /// </summary>		
        public string cUnitCode { get; set; }

        /// <summary>
        /// iType
        /// </summary>		
        public long iType { get; set; }

        /// <summary>
        /// coefficient
        /// </summary>		
        public long coefficient { get; set; }

        /// <summary>
        /// cDescription
        /// </summary>		
        public string cDescription { get; set; }


    }
    public class FMProject
    {
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public int Rank { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>		
        public string PID { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>		
        public long ProjectType { get; set; }
        public string AccountID { get; set; }
    }
    public class DataDictModel
    {
        public string DictID { get; set; }
        public string cDictType { get; set; }
        public string cDictCode { get; set; }
        public string cDictName { get; set; }
        public string Pid { get; set; }
        public int iRank { get; set; }
        public string KeyIndex { get; set; }
        public string cPrivCode { get; set; }
        public string Owner { get; set; }
        public int iOrder { get; set; }
        public string State { get; set; }
        public string CreatedTime { get; set; }
        public string LastUpdateTime { get; set; }
        public string IsDel { get; set; }
        public long EnterpriseID { get; set; }
        public string cDescription { get; set; }
    }
    public class Summary
    {
        public string ReceiptAbstractID { get; set; }
        public string ReceiptAbstractIDs { get; set; }
        public string ReceiptAbstractName { get; set; }
        public string SettleSummaryGroupCode { get; set; }
        public int SettleSummaryID { get; set; }
        public string SettleSummaryName { get; set; }
        public string AccoSubjectID { get; set; }
        public string AccoSubjectCode { get; set; }
        public string EnterpriseID { get; set; }
    }
    public class ResponseProductInfoListForFDCostCoefficient
    {
        /// <summary>
        /// 商品代号ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 商品代号
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品分类ID
        /// </summary>
        public string ClassificationID { get; set; }
        /// <summary>
        /// 商品分类名称
        /// </summary>
        public string ClassificationName { get; set; }

        /// <summary>
        /// 计量单位表，计量单位ID
        /// </summary>
        public string UnitID { get; set; }
        /// <summary>
        /// 计量单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 计量单位表，计量单位ID 本地计量
        /// </summary>
        public string MeasureUnitID { get; set; }
        /// <summary>
        /// 计量单位名称 本地计量
        /// </summary>
        public string MeasureUnit { get; set; }
    }
    /// <summary>
    /// 批次和品种可通用
    /// </summary>
    public class ChickenFarm
    {
        public string ID { get; set; }
        public string Name { get; set; }	
        public string AffiliationName { get; set; }
        public string pid { get; set; }
        public bool isLast { get; set; }
        public string Level { get; set; }
    }

    public class Breeding
    {
        public string BreedingID { get; set; }
        public string BreedingNo { get; set; }
        public string BreedingType { get; set; }
        public string BreedingTypeName { get; set; }
        public string BreedingName { get; set; }
        public string ChickenFarmID { get; set; }
    }
    public class ProductInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string productId { get; set; }
        /// <summary>
        /// 根
        /// </summary>
        public string unitName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string unitId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string enterpriseId { get; set; }
        /// <summary>
        /// 北京助农
        /// </summary>
        public string enterpriseName { get; set; }
        /// <summary>
        /// 小狐狸坚果
        /// </summary>
        public string productName { get; set; }
        public string groupClassificationId { get; set; }
    }
    public class FA_AssetsClassification
    {
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationID { get; set; }
        /// <summary>
        /// 房屋
        /// </summary>
        public string ClassificationName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Rank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FixedYears { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ResidualValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DepreciationMethodID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ResetFixedYears { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AssetsAccoSubjectID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DepreciationAccoSubjectID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CodeRule { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccruedRule { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ModifiedDate { get; set; }
    }

    public class ChickenFarmModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ChickenFarmID { get; set; }
        /// <summary>
        /// 种鸡育雏育成场
        /// </summary>
        public string ChickenFarmName { get; set; }
        /// <summary>
        /// 育雏育成场
        /// </summary>
        public string ChickenFarmShortName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ChickenFarmNumer { get; set; }
        /// <summary>
        /// 育雏育成场
        /// </summary>
        public string FullAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ChickenFarmType { get; set; }
        /// <summary>
        /// 产蛋种鸡场
        /// </summary>
        public string ChickenFarmTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int iCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 汪伟良
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 种鸡2.0测试
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AreaID { get; set; }
        /// <summary>
        /// 北京市/市辖区/东城区
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 默认
        /// </summary>
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MarketName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PoultryCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AppID { get; set; }
    }
    public class CarriageAbstract
    {
        /// <summary>
        /// 
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FieldValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsEnd { get; set; }
    }
    public class DataDictDataResult
    {
        public long DictID { get; set; }
        public string cDictType { get; set; }
        public string cDictCode { get; set; }
        public string cDictName { get; set; }
        public long Pid { get; set; }
        public long iRank { get; set; }
        public string KeyIndex { get; set; }
        public string cPrivCode { get; set; }
        public long Owner { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public long iOrder { get; set; }
        public string State { get; set; }
        public string IsDel { get; set; }
        public long EnterpriseID { get; set; }
        public string cDescription { get; set; }
    }
    public class Jurisdiction
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string AffiliationName { get; set; }
        public string pid { get; set; }
        public string isLast { get; set; }
        public string Level { get; set; }
    }
    public class PigTypes
    {
        public string dictId { get; set; }
        public string dictName { get; set; }
    }
    public class BIZProjectNameSetting
    {
        //项目ID
        public string ProjectID { get; set; }

        //项目名称
        public string ProjectName { get; set; }

        //项目编码
        public string ProjectCode { get; set; }

        //项目类型
        public string ProjectType { get; set; }

        //级次
        public int Rank { get; set; }

        //父级ID
        public string PID { get; set; }

        //备注
        public string Remarks { get; set; }

        //制单人ID
        public string OwnerID { get; set; }

        //所属公司ID
        public string EnterpriseID { get; set; }

        //创建日期
        public DateTime CreatedDate { get; set; }

        //最后修改日期
        public DateTime ModifiedDate { get; set; }

        //是否结题
        public int IsKnot { get; set; }

        public BIZProjectNameSetting() { }
    }
    public class PigFarmModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pigFarmId { get; set; }
        /// <summary>
        /// 探感育成场
        /// </summary>
        public string pigFarmFullName { get; set; }
        /// <summary>
        /// 探感育成场
        /// </summary>
        public string pigFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pigFarmCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pigFarmScale { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string areaId { get; set; }
        /// <summary>
        /// 探感
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string begindate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string personId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string enterpriseId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cultureMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool isUse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string enablePeriod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pigWarehouseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool finishGuide { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string managementId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long recordId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string numericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createdDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createdOwnerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string modifiedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ownerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remarks { get; set; }
    }

    public class BIZ_AccoSubject
    {
        /// <summary>
        /// 科目ID
        /// </summary>		
        public string AccoSubjectID { get; set; }

        /// <summary>
        /// 科目编码
        /// </summary>		
        public string cAccoSubjectCode { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>		
        public string cAccoSubjectName { get; set; }

        /// <summary>
        /// 科目全称
        /// </summary>		
        public string cAccoSubjectFullName { get; set; }

        /// <summary>
        /// 系统科目
        /// </summary>		
        public long AccoSubjectSystemID { get; set; }

        /// <summary>
        ///科目类型
        /// </summary>		
        public long AccoSubjectType { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>		
        public long ExpenseType { get; set; }

        /// <summary>
        /// 币种
        /// </summary>		
        public string Currency { get; set; }


        /// <summary>
        /// 父级
        /// </summary>		
        public string PID { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 级次
        /// </summary>		
        public int Rank { get; set; } 

        /// <summary>
        /// bTorF
        /// </summary>		
        public bool bTorF { get; set; } = true;

        /// <summary>
        /// bLorR
        /// </summary>		
        public bool bLorR { get; set; } = false;

        /// <summary>
        /// bEnd
        /// </summary>		
        public bool bEnd { get; set; } = false;

        /// <summary>
        /// bProject
        /// </summary>		
        public bool bProject { get; set; } = false;

        /// <summary>
        /// bCus
        /// </summary>		
        public bool bCus { get; set; } = false;

        /// <summary>
        /// bPerson
        /// </summary>		
        public bool bPerson { get; set; } = false;

        /// <summary>
        /// bSup
        /// </summary>		
        public bool bSup { get; set; } = false;

        /// <summary>
        /// bDept
        /// </summary>		
        public bool bDept { get; set; } = false;

        /// <summary>
        /// bItem
        /// </summary>		
        public bool bItem { get; set; } = false;

        /// <summary>
        /// bCash
        /// </summary>		
        public bool bCash { get; set; } = false;

        /// <summary>
        /// bBank
        /// </summary>		
        public bool bBank { get; set; } = false;

        /// <summary>
        /// 使用状态
        /// </summary>		
        public bool IsUse { get; set; } = false;

        /// <summary>
        /// 轴数据
        /// </summary>
        public string cAxis { get; set; }
    }
    public class RestFulResult<T>
    {

        public int code { get; set; }
        public T data { get; set; }
        public int count { get; set; }
        public string error { get; set; }
        public string msg { get; set; }
    }
    public class Bank_Account
    {
        public string PayerID { get; set; } //收款人
        public string BankID { get; set; } //银行
        public string DepositBank { get; set; }//开户行
        public string AccountName { get; set; }//账户名称
        public string AccountFullName { get; set; }
        public string AccountID { get; set; }//账户
        public string AccountNumber { get; set; }//账户号码
        public string UserCenterPayee { get; set; }// 请求金融用
        public string BankCode { get; set; } //银行类型 ABC ICBC。。。
        public string PayeeID { get; set; } //收款人--bo_id
        /// <summary>
        /// 账户属性 （0：个人，1：公司）
        /// </summary>		
        public bool AccountNature { get; set; }
    }
    public class AccountInfoReq
    {
        public string BusinessType { get; set; }
        public string EnterpriseID { get; set; }
        public List<string> CustomerIdList { get; set; }
    }
    public class PersonAccountInfo
    {
        public PersonAccountInfo()
        {
            BankID = "0";
        }
        public string PersonID { get; set; } //收款人
        /// <summary>
        /// BankID
        /// </summary>		
        public string BankID { get; set; }

        /// <summary>
        /// AccountName
        /// </summary>		
        public string AccountName { get; set; }

        /// <summary>
        /// AccountNumber
        /// </summary>		
        public string AccountNumber { get; set; }

        /// <summary>
        /// DepositBank
        /// </summary>		
        public string DepositBank { get; set; }

        /// <summary>
        /// IDCard
        /// </summary>		
        public string IDCard { get; set; }

        /// <summary>
        /// MobilePhone
        /// </summary>		
        public string MobilePhone { get; set; }
        public string BO_ID { get; set; }
        public string BankAbbr { get; set; }
    }
    public class SA_CustomerAccount
    {

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// CustomerID
        /// </summary>		
        public string CustomerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }

        /// <summary>
        /// BankID
        /// </summary>		
        public string BankID { get; set; }
        public string BankName { get; set; }

        /// <summary>
        /// AccountName
        /// </summary>		
        public string AccountName { get; set; }

        /// <summary>
        /// AccountNumber
        /// </summary>		
        public string AccountNumber { get; set; }

        /// <summary>
        /// DepositBank
        /// </summary>		
        public string DepositBank { get; set; }

        /// <summary>
        /// IDCard
        /// </summary>		
        public string IDCard { get; set; }

        /// <summary>
        /// MobilePhone
        /// </summary>		
        public string MobilePhone { get; set; }

        /// <summary>
        /// AccountType
        /// </summary>		
        public bool AccountType { get; set; }

        /// <summary>
        /// AccountNature
        /// </summary>		
        public bool AccountNature { get; set; }

        /// <summary>
        /// IsPayment1
        /// </summary>		
        public bool IsPayment1 { get; set; }

        /// <summary>
        /// IsPayment2
        /// </summary>		
        public bool IsPayment2 { get; set; }
        public string BankAbbr { get; set; }

    }
    public  class AccountInfo
    {
        public AccResultModel AccountData { get; set; }
        public string PaymentTypeID { get; set; }
        public string PaymentTypeName { get; set; }
    }
    public class AccResultModel
    {
        public bool ResultState { get; set; }
        public int CodeNo { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }

    public class BIZ_Area
    {

        /// <summary>
        /// 地理区域ID
        /// </summary>		
        public string AreaID { get; set; }

        /// <summary>
        /// 地理区域名称
        /// </summary>		
        public string AreaName { get; set; }

        /// <summary>
        /// 地理区域全称
        /// </summary>		
        public string AreaFullName { get; set; }

        /// <summary>
        /// 地理区域编码
        /// </summary>		
        public string AreaCode { get; set; }

        /// <summary>
        /// PID
        /// </summary>		
        public string PID { get; set; }

        /// <summary>
        /// 轴
        /// </summary>		
        public string Axis { get; set; }

        /// <summary>
        ///级次
        /// </summary>		
        public int Rank { get; set; }

    }
    public class DataDictRequest 
    {
        public DataDictRequest()
        {
            this.DictID = -1;
            this.PID = -1;
            this.EnterpriseID = -1;
        }
        public long DictID { get; set; }
        public long PID { get; set; }
        public string CPrivCode { get; set; }
        public long EnterpriseID { get; set; }
        public string DictType { get; set; }
    }

    public class NumberSearchModel
    {
        public string EnterpriseID { get; set; }
        public string SettleReceipType { get; set; }
        public string ChildSettleReceipType { get; set; }
        public string DataDate { get; set; }
        public string TicketedPointID { get; set; }
        public string IsComplex { get; set; }
    }
    public class CustomerSearch
    {
        public string EnterpriseID { get; set; }

        public string CustomerName { get; set; }
        public string CustomerID { get; set; }
        //public bool IsUse { get; set; }
    }
    public class Biz_CustomerDrop
    {
        public string CustomerID { get; set; }

        public string CustomerName { get; set; }
        public string EnterpriseID { get; set; }
        public string ChildValue { get; set; }
    }

    public class OrgEnteRequest
    {
        //使用单位
        public string enterpriseId { get; set; }
        // 指定参数，用于选中返回数据
        public string checklist { get; set; }
        //组织父级
        public string pid { get; set; } = "-1";
        //过滤条件
        public string name { get; set; }
        //是否组织树
        public string istree { get; set; } = "1";
        public string boid { get; set; } 
        //是否主组织 1是 0 不是 -1全部 默认主组织 istree=1时生效
        public string ismaster { get; set; } = "1";
        //是否全部权限单位 1全部 0集团授权的单位 默认集团授权的单位
        public string ispowerall { get; set; } = "1";
        //是否含场 1含 0不含 默认含
        public string isthree { get; set; } = "0";
        // -1 全部 0停用 1启用
        public string isuse { get; set; } = "-1";
    }
    public class ReportSummary
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public  string Name { get; set; }
        /// <summary>
        ///传给后端汇总方式
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 前端汇总方式展示列
        /// </summary>
        public string DataValue { get; set; }

    }
    public class OrgEnteResult
    {
        public string Type { get; set; }
        public string cAxis { get; set; }
        public string Abled { get; set; }
        public string Check { get; set; }
        public string pid { get; set; }
        public int IsEnter { get; set; }
        public string id { get; set; }
        public string Name { get; set; }
        public bool IsUse { get; set; }
    }
    public class OrgDataResult
    {
        public string sortId { get; set; }
        public string sortName { get; set; }
        public string pid { get; set; }
        public string cFullName { get; set; }
        public string sortRank { get; set; }
        public string isEnd { get; set; }
        public bool isUsed { get; set; }
        public string cAxis { get; set; }
        public int type { get; set; }
    }
    public class SubjectRequest
    {
        public string EnterpriseID { get; set; }
        public string DataDate { get; set; }
        public bool? IsGroup { get; set; }
    }
    public class MarketResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string MarketId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MarketName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsUse { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CFullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Rank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CAxis { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsEnd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MarketType { get; set; }
       
    }
    public class FMTicketedPoint: FM_TicketedPoint
    {
        public string PID { get; set; }
        public string IdKey { get; set; }
        public string PidKey { get; set; }
    }
    public class FDAccountSearch
    {
        public string ResponsiblePerson { get; set; }
        public string EnterpriseID { get; set; }
        public string AccountID { get; set; }
        public string DataDate { get; set; }
        public bool IsSearchBal { get; set; }
    }
    //public class FD_Account
    //{
    //    public string PaymentTypeID
    //    {
    //        get;
    //        set;
    //    }
    //    public string PaymentTypeName
    //    {
    //        get;
    //        set;
    //    }
    //}
    #region 发OA消息
    public class OASend
    {
        /// <summary>
        /// 
        /// </summary>
        public long Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long BusinessID { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long OwnerID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long EnterpriseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long NewsCenterAbstract { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserIds { get; set; }
    }

    #endregion
    #region 智农通消息
    public class NoticeInfo
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MsgType { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public List<long> UserId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string MsgSendDescribe { get; set; }
        /// <summary>
        /// 公众号id
        /// </summary>
        public long AccontId { get; set; }
        /// <summary>
        /// (可选：企业ID),移动支付默认 888  
        /// </summary>
        public string orgId { get; set; }
        /// <summary>
        /// (可选：业务ID:流水号唯一值)
        /// </summary>
        public string bizId { get; set; }
    }

    /// <summary>
    /// 消息格式类型
    /// </summary>
    public enum MessageType
    {
        push,
        notice,
        html
    }

    public enum MessageQuene
    {
        /// <summary>
        /// 大量消息队列
        /// </summary>
        large,
        /// <summary>
        /// 普通消息量队列
        /// </summary>
        general

    }
    #endregion
}
