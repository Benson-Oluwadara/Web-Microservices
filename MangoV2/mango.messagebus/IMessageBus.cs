using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mango.messagebus
{
    public interface IMessageBus
    {
        Task PublicMessage(object message, string topic_queue_name);
    }
}
