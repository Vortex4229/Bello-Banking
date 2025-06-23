using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Security.Cryptography;

namespace Bello_Banking_API;

public static class RootMethodsRepo {
	
	public static string PasswordEncryption(string password) {
		var data = Encoding.ASCII.GetBytes(password);
		data = SHA256.Create().ComputeHash(data);
		return Encoding.ASCII.GetString(data);
	}
	
	public static bool Register(MySqlConnection conn, string username, string password, string email, string firstName, string lastName) {
		var registrationQuery =
			"INSERT INTO users(username, password, email, firstname, lastname, balance) values (@username, " +
			"@password, @email, @firstname, @lastname, @balance)";
		var usernameTakenQuery = "SELECT EXISTS (SELECT * FROM users WHERE username = @username) as user_exists";
		
		var registrationCmd = new MySqlCommand(registrationQuery, conn);
		var usernameTakenCmd = new  MySqlCommand(usernameTakenQuery, conn);
		registrationCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
		registrationCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = PasswordEncryption(password);
		registrationCmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
		registrationCmd.Parameters.Add("@firstname", MySqlDbType.VarChar).Value = firstName;
		registrationCmd.Parameters.Add("@lastname", MySqlDbType.VarChar).Value = lastName;
		registrationCmd.Parameters.Add("@balance", MySqlDbType.Int64).Value = 2000;
		usernameTakenCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;

		var usernameTaken = false;
		var success = false;

		try {
			conn.Open();
			
			var usernameTakenReader = usernameTakenCmd.ExecuteReader();
			while (usernameTakenReader.Read())
				usernameTaken = usernameTakenReader.GetInt64(0) == 1;
			usernameTakenReader.Close();

			if (!usernameTaken) {
				registrationCmd.ExecuteNonQuery();
				success = true;
			}
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}
		
		return success;
	}
	
	public static (bool, ulong?) Login(MySqlConnection conn, string username, string password) {
		var accountIdQuery = "SELECT id FROM users WHERE username=@username AND password=@password";
		var loginQuery =
			"SELECT EXISTS (SELECT * FROM users WHERE username=@username AND password=@password) AS user_exists;";

		var loginCmd = new MySqlCommand(loginQuery, conn);
		var accountIdCmd = new MySqlCommand(accountIdQuery, conn);
		loginCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
		loginCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = PasswordEncryption(password);
		accountIdCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
		accountIdCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = PasswordEncryption(password);
		
		var loginSuccess = false;
		ulong? accountId = null;

		try {
			conn.Open();
			
			var loginCmdReader = loginCmd.ExecuteReader();
			while (loginCmdReader.Read())
				loginSuccess = loginCmdReader.GetInt64(0) == 1;
			loginCmdReader.Close();
			
			if (loginSuccess) {
				var accountIdCmdReader = accountIdCmd.ExecuteReader();
				while (accountIdCmdReader.Read())
					accountId = accountIdCmdReader.GetUInt64(0);
				accountIdCmdReader.Close();
			}
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return (loginSuccess, accountId);
	}
}