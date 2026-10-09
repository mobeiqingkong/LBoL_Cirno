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
    public sealed class CirnoRCharacterFrozenHexagramSeDef : CirnoRCharacterStatusEffectTemplate
    {
        // // Keywords don't have sprites.
        // public override Sprite LoadSprite() => null;

        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(TempFirepower) };
            // config.DurationDecreaseTiming = DurationDecreaseTiming.NormalTurnStart;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrozenHexagramSeDef))]
    public sealed class CirnoRCharacterFrozenHexagramSe : StatusEffect
    {
        public int TempFirepowerValue
        {
            get
            {
                int result = 2;
                if (base.Owner != null && base.Level != 0)
                {
                    result = result * base.Level;
                }
                return result;
            }
        }

        public int ShieldValue
        {
            get
            {
                int result = 2;
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
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnEnding, this.TurnEnding);
        }

        private IEnumerable<BattleAction> OnCardUsing(CardUsingEventArgs args)
        {
            bool flag = Enumerable.Contains<ManaColor>(args.Card.Config.Colors, ManaColor.Blue);
            if (flag)
            {
                base.NotifyActivating();
                yield return new ApplyStatusEffectAction<TempFirepower>(
                    base.Battle.Player,
                    this.TempFirepowerValue,
                    new int?(0),
                    new int?(0),
                    new int?(0),
                    0.2f,
                    true
                );
                yield return new CastBlockShieldAction(
                    base.Battle.Player,
                    base.Battle.Player,
                    0,
                    this.ShieldValue,
                    BlockShieldType.Direct,
                    true
                );
            }
            yield break;
        }

        private IEnumerable<BattleAction> TurnEnding(UnitEventArgs args)
        {
            if (base.Level > 0)
                base.Level--;
            if (base.Level == 0)
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }
    }
}
