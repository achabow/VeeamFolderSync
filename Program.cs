using System;
using VeeamFolderSync.Services;

if (args.Length != 4)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run <source> <replica> <seconds> <logfile>");
    return;
}

var src     = args[0];
var dst     = args[1];
var seconds = int.Parse(args[2]);
var logFile = args[3];

var logger = new SimpleLogger(logFile);
var sync   = new SyncService(src, dst, logger);

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    logger.Write("Received Ctrl+C – shutting down.");
    Environment.Exit(0);
};

logger.Write("Sync daemon started – press Ctrl+C to stop.");

while (true)
{
    try
    {
        sync.SyncOnce();
    }
    catch (Exception ex)
    {
        logger.Write($"ERROR: {ex.Message}");
    }
    Thread.Sleep(seconds * 1000);
}