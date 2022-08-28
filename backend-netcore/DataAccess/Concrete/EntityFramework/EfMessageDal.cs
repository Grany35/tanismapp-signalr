using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using DataAccess.Helpers;
using DataAccess.Migrations;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{

    public class EfMessageDal : EfEntityFrameworkBase<Message, ContextDb>, IMessageDal
    {
        

       

        

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            using (var context = new ContextDb())
            {
                var query = from message in context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable()
                            join sender in context.Users on message.SenderId equals sender.Id
                            join receipent in context.Users on message.RecipientId equals receipent.Id
                            select new MessageDto
                            {
                                Content = message.Content,
                                DateRead = message.DateRead,
                                MessageSent = message.MessageSent,
                                RecipientId = message.RecipientId,
                                RecipientUserName = message.RecipientUserName,
                                SenderId = message.SenderId,
                                SenderUserName = message.SenderUserName,
                                Id = message.Id,
                                RecipientPhotoUrl = receipent.Photos.FirstOrDefault(x => x.IsMain).Url,
                                SenderPhotoUrl = sender.Photos.FirstOrDefault(x => x.IsMain).Url,
                                RecipientDeleted = message.RecipientDeleted,
                                SenderDeleted = message.SenderDeleted
                            };
                query = messageParams.Container switch
                {
                    "Inbox" => query.Where(u => u.RecipientUserName == messageParams.UserName && u.RecipientDeleted == false),
                    "Outbox" => query.Where(u => u.SenderUserName == messageParams.UserName && u.SenderDeleted == false),
                    _ => query.Where(u => u.RecipientUserName == messageParams.UserName && u.RecipientDeleted == false && u.DateRead == null)
                };

                return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
            }
        }

        public async Task<List<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            using (var context = new ContextDb())
            {
                var messages = from message in context.Messages
                    .Where(x => x.RecipientUserName == currentUserName && x.RecipientDeleted == false && x.SenderUserName == recipientUserName || x.RecipientUserName == recipientUserName && x.SenderUserName == currentUserName && x.SenderDeleted == false)
                    .OrderBy(x => x.MessageSent)
                    .AsQueryable()
                               join sender in context.Users on message.SenderId equals sender.Id
                               join receipent in context.Users on message.RecipientId equals receipent.Id
                               select new MessageDto
                               {
                                   Content = message.Content,
                                   DateRead = message.DateRead,
                                   MessageSent = message.MessageSent,
                                   RecipientId = receipent.Id,
                                   RecipientUserName = receipent.UserName,
                                   SenderId = sender.Id,
                                   SenderUserName = sender.UserName,
                                   Id = message.Id,
                                   RecipientPhotoUrl = receipent.Photos.FirstOrDefault(x => x.IsMain).Url,
                                   SenderPhotoUrl = sender.Photos.FirstOrDefault(x => x.IsMain).Url,
                                   RecipientDeleted = message.RecipientDeleted,
                                   SenderDeleted = message.SenderDeleted

                               };


                var asd = context.Messages;
                var unreadMessages = asd.Where(x => x.DateRead == null && x.RecipientUserName == currentUserName).ToList();




                if (unreadMessages.Any())
                {
                    foreach (var message in unreadMessages)
                    {
                        message.DateRead = DateTime.Now;

                    }
                    await context.SaveChangesAsync();
                }

                return new List<MessageDto>(messages);

            }


        }

        
    }
}
