using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterNaturalFieldDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1, Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 4;
            config.UpgradedValue1 = 4;

            config.Value2 = 2;
            config.UpgradedValue2 = 3;

            config.Mana = new ManaGroup() { Green = 1 };
            config.UpgradedMana = new ManaGroup() { Green = 1 };

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.None;

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterNaturalFieldSe) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterNaturalFieldSe),
            };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterNaturalFieldDef))]
    public sealed class CirnoRCharacterNaturalField : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // yield return new CastBlockShieldAction(Battle.Player, Battle.Player, base.Value1, 0, BlockShieldType.Direct, true);
            yield return BuffAction<CirnoRCharacterNaturalFieldSe>(Value2, Value2);
            yield break;
        }
    }
}
