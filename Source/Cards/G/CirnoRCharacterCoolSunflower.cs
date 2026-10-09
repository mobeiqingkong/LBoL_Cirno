using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.Cards.Template;
using JetBrains.Annotations;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterCoolSunflowerDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1, Any = 1 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 2;
            config.UpgradedValue1 = 2;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Mana = new ManaGroup() { Any = 0 };

            config.Keywords = Keyword.Exile | Keyword.TempMorph;
            config.UpgradedKeywords = Keyword.Exile | Keyword.TempMorph;

            config.Illustrator = "";

            config.RelativeCards = new List<string>() { nameof(CirnoRCharacterFairyFlower) };
            config.UpgradedRelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterFairyFlower),
            };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterCoolSunflowerDef))]
    public sealed class CirnoRCharacterCoolSunflower : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // Add a token card to the hand.
            yield return new AddCardsToHandAction(
                Library.CreateCards<CirnoRCharacterFairyFlower>(Value1, false)
            );
            // Add a token card to the draw pile in a random position.
            if (IsUpgraded)
            {
                List<CirnoRCharacterFairyFlower> cards = Library
                    .CreateCards<CirnoRCharacterFairyFlower>(2, false)
                    .ToList<CirnoRCharacterFairyFlower>();
                foreach (Card card in cards)
                {
                    // card.IsAccuracy = true;
                    card.SetTurnCost(new ManaGroup() { Any = 0 });
                }
                yield return new AddCardsToDrawZoneAction(
                    cards,
                    DrawZoneTarget.Random,
                    AddCardsType.Normal
                );
                yield return new DrawManyCardAction(Value2);
            }
            yield break;
        }
    }
}
