using Architecture.Common.Application.Query;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FM_NewCarryForwardVoucher;
using FinanceManagement.Common;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class FM_NewCarryForwardVoucherODataProvider : OneWithManyQueryProvider<FM_NewCarryForwardVoucherODataEntity, FM_NewCarryForwardVoucherDetailODataEntity>
    {
        private readonly string inventorypid = "2016030703636788604";
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        BIZ_DataDictODataProvider _dictProvider;
        FMBaseCommon _baseUnit;
        IHttpContextAccessor _httpContextAccessor;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;

        public FM_NewCarryForwardVoucherODataProvider(IIdentityService identityservice, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _baseUnit = baseUnit;
            _identityservice = identityservice;
            _context = context;
            _treeModel = treeModel;
            _dictProvider = dictProvider;
            _httpContextAccessor = httpContextAccessor;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }

        public IEnumerable<FM_NewCarryForwardVoucherODataEntity> GetList(ODataQueryOptions<FM_NewCarryForwardVoucherODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FM_NewCarryForwardVoucherODataEntity> GetData(ODataQueryOptions<FM_NewCarryForwardVoucherODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<FM_NewCarryForwardVoucherODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.TransferAccountsType USING utf8mb4) TransferAccountsType,	
                                        dict.DataDictName AS TransferAccountsTypeName,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                        tick.TicketedPointName,
                                        CONVERT(t1.Number USING utf8mb4) Number,
                                        CONVERT(t1.DataSource USING utf8mb4) DataSource,
                                        (CASE WHEN t1.DataSource=1612131439999999999 THEN '种猪流转表/仔猪流转表/肥猪流转表/后备猪流转表/饲养成本明细表/养户成本汇总表/费用分摊明细表/会计辅助账' 
                                              WHEN t1.DataSource=2208151434260000109 THEN '种禽成本流转表/孵化成本流转表/蛋品成本明细表'
                                              WHEN t1.DataSource=1612131439999999998 THEN '成本汇总表/原材料成本' ELSE st.cText END) AS DataSourceName,	
                                        CONVERT(t1.TransferAccountsAbstract USING utf8mb4) TransferAccountsAbstract,
                                        ifnull(dict1.DataDictName,bsdict.cDictName) AS TransferAccountsAbstractName,
                                        CONVERT(t1.TransferAccountsSort USING utf8mb4) TransferAccountsSort,
                                        dict2.DataDictName AS TransferAccountsSortName,	
                                        CONVERT(t1.Remarks USING utf8mb4) Remarks,	
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP1.Name AS OwnerName,	
                                        t1.SettleNumber,
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(t1.TransactorID USING utf8mb4) TransactorID,	
                                        HP2.Name AS TransactorName,
                                        CONVERT(t1.TransactorDate USING utf8mb4) TransactorDate,
                                        CONVERT(R2.CheckedByID USING utf8mb4) AS CheckedId,	
                                        HP3.Name AS CheckedName,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM
                                        `nxin_qlw_business`.`fm_CarryForwardvoucher` t1
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON t1.TransferAccountsType=dict.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON t1.TicketedPointID=tick.TicketedPointID AND t1.EnterpriseID=tick.EnterpriseID
                                        LEFT JOIN `qlw_nxin_com`.`stmenu` st ON t1.DataSource=st.MenuID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict1 ON t1.TransferAccountsAbstract=dict1.DataDictID
                                        LEFT JOIN `qlw_nxin_com`.BSDataDict bsdict ON t1.TransferAccountsAbstract=bsdict.DictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict2 ON t1.TransferAccountsSort =dict2.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536 AND R1.reviwetype=2208180928040000109
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R2 ON R2.NumericalOrder=t1.NumericalOrder AND R2.CheckMark=16 AND R2.reviwetype=2208180928040000109
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP3 ON HP3.BO_ID=R2.CheckedByID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.TransactorID=HP2.Bo_ID
                                        WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId} 
                                        GROUP BY t1.NumericalOrder
                                        order by t1.CreatedDate desc ";
            return _context.FM_NewCarryForwardVoucherDataSet.FromSqlInterpolated(sql);
        }
        /// <summary>
        /// 生成凭证结果列表`
        /// </summary>
        /// <returns></returns>
        public IQueryable<FM_NewCarryForwardVoucherRecordODataEntity> GetRecordList(ODataQueryOptions<FM_NewCarryForwardVoucherRecordODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@" SELECT 
                                            t1.RecordID,	
                                            CONVERT(t1.NumericalOrderCarry USING utf8mb4) NumericalOrderCarry,	
                                            CONVERT(t1.NumericalOrderSettl USING utf8mb4) NumericalOrderSettl,	
                                            CONVERT(t1.TransferAccountsType USING utf8mb4) TransferAccountsType,	
                                            dict.`DataDictName` AS TransferAccountsName,
                                            t1.CarryName,
                                            CONVERT(t1.DataSource USING utf8mb4) DataSource,
                                            (CASE WHEN t1.DataSource=1612131439999999999 THEN '种猪流转表/仔猪流转表/肥猪流转表/后备猪流转表/饲养成本明细表/养户成本汇总表/费用分摊明细表/会计辅助账' 
                                                  WHEN t1.DataSource=2208151434260000109 THEN '种禽成本流转表/孵化成本流转表/蛋品成本明细表'
                                                  WHEN t1.DataSource=1612131439999999998 THEN '成本汇总表/原材料成本' ELSE st.cText END) AS DataSourceName,	
                                            CONVERT(t3.Number USING utf8mb4) Number,	
                                            CONVERT(IF(t3.DataDate='0000-00-00',NULL,t3.DataDate) USING utf8mb4) DataDate,
                                            CONVERT(t1.OwnerID USING utf8mb4) OwnerID,	
                                            HP2.`Name` AS OwnerName,
                                            t1.ImplementResult,
                                            t1.ResultState,
                                            CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                            CONVERT(DATE_FORMAT(t1.TransBeginDate,'%Y-%m-%d') USING utf8mb4) TransBeginDate,
                                            CONVERT(DATE_FORMAT(t1.TransEndDate,'%Y-%m-%d') USING utf8mb4) TransEndDate,
                                            CONVERT(t1.TransferAccountsAbstract USING utf8mb4) TransferAccountsAbstract,
                                            CONVERT(t2.TransferAccountsSort USING utf8mb4) TransferAccountsSort,t1.`TransSummary`,t1.`TransSummaryName`,t1.`TransWhereList`,t1.`Remarks`
                                             FROM `nxin_qlw_business`.`fm_CarryForwardvoucherrecord` t1
                                            LEFT JOIN `nxin_qlw_business`.`FM_CarryForwardVoucher` t2 ON t1.NumericalOrderCarry=t2.`NumericalOrder`
                                            LEFT JOIN `nxin_qlw_business`.`fd_settlereceipt` t3 ON t1.`NumericalOrderSettl`=t3.`NumericalOrder`
                                            LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON t1.TransferAccountsType=dict.DataDictID
                                            LEFT JOIN `qlw_nxin_com`.`stmenu` st ON t1.DataSource=st.MenuID
                                            LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.OwnerID=HP2.Bo_ID
                                        WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId} 
                                        ORDER BY t1.CreatedDate DESC ";
            return _context.FM_NewCarryForwardVoucherRecordDataSet.FromSqlInterpolated(sql);
        }
        /// <summary>
        /// 校验重复生成会计凭证sql`
        /// </summary>
        /// <returns></returns>
        public Task<List<FM_NewCarryForwardVoucherRecordODataEntity>> CheckRepeatSettl(FM_CarryForwardVoucherSearchCommand model)
        {
            string sql = $@" SELECT
                                            t1.RecordID,	
                                            CONVERT(t1.NumericalOrderCarry USING utf8mb4) NumericalOrderCarry,	
                                            CONVERT(t1.NumericalOrderSettl USING utf8mb4) NumericalOrderSettl,	
                                            CONVERT(t1.TransferAccountsType USING utf8mb4) TransferAccountsType,	
                                            dict.`DataDictName` AS TransferAccountsName,
                                            t1.CarryName,
                                            CONVERT(t1.DataSource USING utf8mb4) DataSource,
                                            (CASE WHEN t1.DataSource=1612131439999999999 THEN '种猪流转表/仔猪流转表/肥猪流转表/后备猪流转表/饲养成本明细表/养户成本汇总表/费用分摊明细表/会计辅助账' 
                                                  WHEN t1.DataSource=2208151434260000109 THEN '种禽成本流转表/孵化成本流转表/蛋品成本明细表'
                                                  WHEN t1.DataSource=1612131439999999998 THEN '成本汇总表/原材料成本' ELSE st.cText END) AS DataSourceName,	
                                            CONVERT(t3.Number USING utf8mb4) Number,	
                                            CONVERT(IF(t3.DataDate='0000-00-00',NULL,t3.DataDate) USING utf8mb4) DataDate,
                                            CONVERT(t1.OwnerID USING utf8mb4) OwnerID,	
                                            HP2.`Name` AS OwnerName,
                                            t1.ImplementResult,
                                            t1.ResultState,
                                            CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                            CONVERT(DATE_FORMAT(t1.TransBeginDate,'%Y-%m-%d') USING utf8mb4) TransBeginDate,
                                            CONVERT(DATE_FORMAT(t1.TransEndDate,'%Y-%m-%d') USING utf8mb4) TransEndDate,
                                            CONVERT(t1.TransferAccountsAbstract USING utf8mb4) TransferAccountsAbstract,
                                            CONVERT(t2.TransferAccountsSort USING utf8mb4) TransferAccountsSort,t1.`TransSummary`,t1.`TransSummaryName`,t1.`TransWhereList`,t1.`Remarks`
                                             FROM `nxin_qlw_business`.`fm_CarryForwardvoucherrecord` t1
                                            LEFT JOIN `nxin_qlw_business`.`FM_CarryForwardVoucher` t2 ON t1.NumericalOrderCarry=t2.`NumericalOrder`
                                            LEFT JOIN `nxin_qlw_business`.`fd_settlereceipt` t3 ON t1.`NumericalOrderSettl`=t3.`NumericalOrder`
                                            LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON t1.TransferAccountsType=dict.DataDictID
                                            LEFT JOIN `qlw_nxin_com`.`stmenu` st ON t1.DataSource=st.MenuID
                                            LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.OwnerID=HP2.Bo_ID
                                        WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId} 
                                        AND  ('{model.Begindate}' BETWEEN TransBeginDate AND  TransEndDate OR '{model.Enddate}' BETWEEN TransBeginDate AND  TransEndDate)
                                        AND NumericalOrderCarry IN ({model.NumericalOrderList}) and t3.Number is not null
                                        ORDER BY t1.CreatedDate DESC ";
            return _context.FM_NewCarryForwardVoucherRecordDataSet.FromSqlRaw(sql).ToListAsync();
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        public FM_NewCarryForwardVoucherODataEntity GetSingleData(long manyQuery)
        {
            FormattableString sql = $@"SELECT  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.TransferAccountsType USING utf8mb4) TransferAccountsType,	
                                        dict.DataDictName AS TransferAccountsTypeName,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                        tick.TicketedPointName,
                                        CONVERT(t1.Number USING utf8mb4) Number,
                                        CONVERT(t1.DataSource USING utf8mb4) DataSource,
                                        (CASE WHEN t1.DataSource=1612131439999999999 THEN '种猪流转表/仔猪流转表/肥猪流转表/后备猪流转表/饲养成本明细表/养户成本汇总表/费用分摊明细表/会计辅助账' 
                                              WHEN t1.DataSource=2208151434260000109 THEN '种禽成本流转表/孵化成本流转表/蛋品成本明细表'
                                              WHEN t1.DataSource=1612131439999999998 THEN '成本汇总表/原材料成本' ELSE st.cText END) AS DataSourceName,	
                                        CONVERT(t1.TransferAccountsAbstract USING utf8mb4) TransferAccountsAbstract,
                                        ifnull(dict1.DataDictName,bsdict.cDictName) AS TransferAccountsAbstractName,	
                                        CONVERT(t1.TransferAccountsSort USING utf8mb4) TransferAccountsSort,
                                        dict2.DataDictName AS TransferAccountsSortName,	
                                        CONVERT(t1.Remarks USING utf8mb4) Remarks,	
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP1.Name AS OwnerName,
                                        t1.SettleNumber,
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(t1.TransactorID USING utf8mb4) TransactorID,	
                                        HP2.Name AS TransactorName,
                                         CONVERT(R2.CheckedByID USING utf8mb4) AS CheckedId,	
                                        HP3.Name AS CheckedName,
                                        CONVERT(t1.TransactorDate USING utf8mb4) TransactorDate,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM
                                        `nxin_qlw_business`.`fm_CarryForwardvoucher` t1
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON t1.TransferAccountsType=dict.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON t1.TicketedPointID=tick.TicketedPointID AND t1.EnterpriseID=tick.EnterpriseID
                                        LEFT JOIN `qlw_nxin_com`.`stmenu` st ON t1.DataSource=st.MenuID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict1 ON t1.TransferAccountsAbstract=dict1.DataDictID
                                        LEFT JOIN `qlw_nxin_com`.BSDataDict bsdict ON t1.TransferAccountsAbstract=bsdict.DictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict2 ON t1.TransferAccountsSort =dict2.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536 AND R1.reviwetype=2208180928040000109
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R2 ON R2.NumericalOrder=t1.NumericalOrder AND R2.CheckMark=16 AND R2.reviwetype=2208180928040000109
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP3 ON HP3.BO_ID=R2.CheckedByID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.TransactorID=HP2.Bo_ID
                                    WHERE t1.NumericalOrder ={manyQuery}";
            var main = _context.FM_NewCarryForwardVoucherDataSet.FromSqlInterpolated(sql).FirstOrDefault();
            var detail = GetDetaiListAsync(main.TransferAccountsType, manyQuery);
            main.Lines = detail;
            return main;
        }
        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="TransferAccountsType"></param>
        /// <returns></returns>
        public List<FM_NewCarryForwardVoucherDetailODataEntity> GetDetaiListAsync(string TransferAccountsType, long manyQuery)
        {
            FormattableString extend = $@"SELECT A.RecordID as RecordID,CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail, 
                                        A.Sort,'' as SortName,CONVERT(A.Symbol USING utf8mb4) Symbol,
                                        CONVERT(A.Object USING utf8mb4) Object,'' as ObjectName,
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FM_CarryForwardVoucherExtend as A
                                        WHERE NumericalOrder={manyQuery}";
            var resultExt = _context.FM_NewCarryForwardVoucherExtendDataSet.FromSqlInterpolated(extend).ToList();
            FormattableString formula = $@"SELECT A.RecordID AS RecordID,CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail, 
                                        A.RowNum,
                                        CONVERT(A.Bracket USING utf8mb4) Bracket,
                                        CONVERT(A.FormulaID USING utf8mb4) FormulaID,
                                        '' as FormulaName,
                                        CONVERT(A.Operator USING utf8mb4) Operator,
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FM_CarryForwardVoucherFormula AS A
                                        WHERE NumericalOrder={manyQuery}";
            var resultFormula = _context.FM_NewCarryForwardVoucherFormulaDataSet.FromSqlInterpolated(formula).ToList();
            FormattableString sql = $@"SELECT B.RecordID,CONVERT(B.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(B.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        CONVERT(B.ReceiptAbstractID USING utf8mb4) ReceiptAbstractID,
                                        IFNULL(se.SettleSummaryName,se1.SettleSummaryGroupName) AS ReceiptAbstractName,
                                        CONVERT(B.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                        acc.AccoSubjectFullName AS AccoSubjectName,
                                        CONVERT(B.AccoSubjectCode USING utf8mb4) AccoSubjectCode,
                                        B.IsPerson,B.IsCustomer,B.IsMarket,B.IsProduct,B.IsPigFram,B.IsProject,B.IsSum,B.DebitFormula,B.DebitSecFormula,B.CreditFormula,B.CreditSecFormula,
                                        CONVERT(B.ModifiedDate USING utf8mb4) ModifiedDate
                                        ,CONVERT(v.dBegin USING utf8mb4) AccoSubjectdBegin,CONVERT(v.dEnd USING utf8mb4) AccoSubjectdEnd
                                        ,CONVERT(IFNULL(v1.dBegin,v2.dBegin) USING utf8mb4) ReceiptAbstractdBegin,CONVERT(IFNULL(v1.dEnd,v2.dEnd) USING utf8mb4) ReceiptAbstractdEnd
                                         FROM `nxin_qlw_business`.FM_CarryForwardVoucherDetail  B
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` se ON B.ReceiptAbstractID=se.SettleSummaryID AND (se.EnterpriseID={_identityservice.EnterpriseId} OR se.EnterpriseID={_identityservice.GroupId})
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummarygroup` se1 ON B.ReceiptAbstractID=se1.SettleSummaryGroupID AND (se1.EnterpriseID={_identityservice.EnterpriseId} OR se1.EnterpriseID={_identityservice.GroupId})
                                        LEFT JOIN `qlw_nxin_com`.`biz_accosubject` acc ON B.AccoSubjectID=acc.AccoSubjectID AND (acc.EnterpriseID={_identityservice.EnterpriseId} OR acc.EnterpriseID={_identityservice.GroupId})
                                        LEFT JOIN qlw_nxin_com.biz_versionsetting v ON v.VersionID=acc.VersionID
                                        LEFT JOIN qlw_nxin_com.biz_versionsetting v1 ON v1.VersionID=se.VersionID
                                        LEFT JOIN qlw_nxin_com.biz_versionsetting v2 ON v2.VersionID=se1.VersionID                                        
                                        WHERE B.NumericalOrder ={manyQuery}";
            var result = _context.FM_NewCarryForwardVoucherDetailDataSet.FromSqlInterpolated(sql).ToList();
            if (result?.Count > 0)
            {
                result = SetBasicDataMethod(result, resultExt, resultFormula, TransferAccountsType);//分录条件赋值
            }
            return result;
        }
        private List<FM_NewCarryForwardVoucherDetailODataEntity> SetBasicDataMethod(List<FM_NewCarryForwardVoucherDetailODataEntity> result, List<FM_NewCarryForwardVoucherExtendODataEntity> resultExt, List<FM_NewCarryForwardVoucherFormulaODataEntity> resultFormula, string TransferAccountsType)
        {
            //公式信息赋值
            if (resultFormula.Count > 0)
            {
                SetFormulaNameMethod(result, Convert.ToInt64(TransferAccountsType), resultFormula);
            }
            List<TreeModelODataEntity> produtClassList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> supplierList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> suppliesList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> chickenList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> breedList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> batchList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> changquList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> jisheList = new List<TreeModelODataEntity>();
            List<BIZ_DataDictODataEntity> dictList = new List<BIZ_DataDictODataEntity>();
            List<TreeModelODataEntity> marketList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> pigfarmList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> chickenfarmList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> personList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> porductList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> pigtypeList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> assetsClassificationList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> outInTypeList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> invoiceTypeList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> carriageAbstractList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> inventoryList = new List<TreeModelODataEntity>();
            List<BIZ_DataDictODataEntity> costList = new List<BIZ_DataDictODataEntity>();
            if (resultExt.Count() > 0)
            {
                var groupKey = resultExt.GroupBy(s => s.Sort).Select(s => s.Key);
                foreach (var key in groupKey)
                {
                    switch (key)
                    {
                        case (int)SortTypeEnum.商品分类:
                            produtClassList = _treeModel.GetProductGroupClassAsync(_identityservice.GroupId);
                            break;
                        case (int)SortTypeEnum.供应商:
                            supplierList = _treeModel.GetSupplierAsync();
                            break;
                        case (int)SortTypeEnum.物品分类:
                            suppliesList = _treeModel.GetSuppliesAsync(_identityservice.EnterpriseId);
                            break;
                        case (int)SortTypeEnum.鸡场:
                            chickenList = _baseUnit.GetChickenFarm(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId, Boid = _identityservice.UserId });
                            break;
                        case (int)SortTypeEnum.品种:
                            breedList = _baseUnit.GetBreeding(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                            break;
                        case (int)SortTypeEnum.批次:
                            batchList = _baseUnit.GetBatching(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                            break;
                        case (int)SortTypeEnum.厂区:
                            changquList = _baseUnit.GetJurisdictionList(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId, Boid = _identityservice.UserId }, "2");
                            break;
                        case (int)SortTypeEnum.鸡舍:
                            jisheList = _baseUnit.GetJurisdictionList(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId, Boid = _identityservice.UserId }, "3");
                            break;
                        case (int)SortTypeEnum.销售摘要:
                            dictList.AddRange(_dictProvider.GetDataDictAsync("201610140104402501").Result);//销售摘要
                            break;
                        case (int)SortTypeEnum.采购摘要:
                            dictList.AddRange(_dictProvider.GetDataDictAsync("201610140104402301").Result);//采购摘要
                            break;
                        case (int)SortTypeEnum.费用性质:
                            costList.AddRange(_dictProvider.GetDataDictAsyncExtend("202205111355001101").Result);//费用性质
                            break;
                        case (int)SortTypeEnum.猪场:
                            pigfarmList = _baseUnit.GetPigFarm(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                            break;
                        case (int)SortTypeEnum.养殖场:
                            chickenfarmList = _baseUnit.getChickenFarmList(_identityservice.EnterpriseId);
                            break;
                        case (int)SortTypeEnum.部门:
                            marketList = _baseUnit.GetMarket(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                            break;
                        case (int)SortTypeEnum.人员:
                            personList = _baseUnit.GetPerson(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                            break;
                        case (int)SortTypeEnum.商品代号:
                            porductList = _baseUnit.getProductData(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                            break;
                        case (int)SortTypeEnum.猪只类型:
                            pigtypeList = _baseUnit.GetPigTypes(_identityservice.EnterpriseId);
                            break;
                        case (int)SortTypeEnum.资产类别:
                            assetsClassificationList = _baseUnit.getFA_AssetsClassificationData(_identityservice.GroupId);
                            break;
                        case (int)SortTypeEnum.出入库方式:
                            outInTypeList = _baseUnit.GetInOutAbstract();
                            break;
                        case (int)SortTypeEnum.发票类型:
                            invoiceTypeList = _baseUnit.GetInvoiceType();
                            break;
                        case (int)SortTypeEnum.运费摘要:
                            carriageAbstractList = _baseUnit.GetCarriageAbstract( _identityservice.EnterpriseId);
                            break;
                        case (int)SortTypeEnum.存货分类:
                            inventoryList = _dictProvider.GetDataDictConvertDrop(inventorypid);
                            break;
                        default:
                            break;
                    }
                }
            }
            result?.ForEach(s =>
            {
                var list = resultExt?.Where(o => o.NumericalOrderDetail == s.NumericalOrderDetail).ToList();
                var list1 = resultFormula?.Where(o => o.NumericalOrderDetail == s.NumericalOrderDetail).ToList();
                list?.ForEach(o =>
                {
                    switch (o.Sort)
                    {
                        case (int)SortTypeEnum.商品分类:
                            o.ObjectName = produtClassList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "商品分类";
                            break;
                        case (int)SortTypeEnum.供应商:
                            o.ObjectName = supplierList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "供应商";
                            break;
                        case (int)SortTypeEnum.物品分类:
                            o.ObjectName = suppliesList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "物品分类";
                            break;
                        case (int)SortTypeEnum.鸡场:
                            o.ObjectName = chickenList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "鸡场";
                            break;
                        case (int)SortTypeEnum.品种:
                            o.ObjectName = breedList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "品种";
                            break;
                        case (int)SortTypeEnum.批次:
                            o.ObjectName = batchList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "批次";
                            break;
                        case (int)SortTypeEnum.厂区:
                            o.ObjectName = changquList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "厂区";
                            break;
                        case (int)SortTypeEnum.鸡舍:
                            o.ObjectName = jisheList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "鸡舍";
                            break;
                        case (int)SortTypeEnum.销售摘要:
                            o.ObjectName = dictList?.Where(_ => _.DataDictID == o.Object)?.FirstOrDefault()?.DataDictName;
                            o.SortName = "销售摘要";
                            break;
                        case (int)SortTypeEnum.采购摘要:
                            o.ObjectName = dictList?.Where(_ => _.DataDictID == o.Object)?.FirstOrDefault()?.DataDictName;
                            o.SortName = "采购摘要";
                            break;
                        case (int)SortTypeEnum.费用性质:
                            o.ObjectName = costList?.Where(_ => _.DataDictID == o.Object)?.FirstOrDefault()?.DataDictName;
                            o.SortName = "费用性质";
                            break;
                        case (int)SortTypeEnum.猪场:
                            o.ObjectName = pigfarmList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "猪场";
                            break;
                        case (int)SortTypeEnum.养殖场:
                            o.ObjectName = chickenfarmList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "养殖场";
                            break;
                        case (int)SortTypeEnum.部门:
                            o.ObjectName = marketList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "部门";
                            break;
                        case (int)SortTypeEnum.人员:
                            o.ObjectName = personList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "人员";
                            break;
                        case (int)SortTypeEnum.商品代号:
                            o.ObjectName = porductList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "商品代号";
                            break;
                        case (int)SortTypeEnum.猪只类型:
                            o.ObjectName = pigtypeList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "猪只类型";
                            break;
                        case (int)SortTypeEnum.资产类别:
                            o.ObjectName = assetsClassificationList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "资产类别";
                            break;
                        case (int)SortTypeEnum.出入库方式:
                            o.ObjectName = outInTypeList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "出入库方式";
                            break;
                        case (int)SortTypeEnum.发票类型:
                            o.ObjectName = invoiceTypeList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "发票类型";
                            break;
                        case (int)SortTypeEnum.运费摘要:
                            o.ObjectName = carriageAbstractList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "运费摘要";
                            break;
                        case (int)SortTypeEnum.存货分类:
                            o.ObjectName = inventoryList?.Where(_ => _.Id == o.Object)?.FirstOrDefault()?.cName;
                            o.SortName = "存货分类";
                            break;
                        default:
                            break;
                    }
                });
                s.Extends = list;
                s.Formulas = list1.OrderBy(s => s.RowNum).ToList();
            });
            return result;
        }
        private void SetFormulaNameMethod(List<FM_NewCarryForwardVoucherDetailODataEntity> result, long TransferAccountsType, List<FM_NewCarryForwardVoucherFormulaODataEntity> resultFormula)
        {
            List<BIZ_DataDictODataEntity> dictlist = new List<BIZ_DataDictODataEntity>();
            switch (TransferAccountsType)
            {
                case (long)TransferAccountsTypeEnum.销售结转:
                case (long)TransferAccountsTypeEnum.采购结转:
                case (long)TransferAccountsTypeEnum.福利计提:
                case (long)TransferAccountsTypeEnum.养户成本结转:
                case (long)TransferAccountsTypeEnum.禽成本蛋鸡结转:
                case (long)TransferAccountsTypeEnum.损益结转:
                case (long)TransferAccountsTypeEnum.折旧计提:
                case (long)TransferAccountsTypeEnum.物品结转:
                case (long)TransferAccountsTypeEnum.预收款核销:
                case (long)TransferAccountsTypeEnum.税费抵扣:
                case (long)TransferAccountsTypeEnum.运费结转:
                case (long)TransferAccountsTypeEnum.生产成本结转:
                    dictlist.AddRange(GetDataDictAsync(TransferAccountsType));
                    break;
                case (long)TransferAccountsTypeEnum.薪资计提:
                    dictlist.AddRange(GetSalarySetItemList(_identityservice.EnterpriseId, out bool IsSetNewFormula));
                    break;
                case (long)TransferAccountsTypeEnum.费用分摊结转:
                case (long)TransferAccountsTypeEnum.猪成本结转:
                    dictlist.AddRange(GetDataDictAsync(TransferAccountsType));
                    dictlist.AddRange(GetShareCostItemList(_identityservice.EnterpriseId, out bool IsSetNewFormula1));
                    break;
                case (long)TransferAccountsTypeEnum.禽成本结转:
                    dictlist.AddRange(GetPoultryCostItemList());
                    break;
                default:
                    break;
            }
            dictlist.Add(new BIZ_DataDictODataEntity() { DataDictID = "2208081518000000151", DataDictName = "贷方合计" });
            dictlist.Add(new BIZ_DataDictODataEntity() { DataDictID = "2208081518000000152", DataDictName = "借方合计" });
            foreach (var item in resultFormula)
            {
                var dict = dictlist.Where(s => s.DataDictID == item.FormulaID).FirstOrDefault();
                if (dict != null)
                {
                    item.FormulaName = dict.DataDictName;
                }
                else
                {
                    item.FormulaName = item.FormulaID;//返回常量时使用
                }
            }
            //动态映射公式列
            foreach (var item in result)
            {
                string dformula = item.DebitSecFormula;
                string cformula = item.CreditSecFormula;
                foreach (var dict in dictlist)
                {
                    if (!string.IsNullOrEmpty(dformula))
                    {
                        if ((bool)dformula?.Contains(dict.DataDictID))
                        {
                            var value = dict.DataDictName.TrimStart('[').TrimEnd(']');
                            dformula = dformula?.Replace("[" + dict.DataDictID + "]", "[" + (value) + "]");
                        }
                    }
                    if (!string.IsNullOrEmpty(cformula))
                    {
                        if ((bool)cformula?.Contains(dict.DataDictID))
                        {
                            var value = dict.DataDictName.TrimStart('[').TrimEnd(']');
                            cformula = cformula?.Replace("[" + dict.DataDictID + "]", "[" + (value) + "]");
                        }
                    }
                }
                item.DebitFormula = dformula?.Replace("[2201101409320000151]", "").Replace("[2201101409320000152]", "");
                item.CreditFormula = cformula?.Replace("[2201101409320000151]", "").Replace("[2201101409320000152]", "");
            }
        }
        /// <summary>
        /// 获取公式信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<BIZ_DataDictODataEntity> GetDataDictAsync(long pid)
        {
            try
            {
                FormattableString sql = $@"SELECT  CONVERT(DataDictID USING utf8mb4) DataDictID,DataDictName,	
                                        CONVERT(DataDictType USING utf8mb4) DataDictType,	
                                        CONVERT(PID USING utf8mb4) PID,	
                                        CONVERT(EnterpriseID USING utf8mb4) EnterpriseID,
                                        CONVERT(CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(ModifiedDate USING utf8mb4) ModifiedDate,'' as cPrivCode
                                        FROM `nxin_qlw_business`.`biz_datadict`
                                        WHERE PID={pid}";
                return _context.BIZ_DataDictDataSet.FromSqlInterpolated(sql).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取薪资计提公式
        /// </summary>
        public List<BIZ_DataDictODataEntity> GetSalarySetItemList(string EnterpriseId, out bool IsSetNewFormula)
        {
            IsSetNewFormula = true;
            try
            {
                List<BIZ_DataDictODataEntity> list = new List<BIZ_DataDictODataEntity>();
                AuthenticationHeaderValue authentication = null;
                bool verification = AuthenticationHeaderValue.TryParse(_httpContextAccessor.HttpContext.Request.GetAuthToken(), out authentication);
                if (!verification)
                {
                    return new List<BIZ_DataDictODataEntity>();
                }
                string url = $"{_hostCongfiguration.DBN_HrServiceHost}/api/HrSalaryInfo/getSalarySetItem";
                var res = _httpClientUtil.PostJsonAsync<ResultModel<SalarySetItem>>(url, new { EnterpriseId = EnterpriseId }, (a) => { a.Authorization = authentication; }).Result;
                var result = res.Code == 0 ? res.Data : new List<SalarySetItem>();
                foreach (var item in result)
                {
                    list.Add(new BIZ_DataDictODataEntity()
                    {
                        DataDictID = item.SetItemId,
                        DataDictName = item.SetItemName
                    });
                }
                list.Add(new BIZ_DataDictODataEntity()
                {
                    DataDictID = "2111261612570000166",
                    DataDictName = "月收入"
                });
                list.Add(new BIZ_DataDictODataEntity()
                {
                    DataDictID = "2111261612570000188",
                    DataDictName = "绩效工资"
                });
                return list;

            }
            catch (Exception ex)
            {
                return new List<BIZ_DataDictODataEntity>();
            }
        }
        /// <summary>
        /// 获取费用分摊结转公式
        /// </summary>
        public List<BIZ_DataDictODataEntity> GetShareCostItemList(string EnterpriseId, out bool IsSetNewFormula)
        {
            IsSetNewFormula = true;
            try
            {
                List<BIZ_DataDictODataEntity> list = new List<BIZ_DataDictODataEntity>();
                string url = $"{_hostCongfiguration._wgUrl}/cost/management/api/FM_CostProject/GetCostProjectByEnterpriseID?enterpriseId=" + EnterpriseId;
                var result = _httpClientUtil.GetJsonAsync<List<ShareCostItem>>(url).Result;
                foreach (var item in result)
                {
                    list.Add(new BIZ_DataDictODataEntity()
                    {
                        DataDictID = item.CostProjectId,
                        DataDictName = item.CostProjectCode + item.CostProjectName
                    });
                }
                return list;

            }
            catch (Exception ex)
            {
                return new List<BIZ_DataDictODataEntity>();
            }
        }
        /// <summary>
        /// 获取禽成本结转公式
        /// </summary>
        public List<BIZ_DataDictODataEntity> GetPoultryCostItemList()
        {
            AuthenticationHeaderValue authentication = null;
            AuthenticationHeaderValue.TryParse(_httpContextAccessor.HttpContext.Request.GetAuthToken(), out authentication);
            try
            {
                List<BIZ_DataDictODataEntity> list = new List<BIZ_DataDictODataEntity>();
                var result = _httpClientUtil.PostJsonAsync<List<BIZ_DataDictODataEntity>>($"{_hostCongfiguration._wgUrl}/q/reportbreed/PoultryCostProject", null,
                        (a) => { a.Authorization = authentication; }).Result;
                foreach (var item in result)
                {
                    list.Add(new BIZ_DataDictODataEntity()
                    {
                        DataDictID = item.DataDictID,
                        DataDictName = item.DataDictName
                    });
                }
                return list;

            }
            catch (Exception ex)
            {
                return new List<BIZ_DataDictODataEntity>();
            }
        }
        public override Task<List<FM_NewCarryForwardVoucherDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            throw new NotImplementedException();
        }
        public override Task<FM_NewCarryForwardVoucherODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 预收款核销
        /// </summary>
        /// <returns></returns>
        public Task<List<AdvanceCollectionODataEnetity>> AdvanceCollectionDataList(FM_CarryForwardVoucherSearchCommand model)
        {
            string sql = $@" SELECT CONVERT(FLOOR(1 + (RAND() * 100000000)), CHAR) AS PrimaryKey,
	                                CONVERT(CustomerID, CHAR) AS CustomerID,
	                                CONVERT(SalesmanID, CHAR) AS SalesmanID,
	                                CONVERT(MarketID, CHAR) AS MarketID,
	                                SUM(CancellationAmount) AS CancellationAmount  FROM (
                                                    SELECT ParentValue SKNumericalOrder, IFNULL(B2.Payment, 0) AS CancellationAmount,B2.`OwnerID` AS SKOwnerID,
                                                    pe.Name AS SKName,DATE_FORMAT(B2.`ModifiedDate`,'%Y-%m-%d') AS HXDate,
                                                    sa.*
                                                    FROM nxin_qlw_business.BIZ_Related B
                                                    INNER JOIN nxin_qlw_business.BIZ_RelatedDetail B2 ON B2.RelatedID = B.RelatedID
                                                    AND B.RelatedType = 201610210104402122  -- 关联关系
                                                    AND B.ParentType IN (1611231950150000101,1612101120530000101)  -- 收款单/收款汇总
                                                    AND B.ChildType = 1610311318270000101   -- 销售单
                                                    AND B2.RelatedDetailType=1910181122150000102 -- 预收款核销Appid
                                                    LEFT JOIN `nxin_qlw_business`.`hr_person` pe ON B2.OwnerID=pe.BO_ID
                                                    LEFT JOIN nxin_qlw_business.`sa_sales` sa ON b.`ChildValue`=sa.`NumericalOrder`
                                                    WHERE sa.`EnterpriseID`= {model.EnterpriseID}
                                                    HAVING HXDate BETWEEN '{model.Begindate}' AND '{model.Enddate}'
                                       )  temp   GROUP BY CustomerID,SalesmanID,MarketID ";
            return _context.AdvanceCollectionDataSet.FromSqlRaw(sql).ToListAsync();
        }

        /// <summary>
        /// 税费抵扣
        /// </summary>
        /// <returns></returns>
        public Task<List<ExpenseODataEnetity>> ExpenseDataList(FM_CarryForwardVoucherSearchCommand model)
        {
            string sql = $@" SELECT CONVERT(FLOOR(1 + (RAND() * 100000000)), CHAR) AS PrimaryKey,
                                CONVERT(f.PersonID, CHAR) AS SalesmanID,SUM(k.TaxAmount) AS TaxAmount,
                                CONVERT(bm.MarketID, CHAR) AS MarketID,k.Type
                                FROM qlw_nxin_com.fd_expense f
                                LEFT JOIN qlw_nxin_com.fd_trafficticket k ON f.NumericalOrder = k.NumericalOrder
                                LEFT JOIN nxin_qlw_business.biz_related r ON r.RelatedType =201610210104402122 AND r.ChildValue =f.NumericalOrder
                                LEFT JOIN nxin_qlw_business.fd_settlereceipt t1 ON r.ParentValue=t1.NumericalOrder -- AND f.PersonID=t1.PersonID
                                LEFT JOIN qlw_nxin_com.bsdatadict c  ON f.ExpenseType=c.DictID
                                LEFT JOIN nxin_qlw_business.hr_person d ON f.PersonID=d.PersonID
                                INNER JOIN `qlw_nxin_com`.hr_postinformation hp ON hp.IsUse = 1 AND hp.PersonID = d.PersonID
                                LEFT JOIN `qlw_nxin_com`.biz_market bm ON bm.MarketID = hp.MarketID
                                WHERE f.EnterpriseID= {model.EnterpriseID} AND f.DataDate  BETWEEN '{model.Begindate}' AND '{model.Enddate}'  AND k.Type<>5 AND k.TaxAmount<>0 
                                GROUP BY f.PersonID,bm.MarketID,k.Type ";
            return _context.ExpenseDataSet.FromSqlRaw(sql).ToListAsync();
        }
    }
}
