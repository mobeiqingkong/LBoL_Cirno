using CirnoRCharacterMod.Config;
using CirnoRCharacterMod.ImageLoader;
using CirnoRCharacterMod.Localization;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;

namespace CirnoRCharacterMod.StatusEffects
{
    public class CirnoRCharacterStatusEffectTemplate : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return CirnoRCharacterDefaultConfig.DefaultID(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return CirnoRCharacterLocalization.StatusEffectsBatchLoc.AddEntity(this);
        }

        public override Sprite LoadSprite()
        {
            return CirnoRCharacterImageLoader.LoadStatusEffectLoader(status: this);
        }

        public override StatusEffectConfig MakeConfig()
        {
            return GetDefaultStatusEffectConfig();
        }

        public static StatusEffectConfig GetDefaultStatusEffectConfig()
        {
            return CirnoRCharacterDefaultConfig.DefaultStatusEffectConfig();
        }
    }
}
