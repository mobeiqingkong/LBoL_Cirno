using CirnoRCharacterMod.Config;
using CirnoRCharacterMod.ImageLoader;
using CirnoRCharacterMod.Localization;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;

namespace CirnoRCharacterMod.CirnoRCharacterUlt
{
    public class CirnoRCharacterUltTemplate : UltimateSkillTemplate
    {
        public override IdContainer GetId()
        {
            return CirnoRCharacterDefaultConfig.DefaultID(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return CirnoRCharacterLocalization.UltimateSkillsBatchLoc.AddEntity(this);
        }

        public override Sprite LoadSprite()
        {
            return CirnoRCharacterImageLoader.LoadUltLoader(ult: this);
        }

        public override UltimateSkillConfig MakeConfig()
        {
            throw new System.NotImplementedException();
        }

        public UltimateSkillConfig GetDefaulUltConfig()
        {
            return CirnoRCharacterDefaultConfig.DefaultUltConfig();
        }
    }
}
