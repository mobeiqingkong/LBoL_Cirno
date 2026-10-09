using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.Base;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using CirnoRCharacterMod.Cards.Template;
using CirnoRCharacterMod.Config;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace CirnoRCharacterMod
{
    [BepInPlugin(CirnoRCharacterMod.PInfo.GUID, CirnoRCharacterMod.PInfo.Name, CirnoRCharacterMod.PInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {
        //The Unique mod ID of the mod.
        //If defined, this is also the ID used by the Act 1 boss.
        //WARNING: It is mandatory to rename it to avoid issues.
        public static string modUniqueID = "CirnoRCharacterMod";
        //Name of the character.
        //This is also the prefix that is used before every .png file in DirResources. 
        public static string playerName = "CirnoRCharacter";
        //Whether to us an ingame or custom model.
        //InGame: Will load the character model of the ingame character.
        //Custom: Will load DirResource/CirnoRCharacterModel.png 
        public static bool useInGameModel = true;

        // ------------------------------------------------------------
        //  方案 C 开关：官方骨骼 + 官方动画 + 自建图集（可换贴图）
        //
        //  true  —— 走 Model.LoadPathCAsync()：
        //           骨骼 JSON 用 DirResources/cirno.json（官方逐字节副本），
        //           图集/贴图用 DirResources/cirno.atlas + cirno.png（可换），
        //           scale / defaultMix / 8 条动画过渡表在运行时从官方资产读出后灌入。
        //           => 官方动画手感全保留，画面可换皮。
        //  false —— 直接吃官方资产，连官方原皮一起拿来（换不了皮肤）。
        //
        //  换皮只需替换 DirResources/cirno.png，且**必须保持 1984x523 的原版布局**
        //  （每块 region 位置不动，只改像素）；改完不用重编译，重进游戏即可。
        // ------------------------------------------------------------
        public static bool useCustomAtlas = true;

        // ------------------------------------------------------------
        //  站立骨骼开关：冰之勇者自建 Spine 三件套
        //  （cirnoice.json / cirnoice.atlas / cirnoice.png，骨骼原点在【脚底】，与官方 root 一致）。
        //  true  —— 走 Model.LoadPathStandAsync()：14 骨 / 18 槽（root / waist / chest / head /
        //           双臂 / 双腿 / 披风 ×2 / 冰翼 ×4 / 眼睛 ×2），22 条动画。
        //           几何照抄官方 Cirno（Box/Shield/Hp），设计高 1680 -> StandScale。
        //  false —— 回退到方案 C（官方骨骼 + 换皮）。
        //
        //  ===== 眼睛状态是【数据驱动】的，本开关之外不需要任何额外配置 =====
        //   攻击（shoot1~4 的 start/loop/end、spell）-> 亮眼 eyes_bright（eyeLb/eyeRb，加法混合）
        //   防御 defend / 格挡 guard / 擦弹 graze        -> 绿眼 eyes_green（eyeLg/eyeRg）
        //   眨眼 blink 由 UnitView 自动在 track 1 播放（AllAnimationsNames 里名字对就行）
        //  详见 Model.cs 里站立骨骼段的注释。
        // ------------------------------------------------------------
        public static bool useStandModel = true;

        //If InGame is selected, this is the model that will be loaded. 
        //Check LBoL.EntityLib.EnemyUnits.Character or using LBoL.EntityLib.PlayerUnits for a list of all the characters available. 
        public static string modelName = nameof(LBoL.EntityLib.PlayerUnits.Cirno);
        // public static string modelName = nameof(CirunoR);
        //Some in-game model needs to be flipped (most notably elites).
        //玩家角色为 false。官方 "Cirno" 行的 Flip 本来就是 false，这里设 true 会让人物背对玩家。
        public static bool modelIsFlipped = false;
        //The character's off-color.
        //Used to separate cards in the card collection and put the off-color cards at the end.
        public static List<ManaColor> offColors = new List<ManaColor>() { ManaColor.Colorless };

        //Whether the Act 1 boss should be enabled.
        //The value can be customized LBoL/BepInEx/config/
        public static ConfigEntry<bool> enableAct1Boss;

        public static CustomConfigEntry<bool> enableAct1BossEntry = new CustomConfigEntry<bool>(
            value: false,
            section: "EnableAct1Boss",
            key: "EnableAct1Boss",
            description: "Toggle the Act 1 boss. Default: Off");

        // ------------------------------------------------------------
        //  模型整体上下偏移（世界单位）—— 仅作微调，默认 0
        //
        //  历史：v5 之前用自建骨骼时，立绘画的是「脚底贴画布底边」，模型原点因此落在
        //  脚底；而游戏把护罩光罩挂在 effectRoot（= 原点），于是上半身会被顶到光罩外，
        //  当时靠默认 -0.9775（= 半身高 1.955/2）把原点从脚底挪到身体中心。
        //
        //  现在走方案 C：骨骼设计尺寸就是官方的 687.85 x 1150，几何参数也照抄官方
        //  "Cirno" 行（Offset=(0,-1.0)、Hp=(0,-1.3)、Info=(0,1.2)），站位与官方完全一致，
        //  所以**默认必须是 0**。再叠一个 -0.9775 会把人往下拽半个身高。
        // ------------------------------------------------------------
        public static ConfigEntry<float> spineOffsetY;

        public const float SpineOffsetYDefault = 0f;

        public static CustomConfigEntry<float> spineOffsetYEntry = new CustomConfigEntry<float>(
            value: SpineOffsetYDefault,
            section: "SpineModel",
            key: "SpineOffsetY",
            description: "模型整体上下偏移（世界单位，正=上，负=下）。"
                       + "方案 C 用官方几何，默认 0 即与官方琪露诺站位一致。"
                       + "注意 BepInEx 只在配置文件缺失时才写默认值 —— 若旧的 "
                       + "BepInEx/config/*.cfg 里还留着 -0.9775，请手工改成 0 或删掉该行。");

        // ------------------------------------------------------------
        //  亮眼（eyeLb / eyeRb）的加法混合开关
        //
        //  骨骼 JSON 里这两个槽声明的是 "blend": "additive"。但 spine-unity 对**运行时自建**的
        //  SkeletonDataAsset 默认不会去换加法材质 —— BlendModeMaterials.applyAdditiveMaterial
        //  默认 false、requiresBlendModeMaterials 默认 false，ApplyMaterials() 进来第一行就 return。
        //  官方资产是编辑器里烘好的，所以我们这条运行时通路必须自己调
        //  SetupRuntimeBlendModeMaterials(true, {additiveTemplate})。
        //
        //  模板材质取 URP 的 "Universal Render Pipeline/Particles/Unlit"：它是本作里唯一一个
        //  把 _SrcBlend / _DstBlend / _ZWrite / _Cull 作为材质属性暴露出来的 shader（已从
        //  effects bundle 里核对过属性表），改成 Blend SrcAlpha One 就是标准加法，且它按顶点色
        //  调制，正好对上 Spine 写进网格的槽色 alpha 轨道。
        //
        //  true  —— 亮眼 = 真加法（和交付的 eyes_bright.gif 一致，会溢出发白的光晕）
        //  false —— 亮眼 = 普通 alpha 混合（浅青色高光，不会发光；出问题时的应急开关）
        // ------------------------------------------------------------
        public static ConfigEntry<bool> useAdditiveEyeGlow;

        public const bool UseAdditiveEyeGlowDefault = true;

        public static CustomConfigEntry<bool> useAdditiveEyeGlowEntry = new CustomConfigEntry<bool>(
            value: UseAdditiveEyeGlowDefault,
            section: "SpineModel",
            key: "UseAdditiveEyeGlow",
            description: "亮眼(攻击时)是否使用加法混合。true=发光(默认)；false=普通高光。"
                       + "若亮眼出现色块/发白异常，把这里改成 false 即可，不用重编译。");

        // ------------------------------------------------------------
        //  对白总开关
        //
        //  false —— 完全忽略插件目录下的 Dialog*.yaml：不读文件、不播任何气泡
        //           （开场白 / 与敌人的交替对白 / 击败语 / 击败正邪的结局台词，全部不出现）。
        //           想自己重写台词、或就是不想看对白的玩家把它关掉即可。
        //
        //  只影响【对白】。卡牌/状态/展品的本地化（Cards*.yaml 等）不受影响。
        //  ⚠️ BepInEx 只在启动时读配置 ⇒ 改完要重启游戏。
        // ------------------------------------------------------------
        public static ConfigEntry<bool> enableDialogue;

        public const bool EnableDialogueDefault = true;

        public static CustomConfigEntry<bool> enableDialogueEntry = new CustomConfigEntry<bool>(
            value: EnableDialogueDefault,
            section: "Dialogue",
            key: "EnableDialogue",
            description: "是否启用本 mod 的对白（开场白 / 与敌人的交替对白 / 击败语 / 结局台词）。"
                       + "false = 完全忽略 Dialog*.yaml，一句话都不说（卡牌本地化不受影响）。"
                       + "改完需重启游戏。默认：true");

        private static readonly Harmony harmony = CirnoRCharacterMod.PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;

        internal static TemplateSequenceTable sequenceTable = new TemplateSequenceTable();

        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());

        // add this for audio loading
        internal static DirectorySource directorySource = new DirectorySource(CirnoRCharacterMod.PInfo.GUID, "");


        private void Awake()
        {
            log = Logger;
            // Load the custom config entry.
            enableAct1Boss = Config.Bind(enableAct1BossEntry.Section, enableAct1BossEntry.Key, enableAct1BossEntry.Value, enableAct1BossEntry.Description);
            spineOffsetY = Config.Bind(spineOffsetYEntry.Section, spineOffsetYEntry.Key, spineOffsetYEntry.Value, spineOffsetYEntry.Description);
            useAdditiveEyeGlow = Config.Bind(useAdditiveEyeGlowEntry.Section, useAdditiveEyeGlowEntry.Key, useAdditiveEyeGlowEntry.Value, useAdditiveEyeGlowEntry.Description);
            enableDialogue = Config.Bind(enableDialogueEntry.Section, enableDialogueEntry.Key, enableDialogueEntry.Value, enableDialogueEntry.Description);

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            CardIndexGenerator.PromiseClearIndexSet();
            EntityManager.RegisterSelf();

            harmony.PatchAll();

            //Watermark is now unused as of 1.8.0.
            //The code is temporarily kept if the devs change their minds.
            /*if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
                WatermarkWrapper.ActivateWatermark();*/

            Func<Sprite> getSprite = () => ResourceLoader.LoadSprite("BossIcon.png", directorySource);
            EnemyUnitTemplate.AddBossNodeIcon(nameof(CirnoRCharacterMod.Enemies.CirnoRCharacterMod), getSprite);
        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }
    }
}
