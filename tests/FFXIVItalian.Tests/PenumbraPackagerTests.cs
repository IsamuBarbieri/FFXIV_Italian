using System.IO.Compression;
using System.Text;
using System.Text.Json;
using FFXIVItalian.Core.Packaging;
using FFXIVItalian.Patcher;
using Xunit;

namespace FFXIVItalian.Tests;

public class PenumbraPackagerTests
{
    [Fact]
    public async Task SelectableTranslationsAreRegisteredInV4MetadataAndEnabledByDefault()
    {
        string folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(folder, "dialogue"));
        Directory.CreateDirectory(Path.Combine(folder, "world"));
        try
        {
            File.WriteAllText(Path.Combine(folder, "dialogue", "customtalk.json"), "{}");
            File.WriteAllText(Path.Combine(folder, "world", "fate.json"), "{}");
            var files = new Dictionary<string, byte[]>
            {
                ["exd/customtalk_720896_en.exd"] = [1, 2],
                ["exd/fate_0_en.exd"] = [3, 4],
                ["readme_ita.txt"] = "Istruzioni"u8.ToArray()
            };
            var meta = new PenumbraMeta { Groups = TranslationOptionBuilder.Create(folder, files.Keys) };
            string pmp = Path.Combine(folder, "test.pmp");
            await PenumbraPackager.CreatePmpPackageAsync(pmp, meta, files);
            TranslationBuildVerifier.VerifyPackage(pmp, meta, files);
            using (var archive = ZipFile.OpenRead(pmp))
            {
                using var metadata = JsonDocument.Parse(archive.GetEntry("meta.json")!.Open());
                Assert.Equal(4, metadata.RootElement.GetProperty("FileVersion").GetInt32());
                using var defaults = JsonDocument.Parse(archive.GetEntry("default_mod.json")!.Open());
                Assert.Single(defaults.RootElement.GetProperty("Files").EnumerateObject());
                var groups = metadata.RootElement.GetProperty("Groups");
                Assert.Equal(2, groups.GetArrayLength());
                Assert.Equal("Missioni e dialoghi", groups[0].GetProperty("Name").GetString());
                Assert.Contains("Pagine di testo: 1", groups[0].GetProperty("Description").GetString());
                Assert.Equal(1UL, groups[0].GetProperty("DefaultSettings").GetUInt64());
            }
            files["exd/fate_0_en.exd"] = [9];
            Assert.Throws<InvalidDataException>(() => TranslationBuildVerifier.VerifyPackage(pmp, meta, files));
        }
        finally { Directory.Delete(folder, true); }
    }

    [Fact]
    public void ResourceFilesExcludeEditorialFoldersAndKeepExplicitGameRoots()
    {
        string folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        try
        {
            foreach (string relative in new[] { "assets/ui/icon/test.tex", "ui/logo/test.tex", "batches/draft.json", "updates/backup.json", "unknown/report.txt", "translations/system/addon.json" })
            {
                string file = Path.Combine(folder, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(file)!);
                File.WriteAllText(file, "test");
            }
            Assert.Equal(new[] { "assets/ui/icon/test.tex", "ui/logo/test.tex" },
                PenumbraPackager.GetResourceFiles(folder).Select(file => Path.GetRelativePath(folder, file).Replace('\\', '/')).Order());
        }
        finally { Directory.Delete(folder, true); }
    }

    [Fact]
    public async Task CreatePmpPackageAsync_GeneratesValidZipAndManifests()
    {
        string tempPmp = Path.Combine(Path.GetTempPath(), $"test_mod_{Guid.NewGuid():N}.pmp");

        try
        {
            var meta = new PenumbraMeta
            {
                Name = "Test Italian Mod",
                Author = "Tester",
                Description = "A test package",
                Version = "1.0.0"
            };

            var files = new Dictionary<string, byte[]>
            {
                ["exd/test_en.exd"] = Encoding.UTF8.GetBytes("Binary EXD dummy data"),
                ["readme.txt"] = Encoding.UTF8.GetBytes("Penumbra test")
            };

            await PenumbraPackager.CreatePmpPackageAsync(tempPmp, meta, files);

            Assert.True(File.Exists(tempPmp));

            // Inspect ZIP archive contents
            using var archive = ZipFile.OpenRead(tempPmp);
            var metaEntry = archive.GetEntry("meta.json");
            Assert.NotNull(metaEntry);

            using (var stream = metaEntry.Open())
            {
                var doc = await JsonDocument.ParseAsync(stream);
                Assert.Equal("Test Italian Mod", doc.RootElement.GetProperty("Name").GetString());
                Assert.Equal("1.0.0", doc.RootElement.GetProperty("Version").GetString());
            }

            var defaultModEntry = archive.GetEntry("default_mod.json");
            Assert.NotNull(defaultModEntry);

            using (var stream = defaultModEntry.Open())
            {
                var doc = await JsonDocument.ParseAsync(stream);
                var filesProp = doc.RootElement.GetProperty("Files");
                Assert.True(filesProp.TryGetProperty("exd/test_en.exd", out var targetVal));
                Assert.Equal("exd/test_en.exd", targetVal.GetString());
            }

            var exdEntry = archive.GetEntry("exd/test_en.exd");
            Assert.NotNull(exdEntry);
        }
        finally
        {
            if (File.Exists(tempPmp))
            {
                File.Delete(tempPmp);
            }
        }
    }
}

