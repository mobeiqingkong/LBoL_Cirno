using System.Collections.Generic;
using System.Linq;
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
    public sealed class CirnoRCharacterFrozenHexagramShieldSeDef
        : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(LockedOn) };
            // config.DurationDecreaseTiming = DurationDecreaseTiming.NormalTurnStart;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrozenHexagramShieldSeDef))]
    public sealed class CirnoRCharacterFrozenHexagramShieldSe : StatusEffect
    {
        public int ShieldValue
        {
            get
            {
                int result = 6;
                if (base.Owner != null && base.Level != 0)
                {
                    result = result * base.Level;
                }
                return result;
            }
        }

        public int LockedOnValue
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
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnEnding, this.TurnEnding);
        }

        private IEnumerable<BattleAction> TurnEnding(UnitEventArgs args)
        {
            yield return new CastBlockShieldAction(
                base.Battle.Player,
                base.Battle.Player,
                0,
                this.ShieldValue,
                BlockShieldType.Direct,
                true
            );
            yield return new ApplyStatusEffectAction<LockedOn>(
                base.Battle.Player,
                this.LockedOnValue,
                new int?(0),
                new int?(0),
                new int?(0),
                0.2f,
                true
            );
            yield break;
        }
    }
}
