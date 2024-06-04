using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_AccountConfiguration : IEntityTypeConfiguration<FD_Account>
    {
        public void Configure(EntityTypeBuilder<FD_Account> entity)
        {

            entity.HasKey(e => e.AccountID)
            .HasName("PRIMARY");

            entity.Property(e => e.AccountID)
           .HasComment("账户ID")
           .AddStringToLongConvert();

            entity.Property(e => e.Guid)
           .HasComment("全球唯一关键字");

            entity.Property(e => e.AccountName)
           .HasComment("资金账户名称");

            entity.Property(e => e.AccountNumber)
            .HasComment("资金账号（银行账号）");

            entity.Property(e => e.AccountType)
            .HasComment("账户类型ID（字典表）")
            .AddStringToLongConvert();

            entity.Property(e => e.AccountFullName)
           .HasComment("账户全称");

            entity.Property(e => e.BankID)
           .HasComment("开户银行ID（字典表）")
           .AddStringToLongConvert();

            entity.Property(e => e.BankAreaID)
           .HasComment("银行地理区域ID")
           .AddStringToLongConvert();

            entity.Property(e => e.DepositBank)
           .HasComment("开户行");

            entity.Property(e => e.Address)
            .HasComment("详细地址");

            entity.Property(e => e.AccoSubjectID)
           .HasComment("会计科目")
           .AddStringToLongConvert();

            entity.Property(e => e.ExpenseAccoSubjectID)
           .HasComment("费用科目")
           .AddStringToLongConvert();


            entity.Property(e => e.AccountUseType)
          .HasComment("账户功能ID（字典表）")
          .AddStringToLongConvert();

            entity.Property(e => e.ResponsiblePerson)
           .HasComment("负责人")
           .AddStringToLongConvert();

            entity.Property(e => e.IsUse)
           .HasComment("状态");

            entity.Property(e => e.MarketID)
           .HasComment("部门")
           .AddStringToLongConvert();

            entity.Property(e => e.BankNumber)
           .HasComment("银行企业编号");

            entity.Property(e => e.Remarks)
           .HasComment("备注");

            entity.Property(e => e.OpenBankEnterConnect)
          .HasComment("开通银企连接");

            entity.Property(e => e.TubeAccountNumber)
           .HasComment("现管账号");

            entity.Property(e => e.OwnerID)
            .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");

            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");
        }
    }
}
