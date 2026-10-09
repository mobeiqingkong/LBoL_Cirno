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
    public sealed class CirnoRCharacterDiamondBlizzardDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(950);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 1, Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1, Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 4;
            config.UpgradedDamage = 4;

            config.Value1 = 3;
            config.UpgradedValue1 = 4;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Illustrator = "笙舟";
            //The Accuracy keyword is enough to make an attack accurate.
            config.Keywords = Keyword.Accuracy;
            config.UpgradedKeywords = Keyword.Accuracy;

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterDiamondBlizzardDef))]
    public sealed class CirnoRCharacterDiamondBlizzard : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // Attack the enemy Value1 times. The attack animation is only played once.
            // To play the animation as many time as Value1, set the last parameter of Guns() from false to true.
            base.CardGuns = new Guns(base.GunName, base.Value1, false);
            foreach (GunPair gunPair in base.CardGuns.GunPairs)
            {
                yield return base.AttackAction(selector, gunPair);
            }
            if (!base.Battle.BattleShouldEnd && base.IsUpgraded)
            {
                yield return new DrawManyCardAction(base.Value2);
            }
            yield break;
        }
    }
}
