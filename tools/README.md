# Generatore texture della creazione del personaggio

Esegui dalla cartella del progetto:

```powershell
pwsh -File .\tools\generate_clan_textures.ps1
```

Il tool disegna i titoli in maiuscolo con Cinzel, installato nei font utente o di Windows, e li rende a doppia risoluzione. Genera le texture standard e HR1 in `data/assets/ui/icon/126000/en`, pronte per il packager.

Nomi, altezza visibile, margine sinistro e colore dell'alone sono configurabili in `clan_texture_labels.json`. Cinzel Regular ha il centro bianco e un alone azzurro sfumato, misurato dalla texture originale.

| Texture | Righe `Lobby` nell'EXD | Contenuto |
| --- | --- | --- |
| `126011–126082` | `122–144`, `462–468` | Clan |
| `126101` | `1973` | Calendario Eorzeano |
| `126301–126308` | `178, 180, 182, 184, 186, 188, 190, 192` | Classi iniziali |
| `126310–126312` | `1804–1806` | Ruoli |
| `126501` | `2025` | Mondo |

La frase sui bonus EXP è nella riga `820` di `Lobby`, dentro un marcatore SeString. La traduzione è in `data/translations/system/lobby.json` e viene inclusa nel file EXD dal packager.

Per visualizzare i `.tex` con Foto o Paint, esportali in PNG:

```powershell
pwsh -File .\tools\export_clan_texture_previews.ps1
```

Puoi esportare una sola texture aggiungendo `-Ids 126011`.

Le anteprime vengono salvate in `tools/clan_texture_previews`: il PNG semplice mantiene la trasparenza, mentre `_preview.png` mostra la texture su uno sfondo blu simile al pannello di gioco.

`leftInset` è espresso in pixel HR1 e regola il margine interno a sinistra.

## Cartelli delle zone

`AreaTextureTool` estrae le texture originali e rigenera i cartelli con testo italiano, nello stesso formato usato dal client. Usa il font **Jupiter Pro Regular** installato come `Jupiter-Pro.ttf`; le minuscole sono disegnate a 5/6 della dimensione delle maiuscole. Il testo è creato in Photoshop con `JupiterPro`, usando le alternative contestuali del font completo per la N iniziale curva. Photoshop applica direttamente il preset `Zone Style.ASL` tramite `AreaTextureTool/Styles/apply_zone_style.jsx`.

Per ricostruire il catalogo dei cartelli a partire dai nomi tradotti e verificare i percorsi contro l'installazione locale del gioco:

```powershell
dotnet run --project .\tools\AreaTextureTool -- catalog
```

Il catalogo legge gli ID delle texture dai campi `PlaceNameRegionIcon` e `PlaceNameIcon` di `TerritoryType`, controlla formato e dimensioni nel client locale e associa i nomi italiani di `placename.json`. Quando il territorio è legato a una duty e il cartello è specifico dell'attività, usa il nome tradotto in `contentfindercondition.json`. Alcune intestazioni di regione senza una voce in quei due fogli hanno una resa esplicita nel catalogo. I codici SeString vengono rimossi dal testo rasterizzato senza modificare i JSON. La configurazione trovata è salvata in `AreaTextureTool/area_texture_labels.json`.

Per estrarre una texture originale in PNG:

```powershell
dotnet run --project .\tools\AreaTextureTool -- dump ui/icon/123000/en/123202.tex .\tools\fullscreen_texture_previews\original_new_gridania.png
```

Per generare tutte le voci configurate, comprese le varianti HR1 (richiede Photoshop installato):

```powershell
pwsh -File .\tools\generate_area_textures.ps1
```

Le voci individuate producono le versioni standard e HR1 sotto `data/assets`, mantenendo i percorsi originali del gioco; le anteprime PNG finali sono in `tools/fullscreen_texture_previews` e gli intermedi Photoshop vengono rimossi dopo la conversione. Aggiungi una voce a `AreaTextureTool/area_texture_labels.json` per riusare il generatore su altri cartelli. Una configurazione diversa si può specificare allo script con `-ConfigPath`. Per il catalogo e il dump, la cartella sqpack si specifica con `--sqpack`. Parametri del preset e geometria sono documentati in `AreaTextureTool/Styles/README.md`.

Le texture full screen hanno una configurazione separata e usano `FONTSPRINGDEMO-JupiterProBold`; per la punteggiatura usa `JupiterPro-Bold`. Il generatore duplica il livello `This Uses Jupiter Font` dal PSD di riferimento, mantenendo font, rapporto delle minuscole, trasformazione, colore e bevel; ingrandisce il testo 1,7× e calibra il bagliore esterno per evitare che si fonda tra le lettere. Il file `full_screen_texture_labels.json` contiene anche texture compatte da 128 px di altezza oltre agli annunci da 360 px. L’ASL fornito resta invariato perché il preset che Photoshop ne ricava è diverso dal PSD. Se il PSD non è già aperto, il tool lo apre dal percorso predefinito `..\..\Duty Complete font.psd`; si può cambiare percorso con `-FullScreenReferencePsdPath`.

```powershell
pwsh -File .\tools\generate_area_textures.ps1 -ConfigPath .\tools\AreaTextureTool\full_screen_texture_labels.json
```

Il full screen non importa preset in Photoshop e quindi non aggiunge stili duplicati. `-ReloadStyle` riguarda i preset ASL delle zone e delle regioni.

## Maiuscole delle traduzioni

Lo script controlla i fogli completati secondo `dotnet run --project src/FFXIVItalian.Extractor -- status`.
Preserva articoli minuscoli, possessivi inglesi e maiuscole interne dei nomi con apostrofo.
Mostra prima l'anteprima; aggiungi `--apply` per salvare le correzioni:

```powershell
python .\tools\fix_translation_capitalization.py
python .\tools\fix_translation_capitalization.py --apply
python .\tools\fix_translation_capitalization.py --self-test
```
