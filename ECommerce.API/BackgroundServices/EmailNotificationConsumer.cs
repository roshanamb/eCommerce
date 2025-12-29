using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ECommerce.API.BackgroundServices;

public class EmailNotificationConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageQueueService _queue;
    private readonly IEmailSenderService _emailSender;

    public EmailNotificationConsumer(IServiceScopeFactory scopeFactory, IMessageQueueService queue, IEmailSenderService emailSender)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _emailSender = emailSender;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _queue.Subscribe<EmailNotificationDto>("email_notifications", async email =>
        {
            using var scope = _scopeFactory.CreateScope();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

            Console.WriteLine($"Sending email to: {email.To}");
            await _emailSender.SendEmailAsync(email);
        });

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }
}