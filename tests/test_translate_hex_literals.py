import unittest

from tools.translate_hex_literals import control_signature, macro_signature, replace_literals


class TranslateHexLiteralsTests(unittest.TestCase):
    def test_signature_rejects_stray_terminators_and_invalid_utf8(self):
        for raw in (b"text\x03", b"\xff"):
            with self.subTest(raw=raw), self.assertRaises(ValueError):
                control_signature(raw)

    def test_signature_preserves_controls_and_validates_nested_lengths(self):
        original = "<hex:02080EFF0C022B08FF0674687265650303>"
        translated = replace_literals(original, {"three": "quattro"})
        self.assertEqual(macro_signature(original), macro_signature(translated))
        self.assertNotEqual(macro_signature(original), macro_signature(translated.replace("022B", "022C")))
        with self.assertRaises(ValueError):
            macro_signature(translated.replace("FF08", "FF09"))

    def test_replace_literals_updates_nested_macro_and_string_lengths(self):
        original = "<hex:02080EFF0C022B08FF0674687265650303>"

        result = replace_literals(original, {"three": "tre"})

        self.assertEqual(result, "<hex:02080CFF0A022B06FF047472650303>")

    def test_replace_literals_decodes_extended_length_markers(self):
        body = b"\x01" * 248 + b"\xFF\x06three"
        original = "<hex:" + (b"\x02\x08\xF0\xFF" + body + b"\x03").hex() + ">"

        result = replace_literals(original, {"three": "quattro"})

        self.assertTrue(result.startswith("<hex:0208F20101"))
        self.assertTrue(result.endswith("FF087175617474726F03>"))


if __name__ == "__main__":
    unittest.main()
