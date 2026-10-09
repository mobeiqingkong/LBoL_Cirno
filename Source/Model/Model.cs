using CirnoRCharacterMod.Localization;
using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using Spine;
using Spine.Unity;
using System.Text;
using UnityEngine;

namespace CirnoRCharacterMod.model
{
    /// <summary>
    /// 方案 C —— 官方骨骼 + 官方动画 + 自建图集（可换贴图）。
    ///
    /// 为什么这么做
    /// ------------
    /// 纯「用官方资产」（useInGameModel = true 且不开 useCustomAtlas）会连官方那张皮
    /// 一起拿来，换不了皮肤；纯「运行时自建」又会丢掉官方 SkeletonDataAsset 上的
    /// defaultMix / 动画过渡表。本类两条都要：
    /// 骨骼 JSON 用官方那份（逐字节副本，见 DirResources/cirno.json），图集 + 贴图用
    /// 我们自己的，而 scale / defaultMix / 8 条过渡表**在运行时从官方资产读出来**再灌进
    /// 自建资产 —— 官方动画手感一点不丢，画面却可以随便换。
    ///
    /// 三条必须守住的硬约束（都是实测踩出来的）
    /// ----------------------------------------
    /// 1. 绝不修改官方资产本身。它是 Addressables 的全局共享实例，改它的材质会让原版
    ///    琪露诺一起被换皮。所以只读字段 + new Material(官方材质) 克隆。
    /// 2. .atlas 首页名与 Texture2D.name 必须都是 "cirno"（spine-unity 拿页名去掉 .png 后
    ///    做 OrdinalIgnoreCase 比对，对不上直接抛
    ///    ArgumentException: Could not find matching atlas page）。
    /// 3. TextAsset 的 name 不能含 ".skel"，否则 spine-unity 会按二进制格式解析。
    ///    new TextAsset(string) 的 name 默认为空串，正好安全。
    ///
    /// 参数顺序陷阱
    /// ------------
    /// SkeletonDataAsset.CreateRuntimeInstance(..., initialize: false) 建完后先灌
    /// defaultMix / 过渡表，最后自己调 GetSkeletonData(true) —— 因为 GetSkeletonData 内部会
    /// InitializeWithData() 建 AnimationStateData 再 FillStateData()，而 stateData 一旦建好
    /// 就被缓存（见 spine-unity 反编译 GetSkeletonData / InitializeWithData / FillStateData）。
    /// 先 initialize 再改字段的话，那份 stateData 里装的还是默认值。
    /// </summary>
    public sealed class CirnoRCharacterModel : UnitModelTemplate
    {
        public static bool useInGameModel = BepinexPlugin.useInGameModel;
        public static bool useStandModel = BepinexPlugin.useStandModel;
        public static string model_name = useInGameModel
            ? BepinexPlugin.modelName
            : "CirnoRCharacterModel.png";

        // 必杀立绘沿用本 Mod 自己的图，不跟着官方模型一起换。
        public static string spellsprite_name = "CirnoRCharacterStand.png";

        /// <summary>
        /// 几何参数直接照抄官方 "Cirno" 那一行
        /// （Type=1 Spine / Flip=false / Offset=(0,-1.0) / Box=(0.8,1.8) / Shield=1.2 /
        ///  Hp=(0,-1.3) / Info=(0,1.2) / HasSpellPortrait=true）。
        /// 这样站位、碰撞盒、血条、护罩位置与官方琪露诺完全一致 —— 骨骼设计尺寸就是官方的
        /// 687.85 x 1150，连 root 骨偏移都不需要（别再用 DefaultConfig() 手搓）。
        /// </summary>
        public const string BaseModelName = "Cirno";

        // DirResources 下的子目录名；三件套放根目录就留空（代码先试 SpineDir 再回退根目录）。
        public const string SpineDir = "";
        // 三件套必须同名（不含扩展名）
        public const string SpineName = "cirno";

        /// <summary>
        /// 方案 C 自建 SkeletonDataAsset 的运行时名字。SpineModelReloadPatch 靠它认领
        /// 「这是本 Mod 自己建的资产」，从而只对自己做兜底，不去干扰场上其他单位。
        /// </summary>
        public const string RuntimeAssetName = "CirnoRCharacterSpineC";

