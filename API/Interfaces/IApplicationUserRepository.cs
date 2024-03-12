using API.DTOs;
using API.Entities;
using API.Helpers;
using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IApplicationUserRepository
    {
        void Update(ApplicationUser user);
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(int id);
        Task<ApplicationUser> GetUserByUsernameAsync(string username);
        Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams);
        Task<MemberDTO> GetMemberAsync(string username);
        Task<string> GetUserGender(string username);
    }
}


