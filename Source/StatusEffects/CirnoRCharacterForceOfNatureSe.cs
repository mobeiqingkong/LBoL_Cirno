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
    public sealed class CirnoRCharacterForceOfNatureSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            // config.RelativeEffects = new List<string>() { nameof(TempFirepower) };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterForceOfNatureSeDef))]
    public sealed class CirnoRCharacterForceOfNatureSe : StatusEffect
    {
        public ManaGroup Mana
        {
            get { return ManaGroup.Single(ManaColor.Green); }
        }

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Battle.Player.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted)
            );
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            if (!base.Battle.BattleShouldEnd)
            {
                base.NotifyActivating();
                yield return new GainManaAction(ManaGroup.Greens(base.Level));
            }
            yield break;
        }
    }
}
