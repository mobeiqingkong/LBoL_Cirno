using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using DG.Tweening;
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
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using Mono.CompilerServices.SymbolWriter;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterWitherDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(501);

            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 2 };
            config.UpgradedCost = new ManaGroup() { Green = 1 };
            config.Rarity = Rarity.Rare;

            config.IsXCost = true;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 2;
            config.UpgradedDamage = 4;

            config.Value1 = 3;
            config.UpgradedValue1 = 3;

            config.Value2 = 6;
            config.UpgradedValue2 = 8;

            config.Illustrator = "";
            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            config.Mana = new ManaGroup() { Any = 2 };

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDeathOfNatureSe),
            };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterWitherDef))]
    public sealed class CirnoRCharacterWither : CirnoRCharacterCard
    {
        private ManaGroup _greenMana = new ManaGroup() { Green = 2 };
        public ManaGroup GreenMana
        {
            get { return _greenMana; }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // base.CardGuns = new Guns(base.GunName, 1, false);
            if (IsUpgraded)
            {
                foreach (Unit enemy in base.Battle.AllAliveEnemies)
                {
                    yield return DebuffAction<CirnoRCharacterDeathOfNatureSe>(
                        enemy,
                        base.Value1,
                        base.Value1
                    );
                }
            }

            int greenMana = base.SynergyAmount(consumingMana, ManaColor.Green, 1);

            foreach (Unit enemy in base.Battle.AllAliveEnemies)
            {
                yield return DebuffAction<CirnoRCharacterDeathOfNatureSe>(
                    enemy,
                    greenMana * base.Value1,
                    greenMana * base.Value1
                );
            }

            int anyMana = base.SynergyAmount(consumingMana, ManaColor.Any, 2);
            for (int i = 0; i < anyMana; i++)
            {
                // foreach (GunPair gunPair in base.CardGuns.GunPairs)
                // {
                //     yield return base.AttackAllAliveEnemyAction(gunPair);
                // }
                // GunPair gunPair = base.CardGuns.GunPairs[0];
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

            // // int totalLevel = 0;
            // IEnumerable<EnemyUnit> enemies = base.Battle.AllAliveEnemies;
            // foreach (EnemyUnit enemy in enemies)
            // {
            //     CirnoRCharacterDeathOfNatureSe cirnoRCharacterDeathOfNatureSe =
            //         enemy.GetStatusEffect<CirnoRCharacterDeathOfNatureSe>();
            //     if (
            //         cirnoRCharacterDeathOfNatureSe != null
            //         && cirnoRCharacterDeathOfNatureSe.Level > 0
            //     )
            //     {
            //         // totalLevel = totalLevel + cirnoRCharacterDeathOfNatureSe.Level;
            //         yield return new DamageAction(
            //             enemy,
            //             enemy,
            //             DamageInfo.HpLose(cirnoRCharacterDeathOfNatureSe.Level),
            //             GunNameID.GetGunFromId(4660)
            //         );
            //         yield return new DamageAction(
            //             enemy,
            //             enemy,
            //             DamageInfo.HpLose(cirnoRCharacterDeathOfNatureSe.Level),
            //             GunNameID.GetGunFromId(4660)
            //         );
            //     }
            // }
            // if (totalLevel > 0)
            // {
            //     foreach (EnemyUnit enemy in enemies)
            //     {
            //         CirnoRCharacterDeathOfNatureSe cirnoRCharacterDeathOfNatureSe = enemy.GetStatusEffect<CirnoRCharacterDeathOfNatureSe>();
            //         yield return new DamageAction(enemy, enemy, DamageInfo.HpLose(base.Value1 * totalLevel), GunNameID.GetGunFromId(4660));
            //         // if (cirnoRCharacterDeathOfNatureSe != null && enemy.IsAlive)
            //         // {
            //         //     yield return new RemoveStatusEffectAction(cirnoRCharacterDeathOfNatureSe, true, 0.2f);
            //         // }

            //     }
            // }
            // yield break;
        }
    }
}
