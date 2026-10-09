using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterIcicleFallDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(951);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 1, Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1, Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.RandomEnemy;

            config.Damage = 9;
            config.UpgradedDamage = 9;

            config.Value1 = 2;
            config.UpgradedValue1 = 3;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Illustrator = "菜包return";
            // Even though Debut doesn't appear at the end of the card, Keywords have to be used instead of RelativeKeyword. If RelativeKeyword is used, the Debut effect will not work.
            config.Keywords = Keyword.Debut;
            config.UpgradedKeywords = Keyword.Debut;

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterIcicleFallDef))]
    public sealed class CirnoRCharacterIcicleFall : CirnoRCharacterCard
    {
        // Debut is once per combat.
        // If the card was already played this combat, change the description of the card to grey out the Debut part.
        protected override string GetBaseDescription()
        {
            if (!base.DebutActive)
            {
                return base.GetExtraDescription1;
            }
            return base.GetBaseDescription();
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // Attack the enemy Value1 times. The attack animation is only played once.
            // To play the animation as many time as Value1, set the last parameter of Guns() from false to true.
            base.CardGuns = new Guns(base.GunName, base.Value1, true);
            foreach (GunPair gunPair in base.CardGuns.GunPairs)
            {
                yield return base.AttackAction(selector, gunPair);
            }
            // TriggeredAnyhow: The Debut hasn't been activated this combat yet.
            if (base.TriggeredAnyhow)
            {
                yield return new DrawManyCardAction(base.Value2);
            }
            yield break;
        }
    }
}
