using Architecture.Common.Application.Query;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using RabbitMQ.Client.Framing.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    public class FD_PaymentReceivablesTODataProvider : OneWithManyQueryProvider<FD_PaymentReceivablesEntity, FD_PaymentReceivablesDetailEntity>
    {
        ILogger<FD_PaymentReceivablesTODataProvider> _logger;
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_PaymentReceivablesTODataProvider(ILogger<FD_PaymentReceivablesTODataProvider> logger, IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
            _logger = logger;
        }

        public IEnumerable<FD_PaymentReceivablesEntity> GetList(string entes)
        {
            var datas = GetData(entes).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_PaymentReceivablesEntity> GetData(string entes)
        {
            return GetDatas(entes);
        }
        public List<FD_PaymentExtendEntity> GetExtend(string num)
        {
            FormattableString sql = $@"SELECT 
                                        f2.`RecordID`,
                                        CONCAT(f2.`NumericalOrder`) AS NumericalOrder,
                                        CONCAT(f2.`CollectionId`) AS CollectionId,
                                        IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`) AS CollectionName,
                                        f2.AccountName,
                                        f2.`BankDeposit`,
                                        f2.`BankAccount`,
                                        f2.`Amount` ,
                                        CONCAT(hr.BO_ID) BO_ID,
                                        CONCAT(f2.personid) PersonID,
                                        f2.`BankAccount` AS BankNumber,
                                        sc.AccountNature,
                                        bs.KeyIndex AS BankCode,
                                        f2.BankDeposit `BankName`,
                                        CONCAT(f2.RecheckId) RecheckId ,
                                        e7.Name RecheckName,
                                        f2.PayResult,f2.Status ,f2.IsRecheck,CONCAT(fp.`Number`) Number ,tp.`TicketedPointName`,
                                        CONCAT(f2.TradeNo) TradeNo
                                        FROM
                                        `nxin_qlw_business`.`fd_paymentextend` f2 
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivables` fp ON fp.`NumericalOrder` = f2.`NumericalOrder` 
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = fp.`TicketedPointID`
                                        LEFT JOIN (SELECT 
                                          bs.`RecordID`,
                                          CONCAT(SUBSTRING_INDEX(bs.BankDeposit,'银行',1),'银行' ) `BankDeposit`,
                                          BankDeposit `BankName`
                                        FROM
                                          `nxin_qlw_business`.`fd_paymentextend` bs) f22 ON f22.RecordID = f2.RecordID
                                            LEFT JOIN `qlw_nxin_com`.BSDataDict bs ON bs.cdictname = f22.BankDeposit
                                            LEFT JOIN nxin_qlw_business.hr_person hr ON hr.personid = f2.personid
                                            LEFT JOIN nxin_qlw_business.SA_CustomerAccount sc ON sc.CustomerID = f2.CollectionId
                                            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = f2.`CollectionId` -- 单位
                                            LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = f2.`CollectionId` -- 员工
                                            LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = f2.`CollectionId` -- 客户
                                            LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = f2.`CollectionId` -- 供应商
                                            LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = f2.`CollectionId` -- 部门
                                            LEFT JOIN `nxin_qlw_business`.`hr_person` e7 on e7.bo_id = f2.RecheckId  -- 复核人
                                            WHERE f2.NumericalOrder = {num}
                                            GROUP BY f2.`RecordID`
    ";
            return _context.FD_PaymentExtendEntityDataSet.FromSqlInterpolated(sql).ToList();
        }
        /// <summary>
        /// 支付列表
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<PaymentList> GetPaymentList(string num)
        {
            var Head = GetDataAsync(Convert.ToInt64(num)).Result;
            FormattableString sql = null;
            //付款单 收款人唯一
            if (Head?.SettleReceipType == "201611180104402202")
            {
                sql = $@"SELECT 
                                      f2.RecordId,
                                      CONCAT(fa.AccountNumber) AS AccountID,
                                      fa.AccountFullName AS AccountName,
                                      fa.BankNumber,
                                      d.KeyIndex AS PayBank,
                                      f2.Amount,
                                      CONCAT(pe.PersonId) AS PersonId,
                                      IFNULL(IFNULL(IFNULL(hp.Name,e4.`CustomerName`),e5.`SupplierName`),e6.`Name`) AS PersonName,
                                      CONCAT(pe.CollectionId) AS CollectionId,
                                      pe.AccountName AS CollectionName,
                                      pe.BankDeposit AS CollectionBank,
                                      pe.BankAccount,
                                      pe.PayResult,
                                      pe.Status,
                                      pe.RecordId AS extendRecordID,
                                      pe.PayUse,
                                      pe.TransferType,
                                      pe.IsRecheck
                                    FROM
                                      `nxin_qlw_business`.`fd_paymentreceivables` f 
                                      INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` f2 
                                        ON f.`NumericalOrder` = f2.`NumericalOrder` 
                                        AND f.`IsGroupPay` = 1 
                                      LEFT JOIN `nxin_qlw_business`.`fd_account` fa 
                                        ON fa.`AccountID` = f2.`AccountID` 
                                      LEFT JOIN qlw_nxin_com.BSDataDict d 
                                        ON d.dictid = fa.BankID 
                                      LEFT JOIN nxin_qlw_business.fd_paymentextend pe 
                                        ON pe.NumericalOrder = f.numericalorder 
                                      LEFT JOIN `nxin_qlw_business`.`hr_person` hp ON hp.`BO_ID` = pe.`PersonId`
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = pe.`PersonId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = pe.`PersonId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e6 ON e6.`PersonId` = pe.`PersonId` -- 部门
                                    Where f.NumericalOrder = {num} 
                                    GROUP BY f2.`RecordID` ";
            }
            //汇总单，收款人不唯一
            else
            {
                sql = $@"SELECT 
                                      f2.RecordId,
                                      CONCAT(fa.AccountNumber) AS AccountID,
                                      fa.AccountName,
                                      fa.BankNumber,
                                      d.KeyIndex AS PayBank,
                                      f2.Amount,
                                      CONCAT(pe.PersonId) AS PersonId,
                                      IFNULL(IFNULL(IFNULL(IFNULL(hp.Name,e4.`CustomerName`),e5.`SupplierName`),e6.`Name`),e7.`EnterpriseName`) AS PersonName,
                                      CONCAT(pe.CollectionId) AS CollectionId,
                                      pe.AccountName AS CollectionName,
                                      pe.BankDeposit AS CollectionBank,
                                      pe.BankAccount,
                                      pe.PayResult,
                                      pe.Status,
                                      pe.RecordId AS extendRecordID,
                                      pe.PayUse,
                                      pe.TransferType,
                                      pe.IsRecheck
                                    FROM
                                      `nxin_qlw_business`.`fd_paymentreceivables` f 
                                      INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` f2 
                                        ON f.`NumericalOrder` = f2.`NumericalOrder` 
                                        AND f.`IsGroupPay` = 1 
                                      LEFT JOIN `nxin_qlw_business`.`fd_account` fa 
                                        ON fa.`AccountID` = f2.`AccountID` 
                                      LEFT JOIN qlw_nxin_com.BSDataDict d 
                                        ON d.dictid = fa.BankID 
                                      LEFT JOIN nxin_qlw_business.fd_paymentextend pe 
                                        ON pe.NumericalOrder = f.numericalorder AND pe.`Guid` = f2.`Guid`
                                      LEFT JOIN `nxin_qlw_business`.`hr_person` hp ON hp.`BO_ID` = pe.`PersonId`
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = pe.`PersonId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = pe.`PersonId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e6 ON e6.`PersonId` = pe.`PersonId` -- 部门
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e7 ON e7.`enterpriseid` = pe.`PersonId` -- 单位
                                    Where f.NumericalOrder = {num} 
                                    GROUP BY f2.`RecordID` ";
            }
             
            return _context.PaymentListEntityDataSet.FromSqlInterpolated(sql).ToList();
        }
        /// <summary>
        /// 支付结果列表 对外接口
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<PaymentOpenList> GetPaymentListByOpen(string num)
        {
            var Head = GetDataAsync(Convert.ToInt64(num)).Result;
            FormattableString sql = null;
            //付款单 收款人唯一
            if (Head?.SettleReceipType == "201611180104402202")
            {
                sql = $@"SELECT 
                                      f2.`RecordID`,f.`IsGroupPay`,CONCAT(f2.`NumericalOrder`) NumericalOrder,CONCAT(pe.`TradeNo`) TradeNo,
                                      CONCAT(fa.AccountNumber) AS ffAccountID,
                                      fa.`AccountName` AS ffAccountName,
                                      fa.DepositBank ffDepositBank,
                                      pe.BankAccount AS sfBankNumber,
                                      pe.BankDeposit AS sfAccountName,
                                      IFNULL(IFNULL(IFNULL(hp.Name,e4.`CustomerName`),e5.`SupplierName`),e6.`Name`) AS sfPersonName,
                                      f2.Amount,
                                      pe.PayResult,
                                      (CASE WHEN pe.Status = '1' THEN '交易成功'
                                      WHEN pe.Status = '2' THEN '交易失败'
                                      WHEN pe.Status = '3' THEN '处理中'
                                      WHEN pe.Status = '0' THEN '未发起' END) sfStatus,
                                      pe.PayUse
                                    FROM
                                      `nxin_qlw_business`.`fd_paymentreceivables` f 
                                      INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` f2 
                                        ON f.`NumericalOrder` = f2.`NumericalOrder` 
                                        AND f.`IsGroupPay` = 1 
                                      LEFT JOIN `nxin_qlw_business`.`fd_account` fa 
                                        ON fa.`AccountID` = f2.`AccountID` 
                                      LEFT JOIN qlw_nxin_com.BSDataDict d 
                                        ON d.dictid = fa.BankID 
                                      LEFT JOIN nxin_qlw_business.fd_paymentextend pe 
                                        ON pe.NumericalOrder = f.numericalorder 
                                      LEFT JOIN `nxin_qlw_business`.`hr_person` hp ON hp.`BO_ID` = pe.`PersonId`
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = pe.`PersonId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = pe.`PersonId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e6 ON e6.`PersonId` = pe.`PersonId` -- 部门
                                    WHERE f.NumericalOrder = {num}
                                    GROUP BY f2.`RecordID` ";
            }
            //汇总单，收款人不唯一
            else
            {
                sql = $@"SELECT 
                                      f2.`RecordID`,f.`IsGroupPay`,CONCAT(f2.`NumericalOrder`) NumericalOrder,CONCAT(pe.`TradeNo`) TradeNo,
                                      CONCAT(fa.AccountNumber) AS ffAccountID,
                                      fa.`AccountName` AS ffAccountName,
                                      fa.DepositBank ffDepositBank,
                                      pe.BankAccount AS sfBankNumber,
                                      pe.BankDeposit AS sfAccountName,
                                      IFNULL(IFNULL(IFNULL(hp.Name,e4.`CustomerName`),e5.`SupplierName`),e6.`Name`) AS sfPersonName,
                                      f2.Amount,
                                      pe.PayResult,
                                      (CASE WHEN pe.Status = '1' THEN '交易成功'
                                      WHEN pe.Status = '2' THEN '交易失败'
                                      WHEN pe.Status = '3' THEN '处理中'
                                      WHEN pe.Status = '0' THEN '未发起' END) sfStatus,
                                      pe.PayUse
                                    FROM
                                      `nxin_qlw_business`.`fd_paymentreceivables` f 
                                      INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` f2 
                                        ON f.`NumericalOrder` = f2.`NumericalOrder` 
                                        AND f.`IsGroupPay` = 1 
                                      LEFT JOIN `nxin_qlw_business`.`fd_account` fa 
                                        ON fa.`AccountID` = f2.`AccountID` 
                                      LEFT JOIN qlw_nxin_com.BSDataDict d 
                                        ON d.dictid = fa.BankID 
                                      LEFT JOIN nxin_qlw_business.fd_paymentextend pe 
                                        ON pe.NumericalOrder = f.numericalorder AND pe.`Guid` = f2.`Guid`
                                      LEFT JOIN `nxin_qlw_business`.`hr_person` hp ON hp.`BO_ID` = pe.`PersonId`
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = pe.`PersonId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = pe.`PersonId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e6 ON e6.`PersonId` = pe.`PersonId` -- 部门
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e7 ON e7.`enterpriseid` = pe.`PersonId` -- 单位
                                    Where f.NumericalOrder = {num} 
                                    GROUP BY f2.`RecordID` ";
            }

            return _context.PaymentOpenListEntityDataSet.FromSqlInterpolated(sql).ToList();
        }
        /// <summary>
        /// 复核支付列表
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<RecheckPaymentList> GetRecheckPaymentList(string entes,int isExitRstatus = 0,string userid = "0",string begindate = "",string enddate = "")
        {
            string sql = $@"         SELECT 
                                                  CONCAT(UUID()) rrid,COUNT(rfmd.ParentValueDetail) NoReviewPerson,
                                                  CONVERT(DATE_FORMAT( f.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                                  IF(IFNULL(expen.Pressing,0) = 0,99,0) Level,
                                                  IFNULL(rfm.`ParentType`,0) AS MaxLevel,
                                                  IFNULL(rfm.`ChildType`,0) AS RawLevel,
                                                  IF(rfm.`ParentType` IS NULL,pe.Status,IFNULL(rfm.`ChildValueDetail`,0)) RStatus,
                                                  IFNULL(CONCAT(rfm.ParentValueDetail),0) AS rPersonId,
                                                  hp2.Name as rPersonName,
                                                  CONCAT(f.`Number`) Number,
                                                  CONCAT(f.`EnterpriseID`) EnterpriseID,
                                                  e.`EnterpriseName`,
                                                  f2.RecordId,
                                                  CONCAT(fa.AccountID) AS AccountID,
                                                  fa.AccountName,
                                                  fa.AccountNumber,
                                                  d.KeyIndex AS PayBank,
                                                  f2.Amount,
                                                  CONCAT(pe.PersonId) AS PersonId,
                                                  IFNULL(IFNULL(IFNULL(hp.Name,e4.`CustomerName`),e5.`SupplierName`),e6.`Name`) AS PersonName,
                                                  CONCAT(pe.CollectionId) AS CollectionId,
                                                  pe.AccountName AS CollectionName,
                                                  pe.BankDeposit AS CollectionBank,
                                                  pe.BankAccount,
                                                  pe.PayResult,
                                                  pe.Status,
                                                  pe.RecordId AS extendRecordID,
                                                  pe.PayUse,
                                                  pe.TransferType,
                                                  e7.`Name` AS RecheckName,
                                                  e77.`Photo` RecheckPhoto,
                                                  CONCAT(pe.`RecheckId`) RecheckId,
                                                  CONCAT(f.`SettleReceipType`) SettleReceipType,
                                                  CONCAT(f.NumericalOrder) NumericalOrder
                                                FROM
                                                  `nxin_qlw_business`.`fd_paymentreceivables` f 
                                                  INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` f2 
	                                            ON f.`NumericalOrder` = f2.`NumericalOrder` 
	                                            AND f.`IsGroupPay` = 1 
                                                  LEFT JOIN `nxin_qlw_business`.`fd_account` fa 
	                                            ON fa.`AccountID` = f2.`AccountID` 
                                                  LEFT JOIN qlw_nxin_com.BSDataDict d 
	                                            ON d.dictid = fa.BankID 
                                                  LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e
	                                            ON e.`EnterpriseID` = f.`EnterpriseID`
                                                  LEFT JOIN nxin_qlw_business.fd_paymentextend pe 
	                                            ON pe.NumericalOrder = f.numericalorder 
                                                  LEFT JOIN `nxin_qlw_business`.`hr_person` hp ON hp.`BO_ID` = pe.`PersonId`
	                                            LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = pe.`PersonId` -- 客户
	                                            LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = pe.`PersonId` -- 供应商
	                                            LEFT JOIN `nxin_qlw_business`.`hr_person` e6 ON e6.`PersonId` = pe.`PersonId` -- 部门
	                                            LEFT JOIN `nxin_qlw_business`.`hr_person` e7 ON e7.`BO_ID` = pe.`RecheckId`
	                                            LEFT JOIN `qlw_nxin_com`.`hr_person` e77 ON e77.personid = e7.`PersonID`
                                                LEFT JOIN nxin_qlw_business.biz_related r ON r.ParentValue = f.Numericalorder
                                   --             LEFT JOIN nxin_qlw_business.fd_scheduleplan plan ON plan.GroupId = {_identityservice.GroupId} AND plan.`PayNumericalOrder` = f.`Numericalorder` 
                                                LEFT JOIN qlw_nxin_com.fd_expense expen ON expen.NumericalOrder = r.ChildValue
                                                LEFT JOIN `nxin_qlw_business`.`biz_related_fm` rfm on rfm.ParentValue = pe.`NumericalOrder` and rfm.childvalue = pe.`RecordID` and rfm.`RelatedType` = 2205231634370000109 AND rfm.ParentType <> 2205231634370000109
                                                LEFT JOIN `nxin_qlw_business`.`biz_related_fm` rfmd ON rfmd.ParentValue = pe.`NumericalOrder` AND rfmd.childvalue = pe.`RecordID` AND rfmd.`RelatedType` = 2205231634370000109 AND rfmd.childValueDetail = 0 AND rfmd.ParentType <> 2205231634370000109
                                                LEFT JOIN `nxin_qlw_business`.`hr_person` hp2 on hp2.`BO_ID` = rfm.`ParentValueDetail`
                                                WHERE f.EnterpriseID IN ({entes}) AND f.`SettleReceipType` = 201611180104402202 AND pe.`IsRecheck` = TRUE AND f.DataDate BETWEEN '{begindate}' AND '{enddate}' 
                                                GROUP BY f2.`RecordID`,rfm.RelatedID
                                                UNION ALL
                                                SELECT 
                                                  CONCAT(UUID()) rrid,COUNT(rfmd.ParentValueDetail) NoReviewPerson,
                                                  CONVERT(DATE_FORMAT( f.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                                  IF(IFNULL(expen.Pressing,0) = 0,99,0) Level,
                                                  IFNULL(rfm.`ParentType`,0) AS MaxLevel,
                                                  IFNULL(rfm.`ChildType`,0) AS RawLevel,
                                                  IF(rfm.`ParentType` IS NULL,pe.Status,IFNULL(rfm.`ChildValueDetail`,0)) RStatus,
                                                  IFNULL(CONCAT(rfm.ParentValueDetail),0) AS rPersonId,
                                                  hp2.Name as rPersonName,
                                                  CONCAT(f.`Number`) Number,
                                                  CONCAT(f.`EnterpriseID`) EnterpriseID,
                                                  e.`EnterpriseName`,
                                                  f2.RecordId,
                                                  CONCAT(fa.AccountID) AS AccountID,
                                                  fa.AccountName,
                                                  fa.AccountNumber,
                                                  d.KeyIndex AS PayBank,
                                                  f2.Amount,
                                                  CONCAT(pe.PersonId) AS PersonId,
                                                  IFNULL(IFNULL(IFNULL(hp.Name,e4.`CustomerName`),e5.`SupplierName`),e6.`Name`) AS PersonName,
                                                  CONCAT(pe.CollectionId) AS CollectionId,
                                                  pe.AccountName AS CollectionName,
                                                  pe.BankDeposit AS CollectionBank,
                                                  pe.BankAccount,
                                                  pe.PayResult,
                                                  pe.Status,
                                                  pe.RecordId AS extendRecordID,
                                                  pe.PayUse,
                                                  pe.TransferType,
                                                  e7.`Name` AS RecheckName,
                                                  e77.`Photo` RecheckPhoto,
                                                  CONCAT(pe.`RecheckId`) RecheckId,
                                                  CONCAT(f.`SettleReceipType`) SettleReceipType,
                                                  CONCAT(f.NumericalOrder) NumericalOrder
                                                FROM
                                                  `nxin_qlw_business`.`fd_paymentreceivables` f 
                                                  INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` f2 
	                                            ON f.`NumericalOrder` = f2.`NumericalOrder` 
	                                            AND f.`IsGroupPay` = 1 
                                                  LEFT JOIN `nxin_qlw_business`.`fd_account` fa 
	                                            ON fa.`AccountID` = f2.`AccountID` 
                                                  LEFT JOIN qlw_nxin_com.BSDataDict d 
	                                            ON d.dictid = fa.BankID 
                                                  LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e
	                                            ON e.`EnterpriseID` = f.`EnterpriseID`
                                                  LEFT JOIN nxin_qlw_business.fd_paymentextend pe 
	                                            ON pe.NumericalOrder = f.numericalorder AND pe.`Guid` = f2.`Guid`
                                                  LEFT JOIN `nxin_qlw_business`.`hr_person` hp ON hp.`BO_ID` = pe.`PersonId`
	                                            LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = pe.`PersonId` -- 客户
	                                            LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = pe.`PersonId` -- 供应商
	                                            LEFT JOIN `nxin_qlw_business`.`hr_person` e6 ON e6.`PersonId` = pe.`PersonId` -- 部门
	                                            LEFT JOIN `nxin_qlw_business`.`hr_person` e7 ON e7.`BO_ID` = pe.`RecheckId`
	                                            LEFT JOIN `qlw_nxin_com`.`hr_person` e77 ON e77.personid = e7.`PersonID`
                                                LEFT JOIN nxin_qlw_business.biz_related r ON r.ParentValue = f.Numericalorder
                                        --        LEFT JOIN nxin_qlw_business.fd_scheduleplan plan ON plan.GroupId = {_identityservice.GroupId} AND plan.`PayNumericalOrder` = f.`Numericalorder` 
                                                LEFT JOIN qlw_nxin_com.fd_expense expen ON expen.NumericalOrder = r.ChildValue
                                                LEFT JOIN `nxin_qlw_business`.`biz_related_fm` rfm on rfm.ParentValue = pe.`NumericalOrder` and rfm.childvalue = pe.`RecordID` and rfm.`RelatedType` = 2205231634370000109 AND rfm.ParentType <> 2205231634370000109
                                                LEFT JOIN `nxin_qlw_business`.`biz_related_fm` rfmd ON rfmd.ParentValue = pe.`NumericalOrder` AND rfmd.childvalue = pe.`RecordID` AND rfmd.`RelatedType` = 2205231634370000109 AND rfmd.childValueDetail = 0 AND rfmd.ParentType <> 2205231634370000109
                                                LEFT JOIN `nxin_qlw_business`.`hr_person` hp2 on hp2.`BO_ID` = rfm.`ParentValueDetail`
                                                WHERE f.EnterpriseID IN ({entes}) AND f.`SettleReceipType` = 201611180104402204 AND pe.`IsRecheck` = TRUE  AND f.DataDate BETWEEN '{begindate}' AND '{enddate}' 
                                                GROUP BY f2.`RecordID`,rfm.RelatedID   ";

            var data = _context.RecheckPaymentListEntityDataSet.FromSqlRaw(sql).ToList();
            #region 查询未复核数据
            if (isExitRstatus > -1)
            {
                //移除越级对象（所有人复核过才能进入下一级显示出来）
                List<RecheckPaymentList> removes = new List<RecheckPaymentList>();
                foreach (var item in data.Where(m => m.rPersonId != "0").GroupBy(m => m.RecordId))
                {
                    foreach (var items in item.OrderBy(m => m.RawLevel).GroupBy(m => m.RawLevel).ToList())
                    {
                        var counts = item.Where(m => m.RawLevel == items.Key).ToList();
                        var isPass = item.Where(m => m.RStatus == 1 && m.RawLevel == items.Key).ToList();
                        var isRemove = item.Where(m => m.RStatus == 0 && m.RawLevel == items.Key).ToList();
                        if (counts.Count == isPass.Count || isRemove.Count == 0)
                        {
                            removes.AddRange(isPass);
                            continue;
                        }
                        else
                        {
                            if (isPass.Count > 0)
                            {
                                removes.AddRange(isPass);
                            }
                        }

                    }
                }
                //第一波剔除 已复核数据
                foreach (var item in removes)
                {
                    data.Remove(item);
                }
                removes.Clear();

                foreach (var item in data.Where(m => m.rPersonId != "0").GroupBy(m => m.RecordId))
                {
                    int i = 0;
                    foreach (var items in item.OrderBy(m => m.RawLevel).GroupBy(m => m.RawLevel).ToList())
                    {
                        var isRemove = item.Where(m => m.RStatus == 0 && m.RawLevel == items.Key).ToList();
                        if (isRemove.Count == 0)
                        {
                            continue;
                        }
                        else if (i == 1)
                        {
                            removes.AddRange(isRemove);
                        }
                        else
                        {
                            i = 1;
                            continue;
                        }
                    }
                }
                //第二波剔除 只保留最小级次未复核数据，其他数据清除  最重数据
                foreach (var item in removes)
                {
                    data.Remove(item);
                }
                removes.Clear();
                //找到所有 阶段流程 复核数据，找到不属于自己的数据 剔除
                var tempData = data.Where(m => m.rPersonId != "0" && m.rPersonId != userid).ToList();
                foreach (var item in tempData)
                {
                    data.Remove(item);
                }
            }
            #endregion
            return data.Where(m => m.rPersonId == userid).ToList();
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<FD_ReceivablesEntity> GetReceivablesDataAsync(long num, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT 
                                        ifnull(p3.Status,0) PayStatus,
                                        0 as PayCount,
                                          CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        ss.`SettleSummaryName` AS ReceiptAbstractName,
                                        Concat(p2.`ReceiptAbstractID`) as ReceiptAbstractID,
                                        e2.`EnterpriseName` AS CollectionName,
                                        Concat(e2.EnterpriseID) as CollectionId,
                                        hr.`Name` AS OwnerName,
                                        (p4.Debit) AS Amount,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        p.IsGroupPay,
                                        p3.Status,
                                        p3.IsRecheck,
                                        Concat(p2.BusinessType) as BusinessType,
                                        bd.DataDictName as BusinessTypeName,
                                        bs.Remarks as UploadInfo,
                                        IFNULL(CONCAT(br.ChildValue),GROUP_CONCAT(scbr.ChildValue)) AS ApplyNumericalOrder,
                                        IFNULL(CONCAT(br.ChildType),CONCAT(scbr.ChildType)) AS ApplyAppId,
                                        '' CheckedByID,
                                        '' AuditName,
                                        bank.Remarks BankUrl, false IsPayBack,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10 ,a.Auxiliary 
                                        FROM
                                          `nxin_qlw_business`.`fd_paymentreceivables` p 
                                          INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                          LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p3.`NumericalOrder` 
                                          LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = {num}
                                          LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND p4.LorR = 1 
                                          LEFT JOIN  qlw_nxin_com.biz_accosubject a ON p4.AccoSubjectID=a.AccoSubjectID
                                          LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                          LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                          LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                          LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                          LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd on bd.datadictid = p2.BusinessType 
                                          LEFT JOIN `nxin_qlw_business`.`biz_related` br ON br.ParentValue = p.`NumericalOrder` AND br.ParentType = 1612011058280000101 AND br.ChildType <> 201612070104402204
                                          LEFT JOIN `nxin_qlw_business`.`biz_related` scbr ON scbr.ParentValue = p.`NumericalOrder` AND scbr.ChildType = 201612070104402204 AND scbr.ParentValueDetail = p2.RecordID
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                        WHERE p.NumericalOrder = {num}  AND p.IsGroupPay = true 
                                        GROUP BY p.`NumericalOrder` 
                                        order by p.CreatedDate";

            return _context.FD_ReceivablesEntityDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 表头(移动支付专用
        /// </summary>
        /// <param name="num"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<FD_PaymentReceivablesMobileEntity> GetDataMobileAsync(long num, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT 
                                        IFNULL(p3.Status,0) PayStatus,
                                        IFNULL(info2.PayCount,0) AS PayCount,
                                        IFNULL(info3.IsRecheck,0) IsRecheck,
                                          CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        IFNULL(ss.`SettleSummaryName`,ssg.`SettleSummaryGroupName`) AS ReceiptAbstractName,
                                        CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
                                        e2.`EnterpriseName` AS CollectionName,
                                        CONCAT(e2.EnterpriseID) AS CollectionId,
                                        hr.`Name` AS OwnerName,
                                        (p4.Debit) AS Amount,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        item.`ProjectName`,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        bp.`ProductName`,
                                        p2.Content,
                                        p.IsGroupPay,
                                        p3.Status,
                                        CONCAT(p2.BusinessType) AS BusinessType,
                                        bd.DataDictName AS BusinessTypeName,
                                        bs.Remarks AS UploadInfo,
                                        IFNULL(CONCAT(br.ChildValue),GROUP_CONCAT(scbr.ChildValue)) AS ApplyNumericalOrder,
                                        IFNULL(CONCAT(br.ChildType),CONCAT(scbr.ChildType)) AS ApplyAppId,
                                        bank.Remarks BankUrl, FALSE IsPayBack,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10,a.Auxiliary 
                                        FROM
                                          `nxin_qlw_business`.`fd_paymentreceivables` p 
                                          INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                          LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = {num} 
                                          LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND p4.LorR = 0 
                                          LEFT JOIN  qlw_nxin_com.biz_accosubject a ON p4.AccoSubjectID=a.AccoSubjectID
                                          LEFT JOIN `qlw_nxin_com`.`biz_product` bp ON bp.`ProductID` = p4.`ProductID`
                                          LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = p.NumericalOrder
                                          LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                          LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                          LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                          LEFT JOIN `qlw_nxin_com`.`biz_settlesummaryGroup` ssg ON ssg.`SettleSummaryGroupID` = p2.`ReceiptAbstractID`
                                          LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                          LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd ON bd.datadictid = p2.BusinessType 
                                          LEFT JOIN `nxin_qlw_business`.`biz_related` br ON br.ParentValue = p.`NumericalOrder` AND br.ParentType = 1612011058280000101 AND br.ChildType <> 201612070104402204
                                          LEFT JOIN `nxin_qlw_business`.`biz_related` scbr ON scbr.ParentValue = p.`NumericalOrder` AND scbr.ChildType = 201612070104402204 AND scbr.ParentValueDetail = p2.RecordID
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank ON bank.SourceNum = p.`NumericalOrder` 
                                          LEFT JOIN `qlw_nxin_com`.`ppm_project` item ON item.`ProjectID` = p4.`ProjectID`
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) FileNum FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num}) AND `Status` IN (0,2) GROUP BY NumericalOrder) info ON info.NumericalOrder = p.numericalorder
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) PayCount FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num})  GROUP BY NumericalOrder) info2 ON info2.NumericalOrder = p.numericalorder
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) IsRecheck FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num}) AND IsRecheck = TRUE  GROUP BY NumericalOrder) info3 ON info3.NumericalOrder = p.numericalorder

                                        WHERE p.NumericalOrder = {num}  AND p.IsGroupPay = TRUE 
                                        GROUP BY p.`NumericalOrder` 
                                        ORDER BY p.CreatedDate";

            return _context.FD_PaymentReceivablesMobileEntityDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_PaymentReceivablesEntity> GetDataAsync(long num, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT 
                                        ifnull(p3.Status,0) PayStatus,
                                        IFNULL(info2.PayCount,0) as PayCount,
                                          CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        IFNULL(ss.`SettleSummaryName`,ssg.`SettleSummaryGroupName`) AS ReceiptAbstractName,
                                        Concat(p2.`ReceiptAbstractID`) as ReceiptAbstractID,
                                        e2.`EnterpriseName` AS CollectionName,
                                        Concat(e2.EnterpriseID) as CollectionId,
                                        hr.`Name` AS OwnerName,
                                        (p4.Debit) AS Amount,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        p.IsGroupPay,
                                        IFNULL(info.FileNum,0) Status,
                                        IFNULL(info3.IsRecheck,0) IsRecheck,
                                        Concat(p2.BusinessType) as BusinessType,
                                        bd.DataDictName as BusinessTypeName,
                                        bs.Remarks as UploadInfo,
                                        Concat(br.ChildValue) AS ApplyNumericalOrder,
                                        Concat(br.ChildType) AS ApplyAppId,
                                        '' CheckedByID,
                                        '' AuditName,
                                        bank.Remarks BankUrl, false IsPayBack,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10 
                                        FROM
                                          `nxin_qlw_business`.`fd_paymentreceivables` p 
                                          INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                          LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = {num} 
                                          LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND p4.LorR = 0 
                                          LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = {num} 
                                          LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                          LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                          LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                          LEFT JOIN `qlw_nxin_com`.`biz_settlesummaryGroup` ssg ON ssg.`SettleSummaryGroupID` = p2.`ReceiptAbstractID`
                                          LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                          LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd on bd.datadictid = p2.BusinessType 
                                          LEFT JOIN `nxin_qlw_business`.`biz_related` br ON br.ParentValue = p.`NumericalOrder` AND br.ParentType = 1612011058280000101
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) FileNum FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num}) AND `Status` IN (0,2) GROUP BY NumericalOrder) info ON info.NumericalOrder = p.numericalorder
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) PayCount FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num})  GROUP BY NumericalOrder) info2 ON info2.NumericalOrder = p.numericalorder
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) IsRecheck FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num}) AND IsRecheck = TRUE  GROUP BY NumericalOrder) info3 ON info3.NumericalOrder = p.numericalorder

                                        WHERE p.NumericalOrder = {num}  AND p.IsGroupPay = true 
                                        GROUP BY p.`NumericalOrder` 
                                        order by p.CreatedDate";
                                    
            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 收款单表头关系查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<Enterprise> GetApplyNumericalorders(long num, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT '' EnterpriseName,IFNULL(GROUP_CONCAT(childvalue),'') AS EnterpriseID FROM `nxin_qlw_business`.`biz_related`  WHERE parentvalue = {num} ";

            return _context.EnterpriseEntityDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 收款单表体关联销售单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<RelatedList> GetSalesRelatedList(long num, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONCAT(r.RelatedID) AS RelatedID,CONCAT(r.ParentValueDetail) AS RecordId,CONCAT(r.ChildValue) AS ApplyNumericalOrder,rd.Paid,rd.Payable,rd.Payment FROM nxin_qlw_business.`biz_related` r
                                    INNER JOIN nxin_qlw_business.`biz_relateddetail` rd ON rd.relatedID = r.relatedID
                                    WHERE r.ParentValue = {num}";

            return _context.RelatedListEntityDataSet.FromSqlInterpolated(sql).ToList();
        }
        /// <summary>
        /// 收款单表体关联销售单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<SaSalseUnionReceivables> GetSalseUnionReceivables(string nums = "0")
        {
            if (string.IsNullOrEmpty(nums))
            {
                return new List<SaSalseUnionReceivables>();
            }
            FormattableString sql = $@"SELECT CONCAT(rrd.`RecordID`) RecordID,CONCAT(sa.Number) Number,rrd.Payable,rrd.payment Amount,
                                    CONCAT(p2.`PaymentTypeID`) PaymentTypeID,dict.DataDictName PaymentTypeName,p3.`BankAccount` 
                                    FROM `nxin_qlw_business`.`fd_paymentreceivables` p
                                    INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p.numericalorder = p2.numericalorder
                                    INNER JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.numericalorder = p.numericalorder
                                    INNER JOIN `nxin_qlw_business`.`biz_related` rr ON rr.parentvalue = p.numericalorder AND rr.parenttype IN (1611231950150000101) AND childtype = 1610311318270000101
                                    INNER JOIN `nxin_qlw_business`.`biz_relateddetail` rrd ON rrd.RelatedID = rr.RelatedID
                                    INNER JOIN `nxin_qlw_business`.`sa_sales` sa ON sa.numericalorder = rr.childvalue
                                    LEFT JOIN `nxin_qlw_business`.`biz_datadict` dict ON dict.datadictid = p2.Paymenttypeid
                                    WHERE sa.numericalorder IN({nums})  -- 销售单流水号
                                    GROUP BY rrd.recordid";

            return _context.SaSalseUnionReceivablesDataSet.FromSqlInterpolated(sql).ToList();
        }
        /// <summary>
        /// 获取资金主页 收款数据
        /// </summary>
        /// <returns></returns>
        public List<FundReceivablesData> GetFundReceivablesData(string EnterpriseID,string BeginDate,string EndDate, NoneQuery query = null)
        {
            //根据时间条件和单位条件，按一级摘要汇总展示摘要名称和收款金额 默认取收款单
            string sql = $@"SELECT UUID() Guid,gsettle.`SettleSummaryGroupName`,CONCAT(gsettle.`SettleSummaryGroupId`) SettleSummaryGroupId,(SUM(payd.`Amount`)+ SUM(payd.`Charges`)) Amount FROM `nxin_qlw_business`.`fd_paymentreceivables` pay
            INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` payd ON pay.`NumericalOrder` = payd.`NumericalOrder`
            LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` settle ON settle.`SettleSummaryID` = payd.`ReceiptAbstractID` AND settle.`EnterpriseID` IN ({EnterpriseID})
            INNER JOIN `qlw_nxin_com`.`biz_settlesummarygroup` gsettle ON gsettle.`SettleSummaryGroupID` = settle.`SettleSummaryGroupID` AND gsettle.`EnterpriseID` IN ({_identityservice.GroupId})
            WHERE pay.`EnterpriseID` IN ({EnterpriseID}) AND pay.`DataDate` BETWEEN '{BeginDate}' AND '{EndDate}' AND pay.SettleReceipType IN (201610220104402201,201611180104402201,201611180104402203)
            GROUP BY settle.`SettleSummaryGroupID`";

            var data = _context.FundReceivablesDataDataSet.FromSqlRaw(sql).ToList();
            //不做收款的 取会计凭证
            if (data.Count == 0)
            {
                sql = $@"SELECT UUID() Guid,gsettle.`SettleSummaryGroupName`,CONCAT(gsettle.`SettleSummaryGroupId`) SettleSummaryGroupId,if(SUM(payd.debit) = 0,sum(payd.Credit),SUM(payd.debit)) Amount FROM `nxin_qlw_business`.`fd_settlereceipt` pay
                INNER JOIN `nxin_qlw_business`.`fd_settlereceiptdetail` payd ON pay.`NumericalOrder` = payd.`NumericalOrder`
                LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` settle ON settle.`SettleSummaryID`= payd.`ReceiptAbstractID` AND settle.`EnterpriseID` IN ({EnterpriseID})
                INNER JOIN `qlw_nxin_com`.`biz_settlesummarygroup` gsettle ON gsettle.`SettleSummaryGroupID` = settle.`SettleSummaryGroupID` AND gsettle.`EnterpriseID` IN ({_identityservice.GroupId})
                WHERE pay.`EnterpriseID` IN ({EnterpriseID}) AND pay.`DataDate` BETWEEN '{BeginDate}' AND '{EndDate}' AND pay.SettleReceipType IN (201610220104402201,201611180104402201,201611180104402203)
                GROUP BY settle.`SettleSummaryGroupID`";
                data = _context.FundReceivablesDataDataSet.FromSqlRaw(sql).ToList();
            }
            return data;
        }
        /// <summary>
        /// 资金主页 付款管理
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<FundReceivablesData> GetFundPayReceivablesData(string EnterpriseID, string BeginDate, string EndDate, NoneQuery query = null)
        {
            //根据时间条件和单位条件，按一级摘要汇总展示摘要名称和收款金额 默认取收款单
            string sql = $@"SELECT UUID() Guid,gsettle.`SettleSummaryGroupName`,CONCAT(gsettle.`SettleSummaryGroupId`) SettleSummaryGroupId,if(SUM(payd.debit) = 0,sum(payd.Credit),SUM(payd.debit)) Amount FROM `nxin_qlw_business`.`fd_settlereceipt` pay
                INNER JOIN `nxin_qlw_business`.`fd_settlereceiptdetail` payd ON pay.`NumericalOrder` = payd.`NumericalOrder`
                LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` settle ON settle.`SettleSummaryID`= payd.`ReceiptAbstractID` AND settle.`EnterpriseID` IN ({EnterpriseID})
                INNER JOIN `qlw_nxin_com`.`biz_settlesummarygroup` gsettle ON gsettle.`SettleSummaryGroupID` = settle.`SettleSummaryGroupID` AND gsettle.`EnterpriseID` IN ({_identityservice.GroupId})
                WHERE pay.`EnterpriseID` IN ({EnterpriseID}) AND pay.`DataDate` BETWEEN '{BeginDate}' AND '{EndDate}' AND pay.SettleReceipType IN (201610220104402202,201611180104402202,201611180104402204)
                GROUP BY settle.`SettleSummaryGroupID`";

            var data = _context.FundReceivablesDataDataSet.FromSqlRaw(sql).ToList();
            //不做收款的 取会计凭证
            if (data.Count == 0)
            {
                sql = $@"SELECT UUID() Guid,gsettle.`SettleSummaryGroupName`,CONCAT(gsettle.`SettleSummaryGroupId`) SettleSummaryGroupId,(SUM(payd.`Amount`)+ SUM(payd.`Charges`)) Amount FROM `nxin_qlw_business`.`fd_paymentreceivables` pay
            INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` payd ON pay.`NumericalOrder` = payd.`NumericalOrder`
            LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` settle ON settle.`SettleSummaryID` = payd.`ReceiptAbstractID` AND settle.`EnterpriseID` IN ({EnterpriseID})
            INNER JOIN `qlw_nxin_com`.`biz_settlesummarygroup` gsettle ON gsettle.`SettleSummaryGroupID` = settle.`SettleSummaryGroupID` AND gsettle.`EnterpriseID` IN ({_identityservice.GroupId})
            WHERE pay.`EnterpriseID` IN ({EnterpriseID}) AND pay.`DataDate` BETWEEN '{BeginDate}' AND '{EndDate}' AND pay.SettleReceipType IN (201610220104402202,201611180104402202,201611180104402204)
            GROUP BY settle.`SettleSummaryGroupID`";
                data = _context.FundReceivablesDataDataSet.FromSqlRaw(sql).ToList();
            }
            return data;
        }
        /// <summary>
        /// 资金主页 付款管理(付款方式)
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<FundReceivablesData> GetFundPayReceivablesPayTypeData(string EnterpriseID, string BeginDate, string EndDate, NoneQuery query = null)
        {
            //根据时间条件和单位条件，按一级摘要汇总展示摘要名称和收款金额 默认取收款单
            string sql = $@"SELECT UUID() Guid,bd.DataDictName AS SettleSummaryGroupName,concat(bd.DataDictId) SettleSummaryGroupId,(SUM(payd.`Amount`)+ SUM(payd.`Charges`)) Amount FROM `nxin_qlw_business`.`fd_paymentreceivables` pay
            INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` payd ON pay.`NumericalOrder` = payd.`NumericalOrder`
            LEFT JOIN `nxin_qlw_business`.`biz_datadict` bd ON bd.datadictid = payd.`PaymentTypeID`
            WHERE pay.`EnterpriseID` IN ({EnterpriseID}) AND pay.`DataDate` BETWEEN '{BeginDate}' AND '{EndDate}' AND pay.SettleReceipType IN (201610220104402202,201611180104402202,201611180104402204)
            GROUP BY payd.`PaymentTypeID`";
            var data = _context.FundReceivablesDataDataSet.FromSqlRaw(sql).ToList();
            return data;
        }
        /// <summary>
        /// 获取资金主页 收款数据(客户)
        /// </summary>
        /// <returns></returns>
        public List<FundReceivablesCusData> GetFundReceivablesCusData(string EnterpriseID, string BeginDate, string EndDate, NoneQuery query = null)
        {
            //根据时间条件和单位条件，按一级摘要汇总展示摘要名称和收款金额 默认取收款单
            string sql = $@"SELECT UUID() Guid,bcus.CustomerName,ent.EnterpriseName,SUM(payd.Amount) Amount FROM `nxin_qlw_business`.`fd_paymentreceivables` pay
                INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` payd on payd.numericalorder = pay.numericalorder
                INNER JOIN `qlw_nxin_com`.`biz_settlesummary` settle ON settle.`SettleSummaryCode` LIKE '0101%' AND payd.`ReceiptAbstractID` = settle.`SettleSummaryID`
                INNER JOIN `qlw_nxin_com`.`biz_customer` bcus ON bcus.customerId = payd.`CustomerID`
                INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON ent.enterpriseid = pay.`EnterpriseID`
                WHERE pay.`EnterpriseID` IN ({EnterpriseID}) AND pay.`DataDate` BETWEEN '{BeginDate}' AND '{EndDate}' AND pay.SettleReceipType IN (201610220104402201,201611180104402201,201611180104402203)
                GROUP BY payd.`CustomerID`";

            var data = _context.FundReceivablesCusDataDataSet.FromSqlRaw(sql).ToList();
            //不做收款的 取会计凭证
            if (data.Count == 0)
            {
                sql = $@"SELECT UUID() Guid,bcus.CustomerName,ent.EnterpriseName,if(SUM(payd.debit) = 0,sum(payd.Credit),SUM(payd.debit)) Amount FROM `nxin_qlw_business`.`fd_settlereceipt` pay
                INNER JOIN `nxin_qlw_business`.`fd_settlereceiptdetail` payd on payd.numericalorder = pay.numericalorder
                INNER JOIN `qlw_nxin_com`.`biz_settlesummary` settle ON settle.`SettleSummaryCode` LIKE '0101%' AND payd.`ReceiptAbstractID` = settle.`SettleSummaryID`
                INNER JOIN `qlw_nxin_com`.`biz_customer` bcus ON bcus.customerId = payd.`CustomerID`
                INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON ent.enterpriseid = pay.`EnterpriseID`
                WHERE pay.`EnterpriseID` IN ({EnterpriseID}) AND pay.`DataDate` BETWEEN '{BeginDate}' AND '{EndDate}' AND pay.SettleReceipType IN (201610220104402201,201611180104402201,201611180104402203)
                GROUP BY payd.`CustomerID`";
                data = _context.FundReceivablesCusDataDataSet.FromSqlRaw(sql).ToList();
            }
            return data;
        }
        /// <summary>
        /// 资金主页 付款统计(网银支付统计)
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<PayResultCount> GetPayResultCount(string EnterpriseID, string BeginDate, string EndDate, NoneQuery query = null)
        {
            //根据时间条件和单位条件，按一级摘要汇总展示摘要名称和收款金额 默认取收款单
            string sql = $@"SELECT UUID() Guid,
                        CASE WHEN paye.status = 1 
                        THEN '成功'
                        WHEN paye.status = 2 
                        THEN '失败'
                        WHEN paye.status = 3
                        THEN '交易中' END PayResult,COUNT(*) COUNT
                         FROM `nxin_qlw_business`.fd_paymentreceivables pay
                        INNER JOIN `nxin_qlw_business`.`fd_paymentextend` paye ON paye.NumericalOrder = pay.`NumericalOrder`
                        WHERE paye.Status IN (1,2,3) AND pay.`EnterpriseID` IN ({EnterpriseID}) AND pay.`DataDate` BETWEEN '{BeginDate}' AND '{EndDate}'
                        GROUP BY paye.Status";
            var data = _context.PayResultCountDataSet.FromSqlRaw(sql).ToList();
            return data;
        }
        /// <summary>
        /// 资金主页 资金计划 根据模板获取 日期
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TemplateDataDrop> GetTemplateDataDrop(string TemplateId)
        {
            //根据时间条件和单位条件，按一级摘要汇总展示摘要名称和收款金额 默认取收款单
            string sql = $@"SELECT PlanPeriod,CONCAT(fp.EnterpriseId) EnterpriseId,be.EnterpriseName,CONCAT(fp.NumericalOrder) NumericalOrder FROM nxin_qlw_business.`fm_fundplan` fp
            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` be ON be.enterpriseid = fp.enterpriseid WHERE templateid = {TemplateId}";
            var data = _context.TemplateDataDropDataSet.FromSqlRaw(sql).ToList();
            return data;
        }
        /// <summary>
        /// 资金主页 资金计划 根据模板获取未填写单位数据
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        public List<TemplateEnterData> GetTemplateEnterData(string EnterpriseID)
        {
            if (string.IsNullOrEmpty(EnterpriseID))
            {
                return new List<TemplateEnterData>();
            }
            string sql = $@"SELECT be.EnterpriseName FROM `qlw_nxin_com`.`biz_enterprise` be
            WHERE be.EnterpriseId IN ({EnterpriseID})";
            var data = _context.TemplateEnterDataSet.FromSqlRaw(sql).ToList();
            return data;
        }
        /// <summary>
        /// 资金主页 资金计划 根据模板获取 日期
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TemplateDataDrop> GetTemplateDataDropGroup(string TemplateId)
        {
            //根据时间条件和单位条件，按一级摘要汇总展示摘要名称和收款金额 默认取收款单
            string sql = $@"SELECT PlanPeriod,CONCAT(fp.EnterpriseId) EnterpriseId,be.EnterpriseName,CONCAT(fp.NumericalOrder) NumericalOrder FROM nxin_qlw_business.`fm_fundplan` fp
            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` be ON be.enterpriseid = fp.enterpriseid WHERE templateid = {TemplateId} GROUP by fp.PlanPeriod";
            var data = _context.TemplateDataDropDataSet.FromSqlRaw(sql).ToList();
            return data;
        }
        /// <summary>
        /// 资金主页 资金计划 获取模板
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TemplateData> GetTemplate(string TemplateId)
        {
            //根据时间条件和单位条件，按一级摘要汇总展示摘要名称和收款金额 默认取收款单
            string sql = $@"SELECT EnterpriseList FROM nxin_qlw_business.`fm_fundtemplate` WHERE numericalorder  = {TemplateId}";
            var data = _context.TemplateDataSet.FromSqlRaw(sql).ToList();
            return data;
        }
        /// <summary>
        /// 资金主页 余额管理-个数
        /// </summary>
        /// <returns></returns>
        public List<BankInfoCount> GetBankInfoCount(string enterpriseid)
        {
            string sql = $@"-- 账户类型下的账户个数
            SELECT CONCAT(AccountType) as BankID,b.`cDictName` as BankName,COUNT(*) AS BankCount,'' ImgUrl
            FROM nxin_qlw_business.`fd_account` a 
            INNER JOIN qlw_nxin_com.`bsdatadict` b ON a.`AccountType`=b.`DictID`
            WHERE a.`EnterpriseID`IN({enterpriseid}) and a.IsUse = 1
            GROUP BY a.`AccountType`
            union all
            -- 开户银行下的账户个数
            SELECT Concat(BankID) as BankID,b.`cDictName` as BankName,COUNT(*) AS BankCount,concat('https://apimg.alipay.com/combo.png?d=cashier&t=',b.KeyIndex) ImgUrl
            FROM nxin_qlw_business.`fd_account` a 
            INNER JOIN qlw_nxin_com.`bsdatadict` b ON a.BankID=b.`DictID`
            WHERE a.`EnterpriseID` IN({enterpriseid}) and a.IsUse = 1
            GROUP BY a.`BankID`

";
            var data = _context.BankInfoCountDataSet.FromSqlRaw(sql).ToList();
            return data;
        }
        /// <summary>
        /// 资金主页 资金归集 向上 向下（资金下拨）
        /// </summary>
        /// <returns></returns>
        public List<CashSweepInfo> GetCashSweepInfo(string enterpriseid,string beginDate,string endDate)
        {
            string sql = $@"SELECT UUID() Guid,
DATE_FORMAT( A.DataDate,'%Y-%m-%d') DataDate,
ent.EnterpriseName EnterpriseName,
ent2.EnterpriseName EnterpriseNameDetial,
d1.cDictName SweepTypeName,
IFNULL(FD.AutoSweepBalance,0.0) AutoSweepBalance,
		    CASE WHEN e.excuteStatusResults IS NULL  THEN '未归集'  
		         WHEN (FIND_IN_SET(1, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集成功' 
		         WHEN (FIND_IN_SET(0, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集失败'
		         WHEN (FIND_IN_SET(4, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集失败'
		         WHEN (FIND_IN_SET(2, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '处理中'
		         ELSE '部分归集成功' END TradeResult  ,
                 IFNULL(Concat('下次执行时间:',IFNULL(DATE_FORMAT( A.ExcuteDate,'%H:%i:%s'),A.ExcuteDate)),'') AS AutoTime,
                 CONVERT(A.SweepDirectionID USING utf8mb4) SweepDirectionID
    
FROM  nxin_qlw_business.FM_CashSweep A 
LEFT JOIN  nxin_qlw_business .HR_Person HP1 ON HP1.BO_ID=A.OwnerID
LEFT JOIN nxin_qlw_business.fd_account fa ON fa.AccountID=A.AccountID
LEFT JOIN  qlw_nxin_com.bsdatadict d ON d.DictID=A.SweepDirectionID AND d.pid=1811191754180000100
LEFT JOIN  qlw_nxin_com.bsdatadict d1 ON d1.DictID=A.SweepType AND d1.pid=1811191754180000200
LEFT JOIN  nxin_qlw_business .HR_Person HP ON HP.BO_ID=A.ExcuterID
LEFT JOIN  qlw_nxin_com . biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID
LEFT JOIN qlw_nxin_com.bsfile bs ON bs.NumericalOrder = A.NumericalOrder
INNER JOIN nxin_qlw_business.FM_CashSweepdetail FD ON a.NumericalOrder=fd.NumericalOrder
LEFT JOIN  qlw_nxin_com . biz_enterprise  ent2 ON FD.EnterpriseID=ent2.EnterpriseID
LEFT JOIN nxin_qlw_business.fd_account fa2 ON fa2.AccountID=FD.AccountID
    
LEFT JOIN (SELECT a.numericalorder,GROUP_CONCAT(DISTINCT faac.results) resultsRESu 
                        	FROM NXin_Qlw_Business.fm_cashsweep a 
                        	INNER JOIN qlw_nxin_com.faauditrecord faac  ON a.NumericalOrder = faac.NumericalOrder
                        	WHERE a.enterpriseid in ({enterpriseid}) and a.DataDate BETWEEN '{beginDate}' and '{endDate}'
                        	GROUP BY a.NumericalOrder
                        	) audit ON a.NumericalOrder = audit.NumericalOrder -- 审批结果
                    LEFT JOIN (SELECT a.numericalorder,GROUP_CONCAT(DISTINCT b.Status) excuteStatusResults 
                    		FROM NXin_Qlw_Business.fm_cashsweep a 
                    		INNER JOIN nxin_qlw_business.fm_cashsweepdetail b  ON a.NumericalOrder = b.NumericalOrder
                        	WHERE a.enterpriseid in ({enterpriseid})  and a.DataDate BETWEEN '{beginDate}' and '{endDate}'
                    		GROUP BY a.NumericalOrder
                    	  )e ON A.NumericalOrder = e.NumericalOrder -- 执行状态
                          WHERE a.enterpriseid in ({enterpriseid}) and a.DataDate BETWEEN '{beginDate}' and '{endDate}'

";
            var data = _context.CashSweepInfoDataSet.FromSqlRaw(sql).ToList();
            return data;
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<FD_PaymentReceivablesEntity> GetSummaryDataAsync(long num, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT 
                                        ifnull(p3.Status,0) PayStatus,
                                          CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        IFNULL(info2.PayCount,0) as PayCount,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        IFNULL(ss.`SettleSummaryName`,ssg.`SettleSummaryGroupName`) AS ReceiptAbstractName,
                                        Concat(p2.`ReceiptAbstractID`) as ReceiptAbstractID,
                                        e2.`EnterpriseName` AS CollectionName,
                                        Concat(e2.EnterpriseID) as CollectionId,
                                        hr.`Name` AS OwnerName,
                                        SUM(p4.Debit) AS Amount,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        p.IsGroupPay,
                                        Concat(p2.BusinessType) as BusinessType,
                                        bd.DataDictName as BusinessTypeName,
                                        bs.Remarks as UploadInfo,
                                        IFNULL(CONCAT(br.ChildValue),GROUP_CONCAT(scbr.ChildValue)) AS ApplyNumericalOrder,
                                        IFNULL(CONCAT(br.ChildType),CONCAT(scbr.ChildType)) AS ApplyAppId,
                                        p3.Status,
                                        p3.IsRecheck,
                                        '' CheckedByID,
                                        '' AuditName,
                                        bank.Remarks BankUrl, false IsPayBack,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10 
                                        FROM
                                          `nxin_qlw_business`.`fd_paymentreceivables` p 
                                          INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                          LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p2.`NumericalOrder`  AND p3.Guid = p2.Guid
                                          LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder`AND p4.guid = p2.Guid AND p4.LorR = 0
                                          LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder IN ({num})
                                          LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                          LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                          LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                          LEFT JOIN `qlw_nxin_com`.`biz_settlesummaryGroup` ssg ON ssg.`SettleSummaryGroupID` = p2.`ReceiptAbstractID`
                                          LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                          LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd on bd.datadictid = p2.BusinessType 
                                          LEFT JOIN `nxin_qlw_business`.`biz_related` br ON br.ParentValue = p.`NumericalOrder` AND br.ParentType = 1612011058280000101 AND br.ChildType <> 201612070104402204
                                          LEFT JOIN `nxin_qlw_business`.`biz_related` scbr ON scbr.ParentValue = p.`NumericalOrder` AND scbr.ChildType = 201612070104402204 AND scbr.ParentValueDetail = p2.RecordID
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) FileNum FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num}) AND `Status` IN (0,2) GROUP BY NumericalOrder) info ON info.NumericalOrder = p.numericalorder
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) PayCount FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num})  GROUP BY NumericalOrder) info2 ON info2.NumericalOrder = p.numericalorder
                                          LEFT JOIN (SELECT NumericalOrder,COUNT(1) IsRecheck FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({num}) AND IsRecheck = TRUE  GROUP BY NumericalOrder) info3 ON info3.NumericalOrder = p.numericalorder

                                        WHERE p.NumericalOrder = {num}  AND p.IsGroupPay = true 
                                        GROUP BY p.`NumericalOrder` 
                                        order by p.CreatedDate";

            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FD_PaymentReceivablesEntity> GetDatas(string entes)
        {
            string sql = $@" SELECT 
                                        ifnull(p3.Status,0) PayStatus,
                                        0 as IsRecheck,
                                        0 as Status,
                                        0 as PayCount,
                                        CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        ss.`SettleSummaryName` AS ReceiptAbstractName,
                                        Concat(p2.`ReceiptAbstractID`) as ReceiptAbstractId,
                                        IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`) AS CollectionName,
                                        CONCAT(p3.CollectionId) AS CollectionId,
                                        hr.`Name` AS OwnerName,
                                        p4.Debit as Amount,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        bs.Remarks as UploadInfo,
                                        Concat(p2.BusinessType) as BusinessType,
                                        bd.DataDictName as BusinessTypeName,
                                        p.IsGroupPay,
                                        '' AS ApplyNumericalOrder,
                                        '' CheckedByID,
                                        '' AuditName,
                                        '' AS ApplyAppId ,
                                        bank.Remarks BankUrl, false IsPayBack,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10 
                                        FROM
                                        `nxin_qlw_business`.`fd_paymentreceivables` p 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p2.`NumericalOrder` 
					                    LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder`AND p4.LorR = 0 
                                        LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = p.NumericalOrder
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd on bd.datadictid = p2.BusinessType 
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                        WHERE p.EnterpriseID in ({entes}) AND p.IsGroupPay = true and p.SettleReceipType = 201611180104402202 
                                        GROUP BY p.`NumericalOrder` 
                                        order by p.CreatedDate
";

            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlRaw(sql);
        }

        /// <summary>
        /// 收款退回列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<RefundList> GetRefundsList(string entes)
        {
            string sql = $@" SELECT DISTINCT CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,CONCAT(p3.NumericalOrder) NumericalOrder,CONCAT(p3.RecordID) RecordID,
            CONCAT(p.EnterpriseID) EnterpriseID,CONCAT(p.SettleReceipType) SettleReceipType,bd.DataDictName SettleReceipTypeName,CONCAT(p.Number) Number,IFNULL(e4.`CustomerName`,'') AS CollectionNames,
            CONCAT(p3.CollectionId) AS CollectionIds,IFNULL((brd.Paid),(p3.Amount)) Paid,IFNULL((((brd.Payable))),0.00) Payment
            FROM `nxin_qlw_business`.`fd_paymentreceivables` p
            INNER JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.numericalorder = p.numericalorder
            LEFT JOIN (
		        SELECT SUM(brd.`Payable`) Payable,(brd.`Paid`) Paid,SUM(brd.`Payment`) Payment,br.ChildValueDetail FROM `nxin_qlw_business`.`biz_related` br
		        INNER JOIN `nxin_qlw_business`.`biz_relateddetail` brd ON brd.RelatedID = br.RelatedID AND br.RelatedType = '201610210104402122' AND br.ParentType = '1612011058280000101' AND ChildType IN ('201611180104402201','201611180104402203')
		        GROUP BY br.`ChildValueDetail`
            ) brd ON brd.ChildValueDetail = p3.RecordID 
            LEFT JOIN `nxin_qlw_business`.`biz_datadict` bd ON bd.Datadictid = p.SettleReceipType
            INNER JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
            WHERE p.EnterpriseID IN({entes}) AND p.IsGroupPay = TRUE AND p.SettleReceipType IN (201611180104402201,201611180104402203)
            GROUP BY p3.RecordID";

            return _context.RefundListDataSet.FromSqlRaw(sql);
        }
        /// <summary>
        /// 付款汇总列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FD_PaymentReceivablesEntity> GetSummaryDatas(string entes)
        {
            string sql = $@" SELECT '' CheckedByID,'' AuditName,ttt.IsRecheck,ifnull(ttt.Status,0) PayStatus,0 as PayCount,ttt.NumericalOrder,ttt.Guid,ttt.SettleReceipType,ttt.DataDate,ttt.TicketedPointID,ttt.Number,ttt.Remarks,ttt.OwnerID,ttt.EnterpriseID,ttt.CreatedDate,ttt.ModifiedDate,ttt.AttachmentNum,ttt.EnterpriseName,ttt.TicketedPointName,ttt.ReceiptAbstractName,ttt.ReceiptAbstractID,ttt.OwnerName,ttt.DebitAccoSubjectID,ttt.ProjectID,ttt.ProductID,ttt.Content,ttt.UploadInfo,ttt.BusinessType,ttt.BusinessTypeName,ttt.IsGroupPay,ttt.ApplyNumericalOrder,ttt.ApplyAppId,GROUP_CONCAT(CollectionNames) CollectionName,GROUP_CONCAT(ttt.CollectionIds) AS CollectionId,SUM(ttt.Amounts) AS Amount, false IsPayBack 
,Auxiliary1,Auxiliary2,Auxiliary3,Auxiliary4,Auxiliary5,Auxiliary6,Auxiliary7,Auxiliary8,Auxiliary9, Auxiliary10 FROM (
SELECT 
                                        CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        ss.`SettleSummaryName` AS ReceiptAbstractName,
                                        CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
                                        (IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`)) AS CollectionNames,
                                        CONCAT(p3.CollectionId) AS CollectionIds,
                                        hr.`Name` AS OwnerName,
                                        (p2.Amount) AS Amounts,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        bs.Remarks AS UploadInfo,
                                        CONCAT(p2.BusinessType) AS BusinessType,
                                        bd.DataDictName AS BusinessTypeName,
                                        p3.IsRecheck,
                                        p.IsGroupPay,
                                        '' AS ApplyNumericalOrder,
                                        '' AS ApplyAppId ,
                                        bank.Remarks BankUrl,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10 
                                        FROM
                                        `nxin_qlw_business`.`fd_paymentreceivables` p 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p3.`NumericalOrder` AND p3.`Guid` = p2.`Guid`
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND lorr = 1
                                        LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = p.NumericalOrder
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd ON bd.datadictid = p2.BusinessType 
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                        WHERE p.EnterpriseID IN ({entes}) AND p.IsGroupPay = TRUE AND p.SettleReceipType = 201611180104402204
                                        GROUP BY p2.`RecordID` ORDER BY p.CreatedDate
)
ttt
GROUP BY ttt.numericalorder
";

            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlRaw(sql);
        }

        /// <summary>
        /// 付款单 二合一
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FD_PaymentReceivablesEntity> GetMergeDatas(string ents,string beginDate = "",string endDate = "")
        {
            string keysSql = $@" SELECT CONCAT(NumericalOrder) NumericalOrder,IsGroupPay FROM nxin_qlw_business.fd_paymentreceivables p 
            WHERE p.EnterpriseID IN ({ents}) AND p.SettleReceipType IN (201611180104402204,201611180104402202)            ";
            if (!string.IsNullOrEmpty(beginDate) && !string.IsNullOrEmpty(endDate))
            {
                keysSql += $@" and DataDate BETWEEN '{beginDate}' and '{endDate}'";
            }
            var keyList = _context.NumericalOrderDataSet.FromSqlRaw(keysSql).ToList().Where(m => m.IsGroupPay).ToList();
            if (keyList?.Count == 0)
            {
                keyList = new List<NumericalOrderData>();
                keyList.Add(new NumericalOrderData() { NumericalOrder = "0" });
            }
            var keys = string.Join(',', keyList.Select(m => m.NumericalOrder));
            string sql = $@" SELECT '' CheckedByID,'' AuditName,ttt.IsRecheck,ifnull(ttt.Status,0) PayStatus,IFNULL(info2.PayCount,0) PayCount,IFNULL(info.FileNum,0) AS Status,ttt.VoucherNumber,ttt.NumericalOrder,ttt.BankUrl AS BankUrl,ttt.Guid,ttt.SettleReceipType,ttt.DataDate,ttt.TicketedPointID,ttt.Number,ttt.Remarks,ttt.OwnerID,ttt.EnterpriseID,ttt.CreatedDate,ttt.ModifiedDate,ttt.AttachmentNum,ttt.EnterpriseName,ttt.TicketedPointName,ttt.ReceiptAbstractName,ttt.ReceiptAbstractID,ttt.OwnerName,ttt.DebitAccoSubjectID,ttt.ProjectID,ttt.ProductID,ttt.Content,ttt.UploadInfo,ttt.BusinessType,ttt.BusinessTypeName,ttt.IsGroupPay,ttt.ApplyNumericalOrder,ttt.ApplyAppId,GROUP_CONCAT(CollectionNames) CollectionName,GROUP_CONCAT(ttt.CollectionIds) AS CollectionId,SUM(ttt.Amounts) AS Amount, false IsPayBack 
,Auxiliary1,Auxiliary2,Auxiliary3,Auxiliary4,Auxiliary5,Auxiliary6,Auxiliary7,Auxiliary8,Auxiliary9, Auxiliary10 FROM (
SELECT 
                                        CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        ss.`SettleSummaryName` AS ReceiptAbstractName,
                                        CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
                                        (IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`)) AS CollectionNames,
                                        CONCAT(p3.CollectionId) AS CollectionIds,
                                        hr.`Name` AS OwnerName,
                                        IF(p.`SettleReceipType` = 201611180104402202 ,p3.Amount,p2.Amount) AS Amounts,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        bs.Remarks AS UploadInfo,
                                        CONCAT(p2.BusinessType) AS BusinessType,
                                        bd.DataDictName AS BusinessTypeName,
                                        p.IsGroupPay,
                                        '' AS ApplyNumericalOrder,
                                        '' AS ApplyAppId,
                                        p3.Status,
                                        p3.IsRecheck,
                                        bank.Remarks BankUrl,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10 
                                        FROM
                                        `nxin_qlw_business`.`fd_paymentreceivables` p 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p2.`NumericalOrder` AND p3.`Guid` = p2.`Guid`
                                        INNER JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND lorr = 1
                                        LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = p.NumericalOrder
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd ON bd.datadictid = p2.BusinessType 
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                        WHERE p.NumericalOrder IN ({keys})
                                        GROUP BY p2.`RecordID` ORDER BY p.CreatedDate
)
ttt
LEFT JOIN (SELECT NumericalOrder,COUNT(1) FileNum FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({keys}) AND `Status` <> 1 AND `Status` <> 3 AND  isrecheck <> 1 GROUP BY NumericalOrder) info ON info.NumericalOrder = ttt.numericalorder
LEFT JOIN (SELECT NumericalOrder,COUNT(1) PayCount FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({keys})  GROUP BY NumericalOrder) info2 ON info2.NumericalOrder = ttt.numericalorder
LEFT JOIN (SELECT NumericalOrder,COUNT(1) IsRecheck FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({keys}) AND IsRecheck = TRUE  GROUP BY NumericalOrder) info3 ON info3.NumericalOrder = ttt.numericalorder
GROUP BY ttt.numericalorder
";

            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlRaw(sql);
        }
        /// <summary>
        /// 付款单 二合一 （付款单列表特殊优化处理）
        /// 收款单 二合一 传参
        /// </summary>
        /// <param name="ents"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="IsCollection">特殊逻辑处理，带收款名称的筛选会慢</param>
        /// <returns></returns>
        public List<FD_PaymentReceivablesHeadEntity> GetMergeDatasFilterNums(string ents, string beginDate = "", string endDate = "", bool IsCollection = false, bool isPay = true)
        {
            string keysSql = $@" SELECT CONCAT(NumericalOrder) NumericalOrder,IsGroupPay FROM nxin_qlw_business.fd_paymentreceivables p 
            WHERE p.EnterpriseID IN ({ents}) AND p.SettleReceipType IN (201611180104402204,201611180104402202)            ";
            if (!isPay)
            {
                keysSql = $@" SELECT CONCAT(NumericalOrder) NumericalOrder,IsGroupPay FROM nxin_qlw_business.fd_paymentreceivables p 
            WHERE p.EnterpriseID IN ({ents}) AND p.SettleReceipType IN (201611180104402203,201611180104402201)            ";
            }
            if (!string.IsNullOrEmpty(beginDate) && !string.IsNullOrEmpty(endDate))
            {
                keysSql += $@" and DataDate BETWEEN '{beginDate}' and '{endDate}'";
            }
            _logger.LogInformation("keysSql:" + keysSql);
            var keyList = _context.NumericalOrderDataSet.FromSqlRaw(keysSql).ToList().Where(m => m.IsGroupPay).ToList();
            if (keyList?.Count == 0)
            {
                keyList = new List<NumericalOrderData>
                {
                    new NumericalOrderData() { NumericalOrder = "0" }
                };
            }
            var keys = string.Join(',', keyList.Select(m => m.NumericalOrder));
            #region 存在收款单位过滤
            if (IsCollection)
            {
                string sql = $@" SELECT CONCAT(p.EnterpriseID) EnterpriseID,
                            CONCAT(p.NumericalOrder) NumericalOrder,
                            CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                            CONCAT(p.TicketedPointID) TicketedPointID,
                            CONCAT(p.Number) Number,
                            CONCAT(pd.BusinessType) BusinessType,'' AS CollectionName,
                            p3.CollectionId,   
                            CONCAT(p.OwnerID) OwnerID,Sum(p3.Amount) Amount from nxin_qlw_business.fd_paymentreceivables p
            inner join nxin_qlw_business.fd_paymentreceivablesdetail pd on pd.numericalorder = p.numericalorder
            inner join nxin_qlw_business.fd_paymentextend p3 on p3.numericalorder = p.numericalorder

            where p.NumericalOrder in ({keys})
            GROUP BY p.NumericalOrder 
            ";
                var list = _context.FD_PaymentReceivablesHeadEntityDataSet.FromSqlRaw(sql).ToList();
                //收款单位赋值逻辑
                string enterInfoSql = $@" SELECT EnterpriseID AS ID,EnterpriseName as Name,EnterpriseId FROM qlw_nxin_com.biz_enterprise WHERE EnterpriseID IN ({ents})";
                string personInfoSql = $@" SELECT PersonID AS ID,Name,0 as EnterpriseId FROM `nxin_qlw_business`.`hr_person`   ";
                string customerInfoSql = $@" SELECT CustomerID AS ID,CustomerName as Name,EnterpriseId FROM `nxin_qlw_business`.`sa_customer` WHERE EnterpriseID IN ({ents}) GROUP BY CustomerID ";
                string supplierInfoSql = $@" SELECT SupplierID AS ID,SupplierName as Name,EnterpriseId FROM `nxin_qlw_business`.`pm_supplier` WHERE EnterpriseID IN ({ents}) GROUP BY SupplierID ";
                string marketInfoSql = $@" SELECT MarketID AS ID,MarketName as Name,EnterpriseId FROM `nxin_qlw_business`.`biz_market`  WHERE EnterpriseID IN ({ents}) GROUP BY MarketID ";
                var enterList = _context.KeyNameInfoDataSet.FromSqlRaw(enterInfoSql).ToDictionary(m => m.ID, m => m.Name);
                var personList = _context.KeyNameInfoDataSet.FromSqlRaw(personInfoSql).ToDictionary(m => m.ID, m => m.Name);
                var customerList = _context.KeyNameInfoDataSet.FromSqlRaw(customerInfoSql).ToDictionary(m => m.ID, m => m.Name);
                var supplierList = _context.KeyNameInfoDataSet.FromSqlRaw(supplierInfoSql).ToDictionary(m => m.ID, m => m.Name);
                var marketList = _context.KeyNameInfoDataSet.FromSqlRaw(marketInfoSql).ToDictionary(m => m.ID, m => m.Name);
                foreach (var item in list)
                {
                    if (item.BusinessType == "201611160104402105")
                    {
                        if (enterList.ContainsKey(item.CollectionId))
                        {
                            item.CollectionName = enterList[item.CollectionId];
                        }
                    }
                    else if (item.BusinessType == "201611160104402103")
                    {
                        if (personList.ContainsKey(item.CollectionId))
                        {
                            item.CollectionName = personList[item.CollectionId];
                        }
                    }
                    else if (item.BusinessType == "201611160104402101")
                    {
                        if (customerList.ContainsKey(item.CollectionId))
                        {
                            item.CollectionName = customerList[item.CollectionId];
                        }
                    }
                    else if (item.BusinessType == "201611160104402104")
                    {
                        if (supplierList.ContainsKey(item.CollectionId))
                        {
                            item.CollectionName = supplierList[item.CollectionId];
                        }
                    }
                    else if (item.BusinessType == "201611160104402102")
                    {
                        if (marketList.ContainsKey(item.CollectionId))
                        {
                            item.CollectionName = marketList[item.CollectionId];
                        }
                    }
                }
                return list;
            }
            else
            {
                string sql = $@" SELECT CONCAT(p.EnterpriseID) EnterpriseID,
                            CONCAT(p.NumericalOrder) NumericalOrder,
                            CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                            CONCAT(p.TicketedPointID) TicketedPointID,
                            CONCAT(p.Number) Number,
                            CONCAT(pd.BusinessType) BusinessType,'' AS CollectionName,
                            0 as CollectionId,   
                            CONCAT(p.OwnerID) OwnerID,Sum(pd.Amount) Amount from nxin_qlw_business.fd_paymentreceivables p
            inner join nxin_qlw_business.fd_paymentreceivablesdetail pd on pd.numericalorder = p.numericalorder
            where p.NumericalOrder in ({keys})
            GROUP BY p.NumericalOrder 
            ";
                var list = _context.FD_PaymentReceivablesHeadEntityDataSet.FromSqlRaw(sql).ToList();
                return list;
            }
            #endregion
        }
        /// <summary>
        /// 付款单 二合一 （付款单列表特殊优化处理）
        /// 主查筛选出的结果流水
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        /// 
        public IQueryable<FD_PaymentReceivablesEntity> GetMergeDatasList(string keys)
        {
            if (string.IsNullOrEmpty(keys))
            {
                keys = "0";
            }
            string sql = $@" SELECT '' CheckedByID,'' AuditName,ttt.IsRecheck,ifnull(ttt.Status,0) PayStatus,IFNULL(info2.PayCount,0) PayCount,IFNULL(info.FileNum,0) AS Status,ttt.NumericalOrder,ttt.BankUrl AS BankUrl,ttt.Guid,ttt.SettleReceipType,ttt.DataDate,ttt.TicketedPointID,ttt.Number,ttt.VoucherNumber,ttt.Remarks,ttt.OwnerID,ttt.EnterpriseID,ttt.CreatedDate,ttt.ModifiedDate,ttt.AttachmentNum,ttt.EnterpriseName,ttt.TicketedPointName,ttt.ReceiptAbstractName,ttt.ReceiptAbstractID,ttt.OwnerName,ttt.DebitAccoSubjectID,ttt.ProjectID,ttt.ProductID,ttt.Content,ttt.UploadInfo,ttt.BusinessType,ttt.BusinessTypeName,ttt.IsGroupPay,ttt.ApplyNumericalOrder,ttt.ApplyAppId,GROUP_CONCAT(CollectionNames) CollectionName,GROUP_CONCAT(ttt.CollectionIds) AS CollectionId,SUM(ttt.Amounts) AS Amount, false IsPayBack
                            ,Auxiliary1,Auxiliary2,Auxiliary3,Auxiliary4,Auxiliary5,Auxiliary6,Auxiliary7,Auxiliary8,Auxiliary9, Auxiliary10 FROM (
                                SELECT 
                                        CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`, 
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        ss.`SettleSummaryName` AS ReceiptAbstractName,
                                        CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
                                        (IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`)) AS CollectionNames,
                                        CONCAT(p3.CollectionId) AS CollectionIds,
                                        hr.`Name` AS OwnerName,
                                        IF(p.`SettleReceipType` = 201611180104402202 ,p3.Amount,p2.Amount) AS Amounts,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        bs.Remarks AS UploadInfo,
                                        CONCAT(p2.BusinessType) AS BusinessType,
                                        bd.DataDictName AS BusinessTypeName,
                                        p.IsGroupPay,
                                        '' AS ApplyNumericalOrder,
                                        '' AS ApplyAppId,
                                        p3.Status,
                                        p3.IsRecheck,
                                        bank.Remarks BankUrl,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10
                                        FROM
                                        `nxin_qlw_business`.`fd_paymentreceivables` p 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p2.`NumericalOrder` AND p3.`Guid` = p2.`Guid`
                                        INNER JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND lorr = 1
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd ON bd.datadictid = p2.BusinessType 
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                          LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = p.NumericalOrder
                                        WHERE p.NumericalOrder IN ({keys})
                                        GROUP BY p2.`RecordID` ORDER BY p.CreatedDate
)
ttt
LEFT JOIN (SELECT NumericalOrder,COUNT(1) FileNum FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({keys}) AND `Status` <> 1 AND `Status` <> 3 AND  isrecheck <> 1 GROUP BY NumericalOrder) info ON info.NumericalOrder = ttt.numericalorder
LEFT JOIN (SELECT NumericalOrder,COUNT(1) PayCount FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({keys})  GROUP BY NumericalOrder) info2 ON info2.NumericalOrder = ttt.numericalorder
LEFT JOIN (SELECT NumericalOrder,COUNT(1) IsRecheck FROM nxin_qlw_business.fd_paymentextend p WHERE p.NumericalOrder IN ({keys}) AND IsRecheck = TRUE  GROUP BY NumericalOrder) info3 ON info3.NumericalOrder = ttt.numericalorder
GROUP BY ttt.numericalorder
";

            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlRaw(sql);
        }
        /// <summary>
        /// 付款单 二合一(金蝶专用)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FD_PaymentReceivablesEntity> GetMergeDatasByKingDee(string entes,string beginDate,string endDate)
        {
            string sql = $@" SELECT Auxiliary1,Auxiliary2,Auxiliary3,Auxiliary4,Auxiliary5,Auxiliary6,Auxiliary7,Auxiliary8,Auxiliary9, Auxiliary10,'' CheckedByID,'' AuditName,ttt.IsRecheck,ifnull(ttt.Status,0) PayStatus,IFNULL(info2.PayCount,0) PayCount,IFNULL(info.FileNum,0) AS Status,ttt.VoucherNumber,ttt.NumericalOrder,ttt.BankUrl,ttt.Guid,ttt.SettleReceipType,ttt.DataDate,ttt.TicketedPointID,ttt.Number,ttt.Remarks,ttt.OwnerID,ttt.EnterpriseID,ttt.CreatedDate,ttt.ModifiedDate,ttt.AttachmentNum,ttt.EnterpriseName,ttt.TicketedPointName,ttt.ReceiptAbstractName,ttt.ReceiptAbstractID,ttt.OwnerName,ttt.DebitAccoSubjectID,ttt.ProjectID,ttt.ProductID,ttt.Content,ttt.UploadInfo,ttt.BusinessType,ttt.BusinessTypeName,ttt.IsGroupPay,ttt.ApplyNumericalOrder,ttt.ApplyAppId,GROUP_CONCAT(CollectionNames) CollectionName,GROUP_CONCAT(ttt.CollectionIds) AS CollectionId,SUM(ttt.Amounts) AS Amount,IF(IsPayBack = '0' ,FALSE,TRUE) IsPayBack  FROM (
SELECT 
                                        CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        ss.`SettleSummaryName` AS ReceiptAbstractName,
                                        CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
                                        (IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`)) AS CollectionNames,
                                        CONCAT(p3.CollectionId) AS CollectionIds,
                                        hr.`Name` AS OwnerName,
                                        IF(p.`SettleReceipType` = 201611180104402202 ,p5.Debit,p2.Amount) AS Amounts,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        bs.Remarks AS UploadInfo,
                                        CONCAT(p2.BusinessType) AS BusinessType,
                                        bd.DataDictName AS BusinessTypeName,
                                        p.IsGroupPay,
                                        '' AS ApplyNumericalOrder,
                                        '' AS ApplyAppId,
                                        p3.Status,
                                        p3.IsRecheck,
                                        bank.Remarks BankUrl,IFNULL(br.`Remarks`,FALSE) IsPayBack,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10
                                        FROM
                                        `nxin_qlw_business`.`fd_paymentreceivables` p 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p2.`NumericalOrder` AND p3.`Guid` = p2.`Guid`
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND lorr = 1
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p5 ON p5.NumericalOrder = p.`NumericalOrder` AND p5.lorr = 0
                                        LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = p.NumericalOrder
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
                                        LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                        LEFT JOIN `nxin_qlw_business`.biz_datadict bd ON bd.datadictid = p2.BusinessType 
                                        LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                        LEFT JOIN `nxin_qlw_business`.`biz_related` br ON br.`ChildType` = 201611180104402201 AND ParentType = 1612011058280000101 AND br.ParentValue = p.`NumericalOrder` 
                                        WHERE p.EnterpriseID IN ({entes}) AND p.DataDate BETWEEN '{beginDate}' AND '{endDate}' AND p.IsGroupPay = TRUE AND p.SettleReceipType IN (201611180104402204,201611180104402202)
                                        GROUP BY p2.`RecordID` ORDER BY p.CreatedDate
)
ttt
LEFT JOIN (SELECT NumericalOrder,COUNT(1) FileNum FROM nxin_qlw_business.fd_paymentextend WHERE `Status` <> 1 AND `Status` <> 3 AND  isrecheck <> 1 GROUP BY NumericalOrder) info ON info.NumericalOrder = ttt.numericalorder
LEFT JOIN (SELECT NumericalOrder,COUNT(1) PayCount FROM nxin_qlw_business.fd_paymentextend  GROUP BY NumericalOrder) info2 ON info2.NumericalOrder = ttt.numericalorder
GROUP BY ttt.numericalorder
";

            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlRaw(sql);
        }
        /// <summary>
        /// 收款单 二合一
        /// </summary>
        /// <param name="entes">权限单位</param>
        /// <returns></returns>
        public IQueryable<FD_PaymentReceivablesEntity> GetReceivablesMergeDatas(string keys)
        {
            if (string.IsNullOrEmpty(keys))
            {
                keys = "0";
            }
            string sql = $@" SELECT ttt.IsRecheck,ifnull(ttt.Status,0) PayStatus,IFNULL(info2.PayCount,0) PayCount,IFNULL(info.FileNum,0) AS Status,ttt.VoucherNumber,ttt.NumericalOrder,ttt.BankUrl,ttt.Guid,ttt.SettleReceipType,ttt.DataDate,ttt.TicketedPointID,ttt.Number,ttt.Remarks,ttt.OwnerID,ttt.EnterpriseID,ttt.CreatedDate,ttt.ModifiedDate,ttt.AttachmentNum,ttt.EnterpriseName,ttt.TicketedPointName,ttt.ReceiptAbstractName,ttt.ReceiptAbstractID,ttt.OwnerName,ttt.DebitAccoSubjectID,ttt.ProjectID,ttt.ProductID,ttt.Content,ttt.UploadInfo,ttt.BusinessType,ttt.BusinessTypeName,ttt.IsGroupPay,ttt.ApplyNumericalOrder,ttt.ApplyAppId,GROUP_CONCAT(CollectionNames) CollectionName,GROUP_CONCAT(ttt.CollectionIds) AS CollectionId,0.00 AS Amount, false IsPayBack,CONCAT(ttt.CheckedByID) CheckedByID,ttt.AuditName
,Auxiliary1,Auxiliary2,Auxiliary3,Auxiliary4,Auxiliary5,Auxiliary6,Auxiliary7,Auxiliary8,Auxiliary9, Auxiliary10 
FROM (
SELECT 
                                        CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        re.CheckedByID CheckedByID,
                                        Audit.Name AuditName,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        ss.`SettleSummaryName` AS ReceiptAbstractName,
                                        CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
                                        (IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`)) AS CollectionNames,
                                        CONCAT(p3.CollectionId) AS CollectionIds,
                                        hr.`Name` AS OwnerName,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        bs.Remarks AS UploadInfo,
                                        CONCAT(p2.BusinessType) AS BusinessType,
                                        bd.DataDictName AS BusinessTypeName,
                                        p.IsGroupPay,
                                        '' AS ApplyNumericalOrder,
                                        '' AS ApplyAppId,
                                        p3.IsRecheck,
                                        p3.Status,
                                        bank.Remarks BankUrl,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10 
                                        FROM
                                        `nxin_qlw_business`.`fd_paymentreceivables` p 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p2.`NumericalOrder` AND p3.`Guid` = p2.`Guid`
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND lorr = 1
                                        LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = p.NumericalOrder
                                        INNER JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` and e.pid = {_identityservice.GroupId} 
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd ON bd.datadictid = p2.BusinessType 
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                          LEFT JOIN nxin_qlw_business.biz_reviwe re on re.NumericalOrder =  p.numericalorder and re.ReviweType = 1611231950150000101 and CheckMark = 16
                                          LEFT JOIN nxin_qlw_business.hr_person Audit on Audit.BO_ID = re.CheckedByID
                                        WHERE p.NumericalOrder IN ({keys})
                                        GROUP BY p2.`RecordID` ORDER BY p.CreatedDate
)
ttt
LEFT JOIN (SELECT NumericalOrder,COUNT(1) FileNum FROM nxin_qlw_business.fd_paymentextend p WHERE  p.NumericalOrder IN ({keys}) AND `Status` <> 1 AND `Status` <> 3 AND  isrecheck <> 1 GROUP BY NumericalOrder) info ON info.NumericalOrder = ttt.numericalorder
LEFT JOIN (SELECT NumericalOrder,COUNT(1) PayCount FROM nxin_qlw_business.fd_paymentextend p WHERE  p.NumericalOrder IN ({keys}) GROUP BY NumericalOrder) info2 ON info2.NumericalOrder = ttt.numericalorder
GROUP BY ttt.numericalorder
";

            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlRaw(sql);
        }
        public List<ReceiptAmout> GetReceiptAmouts(string nums = "0")
        {
            if (string.IsNullOrEmpty(nums))
            {
                nums = "0";
            }
            return _context.ReceiptAmoutDataSet.FromSqlRaw($@"SELECT CONCAT(charg.NumericalOrder) NumericalOrder,SUM(charges) + SUM(Amount) Amount FROM nxin_qlw_business.`fd_paymentreceivables` charg 
            INNER JOIN nxin_qlw_business.fd_paymentreceivablesdetail chargde ON chargde.NumericalOrder = charg.NumericalOrder
            WHERE charg.NumericalOrder in ({nums})
            GROUP BY charg.NumericalOrder").ToList();
        }
        /// <summary>
        /// 收款单 二合一(金蝶专用）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<FD_PaymentReceivablesEntity> GetReceivablesMergeDatasByKingDee(string entes, string beginDate, string endDate)
        {
            string sql = $@" SELECT '' CheckedByID,'' AuditName,ttt.IsRecheck,ifnull(ttt.Status,0) PayStatus,IFNULL(info2.PayCount,0) PayCount,IFNULL(info.FileNum,0) AS Status,ttt.VoucherNumber,ttt.NumericalOrder,ttt.BankUrl,ttt.Guid,ttt.SettleReceipType,ttt.DataDate,ttt.TicketedPointID,ttt.Number,ttt.Remarks,ttt.OwnerID,ttt.EnterpriseID,ttt.CreatedDate,ttt.ModifiedDate,ttt.AttachmentNum,ttt.EnterpriseName,ttt.TicketedPointName,ttt.ReceiptAbstractName,ttt.ReceiptAbstractID,ttt.OwnerName,ttt.DebitAccoSubjectID,ttt.ProjectID,ttt.ProductID,ttt.Content,ttt.UploadInfo,ttt.BusinessType,ttt.BusinessTypeName,ttt.IsGroupPay,ttt.ApplyNumericalOrder,ttt.ApplyAppId,GROUP_CONCAT(CollectionNames) CollectionName,GROUP_CONCAT(ttt.CollectionIds) AS CollectionId,temp.Amount AS Amount, false IsPayBack 
,Auxiliary1,Auxiliary2,Auxiliary3,Auxiliary4,Auxiliary5,Auxiliary6,Auxiliary7,Auxiliary8,Auxiliary9, Auxiliary10 FROM (
SELECT 
                                        CONCAT(p.`NumericalOrder`) AS NumericalOrder,
                                        p.`Guid`,
                                        CONCAT(p.`SettleReceipType`) AS SettleReceipType,
                                        CONVERT(DATE_FORMAT( p.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONCAT(p.`TicketedPointID`) AS TicketedPointID,
                                        CONCAT(p.`Number`) AS Number,
                                        IFNULL(case p.SettleReceipType When (201611180104402201) then CONCAT('收-',vc.Number) When (201611180104402203) then CONCAT('收-',vc.Number) else CONCAT('付-',vc.Number) end,'') as VoucherNumber,
                                        p.`Remarks`,
                                        CONCAT(p.`OwnerID`) AS OwnerID,
                                        CONCAT(p.`EnterpriseID`) AS EnterpriseID,
                                        CONVERT(DATE_FORMAT( p.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate,
                                        CONVERT(DATE_FORMAT( p.ModifiedDate,'%Y-%m-%d') USING utf8mb4) ModifiedDate,
                                        p.`AttachmentNum`,
                                        e.`EnterpriseName`,
                                        tp.`TicketedPointName`,
                                        ss.`SettleSummaryName` AS ReceiptAbstractName,
                                        CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
                                        (IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`)) AS CollectionNames,
                                        CONCAT(p3.CollectionId) AS CollectionIds,
                                        hr.`Name` AS OwnerName,
                                        p5.Debit AS Amounts,
                                        CONCAT(p4.`AccoSubjectID`) AS DebitAccoSubjectID,
                                        CONCAT(p4.`ProjectID`) AS ProjectID,
                                        CONCAT(p4.`ProductID`) AS ProductID,
                                        p2.Content,
                                        bs.Remarks AS UploadInfo,
                                        CONCAT(p2.BusinessType) AS BusinessType,
                                        bd.DataDictName AS BusinessTypeName,
                                        p.IsGroupPay,
                                        '' AS ApplyNumericalOrder,
                                        '' AS ApplyAppId,
                                        p3.IsRecheck,
                                        p3.Status,
                                        bank.Remarks BankUrl,
                                        CONCAT(p4.Auxiliary1) AS Auxiliary1,
                                        CONCAT(p4.Auxiliary2) AS Auxiliary2,
                                        CONCAT(p4.Auxiliary3) AS Auxiliary3,
                                        CONCAT(p4.Auxiliary4) AS Auxiliary4,
                                        CONCAT(p4.Auxiliary5) AS Auxiliary5,
                                        CONCAT(p4.Auxiliary6) AS Auxiliary6,
                                        CONCAT(p4.Auxiliary7) AS Auxiliary7,
                                        CONCAT(p4.Auxiliary8) AS Auxiliary8,
                                        CONCAT(p4.Auxiliary9) AS Auxiliary9,
                                        CONCAT(p4.Auxiliary10) AS Auxiliary10 
                                        FROM
                                        `nxin_qlw_business`.`fd_paymentreceivables` p 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 ON p2.`NumericalOrder` = p.`NumericalOrder` 
                                        INNER JOIN `nxin_qlw_business`.`fd_paymentextend` p3 ON p3.`NumericalOrder` = p2.`NumericalOrder` AND p3.`Guid` = p2.`Guid`
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p4 ON p4.NumericalOrder = p.`NumericalOrder` AND lorr = 1
                                        LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesvoucherdetail` p5 ON p5.NumericalOrder = p.`NumericalOrder` AND p5.lorr = 0
                                        LEFT JOIN nxin_qlw_business.fd_settlereceipt vc on vc.NumericalOrder = p.NumericalOrder
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e ON p.`EnterpriseID` = e.`EnterpriseID` 
                                        LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tp ON tp.`TicketedPointID` = p.`TicketedPointID` 
                                        LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` ss ON ss.`SettleSummaryID` = p2.`ReceiptAbstractID`
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
                                        LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
                                        LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
                                        LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
                                          LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = p.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = p.`OwnerID`
                                          LEFT JOIN `nxin_qlw_business`.biz_datadict bd ON bd.datadictid = p2.BusinessType 
                                          LEFT JOIN nxin_qlw_business.fd_bankreceivable bank on bank.SourceNum = p.`NumericalOrder` 
                                        WHERE p.EnterpriseID IN ({entes}) AND p.DataDate BETWEEN '{beginDate}' AND '{endDate}' AND p.IsGroupPay = TRUE AND p.SettleReceipType IN (201610220104402201,201611180104402201,201611180104402203)
                                        GROUP BY p2.`RecordID` ORDER BY p.CreatedDate
)
ttt
LEFT JOIN (SELECT NumericalOrder,COUNT(1) FileNum FROM nxin_qlw_business.fd_paymentextend WHERE `Status` <> 1 AND `Status` <> 3 AND  isrecheck <> 1 GROUP BY NumericalOrder) info ON info.NumericalOrder = ttt.numericalorder
LEFT JOIN (SELECT NumericalOrder,COUNT(1) PayCount FROM nxin_qlw_business.fd_paymentextend  GROUP BY NumericalOrder) info2 ON info2.NumericalOrder = ttt.numericalorder
LEFT JOIN (
    select charg.NumericalOrder,SUM(charges) + SUM(Amount) Amount from nxin_qlw_business.`fd_paymentreceivables` charg 
    inner join nxin_qlw_business.fd_paymentreceivablesdetail chargde on chargde.NumericalOrder = charg.NumericalOrder
    where charg.EnterpriseID IN ({entes})
    GROUP BY charg.NumericalOrder
) temp on temp.NumericalOrder = ttt.numericalorder
GROUP BY ttt.numericalorder
";

            return _context.FD_PaymentReceivablesEntityDataSet.FromSqlRaw(sql);
        }
        /// <summary>
        /// 银行收款处理列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<BankReceivablesEntity> GetBankReceivables(string entes)
        {
            string sql = $@" SELECT 
  CONCAT(bank.`NumericalOrder`) as NumericalOrder,
  bank.`transIndex`,
  bank.`dataSource`,
  bank.`bankSerial`,
  bank.`amount`,
  CONVERT(DATE_FORMAT(bank.`receiveDay`,'%Y-%m-%d') USING utf8mb4) as receiveDay,
  bank.`entId`,
  en.EnterpriseName `entName`,
  bank.`acctIndex`,
  '' as AccountName,
  '' as AccountId,
  bank.`acctNo`,
  bank.`otherSideName`,
  bank.`otherSideAcctIndex`,
  bank.`otherSideAcct`,
  bank.`fee`,
  bank.`msgCode`,
  bank.`msg`,
  bank.`custList`,
'' custName,
  bank.`IsGenerate`,
  CONCAT(bank.`SourceNum`) SourceNum,
  bank.`Remarks`,
  bank.`CreateTime` 
FROM
  `nxin_qlw_business`.`fd_bankreceivable` bank
  LEFT JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.enterpriseid = bank.entid
WHERE bank.entId IN({entes}) 
";

            return _context.BankReceivablesEntityDbSet.FromSqlRaw(sql);
        }
        /// <summary>
        /// 银行收款处理列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<BankAccountInfo> GetBankAccountInfo(string entes)
        {
            string sql = $@" SELECT UUID() Guid,`AccountNumber`,`AccountName`,CONCAT(AccountId) AccountId,CONCAT(EnterpriseID) EnterpriseID FROM `nxin_qlw_business`.`fd_account`
WHERE EnterpriseID IN({entes}) 
";

            return _context.BankAccountInfoDbSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public override Task<List<FD_PaymentReceivablesDetailEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
  p2.`RecordID`,
  CONCAT(p2.`NumericalOrder`) AS NumericalOrder,
  p2.`Guid`,
  CONCAT(p2.`BusinessType`) AS BusinessType,
  CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
  CONCAT(p2.`PaymentTypeID`) AS PaymentTypeID,
  CONCAT(p2.`AccountID`) AS AccountID,
  acc.`AccountName`,
  CONCAT(p2.`CustomerID`) AS CustomerID,
  CONCAT(p2.`PersonID`) AS PersonID,
  hr.`Name` AS PersonName,
  CONCAT(p2.`MarketID`) AS MarketID,
  market.`MarketName`,
  CONCAT(p1.`EnterpriseID`) AS EnterpriseID,
  p2.`Amount`,
  p2.`Content`,
  p2.`AttachCount`,
  CONCAT(p4.`AccoSubjectID`) AS AccoSubjectID,
  p2.`Charges` ,
  CONCAT(p4.OrganizationSortID) AS OrganizationSortID,
  '' as ApplyNumericalOrder
FROM
  `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 
   INNER JOIN `nxin_qlw_business`.`fd_paymentreceivables` p1 ON p1.`NumericalOrder` = p2.`NumericalOrder`
   LEFT join nxin_qlw_business.fd_paymentreceivablesvoucherdetail p4 on p4.NumericalOrder = p2.NumericalOrder and p4.LorR = 1 and p4.guid = p2.guid
   LEFT JOIN `nxin_qlw_business`.`fd_account` acc ON acc.`AccountID` = p2.`AccountID`
   LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`PersonID` = p2.`PersonID`
   LEFT JOIN `nxin_qlw_business`.`biz_market` market ON market.`MarketID` = p2.`MarketID`
where p2.NumericalOrder = {manyQuery}
Group by p2.RecordID
";
            return _context.FD_PaymentReceivablesDetailEntityDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        /// <summary>
        /// 收款单表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public Task<List<FD_PaymentReceivablesDetailEntity>> GetReceivablesDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
  p2.`RecordID`,
  CONCAT(p2.`NumericalOrder`) AS NumericalOrder,
  p2.`Guid`,
  CONCAT(p2.`BusinessType`) AS BusinessType,
  CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
  CONCAT(p2.`PaymentTypeID`) AS PaymentTypeID,
  CONCAT(p2.`AccountID`) AS AccountID,
  acc.`AccountName`,
  CONCAT(p2.`CustomerID`) AS CustomerID,
  CONCAT(p2.`PersonID`) AS PersonID,
  hr.`Name` AS PersonName,
  CONCAT(p2.`MarketID`) AS MarketID,
  market.`MarketName`,
  CONCAT(p1.`EnterpriseID`) AS EnterpriseID,
  p2.`Amount`,
  p2.`Content`,
  p2.`AttachCount`,
  CONCAT(p4.`AccoSubjectID`) AS AccoSubjectID,
  p2.`Charges` ,
  CONCAT(p4.OrganizationSortID) AS OrganizationSortID,
  '' as ApplyNumericalOrder
FROM
  `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 
   INNER JOIN `nxin_qlw_business`.`fd_paymentreceivables` p1 ON p1.`NumericalOrder` = p2.`NumericalOrder`
   LEFT join nxin_qlw_business.fd_paymentreceivablesvoucherdetail p4 on p4.NumericalOrder = p2.NumericalOrder and p4.guid = p2.guid
   LEFT JOIN `nxin_qlw_business`.`fd_account` acc ON acc.`AccountID` = p2.`AccountID`
   LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`PersonID` = p2.`PersonID`
   LEFT JOIN `nxin_qlw_business`.`biz_market` market ON market.`MarketID` = p2.`MarketID`
where p2.NumericalOrder = {manyQuery}
Group by p2.RecordID
";
            return _context.FD_PaymentReceivablesDetailEntityDataSet.FromSqlInterpolated(sql).ToListAsync();
        }

        /// <summary>
        /// 收款单表体费用科目专用
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public Task<List<FD_ReceivablesDetailEntity>> GetReceivablesDetaiByCost(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
  p2.`RecordID`,
  CONCAT(p2.`NumericalOrder`) AS NumericalOrder,
  p2.`Guid`,
  CONCAT(p2.`BusinessType`) AS BusinessType,
  CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
  CONCAT(p2.`PaymentTypeID`) AS PaymentTypeID,
  CONCAT(p2.`AccountID`) AS AccountID,
  CONCAT(p2.`CustomerID`) AS CustomerID,
  CONCAT(p2.`PersonID`) AS PersonID,
  CONCAT(p2.`MarketID`) AS MarketID,
  CONCAT(p1.`EnterpriseID`) AS EnterpriseID,
  p2.`Amount`,
  p2.`Content`,
  p2.`AttachCount`,
  CONCAT(p4.`AccoSubjectID`) AS AccoSubjectID,
  p2.`Charges` ,
  CONCAT(p4.OrganizationSortID) AS OrganizationSortID
FROM
  `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 
   INNER JOIN `nxin_qlw_business`.`fd_paymentreceivables` p1 ON p1.`NumericalOrder` = p2.`NumericalOrder`
   LEFT join nxin_qlw_business.fd_paymentreceivablesvoucherdetail p4 on p4.NumericalOrder = p2.NumericalOrder AND p4.LorR = 0 AND p4.guid = p2.guid AND  p4.Debit = p2.Charges
where p2.NumericalOrder = {manyQuery}
Group by p2.RecordID
";
            return _context.FD_ReceivablesDetailEntityDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        /// <summary>
        /// 收款汇总单表体费用科目专用
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public Task<List<FD_ReceivablesDetailEntity>> GetReceivablesSummaryDetaiByCost(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
  p2.`RecordID`,
  CONCAT(p2.`NumericalOrder`) AS NumericalOrder,
  p2.`Guid`,
  CONCAT(p2.`BusinessType`) AS BusinessType,
  CONCAT(p2.`ReceiptAbstractID`) AS ReceiptAbstractID,
  CONCAT(p2.`PaymentTypeID`) AS PaymentTypeID,
  CONCAT(p2.`AccountID`) AS AccountID,
  CONCAT(p2.`CustomerID`) AS CustomerID,
  CONCAT(p2.`PersonID`) AS PersonID,
  CONCAT(p2.`MarketID`) AS MarketID,
  CONCAT(p1.`EnterpriseID`) AS EnterpriseID,
  p2.`Amount`,
  p2.`Content`,
  p2.`AttachCount`,
  CONCAT(p4.`AccoSubjectID`) AS AccoSubjectID,
  p2.`Charges` ,
  CONCAT(p4.OrganizationSortID) AS OrganizationSortID
FROM
  `nxin_qlw_business`.`fd_paymentreceivablesdetail` p2 
   INNER JOIN `nxin_qlw_business`.`fd_paymentreceivables` p1 ON p1.`NumericalOrder` = p2.`NumericalOrder`
   INNER JOIN nxin_qlw_business.fd_paymentreceivablesvoucherdetail p4 ON p4.NumericalOrder = p2.NumericalOrder AND p4.Debit = p2.Charges AND lorr = 0 AND p4.guid = p2.guid
where p2.NumericalOrder = {manyQuery}
Group by p2.RecordID
";
            return _context.FD_ReceivablesDetailEntityDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        /// <summary>
        /// 付款汇总单 第一个信息表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public Task<List<FD_PaymentReceivablesSummaryDetailEntity>> GetSummaryDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT
                                        p2.RecordID,
                                        CONCAT(p.NumericalOrder) AS NumericalOrder,
                                        CONCAT(p2.ReceiptAbstractID) AS ReceiptAbstractID,
                                        CONCAT(p4.AccoSubjectID) AS AccoSubjectID,
                                        p2.Content,
                                        CONCAT(p2.BusinessType) BusinessType,
                                        CONCAT(p3.CollectionId) AS CollectionId,
                                        IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`) AS CollectionName,
                                        CONCAT(p3.PersonId) PersonId,
                                        CONCAT(p3.`AccountName`) AccountName,
                                        CONCAT(p3.`BankDeposit`) BankDeposit,
                                        CONCAT(p3.`BankAccount`) BankAccount,
                                        p2.Amount,
                                        p2.Charges,
                                        CONCAT(p4.ProjectID) AS ProjectID,
                                        pro.`ProjectName`,
                                        CONCAT(p4.EnterpriseID) AS EnterpriseID,
                                        CONCAT(p4.ProductID) AS ProductID,
                                        bp.`ProductName`,
                                        IFNULL(settle.`SettleSummaryName`,settlegroup.`SettleSummaryGroupName`) ReceiptAbstractName,
                                        p4.Guid,
                                        CONCAT(p4.OrganizationSortID) AS OrganizationSortID,
                                        '' AS ApplyNumericalOrder
                                        FROM
                                        nxin_qlw_business.fd_paymentreceivables p
                                        INNER JOIN nxin_qlw_business.fd_paymentreceivablesdetail p2 ON p2.NumericalOrder = p.NumericalOrder
                                        INNER JOIN nxin_qlw_business.fd_paymentextend p3 ON p3.NumericalOrder = p3.NumericalOrder AND p3.Guid = p2.Guid
                                        LEFT JOIN nxin_qlw_business.fd_paymentreceivablesvoucherdetail p4 ON p4.NumericalOrder = p.NumericalOrder AND p4.guid = p2.Guid AND lorr = 0
                                        LEFT JOIN qlw_nxin_com.biz_enterprise e ON p.EnterpriseID = e.EnterpriseID
                                        LEFT JOIN nxin_qlw_business.biz_ticketedpoint tp ON tp.TicketedPointID = p.TicketedPointID
                                        LEFT JOIN qlw_nxin_com.biz_settlesummary ss ON ss.SettleSummaryID = p2.ReceiptAbstractID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise e2 ON e2.EnterpriseID = p3.CollectionId
                                        LEFT JOIN qlw_nxin_com.bsfile bs ON bs.NumericalOrder = p.NumericalOrder
                                        LEFT JOIN nxin_qlw_business.hr_person hr ON hr.BO_ID = p.OwnerID
					                    LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
					                    LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
					                    LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
					                    LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
					                    LEFT JOIN `qlw_nxin_com`.`biz_product` bp ON bp.`ProductID` = p4.`ProductID`
					                    LEFT JOIN `qlw_nxin_com`.`ppm_project` pro ON pro.`ProjectID` = p4.`ProjectID`
					                    LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` settle ON settle.`SettleSummaryID` = p2.ReceiptAbstractID
					                    LEFT JOIN `qlw_nxin_com`.`biz_settlesummarygroup` settleGroup ON settleGroup.`SettleSummaryGroupID` = p2.ReceiptAbstractID
                                        WHERE p.`NumericalOrder` = {manyQuery}
                                        GROUP BY p2.recordid
";
            return _context.FD_PaymentReceivablesSummaryDetailEntityDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        /// <summary>
        /// 收款汇总单 第一个信息表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public Task<List<FD_PaymentReceivablesSummaryDetailEntity>> GetReceivablesSummaryDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT
                                        p2.RecordID,
                                        CONCAT(p.NumericalOrder) AS NumericalOrder,
                                        CONCAT(p2.ReceiptAbstractID) AS ReceiptAbstractID,
                                        CONCAT(p4.AccoSubjectID) AS AccoSubjectID,
                                        p2.Content,
                                        CONCAT(p2.BusinessType) BusinessType,
                                        CONCAT(p3.CollectionId) AS CollectionId,
                                        IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`) AS CollectionName,
                                        CONCAT(p3.PersonId) PersonId,
                                        CONCAT(p3.`AccountName`) AccountName,
                                        CONCAT(p3.`BankDeposit`) BankDeposit,
                                        CONCAT(p3.`BankAccount`) BankAccount,
                                        p2.Amount,
                                        p2.Charges,
                                        CONCAT(p4.ProjectID) AS ProjectID,
                                        pro.`ProjectName`,
                                        CONCAT(p4.EnterpriseID) AS EnterpriseID,
                                        CONCAT(p4.ProductID) AS ProductID,
                                        bp.`ProductName`,
                                        IFNULL(settle.`SettleSummaryName`,settlegroup.`SettleSummaryGroupName`) ReceiptAbstractName,
                                        p4.Guid,
                                        CONCAT(p4.OrganizationSortID) AS OrganizationSortID,
                                        '' AS ApplyNumericalOrder
                                        FROM
                                        nxin_qlw_business.fd_paymentreceivables p
                                        INNER JOIN nxin_qlw_business.fd_paymentreceivablesdetail p2 ON p2.NumericalOrder = p.NumericalOrder
                                        INNER JOIN nxin_qlw_business.fd_paymentextend p3 ON p3.NumericalOrder = p3.NumericalOrder AND p3.Guid = p2.Guid
                                        LEFT JOIN nxin_qlw_business.fd_paymentreceivablesvoucherdetail p4 ON p4.NumericalOrder = p.NumericalOrder AND lorr = 1 AND p4.guid = p2.guid
                                        LEFT JOIN qlw_nxin_com.biz_enterprise e ON p.EnterpriseID = e.EnterpriseID
                                        LEFT JOIN nxin_qlw_business.biz_ticketedpoint tp ON tp.TicketedPointID = p.TicketedPointID
                                        LEFT JOIN qlw_nxin_com.biz_settlesummary ss ON ss.SettleSummaryID = p2.ReceiptAbstractID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise e2 ON e2.EnterpriseID = p3.CollectionId
                                        LEFT JOIN qlw_nxin_com.bsfile bs ON bs.NumericalOrder = p.NumericalOrder
                                        LEFT JOIN nxin_qlw_business.hr_person hr ON hr.BO_ID = p.OwnerID
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
					                    LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
					                    LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
					                    LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
					                    LEFT JOIN `qlw_nxin_com`.`biz_product` bp ON bp.`ProductID` = p4.`ProductID`
					                    LEFT JOIN `qlw_nxin_com`.`ppm_project` pro ON pro.`ProjectID` = p4.`ProjectID`
					                    LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` settle ON settle.`SettleSummaryID` = p2.ReceiptAbstractID
					                    LEFT JOIN `qlw_nxin_com`.`biz_settlesummarygroup` settleGroup ON settleGroup.`SettleSummaryGroupID` = p2.ReceiptAbstractID
                                        WHERE p.`NumericalOrder` = {manyQuery}
                                        GROUP BY p2.recordid
";
            return _context.FD_PaymentReceivablesSummaryDetailEntityDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        /// <summary>
        /// 获取复核阶段流程数据
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public Task<List<ReviewFlowPath>> GetReviewFlowPath(string num)
        {
            string sql = $@"
            -- MaxLevel = 2205231634370000109 && RawLevel = 0  = 提交人信息
            -- MaxLevel = 2205231634370000109 && RawLevel = 999  = 交易结果信息
            -- SuccessCount = 提交了多少/交易成功
            -- FailCount = 驳回了多少/交易失败
            -- ProcessingCount = 交易中/未复核（复核支付列表点复核）
            -- SuccessTime = SuccessCount 最新时间
            -- FailTime = FailCount 最新时间
            -- ProcessingTime = Processing 最新时间
            -- 获取提交复核人信息（一般都是全部提交没有单独提交的情况，除非是驳回后的 重新发起）,Processing  没有用到  只有最后一段sql使用
            SELECT UUID() rrid,hr2.Name OwnerName,rfm.`ParentType` MaxLevel,rfm.`ChildType` RawLevel,CONCAT(rfm.ParentValue) AS NumericalOrder,
            CONCAT(hr.Name) ReviweName,
            COUNT(rfm.`ChildType`) SuccessCount,
            0 FailCount,
            0 ProcessingCount,
            MAX(rfm.`Remarks`) AS SuccessTime,
            '' AS FailTime,
            '' AS ProcessingTime,
            spe.`Amount` SuccessAmount,
            0.00 FailAmount,
            0.00 ProcessingAmount
            FROM `nxin_qlw_business`.`biz_related_fm` rfm
            LEFT JOIN nxin_qlw_business.`hr_person` hr ON hr.bo_id = rfm.`ParentValueDetail`
            LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivables` pay ON pay.numericalorder = rfm.`ParentValue`
            LEFT JOIN nxin_qlw_business.`hr_person` hr2 ON hr2.bo_id = pay.`OwnerID`
            LEFT JOIN (
            SELECT pe.`NumericalOrder`,SUM(pe.`Amount`) Amount FROM nxin_qlw_business.`fd_paymentextend` pe WHERE pe.`NumericalOrder` = {num}
            ) spe ON spe.NumericalOrder = rfm.`ParentValue`
            WHERE rfm.`RelatedType` = 2205231634370000109 AND rfm.`ParentType` = 2205231634370000109 AND rfm.`ParentValue` = {num}
            GROUP BY rfm.`ChildType`  
            UNION
            -- 获取复核节点数据 以级次为主汇总 查出 复核了几笔，退回了几笔，Processing  没有用到  只有最后一段sql使用
            SELECT UUID() rrid,hr2.Name OwnerName,rfm.`ParentType` MaxLevel,rfm.`ChildType` RawLevel,CONCAT(rfm.ParentValue) AS NumericalOrder,
            CONCAT(hr.Name) ReviweName,
            COUNT(rfmd.`ChildType`) SuccessCount,
            COUNT(rfmd2.`ChildType`) FailCount,
            COUNT(rfmd3.`ChildType`) ProcessingCount,
            IFNULL(MAX(rfm.`Remarks`),'') AS SuccessTime,
            IFNULL(MAX(rfmd2.`Remarks`),'') AS FailTime,
            IFNULL(MAX(rfmd3.`Remarks`),'') AS ProcessingTime,
            SUM(pe.`Amount`) SuccessAmount,
            SUM(pe2.`Amount`) FailAmount,
            SUM(pe3.`Amount`) ProcessingAmount
            FROM `nxin_qlw_business`.`biz_related_fm` rfm
            LEFT JOIN nxin_qlw_business.`hr_person` hr ON hr.bo_id = rfm.`ParentValueDetail`
            LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivables` pay ON pay.numericalorder = rfm.`ParentValue`
            LEFT JOIN nxin_qlw_business.`hr_person` hr2 ON hr2.bo_id = pay.`OwnerID`
            LEFT JOIN `nxin_qlw_business`.`biz_related_fm` rfmd ON rfmd.`ChildValue` = rfm.`ChildValue` AND rfmd.`ChildType` = rfm.`ChildType` AND rfmd.`RelatedType` = 2205231634370000109 AND rfmd.`ParentType` <> 2205231634370000109 AND rfmd.ChildValueDetail = 1
            LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` pe ON pe.RecordId = rfmd.`ChildValue`
            LEFT JOIN `nxin_qlw_business`.`biz_related_fm` rfmd2 ON rfmd2.`ChildValue` = rfm.`ChildValue` AND rfmd2.`ChildType` = rfm.`ChildType` AND rfmd2.`RelatedType` = 2205231634370000109 AND rfmd2.`ParentType` <> 2205231634370000109 AND rfmd2.ChildValueDetail = 2
            LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` pe2 ON pe2.RecordId = rfmd2.`ChildValue`
            LEFT JOIN `nxin_qlw_business`.`biz_related_fm` rfmd3 ON rfmd3.`ChildValue` = rfm.`ChildValue` AND rfmd3.`ChildType` = rfm.`ChildType` AND rfmd3.`RelatedType` = 2205231634370000109 AND rfmd3.`ParentType` <> 2205231634370000109 AND rfmd3.ChildValueDetail = 0
            LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` pe3 ON pe3.RecordId = rfmd3.`ChildValue`
            WHERE rfm.`RelatedType` = 2205231634370000109 AND rfm.`ParentType` <> 2205231634370000109 AND rfm.`ParentValue` = {num}
            GROUP BY rfm.`ChildType`
            UNION
            -- 获取交易状态 以整单的流水为主汇总 查出 交易成功，交易失败，交易中数据
            SELECT UUID() rrid,hr2.Name OwnerName,rfm.`ParentType` MaxLevel,999 RawLevel,CONCAT(rfm.ParentValue) AS NumericalOrder,
            CONCAT(hr.Name) ReviweName,
            COUNT(pe.RecordId) SuccessCount,
            COUNT(pe2.RecordId) FailCount,
            COUNT(pe3.RecordId) ProcessingCount,
            IFNULL(CONVERT(DATE_FORMAT(MAX(pe.ModifiedDate),'%Y-%m-%d %H:%i:%S') USING utf8mb4),'') SuccessTime,
            IFNULL(CONVERT(DATE_FORMAT(MAX(pe2.ModifiedDate),'%Y-%m-%d %H:%i:%S') USING utf8mb4),'') FailTime,
            IFNULL(CONVERT(DATE_FORMAT(MAX(pe3.ModifiedDate),'%Y-%m-%d %H:%i:%S') USING utf8mb4),'') ProcessingTime,
            SUM(pe.`Amount`) SuccessAmount,
            SUM(pe2.`Amount`) FailAmount,
            SUM(pe3.`Amount`) ProcessingAmount
            FROM `nxin_qlw_business`.`biz_related_fm` rfm
            LEFT JOIN nxin_qlw_business.`hr_person` hr ON hr.bo_id = rfm.`ParentValueDetail` 
            LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivables` pay ON pay.numericalorder = rfm.`ParentValue`
            LEFT JOIN nxin_qlw_business.`hr_person` hr2 ON hr2.bo_id = pay.`OwnerID`
            LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` pe ON pe.RecordId = rfm.`ChildValue` AND pe.Status = 1
            LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` pe2 ON pe2.RecordId = rfm.`ChildValue` AND pe2.Status = 2
            LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` pe3 ON pe3.RecordId = rfm.`ChildValue` AND pe3.Status = 3
            WHERE rfm.`RelatedType` = 2205231634370000109 AND rfm.`ParentType` = 2205231634370000109 AND rfm.`ParentValue` = {num}
            GROUP BY rfm.`ChildType`
";
            return _context.ReviewFlowPathDataSet.FromSqlRaw(sql).ToListAsync();
        }
        /// <summary>
        /// 付款汇总单 第一个信息表体
        /// </summary>
        /// <returns></returns>
        public Task<List<ApprovalSetDetail>> GetApprovalSetDetail(long ApprovalTypeID, long EnterpriseID)
        {
            FormattableString sql = $@"
                                    SELECT CONCAT(f2.NumericalOrderDetail) NumericalOrderDetail,CONCAT(f2.NumericalOrder) NumericalOrder,f2.ApprovalCondition FROM  `qlw_nxin_com`.`fa_approvalset` f
                                    INNER JOIN `qlw_nxin_com`.`fa_approvalsetdetail` f2 ON f.Numericalorder = f2.NumericalOrder
                                    WHERE f.ApprovalTypeID = {ApprovalTypeID} AND f.EnterpriseID = {EnterpriseID}
";
            return _context.ApprovalSetDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        /// <summary>
        /// 付款汇总单 付款信息表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public List<FD_PaymentExtendSummaryEntity> GetPaymentExtendSummaryDatas(long manyQuery)
        {
            FormattableString sql = $@"SELECT p3.RecordID,
                                    CONCAT(p3.`NumericalOrder`) NumericalOrder,
                                    UUID() Guid,
                                    p3.CollectionId,
                                    CONCAT(p2.`PaymentTypeID`) AS PaymentTypeID,
                                    CONCAT(p2.`AccountID`) AS AccountID,
                                    acc.`AccountName`,
                                    GROUP_CONCAT(p3.`PersonId`) PersonId,
                                    CONCAT(p4.`AccoSubjectID`) AS AccoSubjectID,
                                    SUM(p3.`Amount`) AS Amount,
                                    p3.Status,p3.IsRecheck,CONCAT(TradeNo) TradeNo
                                    FROM
                                    nxin_qlw_business.fd_paymentreceivables p
                                    INNER JOIN nxin_qlw_business.fd_paymentreceivablesdetail p2 ON p2.NumericalOrder = p.NumericalOrder
                                    INNER JOIN nxin_qlw_business.fd_paymentextend p3 ON p3.NumericalOrder = p3.NumericalOrder AND p3.Guid = p2.Guid
                                    LEFT JOIN nxin_qlw_business.fd_paymentreceivablesvoucherdetail p4 ON p4.NumericalOrder = p.NumericalOrder AND  p4.AccountID = p2.AccountID
                                    LEFT JOIN qlw_nxin_com.biz_enterprise e ON p.EnterpriseID = e.EnterpriseID
                                    LEFT JOIN nxin_qlw_business.biz_ticketedpoint tp ON tp.TicketedPointID = p.TicketedPointID
                                    LEFT JOIN qlw_nxin_com.biz_settlesummary ss ON ss.SettleSummaryID = p2.ReceiptAbstractID
                                    LEFT JOIN qlw_nxin_com.biz_enterprise e2 ON e2.EnterpriseID = p3.CollectionId
                                    LEFT JOIN qlw_nxin_com.bsfile bs ON bs.NumericalOrder = p.NumericalOrder
                                    LEFT JOIN nxin_qlw_business.hr_person hr ON hr.BO_ID = p.OwnerID
                                    LEFT JOIN `nxin_qlw_business`.`fd_account` acc ON acc.`AccountID` = p2.`AccountID`
                                    WHERE p.`NumericalOrder` = {manyQuery}
                                    GROUP BY p2.`AccountID`
";
            var list = _context.FD_PaymentExtendSummaryEntityDataSet.FromSqlInterpolated(sql).ToList();
            string AmountSql = $@"SELECT p3.RecordID,
                    CONCAT(p3.`NumericalOrder`) NumericalOrder,
                    p3.`Guid`,
                    GROUP_CONCAT(p3.CollectionId) CollectionId,
                    CONCAT(p2.`PaymentTypeID`) AS PaymentTypeID,
                    CONCAT(p2.`AccountID`) AS AccountID,
                    GROUP_CONCAT(p3.`PersonId`) PersonId,
                    '' AS AccoSubjectID,
                    SUM(p3.`Amount`) AS Amount,
                    SUM(p2.Charges) AS Charges,
                    p3.Status
                    FROM
                    nxin_qlw_business.fd_paymentreceivables p
                    INNER JOIN nxin_qlw_business.fd_paymentreceivablesdetail p2 ON p2.NumericalOrder = p.NumericalOrder
                    INNER JOIN nxin_qlw_business.fd_paymentextend p3 ON p3.NumericalOrder = p3.NumericalOrder AND p3.Guid = p2.Guid
                    WHERE p.`NumericalOrder` = {manyQuery}
                    GROUP BY p2.`AccountID`";
            //只用于获取金额，防止上方金额翻倍
            var amountList = _context.FD_PaymentExtendSummaryEntityByAmountDataSet.FromSqlRaw(AmountSql).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Amount = amountList[i].Amount;
                list[i].Charges = amountList[i].Charges;
                list[i].CollectionId = amountList[i].CollectionId;
            }
            //去重
            foreach (var item in list)
            {
                var sp = item.PersonID.Split(',').ToList();
                sp = sp.Distinct().ToList();
                item.PersonID = string.Join(",", sp);
            }
            return list;
        }
        /// <summary>
        /// 收款汇总单 收款信息表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public List<FD_PaymentExtendSummaryEntity> GetReceivablesExtendSummaryDatas(long manyQuery)
        {
            FormattableString sql = $@"SELECT p3.RecordID,
                                    CONCAT(p3.`NumericalOrder`) NumericalOrder,
                                    UUID() Guid,
                                    p3.CollectionId,
                                    CONCAT(p2.`PaymentTypeID`) AS PaymentTypeID,
                                    CONCAT(p2.`AccountID`) AS AccountID,
                                    acc.`AccountName`,
                                    GROUP_CONCAT(p3.`PersonId`) PersonId,
                                    CONCAT(p4.`AccoSubjectID`) AS AccoSubjectID,
                                    SUM(p3.`Amount`) AS Amount,
                                    p3.Status,p3.IsRecheck,CONCAT(TradeNo) TradeNo
                                    FROM
                                    nxin_qlw_business.fd_paymentreceivables p
                                    INNER JOIN nxin_qlw_business.fd_paymentreceivablesdetail p2 ON p2.NumericalOrder = p.NumericalOrder
                                    INNER JOIN nxin_qlw_business.fd_paymentextend p3 ON p3.NumericalOrder = p3.NumericalOrder AND p3.Guid = p2.Guid
                                    LEFT JOIN nxin_qlw_business.fd_paymentreceivablesvoucherdetail p4 ON p4.NumericalOrder = p.NumericalOrder AND lorr = 1 AND p4.Debit = p2.Amount
                                    LEFT JOIN qlw_nxin_com.biz_enterprise e ON p.EnterpriseID = e.EnterpriseID
                                    LEFT JOIN nxin_qlw_business.biz_ticketedpoint tp ON tp.TicketedPointID = p.TicketedPointID
                                    LEFT JOIN qlw_nxin_com.biz_settlesummary ss ON ss.SettleSummaryID = p2.ReceiptAbstractID
                                    LEFT JOIN qlw_nxin_com.biz_enterprise e2 ON e2.EnterpriseID = p3.CollectionId
                                    LEFT JOIN qlw_nxin_com.bsfile bs ON bs.NumericalOrder = p.NumericalOrder
                                    LEFT JOIN nxin_qlw_business.hr_person hr ON hr.BO_ID = p.OwnerID
                                    LEFT JOIN `nxin_qlw_business`.`fd_account` acc ON acc.`AccountID` = p2.`AccountID`
                                    WHERE p.`NumericalOrder` = {manyQuery}
                                    GROUP BY p2.`AccountID`
";
            var list = _context.FD_PaymentExtendSummaryEntityDataSet.FromSqlInterpolated(sql).ToList();
            string AmountSql = $@"SELECT p3.RecordID,
                    CONCAT(p3.`NumericalOrder`) NumericalOrder,
                    p3.`Guid`,
                    GROUP_CONCAT(p3.CollectionId) CollectionId,
                    CONCAT(p2.`PaymentTypeID`) AS PaymentTypeID,
                    CONCAT(p2.`AccountID`) AS AccountID,
                    GROUP_CONCAT(p3.`PersonId`) PersonId,
                    '' AS AccoSubjectID,
                    SUM(p3.`Amount`) AS Amount,
                    SUM(p2.Charges) AS Charges,
                    p3.Status
                    FROM
                    nxin_qlw_business.fd_paymentreceivables p
                    INNER JOIN nxin_qlw_business.fd_paymentreceivablesdetail p2 ON p2.NumericalOrder = p.NumericalOrder
                    INNER JOIN nxin_qlw_business.fd_paymentextend p3 ON p3.NumericalOrder = p3.NumericalOrder AND p3.Guid = p2.Guid
                    WHERE p.`NumericalOrder` = {manyQuery}
                    GROUP BY p2.`AccountID`";
            //只用于获取金额，防止上方金额翻倍
            var amountList = _context.FD_PaymentExtendSummaryEntityByAmountDataSet.FromSqlRaw(AmountSql).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Amount = amountList[i].Amount;
                list[i].Charges = amountList[i].Charges;
                list[i].CollectionId = amountList[i].CollectionId;
            }
            //去重
            foreach (var item in list)
            {
                var sp = item.PersonID.Split(',').ToList();
                sp = sp.Distinct().ToList();
                item.PersonID = string.Join(",", sp);
            }
            return list;
        }

        public override IQueryable<FD_PaymentReceivablesEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }
    }
}
