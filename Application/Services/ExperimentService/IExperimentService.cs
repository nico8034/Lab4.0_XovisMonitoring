using Domain.Entities;

namespace Application.Services.ExperimentService;

public interface IExperimentService
{
    Guid StartExperiment(bool withImages);
    void StopExperiment();
    Task RunExperiment();
    void SetDataInterval(int intervalMilliseconds);
    bool isRunning();
    Experiment? GetCurrentExperiment();
}