using System.Collections.Generic;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Exhibits
{
    public sealed class CirnoRCharacterExhibitBDef : CirnoRCharacterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            ExhibitConfig exhibitConfig = this.GetDefaultExhibitConfig();
            exhibitConfig.Value1 = 1;
            exhibitConfig.Value2 = 5;
            exhibitConfig.Mana = new ManaGroup() { Green = 1 };
            exhibitConfig.BaseManaColor = ManaColor.Green;
            exhibitConfig.HasCounter = false;
            // exhibitConfig.InitialCounter = 0;

            exhibitConfig.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDaiyouseiBlessSe),
                nameof(CirnoRCharacterNaturalGrowthSe),
            };

            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterExhibitBDef))]
    public sealed class CirnoRCharacterExhibitB : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent<UnitEventArgs>(
                base.Battle.Player.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnPlayerTurnStarted)
            );
            // base.ReactBattleEvent<HealEventArgs>(base.Battle.Player.HealingGiving, new EventSequencedReactor<HealEventArgs>(this.OnHealAction));
            // base.ReactBattleEvent<DamageEventArgs>(base.Owner.DamageReceived, new EventSequencedReactor<DamageEventArgs>(this.afterAtack));
        }

        private IEnumerable<BattleAction> OnPlayerTurnStarted(UnitEventArgs args)
        {
            if (base.Battle.Player.TurnCounter == 1)
            {
                base.NotifyActivating();
                yield return new ApplyStatusEffectAction<CirnoRCharacterDaiyouseiBlessSe>(
                    base.Battle.Player,
                    base.Value1,
                    0
                );
                yield return new ApplyStatusEffectAction<CirnoRCharacterNaturalGrowthSe>(
                    base.Battle.Player,
                    Value1,
                    Value1
                );
            }
            yield break;
        }
    }
}
