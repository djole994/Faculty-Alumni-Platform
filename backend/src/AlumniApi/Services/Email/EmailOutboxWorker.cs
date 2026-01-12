using AlumniApi.Models;
using AlumniApi.Models.Email;
using AlumniApi.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace AlumniApi.Services.Email;

public sealed class EmailOutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailOutboxWorker> _logger;

    public EmailOutboxWorker(IServiceScopeFactory scopeFactory, ILogger<EmailOutboxWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AlumniContext>();
                var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                var now = DateTime.UtcNow;

                var batch = await db.EmailOutboxes
                    .Where(x => x.Status == EmailOutboxStatus.Pending
                                && (x.NextAttemptAtUtc == null || x.NextAttemptAtUtc <= now))
                    .OrderBy(x => x.CreatedAtUtc)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var x in batch)
                {
                    // mark as Sending
                    x.Status = EmailOutboxStatus.Sending;
                    x.LastAttemptAtUtc = now;
                    await db.SaveChangesAsync(stoppingToken);

                    try
                    {
                        await sender.SendAsync(new EmailMessage(x.To, x.Subject, x.HtmlBody, x.TextBody), stoppingToken);

                        x.Status = EmailOutboxStatus.Sent;
                        x.SentAtUtc = DateTime.UtcNow;
                        x.LastError = null;
                        x.NextAttemptAtUtc = null;
                    }
                    catch (Exception ex)
                    {
                        x.FailureCount++;
                        x.LastError = ex.Message;

                        var nextDelay = GetNextDelay(x.FailureCount);

                        if (nextDelay is null)
                        {
                            x.Status = EmailOutboxStatus.Failed; // završeno
                            x.NextAttemptAtUtc = null;
                        }
                        else
                        {
                            x.Status = EmailOutboxStatus.Pending; // retry kasnije
                            x.NextAttemptAtUtc = DateTime.UtcNow.Add(nextDelay.Value);
                        }

                        _logger.LogWarning(ex, "Email send failed. outboxId={Id} failures={FailCount}", x.Id, x.FailureCount);
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmailOutboxWorker loop error");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private static TimeSpan? GetNextDelay(int failureCount) => failureCount switch
    {
        1 => TimeSpan.FromMinutes(5),
        2 => TimeSpan.FromHours(2),
        3 => TimeSpan.FromHours(12),
        _ => null // posle 3 fail-a (ukupno 4 pokušaja: odmah + 3 retry) -> Failed
    };
}
