using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    internal static class Ui
    {
        public static readonly Color Navy = Color.FromArgb(18, 32, 47);
        public static readonly Color NavyLight = Color.FromArgb(28, 48, 68);
        public static readonly Color Teal = Color.FromArgb(0, 168, 150);
        public static readonly Color Surface = Color.FromArgb(244, 247, 250);
        public static readonly Color Text = Color.FromArgb(34, 47, 62);

        public static Label Label(string text, float size, FontStyle style)
        {
            return new Label { Text = text, AutoSize = true, Font = new Font("Segoe UI", size, style), ForeColor = Text, Margin = new Padding(4, 5, 4, 2) };
        }

        public static Button Button(string text, Color color)
        {
            Button b = new Button { Text = text, Height = 38, Width = 82, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand, Margin = new Padding(4) };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = NavyLight;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersHeight = 38;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 238, 233);
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.RowTemplate.Height = 32;
            grid.DataBindingComplete += delegate
            {
                foreach (DataGridViewColumn column in grid.Columns)
                {
                    if (column.Name == "AutorId" || column.Name == "CategoriaId" || column.Name == "UsuarioId" || column.Name == "LibroId") column.Visible = false;
                    else if (column.Name == "PrestamoId") column.HeaderText = "N.º préstamo";
                    else if (column.Name == "FechaPrestamo") column.HeaderText = "Fecha préstamo";
                    else if (column.Name == "FechaDevolucionEsperada") column.HeaderText = "Devolución esperada";
                    else if (column.Name == "FechaDevolucionReal") column.HeaderText = "Devolución real";
                    else if (column.Name == "CantidadTotal") column.HeaderText = "Total";
                    else if (column.Name == "CantidadDisponible") column.HeaderText = "Disponibles";
                    else if (column.Name == "ProgramaAcademico") column.HeaderText = "Programa académico";
                }
            };
        }

        public static void Error(Exception ex)
        {
            string message;
            ValidacionException validation = ex as ValidacionException;
            SqlException sql = ex as SqlException;
            if (validation != null) message = validation.Message;
            else if (sql != null && (sql.Number == 2601 || sql.Number == 2627)) message = "El registro está duplicado. Verifique códigos, ISBN, documento o correo.";
            else if (sql != null && sql.Number == 547) message = "No se puede completar la operación porque el registro está relacionado con otros datos.";
            else message = "No fue posible completar la operación. Detalle: " + ex.Message;
            MessageBox.Show(message, "Biblioteca - Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static DataTable Table(params string[] columns)
        {
            DataTable table = new DataTable();
            foreach (string column in columns) table.Columns.Add(column);
            return table;
        }
    }

    internal static class DemoData
    {
        public static DataTable Autores()
        {
            DataTable t = Ui.Table("AutorId", "Codigo", "Nombre", "Apellidos", "Nacionalidad", "FechaNacimiento");
            t.Rows.Add("1", "AUT-001", "Robert", "Martin", "Estadounidense", "1952-12-05");
            t.Rows.Add("2", "AUT-002", "Andrew", "Tanenbaum", "Estadounidense", "1944-03-16");
            t.Rows.Add("3", "AUT-003", "Ian", "Sommerville", "Británico", "1951-02-23");
            return t;
        }

        public static DataTable Categorias()
        {
            DataTable t = Ui.Table("CategoriaId", "Nombre", "Descripcion");
            t.Rows.Add("1", "Programación", "Lenguajes, algoritmos y desarrollo de software");
            t.Rows.Add("2", "Bases de datos", "Modelado, SQL y administración de datos");
            t.Rows.Add("3", "Redes", "Comunicación y redes de computadores");
            t.Rows.Add("4", "Inteligencia Artificial", "Aprendizaje automático y sistemas inteligentes");
            return t;
        }

        public static DataTable Usuarios()
        {
            DataTable t = Ui.Table("UsuarioId", "Documento", "Nombre", "Apellidos", "Telefono", "Correo", "ProgramaAcademico");
            t.Rows.Add("1", "1032456789", "Laura", "Gómez", "3005551101", "laura.gomez@institucion.edu.co", "Ingeniería de Sistemas");
            t.Rows.Add("2", "1019988776", "Carlos", "Ramírez", "3155552410", "carlos.ramirez@institucion.edu.co", "Ingeniería Electrónica");
            t.Rows.Add("3", "1001234567", "Valentina", "Torres", "3205558741", "valentina.torres@institucion.edu.co", "Matemáticas");
            return t;
        }

        public static DataTable Libros()
        {
            DataTable t = Ui.Table("LibroId", "Codigo", "ISBN", "Titulo", "AutorId", "Autor", "CategoriaId", "Categoria", "CantidadTotal", "CantidadDisponible", "Disponibilidad");
            t.Rows.Add("1", "LIB-001", "9780132350884", "Código limpio", "1", "Robert Martin", "1", "Programación", "5", "4", "Disponible");
            t.Rows.Add("2", "LIB-002", "9780132126953", "Redes de computadoras", "2", "Andrew Tanenbaum", "3", "Redes", "3", "3", "Disponible");
            t.Rows.Add("3", "LIB-003", "9780137035151", "Ingeniería de software", "3", "Ian Sommerville", "1", "Programación", "4", "3", "Disponible");
            t.Rows.Add("4", "LIB-004", "9780133970777", "Fundamentos de bases de datos", "2", "Andrew Tanenbaum", "2", "Bases de datos", "2", "0", "Prestado");
            return t;
        }

        public static DataTable Prestamos()
        {
            DataTable t = Ui.Table("PrestamoId", "Documento", "Usuario", "Codigo", "Titulo", "FechaPrestamo", "FechaDevolucionEsperada", "Estado");
            t.Rows.Add("1001", "1032456789", "Laura Gómez", "LIB-001", "Código limpio", DateTime.Today.AddDays(-3).ToShortDateString(), DateTime.Today.AddDays(7).ToShortDateString(), "ACTIVO");
            t.Rows.Add("1002", "1001234567", "Valentina Torres", "LIB-003", "Ingeniería de software", DateTime.Today.AddDays(-10).ToShortDateString(), DateTime.Today.AddDays(4).ToShortDateString(), "ACTIVO");
            return t;
        }

        public static DataTable Reporte(string name)
        {
            if (name == "Cantidad de libros por categoría")
            {
                DataTable c = Ui.Table("Categoria", "Titulos", "Ejemplares"); c.Rows.Add("Programación", "2", "9"); c.Rows.Add("Bases de datos", "1", "2"); c.Rows.Add("Redes", "1", "3"); c.Rows.Add("Inteligencia Artificial", "0", "0"); return c;
            }
            if (name == "Usuarios con préstamos")
            {
                DataTable u = Ui.Table("Documento", "Usuario", "Correo", "ProgramaAcademico"); u.Rows.Add("1032456789", "Laura Gómez", "laura.gomez@institucion.edu.co", "Ingeniería de Sistemas"); u.Rows.Add("1001234567", "Valentina Torres", "valentina.torres@institucion.edu.co", "Matemáticas"); return u;
            }
            return Prestamos();
        }
    }

    internal abstract class ModuleFormBase : Form
    {
        protected readonly bool DemoMode;
        protected readonly DataGridView Grid;
        protected readonly Panel GridHost;
        protected readonly Panel HeaderHost;
        protected readonly FlowLayoutPanel Fields;
        protected readonly Panel Editor;
        protected int SelectedId;

        protected ModuleFormBase(string title, bool demoMode, bool hasEditor)
        {
            DemoMode = demoMode;
            Text = "Biblioteca - " + title;
            Size = new Size(1120, 680);
            MinimumSize = new Size(900, 560);
            BackColor = Ui.Surface;
            Font = new Font("Segoe UI", 9F);
            StartPosition = FormStartPosition.CenterScreen;

            HeaderHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(24, 16, 24, 10) };
            Label heading = Ui.Label(title, 20F, FontStyle.Bold); heading.ForeColor = Ui.Navy; HeaderHost.Controls.Add(heading);

            Editor = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(18) };
            Fields = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
            Editor.Controls.Add(Fields);

            GridHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 18, 18, 24) };
            Grid = new DataGridView { Dock = DockStyle.Fill };
            Ui.StyleGrid(Grid);
            Grid.CellClick += delegate { SelectRow(); };
            GridHost.Controls.Add(Grid);

            TableLayoutPanel body = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1, ColumnCount = 2, Margin = Padding.Empty, Padding = Padding.Empty };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, hasEditor ? 330F : 0F));
            body.Controls.Add(GridHost, 0, 0); body.Controls.Add(Editor, 1, 0);

            TableLayoutPanel root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Margin = Padding.Empty, Padding = Padding.Empty };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.Controls.Add(HeaderHost, 0, 0); root.Controls.Add(body, 0, 1); Controls.Add(root);
        }

        protected TextBox TextField(string label)
        {
            Panel p = FieldPanel(label); TextBox t = new TextBox { Width = 275, Height = 27, Location = new Point(0, 25), Font = new Font("Segoe UI", 10F) }; p.Controls.Add(t); Fields.Controls.Add(p); return t;
        }
        protected ComboBox ComboField(string label)
        {
            Panel p = FieldPanel(label); ComboBox c = new ComboBox { Width = 275, Height = 29, Location = new Point(0, 24), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) }; p.Controls.Add(c); Fields.Controls.Add(p); return c;
        }
        protected DateTimePicker DateField(string label)
        {
            Panel p = FieldPanel(label); DateTimePicker d = new DateTimePicker { Width = 275, Location = new Point(0, 24), Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10F) }; p.Controls.Add(d); Fields.Controls.Add(p); return d;
        }
        protected NumericUpDown NumberField(string label, int minimum, int maximum)
        {
            Panel p = FieldPanel(label); NumericUpDown n = new NumericUpDown { Width = 275, Location = new Point(0, 24), Minimum = minimum, Maximum = maximum, Value = minimum, Font = new Font("Segoe UI", 10F) }; p.Controls.Add(n); Fields.Controls.Add(p); return n;
        }
        private Panel FieldPanel(string label)
        {
            Panel p = new Panel { Width = 282, Height = 58, Margin = new Padding(0, 0, 0, 6) }; Label l = Ui.Label(label, 8.5F, FontStyle.Bold); l.Location = new Point(0, 0); p.Controls.Add(l); return p;
        }
        protected FlowLayoutPanel ActionBar()
        {
            FlowLayoutPanel bar = new FlowLayoutPanel { Width = 280, Height = 48, WrapContents = false, Margin = new Padding(0, 10, 0, 0) }; Fields.Controls.Add(bar); return bar;
        }
        protected void Safe(Action action)
        {
            try { action(); }
            catch (Exception ex) { Ui.Error(ex); }
        }
        protected virtual void SelectRow() { }
        protected object Cell(string name)
        {
            return Grid.CurrentRow == null || !Grid.Columns.Contains(name) ? null : Grid.CurrentRow.Cells[name].Value;
        }
        protected int CellInt(string name) { object value = Cell(name); return value == null ? 0 : Convert.ToInt32(value); }
        protected string CellText(string name) { object value = Cell(name); return value == null ? string.Empty : Convert.ToString(value); }
    }
}
