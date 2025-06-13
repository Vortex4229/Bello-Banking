using System.Data;
using MySql.Data.MySqlClient;

namespace Bello_Banking_Console_Edition;

public static class AccountManagement {
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
	
	public static void CheckBalance(MySqlConnection conn, ulong? userId) {
		var checkBalanceCmd = new MySqlCommand(CheckBalanceQuery, conn);
		checkBalanceCmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = userId;
		long? balance = null;

		try {
			conn.Open();
			var checkBalanceCmdReader = checkBalanceCmd.ExecuteReader();
			while (checkBalanceCmdReader.Read()) balance = checkBalanceCmdReader.GetInt64(0);
			checkBalanceCmdReader.Close();

			Console.WriteLine($"Balance: ${balance}");
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Unable to check balance, returning to account page...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}
	}
	
	public static bool UpdateBalance(MySqlConnection conn, ulong? userId, long? amount, byte type) {
		var checkBalanceCmd = new MySqlCommand(CheckBalanceQuery, conn);
		checkBalanceCmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = userId;

		var updateBalanceCmd = new MySqlCommand(UpdateBalanceQuery, conn);
		updateBalanceCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		long? balance = null;
		var success = true;

		try {
			conn.Open();
			var checkBalanceCmdReader = checkBalanceCmd.ExecuteReader();
			while (checkBalanceCmdReader.Read()) balance = checkBalanceCmdReader.GetInt64(0);

			checkBalanceCmdReader.Close();

			switch (type) {
				// 0 = withdrawal/send, 1 = deposit
				case 0:
					balance -= amount;
					if (balance < 0) {
						success = false;
						Console.WriteLine("Account balance would be negative, returning to account page...");
					}
					else {
						updateBalanceCmd.Parameters.Add("@balance", MySqlDbType.VarChar).Value = balance;
						updateBalanceCmd.ExecuteNonQuery();
					}

					break;
				case 1:
					balance = balance + amount;
					updateBalanceCmd.Parameters.Add("@balance", MySqlDbType.VarChar).Value = balance;
					updateBalanceCmd.ExecuteNonQuery();
					break;
			}

			if (success)
				Console.WriteLine($"New Account Balance: ${balance}");
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Unable to update balance, returning to account page...");
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

		var success = true;

		try {
			conn.Open();
			sendMoneyCmd.ExecuteNonQuery();
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			success = false;
			Console.WriteLine("Unable to update balance, returning to account page...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}

		return success;
	}
	
	public static void ChangeUsername(MySqlConnection conn, ulong? userId, string? newUsername) {
		var changeUsernameQuery = "UPDATE users SET username=@username WHERE id=@id";

		var changeUsernameCmd = new MySqlCommand(changeUsernameQuery, conn);
		changeUsernameCmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = newUsername;
		changeUsernameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		try {
			conn.Open();
			changeUsernameCmd.ExecuteNonQuery();
			Console.WriteLine("Username updated successfully, returning to account page...");
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Username change failed, returning to account page...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}
	}
	
	public static void ChangePassword(MySqlConnection conn, ulong? userId, string? oldPassword, string? newPassword) {
		var changePasswordQuery = "UPDATE users SET password=@password WHERE id=@id";

		var changePasswordCmd = new MySqlCommand(changePasswordQuery, conn);
		changePasswordCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = RootMethods.PasswordEncryption(newPassword!);
		changePasswordCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var passwordVerified = PasswordVerification(conn, userId, oldPassword);

		try {
			conn.Open();

			if (passwordVerified) {
				changePasswordCmd.ExecuteNonQuery();
				Console.WriteLine("Password changed successfully, returning to account page...");
			}
			else {
				Console.WriteLine("Password change failed, returning to account page...");
			}
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Password change failed, returning to account page...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}
	}
	
	public static void ChangeName(MySqlConnection conn, ulong? userId, string? newFirstName, string? newLastName) {
		var changeFirstNameQuery = "UPDATE users SET firstname=@firstname where id=@id";
		var changeLastNameQuery = "UPDATE users SET lastname=@lastname where id=@id";

		var changeFirstNameCmd = new MySqlCommand(changeFirstNameQuery, conn);
		changeFirstNameCmd.Parameters.Add("@firstname", MySqlDbType.VarChar).Value = newFirstName;
		changeFirstNameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var changeLastNameCmd = new MySqlCommand(changeLastNameQuery, conn);
		changeLastNameCmd.Parameters.Add("@lastname", MySqlDbType.VarChar).Value = newLastName;
		changeLastNameCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		try {
			conn.Open();
			changeFirstNameCmd.ExecuteNonQuery();
			changeLastNameCmd.ExecuteNonQuery();
			Console.WriteLine("Name updated successfully, returning to account page...");
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Name change failed, returning to account page...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}
	}
	
	public static void ChangeEmail(MySqlConnection conn, ulong? userId, string? newEmail) {
		var changeEmailQuery = "UPDATE users SET email=@email where id=@id";

		var changeEmailCmd = new MySqlCommand(changeEmailQuery, conn);
		changeEmailCmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = newEmail;
		changeEmailCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		try {
			conn.Open();
			changeEmailCmd.ExecuteNonQuery();
			Console.WriteLine("Email updated successfully, returning to account page...");
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Email change failed, returning to account page...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}
	}
	
	public static void DeleteAccount(MySqlConnection conn, ulong? userId, string? password) {
		var deleteAccountQuery = "DELETE FROM users WHERE id=@id";

		var deleteAccountCmd = new MySqlCommand(deleteAccountQuery, conn);
		deleteAccountCmd.Parameters.Add("@id", MySqlDbType.Int64).Value = userId;

		var passwordVerified = PasswordVerification(conn, userId, password);

		try {
			conn.Open();
			if (passwordVerified) {
				deleteAccountCmd.ExecuteNonQuery();
				Console.WriteLine("Account deleted successfully, returning to login page...");
			}
			else {
				Console.WriteLine("Password change failed, returning to login page...");
			}
		}
		catch (MySqlException e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Password change failed, returning to login page...");
		}
		finally {
			if (conn.State == ConnectionState.Open) conn.Close();
		}
	}
}