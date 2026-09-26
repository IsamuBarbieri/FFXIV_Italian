# FFXIV Italian — Prompt di Traduzione Diretto & In-Place

Usa questo prompt per avviare sessioni di traduzione sequenziale continua e in-place.

---

```markdown
/goal
Continua ad oltranza fino a che TUTTO il file specificato non è completamente tradotto.
Leggi e scrivi direttamente sul file JSON in modo sequenziale, senza subagenti e senza fermarti.

### MODALITÀ OPERATIVA:
1. CHECK INIZIALE (UNA TANTUM): All'avvio, leggi alcune righe già tradotte all'inizio del file (o nel file di riferimento se specificato dall'utente) per allineare lo stile, le formule ricorrenti e le preposizioni. Poi memorizzale nel contesto e non rileggere più all'indietro durante il resto del processo.
2. Inizia dal PRIMO ID con "translation": "" (o con testo ancora in inglese).
3. Procedi a ciclo continuo in ordine sequenziale di ID, modificando direttamente il file JSON su disco con salvataggi progressivi a blocchi.
4. Mantieni per tutto il file le stesse formule e costrutti osservati nel check iniziale (zero varianti sinonimiche).
5. NON fermarti dopo un blocco: continua automaticamente con il successivo fino a fine file.
6. Pensa il minimo indispensabile: traduzione diretta, fluida e naturale, zero overthinking o elucubrazioni.
7. In chat NON stampare elenchi, spiegazioni o frammenti tradotti. Rispondi solo quando l'intero file è completato (o se esaurisci forzatamente il contesto) con:
   `Fatto: completata traduzione del file [percorso_file]`

---

### REGOLE FERREE:
- MAIUSCOLE: conserva le iniziali maiuscole dell'originale; articoli interni (anche articolati: della, delle) minuscoli. Esempio: "Order of the Twin Adder" → "Ordine della Vipera Gemella".
- COERENZA FORMULE: Uniforma i pattern ripetitivi (formule di danno, durate, condizioni, trigger di combo) a quanto stabilito nel check iniziale. Non alternare sinonimi nello stesso file.
- SESTRING & TAG: Tutti i tag `<hex:...>`, variabili `<FullName>`, `<Forename>`, macro `<If(...)>`, `<br>`, `\uE051`, ecc. vanno copiati IDENTICI dall'originale 1:1, senza alterazioni e senza lasciare tag sbilanciati.
- MAI BILINGUISMO: Niente testo inglese tra parentesi nel client (MAI "Baia del Vespro (Vesper Bay)"). Solo italiano puro.
- INVARIANTI INGLESI: Nomi di abilità, magie (*Fire, Cure, Rampart, Provoke*), Limit Break e comandi slash (`/gpose`) rimangono CATEGORICAMENTE IN INGLESE. Tradurre solo descrizioni e testi d'azione.
- GENERE EPICENO: Per frasi rivolte al giocatore non condizionali, usa forme neutre ("Hai dimostrato valore", non "Sei stato/a bravo/a").

---

### GLOSSARIO CHIAVE:
- Fazioni: The Maelstrom → La Tempesta | Order of the Twin Adder → Ordine della Vipera Gemella | Immortal Flames → Fiamme Immortali | Scions of the Seventh Dawn → Figli della Settima Alba | Grand Company → Grande Compagnia | Free Company → Compagnia Libera.
- Etere: aether → etere | aetherial → eterico/a | Aetheryte → Eterite | Aethernet → Eternet | Aetherial Sea → Mare Etereo | Lifestream → Flusso Vitale | Aether Sickness → Mal d'Etere.
- Unità eorzeane (minuscole, invariabili al plurale, mai convertire in metri): yalm, fulm, ilm, malm | ponze, onze, tonze.
- Classi (Job = Classe): Gladiatore, Paladino, Incursore (Marauder), Guerriero, Cavaliere Oscuro, Eterlama (Gunbreaker) | Incantatore (Conjurer), Mago Bianco, Arcanista, Studioso (Scholar), Astrologo, Saggio (Sage) | Pugile, Monaco, Lanciere, Dragoon (inv.), Furfante (Rogue), Ninja (inv.), Samurai (inv.), Mietitore (Reaper), Vipera | Arciere, Bardo, Artificiere (Machinist), Danzatore/Danzatrice | Taumaturgo, Mago Nero, Evocatore (Summoner), Mago Rosso, Mago Blu, Pittomante.
- Razze: invariate (Hyur, Elezen, Lalafell, Miqo'te, Roegadyn, Au Ra, Hrothgar, Viera).
- Toponimi: The Black Shroud → Velo Nero | Sabbie del Risveglio (Waking Sands) | Pietre Risorte (Rising Stones) | Pedaggio del Redivivo (Revenant's Toll) | Portobirra (Aleport) | Portovino (Wineport) | Cavamulino (Quarrymill) | Terralago (Lakeland).
- NPC: Urianger (aulico/arcaico), Thancred (disinvolto/ironico), Alphinaud (diplomatico/formale), Alisaie (diretta/pungente), Y'shtola (calma/sferzante), Tataru (premurosa/squillante), Estinien (laconico/militare), Emet-Selch (teatrale/cinico).
```
