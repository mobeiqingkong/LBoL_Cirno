using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards;
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
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterWalkingInTheRainSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterWalkingInTheRainSeDef))]
    public sealed class CirnoRCharacterWalkingInTheRainSe : StatusEffect
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

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<CardUsingEventArgs>(
                base.Battle.CardUsing,
                new EventSequencedReactor<CardUsingEventArgs>(this.CardUsing)
            );
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Battle.Player.TurnEnding,
                new EventSequencedReactor<UnitEventArgs>(this.TurnEnding)
            );
            // base.ReactOwnerEvent<DamageEventArgs>(base.Owner.DamageReceived, new EventSequencedReactor<DamageEventArgs>(this.afterAtack));
        }

        private IEnumerable<BattleAction> CardUsing(CardUsingEventArgs args)
        {
            bool flag = Enumerable.Contains<ManaColor>(args.Card.Config.Colors, ManaColor.Green);
            if (flag)
            {
                base.NotifyActivating();
                yield return new CastBlockShieldAction(
                    base.Battle.Player,
                    base.Battle.Player,
                    this.Value1,
                    0,
                    BlockShieldType.Direct,
                    true
                );
            }
            yield break;
        }

        private IEnumerable<BattleAction> TurnEnding(UnitEventArgs args)
        {
            yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }
    }
}
