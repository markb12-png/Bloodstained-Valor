using System.Collections.Generic;
using UnityEngine;

public class PriorityManager : MonoBehaviour
{
    // Tracks whether a high-priority interaction (e.g., sword clash) is currently active
    public static bool isClashActive = false;

    /// <summary>
    /// Overrides lower-priority scripts, disabling everything except the high-priority one.
    /// </summary>
    /// <param name="target">The GameObject whose scripts are being overridden.</param>
    public static void OverrideWithClash(GameObject target)
    {
        Debug.Log("Overriding scripts with SwordClash logic.");

        // Find all MonoBehaviour scripts on the target GameObject
        MonoBehaviour[] scripts = target.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            // Disable everything except the SwordClash script
            if (script.GetType() != typeof(SwordClash) && script.enabled)
            {
                script.enabled = false; // Disable lower-priority scripts
                Debug.Log("Disabled script: " + script.GetType().Name);
            }
        }
    }

    /// <summary>
    /// Restores all previously disabled scripts after the high-priority interaction ends.
    /// </summary>
    /// <param name="target">The GameObject whose scripts are being restored.</param>
    public static void RestorePriority(GameObject target)
    {
        Debug.Log("Restoring scripts after SwordClash ends.");

        // Find all MonoBehaviour scripts on the target GameObject
        MonoBehaviour[] scripts = target.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            // Enable all scripts except SwordClash (which will remain enabled by default)
            if (script.GetType() != typeof(SwordClash) && !script.enabled)
            {
                script.enabled = true; // Re-enable previously disabled scripts
                Debug.Log("Enabled script: " + script.GetType().Name);
            }
        }
    }
}
