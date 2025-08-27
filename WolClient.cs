using System;
using System.Net.Http;
using System.Threading.Tasks;

public class WolClient
{
    private readonly HttpClient _client;
    private readonly string _serverUrl;

    public WolClient(string serverUrl)
    {
        _client = new HttpClient();
        _serverUrl = serverUrl.TrimEnd('/');
    }

    public async Task<string> WakeDevice(string deviceId)
    {
        try
        {
            var response = await _client.GetAsync($"{_serverUrl}/wake/{deviceId}");
            response.EnsureSuccessStatusCode();
            string html = await response.Content.ReadAsStringAsync();

            var start = html.IndexOf("<h2>") + 4;
            var end = html.IndexOf("</h2>");
            if (start >= 0 && end > start)
            {
                return html.Substring(start, end - start);
            }
            return "Sent, but could not parse server response.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
