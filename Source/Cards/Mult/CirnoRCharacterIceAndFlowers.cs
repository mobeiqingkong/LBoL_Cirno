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
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterIceAndFlowersDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(14050);
            config.Colors = new List<ManaColor>()
            {
                ManaColor.Green,
                ManaColor.Blue,
                ManaColor.Any,
            };
            config.Cost = new ManaGroup()
            {
                Green = 1,
                Blue = 1,
                White = 1,
            };
            config.UpgradedCost = new ManaGroup()
            {
                Hybrid = 1,
                HybridColor = 6,
                Any = 1,
            };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 12;
            config.UpgradedDamage = 18;

            config.Value1 = 2;
            config.UpgradedValue1 = 2;

            config.Value2 = 2;
            config.UpgradedValue2 = 3;

            config.Mana = new ManaGroup() { Any = 0 };
            config.UpgradedMana = new ManaGroup() { Any = 0 };

            config.Keywords = Keyword.Exile | Keyword.TempMorph;
            config.UpgradedKeywords = Keyword.Exile | Keyword.TempMorph;

            config.RelativeCards = new List<string>() { nameof(CirnoRCharacterFairyFlower) };
            config.UpgradedRelativeCards = new List<string>()
            {
                nameof(CirnoRCharacterFairyFlower),
            };

            config.Illustrator = "やませちか";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterIceAndFlowersDef))]
    public sealed class CirnoRCharacterIceAndFlowers : CirnoRCharacterCard
    {
        protected override int BaseValue3 { get; set; } = 2;
        protected override int BaseUpgradedValue3 { get; set; } = 2;

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
            // for (int i = 1; i < BaseValue3; i++)
            // {
            //     yield return DebuffAction<CirnoRCharacterFrostSe>(selector.SelectedEnemy, 1, 1);
            // }
            // CirnoRCharacterDaiyouseiBlessSe cirnoRCharacterDaiyouseiBlessSe =
            //     base.Battle.Player.GetStatusEffect<CirnoRCharacterDaiyouseiBlessSe>();
            // if (
            //     cirnoRCharacterDaiyouseiBlessSe != null
            //     && cirnoRCharacterDaiyouseiBlessSe.Level > 0
            // )
            // {
            List<CirnoRCharacterFairyFlower> list = Library
                .CreateCards<CirnoRCharacterFairyFlower>(Value2, false)
                .ToList();
            foreach (CirnoRCharacterFairyFlower cirnoRCharacterFairyFlower in list)
            {
                cirnoRCharacterFairyFlower.SetTurnCost(base.Mana);
                cirnoRCharacterFairyFlower.IsEthereal = true;
                cirnoRCharacterFairyFlower.IsExile = true;
                cirnoRCharacterFairyFlower.IsAccuracy = true;
            }
            yield return new AddCardsToHandAction(list, AddCardsType.Normal, false);
            // }
            yield break;
        }
    }
}
