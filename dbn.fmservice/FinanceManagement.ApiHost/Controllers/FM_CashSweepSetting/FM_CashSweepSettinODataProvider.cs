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
    public class FM_CashSweepSettingODataProvider : OneWithManyQueryProvider<FM_CashSweepSettingODataEntity, FM_CashSweepSettingDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FM_CashSweepSettingODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FM_CashSweepSettingODataEntity> GetList(ODataQueryOptions<FM_CashSweepSettingODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FM_CashSweepSettingODataEntity> GetData(ODataQueryOptions<FM_CashSweepSettingODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FM_CashSweepSettingODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,                                      
                                        CONVERT(A.`Remarks` USING utf8mb4) Remarks,  
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,                                        
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName
                                        FROM nxin_qlw_business.FM_CashSweepSetting A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                        WHERE A.EnterpriseID={_identityservice.EnterpriseId}  ORDER BY A.CreatedDate DESC";
            return _context.FM_CashSweepSettingODataSet.FromSqlInterpolated(sql); 
        }

        public IEnumerable<FM_CashSweepSettingODataEntity> GetDataByEnterID(string EnterpriseId)
        {
            if (string.IsNullOrEmpty(EnterpriseId) || EnterpriseId == "0")
            {
                EnterpriseId = _identityservice.EnterpriseId;
            }
            FormattableString sql = $@"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,                                      
                                        CONVERT(A.`Remarks` USING utf8mb4) Remarks,  
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,                                        
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName
                                        FROM nxin_qlw_business.FM_CashSweepSetting A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                        WHERE A.EnterpriseID={EnterpriseId}  ORDER BY A.CreatedDate DESC";
            return _context.FM_CashSweepSettingODataSet.FromSqlInterpolated(sql);
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FM_CashSweepSettingODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = @$"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        A.`Remarks` Remarks,  
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,                                        
                                        en.EnterpriseName AS EnterpriseName,
                                        HP.Name OwnerName
                                        FROM nxin_qlw_business.FM_CashSweepSetting A
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                    where A.NumericalOrder ={manyQuery}";

            return _context.FM_CashSweepSettingODataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
       
        public override Task<List<FM_CashSweepSettingDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = GetDetaiSql(manyQuery);
            return _context.FM_CashSweepSettingDetailODataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        public List<FM_CashSweepSettingDetailODataEntity> GetDetaiDatas(long manyQuery)
        {
            FormattableString sql = GetDetaiSql(manyQuery);
            return _context.FM_CashSweepSettingDetailODataSet.FromSqlInterpolated(sql).ToList();
        }
        private FormattableString GetDetaiSql(long manyQuery)
        {
            return $@"SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail, 
                                    CONVERT(F.SweepDirection, SIGNED) SweepDirection, 
                                    CONVERT(F.AccountTransferAbstract USING utf8mb4) AccountTransferAbstract, 
                                    F.Remarks,
                                    CONVERT(F.OwnerID USING utf8mb4) OwnerID,   
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,                                   
                                    d.cDictName AccountTransferAbstractName,
                                    h.Name OwnerName
                                    FROM nxin_qlw_business.FM_CashSweepSettingDetail F 
                                    LEFT JOIN qlw_nxin_com.bsdatadict d ON d.DictID=F.AccountTransferAbstract
                                    LEFT JOIN nxin_qlw_business.hr_person h ON h.BO_ID=F.OwnerID
                                    WHERE F.NumericalOrder = {manyQuery} ";
        }
        public Task<List<FM_CashSweepSettingExtODataEntity>> GetExtendDatasAsync(long manyQuery)
        {
            FormattableString sql = GetExtendSql(manyQuery);
            return _context.FM_CashSweepSettingExtODataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        public List<FM_CashSweepSettingExtODataEntity> GetExtendDatas(long manyQuery)
        {
            FormattableString sql = GetExtendSql(manyQuery);
            return _context.FM_CashSweepSettingExtODataSet.FromSqlInterpolated(sql).ToList();
        }
        private FormattableString GetExtendSql(long manyQuery)
        {
            return $@" SELECT 
                                    CONVERT(F.RecordID ,SIGNED) RecordID, 
                                    CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder, 
                                    CONVERT(F.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail, 
                                    CONVERT(F.BusiType ,SIGNED) BusiType, 
                                    CONVERT(F.TicketedPointID USING utf8mb4) TicketedPointID,      
                                    CONVERT(F.ReceiptAbstractID USING utf8mb4) ReceiptAbstractID,      
                                    CONVERT(F.AccoSubjectID USING utf8mb4) AccoSubjectID,      
                                    CONVERT(F.OrganizationSortID USING utf8mb4) OrganizationSortID,    
                                    CONVERT(F.OwnerID USING utf8mb4) OwnerID,                                   
                                    CONVERT( F.ModifiedDate USING utf8mb4) ModifiedDate,
                                    t.TicketedPointName,
                                    s.SettleSummaryName ReceiptAbstractName,
                                    a.AccoSubjectName,a.AccoSubjectFullName,a.AccoSubjectCode,
                                    os.cFullName OrganizationSortName,
                                    h.Name OwnerName
                                    FROM nxin_qlw_business.FM_CashSweepSettingExt F 
                                    LEFT JOIN nxin_qlw_business.biz_ticketedpoint t ON t.TicketedPointID=F.TicketedPointID
                                    LEFT JOIN qlw_nxin_com.biz_settlesummary s ON F.ReceiptAbstractID = s.SettleSummaryID  
                                    LEFT JOIN qlw_nxin_com.biz_accosubject a ON a.AccoSubjectID=F.AccoSubjectID
                                    LEFT JOIN qlw_nxin_com.bsorganizationsort os ON os.SortId=F.OrganizationSortID
                                    LEFT JOIN nxin_qlw_business.hr_person h ON h.BO_ID=F.OwnerID                             
                                    WHERE F.NumericalOrder = {manyQuery} ";
        }
    }
}
