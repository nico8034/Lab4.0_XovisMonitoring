using System.Diagnostics;
using Application.Services.CameraService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using Domain.Entities;

namespace Application.Services.ExperimentService;

public class ExperimentService : IExperimentService
  {
    public int dataInterval { get; set; } = 50;
    public int batchSize { get; set; } = 20;
    public bool _isRunning { get; set; } = false;
    public bool withImages { get; set; } = false;
    public Experiment? currentExperiment { get; set; } = null;

    private readonly ICameraService _xovisService;
    private readonly IMonitoringService _monitoringService;
    private readonly IImageProcessingService _imageProcessingService;

    public ExperimentService(ICameraService xovisService, IMonitoringService monitoringService, IImageProcessingService imageProcessingService)
    {
      _xovisService = xovisService;
      _monitoringService = monitoringService;
      _imageProcessingService = imageProcessingService;
    }

    
    public Guid StartExperiment(bool shouldIncludeImages)
    {

      // Images
      withImages = shouldIncludeImages;
      
      // Experiment
      currentExperiment = new Experiment();
      
      
      // State
      _isRunning = true;

      // Thread
      // var ts = new ThreadStart(this.RunExperiment);
      // var backgroundThread = new Thread(ts);
      //
      // backgroundThread.Start();
      
      // Task
      Task.Run(RunExperiment);

      return currentExperiment.Id;
    }

    public Experiment? GetCurrentExperiment()
    {
      return currentExperiment;
    }

    public async void StopExperiment()
    {
      if (!_isRunning)
        return;
      
      // return if no experiment
      if (currentExperiment == null) return;
      
      _isRunning = false;
      await currentExperiment.Stop();
      currentExperiment = null;
    }

    public int GetDataInterval()
    {
      return dataInterval;
    }

    public void SetDataInterval(int intervalMilliseconds)
    {
      dataInterval = intervalMilliseconds;
    }

    public async Task RunExperiment()
    {
      var stopwatch = new Stopwatch();
      
      Console.WriteLine("Running");
      while (_isRunning)
      {

        await Task.Delay(dataInterval);

        if (currentExperiment == null) continue;
        // System.Console.WriteLine($"------> Fetching image Data {dataInterval}ms");

        // var stopwatch = new Stopwatch();

        // For experiments with images
        if (withImages)
        {
          stopwatch.Start();
          // Get Images from xovisService
          var stereoImage = await _xovisService.GetStereoImage();
          var validationImage = await _xovisService.GetValidationImage();
          // Add images to ExperimentData > Add ExperimentData to ongoing experiment

          var imageResponseSuccess = (stereoImage.Data != null && validationImage.Data != null);
            
          if (imageResponseSuccess)
          {
            currentExperiment.AddExperimentData(
              new ExperimentData(
                stereoImage.Data,
                validationImage.Data,
                _monitoringService.GetRoom().GetZonePeopleCount()
              )
            );

            stopwatch.Stop();
            Console.WriteLine($"Experiment: data input: {currentExperiment.ExperimentData.Count} - time in ms {stopwatch.ElapsedMilliseconds}");
            stopwatch.Reset();

            // Batch processing
            if (currentExperiment.ExperimentData.Count == batchSize)
            {
              _imageProcessingService.SetExperimentName(currentExperiment.ExperimentName);
              _imageProcessingService.AddData(currentExperiment.ExperimentData);
              currentExperiment.ExperimentData.Clear();
              Console.WriteLine("Moved experiment data to processing");
            }
            
          }
        }
        // For experiments without images
        else
        {
          var data = new ExperimentData(_monitoringService.GetRoom().GetZonePeopleCount());
          currentExperiment.AddExperimentData(data);
          Console.WriteLine($"Experimentdata count: {currentExperiment.ExperimentData.Count}");
            
          if (currentExperiment.ExperimentData.Count != batchSize) continue;
            
          _imageProcessingService.SetExperimentName(currentExperiment.ExperimentName);
          _imageProcessingService.AddData(currentExperiment.ExperimentData);
          currentExperiment.ExperimentData.Clear();
          Console.WriteLine("Moved experiment data to processing");
        }
      }
    }

    public bool isRunning()
    {
      return _isRunning;
    }
  }
