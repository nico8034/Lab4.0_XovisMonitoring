using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.ImageProcessingService;

public class ImageProcessingService : IImageProcessingService
  {
    public List<ExperimentData> dataToBeProcessed { get; set; } = new();
    public string CurrentExperimentName { get; set; } = string.Empty;
    public List<string> logFileData { get; set; }
    public bool isRunnung { get; set; } = false;

    public void StartProcessing()
    {
      isRunnung = true;
      Task.Run(Processing);
    }

    public void StopProcessing()
    {
      isRunnung = false;
    }

    private async Task Processing()
    {
      while (isRunnung)
      {
        if (dataToBeProcessed.Count > 0)
        {
          Console.WriteLine("WORKER: Starting batch processing");
          for (var i = 0; i < dataToBeProcessed.Count; i++)
          {
            var data = dataToBeProcessed.ElementAt(i);
            await SaveData(data);
          }
          dataToBeProcessed.Clear();
          Console.WriteLine("WORKER: Done");
        }
        else
        {
          Thread.Sleep(2000);
        }
      }
    }

    public async Task<bool> SaveData(ExperimentData experimentData)
    {
      // Check storage path
      var experimentDataLocatiton = Environment.CurrentDirectory + $@"/Experiments/{CurrentExperimentName}";
      
      // Create directory
      if (!Directory.Exists(experimentDataLocatiton))
        Directory.CreateDirectory(experimentDataLocatiton);

      // List to store log data
      logFileData = new List<string>();

      var experimentHasImages = experimentData.StereoImages != null && experimentData.ValidationImages != null;
      
      // EXPERIMENT WITH IMAGES
      if (experimentHasImages)
      {
        if (!File.Exists($"{experimentDataLocatiton}/imageLog.txt"))
          logFileData.Insert(0, "Image Name, Camera IP, Date, Time ,Zone, Person Count");
        
        try
        {
          SaveImageFromExperimentData(experimentDataLocatiton, experimentData.StereoImages, experimentData);
          SaveImageFromExperimentData(experimentDataLocatiton, experimentData.ValidationImages, experimentData);

          foreach (var line in logFileData)
          {
            await using var sw = new StreamWriter($"{experimentDataLocatiton}/imageLog.txt", true);
            await sw.WriteLineAsync(line);
          }
          logFileData.Clear();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
      // EXPERIMENT OUT IMAGES
      else
      {
        foreach (var item in experimentData.Zones)
        {
          logFileData.Add($"{experimentData.Timestamp:yyyy-MM-dd},{experimentData.Timestamp:HH:mm:ss.fff}, {item.Key}, {item.Value.Item2}");
        }

        if (!File.Exists($"{experimentDataLocatiton}/personCountLog.txt"))
          logFileData.Insert(0, "Date, Time, Zone, Person Count");

        foreach (var line in logFileData)
        {
          await using var sw = new StreamWriter($"{experimentDataLocatiton}/personCountLog.txt", true);
          await sw.WriteLineAsync(line);
        }
        logFileData.Clear();
      }
      // when done
      return true;
    }

    public void SaveImageFromExperimentData(string path, List<CameraImageData> images, ExperimentData expermentDataInformation)
    {
      for (var i = 0; i < images.Count; i++)
      {
        using (var memStream = new MemoryStream(images.ElementAt(i).imageData))
        {
          using (var img = Image.FromStream(memStream))
          {
            var imageName = $"{images.ElementAt(i).timestamp.Day.ToString("D2")}-{images.ElementAt(i).timestamp.Month.ToString("D2")}-{images.ElementAt(i).timestamp.Year}T{images.ElementAt(i).timestamp.Hour.ToString("D2")}-{images.ElementAt(i).timestamp.Minute.ToString("D2")}-{images.ElementAt(i).timestamp.ToString("ss.fff")}_{images.ElementAt(i).imageType}_{images.ElementAt(i).cameraInfo.Name}";
            img.Save($"{path}/{imageName}.png", ImageFormat.Png);

            
            foreach (var item in expermentDataInformation.Zones.Where(item => item.Value.Item1 == images.ElementAt(i).cameraInfo.Ip))
            {
              logFileData.Add($"{imageName}.png, {images.ElementAt(i).cameraInfo.Ip}, {images.ElementAt(i).timestamp:yyyy-MM-dd},{images.ElementAt(i).timestamp:HH:mm:ss.fff}, {item.Key}, {item.Value.Item2}");
            }
            // foreach (var item in expermentDataInformation.Zones)
            // {
            //   Console.WriteLine(item.Value.Item1);
            //   Console.WriteLine(images.ElementAt(i).cameraInfo.Ip);
            //   if (item.Value.Item1 == images.ElementAt(i).cameraInfo.Ip)
            //     logFileData.Add($"{imageName}.png, {images.ElementAt(i).cameraInfo.Ip}, {images.ElementAt(i).timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}, {item.Value.Item1}, {item.Value.Item2}");
            // }
          }
        }
      }

    }

    public void AddData(List<ExperimentData> experimentDatas)
    {
      dataToBeProcessed.AddRange(experimentDatas);
    }

    public void SetExperimentName(string name)
    {
      CurrentExperimentName = name;
    }

    public bool IsActive()
    {
      return isRunnung;
    }
  }