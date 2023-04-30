namespace CommonWebShopDependencyInjection.Services
{
    public class ScopedGuidService : IScopedGuidService
    {
        public readonly Guid Id;
        public ScopedGuidService()
        {
            Id = Guid.NewGuid();
        }

        public string GetGuid()
        {
            return Id.ToString();
        }
    }
}
