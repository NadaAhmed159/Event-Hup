using EventHub.Domain.Common;

namespace EventHub.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
       

        // Navigation
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
