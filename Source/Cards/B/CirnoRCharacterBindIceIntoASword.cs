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
    public sealed class CirnoRCharacterBindIceIntoASwordDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(14020);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Uncommon;

            config.IsPooled = false;
            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 12;
            config.UpgradedDamage = 15;

            // config.Keywords = Keyword.Accuracy;
            // config.UpgradedKeywords = Keyword.Accuracy;
            config.RelativeEffects = new List<string>()
            {
                nameof(Firepower),
                nameof(TempFirepower),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(Firepower),
                nameof(TempFirepower),
            };

            config.Value1 = 3;
            config.UpgradedValue1 = 4;

            config.Value2 = 3;
            config.UpgradedValue2 = 4;

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.Accuracy;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterBindIceIntoASwordDef))]
    public sealed class CirnoRCharacterBindIceIntoASword : CirnoRCharacterCard
    {
        private int PlayerTotalFire
        {
            get
            {
                if (base.Battle == null)
                {
                    return 0;
                }
                return base.Battle.Player.TotalFirepower;
            }
        }

        protected override void OnEnterBattle(BattleController battle)
        {
            base.HandleBattleEvent<DamageDealingEventArgs>(
                base.Battle.Player.DamageDealing,
                new GameEventHandler<DamageDealingEventArgs>(this.OnPlayerDamageDealing),
                (GameEventPriority)0
            );
        }

        private void OnPlayerDamageDealing(DamageDealingEventArgs args)
        {
            if (
                args.ActionSource == this
                && args.DamageInfo.DamageType == DamageType.Attack
                && this.PlayerTotalFire > 0
            )
            {
                args.DamageInfo = args.DamageInfo.IncreaseBy(
                    this.PlayerTotalFire * (base.Value2 - 1)
                );
                args.AddModifier(this);
            }
        }

        protected override int BaseValue3 { get; set; } = 6;
        protected override int BaseUpgradedValue3 { get; set; } = 6;

        // protected override int AdditionalDamage
        // {
        //     get
        //     {
        //         if (base.Battle == null)
        //         {
        //             return 0;
        //         }

        //         int blueCardUsed = base
        //             .Battle.TurnCardUsageHistory.Where(
        //                 (Card card) => card.Config.Colors.Contains(ManaColor.Blue)
        //             )
        //             .Count<Card>();

        //         return Value2 * blueCardUsed;
        //     }
        // }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return base.AttackAction(selector);
            // yield return DebuffAction<CirnoRCharacterFrostSe>(selector.SelectedEnemy, 1, 1);
            // yield return BuffAction<Firepower>(1, 1);
            yield break;
        }
    }
}
