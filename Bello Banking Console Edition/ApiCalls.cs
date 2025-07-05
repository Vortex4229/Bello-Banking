namespace Bello_Banking_Console_Edition;

public static class ApiCalls {
    public static async Task<bool> RegisterCall(string username, string password, string email, string firstName,
        string lastName) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/register/{username}/{password}/{email}/{firstName}/{lastName}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<ulong?> LoginCall(string username, string password) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/login/{username}/{password}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        var data = await response.Content.ReadAsStringAsync();

        return data == "" ? null : Convert.ToUInt64(data);
    }

    public static async Task<string> GetName(ulong userId) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/getName/{userId}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<long?> CheckBalance(ulong userId) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/checkbalance/{userId}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));
        
        var data = await response.Content.ReadAsStringAsync();

        return data == "" ? null : Convert.ToInt64(data);
    }

    public static async Task<bool> UpdateBalance(ulong userId, long amount, byte type) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/updateBalance/{userId}/{amount}/{type}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<bool> SendMoney(string username, long amount) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/sendMoney/{username}/{amount}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<bool> ChangeUsername(ulong userId, string newUsername) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/changeUsername/{userId}/{newUsername}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<bool> ChangePassword(ulong userId, string oldPassword, string newPassword) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/changePassword/{userId}/{oldPassword}/{newPassword}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<bool> ChangeName(ulong userId, string newFirstName, string newLastName) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/changeName/{userId}/{newFirstName}/{newLastName}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<bool> ChangeEmail(ulong userId, string newEmail) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/changeEmail/{userId}/{newEmail}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));
        
        return await response.Content.ReadAsStringAsync() == "True";
    }

    public static async Task<bool> DeleteAccount(ulong userId, string password) {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://bello-banking-api-dhhmb5fhf4bgdfa7.westcentralus-01.azurewebsites.net/");

        var call = $"api/deleteAccount/{userId}/{password}";
        var response =
            await client.GetAsync(new Uri(call, UriKind.Relative));

        return await response.Content.ReadAsStringAsync() == "True";
    }
}