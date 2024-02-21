using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actividad2EV.Models
{
    internal class Paradas
    {
        public Paradas()
        {
        }

        public Paradas(int numLinea, string municipio, DateTime intervaloHS)
        {
            NumLinea = numLinea;
            Municipio = municipio;
            IntervaloHS = intervaloHS;
        }

        public int NumLinea { get; set; }
        public String Municipio { get; set; }
        public DateTime IntervaloHS { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Paradas paradas &&
                   NumLinea == paradas.NumLinea &&
                   Municipio == paradas.Municipio &&
                   IntervaloHS == paradas.IntervaloHS;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NumLinea, Municipio, IntervaloHS);
        }
    }

}
