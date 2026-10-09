using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterTakeANapDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 0 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 4;
            config.UpgradedValue1 = 4;

            config.Value2 = 4;
            config.UpgradedValue2 = 4;

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            config.Illustrator = "ちばや";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterTakeANapDef))]
    public sealed class CirnoRCharacterTakeANap : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return base.DefenseAction(true);
            DrawManyCardAction drawAction = new DrawManyCardAction(base.Value1);
            yield return drawAction;
            IEnumerable<Card> drawnCards = drawAction.DrawnCards;
            List<Card> list = new List<Card>();
            foreach (Card card in drawnCards)
            {
                if (card.Zone == CardZone.Hand && !card.Config.Colors.Contains(ManaColor.Green))
                {
                    list.Add(card);
                }
            }
            if (list.Count > 0)
            {
                yield return new DiscardManyAction(list);
            }
        }
    }
}
