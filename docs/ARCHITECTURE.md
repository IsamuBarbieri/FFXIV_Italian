# Architettura e manutenzione

## Moduli e flusso

| Modulo | Responsabilità |
| --- | --- |
| Core | Lettura del corpus, percorsi, decisioni terminologiche, SeString, EXD e pacchetti. |
| Extractor | Estrazione con Lumina, ricerca/ispezione, stato, validazione, batch e aggiornamento del corpus. |
| DiffTool | Confronto opzionale di due snapshot JSON, distinto dall'aggiornamento del client locale. |
| Patcher | Compilazione dei soli JSON approvati, risorse localizzate, pacchetto Penumbra e deploy locale. |

Mantieni questi confini. Il progetto legge gli archivi SqPack dall'installazione locale e non li modifica. Non versionare dump o asset originali del gioco. Il `.pmp` è un artefatto generato, escluso da Git.

Le tre procedure editoriali sono [Traduzione](TRANSLATION.md), [Revisione](REVISION.md) e [Aggiornamento del client](CLIENT_UPDATE.md). Lo stato dipende dal percorso del JSON, non da percentuali di completamento o esiti dei test.

## Build e verifiche

Prerequisiti: .NET 10 SDK; Python 3.10 o successivo per le utility, che usano la libreria standard. Lumina e i pacchetti di test sono dichiarati nei rispettivi `.csproj`.

```powershell
dotnet test
python -m unittest discover -s tests
```

Alcuni test verificano il corpus approvato; quelli che dipendono dal client locale non attestano compatibilità con un client diverso. I generatori delle texture sono progetti separati dalla soluzione principale.

Il patcher usa come default `G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack`. Il primo argomento posizionale può indicare un'altra cartella SqPack esistente. La cartella di deploy Penumbra è attualmente fissata nel patcher a `G:\SquareEnix\FFXIV_Mod\FFXIV Italiano (Test In-Game)`; non esiste un `appsettings.json` letto dal patcher.

```powershell
dotnet run --configuration Release --project src/FFXIVItalian.Patcher -- <cartella-sqpack>
.\rebuild.ps1
```

Il patcher produce `FFXIV_Italian.pmp`, effettua il deploy se la cartella locale configurata esiste e controlla `IsResumeGameAfterPluginLoad` in Dalamud. I wrapper di rebuild avviano questo stesso processo. Non avviarli durante una semplice revisione linguistica.

## Dati e risorse

`data/translations/` contiene i JSON compilabili. Gli altri stati editoriali non vengono compilati. Le categorie sono activities, combat, crafting, dialogue, housing, items, minigames, quests, shops, social, system e world; i fogli non categorizzati possono usare misc. Le quest narrative mantengono espansione e cartella numerica; i master sono in `quests/master/`.

Le risorse distribuibili sono sotto `data/assets/` oppure nei percorsi di gioco espliciti `data/ui/`, `data/exd/`, `data/common/`, `data/bg/`, `data/char/`, `data/sound/`, `data/vfx/` e `data/shader/`. In `assets/` viene rimosso il prefisso assets; negli altri casi resta il percorso relativo a data. Cartelle di lavoro, batch, glossario, backup e rapporti non vengono incluse.

`python tools/organize_sheets.py` serve soltanto quando viene richiesta una riorganizzazione. Preserva i file approvati, distingue gli altri stati dalla presenza di traduzioni e blocca le collisioni prima di spostare file. Non usarlo come passaggio automatico dopo una revisione o un aggiornamento: può cambiare cartelle e non equivale ad approvazione.

Il glossario è incorporato nel Core in fase di build, insieme al catalogo PlaceName. Dopo modifiche alle decisioni ricompila prima dei controlli C#. Le annotazioni delle colonne e il titolo `## Voci` sono letti dai parser: conserva la struttura delle tabelle e verifica i test del glossario.

## Batch opzionali

Il flusso principale modifica direttamente i campi assegnati. Per batch nel formato JSON del progetto sono disponibili:

```powershell
dotnet run --project src/FFXIVItalian.Extractor -- export-batch <foglio> --size 50
dotnet run --project src/FFXIVItalian.Extractor -- import-batch <foglio> <batch.json>
```

L'import C# gestisce i campi espliciti previsti dal suo codice; non è un importatore generale di ogni `translation_col_*` e scrive il master anche quando segnala problemi. Verifica il formato, conserva il diff e valida il file: il comando non approva il risultato.

Per batch esterni con manifest e colonne dedicate restano `scripts/split_untranslated.py` e `scripts/apply_all_translations.py`. I loro formati non sono intercambiabili con quelli C#. Prima dell'applicazione usa `--validate-only`; il confronto dei tag identici non consente traduzioni intenzionali dei payload o aggiunte di condizioni. Questi casi vanno gestiti direttamente con i controlli SeString.

`autofill` riempie soltanto campi vuoti con equivalenze esatte del glossario prive di varianti concorrenti; non consulta il contesto di una scena. Non usarlo come traduttore di dialoghi.

