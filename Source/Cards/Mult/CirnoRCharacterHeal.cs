using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterHealDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green, ManaColor.Blue };
            config.Cost = new ManaGroup() { Hybrid = 1, HybridColor = 6 };

            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.AllEnemies;

            config.Value1 = 9;
            config.UpgradedValue1 = 9;

            config.Value2 = 9;
            config.UpgradedValue2 = 9;

            config.Keywords = Keyword.Exile | Keyword.Ethereal;
            config.UpgradedKeywords = Keyword.Exile | Keyword.Retain;

            config.Illustrator = "白山ゆーき";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterHealDef))]
    public sealed class CirnoRCharacterHeal : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // EnemyUnit selectedEnemy = selector.SelectedEnemy;
            //The player loses life:
            yield return HealAction(base.Value1);
            //This is equvalent to:
            //yield return new HealAction(Battle.Player, selectedEnemy, base.Value1);

            //Target enemy loses hp.
            foreach (Unit enemy in base.Battle.AllAliveEnemies)
            {
                yield return new HealAction(Battle.Player, enemy, base.Value2);
            }
            yield break;
        }
    }
}
