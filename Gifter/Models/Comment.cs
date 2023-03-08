﻿namespace Gifter.Models;

public class Comment
{
    public int Id { get; set; }
    public string Message { get; set; }
    public  UserProfile UserProfile { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}
