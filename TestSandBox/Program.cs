using NLog;
using TestSandBox.Serialization;
using TestSandBox.SerializedObjects;

namespace TestSandBox
{
    internal class Program
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _logger.Info("Hello, World!");

            var fact = new Fact();
            ISerializable serializable = fact;

            _logger.Info($"serializable.GetPlainObjectType().FullName = {serializable.GetPlainObjectType().FullName}");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Info($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}
