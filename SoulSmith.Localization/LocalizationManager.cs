using SoulSmith.Core;

namespace SoulSmith.Localization
{
    public class LocalizationManager
    {
        public static LocalizationManager Instance { get; private set; }

        private Dictionary<string, string> _localizationManifest;

        public LocalizationManager(KeyPathManifest localizationManifest) 
        {
            if (Instance == null) { Instance = this; }

            _localizationManifest = localizationManifest.Manifest;
        }

        public string GetLocalizedString(string key, Dictionary<string, string> variables)
        {
            if (_localizationManifest.TryGetValue(key, out string value))
            {
                // Replace variables in the localized string
                foreach (var variable in variables)
                {
                    value = value.Replace($"{{{variable.Key}}}", variable.Value);
                }

                return value;
            }
            else
            {
                return string.Empty;//throw new KeyNotFoundException($"Localization key '{key}' not found.");
            }
        }
    }
}
