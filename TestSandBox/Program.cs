using NLog;

namespace TestSandBox
{
    internal class Program
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            _logger.Info("Hello, World!");
        }
    }
}
