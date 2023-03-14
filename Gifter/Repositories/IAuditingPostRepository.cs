using Gifter.Models;

namespace Gifter.Repositories
{
    public interface IAuditingPostRepository
    {
        void Add(Post post);
        void Delete(int id);
        List<Post> GetAll();
        List<Post> GetAllWithComments();
        Post GetById(int id);
        void Update(Post post);
    }
}