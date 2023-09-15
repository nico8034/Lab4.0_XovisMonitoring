using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;

namespace Domain.Entities;

public class Experiment
{
  private DateTime StartTime { get; set; }
  private DateTime EndTime { get; set; }
  public List<ExperimentData> ExperimentData { get; set; }
  public string ExperimentName { get; set; }
  private List<string> LogFileData { get; set; }
  public Guid Id { get; set; }

    public Experiment()
    {
      Id = Guid.NewGuid();
      StartTime = DateTime.Now;
      ExperimentName = $"experiment_{StartTime.Date.ToShortDateString()}T{StartTime.Hour:D2}-{StartTime.Minute:D2}-{StartTime.Second:D2}";
      ExperimentData = new List<ExperimentData>();
    }

    public void AddExperimentData(ExperimentData data)
    {
      ExperimentData.Add(data);
    }

    // public List<ExperimentData> GetExperimentData()
    // {
    //   return ExperimentData;
    // }

    public async Task Stop()
    {
      EndTime = DateTime.Now;
      var experimentDataLocation = Environment.CurrentDirectory + @"/Experiments";

      if (!Directory.Exists(experimentDataLocation))
        Directory.CreateDirectory(experimentDataLocation);

      if (ExperimentData.Count > 0)
      {
        await SaveData();
        Console.WriteLine("Saved last experiment data");
      }
    }

    public DateTime GetStartTime()
    {
      return StartTime;
    }
    
    public DateTime GetEndTime()
    {
      return EndTime;
    }
    
    public string GetExperimentName()
    {
      return ExperimentName;
    }

    public async Task SaveData()
    {
      // Check storage path
      var experimentDataLocation = Environment.CurrentDirectory + $@"/Experiments/{ExperimentName}";
      if (!Directory.Exists(experimentDataLocation))
        Directory.CreateDirectory(experimentDataLocation);

      // List of log data
      LogFileData = new List<string>();

      // IF EXPERIMENT WITH IMAGE DATA
      if (ExperimentData[0].ValidationImages != null)
      {
        try
        {
          // Save images
          for (int i = 0; i < this.ExperimentData.Count; i++)
          {
            var data = ExperimentData.ElementAt(i);
            // SaveCameraImage(experimentDataLocation, data.StereoImages, data);
            SaveCameraImage(experimentDataLocation, data.ValidationImages, data);
          }

          if (!File.Exists($"{experimentDataLocation}/imageLog.txt"))
            LogFileData.Insert(0, "Image Name, Camera IP, Date, Time ,Zone, Person Count");

          foreach (var line in LogFileData)
          {
            await using var sw = new StreamWriter($"{experimentDataLocation}/imageLog.txt", true);
            await sw.WriteLineAsync(line);
          }
          LogFileData.Clear();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
      // IF EXPERIMENT WITH NO IMAGE DATA
      else
      {
        foreach (var data in ExperimentData)
        {
          foreach (var item in data.Zones)
          {
            LogFileData.Add($"{item.Value.Item3:yyyy-MM-dd},{data.Timestamp:HH:mm:ss.fff},{item.Value.Item3:HH:mm:ss.fff}, {item.Key}, {item.Value.Item2}");
          }
        }
        
        if (!File.Exists($"{experimentDataLocation}/personCountLog.txt"))
          LogFileData.Insert(0, "Date, Time logged, Updated, Zone, Person Count");
        
        foreach (var line in LogFileData)
        {
          await using var sw = new StreamWriter($"{experimentDataLocation}/personCountLog.txt", true);
          await sw.WriteLineAsync(line);
        }
        LogFileData.Clear();

        // await File.WriteAllLinesAsync($"{experimentDataLocation}/personCountLog.txt", LogFileData);
      }
    }

    public void SaveCameraImage(string path, List<CameraImageData> images, ExperimentData expermentDataInformation)
    {
      for (var i = 0; i < images.Count; i++)
      {
        using (var memStream = new MemoryStream(images.ElementAt(i).imageData))
        {
          using (var img = Image.FromStream(memStream))
          {
            var imageName = $"{expermentDataInformation.Timestamp.Day:D2}-{expermentDataInformation.Timestamp.Month:D2}-{expermentDataInformation.Timestamp.Year}T{expermentDataInformation.Timestamp.Hour:D2}-{expermentDataInformation.Timestamp.Minute:D2}-{expermentDataInformation.Timestamp:ss.fff}_{images.ElementAt(i).imageType}_{images.ElementAt(i).cameraInfo.Name}";
            img.Save($"{path}/{imageName}.png", ImageFormat.Png);
            
            foreach (var item in expermentDataInformation.Zones.Where(item => item.Value.Item1 == images.ElementAt(i).cameraInfo.Ip))
            {
              LogFileData.Add($"{imageName}.png, {images.ElementAt(i).cameraInfo.Ip}, {images.ElementAt(i).timestamp:yyyy-MM-dd},{images.ElementAt(i).timestamp:HH:mm:ss.fff}, {item.Key}, {item.Value.Item2}");
            }
          }
        }
      }

    }
}