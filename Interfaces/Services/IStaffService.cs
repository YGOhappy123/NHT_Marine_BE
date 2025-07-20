using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.User;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IStaffService
    {
        Task<ServiceResponse> UpdateStaffProfile(UpdateStaffDto updateDto, int targetStaffId, int authUserId, int authRoleId);
    }
}
