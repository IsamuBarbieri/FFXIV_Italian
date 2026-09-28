"""Hex text checks for the translation audit."""

import unittest
import json
from pathlib import Path
import tempfile

from scripts.scan_corrupted_values import scan_file, scan_string


class HexTranslationTests(unittest.TestCase):
    def test_reports_unchanged_readable_text_inside_hex(self):
        original = "<hex:02080DFF0B636F6D62696E6174696F6E03>"
        self.assertIn("untranslated text in hex tag 1: combination",
                      scan_string(original, original, english_hex=True))

    def test_ignores_translated_text_and_internal_identifier(self):
        source = "<hex:02080DFF0B636F6D62696E6174696F6E03>"
        target = "<hex:020809FF07636F6D70696C6103>"
        self.assertFalse(any("untranslated text" in issue for issue in scan_string(target, source, english_hex=True)))
        identifier = "<hex:02310CFF054974656D03E802020203>"
        self.assertFalse(any("untranslated text" in issue for issue in scan_string(identifier, identifier, english_hex=True)))

    def test_checks_translation_name_pair(self):
        tag = "<hex:02080DFF0B636F6D62696E6174696F6E03>"
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "names.json"
            path.write_text(json.dumps({"1": {"name": tag, "translation_name": tag}}), encoding="utf-8")
            findings = scan_file(path, compare_translation=True, originals_only=False, english_hex=True)
        self.assertEqual("$.1.translation_name", findings[0]["path"])

    def test_malformed_hex_is_reported_without_crashing(self):
        self.assertIn("hex tag with odd number of digits",
                      scan_string("<hex:0208F>", "<hex:0208F>", english_hex=True))


if __name__ == "__main__":
    unittest.main()
