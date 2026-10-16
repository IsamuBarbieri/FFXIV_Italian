using System.Text;
using FFXIVItalian.TextInspector;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

var temporary = Path.Combine(Path.GetTempPath(), "TextInspector-" + Guid.NewGuid());
Directory.CreateDirectory(temporary);
try
{
    File.WriteAllText(Path.Combine(temporary, "sample.json"), """
        {
          "1": {
            "original": "OK",
            "translation": "Sì"
          },
          "2": {
            "Name": "A<hex:020103>bc",
            "translation_Name": "Nome"
          },
          "3": { "original": "OK", "translation": "Va bene" }
        }
        """, new UTF8Encoding(true));
    File.WriteAllText(Path.Combine(temporary, "invalid.json"), "{");
    var index = TranslationIndex.Load(temporary);
    Check(index.EntryCount == 3, "Incorrect field count");
    Check(index.Find("ok").Count == 2, "Short or duplicate strings lost");
    Check(index.Find("  Sì \n").Single().LineNumber == 3, "Incorrect physical source line");
    Check(index.Find("abc").Single().Field == "Name", "Inline payload broke text matching");
    Check(index.Find("Nome").Single().LineNumber == 7, "Incorrect named field line");
    Check(index.Find(" ").Count == 0, "Empty string matched");
    Check(index.LoadErrors.Count == 1, "Malformed JSON was silently ignored");
    var edit = TranslationEditor.Locate(temporary, index.Find("Sì").Single());
    Check(edit.Field == "translation" && edit.Line == 4, "Editor targeted the source rather than translation");
    var namedEdit = TranslationEditor.Locate(temporary, index.Find("Nome").Single());
    Check(namedEdit.Field == "translation_Name" && namedEdit.Line == 8, "Named translation targeted incorrectly");
    var namedReference = index.Find("Nome").Single();
    var fixturePath = Path.Combine(temporary, "sample.json");
    File.WriteAllText(fixturePath, "\n\n" + File.ReadAllText(fixturePath));
    Check(TranslationEditor.Locate(temporary, namedReference).Line == 10, "Editor used stale indexed lines after file changes");
    var launcher = TranslationEditor.CreateStartInfo("Code.exe", new EditLocation("C:/folder with spaces/file.json", "translation", 4, 5));
    Check(!launcher.UseShellExecute && launcher.ArgumentList.Count == 3 && launcher.ArgumentList[2] == "C:/folder with spaces/file.json:4:5",
        "Editor arguments were split or passed to a shell");
    try
    {
        TranslationEditor.Locate(temporary, namedReference with { FilePath = "data/translations/../outside.json" });
        throw new Exception("Editor accepted a path outside translations");
    }
    catch (InvalidOperationException) { }
    Console.WriteLine("PASS: editor translation fields, fresh line lookup, safe launch arguments and corpus path boundary.");
    var inlinePath = Path.Combine(temporary, "inline.json");
    const string inlineJson = "{\r\n  \"1\": { \"original\": \"Chest à\", \"translation\": \"Forziere\", \"meta\": true },\r\n  \"2\": { \"Name\": \"Other\", \"translation_Name\": \"Altro\" }\r\n}\r\n";
    File.WriteAllText(inlinePath, inlineJson, new UTF8Encoding(true));
    var inlineReference = new TranslationReference("data/translations/inline.json", "1", "original", 1, "", "");
    var inline = new InlineTranslation(temporary, inlineReference);
    Check(inline.SavedValue == "Forziere" && inline.Source == "Chest à", "Inline editor loaded display-normalized text instead of raw values");
    var beforeSave = File.ReadAllBytes(inlinePath);
    var backupDirectory = Path.Combine(temporary, "backups");
    foreach (var empty in new[] { "", " ", "\r\n\t", "\u00A0\u2003" })
    {
        Check(inline.Validate(empty) is not null, "Empty translation was accepted by UI validation");
        try
        {
            inline.Save(empty, backupDirectory);
            throw new Exception("Empty translation was written by the save operation");
        }
        catch (InvalidOperationException) { }
        Check(File.ReadAllBytes(inlinePath).AsSpan().SequenceEqual(beforeSave), "Empty save changed the existing translation");
    }
    Check(!Directory.Exists(backupDirectory), "Blocked empty save created backup files");
    var nodeIdentity = new UiNodeIdentity("Bank", 127, 123, 17, 2963, 0);
    Check(nodeIdentity == new UiNodeIdentity("Bank", 127, 123, 17, 2963, 0), "Same node was treated as a changed selection");
    Check(nodeIdentity != nodeIdentity with { NodeId = 18 } && nodeIdentity != nodeIdentity with { Address = 456 } &&
          nodeIdentity != nodeIdentity with { AddonId = 128 } && nodeIdentity != nodeIdentity with { TextId = 2880 } &&
          nodeIdentity != nodeIdentity with { SheetType = 1 } && nodeIdentity != nodeIdentity with { AddonName = "Other" },
        "Changed or reused UI nodes did not invalidate the editor selection");
    Console.WriteLine("PASS: empty/whitespace saves leave files untouched; node changes invalidate editor identity.");
    const string updatedText = "Cassa \"dell’oro\"\nRiga due";
    var backup = inline.Save(updatedText, backupDirectory);
    Check(backup is not null && File.ReadAllBytes(backup).AsSpan().SequenceEqual(beforeSave), "Inline save backup did not preserve original bytes");
    var afterSave = File.ReadAllBytes(inlinePath);
    var tokenStart = 3 + Encoding.UTF8.GetByteCount(inlineJson[..inlineJson.IndexOf("\"Forziere\"", StringComparison.Ordinal)]);
    var tailLength = beforeSave.Length - tokenStart - Encoding.UTF8.GetByteCount("\"Forziere\"");
    Check(afterSave.AsSpan(0, tokenStart).SequenceEqual(beforeSave.AsSpan(0, tokenStart)) &&
          afterSave.AsSpan(afterSave.Length - tailLength).SequenceEqual(beforeSave.AsSpan(beforeSave.Length - tailLength)),
        "Inline save changed formatting, BOM, CRLF, source or metadata outside the selected JSON value");
    Check(new InlineTranslation(temporary, inlineReference).SavedValue == updatedText, "Quotes, Unicode or newlines were not serialized correctly");
    Check(inline.Save(updatedText, backupDirectory) is null, "Unchanged save created an unnecessary backup");
    File.WriteAllText(inlinePath, File.ReadAllText(inlinePath).Replace("\"Altro\"", "\"Modifica indipendente\"", StringComparison.Ordinal));
    inline.Save("Cassa", backupDirectory);
    Check(File.ReadAllText(inlinePath).Contains("Modifica indipendente"), "Saving clobbered another field edited externally");
    var conflicting = new InlineTranslation(temporary, inlineReference);
    File.WriteAllText(inlinePath, File.ReadAllText(inlinePath).Replace("\"Cassa\"", "\"Modifica esterna\"", StringComparison.Ordinal));
    var conflictBytes = File.ReadAllBytes(inlinePath);
    try
    {
        conflicting.Save("Sovrascrittura", backupDirectory);
        throw new Exception("Inline save ignored an external change to the same field");
    }
    catch (InvalidOperationException) { }
    Check(File.ReadAllBytes(inlinePath).AsSpan().SequenceEqual(conflictBytes), "Conflict handling modified the file");
    File.WriteAllText(inlinePath, "{\"1\":{\"original\":\"Name<hex:020103>\",\"translation\":\"Nome<hex:020103>\"}}");
    var payloadEditor = new InlineTranslation(temporary, inlineReference);
    Check(payloadEditor.Validate("Titolo<hex:020103>") is null && payloadEditor.Validate("Titolo") is not null,
        "Inline edit failed to preserve existing control payloads");
    Check(InlineTranslation.CanPreview("Forziere della compagnia") && !InlineTranslation.CanPreview("Nome<hex:020103>"),
        "Dynamic payloads were accepted by the plain text preview");
    File.WriteAllText(inlinePath, "{\"1\":{\"original\":\"A\",\"translation\":\"B\",\"translation\":\"C\"}}");
    try
    {
        _ = new InlineTranslation(temporary, inlineReference);
        throw new Exception("Inline editor accepted ambiguous duplicate fields");
    }
    catch (InvalidOperationException) { }
    // These fixtures intentionally contain duplicate/invalid data; keep corpus checks separate.
    File.Delete(inlinePath);
    Console.WriteLine("PASS: inline save changes one value, preserves BOM/CRLF, escapes Unicode, backs up, retains external edits and rejects conflicts/payload damage.");
    File.WriteAllText(Path.Combine(temporary, "truncated.json"), """
        {"10":{"original":"Company Chest","translation":"Forziere della Compagnia"},
         "11":{"original":"Company Chest","translation":"Forziere della Compagnia"},
         "12":{"original":"Forziere...","translation":"Literal ellipsis"}}
        """);
    var abbreviatedIndex = TranslationIndex.Load(temporary);
    Check(abbreviatedIndex.Find("Forziere della Com…", out var abbreviated).Count == 2 && abbreviated,
        "Ellipsis should preserve both possible source rows");
    Check(abbreviatedIndex.Find("Forziere della Com...", out abbreviated).Count == 2 && abbreviated,
        "Three-dot truncation not recognized");
    Check(abbreviatedIndex.Find("Forziere...", out abbreviated).Single().RowId == "12" && !abbreviated,
        "An actual ellipsis string must take priority over prefix guesses");
    Check(abbreviatedIndex.Find("Forziere della Com").Count == 0, "Unmarked text must not become an arbitrary prefix guess");
    Check(abbreviatedIndex.Find("Fo...").Count == 0, "Short prefix should not create noisy results");
    var origins = new TextOriginIndex();
    origins.Observe(123, 10, "Forziere della Compagnia");
    Check(!origins.TryResolveNode(789, 123, "Forziere della Com…", "Forziere della Compagnia", out _),
        "A getter alone must not attribute a previously created node");
    origins.BindNode(789, 123, 10);
    Check(origins.TryResolveNode(789, 123, "Forziere della Com…", "Forziere della Compagnia", out var nodeRow) && nodeRow == 10,
        "Getter-to-setter binding did not identify the specific node");
    Check(!origins.TryResolveNode(790, 123, "Forziere della Com…", "Forziere della Compagnia", out _),
        "A binding leaked to a different UI node");
    Check(!origins.TryResolveNode(789, 456, "Forziere della Com…", "Forziere della Compagnia", out _),
        "A replaced original pointer retained the node's old attribution");
    origins.ClearNode(789);
    Check(!origins.TryResolveNode(789, 123, "Forziere della Com…", "Forziere della Compagnia", out _),
        "An unrelated SetText did not invalidate old provenance");
    Check(origins.TryResolve(123, "Forziere della Com…", "Forziere della Compagnia", out var sourceRow) && sourceRow == 10,
        "Observed pointer should identify one row even when text is duplicated");
    Check(!origins.TryResolve(456, "Forziere della Compagnia", "Forziere della Compagnia", out _),
        "Text equality alone must not invent provenance");
    Check(!origins.TryResolve(123, "Unrelated", "Forziere della Compagnia", out _), "Wrong node text retained stale provenance");
    Check(!origins.TryResolve(123, "Forziere della Com…", "Overwritten buffer", out _), "Reused buffer retained stale provenance");
    origins.Observe(123, 11, "Forziere della Compagnia");
    origins.BindNode(789, 123, 11);
    Check(!origins.TryResolveNode(789, 123, "Forziere della Compagnia", "Forziere della Compagnia", out _),
        "An ambiguous shared pointer was attached to a node");
    Check(!origins.TryResolve(123, "Forziere della Compagnia", "Forziere della Compagnia", out _),
        "A pointer observed for multiple rows must remain ambiguous");
    Console.WriteLine("PASS: abbreviated text, exact-match priority, runtime pointer attribution, stale buffers and ambiguity.");
    Directory.CreateDirectory(Path.Combine(temporary, "system"));
    File.WriteAllText(Path.Combine(temporary, "system", "addon.json"), """
        {"2963":{"original":"Company Chest","translation":"Forziere"},
         "2880":{"original":"Company Chest","translation":"Forziere della Compagnia"}}
        """);
    var editedCorpus = TranslationIndex.Load(temporary);
    Check(editedCorpus.FindDeclaredRow("Addon", 2963, "Forziere della Com…").Count == 0,
        "An unsupported text mismatch should not be silently trusted");
    var loadedRow = editedCorpus.FindDeclaredRow("Addon", 2963, "Forziere della Com…", "Forziere della Compagnia");
    Check(loadedRow.Count == 1 && loadedRow[0].RowId == "2963" && loadedRow[0].Translation == "Forziere",
        "Client patch text did not preserve exact ULD row attribution after editing the JSON");
    Check(editedCorpus.FindDeclaredRow("Addon", 2963, "Forziere della Compagnia", "Forziere della Compagnia").Count == 1,
        "Full old client text failed attribution to the updated JSON field");
    Check(editedCorpus.FindDeclaredRow("Addon", 2963, "Altro testo", "Forziere della Compagnia").Count == 0,
        "A genuinely repurposed node was attributed to stale ULD text");
    Check(editedCorpus.FindDeclaredRow("Addon", 2963, "Forziere della Com…", "Testo diverso").Count == 0,
        "An unrelated client row was accepted as supporting evidence");
    Console.WriteLine("PASS: updated JSON versus older client patch retains exact structural row, while unrelated text remains rejected.");
    using var cancellation = new CancellationTokenSource();
    cancellation.Cancel();
    try
    {
        TranslationIndex.Load(temporary, cancellation.Token);
        throw new Exception("Index loading ignored cancellation");
    }
    catch (OperationCanceledException) { }
    Console.WriteLine("PASS: BOM, short strings, duplicates, normalization, payloads, named fields, physical lines and load errors.");
}
finally
{
    Directory.Delete(temporary, true);
}

