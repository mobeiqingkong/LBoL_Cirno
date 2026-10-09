using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.GunName;
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
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterIceTearsSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            // config.HasCount = false;
            // config.IsStackable = true;
            config.HasLevel = false;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterIceTearsSeDef))]
    public sealed class CirnoRCharacterIceTearsSe : StatusEffect
    {
        private int tmpHpValue { get; set; } = 0;

        public int Value1
        {
            get
            {
                int result = 5;
                // if (base.Owner != null && base.Level != 0)
                // {
                //     result = result * base.Level;
                // }
                return result;
            }
        }

        public int Value2
        {
            get
            {
                int result = 1;
                // if (base.Owner != null && base.Level != 0)
                // {
                //     result = result * base.Level;
                // }
                return result;
            }
        }

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

            // base.ReactOwnerEvent<DamageEventArgs>(base.Owner.DamageReceived, this.afterDamage);
        }

        private IEnumerable<BattleAction> damageTaking(DamageEventArgs args)
        {
            this.tmpHpValue = base.Owner.Hp;
            yield break;
        }

        private IEnumerable<BattleAction> damageReceived(DamageEventArgs args)
        {
            if (args.Source != Owner && args.Source.IsAlive && tmpHpValue > base.Owner.Hp)
            {
                yield return new ApplyStatusEffectAction<CirnoRCharacterFrostSe>(
                    args.Source,
                    Value2,
                    new int?(0),
                    new int?(0),
                    new int?(0),
                    0.2f,
                    true
                );
                CirnoRCharacterFrostbiteSe se =
                    args.Source.GetStatusEffect<CirnoRCharacterFrostbiteSe>();
                if (se != null && se.Level > 0)
                {
                    foreach (var action in se.actDamage(args.Source))
                    {
                        yield return action;
                    }
                }
            }
            yield break;
        }
    }
}
