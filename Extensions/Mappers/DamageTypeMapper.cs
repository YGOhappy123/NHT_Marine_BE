using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class DamageTypeMapper
    {
        public static DamageTypeDto ToDamageTypeDto(this DamageType damageType)
        {
            return new DamageTypeDto { TypeId = damageType.TypeId, Name = damageType.Name };
        }
    }
}
