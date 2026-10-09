using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
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
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterNaturalFieldSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            config.IsStackable = false;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterNaturalFieldSeDef))]
    public sealed class CirnoRCharacterNaturalFieldSe : StatusEffect
    {
        public int Value1
        {
            get
            {
                int result = 4;
                // if (base.Owner != null && base.Level != 0)
                // {
                //     result = result * base.Level;
                // }
                return result;
            }
        }

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Owner.TurnEnded,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnEnded)
            );
        }

        private IEnumerable<BattleAction> OnTurnEnded(UnitEventArgs args)
        {
            if (base.Level > 0)
            {
                base.Level--;
                foreach (Unit enemy in base.Battle.AllAliveEnemies)
                {
                    yield return DebuffAction<CirnoRCharacterDeathOfNatureSe>(
                        enemy,
                        Value1,
                        Value1,
                        0,
                        0,
                        true,
                        0.2f
                    );
                }
            }
            if (base.Level == 0)
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }
    }
}
