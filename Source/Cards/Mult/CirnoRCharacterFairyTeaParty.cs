using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.Base;
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
    public sealed class CirnoRCharacterFairyTeaPartyDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(4712);
            config.Colors = new List<ManaColor>() { ManaColor.Blue, ManaColor.Green };
            config.Cost = new ManaGroup() { Blue = 2, Green = 2 };
            config.UpgradedCost = new ManaGroup() { Hybrid = 2, HybridColor = 6 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Value2 = 9;
            config.UpgradedValue2 = 9;

            config.Keywords = Keyword.Exile | Keyword.Ethereal;
            config.UpgradedKeywords = Keyword.Exile | Keyword.Ethereal;

            //Add Weak, Vulnerable, Firepower Down descrption when hovering over the card.
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFairyTeaPartySe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFairyTeaPartySe),
            };
            config.Mana = new ManaGroup() { Philosophy = 1 };

            config.Illustrator = "じゅんけ";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFairyTeaPartyDef))]
    public sealed class CirnoRCharacterFairyTeaParty : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
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

            yield return BuffAction<CirnoRCharacterFairyTeaPartySe>(Value1, Value1);

            yield break;
        }
    }
}
