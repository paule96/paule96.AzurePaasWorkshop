using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusDeadMessage
{
    public class Program
    {
        static void SendMessages(string connectionString, string queueName)
        {
            var sender = new MessageSender(connectionString, queueName);

            dynamic data = new[]
            {
                new {name = "Einstein", firstName = "Albert"},
                //new {name = "Heisenberg", firstName = "Werner"},
                //new {name = "Curie", firstName = "Marie"},
                //new {name = "Hawking", firstName = "Steven"},
                //new {name = "Newton", firstName = "Isaac"},
                //new {name = "Bohr", firstName = "Niels"},
                //new {name = "Faraday", firstName = "Michael"},
                //new {name = "Galilei", firstName = "Galileo"},
                //new {name = "Kepler", firstName = "Johannes"},
                //new {name = "Kopernikus", firstName = "Nikolaus"}
            };


            for (int i = 0; i < data.Length; i++)
            {
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data[i])))
                {
                    ContentType = "application/json",
                    Label = "Scientist",
                    MessageId = i.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(2)
                };

                sender.SendAsync(message).Wait();
                lock (Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Message sent: Id = {0}", message.MessageId);
                    Console.ResetColor();
                }
            }
        }

        static void ReceiveMessages(string connectionString, string queueName)
        {
            var receiver = new MessageReceiver(connectionString, queueName, ReceiveMode.PeekLock);
            
            receiver.RegisterMessageHandler(
                async (message, cancellationToken1) =>
                {
                    if (message.Label != null &&
                        message.ContentType != null &&
                        message.Label.Equals("Scientist", StringComparison.InvariantCultureIgnoreCase) &&
                        message.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var body = message.Body;

                        dynamic scientist = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body));

                        lock (Console.Out)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(
                                "\nMessage received: \n\tMessageId = {0}, \n\tSequenceNumber = {1}, \n\tEnqueuedTimeUtc = {2}," +
                                "\n\tExpiresAtUtc = {5}, \n\tContentType = \"{3}\", \n\tSize = {4},  \n\tContent: [ firstName = {6}, name = {7} ]",
                                message.MessageId,
                                message.SystemProperties.SequenceNumber,
                                message.SystemProperties.EnqueuedTimeUtc,
                                message.ContentType,
                                message.Size,
                                message.ExpiresAtUtc,
                                scientist.firstName,
                                scientist.name);
                            Console.ResetColor();
                        }

                        throw new Exception("Something goes wrong! --> Dead Letter");
                        
                        await receiver.CompleteAsync(message.SystemProperties.LockToken);

                        
                    }
                    else
                    {
                        await receiver.DeadLetterAsync(message.SystemProperties.LockToken); //, "ProcessingError", "Don't know what to do with this message");
                    }
                },
                new MessageHandlerOptions((e) => LogMessageHandlerException(e)) { AutoComplete = false, MaxConcurrentCalls = 1 });
        }

        private static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("Exception: \"{0}\"", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Examples: 
        ///     https://github.com/Azure/azure-service-bus/blob/master/samples/DotNet/Microsoft.Azure.ServiceBus/SendersReceiversWithQueues/Program.cs
        ///     https://github.com/Azure/azure-service-bus/blob/master/samples/DotNet/Microsoft.Azure.ServiceBus/DeadletterQueue/Program.cs
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var connectionString =
                "Endpoint=sb://servicebusnamespace-emwfjy.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=g3hT4eGy5zlMkOj9u0+TZk8hborWGp0bw9n6I3AAVok=";
            var queueName = "servicebusnamespace-queue";

            SendMessages(connectionString, queueName);

            ReceiveMessages(connectionString, queueName);
                    

            Console.ReadLine();
        }
    }
}
