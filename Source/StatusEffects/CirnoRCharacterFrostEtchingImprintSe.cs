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
    public sealed class CirnoRCharacterFrostEtchingImprintSeDef
        : CirnoRCharacterStatusEffectTemplate
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

    [EntityLogic(typeof(CirnoRCharacterFrostEtchingImprintSeDef))]
    public sealed class CirnoRCharacterFrostEtchingImprintSe : StatusEffect
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
        }

        private IEnumerable<BattleAction> OnCardUsing(CardUsingEventArgs args)
        {
            bool flag = Enumerable.Contains<ManaColor>(args.Card.Config.Colors, ManaColor.Blue);
            if (flag)
            {
                EnemyUnit randomAliveEnemy = base.Battle.RandomAliveEnemy;
                for (int i = 1; i <= this.Value1; i++)
                {
                    base.NotifyActivating();
                    yield return new ApplyStatusEffectAction<CirnoRCharacterFrostSe>(
                        randomAliveEnemy,
                        1,
                        new int?(0),
                        new int?(0),
                        new int?(0),
                        0.2f,
                        true
                    );
                }
            }
            yield break;
        }
    }
}
