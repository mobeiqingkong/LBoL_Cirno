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
    public sealed class CirnoRCharacterSuperIceKickDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(401);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 6;
            config.UpgradedDamage = 6;

            config.Keywords = Keyword.Accuracy;
            config.UpgradedKeywords = Keyword.Accuracy;

            config.Value1 = 1;
            config.UpgradedValue1 = 1;

            config.Value2 = 2;
            config.UpgradedValue2 = 2;

            config.RelativeEffects = new List<string>() { };
            config.UpgradedRelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };

            config.Illustrator = "幻想万华镜";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterSuperIceKickDef))]
    public sealed class CirnoRCharacterSuperIceKick : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return base.AttackAction(selector, base.GunName);
            yield return new DrawManyCardAction(base.Value1);
            //Only gain the additional effect if battle wouldn't end and the card is upgraded.
            if (!base.Battle.BattleShouldEnd && base.IsUpgraded)
            {
                for (int i = 0; i < base.Value2; i++)
                {
                    yield return DebuffAction<CirnoRCharacterFrostSe>(selector.SelectedEnemy, 1, 1);
                }
            }
            yield break;
        }
    }
}
