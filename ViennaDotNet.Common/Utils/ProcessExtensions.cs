using System.Diagnostics;

namespace ViennaDotNet.Common.Utils
{
    public static class ProcessExtensions
    {
        public static bool TryStopGracefully(this Process process)
        {
            try
            {
                nint mainWindowHandle = process.MainWindowHandle;
                if (mainWindowHandle != IntPtr.Zero)
                {
                    process.CloseMainWindow();
                    return true;
                }
            }
            catch { }

            process.Kill();
            return false;
        }
    }
}
