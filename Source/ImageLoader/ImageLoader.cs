using LBoL.Base.Extensions;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;

namespace CirnoRCharacterMod.ImageLoader
{
    public sealed class CirnoRCharacterImageLoader
    {
        public static string file_extension = ".png";

        public static PlayerImages LoadPlayerImages(string name)
        {
            PlayerImages sprites = new PlayerImages();
            sprites.AutoLoad(
                name,
                (s) =>
                    ResourceLoader.LoadSprite(
                        s,
                        BepinexPlugin.directorySource,
                        ppu: 100,
                        1,
                        FilterMode.Bilinear,
                        generateMipMaps: true
                    ),
                (s) => ResourceLoader.LoadSpriteAsync(s, BepinexPlugin.directorySource)
            );
            // 设置帧序列
            // sprites.SetFrameSequences();
            return sprites;
        }

        private static Sprite[] LoadAnimationFrames(string baseName, int frameCount)
        {
            Sprite[] frames = new Sprite[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                // 格式：基础名_序号（如 player_walk_0000）
                string framePath = $"{baseName}_{i:D4}";
                frames[i] = ResourceLoader.LoadSprite(framePath, BepinexPlugin.directorySource);

                // 错误处理：如果某帧加载失败
                if (frames[i] == null)
                {
                    BepinexPlugin.log.LogWarning($"Failed to load animation frame: {framePath}");
                }
            }
            return frames;
        }

        public static CardImages LoadCardImages(CardTemplate cardTemplate)
        {
            var imgs = new CardImages(BepinexPlugin.embeddedSource);
            // Load image and change the image on Upgrade if UpgradeImageId is not null.
            imgs.AutoLoad(
                cardTemplate,
                extension: file_extension,
                hasUpgradeImage: !cardTemplate.MakeConfig().UpgradeImageId.IsNullOrEmpty()
            );
            return imgs;
        }

        public static ExhibitSprites LoadExhibitSprite(ExhibitTemplate exhibit)
        {
            var exhibitSprites = new ExhibitSprites();
            exhibitSprites.main = ResourceLoader.LoadSprite(
                exhibit.GetId() + file_extension,
                BepinexPlugin.embeddedSource
            );
            ;
            return exhibitSprites;
        }

        public static Sprite LoadUltLoader(UltimateSkillTemplate ult)
        {
            return LoadSprite(ult.GetId());
        }

        public static Sprite LoadStatusEffectLoader(StatusEffectTemplate status)
        {
            return LoadSprite(status.GetId());
        }

        public static Sprite LoadSprite(IdContainer ID)
        {
            return ResourceLoader.LoadSprite(ID + file_extension, BepinexPlugin.embeddedSource);
        }
    }
}
