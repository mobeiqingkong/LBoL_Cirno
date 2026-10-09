using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFireworksFestivalDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 5;
            config.UpgradedValue1 = 5;

            config.Value2 = 1;
            config.UpgradedValue2 = 2;

            config.Mana = new ManaGroup() { Any = 1 };
            config.UpgradedMana = new ManaGroup() { Any = 0 };

            config.Illustrator = "";

            config.Keywords = Keyword.Exile | Keyword.TempMorph;
            config.UpgradedKeywords = Keyword.Exile | Keyword.TempMorph;

            config.RelativeCards = new List<string>() { };
            config.UpgradedRelativeCards = new List<string>() { };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFireworksFestivalDef))]
    public sealed class CirnoRCharacterFireworksFestival : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            Card[] array = base.Battle.RollCards(
                new CardWeightTable(
                    RarityWeightTable.BattleCard,
                    OwnerWeightTable.Valid,
                    CardTypeWeightTable.CanBeLoot,
                    false
                ),
                base.Value1,
                (CardConfig config) => config.Type == CardType.Defense
            );
            if (array.Length != 0)
            {
                MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(
                    array,
                    false,
                    false,
                    false
                )
                {
                    Source = this,
                };
                yield return new InteractionAction(interaction, false);
                Card selectedCard = interaction.SelectedCard;
                selectedCard.SetTurnCost(base.Mana);
                yield return new AddCardsToHandAction(new Card[] { selectedCard });
                interaction = null;
            }
            yield break;

            // Card[] array = base.Battle.RollCardsWithoutManaLimit(
            //     new CardWeightTable(
            //         RarityWeightTable.BattleCard, // Rarity distribution of the cards. (BattleCard: 40% Common, 40% Uncommon, 20% Rare)
            //         OwnerWeightTable.Valid, // Player and Neutral card pool. (Valid: Includes the Character, Act 1 Boss Exhibit and Neutral cards.)
            //         CardTypeWeightTable.CanBeLoot, // Card types. (Can Be Loot: Can be Attack, Defense, Skill, ability; Cannot be Tools).
            //         false
            //     ),
            //     base.Value1, //Total amount of card to choose from.
            //     (CardConfig config) =>
            //         config.Id != base.Id && config.Keywords.HasFlag(Keyword.Accuracy)
            // );
            // // Card selection.
            // Card CirnoRCharacterIceSoul = Library.CreateCard<CirnoRCharacterIceSoul>();
            // array = array.AddToArray(CirnoRCharacterIceSoul);
            // Card CirnoRCharacterEncounterRain = Library.CreateCard<CirnoRCharacterEncounterRain>();
            // array = array.AddToArray(CirnoRCharacterEncounterRain);
            // Card CirnoRCharacterPerfectGlacialist =
            //     Library.CreateCard<CirnoRCharacterPerfectGlacialist>();
            // array = array.AddToArray(CirnoRCharacterPerfectGlacialist);
            // array = array.Reverse().ToArray();

            // MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(array, false, false, false)
            // {
            //     Source = this
            // };
            // yield return new InteractionAction(interaction, false);
            // Card selectedCard = interaction.SelectedCard;
            // //Change the cost of the card.
            // selectedCard.SetTurnCost(base.Mana);
            // selectedCard.IsEthereal = true;
            // selectedCard.IsExile = true;
            // yield return new AddCardsToHandAction(new Card[] { selectedCard });

            //To choose more than 1 card.
            /*Interaction interactionMultiple = new SelectCardInteraction(0, base.Value2, array, 0);
            IReadOnlyList<Card> cards = ((SelectCardInteraction)interactionMultiple).SelectedCards;
            yield return new AddCardsToHandAction(cards);*/

            Interaction interactionMultiple = new SelectCardInteraction(0, base.Value2, array, 0)
            {
                Source = this,
            };
            yield return new InteractionAction(interactionMultiple, false);
            IReadOnlyList<Card> cards = ((SelectCardInteraction)interactionMultiple).SelectedCards;

            // // MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(array, false, false, false)
            // // {
            // //     Source = this
            // // };
            // // yield return new InteractionAction(interaction, false);
            // // Card selectedCard = interaction.SelectedCard;
            foreach (Card selectedCard in cards)
            {
                ////Change the cost of the card.
                //if (selectedCard != CirnoRCharacterIceSoul
                //|| selectedCard != CirnoRCharacterEncounterRain
                //|| selectedCard != CirnoRCharacterPerfectGlacialist)
                //{
                //    selectedCard.SetTurnCost(base.Mana);
                //    selectedCard.IsEthereal = true;
                //    selectedCard.IsExile = true;
                //}
                selectedCard.SetTurnCost(base.Mana);
                selectedCard.IsEthereal = true;
                selectedCard.IsExile = true;
            }
            yield return new AddCardsToHandAction(cards);

            yield break;
        }
    }
}
