using HarmonyLib;

namespace CirnoRCharacterMod
{
    public static class PInfo
    {
        //Rename the variable below to prevent conflicts between mod.
        public const string GUID = "author.game.typeofmod.CirnoRCharacter";
        public const string Name = "CirnoRCharacterMod";
        public const string version = "0.0.1";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
