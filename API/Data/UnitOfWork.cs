
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public UnitOfWork(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IApplicationUserRepository ApplicationUserRepository => new ApplicationUserRepository(_db, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_db, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_db);

        public async Task<bool> Complete()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _db.ChangeTracker.HasChanges();
        }
    }
}