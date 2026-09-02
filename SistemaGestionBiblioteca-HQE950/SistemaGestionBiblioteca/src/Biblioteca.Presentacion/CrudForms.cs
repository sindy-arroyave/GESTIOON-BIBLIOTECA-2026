using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    internal sealed class AutoresForm : ModuleFormBase
    {
        private readonly TextBox _codigo, _nombre, _apellidos, _nacionalidad;
        private readonly DateTimePicker _nacimiento;
        private readonly AutorServicio _service;

        public AutoresForm(bool demo) : base("Gestión de autores", demo, true)
        {
            _service = demo ? null : new AutorServicio();
            _codigo = TextField("Código *"); _nombre = TextField("Nombre *"); _apellidos = TextField("Apellidos *"); _nacionalidad = TextField("Nacionalidad"); _nacimiento = DateField("Fecha de nacimiento");
            Buttons(Save, Delete, Clear);
            Shown += delegate { LoadRows(); };
        }
        private void Buttons(Action save, Action delete, Action clear)
        {
            FlowLayoutPanel bar = ActionBar(); Button s = Ui.Button("Guardar", Ui.Teal), d = Ui.Button("Eliminar", Color.FromArgb(220, 70, 70)), n = Ui.Button("Nuevo", Ui.NavyLight);
            s.Click += delegate { Safe(save); }; d.Click += delegate { Safe(delete); }; n.Click += delegate { clear(); }; bar.Controls.Add(s); bar.Controls.Add(d); bar.Controls.Add(n);
        }
        private void LoadRows() { Grid.DataSource = DemoMode ? DemoData.Autores() : _service.Listar(); }
        private void Save() { if (DemoMode) return; _service.Guardar(SelectedId, _codigo.Text, _nombre.Text, _apellidos.Text, _nacionalidad.Text, _nacimiento.Value); LoadRows(); Clear(); }
        private void Delete() { if (DemoMode) return; _service.Eliminar(SelectedId); LoadRows(); Clear(); }
        private void Clear() { SelectedId = 0; _codigo.Clear(); _nombre.Clear(); _apellidos.Clear(); _nacionalidad.Clear(); _nacimiento.Value = DateTime.Today.AddYears(-25); }
        protected override void SelectRow() { SelectedId = CellInt("AutorId"); _codigo.Text = CellText("Codigo"); _nombre.Text = CellText("Nombre"); _apellidos.Text = CellText("Apellidos"); _nacionalidad.Text = CellText("Nacionalidad"); DateTime date; if (DateTime.TryParse(CellText("FechaNacimiento"), out date)) _nacimiento.Value = date; }
    }

    internal sealed class CategoriasForm : ModuleFormBase
    {
        private readonly TextBox _nombre, _descripcion;
        private readonly CategoriaServicio _service;
        public CategoriasForm(bool demo) : base("Gestión de categorías", demo, true)
        {
            _service = demo ? null : new CategoriaServicio(); _nombre = TextField("Nombre *"); _descripcion = TextField("Descripción");
            FlowLayoutPanel bar = ActionBar(); Button s = Ui.Button("Guardar", Ui.Teal), d = Ui.Button("Eliminar", Color.FromArgb(220, 70, 70)), n = Ui.Button("Nuevo", Ui.NavyLight); s.Click += delegate { Safe(Save); }; d.Click += delegate { Safe(Delete); }; n.Click += delegate { Clear(); }; bar.Controls.Add(s); bar.Controls.Add(d); bar.Controls.Add(n); Shown += delegate { LoadRows(); };
        }
        private void LoadRows() { Grid.DataSource = DemoMode ? DemoData.Categorias() : _service.Listar(); }
        private void Save() { if (DemoMode) return; _service.Guardar(SelectedId, _nombre.Text, _descripcion.Text); LoadRows(); Clear(); }
        private void Delete() { if (DemoMode) return; _service.Eliminar(SelectedId); LoadRows(); Clear(); }
        private void Clear() { SelectedId = 0; _nombre.Clear(); _descripcion.Clear(); }
        protected override void SelectRow() { SelectedId = CellInt("CategoriaId"); _nombre.Text = CellText("Nombre"); _descripcion.Text = CellText("Descripcion"); }
    }

    internal sealed class UsuariosForm : ModuleFormBase
    {
        private readonly TextBox _documento, _nombre, _apellidos, _telefono, _correo, _programa;
        private readonly UsuarioServicio _service;
        public UsuariosForm(bool demo) : base("Gestión de usuarios", demo, true)
        {
            _service = demo ? null : new UsuarioServicio(); _documento = TextField("Documento *"); _nombre = TextField("Nombre *"); _apellidos = TextField("Apellidos *"); _telefono = TextField("Teléfono *"); _correo = TextField("Correo electrónico *"); _programa = TextField("Programa académico *");
            FlowLayoutPanel bar = ActionBar(); Button s = Ui.Button("Guardar", Ui.Teal), d = Ui.Button("Eliminar", Color.FromArgb(220, 70, 70)), n = Ui.Button("Nuevo", Ui.NavyLight); s.Click += delegate { Safe(Save); }; d.Click += delegate { Safe(Delete); }; n.Click += delegate { Clear(); }; bar.Controls.Add(s); bar.Controls.Add(d); bar.Controls.Add(n); Shown += delegate { LoadRows(); };
        }
        private void LoadRows() { Grid.DataSource = DemoMode ? DemoData.Usuarios() : _service.Listar(); }
        private void Save() { if (DemoMode) return; _service.Guardar(SelectedId, _documento.Text, _nombre.Text, _apellidos.Text, _telefono.Text, _correo.Text, _programa.Text); LoadRows(); Clear(); }
        private void Delete() { if (DemoMode) return; _service.Eliminar(SelectedId); LoadRows(); Clear(); }
        private void Clear() { SelectedId = 0; _documento.Clear(); _nombre.Clear(); _apellidos.Clear(); _telefono.Clear(); _correo.Clear(); _programa.Clear(); }
        protected override void SelectRow() { SelectedId = CellInt("UsuarioId"); _documento.Text = CellText("Documento"); _nombre.Text = CellText("Nombre"); _apellidos.Text = CellText("Apellidos"); _telefono.Text = CellText("Telefono"); _correo.Text = CellText("Correo"); _programa.Text = CellText("ProgramaAcademico"); }
    }

    internal sealed class LibrosForm : ModuleFormBase
    {
        private readonly TextBox _codigo, _isbn, _titulo, _search;
        private readonly ComboBox _autor, _categoria;
        private readonly NumericUpDown _cantidad;
        private readonly LibroServicio _service;
        private readonly AutorServicio _authors;
        private readonly CategoriaServicio _categories;

        public LibrosForm(bool demo) : base("Gestión de libros", demo, true)
        {
            _service = demo ? null : new LibroServicio(); _authors = demo ? null : new AutorServicio(); _categories = demo ? null : new CategoriaServicio();
            _codigo = TextField("Código *"); _isbn = TextField("ISBN único *"); _titulo = TextField("Título *"); _autor = ComboField("Autor *"); _categoria = ComboField("Categoría *"); _cantidad = NumberField("Cantidad total *", 1, 10000);
            FlowLayoutPanel bar = ActionBar(); Button s = Ui.Button("Guardar", Ui.Teal), d = Ui.Button("Eliminar", Color.FromArgb(220, 70, 70)), n = Ui.Button("Nuevo", Ui.NavyLight); s.Click += delegate { Safe(Save); }; d.Click += delegate { Safe(Delete); }; n.Click += delegate { Clear(); }; bar.Controls.Add(s); bar.Controls.Add(d); bar.Controls.Add(n);
            Panel searchPanel = new Panel { Dock = DockStyle.Right, Width = 500, BackColor = Color.White, Padding = new Padding(8) }; _search = new TextBox { Width = 310, Font = new Font("Segoe UI", 10F), Text = "", Location = new Point(10, 21) }; Button search = Ui.Button("Buscar", Ui.NavyLight); search.Width = 90; search.Location = new Point(334, 16); searchPanel.Controls.Add(_search); searchPanel.Controls.Add(search); HeaderHost.Controls.Add(searchPanel); searchPanel.BringToFront(); search.Click += delegate { LoadRows(); }; _search.KeyDown += delegate(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) LoadRows(); };
            Shown += delegate { LoadCombos(); LoadRows(); };
        }
        private void LoadCombos()
        {
            DataTable authors = DemoMode ? DemoData.Autores() : _authors.Listar(); DataColumn full = new DataColumn("NombreCompleto", typeof(string), "Nombre + ' ' + Apellidos"); authors.Columns.Add(full); _autor.DataSource = authors; _autor.DisplayMember = "NombreCompleto"; _autor.ValueMember = "AutorId";
            _categoria.DataSource = DemoMode ? DemoData.Categorias() : _categories.Listar(); _categoria.DisplayMember = "Nombre"; _categoria.ValueMember = "CategoriaId";
        }
        private void LoadRows() { Grid.DataSource = DemoMode ? DemoData.Libros() : _service.Listar(_search.Text); }
        private void Save() { if (DemoMode) return; _service.Guardar(SelectedId, _codigo.Text, _isbn.Text, _titulo.Text, Convert.ToInt32(_autor.SelectedValue), Convert.ToInt32(_categoria.SelectedValue), Convert.ToInt32(_cantidad.Value)); LoadRows(); Clear(); }
        private void Delete() { if (DemoMode) return; _service.Eliminar(SelectedId); LoadRows(); Clear(); }
        private void Clear() { SelectedId = 0; _codigo.Clear(); _isbn.Clear(); _titulo.Clear(); _cantidad.Value = 1; if (_autor.Items.Count > 0) _autor.SelectedIndex = 0; if (_categoria.Items.Count > 0) _categoria.SelectedIndex = 0; }
        protected override void SelectRow() { SelectedId = CellInt("LibroId"); _codigo.Text = CellText("Codigo"); _isbn.Text = CellText("ISBN"); _titulo.Text = CellText("Titulo"); _autor.SelectedValue = CellInt("AutorId"); _categoria.SelectedValue = CellInt("CategoriaId"); decimal quantity; if (decimal.TryParse(CellText("CantidadTotal"), out quantity)) _cantidad.Value = quantity; }
    }
}
