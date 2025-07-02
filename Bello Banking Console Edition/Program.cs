namespace Bello_Banking_Console_Edition;

internal static class Program {
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
                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                        string.IsNullOrWhiteSpace(email)
                        || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) {
                        Console.WriteLine("Values cannot be null!");
                        Console.WriteLine("Registration failed, returning to main menu...");
                    }
                    else {
                        if (await ApiCalls.RegisterCall(username, password, email, firstName, lastName))
                            Console.WriteLine("Registration successful, returning to the main menu...");
                        else
                            Console.WriteLine("Registration failed, returning to the main menu...");
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
                    if (string.IsNullOrWhiteSpace(loginUsername) || string.IsNullOrWhiteSpace(loginPassword)) {
                        Console.WriteLine("Values cannot be null!");
                        Console.WriteLine("Login failed, returning to main menu...");
                        continue;
                    }

                    var currentUserId = await ApiCalls.LoginCall(loginUsername, loginPassword);

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
                        Console.WriteLine("--Account Page--");
                        Console.Write(
                            "Press 1 to check balance, 2 to withdraw, 3 to deposit, 4 to send money, 5 to" +
                            " access account management options, or 6 to exit to login screen: ");
                        var userInput2 = Console.ReadLine();
                        switch (userInput2) {
                            case "1": // check balance
                                Console.Clear();
                                var balance = await ApiCalls.CheckBalance(currentUserId.Value);

                                Console.WriteLine(balance == null
                                    ? "Balance check unsuccessful, returning to account page..."
                                    : $"Balance: {balance}");

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

                                try {
                                    long? sendAmount = Convert.ToInt64(Console.ReadLine());
                                    if (sendAmount is < 0 or null)
                                        throw new Exception("Invalid amount, returning to account page...");
                                    if (string.IsNullOrEmpty(recipientUsername))
                                        throw new Exception(
                                            "Recipient username cannot be null, returning to account page...");

                                    if (await ApiCalls.UpdateBalance(currentUserId.Value, sendAmount.Value, 0)) {
                                        if (await ApiCalls.SendMoney(recipientUsername, sendAmount.Value))
                                            Console.WriteLine("Money sent successfully, returning to account page...");
                                        else
                                            Console.WriteLine(await ApiCalls
                                                .UpdateBalance(currentUserId.Value, sendAmount.Value, 1)
                                                ? "Send failed, returning to account page..."
                                                : "Send critically failed, returning to account page...");
                                    }
                                    else {
                                        Console.WriteLine("Send failed, returning to account page...");
                                    }
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

                            case "5": // update account
                                Console.Clear();
                                Console.WriteLine("--Account Management--");
                                Console.Write(
                                    "Press 1 to update username, 2 to update password, 3 to update name, 4 to " +
                                    "update email, 5 to delete your account, or 6 to return to account page: ");
                                var userInput3 = Console.ReadLine();
                                switch (userInput3) {
                                    case "1": // change username
                                        Console.Clear();
                                        Console.Write("New username: ");
                                        var newUsername = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(newUsername))
                                            Console.WriteLine(await ApiCalls
                                                .ChangeUsername(currentUserId.Value, newUsername)
                                                ? "Username changed successfully, returning to account page..."
                                                : "Username change failed, username may already be taken, returning to account page...");
                                        else
                                            Console.WriteLine(
                                                "New username cannot be blank, returning to account page...");
                                        Console.ReadLine();
                                        break;

                                    case "2": // change password
                                        Console.Clear();
                                        Console.Write("Verify old password: ");
                                        var oldPassword = Console.ReadLine();
                                        Console.Write("New password: ");
                                        var newPassword = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(oldPassword) &&
                                            !string.IsNullOrWhiteSpace(newPassword))
                                            Console.WriteLine(await ApiCalls
                                                .ChangePassword(currentUserId.Value, oldPassword, newPassword)
                                                ? "Password changed successfully, returning to account page..."
                                                : "Password change failed, returning to account page...");
                                        else
                                            Console.WriteLine(
                                                "Passwords cannot be blank, returning to account page...");

                                        Console.ReadLine();
                                        break;

                                    case "3": // change name
                                        Console.Clear();
                                        Console.Write("New first name: ");
                                        var newFirstName = Console.ReadLine();
                                        Console.Write("New last name: ");
                                        var newLastName = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(newFirstName) &&
                                            !string.IsNullOrWhiteSpace(newLastName))
                                            Console.WriteLine(await ApiCalls
                                                .ChangeName(currentUserId.Value, newFirstName, newLastName)
                                                ? "Name changed successfully, returning to account page..."
                                                : "Name change failed, returning to account page...");
                                        else
                                            Console.WriteLine(
                                                "New names cannot be blank, returning to account page...");
                                        Console.ReadLine();
                                        break;

                                    case "4": // change email
                                        Console.Clear();
                                        Console.Write("New email: ");
                                        var newEmail = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(newEmail))
                                            Console.WriteLine(await ApiCalls
                                                .ChangeEmail(currentUserId.Value, newEmail)
                                                ? "Email changed successfully, returning to account page..."
                                                : "Email change failed, returning to account page...");
                                        else
                                            Console.WriteLine(
                                                "New email cannot be blank, returning to account page...");
                                        Console.ReadLine();
                                        break;

                                    case "5": // account deletion
                                        Console.Clear();
                                        Console.Write("Please verify your password: ");
                                        var deletionPassword = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(deletionPassword)) {
                                            if (await ApiCalls.DeleteAccount(currentUserId.Value, deletionPassword)) {
                                                Console.WriteLine(
                                                    "Account deleted successfully, returning to login page...");
                                                exit = true;
                                            }
                                            else {
                                                Console.WriteLine(
                                                    "Account deletion failed, returning to account page...");
                                            }
                                        }
                                        else {
                                            Console.WriteLine("Password cannot be blank, returning to account page...");
                                        }

                                        Console.ReadLine();
                                        break;

                                    case "6":
                                        break;

                                    default:
                                        Console.WriteLine("Invalid input, returning to account page...");
                                        break;
                                }

                                break;

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