using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFairyFlowerDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(6002);
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Any = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 0 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;
            config.IsPooled = false;

            config.Damage = 9;
            config.UpgradedDamage = 9;

            config.Value1 = 6;
            config.UpgradedValue1 = 6;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Illustrator = "";

            config.Keywords = Keyword.Exile | Keyword.Ethereal;
            config.UpgradedKeywords = Keyword.Exile | Keyword.Ethereal;

            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDeathOfNatureSe),
                nameof(CirnoRCharacterFrostbiteSe),
                // nameof(SummerFlowerSe),
                // nameof(CirnoRCharacterFairyFlowerSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDeathOfNatureSe),
                nameof(CirnoRCharacterFrostbiteSe),
                // nameof(SummerFlowerSe),
                // nameof(CirnoRCharacterFairyFlowerSe),
            };

            config.RelativeCards = new List<string>() { nameof(SummerFlower) };
            config.UpgradedRelativeCards = new List<string>() { nameof(SummerFlower) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFairyFlowerDef))]
    public sealed class CirnoRCharacterFairyFlower : CirnoRCharacterCard
    {
        protected override int BaseValue3 { get; set; } = 1;
        protected override int BaseUpgradedValue3 { get; set; } = 1;

        protected override int AdditionalDamage
        {
            get
            {
                if (base.Battle == null)
                {
                    return 0;
                }
                return Value3
                    * (
                        base.GetSeLevel<CirnoRCharacterFairyFlowerSe>()
                        + base.GetSeLevel<SummerFlowerSe>()
                    );
            }
        }

        protected override void OnEnterBattle(BattleController battle)
        {
            base.HandleBattleEvent<DamageDealingEventArgs>(
                base.Battle.Player.DamageDealing,
                new GameEventHandler<DamageDealingEventArgs>(this.OnPlayerDamageDealing),
                (GameEventPriority)0
            );
        }

        private void OnPlayerDamageDealing(DamageDealingEventArgs args)
        {
            if (args.ActionSource == this && args.Targets != null)
            {
                if (
                    args.Targets.Any(
                        (Unit target) => target.HasStatusEffect<CirnoRCharacterFrostbiteSe>()
                    )
                )
                {
                    args.DamageInfo = args.DamageInfo.IncreaseBy(base.Value1);
                    // args.DamageInfo = args.DamageInfo.MultiplyBy(base.Value1);
                }
                int level =
                    args.Targets.First<Unit>()
                        .GetStatusEffect<CirnoRCharacterDeathOfNatureSe>()
                        ?.Level
                    ?? 0;
                if (level > 0)
                {
                    args.DamageInfo = args.DamageInfo.IncreaseBy(base.Value2 * level);
                    // args.DamageInfo = args.DamageInfo.MultiplyBy(base.Value1);
                }
                // args.DamageInfo = args.DamageInfo.IncreaseBy(
                //     Value3
                //         * (
                //             base.GetSeLevel<CirnoRCharacterFairyFlowerSe>()
                //             + base.GetSeLevel<SummerFlowerSe>()
                //         )
                // );
            }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return base.AttackAction(selector);
            yield return DebuffAction<CirnoRCharacterDeathOfNatureSe>(selector.SelectedEnemy, 1);
            yield return BuffAction<CirnoRCharacterFairyFlowerSe>(1);
            yield break;
        }
    }
}
