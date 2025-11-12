namespace OrderSaga.Worker.Settings
{
    public class RabbitMqSettings
    {
        public string Host { get; set; } = "localhost"; // nếu dev
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int Port { get; set; } = 5672;
    }
}
