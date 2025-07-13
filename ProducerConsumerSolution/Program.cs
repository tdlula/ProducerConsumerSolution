
namespace ProducerConsumerSolution.UI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Use dependency injection container
            var mainForm = ServiceContainer.CreateMainForm();
            Application.Run(mainForm);
        }
    }
}