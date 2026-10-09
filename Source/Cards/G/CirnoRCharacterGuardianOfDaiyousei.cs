using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterGuardianOfDaiyouseiDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1 };
            config.UpgradedCost = new ManaGroup() { Green = 0 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Value2 = 3;
            config.UpgradedValue2 = 3;

            config.Shield = 6;
            config.UpgradedShield = 6;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 1 };

            config.Illustrator = "てらちーとゆいな";

            config.Keywords = Keyword.Shield;
            config.UpgradedKeywords = Keyword.Shield | Keyword.Initial;

            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterGuardianOfDaiyouseiSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterGuardianOfDaiyouseiSe),
            };

            // config.RelativeCards = new List<string>() { nameof(SummerFlower) };
            // config.UpgradedRelativeCards = new List<string>() { nameof(SummerFlower) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterGuardianOfDaiyouseiDef))]
    public sealed class CirnoRCharacterGuardianOfDaiyousei : CirnoRCharacterCard
    {
        protected override int BaseValue3 { get; set; } = 6;
        protected override int BaseUpgradedValue3 { get; set; } = 6;

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return new CastBlockShieldAction(Battle.Player, Battle.Player, Shield, true);
            yield return BuffAction<CirnoRCharacterGuardianOfDaiyouseiSe>(Value1, Value1);
        }
    }
}
