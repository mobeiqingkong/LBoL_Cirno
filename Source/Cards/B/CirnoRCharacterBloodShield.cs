using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.Patches;
using CirnoRCharacterMod.StatusEffects;
using DG.Tweening;
using HarmonyLib;
using JetBrains.Annotations;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.Randoms;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.BattleModifiers.Actions;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterBloodShieldDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 3;
            config.UpgradedValue1 = 3;

            config.Value2 = 12;
            config.UpgradedValue2 = 15;

            config.Keywords = Keyword.Shield;
            config.UpgradedKeywords = Keyword.Shield;

            config.Mana = new ManaGroup() { Green = 1, Blue = 1 };
            config.UpgradedMana = new ManaGroup() { Philosophy = 3 };

            // config.RelativeEffects = new List<string>() { nameof(FrostArmor) };
            // config.UpgradedRelativeEffects = new List<string>() { nameof(FrostArmor) };
            config.Illustrator = "小柚こゆき❄";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterBloodShieldDef))]
    public sealed class CirnoRCharacterBloodShield : CirnoRCharacterCard
    {
        protected override int BaseValue3 { get; set; } = 5;
        protected override int BaseUpgradedValue3 { get; set; } = 5;

        protected override int BaseValue4 { get; set; } = 20;
        protected override int BaseUpgradedValue4 { get; set; } = 25;

        public override Interaction Precondition()
        {
            List<CirnoRCharacterBloodShield> list = Library
                .CreateCards<CirnoRCharacterBloodShield>(2, this.IsUpgraded)
                .ToList<CirnoRCharacterBloodShield>();
            CirnoRCharacterBloodShield cirnoRCharacterCoolAfternoon = list[0];
            CirnoRCharacterBloodShield cirnoRCharacterCoolAfternoon2 = list[1];
            cirnoRCharacterCoolAfternoon.ChoiceCardIndicator = 1;
            cirnoRCharacterCoolAfternoon2.ChoiceCardIndicator = 2;
            cirnoRCharacterCoolAfternoon.SetBattle(base.Battle);
            cirnoRCharacterCoolAfternoon2.SetBattle(base.Battle);
            return new MiniSelectCardInteraction(list, false, false, false);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            MiniSelectCardInteraction miniSelectCardInteraction =
                (MiniSelectCardInteraction)precondition;
            Card card =
                (miniSelectCardInteraction != null) ? miniSelectCardInteraction.SelectedCard : null;
            if (card != null)
            {
                if (card.ChoiceCardIndicator == 1)
                {
                    yield return LoseLifeAction(base.Value1);
                    yield return new CastBlockShieldAction(
                        base.Battle.Player,
                        base.Battle.Player,
                        0,
                        base.Value2,
                        BlockShieldType.Direct,
                        true
                    );
                }
                else
                {
                    yield return LoseLifeAction(base.Value3);
                    yield return new CastBlockShieldAction(
                        base.Battle.Player,
                        base.Battle.Player,
                        0,
                        base.Value4,
                        BlockShieldType.Direct,
                        true
                    );
                }
            }
            yield break;
        }
    }
}
