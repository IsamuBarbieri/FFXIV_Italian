# FFXIV Italiano Text Inspector

Plugin Dalamud locale per trovare la voce del corpus associata a un testo dell'interfaccia.

## Comandi

Il pannello grafico compare al caricamento. L'interruttore attiva o disattiva l'ispezione; **Ricarica le traduzioni** rilegge il corpus e mostra eventuali errori. Si può riaprire il pannello dal gestore plugin Dalamud tramite il pulsante di configurazione. I comandi seguenti sono facoltativi.

- `/itinspect`: attiva o disattiva l'ispezione sotto il puntatore.
- `/itinspect on` e `/itinspect off`: imposta esplicitamente lo stato.
- `/itinspect reload`: rilegge `data/translations/` dopo modifiche al corpus.

Il riquadro mostra il percorso JSON, il numero di riga fisico del campo sorgente, l'ID della voce e il campo. Se il testo è duplicato, elenca le corrispondenze. Il plugin legge il contenuto visibile dei nodi SeString e cerca una corrispondenza esatta, ignorando i tag `<hex:...>`, i controlli e le differenze di maiuscole/minuscole. I testi dinamici e quelli assenti dal corpus non sono identificabili con certezza; le corrispondenze sono candidati, non una prova della provenienza del testo. I numeri di riga si aggiornano ricaricando il corpus.

Per i testi troncati (per esempio `Forziere della Com…`) prova prima il testo originale conservato dal nodo, letto con `ReadProcessMemory` e un limite di 4096 byte. Se non è disponibile, cerca i candidati che iniziano con il prefisso visibile, tramite un indice ordinato. Questa ricerca si attiva soltanto con puntini finali e almeno quattro caratteri di prefisso; le corrispondenze esatte hanno sempre precedenza.

Il riquadro identifica anche la finestra UI e il nodo. Il tracciamento osserva `RaptureTextModule.GetAddonText` e `AtkTextNode.SetText`: quando vede sullo stesso thread una lettura Addon seguita entro 100 ms dall'assegnazione dello stesso puntatore a un nodo, registra la riga per quel nodo. La risoluzione verifica ancora puntatore e testo originale; un puntatore osservato per righe diverse resta ambiguo. Un'altra chiamata SetText invalida la precedente associazione del nodo. Gli hook passano sempre i dati originali al gioco senza modificarli.

**Dopo il caricamento o un aggiornamento riapri la finestra del gioco da ispezionare**, così il plugin può osservare lettura e assegnazione. Quando riesce, mostra `Origine tracciata GetAddonText → SetText: Addon#ID` e la sola riga corrispondente. Negli altri casi mostra esplicitamente candidati testuali: trasformazioni, copie intermedie, buffer condivisi, righe di altri fogli o finestre già aperte possono impedire il tracciamento. Una riga Addon condivisa può essere usata in più finestre; cambiarla nel corpus modifica tutti quegli usi. Il tracciamento non crea una traduzione specifica per un solo nodo UI.

### Riferimenti della UI e Addon Inspector

Per i riferimenti Addon viene letto anche il testo della riga effettivamente caricata nel client, sul thread del gioco, e conservato per la sessione del plugin. La verifica strutturale accetta quindi il testo della patch corrente anche quando il JSON è già stato corretto. Esempio: il client mostra ancora "Forziere della Compagnia", il JSON contiene "Forziere"; il nodo Bank#17 resta associato ad Addon#2963. Un testo estraneo sia al client sia al corpus continua a essere rifiutato. Questa lettura non ricarica o modifica gli archivi del client.

Se non c'è una sequenza runtime osservata, il plugin legge `TextId` e `SheetType` del nodo. Per i nodi presenti nella lista del widget controlla anche la definizione `ui/uld/<nome-finestra>.uld`, tramite Lumina. Usa il riferimento Addon/Lobby solo quando il testo della riga è compatibile con quello corrente, anche troncato. Le definizioni sono riferimenti del layout iniziale e non provano da sole la provenienza di un testo cambiato a runtime. I componenti annidati con ID locali non vengono cercati nella lista del widget principale; un file ULD mancante non blocca gli altri metodi.

Esempio verificato sul client locale: `ui/uld/bank.uld → nodo 17 → Addon#2963`, cioè `data/translations/system/addon.json#2963:translation` ("Forziere della Compagnia"). Questo distingue la scritta dalle righe omonime di Item, FCRights e Addon#2880, senza escluderle arbitrariamente per somiglianza.

