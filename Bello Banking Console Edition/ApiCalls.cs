namespace Bello_Banking_Console_Edition;

public static class ApiCalls {

    private static (HttpClient, HttpClientHandler) SecurityBypassClient() { // allows for access to the local host API, DELETE LATER
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = 
            (sender, cert, chain, sslPolicyErrors) => true;
        return (new HttpClient(clientHandler),  clientHandler);
    }
    
    public static async Task<bool> RegisterCall(string username, string password, string email, string firstName, string lastName) {
        var (client, clientHandler) = SecurityBypassClient();
        client.BaseAddress = new Uri("https://localhost:7016/");

        string call = $"api/register/{username}/{password}/{email}/{firstName}/{lastName}";
        HttpResponseMessage response = 
            await client.GetAsync(new Uri(call, UriKind.Relative));

        client.Dispose();
        clientHandler.Dispose();

        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<ulong?> LoginCall(string username, string password) {
        var (client, clientHandler) = SecurityBypassClient();
        client.BaseAddress = new Uri("https://localhost:7016/");
        
        string call = $"api/login/{username}/{password}";
        HttpResponseMessage response =
            await client.GetAsync(new Uri(call, UriKind.Relative));
        
        client.Dispose();
        clientHandler.Dispose();

        string data = await response.Content.ReadAsStringAsync();
        
        return data == "" ? null : Convert.ToUInt64(data);
    }

    public static async Task<string> GetName(ulong userId) {
        var (client, clientHandler) = SecurityBypassClient();
        client.BaseAddress = new Uri("https://localhost:7016/");
        
        string call = $"api/getName/{userId}";
        HttpResponseMessage response =
            await client.GetAsync(new Uri(call, UriKind.Relative));
        
        client.Dispose();
        clientHandler.Dispose();
        
        return await response.Content.ReadAsStringAsync();
    }
    
    public static async Task<long?> CheckBalance(ulong userId) {
        var (client, clientHandler) = SecurityBypassClient();
        client.BaseAddress = new Uri("https://localhost:7016/");
        
        string call = $"api/checkbalance/{userId}";
        HttpResponseMessage response = 
            await client.GetAsync(new Uri(call, UriKind.Relative));
        
        client.Dispose();
        clientHandler.Dispose();
        
        string data = await response.Content.ReadAsStringAsync();
        
        return data == "" ? null : Convert.ToInt64(data);
    }
    
    public static async Task<bool> UpdateBalance(ulong userId, long amount, byte type) {
        var (client, clientHandler) = SecurityBypassClient();
        client.BaseAddress = new Uri("https://localhost:7016/");

        string call = $"api/updateBalance/{userId}/{amount}/{type}";
        HttpResponseMessage response =
            await client.GetAsync(new Uri(call, UriKind.Relative));
        
        client.Dispose();
        clientHandler.Dispose();
        
        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<bool> SendMoney(string username, long amount) {
        var (client, clientHandler) = SecurityBypassClient();
        client.BaseAddress = new Uri("https://localhost:7016/");
        
        string call = $"api/sendMoney/{username}/{amount}";
        HttpResponseMessage response =
            await client.GetAsync(new Uri(call, UriKind.Relative));
        
        client.Dispose();
        clientHandler.Dispose();

        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<bool> ChangeUsername(ulong userId, string username) {
        var (client, clientHandler) = SecurityBypassClient();
        client.BaseAddress = new Uri("https://localhost:7016/");

        string call = $"api/changeUsername/{userId}/{username}";
        HttpResponseMessage response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        client.Dispose();
        clientHandler.Dispose();

        return await response.Content.ReadAsStringAsync() == "True";
    }



}