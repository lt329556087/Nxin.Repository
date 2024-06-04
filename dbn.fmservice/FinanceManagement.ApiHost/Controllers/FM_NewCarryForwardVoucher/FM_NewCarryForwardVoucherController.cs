using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using Aspose.Cells;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_NewCarryForwardVoucher
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_NewCarryForwardVoucherController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        FMBaseCommon _baseUnit;
        FM_NewCarryForwardVoucherODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        BIZ_DataDictODataProvider _dictProvider;
        private TreeModelODataProvider _treeModel;
        private IIdentityService _identityservice;

        public FM_NewCarryForwardVoucherController(IIdentityService identityservice, IMediator mediator, FM_NewCarryForwardVoucherODataProvider provider, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, FMBaseCommon baseUnit, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _identityservice = identityservice;
            _mediator = mediator;
            _provider = provider;
            _comUtil = comUtil;
            _treeModel = treeModel;
            _baseUnit = baseUnit;
            _dictProvider = dictProvider;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public Result GetDetail(long key)
        {
            var result = new Result();
            var data = _provider.GetSingleData(key);
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FM_NewCarryForwardVoucherAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FM_NewCarryForwardVoucherDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FM_NewCarryForwardVoucherModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //复制
        [HttpPost]
        [Route("Copy")]
        public async Task<Result> Copy([FromBody] FM_NewCarryForwardVoucherCopyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        /// <summary>
        /// 事件上传
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        [DisableRequestSizeLimit]
        public IActionResult Upload(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return Ok(new { code = "201", msg = "请上传文件" });
                }
                var fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension == null)
                {
                    return Ok(new { code = "201", msg = "文件无后缀信息" });
                }
                long length = file.Length;
                if (length > 1024 * 1024 * 300) //200M
                {
                    return Ok(new { code = "201", msg = "上传文件不能超过300M" });
                }
                AuthenticationHeaderValue authentication = null;
                bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
                if (!verification)
                {
                    return null;
                }
                List<FM_NewCarryForwardVoucherImport> importList = new List<FM_NewCarryForwardVoucherImport>();
                using (Stream fs = file.OpenReadStream())
                {
                    Workbook workbook = new Workbook(fs);
                    Worksheet sheet = workbook.Worksheets[0];
                    Cells cells = sheet.Cells;
                    DataTable dt = cells.ExportDataTable(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                    Dictionary<string, string> columDic = new Dictionary<string, string>()
                    {
                        {"单位名称","EnterpriseName"},
                        {"结转类别","TransferAccountsTypeName"},
                        {"来源数据","DataSourceName"},
                        {"模板名称","Remarks"},
                        {"单据字","TicketedPointName"},
                        {"末级摘要","ReceiptAbstractName"},
                        {"会计科目全称","AccoSubjectName"},
                        {"辅助项目","AuxiliaryProject"},
                        {"借方金额","DebitFormula"},
                        {"贷方金额","CreditFormula"},
                        {"分录生成条件","Condition"},
                        {"制单人","OwnerName"}
                    };
                    if (dt.Columns != null && dt.Columns.Count == columDic.Count)
                    {
                        string msg = "";
                        List<string> ct = new List<string>(columDic.Keys);
                        for (int i = 0; i < columDic.Count; i++)
                        {
                            if (columDic.ContainsKey(dt.Columns[i].ColumnName))
                            {
                                dt.Columns[i].ColumnName = columDic[dt.Columns[i].ColumnName];
                            }
                            else
                            {
                                msg += "模板中列名[" + dt.Columns[i].ColumnName + "]与单据中的列名[" + ct[i] + "]不一致,请修改后再导入";
                            }
                        }
                        if (!string.IsNullOrEmpty(msg))
                        {
                            return Ok(new { code = "201", status = false, msg = msg });
                        }
                    }
                    else
                    {
                        return Ok(new { code = "201", status = false, msg = "文件列不匹配" });
                    }
                    #region 验证年度是否有会计结账 + 名称属性转换为ID 
                    var transferAccountsTypeList = _dictProvider.GetDataDictAsyncDisposed("1211081429200099101");//结转类型
                    var ticketedPointList = _baseUnit.GetSymbol(_identityService.EnterpriseId).Result.Data;
                    var abstractList = _baseUnit.GetReceiptAbstractAllList(_identityService.EnterpriseId).Result;
                    var subjectList = _baseUnit.GetSubjectList(_identityService.EnterpriseId, authentication);
                    #region 分录条件
                    var produtClassList = _treeModel.GetProductGroupClassAsync(_identityservice.GroupId);
                    var supplierList = _treeModel.GetSupplierAsync();
                    var suppliesList = _treeModel.GetSuppliesAsync(_identityservice.EnterpriseId);
                    var chickenList = _baseUnit.GetChickenFarm(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId, Boid = _identityservice.UserId });
                    var breedList = _baseUnit.GetBreeding(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                    var batchList = _baseUnit.GetBatching(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                    var changquList = _baseUnit.GetJurisdictionList(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId, Boid = _identityservice.UserId }, "2");
                    var jisheList = _baseUnit.GetJurisdictionList(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId, Boid = _identityservice.UserId }, "3");
                    var saleAbstractList = _dictProvider.GetDataDictAsyncDisposed("201610140104402501");//销售摘要
                    var purchaseAbstractList = _dictProvider.GetDataDictAsyncDisposed("201610140104402301");//采购摘要
                    var costTypeList = _dictProvider.GetDataDictDisposed("202205111355001101");//费用性质
                    var marketList = _baseUnit.GetMarket(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                    var personList = _baseUnit.GetPerson(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                    var porductList = _baseUnit.getProductData(new DropSelectSearch() { EnterpriseID = _identityservice.EnterpriseId });
                    var pigtypeList = _baseUnit.GetPigTypes(_identityService.EnterpriseId);
                    var pigfarmList = _baseUnit.GetPigFarm(new DropSelectSearch() { EnterpriseID= _identityService.EnterpriseId } );
                    var assetsClassificationList= _baseUnit.getFA_AssetsClassificationData(_identityService.GroupId);
                    #endregion

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        FM_NewCarryForwardVoucherImport import = new FM_NewCarryForwardVoucherImport();
                        List<string> operatorList = new List<string>() { "+", "-", "*", "/", "^" };
                        List<string> bracketList = new List<string>() { "(", ")" };
                        //模板名称
                        import.Remarks = dt.Rows[i]["Remarks"].ToString();
                        //结转类别
                        var Trans = transferAccountsTypeList.Where(m => m.DataDictName == dt.Rows[i]["TransferAccountsTypeName"].ToString()).FirstOrDefault();
                        if (Trans != null && !string.IsNullOrEmpty(dt.Rows[i]["TransferAccountsTypeName"].ToString()))
                        {
                            import.TransferAccountsType = Trans.DataDictID;
                            import.TransferAccountsTypeName = Trans.DataDictName;
                        }
                        //数据来源
                        var dataSource = _baseUnit.GetDataSource(import.TransferAccountsType);
                        if (dataSource != null)
                        {
                            import.DataSource = dataSource.DataSource;
                            import.DataSourceName = dataSource.DataSourceName;
                        }
                        //单据字
                        var Ttemp = ticketedPointList.Where(m => m.TicketedPointName == dt.Rows[i]["TicketedPointName"].ToString()).FirstOrDefault();
                        if (Ttemp != null && !string.IsNullOrEmpty(dt.Rows[i]["TicketedPointName"].ToString()))
                        {
                            import.TicketedPointID = Ttemp.TicketedPointID;
                            import.TicketedPointName = Ttemp.TicketedPointName;
                        }
                        //末级摘要
                        var Abstracts = abstractList.Where(m => m.name == dt.Rows[i]["ReceiptAbstractName"].ToString()).FirstOrDefault();
                        if (Abstracts != null && !string.IsNullOrEmpty(dt.Rows[i]["ReceiptAbstractName"].ToString()))
                        {
                            import.ReceiptAbstractID = Abstracts.id;
                            import.ReceiptAbstractName = Abstracts.name;
                        }
                        //会计科目
                        var Subjects = subjectList.Where(m => m.AccoSubjectFullName == dt.Rows[i]["AccoSubjectName"].ToString()).FirstOrDefault();
                        if (Subjects != null && !string.IsNullOrEmpty(dt.Rows[i]["AccoSubjectName"].ToString()))
                        {
                            import.AccoSubjectID = Subjects.AccoSubjectId;
                            import.AccoSubjectName = Subjects.AccoSubjectFullName;
                            import.AccoSubjectCode = Subjects.AccoSubjectCode;
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[i]["AuxiliaryProject"].ToString()))
                        {
                            string AuxiliaryProject = dt.Rows[i]["AuxiliaryProject"].ToString();
                            var projectList = AuxiliaryProject.Split(',');
                            foreach (var item in projectList)
                            {
                                switch (item)
                                {
                                    case "员工":
                                        import.IsPerson = true;
                                        break;
                                    case "部门":
                                        import.IsMarket = true;
                                        break;
                                    case "客/商":
                                        import.IsCustomer = true;
                                        break;
                                    case "商品":
                                        import.IsProduct = true;
                                        break;
                                    case "项目":
                                        import.IsProject = true;
                                        break;
                                    case "汇总":
                                        import.IsSum = true;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[i]["DebitFormula"].ToString()))
                        {
                            int tableType = 0;
                            import.DebitFormula = dt.Rows[i]["DebitFormula"].ToString();
                            string debitFormula = dt.Rows[i]["DebitFormula"].ToString();
                            if (import.DebitFormula.Contains("[取值明细(费用分摊明细表)]"))
                            {
                                tableType = 1;
                                import.DebitFormula = import.DebitFormula.Replace("[取值明细(费用分摊明细表)]", "");
                            }
                            if (import.DebitFormula.Contains("[分摊明细(费用分摊明细表)]"))
                            {
                                tableType = 2;
                                import.DebitFormula = import.DebitFormula.Replace("[分摊明细(费用分摊明细表)]", "");
                            }
                            var formulaList = GetFormulaList(Convert.ToInt64(import.TransferAccountsType), tableType);
                            List<FM_NewCarryForwardVoucherFormulaCommand> Formulas = new List<FM_NewCarryForwardVoucherFormulaCommand>();
                            foreach (var formula in formulaList)
                            {
                                if (debitFormula.Contains("[" + formula.DataDictName + "]")|| debitFormula.Contains( formula.DataDictName ))
                                {
                                    debitFormula = debitFormula.Replace(formula.DataDictName.TrimStart('[').TrimEnd(']'), formula.DataDictID);
                                }
                            }
                            import.DebitSecFormula = debitFormula;
                            string newDebitFormula = debitFormula.Replace("+", "[+]").Replace("-", "[-]").Replace("*", "[*]").Replace("/", "[/]").Replace("^", "[^]").Replace("(", "[(]").Replace(")", "[)]").Replace("[2201101409320000151]", "").Replace("[2201101409320000152]", "");
                            var debitprojectList = newDebitFormula.Split(']', StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < debitprojectList.Length; j++)
                            {
                                string str = debitprojectList[j].TrimStart('[');
                                if (operatorList.Contains(str))
                                {
                                    Formulas.Add(new FM_NewCarryForwardVoucherFormulaCommand() { Operator = str, RowNum = j + 1 });
                                }
                                else if (bracketList.Contains(str))
                                {
                                    Formulas.Add(new FM_NewCarryForwardVoucherFormulaCommand() { Bracket = str, RowNum = j + 1 });
                                }
                                else
                                {
                                    Formulas.Add(new FM_NewCarryForwardVoucherFormulaCommand() { FormulaID = str, RowNum = j + 1 });
                                }
                            }
                            import.Formulas = Formulas;
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[i]["CreditFormula"].ToString()))
                        {
                            int tableType = 0;
                            import.CreditFormula = dt.Rows[i]["CreditFormula"].ToString();
                            string creditFormula = dt.Rows[i]["CreditFormula"].ToString();
                            if (import.CreditFormula.Contains("[取值明细(费用分摊明细表)]"))
                            {
                                tableType = 1;
                                import.CreditFormula = import.CreditFormula.Replace("[取值明细(费用分摊明细表)]", "");
                            }
                            if (import.CreditFormula.Contains("[分摊明细(费用分摊明细表)]"))
                            {
                                tableType = 2;
                                import.CreditFormula = import.CreditFormula.Replace("[分摊明细(费用分摊明细表)]", "");
                            }
                            var formulaList = GetFormulaList(Convert.ToInt64(import.TransferAccountsType), tableType);
                            List<FM_NewCarryForwardVoucherFormulaCommand> Formulas = new List<FM_NewCarryForwardVoucherFormulaCommand>();
                            foreach (var formula in formulaList)
                            {
                                if (creditFormula.Contains("[" + formula.DataDictName + "]")|| creditFormula.Contains(formula.DataDictName ))
                                {
                                    creditFormula = creditFormula.Replace(formula.DataDictName.TrimStart('[').TrimEnd(']'), formula.DataDictID);
                                }
                            }
                            import.CreditSecFormula = creditFormula;
                            string newCreditFormula = creditFormula.Replace("+", "[+]").Replace("-", "[-]").Replace("*", "[*]").Replace("/", "[/]").Replace("^", "[^]").Replace("(", "[(]").Replace(")", "[)]").Replace("[2201101409320000151]", "").Replace("[2201101409320000152]", "");
                            var debitprojectList = newCreditFormula.Split(']', StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < debitprojectList.Length; j++)
                            {
                                string str = debitprojectList[j].TrimStart('[');
                                if (operatorList.Contains(str))
                                {
                                    Formulas.Add(new FM_NewCarryForwardVoucherFormulaCommand() { Operator = str, RowNum = j + 1 });
                                }
                                else if (bracketList.Contains(str))
                                {
                                    Formulas.Add(new FM_NewCarryForwardVoucherFormulaCommand() { Bracket = str, RowNum = j + 1 });
                                }
                                else
                                {
                                    Formulas.Add(new FM_NewCarryForwardVoucherFormulaCommand() { FormulaID = str, RowNum = j + 1 });
                                }
                            }
                            import.Formulas = Formulas;
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[i]["Condition"].ToString()))
                        {
                            List<FM_NewCarryForwardVoucherExtendCommand> Extends = new List<FM_NewCarryForwardVoucherExtendCommand>();
                            string Condition = dt.Rows[i]["Condition"].ToString();
                            var conditionList = Condition.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in conditionList)
                            {
                                string sort = item.Substring(item.IndexOf('['), item.LastIndexOf(']')).TrimStart('[');
                                var content = item.Replace("[" + sort + "]", "").Split("&&");
                                switch (sort)
                                {
                                    case "费用性质":
                                        foreach (var cont in content)
                                        {
                                            Extends.Add(new FM_NewCarryForwardVoucherExtendCommand() { Sort = (int)SortTypeEnum.费用性质, Symbol = "=", Object = costTypeList.Where(s => s.DataDictName == cont).FirstOrDefault()?.DataDictID });
                                        }
                                        break;
                                    case "部门":
                                        foreach (var cont in content)
                                        {
                                            Extends.Add(new FM_NewCarryForwardVoucherExtendCommand() { Sort = (int)SortTypeEnum.部门, Symbol = "=", Object = marketList.Where(s => s.cName == cont).FirstOrDefault()?.Id });
                                        }
                                        break;
                                    case "商品代号":
                                        foreach (var cont in content)
                                        {
                                            Extends.Add(new FM_NewCarryForwardVoucherExtendCommand() { Sort = (int)SortTypeEnum.商品代号, Symbol = "=", Object = porductList.Where(s => s.cName == cont).FirstOrDefault()?.Id });
                                        }
                                        break;
                                    case "猪只类型":
                                        foreach (var cont in content)
                                        {
                                            Extends.Add(new FM_NewCarryForwardVoucherExtendCommand() { Sort = (int)SortTypeEnum.猪只类型, Symbol = "=", Object = pigtypeList.Where(s => s.cName == cont).FirstOrDefault()?.Id });
                                        }
                                        break;
                                    case "猪场":
                                        foreach (var cont in content)
                                        {
                                            Extends.Add(new FM_NewCarryForwardVoucherExtendCommand() { Sort = (int)SortTypeEnum.猪场, Symbol = "=", Object = pigfarmList.Where(s => s.cName == cont).FirstOrDefault()?.Id });
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            import.Extends = Extends;
                        }
                        importList.Add(import);
                    }
                    #endregion

                }
                var groupList = importList.GroupBy(s => new { s.TransferAccountsType, s.DataSource, s.Remarks, s.TicketedPointID });
                Result result = new Result();
                foreach (var item in groupList)
                {
                    FM_NewCarryForwardVoucherAddCommand model = new FM_NewCarryForwardVoucherAddCommand()
                    {
                        DataSource = item.Key.DataSource,
                        TicketedPointID = item.Key.TicketedPointID,
                        Remarks = item.Key.Remarks,
                        TransferAccountsType = item.Key.TransferAccountsType,
                        CreatedDate = DateTime.Now
                    };
                    List<FM_NewCarryForwardVoucherDetailCommand> Lines = new List<FM_NewCarryForwardVoucherDetailCommand>();
                   
                    foreach (var detail in item)
                    {
                        Lines.Add(new FM_NewCarryForwardVoucherDetailCommand()
                        {
                            ReceiptAbstractID = detail.ReceiptAbstractID,
                            AccoSubjectCode = detail.AccoSubjectCode,
                            AccoSubjectID = detail.AccoSubjectID,
                            IsPerson = detail.IsPerson,
                            IsCustomer = detail.IsCustomer,
                            IsMarket = detail.IsMarket,
                            IsPigFram = detail.IsPigFram,
                            IsProject = detail.IsProject,
                            IsProduct = detail.IsProduct,
                            IsSum = detail.IsSum,
                            DebitFormula = detail.DebitFormula,
                            DebitSecFormula = detail.DebitSecFormula,
                            CreditFormula = detail.CreditFormula,
                            CreditSecFormula = detail.CreditSecFormula,
                            Formulas = detail.Formulas,
                            Extends = detail.Extends
                        });
                    }
                    model.Lines = Lines;
                    result = _mediator.Send(model, HttpContext.RequestAborted).Result;
                    if (result.code!=0)
                    {
                        return Ok(new { code = result.code, status = false, msg = result.msg+"参数"+JsonConvert.SerializeObject(model), });
                    }
                }
                return Ok(new { code = result.code, status = true, msg = result.data, });
            }
            catch (Exception e)
            {
                return Ok(new { code = ErrorCode.RequestArgumentError.GetIntValue(), status = false, msg = "导入失败", erro = e });
            }
        }


        //校验重复生成会计凭证
        [HttpPost]
        [Route("CheckRepeatSettl")]
        public async Task<List<FM_NewCarryForwardVoucherRecordODataEntity>> CheckRepeatSettl(FM_CarryForwardVoucherSearchCommand request)
        {
            var data = await _provider.CheckRepeatSettl(request);
            return data;
        }
        /// <summary>
        /// 预收款核销接口  --仅结转使用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AdvanceCollectionDataList")]
        public async Task<List<AdvanceCollectionODataEnetity>> AdvanceCollectionDataList(FM_CarryForwardVoucherSearchCommand request)
        {
            var data = await _provider.AdvanceCollectionDataList(request);
            return data;
        }
        /// <summary>
        /// 税费抵扣结转  --仅结转使用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExpenseDataList")]
        public async Task<List<ExpenseODataEnetity>> ExpenseDataList(FM_CarryForwardVoucherSearchCommand request)
        {
            var data = await _provider.ExpenseDataList(request);
            return data;
        }
        [HttpPost]
        [Route("MakeVoucherPorc")]
        public async Task<Result> MakeVoucherPorc(FM_CarryForwardVoucherSearchCommand request)
        {
            try
            {
                AuthenticationHeaderValue authentication = null;
                bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
                if (!verification)
                {
                    return new Result() { code = 1, msg = "无身份权限" };
                }
                request.GroupID = _identityService.GroupId;
                var data = _provider.GetSingleData(Convert.ToInt64(request.NumericalOrder));
                if (data == null) return new Result() { code = 1, msg = request.NumericalOrder + "单据不存在" };
                List<Common.MakeVoucherCommon.EnterprisePeriod> periods = new List<Common.MakeVoucherCommon.EnterprisePeriod>();
                //获取会计期间
                periods = _comUtil.getEnterprisePeriod(new Common.MakeVoucherCommon.EnterprisePeriodSearch() { EnterpriseID = request.EnterpriseID, Year = Convert.ToDateTime(request.Enddate).Year, Month = Convert.ToDateTime(request.Enddate).Month }).Result;
                MakeVoucherBase _make = CreateMakeVoucherBaseMothed(data?.TransferAccountsType);
                if (_make == null)
                {
                    return new Result() { code = 1, msg = "请选择结转类别" };
                }
                var date = request.IsCurrentData ? DateTime.Now : (DateTime)periods.FirstOrDefault()?.EndDate;
                //验证科目摘要
                var misSubjectList =data?.Lines?.Where(p => !string.IsNullOrEmpty(p.AccoSubjectdBegin) && !string.IsNullOrEmpty(p.AccoSubjectdEnd) && (date < DateTime.Parse(p.AccoSubjectdBegin) || date > DateTime.Parse(p.AccoSubjectdEnd)))?.ToList();
                var misReceiptAbstractList = data?.Lines?.Where(p => !string.IsNullOrEmpty(p.ReceiptAbstractdBegin) && !string.IsNullOrEmpty(p.ReceiptAbstractdEnd) && (date < DateTime.Parse(p.ReceiptAbstractdBegin) || date > DateTime.Parse(p.ReceiptAbstractdEnd)))?.ToList();
                var errorMsg = string.Empty;

                if(misSubjectList?.Count > 0)
                {
                    if(misReceiptAbstractList?.Count > 0)
                    {
                        errorMsg = "模板会计科目/摘要与当前年度会计科目/摘要版本不一致";
                        //return new Result() { code = 1, msg = "模板会计科目/摘要与当前年度会计科目/摘要版本不一致" };
                    }
                    else
                    {
                        errorMsg = "模板会计科目与当前年度会计科目版本不一致";
                        //return new Result() { code = 1, msg = "模板会计科目与当前年度会计科目版本不一致" };
                    }
                }
                else if(misReceiptAbstractList?.Count > 0)
                {
                    errorMsg = "模板摘要与当前年度摘要版本不一致";
                    //return new Result() { code = 1, msg = "模板摘要与当前年度摘要版本不一致" };
                }
                FinanceManagement.Common.NewMakeVoucherCommon.FD_SettleReceipt result = _make?.getVoucherList(data, request, authentication);
                result.Lines = result.Lines?.Where(p => p.Debit != 0 || p.Credit != 0).ToList();
                result.DataDate = date;// request.IsCurrentData ? DateTime.Now : (DateTime)periods.FirstOrDefault()?.EndDate;
                result.CurrentEnterDate = request.CurrentEnterDate;
                result.EnterpriseID = _identityService.EnterpriseId;
                result.OwnerID = _identityService.UserId;
                result.CarryData = data;
                result.SettleReceipType = request.SettleReceipType;
                result.ErrorMsg = errorMsg;
                result.IsSettleCheckOut = _make.VerificationCheckout(request, 1611091727140000101, data?.TransferAccountsType);//会计凭证结账
                return await _mediator.Send(result, HttpContext.RequestAborted);
            }
            catch (Exception ex)
            {
                return new Result() { code = 1, msg = "生成失败请联系管理员", data = ex.ToString() };
                throw;
            }
        }

        private MakeVoucherBase CreateMakeVoucherBaseMothed(string TransferAccountsType)
        {
            MakeVoucherBase _make = null;
            switch (TransferAccountsType)
            {
                case "1911081429200099101"://销售结转
                    _make = new SaleSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099111"://采购结转
                    _make = new PurchaseSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099102"://薪资计提
                    var option = _baseUnit.OptionConfigValue("20221024095722", _identityService.EnterpriseId);
                    if (option != "0" && !string.IsNullOrEmpty(option))
                    {
                        _make = new SalaryDBNSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    }
                    else
                    {
                        _make = new SalarySummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    }
                    break;
                case "1911081429200099103"://福利计提
                    var option1 = _baseUnit.OptionConfigValue("20221024095722", _identityService.EnterpriseId);
                    if (option1 != "0" && !string.IsNullOrEmpty(option1))
                    {
                        _make = new WelfareDBNSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    }
                    else
                    {
                        _make = new WelfareSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    }
                    break;
                case "1911081429200099105"://禽成本结转
                    _make = new CheckenCostSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099106"://禽成本结转
                    _make = new CheckenCirculationCostSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099104"://猪成本结转
                    _make = new PigCostSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099107"://养户成本结转
                    _make = new BreederSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099108"://费用分摊结转
                    _make = new ShareCostCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099109"://损益结转
                    _make = new LossIncomeCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099110"://折旧计提
                    _make = new DepreciationCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099112"://物品结转
                    _make = new SuppliesSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099113"://预收款核销
                    _make = new AdvanceCollectionSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099114"://税费抵扣结转
                    _make = new ExpenseSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099115"://运费汇总表
                    _make = new FreightSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                case "1911081429200099116"://生产成本结转
                    _make = new ProductionCostSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                    break;
                default:
                    break;
            }
            return _make;
        }


        private List<BIZ_DataDictODataEntity> GetFormulaList(long transType, int tableType)
        {
            #region 公式信息
            List<BIZ_DataDictODataEntity> formulaList = new List<BIZ_DataDictODataEntity>();
            switch (transType)
            {
                case (long)TransferAccountsTypeEnum.销售结转:
                case (long)TransferAccountsTypeEnum.采购结转:
                case (long)TransferAccountsTypeEnum.福利计提:
                case (long)TransferAccountsTypeEnum.养户成本结转:
                case (long)TransferAccountsTypeEnum.禽成本蛋鸡结转:
                case (long)TransferAccountsTypeEnum.禽成本结转:
                case (long)TransferAccountsTypeEnum.损益结转:
                case (long)TransferAccountsTypeEnum.折旧计提:
                case (long)TransferAccountsTypeEnum.物品结转:
                case (long)TransferAccountsTypeEnum.预收款核销:
                case (long)TransferAccountsTypeEnum.税费抵扣:
                case (long)TransferAccountsTypeEnum.运费结转:
                case (long)TransferAccountsTypeEnum.生产成本结转:
                    formulaList.AddRange(_dictProvider.GetDataDictAsync(transType.ToString()).Result);//字典里面的公式
                    break;
                case (long)TransferAccountsTypeEnum.薪资计提:
                    formulaList.AddRange(GetSalarySetItemList());
                    break;
                case (long)TransferAccountsTypeEnum.费用分摊结转:
                case (long)TransferAccountsTypeEnum.猪成本结转:
                    formulaList.AddRange(_dictProvider.GetDataDictAsync(transType.ToString()).Result);//字典里面的公式
                    formulaList.AddRange(GetShareCostItemList(tableType));
                    break;
                default:
                    break;
            }
            formulaList.Add(new BIZ_DataDictODataEntity() { DataDictID = "2208081518000000151", DataDictName = "贷方合计" });
            formulaList.Add(new BIZ_DataDictODataEntity() { DataDictID = "2208081518000000152", DataDictName = "借方合计" });
            return formulaList;
            #endregion
        }
        private List<BIZ_DataDictODataEntity> GetSalarySetItemList()
        {
            AuthenticationHeaderValue authentication = null;
            bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
            if (!verification)
            {
                return null;
            }
            List<SalarySetItem> result = new List<SalarySetItem>();
            var option = OptionConfigValue("20221024095722");
            if (option)
            {
                result = new List<SalarySetItem>() { new SalarySetItem()
                {
                    SetItemId = "2111261612570000166",
                    SetItemName = "月收入"
                },new SalarySetItem()
                {
                    SetItemId = "2111261612570000188",
                    SetItemName = "绩效工资"
                }};
            }
            else
            {
                result = _baseUnit.GetSalarySetItemList(new Common.NewMakeVoucherCommon.FM_CarryForwardVoucherSearchCommand() { EnterpriseID = _identityService.EnterpriseId }, authentication);
            }
            result.Insert(0, new SalarySetItem() { SetItemId = "1612131431310000101", SetItemName = "薪资汇总表" });
            return result.Select(s => new BIZ_DataDictODataEntity() { DataDictID = s.SetItemId, DataDictName = "["+s.SetItemName+"]" }).ToList();
        }
        private List<BIZ_DataDictODataEntity> GetShareCostItemList(int tableType)
        {
            List<ShareCostItem> result = new List<ShareCostItem>();
            result = _baseUnit.GetShareCostItemList(new Common.NewMakeVoucherCommon.FM_CarryForwardVoucherSearchCommand() { EnterpriseID = _identityService.EnterpriseId });
            result.ForEach(s =>
            {
                s.CostProjectName = "["+s.CostProjectCode + s.CostProjectName+"]";
            });
            if (tableType == 1)
            {
                result.Insert(0, new ShareCostItem() { CostProjectId = "2201101409320000151", CostProjectName = "取值明细(费用分摊明细表)" });
            }
            else
            {
                result.Insert(0, new ShareCostItem() { CostProjectId = "2201101409320000152", CostProjectName = "分摊明细(费用分摊明细表)" });
            }
            return result.Select(s => new BIZ_DataDictODataEntity() { DataDictID = s.CostProjectId, DataDictName = s.CostProjectName }).ToList();
        }
        private bool OptionConfigValue(string optionId)
        {
            var result = _baseUnit.OptionConfigValue(optionId, _identityService.EnterpriseId);
            if (result != "0" && !string.IsNullOrEmpty(result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
