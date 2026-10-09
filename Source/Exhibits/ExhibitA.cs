using System;
using System.Collections.Generic;
using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoLEntitySideloader.Attributes;
using UnityEngine;

namespace CirnoRCharacterMod.Exhibits
{
    public sealed class CirnoRCharacterExhibitADef : CirnoRCharacterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            ExhibitConfig exhibitConfig = this.GetDefaultExhibitConfig();

            exhibitConfig.Mana = new ManaGroup() { Blue = 1 };
            exhibitConfig.Value1 = 1;
            exhibitConfig.Value2 = 3;
            exhibitConfig.BaseManaColor = ManaColor.Blue;
            // exhibitConfig.RelativeCards = new List<string>() { nameof(RManaCard) };
            exhibitConfig.RelativeEffects = new List<string>()
            {
                nameof(CirnoRCharacterFrostEtchingImprintSe),
                // nameof(CirnoRCharacterFrostCondensationSe),
                // nameof(CirnoRCharacterFrostSe),
            };
            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterExhibitADef))]
    public sealed class CirnoRCharacterExhibitA : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent<UnitEventArgs>(
                base.Battle.Player.TurnStarting,
                new EventSequencedReactor<UnitEventArgs>(this.OnPlayerTurnStarted)
            );
        }

        private IEnumerable<BattleAction> OnPlayerTurnStarted(UnitEventArgs args)
        {
            if (base.Battle.Player.TurnCounter == 1)
            {
                base.NotifyActivating();
                // yield return new ApplyStatusEffectAction<Firepower>(base.Battle.Player, base.Value1, 0, 0, 0, 0.2f);
                // yield return new AddCardsToHandAction(new Card[] { Library.CreateCard<RManaCard>() });
                // foreach (Unit enemy in base.Battle.AllAliveEnemies)
                // {
                //     yield return new ApplyStatusEffectAction<LockedOn>(enemy, base.Value2, 0, 0, 0, 0.2f);
                //     // enemy = null;
                // }
                yield return new ApplyStatusEffectAction<CirnoRCharacterFrostEtchingImprintSe>(
                    base.Owner,
                    Value1,
                    Value1
                );
            }
            yield break;
        }
    }
}
