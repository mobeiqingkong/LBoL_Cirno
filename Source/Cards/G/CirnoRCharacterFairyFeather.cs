using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.GapOptions;
using LBoL.Core.Intentions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFairyFeatherDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 2 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Defense;
            config.TargetType = TargetType.Self;
            // config.IsPooled = false;

            config.Damage = 3;
            config.UpgradedDamage = 3;

            config.Block = 10;
            config.UpgradedBlock = 12;

            config.Value1 = 10;
            config.UpgradedValue1 = 12;

            config.Value2 = 1;
            config.UpgradedValue2 = 2;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 1 };

            config.Keywords = Keyword.Block;
            config.UpgradedKeywords = Keyword.Block;
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterDeathOfNatureSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterDeathOfNatureSe),
            };
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFairyFeatherDef))]
    public sealed class CirnoRCharacterFairyFeather : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return new CastBlockShieldAction(
                base.Battle.Player,
                base.Battle.Player,
                base.Block,
                true
            );
            yield return base.BuffAction<CirnoRCharacterFairyFeatherSe>(Value2, 0, 0, 0, 0.2f);

            // yield return base.AttackAction(selector, GunNameID.GetGunFromId(6002));
            // if (base.Battle.BattleShouldEnd)
            // {
            //     yield break;
            // }
            // yield return base.BuffAction<CirnoRCharacterFairyFeatherSe>(base.Value1, 0, 0, 0, 0.2f);
            yield break;
        }
    }
}