        /// <summary>
        /// 站立骨骼自建 SkeletonDataAsset 的运行时名字。
        /// 【必须】与方案 C 一样能被 SpineModelReloadPatch 认领 —— 自建资产不会被官方
        /// UnitView 自动重建，若名字对不上，补丁 A（强制 Initialize）就不会跑，
        /// valid 恒为 false，于是 SkeletonAnimation.Update() 第一行 `if (!valid) return;`
        /// 直接返回：人物位置正确但一格动画都不播（已踩）。
        /// </summary>
        public const string StandRuntimeAssetName = "CirnoRCharacterSpineIce";

        /// <summary>凡本 Mod 运行时自建的 Spine 资产都算"自己的"，供兜底补丁统一认领。</summary>
        public static bool IsOwnRuntimeAsset(SkeletonDataAsset asset)
        {
            return asset != null
                && (asset.name == RuntimeAssetName || asset.name == StandRuntimeAssetName);
        }

        // 取不到官方资产时的兜底值（实测自官方 cirno_SkeletonData：scale=0.0017 / defaultMix=0.1）
        public const float FallbackScale = 0.0017f;
        public const float FallbackDefaultMix = 0.1f;

        // ------------------------------------------------------------
        //  站立骨骼（冰之勇者）：三件套同名主干
        //  DirResources/cirnoice.{json,atlas,png} —— 用户 PSD 分层重导的 14 骨 / 18 槽版本。
        //  图集首页名与 Texture2D.name 都必须是这个 stem（spine-unity 拿页名去掉 .png
        //  做 OrdinalIgnoreCase 比对，对不上直接抛 Could not find matching atlas page）。
        // ------------------------------------------------------------
        public const string StandStem = "cirnoice";

        // 站立骨骼设计尺寸：源立绘 中轴 x=554 / 脚底 y=0 / 设计高 1680（原点在脚底中心）。
        // 世界身高对齐官方 1.955（与 MakeConfig 里照抄的官方 "Cirno" Box/Hp/Info 一致）。
        // ★ 改设计高必须同步改这里，否则整幅画等比缩放错，表现为"人物过大/过小 + 站位偏"。
        public const float StandDesignHeight = 1680f;
        public const float StandScale = 1.955f / StandDesignHeight;

        public override IdContainer GetId()
        {
            return BepinexPlugin.modUniqueID;
        }

        public override LocalizationOption LoadLocalization()
        {
            return CirnoRCharacterLocalization.UnitModelBatchLoc.AddEntity(this);
        }

        public override ModelOption LoadModelOptions()
        {
            // 站立骨骼：拆件后的 CirnoRCharacterStand.png 自建三件套（8 件独立 slot）
            if (useStandModel)
            {
                return new ModelOption(LoadPathStandAsync());
            }
            // 方案 C：官方骨骼/动画 + 自建图集（换贴图）
            if (useInGameModel && BepinexPlugin.useCustomAtlas)
            {
                return new ModelOption(LoadPathCAsync());
            }
            if (useInGameModel)
            {
                // 纯官方资产（官方骨骼 + 官方动画 + 官方原皮）
                return new ModelOption(ResourcesHelper.LoadSpineUnitAsync(model_name));
            }
            // 静态立绘兜底
            return new ModelOption(
                ResourceLoader.LoadSpriteAsync(model_name, BepinexPlugin.directorySource, ppu: 565)
            );
        }

        // ------------------------------------------------------------
        //  方案 C 的加载入口
        //
        //  UniTask.Defer：真正的构建推迟到被 await 时才执行。
        //  因为 LoadModelOptions() 会被框架在「校验」和「实际加载」两处各调一次，
        //  直接同步构建会白建两遍（自建 + 解析 777 KB JSON 并不便宜）。
        // ------------------------------------------------------------
        private static SkeletonDataAsset _spineCache;

        private UniTask<SkeletonDataAsset> LoadPathCAsync()
        {
            if (_spineCache != null)
                return UniTask.FromResult(_spineCache);

            return UniTask.Defer(BuildPathCAsync);
        }

        private async UniTask<SkeletonDataAsset> BuildPathCAsync()
        {
            if (_spineCache != null)
                return _spineCache;

            _spineCache = await BuildCoreAsync();
            return _spineCache;
        }

        /// <summary>按候选前缀依次找资源，返回第一个存在的字节内容。</summary>
        private static byte[] ReadBinary(string fileName, DirectorySource source, out string hitPrefix)
        {
            foreach (string prefix in Candidates())
            {
                byte[] bytes = ResourceLoader.ResourceBinary(prefix + fileName, source);
                if (bytes != null && bytes.Length > 0)
                {
                    hitPrefix = prefix;
                    return bytes;
                }
            }
            hitPrefix = null;
            return null;
        }

