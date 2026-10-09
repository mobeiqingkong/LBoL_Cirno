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
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterMuggleWandDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(13180);

            config.Colors = new List<ManaColor>() { ManaColor.Black };
            config.Cost = new ManaGroup() { Black = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.IsPooled = false;
            config.Damage = 6;
            config.UpgradedDamage = 10;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Value1 = 3;
            config.UpgradedValue1 = 3;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Keywords =
                Keyword.Exile | Keyword.Ethereal | Keyword.Replenish | Keyword.Accuracy;
            config.UpgradedKeywords =
                Keyword.Exile | Keyword.Ethereal | Keyword.Replenish | Keyword.Accuracy;

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterMuggleWandSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterMuggleWandSe),
            };

            // 原 Any 3：1 费打出后倒赚 2 点法力 -> 改为净 0
            config.Mana = new ManaGroup() { Any = 1 };
            config.UpgradedMana = new ManaGroup() { Any = 1 };

            config.RelativeCards = new List<string>() { };
            config.UpgradedRelativeCards = new List<string>() { };
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterMuggleWandDef))]
    public sealed class CirnoRCharacterMuggleWand : CirnoRCharacterCard
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
            base.CardGuns = new Guns(base.GunName, base.Value1, true);
            foreach (GunPair gunPair in base.CardGuns.GunPairs)
            {
                yield return base.AttackAction(selector, gunPair);
            }

            yield return DebuffAction<CirnoRCharacterMuggleWandSe>(
                selector.SelectedEnemy,
                base.Value2,
                base.Value2,
                0,
                0,
                true,
                0.2f
            );
            yield break;
        }
    }
}
