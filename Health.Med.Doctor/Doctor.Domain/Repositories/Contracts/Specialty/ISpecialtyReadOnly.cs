﻿namespace Doctor.Domain.Repositories.Contracts.Specialty;

public interface ISpecialtyReadOnly
{
    Task<Domain.Entities.Specialty> RecoverByIdAsync(Guid id);
    Task<Entities.Specialty> RecoverByStandardDescriptionAsync(string standardizedDescription);
    Task<IEnumerable<Entities.Specialty>> RecoverAllAsync(int page, int pageSize);
}
