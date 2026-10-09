using CirnoRCharacterMod.Dialogue;
using CirnoRCharacterMod.ImageLoader;
using CirnoRCharacterMod.Localization;
using Cysharp.Threading.Tasks;
//using DG.Tweening;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;

//using CirnoRCharacterMod.BattleActions;

namespace CirnoRCharacterMod
{
    public sealed class CirnoRCharacterModDef : PlayerUnitTemplate
    {
        public UniTask<Sprite>? LoadSpellPortraitAsync { get; private set; }

        public override IdContainer GetId()
        {
            return BepinexPlugin.modUniqueID;
        }

        public override LocalizationOption LoadLocalization()
        {
            return CirnoRCharacterLocalization.PlayerUnitBatchLoc.AddEntity(this);
        }

        public override PlayerImages LoadPlayerImages()
        {
            return CirnoRCharacterImageLoader.LoadPlayerImages(BepinexPlugin.playerName);
        }

        public override PlayerUnitConfig MakeConfig()
        {
            return CirnoRCharacterLoadouts.playerUnitConfig;
        }

        [EntityLogic(typeof(CirnoRCharacterModDef))]
        public sealed class CirnoRCharacterMod : PlayerUnit
        {
            /// <summary>
            /// 全人物对话 + 结局分流的挂载点。
            /// 见 Source/Dialogue/：进场说见面语；击败有名角色补一句；
            /// 击败最终 BOSS 正邪时按是否持有【境界感知仪】播放 Good / Bad 结局台词
            /// （判据用游戏自带的 CanEnterTrueEnding()，与真实结局分支同源）。
            /// </summary>
            protected override void OnEnterBattle(BattleController battle)
            {
                base.OnEnterBattle(battle);
                ReactBattleEvent(battle.BattleStarted,
                    args => CirnoRDialogue.OnBattleStarted(battle));
                ReactBattleEvent(battle.EnemyDied,
                    args => CirnoRDialogue.OnEnemyDied(battle, args));
            }
        }
    }
}
