using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FM_CarryForwardVoucher;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class FM_CarryForwardVoucherODataProvider : OneWithManyQueryProvider<FM_CarryForwardVoucherODataEntity, FM_CarryForwardVoucherDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        FMBaseCommon _baseUnit;

        public FM_CarryForwardVoucherODataProvider(IIdentityService identityservice, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel)
        {
            _baseUnit = baseUnit;
            _identityservice = identityservice;
            _context = context;
            _treeModel = treeModel;
        }

        public IEnumerable<FM_CarryForwardVoucherODataEntity> GetList(ODataQueryOptions<FM_CarryForwardVoucherODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FM_CarryForwardVoucherODataEntity> GetData(ODataQueryOptions<FM_CarryForwardVoucherODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FM_CarryForwardVoucherODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.TransferAccountsType USING utf8mb4) TransferAccountsType,	
                                        dict.DataDictName AS TransferAccountsTypeName,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                        tick.TicketedPointName,
                                        CONVERT(t1.DataSource USING utf8mb4) DataSource,
                                        st.cText AS DataSourceName,	
                                        CONVERT(t1.TransferAccountsAbstract USING utf8mb4) TransferAccountsAbstract,
                                        ifnull(dict1.DataDictName,bsdict.cDictName) AS TransferAccountsAbstractName,	
                                        CONVERT(t1.TransferAccountsSort USING utf8mb4) TransferAccountsSort,
                                        dict2.DataDictName AS TransferAccountsSortName,	
                                        CONVERT(t1.Remarks USING utf8mb4) Remarks,	
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP1.Name AS OwnerName,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(t1.TransactorID USING utf8mb4) TransactorID,	
                                        HP2.Name AS TransactorName,
                                        CONVERT(t1.TransactorDate USING utf8mb4) TransactorDate,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM
                                        `nxin_qlw_business`.`fm_carryforwardvoucher` t1
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON t1.TransferAccountsType=dict.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON t1.TicketedPointID=tick.TicketedPointID AND t1.EnterpriseID=tick.EnterpriseID
                                        LEFT JOIN `qlw_nxin_com`.`stmenu` st ON t1.DataSource=st.MenuID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict1 ON t1.TransferAccountsAbstract=dict1.DataDictID
                                        LEFT JOIN `qlw_nxin_com`.BSDataDict bsdict ON t1.TransferAccountsAbstract=bsdict.DictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict2 ON t1.TransferAccountsSort =dict2.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.TransactorID=HP2.Bo_ID
                                    WHERE t1.NumericalOrder ={manyQuery}";
            return _context.FM_CarryForwardVoucherDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<FM_CarryForwardVoucherODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.TransferAccountsType USING utf8mb4) TransferAccountsType,	
                                        dict.DataDictName AS TransferAccountsTypeName,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                        tick.TicketedPointName,
                                        CONVERT(t1.DataSource USING utf8mb4) DataSource,
                                        st.cText AS DataSourceName,	
                                        CONVERT(t1.TransferAccountsAbstract USING utf8mb4) TransferAccountsAbstract,
                                        ifnull(dict1.DataDictName,bsdict.cDictName) AS TransferAccountsAbstractName,
                                        CONVERT(t1.TransferAccountsSort USING utf8mb4) TransferAccountsSort,
                                        dict2.DataDictName AS TransferAccountsSortName,	
                                        CONVERT(t1.Remarks USING utf8mb4) Remarks,	
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP1.Name AS OwnerName,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(t1.TransactorID USING utf8mb4) TransactorID,	
                                        HP2.Name AS TransactorName,
                                        CONVERT(t1.TransactorDate USING utf8mb4) TransactorDate,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM
                                        `nxin_qlw_business`.`fm_carryforwardvoucher` t1
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON t1.TransferAccountsType=dict.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON t1.TicketedPointID=tick.TicketedPointID AND t1.EnterpriseID=tick.EnterpriseID
                                        LEFT JOIN `qlw_nxin_com`.`stmenu` st ON t1.DataSource=st.MenuID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict1 ON t1.TransferAccountsAbstract=dict1.DataDictID
                                        LEFT JOIN `qlw_nxin_com`.BSDataDict bsdict ON t1.TransferAccountsAbstract=bsdict.DictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict2 ON t1.TransferAccountsSort =dict2.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.TransactorID=HP2.Bo_ID
                                        WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId} 
                                        GROUP BY t1.NumericalOrder
                                        order by t1.CreatedDate desc ";
            return _context.FM_CarryForwardVoucherDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public override Task<List<FM_CarryForwardVoucherDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            List<TreeModelODataEntity> produtClassList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> supplierList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> suppliesList = new List<TreeModelODataEntity>();

            List<TreeModelODataEntity> chickenList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> breedList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> batchList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> changquList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> jisheList = new List<TreeModelODataEntity>();


            FormattableString extend = $@"SELECT A.RecordID as RecordID,CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail, 
                                        A.Sort,'' as SortName,CONVERT(A.Symbol USING utf8mb4) Symbol,
                                        CONVERT(A.Object USING utf8mb4) Object,'' as ObjectName,
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FM_CarryForwardVoucherExtend as A
                                        WHERE NumericalOrder={manyQuery}";
            var resultExt = _context.FM_CarryForwardVoucherExtendDataSet.FromSqlInterpolated(extend).ToList();

            FormattableString formula = $@"SELECT A.RecordID AS RecordID,CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail, 
                                        A.RowNum,
                                        CONVERT(A.Bracket USING utf8mb4) Bracket,
                                        CONVERT(A.FormulaID USING utf8mb4) FormulaID,
                                        CONVERT(A.Operator USING utf8mb4) Operator,
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM `nxin_qlw_business`.FM_CarryForwardVoucherFormula AS A
                                        WHERE NumericalOrder={manyQuery}";
            var resultFormula = _context.FM_CarryForwardVoucherFormulaDataSet.FromSqlInterpolated(formula).ToList();


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
                        default:
                            break;
                    }
                }

            }
            FormattableString sql = $@"SELECT B.RecordID,CONVERT(B.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(B.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        CONVERT(B.ReceiptAbstractID USING utf8mb4) ReceiptAbstractID,
                                        se.SettleSummaryName AS ReceiptAbstractName,
                                        CONVERT(B.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                        acc.AccoSubjectFullName as AccoSubjectName,
                                        CONVERT(B.AccoSubjectCode USING utf8mb4) AccoSubjectCode,
                                        B.IsPerson,B.IsCustomer,B.IsMarket,B.IsProduct,B.DebitFormula,B.DebitSecFormula,B.CreditFormula,B.CreditSecFormula,
                                        CONVERT(B.ModifiedDate USING utf8mb4) ModifiedDate
                                         FROM `nxin_qlw_business`.FM_CarryForwardVoucherDetail  B
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` se ON B.ReceiptAbstractID=se.SettleSummaryID AND (se.EnterpriseID={_identityservice.EnterpriseId} or se.EnterpriseID={_identityservice.GroupId})
                                        LEFT JOIN `qlw_nxin_com`.`biz_accosubject` acc ON B.AccoSubjectID=acc.AccoSubjectID AND (acc.EnterpriseID={_identityservice.EnterpriseId} or acc.EnterpriseID={_identityservice.GroupId})
                                    WHERE B.NumericalOrder ={manyQuery}";
            var result = _context.FM_CarryForwardVoucherDetailDataSet.FromSqlInterpolated(sql).ToList();

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
                        default:
                            break;
                    }
                });
                s.Extends = list;
                s.Formulas = list1;
            });
            var task = Task.Factory.StartNew(() =>
               {
                   return result;
               });
            return task;
        }


        /// <summary>
        /// 生成凭证结果列表`
        /// </summary>
        /// <returns></returns>
        public IQueryable<FM_CarryForwardVoucherRecordODataEntity> GetRecordList(ODataQueryOptions<FM_CarryForwardVoucherRecordODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@" SELECT 
                                            t1.RecordID,	
                                            CONVERT(t1.NumericalOrderCarry USING utf8mb4) NumericalOrderCarry,	
                                            CONVERT(t1.NumericalOrderSettl USING utf8mb4) NumericalOrderSettl,	
                                            CONVERT(t1.TransferAccountsType USING utf8mb4) TransferAccountsType,	
                                            dict.`DataDictName` AS TransferAccountsName,
                                            t1.CarryName,
                                            CONVERT(t1.DataSource USING utf8mb4) DataSource,
                                            st.`cText` AS DataSourceName,
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
                                             FROM `nxin_qlw_business`.`fm_carryforwardvoucherrecord` t1
                                            LEFT JOIN `nxin_qlw_business`.`FM_CarryForwardVoucher` t2 ON t1.NumericalOrderCarry=t2.`NumericalOrder`
                                            LEFT JOIN `nxin_qlw_business`.`fd_settlereceipt` t3 ON t1.`NumericalOrderSettl`=t3.`NumericalOrder`
                                            LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON t1.TransferAccountsType=dict.DataDictID
                                            LEFT JOIN `qlw_nxin_com`.`stmenu` st ON t1.DataSource=st.MenuID
                                            LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.OwnerID=HP2.Bo_ID
                                        WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId} 
                                        ORDER BY t1.CreatedDate DESC ";
            return _context.FM_CarryForwardVoucherRecordDataSet.FromSqlInterpolated(sql);
        }


        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        public FM_CarryForwardVoucherODataEntity GetSingleData(long manyQuery)
        {
            FormattableString sql = $@"SELECT  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(t1.TransferAccountsType USING utf8mb4) TransferAccountsType,	
                                        dict.DataDictName AS TransferAccountsTypeName,
                                        CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                        tick.TicketedPointName,
                                        CONVERT(t1.DataSource USING utf8mb4) DataSource,
                                        st.cText AS DataSourceName,	
                                        CONVERT(t1.TransferAccountsAbstract USING utf8mb4) TransferAccountsAbstract,
                                        ifnull(dict1.DataDictName,bsdict.cDictName) AS TransferAccountsAbstractName,	
                                        CONVERT(t1.TransferAccountsSort USING utf8mb4) TransferAccountsSort,
                                        dict2.DataDictName AS TransferAccountsSortName,	
                                        CONVERT(t1.Remarks USING utf8mb4) Remarks,	
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                        HP1.Name AS OwnerName,	
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(t1.TransactorID USING utf8mb4) TransactorID,	
                                        HP2.Name AS TransactorName,
                                        CONVERT(t1.TransactorDate USING utf8mb4) TransactorDate,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM
                                        `nxin_qlw_business`.`fm_carryforwardvoucher` t1
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON t1.TransferAccountsType=dict.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON t1.TicketedPointID=tick.TicketedPointID AND t1.EnterpriseID=tick.EnterpriseID
                                        LEFT JOIN `qlw_nxin_com`.`stmenu` st ON t1.DataSource=st.MenuID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict1 ON t1.TransferAccountsAbstract=dict1.DataDictID
                                        LEFT JOIN `qlw_nxin_com`.BSDataDict bsdict ON t1.TransferAccountsAbstract=bsdict.DictID
                                        LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict2 ON t1.TransferAccountsSort =dict2.DataDictID
                                        LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536
                                        LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.TransactorID=HP2.Bo_ID
                                    WHERE t1.NumericalOrder ={manyQuery}";
            var main = _context.FM_CarryForwardVoucherDataSet.FromSqlInterpolated(sql).FirstOrDefault();
            var detail = GetDetaiDatasAsync(manyQuery).Result.ToList();
            main.Lines = detail;
            return main;
        }
    }
}
