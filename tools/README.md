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
