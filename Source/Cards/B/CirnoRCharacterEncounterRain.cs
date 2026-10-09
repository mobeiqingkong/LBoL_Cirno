using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.Patches;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterEncounterRainDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 2, Any = 2 };
            config.UpgradedCost = new ManaGroup() { Blue = 1, Any = 2 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.AllEnemies;

            config.Value1 = 3;
            config.UpgradedValue1 = 3;

            config.Value2 = 3;
            config.UpgradedValue2 = 3;

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.None;

            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterMoistSe),
                nameof(CirnoRCharacterFrostflowSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterMoistSe),
                nameof(CirnoRCharacterFrostflowSe),
            };

            config.Mana = new ManaGroup() { Any = 3 };
            config.UpgradedMana = new ManaGroup() { Any = 3 };

            config.RelativeCards = new List<string>() { };
            config.UpgradedRelativeCards = new List<string>() { };
            config.Illustrator = "POKIO";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterEncounterRainDef))]
    public sealed class CirnoRCharacterEncounterRain : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return BuffAction<CirnoRCharacterFrostflowSe>(Value1, Value1);
            yield return BuffAction<CirnoRCharacterMoistSe>(Value2, Value2);
            yield break;
        }
    }
}
