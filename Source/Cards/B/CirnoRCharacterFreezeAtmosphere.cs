using System.Collections.Generic;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.Patches;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFreezeAtmosphereDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 0 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.AllEnemies;

            config.Value1 = 2;
            config.UpgradedValue1 = 2;

            config.Value2 = 2;
            config.UpgradedValue2 = 2;
            config.RelativeEffects = new List<string>()
            {
                nameof(TempFirepower),
                nameof(TempFirepowerNegative),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(TempFirepower),
                nameof(TempFirepowerNegative),
            };

            config.Illustrator = "かずま";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFreezeAtmosphereDef))]
    public sealed class CirnoRCharacterFreezeAtmosphere : CirnoRCharacterCard
    {
        // protected override void OnEnterBattle(BattleController battle)
        // {
        //     base.ReactBattleEvent<DamageEventArgs>(CustomGameEventManager.PostCustomDamageEvent, new EventSequencedReactor<DamageEventArgs>(this.afterAtack));
        // }

        // private IEnumerable<BattleAction> afterAtack(DamageEventArgs args)
        // {
        //     // Activate a visual effect to notify that the card was changed.
        //     base.NotifyActivating();
        //     this.DeltaValue1--;
        //     // Update the card's textbox.
        //     base.NotifyChanged();
        //     yield break;
        // }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            foreach (Unit enemy in base.Battle.AllAliveEnemies)
            {
                yield return DebuffAction<TempFirepowerNegative>(enemy, Value1, Value1);
            }
            yield return BuffAction<TempFirepower>(Value2, Value2);
            yield break;
        }
    }
}
