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

### GLOSSARIO APPROVATO:
Leggi `data/glossary/Glossary.md` prima di tradurre. Applica le voci attestate e verifica le note sulle varianti nel contesto.
```
