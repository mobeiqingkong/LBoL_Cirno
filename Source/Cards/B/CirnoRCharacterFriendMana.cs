using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;
using UnityEngine.InputSystem;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFriendManaDef : CirnoRCharacterCardTemplate
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

            config.Mana = new ManaGroup()
            {
                White = 1,
                Red = 1,
                Green = 2,
            };
            config.UpgradedMana = new ManaGroup()
            {
                White = 1,
                Red = 1,
                Green = 2,
                Philosophy = 1,
            };
            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Value2 = 2;
            config.UpgradedValue2 = 2;

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            config.Illustrator = "☢核燃黑猫☢";

            config.RelativeKeyword = Keyword.Overdraft;
            config.UpgradedRelativeKeyword = Keyword.Overdraft;
            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            // config.RelativeCards = new List<string>() { nameof(Zhukeling) };
            // config.UpgradedRelativeCards = new List<string>() { nameof(Zhukeling) };

            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFriendManaDef))]
    public sealed class CirnoRCharacterFriendMana : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return new GainManaAction(base.Mana);
            if (base.IsUpgraded)
                yield return new DrawManyCardAction(Value1);
            yield return new LockRandomTurnManaAction(Value2);
            yield break;
        }
    }
}
