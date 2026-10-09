using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CirnoRCharacterMod.BattleActions;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.StatusEffects;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Character.Cirno.Friend;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Utils.ArrayExtensions;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterFairyWindDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>()
            {
                ManaColor.Green,
                ManaColor.Blue,
                ManaColor.Black,
                ManaColor.White,
            };
            config.Cost = new ManaGroup()
            {
                // Any = 1, Hybrid = 1, HybridColor = 6
                Green = 1,
                Blue = 1,
                Black = 1,
                White = 1,
            };
            config.UpgradedCost = new ManaGroup()
            {
                Hybrid = 1,
                HybridColor = 6,
                Black = 1,
                White = 1,
            };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.AllEnemies;

            config.Value1 = 6;
            config.UpgradedValue1 = 8;

            config.Value2 = 6;
            config.UpgradedValue2 = 8;

            config.Mana = new ManaGroup() { Philosophy = 1 };
            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            // config.RelativeCards = new List<string>() { nameof(SunnyFriend), nameof(StarFriend), nameof(LunaFriend), nameof(LarvaFriend), nameof(LilyFriend), nameof(DayaojingFriend) };
            // config.UpgradedRelativeCards = new List<string>() { nameof(SunnyFriend), nameof(StarFriend), nameof(LunaFriend), nameof(LarvaFriend), nameof(LilyFriend), nameof(DayaojingFriend) };

            config.Illustrator = "HAM";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterFairyWindDef))]
    public sealed class CirnoRCharacterFairyWind : CirnoRCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            // ManaGroup leftMana = base.Battle.BattleMana;
            // ManaGroup newMana = new ManaGroup() { Philosophy = leftMana.Amount };
            // yield return new ConvertManaAction(leftMana, newMana, false);

            // List<Card> handCards = base.Battle.HandZone.Where((Card card) => card != this).ToList<Card>();
            // foreach (Card card in handCards)
            // {
            //     // 丢弃所有手牌
            //     yield return new DiscardAction(card);
            // }

            // List<Card> cards = new List<Card>();
            // Card sunnyFriend = Library.CreateCard<SunnyFriend>(base.IsUpgraded);
            // cards.Add(sunnyFriend);
            // Card starFriend = Library.CreateCard<StarFriend>(base.IsUpgraded);
            // cards.Add(starFriend);
            // Card lunaFriend = Library.CreateCard<LunaFriend>(base.IsUpgraded);
            // cards.Add(lunaFriend);
            // Card larvaFriend = Library.CreateCard<LarvaFriend>(base.IsUpgraded);
            // cards.Add(larvaFriend);
            // Card lilyFriend = Library.CreateCard<LilyFriend>(base.IsUpgraded);
            // cards.Add(lilyFriend);
            // Card dayaojingFriend = Library.CreateCard<DayaojingFriend>(base.IsUpgraded);
            // cards.Add(dayaojingFriend);
            // foreach (Card card in cards)
            // {
            //     card.IsEthereal = true;
            // }
            // // Library.CreateCards<DayaojingFriend>(base.Value1, base.IsUpgraded);
            // yield return new AddCardsToHandAction(cards);
            foreach (Unit enemy in base.Battle.AllAliveEnemies)
            {
                foreach (StatusEffect se in enemy.StatusEffects.ToList())
                {
                    if (se.Type == StatusEffectType.Positive)
                    {
                        yield return new RemoveStatusEffectAction(se, true, 0.2f);
                    }
                }
            }
            yield return new RequestEndPlayerTurnAction();
            yield break;
        }
    }
}
