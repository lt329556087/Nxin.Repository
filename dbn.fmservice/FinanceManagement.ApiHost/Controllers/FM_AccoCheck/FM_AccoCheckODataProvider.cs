using Architecture.Common.Application.Query;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FM_AccoCheck;
using FinanceManagement.Common;
using FinanceManagement.Common.MonthEndCheckout;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class FM_AccoCheckODataProvider : OneWithManyQueryProvider<FM_AccoCheckODataEntity, FM_AccoCheckDetailODataEntity>
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        BIZ_DataDictODataProvider _dictProvider;
        FMBaseCommon _baseUnit;
        IHttpContextAccessor _httpContextAccessor;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        IFM_AccoCheckRepository _repository;
        IFM_AccoCheckRuleRepository _rulerepository;
        IFM_AccoCheckDetailRepository _detailRepository;
        IFM_AccoCheckExtendRepository _extend;


        public FM_AccoCheckODataProvider(IIdentityService identityservice, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, IFM_AccoCheckRepository repository, IFM_AccoCheckRuleRepository rulerepository, IFM_AccoCheckDetailRepository detailRepository, IFM_AccoCheckExtendRepository extend)
        {
            _baseUnit = baseUnit;
            _identityservice = identityservice;
            _context = context;
            _treeModel = treeModel;
            _dictProvider = dictProvider;
            _httpContextAccessor = httpContextAccessor;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
            _repository = repository;
            _rulerepository = rulerepository;
            _detailRepository = detailRepository;
            _extend = extend;
        }

        public override Task<FM_AccoCheckODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<FM_AccoCheckODataEntity> GetDatas(NoneQuery query = null)
        {
            throw new NotImplementedException();
        }

        public override Task<List<FM_AccoCheckDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            throw new NotImplementedException();
        }

        public List<FM_AccoCheckCommand> GetList(int year, List<TreeModelODataEntity> persons)
        {
            List<FM_AccoCheckCommand> datas = new List<FM_AccoCheckCommand>();
            var result = _repository.GetListByYear(year, _identityservice.EnterpriseId).Result;
            foreach (var item in result)
            {
                List<FM_AccoCheckDetail> details = _detailRepository.GetListAsync(item.NumericalOrder).Result;
                List<FM_AccoCheckExtend> extends = _extend.GetExtends(item.NumericalOrder).Result;
                FM_AccoCheckCommand data = new FM_AccoCheckCommand()
                {
                    RecordID = item.RecordID,
                    NumericalOrder = item.NumericalOrder,
                    DataDate = item.DataDate,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    CheckMark=item.CheckMark,
                    Remarks = item.Remarks,
                    OwnerID = item.OwnerID,
                    OwnerName = persons?.Where(s => s.ExtendId == item.OwnerID).FirstOrDefault()?.cName,
                    EnterpriseID = item.EnterpriseID,
                    CreatedDate = item.CreatedDate.ToString("yyyy-MM-dd"),
                    ModifiedDate = item.ModifiedDate.ToString("yyyy-MM-dd"),
                    Lines = new List<FM_AccoCheckDetailCommand>()
                };
                foreach (var detail in details)
                {
                    var detaidata = new FM_AccoCheckDetailCommand()
                    {

                        RecordID = detail.RecordID,
                        NumericalOrder = detail.NumericalOrder,
                        NumericalOrderDetail = detail.NumericalOrderDetail,
                        AccoCheckType = detail.AccoCheckType,
                        CheckMark = detail.CheckMark,
                        IsNew = detail.IsNew,
                        OwnerID = detail.OwnerID,
                        OwnerName = persons?.Where(s => s.ExtendId == detail.OwnerID).FirstOrDefault()?.cName,
                        ModifiedDate = detail.ModifiedDate.ToString("yyyy-MM-dd"),
                        Extends = new List<FM_AccoCheckExtendCommand>()
                    };
                    var extenddata = extends.Where(s => s.NumericalOrder == item.NumericalOrder && s.NumericalOrderDetail == detail.NumericalOrderDetail).ToList();
                    foreach (var extend in extenddata)
                    {
                        detaidata.Extends.Add(new FM_AccoCheckExtendCommand()
                        {
                            RecordID = extend.RecordID,
                            NumericalOrder = extend.NumericalOrder,
                            NumericalOrderDetail = extend.NumericalOrderDetail,
                            MenuID = extend.MenuID.ToString(),
                            CheckMark = extend.CheckMark,
                            ModifiedDate = extend.ModifiedDate.ToString("yyyy-MM-dd"),
                        });
                    }
                    data.Lines.Add(detaidata);
                }
                datas.Add(data);
            }
            return datas;
        }
        public List<Biz_EnterprisePeriodODataEntity> GetEnterprisePeriod(int year)
        {
            FormattableString sql = $@" SELECT `RecordID`,Year,Month,Remarks,false as IsCheck,
                                        CONVERT(DATE_FORMAT( StartDate,'%Y-%m-%d') USING utf8mb4) StartDate,
                                        CONVERT(DATE_FORMAT( EndDate,'%Y-%m-%d') USING utf8mb4) EndDate,
                                        CONVERT(OwnerID USING utf8mb4) OwnerID,	
                                        CONVERT(EnterpriseID USING utf8mb4) EnterpriseID,
                                        CONVERT(CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(ModifiedDate USING utf8mb4) ModifiedDate
                                        FROM 
                                        `qlw_nxin_com`.biz_enterpriseperiod
                                        WHERE EnterpriseID={_identityservice.EnterpriseId} AND YEAR={year}
                                        ORDER BY Month  ";
            var periods = _context.Biz_EnterprisePeriodEntityDataSet.FromSqlInterpolated(sql).ToList();
            var checkes = GetList(year, new List<TreeModelODataEntity>());
            foreach (var item in periods)
            {
                var checkevalue = checkes.Where(s => s.DataDate.Month == item.Month).FirstOrDefault();
                if (checkevalue != null)
                {
                    int checkmark = checkevalue.Lines.Where(s => s.CheckMark == false).Count();
                    item.IsCheck = checkmark == 0 ? true : false;
                }
            }
            return periods;
        }


        public async Task< List<FM_AccoCheckRuleODataEntity>> GetAaccocheckRule()
        {
            List<FM_AccoCheckRuleODataEntity> list = this.GetAaccocheckRuleList();
            if (list.Count == 0)
            {
                List<FM_AccoCheckRule> ruleList = PrefabricateAccoCheckRule();
                await _rulerepository.AddRangeAsync(ruleList);
                await _rulerepository.UnitOfWork.SaveChangesAsync();
                return this.GetAaccocheckRuleList();
            }
            else
            {
                return list;
            }
        }

        public List<FM_AccoCheckRuleODataEntity> GetAaccocheckRuleList()
        {
            FormattableString sql = $@" SELECT 
                                        a.RecordID,	
                                        CONVERT(a.EnterpriseID USING utf8mb4) EnterpriseID,
                                        ent.EnterpriseName,
                                        CONVERT(a.AccoCheckType USING utf8mb4) AccoCheckType,
                                        CONVERT(a.MasterDataSource USING utf8mb4) MasterDataSource,
                                        a.MasterFormula,a.MasterSecFormula,
                                        CONVERT(a.FollowDataSource USING utf8mb4) FollowDataSource,
                                        a.FollowFormula,a.FollowSecFormula,
                                        CONVERT(a.CheckValue USING utf8mb4) CheckValue,
                                        CONVERT(a.OwnerID USING utf8mb4) OwnerID,
                                        hr.Name AS OwnerName,
                                        a.IsUse,a.CreatedDate,a.ModifiedDate
                                        FROM 
                                        `nxin_qlw_business`.FM_AccoCheckRule a
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON a.OwnerID=hr.BO_ID
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON a.EnterpriseID=ent.EnterpriseID
                                        WHERE a.EnterpriseID={_identityservice.EnterpriseId} ";
            var list = _context.FM_AccoCheckRuleDataSet.FromSqlInterpolated(sql).ToList();
            return list;
        }

        /// <summary>
        /// 预制一键结账规则
        /// </summary>
        /// <returns></returns>
        private List<FM_AccoCheckRule> PrefabricateAccoCheckRule()
        {
            var accoSubjectList = _baseUnit.GetSubjectList(_identityservice.EnterpriseId,null);
            string kucunxianjin= accoSubjectList.Where(s => s.AccoSubjectName == "库存现金").FirstOrDefault()?.AccoSubjectId;
            string yinhangcunkuan = accoSubjectList.Where(s => s.AccoSubjectName == "银行存款").FirstOrDefault()?.AccoSubjectId;
            string huobizijin = accoSubjectList.Where(s => s.AccoSubjectName == "其他货币资金").FirstOrDefault()?.AccoSubjectId;
            string yingshouzhangkuan = accoSubjectList.Where(s => s.AccoSubjectName == "应收账款").FirstOrDefault()?.AccoSubjectId;
            string zhuyingshouru = accoSubjectList.Where(s => s.AccoSubjectName == "主营业务收入").FirstOrDefault()?.AccoSubjectId;
            string zhuyingchengben = accoSubjectList.Where(s => s.AccoSubjectName == "主营业务成本").FirstOrDefault()?.AccoSubjectId;
            string yuancailiao = accoSubjectList.Where(s => s.AccoSubjectName == "原材料").FirstOrDefault()?.AccoSubjectId;
            string baozhuangwu = accoSubjectList.Where(s => s.AccoSubjectName == "包装物").FirstOrDefault()?.AccoSubjectId;
            string banchengpin = accoSubjectList.Where(s => s.AccoSubjectName == "自制半成品及在产品").FirstOrDefault()?.AccoSubjectId;
            string yingfuzhangkuan = accoSubjectList.Where(s => s.AccoSubjectName == "应付账款").FirstOrDefault()?.AccoSubjectId;
            string kuncunshangpin = accoSubjectList.Where(s => s.AccoSubjectName == "库存商品").FirstOrDefault()?.AccoSubjectId;
            string shengwuzichan = accoSubjectList.Where(s => s.AccoSubjectName == "生产性生物资产").FirstOrDefault()?.AccoSubjectId;
            string shengwuzhejiu = accoSubjectList.Where(s => s.AccoSubjectName == "生产性生物资产累计折旧").FirstOrDefault()?.AccoSubjectId;
            string shengwujianzhi = accoSubjectList.Where(s => s.AccoSubjectName == "生产性生物资产减值准备").FirstOrDefault()?.AccoSubjectId;
            string shengwuxiaohao = accoSubjectList.Where(s => s.AccoSubjectName == "消耗性生物资产").FirstOrDefault()?.AccoSubjectId;
            string shengchanchengben = accoSubjectList.Where(s => s.AccoSubjectName == "生产成本").FirstOrDefault()?.AccoSubjectId;
            string gudingzichan = accoSubjectList.Where(s => s.AccoSubjectName == "固定资产").FirstOrDefault()?.AccoSubjectId;
            string leijizhejiu = accoSubjectList.Where(s => s.AccoSubjectName == "累计折旧").FirstOrDefault()?.AccoSubjectId;
            string zhouzhuancailiao = accoSubjectList.Where(s => s.AccoSubjectName == "周转材料").FirstOrDefault()?.AccoSubjectId;
            var EnterpriseId = _identityservice.EnterpriseId;
            var UserId = _identityservice.UserId;
            List<FM_AccoCheckRule> ruleList = new List<FM_AccoCheckRule>()
                {
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.销售结账.GetValue().ToString(),AccocheckDataSource.销售单.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.销售结账.GetValue().ToString(),AccocheckDataSource.猪只销售单.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.销售结账.GetValue().ToString(),AccocheckDataSource.猪只销售.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.销售结账.GetValue().ToString(),AccocheckDataSource.精液销售单.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.销售结账.GetValue().ToString(),AccocheckDataSource.其他销售单.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.销售结账.GetValue().ToString(),AccocheckDataSource.直营销售单.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.销售结账.GetValue().ToString(),AccocheckDataSource.折扣计提.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.销售结账.GetValue().ToString(),AccocheckDataSource.销售汇总表.GetValue().ToString(),"{(销售数量)}","{(1905141116550000108)}",AccocheckDataSource.存货汇总表.GetValue().ToString(),"{(本期出库数量)}","{(1905141116550000116)}",AccocheckState.一致.GetValue().ToString(),UserId,true),

                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.采购结账.GetValue().ToString(),AccocheckDataSource.采购单.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.采购结账.GetValue().ToString(),AccocheckDataSource.采购单集团版.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.采购结账.GetValue().ToString(),AccocheckDataSource.猪只采购.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.采购结账.GetValue().ToString(),AccocheckDataSource.猪只采购单.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.采购结账.GetValue().ToString(),AccocheckDataSource.精液采购单.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.采购结账.GetValue().ToString(),AccocheckDataSource.采购汇总表.GetValue().ToString(),"{(入库数量)}","{(1905141116550000112)}",AccocheckDataSource.存货汇总表.GetValue().ToString(),"{(本期入库数量)}","{(1905141116550000115)}",AccocheckState.一致.GetValue().ToString(),UserId,true),

                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.物品结账.GetValue().ToString(),AccocheckDataSource.物品明细表.GetValue().ToString(),"{(期末结存金额)}","{(1905141116550000118)}",AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1411&周转材料](期末余额)}","{[1411&"+zhouzhuancailiao+"](1905141116550000107)}",AccocheckState.一致.GetValue().ToString(),UserId,true),

                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.存货结账.GetValue().ToString(),AccocheckDataSource.存货汇总表.GetValue().ToString(),"{(期末结存数量)}","{(1905141116550000117)}",AccocheckDataSource.成本汇总表.GetValue().ToString(),"{(期末数量)}","{(1905141116550000119)}",AccocheckState.一致.GetValue().ToString(),UserId,true),

                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.固定资产.GetValue().ToString(),AccocheckDataSource.折旧汇总表.GetValue().ToString(),"{(本期折旧)}","{(1905141116550000126)}","","","",AccocheckState.已计提.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.固定资产.GetValue().ToString(),AccocheckDataSource.折旧汇总表.GetValue().ToString(),"{(本期折旧)}","{(1905141116550000126)}",AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1602&累计折旧](本期贷方)}","{[1602&"+leijizhejiu+"](1905141116550000106)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.固定资产.GetValue().ToString(),AccocheckDataSource.固定资产卡片报表.GetValue().ToString(),"{(累计折旧)}","{(1905141116550000127)}",AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1602&累计折旧](期末余额)}","{[1602&"+leijizhejiu+"](1905141116550000107)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.固定资产.GetValue().ToString(),AccocheckDataSource.固定资产卡片报表.GetValue().ToString(),"{(资产原值)}","{(1905141116550000128)}",AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1601&固定资产](期末余额)}","{[1601&"+gudingzichan+"](1905141116550000107)}",AccocheckState.一致.GetValue().ToString(),UserId,true),

                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.成本管理.GetValue().ToString(),AccocheckDataSource.成本汇总表.GetValue().ToString(),"{(采购成本)}","{(1905141116550000121)}","1","{【采购汇总表】(不含税采购金额)}+{【运费汇总表】(实际运费)}","{【1612121053430000101】(1905141116550000113)}+{【1707311529570000105】(1905141116550000114)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.成本管理.GetValue().ToString(),AccocheckDataSource.成本汇总表.GetValue().ToString(),"{(销售成本)}","{(1905141116550000120)}",AccocheckDataSource.销售汇总表.GetValue().ToString(),"{(销售成本)}","{(1905141116550000109)}",AccocheckState.一致.GetValue().ToString(),UserId,true),

                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.会计凭证.GetValue().ToString(),"财务审核","1905141116550000101","","","",AccocheckState.已审核.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[232|损益类](期末余额)}","{[232|232](1905141116550000107)}","","","",AccocheckState.无余额.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[231|成本类](期末余额)}","{[231|231](1905141116550000107)}","","","",AccocheckState.无余额.GetValue().ToString(),UserId,true),

                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1001&库存现金](期末余额)}+{[1002&银行存款](期末余额)}+{[1012&其他货币资金](期末余额)}","{[1001&"+kucunxianjin+"](1905141116550000107)}+{[1002&"+yinhangcunkuan+"](1905141116550000107)}+{[1012&"+huobizijin+"](1905141116550000107)}",AccocheckDataSource.资金汇总表.GetValue().ToString(),"{(期末金额)}","{(1905141116550000129)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.往来余额表.GetValue().ToString(),"{[2202&应付账款](期末余额)}","{[2202&"+yingfuzhangkuan+"](1905141116550000104)}",AccocheckDataSource.应付账款汇总表.GetValue().ToString(),"{(应付账款)}","{(1905141116550000131)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.往来余额表.GetValue().ToString(),"{[1122&应收账款](期末余额)}","{[1122&"+yingshouzhangkuan+"](1905141116550000104)}",AccocheckDataSource.应收账款汇总表.GetValue().ToString(),"{(应收净额)}","{(1905141116550000130)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[6001&主营业务收入](本期贷方)}","{[6001&"+zhuyingshouru+"](1905141116550000106)}",AccocheckDataSource.销售汇总表.GetValue().ToString(),"{(销售净额)}","{(1905141116550000110)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[6401&主营业务成本](本期借方)}","{[6401&"+zhuyingchengben+"](1905141116550000105)}",AccocheckDataSource.销售汇总表.GetValue().ToString(),"{(销售成本)}","{(1905141116550000109)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1403&原材料](期末余额)}","{[1403&"+yuancailiao+"](1905141116550000107)}",AccocheckDataSource.成本汇总表.GetValue().ToString(),"{(原材料期末成本)}","{(1905141116550000122)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1409&包装物](期末余额)}","{[1409&"+baozhuangwu+"](1905141116550000107)}",AccocheckDataSource.成本汇总表.GetValue().ToString(),"{(包装物期末成本)}","{(1905141116550000123)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1412&自制半成品及在产品](期末余额)}","{[1412&"+banchengpin+"](1905141116550000107)}",AccocheckDataSource.成本汇总表.GetValue().ToString(),"{(半成品期末成本)}","{(1905141116550000124)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1405&库存商品](期末余额)}+{[1621&生产性生物资产](期末余额)}-{[1622&生产性生物资产累计折旧](期末余额)}-{[1623&生产性生物资产减值准备](期末余额)}+{[1421&消耗性生物资产](期末余额)}+{[5001&生产成本](期末余额)}","{[1405&"+kuncunshangpin+"](1905141116550000107)}+{[1621&"+shengwuzichan+"](1905141116550000107)}-{[1622&"+shengwuzhejiu+"](1905141116550000107)}-{[1623&"+shengwujianzhi+"](1905141116550000107)}+{[1421&"+shengwuxiaohao+"](1905141116550000107)}+{[5001&"+shengchanchengben+"](1905141116550000107)}",AccocheckDataSource.成本汇总表.GetValue().ToString(),"{(产成品期末成本)}","{(1905141116550000125)}",AccocheckState.一致.GetValue().ToString(),UserId,true),
                    new FM_AccoCheckRule(EnterpriseId,AccoCheckTypeEnum.会计结账.GetValue().ToString(),AccocheckDataSource.科目余额表.GetValue().ToString(),"{[1621&生产性生物资产](期末余额)}-{[1622&生产性生物资产累计折旧](期末余额)}+{[1421&消耗性生物资产](期末余额)}+{[5001&生产成本](期末余额)}","{[1621&"+shengwuzichan+"](1905141116550000107)}-{[1622&"+shengwuzhejiu+"](1905141116550000107)}+{[1421&"+shengwuxiaohao+"](1905141116550000107)}+{[5001&"+shengchanchengben+"](1905141116550000107)}",AccocheckDataSource.成本变动汇总表.GetValue().ToString(),"{(期末原值)}-{(期末累计折旧)}+{(期末待摊费用)}","{(1905141116550000132)}-{(1905141116550000133)}+{(1905141116550000134)}",AccocheckState.一致.GetValue().ToString(),UserId,true),

                };
            return ruleList;
        }
        private List<AccoSubjectInfo> GetAccoSubjectList()
        {
            return _context.AccoSubjectInfoDataSet.FromSqlRaw($@"
              SELECT CONCAT(A.AccoSubjectID) AccoSubjectId,CONCAT(A.AccoSubjectCode) AccoSubjectCode,A.AccoSubjectFullName AccoSubjectName,CONCAT(A.EnterpriseID) EnterpriseId,
            `IsTorF`,`IsLorR`,`IsProject`,`IsCus`,`IsPerson`,`IsSup`,`IsDept`,`IsItem`,`IsCash`,`IsBank` FROM qlw_nxin_com.`biz_accosubject` A INNER JOIN qlw_nxin_com.`biz_versionsetting` B ON A.`VersionID`=B.`VersionID`
            INNER JOIN qlw_nxin_com.`biz_enterprise` C ON A.`EnterpriseID`=C.`EnterpriseID`
            WHERE (C.`EnterpriseID`={_identityservice.GroupId} OR C.`EnterpriseID`={_identityservice.EnterpriseId}) AND  B.`iVersionType`=1712221411430000101 AND B.`EnterpriseID`={_identityservice.GroupId} AND '{DateTime.Now.ToString("yyyy-MM-dd")}' BETWEEN B.`dBegin` AND B.`dEnd`  AND A.IsUse = TRUE 
            ").ToList();
        }

    }
}
