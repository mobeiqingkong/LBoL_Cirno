using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFrostKingDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 2 };
            config.UpgradedCost = new ManaGroup() { Blue = 2 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Self;

            config.Damage = 1;
            config.UpgradedDamage = 1;

            config.Keywords = Keyword.None;
            config.UpgradedKeywords = Keyword.Initial;
            config.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostEtchingImprintSe),
                nameof(CirnoRCharacterIceTearsSe),
            };
            config.UpgradedRelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostEtchingImprintSe),
                nameof(CirnoRCharacterIceTearsSe),
            };

            config.Block = 10;
            config.UpgradedBlock = 10;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Value2 = 1;
            config.UpgradedValue2 = 1;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFrostKingDef))]
    public sealed class CirnoRCharacterFrostKing : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // yield return new CastBlockShieldAction(Battle.Player, Battle.Player, base.Block, true);
            yield return BuffAction<CirnoRCharacterFrostEtchingImprintSe>(Value1, Value1);
            yield return BuffAction<CirnoRCharacterIceTearsSe>(Value1, Value1);
            yield break;
        }
    }
}
