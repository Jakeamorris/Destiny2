using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Destiny2.Services
{
    class ManifestSettings : IManifestSettings
    {        
        public FileInfo DbPath { get; }
        public FileInfo VersionPath { get; }

        private int _manifestCheckTimeout = 5 * 60 * 1000;
        public int ManifestCheckTimeout
        {
            get { return _manifestCheckTimeout; }
            internal set { _manifestCheckTimeout = value; }
        }

        public ManifestSettings(ILogger<ManifestSettings> logger, string dbPath = "")
        {
            var dir = GetDbPath(logger, dbPath);
            if(!Directory.Exists(dir))
            {
                logger.LogInformation($"Manifest DB directory ({dir}) doesn't exist, creating it.");
                Directory.CreateDirectory(dir);
            }

            VersionPath = new FileInfo(Path.Combine(dir, "ManifestVersion.txt"));
            DbPath = new FileInfo(Path.Combine(dir, "Manifest.db"));

            logger.LogInformation($"DbPath = {DbPath}.");
        }

        private static string GetDbPath(ILogger logger, string dbPath)
        {
            if(!string.IsNullOrEmpty(dbPath))
            {
                return dbPath;
            }

            var scratchFolder = GetScratchFolder();
            if(!Directory.Exists(scratchFolder))
            {
                logger.LogInformation($"Scratch folder ({scratchFolder}) doesn't exist, creating it.");
                Directory.CreateDirectory(scratchFolder);
            }

            return Path.Combine(scratchFolder, "Destiny2Manifest");
        }

        private static string GetScratchFolder()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Environment.GetEnvironmentVariable("HOME");
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }
}