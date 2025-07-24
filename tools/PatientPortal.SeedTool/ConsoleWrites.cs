public class ConsoleWrites
{
    public static void WriteHeader()
    {
        // Top border in magenta
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("╔════════════════════════════════════════╗");
        
        // Middle line: magenta borders with cyan text
        Console.Write("║");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("   PatientPortal Database Seed Tool     ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("║");
        
        // Bottom border in magenta
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.ResetColor();
    }

    public static void WriteCounts(int staffCount, int patientCount)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"Staff: {staffCount} | Patients: {patientCount}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void WriteOperationParams(int initialStaff, int initialPatients)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("Operation Parameters:");
        Console.ResetColor();
        Console.WriteLine($"  Staff:    {initialStaff}");
        Console.WriteLine($"  Patients: {initialPatients}");
    }

    public static void WriteOperationResults(int initialStaff, int initialPatients, int finalStaff, int finalPatients)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("Operation Results:");
        Console.ResetColor();
        Console.WriteLine($"  Staff:    {finalStaff} (↑{finalStaff - initialStaff})");
        Console.WriteLine($"  Patients: {finalPatients} (↑{finalPatients - initialPatients})");
    }
}