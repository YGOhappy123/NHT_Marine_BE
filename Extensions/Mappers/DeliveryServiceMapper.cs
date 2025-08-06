using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class DeliveryServiceMapper
    {
        public static DeliveryServiceDto ToDeliveryServiceDto(this DeliveryService deliveryService)
        {
            return new DeliveryServiceDto
            {
                ServiceId = deliveryService.ServiceId,
                Name = deliveryService.Name,
                ContactPhone = deliveryService.ContactPhone,
            };
        }
    }
}
