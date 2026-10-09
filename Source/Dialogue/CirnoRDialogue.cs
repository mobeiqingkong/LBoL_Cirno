using System.Collections.Generic;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using UnityEngine;

namespace CirnoRCharacterMod.Dialogue
{
    /// <summary>
    /// 对话的驱动逻辑（台词本身在 Dialog{语言}.yaml 里，见 CirnoRDialogueData）。
    ///
    /// 挂载点（在 CirnoRCharacterPlayer.cs 的 PlayerUnit 里注册）：
    ///   Battle.BattleStarted -> OnBattleStarted
    ///       ① 琪露诺说开场白（可多句）
    ///       ② 与敌人**交替**对白：敌人 → 琪露诺 → 敌人 → …
    ///          只给少数有分量的角色配了这段（人人都有反而没意思），
    ///          而且**每场 run 只在第一次遇到该角色时播**（用 GameRun.ExtraFlags 记账）。
    ///   Battle.EnemyDied     -> OnEnemyDied
    ///       有名角色补一句"击败语"；若击败的是最终 BOSS 正邪，
    ///       则按是否持有【境界感知仪】播放 Good / Bad 结局台词
    ///
    /// 说话机制借用游戏自带的 PerformAction.Chat(source, content, chatTime, delay, waitTime, talk)：
    /// 第一个参数就是【发言单位】，所以敌人也能开口（官方的 LoveLetter / Bribery 就是这么干的：
    /// 玩家 delay=0 说一句，敌人 delay=1.5 回一句）。
    ///
    /// ===== 为什么多轮对白必须用 waitTime 而不是 delay =====
    /// ChatWidget.InternalChat 的第一行就是 this.DOKill(complete: true) —— 
    /// **同一个人再开口，前一句的气泡会被立刻完成并掐掉**。
    /// delay 只负责"气泡晚点浮出来"、不阻塞动作队列，所以若把整段对白都用 delay 排好、
    /// waitTime 全填 0，这些 Chat 会在同一帧全部下发 —— 同一个人后面的句子当场把前面的杀了，
    /// 结果只剩最后一句。因此逐句用 waitTime 让队列真的等下去。
    /// （BattleStarted 是 StartBattleAction 的最后一个 phase，在这里阻塞只是推迟玩家第一回合；
    ///   而 GameDirector 对已死单位会跳过播放**且不阻塞**，所以击败语那边同样安全。）
    ///
    /// 结局分流没有自己造轮子，而是直接读游戏自带的 TrueEndingProviders：
    ///   官方展品 JingjieGanzhiyi（境界感知仪）在 OnAdded 时把自己加进 TrueEndingProviders，
    ///   GameRunController.FinishStation() 里就是用它决定进 TrueEnd(好) 还是 NormalEnd(坏)。
    /// 所以这里的台词与游戏真正的结局分支是同一判据，不会出现"嘴上说回家了、实际上被 NormalEnd 了"。
    /// </summary>
    internal static class CirnoRDialogue
    {
        /// <summary>最终 BOSS。击败它时触发结局分流台词。</summary>
        private static readonly HashSet<string> FinalBosses =
            new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { "Seija" };

        private const float FirstDelay = 0.5f;    // 进场后多久开口
        private const float LineGap = 0.28f;      // 句与句之间的停顿
        private const float MinLine = 1.1f;       // 单句最短停留
        private const float MaxLine = 3.2f;       // 单句最长停留（再长就没人等了）
        private const float MaxTalkBase = 11.0f;  // 简繁（以及无台词表、回落中文的其它语言）
        private const float EndingTime = 4.2f;

        /// <summary>本场战斗是否已经说过见面语（BattleStarted 只会触发一次，留个保险）。</summary>
        private static BattleController _greeted;

        /// <summary>
        /// 对白总开关（BepInEx 配置 <c>Dialogue/EnableDialogue</c>，默认开）。
        ///
        /// ⚠️ 闸门必须开在这里，不能只靠"读不到台词表"：
        /// PickMeetLine 在取不到 Meet 键时会落到兜底池，而兜底池的应急值是「……」
        /// —— 那样反而会在屏幕上冒出一个空气泡。所以关掉时直接在入口 yield break。
        ///
        /// 配置项本身取异常一律当作"开"（配置读不出来时，宁可照常演也别静默失效）。
        /// </summary>
        internal static bool DialogueEnabled
        {
            get
            {
                try
                {
                    return BepinexPlugin.enableDialogue == null || BepinexPlugin.enableDialogue.Value;
                }
                catch
                {
                    return true;
                }
            }
        }

