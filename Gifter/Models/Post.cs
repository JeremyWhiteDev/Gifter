using System.ComponentModel.DataAnnotations;

namespace Gifter.Models;

public class Post
{
    public int Id { get; set; }

    [Required] 
    public string Title { get; set; }

    [Required]
    public string ImageUrl { get; set; } = null;

    public string Caption { get; set; } = null;

    [Required]
    public DateTime DateCreated { get; set; }

    [Required]
    public int UserProfileId { get; set; }

    public UserProfile? UserProfile { get; set; } = null;

    public List<Comment>? Comments { get; set; } = new List<Comment>();

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, ImageUrl: {ImageUrl}, Caption: {Caption}, DateCreated: {DateCreated}, UserProfileId: {UserProfileId}";
    }
}
