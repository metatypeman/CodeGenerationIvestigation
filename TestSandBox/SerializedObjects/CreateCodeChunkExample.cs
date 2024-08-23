namespace TestSandBox.SerializedObjects
{
    public class CreateCodeChunkExample
    {
        public void Run()
        {
            try
            {
                while (true)
                {
                    ICodeChunksContext codeChunksContext = null;

                    codeChunksContext.CreateCodeChunk("7CDD325A-CFE3-4E47-B2C6-161A3CBC0E19", () => {
                        var a = 12;
                    });

                    codeChunksContext.CreateCodeChunk("1A1F77B7-5FA3-442E-A163-36220EF62A04", (ICodeChunkWithSelfReference selfReference) => {
                        var b = 16;

                        var c = () => {
                            //codeChunksContext.CreateCodeChunk("FA5CC917-6A99-4949-9CA9-2F13BA367B65", () => {
                            //    var d = 12;
                            //});
                        };
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
