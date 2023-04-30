namespace CommonWebShopDependencyInjection.Services
{
    public class TransientGuidService : ITransientGuidService
    {
        public readonly Guid Id;
        public TransientGuidService()
        {
            Id = Guid.NewGuid();
        }

        public string GetGuid()
        {
            return Id.ToString();
        }
    }
}
