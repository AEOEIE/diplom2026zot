using System.Net.Http.Json;
using NewDiplom.Client.DTOs.Shipments;

namespace NewDiplom.Client.Services;

public class ShipmentService
{
    private readonly HttpClient _http;

    public ShipmentService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ShipmentDto>?> GetMyShipmentsAsync()
    {
        return await _http.GetFromJsonAsync<List<ShipmentDto>>(
            "api/Shipments/my");
    }

    public async Task<List<ShipmentDto>?> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<ShipmentDto>>(
            "api/Shipments");
    }

    public async Task<bool> CreateAsync(
        CreateShipmentRequest request)
    {
        var response = await _http.PostAsJsonAsync(
            "api/Shipments",
            request);

        return response.IsSuccessStatusCode;
    }
}