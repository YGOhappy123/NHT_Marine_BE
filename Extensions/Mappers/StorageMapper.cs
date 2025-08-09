using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class StorageTypeMapper
    {
        public static StorageDto ToStorageDto(this Storage storage)
        {
            return new StorageDto
            {
                StorageId = storage.StorageId,
                Name = storage.Name,
                TypeId = storage.TypeId,
                Type = storage.Type?.ToStorageTypeDto(),
                ProductItems = storage
                    .ProductItems?.Select(iv => new InventoryDto { Quantity = iv.Quantity, ProductItemId = iv.ProductItemId })
                    .ToList(),
            };
        }

        public static StorageTypeDto ToStorageTypeDto(this StorageType storageType)
        {
            return new StorageTypeDto { TypeId = storageType.TypeId, Name = storageType.Name };
        }
    }
}
