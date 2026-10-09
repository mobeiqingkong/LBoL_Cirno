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
    public sealed class CirnoRCharacterTheHiddenBiosphereDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup()
            {
                Any = 1,
                Hybrid = 1,
                HybridColor = 6,
            };
            config.UpgradedCost = new ManaGroup() { Hybrid = 1, HybridColor = 6 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.Initial;

            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterTheHiddenBiosphereSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterTheHiddenBiosphereSe),
            };

            config.Mana = new ManaGroup() { Any = 3 };
            config.UpgradedMana = new ManaGroup() { Any = 3 };

            config.RelativeCards = new List<string>() { };
            config.UpgradedRelativeCards = new List<string>() { };
            config.Illustrator = "メジロ";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterTheHiddenBiosphereDef))]
    public sealed class CirnoRCharacterTheHiddenBiosphere : CirnoRCharacterCard
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
            // Card cardAyaWindGirl = Library.CreateCard<AyaWindGirl>();
            // cardAyaWindGirl.Keywords = Keyword.Retain;
            // cardAyaWindGirl.SetTurnCost(base.Mana);
            // // Card cardAyaWindGirl = Library.CreateCard<AyaWindGirl>();
            // yield return new AddCardsToHandAction(cardAyaWindGirl);
            // yield return new Debuff(cardAyaWindGirl);
            yield return BuffAction<CirnoRCharacterTheHiddenBiosphereSe>(Value1, Value1);
            yield break;
        }
    }
}
