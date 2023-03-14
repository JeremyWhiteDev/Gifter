using Gifter.Models;
using Gifter.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gifter.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileRepository _userProfileRepository;


public UserProfileController(IUserProfileRepository userProfileRepository)
{
        _userProfileRepository = userProfileRepository;
}

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_userProfileRepository.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var post = _userProfileRepository.GetById(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpGet("WithPosts")]
    public IActionResult GetAllWithPosts()
    {
        return Ok(_userProfileRepository.GetAllWithPosts());
    }
    [HttpGet("WithPostsAndComments")]
    public IActionResult GetAllWithPostsAndComments()
    {
        var result = _userProfileRepository.GetAllWithPostsAndComments();
        //ignore null values for endpoint
        return new JsonResult(result, new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
     
    }


    [HttpPost]
    public IActionResult Post(UserProfile userProfile)
    {
        _userProfileRepository.Add(userProfile);
        return CreatedAtAction("Get", new { id = userProfile.Id }, userProfile);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, UserProfile userProfile)
    {
        if (id != userProfile.Id)
        {
            return BadRequest();
        }

        _userProfileRepository.Update(userProfile);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _userProfileRepository.Delete(id);
        return NoContent();
    }

}