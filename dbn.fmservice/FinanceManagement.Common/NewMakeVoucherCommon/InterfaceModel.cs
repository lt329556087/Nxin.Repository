using FinanceManagement.Common.MonthEndCheckout;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    public class PigCirculation
    {
        /// <summary>
        /// 
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 肇州二场公猪组
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigFarmId { get; set; }
        /// <summary>
        /// 肇州二场
        /// </summary>
        public string PigFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductLineId { get; set; }
        /// <summary>
        /// 肇州二场公猪组
        /// </summary>
        public string ProductLineName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 生产公猪
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BegdecimalotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FallbackInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FallbackInAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInAmount { get; set; }
        public decimal AllocateInTotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocatedecimalotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInAmount { get; set; }
        public decimal BatchInTotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchdecimalotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutTotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutTotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutTotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutTotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutTotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigBatchDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndTotalDepreciation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndSemen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigBatchDeath { get; set; }

        /// <summary>
        /// 待配转入原值
        /// </summary>
        public decimal ReadyBreedingInAmount { get; set; }
        /// <summary>
        /// 待配转入累计折旧
        /// </summary>
        public decimal ReadyBreedingInTotalDepreciation { get; set; }
        /// <summary>
        /// 失配转入原值
        /// </summary>
        public decimal MissBreedingInAmount { get; set; }
        /// <summary>
        /// 失配转入累计折旧
        /// </summary>
        public decimal MissBreedingInTotalDepreciation { get; set; }
        /// <summary>
        /// 断奶转入原值
        /// </summary>
        public decimal WeaningInAmount { get; set; }
        /// <summary>
        /// 断奶转入累计折旧
        /// </summary>
        public decimal WeaningInTotalDepreciation { get; set; }
        /// <summary>
        /// 待配转出原值
        /// </summary>
        public decimal ReadyBreedingOutAmount { get; set; }
        /// <summary>
        /// 待配转出累计折旧
        /// </summary>
        public decimal ReadyBreedingOutTotalDepreciation { get; set; }
        /// <summary>
        /// 失配转出原值
        /// </summary>
        public decimal MissBreedingOutAmount { get; set; }
        /// <summary>
        /// 失配转出累计折旧
        /// </summary>
        public decimal MissBreedingOutTotalDepreciation { get; set; }
        /// <summary>
        /// 哺乳母猪转出原值
        /// </summary>
        public decimal LactatingOutAmount { get; set; }
        /// <summary>
        /// 哺乳母猪转出累计折旧
        /// </summary>
        public decimal LactatingOutTotalDepreciation { get; set; }
        /// <summary>
        /// 哺乳母猪转出种猪死亡净值
        /// </summary>
        public decimal LactatingOutPigDeathValue { get; set; }
    }
    public class SucklingPigCirculation
    {
        /// <summary>
        /// 
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 肇州二场分娩组1
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigFarmId { get; set; }
        /// <summary>
        /// 肇州二场
        /// </summary>
        public string PigFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductLineId { get; set; }
        /// <summary>
        /// 肇州二场分娩组1
        /// </summary>
        public string ProductLineName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 仔猪
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigType { get; set; }
        /// <summary>
        /// 乳猪
        /// </summary>
        public string PigTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthInPigletCost { get; set; }
        public decimal LactatingInTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingdecimalransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchOutsideDeath { get; set; }
        public decimal SaleInsideInTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsidedecimalransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInBatchInsideDeath { get; set; }
        public decimal SaleOutsideInTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsidedecimalransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigTransferValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigDeathValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchOutsideDeath { get; set; }
    }
    public class GrowCirculation
    {
        /// <summary>
        /// 
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 肇州二场保育段
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigFarmId { get; set; }
        /// <summary>
        /// 肇州二场
        /// </summary>
        public string PigFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductLineId { get; set; }
        /// <summary>
        /// 肇州二场保育段
        /// </summary>
        public string ProductLineName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 保育猪
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigType { get; set; }
        /// <summary>
        /// 保育
        /// </summary>
        public string PigTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBreederInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBreederInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchOutsideDeath { get; set; }
    }
    public class FallbackPigCirculation
    {
        /// <summary>
        /// 
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 肇州二场配怀组1
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigFarmId { get; set; }
        /// <summary>
        /// 肇州二场
        /// </summary>
        public string PigFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductLineId { get; set; }
        /// <summary>
        /// 肇州二场配怀组1
        /// </summary>
        public string ProductLineName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 后备母猪
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigType { get; set; }
        /// <summary>
        /// 后备母猪
        /// </summary>
        public string PigTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBreederInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBreederInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchOutsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigletCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndFeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVeterinaryDrug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOtherMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchInsideDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchOutsideDeath { get; set; }
    }

    public class FeedCostCirculation
    {
        /// <summary>
        /// 
        /// </summary>
        public string DeptID { get; set; }
        /// <summary>
        /// 肇州二场分娩组1
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigFarmID { get; set; }
        /// <summary>
        /// 肇州二场
        /// </summary>
        public string PigFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductLineID { get; set; }
        /// <summary>
        /// 肇州二场分娩组1
        /// </summary>
        public string ProductLineName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 仔猪
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 乳猪
        /// </summary>
        public string BatchTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EarNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FeedDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Depreciated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MaterialCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MedicantCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal VaccineCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OtherCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FeeCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SumCost { get; set; }
    }

    public class PigCirculationSummary
    {

        /// <summary>
        /// 
        /// </summary>
        public string FarmerID { get; set; }
        /// <summary>
        /// 永乐服务部
        /// </summary>
        public string FarmerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 肇州二场公猪组
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigFarmId { get; set; }
        /// <summary>
        /// 肇州二场
        /// </summary>
        public string PigFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductLineId { get; set; }
        /// <summary>
        /// 肇州二场公猪组
        /// </summary>
        public string ProductLineName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 生产公猪
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PigTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BegdecimalotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FallbackInCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FallbackInAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingInPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingInPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInAmount1 { get; set; }
        public decimal AllocateInTotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocatedecimalotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInAmount1 { get; set; }
        public decimal BatchInTotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchdecimalotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthOutPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReadyBreedingOutPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MissBreedingOutPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutTotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutTotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutTotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutTotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutTotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigBatchDeath1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndAmount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndTotalDepreciation1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndFeed1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVeterinaryDrug1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccin1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOtherMaterial1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigTransferValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndSemen1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigDeathValue1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigBatchDeath1 { get; set; }

        /// <summary>
        /// 待配转入原值
        /// </summary>
        public decimal ReadyBreedingInAmount1 { get; set; }
        /// <summary>
        /// 待配转入累计折旧
        /// </summary>
        public decimal ReadyBreedingInTotalDepreciation1 { get; set; }
        /// <summary>
        /// 失配转入原值
        /// </summary>
        public decimal MissBreedingInAmount1 { get; set; }
        /// <summary>
        /// 失配转入累计折旧
        /// </summary>
        public decimal MissBreedingInTotalDepreciation1 { get; set; }
        /// <summary>
        /// 断奶转入原值
        /// </summary>
        public decimal WeaningInAmount1 { get; set; }
        /// <summary>
        /// 断奶转入累计折旧
        /// </summary>
        public decimal WeaningInTotalDepreciation1 { get; set; }
        /// <summary>
        /// 待配转出原值
        /// </summary>
        public decimal ReadyBreedingOutAmount1 { get; set; }
        /// <summary>
        /// 待配转出累计折旧
        /// </summary>
        public decimal ReadyBreedingOutTotalDepreciation1 { get; set; }
        /// <summary>
        /// 失配转出原值
        /// </summary>
        public decimal MissBreedingOutAmount1 { get; set; }
        /// <summary>
        /// 失配转出累计折旧
        /// </summary>
        public decimal MissBreedingOutTotalDepreciation1 { get; set; }
        /// <summary>
        /// 哺乳母猪转出原值
        /// </summary>
        public decimal LactatingOutAmount1 { get; set; }
        /// <summary>
        /// 哺乳母猪转出累计折旧
        /// </summary>
        public decimal LactatingOutTotalDepreciation1 { get; set; }
        /// <summary>
        /// 哺乳母猪转出种猪死亡净值
        /// </summary>
        public decimal LactatingOutPigDeathValue1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthInCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ChildbirthInPigletCost2 { get; set; }
        public decimal LactatingInTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingdecimalransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal LactatingInPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchOutsideDeath2 { get; set; }
        public decimal SaleInsideInTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsidedecimalransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideInBatchInsideDeath2 { get; set; }
        public decimal SaleOutsideInTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsidedecimalransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideInBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutBatchOutsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigletCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndFeed2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVeterinaryDrug2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccin2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOtherMaterial2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigTransferValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigDeathValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchInsideDeath2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchOutsideDeath2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepInCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepInPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBreederInCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBreederInPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EmergenceOutBatchOutsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigletCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndFeed3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVeterinaryDrug3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccin3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOtherMaterial3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchInsideDeath3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchOutsideDeath3 { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepInCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepInPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InsidePurchaseInPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutPurchaseInPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBreederInCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBreederInPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchInBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateInBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentCostCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AcrossStepOutBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BatchOutBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AllocateOutBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOutBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleInsideOutBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaleOutsideOutBatchOutsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigletCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndFeed4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVeterinaryDrug4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccin4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOtherMaterial4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchInsideDeath4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBatchOutsideDeath4 { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int FeedDays5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Depreciated5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MaterialCost5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MedicantCost5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal VaccineCost5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OtherCost5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FeeCost5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SumCost5 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal AdjustCount6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigCount6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigCost6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginMaterial6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginMedicant6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccine6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOther6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginInnerDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOuterDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginDirectFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBuildingFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPredictFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginAdjustFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginAdditionFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginHoldUpFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentPigCount6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentPigCost6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentMaterial6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentMedicant6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentVaccine6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentOther6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentInnerDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentOuterDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentDirectFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentBuildingFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentPredictFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentAdjustFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentAdditionFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentHoldUpFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecyclePigCount6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecyclePigCost6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleMaterial6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleMedicant6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleVaccine6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleOther6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleInnerDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleOuterDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleDirectFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBuildingFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecyclePredictFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleAdjustFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleAdditionFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleHoldUpFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathPigCount6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathPigCost6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathMaterial6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathMedicant6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathVaccine6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOther6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInnerDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOuterDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathDirectFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathBuildingFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathPredictFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathAdjustFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathAdditionFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathHoldUpFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigCount6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigCost6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndMaterial6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndMedicant6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccine6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOther6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndInnerDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOuterDeath6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndDirectFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBuildingFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPredictFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndAdjustFeedFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndAdditionFee6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndHoldUpFee6 { get; set; }

    }
    public class PigSearchModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EnterpriseId { get; set; }
        public List<string> ProductIdList { get; set; } = new List<string>();
        public List<string> DeptIdList { get; set; } = new List<string>();
        public List<string> PigTypeIdList { get; set; } = new List<string>();
        public List<string> PigFarmIdList { get; set; } = new List<string>();
        public List<string> Summary { get; set; } = new List<string>();
        public List<string> SummaryName { get; set; } = new List<string>();
    }
    public class GetCheckenCirculationCostModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ChickenFarmID { get; set; }
        /// <summary>
        /// 禽场1
        /// </summary>
        public string ChickenFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ZoningID { get; set; }
        /// <summary>
        /// 场1
        /// </summary>
        public string ZoningName { get; set; }
        /// <summary>
        /// 育雏栋舍1
        /// </summary>
        public string HenHouseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 健公雏
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HenHouseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchID { get; set; }
        /// <summary>
        /// 期初01
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BreedingID { get; set; }
        /// <summary>
        /// 海兰褐
        /// </summary>
        public string BreedingName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SexType { get; set; }
        /// <summary>
        /// 公
        /// </summary>
        public string SexTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InQuantity1Pig5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InCost1Pig5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OriginalValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DepreciationAccumulated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InNetValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost7 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost7_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostHen6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenQuantity3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost9_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost3_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig3_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig4_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig5_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostHen6_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost4_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken21 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken31 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken41 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken71 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost81 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutQuantityChicken1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken7 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenQuantity6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutQuantityPig21 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost31 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost32 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost33 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost34 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost37 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenQuantity8_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost81 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost82 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost83 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost84 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost87 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost8 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockCost7 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockQuantity2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockCost_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenQuantity3_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost3_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost9_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost10_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig3_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig4_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostPig5_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostHen6_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost4_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost118 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenQuantity150 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost150 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost151 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost152 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenQuantity152 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost152 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenQuantity11 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost11 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenQuantity12 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost12 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenQuantity12 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenQuantity11 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal originalQuantityReduce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal originalValueReduce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal depreciationAccumulatedReduce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal originalQuantitySalesReduce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal originalValueSalesReduce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal depreciationAccumulatedSalesReduce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenQuantity110 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost110 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal endOriginalValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal endDepreciationAccumulated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal endNetValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost113_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost114_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost115_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost117_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCostSum110_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InCostPig3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InCostPig3_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken51 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutCostChicken5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost35 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OutHenCost85 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InHenCost116_1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SetStockCost5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InCostPig3_2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI101 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI102 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI103 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI104 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI105 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI106 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI107 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI108 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI109 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI101 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI102 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI103 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI104 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI105 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI106 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI107 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI108 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI109 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU101 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU102 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU103 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU104 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU105 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU106 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU107 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU108 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU109 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG101 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG102 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG103 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG104 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG105 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG106 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG107 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG108 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG109 { get; set; }
        public decimal OutHouseCost1 { get; set; }
        public decimal OutHouseCost2 { get; set; }
        public decimal OutHouseCost4 { get; set; }
        public decimal OutHouseCost3 { get; set; }
        public decimal OutHouseCost6 { get; set; }
        public decimal OutHouseCost5 { get; set; }
        public decimal OutHouseCost { get; set; }
        public decimal originalValueHouseReduce { get; set; }
        public decimal depreciationAccumulatedHouseReduce { get; set; }
    }
    public class CheckenHatchFlowCostSummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string FhChickenFarmID { get; set; }
        /// <summary>
        /// 孵化厂1
        /// </summary>
        public string FhChickenFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FhBatchID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FhBatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FarmID { get; set; }
        /// <summary>
        /// 禽场1
        /// </summary>
        public string FarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchID { get; set; }
        /// <summary>
        /// 引种真正成熟批次001
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BreedingID { get; set; }
        /// <summary>
        /// 海兰褐
        /// </summary>
        public string BreedingName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BroodProductID { get; set; }
        /// <summary>
        /// 验收合格种蛋-FJ
        /// </summary>
        public string BroodProductName { get; set; }
        /// <summary>
        /// 产蛋栋舍3
        /// </summary>
        public string HenHouseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HenHouseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost31 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost32 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity41 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost41 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity42 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost42 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity43 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost43 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity44 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost44 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity45 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost45 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity46 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost46 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity51 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost51 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity52 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost52 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity55 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost55 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity53 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost53 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity54 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity61 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost61 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost61Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity62 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost62 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost62Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity63 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost63 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost63Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInQuantity6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost6Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedEndQuantity1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedEndCost1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedEndCost1Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BreedInCost33 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIFEI201 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DIANFEI201 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHEBEIZHEJIU201 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RENGONG201 { get; set; }
    }
    public class EggHatchFlowCostSummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public long? StockType { get; set; }
        /// <summary>
        /// 产成品
        /// </summary>
        public string StockTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationName1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationID1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationName2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationID2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationName3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationID3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationName4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationID4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationName5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationID5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProductClassificationID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductClassificationName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? ProductGroupID { get; set; }
        /// <summary>
        /// 破损蛋
        /// </summary>
        public string ProductGroupName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationName1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationID1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationName2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationID2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationName3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationID3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationName4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationID4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationName5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationID5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? ClassificationID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassificationName { get; set; }
        /// <summary>
        /// 产蛋
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductBatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ChickenFarmID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ChickenFarmName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? HenHouseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HenHouseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BreedingName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchRemarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 破损蛋
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 枚
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? BatchID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity100 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost100 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity201 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost201 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost211 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost212 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost213 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost214 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost215 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost221 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity230 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost230 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost2301 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInPrice2301 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity2302 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInPrice2302 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity231 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost231 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity232 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost232 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity233 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost233 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity234 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost234 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost235 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity235 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInQuantity240 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost240 { get; set; }
        public decimal ZHIJIERENGONGJZHIGONGGONGZI101 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutQuantity301 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutCost301 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutQuantity302 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutCost302 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutQuantity303 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutCost303 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutQuantity304 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutCost304 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutQuantity305 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutCost305 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutQuantity310 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutCost310 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutQuantity401 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutCost401 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInPrice233 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost2331 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost2332 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDOutCost402 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SHUIDIANFEI110 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ZDInCost220 { get; set; }
    }
    public class GroupModel
    {
        public bool ProductId { get; set; }
        public bool DeptId { get; set; }
        public bool FarmerID { get; set; }
        public Group GroupBy(PigCirculationSummary dym)
        {
            var gro = new Group();
            if (ProductId)
            {
                gro.ProductId = dym.ProductId;
            }
            if (DeptId)
            {
                gro.DeptId = dym.DeptId;
            }
            if (FarmerID)
            {
                gro.FarmerID = dym.FarmerID;
            }
            return gro;
        }
        public struct Group
        {
            public string ProductId { get; set; }
            public string DeptId { get; set; }
            public string FarmerID { get; set; }
        }

    }

    public class SuppliesGroupModel
    {
        public bool MarketId { get; set; }
        public bool SupplierID { get; set; }
        public bool ProjectNameTwo { get; set; }
        public Group GroupBy(SuppliesModelForDataResult dym)
        {
            var gro = new Group();
            if (MarketId)
            {
                gro.MarketId = dym.MarketID;
            }
            if (SupplierID)
            {
                gro.SupplierID = dym.SupplierID;
            }
            return gro;
        }
        public struct Group
        {
            public string MarketId { get; set; }
            public string SupplierID { get; set; }
        }

    }

    public class LossGroupModel
    {
        public bool ProductId { get; set; }
        public bool DeptId { get; set; }
        public bool PersonId { get; set; }
        public bool CustomerId { get; set; }
        public bool ProjectId { get; set; }
        public Group GroupBy(MySettleReceiptDataResult dym)
        {
            var gro = new Group();
            if (ProductId)
            {
                gro.ProductId = dym.ProductID;
            }
            if (DeptId)
            {
                gro.DeptId = dym.MarketID;
            }
            if (PersonId)
            {
                gro.PersonId = dym.PersonID;
            }
            if (CustomerId)
            {
                gro.CustomerId = dym.CustomerID;
            }
            if (ProjectId)
            {
                gro.ProjectId = dym.ProjectID;
            }
            return gro;
        }
        public struct Group
        {
            public string ProductId { get; set; }
            public string DeptId { get; set; }
            public string PersonId { get; set; }
            public string CustomerId { get; set; }
            public string ProjectId { get; set; }
        }

    }

    public class BreederSummaryResultModel: EntitySubClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string DeptID { get; set; }
        /// <summary>
        /// 永乐服务部
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FarmerID { get; set; }
        /// <summary>
        /// 陈东玉
        /// </summary>
        public string FarmerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 育肥猪
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AdjustCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPigCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginMedicant { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginVaccine { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOther { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginInnerDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginOuterDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginDirectFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginBuildingFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginPredictFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginAdjustFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginAdditionFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BeginHoldUpFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentPigCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentPigCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentMedicant { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentVaccine { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentOther { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentInnerDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentOuterDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentDirectFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentBuildingFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentPredictFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentAdjustFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentAdditionFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentHoldUpFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecyclePigCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecyclePigCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleMedicant { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleVaccine { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleOther { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleInnerDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleOuterDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleDirectFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleBuildingFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecyclePredictFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleAdjustFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleAdditionFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RecycleHoldUpFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathPigCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathPigCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathMedicant { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathVaccine { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOther { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathInnerDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathOuterDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathDirectFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathBuildingFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathPredictFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathAdjustFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathAdditionFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DeathHoldUpFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPigCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndMedicant { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndVaccine { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOther { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndInnerDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndOuterDeath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndDirectFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndBuildingFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndPredictFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndAdjustFeedFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndAdditionFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal EndHoldUpFee { get; set; }
        /// <summary>
        /// 期末总成本
        /// </summary>
        public decimal EndTotalAmount { get; set; }
    }
    public class ShareCostItem
    {
        /// <summary>
        /// 费用项目ID
        /// </summary>

        public string CostProjectId { get; set; }

        ///项目名称
        public string CostProjectName { get; set; }
        public string CostProjectCode { get; set; }
        public int IsUse { get; set; }

    }
    public class ExpenseDetailsReportDto
    {
        /// <summary>
        /// 费用项目ID
        /// </summary>
        public string CostProjectID { get; set; }
        /// <summary>
        /// 费用项目类型ID
        /// </summary>
        public string CostProjectTypeID { get; set; }
        public string CostProjectTypeName { get; set; }
        /// <summary>
        /// 费用项目编码
        /// </summary>
        public string CostProjectCode { get; set; }
        /// <summary>
        /// 费用项目名称
        /// </summary>
        public string CostProjectName { get; set; }
        /// <summary>
        /// 预置项ID
        /// </summary>
        public string PresetItem { get; set; }
        /// <summary>
        /// 归集类型
        /// </summary>
        public string CollectionType { get; set; }
        public string CollectionTypeName { get; set; }
        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 科目Code
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 科目名称
        /// </summary>
        public string AccoSubjectName { get; set; }

        /// <summary>
        /// 猪场ID
        /// </summary>
        public string PigFarmID { get; set; }
        public string PigFarmName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptOrOthersID { get; set; }
        public string DeptOrOthersName { get; set; }
        /// <summary>
        /// 费用金额
        /// </summary>
        public decimal ExpenseAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        #region 大北农版
        /// <summary>
        /// 费用性质
        /// </summary>
        public string ExpenseNature { get; set; } = "0";
        public string ExpenseNatureName { get; set; }

        public string Nature { get; set; } = "0";
        public string NatureName { get; set; }

        /// <summary>
        /// 部门ID（人员归集辅助维度）
        /// </summary>
        public string DeptExtendID { get; set; }
        /// <summary>
        /// 生产组ID
        /// </summary>
        public string ProductionTeamID { get; set; } = "0";
        public string ProductionTeamName { get; set; }

        /// <summary>
        /// 生产组ID
        /// </summary>
        public string ProductLineID { get; set; } = "0";
        public string ProductLineName { get; set; }

        public decimal Amount { get; set; }
        /// <summary>
        /// 生产组ID
        /// </summary>
        public string MarketID { get; set; } = "0";
        public string MarketName { get; set; }

        /// <summary>
        /// 员工负责批次数
        /// </summary>
        public decimal PersonalBatchQuantity { get; set; } = 0;

        /// <summary>
        /// 组负责批次数
        /// </summary>
        public decimal TeamBatchQuantity { get; set; } = 0;

        /// <summary>
        /// 场负责批次数
        /// </summary>
        public decimal FarmBatchQuantity { get; set; } = 0;
        #endregion
    }


    public class MySettleReceiptDataResult
    {
        /// <summary>
        /// 汇总方式1
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }
        public string SummaryType1FieldName { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType2FieldName { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        public string SummaryType3FieldName { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 凭证字号
        /// </summary>
        public string Number { get; set; }
        public string TicketedPointName { get; set; }
        public string ANumber { get; set; }
        /// <summary>
        /// 记账号
        /// </summary>
        public string AccountNo { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string SettleSummaryName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectName { get; set; }
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 客户/供应商
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 部门/员工
        /// </summary>
        public string Cname { get; set; }
        public string MarketName { get; set; }
        public string HName { get; set; }
        /// <summary>
        /// 凭证类别
        /// </summary>
        public string SettleReceipType { get; set; }
        public string twoMarketName { get; set; }
        /// <summary>
        /// 借方金额
        /// </summary>
        public decimal Debit { get; set; }
        /// <summary>
        /// 贷方金额
        /// </summary>
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string ProjectName { get; set; }
        public string SuoShuDanWei { get; set; }
        /// <summary>
        /// 现金流量科目
        /// </summary>
        public string Cashflowproject { get; set; }

        /// <summary>
        /// 业务单元
        /// </summary>
        public string OrgMarketName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string TicketedPointID { get; set; }
        public string MarketID { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        public string ProjectID { get; set; }
        public string AccoSubjectID { get; set; }
        public bool? IsCus { get; set; }
        public bool? IsSup { get; set; }
        public string ProductID { get; set; }
    }


    public class FMResultModel
    {
        public FMResultModel()
        {
            //默认成功
            ResultState = true;
            Msg = "成功!";
        }

        /// <summary>
        /// 返回状态
        /// </summary>
        public bool ResultState { get; set; }
        /// <summary>
        /// 消息编号
        /// </summary>
        public int CodeNo { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }
    }
    public class Biz_Enterpriseperiod
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int RecordID { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public String StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 所属单位ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 最后修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }

    public class Biz_EnterpriseperiodEX
    {
        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 启用年度-建账
        /// </summary>
        public int EnableYear { get; set; }

        /// <summary>
        /// 启用会计期间-建账
        /// </summary>
        public int EnableMonth { get; set; }

        /// <summary>
        /// 会计期间明细
        /// </summary>
        public List<Biz_Enterpriseperiod> Details { get; set; }
    }

    public class Biz_EnterprisePeriodInfo
    {
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        public string BO_ID { get; set; }

        /// <summary>
        /// 启用年度
        /// </summary>
        public int EnableYear { get; set; }

        /// <summary>
        /// 启用会计期间
        /// </summary>
        public int EnableMonth { get; set; }

        /// <summary>
        /// 单位注册日期
        /// </summary>
        public string RegisterDate { get; set; }

        /// <summary>
        /// 注册年份
        /// </summary>
        public int RegisterYear { get; set; }

        /// <summary>
        /// 当前编辑年度
        /// </summary>
        public int CurrentEditYear { get; set; }

        /// <summary>
        /// 会计期间集合
        /// </summary>
        public List<Biz_EnterpriseperiodEX> LstPeriod { get; set; }
    }



    public class MaterialsCostReport
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal scQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal yclCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal bzCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal bcpCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal zcscCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal hjscCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal hzscCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ccpCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal costSub { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal scfy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal unitCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal xishu { get; set; }
        /// <summary>
        /// 公斤
        /// </summary>
        public string MeasurementUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal XZRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FLRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BXRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal GJJRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal RSDLRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SFRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DFRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal QTRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ftCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ftCostDetail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int iOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsSubTotal { get; set; }
        /// <summary>
        /// 产成品
        /// </summary>
        public string SummaryType1 { get; set; }
        /// <summary>
        /// 产成品
        /// </summary>
        public string SummaryType1Name { get; set; }
        /// <summary>
        /// 存货分类
        /// </summary>
        public string SummaryType1FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType2Name { get; set; }
        /// <summary>
        /// 商品代号
        /// </summary>
        public string SummaryType2FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SortId { get; set; }
    }
    public class CostSummaryReport
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal IOrder { get; set; }
        /// <summary>
        /// 商品E(中间品)
        /// </summary>
        public string SummaryType1 { get; set; }
        /// <summary>
        /// 商品E(中间品)
        /// </summary>
        public string SummaryType1Name { get; set; }
        /// <summary>
        /// 商品代号
        /// </summary>
        public string SummaryType1FieldName { get; set; }
        /// <summary>
        /// 中间品
        /// </summary>
        public string SummaryType2 { get; set; }
        /// <summary>
        /// 中间品
        /// </summary>
        public string SummaryType2Name { get; set; }
        /// <summary>
        /// 存货分类
        /// </summary>
        public string SummaryType2FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3FieldName { get; set; }
        /// <summary>
        /// 本
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qcQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qcAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal rkQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal cgQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal scQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal tzQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal cmQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal cgcmQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal dbcmQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal hscmQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal rkAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal cgAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal scAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal tzAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal cmAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal cgcmAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal dbcmAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal hscmAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal syAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ckQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal xsQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal lyQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal kyQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal kyAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal kyscQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal kyscAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qtlyQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal hsQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ckAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal xsAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal tzCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal lyAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qtlyAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal hsAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ckAmountSum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qmQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qmAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qmUnitAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSubTotal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SortId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal yhlrAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal yhlrzcAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal dbzrQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal dbzrAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal dbzcAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal dbzcQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal dqAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qtlzAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal qtlzQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal wzlyAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal wzlyQuantity { get; set; }
    }

    public class ProductionCostSummary
    {
        public string ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal tzAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal tzCost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal yclCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal bzCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal bcpCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal zcscCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal hzscCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal hjscCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ccpCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal costSub { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal scfy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal unitCost { get; set; }
    }
}
