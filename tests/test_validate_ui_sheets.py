import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from validate_ui_sheets import check_sestring_controls
from translate_hex_literals import replace_literals
from validate_ui_sheets import UNTRANSLATED_LICENSES


class UiMacroChecksTests(unittest.TestCase):
    def test_only_general_software_notices_are_excluded_from_translation(self):
        self.assertIn(("addontransient", "173", "original"), UNTRANSLATED_LICENSES)
        for row_id in ("247", "1016"):
            self.assertNotIn(("addontransient", row_id, "original"), UNTRANSLATED_LICENSES)

    def test_localized_branches_keep_conditions_and_controls(self):
        original = "<hex:02080EFF0C022B08FF0674687265650303>"
        translated = replace_literals(original, {"three": "quattro"})
        self.assertEqual([], check_sestring_controls(original, translated))
        self.assertTrue(check_sestring_controls(original, translated.replace("022B", "022C")))
        self.assertTrue(check_sestring_controls(original, translated.replace("FF08", "FF09")))
        self.assertTrue(check_sestring_controls(original, ""))

    def test_player_agreement_can_be_added_without_changing_native_conditions(self):
        gender = "<hex:02080FE905FF05736F6C61FF05736F6C6F03>"
        self.assertEqual([], check_sestring_controls("Enter solo.<hex:02100103>", "Entra da " + gender + ".<hex:02100103>"))
        self.assertTrue(check_sestring_controls("Enter solo.", "Entra da " + gender.replace("02080F", "020810")))
        self.assertTrue(check_sestring_controls("Enter solo.", "Entra da " + gender.replace("E905", "E906")))
        self.assertTrue(check_sestring_controls(gender, "Solo"))
        self.assertTrue(check_sestring_controls("Enter solo.", "<hex:02080CE905FF0502100103FF026F03>"))

    def test_fragmented_macros_are_compared_as_a_complete_stream(self):
        fragmented = "<hex:02080EFF0C022B08FF06746872656503><hex:03>"
        complete = "<hex:02080CFF0A022B06FF047472650303>"
        self.assertEqual([], check_sestring_controls(fragmented, complete))


if __name__ == "__main__":
    unittest.main()
