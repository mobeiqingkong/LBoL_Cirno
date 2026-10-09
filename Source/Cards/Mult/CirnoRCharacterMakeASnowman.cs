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
    public sealed class CirnoRCharacterMakeASnowmanDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green, ManaColor.Blue };
            config.Cost = new ManaGroup()
            {
                Green = 1,
                Blue = 1,
                Any = 1,
            };
            config.UpgradedCost = new ManaGroup()
            {
                Green = 1,
                Blue = 1,
                Any = 1,
            };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Shield = 4;
            config.UpgradedShield = 8;

            config.Value1 = 4;
            config.UpgradedValue1 = 8;

            config.Value2 = 2;
            config.UpgradedValue2 = 2;

            config.Illustrator = "";

            config.Keywords = Keyword.Shield;
            config.UpgradedKeywords = Keyword.Shield;

            config.RelativeEffects = new List<string>() { nameof(Graze) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(Graze) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterMakeASnowmanDef))]
    public sealed class CirnoRCharacterMakeASnowman : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // int totalLevel = 0;
            // IEnumerable<EnemyUnit> enemies = base.Battle.AllAliveEnemies;
            // foreach (EnemyUnit enemy in enemies)
            // {
            //     CirnoRCharacterDeathOfNatureSe cirnoRCharacterDeathOfNatureSe = enemy.GetStatusEffect<CirnoRCharacterDeathOfNatureSe>();
            //     if (cirnoRCharacterDeathOfNatureSe != null && cirnoRCharacterDeathOfNatureSe.Level > 0)
            //     {
            //         totalLevel = totalLevel + cirnoRCharacterDeathOfNatureSe.Level;
            //     }
            // }
            yield return new CastBlockShieldAction(Battle.Player, Battle.Player, base.Shield, true);
            // yield return BuffAction<FrostArmor>(Value1, Value1);
            // yield return BuffAction<Graze>(Value2, Value2);
            yield return BuffAction<Graze>(Value2, Value2);
            yield break;
        }
    }
}
