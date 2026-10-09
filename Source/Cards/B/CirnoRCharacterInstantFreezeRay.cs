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
    public sealed class CirnoRCharacterInstantFreezeRayDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(14020);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 6;
            config.UpgradedDamage = 9;

            // config.Keywords = Keyword.Accuracy;
            // config.UpgradedKeywords = Keyword.Accuracy;
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.Accuracy;
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterInstantFreezeRayDef))]
    public sealed class CirnoRCharacterInstantFreezeRay : CirnoRCharacterCard
    {
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
                    int level = args
                        .Targets.First<Unit>()
                        .GetStatusEffect<CirnoRCharacterFrostbiteSe>()
                        .Level;
                    args.DamageInfo = args.DamageInfo.IncreaseBy(base.Value1 * level);
                }
            }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return base.AttackAction(selector);
            yield return DebuffAction<CirnoRCharacterFrostSe>(selector.SelectedEnemy, 1, 1);
            yield break;
        }
    }
}
