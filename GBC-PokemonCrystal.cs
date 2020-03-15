using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using CrowdControl.Common;
using JetBrains.Annotations;
using CrowdControl.Games;

namespace CrowdControl.Games.Packs
{
    [UsedImplicitly]
    public class PokemonCrystal : GBEffectPack
    {
        [NotNull]
        private readonly IPlayer _player;

        public PokemonCrystal([NotNull] IPlayer player, [NotNull] Func<CrowdControlBlock, bool> responseHandler, [NotNull] Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) => _player = player;

        private volatile bool _quitting = false;
        public override void Dispose()
        {
            _quitting = true;
            base.Dispose();
        }

        private static readonly Dictionary<string, (string name, byte id)> _Pokemon = new Dictionary<string, (string, byte)>
        {
            {"bulbasaur", ("Bulbasaur", 0x01)},
            {"ivysaur", ("Ivysaur", 0x02)},
            {"venusaur", ("Venusaur", 0x03)},
            {"charmander", ("Charmander", 0x04)},
            {"charmeleon", ("Charmeleon", 0x05)},
            {"charizard", ("Charizard", 0x06)},
            {"squirtle", ("Squirtle", 0x07)},
            {"wartortle", ("Wartortle", 0x08)},
            {"blastoise", ("Blastoise", 0x09)},
            {"caterpie", ("Caterpie", 0x0A)},
            {"metapod", ("Metapod", 0x0B)},
            {"butterfree", ("Butterfree", 0x0C)},
            {"weedle", ("Weedle", 0x0D)},
            {"kakuna", ("Kakuna", 0x0E)},
            {"beedrill", ("Beedrill", 0x0F)},
            {"pidgey", ("Pidgey", 0x10)},
            {"pideotto", ("Pidgeotto", 0x11)},
            {"pidgeot", ("Pidgeot", 0x12)},
            {"rattata", ("Rattata", 0x13)},
            {"raticate", ("Raticate", 0x14)},
            {"spearow", ("Spearow", 0x15)},
            {"fearow", ("Fearow", 0x16)},
            {"ekans", ("Ekans", 0x17)},
            {"arbok", ("Arbok", 0x18)},
            {"pikachu", ("Pikachu", 0x19)},
            {"raichu", ("Raichu", 0x1A)},
            {"sandshrew", ("Sandshrew", 0x1B)},
            {"sandslash", ("Sandslash", 0x1C)},
            {"nidoranfemale", ("Nidoran (Female)", 0x1D)},
            {"nidorina", ("Nidorina", 0x1E)},
            {"nidoqueen", ("Nidoqueen", 0x1F)},
            {"nidoranmale", ("Nidoran (Male)", 0x20)},
            {"nidorino", ("Nidorino", 0x21)},
            {"nidoking", ("Nidoking", 0x22)},
            {"clefairy", ("Clefairy", 0x23)},
            {"clefable", ("Clefable", 0x24)},
            {"vulpix", ("Vulpix", 0x25)},
            {"ninetales", ("Ninetales", 0x26)},
            {"jigglypuff", ("Jigglypuff", 0x27)},
            {"wigglytuff", ("Wigglytuff", 0x28)},
            {"zubat", ("Zubat", 0x29)},
            {"golbat", ("Golbat", 0x2A)},
            {"oddish", ("Oddish", 0x2B)},
            {"gloom", ("Gloom", 0x2C)},
            {"vileplume", ("Vileplume", 0x2D)},
            {"paras", ("Paras", 0x2E)},
            {"parasect", ("Parasect", 0x2F)},
            {"venonat", ("Venonat", 0X30)},
            {"venomoth", ("Venomoth", 0x31)},
            {"diglett", ("Diglett", 0x32)},
            {"dugtrio", ("Dugtrio", 0x33)},
            {"meowth", ("Meowth", 0x34)},
            {"persian", ("Persian", 0x35)},
            {"psyduck", ("Psyduck", 0x36)},
            {"golduck", ("Golduck", 0x37)},
            {"mankey", ("Mankey", 0x38)},
            {"primeape", ("Primeape", 0x39)},
            {"growlithe", ("Growlithe", 0x3a)},
            {"arcanine", ("Arcanine", 0x3B)},
            {"poliwag", ("Poliwag", 0x3C)},
            {"poliwhirl", ("Poliwhirl", 0x3D)},
            {"poliwrath", ("Poliwrath", 0x3E)},
            {"abra", ("Abra", 0x3F)},
            {"kadabra", ("Kadabra", 0x40)},
            {"alakazam", ("Alakazam", 0X41)},
            {"machop", ("Machop", 0x42)},
            {"machoke", ("Machoke", 0x43)},
            {"machamp", ("Machamp", 0x44)},
            {"bellsprout", ("Bellsprout", 0x45)},
            {"weepinbell", ("Weepinbell", 0x46)},
            {"victreebel", ("Victreebel", 0x47)},
            {"tentacool", ("Tentacool", 0x48)},
            {"tentacruel", ("Tentacruel", 0x49)},
            {"geodude", ("Geodude", 0x4A)},
            {"graveler", ("Graveler", 0x4B)},
            {"golem", ("Golem", 0x4C)},
            {"ponyta", ("Ponyta", 0x4D)},
            {"rapidash", ("Rapidash", 0x4E)},
            {"slowpoke", ("Slowpoke", 0x4F)},
            {"slowbro", ("Slowbro", 0x50)},
            {"magnemite", ("Magnemite", 0X51)},
            {"magneton", ("Magneton", 0x52)},
            {"farfetchd", ("Farfetch'd", 0x53)},
            {"doduo", ("Doduo", 0x54)},
            {"dodrio", ("Dodrio", 0x55)},
            {"seel", ("Seel", 0x56)},
            {"dewgong", ("Dewgong", 0x57)},
            {"grimer", ("Grimer", 0x58)},
            {"muk", ("Muk", 0x59)},
            {"shellder", ("Shellder", 0x5A)},
            {"cloyster", ("Cloyster", 0x5B)},
            {"gastly", ("Gastly", 0x5C)},
            {"haunter", ("Haunter", 0x5D)},
            {"gengar", ("Gengar", 0x5E)},
            {"onix", ("Onix", 0x5F)},
            {"drowzee", ("Drowzee", 0x60)},
            {"hypno", ("Hypno", 0X61)},
            {"krabby", ("Krabby", 0x62)},
            {"kingler", ("Kingler", 0x63)},
            {"voltorb", ("Voltorb", 0x64)},
            {"electrode", ("Electrode", 0x65)},
            {"exeggcute", ("Exeggcute", 0x66)},
            {"exeggutor", ("Exeggutor", 0x67)},
            {"cubone", ("Cubone", 0x68)},
            {"marowak", ("Marowak", 0x69)},
            {"hitmonlee", ("Hitmonlee", 0x6A)},
            {"hitmonchan", ("Hitmonchan", 0x6B)},
            {"lickitung", ("Lickitung", 0x6C)},
            {"koffing", ("Koffing", 0x6D)},
            {"weezing", ("Weezing", 0x6E)},
            {"rhyhorn", ("Rhyhorn", 0x6F)},
            {"rhydon", ("Rhydon", 0x70)},
            {"chansey", ("Chansey", 0X71)},
            {"tangela", ("Tangela", 0x72)},
            {"kangaskhan", ("Kangaskhan", 0x73)},
            {"horsea", ("Horsea", 0x74)},
            {"seadra", ("Seadra", 0x75)},
            {"goldeen", ("Goldeen", 0x76)},
            {"seaking", ("Seaking", 0x77)},
            {"staryu", ("Staryu", 0x78)},
            {"starmie", ("Starmie", 0x79)},
            {"mrmime", ("Mr. Mime", 0x7A)},
            {"scyther", ("Scyther", 0x7B)},
            {"jynx", ("Jynx", 0x7C)},
            {"electabuzz", ("Electabuzz", 0x7D)},
            {"magmar", ("Magmar", 0x7E)},
            {"pinsir", ("Pinsir", 0x7F)},
            {"tauros", ("Tauros", 0x80)},
            {"magikarp", ("Magikarp", 0X81)},
            {"gyrados", ("Gyrados", 0x82)},
            {"lapras", ("Lapras", 0x83)},
            {"ditto", ("Ditto", 0x84)},
            {"eevee", ("Eevee", 0x85)},
            {"vaporeon", ("Vaporeon", 0x86)},
            {"jolteon", ("Jolteon", 0x87)},
            {"flareon", ("Flareon", 0x88)},
            {"porygon", ("Porygon", 0x89)},
            {"omanyte", ("Omanyte", 0x8A)},
            {"omastar", ("Omastar", 0x8B)},
            {"kabuto", ("Kabuto", 0x8C)},
            {"kabutops", ("Kabutops", 0x8D)},
            {"aerodactyl", ("Aerodactyl", 0x8E)},
            {"snorlax", ("Snorlax", 0x8F)},
            {"articuno", ("Articuno", 0x90)}, 
            {"zapdos", ("Zapdos", 0X91)},
            {"moltres", ("Moltres", 0x92)},
            {"dratini", ("Dratini", 0x93)},
            {"dragonair", ("Dragonair", 0x94)},
            {"dragonite", ("Dragonite", 0x95)},
            {"mewtwo", ("Mewtwo", 0x96)},
            {"mew", ("Mew", 0x97)},
            {"chikorita", ("Chikorita", 0x98)},
            {"bayleef", ("Bayleef", 0x99)},
            {"meganium", ("Meganium", 0x9A)},
            {"cyndaquil", ("Cyndaquil", 0x9B)},
            {"quilava", ("Quilava", 0x9C)},
            {"typhlosion", ("Typhlosion", 0x9D)},
            {"totodile", ("Totodile", 0x9E)},
            {"croconaw", ("Croconaw", 0x9F)},          
            {"feraligatr", ("Feraligatr", 0XA0)},
            {"sentret", ("Sentret", 0xA1)},
            {"furret", ("Furret", 0xA2)},
            {"hoothoot", ("Hoothoot", 0xA3)},
            {"noctowl", ("Noctowl", 0xA4)},
            {"ledyba", ("Ledyba", 0xA5)},
            {"ledian", ("Ledian", 0xA6)},
            {"spinarak", ("Spinarak", 0xA7)},
            {"ariados", ("Ariados", 0xA8)},
            {"crobat", ("Crobat", 0xA9)},
            {"chinchou", ("Chinchou", 0xAA)},
            {"lanturn", ("Lanturn", 0xAB)},
            {"pichu", ("Pichu", 0xAC)},
            {"cleffa", ("Cleffa", 0xAD)},
            {"igglybuff", ("Igglybuff", 0xAE)},
            {"togepi", ("Togepi", 0xAF)},     
            {"togetic", ("Togetic", 0XB0)},
            {"natu", ("Natu", 0xB1)},
            {"xatu", ("Xatu", 0xB2)},
            {"mareep", ("Mareep", 0xBB)},
            {"flaaffy", ("Flaaffy", 0xB4)},
            {"amphiros", ("Amphiros", 0xB5)},
            {"bellossom", ("Bellossom", 0xB6)},
            {"marill", ("Marill", 0xB7)},
            {"azumarill", ("Azumarill", 0xB8)},
            {"sudowoodo", ("Sudowoodo", 0xB9)},
            {"politoed", ("Politoed", 0xBA)},
            {"hoppip", ("Hoppip", 0xBB)},
            {"skiploom", ("Skiploom", 0xBC)},
            {"jumpluff", ("Jumpluff", 0xBD)},
            {"aipom", ("Aipom", 0xBE)},
            {"sunkern", ("Sunkern", 0xBF)},     
            {"sunflora", ("Sunflora", 0XC0)},
            {"yanma", ("Yanma", 0xC1)},
            {"wooper", ("Wooper", 0xC2)},
            {"quagsire", ("Quagsire", 0xC3)},
            {"espeon", ("Espeon", 0xC4)},
            {"umbreon", ("Umbreon", 0xC5)},
            {"murkrow", ("Murkrow", 0xC6)},
            {"slowking", ("Slowking", 0xC7)},
            {"misdreavus", ("Misdreavus", 0xC8)},
            {"unown", ("Unown", 0xC9)},
            {"wobbuffet", ("Wobbuffet", 0xCA)},
            {"girafarig", ("Girafarig", 0xCB)},
            {"pineco", ("Pineco", 0xCC)},
            {"forretress", ("Forretress", 0xCD)},
            {"dunsparce", ("Dunsparce", 0xCE)},
            {"gligar", ("Gligar", 0xCF)},     
            {"steelix", ("Steelix", 0XD0)},
            {"snubbull", ("Snubbull", 0xD1)},
            {"granbull", ("Granbull", 0xD2)},
            {"qwilfish", ("Qwilfish", 0xD3)},
            {"scizor", ("Scizor", 0xD4)},
            {"shuckle", ("Shuckle", 0xD5)},
            {"heracross", ("Heracross", 0xD6)},
            {"sneasel", ("Sneasel", 0xD7)},
            {"teddiursa", ("Teddiursa", 0xD8)},
            {"ursaring", ("Ursaring", 0xD9)},
            {"slugma", ("Slugma", 0xDA)},
            {"magcargo", ("Magcargo", 0xDB)},
            {"swinub", ("Swinub", 0xDC)},
            {"piloswine", ("Piloswine", 0xDD)},
            {"corsola", ("Corsola", 0xDE)},
            {"remoraid", ("Remoraid", 0xDF)},     
            {"octillery", ("Octillery", 0XE0)},
            {"delibird", ("Delibird", 0xE1)},
            {"mantine", ("Mantine", 0xE2)},
            {"skarmory", ("Skarmory", 0xE3)},
            {"houndour", ("Houndour", 0xE4)},
            {"houndoom", ("Houndoom", 0xE5)},
            {"kingdra", ("Kingdra", 0xE6)},
            {"phanpy", ("Phanpy", 0xE7)},
            {"donphan", ("Donphan", 0xE8)},
            {"porygon2", ("Porygon2", 0xE9)},
            {"stantler", ("Stantler", 0xEA)},
            {"smeargle", ("Smeargle", 0xEB)},
            {"tyrogue", ("Tyrogue", 0xEC)},
            {"hitmontop", ("Hitmontop", 0xED)},
            {"smoochum", ("Smoochum", 0xEE)},
            {"elekid", ("Elekid", 0xEF)},     
            {"magby", ("Magby", 0XF0)},
            {"miltank", ("Miltank", 0xF1)},
            {"blissey", ("Blissey", 0xF2)},
            {"raikou", ("Raikou", 0xF3)},
            {"entei", ("Entei", 0xF4)},
            {"suicune", ("Suicune", 0xF5)},
            {"larivitar", ("Larvitar", 0xF6)},
            {"pupitar", ("Pupitar", 0xF7)},
            {"tyranitar", ("Tyranitar", 0xF8)},
            {"lugia", ("Lugia", 0xF9)},
            {"hooh", ("Ho-Oh", 0xFA)},
            {"celebi", ("Celebi", 0xFB)},
        };
        
