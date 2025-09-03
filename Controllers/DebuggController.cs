// Put this in a suitable place (e.g., a DebugController or a minimal API endpoint)
// TODO : Do NOT leave this enabled in production long-term.

using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;
using MailKit.Security;
using MailKit.Net.Smtp;


[Route("debug/smtp")]
public class SmtpDebugController : ControllerBase
{
    [HttpGet("connect")]
    public async Task<IActionResult> Connect()
    {
        string host = Environment.GetEnvironmentVariable("MailHost");
        int port = int.Parse(Environment.GetEnvironmentVariable("MailPort") ?? "587");

        if (string.IsNullOrWhiteSpace(host))
            return BadRequest("MailHost is not set.");

        try
        {
            // 5s timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var client = new TcpClient();
            await client.ConnectAsync(host, port, cts.Token);

            var addresses = await Dns.GetHostAddressesAsync(host);
            return Ok(new
            {
                Host = host,
                Port = port,
                ResolvedIPs = addresses.Select(a => a.ToString()).ToArray(),
                Connected = client.Connected
            });
        }
        catch (SocketException ex)
        {
            return StatusCode(502, $"TCP connect failed: {ex.SocketErrorCode} - {ex.Message}");
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "TCP connect timed out (5s).");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("handshake")]
    public async Task<IActionResult> Handshake()
    {
        string host = Environment.GetEnvironmentVariable("MailHost");
        string user = Environment.GetEnvironmentVariable("Mail");
        string pass = Environment.GetEnvironmentVariable("MailPassword");
        int port = int.Parse(Environment.GetEnvironmentVariable("MailPort") ?? "587");

        var tried = new List<string>();

        async Task<(bool ok, string msg)> TryConnect(int p, SecureSocketOptions opt)
        {
            tried.Add($"host={host} port={p} option={opt}");
            using var client = new SmtpClient();
            client.Timeout = 5000; // 5 seconds
            try
            {
                await client.ConnectAsync(host, p, opt);
                if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(pass))
                {
                    await client.AuthenticateAsync(user, pass);
                }
                await client.DisconnectAsync(true);
                return (true, $"Handshake OK on port {p} using {opt}.");
            }
            catch (Exception ex)
            {
                return (false, $"{opt} on port {p} failed: {ex.GetType().Name} - {ex.Message}");
            }
        }

        // Respect configured port first, then try the other common one
        var attempts = new List<(int, SecureSocketOptions)>
        {
            (port, port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls),
            (587, SecureSocketOptions.StartTls),
            (465, SecureSocketOptions.SslOnConnect)
        }.Distinct().ToList();

        var results = new List<string>();
        foreach (var (p, opt) in attempts)
        {
            var (ok, msg) = await TryConnect(p, opt);
            results.Add(msg);
            if (ok) return Ok(new { Tried = tried, Result = msg });
        }

        return StatusCode(502, new { Tried = tried, Results = results });
    }
}
