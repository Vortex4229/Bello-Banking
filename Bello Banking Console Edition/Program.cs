namespace Bello_Banking_Console_Edition;

// TODO: Refactor program to call from the Bello Banking API once developed.

internal class Program {
	private static async Task Main() {
		while (true) {
			Console.Clear();
			Console.WriteLine("--Bello Banking Console Edition--");
			Console.Write("Press 1 to register, press 2 to login, or 3 to close the application: ");
			var userInput = Console.ReadLine();
			switch (userInput) {
				case "1":
					Console.Clear();
					Console.WriteLine("--Registry--");
					Console.Write("Username: ");
					var username = Console.ReadLine();
					Console.Write("Password: ");
					var password = Console.ReadLine();
					Console.Write("Email: ");
					var email = Console.ReadLine();
					Console.Write("First name: ");
					var firstName = Console.ReadLine();
					Console.Write("Last name: ");
					var lastName = Console.ReadLine();
					Console.WriteLine("Registering...");
					if (username == null || password == null || email == null || firstName == null ||
					    lastName == null) {
						Console.WriteLine("Values cannot be null!");
						Console.WriteLine("Registration failed, returning to main menu...");
					}
					else {
						if (await ApiCalls.RegisterCall(username, password, email, firstName, lastName)) {
							Console.WriteLine("Registration successful, returning to the main menu...");
						}
						else {
							Console.WriteLine("Registration failed, returning to the main menu...");
						}
					}

					Console.ReadLine();
					break;

				case "2":
					Console.Clear();
					Console.WriteLine("--Login--");
					Console.Write("Username: ");
					var loginUsername = Console.ReadLine();
					Console.Write("Password: ");
					var loginPassword = Console.ReadLine();
					Console.Clear();
					if (loginUsername == null || loginPassword == null) {
						Console.WriteLine("Values cannot be null!");
						Console.WriteLine("Login failed, returning to main menu...");
						continue;
					}

					ulong? currentUserId = await ApiCalls.LoginCall(loginUsername, loginPassword);

					// post-login actions
					if (currentUserId == null) {
						Console.WriteLine("Login failed, returning to main menu...");
						Console.ReadLine();
						break;
					}
					
					Console.WriteLine(
						$"Login successful, welcome {await ApiCalls.GetName(currentUserId.Value)}!");
					Console.ReadLine();
					
					while (true) {
						var exit = false;

						Console.Clear();
						Console.Write(
							"Press 1 to check balance, 2 to withdraw, 3 to deposit, 4 to send money, 5 to" +
							" access account management options, or 6 to exit to login screen: ");
						var userInput2 = Console.ReadLine();
						switch (userInput2) {
							case "1": // check balance
								Console.Clear();
								long? balance = await ApiCalls.CheckBalance(currentUserId.Value);
								
								if (balance == null) 
									Console.WriteLine("Balance check unsuccessful, returning to account page...");
								else 
									Console.WriteLine($"Balance: {balance}");
								
								Console.ReadLine();
								break;

							case "2": // withdraw money
								Console.Clear();
								Console.Write("Withdrawal Amount: ");
								
								try {
									long? withdrawalAmount = Convert.ToInt64(Console.ReadLine());
									if (withdrawalAmount is < 0 or null)
										throw new Exception("Invalid amount, returning to account page...");

									Console.WriteLine(await ApiCalls
										.UpdateBalance(currentUserId.Value, withdrawalAmount.Value, 0)
										? "Withdrawal successful, returning to account page..."
										: "Withdrawal failed, returning to account page...");
								}
								catch (FormatException e) {
									Console.WriteLine(e.Message);
									Console.WriteLine("Invalid amount, returning to account page...");
								}
								catch (Exception i) {
									Console.WriteLine(i.Message);
								}
								
								Console.ReadLine();
								break;

							case "3": // deposit money
								Console.Clear();
								Console.Write("Deposit Amount: ");
								
								try {
									long? depositAmount = Convert.ToInt64(Console.ReadLine());
									if (depositAmount is < 0 or null)
										throw new Exception("Invalid amount, returning to account page...");

									Console.WriteLine(await ApiCalls
										.UpdateBalance(currentUserId.Value, depositAmount.Value, 1)
										? "Deposit successful, returning to account page..."
										: "Deposit failed, returning to account page...");
								}
								catch (FormatException e) {
									Console.WriteLine(e.Message);
									Console.WriteLine("Invalid amount, returning to account page...");
								}
								catch (Exception i) {
									Console.WriteLine(i.Message);
								}
								
								Console.ReadLine();
								break;

							case "4": // send money
								Console.Clear();
								Console.Write("Username of Recipient: ");
								var recipientUsername = Console.ReadLine();
								Console.Write("Amount: ");
								bool moneyRemoved;
								long? sendAmount = null;
								
								try {
									sendAmount = Convert.ToInt64(Console.ReadLine());
									if (sendAmount is < 0 or null)
										throw new Exception("Invalid amount, returning to account page...");
									if (recipientUsername == null)
										throw new Exception("Recipient username cannot be null, returning to account page...");
									
									moneyRemoved = await ApiCalls.UpdateBalance(currentUserId.Value, sendAmount.Value, 0);
								}
								catch (FormatException e) {
									Console.WriteLine(e.Message);
									moneyRemoved = false;
									Console.WriteLine("Invalid amount, returning to account page...");
								}
								catch (Exception i) {
									Console.WriteLine(i.Message);
									moneyRemoved = false;
								}

								if (moneyRemoved) {
									var moneySent = await ApiCalls.SendMoney(recipientUsername!, sendAmount!.Value); 
									// already checked for null values above, so variables are null-suppressed
									if (moneySent) {
										Console.WriteLine("Money sent successfully, returning to account page...");
									}
									else {
										Console.WriteLine(await ApiCalls
											.UpdateBalance(currentUserId.Value, sendAmount.Value, 1)
											? "Send failed, returning to account page..." 
											: "Send critically failed, returning to account page...");
									}
								}
								
								Console.ReadLine();
								break;

							/*case "5": // update account
								Console.Clear();
								Console.Write(
									"Press 1 to update username, 2 to update password, 3 to update name, 4 to " +
									"update email, 5 to delete your account, or 6 to return to account page: ");
								var userInput3 = Console.ReadLine();
								switch (userInput3) {
									case "1": // change username
										Console.Clear();
										Console.Write("New username: ");
										var newUsername = Console.ReadLine();
										AccountManagement.ChangeUsername(Conn, currentUserId, newUsername);
										Console.ReadLine();
										break;

									case "2": // change password
										Console.Clear();
										Console.Write("Verify old password: ");
										var oldPassword = Console.ReadLine();
										Console.Write("New password: ");
										var newPassword = Console.ReadLine();
										if (oldPassword == null || newPassword == null) {
											Console.WriteLine("Values cannot be empty, returning to account page...");
										}
										AccountManagement.ChangePassword(Conn, currentUserId, oldPassword, newPassword);
										Console.ReadLine();
										break;

									case "3": // change name
										Console.Clear();
										Console.Write("New first name: ");
										var newFirstName = Console.ReadLine();
										Console.Write("New last name: ");
										var newLastName = Console.ReadLine();
										AccountManagement.ChangeName(Conn, currentUserId, newFirstName, newLastName);
										Console.ReadLine();
										break;

									case "4": // change email
										Console.Clear();
										Console.Write("New email: ");
										var newEmail = Console.ReadLine();
										AccountManagement.ChangeEmail(Conn, currentUserId, newEmail);
										Console.ReadLine();
										break;

									case "5": // account deletion
										Console.Clear();
										Console.WriteLine("Please verify your password: ");
										var deletionPassword = Console.ReadLine();
										if (deletionPassword == null) {
											Console.WriteLine("Password cannot be empty, returning to account page...");
										}
										AccountManagement.DeleteAccount(Conn, currentUserId, deletionPassword);
										exit = true;
										Console.ReadLine();
										break;
									
									case "6":
										break;

									default:
										Console.WriteLine("Invalid input, returning to account page...");
										break;
								}

								break;*/

							case "6":
								exit = true;
								break;
						}

						if (exit)
							break;
					}
					break;
				case "3":
					Console.Clear();
					Environment.Exit(0);
					break;
			}
		}
	}
}