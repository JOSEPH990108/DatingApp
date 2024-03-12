using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public MessageRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _db.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _db.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _db.Messages.Remove(message);
        }

        public async Task<Connection> GetCollection(string connectionId)
        {
            return await _db.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _db.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _db.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _db.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PageList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _db.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
                "outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
            };

            var mesages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PageList<MessageDTO>.CreateAsync(mesages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var query = _db.Messages
                .Where( m => m.RecipientUsername == currentUsername && m.RecipientDeleted == false
                        && m.SenderUsername == recipientUsername ||
                        m.RecipientUsername == recipientUsername && m.SenderDeleted == false &&
                        m.SenderUsername == currentUsername
                )
                .OrderBy(m => m.MessageSent)
                .AsQueryable();
            
            var unreadMessages = query.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Any()){
                foreach(var message in unreadMessages){
                    message.DateRead = DateTime.UtcNow;
                }
            }
            return await query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            _db.Connections.Remove(connection);
        }

    }
}