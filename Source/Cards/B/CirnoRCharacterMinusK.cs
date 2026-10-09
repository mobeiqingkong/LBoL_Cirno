using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.BattleModifiers.Actions;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterMinusKDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(14020);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            //Mana to consume aside from X
            config.Cost = new ManaGroup() { Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 0 };
            config.Rarity = Rarity.Common;

            //The XCost has to be set.
            config.IsXCost = false;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 3;
            config.UpgradedValue1 = 5;

            config.Value2 = 6;
            config.UpgradedValue2 = 12;

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            config.Illustrator = "";
            config.RelativeEffects = new List<string>() { nameof(FrostArmor), nameof(Reflect) };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(FrostArmor),
                nameof(Reflect),
            };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterMinusKDef))]
    public sealed class CirnoRCharacterMinusK : CirnoRCharacterCard
    {
        // Cost -1 less for each card in hand.
        protected override int BaseValue3 { get; set; } = 6;
        protected override int BaseUpgradedValue3 { get; set; } = 12;

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return LoseLifeAction(base.Value1);
            yield return BuffAction<FrostArmor>(base.Value2, base.Value2);
            // yield return new ApplyStatusEffectAction<FrostArmor>(Battle.Player, base.Value2, 0, 0, 0, 0.2f);
            yield return BuffAction<Reflect>(base.Value3, 0, 0, 0, 0.2f);
        }
    }
}
