using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFrozenHexagramShieldDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 3 };
            config.UpgradedCost = new ManaGroup() { Blue = 2 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Self;

            config.Damage = 3;
            config.UpgradedDamage = 3;

            config.Keywords = Keyword.Shield;
            config.UpgradedKeywords = Keyword.Shield | Keyword.Retain;
            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrozenHexagramShieldSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrozenHexagramShieldSe),
            };

            config.Value1 = 6;
            config.UpgradedValue1 = 6;
            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrozenHexagramShieldDef))]
    public sealed class CirnoRCharacterFrozenHexagramShield : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return BuffAction<CirnoRCharacterFrozenHexagramShieldSe>(Value2, Value2);
            yield break;
        }
    }
}
