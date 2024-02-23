using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actividad2EV.Models
{
    internal class Linea
    {
        public Linea()
        {
        }
        public Linea(int id, string municipioOr, string municipioDest, string horaInic, string intervaloBus)
        {
            Id = id;
            MunicipioOr = municipioOr;
            MunicipioDest = municipioDest;
            HoraInic = horaInic;
            IntervaloBus = intervaloBus;
        }
        [DisplayName("Núm de Linea")]
        public int Id { get; set; }
        [DisplayName("Municipio Origen")]
        public string MunicipioOr { get; set; }
        [DisplayName("Municipio Destino")]
        public string MunicipioDest { get; set; }
        [DisplayName("Hora inicial de salida")]
        public string HoraInic {  get; set; }
        [DisplayName("Intervalo buses")]
        public string IntervaloBus { get; set; } 

        public override bool Equals(object? obj)
        {
            return obj is Linea linea &&
                   Id == linea.Id &&
                   MunicipioOr == linea.MunicipioOr &&
                   MunicipioDest == linea.MunicipioDest &&
                   HoraInic == linea.HoraInic &&
                   IntervaloBus == linea.IntervaloBus;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, MunicipioOr, MunicipioDest, HoraInic, IntervaloBus);
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(MunicipioOr)}: {MunicipioOr}, {nameof(MunicipioDest)}: {MunicipioDest}, {nameof(HoraInic)}: {HoraInic}, {nameof(IntervaloBus)}: {IntervaloBus}";
        }

    }
}