if (args.Length > 0)
{
    var corpus = TranslationIndex.Load(Path.GetFullPath(args[0]));
    Check(corpus.FileCount > 0 && corpus.EntryCount > 0, "Empty corpus");
    Check(corpus.Find("OK").Any(x => x.FilePath == "data/translations/system/addon.json" && x.RowId == "1" && x.LineNumber == 3), "Actual Addon row not found");
    var chest = corpus.Find("Forziere della Com…", out var isAbbreviated);
    Check(isAbbreviated && chest.Count >= 2 && chest.All(x => x.Translation.StartsWith("Forziere della Com", StringComparison.OrdinalIgnoreCase)),
        "Screenshot regression: company chest truncation");
    var chestRow = chest.First(x => x.FilePath == "data/translations/system/addon.json");
    var exactRow = corpus.FindAddonRow(uint.Parse(chestRow.RowId));
    Check(exactRow.Count == 1 && exactRow[0].RowId == chestRow.RowId, "Runtime row lookup did not disambiguate Addon duplicates");
    var bankCurrent = corpus.FindAddonRow(2963).Single();
    var bankLabel = corpus.FindDeclaredRow("Addon", 2963, bankCurrent.Translation);
    Check(bankLabel.Count == 1 && bankLabel[0].RowId == "2963" && bankLabel[0].FilePath == "data/translations/system/addon.json",
        "Bank node 17's ULD TextId should select Addon#2963, excluding item and unrelated Addon rows");
    Check(corpus.FindDeclaredRow("Addon", 2963, bankCurrent.Original).Count == 1,
        "Full text did not validate the ULD reference");
    Check(corpus.FindDeclaredRow("Addon", 2963, "Testo sostituito a runtime").Count == 0,
        "A stale declared TextId incorrectly overrode current text");
    Check(corpus.FindDeclaredRow("Addon", 2882, "Forziere della Com…").Count == 0,
        "An incompatible structural row was accepted");
    Check(corpus.FindDeclaredRow("UnknownSheet", 2963, "Forziere della Com…").Count == 0,
        "An unknown sheet was silently treated as Addon");
    foreach (var error in corpus.LoadErrors) Console.Error.WriteLine(error);
    Check(corpus.LoadErrors.Count == 0, "Corpus files could not be indexed");
    Console.WriteLine($"PASS: real corpus: {corpus.FileCount} files, {corpus.EntryCount} fields.");
}
