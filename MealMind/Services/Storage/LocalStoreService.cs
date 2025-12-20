using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Threading;

namespace MealMind.Services.Storage
{
    // Purpose: Cross-platform local persistence helper for MealMind.
    // - Stores app data in FileSystem.AppDataDirectory.
    // - Reads/writes JSON files for repositories (recipes, favorites, meal plans).

    public sealed class LocalStoreService
    {

        // Static lock shared across all instances.
        // Prevents concurrent reads/writes that can cause file access exceptions on mobile platforms.
        private static readonly SemaphoreSlim _ioLock = new(1, 1);

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };


        // Per-platform app data folder
        public string AppDataPath => FileSystem.AppDataDirectory;

        // Storage structure:
        // recipes cached recipe objects by mealId (supports offline access / quick load)
        // state smaller app state files (favorites list, meal plan, settings, etc.)
        public string RecipesDir => Path.Combine(AppDataPath, "recipes");
        public string StateDir => Path.Combine(AppDataPath, "state");



        public LocalStoreService()
        {
            // Ensure required folders exist on startup.
            Directory.CreateDirectory(RecipesDir);
            Directory.CreateDirectory(StateDir);
        }


        // Centralized file path builders so repositories don't hard-code paths.
        public string RecipePath(string mealId) => Path.Combine(RecipesDir, $"{mealId}.json");
        public string StatePath(string fileName) => Path.Combine(StateDir, fileName);



        public async Task<T?> ReadJsonAsync<T>(string path)
        {

            // Ensure only one read/write at a time, to avoid file access conflicts
            // Returning default (null for reference types) lets repositories treat "missing file" as "no saved data yet".

            await _ioLock.WaitAsync();

            try
            {
                if (!File.Exists(path)) return default;

                await using var stream = File.OpenRead(path);
                return await JsonSerializer.DeserializeAsync<T>(stream, JsonOpts);
            }
            finally
            {
                _ioLock.Release();
            }
        }


        // Writes JSON safely using a temp file, then moves it into place.
        // To avoid corrupt files if the app closes or crashes mid-write.
        public async Task WriteJsonAtomicAsync<T>(string path, T data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var tmpPath = path + ".tmp";

            await _ioLock.WaitAsync();
            try
            {
                await using (var stream = File.Create(tmpPath))
                {
                    await JsonSerializer.SerializeAsync(stream, data, JsonOpts);
                    await stream.FlushAsync();
                }

                if (File.Exists(path))
                    // the target is swapped in one operation.

                    File.Replace(tmpPath, path, null);
                else
                    File.Move(tmpPath, path);
            }
            finally
            {
                // catch ignored just to avoid full crash if delete fails
                try { if (File.Exists(tmpPath)) File.Delete(tmpPath); } catch { }
                _ioLock.Release();
            }
        }


        // Delete is also locked to avoid deleting a file while another operation is reading/writing it.

        public async Task DeleteAsync(string path)
        {
            await _ioLock.WaitAsync();
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            finally
            {
                _ioLock.Release();
            }
        }
    }
}