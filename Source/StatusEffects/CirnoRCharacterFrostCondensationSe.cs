using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterFrostCondensationSeDef : CirnoRCharacterStatusEffectTemplate
    {
        // // Keywords don't have sprites.
        // public override Sprite LoadSprite() => null;

        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrostCondensationSeDef))]
    public sealed class CirnoRCharacterFrostCondensationSe : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<DamageEventArgs>(base.Owner.DamageDealt, this.afterAtack);
        }

        // private IEnumerable<BattleAction> OnOwnerTurnStarted(UnitEventArgs args)
        // {
        //     //At the start of the Player's turn, gain Spirit.
        //     yield return BuffAction<Spirit>(base.Level, 0, 0, 0, 0.2f);
        //     //This is equivalent to:
        //     //yield return new ApplyStatusEffectAction<CirnoRCharacterTurnGainSpiritSe>(base.Owner, base.Level, 0, 0, 0, 0.2f);
        //     yield break;
        // }

        private IEnumerable<BattleAction> afterAtack(DamageEventArgs args)
        {
            //At the start of the Player's turn, gain Spirit.
            // yield return BuffAction<Spirit>(base.Level, 0, 0, 0, 0.2f);

            if (
                base.Level > 0
                && args.Target != null
                && args.Target.IsAlive
                && args.Target != this.Owner
            )
            {
                base.Level--;
                yield return DebuffAction<CirnoRCharacterFrostSe>(args.Target, 1, 1);
            }
            if (base.Level == 0)
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            // yield return BuffAction<CirnoRCharacterFrostSe>(base.Level, 0, 0, 0, 0.2f);
            // base.Level--;
            //This is equivalent to:
            //yield return new ApplyStatusEffectAction<CirnoRCharacterTurnGainSpiritSe>(base.Owner, base.Level, 0, 0, 0, 0.2f);
            yield break;
        }
    }
}
