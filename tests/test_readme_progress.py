from datetime import date
import json
import unittest
import xml.etree.ElementTree as ET

from tools.update_readme_progress import parse_status, render, release_plan


def sheet(path, total=100, complete=0):
    return {"Path": path, "Stats": {"TotalRows": total, "TranslatedRows": complete, "PendingRows": total - complete}}


class ReadmeProgressTests(unittest.TestCase):
    def report(self):
        return json.dumps([
            sheet("translations/system/addon.json", complete=90),
            sheet("da_revisionare/system/howto.json", complete=100),
            sheet("translations/dialogue/custom/000/test.json", complete=100),
            sheet("da_tradurre/dialogue/cut_scene/075/voiceman_07515.json", complete=10),
            sheet("da_tradurre/quests/master/quest.json"),
            *[sheet(f"da_tradurre/quests/{key}/001/test_{key}.json")
              for key in ("arr", "heavensward", "stormblood", "shadowbringers", "endwalker", "dawntrail")],
        ])

    def test_narrative_is_partitioned_once_and_drafts_are_not_approved(self):
        groups, sheets = parse_status(self.report())
        self.assertEqual([200, 190, 90], groups["core"])
        self.assertEqual([100, 100, 100], groups["dialogue"])
        self.assertEqual([100, 10, 0], groups["cutscenes"])
        self.assertEqual([100, 0, 0], groups["catalogs"])
        self.assertEqual(1100, sum(group[0] for group in groups.values()))
        self.assertEqual(11, len(sheets))

    def test_invalid_counts_unknown_expansion_and_duplicate_states_are_rejected(self):
        bad_counts = sheet("translations/system/addon.json")
        bad_counts["Stats"]["PendingRows"] = 20
        for report in ([bad_counts], [sheet("da_tradurre/quests/new_expansion/001/test.json")],
                       [sheet("translations/system/addon.json"), sheet("da_tradurre/system/addon.json")]):
            with self.subTest(report=report), self.assertRaises(ValueError):
                parse_status(json.dumps(report))

    def test_homonymous_sheets_in_different_game_paths_remain_distinct(self):
        groups, _ = parse_status(json.dumps([
            sheet("translations/world/aetheryte.json"),
            sheet("translations/world/transport/aetheryte.json"),
        ]))
        self.assertEqual(200, groups["core"][0])

    def test_generated_svg_and_release_plan_describe_scope_approval_and_gaps(self):
        svg = render(self.report(), date(2026, 10, 16))
        ET.fromstring(svg)
        self.assertIn("90 / 200", svg)
        self.assertIn("45,0%", svg)
        self.assertIn("16.10.2026", svg)
        self.assertIn("Cutscene e battute doppiate", svg)
        self.assertIn("Titoli, diari e cataloghi delle missioni", svg)
        report = release_plan(self.report(), date(2026, 10, 16))
        self.assertIn("../data/translations/system/addon.json", report)
        self.assertIn("../data/da_revisionare/system/howto.json", report)
        self.assertIn("| 100 | da_revisionare |", report)
        self.assertNotIn("../data/da_tradurre/dialogue/cut_scene", report)


if __name__ == "__main__":
    unittest.main()