        private static readonly Dictionary<string, (string name, PocketType pocketType, byte id, bool isTeruSama)> _Items = new Dictionary<string, (string, PocketType, byte, bool)> {
            // {"unknown1", ("Unknown 1", PocketType.UNKNOWN, 0x00, true)},
            {"none", ("None", PocketType.UNKNOWN, 0x00, true)},

            {"ballmaster", ("Master Ball", PocketType.POKE_BALLS, 0x01, false)},
            {"ballultra", ("Ultra Ball", PocketType.POKE_BALLS, 0x02, false)},
            {"powderbright", ("BrightPowder", PocketType.ITEMS, 0x03, false)},
            {"ballgreat", ("Great Ball", PocketType.POKE_BALLS, 0x04, false)},
            {"ballpoke", ("Poké Ball", PocketType.POKE_BALLS, 0x05, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x06, true)},
            {"bicycle", ("Bicycle", PocketType.ITEMS, 0x07, false)},
            {"stonemoon", ("Moon Stone", PocketType.ITEMS, 0x08, false)},
            {"antidote", ("Antidote", PocketType.ITEMS, 0x09, false)},
            {"healburn", ("Burn Heal", PocketType.ITEMS, 0x0A, false)},
            {"healice", ("Ice Heal", PocketType.ITEMS, 0x0B, false)},
            {"awkening", ("Awakening", PocketType.ITEMS, 0x0C, false)},
            {"healparalyz", ("Parlyz Heal", PocketType.ITEMS, 0x0D, false)},
            {"restorefull", ("Full Restore", PocketType.ITEMS, 0x0E, false)},
            {"potionmax", ("Max Potion", PocketType.ITEMS, 0x0F, false)},
            {"potionhyper", ("Hyper Potion", PocketType.ITEMS, 0x10, false)},
            {"potionsuper", ("Super Potion", PocketType.ITEMS, 0x11, false)},
            {"potion", ("Potion", PocketType.ITEMS, 0x12, false)},
            {"ropeescape", ("Escape Rope", PocketType.ITEMS, 0x13, false)},
            {"repel", ("Repel", PocketType.ITEMS, 0x14, false)},
            {"elixermax", ("Max Elixer", PocketType.ITEMS, 0x15, false)},
            {"stonefire", ("Fire Stone", PocketType.ITEMS, 0x16, false)},
            {"stonethunder", ("Thunderstone", PocketType.ITEMS, 0x17, false)},
            {"stonewater", ("Water Stone", PocketType.ITEMS, 0x18, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x19, false)},
            {"uphp", ("HP Up", PocketType.ITEMS, 0x1A, false)},
            {"protein", ("Protein", PocketType.ITEMS, 0x1B, false)},
            {"iron", ("Iron", PocketType.ITEMS, 0x1C, false)},
            {"carbos", ("Carbos", PocketType.ITEMS, 0x1D, false)},
            {"luckypunch", ("Lucky Punch", PocketType.ITEMS, 0x1E, false)},
            {"calcium", ("Calcium", PocketType.ITEMS, 0x1F, false)},
            {"rarecandy", ("Rare Candy", PocketType.ITEMS, 0x20, false)},
            {"xaccuracy", ("X Accuracy", PocketType.ITEMS, 0x21, false)},
            {"stoneleaf", ("Leaf Stone", PocketType.ITEMS, 0x22, false)},
            {"powdermetal", ("Metal Powder", PocketType.ITEMS, 0x23, false)},
            {"nugget", ("Nugget", PocketType.ITEMS, 0x24, false)},
            {"pokedoll", ("Poké Doll", PocketType.ITEMS, 0x25, false)},
            {"healfull", ("Full Heal", PocketType.ITEMS, 0x26, false)},
            {"revive", ("Revive", PocketType.ITEMS, 0x27, false)},
            {"revivemax", ("Max Revive", PocketType.ITEMS, 0x28, false)},
            {"guardspec", ("Guard Spec.", PocketType.ITEMS, 0x29, false)},
            {"repelsuper", ("Super Repel", PocketType.ITEMS, 0x2A, false)},
            {"repelmax", ("Max Repel", PocketType.ITEMS, 0x2B, false)},
            {"direhit", ("Dire Hit", PocketType.ITEMS, 0x2C, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x2D, true)},
            {"freshwater", ("Fresh Water", PocketType.ITEMS, 0x2E, false)},
            {"sodapop", ("Soda Pop", PocketType.ITEMS, 0x2F, false)},
            {"lemonade", ("Lemonade", PocketType.ITEMS, 0x30, false)},
            {"xattack", ("X Attack", PocketType.ITEMS, 0x31, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x32, true)},
            {"xdefend", ("X Defend", PocketType.ITEMS, 0x33, false)},
            {"xspeed", ("X Speed", PocketType.ITEMS, 0x34, false)},
            {"xspecial", ("X Special", PocketType.KEY_ITEMS, 0x35, false)},
            {"coincase", ("Coin Case", PocketType.KEY_ITEMS, 0x36, false)},
            {"itemfinder", ("Itemfinder", PocketType.ITEMS, 0x37, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x38, true)},
            {"expshare", ("Exp.Share", PocketType.ITEMS, 0x39, false)},
            {"rodold", ("Old Rod", PocketType.KEY_ITEMS, 0x3A, false)},
            {"rodgood", ("Good Rod", PocketType.KEY_ITEMS, 0x3B, false)},
            {"leafsilver", ("Silver Leaf", PocketType.ITEMS, 0x3C, false)},
            {"rodsuper", ("Super Rod", PocketType.KEY_ITEMS, 0x3D, false)},
            {"ppup", ("PP Up", PocketType.ITEMS, 0x3E, false)},
            {"ether", ("Ether", PocketType.ITEMS, 0x3F, false)},
            {"ethermax", ("Max Ether", PocketType.ITEMS, 0x40, false)},
            {"elixer", ("Elixer", PocketType.ITEMS, 0x41, false)},
            {"scalered", ("Red Scale", PocketType.KEY_ITEMS, 0x42, false)},
            {"potionsecret", ("SecretPotion", PocketType.KEY_ITEMS, 0x43, false)},
            {"ticketss", ("S.S. Ticket", PocketType.KEY_ITEMS, 0x44, false)},
            {"eggmystery", ("Mystery Egg", PocketType.KEY_ITEMS, 0x45, false)},
            {"clearbell", ("Clear Bell", PocketType.KEY_ITEMS, 0x46, false)},
            {"wingsilver", ("Silver Wing", PocketType.KEY_ITEMS, 0x47, false)},
            {"moomoomilk", ("Moomoo Milk", PocketType.ITEMS, 0x48, false)},
            {"quickclaw", ("Quick Claw", PocketType.ITEMS, 0x49, false)},
            {"berrypsncure", ("PSNCureBerry", PocketType.ITEMS, 0x4A, false)},
            {"leafgold", ("Gold Leaf", PocketType.ITEMS, 0x4B, false)},
            {"softsand", ("Soft Sand", PocketType.ITEMS, 0x4C, false)},
            {"sharpbeak", ("Sharp Beak", PocketType.ITEMS, 0x4D, false)},
            {"berryprzcure", ("PRZCureBerry", PocketType.ITEMS, 0x4E, false)},
            {"berryburnt", ("Burnt Berry", PocketType.ITEMS, 0x4F, false)},
            {"berryice", ("Ice Berry", PocketType.ITEMS, 0x50, false)},
            {"poisonbarb", ("Poison Barb", PocketType.ITEMS, 0x51, false)},
            {"kingsrock", ("King's Rock", PocketType.ITEMS, 0x52, false)},
            {"berrybitter", ("Bitter Berry", PocketType.ITEMS, 0x53, false)},
            {"berrymint", ("Mint Berry", PocketType.ITEMS, 0x54, false)},
            {"apricornred", ("Red Apricorn", PocketType.ITEMS, 0x55, false)},
            {"mushroomtiny", ("TinyMushroom", PocketType.ITEMS, 0x56, false)},
            {"mushroombig", ("Big Mushroom", PocketType.ITEMS, 0x57, false)},
            {"powdersilver", ("SilverPowder", PocketType.ITEMS, 0x58, false)},
            {"apricornblu", ("Blu Apricorn", PocketType.ITEMS, 0x59, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x5A, true)},
            {"amuletcoin", ("Amulet Coin", PocketType.ITEMS, 0x5B, false)},
            {"apricornylw", ("Ylw Apricorn", PocketType.ITEMS, 0x5C, false)},
            {"apricorngrn", ("Grn Apricorn", PocketType.ITEMS, 0x5D, false)},
            {"cleansetag", ("Cleanse Tag", PocketType.ITEMS, 0x5E, false)},
            {"mysticwater", ("Mystic Water", PocketType.ITEMS, 0x5F, false)},
            {"twistedspoon", ("TwistedSpoon", PocketType.ITEMS, 0x60, false)},
            {"apricornwht", ("Wht Apricorn", PocketType.ITEMS, 0x61, false)},
            {"blackbelt", ("Blackbelt", PocketType.ITEMS, 0x62, false)},
            {"apricornblk", ("Blk Apricorn", PocketType.ITEMS, 0x63, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x64, true)},
            {"apricornpnk", ("Pnk Apricorn", PocketType.ITEMS, 0x65, false)},
            {"glassesblack", ("BlackGlasses", PocketType.ITEMS, 0x66, false)},
            {"slowpoketail", ("SlowpokeTail", PocketType.ITEMS, 0x67, false)},
            {"bowpink", ("Pink Bow", PocketType.ITEMS, 0x68, false)},
            {"stick", ("Stick", PocketType.ITEMS, 0x69, false)},
            {"smokeball", ("Smoke Ball", PocketType.ITEMS, 0x6A, false)},
            {"nevermeltice", ("NeverMeltIce", PocketType.ITEMS, 0x6B, false)},
            {"magnet", ("Magnet", PocketType.ITEMS, 0x6C, false)},
            {"berrymiracle", ("MiracleBerry", PocketType.ITEMS, 0x6D, false)},
            {"pearl", ("Pearl", PocketType.ITEMS, 0x6E, false)},
            {"pearlbig", ("Big Pearl", PocketType.ITEMS, 0x6F, false)},
            {"everstone", ("Everstone", PocketType.ITEMS, 0x70, false)},
            {"spelltag", ("Spell Tag", PocketType.ITEMS, 0x71, false)},
            {"ragecandybar", ("RageCandyBar", PocketType.ITEMS, 0x72, false)},
            {"gsball", ("GS Ball", PocketType.KEY_ITEMS, 0x73, false)},
            {"cardblue", ("Blue Card", PocketType.KEY_ITEMS, 0x74, false)},
            {"miracleseed", ("Miracle Seed", PocketType.ITEMS, 0x75, false)},
            {"thickclub", ("Thick Club", PocketType.ITEMS, 0x76, false)},
            {"focusband", ("Focus Band", PocketType.ITEMS, 0x77, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x78, true)},
            {"energypowder", ("EnergyPowder", PocketType.ITEMS, 0x79, false)},
            {"energyroot", ("Energy Root", PocketType.ITEMS, 0x7A, false)},
            {"healpowder", ("Heal Powder", PocketType.ITEMS, 0x7B, false)},
            {"herbrevival", ("Revival Herb", PocketType.ITEMS, 0x7C, false)},
            {"hardstone", ("Hard Stone", PocketType.ITEMS, 0x7D, false)},
            {"luckyegg", ("Lucky Egg", PocketType.ITEMS, 0x7E, false)},
            {"keycard", ("Card Key", PocketType.KEY_ITEMS, 0x7F, false)},
            {"machinepart", ("Machine Part", PocketType.KEY_ITEMS, 0x80, false)},
            {"ticketegg", ("Egg Ticket", PocketType.KEY_ITEMS, 0x81, false)},
            {"lostitem", ("Lost Item", PocketType.KEY_ITEMS, 0x82, false)},
            {"stardust", ("Stardust", PocketType.ITEMS, 0x83, false)},
            {"starpiece", ("Star Piece", PocketType.ITEMS, 0x84, false)},
            {"keybasement", ("Basement Key", PocketType.KEY_ITEMS, 0x85, false)},
            {"pass", ("Pass", PocketType.KEY_ITEMS, 0x86, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x87, true)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x88, true)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x89, true)},
            {"charcoal", ("Charcoal", PocketType.ITEMS, 0x8A, false)},
            {"berryjuice", ("Berry Juice", PocketType.ITEMS, 0x8B, false)},
            {"scopelens", ("Scope Lens", PocketType.ITEMS, 0x8C, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x8D, true)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x8E, true)},
            {"metalcoat", ("Metal Coat", PocketType.ITEMS, 0x8F, false)},
            {"dragonfang", ("Dragon Fang", PocketType.ITEMS, 0x90, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x91, true)},
            {"leftovers", ("Leftovers", PocketType.ITEMS, 0x92, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x93, true)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x94, true)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x95, true)},
            {"berrymystery", ("MysteryBerry", PocketType.ITEMS, 0x96, false)},
            {"scaledragon", ("Dragon Scale", PocketType.ITEMS, 0x97, false)},
            {"berserkgene", ("Berserk Gene", PocketType.ITEMS, 0x98, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x99, true)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x9A, true)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0x9B, true)},
            {"sacredash", ("Sacred Ash", PocketType.ITEMS, 0x9C, false)},
            {"ballheavy", ("Heavy Ball", PocketType.POKE_BALLS, 0x9D, false)},
            {"mailflower", ("Flower Mail", PocketType.ITEMS, 0x9E, false)},
            {"balllevel", ("Level Ball", PocketType.POKE_BALLS, 0x9F, false)},
            {"balllure", ("Lure Ball", PocketType.POKE_BALLS, 0xA0, false)},
            {"ballfast", ("Fast Ball", PocketType.POKE_BALLS, 0xA1, false)},
            // {"", ("Teru-sama", PocketType.ITEMS, 0xA2, true)},
            {"lightball", ("Light Ball", PocketType.ITEMS, 0xA3, false)},
            {"ballfriend", ("Friend Ball", PocketType.POKE_BALLS, 0xA4, false)},
            {"ballmoon", ("Moon Ball", PocketType.POKE_BALLS, 0xA5, false)},
            {"balllove", ("Love Ball", PocketType.POKE_BALLS, 0xA6, false)},
            {"boxnormal", ("Normal Box", PocketType.ITEMS, 0xA7, false)},
            {"boxgorgeous", ("Gorgeous Box", PocketType.ITEMS, 0xA8, false)},
            {"stonesun", ("Sun Stone", PocketType.ITEMS, 0xA9, false)},
            {"bowpolkadot", ("Polkadot Bow", PocketType.ITEMS, 0xAA, false)},
            // {"terusama", ("Teru-sama", PocketType.ITEMS, 0xAB, true)},
            {"upgrade", ("Up-Grade", PocketType.ITEMS, 0xAC, false)},
            {"berry", ("Berry", PocketType.ITEMS, 0xAD, false)},
            {"berrygold", ("Gold Berry", PocketType.ITEMS, 0xAE, false)},
            {"bottlesquirt", ("SquirtBottle", PocketType.KEY_ITEMS, 0xAF, false)},
            // {"terusama", ("Teru-sama", PocketType.ITEMS, 0xB0, true)},//Dummy item
            {"parkball", ("Park Ball", PocketType.POKE_BALLS, 0xB1, false)},
            {"wingrainbow", ("Rainbow Wing", PocketType.KEY_ITEMS, 0xB2, false)},
            // {"terusama", ("Teru-sama", PocketType.ITEMS, 0xB3, true)},
            {"brickpiece", ("Brick Piece", PocketType.ITEMS, 0xB4, false)},
            {"mailsurf", ("Surf Mail", PocketType.ITEMS, 0xB5, false)},
            {"mailliteblue", ("Litebluemail", PocketType.ITEMS, 0xB6, false)},
            {"mailportrait", ("Portraitmail", PocketType.ITEMS, 0xB7, false)},
            {"maillovely", ("Lovely Mail", PocketType.ITEMS, 0xB8, false)},
            {"maileon", ("Eon Mail", PocketType.ITEMS, 0xB9, false)},
            {"mailmorph", ("Morph Mail", PocketType.ITEMS, 0xBA, false)},
            {"mailbluesky", ("Bluesky Mail", PocketType.ITEMS, 0xBB, false)},
            {"mailmusic", ("Music Mail", PocketType.ITEMS, 0xBC, false)},
            {"mailmirage", ("Mirage Mail", PocketType.ITEMS, 0xBD, false)},
            // {"terusama", ("Teru-sama", PocketType.ITEMS, 0xBE, true)},//Dummy item
            {"tm01", ("TM01", PocketType.TM_AND_HM, 0xBF, true)},
            {"tm02", ("TM02", PocketType.TM_AND_HM, 0xC0, false)},
            {"tm03", ("TM03", PocketType.TM_AND_HM, 0xC1, false)},
            {"tm04", ("TM04", PocketType.TM_AND_HM, 0xC2, false)},
            // {"tm04_item", ("TM04", PocketType.ITEMS, 0xC3, false)},//Items pocket
            {"tm05", ("TM05", PocketType.TM_AND_HM, 0xC4, false)},
            {"tm06", ("TM06", PocketType.TM_AND_HM, 0xC5, false)},
            {"tm07", ("TM07", PocketType.TM_AND_HM, 0xC6, false)},
            {"tm08", ("TM08", PocketType.TM_AND_HM, 0xC7, false)},
            {"tm09", ("TM09", PocketType.TM_AND_HM, 0xC8, false)},
            {"tm10", ("TM10", PocketType.TM_AND_HM, 0xC9, false)},
            {"tm11", ("TM11", PocketType.TM_AND_HM, 0xCA, false)},
            {"tm12", ("TM12", PocketType.TM_AND_HM, 0xCB, false)},
            {"tm13", ("TM13", PocketType.TM_AND_HM, 0xCC, false)},
            {"tm14", ("TM14", PocketType.TM_AND_HM, 0xCD, false)},
            {"tm15", ("TM15", PocketType.TM_AND_HM, 0xCE, false)},
            {"tm16", ("TM16", PocketType.TM_AND_HM, 0xCF, false)},
            {"tm17", ("TM17", PocketType.TM_AND_HM, 0xD0, false)},
            {"tm18", ("TM18", PocketType.TM_AND_HM, 0xD1, false)},
            {"tm19", ("TM19", PocketType.TM_AND_HM, 0xD2, false)},
            {"tm20", ("TM20", PocketType.TM_AND_HM, 0xD3, false)},
            {"tm21", ("TM21", PocketType.TM_AND_HM, 0xD4, false)},
            {"tm22", ("TM22", PocketType.TM_AND_HM, 0xD5, false)},
            {"tm23", ("TM23", PocketType.TM_AND_HM, 0xD6, false)},
            {"tm24", ("TM24", PocketType.TM_AND_HM, 0xD7, false)},
            {"tm25", ("TM25", PocketType.TM_AND_HM, 0xD8, false)},
            {"tm26", ("TM26", PocketType.TM_AND_HM, 0xD9, false)},
            {"tm27", ("TM27", PocketType.TM_AND_HM, 0xDA, false)},
            {"tm28", ("TM28", PocketType.TM_AND_HM, 0xDB, false)},
            // {"tm28_item", ("TM28", PocketType.ITEMS, 0xDC, true)},//Items pocket
            {"tm29", ("TM29", PocketType.TM_AND_HM, 0xDD, false)},
            {"tm30", ("TM30", PocketType.TM_AND_HM, 0xDE, false)},
            {"tm31", ("TM31", PocketType.TM_AND_HM, 0xDF, false)},
            {"tm32", ("TM32", PocketType.TM_AND_HM, 0xE0, false)},
            {"tm33", ("TM33", PocketType.TM_AND_HM, 0xE1, false)},
            {"tm34", ("TM34", PocketType.TM_AND_HM, 0xE2, false)},
            {"tm35", ("TM35", PocketType.TM_AND_HM, 0xE3, false)},
            {"tm36", ("TM36", PocketType.TM_AND_HM, 0xE4, false)},
            {"tm37", ("TM37", PocketType.TM_AND_HM, 0xE5, false)},
            {"tm38", ("TM38", PocketType.TM_AND_HM, 0xE6, false)},
            {"tm39", ("TM39", PocketType.TM_AND_HM, 0xE7, false)},
            {"tm40", ("TM40", PocketType.TM_AND_HM, 0xE8, false)},
            {"tm41", ("TM41", PocketType.TM_AND_HM, 0xE9, false)},
            {"tm42", ("TM42", PocketType.TM_AND_HM, 0xEA, false)},
            {"tm43", ("TM43", PocketType.TM_AND_HM, 0xEB, false)},
            {"tm44", ("TM44", PocketType.TM_AND_HM, 0xEC, false)},
            {"tm45", ("TM45", PocketType.TM_AND_HM, 0xED, false)},
            {"tm46", ("TM46", PocketType.TM_AND_HM, 0xEE, false)},
            {"tm47", ("TM47", PocketType.TM_AND_HM, 0xEF, false)},
            {"tm48", ("TM48", PocketType.TM_AND_HM, 0xF0, false)},
            {"tm49", ("TM49", PocketType.TM_AND_HM, 0xF1, false)},
            {"tm50", ("TM50", PocketType.TM_AND_HM, 0xF2, false)},
            {"hm01", ("HM01", PocketType.TM_AND_HM, 0xF3, false)},
            {"hm02", ("HM02", PocketType.TM_AND_HM, 0xF4, false)},
            {"hm03", ("HM03", PocketType.TM_AND_HM, 0xF5, false)},
            {"hm04", ("HM04", PocketType.TM_AND_HM, 0xF6, false)},
            {"hm05", ("HM05", PocketType.TM_AND_HM, 0xF7, false)},
            {"hm06", ("HM06", PocketType.TM_AND_HM, 0xF8, false)},
            {"hm07", ("HM07", PocketType.TM_AND_HM, 0xF9, false)},
            {"hm08", ("HM08", PocketType.TM_AND_HM, 0xFA, false)},
            {"hm09", ("HM09", PocketType.TM_AND_HM, 0xFB, false)},
            {"hm10", ("HM10", PocketType.TM_AND_HM, 0xFC, false)},
            {"hm11", ("HM11", PocketType.TM_AND_HM, 0xFD, false)},
            {"hm12", ("HM12", PocketType.TM_AND_HM, 0xFE, false)},
            {"cancel", ("Cancel", PocketType.TM_AND_HM, 0xFF, true)}
        };
        
        private static readonly Dictionary<string, (string name, byte id, byte maxPP)> _Moves = new Dictionary<string, (string, byte, byte)>
        {
            //Gen I moves
            {"pound", ("Pound", 0x01, 35)},
            {"karatechop", ("Karate Chop", 0x02, 25)},
            {"doubleslap", ("Double Slap", 0x03, 10)},
            {"cometpunch", ("Comet Punch", 0x04, 15)},
            {"megapunch", ("Mega Punch", 0x05, 20)},
            {"payday", ("Pay Day", 0x06, 20)},
            {"firepunch", ("Fire Punch", 0x07, 15)},
            {"icepunch", ("Ice Punch", 0x08, 15)},
            {"thunderpunch", ("Thunder Punch", 0x09, 15)},
            {"scratch", ("Scratch", 0x0A, 35)},
            {"visegrip", ("Vise Grip", 0x0B, 30)},
            {"guillotine", ("Guillotine", 0x0C, 5)},
            {"razorwind", ("Razor Wind", 0x0D, 10)},
            {"swordsdance", ("Swords Dance", 0x0E, 30)},
            {"cut", ("Cut", 0x0F, 30)},
            {"gust", ("Gust", 0x10, 35)},
            {"wingattack", ("Wing Attack", 0x11, 35)},
            {"whirlwind", ("Whirldwind", 0x12, 20)},
            {"fly", ("Fly", 0x13, 15)},
            {"bind", ("Bind", 0x14, 20)},
            {"slam", ("Slam", 0x15, 20)},
            {"vinewhip", ("Vine Whip", 0x16, 10)},
            {"stomp", ("Stomp", 0x17, 20)},
            {"doublekick", ("Double Kick", 0x18, 30)},
            {"megakick", ("Mega Kick", 0x19, 5)},
            {"jumpkick", ("Jump Kick", 0x1A, 25)},
            {"rollingkick", ("Rolling Kick", 0x1B, 15)},
            {"sandattack", ("Sand-attack", 0x1C, 15)},
            {"headbutt", ("Headbutt", 0x1D, 15)},
            {"hornattack", ("Horn Attack", 0x1E, 25)},
            {"furyattack", ("Fury Attack", 0x1F, 20)},
            {"horndrill", ("Horn Drill", 0x20, 5)},
            {"tackle", ("Tackle", 0x21, 35)},
            {"bodyslam", ("Body Slam", 0x22, 15)},
            {"wrap", ("Wrap", 0x23, 20)},
            {"takedown", ("Take Down", 0x24, 20)},
            {"thrash", ("Thrash", 0x25, 20)},
            {"doubleedge", ("Double-edge", 0x26, 15)},
            {"tailwhip", ("Tail Whip", 0x27, 30)},
            {"poisonsting", ("Poison Sting", 0x28, 35)},
            {"twineedle", ("Twineedle", 0x29, 20)},
            {"pinmissile", ("Pin Missle", 0x2A, 20)},
            {"leer", ("Leer", 0x2B, 30)},
            {"bite", ("Bite", 0x2C, 25)},
            {"growl", ("Growl", 0x2D, 40)},
            {"roar", ("Roar", 0x2E, 20)},
            {"sing", ("Sing", 0x2F, 15)},
            {"supersonic", ("Supersonic", 0x30, 20)},
            {"sonicboom", ("Sonicboom", 0x31, 20)},
            {"disable", ("Disable", 0x32, 20)},
            {"acid", ("Acid", 0x33, 30)},
            {"ember", ("Ember", 0x34, 25)},
            {"flamethrower", ("Flamethrower", 0x35, 15)},
            {"mist", ("Mist", 0x36, 30)},
            {"watergun", ("Water Gun", 0x37, 25)},
            {"hydropump", ("Hydro Pump", 0x38, 5)},
            {"surf", ("Surf", 0x39, 15)},
            {"icebeam", ("Ice Beam", 0x3A, 10)},
            {"blizzard", ("Blizzard", 0x3B, 5)},
            {"psybeam", ("Psybeam", 0x3C, 20)},
            {"bubblebeam", ("Bubblebeam", 0x3D, 20)},
            {"aurorabeam", ("Aurora Beam", 0x3E, 20)},
            {"hyperbeam", ("Hyper Beam", 0x3F, 5)},
            {"peck", ("Peck", 0x40, 35)},
            {"drillpeck", ("Drill Peck", 0x41, 20)},
            {"submission", ("Submission", 0x42, 25)},
            {"lowkick", ("Low Kick", 0x43, 20)},
            {"counter", ("Counter", 0x44, 20)},
            {"seismictoss", ("Seismic Toss", 0x45, 20)},
            {"strength", ("Strength", 0x46, 15)},
            {"absorb", ("Absorb", 0x47, 20)},
            {"megadrain", ("Mega Drain", 0x48, 10)},
            {"leechseed", ("Leech Seed", 0x49, 10)},
            {"growth", ("Growth", 0x4A, 40)},
            {"razorleaf", ("Razor Leaf", 0x4B, 25)},
            {"solarbeam", ("Solarbeam", 0x4C, 10)},
            {"poisonpowder", ("Poisonpowder", 0x4D, 35)},
            {"stunspore", ("Stun Spore", 0x4E, 30)},
            {"sleeppowder", ("Sleep Powder", 0x4F, 15)},
            {"petaldance", ("Petal Dance", 0x50, 20)},
            {"stringshot", ("String Shot", 0x51, 40)},
            {"dragonrage", ("Dragon Rage", 0x52, 10)},
            {"firespin", ("Fire Spin", 0x53, 15)},
            {"thundershock", ("Thundershock", 0x54, 30)},
            {"thunderbolt", ("Thunderbolt", 0x55, 15)},
            {"thunderwave", ("Thunder Wave", 0x56, 20)},
            {"thunder", ("Thunder", 0x57, 10)},
            {"rockthrow", ("Rock Throw", 0x58, 15)},
            {"earthquake", ("Earthquake", 0x59, 10)},
            {"fissure", ("Fissure", 0x5A, 5)},
            {"dig", ("Dig", 0x5B, 10)},
            {"toxic", ("Toxic", 0x5C, 10)},
            {"confusion", ("Confusion", 0x5D, 25)},
            {"psychic", ("Psychic", 0x5E, 10)},
            {"hypnosis", ("Hypnosis", 0x5F, 20)},
            {"meditate", ("Meditate", 0x60, 40)},
            {"agility", ("Agility", 0x61, 30)},
            {"quickattack", ("Quick Attack", 0x62, 30)},
            {"rage", ("Rage", 0x63, 20)},
            {"teleport", ("Teleport", 0x64, 20)},
            {"nightshade", ("Nightshade", 0x65, 15)},
            {"mimic", ("Mimic", 0x66, 10)},
            {"screech", ("Screech", 0x67, 40)},
            {"doubleteam", ("Double Team", 0x68, 15)},
            {"recover", ("Recover", 0x69, 20)},
            {"harden", ("Harden", 0x6A, 30)},
            {"minimize", ("Minimize", 0x6B, 20)},
            {"smokescreen", ("Smokescreen", 0x6C, 20)},
            {"confuseray", ("Confuse Ray", 0x6D, 10)},
            {"withdraw", ("Withdraw", 0x6E, 40)},
            {"defensecurl", ("Defense Curl", 0x6F, 40)},
            {"barrier", ("barrier", 0x70, 30)},
            {"lightscreen", ("Light Screen", 0x71, 30)},
            {"haze", ("Haze", 0x72, 30)},
            {"reflect", ("Reflect", 0x73, 20)},
            {"focusenergy", ("Focus Energy", 0x74, 30)},
            {"bide", ("Bide", 0x75, 10)},
            {"metronome", ("Metronome", 0x76, 10)},
            {"mirrormove", ("Mirror Move", 0x77, 20)},
            {"selfdestruct", ("Self-Destruct", 0x78, 5)},
            {"eggbomb", ("Egg Bomb", 0x79, 10)},
            {"lick", ("Lick", 0x7A, 30)},
            {"smog", ("Smog", 0x7B, 20)},
            {"sludge", ("Sludge", 0x7C, 20)},
            {"boneclub", ("Bone Club", 0x7D, 20)},
            {"fireblast", ("Fire Blast", 0x7E, 5)},
            {"waterfall", ("Waterfall", 0x7F, 15)},
            {"clamp", ("Clamp", 0x80, 10)},
            {"swift", ("Swift", 0x81, 20)},
            {"skullbash", ("Skullbash", 0x82, 15)},
            {"spikecannon", ("Spike Cannon", 0x83, 15)},
            {"constrict", ("Constrict", 0x84, 35)},
            {"amnesia", ("Amnesia", 0x85, 20)},
            {"kinesis", ("Kinesis", 0x86, 15)},
            {"softboiled", ("Softboiled", 0x87, 10)},
            {"hijumpkick", ("High Jump Kick", 0x88, 20)},
            {"glare", ("Glare", 0x89, 30)},
            {"dreameater", ("Dream Eater", 0x8A, 15)},
            {"poisongas", ("Poison Gas", 0x8B, 40)},
            {"barrage", ("Barrage", 0x8C, 20)},
            {"leechlife", ("Leech Life", 0x8D, 15)},
            {"lovelykiss", ("Lovely Kiss", 0x8E, 10)},
            {"skyattack", ("Sky Attack", 0x8F, 5)},
            {"transform", ("Transform", 0x90, 10)},
            {"bubble", ("Bubble", 0x91, 30)},
            {"dizzypunch", ("Dizzy Punch", 0x92, 10)},
            {"spore", ("Spore", 0x93, 15)},
            {"flash", ("Flash", 0x94, 20)},
            {"psywave", ("Psywave", 0x95, 15)},
            {"splash", ("Splash", 0x96, 40)},
            {"acidarmor", ("Acid Armor", 0x97, 40)},
            {"crabhammer", ("Crabhammer", 0x98, 10)},
            {"explosion", ("Explosion", 0x99, 05)},
            {"furyswipes", ("Fury Swipes", 0x9A, 15)},
            {"bonemerang", ("Bonemerang", 0x9B, 10)},
            {"rest", ("rest", 0x9C, 10)},
            {"rockslide", ("Rock Slide", 0x9D, 10)},
            {"hyperfang", ("Hyper Fang", 0x9E, 15)},
            {"sharpen", ("Sharpen", 0x9F, 30)},
            {"conversion", ("Conversion", 0xA0, 30)},
            {"triattack", ("Tri Attack", 0xA1, 10)},
            {"superfang", ("Super Fang", 0xA2, 10)},
            {"slash", ("Slash", 0xA3, 20)},
            {"substitute", ("Substitute", 0xA4, 10)},
            {"struggle", ("Struggle", 0xA5, 10)},

            //Gen II Moves
            {"sketch", ("Sketch", 0xA6, 01)},
            {"triplekick", ("Triple Kick", 0xA7, 10)},
            {"thief", ("Thief", 0xA8, 10)},
            {"spiderweb", ("Spider Web", 0xA9, 10)},
            {"mindreader", ("Mind Reader", 0xAA, 05)},
            {"nightmare", ("Nightmare", 0xAB, 15)},
            {"flamewheel", ("Flame Wheel", 0xAC, 25)},
            {"snore", ("Snore", 0xAD, 15)},
            {"curse", ("Curse", 0xAE, 10)},
            {"flail", ("Flail", 0xAF, 15)},
            {"conversion2", ("Conversion2", 0xB0, 30)},
            {"aeroblast", ("Aeroblast", 0xB1, 5)},
            {"cottonspore", ("Cotton Spore", 0xB2, 40)},
            {"reversal", ("Reversal", 0xB3, 15)},
            {"spite", ("Spite", 0xB4, 10)},
            {"powdersnow", ("Powder Snow", 0xB5, 25)},
            {"protect", ("Protect", 0xB6, 10)},
            {"machpunch", ("Mach Punch", 0xB7, 30)},
            {"scaryface", ("Scary Face", 0xB8, 10)},
            {"feintattack", ("Feint Attack", 0xB9, 20)},
            {"sweetkiss", ("Sweet Kiss", 0xBA, 10)},
            {"beelydrum", ("Belly Drum", 0xBB, 10)},
            {"sludgebomb", ("Sludge Bomb", 0xBC, 10)},
            {"mudslap", ("Mud-Slap", 0xBD, 10)},
            {"octazooka", ("Octazooka", 0xBE, 10)},
            {"spikes", ("Spikes", 0xBF, 20)},
            {"zapcannon", ("Zap Cannon", 0xC0, 05)},
            {"foresight", ("Foresight", 0xC1, 40)},
            {"destinybond", ("Destiny Bond", 0xC2, 05)},
            {"perishsong", ("Perish Song", 0xC3, 05)},
            {"icywind", ("Ice Wind", 0xC4, 15)},
            {"detect", ("Detect", 0xC5, 05)},
            {"bonerush", ("Bone Rush", 0xC6, 10)},
            {"lockon", ("Lock-on", 0xC7, 5)},
            {"outrage", ("Outrage", 0xC8, 15)},
            {"sandstorm", ("Sandstorm", 0xC9, 10)},
            {"gigadrain", ("Giga Drain", 0xCA, 5)},
            {"endure", ("Endure", 0xCB, 10)},
            {"charm", ("Charm", 0xCC, 20)},
            {"rollout", ("Rollout", 0xCD, 20)},
            {"falseswipe", ("False Swipe", 0xCE, 40)},
            {"swagger", ("Swagger", 0xCF, 15)},
            {"milkdrink", ("Milk Drink", 0xD0, 10)},
            {"spark", ("Spark", 0xD1, 20)},
            {"furycutter", ("Fury Cutter", 0xD2, 20)},
            {"steelwing", ("Steel Wing", 0xD3, 25)},
            {"meanlook", ("Mean Look", 0xD4, 05)},
            {"attract", ("Attract", 0xD5, 15)},
            {"sleeptalk", ("Sleep Talk", 0xD6, 10)},
            {"healbell", ("Heal Bell", 0xD7, 05)},
            {"return", ("Return", 0xD8, 20)},
            {"present", ("Present", 0xD9, 15)},
            {"frustration", ("Frustration", 0xDA, 20)},
            {"safeguard", ("Safeguard", 0xDB, 25)},
            {"painsplit", ("Pain Split", 0xDC, 20)},
            {"sacredfire", ("Sacred Fire", 0xDD, 05)},
            {"magnitude", ("Magnitude", 0xDE, 30)},
            {"dynamicpunch", ("Dynamic Punch", 0xDF, 05)},
            {"megahorn", ("Megahorn", 0xE0, 10)},
            {"dragonbreath", ("Dragon Breath", 0xE1, 20)},
            {"batonpass", ("Baton Pass", 0xE2, 40)},
            {"encore", ("Encore", 0xE3, 05)},
            {"pursuit", ("Pursuit", 0xE4, 20)},
            {"rapidspin", ("Rapid Spin", 0xE5, 40)},
            {"sweetscent", ("Sweet Scent", 0xE6, 20)},
            {"irontail", ("Iron Tail", 0xE7, 15)},
            {"metalclaw", ("Metal Claw", 0xE8, 35)},
            {"vitalthrow", ("Vital Throw", 0xE9, 10)},
            {"morningsun", ("Morning Sun", 0xEA, 5)},
            {"synthesis", ("Synthesis", 0xEB, 5)},
            {"moonlight", ("Moonlight", 0xEC, 5)},
            {"hiddenpower", ("Hidden Power", 0xED, 15)},
            {"crosschop", ("Cross Chop", 0xEE, 5)},
            {"twister", ("Twister", 0xEF, 20)},
            {"raindance", ("Rain Dance", 0xF0, 5)},
            {"sunnyday", ("Sunny Day", 0xF1, 5)},
            {"crunch", ("Crunch", 0xF2, 15)},
            {"mirrorcoat", ("Mirror Coat", 0xF3, 20)},
            {"psychup", ("Psych Up", 0xF4, 10)},
            {"extremespeed", ("Extreme Speed", 0xF5, 5)},
            {"ancientpower", ("Ancient Power", 0xF6, 5)},
            {"shadowball", ("Shadow Ball", 0xF7, 15)},
            {"futuresight", ("Future Sight", 0xF8, 15)},
            {"rocksmash", ("Rock Smash", 0xF9, 15)},
            {"whirlpool", ("Whirlpool", 0xFA, 15)},
            {"beatup", ("Beat Up", 0XFB, 10)}
        };

        private static readonly Dictionary<char, byte> _PokeTextEncoding = new Dictionary<char, byte>() {
            {'A', 0x80},
            {'B', 0x81},
            {'C', 0x82},
            {'D', 0x83},
            {'E', 0x84},
            {'F', 0x85},
            {'G', 0x86},
            {'H', 0x87},
            {'I', 0x88},
            {'J', 0x89},
            {'K', 0x8A},
            {'L', 0x8B},
            {'M', 0x8C},
            {'N', 0x8D},
            {'O', 0x8E},
            {'P', 0x8F},
            {'Q', 0x90},
            {'R', 0x91},
            {'S', 0x92},
            {'T', 0x93},
            {'U', 0x94},
            {'V', 0x95},
            {'W', 0x96},
            {'X', 0x97},
            {'Y', 0x98},
            {'Z', 0x99},

            {'a', 0xA0},
            {'b', 0xA1},
            {'c', 0xA2},
            {'d', 0xA3},
            {'e', 0xA4},
            {'f', 0xA5},
            {'g', 0xA6},
            {'h', 0xA7},
            {'i', 0xA8},
            {'j', 0xA9},
            {'k', 0xAA},
            {'l', 0xAB},
            {'m', 0xAC},
            {'n', 0xAD},
            {'o', 0xAE},
            {'p', 0xAF},
            {'q', 0xB0},
            {'r', 0xB1},
            {'s', 0xB2},
            {'t', 0xB3},
            {'u', 0xB4},
            {'v', 0xB5},
            {'w', 0xB6},
            {'y', 0xB7},
            {'x', 0xB8},
            {'z', 0xB9},

            {'0', 0xF6},
            {'1', 0xF7},
            {'2', 0xF8},
            {'3', 0xF9},
            {'4', 0xFA},
            {'5', 0xFB},
            {'6', 0xFC},
            {'7', 0xFD},
            {'8', 0xFE},
            {'9', 0xFF}
        };

        public override List<ROMInfo> ROMTable => new List<ROMInfo>(new[]
        {
            new ROMInfo("Pokemon Crystal", null, Patching.Ignore, ROMStatus.ValidPatched, s => Patching.MD5(s,"301899B8087289A6436B0A241FBBB474")),
            new ROMInfo("Pokemon Crystal Randomizer", null, Patching.Ignore, ROMStatus.ValidPatched, s => s.Length==262160)
        });

        public override List<Effect> Effects
        {
            get
            {
                List<Effect> effects = new List<Effect>
                {
                    new Effect("Give Pokemon in party", "givepokemon", new [] { "pokepartyslot", "pokespecies", "helditem", "move1", "move2", "move3", "move4", "happiness", "level", "hp", "hpmax", "statatk", "statdef", "statspd", "statspatk", "statspdef" })
                };
                effects.AddRange(_Items.Skip(1).Select(t => new Effect($"{t.Value.name}", $"item_{t.Key}", ItemKind.Usable, "item")));

                effects.AddRange(_Pokemon.Select(t => new Effect($"{t.Value.name}", $"pokespecies_{t.Key}", ItemKind.Usable, "pokespecies")));
                effects.AddRange(_Moves.Select(t => new Effect($"{t.Value.name}", $"move1_{t.Key}", ItemKind.Usable, "move1")));
                effects.AddRange(_Moves.Select(t => new Effect($"{t.Value.name}", $"move2_{t.Key}", ItemKind.Usable, "move2")));
                effects.AddRange(_Moves.Select(t => new Effect($"{t.Value.name}", $"move3_{t.Key}", ItemKind.Usable, "move3")));
                effects.AddRange(_Moves.Select(t => new Effect($"{t.Value.name}", $"move4_{t.Key}", ItemKind.Usable, "move4")));
                effects.AddRange(_Items.Select(t => new Effect($"{t.Value.name}", $"helditem_{t.Key}", ItemKind.Usable, "helditem")));
                return effects;
            }
        }

        public override List<Common.ItemType> ItemTypes => new List<Common.ItemType>(new[]
        {
            new ItemType("Item", "item", ItemType.Subtype.ItemList),

            new ItemType("Pokemon Party Slot", "pokepartyslot", ItemType.Subtype.Slider, "{\"min\":1,\"max\":6}"),
            new ItemType("Pokemon", "pokespecies", ItemType.Subtype.ItemList),
            new ItemType("Move 1", "move1", ItemType.Subtype.ItemList),
            new ItemType("Move 2", "move2", ItemType.Subtype.ItemList),
            new ItemType("Move 3", "move3", ItemType.Subtype.ItemList),
            new ItemType("Move 4", "move4", ItemType.Subtype.ItemList),
            new ItemType("PP Move 1", "ppmove1", ItemType.Subtype.Slider, "{\"min\":0,\"max\":255}"),
            new ItemType("PP Move 2", "ppmove2", ItemType.Subtype.Slider, "{\"min\":0,\"max\":255}"),
            new ItemType("PP Move 3", "ppmove3", ItemType.Subtype.Slider, "{\"min\":0,\"max\":255}"),
            new ItemType("PP Move 4", "ppmove4", ItemType.Subtype.Slider, "{\"min\":0,\"max\":255}"),
            new ItemType("Happiness", "happiness", ItemType.Subtype.Slider, "{\"min\":0,\"max\":255}"),
            new ItemType("Level", "level", ItemType.Subtype.Slider, "{\"min\":1,\"max\":99}"),
            new ItemType("HP", "hp", ItemType.Subtype.Slider, "{\"min\":1,\"max\":65535}"),
            new ItemType("Max HP", "hpmax", ItemType.Subtype.Slider, "{\"min\":2,\"max\":65535}"),
            new ItemType("Attack Stat", "statatk", ItemType.Subtype.Slider, "{\"min\":1,\"max\":65535}"),
            new ItemType("Defense Stat", "statdef", ItemType.Subtype.Slider, "{\"min\":1,\"max\":65535}"),
            new ItemType("Speed Stat", "statspd", ItemType.Subtype.Slider, "{\"min\":1,\"max\":65535}"),
            new ItemType("Special Attack Stat", "statspatk", ItemType.Subtype.Slider, "{\"min\":1,\"max\":65535}"),
            new ItemType("Special Defense Stat", "statspdef", ItemType.Subtype.Slider, "{\"min\":1,\"max\":65535}"),
            new ItemType("Held Item", "helditem", ItemType.Subtype.ItemList)
        });

        public override List<(string, Action)> MenuActions => new List<(string, Action)>();

        public override Game Game { get; } = new Game(0x20, "Pokemon Crystal", "PokemonCrystal", "GB", ConnectorType.GBConnector);

        protected override bool IsReady(EffectRequest request) => true;//TODO

        protected override void RequestData(DataRequest request) => Respond(request, request.Key, null, false, $"Variable name \"{request.Key}\" not known");

        protected override void StartEffect(EffectRequest request)
        {
            if (!IsReady(request))
            {
                DelayEffect(request, TimeSpan.FromSeconds(5));
                return;
            }

            string[] codeParams = request.FinalCode.Split('_');
            //Log.Debug(String.Join(", ", codeParams));
            switch (codeParams[0])
            {
                case "givepokemon":
                    GivePokemon(request, codeParams);
                    break;
            }
        }

        protected override bool StopEffect(EffectRequest request)//TODO
        {
            switch (request.InventoryItem.BaseItem.Code)
            {
                default:
                    return true;
            }
        }

        public override bool StopAllEffects() => base.StopAllEffects(); //TODO

        private void GivePokemon(EffectRequest request, string[] codeParams) {
            var poke = _Pokemon[codeParams[3]];
            var held = _Items[codeParams[5]];
            (string name, byte id, byte maxPP) move1 = _Moves[codeParams[7]], move2 = _Moves[codeParams[9]], move3 = _Moves[codeParams[11]], move4 = _Moves[codeParams[13]];

            if (!byte.TryParse(codeParams[1], out byte pokePartySlot)) {
                return;
            }
            if (!byte.TryParse(codeParams[14], out byte happiness)) {
                return;
            }
            if (!byte.TryParse(codeParams[15], out byte level)) {
                return;
            }
            if (!ushort.TryParse(codeParams[16], out ushort hp)) {
                return;
            }
            if (!ushort.TryParse(codeParams[17], out ushort hpMax)) {
                return;
            }
            if (!ushort.TryParse(codeParams[18], out ushort statAtk)) {
                return;
            }
            if (!ushort.TryParse(codeParams[19], out ushort statDef)) {
                return;
            }
            if (!ushort.TryParse(codeParams[20], out ushort statSpd)) {
                return;
            }
            if (!ushort.TryParse(codeParams[21], out ushort statSpAtk)) {
                return;
            }
            if (!ushort.TryParse(codeParams[22], out ushort statSpDef)) {
                return;
            }

            TryEffect(request,
                () => true,
                () => {
                    var gTrainerID = GTrainerID();
                    if (!gTrainerID.check) return false;
                    PartyPokemon pokeObj = new PartyPokemon(
                        poke.id,
                        held.id,

                        move1.id,
                        move2.id,
                        move3.id,
                        move4.id,

                        gTrainerID.ret,//TODO get original trainer id
                        0x000000,//exp - 0x03 bytes
                        0x0000,//hp exp
                        0x0000,//atk exp
                        0x0000,//def exp
                        0x0000,//speed exp
                        0x0000,//special exp

                        0x1AAA,//DV TODO
                        move1.maxPP,//move 1 - pp
                        move2.maxPP,//move 2 - pp
                        move3.maxPP,//move 3 - pp
                        move4.maxPP,//move 4 - pp

                        happiness,
                        0x00,//pokerus
                        0x0000,//caught data - ?
                        level,
                        0x00,//status
                        hp,
                        hpMax,
                        statAtk,
                        statDef,
                        statSpd,
                        statSpAtk,
                        statSpDef
                    );
                    return SetPartyPokemon(GetSlotFromByte(pokePartySlot), pokeObj);
                },
                () => {
                    Connector.SendMessage($"{request.DisplayViewer} has sent you a {poke.name}!");
                }
            );
        }

        private static ulong GetPartyPokemonAddr(PokemonPartySlot slot) {
            return (ulong)((ulong)Address.ADDR_PARTY_POKEMON * (ulong)slot);
        }

        //<summary>
        //Get/set species of pokemon in party (actual - in pokemon object)
        //</summary>
        private (bool check, byte ret) GSPartyPokemonSpecies_Actual(PokemonPartySlot slot, ProcessingType processingType, PartyPokemon? obj, Byte? species) {
            const ulong addr = GetPartyPokemonAddr(slot);
            bool check = false;
            byte ret = 0xFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.species : obj.Value.species;
            } else if (processingType == ProcessingType.SET && !(species is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)species, "species");
            }
            return (check, ret);
        }

        private (bool check, byte ret) GSPartyPokemonHeldItem(PokemonPartySlot slot, ProcessingType processingType, PartyPokemon? obj, Byte? held) {
            const ulong addr = GetPartyPokemonAddr(slot);
            bool check = false;
            byte ret = 0xFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.heldItem : obj.Value.heldItem;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)held, "heldItem");
            }
            return (check, ret);
        }

        private (bool check, byte ret) GSPartyPokemonMove(PokemonPartySlot slot, ProcessingType processingType, MoveSlot moveSlot, PartyPokemon? obj, Byte? move) {
            const ulong addr = GetPartyPokemonAddr(slot);
            bool check = false;
            byte ret = 0xFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                switch(moveSlot) {
                    case MoveSlot.MOVE_1:
                        ret = obj is null ? pokemon.move1 : obj.Value.move1;
                        break;
                    case MoveSlot.MOVE_2:
                        ret = obj is null ? pokemon.move1 : obj.Value.move2;
                        break;
                    case MoveSlot.MOVE_3:
                        ret = obj is null ? pokemon.move1 : obj.Value.move3;
                        break;
                    case MoveSlot.MOVE_4:
                        ret = obj is null ? pokemon.move1 : obj.Value.move4;
                        break;
                }
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                switch(moveSlot) {
                    case MoveSlot.MOVE_1:
                        check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)move, "move1");
                        break;
                    case MoveSlot.MOVE_2:
                        check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)move, "move2");
                        break;
                    case MoveSlot.MOVE_3:
                        check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)move, "move3");
                        break;
                    case MoveSlot.MOVE_4:
                        check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)move, "move4");
                        break;
                }
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonOTId(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.heldItem : obj.Value.otId;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "otId");
            }
            return (check, ret);
        }

        private (bool check, uint ret) GSPartyPokemonExp(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Int32? newValue) {
            bool check = false;
            uint ret = 0xFFFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.exp : obj.Value.exp;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "exp");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonExpStatHp(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.hpStatExp : obj.Value.hpStatExp;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "hpStatExp");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonExpStatAtk(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.atkStatExp : obj.Value.atkStatExp;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "atkStatExp");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonExpStatDef(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.defStatExp : obj.Value.defStatExp;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "defStatExp");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonExpStatSpd(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.spdStatExp : obj.Value.spdStatExp;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "spdStatExp");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonExpStatSpe(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.speStatExp : obj.Value.speStatExp;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "speStatExp");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonDv(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.dv : obj.Value.dv;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "dv");
            }
            return (check, ret);
        }

        private (bool check, byte ret) GSPartyPokemonMovePP(PokemonPartySlot pokemonPartySlot, MoveSlot slot, PartyPokemon? obj, Byte? newValue) {
            bool check = false;
            byte ret = 0xFF;
                if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                switch(moveSlot) {
                    case MoveSlot.MOVE_1:
                        ret = obj is null ? pokemon.ppMove1 : obj.Value.ppMove1;
                        break;
                    case MoveSlot.MOVE_2:
                        ret = obj is null ? pokemon.ppMove2 : obj.Value.ppMove2;
                        break;
                    case MoveSlot.MOVE_3:
                        ret = obj is null ? pokemon.ppMove3 : obj.Value.ppMove3;
                        break;
                    case MoveSlot.MOVE_4:
                        ret = obj is null ? pokemon.ppMove4 : obj.Value.ppMove4;
                        break;
                }
            } else if (processingType == ProcessingType.SET && !(newValue is null)) {
                switch(moveSlot) {
                    case MoveSlot.MOVE_1:
                        check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)newValue, "ppMove1");
                        break;
                    case MoveSlot.MOVE_2:
                        check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)newValue, "ppMove2");
                        break;
                    case MoveSlot.MOVE_3:
                        check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)newValue, "ppMove3");
                        break;
                    case MoveSlot.MOVE_4:
                        check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)newValue, "ppMove4");
                        break;
                }
            }
            return (check, ret);
        }

        private (bool check, byte ret) GSPartyPokemonHappiness(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Byte? newValue) {
            bool check = false;
            byte ret = 0xFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.happiness : obj.Value.happiness;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)newValue, "happiness");
            }
            return (check, ret);
        }

        private (bool check, byte ret) GSPartyPokemonPokerusStatus(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Byte? newValue) {
            bool check = false;
            byte ret = 0xFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.pokerusStatus : obj.Value.pokerusStatus;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)newValue, "pokerusStatus");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonCaughtData(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.caughtData : obj.Value.caughtData;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "caughtData");
            }
            return (check, ret);
        }

        private (bool check, byte ret) GSPartyPokemonLevel(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Byte? newValue) {
            bool check = false;
            byte ret = 0xFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.level : obj.Value.level;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)newValue, "level");
            }
            return (check, ret);
        }

        private (bool check, byte ret) GSPartyPokemonStatus(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Byte? newValue) {
            bool check = false;
            byte ret = 0xFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.status : obj.Value.status;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (byte)newValue, "status");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonHp(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.hp : obj.Value.hp;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "hp");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonHpMax(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.hpMax : obj.Value.hpMax;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "hpMax");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonStatAtk(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.statAtk : obj.Value.statAtk;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "statAtk");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonStatDef(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.statDef : obj.Value.statDef;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "statDef");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonStatSpd(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.statSpd : obj.Value.statSpd;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "statSpd");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonStatSpAtk(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.statSpAtk : obj.Value.statSpAtk;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "statSpAtk");
            }
            return (check, ret);
        }

        private (bool check, ushort ret) GSPartyPokemonStatSpDef(PokemonPartySlot pokemonPartySlot, PartyPokemon? obj, Short? newValue) {
            bool check = false;
            ushort ret = 0xFFFF;
            if (processingType == ProcessingType.GET) {
                if (obj is null) check |= ConnectorExtensions.ReadObject(this.Connector, addr, out PartyPokemon pokemon);
                ret = obj is null ? pokemon.statSpDef : obj.Value.statSpDef;
            } else if (processingType == ProcessingType.SET && !(held is null)) {
                check |= ConnectorExtensions.WriteField(this.Connector, addr, (ushort)newValue, "statSpDef");
            }
            return (check, ret);
        }

        //<summary>
        // Get/set id of pokemon in party (in party object)
        //</summary>
        private (bool check, byte ret) GSPartyPokemonSpecies(PokemonPartySlot slot, ProcessingType processingType, Byte? newPokemonId) {
            ulong addr = (ulong)((ulong)Address.ADDR_PARTY_SPECIES + (byte)slot);

            bool check = false;
            byte id = 0xFF;
            if (processingType == ProcessingType.GET) {
                check = Connector.Read8(addr, out id);
            } else if (processingType == ProcessingType.SET && !(newPokemonId is null)) {
                check = Connector.Write8(addr, (byte)newPokemonId);
            }
            return (check, id);
        }

        private (bool check, byte ret) GSCurrentPokePartySize(ProcessingType processingType, Byte? newPartySize) {
            bool check = false;
            byte ret = 0xFF;
            if (processingType == ProcessingType.GET) {
                check = Connector.Read8((ulong)Address.ADDR_PARTY_SIZE, out ret);
            } else if (processingType == ProcessingType.SET && (!(newPartySize is null) && newPartySize > 0x00 && newPartySize <= 0x06)) {
                check = Connector.Write8((ulong)Address.ADDR_PARTY_SIZE, (byte)newPartySize);
            }
            return (check, ret);
        }

        private (bool check, byte[] ret) GSPartyPokemonOTName(PokemonPartySlot slot,  ProcessingType processingType) {
            const byte len = 0x0B;
            bool check = false;
            byte[] buff = new byte[len];

            ulong addr = (ulong)((ulong)Address.ADDR_PARTY_POKEMON_OT_NAME + (len * (ulong)slot));
            if (processingType == ProcessingType.GET) {
                check = Connector.Read(addr, buff);
            } else if (processingType == ProcessingType.SET) {
                check &= Connector.Read((ulong)Address.ADDR_OT_NAME, buff);//Read OT name from random addr
                check &= Connector.Write(addr, buff);//Write OT name back to poke
            }
            return (check, buff);
        }

        private (bool check, byte[] ret) GSPartyPokemonNickname(PokemonPartySlot slot, ProcessingType processingType, PartyPokemon? pokemon) {
            const byte len = 0x0B;
            ulong addr = (ulong)((ulong)Address.ADDR_PARTY_POKEMON_NICKNAME + (ulong)(len * (byte)slot));

            bool check = false;
            byte[] buff = new byte[len];
            if (processingType == ProcessingType.GET) {
                check = Connector.Read(addr, buff);
            } else if (processingType == ProcessingType.SET && !(pokemon is null)) {
                var first = _Pokemon.First(t => t.Value.id == pokemon.Value.species);
                check = Connector.Write(addr, buff = MapStringToPokeString(first.Value.name.ToUpper(), len));
            }
            return (check, buff);
        }

        private bool SetPartyPokemon(PokemonPartySlot slot, PartyPokemon partyPokemon) {
            var gPartySize = GSCurrentPokePartySize(ProcessingType.GET, null);
            var gPartyPokemonSpecies = GSPartyPokemonSpecies(slot, ProcessingType.GET, null);
            bool check = gPartySize.check && gPartyPokemonSpecies.check;
            if (check) {
                var gPartyPokeOTName = GSPartyPokemonOTName(slot, ProcessingType.GET);
                if (!gPartyPokeOTName.check) return false;
                if (gPartyPokeOTName.ret.All(t => t == 0x00)) {//No OT
                    check &= GSPartyPokemonOTName(slot, ProcessingType.SET).check;
                }

                check &= GSPartyPokemonNickname(slot, ProcessingType.SET, partyPokemon).check;//Set nickname
                check &= GSPartyPokemonSpecies(slot, ProcessingType.SET, partyPokemon.species).check;//Set species of party pokemon - idk why there are 2 different sources for this...
                if (gPartyPokemonSpecies.ret == 0x00 || gPartyPokemonSpecies.ret == 0xFF) {//There is no pokemon in the slot
                    check &= GSCurrentPokePartySize(ProcessingType.SET, (byte)(gPartySize.ret + 1)).check;//Increase size
                }

                //Manually fix endianness :facepalm:
                System.Reflection.MethodInfo swap = typeof(ConnectorExtensions).GetMethod("SwapEndianness", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                object fixedPP = swap.Invoke(null, new object[] {partyPokemon});

                ulong addr = (ulong)((ulong)Address.ADDR_PARTY_POKEMON + (ulong)(0x30 * (byte)slot));
                // check &= ConnectorExtensions.WriteObject(this.Connector, addr, partyPokemon);
                check &= ConnectorExtensions.WriteObject(this.Connector, addr, fixedPP);
                return check;
            }
            return false;
        }

        private (bool check, ushort ret) GTrainerID() {
            return (ConnectorExtensions.Read16LE(this.Connector, (ulong)Address.ADDR_TRAINER_ID, out ushort ret), ret);
        }

        private static PokemonPartySlot GetSlotFromByte(byte b) {
            switch(b) {
                default:
                case 0x01:
                    return PokemonPartySlot.SLOT_1;
                case 0x02:
                    return PokemonPartySlot.SLOT_2;
                case 0x03:
                    return PokemonPartySlot.SLOT_3;
                case 0x04:
                    return PokemonPartySlot.SLOT_4;
                case 0x05:
                    return PokemonPartySlot.SLOT_5;
                case 0x06:
                    return PokemonPartySlot.SLOT_6;
            }
        }

        private static string UnmapPokeStringToString(byte[] unmapped) {
            char[] buff = new char[unmapped.Length];
            for(var i = 0; i < unmapped.Length; ++i) {
                var found = _PokeTextEncoding.First(t => t.Value == unmapped[i]);
                buff[i] = found.Key;
            }
            return new String(buff);
        }

        private static byte[] MapStringToPokeString(string mapped, uint len) {
            byte[] buff = new byte[len];
            for(var i = 0; i < mapped.Length; ++i) {
                var found = _PokeTextEncoding.Where(t => t.Key.Equals(mapped[i]));
                if (found.Any()) {
                    buff[i] = found.First().Value;
                }
            }

            for(var i = mapped.Length; i < len; ++i) buff[i] = 0x50;
            return buff;
        }

        private enum ProcessingType {

            GET,

            SET

        }

        private enum PokemonPartySlot: byte {

            SLOT_1 = 0x00,

            SLOT_2 = 0x01,

            SLOT_3 = 0x02,

            SLOT_4 = 0x03,

            SLOT_5 = 0x04,

            SLOT_6 = 0x05

        }

        public enum Address : ulong {

            ADDR_TRAINER_ID = 0x3196,

            ADDR_PARTY_SIZE = 0xDCD7,

            ADDR_PARTY_SPECIES = 0xDCD8,

            ADDR_PARTY_POKEMON = 0xDCDF,

            ADDR_PARTY_POKEMON_OT_NAME = 0xDDFF,

            ADDR_PARTY_POKEMON_NICKNAME = 0xDE41,

            ADDR_OT_NAME = 0xF47D,

            ADDR_NEXT_WILD_POKEMON = 0xD204,

            ADDR_DAY_OF_WEEK = 0xD4B6

        }

        public enum PocketType {

            ITEMS,

            KEY_ITEMS,

            POKE_BALLS,

            TM_AND_HM,

            UNKNOWN

        }

        public enum MoveSlot {

            MOVE_1,

            MOVE_2,

            MOVE_3,

            MOVE_4

        }

        //<summary>
        //Struct for Pokemon in party - 0x30 bytes long
        //</summary>
        [StructLayout(LayoutKind.Explicit, Size=0x30)]
        public struct PartyPokemon {

            [NotNull, FieldOffset(0x0000)] public byte species;//ID of pokemon

            [NotNull, FieldOffset(0x0001)] public byte heldItem;//Item held by pokemon

            [NotNull, FieldOffset(0x0002)] public byte move1;

            [NotNull, FieldOffset(0x0003)] public byte move2;

            [NotNull, FieldOffset(0x0004)] public byte move3;

            [NotNull, FieldOffset(0x0005)] public byte move4;

            [NotNull, FieldOffset(0x0006)] public ushort otId;//Original trainer id

            [NotNull, FieldOffset(0x0008)] public uint exp;//0x03 bytes long

            [NotNull, FieldOffset(0x000B)] public ushort hpStatExp;

            [NotNull, FieldOffset(0x000D)] public ushort atkStatExp;

            [NotNull, FieldOffset(0x000F)] public ushort defStatExp;

            [NotNull, FieldOffset(0x0011)] public ushort spdStatExp;

            [NotNull, FieldOffset(0x0013)] public ushort speStatExp;

            [NotNull, FieldOffset(0x0015)] public ushort dv;

            [NotNull, FieldOffset(0x0017)] public byte ppMove1;

            [NotNull, FieldOffset(0x0018)] public byte ppMove2;

            [NotNull, FieldOffset(0x0019)] public byte ppMove3;

            [NotNull, FieldOffset(0x001A)] public byte ppMove4;
            
            [NotNull, FieldOffset(0x001B)] public byte happiness;

            [NotNull, FieldOffset(0x001C)] public byte pokerusStatus;//??????

            [NotNull, FieldOffset(0x001D)] public ushort caughtData;//???????

            [NotNull, FieldOffset(0x001F)] public byte level;

            [NotNull, FieldOffset(0x0020)] public byte status;

            [NotNull, FieldOffset(0x0021)] public byte unused;

            [NotNull, FieldOffset(0x0022)] public ushort hp;

            [NotNull, FieldOffset(0x0024)] public ushort hpMax;

            [NotNull, FieldOffset(0x0026)] public ushort statAtk;

            [NotNull, FieldOffset(0x0028)] public ushort statDef;

            [NotNull, FieldOffset(0x002A)] public ushort statSpd;

            [NotNull, FieldOffset(0x002C)] public ushort statSpAtk;

            [NotNull, FieldOffset(0x002E)] public ushort statSpDef;

            public PartyPokemon(byte species, byte heldItem, byte move1, byte move2, byte move3, byte move4, ushort otId, uint exp, ushort hpStatExp, ushort atkStatExp, ushort defStatExp, ushort spdStatExp, ushort speStatExp, ushort dv, byte ppMove1, byte ppMove2, byte ppMove3, byte ppMove4, byte happiness, byte pokerusStatus, ushort caughtData, byte level, byte status, ushort hp, ushort hpMax, ushort statAtk, ushort statDef, ushort statSpd, ushort statSpAtk, ushort statSpDef) {
                this.species = species;
                this.heldItem = heldItem;
                this.move1 = move1;
                this.move2 = move2;
                this.move3 = move3;
                this.move4 = move4;
                this.otId = otId;
                this.exp = exp;
                this.hpStatExp = hpStatExp;
                this.atkStatExp = atkStatExp;
                this.defStatExp = defStatExp;
                this.spdStatExp = spdStatExp;
                this.speStatExp = speStatExp;
                this.dv = dv;
                this.ppMove1 = ppMove1;
                this.ppMove2 = ppMove2;
                this.ppMove3 = ppMove3;
                this.ppMove4 = ppMove4;
                this.happiness = happiness;
                this.pokerusStatus = pokerusStatus;
                this.caughtData = caughtData;
                this.level = level;
                this.status = status;
                this.unused = 0x00;
                this.hp = hp;
                this.hpMax = hpMax;
                this.statAtk = statAtk;
                this.statDef = statDef;
                this.statSpd = statSpd;
                this.statSpAtk = statSpAtk;
                this.statSpDef = statSpDef;
            }

        }

    }
}
