using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ApplicationUser> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
