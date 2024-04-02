using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicParser;

public class Card
{
    [JsonPropertyName("cmc")]
    public double? Cmc { get; set; }
    [JsonPropertyName("color_identity")]
    public List<string>? ColorIdentity { get; set; }
    [JsonPropertyName("colors")]
    public List<string>? Colors { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }
    [JsonPropertyName("layout")]
    public string? Layout { get; set; }
    [JsonPropertyName("legalities")]
    public Legalities? Legalities { get; set; }
    [JsonPropertyName("mana_cost")]
    public string? ManaCost { get; set; }
    []
    public List<string>? MultiverseIds { get; set; }
    public string? Name { get; set; }
    public string? Object { get; set; }
    public string? OracleId { get; set; }
    public string? OracleText { get; set; }
    public string? Power { get; set; }
    public bool? Reprint { get; set; }
    public string? Set { get; set; }
    public string? SetId { get; set; }
    public bool? Textless { get; set; }
    public string? Toughness { get; set; }
    public string? TypeLine { get; set; }
    
    public Card()
    {
    }

    public Card(JObject obj)
    {
        Cmc = (double)obj["cmc"];
        ColorIdentity = obj["colorIdentity"].ToObject<List<string>>();
        Colors = obj["colors"].ToObject<List<string>>();
        Id = (string)obj["id"];
        Keywords = obj["keywords"].ToObject<List<string>>();
        Lang = (string)obj["lang"];
        Layout = (string)obj["layout"];
        Legalities = obj["legalities"].ToObject<Legalities>();
        ManaCost = (string)obj["manaCost"];
        MultiverseIds = obj["multiverse_ids"].ToObject<List<string>>();
        Name = (string)obj["name"];
        Object = (string)obj["object"];
        OracleId = (string)obj["oracle_id"];
        OracleText = (string)obj["oracle_text"];
        Power = (string)obj["power"];
        Reprint = (bool)obj["reprint"];
        Set = (string)obj["set"];
        SetId = (string)obj["set_id"];
        Textless = (bool)obj["textless"];
        Toughness = (string)obj["toughness"];
        TypeLine = (string)obj["type_line"];
    }
}

public class FullCard
{
    public string? Object { get; set; }
    public string? Id { get; set; }
    public string? Oracle_id { get; set; }
    public List<int>? Multiverse_ids { get; set; }
    public int? Tcgplayer_id { get; set; }
    public int? Cardmarket_id { get; set; }
    public string? Name { get; set; }
    public string? Lang { get; set; }
    public string? Released_at { get; set; }
    public string? Uri { get; set; }
    public string? Scryfall_uri { get; set; }
    public string? Layout { get; set; }
    public bool? Highres_image { get; set; }
    public string? Image_status { get; set; }
    public ImageUris? Image_uris { get; set; }
    public string? Mana_cost { get; set; }
    public double? Cmc { get; set; }
    public string? Type_line { get; set; }
    public string? Oracle_text { get; set; }
    public string? Power { get; set; }
    public string? Toughness { get; set; }
    public List<string>? Colors { get; set; }
    public List<string>? ColorIdentity { get; set; }
    public List<string>? Keywords { get; set; }
    public Legalities? Legalities { get; set; }
    public List<string>? Games { get; set; }
    public bool? Reserved { get; set; }
    public bool? Foil { get; set; }
    public bool? Nonfoil { get; set; }
    public List<string>? Finishes { get; set; }
    public bool? Oversized { get; set; }
    public bool? Promo { get; set; }
    public bool? Reprint { get; set; }
    public bool? Variation { get; set; }
    public string? Set_id { get; set; }
    public string? Set { get; set; }
    public string? Set_name { get; set; }
    public string? Set_type { get; set; }
    public string? Set_uri { get; set; }
    public string? Set_search_uri { get; set; }
    public string? Scryfall_set_uri { get; set; }
    public string? Rulings_uri { get; set; }
    public string? Prints_search_uri { get; set; }
    public string? Collector_number { get; set; }
    public bool? Digital { get; set; }
    public string? Rarity { get; set; }
    public string? Card_back_id { get; set; }
    public string? Artist { get; set; }
    public List<string>? Artist_ids { get; set; }
    public string? Illustration_id { get; set; }
    public string? Border_color { get; set; }
    public string? Frame { get; set; }
    public List<string>? Frame_effects { get; set; }
    public string? Security_stamp { get; set; }
    public bool? Full_art { get; set; }
    public bool? Textless { get; set; }
    public bool? Booster { get; set; }
    public bool? Story_spotlight { get; set; }
    public int? Edhrec_rank { get; set; }
    public Prices? Prices { get; set; }
    public RelatedUris? Related_uris { get; set; }
    public PurchaseUris? Purchase_uris { get; set; }
    
