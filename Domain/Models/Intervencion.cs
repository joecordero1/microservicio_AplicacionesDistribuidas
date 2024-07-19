using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice_VolunTrack.Domain.Models
{
    public class Intervencion
    {
        public string? Id_Animal { get; set; }
        public string? Id_Usuario { get; set; }
        public string? Nombre { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public List<DetalleIntervencion>? Detalles { get; set; }
        public int ResultadoAntes { get; set; }
        public int ResultadoDespues { get; set; }
        public string? Comentarios { get; set; }
        public double? EfectividadCalculada { get; set; }
    }
}
