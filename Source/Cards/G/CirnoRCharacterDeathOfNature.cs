using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterDeathOfNatureDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 2, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Green = 1, Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.AllEnemies;

            config.Value1 = 8;
            config.UpgradedValue1 = 8;
            // config.Keywords = Keyword.Exile;
            // config.UpgradedKeywords = Keyword.Exile;
            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDeathOfNatureSe),
            };

            config.Illustrator = "ことイナリ";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterDeathOfNatureDef))]
    public sealed class CirnoRCharacterDeathOfNature : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            foreach (Unit enemy in base.Battle.AllAliveEnemies)
            {
                yield return DebuffAction<CirnoRCharacterDeathOfNatureSe>(
                    enemy,
                    base.Value1,
                    0,
                    0,
                    0,
                    true,
                    0.2f
                );
            }
            //The player loses life:
            // yield return HealAction(base.Value1);
            //This is equvalent to:
            //yield return new HealAction(Battle.Player, selectedEnemy, base.Value1);

            //Target enemy loses hp.
            // yield return new Debuff(Battle.Player, selectedEnemy, base.Value2);
            yield break;
        }
    }
}
