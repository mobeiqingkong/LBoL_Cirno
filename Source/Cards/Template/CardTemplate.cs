using CirnoRCharacterMod.Config;
using CirnoRCharacterMod.ImageLoader;
using CirnoRCharacterMod.Localization;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;

namespace CirnoRCharacterMod.Cards.Template
{
    public abstract class CirnoRCharacterCardTemplate : CardTemplate
    {
        public override IdContainer GetId()
        {
            return CirnoRCharacterDefaultConfig.DefaultID(this);
        }

        public override CardImages LoadCardImages()
        {
            return CirnoRCharacterImageLoader.LoadCardImages(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return CirnoRCharacterLocalization.CardsBatchLoc.AddEntity(this);
        }

        public CardConfig GetDefaultCardConfig()
        {
            return CirnoRCharacterDefaultConfig.DefaultCardConfig();
        }
    }
}
