using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterGrazeDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 0 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;

            config.Value2 = 1;

            config.Illustrator = "";

            config.Keywords = Keyword.Retain;
            config.UpgradedKeywords = Keyword.Retain;

            config.RelativeEffects = new List<string>() { nameof(Graze) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(Graze) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterGrazeDef))]
    public sealed class CirnoRCharacterGraze : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return LoseLifeAction(base.Value1);
            yield return BuffAction<Graze>(base.Value2, 0, 0, 0, 0.2f);
            yield break;
        }
    }
}
