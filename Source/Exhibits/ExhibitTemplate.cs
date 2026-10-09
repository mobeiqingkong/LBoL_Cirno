using CirnoRCharacterMod.Config;
using CirnoRCharacterMod.ImageLoader;
using CirnoRCharacterMod.Localization;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;

namespace CirnoRCharacterMod.Exhibits
{
    public class CirnoRCharacterExhibitTemplate : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return CirnoRCharacterDefaultConfig.DefaultID(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return CirnoRCharacterLocalization.ExhibitsBatchLoc.AddEntity(this);
        }

        public override ExhibitSprites LoadSprite()
        {
            return CirnoRCharacterImageLoader.LoadExhibitSprite(exhibit: this);
        }

        public override ExhibitConfig MakeConfig()
        {
            return GetDefaultExhibitConfig();
        }

        public ExhibitConfig GetDefaultExhibitConfig()
        {
            return CirnoRCharacterDefaultConfig.DefaultExhibitConfig();
        }
    }
}
