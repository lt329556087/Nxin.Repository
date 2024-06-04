using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_CarryForwardVoucherConfiguration : IEntityTypeConfiguration<FM_CarryForwardVoucher>
    {
        public void Configure(EntityTypeBuilder<FM_CarryForwardVoucher> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.TransferAccountsType)
            .HasComment("结转类别")
            .AddStringToLongConvert();

            entity.Property(e => e.Number)
           .HasComment("单据号")
           .AddStringToLongConvert();

            entity.Property(e => e.TicketedPointID)
            .HasComment("单据字")
            .AddStringToLongConvert();

            entity.Property(e => e.DataSource)
            .HasComment("来源数据")
            .AddStringToLongConvert();

            entity.Property(e => e.TransferAccountsAbstract)
            .HasComment("业务摘要")
            .AddStringToLongConvert();

            entity.Property(e => e.TransferAccountsSort)
             .HasComment("凭证方案")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
             .HasComment("备注");

            entity.Property(e => e.SettleNumber)
            .HasComment("凭证序号");

            entity.Property(e => e.OwnerID)
             .HasComment("制单人ID")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.TransactorID)
            .HasComment("执行人")
            .AddStringToLongConvert();

            entity.Property(e => e.TransactorDate)
          .HasComment("执行时间");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");
            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.FM_CarryForwardVoucher)
            .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();
        }
    }

    public class FM_CarryForwardVoucherDetailConfiguration : IEntityTypeConfiguration<FM_CarryForwardVoucherDetail>
    {
        public void Configure(EntityTypeBuilder<FM_CarryForwardVoucherDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
          .HasComment("明细流水号")
          .AddStringToLongConvert();

            entity.Property(e => e.ReceiptAbstractID)
            .HasComment("结算摘要")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectCode)
            .HasComment("会计科目");

            entity.Property(e => e.AccoSubjectID)
            .HasComment("会计科目ID")
            .AddStringToLongConvert();

            entity.Property(e => e.IsPerson)
            .HasComment("是否人员核算");

            entity.Property(e => e.IsCustomer)
            .HasComment("是否客商核算");

            entity.Property(e => e.IsMarket)
            .HasComment("是否部门核算");

            entity.Property(e => e.IsProduct)
            .HasComment("是否商品核算");

            entity.Property(e => e.IsPigFram)
            .HasComment("是否猪场核算");

            entity.Property(e => e.IsProject)
            .HasComment("是否项目核算");

            entity.Property(e => e.IsSum)
            .HasComment("是否汇总核算");

            entity.Property(e => e.DebitFormula)
            .HasComment("借方公式");

            entity.Property(e => e.DebitSecFormula)
            .HasComment("借方公式信息");

            entity.Property(e => e.CreditFormula)
            .HasComment("贷方公式");

            entity.Property(e => e.CreditSecFormula)
            .HasComment("贷方公式信息");
           

            entity.Property(e => e.ModifiedDate)
          .HasComment("最后修改日期");

        }
    }


    public class FM_CarryForwardVoucherExtendConfiguration : IEntityTypeConfiguration<FM_CarryForwardVoucherExtend>
    {
        public void Configure(EntityTypeBuilder<FM_CarryForwardVoucherExtend> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("记录标识");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
          .HasComment("明细流水号")
          .AddStringToLongConvert();

            entity.Property(e => e.Sort)
            .HasComment("表体行扩展标识");

            entity.Property(e => e.Symbol)
         .HasComment("符号");

            entity.Property(e => e.Object)
            .HasComment("选择清单")
            .AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
          .HasComment("最后修改日期");

        }
    }
    public class FM_CarryForwardVoucherFormulaConfiguration : IEntityTypeConfiguration<FM_CarryForwardVoucherFormula>
    {
        public void Configure(EntityTypeBuilder<FM_CarryForwardVoucherFormula> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("记录标识");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
          .HasComment("明细流水号")
          .AddStringToLongConvert();

            entity.Property(e => e.RowNum)
          .HasComment("序号");

            entity.Property(e => e.Bracket)
            .HasComment("括号标识");

            entity.Property(e => e.FormulaID)
            .HasComment("取数来源")
            .AddStringToLongConvert();

            entity.Property(e => e.Operator)
            .HasComment("符号标识");

            entity.Property(e => e.ModifiedDate)
          .HasComment("最后修改日期");

        }
    }
}