## Grafica del README

`docs/assets/banner.png` è un'illustrazione originale per la pagina del progetto; `download.svg` è il pulsante di download. Questi asset sono documentazione e non entrano nella mod.

Per aggiornare `docs/assets/progress.svg` dai conteggi correnti dell'estrattore:

```powershell
python tools/update_readme_progress.py
```

Lo script esegue `status` senza filtri, conserva la definizione di riga completa dell'estrattore e separa i fogli delle quest dagli altri contenuti senza duplicarli. Richiede Python e .NET, solo per la manutenzione del grafico. Per un rapporto già salvato usa `--status-file <file>` e `--date YYYY-MM-DD`; verifica la data e committa il grafico insieme alle modifiche del corpus.

## Cataloghi generati

Il catalogo dei fogli mancanti si ricava dal client e dal corpus attuale; non viene versionato:

```powershell
New-Item -ItemType Directory -Force tools/reports | Out-Null
dotnet run --project src/FFXIVItalian.Extractor -- discover | Out-File -Encoding utf8 tools/reports/unextracted_sheets.txt
dotnet run --project src/FFXIVItalian.Extractor -- extract-catalog --catalog tools/reports/unextracted_sheets.txt
```

`extract-catalog` estrae i fogli assenti elencati nel rapporto; non sostituisce `update` per revisionare sorgenti cambiate. Il default è `tools/reports/unextracted_sheets.txt`.

Il comando legacy `reorganize` sposta solo JSON nella radice delle traduzioni e rifiuta sovrascritture; per una riorganizzazione completa richiesta dall'utente usa `tools/organize_sheets.py`.

## Strumenti mantenuti

| Strumento | Uso |
| --- | --- |
| `tools/character_voice_audit.py` | Censimento in sola lettura dei fogli Voiceman e riscontro dei contenitori audio nel client Windows locale. Procedura e limiti in [Censimento del parlato](characters/VOICE_CENSUS.md); rapporti non versionati in `tools/reports/`. |
| `tools/terminology_review.py` | Scansioni mirate, code di revisione e applicazione di proposte approvate. |
| `scripts/scan_corrupted_values.py` | Valori sospetti e inglese invariato nei payload hex. |
| `tools/translate_hex_literals.py` | Ricodifica di segmenti UTF-8 con aggiornamento delle lunghezze annidate. |
| `tools/validate_ui_sheets.py` | Copertura degli otto fogli UI, integrità delle macro speciali e licenze invarianti. |
| `tools/organize_sheets.py` | Riorganizzazione esplicita del corpus con controllo delle collisioni. |
| Generatori di texture | Ricostruzione degli asset italiani quando cambiano testi o client. |

I rapporti temporanei sono ignorati da Git. Le ricette che contenevano traduzioni UI per ID sono state rimosse: il corpus è la fonte dei testi e le utility non devono ripristinare vecchie bozze.

## Contributi e uso dell'AI

Per traduzioni e revisione segui le guide operative e conserva ID, sorgenti e metadati. I controlli tecnici non sostituiscono la revisione umana e non promuovono un file. Registra solo decisioni riutilizzabili e limita le modifiche al perimetro richiesto o alla riconciliazione confermata.

Per codice conserva i confini dei moduli, esegui i test pertinenti e descrivi cambiamenti e verifiche. Non inserire credenziali, dati personali, dump o materiale non autorizzato nei prompt. Il progetto è fan-made, non ufficiale e non affiliato a Square Enix; testi, personaggi e marchi di Final Fantasy XIV appartengono ai rispettivi titolari.

## Texture localizzate

### Creazione del personaggio

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

### Cartelli delle zone

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

Le voci individuate producono le versioni standard e HR1 sotto `data/assets`, mantenendo i percorsi originali del gioco; le anteprime PNG finali sono in `tools/fullscreen_texture_previews` e gli intermedi Photoshop vengono rimossi dopo la conversione. Aggiungi una voce a `AreaTextureTool/area_texture_labels.json` per riusare il generatore su altri cartelli. Una configurazione diversa si può specificare allo script con `-ConfigPath`. Per il catalogo e il dump, la cartella sqpack si specifica con `--sqpack`. Parametri del preset e geometria sono documentati nella sezione Stili dei cartelli qui sotto.

Le texture full screen hanno una configurazione separata e usano `FONTSPRINGDEMO-JupiterProBold`; per la punteggiatura usa `JupiterPro-Bold`. Il generatore duplica il livello `This Uses Jupiter Font` dal PSD di riferimento, mantenendo font, rapporto delle minuscole, trasformazione, colore e bevel; ingrandisce il testo 1,7× e calibra il bagliore esterno per evitare che si fonda tra le lettere. Il file `full_screen_texture_labels.json` contiene anche texture compatte da 128 px di altezza oltre agli annunci da 360 px. L'ASL fornito resta invariato perché il preset che Photoshop ne ricava è diverso dal PSD. Se il PSD non è già aperto, il tool lo apre dal percorso predefinito `..\..\Duty Complete font.psd`; si può cambiare percorso con `-FullScreenReferencePsdPath`.

