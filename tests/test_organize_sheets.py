import json
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch


sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from organize_sheets import area
import organize_sheets


class OrganizeSheetsTests(unittest.TestCase):
    def test_two_inputs_cannot_be_planned_for_the_same_destination(self):
        with tempfile.TemporaryDirectory() as folder:
            data = Path(folder)
            pending, review, approved = (data / name for name in ("da_tradurre", "da_revisionare", "translations"))
            files = [pending / "addontransient.json", review / "addontransient.json"]
            for file in files:
                file.parent.mkdir(parents=True, exist_ok=True)
                file.write_text(json.dumps({"1": {"original": "Cancel", "translation": ""}}), encoding="utf-8")
            with patch.object(organize_sheets, "DATA", data), \
                    patch.object(organize_sheets, "TRANSLATIONS", approved), \
                    patch.object(organize_sheets, "EDITORIAL_STATES", (pending, review)):
                with self.assertRaises(FileExistsError):
                    organize_sheets.main()
            self.assertTrue(all(file.exists() for file in files))
            self.assertFalse((pending / "system" / "addontransient.json").exists())

    def test_representative_catalog_sheets_have_useful_areas(self):
        examples = {
            "QuestRedoChapterUI": "quests",
            "AddonTransient": "system",
            "ItemSeries": "items",
            "TripleTriadCard": "minigames",
            "HousingPlacement": "housing",
            "WKSMissionText": "crafting",
        }
        for name, expected in examples.items():
            with self.subTest(name=name):
                self.assertEqual(area(name, ("da_tradurre", "misc", name + ".json")), expected)

    def test_existing_area_is_preserved(self):
        self.assertEqual(area("unknown", ("da_revisionare", "combat", "unknown.json")), "combat")

    def test_translations_directory_is_the_approval_source(self):
        with tempfile.TemporaryDirectory() as folder:
            data = Path(folder) / "data"
            translations = data / "translations"
            pending = data / "da_tradurre"
            review = data / "da_revisionare"
            approved_file = translations / "combat" / "incomplete.json"
            review_file = review / "combat" / "review.json"
            pending_file = pending / "combat" / "pending.json"

            for path, row in (
                (approved_file, {"original": "Cancel"}),
                (review_file, {"original": "Close", "translation": "Chiudi"}),
                (pending_file, {"original": "Confirm"}),
            ):
                path.parent.mkdir(parents=True, exist_ok=True)
                path.write_text(json.dumps({"1": row}), encoding="utf-8")

            with patch.object(organize_sheets, "DATA", data), \
                    patch.object(organize_sheets, "TRANSLATIONS", translations), \
                    patch.object(organize_sheets, "EDITORIAL_STATES", (pending, review)):
                organize_sheets.main()

            self.assertTrue(approved_file.exists())
            self.assertFalse((review / "combat" / "incomplete.json").exists())
            self.assertTrue(review_file.exists())
            self.assertTrue(pending_file.exists())


if __name__ == "__main__":
    unittest.main()
