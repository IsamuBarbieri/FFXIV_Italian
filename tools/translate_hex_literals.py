"""Replace UTF-8 text in SeString hex tags and resize affected payloads."""


def _decode_uint(data: bytes, offset: int) -> tuple[int, int]:
    first = data[offset]
    if 0 < first < 0xD0:
        return first - 1, 1
    if 0xF0 <= first <= 0xFE:
        mask = (first + 1) & 0x0F
        value = 0
        consumed = 1
        for bit, shift in ((8, 24), (4, 16), (2, 8), (1, 0)):
            if mask & bit:
                if offset + consumed >= len(data):
                    raise ValueError("Truncated SeString integer")
                part = data[offset + consumed]
                if part == 0:
                    raise ValueError("Invalid zero byte in SeString integer")
                value |= part << shift
                consumed += 1
        return value, consumed
    raise ValueError(f"Unsupported SeString integer marker: {first:02X}")


def _encode_uint(value: int) -> bytes:
    if not 0 <= value <= 0xFFFFFFFF:
        raise ValueError("SeString integers must fit in 32 bits")
    if value < 0xCF:
        return bytes([value + 1])

    parts = [(value >> shift) & 0xFF for shift in (24, 16, 8, 0)]
    bits = (8, 4, 2, 1)
    mask = sum(bit for part, bit in zip(parts, bits) if part)
    if not mask:
        return b"\x01"
    return bytes([0xEF + mask, *(part for part in parts if part)])


def _replace_text_run(text: bytes, replacements: dict[bytes, bytes] | None) -> bytes:
    if replacements is None:
        text.decode("utf-8")
        if any(byte < 0x20 and byte not in (9, 10, 13) for byte in text):
            raise ValueError("Control byte outside a SeString macro")
        return b""
    return replacements.get(text, text)


def _transform_expressions(data: bytes, replacements: dict[bytes, bytes] | None) -> bytes:
    output = bytearray()
    offset = 0
    while offset < len(data):
        expression, consumed = _transform_expression(data, offset, replacements)
        output.extend(expression)
        offset += consumed
    return bytes(output)


def _transform_expression(
    data: bytes, offset: int, replacements: dict[bytes, bytes] | None
) -> tuple[bytes, int]:
    code = data[offset]
    if code == 0xFF:
        length, marker_size = _decode_uint(data, offset + 1)
        start = offset + 1 + marker_size
        end = start + length
        if end > len(data):
            raise ValueError("Truncated SeString string expression")
        nested = _transform_sestring(data[start:end], replacements)
        expression = b"\xFF" + _encode_uint(len(nested)) + nested
        return expression, 1 + marker_size + length
    if 0xE8 <= code <= 0xEB:
        child, consumed = _transform_expression(data, offset + 1, replacements)
        return bytes([code]) + child, 1 + consumed
    if 0xE0 <= code <= 0xE5:
        left, left_size = _transform_expression(data, offset + 1, replacements)
        right, right_size = _transform_expression(data, offset + 1 + left_size, replacements)
        return bytes([code]) + left + right, 1 + left_size + right_size
    if 0xD0 <= code <= 0xDF or code == 0xEC:
        return bytes([code]), 1
    _, consumed = _decode_uint(data, offset)
    return data[offset:offset + consumed], consumed


def _transform_sestring(data: bytes, replacements: dict[bytes, bytes] | None) -> bytes:
    output = bytearray()
    offset = 0
    while offset < len(data):
        if data[offset] != 0x02:
            end = data.find(b"\x02", offset)
            if end < 0:
                end = len(data)
            output.extend(_replace_text_run(data[offset:end], replacements))
            offset = end
            continue

        if offset + 2 >= len(data):
            raise ValueError("Truncated SeString macro")
        macro_code = data[offset + 1]
        body_length, marker_size = _decode_uint(data, offset + 2)
        body_start = offset + 2 + marker_size
        body_end = body_start + body_length
        if body_end >= len(data) or data[body_end] != 0x03:
            raise ValueError("Invalid SeString macro length or terminator")
        body = _transform_expressions(data[body_start:body_end], replacements)
        output.extend((0x02, macro_code))
        output.extend(_encode_uint(len(body)))
        output.extend(body)
        output.append(0x03)
        offset = body_end + 1
    return bytes(output)


def replace_literals(tag: str, replacements: dict[str, str]) -> str:
    """Replace exact text runs throughout a macro tree, recalculating its lengths."""
    if not tag.startswith("<hex:") or not tag.endswith(">"):
        raise ValueError("Expected a SeString <hex:...> tag")
    raw = bytes.fromhex(tag[5:-1])
    byte_replacements = {old.encode("utf-8"): new.encode("utf-8") for old, new in replacements.items()}
    result = _transform_sestring(raw, byte_replacements)
    return f"<hex:{result.hex().upper()}>"


def macro_signature(tag: str) -> bytes:
    """Validate macro lengths and compare controls independently of visible text."""
    if not tag.startswith("<hex:") or not tag.endswith(">"):
        raise ValueError("Expected a SeString <hex:...> tag")
    raw = bytes.fromhex(tag[5:-1])
    if not raw.startswith(b"\x02"):
        raise ValueError("Expected a SeString macro")
    return control_signature(raw)


def control_signature(raw: bytes) -> bytes:
    """Validate a complete SeString, including macros split across JSON fragments."""
    try:
        return _transform_sestring(raw, None)
    except IndexError as error:
        raise ValueError("Truncated SeString expression") from error


def player_gender_branches(tag: str) -> tuple[str, str] | None:
    """Recognize only If(gnum4) with two plain UTF-8 branches, female then male."""
    raw = bytes.fromhex(tag[5:-1])
    if raw[:2] != b"\x02\x08":
        return None
    length, marker_size = _decode_uint(raw, 2)
    start = 2 + marker_size
    if raw[start:start + 2] != b"\xe9\x05":
        return None
    macro_signature(tag)
    branches = []
    offset = start + 2
    for _ in range(2):
        if offset >= len(raw) or raw[offset] != 0xFF:
            raise ValueError("If(gnum4) requires two string branches")
        size, consumed = _decode_uint(raw, offset + 1)
        begin = offset + 1 + consumed
        branch = raw[begin:begin + size]
        if any(byte < 0x20 for byte in branch):
            raise ValueError("Additional If(gnum4) branches must contain plain text")
        branches.append(branch.decode("utf-8"))
        offset = begin + size
    if offset != start + length:
        raise ValueError("Unexpected If(gnum4) expressions")
    return branches[0], branches[1]


if __name__ == "__main__":
    original = "<hex:020814E4E80202FF065069656365FF0750696563657303>"
    result = replace_literals(original, {"Piece": "Pezzo", "Pieces": "Pezzi"})
    assert result == "<hex:020813E4E80202FF0650657A7A6FFF0650657A7A6903>"
