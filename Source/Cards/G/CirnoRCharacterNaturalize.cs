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
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterNaturalizeDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;
            config.UpgradedValue1 = 2;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 1 };

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile | Keyword.Initial;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterNaturalizeDef))]
    public sealed class CirnoRCharacterNaturalize : CirnoRCharacterCard
    {
        public override Interaction Precondition()
        {
            List<Card> list = base
                .Battle.HandZone.Where((Card card) => card != this)
                .ToList<Card>();
            if (list.Count <= 0)
            {
                return null;
            }
            return new SelectHandInteraction(0, base.Value1, list);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            if (precondition != null)
            {
                IReadOnlyList<Card> cards = ((SelectHandInteraction)precondition).SelectedCards;
                if (cards.Count > 0)
                {
                    foreach (Card card in cards)
                    {
                        yield return new ExileCardAction(card); // 放逐
                        yield return new DrawManyCardAction(base.Value2); // 抽牌
                        yield return new GainManaAction(base.Mana); // 获得法力
                    }
                }
            }
            yield break;
        }
    }
}
