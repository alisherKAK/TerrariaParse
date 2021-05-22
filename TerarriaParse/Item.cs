using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerarriaParse {
  public class Item {
    public string Name { get; set; }
    public Dictionary<string, string> Props { get; set; }
      = new Dictionary<string, string>();
  }
}
