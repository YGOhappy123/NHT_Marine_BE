using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class StorageTypeMapper
    {
        public static StorageTypeDto ToStorageTypeDto(this StorageType storageType)
        {
            return new StorageTypeDto { TypeId = storageType.TypeId, Name = storageType.Name };
        }
    }
}
