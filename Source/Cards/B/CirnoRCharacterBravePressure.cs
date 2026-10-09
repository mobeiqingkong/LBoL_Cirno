using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.Patches;
using CirnoRCharacterMod.StatusEffects;
using DG.Tweening;
using JetBrains.Annotations;
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
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Cards.Neutral.TwoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.BattleModifiers.Actions;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterBravePressureDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 2, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.AllEnemies;

            config.Value1 = 3;
            config.UpgradedValue1 = 3;

            config.Value2 = 3;
            config.UpgradedValue2 = 3;

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            //Add Weak, Vulnerable, Firepower Down descrption when hovering over the card.
            config.RelativeEffects = new List<string>()
            {
                nameof(Weak),
                nameof(Vulnerable),
                nameof(TempFirepowerNegative),
                nameof(LockedOn),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(Weak),
                nameof(Vulnerable),
                nameof(TempFirepowerNegative),
                nameof(LockedOn),
            };

            config.Illustrator = "曦锦";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterBravePressureDef))]
    public sealed class CirnoRCharacterBravePressure : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            //Buff and debuff have either a level or a duration.
            //Duration: Effects that last a certain amount of turns then disappear.
            //Level: Effects that have a fixed duration but that can vary in intensity.
            //Weak and Vulnerable have a duration, FirepowerNegative has a level.
            //DebuffAction's 2nd field is the level, the 3rd one is the duration.
            foreach (Unit enemy in base.Battle.AllAliveEnemies)
            {
                yield return DebuffAction<Weak>(enemy, 0, base.Value1, 0, 0, true, 0.2f);
                yield return DebuffAction<Vulnerable>(enemy, 0, base.Value1, 0, 0, true, 0.2f);
                yield return DebuffAction<TempFirepowerNegative>(
                    enemy,
                    base.Value2,
                    0,
                    0,
                    0,
                    true,
                    0.2f
                );
                yield return DebuffAction<LockedOn>(enemy, base.Value2, 0, 0, 0, true, 0.2f);
            }
            yield break;
        }
    }
}
