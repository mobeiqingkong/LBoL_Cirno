using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using DG.Tweening;
using HarmonyLib;
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
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterFrostSeDef : CirnoRCharacterStatusEffectTemplate
    {
        // // Keywords don't have sprites.
        // public override Sprite LoadSprite() => null;

        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.Type = StatusEffectType.Negative;
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostbiteSe) };
            config.IsStackable = true;
            config.HasLevel = false;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrostSeDef))]
    public sealed class CirnoRCharacterFrostSe : StatusEffect
    {
        public int TempFirepowerNegativeValue
        {
            get { return 1; }
        }

        public int BaseDamage
        {
            get
            {
                int result = 1;
                return result + base.GetSeLevel<CirnoRCharacterMoistSe>();
            }
        }

        public static bool CanPlayEffect { get; set; }

        protected override void OnAdded(Unit unit)
        {
            int cirnoRCharacterIceSoulLevel = base.GetSeLevel<CirnoRCharacterIceSoulSe>();
            if (cirnoRCharacterIceSoulLevel > 0)
            {
                this.React(
                    new CastBlockShieldAction(
                        base.Battle.Player,
                        base.Battle.Player,
                        0,
                        cirnoRCharacterIceSoulLevel * 3,
                        BlockShieldType.Direct,
                        true
                    )
                );
            }

            // base.HandleOwnerEvent<GameEventArgs>(
            //     base.Battle.AllEnemyTurnStarting,
            //     delegate(GameEventArgs _)
            //     {
            //         CanPlayEffect = true;
            //     }
            // );
            // base.ReactOwnerEvent<GameEventArgs>(
            //     base.Battle.AllEnemyTurnStarted,
            //     new EventSequencedReactor<GameEventArgs>(this.OnAllEnemyTurnStarted)
            // );
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Owner.TurnStarted,
                this.OnOwnerTurnStarted
            // new EventSequencedReactor<UnitEventArgs>()
            );
            // this.React(PerformAction.Animation(unit, "hit", 0.3f, null, 0f, -1));
        }

        // private IEnumerable<BattleAction> OnAllEnemyTurnStarted(GameEventArgs args)
        // {
        //     if (base.Owner == null || base.Battle.BattleShouldEnd)
        //     {
        //         yield break;
        //     }
        //     // if (CanPlayEffect)
        //     // {
        //     //     CanPlayEffect = false;
        //     //     yield return PerformAction.Effect(
        //     //         base.Battle.Player,
        //     //         "ColdLaunch",
        //     //         0f,
        //     //         null,
        //     //         0f,
        //     //         PerformAction.EffectBehavior.PlayOneShot,
        //     //         0f
        //     //     );
        //     // }
        //     // yield return DamageAction.LoseLife(base.Owner, this.BaseDamage, "Cold1");
        //     yield return DebuffAction<TempFirepowerNegative>(
        //         Owner,
        //         TempFirepowerNegativeValue,
        //         TempFirepowerNegativeValue,
        //         0,
        //         0,
        //         true,
        //         0.2f
        //     );
        //     yield return new RemoveStatusEffectAction(this, true, 0.1f);
        //     yield break;
        // }

        private IEnumerable<BattleAction> OnOwnerTurnStarted(UnitEventArgs args)
        {
            yield return DebuffAction<TempFirepowerNegative>(
                Owner,
                TempFirepowerNegativeValue,
                TempFirepowerNegativeValue,
                0,
                0,
                true,
                0.2f
            );
            yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }

        public override bool Stack(StatusEffect other)
        {
            if (Owner.IsAlive)
            {
                this.React(DamageAction.LoseLife(base.Owner, this.BaseDamage));
                this.React(
                    new ApplyStatusEffectAction<CirnoRCharacterFrostbiteSe>(Owner, 1, 0, 0, 0, 0.2f)
                );
                // CirnoRCharacterFrostbiteSe se = Owner.GetStatusEffect<CirnoRCharacterFrostbiteSe>();
                // if (se != null)
                // {
                //     foreach (var action in se.actDamage(Owner))
                //     {
                //         this.React(action);
                //     }
                // }
            }
            bool flag = base.Stack(other);
            return flag;
        }
    }
}
