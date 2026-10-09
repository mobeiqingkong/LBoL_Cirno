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
    public sealed class CirnoRCharacterColdDivinityDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(12211);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            //Mana to consume aside from X
            config.Cost = new ManaGroup() { Blue = 1, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Blue = 1, Any = 1 };
            config.Rarity = Rarity.Uncommon;

            //The XCost has to be set.
            config.IsXCost = false;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 14;
            config.UpgradedDamage = 16;

            config.Value1 = 1;
            config.UpgradedValue1 = 3;

            config.Value2 = 2;
            config.UpgradedValue2 = 2;

            // config.Keywords = Keyword.Exile;
            // config.UpgradedKeywords = Keyword.Exile;

            config.Illustrator = "";
            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterColdDivinityDef))]
    public sealed class CirnoRCharacterColdDivinity : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return AttackAllAliveEnemyAction();
            for (int i = 1; i <= Value1; i++)
            {
                foreach (Unit enemy in base.Battle.AllAliveEnemies)
                {
                    yield return new ApplyStatusEffectAction<CirnoRCharacterFrostSe>(enemy, 1, 1);
                }
            }
        }
    }
}
