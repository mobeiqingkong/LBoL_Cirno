using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LBoL.Core;
using LBoLEntitySideloader.Resource;
using YamlDotNet.RepresentationModel;

namespace CirnoRCharacterMod.Dialogue
{
    /// <summary>
    /// 冰之勇者琪露诺 —— 台词表的【读取器】。
    ///
    /// ===== 内容不在这里，在 YAML 里 =====
    /// 台词存在插件目录下的 <c>Dialog{语言}.yaml</c>（与 CardsEn.yaml 同级）：
    ///   · DialogZhHans.yaml（基准语言）· DialogZhHant.yaml · DialogJa.yaml
    ///   · DialogKo.yaml · DialogEn.yaml
    /// 直接编辑那些文件即可，**不需要重新编译**；改完重启游戏生效
    /// （连存档都不用换 —— 本读取器还会比对文件修改时间，改了会重新读）。
    ///
    /// YAML 结构（详见文件内的注释头）：
    ///   Meet / Reply / Defeat / GenericMeet / GenericMeetFallback / GenericDefeatFallback / Ending
    ///
    /// ===== 多轮对白 =====
    /// Meet / Reply / Defeat 的每个值**既可以是一句话，也可以是一串话**：
    ///   Meet   —— 琪露诺的开场白（全部由琪露诺说）
    ///   Reply  —— 与敌人的交替对话，**第 1 句由敌人说**，之后 琪露诺 → 敌人 → 琪露诺 …
    ///             所以偶数下标是敌人、奇数下标是琪露诺；奇偶写反 = 串台词
    ///   Defeat —— 击败该角色后琪露诺说的话
    /// 写成单行字符串时等价于只有 1 句（旧格式仍然兼容，所以其余四语言不翻译也不会坏）。
    ///
    /// 缺键的回落链：当前语言 → DialogZhHans.yaml → 内置应急值（保证永不抛异常）。
    /// 所以可以只翻译一部分。
    ///
    /// ===== 人物出处（写台词前先读这段，别再把她写成喊口号的口号机）=====
    /// 来自同人游戏《东方冰之勇者记 ~ Touhou Hero of Ice Fairy》（GAMEPULSE 游戏脉冲）。
    /// 下面几条**全部是游戏本体内置剧本的原文事实** —— 已从游戏 StreamingAssets/Dialogue.xlsx
    /// 提出 2159 句台词逐条核对（立绘对话 1290 + 气泡对话 869），不是百科转述：
    ///   · 琪露诺 = 雾之湖的「冰之勇者」。她的目标是**救出被掳走的「公主」（雾之湖公主）**，
    ///     再带她回雾之湖。原作措辞只有「公主」「雾之湖公主」，从不点名字。
    ///   · 反派是**「梦魇巫女」= 博丽灵梦**。原作原话：「这该不会指的是『博丽灵梦』
    ///     那个梦魇巫女吧？！」公主被关在**博丽大天守顶端的「博丽大祭坛」**；
    ///     博丽大天守是「足以摧毁幻想乡的终极要塞」。
    ///   · 她的核心台词（原文）：「咱已经数不过来了」「从始至终咱就只有一个想法：
    ///     找到咱的公主，陪她一起回到雾之湖」。
    ///   · 沿途关卡：克劳恩皮丝（「巫女大人最信赖的使者」）→ 魔理沙 → 妖梦 → 萃香
    ///     → 河童荷取 → 射命丸文（大天狗，把琪露诺困在幻术庭院「镜花水月庭」里来回绕圈）。
    ///   · 中段的**兔子列车**：因幡帝（列车长）、铃仙（乘务长）、弟弟、报童/杂鱼兔子；
    ///     还有「博丽神之酿」（其实是鬼之酒）、巡逻无人机、兔子通讯器。
    ///   · 别人叫她**「勇者大人」**；皮丝等人骂她**「笨蛋勇者」「最笨勇者」**。
    ///   · 关键场面：射命丸文拿「交还公主 + 册封雾之湖骑士」换她宣誓效忠、清剿同伴，
    ///     她大笑拒绝 ——「两三句话就想让咱背叛最重要的伙伴？开什么玩笑！」
    ///
    /// ===== 因此台词的写法（用户明确要求：别一直把「勇者」挂嘴边）=====
    ///   · **第一人称一律「咱」**（原作从不用「我」；「我们」写成「咱们」）。
    ///   · **「勇者」只准出现在三种原作句式里**，别处一律删：
    ///       ① 自称题名：「咱是最强的『冰之勇者琪露诺』！」
    ///       ② 被人叫「笨蛋勇者」时的反驳：「咱才不是什么笨蛋勇者啊！」
    ///       ③ 胜利口号：「冰之勇者，大获全胜！」（原作确有，已放进杂兵击败兜底池）
    ///   · **救人对象一律叫「公主」**（可写「咱的公主」「雾之湖公主」），不点名字。
    ///   · 保留笨蛋感：爱逞强、口是心非、数不到十、绕回原点、怕鬼、容易被骗。
    ///   · 语气词：啦 / 啊 / 呀 / 噢 / 嘛 / 哦 / 嘿 / 唔 / 呃 / 咦 / 哼。
    ///   · 日语用「あたい」、韩语用「나」、英语用 I，与官方译法一致。
    ///
    /// ===== 本 Mod 的设定 =====
    /// 她误打误撞、连原因都没弄明白，就从自己的世界跌进了这个世界（幻想乡的另一条时间线）。
    /// 她以为又是博丽那边搞的幻术，想回去得先找到【境界感知仪】。
    ///
    /// ===== 结局分流 =====
    /// 击败最终 BOSS 正邪时，若持有官方展品【境界感知仪】(JingjieGanzhiyi) -> 回得了家（Good）；
    /// 否则 -> 留在这个世界（Bad）。分流判据直接读游戏自带的 TrueEndingProviders，
    /// 与官方"真结局"机制完全同源，不会出现"嘴上说回家了、实际被判 NormalEnd"。
    /// </summary>
    internal static class CirnoRDialogueData
    {
        /// <summary>玩家自己的 Id（= BepinexPlugin.modUniqueID）。</summary>
        public const string PlayerId = "CirnoRCharacterMod";

