using BepInEx.Logging;
using CirnoRCharacterMod.model;
using HarmonyLib;
using LBoL.Presentation.Units;
using Spine;
using Spine.Unity;

namespace CirnoRCharacterMod.Patches
{
    /// <summary>
    /// 自建 Spine 骨骼（方案 C）的加载 / 世界变换兜底补丁。
    ///
    /// ===== 为什么还需要它 =====
    /// Model.BuildCoreAsync() 在运行时自建了一个 SkeletonDataAsset，而官方 UnitView 只在
    /// _spineNeedReload 为 true 时才调 SkeletonAnimation.Initialize(overwrite:true)，
    /// 首次加载不满足该条件 —— asset 被赋上去了，skeleton 却从未重建
    /// （valid=false / 0 动画）。这里在 SpineInitialize 前后各补一手。
    ///
    /// ===== 历史：skin:true 那次的根因链（留档）=====
    /// 自制 cirno.json 曾把骨骼误写成 "skin": true：
    ///   SpineJson.ReadBone: boneData.skinRequired = GetBoolean(item, "skin", false)
    ///   Skeleton.UpdateCache(): obj.sorted = obj.data.skinRequired; obj.active = !obj.sorted;
    ///     ⇒ 骨骼全部 active=false ⇒ updateCache.Count == 0
    ///   Skeleton.UpdateWorldTransform(): for (j = 0; j < updateCache.Count; j++) ...
    ///     ⇒ 循环一次都不跑 ⇒ 骨骼 a/b/c/d 与 worldX/worldY 恒为 0
    ///   RegionAttachment.ComputeWorldVertices: v = offset * bone矩阵 + bone.world
    ///     ⇒ 4 个顶点全塌成 (0,0) ⇒ 零面积网格 ⇒ 人物不可见，且日志零报错。
    /// 已核实：官方 cirno.json 里带 "skin": true 的骨骼是 **0 根**，方案 C 不会再踩。
    /// 保留这段是因为下面 B) 的告警判据（updateCache == 0）就是照它写的。
    ///
    /// ===== 探针骨名 =====
    /// 自制骨骼用 "waist"，官方骨架里**没有**这根骨（官方是 root / Body_* / Head_*）。
    /// 所以探针改成 "root"，否则日志恒为 "(无 waist 骨，非本 Mod 骨骼)"，诊断能力等于报废。
    ///
    /// ===== 只认领自己建的资产 =====
    /// 三处补丁都以 skeletonDataAsset.name == RuntimeAssetName 为前提，不去碰场上其他
    /// 单位（敌人、别的 mod 的角色）的 Spine，也不给它们加每帧开销。
    /// </summary>
    [HarmonyPatch]
    internal static class SpineModelReloadPatch
    {
        private static ManualLogSource Log => BepinexPlugin.log;

        /// <summary>已诊断过的 SkeletonAnimation 实例，避免重复刷日志。</summary>
        private static int _lastInstanceId;

        // 动画推进诊断（每实例只打几条，用来判断 state 到底有没有在跑）
        private static int _diagInstanceId = int.MinValue;
        private static int _diagTicks;
        private static int _diagLogged;

        /// <summary>判定这个 SkeletonAnimation 是否在用本 Mod 运行时自建的资产。</summary>
        private static bool IsOurAsset(SkeletonAnimation anim)
        {
            return anim != null && CirnoRCharacterModel.IsOwnRuntimeAsset(anim.skeletonDataAsset);
        }

        // ---------------------------------------------------------------
        // A) SpineInitialize 之前：确保 skeleton 真正被重建
        // ---------------------------------------------------------------
        [HarmonyPatch(typeof(UnitView), "SpineInitialize")]
        [HarmonyPrefix]
        private static void SpineInitialize_Prefix(ref SkeletonAnimation ___spineSkeleton)
        {
            try
            {
                if (!IsOurAsset(___spineSkeleton)) return;

                bool validBefore = ___spineSkeleton.valid;
                int animCount = -1;
                try
                {
                    SkeletonData sd = ___spineSkeleton.skeletonDataAsset.GetSkeletonData(true);
                    if (sd != null) animCount = sd.Animations.Count;
                }
                catch { }

                // 官方只在 _spineNeedReload 为 true 时才 Initialize，首次加载不满足该条件。
                if (!validBefore || animCount <= 0)
                {
                    ___spineSkeleton.Initialize(overwrite: true, quiet: true);
                    Log?.LogInfo($"[SpineC] 补重建: valid {validBefore} -> {___spineSkeleton.valid}, 动画 {animCount} -> " +
                                 $"{___spineSkeleton.skeleton?.Data?.Animations?.Count ?? -1}");
                }
            }
            catch (System.Exception ex)
            {
                Log?.LogError($"[SpineC] prefix 失败：{ex}");
            }
        }

