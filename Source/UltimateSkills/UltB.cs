using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.Cards;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using JetBrains.Annotations;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.CirnoRCharacterUlt
{
    public sealed class CirnoRCharacterUltBDef : CirnoRCharacterUltTemplate
    {
        public override UltimateSkillConfig MakeConfig()
        {
            UltimateSkillConfig config = GetDefaulUltConfig();
            config.Damage = 0;
            config.Value1 = 1;
            config.Value2 = 2;
            // config.Keywords = Keyword.Shield;
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDaiyouseiBlessSe) };
            config.RelativeCards = new List<string>() { nameof(CirnoRCharacterFairyFlower) };
            return config;
        }
    }

    // [EntityLogic(typeof(CirnoRCharacterUltBDef))]
    // public sealed class CirnoRCharacterUltB : UltimateSkill
    // {
    //     public CirnoRCharacterUltB()
    //     {
    //         base.TargetType = TargetType.SingleEnemy;
    //         base.GunName = GunNameID.GetGunFromId(4158);
    //     }

    //     protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
    //     {
    //         EnemyUnit enemy = selector.GetEnemy(base.Battle);
    //         yield return new DamageAction(base.Owner, enemy, this.Damage, base.GunName, GunType.Single);
    //         yield break;
    //     }
    // }

    [EntityLogic(typeof(CirnoRCharacterUltBDef))]
    public sealed class CirnoRCharacterUltB : UltimateSkill
    {
        public CirnoRCharacterUltB()
        {
            base.TargetType = TargetType.AllEnemies;
            base.GunName = GunNameID.GetGunFromId(4158);
        }

        // protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        // {
        //     EnemyUnit enemy = selector.GetEnemy(base.Battle);
        //     yield return new DamageAction(base.Owner, enemy, this.Damage, base.GunName, GunType.Single);
        //     yield break;
        // }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {
            // EnemyUnit enemy = selector.GetEnemy(base.Battle);
            // yield return new DamageAction(base.Owner, enemy, this.Damage, base.GunName, GunType.Single);

            // //Only apply the status effect if the enemy is still alive after the attack.
            // if (enemy.IsAlive)
            // {
            //     yield return new ApplyStatusEffectAction<LockedOn>(enemy, base.Value1, 0, 0, 0, 0.2f);
            //     yield return new ApplyStatusEffectAction<Weak>(enemy, 0, base.Value1, 0, 0, 0.2f);
            //     yield return new ApplyStatusEffectAction<Vulnerable>(enemy, 0, base.Value1, 0, 0, 0.2f);
            //     yield break;
            // }

            // yield return new CastBlockShieldAction(Battle.Player, Battle.Player, 0, base.Value1, BlockShieldType.Direct, true);
            yield return new ApplyStatusEffectAction<CirnoRCharacterDaiyouseiBlessSe>(
                base.Battle.Player,
                base.Value1,
                0,
                0,
                0,
                0.2f
            );

            // List<SummerFlower> list = Library.CreateCards<SummerFlower>(base.Value2, false);

            // List<SummerFlower> list = Library.CreateCards<SummerFlower>(Value2, false).ToList();
            List<CirnoRCharacterFairyFlower> cards = Library
                .CreateCards<CirnoRCharacterFairyFlower>(2, false)
                .ToList<CirnoRCharacterFairyFlower>();
            foreach (Card card in cards)
            {
                card.IsAccuracy = true;
                card.SetTurnCost(new ManaGroup() { Any = 0 });
            }
            yield return new AddCardsToHandAction(cards);
            // foreach (Unit enemy in base.Battle.AllAliveEnemies)
            // {
            //     // yield return new DamageAction(base.Owner, enemy, this.Damage, base.GunName, GunType.Single);

            //     //Only apply the status effect if the enemy is still alive after the attack.
            //     if (enemy.IsAlive)
            //     {
            //         yield return new ApplyStatusEffectAction<CirnoRCharacterDaiyouseiBlessSe>(enemy, base.Value2, 0, 0, 0, 0.2f);
            //         // yield return new ApplyStatusEffectAction<Weak>(enemy, 0, base.Value1, 0, 0, 0.2f);
            //         // yield return new ApplyStatusEffectAction<Vulnerable>(enemy, 0, base.Value1, 0, 0, 0.2f);
            //     }
            // }
            // IEnumerator<EnemyUnit> enumerator = null;
            // bool battleShouldEnd2 = base.Battle.BattleShouldEnd;
            // if (battleShouldEnd2)
            // {
            //     yield break;
            // }
            // int num;
            // for (int i = 0; i < base.Value2; i = num + 1)
            // {
            //     yield return new DamageAction(base.Owner, base.Battle.AllAliveEnemies, this.Damage, base.GunName, 0);
            //     num = i;
            // }
            // yield break;
            yield break;
        }
    }

    // [EntityLogic(typeof(CirnoRCharacterUltBDef))]
    // public sealed class CirnoRCharacterUltB : UltimateSkill
    // {
    //     // public CirnoRCharacterUltB()
    //     // {
    //     //     base.TargetType = TargetType.SingleEnemy;
    //     //     base.GunName = GunNameID.GetGunFromId(4158);
    //     // }

    //     // protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
    //     // {
    //     //     //Attack all enemies, selector is set to Battle.AllAliveEnemies.
    //     //     yield return base.AttackAction(selector, base.GunName);
    //     //     //If the battle were to end, skip the rest of the method.
    //     //     if (base.Battle.BattleShouldEnd)
    //     //     {
    //     //         yield break;
    //     //     }
    //     //     //Apply the Lock On Debuff to all alive enemies.

    //     //     foreach (BattleAction battleAction in DebuffAction<LockedOn>(base.Battle.AllAliveEnemies, base.Value1, 0, 0, 0, true, 0.2f))
    //     //     {
    //     //         yield return battleAction;
    //     //     }
    //     //     yield break;
    //     // }
    // }
}