        // ==================================================================
        // 配置
        // ==================================================================

        private const string FilePrefix = "Dialog";
        private const string Ext = ".yaml";

        /// <summary>基准语言：当前语言缺哪个键就回落到它。</summary>
        private const Locale BaseLocale = Locale.ZhHans;

        /// <summary>应急兜底：YAML 全部读不到时也不让对话系统抛异常。</summary>
        private static readonly string[] Emergency = { "……" };

        // ==================================================================
        // 状态
        // ==================================================================

        private static Locale _locale = (Locale)(-1);
        private static long _stamp = long.MinValue;

        private static Dictionary<string, string[]> _meet = NewMap();
        private static Dictionary<string, string[]> _reply = NewMap();
        private static Dictionary<string, string[]> _defeat = NewMap();
        private static KeyValuePair<string, string[]>[] _generic = new KeyValuePair<string, string[]>[0];
        private static string[] _genericFallback = Emergency;
        private static string[] _genericDefeatFallback = Emergency;
        private static string[] _seijaWin = Emergency;
        private static string[] _seijaLose = Emergency;

        private static Dictionary<string, string[]> NewMap() =>
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        // ==================================================================
        // 兼容层：sideloader 的 TryGetFileName 签名在版本之间变过
        //   · 0.9.7850（BepInEx/plugins 里那份）：(string id, out string name)
        //   · 1.1.0（创意工坊 3572815053 那份）：(string id, out string name,
        //                                        bool searchSubdirectories = true)
        // 直接写调用会**编译期绑定**到其中一个签名 —— 另一个版本上就抛
        // MissingMethodException（而且 Mono 在 JIT 阶段抛，被调方法自己的
        // try/catch 兜不住，异常会从调用方帧里逃出去打断整个战斗结算）。
        // 所以这里按【运行时实际存在的方法】反射调用：两个版本都能跑，
        // 以后签名再变也不用改代码。
        // ==================================================================

        private static readonly MethodInfo FileNameResolver = ResolveFileNameResolver();

