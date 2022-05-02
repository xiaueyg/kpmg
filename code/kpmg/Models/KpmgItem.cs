using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace KpmgApi.Models
{
    public class KpmgItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Rate> Data { get; set; }
    }

    public class Rate
    {
        public string Date { get; set; }
        public double Number { get; set; }

        [JsonIgnore]
        public string KpmgCode { get; set; }
        [JsonIgnore]
        public KpmgItem KpmgItem { get; set; }
    }
}