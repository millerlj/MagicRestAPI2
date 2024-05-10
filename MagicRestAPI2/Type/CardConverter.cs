using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MagicParser
{
    public class CardConverter
    {
        public MinimizedCard? Read(Card card)
        {
            if (card != null)
            {
                var minimizedCard = new MinimizedCard
                {
                    Cmc = card.Cmc,
                    Colors = card.Colors,
                    ColorIdentity = card.ColorIdentity,
                    Id = card.Id,
                    Keywords = card.Keywords,
                    Language = card.Language,
                    Layout = card.Layout,
                    Legalities = card.Legalities,
                    MultiverseIds = card.MultiverseIds,
                    OracleId = card.OracleId,
                    OracleText = card.OracleText,
                    Name = card.Name,
                    ManaCost = card.ManaCost,
                    TypeLine = card.TypeLine,
                    Power = ParsePower(card.Power),
                    Toughness = ParseToughness(card.Toughness),
                    Set = card.Set,
                    SetId = card.SetId,
                    Reprint = card.Reprint,
                    Textless = card.Textless,
                    Loyalty = card.Loyalty
                };

                return minimizedCard;
            }

            return null;
        }

        public void Write(Utf8JsonWriter writer, MinimizedCard value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private int ParsePower(string power)
        {
            if (int.TryParse(power, out int result))
            {
                return result;
            }

            return 0;
        }

        private int ParseToughness(string toughness)
        {
            if (int.TryParse(toughness, out int result))
            {
                return result;
            }

            return 0;
        }
    }
}
