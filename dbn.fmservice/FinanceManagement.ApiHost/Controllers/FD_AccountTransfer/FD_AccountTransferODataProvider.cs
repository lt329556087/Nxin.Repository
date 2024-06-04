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
    public class FD_AccountTransferODataProvider : OneWithManyQueryProvider<FD_AccountTransferODataEntity, FD_AccountTransferDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_AccountTransferODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_AccountTransferODataEntity> GetList(ODataQueryOptions<FD_AccountTransferODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_AccountTransferODataEntity> GetData(ODataQueryOptions<FD_AccountTransferODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FD_AccountTransferODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
	                                CONVERT(A.Guid USING utf8mb4) Guid, 
	                                CONVERT(A.AccountTransferType USING utf8mb4) AccountTransferType, 
	                                CONVERT(A.AccountTransferAbstract USING utf8mb4) AccountTransferAbstract, 
	                                CONVERT(A.DataDate USING utf8mb4) DataDate, 
	                                A.Remarks, 
	                                CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
	                                CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
	                                CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
	                                CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
	                                D1.DataDictName as AccountTransferTypeName,
	                                D2.DataDictName as AccountTransferAbstractName,
	                                CONVERT(C.EnterpriseID USING utf8mb4) OutEnterpriseID, 
	                                en.EnterpriseName as OutEnterpriseName,
                                    A.UploadUrl,
	                                IFNULL(B.Amount,0) Amount ,IFNULL(C.Amount,0) RAmount, HP.Name OwnerName,0 ApprovalState,'' ApprovalStateName,
                                    CONVERT(r.ParentValueDetail USING utf8mb4) NumericalOrderForCashSweep
	                                FROM nxin_qlw_business.FD_AccountTransfer A
	                                left join nxin_qlw_business.FD_AccountTransferDetail as B on B.NumericalOrder = A.NumericalOrder and B.IsIn = 1
	                                left join nxin_qlw_business.FD_AccountTransferDetail as C on C.NumericalOrder = A.NumericalOrder and C.IsIn = 0
	                                Left Join nxin_qlw_business.BIZ_DataDict D1 on A.AccountTransferType = D1.DataDictID
	                                Left join nxin_qlw_business.biz_datadict D2 on A.AccountTransferAbstract = D2.DataDictID
	                                LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
	                                Left join qlw_nxin_com.biz_enterprise en on C.EnterpriseID = en.EnterpriseID
                                    LEFT JOIN nxin_qlw_business.`biz_related` r ON r.`ChildValue`=A.`NumericalOrder` AND r.`RelatedType`=201610210104402122 AND r.ParentType=2210111522080000109 AND r.ChildType=2108021014530000109 
                      WHERE A.EnterpriseID={_identityservice.EnterpriseId} ";
            return _context.FD_AccountTransferDataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_AccountTransferODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = @$"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(A.Guid USING utf8mb4) Guid, 
                                    CONVERT(A.AccountTransferType USING utf8mb4) AccountTransferType, 
                                    CONVERT(A.AccountTransferAbstract USING utf8mb4) AccountTransferAbstract, 
                                    CONVERT(A.DataDate USING utf8mb4) DataDate, 
                                    A.Remarks, 
                                    CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                    CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                    CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                    CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
                                    D1.DataDictName as AccountTransferTypeName,
                                    D2.DataDictName as AccountTransferAbstractName,
                                    A.UploadUrl,
                                    '0' OutEnterpriseID, 
                                    '' OutEnterpriseName,
                                    0.0 Amount ,0.0 RAmount, HP.Name OwnerName,0 ApprovalState,
                                    '' ApprovalStateName,
                                    CONVERT(r.ParentValueDetail USING utf8mb4) NumericalOrderForCashSweep
                                    FROM nxin_qlw_business.FD_AccountTransfer A
                                    Left Join nxin_qlw_business.BIZ_DataDict D1 on A.AccountTransferType = D1.DataDictID
                                    Left join nxin_qlw_business.biz_datadict D2 on A.AccountTransferAbstract = D2.DataDictID
                                    LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                    LEFT JOIN nxin_qlw_business.`biz_related` r ON r.`ChildValue`=A.`NumericalOrder` AND r.`RelatedType`=201610210104402122 AND r.ParentType=2210111522080000109 AND r.ChildType=2108021014530000109 
                                    where A.NumericalOrder ={manyQuery}";

            return _context.FD_AccountTransferDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }

        public override Task<List<FD_AccountTransferDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = $@"SELECT 
                                    CONVERT(F.RecordID USING utf8mb4) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT( F.Guid USING utf8mb4) Guid, 
                                    CONVERT( F.EnterpriseID USING utf8mb4) EnterpriseID, 
                                    EN.EnterpriseName,
                                    CONVERT( F.AccountID USING utf8mb4) AccountID, 
                                    IFNULL(F.Amount,0) Amount, 
                                    ''  AmountUpper,
                                    F.IsIn, date_format(F.DataDateTime,'%Y-%m-%d') DataDateTime, F.Remarks, 
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
                                    FA.AccountName,FA.DepositBank,FA.AccountNumber,
                                    CONVERT(F.PaymentTypeID USING utf8mb4) PaymentTypeID, 
                                    BD.DataDictName PaymentTypeName
                                    FROM nxin_qlw_business.FD_AccountTransferDetail F
                                    left join nxin_qlw_business.FD_Account FA on F.AccountID=FA.AccountID and F.EnterpriseID=FA.EnterpriseID
                                    LEFT JOIN nxin_qlw_business.BIZ_DataDict BD ON BD.DataDictID=F.PaymentTypeID and  PID=201610140104402001 and F.IsIn=1
                                    Left JOIN qlw_nxin_com.BIz_enterprise EN on F.EnterpriseID = EN.EnterpriseID
                                    where F.NumericalOrder = {manyQuery}";

            return _context.FD_AccountTransferDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
    }
}
