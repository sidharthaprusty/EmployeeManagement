using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeManagement.Models
{
    public class MyTabs
    {
        [JsonProperty("Tabs")]
        public Tab Tabs { get; set; }
    }

    public class Tab
    {
        [JsonProperty("Tab")]
        public Section[] sections { get; set; }

    }

    public class Section
    {
        public string name { get; set; }
        [JsonProperty("Widget")]
        public WidgetItem[] Widget { get; set; }
    }

    public class WidgetItem
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string LinkedPin { get; set; }
    }
}