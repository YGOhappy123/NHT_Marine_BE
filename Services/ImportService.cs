using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class ImportService : IImportService
    {
        private readonly IImportRepository _importRepo;
        private readonly IStorageRepository _storageRepo;
        private readonly IProductRepository _productRepo;
        private readonly IRoleRepository _roleRepo;

        public ImportService(
            IImportRepository importRepo,
            IStorageRepository storageRepo,
            IProductRepository productRepo,
            IRoleRepository roleRepo
        )
        {
            _importRepo = importRepo;
            _storageRepo = storageRepo;
            _productRepo = productRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<List<ImportDto>>> GetAllImports(BaseQueryObject queryObject)
        {
            var (imports, total) = await _importRepo.GetAllProductImports(queryObject);

            var mappedImports = imports.Select(s => s.ToImportDto()).ToList();
            foreach (var import in mappedImports)
            {
                foreach (var item in import.Items!)
                {
                    item.ProductItem = await _productRepo.GetDetailedProductItemById((int)item.ProductItemId!);
                }
            }

            return new ServiceResponse<List<ImportDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = mappedImports,
                Total = total,
                Took = imports.Count,
            };
        }
    }
}
