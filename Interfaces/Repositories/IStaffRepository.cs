using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IStaffRepository
    {
        Task<(List<Staff>, int)> GetAllStaffs(BaseQueryObject queryObject);
        Task<Staff?> GetStaffById(int staffId);
        Task<Staff?> GetStaffByIdIncludeInactive(int staffId);
        Task<Staff?> GetStaffByAccountId(int accountId);
        Task<Staff?> GetStaffByEmail(string email);
        Task<Staff?> GetStaffByEmailIncludeInactive(string email);
        Task AddStaff(Staff staff);
        Task UpdateStaff(Staff staff);
    }
}
