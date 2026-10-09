using System;
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
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.Presentation.Units;
using LBoLEntitySideloader.Attributes;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterIceBeadDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(14020);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Blue = 1 };
            config.Rarity = Rarity.Common;

            config.IsPooled = false;
            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 6;
            config.UpgradedDamage = 6;

            // config.Keywords = Keyword.Accuracy;
            // config.UpgradedKeywords = Keyword.Accuracy;
            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostSe),
                nameof(CirnoRCharacterDeathOfNatureSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostSe),
                nameof(CirnoRCharacterDeathOfNatureSe),
            };

            config.Value1 = 6;
            config.UpgradedValue1 = 6;

            config.Value2 = 3;
            config.UpgradedValue2 = 4;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterIceBeadDef))]
    public sealed class CirnoRCharacterIceBead : CirnoRCharacterCard
    {
        protected override int BaseValue3 { get; set; } = 6;
        protected override int BaseUpgradedValue3 { get; set; } = 6;

        protected override int AdditionalDamage
        {
            get
            {
                if (base.Battle == null)
                {
                    return 0;
                }

                int blueCardUsed = base
                    .Battle.TurnCardUsageHistory.Where(
                        (Card card) => card.Config.Colors.Contains(ManaColor.Blue)
                    )
                    .Count<Card>();

                return Value2 * blueCardUsed;
            }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return base.AttackAction(selector);
            yield return DebuffAction<CirnoRCharacterFrostSe>(selector.SelectedEnemy, 1, 1);
            // yield return BuffAction<Firepower>(1, 1);
            yield break;
        }
    }
}
