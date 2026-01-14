namespace Stamps.Shared.Resources
{
    public static class Strings
    {
        public static string CurrentLanguage { get; set; } = "it";

        // Welcome Screen
        public static string Welcome => CurrentLanguage == "it" ? "BENVENUTO" : "WELCOME";
        public static string ChooseOne => CurrentLanguage == "it" ? "SELUN" : "CHOOSE ONE";
        public static string Customer => CurrentLanguage == "it" ? "UTENTE" : "CUSTOMER";
        public static string Store => CurrentLanguage == "it" ? "NEGOZIO" : "STORE";
        
        // Upgrades
        public static string UpgradeToPlus => CurrentLanguage == "it" ? "PASSA A PLUS" : "UPGRADE TO PLUS";
        public static string UpgradeToPremium => CurrentLanguage == "it" ? "PASSA A PREMIUM" : "UPGRADE TO PREMIUM";
        
        // Actions
        public static string Scan => CurrentLanguage == "it" ? "SCAN" : "SCAN";
        public static string MyCards => CurrentLanguage == "it" ? "Le mie tessere" : "My Cards";
        public static string FindStores => CurrentLanguage == "it" ? "Trova negozi" : "Find Stores";
        public static string Profile => CurrentLanguage == "it" ? "Profilo" : "Profile";
        
        // QR Code
        public static string ShowQRCode => CurrentLanguage == "it" ? "Mostra QR Code" : "Show QR Code";
        public static string ScanToEarn => CurrentLanguage == "it" ? "Fai scansionare per guadagnare punti" : "Scan to earn points";
        
        // Points
        public static string Points => CurrentLanguage == "it" ? "Punti" : "Points";
        public static string TotalPoints => CurrentLanguage == "it" ? "Punti totali" : "Total Points";
    }
}

