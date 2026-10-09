using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterGreatCrusherDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            //Mana to consume aside from X
            config.Cost = new ManaGroup() { Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1 };
            config.Rarity = Rarity.Common;

            //The XCost has to be set.
            config.IsXCost = false;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;
            config.Value2 = 1;
            config.UpgradedValue2 = 2;

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            config.RelativeCards = new List<string>() { nameof(CirnoRCharacterIceBead) };
            config.UpgradedRelativeCards = new List<string>() { nameof(CirnoRCharacterIceBead) };

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.None;
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterGreatCrusherDef))]
    public sealed class CirnoRCharacterGreatCrusher : CirnoRCharacterCard
    {
        protected override int BaseValue3 { get; set; } = 1;
        protected override int BaseUpgradedValue3 { get; set; } = 1;

        public override Interaction Precondition()
        {
            List<Card> list = base
                .Battle.HandZone.Where((Card card) => card != this)
                .ToList<Card>();
            if (list.Count <= 0)
            {
                return null;
            }
            return new SelectHandInteraction(0, base.Value2, list);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // If a card was selected.
            Card cirnoRCharacterIceBead = Library.CreateCard<CirnoRCharacterIceBead>();
            cirnoRCharacterIceBead.IsEthereal = true;
            cirnoRCharacterIceBead.IsExile = true;
            yield return new AddCardsToHandAction(cirnoRCharacterIceBead);
            if (precondition != null)
            {
                IReadOnlyList<Card> cards = ((SelectHandInteraction)precondition).SelectedCards;
                if (cards.Count > 0)
                {
                    foreach (Card card in cards)
                    {
                        // // 放逐
                        // yield return new ExileCardAction(card);
                        // // 移除出本次战斗
                        // yield return new RemoveCardAction(card);
                        // 丢弃
                        List<CirnoRCharacterIceBead> list = Library
                            .CreateCards<CirnoRCharacterIceBead>(base.Value3, false)
                            .ToList<CirnoRCharacterIceBead>();
                        foreach (CirnoRCharacterIceBead item in list)
                        {
                            item.IsEthereal = true;
                            item.IsExile = true;
                        }
                        yield return new ExileCardAction(card);
                        yield return new AddCardsToHandAction(list);
                    }
                }
            }
            yield break;
        }
    }
}
