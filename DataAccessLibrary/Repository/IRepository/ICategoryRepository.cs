using ModelsLibrary.Models;

namespace DataAccessLibrary.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
