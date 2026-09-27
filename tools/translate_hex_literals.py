"""Replace literal UTF-8 alternatives inside a SeString hex tag and resize it."""


def replace_literals(tag: str, replacements: dict[str, str]) -> str:
    assert tag.startswith("<hex:02") and tag.endswith(">")
    raw = bytes.fromhex(tag[5:-1])
    marker = raw[2]
    if marker < 0xD0:
        header = 3
        length = marker - 1
    elif marker == 0xF2:
        header = 5
        length = int.from_bytes(raw[3:5], "big")
    elif marker == 0xF0:
        header = 7
        length = int.from_bytes(raw[3:7], "big")
    else:
        raise ValueError(f"Unsupported SeString length marker: {marker:02X}")
    assert len(raw) == header + length + 1 and raw[-1] == 3
    body = raw[header:-1]
    for old, new in replacements.items():
        old_bytes = old.encode("utf-8")
        new_bytes = new.encode("utf-8")
        assert len(old_bytes) < 0xCF and len(new_bytes) < 0xCF
        needle = b"\xff" + bytes([len(old_bytes) + 1]) + old_bytes
        replacement = b"\xff" + bytes([len(new_bytes) + 1]) + new_bytes
        assert body.count(needle) == 1, (old, tag[:30])
        body = body.replace(needle, replacement)
    if header == 3:
        assert len(body) + 1 < 0xD0
        encoded_length = bytes([len(body) + 1])
    else:
        encoded_length = bytes([marker]) + len(body).to_bytes(header - 3, "big")
    result = raw[:2] + encoded_length + body + b"\x03"
    return f"<hex:{result.hex().upper()}>"


def replace_nested_literal(tag: str, old: str, new: str) -> str:
    """Replace text in an FF length-prefixed payload containing control icons."""
    raw = bytes.fromhex(tag[5:-1])
    assert raw[:2] == b"\x02\x08" and raw[2] < 0xD0
    assert len(raw) == raw[2] + 3 and raw[-1:] == b"\x03"
    body = bytearray(raw[3:-1])
    inner = body.index(0xFF)
    end = body.index(0xFF, inner + 1)
    assert body[inner + 1] == end - inner - 1
    old_bytes = old.encode("utf-8")
    assert bytes(body[inner + 2:end]).count(old_bytes) == 1
    body[inner + 2:end] = bytes(body[inner + 2:end]).replace(old_bytes, new.encode("utf-8"))
    body[inner + 1] = body.index(0xFF, inner + 2) - inner - 1
    assert len(body) + 1 < 0xD0
    result = raw[:2] + bytes([len(body) + 1]) + body + b"\x03"
    assert len(result) == result[2] + 3
    return f"<hex:{result.hex().upper()}>"


if __name__ == "__main__":
    original = "<hex:020814E4E80202FF065069656365FF0750696563657303>"
    result = replace_literals(original, {"Piece": "Pezzo", "Pieces": "Pezzi"})
    assert result == "<hex:020813E4E80202FF0650657A7A6FFF0650657A7A6903>"
