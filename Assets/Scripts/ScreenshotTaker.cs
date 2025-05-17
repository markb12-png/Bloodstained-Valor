using UnityEngine;
using System.IO;

public class ScreenshotTaker : MonoBehaviour
{
    public KeyCode screenshotKey = KeyCode.F12;
    public string fileName = "screenshot.png";

    void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            string folderPath = @"C:\Users\GAME1\Downloads\Krita 1";

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fullPath = Path.Combine(folderPath, fileName);
            ScreenCapture.CaptureScreenshot(fullPath);

            Debug.Log($"Screenshot saved to: {fullPath}");
        }
    }
}
