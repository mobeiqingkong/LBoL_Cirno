using CirnoRCharacterMod.StatusEffects;
using LBoL.Base;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;

namespace CirnoRCharacterMod.Cards.Template
{
    public class CirnoRCharacterCard : Card
    {
        //CirnoRCharacterCard can be used to give additional properties to all the cards.
        //For instance, this can be used to give every card a new custom parameter called Value3.
        //Custom value for display purposes.
        protected virtual int BaseValue3 { get; set; } = 0;
        protected virtual int BaseUpgradedValue3 { get; set; } = 0;
        public int Value3
        {
            get
            {
                if (this.IsUpgraded)
                {
                    return BaseUpgradedValue3;
                }
                return BaseValue3;
            }
        }

        protected virtual int BaseValue4 { get; set; } = 0;
        protected virtual int BaseUpgradedValue4 { get; set; } = 0;
        public int Value4
        {
            get
            {
                if (this.IsUpgraded)
                {
                    return BaseUpgradedValue4;
                }
                return BaseValue4;
            }
        }

        protected virtual ManaGroup baseOverFlowMana { get; set; } = new ManaGroup() { };
        protected virtual ManaGroup baseUpgradedOverFlowMana { get; set; } = new ManaGroup() { };
        public ManaGroup overFlowMana
        {
            get
            {
                if (this.IsUpgraded)
                {
                    return baseUpgradedOverFlowMana;
                }
                return baseOverFlowMana;
            }
        }

        protected virtual StatusEffect tempCustomStatusEffect { get; set; } = null;
        protected virtual StatusEffect tempUpgradedCustomStatusEffect { get; set; } = null;
        public StatusEffect customStatusEffect
        {
            get
            {
                if (this.IsUpgraded)
                {
                    return tempUpgradedCustomStatusEffect;
                }
                return tempCustomStatusEffect;
            }
        }
    }
}
