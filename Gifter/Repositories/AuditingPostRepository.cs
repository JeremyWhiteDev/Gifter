using Gifter.Models;

namespace Gifter.Repositories;

public class AuditingPostRepository : BaseRepository, IPostRepository, IAuditingPostRepository
{
    private readonly IPostRepository _postRepository;
    private readonly IAuditRepository _auditRepository;

    public AuditingPostRepository(IConfiguration configuration) : base(configuration) {
        _postRepository= new PostRepository(configuration);
        _auditRepository= new AuditRepository(configuration);   
    }

    public void Add(Post post)
    {
        _postRepository.Add(post);
        _auditRepository.Add("Post", "insert", null, post.ToString());
    }

    public void Delete(int id)
    {
        var postToDelete = _postRepository.GetById(id);
        _postRepository.Delete(id);
        _auditRepository.Add("Post", "delete", postToDelete.ToString(), null);
    }

    public List<Post> GetAll()
    {
        return _postRepository.GetAll();
    }

    public List<Post> GetAllWithComments()
    {
        return _postRepository.GetAllWithComments();
    }

    public Post GetById(int id)
    {
        return _postRepository.GetById(id);

    }

    public void Update(Post post)
    {
        var postToUpdate = _postRepository.GetById(post.Id);
        _postRepository.Update(post);
        _auditRepository.Add("Post", "update", postToUpdate.ToString(), post.ToString());

    }
}
