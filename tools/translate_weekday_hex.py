"""Translate weekday alternatives embedded in DescriptionString macros."""

import json
import pathlib
import re

from translate_hex_literals import replace_literals

PATH = pathlib.Path(__file__).resolve().parents[1] / "data/translations/misc/descriptionstring.json"
TAG = re.compile(r"<hex:[0-9A-Fa-f]+>")
PLACEHOLDER = re.compile(r"\{@(\d+)\}")
WEEKDAYS = {
    "Sunday": "Domenica", "Monday": "Lunedì", "Tuesday": "Martedì",
    "Wednesday": "Mercoledì", "Thursday": "Giovedì", "Friday": "Venerdì",
    "Saturday": "Sabato",
}
TEXT = {
    1643: "Le Consegne su Misura diventano disponibili dopo aver completato la missione che le sblocca. Per consegnare un oggetto devi avere la classe corrispondente e il livello richiesto. Controlla l'elenco delle consegne per verificare se soddisfi i requisiti.{@0}{@1}Le ricompense dipendono dal tuo livello al momento della consegna. Se non hai raggiunto il livello massimo ricevi punti esperienza, altrimenti ricevi scrip. Le ricompense previste per la classe attuale sono indicate in fondo all'elenco.{@2}{@3}■Tipi di Oggetto{@4}I clienti accettano solo collezionabili di tre categorie: {@5}{@6}oggetti fabbricati{@7}{@8}, {@9}{@10}oggetti raccolti{@11}{@12} e {@13}{@14}pesci{@15}{@16}. Puoi consegnare un oggetto solo con la classe corrispondente.{@17}{@18}Le ricompense aumentano con la collezionabilità. Inoltre, gli oggetti contrassegnati con {@19}{@20}BONUS{@21}{@22} concedono ricompense particolarmente generose. Quando compaiono, vale la pena procurarseli.{@23}{@24}■Limiti delle Consegne{@25}Ogni settimana puoi effettuare fino a sei consegne per cliente e dodici in totale. I limiti si azzerano ogni {@26}{@27} alle {@28} {@29} (ora terrestre).",
    363: "Una volta alla settimana, dopo aver completato una Prova Irreale, puoi parlare con il comandante delle illusioni per accedere a Tane Sospette.{@0}※La disponibilità si rinnova ogni {@1}{@2} alle {@3} {@4} (ora terrestre). Puoi verificarla nella voce Ricerca Incursioni di qualsiasi Prova Irreale.",
    367: "Scoprendo una {@0}{@1}grande illustrazione{@2}{@3} ottieni un {@4}{@5}riracconto{@6}{@7}. Questa ricompensa ti permette di completare un'altra Prova Irreale e accedere di nuovo a Tane Sospette prima della fine della settimana. È quindi un'alternativa alla ricerca delle sole illustrazioni che assegnano molte foglie illusorie.{@8}{@9}{@10}※Puoi ottenere un solo riracconto a settimana e usarlo soltanto nella stessa settimana. I riracconti si azzerano ogni {@11}{@12} alle {@13} {@14} (ora terrestre).",
}


def main():
    rows = json.loads(PATH.read_text(encoding="utf-8"))
    for row_id, template in TEXT.items():
        row = rows[str(row_id)]
        tags = TAG.findall(row["original"])
        found = [int(match.group(1)) for match in PLACEHOLDER.finditer(template)]
        assert found == list(range(len(tags))), row_id
        tags = [replace_literals(tag, WEEKDAYS) if "53756E646179" in tag else tag
                for tag in tags]
        row["translation"] = PLACEHOLDER.sub(lambda match: tags[int(match.group(1))], template)
    PATH.write_text(json.dumps(rows, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
