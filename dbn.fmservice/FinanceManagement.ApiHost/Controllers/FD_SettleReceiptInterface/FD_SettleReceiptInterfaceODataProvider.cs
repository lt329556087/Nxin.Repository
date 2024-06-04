using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceipt;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class FD_SettleReceiptInterfaceODataProvider : OneWithManyQueryProvider<FD_SettleReceiptExtODataEntity, FD_SettleReceiptSubjectDetailODataEntity>
    {
        ILogger<FD_SettleReceiptInterfaceODataProvider> _logger;
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        public FD_SettleReceiptInterfaceODataProvider(IIdentityService identityservice, QlwCrossDbContext context,  ILogger<FD_SettleReceiptInterfaceODataProvider> logger)
        {
            _identityservice = identityservice;
            _context = context;
            _logger = logger;
        }
        public override IQueryable<FD_SettleReceiptExtODataEntity> GetDatas(NoneQuery query = null)
        {
            return null;
        }
        public IQueryable<FD_SettleReceiptExtODataEntity> GetDataList(SettleSearch search)
        {
            var strWhere = "";
            if (string.IsNullOrEmpty(search.SettleReceipType))
            {
                strWhere += " AND A.SettleReceipType NOT IN(201610220104402204,201610220104402205,201610220104402206) ";
                //strWhere += " AND A.SettleReceipType IN(201610220104402201,201610220104402202,201610220104402203) ";
            }
            else
            {
                strWhere += string.Format(" AND A.SettleReceipType IN({0}) ", search.SettleReceipType);
            }
            var sql = GetHeadSql();
            sql += string.Format(@"
                                WHERE A.EnterpriseID= {0} AND  A.DataDate BETWEEN '{1}' AND '{2}' {3}
                                ORDER BY A.DataDate DESC,A.Number DESC  ", search.EnterpriseID,search.BeginDate,search.EndDate,strWhere);
          
            return _context.FD_SettleReceiptExtDataSet.FromSqlRaw(sql);
        }
        private string GetHeadSql()
        {
            return string.Format(@"SELECT 
                         CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                         CONVERT(A.SettleReceipType USING utf8mb4) AS SettleReceipType,
                         CONVERT(A.TicketedPointID USING utf8mb4) AS TicketedPointID,
                         CONVERT(A.Number USING utf8mb4) Number,	
                         CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                         DATE_FORMAT(A.DataDate,'%Y-%m-%d') DataDate,
                         A.Guid,
                         A.Remarks,    
                         AccountNo,
                         AttachmentNum,                                   
                         CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                         CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                         CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                         d.DataDictName SettleReceipTypeName,
                         TicketedPointName,
                         EnterpriseName,h.Name OwnerName
                         FROM  nxin_qlw_business.fd_settlereceipt A 
                         LEFT JOIN  nxin_qlw_business.biz_datadict d ON d. DataDictID=A.SettleReceipType 
                         LEFT JOIN  nxin_qlw_business.biz_ticketedpoint t ON t.TicketedPointID=A.TicketedPointID 
                         LEFT JOIN  qlw_nxin_com . biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID 
                         LEFT JOIN nxin_qlw_business.hr_person h on h.BO_ID=A.OwnerID ");
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_SettleReceiptExtODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = GetDataSql(manyQuery);

            return _context.FD_SettleReceiptExtDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
        
        private FormattableString GetDataSql(long manyQuery)
        {
            return @$"SELECT 
                         CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                         CONVERT(A.SettleReceipType USING utf8mb4) AS SettleReceipType,
                         CONVERT(A.TicketedPointID USING utf8mb4) AS TicketedPointID,
                         CONVERT(A.Number USING utf8mb4) Number,	
                         CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                         DATE_FORMAT(A.DataDate,'%Y-%m-%d') DataDate,
                         A.Guid,
                         A.Remarks,    
                         AccountNo,
                         AttachmentNum,                                   
                         CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                         CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                         CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                         d.cDictName SettleReceipTypeName,
                         TicketedPointName,
                         EnterpriseName
                         FROM  nxin_qlw_business.fd_settlereceipt A 
                         LEFT JOIN  qlw_nxin_com.bsdatadict d ON d.DictID=A.SettleReceipType 
                         LEFT JOIN  nxin_qlw_business.biz_ticketedpoint t ON t.TicketedPointID=A.TicketedPointID 
                         LEFT JOIN  qlw_nxin_com . biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID
					WHERE a.NumericalOrder = {manyQuery}";
        }

        public override Task<List<FD_SettleReceiptSubjectDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = GetDetailSql(manyQuery);
            return _context.FD_SettleReceiptSubjectDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
       
        private FormattableString GetDetailSql(long manyQuery)
        {
            return @$"  SELECT 
                          FD.RecordID,
                          CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                          FD.Guid,
                          CONVERT(FD.EnterpriseID USING utf8mb4) EnterpriseID,                                     
                          CONVERT(FD.ReceiptAbstractID USING utf8mb4) ReceiptAbstractID,
                          CONVERT(FD.AccoSubjectID USING utf8mb4) AccoSubjectID,
                          CONVERT(FD.AccoSubjectCode USING utf8mb4) AccoSubjectCode,
                          CONVERT(FD.CustomerID USING utf8mb4) CustomerID,
                          CONVERT(FD.PersonID USING utf8mb4) PersonID,                                   
                          CONVERT(FD.MarketID USING utf8mb4) MarketID,                                    
                          CONVERT(FD.ProjectID USING utf8mb4) ProjectID,
                          CONVERT(FD.ProductID USING utf8mb4) ProductID,
                          CONVERT(FD.PaymentTypeID USING utf8mb4) PaymentTypeID,                                   
                          CONVERT(FD.AccountID USING utf8mb4) AccountID,
                          FD.LorR,
                          IFNULL(FD.Debit,0.0) Debit,
                          IFNULL(FD.Credit,0.0) Credit,
                          FD.Content,
                          FD.AgingDate,
                          FD.RowNum,
                          CONVERT(FD.OrganizationSortID USING utf8mb4) OrganizationSortID,
                          FD.IsCharges,
                          ent.EnterpriseName, 
                          s.SettleSummaryName ReceiptAbstractName,
                          a.AccoSubjectName,a.AccoSubjectFullName,a.IsProject bProject,a.IsCus bCus,a.IsPerson bPerson,a.IsSup bSup,a.IsDept bDept,
                          bc.CustomerName,
                          h.Name PersonName,
                          m.MarketName,m.cFullName MarketFullName,
                          p.ProjectName,
                          bp.ProductName,
                          bd.cDictName PaymentTypeName,
                          fa.AccountName,fa.DepositBank,
                          bo.SortName OrganizationSortName
                          FROM  nxin_qlw_business.fd_settlereceiptdetail FD 
                          LEFT JOIN  qlw_nxin_com.biz_enterprise  ent ON FD.EnterpriseID=ent.EnterpriseID
                          LEFT JOIN  qlw_nxin_com.biz_settlesummary s ON FD.ReceiptAbstractID=s.SettleSummaryID
                          LEFT JOIN  qlw_nxin_com.biz_accosubject a ON FD.AccoSubjectID=a.AccoSubjectID
                          LEFT JOIN  qlw_nxin_com.biz_customer bc ON bc.CustomerID=fd.CustomerID
                          LEFT JOIN  qlw_nxin_com.hr_person h ON h.PersonID=fd.PersonID
                          LEFT JOIN qlw_nxin_com.biz_market m ON m.MarketID=fd.MarketID
                          LEFT JOIN qlw_nxin_com.ppm_project p ON p.ProjectID=fd.ProjectID
                          LEFT JOIN qlw_nxin_com.biz_product bp ON bp.ProductID=fd.ProductID
                          LEFT JOIN qlw_nxin_com.bsdatadict BD ON BD.DictID=FD.PaymentTypeID 
                          LEFT JOIN nxin_qlw_business.fd_account fa ON fa.AccountID=FD.AccountID
                          LEFT JOIN qlw_nxin_com.bsorganizationsort bo ON bo.SortId=fd.OrganizationSortID
                           WHERE FD.NumericalOrder =  {manyQuery}";
        }
    }
    public class SettleSearch
    {
        public string EnterpriseID { get; set; }
        public string SettleReceipType { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }
}
