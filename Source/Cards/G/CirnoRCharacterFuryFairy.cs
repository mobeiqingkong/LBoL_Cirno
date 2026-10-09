using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFuryFairyDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(14151);

            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1 };
            config.UpgradedCost = new ManaGroup() { Green = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 6;
            config.UpgradedDamage = 8;

            config.Value1 = 1;
            config.UpgradedValue1 = 2;

            config.Value2 = 1;
            config.UpgradedValue2 = 2;

            config.Keywords = Keyword.Accuracy;
            config.UpgradedKeywords = Keyword.Accuracy;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 1 };

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDeathOfNatureSe),
            };
            config.Illustrator = "かめぱすた";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFuryFairyDef))]
    public sealed class CirnoRCharacterFuryFairy : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            base.CardGuns = new Guns(base.GunName, 1, false);
            foreach (GunPair gunPair in base.CardGuns.GunPairs)
            {
                yield return base.AttackAllAliveEnemyAction(gunPair);
                foreach (Unit enemy in base.Battle.AllAliveEnemies)
                {
                    yield return base.DebuffAction<CirnoRCharacterDeathOfNatureSe>(enemy, 1, 1);
                }
            }
            //
            // yield return new DamageAction(base.Owner, selector, this.Damage, base.GunName, GunType.Single);
            // yield return new AddCardsToHandAction(Library.CreateCards<FairyWrath>(base.Value1, false));
            yield break;
        }
    }
}
