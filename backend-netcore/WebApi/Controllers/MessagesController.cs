using Business.Abstract;
using Business.Helpers;
using DataAccess.Abstract;
using DataAccess.Helpers;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IUserDal _userDal;
        private readonly IMessageDal _messageDal;

        public MessagesController(IMessageService messageService, IUserService userService, IUserDal userDal, IMessageDal messageDal)
        {
            _messageService = messageService;
            _userService = userService;
            _userDal = userDal;
            _messageDal = messageDal;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userName == createMessageDto.RecipientUserName.ToLower())
            {
                return BadRequest("Kendine mesaj gönderemezsin");
            }

            var sender = await _userDal.GetAsnc(x => x.UserName == userName);
            var recepient = await _userDal.GetAsnc(x => x.UserName == createMessageDto.RecipientUserName);

            if (recepient == null)
            {
                return NoContent();
            }


            var message = new Message
            {
                Sender = sender,
                Recipient = recepient,
                SenderUserName = sender.UserName,
                RecipientUserName = recepient.UserName,
                Content = createMessageDto.Content,
                RecipientId = recepient.Id,
                SenderId = sender.Id,
                RecipientDeleted = false,
                SenderDeleted = false,


            };

            _messageDal.Add(message);

            return Ok(message);
        }


        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.UserName = User.FindFirst(ClaimTypes.Name).Value;
            var messages = await _messageDal.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return Ok(messages);
        }


        [HttpGet("thread/{username}")]
        public async Task<IActionResult> GetMessageThread(string userName)
        {
            var currentUserName = User.FindFirst(ClaimTypes.Name).Value;
            var result = await _messageDal.GetMessageThread(currentUserName, userName);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var userName = User.FindFirst(ClaimTypes.Name).Value;

            var message = await _messageDal.GetAsnc(x => x.Id == id);

            if (message.SenderUserName != userName && message.RecipientUserName != userName)
            {
                return Unauthorized();
            }

            if (message.SenderUserName == userName)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientUserName == userName)
            {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messageDal.Delete(message);
            }

            return Ok();

        }
    }
}
