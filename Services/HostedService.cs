using System;
using System.Threading;
using System.Threading.Tasks;
using Godwit.Common.Data;
using Godwit.Common.Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Godwit.HandleLoginAction.Services {
    public class HostedService : IHostedService {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public HostedService(IServiceProvider serviceProvider, ILogger<HostedService> logger) {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await DoWork(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        private async Task DoWork(CancellationToken stoppingToken) {
            using var scope = _serviceProvider.CreateScope();
            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<KetoDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await dbContext.Database.EnsureCreatedAsync(stoppingToken);
            _logger.LogInformation(
                "Database is created!");

            if (await userManager.FindByNameAsync("hamza") == null)
                await userManager.CreateAsync(new User("hamza") {
                    Email = "hamza.althunibat@gmail.com",
                    BirthDate = new LocalDate(1981, 7, 10),
                    FirstName = "Hamza",
                    LastName = "Althunibat",
                    CreatedOn = Instant.FromDateTimeOffset(DateTimeOffset.Now),
                    PhoneNumber = "0568966469",
                    Gender = Gender.Male
                }, "1qaz!QAZ");
        }
    }
}