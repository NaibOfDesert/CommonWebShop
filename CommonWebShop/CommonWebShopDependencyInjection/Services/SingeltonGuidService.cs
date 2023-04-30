namespace CommonWebShopDependencyInjection.Services
{
    public class SingeltonGuidService : ISingeltonGuidService
    {
        public readonly Guid Id;
        public SingeltonGuidService() 
        {
            Id = Guid.NewGuid();
        }

        public string GetGuid()
        {
            return Id.ToString();   
        }

    }
}
