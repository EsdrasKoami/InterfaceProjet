using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InterfaceProjet.Helpers
{
    /// <summary>
    /// Remplacement de ApplicationData.Current.LocalSettings pour les applications non-packagées.
    /// Stocke les paramètres dans un fichier JSON à côté de l'exécutable.
    /// </summary>
    public static class LocalSettingsHelper
    {
        private static readonly string _settingsPath = Path.Combine(
            AppContext.BaseDirectory, "appsettings.json");

        private static Dictionary<string, string> _cache = new();

        static LocalSettingsHelper()
        {
            _cache = Load();
        }

        private static Dictionary<string, string> Load()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                           ?? new Dictionary<string, string>();
                }
            }
            catch
            {
                // En cas d'erreur de lecture, utilise un dictionnaire vide
            }
            return new Dictionary<string, string>();
        }

        private static void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsPath, json);
            }
            catch
            {
                // En cas d'erreur d'écriture, ignore silencieusement
            }
        }

        public static string? GetValue(string key, string? defaultValue = null)
        {
            return _cache.TryGetValue(key, out var val) ? val : defaultValue;
        }

        public static void SetValue(string key, string value)
        {
            _cache[key] = value;
            Save();
        }

        public static bool ContainsKey(string key)
        {
            return _cache.ContainsKey(key);
        }
    }
}
