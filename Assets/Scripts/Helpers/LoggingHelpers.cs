using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class LoggingHelpers
    {
        private static Dictionary<string, float> TimeLookup = new Dictionary<string, float>();

        public static void LogEvery(string key, object log, float seconds)
        {
            if (TimeLookup.TryGetValue(key, out float timeValue))
            {
                timeValue += Time.deltaTime;
                if (timeValue > seconds)
                {
                    Debug.Log(log);
                    timeValue = 0f;
                }
            }

            TimeLookup[key] = timeValue;
        }
    }
}
