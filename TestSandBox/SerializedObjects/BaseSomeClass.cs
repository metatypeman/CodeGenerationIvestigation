using NLog;
using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public partial class BaseSomeClass
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void SomeRun()
        {
            IDisposable disposable = null;
            ISerializer serializer = null;

            var globalContext1 = 16;

            LoggedCodeChunkFunctorWithoutResult<int, float>.Run(_logger, "E45B942C-6B8A-4D3E-A842-0F0C82DE7C5E", globalContext1, (ILogger loggerValue, int globalContextValue, float localContextValue) => {
                var a = 17;
            }, disposable, serializer);

            var globalContext2 = "Hi!";

            var functor = new LoggedCodeChunkFunctorWithoutResult<string, decimal>(_logger, "36C1EDC4-6FCE-43C1-B1C1-BD6B579F3C00", globalContext2, (ILogger loggerValue, string globalContextValue, decimal localContextValue) => {
                var a = 18;
            }, disposable, serializer);
        }
    }
}
