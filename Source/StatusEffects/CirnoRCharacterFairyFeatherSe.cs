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
    public sealed class CirnoRCharacterFairyFeatherSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFairyFeatherSeDef))]
    public sealed class CirnoRCharacterFairyFeatherSe : StatusEffect
    {
        public int Value1
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
            base.ReactOwnerEvent<CardUsingEventArgs>(
                base.Battle.CardUsing,
                new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsing)
            );
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Battle.Player.TurnEnding,
                new EventSequencedReactor<UnitEventArgs>(this.TurnEnding)
            );
            // base.ReactOwnerEvent<DamageEventArgs>(base.Owner.DamageReceived, new EventSequencedReactor<DamageEventArgs>(this.afterAtack));
        }

        private IEnumerable<BattleAction> OnCardUsing(CardUsingEventArgs args)
        {
            bool flag = Enumerable.Contains<ManaColor>(args.Card.Config.Colors, ManaColor.Green);
            if (flag)
            {
                base.NotifyActivating();
                yield return new ApplyStatusEffectAction<CirnoRCharacterDeathOfNatureSe>(
                    base.Battle.RandomAliveEnemy,
                    this.Value1,
                    new int?(0),
                    new int?(0),
                    new int?(0),
                    0.2f,
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
