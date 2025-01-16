class Program
{
    static void Main()
    {
        Menu menu = new Menu();
        menu.ShowMenu();
    }
}
class DisplayHelper
{
    public static void ShowMessage(string message, int delayMilliseconds = 1500)
    {
        Console.Clear();
        Console.WriteLine(message);
        Thread.Sleep(delayMilliseconds);
        Console.Clear();
    }

    public static void Pause()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
class Menu
{
    public void ShowMenu()
    {
        string choice;
        do
        {
            Console.Clear();
            Console.WriteLine("= MENU =");
            Console.WriteLine("1. Enter");
            Console.WriteLine("2. Leave");
            Console.WriteLine("3. Management");
            Console.WriteLine("Q. Quit");
            Console.Write("Choose an option: ");
            choice = Console.ReadLine();
            switch (choice.ToLower())
            {
                case "1":
                case "enter":
                    Parking.EnterCarPark();
                    break;
                case "2":
                case "leave":
                    Parking.LeaveCarPark();
                    break;
                case "3":
                case "management":
                    Management managementMenu = new Management();
                    bool stayInManagement = managementMenu.ShowMenu();
                    if (!stayInManagement)
                    {
                        continue;
                    }
                    break;
                case "q":
                case "quit":
                    DisplayHelper.ShowMessage("Quitting...");
                    break;
                default:
                    DisplayHelper.ShowMessage("Invalid input, try again");
                        break;
            }
        } while (choice.ToLower() != "q" || choice.ToLower() != "quit");
    }
}
class Parking
{
    public static void EnterCarPark()
    {
        Console.Clear();
        string plateNumber;
        while (true)
        {
            Console.Write("Enter plate number: ");
            plateNumber = Console.ReadLine();
            plateNumber = plateNumber.Replace(" ", "").ToUpper();
            if (!ValidateLicensePlate(plateNumber))
            {
                DisplayHelper.ShowMessage("Invalid plate number, try again");
            }
            else
            {
                if (FileHandlers.IsCarInCarPark(plateNumber))
                {
                    DisplayHelper.ShowMessage("This car is already in the car park.");
                }
                else
                {
                    break;
                }
            }
        }
        if (CheckCarCount())
        {
            FileHandlers.AddCarToCarPark(plateNumber);
            FileHandlers.IncrementTotalEnteredCount();
            DisplayHelper.ShowMessage("Plate recorded, you may enter");
        }
        else
        {
            DisplayHelper.ShowMessage("Car park is full, come back later.\nReturning to menu...");
        }
    }

    public static void LeaveCarPark()
    {
        string plateNumber;
        while (true)
        {
            Console.Clear();
            Console.Write("Enter plate number: ");
            plateNumber = Console.ReadLine();
            plateNumber = plateNumber.Replace(" ", "").ToUpper();
            if (FileHandlers.IsCarInCarPark(plateNumber))
            {
                break;
            }
            else
            {
                DisplayHelper.ShowMessage("Invalid plate number, try again");
            }
        }
        double cost = FileHandlers.CalculateFeeForCar(plateNumber);
        do
        {
            Console.Clear();
            Console.WriteLine("Total cost is: £" + cost);
            Console.Write("Insert payment: ");
            double payment = Convert.ToDouble(Console.ReadLine());
            cost -= payment;
        } while (cost != 0);
        FileHandlers.RemoveCarAndGenerateReceipt(plateNumber);
    }

    private static bool ValidateLicensePlate(string plateNumber)
    {
        if (plateNumber.Length != 7) // checks if the plate number is 7 digits long
        {
            return false;
        }
        if (!char.IsLetter(plateNumber[0]) || !char.IsLetter(plateNumber[1])) // checks if first 2 characters are letters
        {
            return false;
        }
        if (!char.IsDigit(plateNumber[2]) || !char.IsDigit(plateNumber[3])) // checks if second 2 characters are numbers
        {
            return false;
        }
        if (!char.IsLetter(plateNumber[4]) || !char.IsLetter(plateNumber[5]) || !char.IsLetter(plateNumber[6])) // checks the final 3 characters are letters
        {
            return false;
        }
        return true;
    }

