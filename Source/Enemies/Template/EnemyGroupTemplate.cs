using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using CirnoRCharacterMod.Config;


namespace CirnoRCharacterMod.Enemies.Template
{
    public abstract class CirnoRCharacterEnemyGroupTemplate : EnemyGroupTemplate
    {
        public override IdContainer GetId()
        {
            return CirnoRCharacterDefaultConfig.DefaultID(this);
        }

        public override EnemyGroupConfig MakeConfig()
        {
            return CirnoRCharacterDefaultConfig.DefaultEnemyGroupConfig();
        }

        public EnemyGroupConfig GetDefaultEnemyGroupConfig()
        {
            return CirnoRCharacterDefaultConfig.DefaultEnemyGroupConfig();
        }
    }
}