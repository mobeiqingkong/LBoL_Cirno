using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using HarmonyLib;
using JetBrains.Annotations;
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
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterDeathOfNatureSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.Type = StatusEffectType.Negative;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterDeathOfNatureSeDef))]
    public sealed class CirnoRCharacterDeathOfNatureSe : StatusEffect
    {
        public int loseHpValue
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

        public int loseHpValueForFrost
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

        protected override void OnRemoved(Unit unit)
        {
            if (base.Battle.BattleShouldEnd)
                return;
            int amuletLevel = unit.GetStatusEffect<Amulet>()?.Level ?? 0;
            for (int i = 1; i <= amuletLevel + 1; i++)
            {
                this.React(
                    new ApplyStatusEffectAction<CirnoRCharacterDeathOfNatureSe>(
                        unit,
                        this.Level,
                        0,
                        0,
                        0,
                        0.2f
                    )
                );
            }
        }

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<HealEventArgs>(
                base.Battle.Player.HealingGiving,
                new EventSequencedReactor<HealEventArgs>(this.OnHealing)
            );
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnEnding, this.TurnEnding);
        }

        public IEnumerable<BattleAction> actDamage(Unit unit)
        {
            if (!unit.IsAlive)
            {
                yield break;
            }
            bool flag = base.Owner.HasStatusEffect<CirnoRCharacterFrostbiteSe>();
            int addDamage = 0;
            if (flag)
            {
                addDamage = loseHpValueForFrost;
            }
            yield return new DamageAction(
                unit,
                unit,
                DamageInfo.HpLose(loseHpValue + addDamage),
                GunNameID.GetGunFromId(501)
            );
            yield break;
        }

        private IEnumerable<BattleAction> TurnEnding(UnitEventArgs args)
        {
            foreach (var action in this.actDamage(base.Owner))
            {
                yield return action;
            }
            yield break;
        }

        private IEnumerable<BattleAction> OnHealing(HealEventArgs args)
        {
            foreach (var action in this.actDamage(base.Owner))
            {
                yield return action;
            }
            yield break;
        }
    }
}
