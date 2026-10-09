using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Presentation.UI.ExtraWidgets;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFriendDrawDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Colorless };
            config.Cost = new ManaGroup() { Colorless = 1 };
            config.UpgradedCost = new ManaGroup() { Colorless = 1 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 8;
            config.UpgradedValue1 = 8;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Mana = new ManaGroup() { Philosophy = 2 };
            config.UpgradedMana = new ManaGroup() { Philosophy = 3 };

            config.Illustrator = "oO大黄Oo";
            config.Keywords = Keyword.Exile | Keyword.Overdraft;
            config.UpgradedKeywords = Keyword.Retain | Keyword.Exile | Keyword.Overdraft;

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFriendDrawDef))]
    public sealed class CirnoRCharacterFriendDraw : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            int count = 0;
            while (base.Battle.HandZone.Count < Value1 && count <= 8)
            {
                yield return new DrawManyCardAction(1);
                count++;
            }
            yield return new LockRandomTurnManaAction(Value2);
            yield break;
        }
    }
}
