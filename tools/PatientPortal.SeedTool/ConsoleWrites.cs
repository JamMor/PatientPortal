namespace PatientPortal.SeedTool;

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

    public static void WriteOperationParams(
        bool isSeedingPresets,
        int initialStaff,
        int initialPatients,
        bool isSeedingMessages
    )
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("Operation Parameters:");
        Console.ResetColor();
        Console.WriteLine($"  Seeding Presets:  {(isSeedingPresets ? "Yes" : "No")}");
        Console.WriteLine($"  Staff:    {initialStaff}");
        Console.WriteLine($"  Patients: {initialPatients}");
        Console.WriteLine($"  Seeding Messages: {(isSeedingMessages ? "Yes" : "No")}");
    }

    public static void WriteOperationResults(
        int seededPresetStaff,
        int seededPresetPatients,
        int seededPatients,
        int seededStaff,
        int seededPatientConversations,
        int seededStaffConversations
    )
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("Operation Results:");
        Console.ResetColor();
        Console.WriteLine($"  Preset Staff:    {seededPresetStaff}");
        Console.WriteLine($"  Preset Patients: {seededPresetPatients}");
        Console.WriteLine("  -----------------------------");
        Console.WriteLine($"  Staff:    {seededStaff}");
        Console.WriteLine($"  Patients: {seededPatients}");
        Console.WriteLine("  -----------------------------");
        Console.WriteLine($"  Patient Conversations: {seededPatientConversations}");
        Console.WriteLine($"  Staff Conversations: {seededStaffConversations}");
    }
}
