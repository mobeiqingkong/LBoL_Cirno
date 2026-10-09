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
    public sealed class CirnoRCharacterFrostDomainSeDef : CirnoRCharacterStatusEffectTemplate
    {
        // // Keywords don't have sprites.
        // public override Sprite LoadSprite() => null;

        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            config.IsStackable = true;
            config.HasLevel = false;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrostDomainSeDef))]
    public sealed class CirnoRCharacterFrostDomainSe : StatusEffect
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

        protected override void OnAdded(Unit unit) { }

        public override bool Stack(StatusEffect other)
        {
            bool flag = base.Stack(other);
            // if (flag)
            // {
            //     this.SetManaAndExile(base.Battle.EnumerateAllCards());
            // }
            return flag;
        }
    }
}