        private static string[] Candidates()
        {
            return string.IsNullOrEmpty(SpineDir)
                ? new[] { "" }
                : new[] { SpineDir + "/", "" };
        }

        private static async UniTask<SkeletonDataAsset> BuildCoreAsync()
        {
            DirectorySource source = BepinexPlugin.directorySource;

            // ---- 0) 读官方资产：只读取字段，绝不修改 -------------------
            //     要的是三样：scale、defaultMix、8 条动画过渡表。
            SkeletonDataAsset official = null;
            try
            {
                official = await ResourcesHelper.LoadSpineUnitAsync(model_name);
            }
            catch (System.Exception ex)
            {
                BepinexPlugin.log.LogWarning(
                    $"[SpineC] 读取官方资产 {model_name} 失败：{ex.Message}；" +
                    $"改用兜底参数 scale={FallbackScale} defaultMix={FallbackDefaultMix}");
            }

            float scale = (official != null && official.scale > 0f) ? official.scale : FallbackScale;
            if (official != null)
            {
                BepinexPlugin.log.LogInfo(
                    $"[SpineC] 官方资产已取到：scale={official.scale:F6} " +
                    $"defaultMix={official.defaultMix:F3} 过渡表={official.fromAnimation?.Length ?? 0} 条");
            }

            // ---- 1) 骨骼 JSON（官方逐字节副本） -> TextAsset ------------
            byte[] jsonBytes = ReadBinary(SpineName + ".json", source, out string prefix);
            if (jsonBytes == null)
            {
                BepinexPlugin.log.LogError(
                    $"[SpineC] 找不到骨骼文件 {SpineName}.json" +
                    $"（已试 DirResources/{SpineDir} 与 DirResources/ 根目录）");
                return null;
            }
            TextAsset jsonAsset = new TextAsset(Encoding.UTF8.GetString(jsonBytes));

            // ---- 2) 图集描述 -> TextAsset -------------------------------
            byte[] atlasBytes = ReadBinary(SpineName + ".atlas", source, out _);
            if (atlasBytes == null)
            {
                BepinexPlugin.log.LogError($"[SpineC] 找不到图集文件 {prefix}{SpineName}.atlas");
                return null;
            }
            TextAsset atlasAsset = new TextAsset(Encoding.UTF8.GetString(atlasBytes));

            // ---- 3) 贴图 -> Texture2D -----------------------------------
            //      Texture2D.name 必须等于 atlas 页名去掉 .png 的部分。
            Texture2D texture = ResourceLoader.LoadTexture(prefix + SpineName + ".png", source);
            if (texture == null)
            {
                BepinexPlugin.log.LogError($"[SpineC] 找不到贴图文件 {prefix}{SpineName}.png");
                return null;
            }
            texture.name = SpineName;

            // ---- 4) 材质：克隆官方材质 ----------------------------------
            //      官方 cirno_Material 上挂着描边 shader 与一堆参数
            //      （_OutlineColor=(1,1,0,1) / _OutlineWidth=3 / _StraightAlphaInput=1 /
            //        _OutlineReferenceTexWidth=1024 / _Use8Neighbourhood=1 ...）。
            //      克隆比手搓安全得多 —— 手搓要逐个 HasProperty 试，漏一个描边就没了。
            Material propertySource = CloneOfficialMaterial(official);
            if (propertySource == null)
            {
                Shader shader = Shader.Find("Spine/Skeleton");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                propertySource = new Material(shader);
                if (propertySource.HasProperty("_OutlineColor"))
                    propertySource.SetColor("_OutlineColor", new Color(1f, 1f, 0f, 1f));
                if (propertySource.HasProperty("_OutlineWidth"))
                    propertySource.SetFloat("_OutlineWidth", 3f);
                if (propertySource.HasProperty("_StraightAlphaInput"))
                    propertySource.SetFloat("_StraightAlphaInput", 1f);
                BepinexPlugin.log.LogWarning("[SpineC] 未取到官方材质，已按黄色描边宽 3 手搓兜底材质");
            }

            // ---- 5) 自建图集资产 ----------------------------------------
            SpineAtlasAsset runtimeAtlas = SpineAtlasAsset.CreateRuntimeInstance(
                atlasAsset, new Texture2D[] { texture }, propertySource, true);
            if (runtimeAtlas == null || runtimeAtlas.GetAtlas() == null)
            {
                BepinexPlugin.log.LogError(
                    $"[SpineC] 图集构建失败！检查 {prefix}{SpineName}.atlas 第一行的页名是否等于 " +
                    $"\"{SpineName}.png\"、贴图尺寸是否等于 atlas 里声明的 size。");
                return null;
            }
            BepinexPlugin.log.LogInfo(
                $"[SpineC] 自建图集：{prefix}{SpineName}.atlas ({atlasBytes.Length} B) + " +
                $"{prefix}{SpineName}.png ({texture.width}x{texture.height})");

            // ---- 6) 自建骨骼资产：先灌参数，再解析 ----------------------
            SkeletonDataAsset skeletonData = SkeletonDataAsset.CreateRuntimeInstance(
                jsonAsset, new AtlasAssetBase[] { runtimeAtlas }, false, scale);

            // 打上自己的名字，供 SpineModelReloadPatch 认领（见 RuntimeAssetName 注释）
            skeletonData.name = RuntimeAssetName;

            if (official != null)
            {
                skeletonData.defaultMix = official.defaultMix;
                skeletonData.fromAnimation = official.fromAnimation;
                skeletonData.toAnimation = official.toAnimation;
                skeletonData.duration = official.duration;
            }
            else
            {
                skeletonData.defaultMix = FallbackDefaultMix;
            }

            // GetSkeletonData 内部：ReadSkeletonData -> InitializeWithData(stateData) -> FillStateData
            // 所以必须在这里才做第一次解析，过渡表才灌得进去。
            SkeletonData data = skeletonData.GetSkeletonData(true);
            if (data == null)
            {
                // GetSkeletonData(quiet:true) 把真实异常吞了，这里把能自查的信息全打出来。
                BepinexPlugin.log.LogError(
                    $"[SpineC] SkeletonData 构建失败！\n" +
                    $"        文件：{prefix}{SpineName}.json ({jsonBytes.Length} B) + " +
                    $"{prefix}{SpineName}.atlas ({atlasBytes.Length} B)\n" +
                    $"        按以下顺序自查：\n" +
                    $"        1) skins[].attachments 的顶层键必须是【槽名】而不是骨名；\n" +
                    $"        2) bones[].skin 不能为 true（会让整具骨架不可见）；\n" +
                    $"        3) 动画的 bones/slots 段不能引用不存在的骨名/槽名；\n" +
                    $"        4) skeleton.spine 版本必须与运行时一致（本项目 4.2.03）。");
                return null;
            }

            BepinexPlugin.log.LogInfo(
                $"[SpineC] 解析成功：{data.Bones.Count} 骨 / {data.Slots.Count} 槽 / " +
                $"{data.Skins.Count} 皮肤 / {data.Animations.Count} 动画");

            if (data.FindAnimation("idle") == null)
            {
                BepinexPlugin.log.LogError("[SpineC] 缺少必填动画 idle，游戏会抛 InvalidOperationException！");
            }

            // ---- 7) 模型整体上下偏移（BepInEx 配置项 SpineModel/SpineOffsetY）----
            //      方案 C 走官方几何，默认 0；要微调时改配置即可。
            //      直接改 root 骨 Y：所有骨都是 root 的子节点，等于整幅画整体平移。
            float offY = BepinexPlugin.spineOffsetY != null ? BepinexPlugin.spineOffsetY.Value : 0f;
            if (System.Math.Abs(offY) > 1e-6f)
            {
                BoneData rootBone = data.FindBone("root");
                if (rootBone != null)
                {
                    rootBone.Y += offY / scale;   // 配置是世界单位，骨坐标是设计像素
                    BepinexPlugin.log.LogInfo(
                        $"[SpineC] 模型整体偏移 Y={offY:F3}（root 骨 {offY / scale:+0.0;-0.0} 设计像素）");
                }
            }

            var names = new StringBuilder();
            foreach (Animation anim in data.Animations)
                names.Append(anim.Name).Append(' ');
            BepinexPlugin.log.LogInfo($"[SpineC] 已加载动画({data.Animations.Count})：{names}");

            return skeletonData;
        }

