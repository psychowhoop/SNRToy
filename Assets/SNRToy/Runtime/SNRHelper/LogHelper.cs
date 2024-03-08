
namespace SNRLogHelper
{
    public static class SLog
    {
        public static void Log(params object[] arguments)
        {
            string logMessage = string.Join("", arguments);
            System.Diagnostics.Debug.WriteLine(logMessage);//for vscode
            UnityEngine.Debug.Log(logMessage);
        }

        public static void Err(params object[] arguments)
        {
            string logMessage = string.Join("", arguments);
            UnityEngine.Debug.LogError(logMessage);
            System.Diagnostics.Debug.WriteLine(logMessage);//for vscode
        }

        public static void Warn(params object[] arguments)
        {
            string logMessage = string.Join("", arguments);
            UnityEngine.Debug.LogWarning(logMessage);
            System.Diagnostics.Debug.WriteLine(logMessage);//for vscode
        }




    }


}