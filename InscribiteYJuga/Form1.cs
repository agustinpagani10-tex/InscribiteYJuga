using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using InscribiteYJuga.Backend;

namespace InscribiteYJuga
{
    public partial class Form1 : Form
    {
        private readonly Jugadores jugadores;
        private Guid jugadorSeleccionado;

        public Form1()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            jugadores = new Jugadores();
            CargarCombos();
            ListarActivos();
        }

        private void CargarCombos()
        {
            cboPuesto.DataSource = Enum.GetValues(typeof(PuestoJugador));
            cboEstado.DataSource = Enum.GetValues(typeof(EstadoJugador));
        }

        private Jugador LeerFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtNombreEquipo.Text))
                throw new ArgumentException("Ingrese el nombre del equipo.");

            if (string.IsNullOrWhiteSpace(txtNombreJugador.Text))
                throw new ArgumentException("Ingrese el nombre del jugador.");

            int numero;
            if (!int.TryParse(txtNumeroCamiseta.Text.Trim(), out numero))
                throw new ArgumentException("Ingrese un número de camiseta válido.");
            return new Jugador { Id = jugadorSeleccionado == Guid.Empty ? Guid.NewGuid() : jugadorSeleccionado, NombreEquipo = txtNombreEquipo.Text.Trim(), NombreJugador = txtNombreJugador.Text.Trim(), Puesto = (PuestoJugador)cboPuesto.SelectedItem, Estado = (EstadoJugador)cboEstado.SelectedItem, NumeroCamiseta = numero, Lesionado = chkLesionado.Checked, Activo = true };
        }

        private void Mostrar(System.Collections.Generic.IEnumerable<Jugador> lista)
        {
            dgvJugadores.DataSource = null;
            dgvJugadores.DataSource = lista.ToList();
            if (dgvJugadores.Columns["Id"] != null)
                dgvJugadores.Columns["Id"].Visible = false;
        }

        private void ListarActivos() { Mostrar(jugadores.ListarActivos()); }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                Jugador jugador = LeerFormulario();

                jugadores.Agregar(jugador);

                MessageBox.Show(
                    "Jugador agregado correctamente.",
                    "Inscripción",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Limpiar();
                ListarActivos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                jugadores.Actualizar(LeerFormulario());
                MessageBox.Show("Jugador actualizado correctamente.", "Inscripción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                ListarActivos();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (jugadorSeleccionado == Guid.Empty)
            {
                MessageBox.Show("Debe seleccionar un jugador.", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar el jugador seleccionado?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                jugadores.Eliminar(jugadorSeleccionado);
                MessageBox.Show("Jugador eliminado correctamente.", "Inscripción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                ListarActivos();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            try
            {
                jugadores.Desactivar(jugadorSeleccionado);
                MessageBox.Show("Jugador desactivado correctamente.", "Inscripción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                ListarActivos();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Desactivar", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnBuscar_Click(object sender, EventArgs e) { Mostrar(jugadores.Buscar(txtBuscar.Text)); }
        private void btnListarTodos_Click(object sender, EventArgs e) { Mostrar(jugadores.ListarTodos()); }
        private void btnListarActivos_Click(object sender, EventArgs e) { ListarActivos(); }
        private void btnLimpiar_Click(object sender, EventArgs e) { Limpiar(); }

        private void Limpiar()
        {
            jugadorSeleccionado = Guid.Empty;
            txtNombreEquipo.Clear();
            txtNombreJugador.Clear();
            txtNumeroCamiseta.Clear();
            txtBuscar.Clear();
            chkLesionado.Checked = false;
            if (cboPuesto.Items.Count > 0) cboPuesto.SelectedIndex = 0;
            if (cboEstado.Items.Count > 0) cboEstado.SelectedIndex = 0;
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;
            btnDesactivar.Enabled = false;
            btnAgregar.Enabled = true;
        }

        private void dgvJugadores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvJugadores.Rows[e.RowIndex].DataBoundItem == null) return;
            var jugador = (Jugador)dgvJugadores.Rows[e.RowIndex].DataBoundItem;
            jugadorSeleccionado = jugador.Id;
            txtNombreEquipo.Text = jugador.NombreEquipo;
            txtNombreJugador.Text = jugador.NombreJugador;
            txtNumeroCamiseta.Text = jugador.NumeroCamiseta.ToString();
            cboPuesto.SelectedItem = jugador.Puesto;
            cboEstado.SelectedItem = jugador.Estado;
            chkLesionado.Checked = jugador.Lesionado;
            btnActualizar.Enabled = true;
            btnEliminar.Enabled = true;
            btnDesactivar.Enabled = jugador.Activo;
            btnAgregar.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
