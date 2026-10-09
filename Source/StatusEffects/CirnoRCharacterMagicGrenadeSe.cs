using System;
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
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterMagicGrenadeSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.Type = StatusEffectType.Negative;
            config.IsStackable = true;
            config.HasLevel = false;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterMagicGrenadeSeDef))]
    public sealed class CirnoRCharacterMagicGrenadeSe : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            // base.ReactOwnerEvent<HealEventArgs>(
            //     base.Owner.Turn,
            //     new EventSequencedReactor<HealEventArgs>(this.OnHealing)
            // );
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnStarted, this.OnTurnStarted);
            // base.ReactOwnerEvent<DamageEventArgs>(base.Owner.DamageReceived, this.afterDamage);
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            EnemyUnit enemy = Owner as EnemyUnit;
            System.Random random = new System.Random();
            int chance = 30; // 30% 概率
            if (random.Next(100) < chance) // 生成一个0到99的随机数，如果小于10，则有10%的概率成立
            {
                enemy._turnMoves.Clear();
                enemy.ClearIntentions();
            }
            yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }
    }
}
