using NHT_Marine_BE.Data.Dtos.Response;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IFileService
    {
        Task<ServiceResponse> UploadImageToCloudinary(IFormFile imageFile, string? folderName);
        Task<ServiceResponse> UploadBase64ImageToCloudinary(string base64Image, string? folderName);
        Task<ServiceResponse> DeleteImageFromCloudinary(string imageUrl);
    }
}
