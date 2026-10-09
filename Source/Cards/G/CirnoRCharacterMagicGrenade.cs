using System;
using System.Collections.Generic;
using System.Linq;
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
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterMagicGrenadeDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.GunName = GunNameID.GetGunFromId(4541);

            config.Colors = new List<ManaColor>() { ManaColor.Black };
            config.Cost = new ManaGroup() { Black = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.IsPooled = false;
            config.Damage = 15;
            config.UpgradedDamage = 15;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Keywords =
                Keyword.Exile | Keyword.Ethereal | Keyword.Replenish | Keyword.Accuracy;
            config.UpgradedKeywords =
                Keyword.Exile | Keyword.Ethereal | Keyword.Replenish | Keyword.Accuracy;

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterMagicGrenadeSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterMagicGrenadeSe),
            };

            // 原 Any 3：1 费打出后倒赚 2 点法力（官方对照 ColdBreath 是 2 费才净赚 1 点）。
            // 改为净 0（花 1 得 1），只保留"把黑色换成任意色"的定位。
            config.Mana = new ManaGroup() { Any = 1 };
            config.UpgradedMana = new ManaGroup() { Any = 1 };

            config.RelativeCards = new List<string>() { };
            config.UpgradedRelativeCards = new List<string>() { };
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterMagicGrenadeDef))]
    public sealed class CirnoRCharacterMagicGrenade : CirnoRCharacterCard
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
            yield return AttackAllAliveEnemyAction();
            foreach (EnemyUnit enemy in Battle.AllAliveEnemies)
            {
                yield return DebuffAction<CirnoRCharacterMagicGrenadeSe>(
                    enemy,
                    base.Value1,
                    base.Value1,
                    0,
                    0,
                    true,
                    0.2f
                );
            }
            yield break;
        }
    }
}
