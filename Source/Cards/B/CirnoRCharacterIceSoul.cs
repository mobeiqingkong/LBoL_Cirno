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
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterIceSoulDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 2, Any = 3 };
            config.UpgradedCost = new ManaGroup() { Blue = 1, Any = 2 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Self;

            config.Damage = 1;
            config.UpgradedDamage = 1;

            config.Value1 = 3;
            config.UpgradedValue1 = 3;
            config.Value2 = 3;
            config.UpgradedValue2 = 3;

            config.Keywords = Keyword.Shield | Keyword.Block;
            config.UpgradedKeywords = Keyword.Shield | Keyword.Block;
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterIceSoulSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterIceSoulSe),
            };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterIceSoulDef))]
    public sealed class CirnoRCharacterIceSoul : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return BuffAction<CirnoRCharacterIceSoulSe>(1, 1);
            yield break;
        }
    }
}
