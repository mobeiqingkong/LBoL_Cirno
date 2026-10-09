using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using CirnoRCharacterMod.Enemies.Template;


namespace CirnoRCharacterMod.Enemies
{
    public sealed class CirnoRCharacterEnemyGroupDef : CirnoRCharacterEnemyGroupTemplate
    {
        public override IdContainer GetId() => nameof(CirnoRCharacterMod);

        public override EnemyGroupConfig MakeConfig()
        {
            EnemyGroupConfig config = GetDefaultEnemyGroupConfig();
            config.Name = nameof(CirnoRCharacterMod);
            config.FormationName = VanillaFormations.Single;
            config.Enemies = new List<string>() { nameof(CirnoRCharacterMod) };
            config.EnemyType = EnemyType.Boss;
            config.RollBossExhibit = true;

            return config;
        }
    }
}