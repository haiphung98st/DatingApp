using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> Get()
        {

            //var user = (await _userRepository.GetUsersAsync());
            var userToReturn = await _userRepository.GetMembersAsync();
            return Ok(userToReturn);
        }

        // GET api/<UsersController>/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<AppUser>> Get(int id)
        //{
        //    return await _userRepository.GetUserById(id);
        //}
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> Get(string username)
        {
            var result = await _userRepository.GetMemberByUsername(username);
            return result;
        }

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut]
        public async Task<ActionResult> Put(UpdateMemberInfoDto member)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsername(username);
            _mapper.Map(member, user);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Fail to update!");
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> UploadPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsername(User.GetUsername());
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0) photo.IsMainPhoto = true;
            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync()) return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            return BadRequest("Somethings wrong when upload photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsername(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo.IsMainPhoto) return BadRequest("This photo is already main");
            var mainPhoto = user.Photos.FirstOrDefault(x => x.IsMainPhoto);
            if (mainPhoto != null) mainPhoto.IsMainPhoto = false;
            photo.IsMainPhoto = true;
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Cannot set main photo");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsername(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMainPhoto) return BadRequest("You cannot delete the main photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error);
            }
            user.Photos.Remove(photo);
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Fail to delete photo");
        }
    }
}
