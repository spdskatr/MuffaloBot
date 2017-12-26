using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using ImageMagick;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;
using System.IO;
using MuffaloBotNetFramework2.DiscordComponent;

namespace MuffaloBotNetFramework2.CommandsModules
{
    [MuffaloBotCommandsModule, Cooldown(1, 300, CooldownBucketType.User)]
    public class ImageMagickCommands
    {
        enum ImageEditMode
        {
            Swirl,
            Rescale,
            Wave,
            Implode
        }
        public ImageMagickCommands()
        {
            MagickNET.SetTempDirectory("MagickTemp");
        }
        [Command("swirl")]
        public Task ImageMagickDistort(CommandContext ctx, string link = null)
        {
            return DoImageMagickCommand(ctx, ImageEditMode.Swirl, link);
        }
        [Command("wonky")]
        public Task ImageMagickWonky(CommandContext ctx, string link = null)
        {
            return DoImageMagickCommand(ctx, ImageEditMode.Rescale, link);
        }
        [Command("wave")]
        public Task ImageMagickWave(CommandContext ctx, string link = null)
        {
            return DoImageMagickCommand(ctx, ImageEditMode.Wave, link);
        }
        [Command("implode")]
        public Task ImageMagickImplode(CommandContext ctx, string link = null)
        {
            return DoImageMagickCommand(ctx, ImageEditMode.Implode, link);
        }
        async Task DoImageMagickCommand(CommandContext ctx, ImageEditMode mode, string link)
        {
            await ctx.TriggerTypingAsync().ConfigureAwait(false);
            string attachmentUrl = null;
            if (!string.IsNullOrWhiteSpace(ctx.RawArgumentString) && link != null && Uri.TryCreate(link, UriKind.Absolute, out Uri uri))
            {
                attachmentUrl = link;
            }
            else
            {
                IReadOnlyList<DiscordMessage> messages = await ctx.Channel.GetMessagesAsync(10);
                for (int i = 0; i < messages.Count; i++)
                {
                    if (messages[i].Attachments.Count != 0)
                    {
                        attachmentUrl = messages[i].Attachments[0].Url;
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(attachmentUrl))
            {
                WebClient client = new WebClient();
                byte[] buffer;
                try
                {
                    buffer = await client.DownloadDataTaskAsync(attachmentUrl);
                }
                catch (WebException)
                {
                    await ctx.RespondAsync("Error connecting to image link.");
                    return;
                }
                if (attachmentUrl.EndsWith(".gif"))
                {
                    await DoImageMagickCommandForGif(ctx, buffer, mode);
                }
                else
                {
                    await DoImageMagickCommandForStillImage(ctx, buffer, mode);
                }
            }
            else
            {
                await ctx.RespondAsync("No image found.");
            }
        }
        async Task DoImageMagickCommandForGif(CommandContext ctx, byte[] buffer, ImageEditMode mode)
        {
            if (mode == ImageEditMode.Rescale)
            {
                await ctx.RespondAsync("This mode is not supported for gifs since it is slow and often dramatically increases gif size");
                return;
            }
            MagickImageCollection image;
            try
            {
                image = new MagickImageCollection(buffer);
            }
            catch (MagickMissingDelegateErrorException)
            {
                await ctx.RespondAsync("Image format not recognised.");
                return;
            }
            int originalWidth = image[0].Width, originalHeight = image[0].Height;
            if (originalHeight * originalWidth > 1000000)
            {
                await ctx.RespondAsync($"Gif exceeds maximum size of 1000000 pixels (Actual size: {originalHeight * originalWidth})");
                return;
            }
            if (image.Count > 100)
            {
                await ctx.RespondAsync($"Gif exceeds maximum frame count of 100 pixels (Actual count: {image.Count})");
                return;
            }
            image.Coalesce();
            long rawLength;
            using (MemoryStream stream = new MemoryStream())
            {
                image.Write(stream);
                rawLength = stream.Length;
            }
            double exceed = rawLength / 4194304d;
            double rescale = 1f;
            if (exceed > 1.0)
            {
                rescale = Math.Sqrt(exceed);
            }
            await ctx.TriggerTypingAsync();
            for (int i = 0; i < image.Count; i++)
            {
                IMagickImage frame = image[i];
                if (rescale > 1f)
                {
                    if (rescale > 2f)
                    {
                        frame.AdaptiveResize((int)(frame.Width / rescale), (int)(frame.Height / rescale));
                    }
                    else
                    {
                        frame.Resize((int)(frame.Width / rescale), (int)(frame.Height / rescale));
                    }
                }
                DoMagic(mode, frame, originalWidth, originalHeight);
            }
            await ctx.TriggerTypingAsync();
            image.OptimizeTransparency();
            using (Stream stream = new MemoryStream())
            {
                image.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                await ctx.RespondWithFileAsync(stream, "magic.gif");
            }
        }
        async Task DoImageMagickCommandForStillImage(CommandContext ctx, byte[] buffer, ImageEditMode mode)
        {
            MagickImage image;
            try
            {
                image = new MagickImage(buffer);
            }
            catch (MagickMissingDelegateErrorException)
            {
                await ctx.RespondAsync("Image format not recognised.");
                return;
            }
            int originalWidth = image.Width, originalHeight = image.Height;
            if (originalHeight * originalWidth > 2250000)
            {
                await ctx.RespondAsync($"Image exceeds maximum size of 2250000 pixels (Actual size: {originalHeight * originalWidth})");
            }
            // Do magic
            double exceed = buffer.Length / 8388608d;
            double rescale = 1f;
            if (exceed > 1.0)
            {
                rescale = 1.0 / Math.Sqrt(exceed);
            }
            if (rescale < 1f)
            {
                if (rescale < 0.5f)
                {
                    image.AdaptiveResize((int)(image.Width * rescale), (int)(image.Height * rescale));
                }
                else
                {
                    image.Resize((int)(image.Width * rescale), (int)(image.Height * rescale));
                }
            }
            DoMagic(mode, image, originalWidth, originalHeight);
            using (Stream stream = new MemoryStream())
            {
                image.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                await ctx.RespondWithFileAsync(stream, "magic.png");
            }
        }

        void DoMagic(ImageEditMode mode, IMagickImage image, int originalWidth, int originalHeight)
        {
            switch (mode)
            {
                case ImageEditMode.Swirl:
                    image.Swirl(360);
                    break;
                case ImageEditMode.Rescale:
                    image.LiquidRescale(image.Width / 2, image.Height / 2);
                    image.LiquidRescale((image.Width * 3) / 2, (image.Height * 3) / 2);
                    image.Resize(originalWidth, originalHeight);
                    break;
                case ImageEditMode.Wave:
                    image.BackgroundColor = MagickColor.FromRgb(0, 0, 0);
                    image.Wave(image.Interpolate, 10.0, 150.0);
                    break;
                case ImageEditMode.Implode:
                    image.Implode(0.5d, PixelInterpolateMethod.Average);
                    break;
                default:
                    break;
            }
        }
    }
}
