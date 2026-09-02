using System;
using System.Diagnostics;
using System.IO;

internal static class CaptureLauncher
{
    [STAThread]
    private static int Main()
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string appPath = Path.Combine(appDirectory, "Biblioteca.Presentacion.exe");
        string projectRoot = Path.GetFullPath(Path.Combine(appDirectory, "..", ".."));
        string output = Path.Combine(projectRoot, "docs", "capturas");

        ProcessStartInfo startInfo = new ProcessStartInfo(appPath, "--capture \"" + output + "\"");
        startInfo.UseShellExecute = false;
        startInfo.WorkingDirectory = appDirectory;

        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
