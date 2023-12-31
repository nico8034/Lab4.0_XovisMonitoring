using System;
using System.IO;
using System.Threading.Tasks;
using Application.Services.ExperimentService;
using Domain.Entities;

namespace Application.Services.Logging
{
    public class Logger : ILogger
    {
        private readonly IExperimentService _experimentService;
        private string experimentName = string.Empty;
        private StreamWriter? successWriter = null;
        private StreamWriter? errorWriter = null;
        private const int MAX_ENTRIES_BEFORE_FLUSH = 1000;
        private int successEntryCount = 0;
        private int errorEntryCount = 0;

        public Logger(IExperimentService experimentService)
        {
            _experimentService = experimentService;
        }

        public async Task WriteSuccessLog(ZonePersonCountDTO zone, string ExperimentName, string FileName)
        {
            await EnsureFileIsOpenForWriting(ExperimentName, FileName, "Date,Timestamp,Zone,PersonCount\n", isSuccessLog: true);
            await WriteLogLine($"{zone.CalculatedTimeStamp:yyyy-MM-dd},{zone.CalculatedTimeStamp:HH:mm:ss.fff},{zone.ZoneReference.zone_name},{zone.ZoneReference.personCount}", isSuccessLog: true);
        }

        public async Task WriteErrorLog(string message, string zone, string fileName, DateTime timeStamp)
        {
            if (string.IsNullOrEmpty(experimentName))
            {
                experimentName = _experimentService.GetCurrentExperiment().GetExperimentName();
            }
            
            await EnsureFileIsOpenForWriting(experimentName, fileName, "Date,TimeStamp,Zone,Message\n", isSuccessLog: false);
            await WriteLogLine($"{timeStamp:yyyy-MM-dd},{timeStamp:HH:mm:ss.fff},{zone},{message}", isSuccessLog: false);
        }

        // private async Task EnsureFileIsOpenForWriting(string ExperimentName, string FileName, string header, bool isSuccessLog)
        // {
        //     StreamWriter? targetWriter = isSuccessLog ? successWriter : errorWriter;
        //     if (targetWriter == null || !experimentName.Equals(ExperimentName, StringComparison.OrdinalIgnoreCase))
        //     {
        //         // Close and flush the previous writer if it was open.
        //         if (targetWriter != null)
        //         {
        //             await targetWriter.FlushAsync();
        //             await targetWriter.DisposeAsync();
        //             targetWriter = null;
        //         }
        //
        //         experimentName = ExperimentName;
        //
        //         var directory = Path.Combine(Environment.CurrentDirectory, "Experiments", ExperimentName);
        //         var finalPath = Path.Combine(directory, $"{FileName}.txt");
        //
        //         Directory.CreateDirectory(directory);
        //
        //         if (!File.Exists(finalPath))
        //         {
        //             await File.WriteAllTextAsync(finalPath, header);
        //         }
        //
        //         targetWriter = new StreamWriter(finalPath, true);
        //
        //         if (isSuccessLog)
        //         {
        //             successWriter = targetWriter;
        //             successEntryCount = 0;
        //         }
        //         else
        //         {
        //             errorWriter = targetWriter;
        //             errorEntryCount = 0;
        //         }
        //     }
        // }

        private async Task WriteLogLine(string line, bool isSuccessLog)
        {
            StreamWriter? targetWriter = isSuccessLog ? successWriter : errorWriter;
            int currentCount = isSuccessLog ? successEntryCount : errorEntryCount;

            await targetWriter.WriteLineAsync(line);
            currentCount++;

            if (currentCount >= MAX_ENTRIES_BEFORE_FLUSH)
            {
                await targetWriter.FlushAsync();
                currentCount = 0;
            }

            if (isSuccessLog)
            {
                successEntryCount = currentCount;
            }
            else
            {
                errorEntryCount = currentCount;
            }
        }


        public async ValueTask DisposeAsync()
        {
            if (successWriter != null)
            {
                await FlushAndDisposeAsync(successWriter);
                successWriter = null;
            }
            if (errorWriter != null)
            {
                await FlushAndDisposeAsync(errorWriter);
                errorWriter = null;
            }
        }
        
        private async Task FlushAndDisposeAsync(StreamWriter writer)
        {
            if (writer != null)
            {
                try
                {
                    await writer.FlushAsync();
                    await writer.DisposeAsync();
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine(ex.Message);
                    // Optionally handle the exception or log it
                }
            }
        }
        
        private async Task EnsureFileIsOpenForWriting(string ExperimentName, string FileName, string header, bool isSuccessLog)
        {
            StreamWriter? targetWriter = isSuccessLog ? successWriter : errorWriter;
            if (targetWriter == null || !experimentName.Equals(ExperimentName, StringComparison.OrdinalIgnoreCase))
            {
                if (targetWriter != null)
                {
                    await targetWriter.FlushAsync();
                    await targetWriter.DisposeAsync();
                }

                experimentName = ExperimentName;

                var directory = Path.Combine(Environment.CurrentDirectory, "Experiments", ExperimentName);
                var finalPath = Path.Combine(directory, $"{FileName}.txt");

                Directory.CreateDirectory(directory);

                if (!File.Exists(finalPath))
                {
                    await File.WriteAllTextAsync(finalPath, header);
                }

                targetWriter = new StreamWriter(finalPath, true);

                if (isSuccessLog)
                {
                    successWriter = targetWriter;
                    successEntryCount = 0;
                }
                else
                {
                    errorWriter = targetWriter;
                    errorEntryCount = 0;
                }
            }
        }
    }
}
