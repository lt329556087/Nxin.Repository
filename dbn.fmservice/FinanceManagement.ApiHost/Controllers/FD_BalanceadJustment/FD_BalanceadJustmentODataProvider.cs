using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_BalanceadJustment;
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
    public class FD_BalanceadJustmentODataProvider : OneWithManyQueryProvider<FD_BalanceadJustmentODataEntity, FD_BalanceadJustmentDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BalanceadJustmentODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BalanceadJustmentODataEntity> GetList(ODataQueryOptions<FD_BalanceadJustmentODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public  IQueryable<FD_BalanceadJustmentODataEntity> GetData(ODataQueryOptions<FD_BalanceadJustmentODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_BalanceadJustmentODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT  
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                    CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,
                                    CONVERT(ent.EnterpriseName USING utf8mb4) EnterpriseName,
                                    CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,A.Remarks,
                                    CONVERT(A.AccountID USING utf8mb4) AccountID,
                                    ac.AccountName,
                                    0.00 AS EnterProjectAmount,
                                    0.00 AS BankProjectAmount,
                                    0.00 AS DiffAmount,
                                    CONVERT(A.Number USING utf8mb4) Number,
                                    CONVERT(HP1.BO_ID USING utf8mb4) AS OwnerID,
                                    HP1.Name OwnerName,
                                    CONVERT(HP2.BO_ID USING utf8mb4) AS CheckedByID,
                                    HP2.Name  CheckedByName,
                                    CONVERT(HP3.BO_ID USING utf8mb4) AS ReviewID,
                                    HP3.Name  ReviewName,
                                    CONVERT(DATE_FORMAT( A.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate
                                    FROM `nxin_qlw_business`.FD_BalanceadJustment A 
                                    LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=A.NumericalOrder AND R1.CheckMark=65536
                                    LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R2 ON R2.NumericalOrder=A.NumericalOrder AND R2.CheckMark=2048
                                    LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R3 ON R3.NumericalOrder=A.NumericalOrder AND R3.CheckMark=16
                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP2 ON HP2.BO_ID=R2.CheckedByID
                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP3 ON HP3.BO_ID=R3.CheckedByID
                                    LEFT JOIN `nxin_qlw_business`.`fd_account` ac ON A.AccountID=ac.AccountID
                                    LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON A.EnterpriseID=ent.EnterpriseID
                                    WHERE A.NumericalOrder ={manyQuery}";
            return _context.FD_BalanceadJustmentDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<FD_BalanceadJustmentODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT  
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                    CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,
                                    CONVERT(ent.EnterpriseName USING utf8mb4) EnterpriseName,
                                    CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,A.Remarks,
                                    CONVERT(A.AccountID USING utf8mb4) AccountID,
                                    ac.AccountName,
                                    B.EnterProjectAmount,
                                    B.BankProjectAmount,
                                    B.EnterProjectAmount-B.BankProjectAmount AS DiffAmount,
                                    CONVERT(A.Number USING utf8mb4) Number,
                                    CONVERT(HP1.BO_ID USING utf8mb4) AS OwnerID,
                                    HP1.Name OwnerName,
                                    CONVERT(HP2.BO_ID USING utf8mb4) AS CheckedByID,
                                    HP2.Name  CheckedByName,
                                    CONVERT(HP3.BO_ID USING utf8mb4) AS ReviewID,
                                    HP3.Name  ReviewName,
                                    CONVERT(DATE_FORMAT( A.CreatedDate,'%Y-%m-%d') USING utf8mb4) CreatedDate
                                    FROM `nxin_qlw_business`.FD_BalanceadJustment A 
                                    LEFT JOIN `nxin_qlw_business`.FD_BalanceadJustmentDetail B ON A.NumericalOrder=B.NumericalOrder
                                    LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=A.NumericalOrder AND R1.CheckMark=65536
                                    LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R2 ON R2.NumericalOrder=A.NumericalOrder AND R2.CheckMark=2048
                                    LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R3 ON R3.NumericalOrder=A.NumericalOrder AND R3.CheckMark=16
                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP2 ON HP2.BO_ID=R2.CheckedByID
                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP3 ON HP3.BO_ID=R3.CheckedByID
                                    LEFT JOIN `nxin_qlw_business`.`fd_account` ac ON A.AccountID=ac.AccountID
                                    LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON A.EnterpriseID=ent.EnterpriseID
                                    WHERE  A.EnterpriseID = {_identityservice.EnterpriseId} and B.EnterProjectID =2107271839420000107 AND B.BankProjectID=2107271839420000108
                                    GROUP BY A.NumericalOrder";
     
            return _context.FD_BalanceadJustmentDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public override Task<List<FD_BalanceadJustmentDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT  A.RecordID,
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                    CONVERT(A.EnterProjectID USING utf8mb4) EnterProjectID,
                                    IFNULL(EnterProjectAmount,0.00) AS EnterProjectAmount,
                                    CONVERT(A.BankProjectID USING utf8mb4) BankProjectID,
                                    IFNULL(BankProjectAmount,0.00) AS BankProjectAmount,
                                    '' as  EnterProjectName,'' as BankProjectName
                                    FROM `nxin_qlw_business`.FD_BalanceadJustmentDetail A 
                                    WHERE A.NumericalOrder ={manyQuery}";
            var  result= _context.FD_BalanceadJustmentDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("2107271839420000101", "企业日记账余额");
            dict.Add("2107271839420000102", "银行对账单余额");
            dict.Add("2107271839420000103", "加：银行已收企业未收");
            dict.Add("2107271839420000104", "加：企业已收银行未收");
            dict.Add("2107271839420000105", "减：银行已付企业未付");
            dict.Add("2107271839420000106", "减：企业已付银行未付");
            dict.Add("2107271839420000107", "企业调整后余额");
            dict.Add("2107271839420000108", "银行调整后余额");
            foreach (var item in result.Result)
            {
                foreach (var dictItem in dict)
                {
                    if (item.EnterProjectID==dictItem.Key)
                    {
                        item.EnterProjectName = dictItem.Value;
                    }
                    if (item.BankProjectID == dictItem.Key)
                    {
                        item.BankProjectName = dictItem.Value;
                    }
                }
            }
            return result;

        }
        public Task<List<FD_AccountODataEntity>> GetAccountFirstData(long accountID)
        {
            FormattableString sql = $@"SELECT  CONVERT(A.AccountID USING utf8mb4) AccountID,	
                                    CONVERT(A.AccountNumber USING utf8mb4) AccountNumber,	
                                    CONVERT(A.BankID USING utf8mb4) BankID,
                                    CONVERT(A.BankNumber USING utf8mb4) BankNumber,
                                    CONVERT(A.BankAreaID USING utf8mb4) BankAreaID,
                                    OpenBankEnterConnect
                                    FROM `nxin_qlw_business`.fd_account A 
                                    WHERE A.AccountID ={accountID}";
            return _context.FD_AccountDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        public List<FD_BalanceadJustmentDetailCommand> GetEmptyModel()
        {
            List<FD_BalanceadJustmentDetailCommand> model = new List<FD_BalanceadJustmentDetailCommand>()
            {
                new FD_BalanceadJustmentDetailCommand(){ EnterProjectID="2107271839420000101",EnterProjectName="企业日记账余额",BankProjectID="2107271839420000102",BankProjectName="银行对账单余额"},
                new FD_BalanceadJustmentDetailCommand(){ EnterProjectID="2107271839420000103",EnterProjectName="加：银行已收企业未收",BankProjectID="2107271839420000104",BankProjectName="加：企业已收银行未收"},
                new FD_BalanceadJustmentDetailCommand(){ EnterProjectID="2107271839420000105",EnterProjectName="减：银行已付企业未付",BankProjectID="2107271839420000106",BankProjectName="减：企业已付银行未付"},
                new FD_BalanceadJustmentDetailCommand(){ EnterProjectID="2107271839420000107",EnterProjectName="企业调整后余额",BankProjectID="2107271839420000108",BankProjectName="银行调整后余额"},
            };
            return model;
        }
    }
}
