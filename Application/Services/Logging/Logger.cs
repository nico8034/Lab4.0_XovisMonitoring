using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Logging
{
  public class Logger : ILogger
  {
    public string experimentName { get; set; } = string.Empty;


    public async Task WriteSuccessLog(ZonePersonCountDTO zone, string ExperimentName, string FileName)
    {
      experimentName = ExperimentName;
      // Check storage path
      var directory = Path.Combine(Environment.CurrentDirectory, "Experiments", ExperimentName);
      var finalPath = Path.Combine(directory, $"{FileName}.txt");

      // Create directory
      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);

      // Create log file if it doesnt exist
      if (!File.Exists(finalPath))
      {
        await File.WriteAllTextAsync(finalPath, "Date,Timestamp,Zone,PersonCount\n");
      }
      // Write to file if it exists
      else
      {
        await using var sw = new StreamWriter(finalPath, true);
        await sw.WriteLineAsync($"{zone.CalculatedTimeStamp:yyyy-MM-dd},{zone.CalculatedTimeStamp:HH:mm:ss.fff},{zone.ZoneReference.zone_name},{zone.ZoneReference.personCount}");
      }
    }

    public async Task WriteErrorLog(string message, string zone, string fileName, DateTime timeStamp)
    {
      // Check storage path
      var directory = Environment.CurrentDirectory + $@"/Experiments/{experimentName}";
      var finalPath = $"{directory}/{fileName}.txt";

      // Create directory
      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);

      // Create log file if it doesnt exist
      if (!File.Exists(finalPath))
      {
        await File.WriteAllTextAsync(finalPath, "Date,TimeStamp,Zone,Message\n");
      }
      // Write to file if it exists
      else
      {
        await using var sw = new StreamWriter(finalPath, true);
        await sw.WriteLineAsync($"{timeStamp:yyyy:MM:dd},{timeStamp:HH:mm:ss.fff},{zone},{message}");
      }
    }
  }
}