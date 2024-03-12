using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ApplicationUserRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _db.Users
                .Where(u => u.UserName == username)
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
            var query =  _db.Users.AsQueryable();

            query =  query.Where(u => u.UserName != userParams.CurrentUsername); //Avoid user being display at the match list
            query = query.Where(u => u.Gender == userParams.Gender); 
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u  => u.LastActive)
            };

            return await PageList<MemberDTO>
                .CreateAsync(
                    query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider), 
                    userParams.PageNumber, 
                    userParams.PageSize);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(int id)
        {
            return await _db.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
        {
            return await _db.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _db.Users.Where(u => u.UserName == username).Select(x => x.Gender).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _db.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public void Update(ApplicationUser user)
        {
            _db.Entry(user).State = EntityState.Modified;
        }
    }

}

