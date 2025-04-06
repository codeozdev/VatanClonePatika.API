namespace Repositories
{
    public class BaseEntity : IAuditEntity
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
