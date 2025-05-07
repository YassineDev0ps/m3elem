namespace M3alam.Common.Entities
{
    public interface IDeletableEntity
    {
        public bool? IsDeleted { get; set; }
    }
}