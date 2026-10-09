using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFullyArmedDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Defense;
            config.TargetType = TargetType.Self;

            config.Block = 10;
            config.UpgradedBlock = 12;

            config.Value1 = 10;
            config.UpgradedValue1 = 12;

            config.Value2 = 3;
            config.UpgradedValue2 = 4;

            config.Illustrator = "";

            config.Keywords = Keyword.Block | Keyword.Exile;
            config.UpgradedKeywords = Keyword.Block | Keyword.Exile;

            config.RelativeEffects = new List<string>()
            {
                nameof(FrostArmor),
                nameof(CirnoRCharacterDeathOfNatureSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(FrostArmor),
                nameof(CirnoRCharacterDeathOfNatureSe),
            };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFullyArmedDef))]
    public sealed class CirnoRCharacterFullyArmed : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return new CastBlockShieldAction(Battle.Player, Battle.Player, base.Block, true);
            int totalNum = 0;
            IEnumerable<EnemyUnit> enemies = base.Battle.AllAliveEnemies;
            foreach (EnemyUnit enemy in enemies)
            {
                CirnoRCharacterDeathOfNatureSe cirnoRCharacterDeathOfNatureSe =
                    enemy.GetStatusEffect<CirnoRCharacterDeathOfNatureSe>();
                if (
                    cirnoRCharacterDeathOfNatureSe != null
                    && cirnoRCharacterDeathOfNatureSe.Level > 0
                )
                {
                    totalNum++;
                }
            }
            if (totalNum > 0)
            {
                yield return BuffAction<FrostArmor>(totalNum * Value2, totalNum * Value2);
            }
            // yield return new CastBlockShieldAction(Battle.Player, Battle.Player, base.Value1 + Value2 * totalNum, 0, BlockShieldType.Direct, true);
            yield break;
        }
    }
}