        // ------------------------------------------------------------
        //  站立骨骼（冰之勇者）：自建 Spine 三件套
        //  （cirnoice.json / cirnoice.atlas / cirnoice.png）
        //
        //  与方案 C 的关键区别：骨骼 JSON 不是官方那份，而是自建的 14 骨 / 18 槽骨架
        //  （root / waist / chest / head / 双臂 / 双腿 / 披风 ×2 / 冰翼 ×4 / 眼睛 ×2）。
        //  几何参数沿用官方 "Cirno" 行（Box/Shield/Hp/Info）。
        //  【原点在脚底】与官方 root 一致（官方 root 世界 y=0 即脚底，人物向上长到 1150）；
        //  放身体中心会让游戏按脚底基准摆位时整体下沉约半身高（实测跑到护罩下方）。
        //  原点已是脚底，无需再套 SpineOffsetY（官方同样不套）。
        //
        //  ===== 眼睛状态是【纯数据驱动】的，C# 侧不需要任何状态机 =====
        //  四组眼效贴片（eyeLg/Rg 绿、eyeLb/Rb 亮 additive）常驻骨架，默认 alpha=0，
        //  由各动画自带的槽色时间轴交叉淡化切换。动画名照抄官方 UnitView 会点播的名字：
        //    · 攻击  ShootStartAnimation(gun.Animation)  -> SetAnimation(0, order+"start")
        //            + AddAnimation(0, order+"loop", loop:true) / ShootEndAnimation -> order+"end"
        //            （order 取自 GunConfig.Animation，实测只有 shoot1~4 / spell / skill1）
        //            -> 本骨架在 shoot1~4 的 start/loop/end 与 spell 里点亮【亮眼】
        //    · 防御  CastShieldToSelf / CastShieldToOther -> DefendAnimation() -> "defend"
        //    · 格挡  Guard() -> GuardAnimation() -> "guard"
        //    · 擦弹  GrazeAnimation() -> "graze"
        //            -> 这三条里点亮【绿眼】
        //    · hit / crash 显式清零（被打断时不留半透明残影）
        //  漏写任何一个名字，UnitView 只会 Debug.LogWarning 一声就过去，表现为"动画不播"——
        //  所以 LINT 把这份名单设成了硬性必填（见 _spine_fmt_audit.py 的 REQUIRED）。
        //
        //  blink 走 track 1（UnitView.Blink() -> SetAnimation(1, "blink", loop:false)），
        //  与 track 0 的游戏动画并行；因此 blink 刻意【不写眼效槽】，否则眼皮刚睁开时
        //  亮眼会被 track 1 压掉闪一下。开局 SpineLoaded 里 AllAnimationsNames.Contains("blink")
        //  命中即自动 StartBlinking()（计时 Random.Range(2f, 5f)），无需任何额外干预。
        // ------------------------------------------------------------
        private static SkeletonDataAsset _standCache;

