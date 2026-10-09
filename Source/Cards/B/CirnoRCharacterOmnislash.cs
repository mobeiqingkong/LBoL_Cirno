using System;
using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using DG.Tweening;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.Presentation.UI;
using LBoL.Presentation.Units;
using LBoLEntitySideloader.Attributes;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterOmnislashDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(513);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 9 };
            config.UpgradedCost = new ManaGroup() { Any = 7 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 1;
            config.UpgradedDamage = 1;

            config.Value1 = 2;
            config.UpgradedValue1 = 2;

            config.Value2 = 9;

            // config.Value3 = 1;
            // config.UpgradedValue3 = 2;
            //config.Mana is used here to set
            config.Mana = new ManaGroup { Any = 1 };

            // config.Keywords = Keyword.Exile;
            // config.UpgradedKeywords = Keyword.Exile;

            config.Illustrator = "";
            // nameof(Immune) 寒冰屏障
            config.RelativeEffects = new List<string>()
            {
                nameof(Graze),
                nameof(TempFirepower),
                nameof(Firepower),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(Graze),
                nameof(TempFirepower),
                nameof(Firepower),
            };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterOmnislashDef))]
    public sealed class CirnoRCharacterOmnislash : CirnoRCharacterCard
    {
        // Cost -1 less for each card in hand.
        protected override int BaseValue3 { get; set; } = 1;
        protected override int BaseUpgradedValue3 { get; set; } = 1;
        protected override int BaseValue4 { get; set; } = 3;
        protected override int BaseUpgradedValue4 { get; set; } = 3;
        protected override ManaGroup AdditionalCost
        {
            get
            {
                // Only change the cost while in Battle
                if (base.Battle != null)
                {
                    // return base.Mana * -base.Battle.TurnCardFollowAttackHistory.Count; // base.Battle.HandZone.Count;
                    // base.Battle.TurnCardUsageHistory.Where((Card card) => card.Config.Type == CardType.Attack).Count;
                    // List<Card> list =
                    return base.Mana
                        * -base
                            .Battle.TurnCardUsageHistory.Where(
                                (Card card) => card.Config.Type == CardType.Attack
                            )
                            .Count<Card>(); // base.Battle.HandZone.Count;
                }
                // Otherwise, do not change the ccost (0 additional cost)
                return base.Mana * -0;
            }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return base.BuffAction<Graze>(base.Value1, base.Value1);
            // yield return new ApplyStatusEffectAction<Invincible>(base.Battle.Player, base.Value1, base.Value1, base.Value1, base.Value1, 0.2f);
            for (int i = 1; i <= base.Value2; i++)
            {
                yield return base.AttackAction(selector, base.GunName);
                GameDirector.Shake(i);
                yield return base.BuffAction<TempFirepower>(1, 1);
                if (i % base.Value4 == 0)
                {
                    yield return base.BuffAction<Firepower>(1, 1);
                }
            }
            yield break;
        }
    }
}
