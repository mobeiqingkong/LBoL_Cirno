using System.Collections.Generic;
using CirnoRCharacterMod.Cards;
using CirnoRCharacterMod.CirnoRCharacterUlt;
using CirnoRCharacterMod.Exhibits;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.Cards.Neutral.NoColor;

namespace CirnoRCharacterMod
{
    public class CirnoRCharacterLoadouts
    {
        public static string UltimateSkillA = nameof(CirnoRCharacterUltA);
        public static string UltimateSkillB = nameof(CirnoRCharacterUltB);

        public static string ExhibitA = nameof(CirnoRCharacterExhibitA);
        public static string ExhibitB = nameof(CirnoRCharacterExhibitB);
        public static List<string> DeckA = new List<string>
        {
            nameof(Shoot),
            nameof(Shoot),
            nameof(Boundary),
            nameof(Boundary),
            nameof(CirnoAttackU),
            nameof(CirnoAttackU),
            nameof(CirnoBlockG),
            nameof(CirnoBlockG),
            nameof(CirnoBlockG),
            nameof(CirnoRCharacterInstantFreezeRay),
            nameof(CirnoRCharacterSwordFreezer),
            // nameof(CirnoRCharacterBlockB),
            // nameof(CirnoRCharacterBlockB),
            // nameof(CirnoRCharacterMultiAttack),
        };

        public static List<string> DeckB = new List<string>
        {
            nameof(Shoot),
            nameof(Shoot),
            nameof(Boundary),
            nameof(Boundary),
            nameof(CirnoAttackG),
            nameof(CirnoAttackG),
            nameof(CirnoBlockU),
            nameof(CirnoBlockU),
            nameof(CirnoBlockU),
            nameof(CirnoRCharacterCoolSunflower),
            nameof(CirnoRCharacterThornyJungle),
        };

        public static PlayerUnitConfig playerUnitConfig = new PlayerUnitConfig(
            Id: BepinexPlugin.modUniqueID,
            HasHomeName: true,
            ShowOrder: 8,
            Order: 0,
            UnlockLevel: 0,
            ModleName: "",
            NarrativeColor: "#e58c27",
            IsSelectable: true,
            MaxHp: 65,
            InitialMana: new ManaGroup()
            {
                Blue = 2,
                Red = 0,
                Green = 2,
                Colorless = 0,
                Philosophy = 0,
            },
            InitialMoney: 100,
            InitialPower: 0,
            BasicRingOrder: null,
            LeftColor: ManaColor.Blue,
            RightColor: ManaColor.Green,
            UltimateSkillA: CirnoRCharacterLoadouts.UltimateSkillA,
            UltimateSkillB: CirnoRCharacterLoadouts.UltimateSkillB,
            ExhibitA: CirnoRCharacterLoadouts.ExhibitA,
            ExhibitB: CirnoRCharacterLoadouts.ExhibitB,
            DeckA: CirnoRCharacterLoadouts.DeckA,
            DeckB: CirnoRCharacterLoadouts.DeckB,
            DifficultyA: 1,
            DifficultyB: 1
        );
    }
}