    private static bool CheckCarCount()
    {
        string[] cars = File.ReadAllLines("VehiclesInCarPark.txt");
        return cars.Length <= 45;
    }
}
class Management
{
    private const string ParkingFile = "VehiclesInCarPark.txt";
    private const string ReceiptsFolder = "Receipts";
    public bool ShowMenu()
    {
        if (AuthManagement())
        {
            string choice;
            do
            {
                Console.Clear();
                Console.WriteLine("= MANAGEMENT =");
                Console.WriteLine("1. Capacity");
                Console.WriteLine("2. Earnings");
                Console.WriteLine("3. Search receipts");
                Console.WriteLine("Q. Quit");
                Console.Write("Choose an option: ");
                choice = Console.ReadLine();
                switch (choice.ToLower())
                {
                    case "1":
                    case "capacity":
                        ViewCapacity();
                        break;
                    case "2":
                    case "earnings":
                        ViewEarnings();
                        break;
                    case "3":
                    case "search receipts":
                    case "search":
                        SearchReceipts();
                        break;
                    case "q":
                    case "quit":
                        DisplayHelper.ShowMessage("Quitting...");
                        return false;
                    default:
                        DisplayHelper.ShowMessage("Invalid input, try again");
                        break;
                }
            } while (choice.ToLower() != "q" && choice.ToLower() != "quit");
            return true;
        }
        else
        {
            DisplayHelper.ShowMessage("Access denied.\nQuitting...");
            return false;
        }
    }

    private void ViewCapacity()
    {
        try
        {
            int totalCapacity = 45;
            int currentCount = File.ReadAllLines(ParkingFile).Length;

            int remainingSpaces = totalCapacity - currentCount;
            int totalEntered = FileHandlers.GetTotalEnteredCount();

            Console.Clear();
            Console.WriteLine($"Current Occupancy: {currentCount}/{totalCapacity}");
            Console.WriteLine($"Available Spaces: {remainingSpaces}");
            Console.WriteLine($"Total Cars Entered: {totalEntered}");
            DisplayHelper.Pause();
        } catch (Exception ex) 
        { 
            Console.WriteLine($"Error retrieving capcity: {ex.Message}");
            DisplayHelper.Pause();
        }
    }

    private void ViewEarnings()
    {
        try
        {
            double totalEarnings = 0;

            if (Directory.Exists(ReceiptsFolder))
            {
                string[] files = Directory.GetFiles(ReceiptsFolder, "*.txt");

                if (files.Length == 0)
                {
                    DisplayHelper.ShowMessage("No receipts found.");
                    return;
                }
                foreach (string file in files)
                {
                    string[] lines = File.ReadAllLines(file);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Total cost: £", StringComparison.OrdinalIgnoreCase))
                        {
                            string costText = line.Split('£')[1].Trim();
                            if (double.TryParse(costText, out double cost))
                            {
                                totalEarnings += cost;
                            }
                        }
                    }
                }
            }
            else
            {
                DisplayHelper.ShowMessage("Receipts folder not found");
                return;
            }

            Console.Clear();
            Console.WriteLine($"Total Earnings: £{totalEarnings}");
            DisplayHelper.Pause();
        } catch (Exception ex) 
        { 
            Console.WriteLine($"Error calculating earnings: {ex.Message}");
            DisplayHelper.Pause();
        }
    }

