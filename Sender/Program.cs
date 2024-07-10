using EasyNetQ;

namespace Sender
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            using var bus = RabbitHutch.CreateBus("host=localhost");
            await bus.Advanced.ExchangeDeclareAsync("chat", "fanout");
            Console.WriteLine("Sender");

            while (true)
            {
                var sendTask = SendMessage(bus);
                var receiveTask = ReceiveMessage(bus);

                await Task.WhenAll(sendTask, receiveTask);
               
            }
        }

        static async Task SendMessage(IBus bus)
        {
            var mess = Console.ReadLine();

            if (mess == "exit")
                return;

            var message = new ChatMessage { Message = mess, Sender = "UserName1" };
            await bus.PubSub.PublishAsync(message, "chat");

        }

        static async Task ReceiveMessage(IBus bus)
        {
            await bus.PubSub.SubscribeAsync<ChatMessage>("sender_queue", async (receivedMessage) =>
            {
                Console.WriteLine($"Отправитель: {receivedMessage.Sender}");
                Console.WriteLine($"Получено сообщение: {receivedMessage.Message}");
            });
        }
    }
}
