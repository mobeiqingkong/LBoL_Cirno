using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
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
    public sealed class CirnoRCharacterGuardianOfDaiyouseiSeDef
        : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            // config.HasCount = true;
            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterGuardianOfNatureSe),
            };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterGuardianOfDaiyouseiSeDef))]
    public sealed class CirnoRCharacterGuardianOfDaiyouseiSe : StatusEffect
    {
        public int TempCirnoRCharacterGuardianOfNatureSeValue
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
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnEnded, this.OnOwnerTurnEnded);
        }

        private IEnumerable<BattleAction> OnOwnerTurnEnded(UnitEventArgs args)
        {
            // yield return new AddCardsToHandAction(Library.CreateCards<SummerFlower>(TempCirnoRCharacterFairyFlowerValue, false));

            // yield return new BuffA(Library.CreateCards<SummerFlower>(TempCirnoRCharacterFairyFlowerValue, false));
            yield return new ApplyStatusEffectAction<CirnoRCharacterGuardianOfNatureSe>(
                base.Battle.Player,
                this.TempCirnoRCharacterGuardianOfNatureSeValue,
                new int?(0),
                new int?(0),
                new int?(0),
                0.2f,
                true
            );
            yield break;
        }
    }
}
