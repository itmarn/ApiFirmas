using System.Diagnostics.CodeAnalysis;

namespace apiPrisma.Clases
{
    public class LogWriter
    {
        static public void Log(string Message)
        {
            File.AppendAllText(@"C:\Users\doescobar\Desktop\apiPrisma.log", DateTime.Now.ToString() + ": [" + Message + "]" + Environment.NewLine) ;
        }
    }
}