        private static MethodInfo ResolveFileNameResolver()
        {
            try
            {
                foreach (MethodInfo m in typeof(Source).GetMethods(
                             BindingFlags.Public | BindingFlags.Instance))
                {
                    if (m.Name != "TryGetFileName") continue;
                    ParameterInfo[] ps = m.GetParameters();
                    // 至少 (string, out string)；第 3 个 searchSubdirectories 是可选参数
                    if (ps.Length >= 2
                        && ps[0].ParameterType == typeof(string)
                        && ps[1].IsOut)
                    {
                        return m;
                    }
                }
            }
            catch
            {
                // 反射都失败就当作解析不到 —— 下面会退化成"文件不存在"，不抛
            }
            return null;
        }

        /// <summary>
        /// 在插件目录里找一个资源文件，成功时给出它的完整路径。
        /// **任何情况下都不抛异常**：最坏返回 false（= 当作文件不存在，走兜底文案）。
        /// </summary>
        private static bool TryResolveFile(string name, out string fullPath)
        {
            fullPath = null;
            MethodInfo resolver = FileNameResolver;
            if (resolver == null) return false;

            try
            {
                ParameterInfo[] ps = resolver.GetParameters();
                object[] args =
                    ps.Length == 2
                        ? new object[] { name, null }
                        : new object[] { name, null, true };   // 新版：显式传 searchSubdirectories

                if (!(resolver.Invoke(BepinexPlugin.directorySource, args) is bool found) || !found)
                    return false;

                // resolver 回填的第 2 个参数是**实际文件名**（可能带子目录）；
                // FullPath 两版签名一致，直接用
                string resolved = args[1] as string;
                fullPath = BepinexPlugin.directorySource.FullPath(resolved ?? name);
                return true;
            }
            catch
            {
                fullPath = null;
                return false;
            }
        }

        // ==================================================================
        // 对外入口（签名与旧版完全一致，CirnoRDialogue.cs 无需改动）
        // ==================================================================

        /// <summary>
        /// 见面语：进入战斗、场上有该角色时由琪露诺说。key = 敌人的 Id（= 类名）。
        /// 值是**一串台词**（1 句或多句，全部由琪露诺说）。
        /// </summary>
        public static Dictionary<string, string[]> Meet { get { Refresh(); return _meet; } }

        /// <summary>
        /// 击败语：击败该角色后琪露诺说。值是**一串台词**。
        /// </summary>
        public static Dictionary<string, string[]> Defeat { get { Refresh(); return _defeat; } }

        /// <summary>
        /// 与敌人的交替对话：紧跟见面语，**第 1 句由敌人说**，之后 琪露诺 → 敌人 → 琪露诺 …
        /// 只给少数有分量的角色配了对话（不是人人都有）—— 都开口反而没意思。
        /// 没有配的角色会安静地站着，这也是设计的一部分。
        /// </summary>
        public static Dictionary<string, string[]> Reply { get { Refresh(); return _reply; } }

        /// <summary>杂兵兜底（关键词 -> 候选台词池）。顺序即优先级：先匹配到的先用。</summary>
        public static KeyValuePair<string, string[]>[] GenericMeet { get { Refresh(); return _generic; } }

        /// <summary>兜底见面语：没有任何匹配时用。</summary>
        public static string[] GenericMeetFallback { get { Refresh(); return _genericFallback; } }

        /// <summary>兜底击败语（当前未接线，预留给杂兵击杀播报）。</summary>
        public static string[] GenericDefeatFallback { get { Refresh(); return _genericDefeatFallback; } }

        /// <summary>击败最终 BOSS 且拿到境界感知仪（回得了家）。</summary>
        public static string[] SeijaDefeatedWithGenerator { get { Refresh(); return _seijaWin; } }

        /// <summary>击败最终 BOSS 但没拿到境界感知仪（留在这个世界）。</summary>
        public static string[] SeijaDefeatedWithoutGenerator { get { Refresh(); return _seijaLose; } }

        // ==================================================================
        // 读取 / 缓存
        // ==================================================================

