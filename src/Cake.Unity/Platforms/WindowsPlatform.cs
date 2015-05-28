﻿using Cake.Core;
using Cake.Core.IO;

namespace Cake.Mix.Platforms
{
    public sealed class WindowsPlatform : ISqlPlatform
    {
        private readonly FilePath _outputPath;

        public bool NoGraphics { get; set; }
        public UnityPlatformTarget PlatformTarget { get; set; }

        public WindowsPlatform(FilePath outputPath)
        {
            _outputPath = outputPath;
            PlatformTarget = UnityPlatformTarget.x86;
        }

        public void BuildArguments(ICakeContext context, ProcessArgumentBuilder builder)
        {
            if (NoGraphics)
            {
                builder.Append("-nographics");
            }

            builder.Append(PlatformTarget == UnityPlatformTarget.x64 ? "-buildWindows64Player" : "-buildWindowsPlayer");
            builder.AppendQuoted(_outputPath.MakeAbsolute(context.Environment).FullPath);
        }
    }
}
