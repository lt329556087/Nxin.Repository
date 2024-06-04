using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_Account;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Seedwork.Security;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.Repositories.Interfaces;

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AccountController : ControllerBase
    {
        IMediator _mediator;
        FD_AccountODataProvider _provider;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _baseUnit;
        IBiz_Related _biz_RelatedRepository;
        public FD_AccountController(IMediator mediator, FD_AccountODataProvider provider, HostConfiguration hostCongfiguration, FMBaseCommon baseUnit, IBiz_Related biz_RelatedRepository)
        {
            _mediator = mediator;
            _provider = provider;
            _baseUnit = baseUnit;
            _hostCongfiguration = hostCongfiguration;
            _biz_RelatedRepository = biz_RelatedRepository;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        [PermissionAuthorize(Permission.Retrieve)]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetDataAsync(key);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.AccountNumber))
                {
                    EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                    //解密银行卡号
                    var resultNum = encryptAccount.AccountNumberDecrypt(data.AccountNumber);
                    data.AccountNumber = resultNum?.Item1 == true ? resultNum.Item2 : data.AccountNumber;
                }
                //账户用途 西煤支付 { RelatedType = 201610210104402102, ParentType = 1812271025360000100, ChildType = 1611031642370000101, ParentValue = i, ChildValue = account.ToString(), Remarks = "资金账户设置引用字典表" };
                var param = new Domain.BIZ_Related()
                {
                    RelatedType = "201610210104402102",
                    ParentType = "1812271025360000100",
                    ChildType = "1611031642370000101",
                    ChildValue = data.AccountID
                };
                var relateds =await _biz_RelatedRepository.GetRelated(param);
                if (relateds != null && relateds.Count > 0)
                {
                    data.PurposeList = relateds.Select(p => p.ParentValue)?.ToList();
                }
            }
            result.data = data;
            return result;
        }
        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_AccountAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Delete([FromBody] FD_AccountDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FD_AccountModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        #region 资金账户是否已用
        [HttpGet("IsUsed")]
        public Result IsAccountUse(string AccountID)
        {
            var result = new Result();
            if (string.IsNullOrEmpty(AccountID))
            {
                result.msg = "AccountID空";
                return result;
            }
            var data = _baseUnit.IsAccountUse(AccountID);
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
        #endregion
        #region 开启银企直连的账户
        [HttpPost("GetBankAccountList")]
        public Result GetBankAccountList([FromBody] FD_AccountAODataEntity model)
        {
            var result = new Result();
            var data = _provider.GetBankAccountList(model);
            foreach (var item in data)
            {
                if (string.IsNullOrEmpty(item.AccountNumber)) continue;
                EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                //解密银行卡号
                var resultNum = encryptAccount.AccountNumberDecrypt(item.AccountNumber);
                item.AccountNumber = resultNum?.Item1 == true ? resultNum.Item2 : item.AccountNumber;
                item.BankType =_baseUnit.ConvertBankType(item.BankID);
            }
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
        //[NonAction]
        //public string ConvertBankType(string bankID)
        //{
        //    var bankType = "";
        //    if (bankID == ((long)FMSettleReceipType.CEBBankDicID).ToString())
        //    {
        //        bankType = "CEB";
        //    }
        //    else if (bankID == ((long)FMSettleReceipType.ABCBankDicID).ToString())
        //    {
        //        bankType = "ABC";
        //    }
        //    else if (bankID == ((long)FMSettleReceipType.MinShengBankDicID).ToString())
        //    {
        //        bankType = "CMBC";
        //    }
        //    else if (bankID == ((long)FMSettleReceipType.ICBCBankDicID).ToString())
        //    {
        //        bankType = "ICBC";
        //    }
        //    else if (bankID == ((long)FMSettleReceipType.PSBCBankDicID).ToString())
        //    {
        //        bankType = "PSBC";
        //    }
        //    else if (bankID == ((long)FMSettleReceipType.ADBCBankDicID).ToString())//农业发展
        //    {
        //        bankType = "ADBC";
        //    }
        //    else if (bankID == ((long)FMSettleReceipType.BCMBankDicID).ToString())
        //    {
        //        bankType = "BCM";
        //    }
        //    return bankType;
        //}
        #endregion
    }

}
