using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerarriaParse {
  class Program {
    static void Main(string[] args) {
      string baseUrl = "https://terraria.fandom.com";
      string url = baseUrl + "/wiki/Terraria_Wiki";
      HtmlWeb doc = new HtmlWeb();
      HtmlDocument document = doc.Load(url);
      HtmlNode itemsNode = document.GetElementbyId("box-items");

      List<Node> nodes = new List<Node>();
      foreach(HtmlNode node in 
        itemsNode.Descendants("li")) {
        HtmlNode link = node.Descendants("a").FirstOrDefault();
        string href = link.Attributes["href"].Value;
        string title = link.Attributes["title"].Value;
        Node _node = new Node();
        _node.Link = href;
        _node.Name = title;

        nodes.Add(_node);
      }

      List<Item> items = new List<Item>();
      foreach(Node node in nodes) {
        HtmlDocument itemsDoc = doc.Load(baseUrl + node.Link);
        HtmlNode tableNode;
        try {
          tableNode = itemsDoc.DocumentNode.SelectNodes("//table[@class='terraria']").FirstOrDefault();
        }
        catch(ArgumentNullException ex) {
          continue;
        }
        HtmlNode key = tableNode.Descendants("tr").FirstOrDefault();
        int columnCount = key.Descendants("td").Count();
        List<string> keyNames = new List<string>();
        List<HtmlNode> desc = key.Descendants("th").ToList();
        if(desc.Count <= 0)
          desc = key.Descendants("td").ToList();

        foreach (HtmlNode thNode in desc) {
          keyNames.Add(thNode.InnerText);
        }

        List<HtmlNode> trNodes = tableNode.Descendants("tr").Skip(1).ToList();
        foreach(HtmlNode trNode in trNodes) {
          List<HtmlNode> tdNodes = 
            trNode.Descendants("td").ToList();
          int i = 0;
          Item item = new Item();
          item.Name = node.Name;
          foreach(HtmlNode tdNode in tdNodes) {
            string className;
            try {
              className = tdNode.Attributes["class"].Value;
            }
            catch(NullReferenceException ex) {
              className = "";
            }
            if (className == "il1c")
              continue;

            string text = 
              HtmlUtilities.ConvertToPlainText(tdNode.InnerText);

            item.Props.Add(keyNames[i], text);

            i++;
          }
          items.Add(item);
        }
      }

      using(StreamWriter writer = 
        new StreamWriter("items.json")) {
        string jsonText = JsonConvert.SerializeObject(items);
        writer.WriteLine(jsonText);
      }
    }
  }
}
