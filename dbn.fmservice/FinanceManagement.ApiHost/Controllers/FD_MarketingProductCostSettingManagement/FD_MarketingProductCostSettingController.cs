using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Commands;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceiptInterface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement
{
    /// <summary>
    /// 营销商品成本设定
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_MarketingProductCostSettingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly FD_MarketingProductCostSettingODataProvider _queryProvider;

        public FD_MarketingProductCostSettingController(IMediator mediator, FD_MarketingProductCostSettingODataProvider queryProvider)
        {
            _mediator = mediator;
            _queryProvider = queryProvider;
        }

        /// <summary>
        /// 根据流水号获取详情
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        [HttpGet("{numericalOrder}")]
        public async Task<Result> GetDetail(string numericalOrder)
        {
            var result = new Result()
            {
                code = 0
            };
            var data = await _queryProvider.GetByNumericalOrder(numericalOrder);
            result.data = data;

            return result;
        }

        /// <summary>
        /// 根据账务单位和日期获取信息
        /// </summary>
        /// <param name="accountingEnterpriseID"></param>
        /// <param name="dataDate"></param>
        /// <returns></returns>
        [HttpGet("load")]
        public async Task<Result> Load([FromQuery] string accountingEnterpriseID, [FromQuery] DateTime dataDate)
        {
            return await _queryProvider.Load(accountingEnterpriseID, dataDate);
        }

        /// <summary>
        /// 根据流水号重新获取信息
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        [HttpGet("reload")]
        public async Task<Result> Reload([FromQuery] string numericalOrder)
        {
            return await _queryProvider.Reload(numericalOrder);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result> Add([FromBody] FD_MarketingProductCostSettingCreateCommand cmd)
        {
            return await _mediator.Send(cmd);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<Result> Update([FromBody] FD_MarketingProductCostSettingUpdateCommand cmd)
        {
            return await _mediator.Send(cmd);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<Result> Update([FromBody] FD_MarketingProductCostSettingDeleteCommand cmd)
        {
            return await _mediator.Send(cmd);
        }


        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Export")]
        public async Task<ActionResult> Export([FromQuery] string numericalOrder)
        {
            //获取凭证数据
            var list = await _queryProvider.GetFullList(numericalOrder);

            if (!list.Any())
            {
                return new EmptyResult();
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // 创建 Excel 文件
            using (var package = new ExcelPackage())
            {
                var groupdList = list.GroupBy(c => c.NumericalOrder);

                foreach (var item in groupdList)
                {
                    var first = item.First();
                    var childList = item.ToList();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(first.Number);
                    worksheet.DefaultColWidth = 15;
                    worksheet.Cells[1, 1, 1, 22].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1, 1, 22].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 217, 217, 217));

                    #region 表头
                    worksheet.Cells[1, 1].Value = "账务单位";
                    worksheet.Cells[1, 2].Value = "日期";
                    worksheet.Cells[1, 3].Value = "单据号";
                    worksheet.Cells[1, 4].Value = "商品代号";
                    worksheet.Cells[1, 5].Value = "计量单位";
                    worksheet.Cells[1, 6].Value = "预测单位成本";
                    worksheet.Cells[1, 7].Value = "当期计算成本";
                    worksheet.Cells[1, 8].Value = "差异值";
                    worksheet.Cells[1, 9].Value = "制单人";
                    worksheet.Cells[1, 10].Value = "审核人";
                    worksheet.Cells[1, 11].Value = "备注";
                    #endregion 
                    int startRowNum = 2;

                    foreach (var childItem in childList)
                    {
                        worksheet.Cells[startRowNum, 1].Value = childItem.AccountingEnterpriseName;
                        worksheet.Cells[startRowNum, 2].Value = childItem.DataDate.ToString("yyyy-MM-dd");
                        worksheet.Cells[startRowNum, 3].Value = childItem.Number;
                        worksheet.Cells[startRowNum, 4].Value = childItem.ProductName;
                        worksheet.Cells[startRowNum, 5].Value = childItem.MeasureUnitNanme;
                        worksheet.Cells[startRowNum, 6].Value = childItem.ForecastUnitCost;
                        worksheet.Cells[startRowNum, 7].Value = childItem.CurrentCalcCost;
                        worksheet.Cells[startRowNum, 8].Value = childItem.ForecastAndCurrentDiff;
                        worksheet.Cells[startRowNum, 9].Value = childItem.OwnerName;
                        worksheet.Cells[startRowNum, 10].Value = childItem.AuditUserName;
                        worksheet.Cells[startRowNum, 11].Value = childItem.Remarks;

                        startRowNum++;
                    }

                    foreach (var items in item)
                    {
                        //i=2 不需要前两列的数据
                        for (int i = 3; i < items.GetType().GetProperties().Count(); i++)
                        {
                            worksheet.Cells[startRowNum, i - 2].Value = items.GetType().GetProperty(items.GetType().GetProperties()[i].Name).GetValue(items);
                        }
                        startRowNum++;
                    }
                }

                // 生成 Excel 文件流
                var stream = new MemoryStream(package.GetAsByteArray());

                // 设置响应头信息，告诉浏览器下载文件
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //命名excel名称  防止ASCII 报错
                var contentDisposition = new ContentDisposition
                {
                    FileName = $"营销商品成本设定.xlsx",
                    Inline = false
                };
                Response.Headers.Add("content-disposition", contentDisposition.ToString());

                // 将文件流输出到 Response 中，供前端下载
                await stream.CopyToAsync(Response.Body);
                stream.Close();
            }


            return new EmptyResult();
        }
    }
}