```powershell
pwsh -File .\tools\generate_area_textures.ps1 -ConfigPath .\tools\AreaTextureTool\full_screen_texture_labels.json
```

Il full screen non importa preset in Photoshop e quindi non aggiunge stili duplicati. `-ReloadStyle` riguarda i preset ASL delle zone e delle regioni.

### Stili dei cartelli

- `Zone Style.ASL`: preset Photoshop «Zone Style», rifinito sul dump di Nuova Gridania. Il punto più giallo del gradiente usa il campione originale RGB `255/255/148`; il bevel bianco è leggermente più largo.
- `Region Style.ASL`: copia del preset di zona con nome interno `Region Scl` e il Bagliore esterno ridotto in proporzione al font regionale. Estensione e sfocatura passano rispettivamente da 28/57 px a 11/22,3 px, usando il rapporto tra le maiuscole regionali (26 px) e quelle delle zone (66,3606557377 px). Colori, opacità, bevel e gli altri effetti restano identici.
- `FF Full Screen.ASL`: preset fornito dall'utente e conservato invariato. Photoshop lo applicava con riempimento grigio e sfumatura disattivata, diverso dal livello di riferimento nel PSD. Per i full screen il generatore duplica quindi il livello `This Uses Jupiter Font` da `Duty Complete font.psd` e sostituisce il testo, conservando riempimento, trasformazione, gradienti e bevel senza creare preset duplicati. Il testo viene ingrandito di 1,7× per allinearsi all'esportazione approvata; il Bagliore esterno è calibrato a sfocatura 10 px, estensione 0 px, opacità 70% e oro RGB `255/184/65` nel render 3×. Il fattore raddoppia per HR1. Le texture di festa e livello usano l'altezza originale di 128 px, mentre gli annunci usano 360 px.


#### Font e geometria

Zone e regioni usano il font completo `JupiterPro` (`Jupiter-Pro.ttf`, Regular): `N.alt2`, via OpenType `calt`, riproduce la N iniziale curva. I full screen usano invece `FONTSPRINGDEMO-JupiterProBold`; nel PSD le minuscole sono 127,5288 pt rispetto ai 150 pt delle maiuscole. La riduzione a 5/6 delle minuscole si applica a zone e regioni.

Il generatore lavora a 3 volte la risoluzione finale, a 72 dpi. Per le zone la dimensione delle maiuscole è 66,3606557377 px finali: deriva dalla larghezza di 506 px della prova «New Gridania». Il testo italiano conserva questa dimensione e la spaziatura naturale del font; si riduce solo oltre 940 px di larghezza. Il bordo superiore del testo è a 19 px. Per le regioni: maiuscole 26 px, spaziatura aggiunta di 11 px, bordo superiore a 23 px, riga a 52,5 px. Il full screen usa una texture 1280×360; il livello PSD applica 150 pt e la trasformazione 173,935% sul render 3×, poi viene centrato. I rendering HR1 raddoppiano le dimensioni.

#### Effetti riproducibili

I preset contengono gli effetti effettivi di Photoshop. Il generatore usa `Zone Style.ASL` per le zone e `Region Style.ASL` per le regioni; non applica correzioni ai colori. A risoluzione di lavoro 3×:

| Effetto | Impostazioni |
|---|---|
| Smusso interno | Scalpello deciso, dimensione 6 px, profondità 865%, direzione su |
| Luce | Angolo 120°, altezza 30°, indipendente dalla luce globale |
| Riflesso | Scherma lineare, RGB 255/255/242, opacità 100% |
| Ombra smusso | Moltiplica, RGB 54/46/43, opacità 45% |
| Contorno oro | 0,5 px, RGB 224/173/76, opacità 80% |
| Bagliore interno | RGB 201,5/131,35/42,91, opacità 90%, dimensione 5 px, espansione 12% |
| Bagliore esterno (zone) | Normale, RGB 138/125/112, 85%, estensione 28 px, sfocatura 57 px |
| Bagliore esterno (regioni) | Come sopra, estensione 11 px e sfocatura 22,3 px |

Il preset delle zone e quello regionale vengono selezionati in Photoshop e riutilizzati se già caricati; l'ASL si importa solo quando manca. `-ReloadStyle` forza una sola ricarica all'inizio della generazione dopo una modifica a quei preset. Per i full screen il PSD sorgente resta aperto, viene duplicato solo il livello di testo e non vengono importati stili. I rendering standard lavorano a 3× e HR1 a 6×; `AreaTextureTool pack` riduce l'immagine alla risoluzione finale e la scrive in formato TEX BGRA.

Il profilo full screen resta sul livello PSD sorgente, inclusi riempimento base RGB `190/165/40`, gradienti, bagliore esterno, bagliore interno, bordo e smusso.

La generazione richiede Photoshop installato e disponibile tramite automazione COM. Le copie originali sul Desktop non vengono modificate. Dump e confronti con le immagini originali restano nelle cartelle ignorate da Git.
