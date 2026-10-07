# Stili dei cartelli

- `Zone Style.ASL`: preset Photoshop «Zone Style», rifinito sul dump di Nuova Gridania. Il punto più giallo del gradiente usa il campione originale RGB `255/255/148`; il bevel bianco è leggermente più largo.
- `Region Style.ASL`: copia del preset di zona con nome interno `Region Scl` e il Bagliore esterno ridotto in proporzione al font regionale. Estensione e sfocatura passano rispettivamente da 28/57 px a 11/22,3 px, usando il rapporto tra le maiuscole regionali (26 px) e quelle delle zone (66,3606557377 px). Colori, opacità, bevel e gli altri effetti restano identici.
- `FF Full Screen.ASL`: preset fornito dall’utente e conservato invariato. Photoshop lo applicava con riempimento grigio e sfumatura disattivata, diverso dal livello di riferimento nel PSD. Per i full screen il generatore duplica quindi il livello `This Uses Jupiter Font` da `Duty Complete font.psd` e sostituisce il testo, conservando riempimento, trasformazione, gradienti e bevel senza creare preset duplicati. Il testo viene ingrandito di 1,7× per allinearsi all’esportazione approvata; il Bagliore esterno è calibrato a sfocatura 10 px, estensione 0 px, opacità 70% e oro RGB `255/184/65` nel render 3×. Il fattore raddoppia per HR1. Le texture di festa e livello usano l’altezza originale di 128 px, mentre gli annunci usano 360 px.

Le basi restano conservate in `Zone Style - FF Near base.ASL` e `Zone Style - FF Warm base.ASL`. La variante attiva per le zone cambia la tinta e la larghezza del bevel; quella regionale riduce anche il solo Bagliore esterno come descritto sotto.

## Font e geometria

Zone e regioni usano il font completo `JupiterPro` (`Jupiter-Pro.ttf`, Regular): `N.alt2`, via OpenType `calt`, riproduce la N iniziale curva. I full screen usano invece `FONTSPRINGDEMO-JupiterProBold`; nel PSD le minuscole sono 127,5288 pt rispetto ai 150 pt delle maiuscole. La riduzione a 5/6 delle minuscole si applica a zone e regioni.

Il generatore lavora a 3 volte la risoluzione finale, a 72 dpi. Per le zone la dimensione delle maiuscole è 66,3606557377 px finali: deriva dalla larghezza di 506 px della prova «New Gridania». Il testo italiano conserva questa dimensione e la spaziatura naturale del font; si riduce solo oltre 940 px di larghezza. Il bordo superiore del testo è a 19 px. Per le regioni: maiuscole 26 px, spaziatura aggiunta di 11 px, bordo superiore a 23 px, riga a 52,5 px. Il full screen usa una texture 1280×360; il livello PSD applica 150 pt e la trasformazione 173,935% sul render 3×, poi viene centrato. I rendering HR1 raddoppiano le dimensioni.

## Effetti riproducibili

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

Il preset delle zone e quello regionale vengono selezionati in Photoshop e riutilizzati se già caricati; l’ASL si importa solo quando manca. `-ReloadStyle` forza una sola ricarica all’inizio della generazione dopo una modifica a quei preset. Per i full screen il PSD sorgente resta aperto, viene duplicato solo il livello di testo e non vengono importati stili. I rendering standard lavorano a 3× e HR1 a 6×; `AreaTextureTool pack` riduce l’immagine alla risoluzione finale e la scrive in formato TEX BGRA.

Il profilo full screen resta sul livello PSD sorgente, inclusi riempimento base RGB `190/165/40`, gradienti, bagliore esterno, bagliore interno, bordo e smusso.

La generazione richiede Photoshop installato e disponibile tramite automazione COM. Le copie originali sul Desktop non vengono modificate. Dump e confronti con le immagini originali restano nelle cartelle ignorate da Git.