        private UniTask<SkeletonDataAsset> LoadPathStandAsync()
        {
            if (_standCache != null)
                return UniTask.FromResult(_standCache);
            return UniTask.Defer(BuildStandAsync);
        }

        private async UniTask<SkeletonDataAsset> BuildStandAsync()
        {
            if (_standCache != null)
                return _standCache;

            DirectorySource source = BepinexPlugin.directorySource;
            const string stem = StandStem;

            // ---- 1) 骨骼 JSON ----
            byte[] jsonBytes = ReadBinary(stem + ".json", source, out string prefix);
            if (jsonBytes == null)
            {
                BepinexPlugin.log.LogError($"[Stand] 找不到骨骼文件 {stem}.json（已试 DirResources/ 根目录）");
                return null;
            }
            TextAsset jsonAsset = new TextAsset(Encoding.UTF8.GetString(jsonBytes));

            // ---- 2) 图集 ----
            byte[] atlasBytes = ReadBinary(stem + ".atlas", source, out _);
            if (atlasBytes == null)
            {
                BepinexPlugin.log.LogError($"[Stand] 找不到图集文件 {prefix}{stem}.atlas");
                return null;
            }
            TextAsset atlasAsset = new TextAsset(Encoding.UTF8.GetString(atlasBytes));

            // ---- 3) 贴图（Texture2D.name 必须等于图集页名去掉 .png = "cirnoice"）----
            Texture2D texture = ResourceLoader.LoadTexture(prefix + stem + ".png", source);
            if (texture == null)
            {
                BepinexPlugin.log.LogError($"[Stand] 找不到贴图文件 {prefix}{stem}.png");
                return null;
            }
            texture.name = stem;

            // ---- 4) 材质：克隆官方琪露诺材质（含描边 shader + 全部参数），拿不到则手搓 Spine/Skeleton ----
            // 无论 useInGameModel 开关如何，站立骨骼都该用官方 Cirno 材质（几何同源、配色一致）。
            SkeletonDataAsset official = null;
            try { official = await ResourcesHelper.LoadSpineUnitAsync(BepinexPlugin.modelName); }
            catch (System.Exception ex)
            {
                BepinexPlugin.log.LogWarning($"[Stand] 读取官方资产失败：{ex.Message}；改用 Spine/Skeleton 材质");
            }
            Material propertySource = CloneOfficialMaterial(official);
            if (propertySource == null)
            {
                Shader shader = Shader.Find("Spine/Skeleton");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                propertySource = new Material(shader);
                BepinexPlugin.log.LogWarning("[Stand] 未取到官方材质，已按 Spine/Skeleton 手搓兜底材质");
            }
            // 注意：即便 useInGameModel=false 也用官方 Cirno 材质（描边 shader 与配色），
            // 因为站立骨骼用的是官方几何，材质必须同源才能和原版皮肤一致。

            // ---- 5) 自建图集资产 ----
            SpineAtlasAsset runtimeAtlas = SpineAtlasAsset.CreateRuntimeInstance(
                atlasAsset, new Texture2D[] { texture }, propertySource, true);
            if (runtimeAtlas == null || runtimeAtlas.GetAtlas() == null)
            {
                BepinexPlugin.log.LogError(
                    $"[Stand] 图集构建失败！检查 {prefix}{stem}.atlas 第一行页名是否等于 \"{stem}.png\"、" +
                    $"贴图尺寸是否等于 atlas 声明的 size。");
                return null;
            }
            BepinexPlugin.log.LogInfo(
                $"[Stand] 自建图集：{prefix}{stem}.atlas ({atlasBytes.Length} B) + {prefix}{stem}.png ({texture.width}x{texture.height})");

            // ---- 6) 自建骨骼资产 ----
            SkeletonDataAsset skeletonData = SkeletonDataAsset.CreateRuntimeInstance(
                jsonAsset, new AtlasAssetBase[] { runtimeAtlas }, false, StandScale);
            // 名字必须能被 SpineModelReloadPatch 认领（见 StandRuntimeAssetName 注释）
            skeletonData.name = StandRuntimeAssetName;
            skeletonData.defaultMix = FallbackDefaultMix;

            // ---- 6.5) 亮眼槽（eyeLb/eyeRb）的加法混合 ----
            // 必须在【我们自己第一次 GetSkeletonData】之前调用：SetupRuntimeBlendModeMaterials
            // 内部会 Clear() 掉已解析的数据再重建，所以 defaultMix 得先灌好。
            SetupAdditiveEyeGlow(skeletonData);

            SkeletonData data = skeletonData.GetSkeletonData(true);
            if (data == null)
            {
                BepinexPlugin.log.LogError(
                    $"[Stand] SkeletonData 构建失败！\n" +
                    $"        文件：{prefix}{stem}.json ({jsonBytes.Length} B) + {prefix}{stem}.atlas ({atlasBytes.Length} B)\n" +
                    $"        按以下顺序自查：\n" +
                    $"        1) skins[].attachments 的顶层键必须是【槽名】而不是骨名；\n" +
                    $"        2) bones[].skin 不能为 true（会让整具骨架不可见）；\n" +
                    $"        3) skeleton.spine 版本必须为 4.2.03；\n" +
                    $"        4) 动画的 bones/slots 段不能引用不存在的骨名/槽名。");
                return null;
            }
            BepinexPlugin.log.LogInfo(
                $"[Stand] 解析成功：{data.Bones.Count} 骨 / {data.Slots.Count} 槽 / " +
                $"{data.Skins.Count} 皮肤 / {data.Animations.Count} 动画");

            if (data.FindAnimation("idle") == null)
            {
                BepinexPlugin.log.LogError("[Stand] 缺少必填动画 idle，游戏会抛 InvalidOperationException！");
            }

            var names = new StringBuilder();
            foreach (Animation anim in data.Animations)
                names.Append(anim.Name).Append(' ');
            BepinexPlugin.log.LogInfo($"[Stand] 已加载动画({data.Animations.Count})：{names}");

            _standCache = skeletonData;
            return skeletonData;
        }