    public FullCard()
    {
    }

    public FullCard(JObject obj)
    {
        Object = (string)obj["object"];
        Id = (string)obj["id"];
        Oracle_id = (string)obj["oracle_id"];
        Multiverse_ids = obj["multiverse_ids"].ToObject<List<int>>();
        Tcgplayer_id = (int)obj["tcgplayer_id"];
        Cardmarket_id = (int)obj["cardmarket_id"];
        Name = (string)obj["name"];
        Lang = (string)obj["lang"];
        Released_at = (string)obj["released_at"];
        Uri = (string)obj["uri"];
        Scryfall_uri = (string)obj["scryfall_uri"];
        Layout = (string)obj["layout"];
        Highres_image = (bool)obj["highres_image"];
        Image_status = (string)obj["image_status"];
        Image_uris = obj["image_uris"].ToObject<ImageUris>();
        Mana_cost = (string)obj["mana_cost"];
        Cmc = (double)obj["cmc"];
        Type_line = (string)obj["type_line"];
        Oracle_text = (string)obj["oracle_text"];
        Power = (string)obj["power"];
        Toughness = (string)obj["toughness"];
        Colors = obj["colors"].ToObject<List<string>>();
        ColorIdentity = obj["color_identity"].ToObject<List<string>>();
        Keywords = obj["keywords"].ToObject<List<string>>();
        Legalities = obj["legalities"].ToObject<Legalities>();
        Games = obj["games"].ToObject<List<string>>();
        Reserved = (bool)obj["reserved"];
        Foil = (bool)obj["foil"];
        Nonfoil = (bool)obj["nonfoil"];
        Finishes = obj["finishes"].ToObject<List<string>>();
        Oversized = (bool)obj["oversized"];
        Promo = (bool)obj["promo"];
        Reprint = (bool)obj["reprint"];
        Variation = (bool)obj["variation"];
        Set_id = (string)obj["set_id"];
        Set = (string)obj["set"];
        Set_name = (string)obj["set_name"];
        Set_type = (string)obj["set_type"];
        Set_uri = (string)obj["set_uri"];
        Set_search_uri = (string)obj["set_search_uri"];
        Scryfall_set_uri = (string)obj["scryfall_set_uri"];
        Rulings_uri = (string)obj["rulings_uri"];
        Prints_search_uri = (string)obj["prints_search_uri"];
        Collector_number = (string)obj["collector_number"];
        Digital = (bool)obj["digital"];
        Rarity = (string)obj["rarity"];
        Card_back_id = (string)obj["card_back_id"];
        Artist = (string)obj["artist"];
        Artist_ids = obj["artist_ids"].ToObject<List<string>>();
        Illustration_id = (string)obj["illustration_id"];
        Border_color = (string)obj["border_color"];
        Frame = (string)obj["frame"];
        Frame_effects = obj["frame_effects"].ToObject<List<string>>();
        Security_stamp = (string)obj["security_stamp"];
        Full_art = (bool)obj["full_art"];
        Textless = (bool)obj["textless"];
        Booster = (bool)obj["booster"];
        Story_spotlight = (bool)obj["story_spotlight"];
        Edhrec_rank = (int)obj["edhrec_rank"];
        Prices = obj["prices"].ToObject<Prices>();
        Related_uris = obj["related_uris"].ToObject<RelatedUris>();
        Purchase_uris = obj["purchase_uris"].ToObject<PurchaseUris>();
    }

    public Card ConvertFullCardToCard()
    {
        return new Card()
        {
            Cmc = Cmc,
            ColorIdentity = ColorIdentity,
            Colors = Colors,
            Id = Id,
            Keywords = Keywords,
            Lang = Lang,
            Layout = Layout,
            Legalities = Legalities,
            ManaCost = Mana_cost,
            MultiverseIds = Multiverse_ids.Select(x => x.ToString()).ToList(),
            Name = Name,
            Object = Object,
            OracleId = Oracle_id,
            OracleText = Oracle_text,
            Power = Power,
            Reprint = Reprint,
            Set = Set,
            SetId = Set_id,
            Textless = Textless,
            Toughness = Toughness,
            TypeLine = Type_line
        };
    }
}

