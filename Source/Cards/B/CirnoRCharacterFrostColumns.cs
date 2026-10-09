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
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFrostColumnsDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(952);
            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 1, Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1, Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Defense;
            config.TargetType = TargetType.Self;

            config.Block = 6;
            config.UpgradedBlock = 8;

            config.Shield = 6;
            config.UpgradedShield = 8;

            config.Value1 = 6;
            config.UpgradedValue1 = 8;

            config.Value2 = 2;
            config.UpgradedValue2 = 2;

            config.Illustrator = "";

            config.Keywords = Keyword.Shield | Keyword.Block;
            config.UpgradedKeywords = Keyword.Shield | Keyword.Block;
            config.RelativeEffects = new List<string>() { nameof(TempFirepowerNegative) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(TempFirepowerNegative) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrostColumnsDef))]
    public sealed class CirnoRCharacterFrostColumns : CirnoRCharacterCard
    {
        //Code taken from the base game (Heart Blossoms / SatoriAttack.cs)
        //If any enemy intends to attack, returns True.
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
            // yield return new ApplyStatusEffectAction<Reflect>(Battle.Player, base.Value1, 0, 0, 0, 0.2f);
            yield return new CastBlockShieldAction(
                Battle.Player,
                Battle.Player,
                base.Block,
                base.Shield,
                true
            );
            if (base.PlayInTriggered)
            {
                foreach (Unit enemy in base.Battle.AllAliveEnemies)
                {
                    yield return DebuffAction<TempFirepowerNegative>(
                        enemy,
                        base.Value2,
                        0,
                        0,
                        0,
                        true,
                        0.2f
                    );
                }
                // yield return BuffAction<FreezeBulletSe>(Value2, Value2);
                // yield return new CastBlockShieldAction(Battle.Player, Battle.Player, base.Value2, 0, BlockShieldType.Direct, true);
                // yield return new ApplyStatusEffectAction<Reflect>(Battle.Player, base.Value1, 0, 0, 0, 0.2f);
                // yield return new ApplyStatusEffectAction<TempElectric>(Battle.Player, base.Value2, 0, 0, 0, 0.2f);
                //Warning: TempElectric is different from Electric.
                //TempElectric only lasts for the rest of the turn. Electric lasts for the rest of combat.
            }
            yield break;
        }
    }
}
