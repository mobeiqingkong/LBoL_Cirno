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
    public sealed class CirnoRCharacterWindGodBoostSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(Graze) };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterWindGodBoostSeDef))]
    public sealed class CirnoRCharacterWindGodBoostSe : StatusEffect
    {
        public int GrazeLevel
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
            // base.ReactOwnerEvent<HealEventArgs>(
            //     base.Owner.Turn,
            //     new EventSequencedReactor<HealEventArgs>(this.OnHealing)
            // );
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnEnding, this.TurnEnding);
            // base.ReactOwnerEvent<DamageEventArgs>(base.Owner.DamageReceived, this.afterDamage);
        }

        private IEnumerable<BattleAction> TurnEnding(UnitEventArgs args)
        {
            yield return BuffAction<Graze>(GrazeLevel);
            if (base.Level > 0)
            {
                base.Level--;
            }
            if (base.Level == 0)
            {
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            }
            yield break;
        }
    }
}
