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
    public sealed class CirnoRCharacterMuggleWandSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.Type = StatusEffectType.Negative;
            config.IsStackable = false;
            config.HasLevel = false;
            config.LevelStackType = StackType.Keep;
            config.HasDuration = false;
            config.DurationStackType = StackType.Keep;
            config.DurationDecreaseTiming = DurationDecreaseTiming.TurnEnd;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterMuggleWandSeDef))]
    public sealed class CirnoRCharacterMuggleWandSe : StatusEffect
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
            int maxHp = Owner.MaxHp;
            yield return new DamageAction(
                Owner,
                Owner,
                DamageInfo.HpLose((int)Math.Ceiling(maxHp * 0.03f)),
                GunNameID.GetGunFromId(862)
            ); // 4660
            yield break;
        }
    }
}
