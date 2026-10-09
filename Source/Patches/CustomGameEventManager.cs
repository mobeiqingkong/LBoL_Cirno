using CirnoRCharacterMod.BattleActions;
using HarmonyLib;
using LBoL.Core;

namespace CirnoRCharacterMod.Patches
{
    [HarmonyPatch]
    class CustomGameEventManager
    {
        public static GameEvent<BuffAttackEventArgs> PreCustomEvent { get; set; }
        public static GameEvent<BuffAttackEventArgs> PostCustomEvent { get; set; }

        public static GameEvent<DamageEventArgs> PostCustomDamageEvent { get; set; }

        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.EnterBattle))]
        private static bool Prefix(GameRunController __instance)
        {
            PreCustomEvent = new GameEvent<BuffAttackEventArgs>();
            PostCustomEvent = new GameEvent<BuffAttackEventArgs>();
            PostCustomDamageEvent = new GameEvent<DamageEventArgs>();
            return true;
        }
    }
}
