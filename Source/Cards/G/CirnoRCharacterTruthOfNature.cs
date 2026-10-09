using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterTruthOfNatureDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Green = 1, Any = 1 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 2;
            config.UpgradedValue1 = 3;

            config.Value2 = 6;
            config.UpgradedValue2 = 8;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 2 };

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDaiyouseiBlessSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDaiyouseiBlessSe),
            };

            config.RelativeCards = new List<string>() { nameof(CirnoRCharacterSpringAndTheFairy) };
            config.UpgradedRelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterSpringAndTheFairy),
            };
            config.Illustrator = "べらぼう";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterTruthOfNatureDef))]
    public sealed class CirnoRCharacterTruthOfNature : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            ManaGroup gainMana = new ManaGroup()
            {
                Green = Value1 * base.Battle.AllAliveEnemies.Count(),
            };
            yield return new GainManaAction(gainMana);
            // yield return BuffAction<CirnoRCharacterDaiyouseiBlessSe>(Value1);
            // yield return BuffAction<CirnoRCharacterDaiyouseiBlessSe>(base.Battle.AllAliveEnemies.Count());

            yield return new AddCardsToHandAction(
                Library.CreateCard<CirnoRCharacterSpringAndTheFairy>()
            );
            yield break;
        }
    }
}
