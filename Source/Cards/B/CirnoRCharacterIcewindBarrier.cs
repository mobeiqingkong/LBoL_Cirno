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
using LBoL.EntityLib.StatusEffects.Basic;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterIcewindBarrierDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(400);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            //Mana to consume aside from X
            config.Cost = new ManaGroup() { Blue = 1, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Blue = 1, Any = 1 };
            config.Rarity = Rarity.Uncommon;

            //The XCost has to be set.
            config.IsXCost = false;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 1;
            config.UpgradedValue1 = 2;

            config.Value2 = 6;
            config.UpgradedValue2 = 6;

            config.Block = 6;
            config.UpgradedBlock = 6;

            config.Shield = 6;
            config.UpgradedShield = 6;

            config.RelativeEffects = new List<string>() { nameof(Reflect) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(Reflect) };
            config.Keywords = Keyword.Shield | Keyword.Block | Keyword.Exile;
            config.UpgradedKeywords =
                Keyword.Shield | Keyword.Block | Keyword.Exile | Keyword.Initial;
            // config.UpgradedKeywords = Keyword.Retain | Keyword.Forbidden;
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterIcewindBarrierDef))]
    public sealed class CirnoRCharacterIcewindBarrier : CirnoRCharacterCard
    {
        public override Interaction Precondition()
        {
            List<Card> list = base
                .Battle.HandZone.Where((Card card) => card != this)
                .ToList<Card>();
            if (list.Count <= 0)
            {
                return null;
            }
            return new SelectHandInteraction(0, base.Value1, list);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // If a card was selected.
            if (precondition != null)
            {
                IReadOnlyList<Card> cards = ((SelectHandInteraction)precondition).SelectedCards;
                if (cards.Count > 0)
                {
                    foreach (Card card in cards)
                    {
                        // // 放逐
                        // yield return new ExileCardAction(card);
                        // // 移除出本次战斗
                        // yield return new RemoveCardAction(card);
                        // 丢弃
                        yield return new ExileCardAction(card);

                        // yield return base.AttackAllAliveEnemyAction();
                        yield return new ApplyStatusEffectAction<Reflect>(
                            Battle.Player,
                            base.Value2,
                            0,
                            0,
                            0,
                            0.2f
                        );
                        yield return new CastBlockShieldAction(
                            base.Battle.Player,
                            base.Battle.Player,
                            base.Block,
                            base.Shield,
                            true
                        );
                    }
                }
            }
            yield break;
        }
    }
}
