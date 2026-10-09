using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterHailstormDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(960);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 1, Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1, Blue = 1 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 7;
            config.UpgradedDamage = 7;

            config.Value1 = 2;
            config.UpgradedValue1 = 3;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterHailstormDef))]
    public sealed class CirnoRCharacterHailstorm : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // Attack the enemy Value1 times. The attack animation is only played once.
            // To play the animation as many time as Value1, set the last parameter of Guns() from false to true.
            base.CardGuns = new Guns(base.GunName, base.Value1, false);
            foreach (GunPair gunPair in base.CardGuns.GunPairs)
            {
                yield return base.AttackAction(selector, gunPair);
            }
        }
    }
}
