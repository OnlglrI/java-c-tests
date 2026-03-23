using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using S5_desk.Models;

namespace S5_desk.Services;

public class ApiService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string BaseUrl = "http://localhost:8080";

    private void AddAuthHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        if (!string.IsNullOrEmpty(AppState.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AppState.Token);
        }
    }

    private T? PostJson<T>(string path, object body) where T : class
    {
        try
        {
            var json = JsonConvert.SerializeObject(body);
            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + path);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            AddAuthHeader();
            var response = _httpClient.Send(request);
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch
        {
            return null;
        }
    }

    private T? GetJson<T>(string path) where T : class
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + path);
            AddAuthHeader();
            var response = _httpClient.Send(request);
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch
        {
            return null;
        }
    }

    private T? PutJson<T>(string path, object body) where T : class
    {
        try
        {
            var json = JsonConvert.SerializeObject(body);
            var request = new HttpRequestMessage(HttpMethod.Put, BaseUrl + path);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            AddAuthHeader();
            var response = _httpClient.Send(request);
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch
        {
            return null;
        }
    }

    private T? PatchJson<T>(string path, object? body = null) where T : class
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, BaseUrl + path);
            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            AddAuthHeader();
            var response = _httpClient.Send(request);
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch
        {
            return null;
        }
    }

    private bool DeleteRequest(string path)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, BaseUrl + path);
            AddAuthHeader();
            var response = _httpClient.Send(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public TokenResponse? Login(string username, string password)
    {
        return PostJson<TokenResponse>("/api/auth/login", new { username, password });
    }

    public TokenResponse? Register(string username, string email, string password)
    {
        return PostJson<TokenResponse>("/api/auth/register", new { username, email, password });
    }

    public List<Product> GetProducts()
    {
        return GetJson<List<Product>>("/api/products") ?? new List<Product>();
    }

    public Product? CreateProduct(Product p)
    {
        return PostJson<Product>("/api/products", new { p.Name, p.Description, p.Price, p.Stock });
    }

    public Product? UpdateProduct(Product p)
    {
        return PutJson<Product>($"/api/products/{p.Id}", new { p.Name, p.Description, p.Price, p.Stock });
    }

    public bool DeleteProduct(int id)
    {
        return DeleteRequest($"/api/products/{id}");
    }

    public List<Order> GetOrders()
    {
        return GetJson<List<Order>>("/api/orders") ?? new List<Order>();
    }

    public Order? CreateOrder(int productId, int quantity)
    {
        return PostJson<Order>("/api/orders", new { productId, quantity });
    }

    public bool DeleteOrder(int id)
    {
        return DeleteRequest($"/api/orders/{id}");
    }

    // Admin methods
    public List<User> GetAdminUsers()
    {
        return GetJson<List<User>>("/api/admin/users") ?? new List<User>();
    }

    public bool ToggleUserActive(int id)
    {
        var result = PatchJson<User>($"/api/admin/users/{id}/activate");
        return result != null;
    }

    public List<Order> GetAllOrders()
    {
        return GetJson<List<Order>>("/api/admin/orders") ?? new List<Order>();
    }

    public bool UpdateOrderStatus(int id, string status)
    {
        var result = PatchJson<Order>($"/api/admin/orders/{id}/status", new { status });
        return result != null;
    }

    public Dictionary<string, object> GetStats()
    {
        return GetJson<Dictionary<string, object>>("/api/stats/summary") ?? new Dictionary<string, object>();
    }

    public List<Dictionary<string, string>> GetTopProducts()
    {
        return GetJson<List<Dictionary<string, string>>>("/api/stats/top-products") ?? new List<Dictionary<string, string>>();
    }
}
