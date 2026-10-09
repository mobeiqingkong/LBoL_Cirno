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
    public sealed class CirnoRCharacterNaturalGrowthSeDef : CirnoRCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            // config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFairyFlower) };
            config.HasCount = true;
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterNaturalGrowthSeDef))]
    public sealed class CirnoRCharacterNaturalGrowthSe : StatusEffect
    {
        public int Value1
        {
            get
            {
                int Value1 = 7;
                return Value1;
            }
        }

        public ManaGroup Mana
        {
            get
            {
                ManaGroup mana = new ManaGroup() { Green = 1 };
                return mana;
            }
        }

        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<ManaEventArgs>(base.Battle.ManaConsuming, this.TurnEnding);
        }

        public int SynergyAmount(ManaGroup cost, ManaColor synergyColor, int synergyColorAmount = 1)
        {
            return synergyColor switch
                {
                    ManaColor.Philosophy => cost.Philosophy,
                    ManaColor.Any => cost.Amount,
                    _ => cost.Philosophy + cost.GetValue(synergyColor),
                } / synergyColorAmount
                + base.GameRun.SynergyAdditionalCount;
        }

        private IEnumerable<BattleAction> TurnEnding(ManaEventArgs args)
        {
            int greenMana = SynergyAmount(args.Value, ManaColor.Green, 1);
            this.Count += greenMana;
            if (this.Count >= Value1)
            {
                int drNums = this.Count / Value1;
                yield return new AddCardsToHandAction(
                    Library.CreateCards<CirnoRCharacterFairyFlower>(drNums * this.Level, false)
                );
                this.Count -= Value1 * drNums;
            }
            yield break;
        }
    }
}
