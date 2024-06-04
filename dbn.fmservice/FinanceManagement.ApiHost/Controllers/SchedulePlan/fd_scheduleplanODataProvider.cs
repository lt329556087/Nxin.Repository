using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    public class fd_scheduleplanODataProvider : OneWithManyQueryEntity<fd_scheduleplan>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public fd_scheduleplanODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<fd_scheduleplanEntity> GetList()
        {
            var datas = GetData().AsEnumerable();
            return datas;
        }

        public IQueryable<fd_scheduleplanEntity> GetData()
        {
            return GetDatas().AsQueryable();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<fd_scheduleplanEntity> GetDatas(NoneQuery query = null)
        {
            return _context.fd_scheduleplanDataSet.FromSqlRaw($@"SELECT 
            CASE sp.ApplyMenuId
            when '201702230104402501' then concat('https://qlw.nxin.com/FD_AccountTransfer/AccountTransfer?appid=201512289611681881&numericalorder=',sp.ApplyNumericalOrder)
            when '201702230104402502' then concat('https://qlw.nxin.com/FDAccountFundsAllocation/AccountFundsAllocation?appid=201512291502313749&numericalorder=',sp.ApplyNumericalOrder)
            else concat('https://qlw.nxin.com',app.cAHrefDetails,'?appid=',app.menuid,'&numericalorder=',sp.ApplyNumericalOrder) end cAHrefDetails,
            CONCAT(sp.`NumericalOrder`) NumericalOrder,
            CONCAT(sp.`GroupId`) GroupId,
            sp.`ApplyData`,
            CONCAT(sp.`ApplyEnterpriseId`) ApplyEnterpriseId,
            be.`EnterpriseName` AS ApplyEnterpriseName,
            CONCAT(sp.ApplyContactType) ApplyContactType,
            bustype.`DataDictName` AS ApplyContactTypeName,
            CONCAT(sp.`ApplyMenuId`) ApplyMenuId,
            menu.`cDictName` AS ApplyMenuName,
            CONCAT(sp.`ApplyNumericalOrder`) ApplyNumericalOrder,
            sp.`ApplyPayContent`,
            CONCAT(sp.`ApplyContactEnterpriseId`) ApplyContactEnterpriseId,
            IFNULL(IFNULL(IFNULL(IFNULL(cus.`CustomerName`,mar.`MarketName`),hrs.`Name`),bes.`EnterpriseName`),'其他') ApplyContactEnterpriseName,
            sp.`ApplyEmergency`,
            sp.`ApplyDeadLine`,
            sp.`ApplyAmount`,
            sp.`ApplySurplusAmount`,
            sp.`PayAmount`,
            sp.`DeadLine`,
            sp.`Level`,
            sp.`SettlementMethod`,
            sp.`ScheduleStatus`,
            sp.`OwnerId`,
            hr.Name AS OwnerName,
            sp.`CreatedDate`,
            sp.`ModifiedDate` 
            FROM
            `nxin_qlw_business`.`fd_scheduleplan` sp
            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` be ON be.`EnterpriseID` = sp.`ApplyEnterpriseId`
            LEFT JOIN `qlw_nxin_com`.`bsdatadict` menu ON menu.`dictid` = sp.`ApplyMenuId`
            LEFT JOIN `nxin_qlw_business`.`biz_datadict` bustype ON bustype.`DataDictID` = sp.`ApplyContactType`
            LEFT JOIN `qlw_nxin_com`.`biz_customer` cus ON cus.`CustomerID` = sp.`ApplyContactEnterpriseId`
            LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON mar.`MarketID` = sp.`ApplyContactEnterpriseId`
            LEFT JOIN `qlw_nxin_com`.`HR_Person` hrs ON hrs.`PersonID` = sp.`ApplyContactEnterpriseId`
            LEFT JOIN `qlw_nxin_com`.`BIZ_Enterprise` bes ON bes.`EnterpriseID` = sp.`ApplyContactEnterpriseId`
            LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = sp.`OwnerId`
            LEFT JOIN qlw_nxin_com.stmenu app on app.menuid = sp.ApplyMenuId

            WHERE GroupId = {_identityservice.GroupId} ").ToList();
        }
        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<fd_scheduleplanReportEntity> GetReportDatas(NoneQuery query = null)
        {
            return _context.fd_scheduleplanReportDataSet.FromSqlRaw($@"SELECT 
            CASE sp.ApplyMenuId
            when '201702230104402501' then concat('https://qlw.nxin.com/FD_AccountTransfer/AccountTransfer?appid=201512289611681881&numericalorder=',sp.ApplyNumericalOrder)
            when '201702230104402502' then concat('https://qlw.nxin.com/FDAccountFundsAllocation/AccountFundsAllocation?appid=201512291502313749&numericalorder=',sp.ApplyNumericalOrder)
            else concat('https://qlw.nxin.com',app.cAHrefDetails,'?appid=',app.menuid,'&numericalorder=',sp.ApplyNumericalOrder) end cAHrefDetails,
            CONCAT(sp.`NumericalOrder`) NumericalOrder,
            CONCAT(sp.`GroupId`) GroupId,
            sp.`ApplyData`,
            CONCAT(sp.`ApplyEnterpriseId`) ApplyEnterpriseId,
            be.`EnterpriseName` AS ApplyEnterpriseName,
            CONCAT(sp.ApplyContactType) ApplyContactType,
            bustype.`DataDictName` AS ApplyContactTypeName,
            CONCAT(sp.`ApplyMenuId`) ApplyMenuId,
            menu.`cDictName` AS ApplyMenuName,
            CONCAT(sp.`ApplyNumericalOrder`) ApplyNumericalOrder,
            sp.`ApplyPayContent`,
            CONCAT(sp.`ApplyContactEnterpriseId`) ApplyContactEnterpriseId,
            IFNULL(IFNULL(IFNULL(IFNULL(cus.`CustomerName`,mar.`MarketName`),hrs.`Name`),bes.`EnterpriseName`),'其他') ApplyContactEnterpriseName,
            sp.`ApplyEmergency`,
            sp.`ApplyDeadLine`,
            sp.`ApplyAmount`,
            sp.`ApplySurplusAmount`,
            sp.`PayAmount`,
            sp.`DeadLine`,
            sp.`Level`,
            sp.`SettlementMethod`,
            sp.`ScheduleStatus`,
            sp.`OwnerId`,
            hr.Name AS OwnerName,
            sp.`CreatedDate`,
            sp.`ModifiedDate` ,
            pay.`DataDate` AS PaymentDataDate,
            CONCAT(pay.Number) PaymentNumber,
            SUM(payd.`Amount`) PaymentAmount,
            CONCAT(sp.PayNumericalOrder) PayNumericalOrder
            FROM
            `nxin_qlw_business`.`fd_scheduleplan` sp
            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` be ON be.`EnterpriseID` = sp.`ApplyEnterpriseId`
            LEFT JOIN `qlw_nxin_com`.`bsdatadict` menu ON menu.`dictid` = sp.`ApplyMenuId`
            LEFT JOIN `nxin_qlw_business`.`biz_datadict` bustype ON bustype.`DataDictID` = sp.`ApplyContactType`
            LEFT JOIN `qlw_nxin_com`.`biz_customer` cus ON cus.`CustomerID` = sp.`ApplyContactEnterpriseId`
            LEFT JOIN `qlw_nxin_com`.`biz_market` mar ON mar.`MarketID` = sp.`ApplyContactEnterpriseId`
            LEFT JOIN `qlw_nxin_com`.`HR_Person` hrs ON hrs.`PersonID` = sp.`ApplyContactEnterpriseId`
            LEFT JOIN `qlw_nxin_com`.`BIZ_Enterprise` bes ON bes.`EnterpriseID` = sp.`ApplyContactEnterpriseId`
            LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`BO_ID` = sp.`OwnerId`
            LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivables` pay ON pay.`NumericalOrder` = sp.`PayNumericalOrder`
            LEFT JOIN `nxin_qlw_business`.`fd_paymentreceivablesdetail` payd ON payd.`NumericalOrder` = pay.`NumericalOrder` 
            LEFT JOIN qlw_nxin_com.stmenu app on app.menuid = sp.ApplyMenuId
            WHERE GroupId = {_identityservice.GroupId} GROUP BY sp.NumericalOrder ").ToList();
        }
        public List<MenuInfo> GetMeunInfo(NoneQuery query = null)
        {
            return _context.MeunInfoDataSet.FromSqlRaw($@"SELECT CONCAT(MenuId) MenuId,cText MenuName,cAhrefDetails DetailUrl FROM `qlw_nxin_com`.`stmenu`  ").ToList();
        }
        public List<ApplyRelation> GetApplyRelation(string nums)
        {
            return _context.ApplyRelationDataSet.FromSqlRaw($@"SELECT FD.`NumericalOrder`,
            IFNULL(hp2.PersonID,IFNULL(b.CustomerID,IFNULL(m.`MarketID`,e.`EnterpriseID`))) Id,
            IFNULL(hp2.Name,IFNULL(b.CustomerName,IFNULL(m.`cFullName`,e.`EnterpriseName`))) AS Name
             FROM qlw_nxin_com.FD_ExpenseDetail FD
              LEFT JOIN qlw_nxin_com.HR_Person HP2 ON HP2.PersonID =FD.PersonID
              LEFT JOIN qlw_nxin_com.`biz_customer` b ON FD.CustomerID=b.CustomerID 
              LEFT JOIN qlw_nxin_com.biz_market m ON m.MarketID =FD.MarketID   
              LEFT JOIN qlw_nxin_com.BIZ_Enterprise e ON e.EnterpriseID =FD.CustomerID
              WHERE FD.`NumericalOrder` IN ({nums});
              ").ToList();
        }
    }
}