public class ImageUris
{
    public string Small { get; set; }
    public string Normal { get; set; }
    public string Large { get; set; }
    public string Png { get; set; }
    public string Art_crop { get; set; }
    public string Border_crop { get; set; }

    public ImageUris()
    {
    }

    public ImageUris(JObject obj)
    {
        Small = (string)obj["small"];
        Normal = (string)obj["normal"];
        Large = (string)obj["large"];
        Png = (string)obj["png"];
        Art_crop = (string)obj["art_crop"];
        Border_crop = (string)obj["border_crop"];
    }
}

public class Legalities
{
    public string Standard { get; set; }
    public string Future { get; set; }
    public string Historic { get; set; }
    public string Timeless { get; set; }
    public string Gladiator { get; set; }
    public string Pioneer { get; set; }
    public string Explorer { get; set; }
    public string Modern { get; set; }
    public string Legacy { get; set; }
    public string Pauper { get; set; }
    public string Vintage { get; set; }
    public string Penny { get; set; }
    public string Commander { get; set; }
    public string Oathbreaker { get; set; }
    public string Standardbrawl { get; set; }
    public string Brawl { get; set; }
    public string Alchemy { get; set; }
    public string Paupercommander { get; set; }
    public string Duel { get; set; }
    public string Oldschool { get; set; }
    public string Premodern { get; set; }
    public string Predh { get; set; }

    public Legalities()
    {
    }

    public Legalities(JObject obj)
    {
        Standard = (string)obj["standard"];
        Future = (string)obj["future"];
        Historic = (string)obj["historic"];
        Timeless = (string)obj["timeless"];
        Gladiator = (string)obj["gladiator"];
        Pioneer = (string)obj["pioneer"];
        Explorer = (string)obj["explorer"];
        Modern = (string)obj["modern"];
        Legacy = (string)obj["legacy"];
        Pauper = (string)obj["pauper"];
        Vintage = (string)obj["vintage"];
        Penny = (string)obj["penny"];
        Commander = (string)obj["commander"];
        Oathbreaker = (string)obj["oathbreaker"];
        Standardbrawl = (string)obj["standardbrawl"];
        Brawl = (string)obj["brawl"];
        Alchemy = (string)obj["alchemy"];
        Paupercommander = (string)obj["paupercommander"];
        Duel = (string)obj["duel"];
        Oldschool = (string)obj["oldschool"];
        Premodern = (string)obj["premodern"];
        Predh = (string)obj["predh"];
    }
}

public class Prices
{
    public string Usd { get; set; }
    public string Usd_foil { get; set; }
    public string Usd_etched { get; set; }
    public string Eur { get; set; }
    public string Eur_foil { get; set; }
    public string Tix { get; set; }

    public Prices()
    {
    }

    public Prices(JObject obj)
    {
        Usd = (string)obj["usd"];
        Usd_foil = (string)obj["usd_foil"];
        Usd_etched = (string)obj["usd_etched"];
        Eur = (string)obj["eur"];
        Eur_foil = (string)obj["eur_foil"];
        Tix = (string)obj["tix"];
    }
}

public class RelatedUris
{
    public string? Tcgplayer_infinite_articles { get; set; }
    public string? Tcgplayer_infinite_decks { get; set; }
    public string? Edhrec { get; set; }

    public RelatedUris()
    {
    }

    public RelatedUris(JObject obj)
    {
        Tcgplayer_infinite_articles = (string)obj["tcgplayer_infinite_articles"];
        Tcgplayer_infinite_decks = (string)obj["tcgplayer_infinite_decks"];
        Edhrec = (string)obj["edhrec"];
    }
}

public class PurchaseUris
{
    public string? Tcgplayer { get; set; }
    public string? Cardmarket { get; set; }
    public string? Cardhoarder { get; set; }

    public PurchaseUris()
    {
    }

    public PurchaseUris(JObject obj)
    {
        Tcgplayer = (string)obj["tcgplayer"];
        Cardmarket = (string)obj["cardmarket"];
        Cardhoarder = (string)obj["cardhoarder"];
    }
}

