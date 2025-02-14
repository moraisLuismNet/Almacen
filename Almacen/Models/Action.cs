using System;
using System.Collections.Generic;

namespace Almacen.Models
{
    public partial class Action
    {

        public int Id { get; set; }

        public DateTime FechaAccion { get; set; }

        public string Accion { get; set; } = null!;

        public string Controller { get; set; } = null!;

        public string Ip { get; set; } = null!;

    }
}