        // ------------------------------------------------------------
        //  亮眼槽的加法混合
        //
        //  ===== 为什么非得手动装 =====
        //  骨骼 JSON 里 eyeLb/eyeRb 声明了 "blend": "additive"，但那只是把 SlotData.BlendMode
        //  标成了 Additive，**不等于画面就会加法合成**。真正决定合成方式的是该槽所用材质上的
        //  shader —— spine-unity 的做法是给加法槽换一份"加法材质"，而这份材质的装配入口是：
        //      BlendModeMaterials.ApplyMaterials(skeletonData)
        //  它第一行就是 `if (!requiresBlendModeMaterials) return;`，而
        //  `SkeletonDataAsset.blendModeMaterials` 是 `new BlendModeMaterials()`，
        //  `applyAdditiveMaterial` 与 `requiresBlendModeMaterials` 双双默认 false。
        //  官方资产是编辑器里烘好的（值被序列化进去了），我们这条**运行时自建**通路
        //  必须自己调 SetupRuntimeBlendModeMaterials(true, {additiveTemplate})。
        //  不调的话：眼睛照样亮，只是走普通 alpha 混合 —— 浅青色高光而不是发白光晕。
        //
        //  ===== 为什么模板选 URP 的 Particles/Unlit =====
        //  已把全量 bundle 的 shader 名单扫过一遍：本作里 **没有** Spine 系的加法 shader
        //  （只有 `Spine/Skeleton` 与 `Spine/Blend Modes/Skeleton PMA Multiply`，
        //   spine-unity 的 PMA/StraightAlpha × Additive 那几个变体因为在编辑器里没有材质
        //   引用它们，构建时被剥离了），`Spine/Skeleton` 自身也不暴露 _SrcBlend/_DstBlend。
        //  `Universal Render Pipeline/Particles/Unlit` 是全作唯一一个把
        //  _SrcBlend / _DstBlend / _BlendOp / _ZWrite / _Cull 当**材质属性**暴露出来的 shader
        //  （已从 effects bundle 核对其属性表），改成 Blend SrcAlpha One 就是标准加法；
        //  而且它按顶点色调制，正好对上 Spine 写进网格的槽色 alpha 轨道。
        //
        //  ===== 安全设计 =====
        //  整段包在 try/catch 里，任何一步失败都只降级（打普通 alpha 混合）+ 打日志，
        //  绝不让角色因为"亮眼不够亮"而整个加载失败。另有 BepInEx 配置项
        //  SpineModel/UseAdditiveEyeGlow 可一键关掉。
        // ------------------------------------------------------------
        private static void SetupAdditiveEyeGlow(SkeletonDataAsset asset)
        {
            if (BepinexPlugin.useAdditiveEyeGlow != null && !BepinexPlugin.useAdditiveEyeGlow.Value)
            {
                BepinexPlugin.log.LogInfo(
                    "[Stand] 亮眼加法混合已按配置关闭 -> 走普通 alpha 混合（SpineModel/UseAdditiveEyeGlow=false）");
                return;
            }

            try
            {
                Material template = BuildAdditiveTemplate();
                if (template == null)
                {
                    BepinexPlugin.log.LogWarning(
                        "[Stand] 找不到可用的加法混合模板 shader -> 亮眼降级为普通 alpha 混合" +
                        "（仍是浅青色高光，只是不会发光）");
                    return;
                }

                var templates = new BlendModeMaterials.TemplateMaterials
                {
                    additiveTemplate = template,
                };

                // 内部流程：Clear() -> GetSkeletonData() -> 逐槽挑 IHasTextureRegion ->
                //           按页名克隆 AtlasRegion 并把 page.rendererObject 换成加法材质 -> Clear() 重建
                asset.SetupRuntimeBlendModeMaterials(true, templates);

                // 回读校验：确认加法槽真的被换过材质（比只看有没有抛异常可靠）
                int swapped = 0;
                SkeletonData sd = asset.GetSkeletonData(true);
                if (sd != null)
                {
                    var entries = new System.Collections.Generic.List<Skin.SkinEntry>();
                    for (int i = 0; i < sd.Slots.Count; i++)
                    {
                        SlotData slotData = sd.Slots.Items[i];
                        if (slotData.BlendMode != BlendMode.Additive)
                            continue;
                        entries.Clear();
                        foreach (Skin skin in sd.Skins)
                            skin.GetAttachments(i, entries);
                        foreach (Skin.SkinEntry entry in entries)
                        {
                            // 注意：AtlasRegion.page 是【字段】不是属性（spine-csharp 4.2）
                            if (entry.Attachment is IHasTextureRegion hasRegion
                                && hasRegion.Region is AtlasRegion region
                                && region.page != null
                                && region.page.rendererObject is Material mat
                                && mat.shader != null
                                && mat.shader.name != "Spine/Skeleton")
                            {
                                swapped++;
                            }
                        }
                    }
                }

                if (swapped > 0)
                {
                    BepinexPlugin.log.LogInfo(
                        $"[Stand] 亮眼已启用加法混合：{swapped} 个区域换成模板 shader " +
                        $"\"{template.shader.name}\"（Blend SrcAlpha One）");
                }
                else
                {
                    BepinexPlugin.log.LogWarning(
                        "[Stand] 加法混合装配后回读不到被替换的区域 -> 实际仍走普通 alpha 混合。");
                }
            }
            catch (System.Exception ex)
            {
                BepinexPlugin.log.LogWarning(
                    $"[Stand] 加法混合装配失败（{ex.GetType().Name}: {ex.Message}）" +
                    $"-> 亮眼降级为普通 alpha 混合");
            }
        }

