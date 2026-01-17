using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Serialization;
using Otus.Task1.Models;

namespace Otus.Task1
{
    class Program
    {
        static SaveFile Generate1()
        {
            return new SaveFile
            {
                Coords = (1241.44, 124145.4),
                CurrentLocation = "Dungeon",
                User = new User { Level = 10, Name = "Пушкин", Gender = Gender.Male },
                Items = new[] { new Item { Name = "Топор", Quantity = 2 } },
                CreatedDate = DateTime.UtcNow,
                SaveDate = DateTime.UtcNow,
                FileName = "save1"
            };
        }

        static SaveFile Generate2()
        {
            return new SaveFile
            {
                Coords = (121.44, 124.4),
                CurrentLocation = "Subway",
                User = new User { Level = 10, Name = "Feodorov", Gender = Gender.Female },
                Items = new[] { new Item { Name = "Stick", Quantity = -2 } },
                CreatedDate = DateTime.UtcNow,
                SaveDate = null,
                FileName = "save2"
            };
        }

        static string OutDir()
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "out");
            Directory.CreateDirectory(dir);
            return dir;
        }

        static void SerializeBinary(SaveFile sf)
        {
            var path = Path.Combine(OutDir(), $"{sf.FileName}.bin");

            using var fs = File.Create(path);
            using var bw = new BinaryWriter(fs, Encoding.UTF8, leaveOpen: false);

            bw.Write(sf.CurrentLocation ?? "");
            bw.Write(sf.CreatedDate.ToBinary());
            bw.Write(sf.SaveDate.HasValue);
            if (sf.SaveDate.HasValue) bw.Write(sf.SaveDate.Value.ToBinary());
            bw.Write(sf.FileName ?? "");

            bw.Write(sf.User.Level);
            bw.Write(sf.User.Name ?? "");
            bw.Write(sf.User.GenderCode ?? "");

            bw.Write(sf.Items.Length);
            for (int i = 0; i < sf.Items.Length; i++)
            {
                bw.Write(sf.Items[i].Name ?? "");
                bw.Write(sf.Items[i].Quantity);
            }
        }

        static void SerializeJson(SaveFile sf)
        {
            var path = Path.Combine(OutDir(), $"{sf.FileName}.json");

            var opts = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(sf, opts);
            File.WriteAllText(path, json, new UTF8Encoding(false));
        }

        static void SerializeXml(SaveFile sf)
        {
            var path = Path.Combine(OutDir(), $"{sf.FileName}.xml");

            var ser = new XmlSerializer(typeof(SaveFile));
            using var sw = new StringWriterUtf8();
            ser.Serialize(sw, sf);
            File.WriteAllText(path, sw.ToString(), new UTF8Encoding(false));
        }

        static void Run(string name, Action action)
        {
            try
            {
                action();
                Console.WriteLine(name + ": OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(name + ": " + ex.GetType().Name + " - " + ex.Message);
            }
        }

        static void Main(string[] args)
        {
            var g1 = Generate1();

            Run("Binary g1", () => SerializeBinary(g1));
            Run("Json g1", () => SerializeJson(g1));
            Run("Xml g1", () => SerializeXml(g1));

            Run("Binary g2", () => SerializeBinary(Generate2()));
            Run("Json g2", () => SerializeJson(Generate2()));
            Run("Xml g2", () => SerializeXml(Generate2()));
        }

        sealed class StringWriterUtf8 : StringWriter
        {
            public override Encoding Encoding => new UTF8Encoding(false);
        }
    }
}
