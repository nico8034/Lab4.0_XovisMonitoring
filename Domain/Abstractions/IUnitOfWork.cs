namespace Domain.Abstractions;

public interface IUnitOfWork
{
    Task<int> saveChangesAsync(CancellationToken cancellationToken = default);
}