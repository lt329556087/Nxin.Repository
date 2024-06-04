using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;


namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_CashSweepConfiguration : IEntityTypeConfiguration<FM_CashSweep>
    {
        public void Configure(EntityTypeBuilder<FM_CashSweep> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                   .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder).HasComment("流水号").AddStringToLongConvert(); 

            entity.Property(e => e.EnterpriseID).HasComment("单位ID").AddStringToLongConvert();

            entity.Property(e => e.DataDate).HasComment("单据日期");

            entity.Property(e => e.Number).HasComment("单据号").AddStringToLongConvert();

            entity.Property(e => e.AccountID)
                .HasComment("归集账号")
                .AddStringToLongConvert();

            entity.Property(e => e.SweepDirectionID)
               .HasDefaultValueSql("'0'")
               .HasComment("归集方向")
               .AddStringToLongConvert();

            entity.Property(e => e.SweepType)
                .HasComment("归集类型")
                .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");
           

            entity.Property(e => e.ExcuteDate).HasComment("归集执行日期");

            entity.Property(e => e.ExcuterID).HasComment("归集执行人ID");//.AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期")
                .ValueGeneratedOnAddOrUpdate();

           

            entity.Property(e => e.OwnerID).HasComment("制单人ID").AddStringToLongConvert();

          
            entity.Property(e => e.Remarks)
                .HasDefaultValueSql("''")
                .HasComment("备注")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");


            entity.Property(e => e.CollectionScheme)
                .HasComment("归集方案");

            entity.Property(e => e.IsUse)
               .HasComment("自动归集方案是否启用");


            entity.Property(e => e.AutoTime)
               //.HasColumnType("time")
               .HasComment("自动归集时间");

            entity.Property(e => e.SchemeType)
              .HasComment("归集类型");

            entity.Property(e => e.SchemeAmount)
              .HasComment("方案金额");

            entity.Property(e => e.Rate)
              .HasComment("方案比例");

            entity.Property(e => e.PlanType)
              .HasComment("资金计划类型");

            entity.Property(e => e.SchemeFormula)
              .HasComment("归集公式");

            entity.Property(e => e.SchemeTypeName)
              .HasComment("归集方案类型名称");

            entity.Property(e => e.IsNew)
              .HasComment("是否新菜单");

            entity.Property(e => e.JobID)
             .HasComment("定时任务ID");

            entity.HasMany(o => o.Lines)
               .WithOne(o => o.FM_CashSweep)
               .HasForeignKey(o => o.NumericalOrder)
               .IsRequired();

    }
    }

    public class FM_CashSweepDetailConfiguration : IEntityTypeConfiguration<FM_CashSweepDetail>
    {
        public void Configure(EntityTypeBuilder<FM_CashSweepDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
                     .HasName("PRIMARY");

            entity.Property(e => e.RecordID).HasComment("主键");

            entity.Property(e => e.NumericalOrder)
           .HasComment("流水号")
           .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("明细流水号")
            .AddStringToLongConvert();


            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccountID)
            .HasComment("账户ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccountBalance)
            .HasDefaultValueSql("'0.00'")
            .HasComment("账户余额");

            entity.Property(e => e.OtherAccountBalance)
               .HasDefaultValueSql("'0.00'")
               .HasComment("其他账户余额");

            entity.Property(e => e.TheoryBalance)
               .HasDefaultValueSql("'0.00'")
               .HasComment("理论额度");

            entity.Property(e => e.TransformBalance)
                .HasDefaultValueSql("'0.00'")
                .HasComment("调整金额");


            entity.Property(e => e.AutoSweepBalance)
                .HasDefaultValueSql("'0.00'")
                .HasComment("自动归集金额");

            entity.Property(e => e.ManualSweepBalance)
              .HasDefaultValueSql("'0.00'")
              .HasComment("手动归集金额");

            entity.Property(e => e.Remark)
             .HasDefaultValueSql("''")
             .HasComment("备注")
             .HasCharSet("utf8mb4")
             .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.Status)
                .HasComment("归集执行状态");

            entity.Property(e => e.ExcuteMsg)
                .HasComment("归集执行消息")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

          

            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期")
                .ValueGeneratedOnAddOrUpdate();

           
            //entity.Property(e => e.AccountTransferAbstract)
            //    .HasDefaultValueSql("'0'")
            //    .HasComment("调拨类型(新)");

            //entity.Property(e => e.PaymentAbstractID)
            //    .HasDefaultValueSql("'0'")
            //    .HasComment("付款摘要(新)");

            //entity.Property(e => e.PaymentContent)
            //    .HasComment("付款内容(新)")
            //    .HasCharSet("utf8mb4")
            //    .HasCollation("utf8mb4_general_ci");

            //entity.Property(e => e.PaymentTicketedPointID)
            //    .HasDefaultValueSql("'0'")
            //    .HasComment("付款单据字(新)");

            //entity.Property(e => e.PaymentTypeID)
            //    .HasDefaultValueSql("'0'")
            //    .HasComment("结算方式(新)");

            //entity.Property(e => e.ReceiptAbstractID)
            //    .HasDefaultValueSql("'0'")
            //    .HasComment("收款摘要(新)");

            //entity.Property(e => e.ReceiveTicketedPointID)
            //    .HasDefaultValueSql("'0'")
            //    .HasComment("收款单单据字(新)");

            //entity.Property(e => e.ReceiveTypeID)
            //    .HasDefaultValueSql("'0'")
            //    .HasComment("收款方式(新)");



            //entity.Property(e => e.TransferOrigin)
            //    .HasComment("调拨事由(新)")
            //    .HasCharSet("utf8mb4")
            //    .HasCollation("utf8mb4_general_ci");


        }
    }

    public class fd_settlereceiptextendConfiguration : IEntityTypeConfiguration<fd_settlereceiptextend>
    {
        public void Configure(EntityTypeBuilder<fd_settlereceiptextend> entity)
        {
           entity.HasKey(e => e.RecordID)
                     .HasName("PRIMARY");

            entity.Property(e => e.RecordID).HasComment("主键");

            entity.Property(e => e.NumericalOrder)
           .HasComment("流水号")
           .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("明细流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.AccountName)
         .HasComment("资金账户名称");

            entity.Property(e => e.AccountNumber)
            .HasComment("资金账号（银行账号）");

            entity.Property(e => e.BankID)
           .HasComment("开户银行ID（字典表）")
           .AddStringToLongConvert();
                      
            entity.Property(e => e.DepositBank)
           .HasComment("开户行");

            entity.Property(e => e.CreatedDate)
              .HasComment("创建日期");

            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期")
                .ValueGeneratedOnAddOrUpdate();
        }
    }

    public class FM_CashSweepLogConfiguration : IEntityTypeConfiguration<FM_CashSweepLog>
    {
        public void Configure(EntityTypeBuilder<FM_CashSweepLog> entity)
        {
            entity.HasKey(e => e.RecordID)
                      .HasName("PRIMARY");

            entity.Property(e => e.RecordID).HasComment("主键");

            entity.Property(e => e.NumericalOrder)
           .HasComment("流水号")
           .AddStringToLongConvert();

            entity.Property(e => e.BatchNo)
            .HasComment("批次号")
            .AddStringToLongConvert();

            entity.Property(e => e.ReturnResult)
            .HasComment("返回前端结果");

            entity.Property(e => e.ResponseResult)
            .HasComment("接口返回结果");

            entity.Property(e => e.RequestResult)
           .HasComment("请求参数");

            entity.Property(e => e.Remarks)
           .HasComment("备注");

            entity.Property(e => e.BusType)
           .HasComment("业务类型 1：交易 2：消息通知 0：返回值");

            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期")
                .ValueGeneratedOnAddOrUpdate();
        }
    }

}
