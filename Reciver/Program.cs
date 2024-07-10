using EasyNetQ;
using Sender;

namespace Reciver
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            await bus.Advanced.ExchangeDeclareAsync("chat", "fanout");
   
            Console.WriteLine("Reciver");
            while (true) 
            {
                var sendTask = SendMessage(bus);
                var reciveTask =  ReceiveMessage(bus);

                await Task.WhenAll(sendTask, reciveTask);
            }
            bus.Dispose();
        }
        static async Task SendMessage(IBus bus)
        {
            var mess = Console.ReadLine();

            if (mess == "exit")
                return;

            var message = new ChatMessage { Message = mess, Sender = "UserName2" };
            await bus.PubSub.PublishAsync(message, "chat");

        }
        static async Task ReceiveMessage(IBus bus)
        {
            await bus.PubSub.SubscribeAsync<ChatMessage>("reciver_queue", async (receivedMessage) =>
            {
                Console.WriteLine($"Отправитель: {receivedMessage.Sender}");
                Console.WriteLine($"Получено сообщение: {receivedMessage.Message}");
            });
        }

    }
}

