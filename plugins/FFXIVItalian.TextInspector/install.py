"""Register the local Debug build in Dalamud while the game is closed."""
import datetime
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile


def require_game_closed():
    result = subprocess.run(
        ["powershell.exe", "-NoProfile", "-Command",
         "@(Get-Process -Name ffxiv,ffxiv_dx11 -ErrorAction SilentlyContinue).Count"],
        check=True, capture_output=True, text=True,
    )
    if result.stdout.strip() != "0":
        raise SystemExit("Chiudi FFXIV prima dell'installazione: Dalamud potrebbe sovrascrivere la configurazione.")


def main():
    require_game_closed()
    dll = Path(__file__).resolve().parent / "bin" / "Debug" / "FFXIVItalian.TextInspector.dll"
    if not dll.is_file() or not dll.with_suffix(".json").is_file():
        raise SystemExit("Compila prima il progetto in Debug: DLL o manifest mancanti.")
    config = Path(os.environ["APPDATA"]) / "XIVLauncher" / "dalamudConfig.json"
    original = config.read_bytes()
    settings = json.loads(original.decode("utf-8-sig"))
    locations = settings["DevPluginLoadLocations"]["$values"]
    location = next((entry for entry in locations if os.path.normcase(entry["Path"]) == os.path.normcase(str(dll))), None)
    if location is None:
        location = {"$type": "Dalamud.Configuration.DevPluginLocationSettings, Dalamud", "Path": str(dll), "Nickname": None}
        locations.append(location)
    location["IsEnabled"] = True
    plugin_settings = settings["DevPluginSettings"].setdefault(str(dll), {
        "$type": "Dalamud.Configuration.Internal.DevPluginSettings, Dalamud",
    })
    plugin_settings.update(StartOnBoot=True, NotifyForErrors=True, AutomaticReloading=True)
    encoded = json.dumps(settings, ensure_ascii=False, indent=2).encode("utf-8")
    assert json.loads(encoded) == settings
    require_game_closed()
    if config.read_bytes() != original:
        raise SystemExit("Configurazione cambiata durante l'installazione; riprova.")
    stamp = datetime.datetime.now().strftime("%Y%m%d-%H%M%S-%f")
    backup = config.with_name(config.name + ".bak-TextInspector-" + stamp)
    shutil.copy2(config, backup)
    descriptor, temporary = tempfile.mkstemp(dir=config.parent, prefix="TextInspector-", suffix=".tmp")
    try:
        with os.fdopen(descriptor, "wb") as stream:
            stream.write(encoded)
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, config)
    finally:
        if os.path.exists(temporary):
            os.unlink(temporary)
    assert json.loads(config.read_bytes()) == settings
    print(f"Installato: {dll}\nAvvio automatico: attivo\nBackup: {backup}")


if __name__ == "__main__":
    main()
