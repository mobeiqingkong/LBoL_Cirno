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
    public sealed class CirnoRCharacterPerfectGlacialistDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(400);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 2 };
            config.UpgradedCost = new ManaGroup() { Blue = 2 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Self;

            config.Damage = 3;
            config.UpgradedDamage = 3;

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.Initial;
            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostUpSe),
                nameof(CirnoRCharacterFrostSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostUpSe),
                nameof(CirnoRCharacterFrostSe),
            };

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Illustrator = "cuteg";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterPerfectGlacialistDef))]
    public sealed class CirnoRCharacterPerfectGlacialist : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return BuffAction<CirnoRCharacterFrostUpSe>(Value1, Value1);
            yield break;
        }
    }
}
