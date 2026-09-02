using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    internal sealed class PrestamosForm : ModuleFormBase
    {
        private readonly ComboBox _usuario, _libro;
        private readonly DateTimePicker _fechaEsperada;
        private readonly PrestamoServicio _service;
        private readonly UsuarioServicio _users;
        private readonly LibroServicio _books;

        public PrestamosForm(bool demo) : base("Registro de préstamos", demo, true)
        {
            _service = demo ? null : new PrestamoServicio(); _users = demo ? null : new UsuarioServicio(); _books = demo ? null : new LibroServicio();
            Label help = Ui.Label("Seleccione un usuario y un libro disponible. El préstamo actualiza las existencias en una transacción.", 9F, FontStyle.Regular); help.MaximumSize = new Size(275, 0); help.ForeColor = Color.DimGray; Fields.Controls.Add(help);
            _usuario = ComboField("Usuario *"); _libro = ComboField("Libro disponible *"); _fechaEsperada = DateField("Devolución esperada *"); _fechaEsperada.Value = DateTime.Today.AddDays(15);
            FlowLayoutPanel bar = ActionBar(); Button register = Ui.Button("Registrar", Ui.Teal); register.Width = 130; register.Click += delegate { Safe(Register); }; bar.Controls.Add(register);
            Shown += delegate { LoadCombos(); LoadRows(); };
        }

        private void LoadCombos()
        {
            DataTable users = DemoMode ? DemoData.Usuarios() : _users.Listar(); users.Columns.Add(new DataColumn("NombreCompleto", typeof(string), "Documento + ' - ' + Nombre + ' ' + Apellidos")); _usuario.DataSource = users; _usuario.DisplayMember = "NombreCompleto"; _usuario.ValueMember = "UsuarioId";
            DataTable books = DemoMode ? DemoData.Libros() : _books.Listar(string.Empty); DataView available = new DataView(books); available.RowFilter = "Convert(CantidadDisponible, 'System.Int32') > 0"; DataTable filtered = available.ToTable(); filtered.Columns.Add(new DataColumn("LibroCompleto", typeof(string), "Codigo + ' - ' + Titulo")); _libro.DataSource = filtered; _libro.DisplayMember = "LibroCompleto"; _libro.ValueMember = "LibroId";
        }
        private void LoadRows() { Grid.DataSource = DemoMode ? DemoData.Prestamos() : _service.Activos(); }
        private void Register()
        {
            if (DemoMode) return;
            if (_usuario.SelectedValue == null || _libro.SelectedValue == null) throw new ValidacionException(new[] { "Debe seleccionar un usuario y un libro disponible." });
            int id = _service.Registrar(Convert.ToInt32(_usuario.SelectedValue), Convert.ToInt32(_libro.SelectedValue), _fechaEsperada.Value);
            MessageBox.Show("Préstamo " + id + " registrado correctamente.", "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadCombos(); LoadRows();
        }
    }

    internal sealed class DevolucionesForm : ModuleFormBase
    {
        private readonly PrestamoServicio _service;
        public DevolucionesForm(bool demo) : base("Registro de devoluciones", demo, true)
        {
            _service = demo ? null : new PrestamoServicio();
            Label help = Ui.Label("Seleccione un préstamo activo en la tabla. La devolución cambia su estado y restaura automáticamente la disponibilidad del libro.", 9.5F, FontStyle.Regular); help.MaximumSize = new Size(275, 0); help.ForeColor = Color.DimGray; Fields.Controls.Add(help);
            FlowLayoutPanel bar = ActionBar(); Button returnButton = Ui.Button("Registrar devolución", Ui.Teal); returnButton.Width = 190; returnButton.Click += delegate { Safe(ReturnLoan); }; bar.Controls.Add(returnButton);
            Shown += delegate { LoadRows(); };
        }
        private void LoadRows() { Grid.DataSource = DemoMode ? DemoData.Prestamos() : _service.Activos(); }
        private void ReturnLoan() { if (DemoMode) return; _service.Devolver(SelectedId); MessageBox.Show("Devolución registrada correctamente.", "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadRows(); SelectedId = 0; }
        protected override void SelectRow() { SelectedId = CellInt("PrestamoId"); }
    }

    internal sealed class ConsultasForm : ModuleFormBase
    {
        private readonly ComboBox _type;
        private readonly ConsultaServicio _service;
        public ConsultasForm(bool demo) : base("Consultas e indicadores", demo, false)
        {
            _service = demo ? null : new ConsultaServicio();
            Panel filters = new Panel { Dock = DockStyle.Right, Width = 520, BackColor = Color.White, Padding = new Padding(10) };
            _type = new ComboBox { Width = 300, Height = 30, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Location = new Point(10, 21) };
            _type.Items.AddRange(new object[] { "Libros disponibles", "Libros prestados", "Usuarios con préstamos", "Historial de préstamos", "Cantidad de libros por categoría", "Cantidad de libros prestados" }); _type.SelectedIndex = 4;
            Button run = Ui.Button("Consultar", Ui.Teal); run.Width = 110; run.Location = new Point(324, 16); run.Click += delegate { Safe(LoadRows); }; filters.Controls.Add(_type); filters.Controls.Add(run); HeaderHost.Controls.Add(filters); filters.BringToFront(); Shown += delegate { LoadRows(); };
        }
        private void LoadRows()
        {
            if (DemoMode) { Grid.DataSource = DemoData.Reporte(Convert.ToString(_type.SelectedItem)); return; }
            TipoConsulta type;
            switch (_type.SelectedIndex) { case 0: type = TipoConsulta.LibrosDisponibles; break; case 1: type = TipoConsulta.LibrosPrestados; break; case 2: type = TipoConsulta.UsuariosConPrestamos; break; case 3: type = TipoConsulta.HistorialPrestamos; break; case 4: type = TipoConsulta.LibrosPorCategoria; break; default: type = TipoConsulta.CantidadPrestados; break; }
            Grid.DataSource = _service.Ejecutar(type);
        }
    }
}
