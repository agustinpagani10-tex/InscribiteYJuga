using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace InscribiteYJuga.Backend
{
    public class Jugadores
    {
        private const int MaximoJugadores = 15;
        private const int MaximoTitulares = 11;
        private const int MaximoSuplentes = 4;
        private readonly string archivo;
        private readonly List<Jugador> lista;

        public Jugadores()
            : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InscribiteYJuga", "jugadores.xml"))
        {
        }

        public Jugadores(string archivo)
        {
            this.archivo = archivo;
            lista = Cargar();
        }

        public List<Jugador> ListarTodos()
        {
            return lista.OrderBy(j => j.NumeroCamiseta).ToList();
        }

        public List<Jugador> ListarActivos()
        {
            return lista.Where(j => j.Activo).OrderBy(j => j.NumeroCamiseta).ToList();
        }

        public List<Jugador> Buscar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return ListarTodos();

            texto = texto.Trim();
            return lista.Where(j => j.NombreJugador.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                || j.NombreEquipo.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                || j.NumeroCamiseta.ToString() == texto)
                .OrderBy(j => j.NumeroCamiseta).ToList();
        }

        public void Agregar(Jugador jugador)
        {
            Validar(jugador, null);
            lista.Add(jugador);
            Guardar();
        }

        public void Actualizar(Jugador jugador)
        {
            if (jugador == null)
                throw new ArgumentException("Debe seleccionar un jugador.");

            var existente = lista.FirstOrDefault(j => j.Id == jugador.Id);
            if (existente == null)
                throw new ArgumentException("El jugador no existe.");

            Validar(jugador, jugador.Id);
            existente.NombreEquipo = jugador.NombreEquipo;
            existente.NombreJugador = jugador.NombreJugador;
            existente.Puesto = jugador.Puesto;
            existente.NumeroCamiseta = jugador.NumeroCamiseta;
            existente.Estado = jugador.Estado;
            existente.Lesionado = jugador.Lesionado;
            existente.Activo = jugador.Activo;
            Guardar();
        }

        public void Eliminar(Guid id)
        {
            var jugador = lista.FirstOrDefault(j => j.Id == id);
            if (jugador == null)
                throw new ArgumentException("Debe seleccionar un jugador.");

            lista.Remove(jugador);
            Guardar();
        }

        public void Desactivar(Guid id)
        {
            var jugador = lista.FirstOrDefault(j => j.Id == id);
            if (jugador == null)
                throw new ArgumentException("Debe seleccionar un jugador.");

            jugador.Activo = false;
            Guardar();
        }

        private void Validar(Jugador jugador, Guid? idActual)
        {
            if (jugador == null || string.IsNullOrWhiteSpace(jugador.NombreEquipo) || string.IsNullOrWhiteSpace(jugador.NombreJugador))
                throw new ArgumentException("El equipo y el nombre del jugador son obligatorios.");
            if (jugador.NumeroCamiseta < 1 || jugador.NumeroCamiseta > 99)
                throw new ArgumentException("El número de camiseta debe estar entre 1 y 99.");
            if (lista.Count(j => !idActual.HasValue || j.Id != idActual.Value) >= MaximoJugadores && !idActual.HasValue)
                throw new ArgumentException("El equipo no puede superar los 15 jugadores.");
            if (lista.Any(j => j.NumeroCamiseta == jugador.NumeroCamiseta && (!idActual.HasValue || j.Id != idActual.Value)))
                throw new ArgumentException("El número de camiseta ya está utilizado.");

            var equipo = lista.Where(j => !idActual.HasValue || j.Id != idActual.Value).ToList();
            var titulares = equipo.Count(j => j.Estado == EstadoJugador.Titular);
            var suplentes = equipo.Count(j => j.Estado == EstadoJugador.Suplente);
            if (jugador.Estado == EstadoJugador.Titular && titulares >= MaximoTitulares)
                throw new ArgumentException("El equipo ya tiene 11 titulares.");
            if (jugador.Estado == EstadoJugador.Suplente && suplentes >= MaximoSuplentes)
                throw new ArgumentException("El equipo ya tiene 4 suplentes.");
        }

        private List<Jugador> Cargar()
        {
            try
            {
                if (!File.Exists(archivo))
                    return new List<Jugador>();
                using (var stream = File.OpenRead(archivo))
                    return (List<Jugador>)new XmlSerializer(typeof(List<Jugador>)).Deserialize(stream);
            }
            catch (InvalidOperationException)
            {
                return new List<Jugador>();
            }
        }

        private void Guardar()
        {
            var carpeta = Path.GetDirectoryName(archivo);
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);
            using (var stream = File.Create(archivo))
                new XmlSerializer(typeof(List<Jugador>)).Serialize(stream, lista);
        }
    }
}
