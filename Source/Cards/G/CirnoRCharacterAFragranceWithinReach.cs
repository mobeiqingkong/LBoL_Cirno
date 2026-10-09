using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.GapOptions;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterAFragranceWithinReachDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 2, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Green = 2, Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Defense;
            config.TargetType = TargetType.Self;
            // config.IsPooled = false;

            config.Damage = 3;
            config.UpgradedDamage = 3;

            config.Block = 18;
            config.UpgradedBlock = 21;

            config.Value1 = 18;
            config.UpgradedValue1 = 21;

            config.Value2 = 2;
            config.UpgradedValue2 = 3;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 1 };

            config.Keywords = Keyword.Block;
            config.UpgradedKeywords = Keyword.Block;
            config.RelativeCards = new List<string>() { nameof(CirnoRCharacterFairyFlower) };
            config.UpgradedRelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterFairyFlower),
            };
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterAFragranceWithinReachDef))]
    public sealed class CirnoRCharacterAFragranceWithinReach : CirnoRCharacterCard
    {
        protected override int AdditionalBlock
        {
            get { return Value2 * this.GetSeLevel<CirnoRCharacterFairyFlowerSe>(); }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return new CastBlockShieldAction(
                base.Battle.Player,
                base.Battle.Player,
                base.Block,
                true
            );

            yield break;
        }
    }
}
