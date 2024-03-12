
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessageController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var username = User.GetUsername();

            if(username == createMessageDTO.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }

            var sender = await _unitOfWork.ApplicationUserRepository.GetUserByUsernameAsync(username);
            var recipient = await _unitOfWork.ApplicationUserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if(recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            _unitOfWork.MessageRepository.AddMessage(message);

            if(await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Error occurs while sending message");
        }
        
        [HttpGet]
        public async Task<ActionResult<PageList<MessageDTO>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
            
            return messages;
        }

        // [HttpGet("thread/{username}")]
        // public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        // {
        //     var currentUsername = User.GetUsername();

        //     return Ok(await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if(message.SenderUsername != username && message.RecipientUsername != username) 
                return Unauthorized();

            if(message.SenderUsername == username)
                message.SenderDeleted = true;
            
            if(message.RecipientUsername == username)
                message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted)
            {
                _unitOfWork.MessageRepository.DeleteMessage(message);
            }

            if(await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}