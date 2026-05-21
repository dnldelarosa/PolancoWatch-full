using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolancoWatch.Domain.Entities;
using PolancoWatch.Infrastructure.Data;

namespace PolancoWatch.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private const string SecretMask = "********";
    private readonly ApplicationDbContext _context;

    public SettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("notifications")]
    public async Task<ActionResult<NotificationSettings>> GetNotificationSettings()
    {
        var settings = await _context.NotificationSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new NotificationSettings();
            _context.NotificationSettings.Add(settings);
            await _context.SaveChangesAsync();
        }
        return Ok(MaskSecrets(settings));
    }

    [HttpPut("notifications")]
    public async Task<IActionResult> UpdateNotificationSettings(NotificationSettings settings)
    {
        var existing = await _context.NotificationSettings.FirstOrDefaultAsync();
        if (existing == null)
        {
            if (IsMaskedSecret(settings.TelegramBotToken))
            {
                settings.TelegramBotToken = null;
            }
            if (IsMaskedSecret(settings.SmtpPass))
            {
                settings.SmtpPass = null;
            }
            _context.NotificationSettings.Add(settings);
        }
        else
        {
            existing.TelegramEnabled = settings.TelegramEnabled;
            if (!IsMaskedSecret(settings.TelegramBotToken))
            {
                existing.TelegramBotToken = settings.TelegramBotToken;
            }
            existing.TelegramChatId = settings.TelegramChatId;
            existing.EmailEnabled = settings.EmailEnabled;
            existing.SmtpHost = settings.SmtpHost;
            existing.SmtpPort = settings.SmtpPort;
            existing.SmtpEnableSsl = settings.SmtpEnableSsl;
            existing.SmtpUser = settings.SmtpUser;
            if (!IsMaskedSecret(settings.SmtpPass))
            {
                existing.SmtpPass = settings.SmtpPass;
            }
            existing.TelegramMessageTemplate = settings.TelegramMessageTemplate;
            existing.EmailMessageTemplate = settings.EmailMessageTemplate;
            existing.FromEmail = settings.FromEmail;
            existing.ToEmail = settings.ToEmail;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static NotificationSettings MaskSecrets(NotificationSettings settings)
    {
        return new NotificationSettings
        {
            Id = settings.Id,
            EmailEnabled = settings.EmailEnabled,
            SmtpHost = settings.SmtpHost,
            SmtpPort = settings.SmtpPort,
            SmtpUser = settings.SmtpUser,
            SmtpPass = string.IsNullOrWhiteSpace(settings.SmtpPass) ? null : SecretMask,
            SmtpEnableSsl = settings.SmtpEnableSsl,
            FromEmail = settings.FromEmail,
            ToEmail = settings.ToEmail,
            EmailMessageTemplate = settings.EmailMessageTemplate,
            TelegramEnabled = settings.TelegramEnabled,
            TelegramBotToken = string.IsNullOrWhiteSpace(settings.TelegramBotToken) ? null : SecretMask,
            TelegramChatId = settings.TelegramChatId,
            TelegramMessageTemplate = settings.TelegramMessageTemplate
        };
    }

    private static bool IsMaskedSecret(string? value)
    {
        return string.Equals(value, SecretMask, StringComparison.Ordinal);
    }
}
