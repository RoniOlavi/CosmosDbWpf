using Newtonsoft.Json;
using System;

namespace WpfNoSqlJasenRekisteriApp
{
    public class Jasen
    {
        [JsonProperty(PropertyName = "id")]
        public virtual string Id { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        public int PartitionKey { get; set; }
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Osoite { get; set; }
        public int Postinumero { get; set; }
        public string Puhelin { get; set; }
        public string Sahkoposti { get; set; }
        public DateTime Alkupvm { get; set; }

    }
}