        // ------------------------------------------------------------------
        // 进场：开场白 + 与敌人的交替对白
        // ------------------------------------------------------------------
        public static IEnumerable<BattleAction> OnBattleStarted(BattleController battle)
        {
            if (battle == null || battle.Player == null) yield break;
            if (!DialogueEnabled) yield break;          // 配置关掉：一句话都不说
            if (_greeted == battle) yield break;
            _greeted = battle;

            EnemyUnit speaker;
            string[] opening = PickMeetLine(battle, out speaker);
            bool firstMeeting = MarkFirstMeeting(battle, speaker);

            // 先把整段对话排成一个 (谁, 说什么) 的列表 —— 排完才知道总时长，
            // 台词多的角色要靠它做等比压缩，免得开局被拖到十几秒。
            List<KeyValuePair<Unit, string>> script = new List<KeyValuePair<Unit, string>>();

            // ① 琪露诺的开场白
            if (opening != null)
                foreach (string line in opening)
                    if (!string.IsNullOrEmpty(line))
                        script.Add(new KeyValuePair<Unit, string>(battle.Player, line));

            // ② 交替对白只在【本场 run 第一次遇到这个角色】时播 ——
            //    完整对白是"初次见面"的内容，每场战斗都念一遍没人受得了；
            //    再遇到就只说开场白。
            //    记录方式借游戏自带的 GameRun.ExtraFlags（官方 Yarn 对话的分支判断
            //    也是读它：DialogFunctions.HasFlag -> ExtraFlags.Contains），随存档持久。
            if (firstMeeting && speaker != null && !string.IsNullOrEmpty(speaker.Id)
                && CirnoRDialogueData.Reply.TryGetValue(speaker.Id, out string[] reply)
                && reply != null)
            {
                for (int i = 0; i < reply.Length; i++)
                {
                    if (string.IsNullOrEmpty(reply[i])) continue;
                    script.Add(new KeyValuePair<Unit, string>(
                        i % 2 == 0 ? (Unit)speaker : battle.Player, reply[i]));
                }
            }

            if (script.Count == 0) yield break;

            float total = 0f;
            foreach (KeyValuePair<Unit, string> turn in script) total += SpeakTime(turn.Value);
            float maxTalk = MaxTalkFor(SafeLocale());
            float scale = total > maxTalk ? maxTalk / total : 1f;

            for (int i = 0; i < script.Count; i++)
            {
                string line = script[i].Value;
                float show = SpeakTime(line) * scale;
                float delay = i == 0 ? FirstDelay : 0f;
                // ★ waitTime 才是真正让动作队列等的参数：正文里同一帧下发会被 DOKill 掐掉
                yield return PerformAction.Chat(script[i].Key, line, show, delay, delay + show + LineGap, true);
            }
        }

