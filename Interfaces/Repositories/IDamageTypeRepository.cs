using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IDamageTypeRepository
    {
        Task<bool> VerifyPermission(int authRoleId, string permission);
        Task<(List<DamageType>, int)> GetAllDamageTypes(BaseQueryObject queryObject);
        Task<DamageType?> GetDamageTypeById(int damageTypeId);
        Task<DamageType?> GetDamageTypeByName(string damageTypeName);
        Task<bool> IsDamageTypeBeingUsed(int damageTypeId);
        Task AddDamageType(DamageType damageType);
        Task UpdateDamageType(DamageType damageType);
        Task DeleteDamageType(DamageType damageType);
    }
}
