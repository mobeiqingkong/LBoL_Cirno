using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterFrostflowSeDef : CirnoRCharacterStatusEffectTemplate
    {
        // // Keywords don't have sprites.
        // public override Sprite LoadSprite() => null;

        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostCondensationSe),
            };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrostflowSeDef))]
    public sealed class CirnoRCharacterFrostflowSe : StatusEffect
    {
        public int CirnoRCharacterFrostValue
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
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnStarted, this.OnOwnerTurnStarted);
        }

        private IEnumerable<BattleAction> OnOwnerTurnStarted(UnitEventArgs args)
        {
            //At the start of the Player's turn, gain Spirit.
            yield return BuffAction<CirnoRCharacterFrostCondensationSe>(
                this.CirnoRCharacterFrostValue,
                0,
                0,
                0,
                0.2f
            );
            //This is equivalent to:
            //yield return new ApplyStatusEffectAction<CirnoRCharacterTurnGainSpiritSe>(base.Owner, base.Level, 0, 0, 0, 0.2f);
            yield break;
        }
    }
}
