namespace ContractorBackend.Domain.Common
{
    public interface IHasRowVersion
    {
        byte[] RowVersion { set; get; }
    }
}
