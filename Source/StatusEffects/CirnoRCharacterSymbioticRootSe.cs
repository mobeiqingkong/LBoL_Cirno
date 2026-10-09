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
    public sealed class CirnoRCharacterSymbioticRootSeDef : CirnoRCharacterStatusEffectTemplate
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

    [EntityLogic(typeof(CirnoRCharacterSymbioticRootSeDef))]
    public sealed class CirnoRCharacterSymbioticRootSe : StatusEffect
    {
        public int hpValue { get; set; } = 0;

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<DamageEventArgs>(
                base.Owner.DamageTaking,
                new EventSequencedReactor<DamageEventArgs>(this.damageTaking)
            );
            base.ReactOwnerEvent<DamageEventArgs>(
                base.Owner.DamageReceived,
                new EventSequencedReactor<DamageEventArgs>(this.damageReceived)
            );
        }

        private IEnumerable<BattleAction> damageTaking(DamageEventArgs args)
        {
            this.hpValue = base.Owner.Hp;
            yield break;
        }

        private IEnumerable<BattleAction> damageReceived(DamageEventArgs args)
        {
            if (args.Source != Owner && args.Source.IsAlive && hpValue > base.Owner.Hp)
            {
                CirnoRCharacterDeathOfNatureSe se =
                    args.Source.GetStatusEffect<CirnoRCharacterDeathOfNatureSe>();
                if (se != null && se.Level > 0)
                {
                    foreach (var action in se.actDamage(args.Source))
                    {
                        yield return action;
                    }
                }
            }
            // hpValue = base.Owner.Hp;

            yield break;
        }
    }
}
