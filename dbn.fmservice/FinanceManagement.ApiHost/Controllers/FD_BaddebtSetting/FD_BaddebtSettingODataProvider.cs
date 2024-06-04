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
    public class FD_BaddebtSettingODataProvider 
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BaddebtSettingODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BaddebtSettingODataEntity> GetList(ODataQueryOptions<FD_BaddebtSettingODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_BaddebtSettingODataEntity> GetData(ODataQueryOptions<FD_BaddebtSettingODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public IQueryable<FD_BaddebtSettingODataEntity> GetDatas(NoneQuery query = null)
        {
            var sql = GetHeadSql();
            sql += string.Format(" WHERE A.EnterpriseID={0}", _identityservice.EnterpriseId);

            return _context.FD_BaddebtSettingDataSet.FromSqlRaw(sql);
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<FD_BaddebtSettingODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            if (manyQuery < 1)
            {
                long.TryParse(_identityservice.EnterpriseId, out manyQuery);
            }
            var sql = GetHeadSql();
            sql+=string.Format(" where A.NumericalOrder ={0}",manyQuery);

            return _context.FD_BaddebtSettingDataSet.FromSqlRaw(sql).FirstOrDefaultAsync();
        }
        private string GetHeadSql()
        {
            return @"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
                                        CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
                                        CONVERT(A.ProvisionMethod USING utf8mb4) ProvisionMethod, 
                                        CONVERT(A.BadAccoSubjectOne USING utf8mb4) BadAccoSubjectOne, 
                                        CONVERT(A.BadAccoSubjectTwo USING utf8mb4) BadAccoSubjectTwo, 	                                
                                        CONVERT(A.OtherAccoSubjectOne USING utf8mb4) OtherAccoSubjectOne, 
                                        CONVERT(A.OtherAccoSubjectTwo USING utf8mb4) OtherAccoSubjectTwo, 
                                        CONVERT(A.DebtReceAccoSubjectOne USING utf8mb4) DebtReceAccoSubjectOne, 
                                        CONVERT(A.DebtReceAccoSubjectTwo USING utf8mb4) DebtReceAccoSubjectTwo, 
                                        CONVERT(A.ReceAccoSubjectOne USING utf8mb4) ReceAccoSubjectOne, 
                                        CONVERT(A.ReceAccoSubjectTwo USING utf8mb4) ReceAccoSubjectTwo, 
                                        CONVERT(A.ProvisionReceiptAbstractID USING utf8mb4) ProvisionReceiptAbstractID, 
                                        CONVERT(A.OccurReceiptAbstractID USING utf8mb4) OccurReceiptAbstractID, 
                                        CONVERT(A.RecoverReceiptAbstractID USING utf8mb4) RecoverReceiptAbstractID, 
                                        CONVERT(A.ReversalReceiptAbstractID USING utf8mb4) ReversalReceiptAbstractID, 
                                        CONVERT(A.BadReversalReceiptAbstractID USING utf8mb4) BadReversalReceiptAbstractID, 
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
                                        D.cDictName AS ProvisionMethodName,	  
                                        aBad.AccoSubjectFullName BadAccoSubjectOneName,  
                                        aBad2.AccoSubjectFullName BadAccoSubjectTwoName,
                                        aOther.AccoSubjectFullName OtherAccoSubjectOneName,
                                        aOther2.AccoSubjectFullName OtherAccoSubjectTwoName,
                                        aDebt.AccoSubjectFullName DebtReceAccoSubjectOneName,
                                        aDebt2.AccoSubjectFullName DebtReceAccoSubjectTwoName,
                                        bs.SettleSummaryName ProvisionReceiptAbstractName,
                                        bs2.SettleSummaryName OccurReceiptAbstractName,
                                        bs3.SettleSummaryName RecoverReceiptAbstractName,                      
                                        en.EnterpriseName AS EnterpriseName,
                                        arece.AccoSubjectFullName ReceAccoSubjectOneName,
                                        arece2.AccoSubjectFullName ReceAccoSubjectTwoName,
                                        bs4.SettleSummaryName ReversalReceiptAbstractName,
                                        bs5.SettleSummaryName BadReversalReceiptAbstractName,
                                        HP.Name OwnerName,
                                        'U' DataStatus,
                                        false IsProvision
                                        -- ,CONVERT(A.GroupNumericalOrder USING utf8mb4) GroupNumericalOrder,
                                        -- CONVERT(DATE_FORMAT( fbgs.`StartDate`,'%Y-%m-%d') USING utf8mb4) StartDate,
                                        -- CONVERT(DATE_FORMAT( fbgs.`EndDate`,'%Y-%m-%d') USING utf8mb4) EndDate
                                        FROM nxin_qlw_business.FD_BaddebtSetting A
                                        LEFT JOIN qlw_nxin_com.bsdatadict D ON A.ProvisionMethod = D.DictID 
                                        -- LEFT JOIN nxin_qlw_business.`fd_baddebtgroupsetting` fbgs ON fbgs.`NumericalOrder`=A.`GroupNumericalOrder`
                                        LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
                                        LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
                                        LEFT JOIN qlw_nxin_com.biz_accosubject aBad  ON aBad.AccoSubjectID=A.BadAccoSubjectOne
                                        LEFT JOIN qlw_nxin_com.biz_accosubject aBad2  ON aBad2.AccoSubjectID=A.BadAccoSubjectTwo
                                        LEFT JOIN qlw_nxin_com.biz_accosubject aOther  ON aOther.AccoSubjectID=A.OtherAccoSubjectOne
                                        LEFT JOIN qlw_nxin_com.biz_accosubject aOther2  ON aOther2.AccoSubjectID=A.OtherAccoSubjectTwo
                                        LEFT JOIN qlw_nxin_com.biz_accosubject aDebt  ON aDebt.AccoSubjectID=A.DebtReceAccoSubjectOne
                                        LEFT JOIN qlw_nxin_com.biz_accosubject aDebt2  ON aDebt2.AccoSubjectID=A.DebtReceAccoSubjectTwo
					                    LEFT JOIN qlw_nxin_com.biz_accosubject arece  ON arece.AccoSubjectID= A.ReceAccoSubjectOne 
                                        LEFT JOIN qlw_nxin_com.biz_accosubject arece2  ON arece2.AccoSubjectID= A.ReceAccoSubjectTwo 
                                        LEFT JOIN qlw_nxin_com.biz_settlesummary bs  ON bs.SettleSummaryID=A.ProvisionReceiptAbstractID
                                        LEFT JOIN qlw_nxin_com.biz_settlesummary bs2  ON bs2.SettleSummaryID=A.OccurReceiptAbstractID
                                        LEFT JOIN qlw_nxin_com.biz_settlesummary bs3  ON bs3.SettleSummaryID=A.RecoverReceiptAbstractID
                                        LEFT JOIN qlw_nxin_com.biz_settlesummary bs4  ON bs4.SettleSummaryID=A.ReversalReceiptAbstractID
                                        LEFT JOIN qlw_nxin_com.biz_settlesummary bs5  ON bs5.SettleSummaryID=A.BadReversalReceiptAbstractID";
        }

        #region 获取最近日期设置
        public FD_BaddebtSettingODataEntity GetDataByEnterId(long enterpriseId, string date)
        {
            if (enterpriseId<1)
            {
                return null;
            }
            var sql = GetHeadSql();
            sql += string.Format(" WHERE A.EnterpriseID={0}", _identityservice.EnterpriseId);
            if (string.IsNullOrEmpty(date))
            {
                sql += string.Format(@" AND A.DataDate=(SELECT IFNULL(MAX(DataDate),'{0}') DataDate FROM nxin_qlw_business.FD_BaddebtSetting A
                                        WHERE EnterpriseID={1} AND DataDate<='{0}') ", date, enterpriseId);

            }
            sql += " ORDER BY A.CreatedDate DESC";
            return _context.FD_BaddebtSettingDataSet.FromSqlRaw(sql)?.FirstOrDefault();
            #region
            //string sql =string.Format( @"SELECT CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder, 
            //                            CONVERT(DATE_FORMAT( A.DataDate,'%Y-%m-%d') USING utf8mb4) DataDate,
            //                            CONVERT(A.ProvisionMethod USING utf8mb4) ProvisionMethod, 
            //                            CONVERT(A.BadAccoSubjectOne USING utf8mb4) BadAccoSubjectOne, 
            //                            CONVERT(A.BadAccoSubjectTwo USING utf8mb4) BadAccoSubjectTwo, 	                                
            //                            CONVERT(A.OtherAccoSubjectOne USING utf8mb4) OtherAccoSubjectOne, 
            //                            CONVERT(A.OtherAccoSubjectTwo USING utf8mb4) OtherAccoSubjectTwo, 
            //                            CONVERT(A.DebtReceAccoSubjectOne USING utf8mb4) DebtReceAccoSubjectOne, 
            //                            CONVERT(A.DebtReceAccoSubjectTwo USING utf8mb4) DebtReceAccoSubjectTwo, 
            //                            CONVERT(A.ReceAccoSubjectOne USING utf8mb4) ReceAccoSubjectOne, 
            //                            CONVERT(A.ReceAccoSubjectTwo USING utf8mb4) ReceAccoSubjectTwo, 
            //                            CONVERT(A.ProvisionReceiptAbstractID USING utf8mb4) ProvisionReceiptAbstractID, 
            //                            CONVERT(A.OccurReceiptAbstractID USING utf8mb4) OccurReceiptAbstractID, 
            //                            CONVERT(A.RecoverReceiptAbstractID USING utf8mb4) RecoverReceiptAbstractID, 
            //                            CONVERT(A.ReversalReceiptAbstractID USING utf8mb4) ReversalReceiptAbstractID, 
            //                            CONVERT(A.BadReversalReceiptAbstractID USING utf8mb4) BadReversalReceiptAbstractID, 
            //                            CONVERT(A.OwnerID USING utf8mb4) OwnerID, 
            //                            CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID, 
            //                            CONVERT(A.CreatedDate USING utf8mb4) CreatedDate, 
            //                            CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate, 
            //                            D.cDictName AS ProvisionMethodName,	  
            //                            aBad.AccoSubjectFullName BadAccoSubjectOneName,  
            //                            aBad2.AccoSubjectFullName BadAccoSubjectTwoName,
            //                            aOther.AccoSubjectFullName OtherAccoSubjectOneName,
            //                            aOther2.AccoSubjectFullName OtherAccoSubjectTwoName,
            //                            aDebt.AccoSubjectFullName DebtReceAccoSubjectOneName,
            //                            aDebt2.AccoSubjectFullName DebtReceAccoSubjectTwoName,
            //                            bs.SettleSummaryName ProvisionReceiptAbstractName,
            //                            bs2.SettleSummaryName OccurReceiptAbstractName,
            //                            bs3.SettleSummaryName RecoverReceiptAbstractName,                      
            //                            en.EnterpriseName AS EnterpriseName,
            //                            arece.AccoSubjectFullName ReceAccoSubjectOneName,
            //                            arece2.AccoSubjectFullName ReceAccoSubjectTwoName,
            //                            bs4.SettleSummaryName ReversalReceiptAbstractName,
            //                            bs5.SettleSummaryName BadReversalReceiptAbstractName,
            //                            HP.Name OwnerName,
            //                            'U' DataStatus,
            //                            false IsProvision
            //                            -- ,CONVERT(A.GroupNumericalOrder USING utf8mb4) GroupNumericalOrder,
            //                            -- CONVERT(DATE_FORMAT( fbgs.`StartDate`,'%Y-%m-%d') USING utf8mb4) StartDate,
            //                            -- CONVERT(DATE_FORMAT( fbgs.`EndDate`,'%Y-%m-%d') USING utf8mb4) EndDate
            //                            FROM nxin_qlw_business.FD_BaddebtSetting A
            //                            LEFT JOIN qlw_nxin_com.bsdatadict D ON A.ProvisionMethod = D.DictID 
            //                            -- LEFT JOIN nxin_qlw_business.`fd_baddebtgroupsetting` fbgs ON fbgs.`NumericalOrder`=A.`GroupNumericalOrder`
            //                            LEFT JOIN nxin_qlw_business.HR_Person HP ON HP.BO_ID = A.OwnerID
            //                            LEFT JOIN qlw_nxin_com.biz_enterprise en ON A.EnterpriseID = en.EnterpriseID
            //                            LEFT JOIN qlw_nxin_com.biz_accosubject aBad  ON aBad.AccoSubjectID=A.BadAccoSubjectOne
            //                            LEFT JOIN qlw_nxin_com.biz_accosubject aBad2  ON aBad2.AccoSubjectID=A.BadAccoSubjectTwo
            //                            LEFT JOIN qlw_nxin_com.biz_accosubject aOther  ON aOther.AccoSubjectID=A.OtherAccoSubjectOne
            //                            LEFT JOIN qlw_nxin_com.biz_accosubject aOther2  ON aOther2.AccoSubjectID=A.OtherAccoSubjectTwo
            //                            LEFT JOIN qlw_nxin_com.biz_accosubject aDebt  ON aDebt.AccoSubjectID=A.DebtReceAccoSubjectOne
            //                            LEFT JOIN qlw_nxin_com.biz_accosubject aDebt2  ON aDebt2.AccoSubjectID=A.DebtReceAccoSubjectTwo
            //             LEFT JOIN qlw_nxin_com.biz_accosubject arece  ON arece.AccoSubjectID= A.ReceAccoSubjectOne 
            //                            LEFT JOIN qlw_nxin_com.biz_accosubject arece2  ON arece2.AccoSubjectID= A.ReceAccoSubjectTwo 
            //                            LEFT JOIN qlw_nxin_com.biz_settlesummary bs  ON bs.SettleSummaryID=A.ProvisionReceiptAbstractID
            //                            LEFT JOIN qlw_nxin_com.biz_settlesummary bs2  ON bs2.SettleSummaryID=A.OccurReceiptAbstractID
            //                            LEFT JOIN qlw_nxin_com.biz_settlesummary bs3  ON bs3.SettleSummaryID=A.RecoverReceiptAbstractID
            //                            LEFT JOIN qlw_nxin_com.biz_settlesummary bs4  ON bs4.SettleSummaryID=A.ReversalReceiptAbstractID
            //                            LEFT JOIN qlw_nxin_com.biz_settlesummary bs5  ON bs5.SettleSummaryID=A.BadReversalReceiptAbstractID
            //                        where A.EnterpriseID ={0}  ",enterpriseId);

            //if (string.IsNullOrEmpty(date))
            //{
            //    sql += string.Format(@" AND A.DataDate=(SELECT IFNULL(MAX(DataDate),'{0}') DataDate FROM nxin_qlw_business.FD_BaddebtSetting A
            //                            WHERE EnterpriseID={1} AND DataDate<='{0}') ",date,enterpriseId);

            //}
            //sql += " ORDER BY A.CreatedDate DESC";
            //var dataList = _context.FD_BaddebtSettingDataSet.FromSqlRaw(sql);
            //if (dataList != null && dataList.Count() > 0)
            //{
            //    var dataA = dataList.FirstOrDefault();
            //    var dataB = GetDetaiListAsync(dataA.NumericalOrder);
            //    dataA.Lines = dataB.Result;
            //    return dataA;
            //}
            //return null;
            #endregion
        }

        #endregion
    }
}
