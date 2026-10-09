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
using LBoL.Core.Intentions;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterAlertDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1, Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Defense;
            config.TargetType = TargetType.Self;

            config.Value1 = 6;
            config.UpgradedValue1 = 8;

            config.Value2 = 6;
            config.UpgradedValue2 = 8;

            config.Block = 6;
            config.UpgradedBlock = 8;

            config.Shield = 6;
            config.UpgradedShield = 8;

            config.Keywords = Keyword.Shield | Keyword.Block;
            config.UpgradedKeywords = Keyword.Shield | Keyword.Block;

            config.Illustrator = "ろけ";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterAlertDef))]
    public sealed class CirnoRCharacterAlert : CirnoRCharacterCard
    {
        public override bool Triggered
        {
            get
            {
                IEnumerable<EnemyUnit> enemies = base.Battle.AllAliveEnemies;
                foreach (EnemyUnit enemy in enemies)
                {
                    //Check whether any enemy intends to attack.
                    bool intendsToAttack = enemy.Intentions.Any(
                        delegate(Intention i)
                        {
                            if (!(i is AttackIntention))
                            {
                                //If the enemy intent is a spellcard, check whether it deals damage.
                                SpellCardIntention spellCardIntention = i as SpellCardIntention;
                                if (spellCardIntention == null || spellCardIntention.Damage == null)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    );
                    if (intendsToAttack)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return new CastBlockShieldAction(Battle.Player, Battle.Player, base.Block, true);
            yield return BuffAction<Reflect>(Value2, Value2);
            if (base.PlayInTriggered)
            {
                yield return new CastBlockShieldAction(
                    Battle.Player,
                    Battle.Player,
                    base.Shield,
                    true
                );
            }
            yield break;
        }
    }
}
