using System;
using System.Collections.Generic;
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
        public Linea(int id, string municipioOr, string municipioDest, DateTime horaInic, DateTime intervaloBus)
        {
            Id = id;
            MunicipioOr = municipioOr;
            MunicipioDest = municipioDest;
            HoraInic = horaInic;
            IntervaloBus = intervaloBus;
        }
        public int Id { get; set; }
        public string MunicipioOr { get; set; }
        public string MunicipioDest { get; set; }
        public DateTime HoraInic {  get; set; }
        public DateTime IntervaloBus { get; set; } 

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
    }
}
