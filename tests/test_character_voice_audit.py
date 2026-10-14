import struct
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from character_voice_audit import VoiceIndex, classify, parse_label, read_index


class CharacterVoiceAuditTests(unittest.TestCase):
    def test_directions_and_unvoiced_rows_do_not_become_dialogue(self):
        self.assertEqual(classify("TEXT_VOICEMAN_07000_000010_WUKLAMAT", "[TRIUMPHANT ROAR]"), "direction")
        self.assertEqual(classify("TEXT_VOICEMAN_07000_000010_VALIGARMANDA", "(-???-)[Roar]"), "direction")
        self.assertEqual(classify("TEXT_VOICEMAN_07000_000010_000_001_NONE_VOICE", "Choose your answer."), "explicit_no_voice")
        self.assertEqual(classify("TEXT_VOICEMAN_05005_000010_BISMARCK", "[Long has it been since my name was called.]"), "dialogue_candidate")
        self.assertEqual(classify("TEXT_VOICEMAN_07000_000010_BAKOOLJAJA", '[HEARTY LAUGHTER - “Gwahahaha!”]'), "direction")
        self.assertEqual(classify("TEXT_VOICEMAN_05100_000010_ALISAIE", "[Report if label displays]"), "direction")
        self.assertEqual(classify("TEXT_VOICEMAN_07510_000010_SHANTOTTO", "Ohohoho!"), "vocalization_candidate")

    def test_empty_placeholders_do_not_confirm_audio(self):
        with tempfile.TemporaryDirectory() as directory:
            index_path = Path(directory) / "030000.win32.index2"
            dat_path = index_path.with_suffix(".dat0")
            index = VoiceIndex.__new__(VoiceIndex)
            index.containers = {}
            index.streams = {}
            try:
                # Location 0x10 corresponds to byte offset 128 in dat0.
                data = bytearray(256)
                struct.pack_into("<IIIIII", data, 128, 128, 2, 0, 0, 0, 0)
                dat_path.write_bytes(data)
                self.assertFalse(index._is_scd(index_path, 0x10))
            finally:
                index.close()

    def test_scd_signature_and_zero_filled_placeholders(self):
        with tempfile.TemporaryDirectory() as directory:
            index = VoiceIndex.__new__(VoiceIndex)
            index.containers = {}
            index.streams = {}
            try:
                for name, payload, expected in (("valid", b"SEDBSSCF", True), ("empty", bytes(8), False)):
                    path = Path(directory) / (name + ".index2")
                    data = bytearray(280)
                    struct.pack_into("<IIIIII", data, 128, 128, 2, 8, 0, 0, 1)
                    struct.pack_into("<IHH", data, 152, 0, 24, 8)
                    struct.pack_into("<IIII", data, 256, 16, 0, 32000, 8)
                    data[272:280] = payload
                    path.with_suffix(".dat0").write_bytes(data)
                    self.assertEqual(index._is_scd(path, 0x10), expected)
            finally:
                index.close()

    def test_extended_labels_preserve_alias_and_sequence(self):
        self.assertEqual(parse_label("TEXT_VOICEMAN_02200_A00010_MINFILIA"), ("02200", "A00010", "MINFILIA"))
        self.assertEqual(parse_label("TEXT_VOICEMAN_07000_000010_SYSTEM_000"), ("07000", "000010", "SYSTEM_000"))
        with self.assertRaises(ValueError):
            parse_label("TEXT_UNKNOWN_000010_YSHTOLA")

    def test_index_checks_bounds_and_returns_location(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "fixture.index2"
            data = bytearray(0x88)
            data[:6] = b"SqPack"
            struct.pack_into("<I", data, 12, 0x40)
            struct.pack_into("<II", data, 0x48, 0x80, 8)
            struct.pack_into("<II", data, 0x80, 123, 456)
            path.write_bytes(data)
            self.assertEqual(read_index(path, 8), {123: 456})
            struct.pack_into("<I", data, 0x4C, 16)
            path.write_bytes(data)
            with self.assertRaises(ValueError):
                read_index(path, 8)


if __name__ == "__main__":
    unittest.main()
