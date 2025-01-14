using Microsoft.Extensions.Logging;

namespace FitnessTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>();
                    

#if DEBUG
                builder.Logging.AddDebug();
#endif

                return builder.Build();
            }
            catch (TypeInitializationException ex)
            {
                // Log the inner exception details
                Console.WriteLine($"TypeInitializationException: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw; // Re-throw the exception after logging
            }
        }
    }
}