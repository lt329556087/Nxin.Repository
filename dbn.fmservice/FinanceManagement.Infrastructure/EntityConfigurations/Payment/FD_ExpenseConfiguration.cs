using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_ExpenseConfiguration : IEntityTypeConfiguration<FD_Expense>
    {
        public void Configure(EntityTypeBuilder<FD_Expense> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                .HasName("PRIMARY");

            entity.HasComment("费用报销");

            entity.HasIndex(e => e.DataDate)
                .HasName("idx_DataDate");

            entity.HasIndex(e => e.EnterpriseID)
                .HasName("idx_EnterpriseID");

            entity.HasIndex(e => e.NumericalOrder)
                .HasName("idx_NumericalOrder");

            entity.HasIndex(e => e.OwnerID)
                .HasName("idx_OwnerID");

            //entity.Property(e => e.RecordID)
            //    //.HasColumnType("int(11)")
            //    .HasComment("主键ID");


            entity.Property(e => e.NumericalOrder)
                //.HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.Guid)
              .HasComment("全球唯一关键字");
            //.HasCharSet("utf8")
            //.HasCollation("utf8_general_ci");

            entity.Property(e => e.ExpenseType)
            // .HasColumnType("bigint(20)")
            .HasComment("报销类型（应用ID）")
            .AddStringToLongConvert();

            entity.Property(e => e.ExpenseAbstract)
                //.HasColumnType("bigint(20)")
                .HasComment("报销摘要")
                .AddStringToLongConvert();

            entity.Property(e => e.ExpenseSort)
                //.HasColumnType("bigint(20)")
                .HasComment("单据分类（字典表）")
                .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
               // .HasColumnType("date")
               .HasComment("日期");

            entity.Property(e => e.CreatedDate)
                //.HasColumnType("datetime")
                .HasComment("创建日期");

            entity.Property(e => e.CurrentVerificationAmount)
                //.HasColumnType("decimal(18,2)")
                .HasComment("本次核销金额");

           
            entity.Property(e => e.DraweeID)
                //.HasColumnType("bigint(20)")
                .HasComment("付款人ID")
                .AddStringToLongConvert();

            entity.Property(e => e.EndDate)
                //.HasColumnType("date")
                .HasComment("结束日期");

            entity.Property(e => e.EnterpriseID)
                //.HasColumnType("bigint(20)")
                .HasComment("所属单位ID")
                .AddStringToLongConvert();

            entity.Property(e => e.HouldPayDate)
                //.HasColumnType("date")
                .HasComment("应付款日期（到期日）");

            entity.Property(e => e.ModifiedDate)
                //.HasColumnType("timestamp")
                //.HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后修改日期")
                .ValueGeneratedOnAddOrUpdate();


            entity.Property(e => e.OwnerID)
                //.HasColumnType("bigint(20)")
                .HasComment("制单人ID")
                .AddStringToLongConvert();

            entity.Property(e => e.PayDate)
                //.HasColumnType("date")
                .HasComment("付款日期");

            entity.Property(e => e.PersonID)
               // .HasColumnType("bigint(20)")
                .HasComment("负责人ID")
                .AddStringToLongConvert();

            entity.Property(e => e.Pressing)
                //.HasColumnType("int(11)")
                .HasComment("紧急 默认:0 0否 1是");

            entity.Property(e => e.Remarks)
               // .HasColumnType("varchar(2048)")
                .HasComment("备注");
            //.HasCharSet("utf8")
            // .HasCollation("utf8_general_ci");

            entity.Property(e => e.StartDate)
               // .HasColumnType("date")
                .HasComment("开始日期");


            entity.Property(e => e.Number)
                //.HasColumnType("bigint(20)")
                .HasComment("单据号")
                .AddStringToLongConvert();

            entity.Property(e => e.TicketedPointID)
                //.HasColumnType("bigint(20)")
                .HasComment("单据字")
                .AddStringToLongConvert();
        }
    }

    public class FD_ExpenseDetailConfiguration : IEntityTypeConfiguration<FD_ExpenseDetail>
    {
        public void Configure(EntityTypeBuilder<FD_ExpenseDetail> entity)
        {
            entity.HasKey(e => e.NumericalOrderDetail)
                .HasName("PRIMARY");

            entity.HasComment("费用报销明细");

            entity.HasIndex(e => e.CustomerID)
                .HasName("idx_CustomerID");

            entity.HasIndex(e => e.MarketID)
                .HasName("idx_MarketID");

            entity.HasIndex(e => e.NumericalOrder)
                .HasName("idx_NumericalOrder");

            entity.HasIndex(e => e.PersonID)
                .HasName("idx_PersonID");

            entity.Property(e => e.NumericalOrderDetail)
                .HasColumnType("bigint(20)")
                .HasComment("主键ID")
                .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrder)
               .HasColumnType("bigint(20)")
               .HasComment("流水号")
               .AddStringToLongConvert();

            entity.Property(e => e.AccountInformation)
                .HasColumnType("varchar(1024)")
                .HasComment("账户信息")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .HasComment("金额");

            entity.Property(e => e.BusinessType)
                .HasColumnType("bigint(20)")
                .HasComment("支付类型（字典表）")
                .AddStringToLongConvert();

            entity.Property(e => e.Content)
                .HasColumnType("varchar(1024)")
                .HasComment("内容")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");

            entity.Property(e => e.CustomerID)
                .HasColumnType("bigint(20)")
                .HasComment("客户/供应商ID")
                .AddStringToLongConvert();

            entity.Property(e => e.Guid)
                .HasComment("全球唯一关键字")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");

            entity.Property(e => e.MarketID)
                .HasColumnType("bigint(20)")
                .HasComment("部门ID")
                .AddStringToLongConvert();
           
            entity.Property(e => e.PersonID)
                .HasColumnType("bigint(20)")
                .HasComment("员工ID")
                .AddStringToLongConvert();

            entity.Property(e => e.ProjectID)
                .HasColumnType("bigint(20)")
                .HasComment("项目ID")
                .AddStringToLongConvert();

            entity.Property(e => e.ReceiptAbstractDetail)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("类别明细ID（摘要表）")
                .AddStringToLongConvert();

            entity.Property(e => e.ReceiptAbstractID)
                .HasColumnType("bigint(20)")
                .HasComment("类别ID（摘要表）")
                .AddStringToLongConvert();

            entity.Property(e => e.SettleBusinessType)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("(付款)支付类型（字典表）")
                .AddStringToLongConvert();

            entity.Property(e => e.SettlePayerID)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("结算id")
                .AddStringToLongConvert();
        }
    }

    public class FD_ExpenseExtConfiguration : IEntityTypeConfiguration<FD_ExpenseExt>
    {
        public void Configure(EntityTypeBuilder<FD_ExpenseExt> entity)
        {
            entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

            entity.HasComment("申请收方信息");

            entity.HasIndex(e => e.CollectionId)
                .HasName("CollectionId_Index");

            entity.HasIndex(e => e.NumericalOrder)
                .HasName("NumericalOrder_Index");

            entity.HasIndex(e => e.NumericalOrderDetail)
                .HasName("NumericalOrderDetail_Index");

            entity.Property(e => e.RecordID)
                .HasColumnType("int(11)")
                .HasComment("主键ID");

            entity.Property(e => e.AccountName)
                .HasColumnType("varchar(255)")
                .HasComment("账户信息")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .HasComment("收款金额");

            entity.Property(e => e.BankAccount)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("银行账户")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.BankDeposit)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("开户银行")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.CollectionId)
                .HasColumnType("bigint(20)")
                .HasComment("收款单位")
                .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.PersonId)
                .HasColumnType("bigint(20)")
                .HasComment("收款人")
                .AddStringToLongConvert();
        }
    }
}