        // ---------------------------------------------------------------
        // B) SpineInitialize 之后：校验世界变换，异常才补算
        // ---------------------------------------------------------------
        [HarmonyPatch(typeof(UnitView), "SpineInitialize")]
        [HarmonyPostfix]
        private static void SpineInitialize_Postfix(SkeletonAnimation ___spineSkeleton)
        {
            try
            {
                if (!IsOurAsset(___spineSkeleton)) return;

                Skeleton sk = ___spineSkeleton.skeleton;
                if (sk == null) return;

                // updateCache 为空 ⇒ 渲染必然塌成一点，直接补算（并告警）。
                if (sk.UpdateCacheList.Count == 0)
                {
                    Log?.LogWarning($"[SpineC] updateCache.Count=0（骨骼数 {sk.Bones.Count}）—— " +
                                    "检查 json 里是否误写了骨骼 \"skin\": true（会导致 active=false）。");
                    sk.UpdateWorldTransform(Skeleton.Physics.Update);
                }

                // 只在首次遇到该实例时打印一次摘要，避免每帧刷屏。
                int id = ___spineSkeleton.GetInstanceID();
                if (id == _lastInstanceId) return;
                _lastInstanceId = id;

                Bone probe = null;
                for (int i = 0; i < sk.Bones.Count; i++)
                {
                    if (sk.Bones.Items[i].Data.Name == "root") { probe = sk.Bones.Items[i]; break; }
                }

                Log?.LogInfo($"[SpineC] bones={sk.Bones.Count} slots={sk.Slots.Count} " +
                             $"updateCache={sk.UpdateCacheList.Count} valid={___spineSkeleton.valid}" +
                             (probe != null
                                 ? $" root.world=({probe.WorldX:F4},{probe.WorldY:F4}) active={probe.Active}"
                                 : " (无 root 骨，非本 Mod 骨骼)"));
            }
            catch (System.Exception ex)
            {
                Log?.LogError($"[SpineC] postfix 失败：{ex}");
            }
        }

        // ---------------------------------------------------------------
        // B') 运行期兜底：SkeletonAnimation.Update 之后若世界变换仍为 0 则补算
        //     （UnitView 本身没有 Update()，故 wrap 到 SkeletonAnimation 上）
        //     同样只认领自己建的资产，不给场上其他单位加每帧开销。
        // ---------------------------------------------------------------
        [HarmonyPatch(typeof(SkeletonAnimation), "Update", new System.Type[] { typeof(float) })]
        [HarmonyPostfix]
        private static void SkeletonAnimation_Update_Postfix(SkeletonAnimation __instance)
        {
            try
            {
                if (__instance == null || !__instance.valid) return;
                if (!IsOurAsset(__instance)) return;

                Skeleton sk = __instance.skeleton;
                if (sk == null || sk.UpdateCacheList.Count == 0) return;

                // 用第一根 active 骨做探针；仍为恒等 0 说明这条链路没算世界变换。
                Bone b0 = sk.Bones.Items[0];
                if (b0.Active && b0.A == 0f && b0.D == 0f)
                {
                    sk.UpdateWorldTransform(Skeleton.Physics.Update);
                }

                // ---- 一次性诊断：确认动画状态真的在推进（valid / 轨道 / 时间）----
                int instId = __instance.GetInstanceID();
                if (instId != _diagInstanceId)
                {
                    _diagInstanceId = instId;
                    _diagTicks = 0;
                    _diagLogged = 0;
                }
                if (_diagLogged < 6 && ++_diagTicks % 30 == 0)
                {
                    _diagLogged++;
                    TrackEntry te = null;
                    try { te = __instance.state?.GetCurrent(0); } catch { }
                    Log?.LogInfo(
                        $"[SpineC] 动画诊断#{_diagLogged} valid={__instance.valid} " +
                        $"track0={te?.Animation?.Name ?? "(null)"} t={te?.TrackTime ?? -1f:F3} " +
                        $"root.world=({b0.WorldX:F2},{b0.WorldY:F2})");
                }
            }
            catch { }
        }
    }
}
