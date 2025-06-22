using System.Data;
using MySql.Data.MySqlClient;

namespace Bello_Banking_API;

public static class AccountManagementRepo {
	private const string CheckBalanceQuery = "SELECT balance FROM users WHERE id=@id";
	private const string UpdateBalanceQuery = "UPDATE users SET balance=@balance WHERE id=@id";
	
	private static bool PasswordVerification(MySqlConnection conn, ulong? userId, string? password) {
		var verifyPasswordQuery =
			"SELECT EXISTS (SELECT * FROM users WHERE password=@password AND id=@id) AS user_exists;";

		var checkPasswordCmd = new MySqlCommand(verifyPasswordQuery, conn);
		checkPasswordCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = RootMethods.PasswordEncryption(password!);
		checkPasswordCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var passwordVerified = false;

		try {
			conn.Open();
			var checkPasswordCmdReader = checkPasswordCmd.ExecuteReader();

			while (checkPasswordCmdReader.Read()) passwordVerified = checkPasswordCmdReader.GetInt64(0) == 1;

			checkPasswordCmdReader.Close();
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return passwordVerified;
	}

	public static string? GetName(MySqlConnection conn, ulong? userId) {
		var firstNameQuery = "SELECT firstName FROM users WHERE id=@id";
		var lastNameQuery = "SELECT lastName FROM users WHERE id=@id";
		
		var firstNameCmd = new MySqlCommand(firstNameQuery, conn);
		var lastNameCmd = new MySqlCommand(lastNameQuery, conn);

		string? firstName = null;
		string? lastName = null;

		try {
			conn.Open();

			firstNameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;
			lastNameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

			var firstNameCmdReader = firstNameCmd.ExecuteReader();
			while (firstNameCmdReader.Read())
				firstName = firstNameCmdReader.GetString(0);
			firstNameCmdReader.Close();

			var lastNameCmdReader = lastNameCmd.ExecuteReader();
			while (lastNameCmdReader.Read())
				lastName = lastNameCmdReader.GetString(0);
			lastNameCmdReader.Close();
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return firstName + " " + lastName;
	}
	
	public static long? CheckBalance(MySqlConnection conn, ulong? userId) {
		var checkBalanceCmd = new MySqlCommand(CheckBalanceQuery, conn);
		checkBalanceCmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = userId;
		long? balance = null;

		try {
			conn.Open();
			var checkBalanceCmdReader = checkBalanceCmd.ExecuteReader();
			while (checkBalanceCmdReader.Read()) balance = checkBalanceCmdReader.GetInt64(0);
			checkBalanceCmdReader.Close();
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return balance;
	}
	
	public static bool UpdateBalance(MySqlConnection conn, ulong? userId, long? amount, byte type) {
		var updateBalanceCmd = new MySqlCommand(UpdateBalanceQuery, conn);
		updateBalanceCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		long? balance = CheckBalance(conn, userId);
		var success = false;

		try {
			conn.Open();

			switch (type) {
				// 0 = withdrawal/send, 1 = deposit
				case 0:
					balance -= amount;
					if (balance > 0) {
						updateBalanceCmd.Parameters.Add("@balance", MySqlDbType.VarChar).Value = balance;
						updateBalanceCmd.ExecuteNonQuery();
						success = true;
					}

					break;
				case 1:
					balance = balance + amount;
					updateBalanceCmd.Parameters.Add("@balance", MySqlDbType.VarChar).Value = balance;
					updateBalanceCmd.ExecuteNonQuery();
					break;
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
	
	public static bool SendMoney(MySqlConnection conn, string? username, long? amount) {
		var sendMoneyQuery = "UPDATE users SET balance=balance+@amount WHERE username=@username";
		var sendMoneyCmd = new MySqlCommand(sendMoneyQuery, conn);
		sendMoneyCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
		sendMoneyCmd.Parameters.Add("@amount", MySqlDbType.Int64).Value = amount;

		var success = false;

		try {
			conn.Open();
			sendMoneyCmd.ExecuteNonQuery();
			success = true;
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return success;
	}
	
	public static bool ChangeUsername(MySqlConnection conn, ulong? userId, string? newUsername) {
		var changeUsernameQuery = "UPDATE users SET username=@username WHERE id=@id";

		var changeUsernameCmd = new MySqlCommand(changeUsernameQuery, conn);
		changeUsernameCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = newUsername;
		changeUsernameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var success = false;

		try {
			conn.Open();
			changeUsernameCmd.ExecuteNonQuery();
			success = true;
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return success;
	}
	
	public static bool ChangePassword(MySqlConnection conn, ulong? userId, string? oldPassword, string? newPassword) {
		var changePasswordQuery = "UPDATE users SET password=@password WHERE id=@id";

		var changePasswordCmd = new MySqlCommand(changePasswordQuery, conn);
		changePasswordCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = RootMethods.PasswordEncryption(newPassword!);
		changePasswordCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var passwordVerified = PasswordVerification(conn, userId, oldPassword);
		var success = false;

		try {
			conn.Open();

			if (passwordVerified) {
				changePasswordCmd.ExecuteNonQuery();
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
	
	public static bool ChangeName(MySqlConnection conn, ulong? userId, string? newFirstName, string? newLastName) {
		var changeFirstNameQuery = "UPDATE users SET firstname=@firstname where id=@id";
		var changeLastNameQuery = "UPDATE users SET lastname=@lastname where id=@id";

		var changeFirstNameCmd = new MySqlCommand(changeFirstNameQuery, conn);
		changeFirstNameCmd.Parameters.Add("@firstname", MySqlDbType.VarChar).Value = newFirstName;
		changeFirstNameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var changeLastNameCmd = new MySqlCommand(changeLastNameQuery, conn);
		changeLastNameCmd.Parameters.Add("@lastname", MySqlDbType.VarChar).Value = newLastName;
		changeLastNameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var success = false;

		try {
			conn.Open();
			changeFirstNameCmd.ExecuteNonQuery();
			changeLastNameCmd.ExecuteNonQuery();
			success = true;
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return success;
	}
	
	public static bool ChangeEmail(MySqlConnection conn, ulong? userId, string? newEmail) {
		var changeEmailQuery = "UPDATE users SET email=@email where id=@id";

		var changeEmailCmd = new MySqlCommand(changeEmailQuery, conn);
		changeEmailCmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = newEmail;
		changeEmailCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var success = false;
		
		try {
			conn.Open();
			changeEmailCmd.ExecuteNonQuery();
			success = true;
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return success;
	}
	
	public static bool DeleteAccount(MySqlConnection conn, ulong? userId, string? password) {
		var deleteAccountQuery = "DELETE FROM users WHERE id=@id";

		var deleteAccountCmd = new MySqlCommand(deleteAccountQuery, conn);
		deleteAccountCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var passwordVerified = PasswordVerification(conn, userId, password);
		
		var success = false;

		try {
			conn.Open();
			if (passwordVerified) {
				deleteAccountCmd.ExecuteNonQuery();
				success = true;
			}
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Password change failed, returning to login page...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return success;
	}
}