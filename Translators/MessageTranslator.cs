using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Translators
{
    public static class MessageTranslator
    {
        public static MessageDto ToDto(this Message message)
        {
            if (message != null)
            {
                return new MessageDto()
                {
                    UniqueId = message.UniqueId,
                    Source = message.Source,
                    GroupId = message.GroupId,
                    Timestamp = message.CreatedAt,
                    Content = message.Content,
                };
            }

            return null;
        }

        public static Message ToDomain(this MessageDto dto)
        {
            if (dto != null)
            {
                return new Message()
                {
                    UniqueId = dto.UniqueId,
                    GroupId = dto.GroupId,
                    Source = dto.Source,
                    CreatedAt = dto.Timestamp,
                    Content = dto.Content,
                };
            }

            return null;
        }
    }
}
