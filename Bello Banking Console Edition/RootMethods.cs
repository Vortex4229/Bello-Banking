using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Security.Cryptography;

namespace Bello_Banking_Console_Edition;

public static class RootMethods {
	
	public static string PasswordEncryption(string password) {
		var data = Encoding.ASCII.GetBytes(password);
		data = SHA256.Create().ComputeHash(data);
		return Encoding.ASCII.GetString(data);
	}
	
	public static void Register(MySqlConnection conn, string username, string password, string email, string firstName, string lastName) {
		var registrationQuery =
			"INSERT INTO users(username, password, email, firstname, lastname, balance) values (@username, " +
			"@password, @email, @firstname, @lastname, @balance)";
		var registrationCmd = new MySqlCommand(registrationQuery, conn);

		try {
			registrationCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
			registrationCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = PasswordEncryption(password);
			registrationCmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
			registrationCmd.Parameters.Add("@firstname", MySqlDbType.VarChar).Value = firstName;
			registrationCmd.Parameters.Add("@lastname", MySqlDbType.VarChar).Value = lastName;
			registrationCmd.Parameters.Add("@balance", MySqlDbType.Int64).Value = 2000;
			conn.Open();
			registrationCmd.ExecuteNonQuery();
			Console.WriteLine("Registration complete! Returning to main menu...");
			Console.ReadLine();
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Registration failed, returning to main menu...");
			Console.ReadLine();
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}
	}
	
	public static (bool, ulong?) Login(MySqlConnection conn, string username, string password) {
		var firstNameQuery = "SELECT firstName FROM users WHERE id=@id";
		var lastNameQuery = "SELECT lastName FROM users WHERE id=@id";
		var accountIdQuery = "SELECT id FROM users WHERE username=@username AND password=@password";
		var loginQuery =
			"SELECT EXISTS (SELECT * FROM users WHERE username=@username AND password=@password) AS user_exists;";
		
		var loginSuccess = false;
		string? firstName = null;
		string? lastName = null;
		ulong? accountId = null;

		var loginCmd = new MySqlCommand(loginQuery, conn);
		var firstNameCmd = new MySqlCommand(firstNameQuery, conn);
		var lastNameCmd = new MySqlCommand(lastNameQuery, conn);
		var accountIdCmd = new MySqlCommand(accountIdQuery, conn);
		loginCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
		loginCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = PasswordEncryption(password);
		accountIdCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
		accountIdCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = PasswordEncryption(password);

		try {
			conn.Open();
			var loginCmdReader = loginCmd.ExecuteReader();

			while (loginCmdReader.Read())
				loginSuccess = loginCmdReader.GetInt64(0) == 1;
			loginCmdReader.Close();

			if (loginSuccess == false) {
				if (conn.State == ConnectionState.Open) conn.Close();
				Console.WriteLine("Login failed, invalid username or password, returning to main menu...");
				Console.ReadLine();
			}
			else {
				var accountIdCmdReader = accountIdCmd.ExecuteReader();
				while (accountIdCmdReader.Read())
					accountId = accountIdCmdReader.GetUInt64(0);
				accountIdCmdReader.Close();

				firstNameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = accountId;
				lastNameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = accountId;

				var firstNameCmdReader = firstNameCmd.ExecuteReader();
				while (firstNameCmdReader.Read())
					firstName = firstNameCmdReader.GetString(0);
				firstNameCmdReader.Close();

				var lastNameCmdReader = lastNameCmd.ExecuteReader();
				while (lastNameCmdReader.Read())
					lastName = lastNameCmdReader.GetString(0);
				lastNameCmdReader.Close();

				Console.WriteLine($"Login successful! Welcome {firstName} {lastName}!");
				Console.ReadLine();
			}
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Login failed, returning to main menu...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return (loginSuccess, accountId);
	}
}