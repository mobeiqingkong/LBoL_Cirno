using System.Collections.Generic;
using System.Linq;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.GunName;
using CirnoRCharacterMod.StatusEffects;
using DG.Tweening;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Marisa;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using Mono.CompilerServices.SymbolWriter;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterUltimateBlizzardDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.GunName = GunNameID.GetGunFromId(14051);

            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            // Mana to consume aside from X
            config.Cost = new ManaGroup() { Blue = 1 };
            config.Rarity = Rarity.Rare;

            // The XCost has to be set.
            config.IsXCost = true;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 4;
            config.UpgradedDamage = 6;

            config.Shield = 4;
            config.UpgradedShield = 6;

            config.Value1 = 6;
            config.UpgradedValue1 = 8;

            config.Value2 = 4;
            config.UpgradedValue2 = 6;

            config.Mana = new ManaGroup() { Any = 1 };

            config.Keywords = Keyword.Shield;
            config.UpgradedKeywords = Keyword.Shield;

            config.RelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(CirnoRCharacterFrostSe) };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterUltimateBlizzardDef))]
    public sealed class CirnoRCharacterUltimateBlizzard : CirnoRCharacterCard
    {
        // // Create another ManaGroup that can be used to display the UU mana on the card.
        private ManaGroup _blueMana = new ManaGroup() { Blue = 1 };
        public ManaGroup BlueMana
        {
            get { return _blueMana; }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            base.CardGuns = new Guns(base.GunName, 1, false);
            // foreach (GunPair gunPair in base.CardGuns.GunPairs)
            // {
            //     yield return base.AttackAllAliveEnemyAction(gunPair);
            // }
            // // yield return base.AttackAllAliveEnemyAction();

            int blueMana = base.SynergyAmount(consumingMana, ManaColor.Blue, 1);
            // yield return new HealAction(base.Battle.Player, base.Battle.Player, blueMana * base.Value2);
            // yield return BuffAction<FrostArmor>(blueMana * base.Value2, blueMana * base.Value2);
            for (int i = 0; i < blueMana; i++)
            {
                yield return new CastBlockShieldAction(
                    base.Battle.Player,
                    base.Battle.Player,
                    base.Shield,
                    true
                );
            }

            int anyMana = base.SynergyAmount(consumingMana, ManaColor.Any, 1);
            for (int i = 0; i < anyMana; i++)
            {
                foreach (GunPair gunPair in base.CardGuns.GunPairs)
                {
                    yield return base.AttackAllAliveEnemyAction(gunPair);
                }

                foreach (Unit enemy in base.Battle.AllAliveEnemies)
                {
                    yield return base.DebuffAction<CirnoRCharacterFrostSe>(enemy, 1, 1);
                }
            }
            yield break;
        }
    }
}
