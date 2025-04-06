using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Repositories.Interceptors;
public class AuditDbContextInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entityEntry in eventData.Context!.ChangeTracker.Entries().ToList())
        {
            if (entityEntry.Entity is IAuditEntity auditEntity)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        auditEntity.Created = DateTime.Now;
                        eventData.Context.Entry(auditEntity).Property(x => x.Updated).IsModified = false;
                        break;

                    case EntityState.Modified:
                        eventData.Context.Entry(auditEntity).Property(x => x.Created).IsModified = false;
                        auditEntity.Updated = DateTime.Now;
                        break;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
