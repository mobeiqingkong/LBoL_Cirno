using System.Collections.Generic;
using System.Linq;
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
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.BattleModifiers.Actions;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterPerfectFreezeDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(14038);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 2, Blue = 2 };
            config.UpgradedCost = new ManaGroup() { Any = 1, Blue = 2 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 1;
            config.UpgradedDamage = 1;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Illustrator = "";
            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;
            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostSe),
                nameof(CirnoRCharacterFrostUpSe),
                nameof(CirnoRCharacterFrostDomainSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostSe),
                nameof(CirnoRCharacterFrostUpSe),
                nameof(CirnoRCharacterFrostDomainSe),
            };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterPerfectFreezeDef))]
    public sealed class CirnoRCharacterPerfectFreeze : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // foreach (Unit enemy in base.Battle.AllAliveEnemies)
            // {
            //     yield return new ApplyStatusEffectAction<CirnoRCharacterFrostSe>(enemy, Value1, Value1);
            // }
            //
            // foreach (GunPair gunPair in base.CardGuns.GunPairs)
            // {
            //     yield return base.AttackAction(selector, gunPair);
            // }
            base.CardGuns = new Guns(base.GunName, 1, true); //
            foreach (GunPair gunPair in base.CardGuns.GunPairs)
            {
                yield return AttackAllAliveEnemyAction(gunPair);
            }
            foreach (Unit enemy in base.Battle.AllAliveEnemies)
            {
                yield return new ApplyStatusEffectAction<CirnoRCharacterFrostSe>(enemy, 1, 1);
            }
            yield return base.BuffAction<CirnoRCharacterFrostUpSe>(Value1, Value1);
            yield return base.BuffAction<CirnoRCharacterFrostDomainSe>(Value1, Value1);

            // // Attack the enemy Value1 times. The attack animation is only played once.
            // // To play the animation as many time as Value1, set the last parameter of Guns() from false to true.
            // base.CardGuns = new Guns(base.GunName, base.Value1, true);
            // foreach (GunPair gunPair in base.CardGuns.GunPairs)
            // {
            //     yield return base.AttackAction(selector, gunPair);
            // }
        }
    }
}
