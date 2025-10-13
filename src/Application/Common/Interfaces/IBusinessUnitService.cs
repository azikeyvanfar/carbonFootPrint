using System;
using System.Threading.Tasks;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IBusinessUnitService
    {
        Task<bool> ValidateBusinessUnitIsInArea(Guid areaId, Guid? businessUnitId);

        //Task<IQueryable<BusinessUnit>> GetChildrenBusinessUnits(Guid businessUnitId, bool? containsCurrentBusinessUnit = true);
        //Task<IQueryable<BusinessUnit>> GetCurrentUserChildrenBusinessUnitsAsync(Guid businessUnitId, bool? containsCurrentBusinessUnit = true);
        //IQueryable<BusinessUnit> GetCurrentUserChildrenBusinessUnits(Guid businessUnitId, bool? containsCurrentBusinessUnit = true);
    }
}
