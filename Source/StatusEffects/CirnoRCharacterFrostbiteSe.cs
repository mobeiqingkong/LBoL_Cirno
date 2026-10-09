using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using DG.Tweening;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.BattleModifiers.Actions;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterFrostbiteSeDef : CirnoRCharacterStatusEffectTemplate
    {
        // // Keywords don't have sprites.
        // public override Sprite LoadSprite() => null;

        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrozenSe) };
            config.HasCount = true;
            config.Type = StatusEffectType.Negative;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrostbiteSeDef))]
    public sealed class CirnoRCharacterFrostbiteSe : StatusEffect
    {
        public int BaseDamage
        {
            get
            {
                int result = 1;
                if (base.Owner != null && base.Level != 0)
                {
                    result = result * base.Level;
                }
                int cirnoRCharacterMoistSeLevel = base.GetSeLevel<CirnoRCharacterMoistSe>();
                return result + cirnoRCharacterMoistSeLevel;
            }
        }

        public int StackMultiply
        {
            get
            {
                int cirnoRCharacterFrostUpSeLevel = base.GetSeLevel<CirnoRCharacterFrostUpSe>();
                return cirnoRCharacterFrostUpSeLevel + 1;
            }
        }

        public int StackDamage
        {
            get { return this.BaseDamage * this.StackMultiply; }
        }

        public int damageCount
        {
            get { return 9; }
        }

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<LBoL.Core.StatusEffectApplyEventArgs>(
                base.Owner.StatusEffectAdding,
                new EventSequencedReactor<StatusEffectApplyEventArgs>(this.OnBuffAttack)
            );
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Owner.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted)
            );
        }

        private IEnumerable<BattleAction> OnBuffAttack(StatusEffectApplyEventArgs args)
        {
            bool flag = base.Battle.Player.HasStatusEffect<CirnoRCharacterFrostDomainSe>();
            if (flag && args.Effect is CirnoRCharacterFrostSe && args.Unit == base.Owner)
            {
                foreach (var action in this.actDamage(base.Owner))
                {
                    yield return action;
                }
            }
            yield break;
        }

        public IEnumerable<BattleAction> actDamage(Unit unit)
        {
            yield return DamageAction.LoseLife(unit, StackDamage);
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            // yield return new DamageAction(
            //     Owner,
            //     Owner,
            //     DamageInfo.HpLose(StackDamage),
            //     GunNameID.GetGunFromId(862)
            // );
            foreach (var action in this.actDamage(base.Owner))
            {
                yield return action;
            }
            this.Count++;
            if (this.Count >= damageCount)
            {
                this.Count = this.Count - damageCount;
                this.React(DebuffAction<CirnoRCharacterFrozenSe>(Owner, 1, 1, 0, 0, true, 0.2f));
            }
            // yield return DamageAction.LoseLife(base.Owner, StackDamage);
            // this.React(DamageAction.LoseLife(base.Owner, StackDamage));
            // bool flag = base.Battle.Player.HasStatusEffect<CirnoRCharacterFrostDomainSe>();
            // if (this.Level >= 1 && !flag)
            // {
            //     this.Level = (int)Level / 2;
            // }
            //this.Count++;
            yield break;
        }

        // public override bool Stack(StatusEffect other)
        // {
        //     bool flag = base.Stack(other);
        //     if (flag)
        //     {
        //         this.React(DamageAction.LoseLife(base.Owner, StackDamage));
        //         this.Count++;
        //         if (this.Count >= damageCount)
        //         {
        //             this.Count = this.Count - damageCount;
        //             foreach (EnemyUnit enemy in this.Battle.AllAliveEnemies)
        //             {
        //                 this.React(
        //                     DebuffAction<CirnoRCharacterFrozenSe>(Owner, 1, 1, 0, 0, true, 0.2f)
        //                 );
        //             }
        //         }
        //     }
        //     return flag;
        // }
    }
}
