using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    public sealed class CirnoRCharacterIceSoulSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterIceSoulSeDef))]
    public sealed class CirnoRCharacterIceSoulSe : StatusEffect
    {
        public int blockValue
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

        public int shieldValue
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
                new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsing)
            );
        }

        private IEnumerable<BattleAction> OnCardUsing(CardUsingEventArgs args)
        {
            bool flag = Enumerable.Contains<ManaColor>(args.Card.Config.Colors, ManaColor.Blue);
            if (flag)
            {
                base.NotifyActivating();
                yield return new CastBlockShieldAction(
                    base.Battle.Player,
                    base.Battle.Player,
                    this.blockValue,
                    0,
                    BlockShieldType.Direct,
                    true
                );
            }
            yield break;
        }
    }
}
