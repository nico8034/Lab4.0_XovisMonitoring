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
    private readonly IImageProcessingService _imageProcessingService;

    public ExperimentService(IImageProcessingService imageProcessingService)
    {
      // _xovisService = xovisService;
      _imageProcessingService = imageProcessingService;
    }

    
    public Guid StartExperiment(bool shouldIncludeImages)
    {

      // Images
      withImages = shouldIncludeImages;
      
      // Experiment
      currentExperiment = new Experiment();
      
      //Start service for experiments that require images
      if (withImages)
      {
        // State
        _isRunning = true;
        
        // Task
        Task.Run(RunExperiment);
      }
      
      // Just return experiment id for experiments that dont require images
      return currentExperiment.Id;
    }

    public Experiment? GetCurrentExperiment()
    {
      return currentExperiment;
    }

    public async void StopExperiment()
    {
      
      // return if no experiment
      if (currentExperiment == null) return;
      
      _isRunning = false;
      if(withImages) await currentExperiment.Stop();
      currentExperiment = null;
    }

    public int GetDataInterval()
    {
      return dataInterval;
    }

    public bool GetWithImages()
    {
      return withImages;
    }

    public void SetDataInterval(int intervalMilliseconds)
    {
      dataInterval = intervalMilliseconds;
    }

    public void SetBatchsize(int batchsize)
    {
      batchSize = batchsize;
    }

    public async Task RunExperiment()
    {
      var stopwatch = new Stopwatch();
      while (_isRunning)
      {

        await Task.Delay(dataInterval);
        stopwatch.Start();

        if (currentExperiment == null) continue;

        // For experiments with images
        if (withImages)
        {
          // stopwatch.Start();
          // Get Images from xovisService
          // var stereoImage = await _xovisService.GetStereoImage();
          var validationImage = await _xovisService.GetValidationImage();
          // Add images to ExperimentData > Add ExperimentData to ongoing experiment

          // var imageResponseSuccess = (stereoImage.Data != null && validationImage.Data != null);
          var imageResponseSuccess = (validationImage.Data != null);
            
          if (imageResponseSuccess)
          {
            // currentExperiment.AddExperimentData(
            //   new ExperimentData(
            //     validationImage.Data,
            //     _monitoringService.GetRoom().GetZonePeopleCount()
            //   )
            // );

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
        // else
        // {
        //   var data = new ExperimentData(_monitoringService.GetRoom().GetZonePeopleCount());
        //   currentExperiment.AddExperimentData(data);
        //   
        //   // Log how much data we got so fare
        //   if(currentExperiment.ExperimentData.Count > 0 && currentExperiment.ExperimentData.Count % 10 == 0) Console.WriteLine($"EXPERIMENT: Added data {currentExperiment.ExperimentData.Count}");
        //   
        //   if (currentExperiment.ExperimentData.Count != batchSize) continue;
        //     
        //   _imageProcessingService.SetExperimentName(currentExperiment.ExperimentName);
        //   _imageProcessingService.AddData(currentExperiment.ExperimentData);
        //   currentExperiment.ExperimentData.Clear();
        //   Console.WriteLine("Moved experiment data to processing");
        // }
      }
    }

    public bool isRunning()
    {
      return _isRunning;
    }
  }
