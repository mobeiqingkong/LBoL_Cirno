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
    public sealed class CirnoRCharacterFrozenHexagramDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Damage = 3;
            config.UpgradedDamage = 3;

            config.Block = 2;
            config.Shield = 2;

            config.Keywords = Keyword.Shield;
            config.UpgradedKeywords = Keyword.Shield;
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrozenHexagramSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrozenHexagramSe),
            };

            config.Value1 = 2;
            config.UpgradedValue1 = 2;
            config.Value2 = 2;
            config.UpgradedValue2 = 2;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrozenHexagramDef))]
    public sealed class CirnoRCharacterFrozenHexagram : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            if (IsUpgraded)
            {
                yield return BuffAction<TempFirepower>(this.Value1, this.Value1);
                yield return new CastBlockShieldAction(base.Battle.Player, this.Shield, true);
            }
            yield return BuffAction<CirnoRCharacterFrozenHexagramSe>(1, 1);
            yield break;
        }
    }
}
