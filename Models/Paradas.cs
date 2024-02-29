using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Paradas(int numLinea, string municipio, string intervaloHS)
        {
            NumLinea = numLinea;
            Municipio = municipio;
            IntervaloHS = intervaloHS;
        }

        [DisplayName("Número de Linea")]
        public int NumLinea { get; set; }
        [DisplayName("Municipio")]
        public string Municipio { get; set; }
        [DisplayName("Intervalos desde la hora de salida")]
        public string IntervaloHS { get; set; }

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

        public override string ToString()
        {
            return $"{nameof(NumLinea)}: {NumLinea}, {nameof(Municipio)}: {Municipio}, {nameof(IntervaloHS)}: {IntervaloHS}";
        }
       
    }

}
