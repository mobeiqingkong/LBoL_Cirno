using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.GapOptions;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterEntangledRootsDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green, ManaColor.Black };
            config.Cost = new ManaGroup() { Green = 1, Black = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.SingleEnemy;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Value2 = 1;
            config.UpgradedValue2 = 2;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 1 };

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile | Keyword.Retain;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterEntangledRootsDef))]
    public sealed class CirnoRCharacterEntangledRoots : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return DebuffAction<CirnoRCharacterEntangledRootsSe>(
                selector.SelectedEnemy,
                Value1,
                Value1
            );
            List<Card> list0 = (
                from card in base.Battle.HandZone
                where card.Id == "CirnoRCharacterEntangledRoots"
                select card
            ).ToList<Card>();
            if (list0.Count > 0)
            {
                foreach (Card card in list0)
                {
                    if (card != this)
                    {
                        yield return new MoveCardAction(card, CardZone.Exile);
                    }
                }
            }

            List<Card> list1 = (
                from card in base.Battle.DrawZone
                where card.Id == "CirnoRCharacterEntangledRoots"
                select card
            ).ToList<Card>();
            if (list1.Count > 0)
            {
                foreach (Card card in list1)
                {
                    if (card != this)
                    {
                        yield return new MoveCardAction(card, CardZone.Exile);
                    }
                }
            }

            List<Card> list2 = (
                from card in base.Battle.DiscardZone
                where card.Id == "CirnoRCharacterEntangledRoots"
                select card
            ).ToList<Card>();
            if (list2.Count > 0)
            {
                foreach (Card card in list2)
                {
                    yield return new MoveCardAction(card, CardZone.Exile);
                }
            }

            yield break;
        }
    }
}
