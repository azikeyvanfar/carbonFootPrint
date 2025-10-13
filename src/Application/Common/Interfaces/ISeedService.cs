using System.Threading.Tasks;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface ISeedService
    {
        Task<bool> SeedLookups();
        //Task<bool> SeedUnits();

    }
}
