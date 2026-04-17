using ResponseForge.Sample.Models;

namespace ResponseForge.Sample.Services;

public interface IMockDataAppService
{
    List<Product> GetAll();
    Product? GetById(Guid id);
    Product Create(CreateProductRequest request);
    bool Delete(Guid id);
}
