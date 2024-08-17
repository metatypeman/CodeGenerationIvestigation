using NLog;
using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public class LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext>
    {
        public static LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext> Run(ILogger logger, string codeChunksContextId, TGlobalContext globalContext,
            Action<ILogger, TGlobalContext, TLocalContext> action,
            IDisposable context, ISerializer threadPool)
        {
            throw new NotImplementedException();
        }

        public LoggedCodeChunkFunctorWithoutResult(ILogger logger, string codeChunksContextId, TGlobalContext globalContext,
            Action<ILogger, TGlobalContext, TLocalContext> action,
            IDisposable context, ISerializer threadPool)
        {

        }
    }
}
