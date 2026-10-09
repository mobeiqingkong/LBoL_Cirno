using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    public sealed class CirnoRCharacterFrozenSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.Type = StatusEffectType.Negative;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrozenSeDef))]
    public sealed class CirnoRCharacterFrozenSe : StatusEffect
    {
        public int Value1
        {
            get
            {
                int result = 1;
                if (base.Owner != null && base.Level != 0)
                {
                    result = result * base.Level;
                }
                return result;
            }
        }

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Owner.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted)
            );
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Owner.TurnEnded,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnEnded)
            );
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            EnemyUnit enemy = Owner as EnemyUnit;
            enemy._turnMoves.Clear();
            enemy.ClearIntentions();
            yield break;
        }

        private IEnumerable<BattleAction> OnTurnEnded(UnitEventArgs args)
        {
            if (base.Level > 0)
                base.Level--;
            if (base.Level == 0)
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }
    }
}
