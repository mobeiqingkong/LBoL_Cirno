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
    public sealed class CirnoRCharacterFairyTeaPartySeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.IsStackable = true;
            config.HasLevel = false;
            // config.DurationDecreaseTiming = DurationDecreaseTiming.NormalTurnStart;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFairyTeaPartySeDef))]
    public sealed class CirnoRCharacterFairyTeaPartySe : StatusEffect
    {
        public int Value1
        {
            get
            {
                int result = 1;
                return result;
            }
        }

        public int limitValue
        {
            get
            {
                int result = 9;
                return result;
            }
        }

        public int leftValue { get; set; } = 9;

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<CardUsingEventArgs>(
                base.Battle.CardUsing,
                new EventSequencedReactor<CardUsingEventArgs>(this.CardUsing)
            );
            base.ReactOwnerEvent<UnitEventArgs>(
                base.Owner.TurnEnded,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnEnded)
            );
        }

        private IEnumerable<BattleAction> CardUsing(CardUsingEventArgs args)
        {
            if (leftValue <= 0)
            {
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
                yield break;
            }
            bool blueFlag = Enumerable.Contains<ManaColor>(args.Card.Config.Colors, ManaColor.Blue);
            if (blueFlag)
            {
                base.NotifyActivating();
                yield return new DrawManyCardAction(Value1);
                leftValue -= 1;
                if (leftValue <= 0)
                {
                    yield return new RemoveStatusEffectAction(this, true, 0.1f);
                    yield break;
                }
            }
            bool greenFlag = Enumerable.Contains<ManaColor>(
                args.Card.Config.Colors,
                ManaColor.Green
            );
            if (greenFlag)
            {
                base.NotifyActivating();
                yield return new GainManaAction(new ManaGroup() { Philosophy = 1 });
                leftValue -= 1;
                if (leftValue <= 0)
                {
                    yield return new RemoveStatusEffectAction(this, true, 0.1f);
                    yield break;
                }
            }
            yield break;
        }

        private IEnumerable<BattleAction> OnTurnEnded(UnitEventArgs args)
        {
            yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }
    }
}
