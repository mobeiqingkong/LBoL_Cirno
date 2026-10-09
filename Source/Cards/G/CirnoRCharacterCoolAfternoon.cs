using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using JetBrains.Annotations;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Intentions;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterCoolAfternoonDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 1 };
            config.UpgradedCost = new ManaGroup() { Green = 1 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Shield = 8;
            config.UpgradedShield = 12;

            config.Value1 = 8;
            config.UpgradedValue1 = 12;

            config.Value2 = 8;
            config.UpgradedValue2 = 12;

            config.Keywords = Keyword.Shield | Keyword.Exile;
            config.UpgradedKeywords = Keyword.Shield | Keyword.Exile;

            config.Mana = new ManaGroup() { Green = 1, Blue = 1 };
            config.UpgradedMana = new ManaGroup() { Philosophy = 3 };

            // config.RelativeEffects = new List<string>() { nameof(FrostArmor) };
            // config.UpgradedRelativeEffects = new List<string>() { nameof(FrostArmor) };
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterCoolAfternoonDef))]
    public sealed class CirnoRCharacterCoolAfternoon : CirnoRCharacterCard
    {
        public override Interaction Precondition()
        {
            List<CirnoRCharacterCoolAfternoon> list = Library
                .CreateCards<CirnoRCharacterCoolAfternoon>(2, this.IsUpgraded)
                .ToList<CirnoRCharacterCoolAfternoon>();
            CirnoRCharacterCoolAfternoon cirnoRCharacterCoolAfternoon = list[0];
            CirnoRCharacterCoolAfternoon cirnoRCharacterCoolAfternoon2 = list[1];
            cirnoRCharacterCoolAfternoon.ChoiceCardIndicator = 1;
            cirnoRCharacterCoolAfternoon2.ChoiceCardIndicator = 2;
            cirnoRCharacterCoolAfternoon.SetBattle(base.Battle);
            cirnoRCharacterCoolAfternoon2.SetBattle(base.Battle);
            return new MiniSelectCardInteraction(list, false, false, false);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            MiniSelectCardInteraction miniSelectCardInteraction =
                (MiniSelectCardInteraction)precondition;
            Card card =
                (miniSelectCardInteraction != null) ? miniSelectCardInteraction.SelectedCard : null;
            if (card != null)
            {
                if (card.ChoiceCardIndicator == 1)
                {
                    yield return new GainManaAction(base.Mana);
                }
                else
                {
                    // yield return BuffAction<FrostArmor>(Value1, Value1);
                    yield return new CastBlockShieldAction(
                        Battle.Player,
                        Battle.Player,
                        Shield,
                        true
                    );
                }
            }
            yield break;
        }
    }
}
