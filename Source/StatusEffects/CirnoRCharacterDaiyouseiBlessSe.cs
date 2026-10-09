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
    public sealed class CirnoRCharacterDaiyouseiBlessSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(TempFirepower) };
            // config.DurationDecreaseTiming = DurationDecreaseTiming.NormalTurnStart;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterDaiyouseiBlessSeDef))]
    public sealed class CirnoRCharacterDaiyouseiBlessSe : StatusEffect
    {
        public int TempFirepowerValue
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

        public int TempCirnoRCharacterNaturalGrowthSeValue
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
            // base.ReactOwnerEvent<UnitEventArgs>(base.Battle.Player.TurnStarting, new EventSequencedReactor<UnitEventArgs>(this.OnPlayerTurnStarted));
            // base.ReactOwnerEvent<DamageEventArgs>(base.Owner.DamageReceived, new EventSequencedReactor<DamageEventArgs>(this.afterAtack));
        }

        private IEnumerable<BattleAction> OnCardUsing(CardUsingEventArgs args)
        {
            bool flag = Enumerable.Contains<ManaColor>(args.Card.Config.Colors, ManaColor.Green);
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
            }
            yield break;
        }

        // private IEnumerable<BattleAction> OnPlayerTurnStarted(UnitEventArgs args)
        // {
        //     base.NotifyActivating();
        //     yield return new ApplyStatusEffectAction<CirnoRCharacterNaturalGrowthSe>(base.Owner, TempCirnoRCharacterNaturalGrowthSeValue, TempCirnoRCharacterNaturalGrowthSeValue);
        //     yield break;
        // }

        // private IEnumerable<BattleAction> TurnEnding(UnitEventArgs args)
        // {

        //     yield return new HealAction(base.Battle.Player, base.Battle.Player, this.HealValue);
        //     // yield return new CastBlockShieldAction(base.Battle.Player, base.Battle.Player, 0, this.ShieldValue, BlockShieldType.Direct, true);
        //     // yield return new ApplyStatusEffectAction<LockedOn>(base.Battle.Player, this.LockedOnValue, new int?(0), new int?(0), new int?(0), 0.2f, true);
        //     yield break;
        // }

        // private IEnumerable<BattleAction> afterAtack(DamageEventArgs args)
        // {
        //     //At the start of the Player's turn, gain Spirit.
        //     // yield return BuffAction<Spirit>(base.Level, 0, 0, 0, 0.2f);
        //     if (base.Level > 0 && args.Source != null && args.Source != this.Owner && args.Source.IsAlive)
        //     {
        //         base.Level--;
        //         yield return new ApplyStatusEffectAction<CirnoRCharacterDeathOfNatureSe>(args.Source, 1, 0, 0, 0, 0.2f);
        //     }
        //     if (base.Level == 0) yield return new RemoveStatusEffectAction(this, true, 0.1f);
        //     yield break;
        // }
    }
}
