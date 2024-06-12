using ModelsLibrary.Models;

namespace DataAccessLibrary.Repository.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        void Update(ProductImage productImage);
    }
}
