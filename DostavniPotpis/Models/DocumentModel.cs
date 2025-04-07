using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Models
{
    public class DocumentModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int Oznvd { get; set; }
        [Indexed]
        public int Godina { get; set; }
        [Indexed]
        public int Brdok { get; set; }
        public int OznStDok { get; set; }
        public string Document { get; set; }
        public string Kupac { get; set; }
        public string KupacDio { get; set; }
        public string Adresa { get; set; }
        public byte[] ImageData { get; set; }
        public string Potpisao { get; set; }
        public string Backgroundcolor { get; set; }
        public string FullName => $"{Kupac} {KupacDio}";
        public string Izvrsilac { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Preneseno { get; set; }
    }
}