    private void SearchReceipts()
    {
        try
        {
            Console.Clear();
            Console.WriteLine("Search receipts by:");
            Console.WriteLine("1. License Plate");
            Console.WriteLine("2. Date (DD-MM-YYYY)");
            string searchChoice = Console.ReadLine();

            string searchTerm = "";
            bool searchByDate = false;

            switch (searchChoice.ToLower())
            {
                case "1":
                case "license plate":
                    Console.Clear();
                    Console.Write("Enter license plate: ");
                    searchTerm = Console.ReadLine().Replace(" ", "").ToUpper();
                    break;
                case "2":
                case "date":
                    Console.Clear();
                    Console.Write("Enter date (DD-MM-YYYY): ");
                    searchTerm = Console.ReadLine();
                    searchByDate = true;
                    break;
                default:
                    DisplayHelper.ShowMessage("Invalid input, try again");
                    return;
            }

            bool found = false;
            if (Directory.Exists(ReceiptsFolder))
            {
                string[] files = Directory.GetFiles(ReceiptsFolder, "*.txt");

                foreach (string file in files)
                {
                    string fileContent = File.ReadAllText(file);

                    if (searchByDate)
                    {
                        string entryLine = fileContent.Split('\n').FirstOrDefault(line => line.StartsWith("Time entered:", StringComparison.OrdinalIgnoreCase));

                        if (!string.IsNullOrEmpty(entryLine))
                        {
                            string dateEntered = entryLine.Replace("Time entered: ", "").Trim().Split(' ')[0].Replace("/", "-");

                            if (dateEntered == searchTerm)
                            {
                                Console.Clear();
                                Console.WriteLine($"Receipt found: {file}");
                                Console.WriteLine(fileContent);
                                found = true;
                            }
                        }
                    }
                    else
                    {
                        if (fileContent.Contains($"Receipt for {searchTerm}", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.Clear();
                            Console.WriteLine($"Receipt found: {file}");
                            Console.WriteLine(fileContent);
                            found = true;
                        }
                    }
                }
                if (!found)
                {
                    Console.WriteLine("No receipts found matching the criteria.");
                }
            }
            else
            {
                Console.WriteLine("Receipts folder not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching receipts: {ex.Message}");
            DisplayHelper.Pause();
        }
        DisplayHelper.Pause();
    }

    private bool AuthManagement()
    {
        Console.Clear();
        for (int attemptsLeft = 3; attemptsLeft > 0; attemptsLeft--)
        {
            Console.Write("Enter admin password: ");
            string adminPass = Console.ReadLine();
            if (adminPass == "AdminPassword")
            {
                return true;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Password incorrect, try again");
                Console.WriteLine(attemptsLeft - 1 + " Attempts left");
            }
        }
        return false;
    }
}
class FileHandlers
{
    private static string filePath = "VehiclesInCarPark.txt";
    public static bool IsCarInCarPark(string licensePlate)
    {
        string[] lines = File.ReadAllLines("VehiclesInCarPark.txt");
        foreach (string line in lines)
        {
            if (line.StartsWith(licensePlate + ","))
            {
                return true;
            }
        }
        return false;
    }

    public static void AddCarToCarPark(string licensePlate)
    {
        string entryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        string entry = $"{licensePlate},{entryTime}"; 
        File.AppendAllText(filePath, entry + Environment.NewLine); 
    }

    public static void RemoveFromCarPark(string licensePlate)
    {
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            List<string> updatedLines = new List<string>();

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (!parts[0].Trim().Equals(licensePlate.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    updatedLines.Add(line);
                }
            }

            File.WriteAllLines(filePath, updatedLines); 
        }
    }

    public static DateTime? GetEntryTime(string licensePlate) 
    {
        string[] lines = File.ReadAllLines("VehiclesInCarPark.txt");
        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            if (parts[0] == licensePlate)
            {
                if (DateTime.TryParse(parts[1], out DateTime entryTime))
                {
                    return entryTime;
                }
                break;
            }
        }
        return null;
    }

    public static double CalculateFeeForCar(string licensePlate)
    {
        DateTime? entryTime = GetEntryTime(licensePlate);
        if (entryTime.HasValue)
        {
            DateTime exitTime = DateTime.Now;
            return ParkingFeeCalculator.CalculateParkingFee(entryTime.Value, exitTime);
        }
        else
        {
            throw new Exception("Car not found in the car park.");
        }
    }