Il pannello conserva l'ultimo nodo mentre sposti il mouse sui controlli. **Apri Addon Inspector** apre il widget Dalamud tramite `/xldata ai` e copia il nome della finestra: cercala nell'inspector e seleziona il NodeId indicato. Non c'è un collegamento a un'API interna per presele­zionare automaticamente il nodo. **Copia diagnosi** raccoglie contesto, indirizzo del nodo, TextId, SheetType, definizione ULD e riferimenti al corpus; **Copia riferimento** copia `percorso#ID:campo`.

## Build e caricamento locale

### Modifica in gioco e anteprima

Passa il mouse sul testo, spostati nel pannello e premi **Modifica in gioco** sul riferimento desiderato. L'editor legge il valore JSON originale per ID e campo, senza normalizzare spazi, righe o payload, e fissa il nodo selezionato anche se sposti il mouse altrove.

Quando ispezioni un altro nodo, l'editor si svuota e si chiude, scarta la bozza non salvata e interrompe l'anteprima precedente. Devi premere nuovamente **Modifica in gioco** per la nuova voce. Passare su uno spazio vuoto o sui controlli del pannello non cambia la selezione. Il salvataggio di testo vuoto o composto solo da spazi è bloccato sia nella GUI sia nella funzione che scrive il file.

- **Anteprima live sul nodo selezionato** applica la bozza di testo semplice sul thread del gioco, senza modificare la patch. Le modifiche al testo sono applicate entro circa 100 ms. Se il client ripristina la scritta iniziale, l'anteprima viene riapplicata; se il nodo scompare o riceve una scritta diversa, viene sospesa. I nodi con payload e le bozze con macro non hanno anteprima live in questa versione.
- **Salva nel JSON** sostituisce soltanto il valore del campo di traduzione: mantiene gli altri byte, inclusi ID, sorgenti, metadati, BOM, indentazione e terminatori di riga. Prima del salvataggio controlla che sorgente e traduzione non siano cambiate esternamente. Le modifiche indipendenti ad altri campi vengono conservate. Crea un backup nella cartella di configurazione del plugin, sottocartella `translation-backups`, e ricarica l'indice.
- **Annulla / chiudi editor** scarta la bozza non salvata e ripristina il testo precedente se il nodo esiste ancora e contiene l'anteprima. Le modifiche già salvate nel JSON restano. Anche chiudere il pannello o scaricare il plugin interrompe l'anteprima; una bozza non salvata si perde scaricando o ricaricando il plugin.

L'editor usa il validatore SeString del Core; in questa versione richiede inoltre che i tag della traduzione iniziale restino invariati. Per interventi sui payload usa VS Code. La preview modifica solo il nodo scelto; una modifica salvata a una riga condivisa influenzerà tutti i suoi usi quando aggiornerai la patch. Le modifiche restano nello stato editoriale corrente e non vengono approvate automaticamente.

Nel pannello, **Modifica in VS Code** apre il campo `translation` o `translation_<campo>` della voce selezionata. La riga viene ricalcolata dal file corrente per ID e campo prima di aprire l'editor, anche se il file è cambiato dopo l'indicizzazione. Il pulsante richiede VS Code installato nelle cartelle standard Windows. Dopo il salvataggio usa **Ricarica le traduzioni** per aggiornare l'indice; la patch del gioco si aggiorna tramite il normale flusso di rebuild/deploy, non con il ricaricamento dell'indice. Se ci sono più candidati, scegli il riferimento desiderato prima di aprirlo.

Richiede Dalamud e il .NET SDK usato dalla versione corrente di Dalamud. Compila il progetto:

```powershell
dotnet build .\plugins\FFXIVItalian.TextInspector\FFXIVItalian.TextInspector.csproj -c Debug
```

Aggiungi il DLL generato sotto `plugins/FFXIVItalian.TextInspector/bin/` alle posizioni dei plugin di sviluppo in `/xlsettings`, quindi abilitalo in `/xlplugins`.

In alternativa, dopo la build e con FFXIV chiuso:

```powershell
python plugins/FFXIVItalian.TextInspector/install.py
```

L'installer salva un backup della configurazione e registra la build Debug con avvio e ricaricamento automatici. Dopo l'installazione iniziale, ricompila lo stesso progetto mentre giochi: Dalamud ricarica la DLL modificata. Non occorre rieseguire l'installer per aggiornare il codice. Per aggiornare soltanto i JSON usa il pulsante nel pannello. Il ricaricamento ricrea il plugin e il suo indice.

Verifica dell'indice, indipendente dal gioco:

```powershell
dotnet run --project tests/FFXIVItalian.TextInspector.Checks -- data/translations
```

Il plugin cerca `data/translations/` risalendo dalla cartella del DLL. È quindi pensato per essere caricato dalla build del repository locale; non include né modifica il corpus.
