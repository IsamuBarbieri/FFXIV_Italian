using System.IO.Compression;
using System.Text;
using System.Text.Json;
using FFXIVItalian.Core.Packaging;
using Xunit;

namespace FFXIVItalian.Tests;

public class PenumbraPackagerTests
{
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

