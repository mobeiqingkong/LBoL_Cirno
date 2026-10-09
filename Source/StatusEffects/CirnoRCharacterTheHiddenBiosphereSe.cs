using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterTheHiddenBiosphereSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            config.IsStackable = true;
            config.HasLevel = false;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterTheHiddenBiosphereSeDef))]
    public sealed class CirnoRCharacterTheHiddenBiosphereSe : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            // base.ReactOwnerEvent<HealEventArgs>(
            //     base.Owner.Turn,
            //     new EventSequencedReactor<HealEventArgs>(this.OnHealing)
            // );
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnEnding, this.TurnEnding);
            // base.ReactOwnerEvent<DamageEventArgs>(base.Owner.DamageReceived, this.afterDamage);
        }

        private IEnumerable<BattleAction> TurnEnding(UnitEventArgs args)
        {
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
            yield break;
        }
    }
}
