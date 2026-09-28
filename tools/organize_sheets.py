"""Sort translation sheets by editorial state and game area."""

import json
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
TRANSLATIONS = ROOT / "data" / "translations"
GLOSSARY = ROOT / "data" / "glossary" / "Glossary.md"

# First matching area wins. Existing categorized sheets keep their area.
AREAS = {
    "quests": r"^(Quest|LegacyQuest|QuestRedo|QuestReward|Leve|Journal|CompleteJournal|Completion|ScenarioType|Guildleve|GuildOrder|RelicNote)",
    "dialogue": r"(Talk|NpcYell|Snipe|TopicSelect|PointMenu|ContentEntry)$|^(ContentTalk|Gimmick|EmjVoiceNpc|HalloweenNpcSelect|PointMenuString|SnipeTalkName)",
    "combat": r"^(Action|Aoz|AOZ|AttackType|BgcArmy|BuddyAction|ClassJobActionUICategory|CompanionMove|CompanionTransient|DpsChallenge|FieldMarker|GeneralAction|Marker|ManeuversArmor|PetAction|PvP|PvPSelect|XPVP|XBMAction|XBMPet|XBMElement|XBMScore|MKDTrait)",
    "items": r"^(AnimaWeapon|BuddyEquip|Cabinet|DeepDungeonItem|EmjCostume|EurekaAetherItem|EurekaMagicite|EventItem|Glasses|ItemSearchCategory|ItemSeries|ItemSpecialBonus|MJIItem|MYCTemporaryItem|Ornament|PetMirage|PhantomWeapon|Relic6|Stain|XBMItem|YKW)",
    "shops": r"(Shop|ShopCategory|ShopItemGroup|ShopItemSet|ShopFilterType|ShopWelcomText|ExchangeShop)|^(FittingShop|TomestoneConvert)",
    "crafting": r"^(AirshipExploration|Collectables|CompanyCraft|Craft|Fish|Gathering|HWD|HugeCraftworks|Recipe|SecretRecipe|SharlayanCraft|Spearfishing|Submarine|WKSCosmoTool|WKSDevGrade|WKSItemSubCategory|WKSMecha|WKSMission|WKSPlanet|WKSNextPlanet|WKSPioneering|WKSText|WKSFate|WKSEmergency|BankaCraft|MJICraftworks|ValentionSweets)",
    "housing": r"^(Aquarium|Banner|CharaCard|Furniture|GroupPose|Housing|MJIHud|Orchestrion|Tofu|YardCatalog)",
    "minigames": r"^(ChocoboRace|RacingChocobo|Colosseum|FashionCheck|GoldSaucer|Lottery|MiniGame|Minion|Omikuji|RideShooting|TripleTriad|WeeklyBingo|WeddingBGM|Perform)",
    "social": r"^(Circle|CompanyAction|FC|FCA|Fcc|FCH|FCR|GcArmy|GCShop|MateAuthority|PlayerSearch|RetainerTask|CharaMakeName|CharaCardPlayStyle|OnlineStatus)",
    "system": r"^(AddonTransient|AkatsukiNoteString|Attributive|ChatBubble|ColorFilter|ConfigKey|Credit|CSBonus|DawnMemberUI|Description|EmjAddon|EventTutorial|ExtraCommand|ExVersion|FGSAddon|Guide|HowToPage|Hud|LoadingTips|LogFilter|LogKind|McGuffinUI|MultipleHelp|NotebookDivision|OpenContentCandidate|Platform|PreHandler|QTE|QuickChat|TextCommand|WebGuidance|WebURL)",
    "world": r"^(Achievement|Adventure|Aetheryte|BeastReputationRank|BeastTribe|BNpcName|ChocoboTaxi|ClassJobCategory|Companion|CutsceneName|Emote|ENpcResident|EObjName|FateEvent|GcRank|GFate|GrandCompany|Mount|MonsterNote|Pet|Town|Warp|World|Weather|Treasure|EObj|MKDLore|MJIName)",
    "activities": r"^(Content|Contents|DeepDungeon|DynamicEvent|Eureka|Event|IKD|InstanceContent|KTG|MassivePcContent|MJI|MKD|MYC|PartyContent|PublicContent|SkyIsland|VVD|WKS|XBM)",
}


def area(name, parts):
    for part in parts:
        if part in {"quests", "dialogue", "combat", "items", "shops", "crafting", "housing", "minigames", "social", "system", "world", "activities"}:
            if part != "misc":
                return part
    for category, pattern in AREAS.items():
        if re.search(pattern, name, re.IGNORECASE):
            return category
    return "misc"


def main():
    approved = set()
    in_section = False
    for line in GLOSSARY.read_text(encoding="utf-8-sig").splitlines():
        if line.startswith("## "):
            in_section = line == "## File approvati"
        elif in_section and line.startswith("- `"):
            approved.add(line.split("`", 2)[1].lower())

    moves = []
    counts = {"da_tradurre": 0, "da_revisionare": 0, "revisionati": 0}
    for source in sorted(TRANSLATIONS.rglob("*.json")):
        parts = source.relative_to(TRANSLATIONS).parts
        category = area(source.stem, parts)
        data = json.loads(source.read_text(encoding="utf-8-sig"))
        translated = any(
            isinstance(row, dict) and any(
                key.startswith("translation") and isinstance(value, str) and value.strip()
                for key, value in row.items()
            )
            for row in data.values()
        )
        relative = f"{category}/{source.name}".lower()
        if relative in approved:
            state = "revisionati"
            target = TRANSLATIONS / category / source.name
        else:
            state = "da_revisionare" if translated else "da_tradurre"
            if category == "quests":
                subpath = parts[parts.index("quests") + 1:-1] if "quests" in parts else ()
                if not subpath or not subpath[-1].isdigit():
                    subpath = ("master",)
            else:
                subpath = ()
            target = TRANSLATIONS / state / category / Path(*subpath) / source.name
        counts[state] += 1
        if source != target:
            if target.exists():
                raise FileExistsError(f"Destinazione già presente: {target}")
            moves.append((source, target))

    for source, target in moves:
        target.parent.mkdir(parents=True, exist_ok=True)
        source.rename(target)
    print(f"Spostati {len(moves)} file. Stati: {counts}")


if __name__ == "__main__":
    main()
