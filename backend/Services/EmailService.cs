using System.Net;
using System.Net.Mail;
using System.Text;

namespace GongCanApi.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 發送郵件
    /// </summary>
    private async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        try
        {
            // 優先從環境變數讀取（更安全），如果沒有則從配置讀取
            var smtpHost = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST") 
                ?? _configuration["Email:SmtpHost"];
            var smtpPortStr = Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") 
                ?? _configuration["Email:SmtpPort"] ?? "587";
            var smtpPort = int.Parse(smtpPortStr);
            var smtpUsername = Environment.GetEnvironmentVariable("EMAIL_SMTP_USERNAME") 
                ?? _configuration["Email:SmtpUsername"];
            var smtpPassword = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") 
                ?? _configuration["Email:SmtpPassword"];
            var fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM_EMAIL") 
                ?? _configuration["Email:FromEmail"];
            var fromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") 
                ?? _configuration["Email:FromName"] ?? "共餐活動系統";

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
            {
                _logger.LogWarning("郵件設定不完整，跳過發送郵件");
                return;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? smtpUsername, fromName, Encoding.UTF8),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };

            mailMessage.To.Add(new MailAddress(toEmail, toName, Encoding.UTF8));

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation($"郵件已成功發送到 {toEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"發送郵件到 {toEmail} 時發生錯誤");
            // 不拋出異常，避免影響主要業務流程
        }
    }

    /// <summary>
    /// 通知團主活動已滿團
    /// </summary>
    public async Task NotifyHostEventFullAsync(string hostEmail, string hostName, string eventTitle, string eventId, int currentParticipants, int capacity)
    {
        var subject = $"🎉 恭喜！您的共餐活動「{eventTitle}」已額滿";
        var htmlBody = GetEventFullEmailTemplate(hostName, eventTitle, eventId, currentParticipants, capacity);
        await SendEmailAsync(hostEmail, hostName, subject, htmlBody);
    }

    /// <summary>
    /// 通知團主有人取消預約
    /// </summary>
    public async Task NotifyHostCancellationAsync(string hostEmail, string hostName, string eventTitle, string participantName, int cancelledCount, int remainingParticipants, int capacity)
    {
        var subject = $"📢 共餐活動「{eventTitle}」有參與者取消預約";
        var htmlBody = GetHostCancellationEmailTemplate(hostName, eventTitle, participantName, cancelledCount, remainingParticipants, capacity);
        await SendEmailAsync(hostEmail, hostName, subject, htmlBody);
    }

    /// <summary>
    /// 通知參與者有人取消預約（通知其他參與者）
    /// </summary>
    public async Task NotifyParticipantCancellationAsync(string participantEmail, string participantName, string eventTitle, string eventId, int remainingParticipants, int capacity)
    {
        var subject = $"📢 共餐活動「{eventTitle}」有參與者取消預約";
        var htmlBody = GetParticipantCancellationEmailTemplate(participantName, eventTitle, eventId, remainingParticipants, capacity);
        await SendEmailAsync(participantEmail, participantName, subject, htmlBody);
    }

    /// <summary>
    /// 通知取消預約的參與者本人
    /// </summary>
    public async Task NotifySelfCancellationAsync(string participantEmail, string participantName, string eventTitle, string eventId)
    {
        var subject = $"✅ 您已成功取消共餐活動「{eventTitle}」的預約";
        var htmlBody = GetSelfCancellationEmailTemplate(participantName, eventTitle, eventId);
        await SendEmailAsync(participantEmail, participantName, subject, htmlBody);
    }

    /// <summary>
    /// 通知參與者活動已被商家取消
    /// </summary>
    public async Task NotifyEventCancelledByHostAsync(string participantEmail, string participantName, string eventTitle, string eventId, string hostName)
    {
        var subject = $"⚠️ 共餐活動「{eventTitle}」已被取消";
        var htmlBody = GetEventCancelledByHostEmailTemplate(participantName, eventTitle, eventId, hostName);
        await SendEmailAsync(participantEmail, participantName, subject, htmlBody);
    }

    /// <summary>
    /// 通知參與者預約成功
    /// </summary>
    public async Task NotifyParticipationSuccessAsync(string participantEmail, string participantName, string eventTitle, string eventId, DateTime startTime, DateTime endTime, string? fullAddress, int participantCount, int currentParticipants, int capacity)
    {
        var subject = $"🎉 預約成功！共餐活動「{eventTitle}」";
        var htmlBody = GetParticipationSuccessEmailTemplate(participantName, eventTitle, eventId, startTime, endTime, fullAddress, participantCount, currentParticipants, capacity);
        await SendEmailAsync(participantEmail, participantName, subject, htmlBody);
    }

    #region Email Templates

    private string GetEventFullEmailTemplate(string hostName, string eventTitle, string eventId, int currentParticipants, int capacity)
    {
        return $@"
<!DOCTYPE html>
<html lang=""zh-TW"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>活動已額滿</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Microsoft JhengHei', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">🎉 活動已額滿！</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                親愛的 <strong style=""color: #667eea;"">{hostName}</strong>，
                            </p>
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                恭喜您！您的共餐活動已成功額滿！
                            </p>
                            <div style=""background-color: #f8f9fa; border-left: 4px solid #667eea; padding: 20px; margin: 30px 0; border-radius: 5px;"">
                                <p style=""margin: 0 0 10px 0; color: #333333; font-size: 16px; font-weight: bold;"">活動資訊</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動名稱：</strong>{eventTitle}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動 ID：</strong>{eventId}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>參與人數：</strong>{currentParticipants} / {capacity} 人</p>
                            </div>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                您的活動已經達到預設的參與人數上限，系統已自動將活動狀態更新為「已額滿」。
                            </p>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                感謝您使用共餐活動平台，祝活動順利！
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 10px 10px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                此為系統自動發送郵件，請勿直接回覆。
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GetHostCancellationEmailTemplate(string hostName, string eventTitle, string participantName, int cancelledCount, int remainingParticipants, int capacity)
    {
        return $@"
<!DOCTYPE html>
<html lang=""zh-TW"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>參與者取消預約</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Microsoft JhengHei', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">📢 參與者取消預約</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                親愛的 <strong style=""color: #f5576c;"">{hostName}</strong>，
                            </p>
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                您的共餐活動有參與者取消了預約。
                            </p>
                            <div style=""background-color: #fff5f5; border-left: 4px solid #f5576c; padding: 20px; margin: 30px 0; border-radius: 5px;"">
                                <p style=""margin: 0 0 10px 0; color: #333333; font-size: 16px; font-weight: bold;"">活動資訊</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動名稱：</strong>{eventTitle}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>取消預約者：</strong>{participantName}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>取消人數：</strong>{cancelledCount} 人</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>目前參與人數：</strong>{remainingParticipants} / {capacity} 人</p>
                            </div>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                活動目前還有 <strong style=""color: #f5576c;"">{capacity - remainingParticipants}</strong> 個名額可供報名。
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 10px 10px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                此為系統自動發送郵件，請勿直接回覆。
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GetParticipantCancellationEmailTemplate(string participantName, string eventTitle, string eventId, int remainingParticipants, int capacity)
    {
        return $@"
<!DOCTYPE html>
<html lang=""zh-TW"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>活動參與者取消預約</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Microsoft JhengHei', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">📢 活動資訊更新</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                親愛的 <strong style=""color: #4facfe;"">{participantName}</strong>，
                            </p>
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                您預約的共餐活動有參與者取消了預約。
                            </p>
                            <div style=""background-color: #f0f9ff; border-left: 4px solid #4facfe; padding: 20px; margin: 30px 0; border-radius: 5px;"">
                                <p style=""margin: 0 0 10px 0; color: #333333; font-size: 16px; font-weight: bold;"">活動資訊</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動名稱：</strong>{eventTitle}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動 ID：</strong>{eventId}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>目前參與人數：</strong>{remainingParticipants} / {capacity} 人</p>
                            </div>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                活動目前還有 <strong style=""color: #4facfe;"">{capacity - remainingParticipants}</strong> 個名額可供報名。
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 10px 10px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                此為系統自動發送郵件，請勿直接回覆。
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GetSelfCancellationEmailTemplate(string participantName, string eventTitle, string eventId)
    {
        return $@"
<!DOCTYPE html>
<html lang=""zh-TW"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>取消預約確認</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Microsoft JhengHei', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #fa709a 0%, #fee140 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">✅ 取消預約確認</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                親愛的 <strong style=""color: #fa709a;"">{participantName}</strong>，
                            </p>
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                您已成功取消共餐活動的預約。
                            </p>
                            <div style=""background-color: #fff9f0; border-left: 4px solid #fee140; padding: 20px; margin: 30px 0; border-radius: 5px;"">
                                <p style=""margin: 0 0 10px 0; color: #333333; font-size: 16px; font-weight: bold;"">活動資訊</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動名稱：</strong>{eventTitle}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動 ID：</strong>{eventId}</p>
                            </div>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                感謝您使用共餐活動平台，期待下次再為您服務！
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 10px 10px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                此為系統自動發送郵件，請勿直接回覆。
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GetEventCancelledByHostEmailTemplate(string participantName, string eventTitle, string eventId, string hostName)
    {
        return $@"
<!DOCTYPE html>
<html lang=""zh-TW"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>活動已被取消</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Microsoft JhengHei', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">⚠️ 活動已被取消</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                親愛的 <strong style=""color: #ff6b6b;"">{participantName}</strong>，
                            </p>
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                很抱歉通知您，您預約的共餐活動已被商家取消。
                            </p>
                            <div style=""background-color: #fff5f5; border-left: 4px solid #ff6b6b; padding: 20px; margin: 30px 0; border-radius: 5px;"">
                                <p style=""margin: 0 0 10px 0; color: #333333; font-size: 16px; font-weight: bold;"">活動資訊</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動名稱：</strong>{eventTitle}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>活動 ID：</strong>{eventId}</p>
                                <p style=""margin: 5px 0; color: #666666; font-size: 14px;""><strong>主辦者：</strong>{hostName}</p>
                            </div>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                您的預約已自動取消，如有任何疑問，請聯繫活動主辦者。
                            </p>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                感謝您使用共餐活動平台，期待下次再為您服務！
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 10px 10px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                此為系統自動發送郵件，請勿直接回覆。
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GetParticipationSuccessEmailTemplate(string participantName, string eventTitle, string eventId, DateTime startTime, DateTime endTime, string? fullAddress, int participantCount, int currentParticipants, int capacity)
    {
        var startTimeStr = startTime.ToString("yyyy年MM月dd日 HH:mm");
        var endTimeStr = endTime.ToString("yyyy年MM月dd日 HH:mm");
        var addressDisplay = string.IsNullOrWhiteSpace(fullAddress) ? "地址待確認" : fullAddress;
        
        return $@"
<!DOCTYPE html>
<html lang=""zh-TW"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>預約成功</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Microsoft JhengHei', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">🎉 預約成功！</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                親愛的 <strong style=""color: #667eea;"">{participantName}</strong>，
                            </p>
                            <p style=""margin: 0 0 20px 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                恭喜您！您已成功預約共餐活動。
                            </p>
                            <div style=""background-color: #f8f9fa; border-left: 4px solid #667eea; padding: 20px; margin: 30px 0; border-radius: 5px;"">
                                <p style=""margin: 0 0 15px 0; color: #333333; font-size: 18px; font-weight: bold;"">📋 活動資訊</p>
                                <p style=""margin: 8px 0; color: #666666; font-size: 14px;""><strong style=""color: #333333;"">活動名稱：</strong>{eventTitle}</p>
                                <p style=""margin: 8px 0; color: #666666; font-size: 14px;""><strong style=""color: #333333;"">活動 ID：</strong>{eventId}</p>
                                <p style=""margin: 8px 0; color: #666666; font-size: 14px;""><strong style=""color: #333333;"">開始時間：</strong>{startTimeStr}</p>
                                <p style=""margin: 8px 0; color: #666666; font-size: 14px;""><strong style=""color: #333333;"">結束時間：</strong>{endTimeStr}</p>
                                <p style=""margin: 8px 0; color: #666666; font-size: 14px;""><strong style=""color: #333333;"">活動地點：</strong>{addressDisplay}</p>
                                <p style=""margin: 8px 0; color: #666666; font-size: 14px;""><strong style=""color: #333333;"">預約人數：</strong>{participantCount} 人</p>
                            </div>
                            <div style=""background-color: #e8f5e9; border-left: 4px solid #4caf50; padding: 15px; margin: 20px 0; border-radius: 5px;"">
                                <p style=""margin: 0; color: #2e7d32; font-size: 14px;"">
                                    <strong>📊 活動狀態：</strong>目前參與人數 {currentParticipants} / {capacity} 人
                                </p>
                            </div>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                請準時參加活動，期待與您共度美好時光！
                            </p>
                            <p style=""margin: 20px 0 0 0; color: #333333; font-size: 16px; line-height: 1.6;"">
                                如有任何問題，請聯繫活動主辦者。
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 10px 10px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                此為系統自動發送郵件，請勿直接回覆。
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    #endregion
}

