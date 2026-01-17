using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Otus.Task1.Models
{
    public enum Gender
    {
        None = 0,
        Male = 1,
        Female = 2,
    }

    public sealed class Item
    {
        [XmlElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        private int _quantity;

        [XmlElement("quantity")]
        [JsonPropertyName("quantity")]
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(Quantity), "Quantity must be >= 0");
                _quantity = value;
            }
        }
    }

    public sealed class User
    {
        [XmlAttribute("level")]
        [JsonPropertyName("level")]
        public int Level { get; set; }

        [XmlElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [XmlIgnore]
        [JsonIgnore]
        public Gender Gender { get; set; } = Gender.None;

        [XmlElement("gender")]
        [JsonPropertyName("gender")]
        public string GenderCode
        {
            get => Gender switch
            {
                Gender.Male => "m",
                Gender.Female => "f",
                _ => ""
            };
            set => Gender = value switch
            {
                "m" => Gender.Male,
                "f" => Gender.Female,
                "" => Gender.None,
                _ => throw new FormatException("Gender must be 'm' or 'f'")
            };
        }
    }

    [Serializable]
    [XmlRoot("saveFile")]
    public class SaveFile
    {
        [XmlElement("currentLocation")]
        [JsonPropertyName("currentLocation")]
        public string CurrentLocation { get; set; } = "";

        [XmlElement("u")]
        [JsonPropertyName("u")]
        public User User { get; set; } = new User();

        [XmlArray("items")]
        [XmlArrayItem("item")]
        [JsonPropertyName("items")]
        public Item[] Items { get; set; } = Array.Empty<Item>();

        [XmlIgnore]
        [JsonIgnore]
        public (double, double) Coords { get; set; }

        [XmlElement("createdDate")]
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [XmlElement("saveDate")]
        [JsonPropertyName("saveDate")]
        public DateTime? SaveDate { get; set; }

        [XmlElement("fileName")]
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = "";
    }
}