        /// <summary>
        /// 记下"这场 run 里已经见过这个角色了"，返回是否**第一次**见。
        /// 兜底方向是"当作第一次"——万一 ExtraFlags 出问题，宁可多演一遍完整对白，也别把内容吞了。
        /// </summary>
        private static bool MarkFirstMeeting(BattleController battle, EnemyUnit speaker)
        {
            try
            {
                if (speaker == null || string.IsNullOrEmpty(speaker.Id)) return false;
                return battle.GameRun.ExtraFlags.Add("CirnoRMet." + speaker.Id);
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 整段对白的时长上限：**按语言取值**，不再是一把固定 11 秒的尺子。
        ///
        /// 为什么：这个上限的本意是"别把开局拖太久"，而同一句话在不同语言里的显示宽度本来就不同
        /// （SpeakTime 按全角 1 / 半角 0.55 折算）。实测同一批 70 段的自然长度 ——
        /// 简繁 9.7s · 日 11.5s · 韩 11.2s · 英 13.5s —— 用同一个 11 秒去卡，
        /// 英文会 65/70 段超标，最长的被压到 0.59 倍，读起来明显赶（韩日次之）。
        ///
        /// 取值 = 该语言自然长度的约 1.13 倍，与中文「11s / 9.7s」的松紧度一致，
        /// 这样各语言被压缩的比例相当。代价是英文玩家开局多等约 4 秒 —— 已与作者确认按此方案。
        /// 没有台词表的其它语言（Ru/Es/De…）内容会回落中文，所以按中文算。
        /// </summary>
        private static float MaxTalkFor(Locale locale)
        {
            switch (locale)
            {
                case Locale.En: return 15.0f;
                case Locale.Ja: return 13.0f;
                case Locale.Ko: return 13.0f;
                default: return MaxTalkBase;
            }
        }

        /// <summary>
        /// 取当前语言；任何异常都退回中文 —— 本方法在战斗协程里跑，**不许抛**
        /// （异常会让 ActionResolver 的协程直接终止，之后每次出牌连锁崩）。
        /// ⚠️ 必须全限定 LBoL.Core.Localization：简写会被 mod 自己的命名空间遮蔽。
        /// </summary>
        private static Locale SafeLocale()
        {
            try
            {
                return LBoL.Core.Localization.CurrentLocale;
            }
            catch
            {
                return Locale.ZhHans;
            }
        }

        /// <summary>
        /// 按字数估一个停留时长：全角字算 1、半角算 0.55，再线性换算并夹在 [MinLine, MaxLine]。
        /// 用固定时长会出现"短句干等、长句没读完就消失"，所以按长度给。
        /// </summary>
        private static float SpeakTime(string line)
        {
            float width = 0f;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                width += (c >= 0x1100 && c <= 0xFF60) ? 1f : 0.55f;
            }
            return Mathf.Clamp(0.75f + width * 0.085f, MinLine, MaxLine);
        }

        /// <summary>
        /// 挑见面语，并把命中的敌人通过 <paramref name="matched"/> 带出来（决定由谁接话）。
        /// 精确命中才有对白；杂兵兜底走 generic 池，不配对白（也不该配 —— 每场杂兵都聊两句会很烦）。
        /// </summary>
        private static string[] PickMeetLine(BattleController battle, out EnemyUnit matched)
        {
            matched = null;
            // 杂兵只从池里随机取【一句】—— 每场杂兵都聊两句会很烦
            string genericKeyMatch = null;

            foreach (EnemyUnit enemy in battle.AllAliveEnemies)
            {
                if (enemy == null) continue;
                string id = enemy.Id;
                if (string.IsNullOrEmpty(id)) continue;

                // 1) 精确命中（有名有姓的角色）
                if (CirnoRDialogueData.Meet.TryGetValue(id, out string[] exact))
                {
                    matched = enemy;
                    return exact;
                }

                // 2) 关键词命中（杂兵）—— 记录但继续找更精确的
                if (genericKeyMatch == null)
                {
                    foreach (KeyValuePair<string, string[]> kv in CirnoRDialogueData.GenericMeet)
                    {
                        if (id.IndexOf(kv.Key, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            genericKeyMatch = kv.Value[Random.Range(0, kv.Value.Length)];
                            break;
                        }
                    }
                }
            }

            if (genericKeyMatch != null) return new[] { genericKeyMatch };

            string[] fallback = CirnoRDialogueData.GenericMeetFallback;
            return new[] { fallback[Random.Range(0, fallback.Length)] };
        }

        // ------------------------------------------------------------------
        // 击杀：击败语 / 结局分流
        // ------------------------------------------------------------------
        public static IEnumerable<BattleAction> OnEnemyDied(BattleController battle, DieEventArgs args)
        {
            if (battle == null || battle.Player == null || args == null) yield break;
            if (!DialogueEnabled) yield break;          // 配置关掉：击败语与结局台词都不播

            EnemyUnit enemy = args.Unit as EnemyUnit;
            if (enemy == null) yield break;
            string id = enemy.Id;
            if (string.IsNullOrEmpty(id)) yield break;

            // ---- 最终 BOSS：结局分流 ----
            if (FinalBosses.Contains(id))
            {
                // 与游戏真正的结局判据同源：有 Provider 且无 Blocker 才算能回家
                bool canGoHome = battle.GameRun.CanEnterTrueEnding();

                string[] ending = canGoHome
                    ? CirnoRDialogueData.SeijaDefeatedWithGenerator
                    : CirnoRDialogueData.SeijaDefeatedWithoutGenerator;

                foreach (BattleAction action in Say(battle, ending, 0.8f, EndingTime))
                    yield return action;
                yield break;
            }

            // ---- 有名角色：补一句击败语（杂兵不刷，避免每场都念）----
            if (CirnoRDialogueData.Defeat.TryGetValue(id, out string[] lines))
                foreach (BattleAction action in Say(battle, lines, 0.5f, 0f))
                    yield return action;
        }

        /// <summary>
        /// 由琪露诺逐句说完一串台词。多句时后面每句都让队列等一等 ——
        /// 同一个人连说两句若不阻塞，后一句会当场把前一句的气泡掐掉（见类头说明）。
        /// 返回可枚举（而不是单个动作）是为了能安全地"一句话都没有"。
        /// </summary>
        private static IEnumerable<BattleAction> Say(BattleController battle, string[] lines,
                                                     float delay, float fixedTime)
        {
            if (lines == null) yield break;
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                float show = fixedTime > 0f ? fixedTime : SpeakTime(lines[i]);
                float lead = i == 0 ? delay : 0f;
                yield return PerformAction.Chat(battle.Player, lines[i], show, lead, lead + show + LineGap, true);
            }
        }
    }
}
