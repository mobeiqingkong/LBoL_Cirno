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
    public sealed class CirnoRCharacterElfFlowerFieldDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 2 };
            config.UpgradedCost = new ManaGroup() { Green = 1 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Value2 = 7;
            config.UpgradedValue2 = 7;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 1 };

            config.Illustrator = "";

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterElfFlowerFieldSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterElfFlowerFieldSe),
            };

            config.RelativeCards = new List<string>() { nameof(CirnoRCharacterFairyFlower) };
            config.UpgradedRelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterFairyFlower),
            };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterElfFlowerFieldDef))]
    public sealed class CirnoRCharacterElfFlowerField : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return BuffAction<CirnoRCharacterElfFlowerFieldSe>(Value1, Value1);
        }
    }
}
