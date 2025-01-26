using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class MediaPipe : MonoBehaviour
{
    private Process pythonProcess;
    // Path relative to the StreamingAssets folder
    private string exeRelativePath = "Model.exe";
    public void StartPythonProcess()
    {
        // Determine the absolute path to the executable
        string exePath = Path.Combine(Application.streamingAssetsPath, exeRelativePath);

        // Log the executable path for verification
        UnityEngine.Debug.Log($"Attempting to start Python executable at: {exePath}");

        if (!File.Exists(exePath))
        {
            UnityEngine.Debug.LogError($"Python executable not found at path: {exePath}");
            return;
        }

        // Initialize the process start info
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = "", // Add any command-line arguments if necessary
            UseShellExecute = true, // Allows the terminal window to be visible
            CreateNoWindow = false  // Ensures the window is created
        };

        // Start the process
        pythonProcess = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        // Handle process exit
        pythonProcess.Exited += (sender, args) =>
        {
            UnityEngine.Debug.LogError("Python process exited unexpectedly.");
            // Optionally, implement a retry mechanism or notify the user
        };

        try
        {
            // Start the process
            bool started = pythonProcess.Start();
            if (started)
            {
                UnityEngine.Debug.Log("Python process started successfully.");
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to start Python process.");
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Exception while starting Python process: {e.Message}");
        }
    }
}
