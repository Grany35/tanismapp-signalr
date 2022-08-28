using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WebApi.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageDal _messageDal;
        private readonly IUserDal _userDal;

        public MessageHub(IMessageDal messageDal, IUserDal userDal)
        {
            _messageDal = messageDal;
            _userDal = userDal;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.FindFirst(ClaimTypes.Name)?.Value, otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageDal.GetMessageThread(Context.User.FindFirst(ClaimTypes.Name)?.Value, otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var userName = Context.User.FindFirst(ClaimTypes.Name)?.Value;
            if (userName == createMessageDto.RecipientUserName.ToLower())
            {
                throw new HubException("Kendine mesaj gönderemezsin");
            }

            var sender = await _userDal.GetAsnc(x => x.UserName == userName);
            var recepient = await _userDal.GetAsnc(x => x.UserName == createMessageDto.RecipientUserName);

            if (recepient == null)
            {
                throw new HubException("Kulalnıcı bulunamadı");
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

            var groupName = GetGroupName(sender.UserName, recepient.UserName);

            

            _messageDal.Add(message);


            await Clients.Group(groupName).SendAsync("NewMessage", message);

            //return Ok(message);
        }


        

        


        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
