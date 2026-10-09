using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFrostEnergyBlessingDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 3;
            config.UpgradedValue1 = 4;
            config.Value2 = 3;
            config.UpgradedValue2 = 4;
            config.ImageId = nameof(CirnoRCharacterFrostEnergyBlessing);
            // config.UpgradeImageId = String.Concat(nameof(CirnoRCharacterFrostEnergyBlessing), "Upgrade");

            config.Illustrator = "和音ハカ";

            config.RelativeEffects = new List<string>() { nameof(TempFirepower) };

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.Retain;

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrostEnergyBlessingDef))]
    public sealed class CirnoRCharacterFrostEnergyBlessing : CirnoRCharacterCard
    {
        public override Interaction Precondition()
        {
            List<CirnoRCharacterFrostEnergyBlessing> list = Library
                .CreateCards<CirnoRCharacterFrostEnergyBlessing>(2, this.IsUpgraded)
                .ToList<CirnoRCharacterFrostEnergyBlessing>();
            CirnoRCharacterFrostEnergyBlessing cirnoRCharacterFrostEnergyBlessing = list[0];
            CirnoRCharacterFrostEnergyBlessing cirnoRCharacterFrostEnergyBlessing2 = list[1];
            cirnoRCharacterFrostEnergyBlessing.ChoiceCardIndicator = 1;
            cirnoRCharacterFrostEnergyBlessing2.ChoiceCardIndicator = 2;
            cirnoRCharacterFrostEnergyBlessing.SetBattle(base.Battle);
            cirnoRCharacterFrostEnergyBlessing2.SetBattle(base.Battle);
            return new MiniSelectCardInteraction(list, false, false, false);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // //If a card was selected.
            // if (precondition != null)
            // {
            //     IReadOnlyList<Card> cards = ((SelectHandInteraction)precondition).SelectedCards;
            //     if (cards.Count > 0)
            //     {
            //         foreach (Card card in cards)
            //         {
            //             yield return new BuffAttackCardAction(card, base.Value1);
            //         }
            //     }
            // }
            // yield break;

            MiniSelectCardInteraction miniSelectCardInteraction =
                (MiniSelectCardInteraction)precondition;
            Card card =
                (miniSelectCardInteraction != null) ? miniSelectCardInteraction.SelectedCard : null;
            if (card != null)
            {
                if (card.ChoiceCardIndicator == 1)
                {
                    yield return BuffAction<TempFirepower>(Value1, Value1);
                }
                else
                {
                    yield return BuffAction<TempSpirit>(Value2, Value2);
                }
            }
            yield break;
        }
    }
}
