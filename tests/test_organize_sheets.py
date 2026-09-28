import sys
import unittest
from pathlib import Path


sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from organize_sheets import area


class OrganizeSheetsTests(unittest.TestCase):
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


if __name__ == "__main__":
    unittest.main()
