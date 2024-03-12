
namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUserRepository {get;}
        IMessageRepository MessageRepository {get;}
        ILikesRepository LikesRepository {get;}
        Task<bool> Complete(); // Complete Transaction, else all roll back
        bool HasChanges();

    }
}