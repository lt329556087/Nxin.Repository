using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class FD_AccountODataProvider 
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_AccountODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_AccountAODataEntity> GetList(ODataQueryOptions<FD_AccountAODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_AccountAODataEntity> GetData(ODataQueryOptions<FD_AccountAODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public IQueryable<FD_AccountAODataEntity> GetDatas(NoneQuery query = null)
        {
            var sql = GetHeadBaseSql();
            sql += string.Format(" WHERE F.EnterpriseID={0}", _identityservice.EnterpriseId);

            return _context.FD_AccountADataSet.FromSqlRaw(sql);
        }
        //根据单位等条件获取账户 -- 开启银企直连
        public IQueryable<FD_AccountAODataEntity> GetBankAccountList(FD_AccountAODataEntity model)
        {
            var sql = GetHeadBaseSql();
            if (string.IsNullOrEmpty(model.EnterpriseID))
            {
                model.EnterpriseID = _identityservice.EnterpriseId;
            }
            sql += string.Format(" WHERE F.EnterpriseID in({0}) AND  F.OpenBankEnterConnect=true", model.EnterpriseID);
            
            return _context.FD_AccountADataSet.FromSqlRaw(sql);
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<FD_AccountAODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            if (manyQuery < 1)
            {
                long.TryParse(_identityservice.EnterpriseId, out manyQuery);
            }
            var sql = GetHeadSql();
            sql+=string.Format(" where F.AccountID ={0}", manyQuery);
            var data = _context.FD_AccountADataSet.FromSqlRaw(sql).AsNoTracking().FirstOrDefault();
            return _context.FD_AccountADataSet.FromSqlRaw(sql).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 表头-获取资金账户详情，设置不追踪实体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public FD_AccountAODataEntity GetDataByAutoWrite(long manyQuery, NoneQuery query = null)
        {
            if (manyQuery < 1)
            {
                long.TryParse(_identityservice.EnterpriseId, out manyQuery);
            }
            var sql = GetHeadSql();
            sql += string.Format(" where F.AccountID ={0}", manyQuery);
            return _context.FD_AccountADataSet.FromSqlRaw(sql).AsNoTracking().FirstOrDefault();
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public FD_AccountAODataEntity GetData(long manyQuery, NoneQuery query = null)
        {
            if (manyQuery < 1)
            {
                long.TryParse(_identityservice.EnterpriseId, out manyQuery);
            }
            var sql = GetHeadSql();
            sql += string.Format(" where F.AccountID ={0}", manyQuery);

            return _context.FD_AccountADataSet.FromSqlRaw(sql).FirstOrDefault();
        }
        /// <summary>
        /// 银行流水专用
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<FD_AccountAODataEntity> GetDataByEnterpriseId(long enteid)
        {
            var sql = GetHeadBaseSql();
            sql += string.Format(" where F.EnterpriseID ={0}", enteid);

            return _context.FD_AccountADataSet.FromSqlRaw(sql).ToList();
        }
        private string GetHeadBaseSql()
        {
            return @"     SELECT CONVERT(F.AccountID USING utf8mb4) AccountID, 
                          CONVERT(F.Guid USING utf8mb4) Guid, 
                          F.AccountName, 
                          F.AccountNumber, 
                          CONVERT(F.AccountType USING utf8mb4) AccountType, 
                          F.AccountFullName, 
                          CONVERT(F.BankID USING utf8mb4) BankID, 
                          CONVERT(F.BankAreaID USING utf8mb4) BankAreaID, 
                          F.DepositBank, 
                          F.Address, 
                          CONVERT(F.AccoSubjectID USING utf8mb4) AccoSubjectID, 
                          CONVERT(F.ExpenseAccoSubjectID USING utf8mb4) ExpenseAccoSubjectID,
                          CONVERT(F.AccountUseType USING utf8mb4) AccountUseType,
                          CONVERT(F.ResponsiblePerson USING utf8mb4) ResponsiblePerson, 
                          F.Remarks, 
                          CONVERT(F.OwnerID USING utf8mb4) OwnerID,   
                          CONVERT(DATE_FORMAT( F.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate, 
                          CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate, 
                          F.IsUse,
                          F.Sort,
                          CONVERT(F.EnterpriseID USING utf8mb4) EnterpriseID,
                          F.BankNumber,
                          F.OpenBankEnterConnect,
                          F.TubeAccountNumber,
                          CONVERT(F.MarketID USING utf8mb4) MarketID
                          ,d.`cDictName` AccountTypeName,bd.`cDictName` BankName,`cFullName`  BankAreaName
                          ,a.AccoSubjectName AccoSubjectName,'' ExpenseAccoSubjectName
                          ,bdu.`cDictName` AccountUseTypeName
                          ,'' ResponsiblePersonName,
                          hp.`Name` OwnerName,
                          e.`EnterpriseName`,
                          '' PaymentTypeID, '' PaymentTypeName
                          ,'' MarketName  
                          ,CASE F.IsUse  WHEN 0 THEN '停用' WHEN 1 THEN '启用用' END IsUseName
                          ,a.AccoSubjectCode
                          FROM nxin_qlw_business.FD_Account F
                          LEFT JOIN qlw_nxin_com.bsdatadict d ON F.AccountType=d.DictID
                          LEFT JOIN qlw_nxin_com.bsdatadict bd ON F.BankID=bd.DictID
                          LEFT JOIN qlw_nxin_com.`bsarea` bs ON F.BankAreaID=bs.`AreaID`
                          LEFT JOIN qlw_nxin_com.`biz_accosubject` a ON F.AccoSubjectID=a.`AccoSubjectID` 
                          -- LEFT JOIN qlw_nxin_com.`biz_accosubject` a1 ON F.ExpenseAccoSubjectID=a1.`AccoSubjectID` 
                          LEFT JOIN qlw_nxin_com.bsdatadict bdu ON F.AccountUseType=bdu.DictID
                          -- LEFT JOIN nxin_qlw_business.`hr_person` h ON F.`ResponsiblePerson` = h.`PersonID`
                          LEFT JOIN nxin_qlw_business.`hr_person` hp ON F.`OwnerID` = hp.`BO_ID`
                          LEFT JOIN qlw_nxin_com.`biz_enterprise` e ON F.`EnterpriseID`=e.`EnterpriseID`
                          -- LEFT JOIN nxin_qlw_business.`biz_related`  r ON r.ChildValue = F.AccountID AND RelatedType = 201610210104402102 AND ParentType = 201610140104402001 AND ChildType = 1611031642370000101 
                          -- LEFT JOIN qlw_nxin_com.bsdatadict bdp ON r.ParentValue=bdp.DictID;
                            ";
        }
        private string GetHeadSql()
        {
            return @" SELECT CONVERT(F.AccountID USING utf8mb4) AccountID, 
                      CONVERT(F.Guid USING utf8mb4) Guid, 
                      F.AccountName, 
                      F.AccountNumber, 
                      CONVERT(F.AccountType USING utf8mb4) AccountType, 
                      F.AccountFullName, 
                      CONVERT(F.BankID USING utf8mb4) BankID, 
                      CONVERT(F.BankAreaID USING utf8mb4) BankAreaID, 
                      F.DepositBank, 
                      F.Address, 
                      CONVERT(F.AccoSubjectID USING utf8mb4) AccoSubjectID, 
                      CONVERT(F.ExpenseAccoSubjectID USING utf8mb4) ExpenseAccoSubjectID,
                      CONVERT(F.AccountUseType USING utf8mb4) AccountUseType,
                      CONVERT(F.ResponsiblePerson USING utf8mb4) ResponsiblePerson, 
                      F.Remarks, 
                      CONVERT(F.OwnerID USING utf8mb4) OwnerID,   
                      CONVERT(DATE_FORMAT( F.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate, 
                      CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate, 
                      F.IsUse,
                      F.Sort,
                      CONVERT(F.EnterpriseID USING utf8mb4) EnterpriseID,
                      F.BankNumber,
                      F.OpenBankEnterConnect,
                      F.TubeAccountNumber,
                      CONVERT(F.MarketID USING utf8mb4) MarketID
                      ,d.`cDictName` AccountTypeName,bd.`cDictName` BankName,`cFullName`  BankAreaName
                      ,a.`AccoSubjectFullName` AccoSubjectName,a1.`AccoSubjectFullName` ExpenseAccoSubjectName
                      ,bdu.`cDictName` AccountUseTypeName
                      ,h.`Name` ResponsiblePersonName,
                      hp.`Name` OwnerName,
                      e.`EnterpriseName`,
                      CONCAT(bdp.`DictID`) PaymentTypeID, bdp.cDictName PaymentTypeName
                      ,m.MarketName
                      ,CASE F.IsUse  WHEN 0 THEN '停用' WHEN 1 THEN '启用用' END IsUseName
                      ,a.AccoSubjectCode
                      FROM nxin_qlw_business.FD_Account F
                      LEFT JOIN qlw_nxin_com.bsdatadict d ON F.AccountType=d.DictID
                      LEFT JOIN qlw_nxin_com.bsdatadict bd ON F.BankID=bd.DictID
                      LEFT JOIN qlw_nxin_com.`bsarea` bs ON F.BankAreaID=bs.`AreaID`
                      LEFT JOIN qlw_nxin_com.`biz_accosubject` a ON F.AccoSubjectID=a.`AccoSubjectID` 
                      LEFT JOIN qlw_nxin_com.`biz_accosubject` a1 ON F.ExpenseAccoSubjectID=a1.`AccoSubjectID` 
                      LEFT JOIN qlw_nxin_com.bsdatadict bdu ON F.AccountUseType=bdu.DictID
                      LEFT JOIN nxin_qlw_business.`hr_person` h ON F.`ResponsiblePerson` = h.`PersonID`
                      LEFT JOIN nxin_qlw_business.`hr_person` hp ON F.`OwnerID` = hp.`BO_ID`
                      LEFT JOIN qlw_nxin_com.`biz_enterprise` e ON F.`EnterpriseID`=e.`EnterpriseID`
                      LEFT JOIN nxin_qlw_business.`biz_related`  r ON r.ChildValue = F.AccountID AND RelatedType = 201610210104402102 AND ParentType = 201610140104402001 AND ChildType = 1611031642370000101 
                      LEFT JOIN qlw_nxin_com.bsdatadict bdp ON r.ParentValue=bdp.DictID
                      LEFT JOIN nxin_qlw_business.biz_market m ON m.MarketID=f.MarketID  
                     ";
        }
    }
}