        /// <summary>
        /// 需要时重新读取：语言变了、或文件被改过（比对修改时间）。
        /// 这样调台词不必重启游戏 —— 改完 YAML 下一场战斗就生效。
        /// </summary>
        private static void Refresh()
        {
            // ★ 整个方法体都在 try 里，这不是多余的谨慎。
            // 本方法由战斗反应调用（OnBattleStarted / OnEnemyDied），而反应里抛出
            // 的异常会让 ActionResolver 的协程被**直接终止** —— 战斗结算半路死掉、
            // 状态不收敛，玩家下一次出牌就会连锁崩到官方 UI 上（表现为卡死）。
            // 所以这里**任何异常都不许逃出去**，最坏也只是台词退化成兜底内容。
            try
            {
                // 配置里关掉对白 ⇒ 彻底不碰文件（连"有没有 Dialog*.yaml"都不问）。
                // 说话的闸门在 CirnoRDialogue 的入口，这里只是省掉无用的 IO。
                if (!CirnoRDialogue.DialogueEnabled)
                {
                    if (!_loggedOff)
                    {
                        _loggedOff = true;
                        BepinexPlugin.log.LogInfo(
                            "[Dialog] 配置 Dialogue/EnableDialogue=false —— 已忽略 Dialog*.yaml，不播任何对白。");
                    }
                    return;
                }

                Locale cur = LBoL.Core.Localization.CurrentLocale;
                // 当前语言与基准语言任一被改动都要重读；取两者的较新时间
                long stamp = Math.Max(StampOf(FilePrefix + cur + Ext),
                                      StampOf(FilePrefix + BaseLocale + Ext));

                if (_loaded && cur == _locale && stamp == _stamp) return;
                _locale = cur;
                _stamp = stamp;

                LoadAll(cur);
            }
            catch (Exception ex)
            {
                try
                {
                    BepinexPlugin.log.LogError("[Dialog] 台词表读取失败，改用兜底内容：" + ex);
                }
                catch
                {
                    // 连日志本身都不许抛
                }
            }
            _loaded = true;
        }

        private static bool _loaded;
        private static bool _loggedOff;

        private static long StampOf(string name)
        {
            try
            {
                string full;
                if (!TryResolveFile(name, out full)) return long.MinValue;
                return File.Exists(full) ? File.GetLastWriteTimeUtc(full).Ticks : long.MinValue;
            }
            catch
            {
                return long.MinValue;
            }
        }

        private static void LoadAll(Locale cur)
        {
            YamlMappingNode curRoot = ReadYaml(cur);
            YamlMappingNode baseRoot = cur == BaseLocale ? null : ReadYaml(BaseLocale);

            Dictionary<string, string[]> meet = NewMap();
            Dictionary<string, string[]> reply = NewMap();
            Dictionary<string, string[]> defeat = NewMap();
            List<KeyValuePair<string, string[]>> generic = new List<KeyValuePair<string, string[]>>();

            // 先铺基准语言，再用当前语言覆盖 —— 支持"只翻译一部分"
            FillLines(meet, baseRoot, "Meet");
            FillLines(meet, curRoot, "Meet");
            FillLines(reply, baseRoot, "Reply");
            FillLines(reply, curRoot, "Reply");
            FillLines(defeat, baseRoot, "Defeat");
            FillLines(defeat, curRoot, "Defeat");
            FillGeneric(generic, baseRoot, "GenericMeet");
            FillGeneric(generic, curRoot, "GenericMeet");

            _meet = meet;
            _reply = reply;
            _defeat = defeat;
            _generic = generic.ToArray();

            _genericFallback = Or(Seq(baseRoot, "GenericMeetFallback"),
                                  Seq(curRoot, "GenericMeetFallback"), Emergency);
            _genericDefeatFallback = Or(Seq(baseRoot, "GenericDefeatFallback"),
                                        Seq(curRoot, "GenericDefeatFallback"), Emergency);

            YamlMappingNode endingBase = Map(baseRoot, "Ending");
            YamlMappingNode endingCur = Map(curRoot, "Ending");
            _seijaWin = Or(Lines(Child(endingCur, "WithGenerator")),
                           Lines(Child(endingBase, "WithGenerator")), Emergency);
            _seijaLose = Or(Lines(Child(endingCur, "WithoutGenerator")),
                            Lines(Child(endingBase, "WithoutGenerator")), Emergency);

            BepinexPlugin.log.LogInfo(
                $"[Dialog] 台词表已载入（{cur}）：见面语 {_meet.Count}条/{CountLines(_meet)}句 · " +
                $"对白 {_reply.Count}条/{CountLines(_reply)}句 · 击败语 {_defeat.Count}条/{CountLines(_defeat)}句 · " +
                $"杂兵类 {_generic.Length} · 兜底 {_genericFallback.Length}");
        }

