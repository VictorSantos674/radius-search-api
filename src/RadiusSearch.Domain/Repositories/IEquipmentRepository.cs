using RadiusSearch.Domain.Entities;

namespace RadiusSearch.Domain.Repositories;

public interface IEquipmentRepository
{
    IEnumerable<Equipment> GetAll();
}
