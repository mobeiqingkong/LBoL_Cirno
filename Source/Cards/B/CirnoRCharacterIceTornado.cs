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
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterIceTornadoDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(12211);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            //Mana to consume aside from X
            config.Cost = new ManaGroup() { Blue = 2 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Uncommon;

            //The XCost has to be set.
            config.IsXCost = false;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 12;
            config.UpgradedDamage = 16;

            config.Value1 = 12;
            config.UpgradedValue1 = 16;
            config.Value2 = 1;
            config.UpgradedValue2 = 2;

            // config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            // config.UpgradedRelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            config.RelativeCards = new List<string>()
            {
                // nameof(Blizzard),
                nameof(CirnoRCharacterIceBead),
            };
            config.UpgradedRelativeCards = new List<string>()
            {
                // nameof(Blizzard),
                nameof(CirnoRCharacterIceBead),
            };

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.None;
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterIceTornadoDef))]
    public sealed class CirnoRCharacterIceTornado : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // If a card was selected.
            yield return AttackAllAliveEnemyAction();

            List<CirnoRCharacterIceBead> list = Library
                .CreateCards<CirnoRCharacterIceBead>(base.Value2, false)
                .ToList<CirnoRCharacterIceBead>();
            foreach (CirnoRCharacterIceBead item in list)
            {
                item.IsEthereal = true;
                item.IsExile = true;
            }

            yield return new AddCardsToHandAction(list);
            yield break;
        }
    }
}