        /// <summary>统计一个映射里总共多少句（多轮对白用得上：条数 ≠ 句数）。</summary>
        private static int CountLines(Dictionary<string, string[]> map)
        {
            int n = 0;
            foreach (string[] v in map.Values) if (v != null) n += v.Length;
            return n;
        }

        /// <summary>读取指定语言的 Dialog*.yaml；不存在或解析失败返回 null。</summary>
        private static YamlMappingNode ReadYaml(Locale locale)
        {
            string name = FilePrefix + locale + Ext;
            // 必须先判存在：DirectorySource.Load 失败会返回 null，
            // 而 ResourceLoader.LoadYaml 内部直接 new StreamReader(stream) → 会 NRE
            if (!TryResolveFile(name, out _)) return null;
            return ResourceLoader.LoadYaml(name, BepinexPlugin.directorySource);
        }

        private static void FillLines(Dictionary<string, string[]> dst, YamlMappingNode root, string section)
        {
            YamlMappingNode map = Map(root, section);
            if (map == null) return;
            foreach (KeyValuePair<YamlNode, YamlNode> kv in map.Children)
            {
                string k = Scalar(kv.Key);
                if (string.IsNullOrEmpty(k)) continue;
                string[] lines = Lines(kv.Value);
                if (lines == null || lines.Length == 0) continue;
                dst[k] = lines;
            }
        }

        /// <summary>
        /// 把 YAML 节点读成台词序列：**单行字符串**（1 句）与**字符串列表**（多句）都接受。
        /// 兼容旧格式是有意的 —— 其余四语言未翻译时仍是单行写法，照样能跑。
        /// </summary>
        private static string[] Lines(YamlNode node)
        {
            if (node is YamlSequenceNode seq) return SeqStrings(seq);
            string s = Scalar(node);
            return string.IsNullOrEmpty(s) ? null : new[] { s };
        }

        private static void FillGeneric(List<KeyValuePair<string, string[]>> dst,
                                        YamlMappingNode root, string section)
        {
            YamlMappingNode map = Map(root, section);
            if (map == null) return;
            foreach (KeyValuePair<YamlNode, YamlNode> kv in map.Children)
            {
                string kw = Scalar(kv.Key);
                string[] pool = SeqStrings(kv.Value as YamlSequenceNode);
                if (string.IsNullOrEmpty(kw) || pool == null) continue;
                int idx = dst.FindIndex(p => string.Equals(p.Key, kw, StringComparison.OrdinalIgnoreCase));
                KeyValuePair<string, string[]> entry = new KeyValuePair<string, string[]>(kw, pool);
                if (idx >= 0) dst[idx] = entry; else dst.Add(entry);
            }
        }

        // ---- YAML 取值小工具（不依赖 YamlNode 的 Equals 语义，逐个比字符串）----

        private static YamlNode Child(YamlMappingNode map, string key)
        {
            if (map == null) return null;
            foreach (KeyValuePair<YamlNode, YamlNode> kv in map.Children)
                if (Scalar(kv.Key) == key) return kv.Value;
            return null;
        }

        private static YamlMappingNode Map(YamlMappingNode map, string key) =>
            Child(map, key) as YamlMappingNode;

        private static YamlSequenceNode SeqNode(YamlMappingNode map, string key) =>
            Child(map, key) as YamlSequenceNode;

        private static string Scalar(YamlNode node) => (node as YamlScalarNode)?.Value;

        private static string[] SeqStrings(YamlSequenceNode seq)
        {
            if (seq == null) return null;
            List<string> list = new List<string>();
            foreach (YamlNode node in seq.Children)
            {
                string s = Scalar(node);
                if (!string.IsNullOrEmpty(s)) list.Add(s);
            }
            return list.Count > 0 ? list.ToArray() : null;
        }

        /// <summary>按优先级取第一个非空序列。</summary>
        private static string[] Seq(YamlMappingNode map, string key) => SeqStrings(SeqNode(map, key));

        private static string[] Or(params string[][] candidates)
        {
            foreach (string[] c in candidates)
                if (c != null && c.Length > 0) return c;
            return Emergency;
        }
    }
}
