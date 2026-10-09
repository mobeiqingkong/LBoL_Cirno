using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using DG.Tweening;
using HarmonyLib;
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
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    // Empty status effect that is purely used to define a new pseudo-keyword.
    // See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class CirnoRCharacterElfFlowerFieldSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            // config.HasCount = true;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterElfFlowerFieldSeDef))]
    public sealed class CirnoRCharacterElfFlowerFieldSe : StatusEffect
    {
        public int TempCirnoRCharacterFairyFlowerValue
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

        public ManaGroup Mana
        {
            get
            {
                int result = 1;
                if (base.Owner != null && base.Level != 0)
                {
                    result = result * base.Level;
                }
                return new ManaGroup() { Green = result };
            }
        }

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnStarted, this.OnOwnerTurnStarted);
        }

        private IEnumerable<BattleAction> OnOwnerTurnStarted(UnitEventArgs args)
        {
            yield return new GainManaAction(Mana);
            yield return new AddCardsToHandAction(
                Library.CreateCards<CirnoRCharacterFairyFlower>(
                    TempCirnoRCharacterFairyFlowerValue,
                    false
                )
            );
            yield break;
        }
    }
}
