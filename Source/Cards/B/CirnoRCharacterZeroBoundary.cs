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
    public sealed class CirnoRCharacterZeroBoundaryDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(12211);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            //Mana to consume aside from X
            config.Cost = new ManaGroup() { Blue = 2, Any = 2 };
            config.Rarity = Rarity.Uncommon;

            //The XCost has to be set.
            config.IsXCost = false;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Damage = 4;
            config.UpgradedDamage = 6;

            config.Block = 4;
            config.UpgradedBlock = 6;

            config.Value1 = 4;
            config.UpgradedValue1 = 6;

            config.Keywords = Keyword.Block | Keyword.Exile;
            config.UpgradedKeywords = Keyword.Block | Keyword.Exile;
            // config.RelativeEffects = new List<string>() { nameof(Reflect) };
            config.Illustrator = "茶まるく";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterZeroBoundaryDef))]
    public sealed class CirnoRCharacterZeroBoundary : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // yield return new ApplyStatusEffectAction<Reflect>(Battle.Player, base.Value1, 0, 0, 0, 0.2f);
            // If a card was selected.
            List<Card> cards = base
                .Battle.HandZone.Where((Card card) => card != this)
                .ToList<Card>();
            // IReadOnlyList<Card> cards = base.Battle.HandZone.Where((Card card) => card != this)

            foreach (Card card in cards)
            {
                // // 放逐
                // yield return new ExileCardAction(card);
                // // 移除出本次战斗
                // yield return new RemoveCardAction(card);
                // 丢弃
                yield return new DiscardAction(card);
                yield return new CastBlockShieldAction(
                    base.Battle.Player,
                    base.Battle.Player,
                    base.Block,
                    true
                );
                yield return base.AttackAllAliveEnemyAction();
            }
            // yield return new ApplyStatusEffectAction<LBoL.Core.StatusEffects.Firepower>(Battle.Player, base.Value1, 0, 0, 0, 0.2f) { };
            // yield return new ApplyStatusEffectAction<LBoL.Core.StatusEffects.TempFirepower>(Battle.Player, base.Value2, 0, 0, 0, 0.2f) { };
            // yield break;
        }
    }
}
