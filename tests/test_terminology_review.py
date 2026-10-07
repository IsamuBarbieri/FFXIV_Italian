"""Focused checks for the human approval boundary and JSON preservation."""

import argparse
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

from tools import terminology_review as review


class TerminologyReviewTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.glossary_function = staticmethod(review.glossary)
        cls.canonical_glossary = review.glossary()

    def scan_at(self, args):
        for name, default in (("approved_only", False), ("include_review", False),
                              ("all_matches", False), ("strict_canonical_case", False),
                              ("checklist", "__missing_checklist__.md")):
            if not hasattr(args, name):
                setattr(args, name, default)
        with patch.object(review, "glossary", return_value=self.canonical_glossary):
            review.scan(args)

    def test_all_places_are_available_and_conflicting_names_are_normalized(self):
        _, entries = review.glossary()
        places = [entry for entry in entries if entry["reference"].startswith("world/placename.json#")]
        expected = json.loads(review.PLACE_NAMES.read_text(encoding="utf-8-sig"))
        self.assertEqual(len(expected), len({entry["reference"] for entry in places}))
        self.assertEqual({"Fortezza Oscura di Dzemael"},
                         {entry["italian"] for entry in places if entry["english"] == "Dzemael Darkhold"})

    def test_general_scan_catches_an_unlisted_place(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            (root / "draft.json").write_text(
                json.dumps({"1": {"original": "Travel to New Gridania.",
                                  "translation": "Viaggia verso Gridania Nuova."}}), encoding="utf-8")
            output = root / "queue.json"
            args = argparse.Namespace(file="draft.json", term=None, old=None,
                                      include_approved=False, limit=10, out=str(output), overwrite=False)
            with patch.object(review, "TRANSLATIONS", root):
                self.scan_at(args)
            queue = json.loads(output.read_text(encoding="utf-8"))
            self.assertEqual(["Nuova Gridania"], queue[0]["canonical"])
            self.assertIn("world/placename.json#52:name", queue[0]["references"])

    def test_targeted_scan_uses_resolved_place_form(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            (root / "draft.json").write_text(
                json.dumps({"1": {"original": "Enter Dzemael Darkhold.",
                                  "translation": "Entra nella Fortezza di Dzemael."}}), encoding="utf-8")
            output = root / "queue.json"
            args = argparse.Namespace(file="draft.json", term="Dzemael Darkhold", old=None,
                                      include_approved=False, limit=10, out=str(output), overwrite=False)
            with patch.object(review, "TRANSLATIONS", root):
                self.scan_at(args)
            queue = json.loads(output.read_text(encoding="utf-8"))
            self.assertEqual(["Fortezza Oscura di Dzemael"], queue[0]["canonical"])

    def test_scan_finds_contextual_name_without_rewriting(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            source = root / "draft.json"
            source.write_text(json.dumps({"1": {"original": "Open Duty Finder", "translation": "Apri il Cercatore"}}, indent=2), encoding="utf-8")
            output = root / "queue.json"
            args = argparse.Namespace(file="draft.json", term="Duty Finder", old=None,
                                      include_approved=False, limit=10, out=str(output), overwrite=False)
            with patch.object(review, "TRANSLATIONS", root):
                self.scan_at(args)
            queue = json.loads(output.read_text(encoding="utf-8"))
            self.assertEqual(1, len(queue))
            self.assertEqual(["Ricerca Incarichi"], queue[0]["canonical"])
            self.assertEqual("pending", queue[0]["status"])
            self.assertIn("Cercatore", source.read_text(encoding="utf-8"))

    def test_scan_finds_glamours_in_prose(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            (root / "draft.json").write_text(
                json.dumps({"1": {"original": "These glamours affect gear.",
                                  "translation": "Applica glamour all'equipaggiamento."}}), encoding="utf-8")
            output = root / "queue.json"
            args = argparse.Namespace(file="draft.json", term="Glamours", old=None,
                                      include_approved=False, limit=10, out=str(output), overwrite=False)
            with patch.object(review, "TRANSLATIONS", root):
                self.scan_at(args)
            self.assertEqual("Glamours", json.loads(output.read_text(encoding="utf-8"))[0]["english"])

    def test_only_approved_proposal_is_applied_and_other_bytes_stay_put(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            source = root / "draft.json"
            original = '{\n  "1": {\n    "original": "Open Duty Finder",\n    "translation": "Apri il Cercatore"\n  },\n  "2": {\n    "original": "Cancel",\n    "translation": "Annulla"\n  }\n}\n'
            original_bytes = original.replace("\n", "\r\n").encode("utf-8")
            source.write_bytes(original_bytes)
            queue = [{"file": "draft.json", "row_id": "1", "source_field": "original",
                      "target_field": "translation", "original": "Open Duty Finder",
                      "translation": "Apri il Cercatore", "suggestion": "Apri Ricerca Incarichi",
                      "status": "pending"}]
            queue_path = root / "queue.json"
            queue_path.write_text(json.dumps(queue), encoding="utf-8")
            with patch.object(review, "TRANSLATIONS", root):
                review.apply(argparse.Namespace(review=str(queue_path), dry_run=False))
                self.assertEqual(original_bytes, source.read_bytes())
                queue[0]["status"] = "approved"
                queue_path.write_text(json.dumps(queue), encoding="utf-8")
                review.apply(argparse.Namespace(review=str(queue_path), dry_run=True))
                self.assertEqual(original_bytes, source.read_bytes())
                review.apply(argparse.Namespace(review=str(queue_path), dry_run=False))
                self.assertEqual(original_bytes.replace(b"Apri il Cercatore", b"Apri Ricerca Incarichi"), source.read_bytes())
                with self.assertRaises(ValueError):
                    review.apply(argparse.Namespace(review=str(queue_path), dry_run=False))

    def test_tags_must_stay_identical(self):
        self.assertTrue(review.same_tokens("<hex:AA>Test {name}", "<hex:AA>Prova {name}"))
        self.assertFalse(review.same_tokens("<hex:01>Test", "<hex:02>Prova"))

    def test_old_italian_finds_impact_even_without_english_name(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            (root / "draft.json").write_text(
                json.dumps({"1": {"original": "Return there.",
                                  "translation": "Torna alle Sabbie del Risveglio."}}, indent=2), encoding="utf-8")
            output = root / "queue.json"
            args = argparse.Namespace(file="draft.json", term="The Waking Sands", old="Sabbie del Risveglio",
                                      include_approved=False, limit=10, out=str(output), overwrite=False)
            with patch.object(review, "TRANSLATIONS", root):
                self.scan_at(args)
            queue = json.loads(output.read_text(encoding="utf-8"))
            self.assertEqual(1, len(queue))
            self.assertEqual("The Waking Sands", queue[0]["english"])

    def test_numbered_linkshell_uses_the_source_number(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            (root / "draft.json").write_text(
                json.dumps({"1": {"original": "Cross-world Linkshell [3]",
                                  "translation": "Fonosfera [3]"}}, indent=2), encoding="utf-8")
            output = root / "queue.json"
            args = argparse.Namespace(file="draft.json", term=None, old=None,
                                      include_approved=False, limit=10, out=str(output), overwrite=False)
            with patch.object(review, "TRANSLATIONS", root):
                self.scan_at(args)
            queue = json.loads(output.read_text(encoding="utf-8"))
            self.assertEqual(1, len(queue))
            self.assertEqual("Cross-world Linkshell [3]", queue[0]["english"])
            self.assertEqual(["Fonoperla Intermondo [3]"], queue[0]["canonical"])

    def test_approved_sources_are_derived_from_translation_directory(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            translations = root / "translations"
            (translations / "system").mkdir(parents=True)
            (translations / "system" / "sample.json").write_text(
                json.dumps({"1": {"original": "Cancel", "translation": "Annulla"}}), encoding="utf-8")
            glossary_path = root / "Glossary.md"
            glossary_path.write_text(
                "## Voci\n\n### Interfaccia\n\n"
                "| Inglese | Italiano | Fonte | Uso |\n| --- | --- | --- | --- |\n"
                "| Cancel | Annulla | `system/sample.json#1:original` |  |\n"
                "| Confirm | Conferma | Decisione dell'utente |  |\n", encoding="utf-8")

            with patch.object(review, "TRANSLATIONS", translations), \
                    patch.object(review, "GLOSSARY", glossary_path):
                approved, entries = self.glossary_function()

            self.assertEqual({"system/sample.json"}, approved)
            self.assertEqual(["system/sample.json#1:original", "Decisione dell'utente"],
                             [entry["reference"] for entry in entries])


if __name__ == "__main__":
    unittest.main()
