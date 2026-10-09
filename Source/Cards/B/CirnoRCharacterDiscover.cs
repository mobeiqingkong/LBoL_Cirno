using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.Patches;
using CirnoRCharacterMod.StatusEffects;
using DG.Tweening;
using HarmonyLib;
using JetBrains.Annotations;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.Randoms;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.BattleModifiers.Actions;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterDiscoverDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 2, Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;
            config.UpgradedValue1 = 2;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Mana = new ManaGroup() { Any = 2 };
            config.UpgradedMana = new ManaGroup() { Any = 2 };

            config.Illustrator = "";

            config.Keywords = Keyword.Exile | Keyword.TempMorph;
            config.UpgradedKeywords = Keyword.Exile | Keyword.TempMorph;

            config.RelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterIceSoul),
                nameof(CirnoRCharacterPerfectFreeze),
                nameof(CirnoRCharacterUltimateBlizzard),
            };
            config.UpgradedRelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterIceSoul),
                nameof(CirnoRCharacterPerfectFreeze),
                nameof(CirnoRCharacterUltimateBlizzard),
            };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterDiscoverDef))]
    public sealed class CirnoRCharacterDiscover : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            Card[] array = base.Battle.RollCardsWithoutManaLimit(
                new CardWeightTable(
                    RarityWeightTable.OnlyRare, // Rarity distribution of the cards. (BattleCard: 40% Common, 40% Uncommon, 20% Rare)
                    OwnerWeightTable.Valid, // Player and Neutral card pool. (Valid: Includes the Character, Act 1 Boss Exhibit and Neutral cards.)
                    CardTypeWeightTable.CanBeLoot, // Card types. (Can Be Loot: Can be Attack, Defense, Skill, ability; Cannot be Tools).
                    false
                ),
                base.Value1, //Total amount of card to choose from.
                (CardConfig config) =>
                    config.Id != base.Id && config.Keywords.HasFlag(Keyword.Accuracy)
            );
            // Card selection.
            Card CirnoRCharacterIceSoul = Library.CreateCard<CirnoRCharacterIceSoul>();
            array = array.AddToArray(CirnoRCharacterIceSoul);
            Card CirnoRCharacterPerfectFreeze = Library.CreateCard<CirnoRCharacterPerfectFreeze>();
            array = array.AddToArray(CirnoRCharacterPerfectFreeze);
            Card CirnoRCharacterUltimateBlizzard =
                Library.CreateCard<CirnoRCharacterUltimateBlizzard>();
            array = array.AddToArray(CirnoRCharacterUltimateBlizzard);
            array = array.Reverse().ToArray();

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
                selectedCard.IsEthereal = true;
                selectedCard.IsExile = true;
                if (!selectedCard.IsXCost)
                {
                    selectedCard.SetTurnCost(base.Mana);
                }
            }
            yield return new AddCardsToHandAction(cards);

            yield break;
        }
    }
}
