using System;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using CirnoRCharacterMod.Config;
using CirnoRCharacterMod.Localization;


namespace CirnoRCharacterMod.Enemies.Template
{
    public class CirnoRCharacterEnemyUnitTemplate : EnemyUnitTemplate
    {
        public override IdContainer GetId()
        {
            return CirnoRCharacterDefaultConfig.DefaultID(this);
        }

        public override EnemyUnitConfig MakeConfig()
        {
            return CirnoRCharacterDefaultConfig.DefaultEnemyUnitConfig();
        }

        public override LocalizationOption LoadLocalization()
        {
            return CirnoRCharacterLocalization.EnemiesUnitBatchLoc.AddEntity(this);
        }

        public override Type TemplateType()
        {
            return typeof(EnemyUnitTemplate);
        }

        public EnemyUnitConfig GetDefaultEnemyUnitConfig()
        {
            return CirnoRCharacterDefaultConfig.DefaultEnemyUnitConfig();
        }


    }
}