using System.Collections.Generic;
using Domain.Entities;

namespace Application.Services.ImageProcessingService;

public interface IImageProcessingService
{
    void AddData(List<ExperimentData> experimentDatas);
    void StartProcessing();
    void StopProcessing();
    void SetExperimentName(string name);
    bool IsActive();
}