namespace Application.Experiments.DTOs;

public class ExperimentInfoDTO
{
    public Guid? Id { get; set; } 
    public string? Name { get; set; } = String.Empty;
    public DateTime? StartedAt { get; set; }
}