namespace NHT_Marine_BE.Services
{
    public class AppBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AppBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // await HandleUnhandledOrders();

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = DateTime.Today.AddDays(1).AddMinutes(1);
                var delay = nextRun - now;

                await Task.Delay(delay, stoppingToken);
                // await HandleUnhandledOrders();
            }
        }
    }
}
