using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using VeeamFolderSync.Services;

namespace VeeamFolderSync.Services
{
    public class SyncService
    {
        private readonly string _source;
        private readonly string _replica;
        private readonly SimpleLogger _log;

        public SyncService(string source, string replica, SimpleLogger logger)
        {
            _source = source;
            _replica = replica;
            _log = logger;

            if (!Directory.Exists(_source))
                throw new DirectoryNotFoundException($"Source folder missing: {_source}");

            Directory.CreateDirectory(_replica);
        }

        public void SyncOnce()
        {
            _log.Write("Starting sync cycle...");

            // Copy / update files
            foreach (var srcFile in Directory.EnumerateFiles(_source, "*", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(_source, srcFile);
                var dstFile = Path.Combine(_replica, rel);

                Directory.CreateDirectory(Path.GetDirectoryName(dstFile)!);

                if (!File.Exists(dstFile) || !FilesEqual(srcFile, dstFile))
                {
                    File.Copy(srcFile, dstFile, true);
                    _log.Write($"Copied  {rel}");
                }
            }

            // Remove surplus files
            foreach (var dstFile in Directory.EnumerateFiles(_replica, "*", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(_replica, dstFile);
                var srcFile = Path.Combine(_source, rel);

                if (!File.Exists(srcFile))
                {
                    File.Delete(dstFile);
                    _log.Write($"Deleted {rel}");
                }
            }

            // Remove surplus directories (bottom-up)
            foreach (var dstDir in Directory.EnumerateDirectories(_replica, "*", SearchOption.AllDirectories)
                                            .OrderByDescending(d => d.Length))
            {
                var rel = Path.GetRelativePath(_replica, dstDir);
                var srcDir = Path.Combine(_source, rel);

                if (!Directory.Exists(srcDir))
                {
                    Directory.Delete(dstDir, true);
                    _log.Write($"Deleted dir  {rel}");
                }
            }

            _log.Write("Sync cycle finished.");
        }

        private static bool FilesEqual(string a, string b)
        {
            const int buffer = 1 << 20; // 1 MB
            using var fsA = new FileStream(a, FileMode.Open, FileAccess.Read, FileShare.Read, buffer);
            using var fsB = new FileStream(b, FileMode.Open, FileAccess.Read, FileShare.Read, buffer);

            if (fsA.Length != fsB.Length) return false;

            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(fsA)) ==
                   Convert.ToHexString(sha.ComputeHash(fsB));
        }
    }
}