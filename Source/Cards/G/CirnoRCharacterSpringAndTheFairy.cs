using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterSpringAndTheFairyDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 0 };
            config.Rarity = Rarity.Rare;
            config.IsPooled = false;
            config.FindInBattle = false;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Self;

            // //If IsPooled is false then the card cannot be discovered or added to the library at the end of combat.
            // config.IsPooled = false;

            config.Value1 = 3;
            config.UpgradedValue1 = 3;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            //The Forbidden Keyword makes the card unplayable.
            //The Shield keyword is here to add the Barrier description to the tooltip.
            config.Keywords = Keyword.Shield | Keyword.Ethereal;
            config.UpgradedKeywords = Keyword.Shield | Keyword.Ethereal;

            config.Illustrator = "東天紅";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterSpringAndTheFairyDef))]
    public sealed class CirnoRCharacterSpringAndTheFairy : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return base.BuffAction<CirnoRCharacterSpringAndTheFairySe>(Value2, Value2);
        }
    }
}
