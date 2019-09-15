using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatlerWaldorfCorp.EventProcessor.Queues
{
    public class QueueOptions
    {
        public string ProximityDetectedEventQueueName { get; set; }

        public string MemberLocationRecordedEventQueueName { get; set; }
    }
}
