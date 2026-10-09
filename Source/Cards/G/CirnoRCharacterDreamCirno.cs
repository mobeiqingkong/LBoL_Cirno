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
    public sealed class CirnoRCharacterDreamCirnoDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Black };
            config.Cost = new ManaGroup() { Black = 1 };
            config.UpgradedCost = new ManaGroup() { Black = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            // config.Keywords = Keyword.None;
            // config.UpgradedKeywords = Keyword.Initial;
            config.RelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterMagicGrenade),
                nameof(CirnoRCharacterMuggleWand),
            };
            config.UpgradedRelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterMagicGrenade),
                nameof(CirnoRCharacterMuggleWand),
            };

            config.Mana = new ManaGroup() { Any = 3 };
            config.UpgradedMana = new ManaGroup() { Any = 3 };
            config.Illustrator = "大魔王るあえる";

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;
            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterDreamCirnoDef))]
    public sealed class CirnoRCharacterDreamCirno : CirnoRCharacterCard
    {
        public override Interaction Precondition()
        {
            if (IsUpgraded)
                return null;
            List<CirnoRCharacterDreamCirno> list = Library
                .CreateCards<CirnoRCharacterDreamCirno>(2, this.IsUpgraded)
                .ToList<CirnoRCharacterDreamCirno>();
            CirnoRCharacterDreamCirno cirnoRCharacterDreamCirno = list[0];
            CirnoRCharacterDreamCirno cirnoRCharacterDreamCirno2 = list[1];
            cirnoRCharacterDreamCirno.ChoiceCardIndicator = 1;
            cirnoRCharacterDreamCirno2.ChoiceCardIndicator = 2;
            cirnoRCharacterDreamCirno.SetBattle(base.Battle);
            cirnoRCharacterDreamCirno2.SetBattle(base.Battle);
            return new MiniSelectCardInteraction(list, false, false, false);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            if (IsUpgraded)
            {
                List<Card> cards = new List<Card>();
                Card cirnoRCharacterMagicGrenade =
                    Library.CreateCard<CirnoRCharacterMagicGrenade>();
                cards.Add(cirnoRCharacterMagicGrenade);
                Card cirnoRCharacterMuggleWand = Library.CreateCard<CirnoRCharacterMuggleWand>();
                cards.Add(cirnoRCharacterMuggleWand);
                yield return new AddCardsToDiscardAction(cards);
            }
            else
            {
                MiniSelectCardInteraction miniSelectCardInteraction =
                    (MiniSelectCardInteraction)precondition;
                Card card =
                    (miniSelectCardInteraction != null)
                        ? miniSelectCardInteraction.SelectedCard
                        : null;
                if (card != null)
                {
                    if (card.ChoiceCardIndicator == 1)
                    {
                        yield return new AddCardsToDiscardAction(
                            Library.CreateCard<CirnoRCharacterMagicGrenade>()
                        );
                    }
                    else
                    {
                        yield return new AddCardsToDiscardAction(
                            Library.CreateCard<CirnoRCharacterMuggleWand>()
                        );
                    }
                }
            }

            // yield return new DrawManyCardAction(Value2);
            yield break;
        }
    }
}
