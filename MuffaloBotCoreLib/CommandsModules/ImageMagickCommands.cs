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

namespace MuffaloBotCoreLib.CommandsModules
{
    [MuffaloBotCommandsModule, Cooldown(1, 60, CooldownBucketType.User)]
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
                Stream downloadStream;
                try
                {
                    downloadStream = await client.OpenReadTaskAsync(attachmentUrl);
                }
                catch (WebException)
                {
                    await ctx.RespondAsync("Error connecting to image link.");
                    return;
                }
                if (attachmentUrl.EndsWith(".gif"))
                {
                    await DoImageMagickCommandForGif(ctx, downloadStream, mode);
                }
                else
                {
                    await DoImageMagickCommandForStillImage(ctx, downloadStream, mode);
                }
                downloadStream.Dispose();
            }
            else
            {
                await ctx.RespondAsync("No image found.");
            }
        }
        async Task DoImageMagickCommandForGif(CommandContext ctx, Stream downloadStream, ImageEditMode mode)
        {
            MagickImageCollection image;
            try
            {
                image = new MagickImageCollection(downloadStream);
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
            }
            image.Coalesce();
            foreach (MagickImage frame in image)
            {
                DoMagic(mode, frame, originalWidth, originalHeight);
            }
            using (Stream stream = new MemoryStream())
            {
                image.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                await ctx.RespondWithFileAsync(stream, "magic.gif");
            }
        }
        async Task DoImageMagickCommandForStillImage(CommandContext ctx, Stream downloadStream, ImageEditMode mode)
        {
            MagickImage image;
            try
            {
                image = new MagickImage(downloadStream);
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
            DoMagic(mode, image, originalWidth, originalHeight);
            using (Stream stream = new MemoryStream())
            {
                image.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                await ctx.RespondWithFileAsync(stream, "magic.png");
            }
        }

        void DoMagic(ImageEditMode mode, MagickImage image, int originalWidth, int originalHeight)
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
