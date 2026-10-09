using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterTwinFairiesDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Green, ManaColor.Blue };
            //Hybrid colors:
            //0 = W/U
            //1 = W/B
            //2 = W/R
            //3 = W/G
            //4 = U/B
            //5 = U/R 7(黑+红)
            //6 = U/G 8(黑+绿)
            //7 = B/R 9(2个白色+2个红色)
            //8 = B/G 10(2个白色)
            //9 = R/G
            //As of 1.5.1: Colorless hybrid are not supported.
            config.Cost = new ManaGroup() { Hybrid = 1, HybridColor = 6 };
            config.UpgradedCost = new ManaGroup() { Any = 0 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Mana = new ManaGroup() { Blue = 1, Green = 1 };
            config.UpgradedMana = new ManaGroup() { Blue = 1, Green = 1 };

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterTwinFairiesDef))]
    public sealed class CirnoRCharacterTwinFairies : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return new GainManaAction(base.Mana);
            yield return new GainTurnManaAction(base.Mana);
            yield break;
        }
    }
}
