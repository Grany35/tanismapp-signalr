using Core.DataAccess;
using DataAccess.Helpers;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IMessageDal : IEntityRepository<Message>
    {
        
        
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<List<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
    }
}
