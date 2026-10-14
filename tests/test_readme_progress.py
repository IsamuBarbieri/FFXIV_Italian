from datetime import date
import unittest
import xml.etree.ElementTree as ET

from tools.update_readme_progress import parse_status, render


class ReadmeProgressTests(unittest.TestCase):
    def report(self, separator="."):
        total = f"1{separator}000"
        return "\n".join([
            f"TOTALE PARZIALE/COMPLESSIVO {total} 400 600 40,0%",
            *[f"Quests {key} (1 missioni) 100 0 100 0,0% [DA FARE]"
              for key in ("ARR", "HEAVENSWARD", "STORMBLOOD", "SHADOWBRINGERS", "ENDWALKER", "DAWNTRAIL")],
        ])

    def test_both_locales_keep_total_and_quest_groups_without_double_counting(self):
        for separator in (".", ","):
            with self.subTest(separator=separator):
                total, complete, shared, quests = parse_status(self.report(separator))
                self.assertEqual((1000, 400, (400, 400)), (total, complete, shared))
                self.assertEqual(6, len(quests))

    def test_invalid_totals_and_missing_expansions_are_rejected(self):
        for report in (self.report().replace("400 600", "400 500"), self.report().replace("Quests ARR", "Quests UNKNOWN")):
            with self.assertRaises(ValueError):
                parse_status(report)

    def test_generated_svg_is_valid_and_contains_counts_and_date(self):
        svg = render(self.report(), date(2026, 10, 14))
        ET.fromstring(svg)
        self.assertIn("400 / 1.000", svg)
        self.assertIn("40,0%", svg)
        self.assertIn("14.10.2026", svg)


if __name__ == "__main__":
    unittest.main()
