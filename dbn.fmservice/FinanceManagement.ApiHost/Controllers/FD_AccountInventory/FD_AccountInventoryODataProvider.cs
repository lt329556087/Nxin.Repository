using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_AccountInventory
{
    public class FD_AccountInventoryODataProvider : OneWithManyQueryProvider<FD_AccountInventoryODataEntity, FD_AccountInventoryDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_AccountInventoryODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_AccountInventoryODataEntity> GetList(ODataQueryOptions<FD_AccountInventoryODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public  IQueryable<FD_AccountInventoryODataEntity> GetData(ODataQueryOptions<FD_AccountInventoryODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_AccountInventoryODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT   
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(A.Guid USING utf8mb4) Guid,
                                    CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,
                                    CONVERT(A.DataDate USING utf8mb4) DataDate,
                                    CONVERT(A.TicketedPointID USING utf8mb4) TicketedPointID,
                                    CONVERT(A.Number USING utf8mb4) Number,
                                    CONVERT(A.ResponsiblePerson USING utf8mb4) ResponsiblePerson,
                                    HP.Name ResponsiblePersonName, 
                                    0.0 SumFrozeAmount,0.0 SumAvailableAmount,0.0 SumAmount, '' OwnerName , '' CheckedByName, '' ReviewName,                             
                                    Remarks
                                    FROM nxin_qlw_business.FD_AccountInventory A
                                    LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.PersonID=A.ResponsiblePerson
                                    where A.NumericalOrder ={manyQuery}";
            return _context.FD_AccountInventoryDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<FD_AccountInventoryODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT  
                                    CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(A.Guid USING utf8mb4) Guid,
                                    CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,
                                    CONVERT(A.DataDate USING utf8mb4) DataDate,
                                    CONVERT(A.TicketedPointID USING utf8mb4) TicketedPointID,
                                    CONVERT(A.Number USING utf8mb4) Number,
                                    CONVERT(A.ResponsiblePerson USING utf8mb4) ResponsiblePerson,
                                    HP.Name ResponsiblePersonName, A.Remarks, 
                                    IFNULL( Sum(B.DepositAmount+B.FuturesBond+B.OtherBond+B.BankFrozen+B.OtherAmount),0)  SumFrozeAmount  ,Sum(IFNULL(B.FlowAmount,0)) SumAvailableAmount,
                                    IFNULL( Sum(B.DepositAmount+B.FuturesBond+B.OtherBond+B.BankFrozen+B.OtherAmount+B.FlowAmount),0) SumAmount,
                                    HP1.Name OwnerName,
                                    HP2.Name  CheckedByName,
                                    HP3.Name  ReviewName
                                    FROM nxin_qlw_business.FD_AccountInventory A 
                                    left join nxin_qlw_business.FD_AccountInventoryDetail B on A.NumericalOrder=B.NumericalOrder
                                    left join nxin_qlw_business.BIZ_Reviwe R1 on R1.NumericalOrder=A.NumericalOrder and R1.CheckMark=65536
                                    left join nxin_qlw_business.BIZ_Reviwe R2 on R2.NumericalOrder=A.NumericalOrder and R2.CheckMark=16
                                    left join nxin_qlw_business.BIZ_Reviwe R3 on R3.NumericalOrder=A.NumericalOrder and R3.CheckMark=2048
                                    LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.PersonID=A.ResponsiblePerson
                                    LEFT JOIN nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                    LEFT JOIN nxin_qlw_business.HR_Person HP2 ON HP2.BO_ID=R2.CheckedByID
                                    LEFT JOIN nxin_qlw_business.HR_Person HP3 ON HP3.BO_ID=R3.CheckedByID
                                    WHERE  A.EnterpriseID = {_identityservice.EnterpriseId}
                                    GROUP BY A.NumericalOrder";
     
            return _context.FD_AccountInventoryDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表体
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <returns></returns>
        public override Task<List<FD_AccountInventoryDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
                                    f.RecordID ,
                                    CONVERT(f.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(f.Guid USING utf8mb4) Guid,
                                    CONVERT(f.AccountID USING utf8mb4) AccountID,
                                    CONVERT(f.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                    CONVERT(f.AccoSubjectCode USING utf8mb4) AccoSubjectCode,
                                    IFNULL( f.FlowAmount,0.0) FlowAmount, 
                                    IFNULL( f.DepositAmount,0.0) DepositAmount, 
                                    IFNULL( f.FrozeAmount,0.0) FrozeAmount, 
                                    IFNULL( f.FuturesBond,0.0) FuturesBond, 
                                    IFNULL( f.OtherBond,0.0) OtherBond, 
                                    IFNULL( f.BankFrozen,0.0) BankFrozen, 
                                    IFNULL( f.OtherAmount,0.0) OtherAmount, 
                                    IFNULL( f.BookAmount,0.0) BookAmount, 
                                    0.0 Subtotal,
                                    0.0 SumAmount,
                                    0.0 Diff,
                                    0 bEnd,
                                    '' cAccoSubjectFullName ,
                                    '' cAccoSubjectName, 
                                    f.Remarks,fa.AccountName 

                                    FROM nxin_qlw_business.FD_AccountInventoryDetail f
                                    LEFT JOIN nxin_qlw_business.fd_account fa ON f.AccountID=fa.AccountID where f.NumericalOrder = {manyQuery}";
            return _context.FD_AccountInventoryDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        public decimal GetAccountinventory(FDAccountSearch search)
        {
            decimal amount = 0;
            try
            {
                var sql = string.Format(@"SELECT CONVERT(IFNULL(FlowAmount,0.0) USING utf8mb4) EnterpriseId,0 Level FROM nxin_qlw_business.fd_accountinventory f
                                        INNER JOIN nxin_qlw_business.fd_accountinventorydetail fd ON f.NumericalOrder=fd.NumericalOrder
                                        WHERE EnterpriseID={0} AND AccountID={2} AND DataDate='{1}' ", search.EnterpriseID, search.DataDate, search.AccountID);
                var resultList = _context.GetBalanceSheetReview.FromSqlRaw(sql).AsNoTracking();
                if(resultList?.Count()>0) {
                    decimal.TryParse(resultList.FirstOrDefault().EnterpriseId, out amount);
                }
                return amount;
            }
            catch 
            {
                return amount;
            }
        }
    }
}
