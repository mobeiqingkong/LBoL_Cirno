using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterTheGiftOfFogLakeDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(950);

            config.Colors = new List<ManaColor>() { ManaColor.Blue, ManaColor.Green };
            config.Cost = new ManaGroup() { Hybrid = 1, HybridColor = 6 };
            config.UpgradedCost = new ManaGroup() { Hybrid = 1, HybridColor = 6 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;
            config.Mana = new ManaGroup() { Blue = 1, Green = 1 };
            config.UpgradedMana = new ManaGroup()
            {
                Blue = 1,
                Green = 1,
                Philosophy = 1,
            };

            config.Damage = 4;
            config.UpgradedDamage = 4;

            config.Value1 = 2;
            config.UpgradedValue1 = 2;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Illustrator = "";
            //The Accuracy keyword is enough to make an attack accurate.
            config.Keywords = Keyword.Exile | Keyword.Overdraft;
            config.UpgradedKeywords = Keyword.Exile;

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterTheGiftOfFogLakeDef))]
    public sealed class CirnoRCharacterTheGiftOfFogLake : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return new DrawManyCardAction(Value1);
            yield return new GainManaAction(base.Mana); // 获得法力
            if (!IsUpgraded)
            {
                yield return new LockRandomTurnManaAction(Value2);
            }
            yield break;
        }
    }
}
