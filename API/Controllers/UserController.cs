using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using AutoMapper;
using API.DTOs;
using API.Extensions;
using API.Entities;
using API.Helpers;

namespace API.Controllers;

[Authorize]
public class UserController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public UserController(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PageList<MemberDTO>>> GetUsers([FromQuery]UserParams userParams)
    {
        var gender = await _unitOfWork.ApplicationUserRepository.GetUserGender(User.GetUsername());
        userParams.CurrentUsername = User.GetUsername();

        if(string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "male" ? "female" : "male"; //If user's gender is male, then userParams.Gender == female and vice versa
        }

        var users = await _unitOfWork.ApplicationUserRepository.GetMembersAsync(userParams);

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }

    [HttpGet("{username}")] // api/user/joseph
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        return await _unitOfWork.ApplicationUserRepository.GetMemberAsync(username);

    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {

        var user = await _unitOfWork.ApplicationUserRepository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return NotFound();

        _mapper.Map(memberUpdateDTO, user);

        if(await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update. Please contact developer for assistance.");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto (IFormFile file)
    {
        var user = await _unitOfWork.ApplicationUserRepository.GetUserByUsernameAsync(User.GetUsername());

         if(user == null) return NotFound();

         var result = await _imageService.AddPhotoAsync(file);

         if(result.Error != null) return BadRequest(result.Error.Message);

         var photo =new Photo
         {
            ImageUrl = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
         };

         if(user.Photos.Count == 0) photo.IsMain = true;

         user.Photos.Add(photo);

         if(await _unitOfWork.Complete())
         {
            return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDTO>(photo));
         }  

         return BadRequest("Problem occurs during adding photo.");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _unitOfWork.ApplicationUserRepository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if(photo == null) return NotFound();

        if(photo.IsMain) return BadRequest("Cannot set main photo to main again.");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

        if(currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if(await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Problem occurs during setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _unitOfWork.ApplicationUserRepository.GetUserByUsernameAsync(User.GetUsername());

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if(photo == null) return NotFound();

        if(photo.IsMain) return BadRequest("You cannot delete your main photo");

        if(photo.PublicId != null)
        {
            var result = await _imageService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if(await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem occurs during deleting photo");
    }
}
