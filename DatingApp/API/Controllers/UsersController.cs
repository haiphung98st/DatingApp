using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> Get()
        {

            //var user = (await _userRepository.GetUsersAsync());
            var userToReturn =  await _userRepository.GetMembersAsync();
            return Ok(userToReturn);
        }

        // GET api/<UsersController>/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<AppUser>> Get(int id)
        //{
        //    return await _userRepository.GetUserById(id);
        //}
        [HttpGet("{username}")]
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
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
    }
}
