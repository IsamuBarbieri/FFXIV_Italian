using System.Text.Json.Nodes;
using FFXIVItalian.Extractor;

namespace FFXIVItalian.Tests;

public class GameUpdateTests
{
    [Fact]
    public void MergePreservesTranslationsMetadataAndRemovedRowsAndReportsChanges()
    {
        var before = JsonNode.Parse("""{"1":{"original":"old","translation":"italiano","note":"keep"},"2":{"original":"same","translation":"uguale"},"3":{"original":"gone","translation":"conserva"}}""")!.AsObject();
        var fresh = JsonNode.Parse("""{"1":{"original":"new","translation":""},"2":{"original":"same","translation":"uguale"},"4":{"original":"added","translation":""}}""")!.AsObject();
        var changes = new List<UpdateChange>();
        var merged = GameUpdate.Merge("file.json", before, fresh, changes);
        Assert.Equal("italiano", merged["1"]!["translation"]!.GetValue<string>());
        Assert.Equal("keep", merged["1"]!["note"]!.GetValue<string>());
        Assert.Equal("new", merged["1"]!["original"]!.GetValue<string>());
        Assert.True(JsonNode.DeepEquals(before["3"], merged["3"]));
        Assert.Equal("", merged["4"]!["translation"]!.GetValue<string>());
        Assert.Equal(new[] { "modified", "added", "removed" }, changes.Select(c => c.Kind));
        Assert.Equal("old", before["1"]!["original"]!.GetValue<string>());
        Assert.Equal(new[] { "original" }, changes[0].Fields);
    }

    [Fact]
    public void MergeRejectsAnUnmappedSourceSchema()
    {
        var before = JsonNode.Parse("""{"1":{"original":"old","translation":"conserva"}}""")!.AsObject();
        var fresh = JsonNode.Parse("""{"1":{"name":"new","translation_name":""}}""")!.AsObject();
        Assert.Throws<InvalidDataException>(() => GameUpdate.Merge("file.json", before, fresh, []));
    }
}
