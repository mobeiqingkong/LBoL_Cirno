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
    public sealed class CirnoRCharacterSpringAndTheFairySeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterSpringAndTheFairySeDef))]
    public sealed class CirnoRCharacterSpringAndTheFairySe : StatusEffect
    {
        public int Value1
        {
            get
            {
                int result = 3;
                if (base.Owner != null && base.Level != 0)
                {
                    result = result * base.Level;
                }
                return result;
            }
        }

        public int Value2
        {
            get
            {
                int result = 3;
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
                this.Value1,
                BlockShieldType.Direct,
                true
            );
            yield return new HealAction(base.Battle.Player, base.Battle.Player, this.Value2);

            yield break;
        }
    }
}
