using System;
using System.Collections.Generic;
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
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards;
using LBoL.EntityLib.StatusEffects;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.EntityLib.StatusEffects.ExtraTurn;
using LBoLEntitySideloader.Attributes;

namespace CirnoRCharacterMod.CirnoRCharacterUlt
{
    public sealed class CirnoRCharacterUltADef : CirnoRCharacterUltTemplate
    {
        public override UltimateSkillConfig MakeConfig()
        {
            UltimateSkillConfig config = GetDefaulUltConfig();
            config.Damage = 9;
            config.Value1 = 1;
            // Add the relative status effects in the description box.
            // config.RelativeEffects = new List<string>() { nameof(Weak) };
            return config;
        }
    }

    [EntityLogic(typeof(CirnoRCharacterUltADef))]
    public sealed class CirnoRCharacterUltA : UltimateSkill
    {
        public CirnoRCharacterUltA()
        {
            base.TargetType = TargetType.AllEnemies;
            base.GunName = GunNameID.GetGunFromId(4158);
        }

        public ManaGroup Mana { get; set; }

        // protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        // {
        //     foreach (Unit enemy in base.Battle.AllAliveEnemies)
        //     {
        //         yield return new DamageAction(base.Owner, enemy, this.Damage, base.GunName, GunType.Single);
        //     }
        //     yield return BuffAction<ExtraTurn>(1, 0, 0, 0, 0.2f);
        //     yield return new RequestEndPlayerTurnAction();
        //     yield break;
        // }
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {
            foreach (Unit enemy in base.Battle.AllAliveEnemies)
            {
                yield return new ApplyStatusEffectAction<CirnoRCharacterFrostSe>(enemy, 1, 1);
            }
            yield return new ApplyStatusEffectAction<ExtraTurn>(Battle.Player, 1, 0, 0, 0, 0.2f)
            { };
            // BuffAction<ExtraTurn>(1, 0, 0, 0, 0.2f);
            yield return new RequestEndPlayerTurnAction();
            yield return new ApplyStatusEffectAction<TimeIsLimited>(
                Battle.Player,
                Value1,
                0,
                0,
                0,
                0.2f
            )
            { };
            yield break;
        }
    }
}
