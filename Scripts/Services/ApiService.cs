using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Temptica.TwitchBot.Shared.ResponseModels;

namespace Temptic404Overlay.Scripts.Services;

public class ApiService
{
	private readonly HttpClient _client = new(){BaseAddress = new Uri("https://localhost:7299/api/") };

	public async Task<string> GetLastSub()
	{
		var response = await _client.GetAsync($"Alert/LastSubAlert");
		
		return response.IsSuccessStatusCode ? (await response.Content.ReadFromJsonAsync<AlertResponseModel>()).Username : "Be the first!";
	}
	
	public async Task<string> GetLastCheer()
	{
		var response = await _client.GetAsync($"Alert/LastCheerAlert");
		if (!response.IsSuccessStatusCode) return "Be the first!";
		
		var alert = await response.Content.ReadFromJsonAsync<AlertResponseModel>();
		return $"{alert.Username} ({alert.Amount} bits)";

	}

	public async Task<string> GetFirstViewer()
	{
		var response = await _client.GetAsync($"Stream/GetFirstViewer");
		
		return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "Be the first!";
	}
}