        /// <summary>
        /// 造一份"加法混合"模板材质。spine-unity 只会拿它去 new Material(template) 再覆盖
        /// mainTexture，所以真正起作用的只有它身上的 shader 与渲染状态。
        /// </summary>
        private static Material BuildAdditiveTemplate()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
                return null;

            Material m = new Material(shader) { name = "CirnoRIceEyeAdditive" };

            // 贴图随后由 BlendModeMaterials.CreateReplacementMaterial 用 originalPage 的
            // mainTexture 覆盖（URP 的 _BaseMap 带 [MainTexture] 特性，mainTexture 写得进去）。
            if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", Texture2D.whiteTexture);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", Color.white);
            if (m.HasProperty("_Color")) m.SetColor("_Color", Color.white);

            // 关键：Blend SrcAlpha One == 帧缓冲 += 贴图 × alpha，与离线 GIF 的合成公式一致
            if (m.HasProperty("_SrcBlend")) m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            if (m.HasProperty("_DstBlend")) m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
            if (m.HasProperty("_BlendOp")) m.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Add);
            if (m.HasProperty("_ZWrite")) m.SetFloat("_ZWrite", 0f);
            if (m.HasProperty("_Cull")) m.SetFloat("_Cull", (float)UnityEngine.Rendering.CullMode.Off);
            if (m.HasProperty("_AlphaClip")) m.SetFloat("_AlphaClip", 0f);
            if (m.HasProperty("_Blend")) m.SetFloat("_Blend", 2f);          // URP: 2 = Additive
            if (m.HasProperty("_Surface")) m.SetFloat("_Surface", 1f);      // URP: 1 = Transparent

            m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m.DisableKeyword("_ALPHATEST_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            // 与 Spine 图集页材质同队列，保证子网格排序仍按"画序"走，加法层不会跑到眼睛底下
            m.renderQueue = 3000;
            return m;
        }

        /// <summary>
        /// 克隆官方材质。new Material(官方材质) 会连 shader 与全部属性一起复制，
        /// 描边颜色/宽度/参考宽度等一个不漏；绝不能直接改官方那份（全局共享实例）。
        /// </summary>
        private static Material CloneOfficialMaterial(SkeletonDataAsset official)
        {
            if (official == null || official.atlasAssets == null)
                return null;

            for (int i = 0; i < official.atlasAssets.Length; i++)
            {
                AtlasAssetBase atlasAsset = official.atlasAssets[i];
                if (atlasAsset == null || atlasAsset.Materials == null)
                    continue;

                foreach (Material m in atlasAsset.Materials)
                {
                    if (m == null || m.shader == null)
                        continue;
                    return new Material(m);
                }
            }
            return null;
        }

        public override UniTask<Sprite> LoadSpellSprite()
        {
            // 必杀立绘始终用本 Mod 自己的图：它和骨骼换不换皮是两件事，不要顺手改掉。
            return ResourceLoader.LoadSpriteAsync(
                spellsprite_name,
                BepinexPlugin.directorySource
            );
        }

        public override UnitModelConfig MakeConfig()
        {
            // 站立骨骼与方案 C 都是 Spine 模型，几何参数都照抄官方 "Cirno" 行
            // （站位 / 碰撞盒 / 血条 / 护罩全部与官方一致）。即使把 useInGameModel 关掉、
            // 只开 useStandModel，也不能掉进下面的静态立绘兜底（Type=0 会让 Spine 静默不加载）。
            if (useInGameModel || useStandModel)
            {
                // 几何参数照抄官方 "Cirno" 行：站位 / 碰撞盒 / 血条 / 护罩全部与官方一致。
                UnitModelConfig config = UnitModelConfig.FromName(BaseModelName).Copy();
                config.Flip = BepinexPlugin.modelIsFlipped;
                return config;
            }

            UnitModelConfig spriteConfig = DefaultConfig().Copy();
            spriteConfig.Flip = BepinexPlugin.modelIsFlipped;
            spriteConfig.Type = 0;
            spriteConfig.Offset = new Vector2(0, -0.10f);
            spriteConfig.HasSpellPortrait = true;
            return spriteConfig;
        }
    }
}