    public static void RemoveCarAndGenerateReceipt(string licensePlate)
    {
        try
        {
            DateTime? entryTime = GetEntryTime(licensePlate);
            if (entryTime.HasValue)
            {
                DateTime exitTime = DateTime.Now;
                double fee = ParkingFeeCalculator.CalculateParkingFee(entryTime.Value, exitTime);
                bool isPaid = true;
                ReceiptHandler.GenerateReceipt(licensePlate, entryTime.Value, exitTime, fee, isPaid);
                RemoveFromCarPark(licensePlate);
                DisplayHelper.ShowMessage($"{licensePlate} has been removed from the car park");
            }
            else
            {
                DisplayHelper.ShowMessage("Vehicle not found in car park");
            }
        } catch (Exception ex)
        {
            Console.WriteLine($"Error processing car exit: {ex.Message}");
            DisplayHelper.Pause();
        }
    }

    public static int GetTotalEnteredCount()
    {
        string countFile = "CarCount.txt";

        try
        {
            if (File.Exists(countFile))
            {
                string content = File.ReadAllText(countFile);
                if (int.TryParse(content, out int count))
                {
                    return count;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading car count: {ex.Message}");
        }
        return 0;
    }

    public static void IncrementTotalEnteredCount()
    {
        string countFile = "CarCount.txt";

        try
        {
            int count = 0;
            if (File.Exists(countFile))
            {
                string content = File.ReadAllText(countFile);
                int.TryParse(content, out count);
            }
            count++;
            File.WriteAllText(countFile, count.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating car count: {ex.Message}");
        }
    }
}
class ParkingFeeCalculator
{
    private const double FirstHourFee = 2.00;
    private const double AdditionalHourlyFee = 1.50;
    private const double DailyMaxFee = 20.00;

    public static double CalculateParkingFee(DateTime entryTime, DateTime exitTime)
    {
        TimeSpan duration = exitTime - entryTime;
        int fullDays = (int)duration.TotalDays;
        double remainingHours = duration.TotalHours - (fullDays * 24);
        double totalFee = 0;
        totalFee += fullDays * DailyMaxFee;

        if (remainingHours > 0)
        {
            if (remainingHours <= 1)
            {
                totalFee += FirstHourFee;
            }
            else
            {
                totalFee += FirstHourFee + ((int)Math.Ceiling(remainingHours) - 1) * AdditionalHourlyFee;
            }
            totalFee = Math.Min(totalFee, (fullDays + 1) * DailyMaxFee);
        }
        return totalFee;
    }
}
class ReceiptHandler
{
    private const string ReceiptFolder = "Receipts";

    public static void GenerateReceipt(string licensePlate, DateTime entryTime, DateTime exitTime, double totalCost, bool isPaid)
    {
        try
        {
            if (!Directory.Exists(ReceiptFolder))
            {
                Directory.CreateDirectory(ReceiptFolder);
            }

            string safeLicensePlate = SanitizeFileName(licensePlate);

            TimeSpan duration = exitTime - entryTime;
            string receipt = $"Receipt for {licensePlate}\n" +
                             $"-----------------------------------\n" +
                             $"Time entered: {entryTime}\n" +
                             $"Time exited: {exitTime}\n" +
                             $"Duration: {duration.Days} Days, {duration.Hours} Hours\n" +
                             $"Total cost: £{totalCost}\n" +
                             $"Paid: {(isPaid ? "Yes" : "No")}\n" +
                             $"-----------------------------------\n";

            string filePath = Path.Combine(ReceiptFolder, $"{licensePlate}_Receipt.txt");
            File.WriteAllText(filePath, receipt);
            Console.WriteLine(receipt);
            DisplayHelper.Pause();
        } catch (Exception ex)
        {
            Console.WriteLine($"Error generating receipt: {ex.Message}");
            DisplayHelper.Pause();
        }
    }

    private static string SanitizeFileName(string input)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            input = input.Replace(c.ToString(), "_");
        }
        return input;
    }
}