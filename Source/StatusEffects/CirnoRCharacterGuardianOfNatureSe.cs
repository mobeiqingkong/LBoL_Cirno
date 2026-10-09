using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
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
    public sealed class CirnoRCharacterGuardianOfNatureSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            // config.HasCount = true;
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterGuardianOfNatureSeDef))]
    public sealed class CirnoRCharacterGuardianOfNatureSe : StatusEffect
    {
        public int Value1
        {
            get
            {
                int result = 3;
                // if (base.Owner != null && base.Level != 0)
                // {
                //     result = result * base.Level;
                // }
                return result;
            }
        }

        protected override void OnAdded(Unit unit)
        {
            // base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnStarted, this.OnOwnerTurnStarted);
            base.ReactOwnerEvent<DamageEventArgs>(
                base.Owner.DamageReceived,
                new EventSequencedReactor<DamageEventArgs>(this.afterAtack)
            );
        }

        private IEnumerable<BattleAction> afterAtack(DamageEventArgs args)
        {
            //At the start of the Player's turn, gain Spirit.
            // yield return BuffAction<Spirit>(base.Level, 0, 0, 0, 0.2f);
            if (
                base.Level > 0
                && args.Source != null
                && args.Source != this.Owner
                && args.Source.IsAlive
            )
            {
                base.Level--;
                yield return new ApplyStatusEffectAction<CirnoRCharacterDeathOfNatureSe>(
                    args.Source,
                    Value1,
                    0,
                    0,
                    0,
                    0.2f
                );
            }
            if (base.Level == 0)
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }
    }
}
