namespace ContractorBackend.Domain.Common
{
    public interface IHasRowIntegrity
    {
        string Hash { set; get; }
    }
}
