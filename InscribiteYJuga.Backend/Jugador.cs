using System;

namespace InscribiteYJuga.Backend
{
    public enum EstadoJugador
    {
        Titular,
        Suplente
    }

    public enum PuestoJugador
    {
        Arquero,
        Defensor,
        Mediocampo,
        Delantero
    }

    public class Jugador
    {
        public Jugador()
        {
            Id = Guid.NewGuid();
            Activo = true;
        }

        public Guid Id { get; set; }
        public string NombreEquipo { get; set; }
        public string NombreJugador { get; set; }
        public PuestoJugador Puesto { get; set; }
        public int NumeroCamiseta { get; set; }
        public EstadoJugador Estado { get; set; }
        public bool Lesionado { get; set; }
        public bool Activo { get; set; }
    }
}
