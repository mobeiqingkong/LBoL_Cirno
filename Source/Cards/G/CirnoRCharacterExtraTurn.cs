using System.Collections.Generic;
using CirnoRCharacterMod.Cards.Template;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.Cards;
using LBoL.EntityLib.StatusEffects.ExtraTurn;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.Cards
{
    public sealed class CirnoRCharacterExtraTurnDef : CirnoRCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetDefaultCardConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Green };
            config.Cost = new ManaGroup() { Green = 2, Any = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            //Mana config for the Time Limit
            config.Mana = new ManaGroup() { Any = 1 };
            config.UpgradedMana = new ManaGroup() { Any = 1 };

            config.RelativeEffects = new List<string>() { nameof(TimeIsLimited) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(TimeIsLimited) };

            config.Keywords = Keyword.Exile;
            config.UpgradedKeywords = Keyword.Exile;

            config.Illustrator = "ロリメイト＠コミケ予定だった";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterExtraTurnDef))]
    //TimeStopCards inhehit from LimitedStopTimeCard instead of Card
    public sealed class CirnoRCharacterExtraTurn : LimitedStopTimeCard
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector,
            ManaGroup consumingMana,
            Interaction precondition
        )
        {
            yield return BuffAction<ExtraTurn>(1, 0, 0, 0, 0.2f);

            yield return new RequestEndPlayerTurnAction();
            if (base.Limited)
            {
                yield return DebuffAction<TimeIsLimited>(
                    base.Battle.Player,
                    1,
                    0,
                    0,
                    0,
                    true,
                    0.2f
                );
            }
            yield break;
        }
    }
}
