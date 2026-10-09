using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
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
    public sealed class CirnoRCharacterNaturalPlagueDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Green = 1, Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.SingleEnemy;

            config.Value1 = 8;
            config.UpgradedValue1 = 8;

            config.Value2 = 4;
            config.UpgradedValue2 = 4;
            // config.Keywords = Keyword.Exile;
            // config.UpgradedKeywords = Keyword.Exile;
            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDeathOfNatureSe),
            };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterNaturalPlagueDef))]
    public sealed class CirnoRCharacterNaturalPlague : CirnoRCharacterCard
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
                    enemy == selector.SelectedEnemy ? Value1 : Value2,
                    0,
                    0,
                    0,
                    true,
                    0.2f
                );
            }
            if (IsUpgraded)
            {
                foreach (EnemyUnit enemy in base.Battle.AllAliveEnemies)
                {
                    CirnoRCharacterDeathOfNatureSe se =
                        enemy.GetStatusEffect<CirnoRCharacterDeathOfNatureSe>();
                    if (se != null && se.Level > 0)
                    {
                        foreach (var action in se.actDamage(enemy))
                        {
                            yield return action;
                        }
                    }
                }
            }
            yield break;
        }
    }
}
