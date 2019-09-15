using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using RabbitMQ.Client;
using StatlerWaldorfCorp.EventProcessor.Events;
using StatlerWaldorfCorp.EventProcessor.Queues;
using StatlerWaldorfCorp.EventProcessor.Queues.AMQP;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace StatlerWaldorfCorp.EventProcessor.Tests.Queues.AMQP
{
    public class AMQPEventEmitterTest
    {
        [Fact]
        public void AmqpEmitterUsesBasicPublish()
        {
            var logger = new Mock<ILogger<AMQPEventEmitter>>();
            var queueOptions = new QueueOptions
            {
                ProximityDetectedEventQueueName = "proximitydetected",
                MemberLocationRecordedEventQueueName = "memberlocationrecorded"
            };
            var queueOptionsWrapper = new Mock<IOptions<QueueOptions>>();
            queueOptionsWrapper.Setup(o => o.Value).Returns(queueOptions);

            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            var model = new Mock<IModel>();

            // Ensure that we call basic publish on the appropriate queue name (routing key)
            model.Setup(m => m.BasicPublish(It.IsAny<string>(),
                It.Is<string>(rk => rk == queueOptions.ProximityDetectedEventQueueName),
                It.IsAny<bool>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<byte[]>()));
            connection.Setup(c => c.CreateModel()).Returns(model.Object);
            factory.Setup(f => f.CreateConnection()).Returns(connection.Object);

            var emitter = new AMQPEventEmitter(logger.Object, queueOptionsWrapper.Object, factory.Object);

            var evt = new ProximityDetectedEvent
            {
                SourceMemberID = Guid.NewGuid(),
                TargetMemberID = Guid.NewGuid()
            };
            emitter.EmitProximityDetectedEvent(evt);

            model.VerifyAll();
        }
    }

}
