"""Focused checks for the human approval boundary and JSON preservation."""

import argparse
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

from tools import terminology_review as review


class TerminologyReviewTests(unittest.TestCase):
    def test_scan_finds_contextual_name_without_rewriting(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            source = root / "draft.json"
            source.write_text(json.dumps({"1": {"original": "Open Duty Finder", "translation": "Apri il Cercatore"}}, indent=2), encoding="utf-8")
            output = root / "queue.json"
            args = argparse.Namespace(file="draft.json", term="Duty Finder", old=None,
                                      include_approved=False, limit=10, out=str(output), overwrite=False)
            with patch.object(review, "TRANSLATIONS", root):
                review.scan(args)
            queue = json.loads(output.read_text(encoding="utf-8"))
            self.assertEqual(1, len(queue))
            self.assertEqual(["Ricerca Incarichi"], queue[0]["canonical"])
            self.assertEqual("pending", queue[0]["status"])
            self.assertIn("Cercatore", source.read_text(encoding="utf-8"))

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
        self.assertFalse(review.same_tokens("<hex:AA>Test", "<hex:BB>Prova"))
        self.assertTrue(review.small_revision("Vai a Sabbie del Risveglio.", "Vai alle Sabbie del Risveglio."))
        self.assertFalse(review.small_revision("Registratore Incarichi " * 40, "Controllo Prontezza " * 40))

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
                review.scan(args)
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
                review.scan(args)
            queue = json.loads(output.read_text(encoding="utf-8"))
            self.assertEqual(1, len(queue))
            self.assertEqual("Cross-world Linkshell [3]", queue[0]["english"])
            self.assertEqual(["Fonoperla Intermondo [3]"], queue[0]["canonical"])


if __name__ == "__main__":
    unittest.main()
