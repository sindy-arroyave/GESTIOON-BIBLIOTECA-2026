using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    internal sealed class MainForm : Form
    {
        private readonly bool _demo;
        private readonly Panel _content;
        private readonly Label _viewTitle;

        public MainForm(bool demo)
        {
            _demo = demo;
            Text = "Sistema de Gestión de Biblioteca";
            Size = new Size(1280, 760);
            MinimumSize = new Size(1050, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Ui.Surface;
            Font = new Font("Segoe UI", 9F);

            Panel sidebar = new Panel { Dock = DockStyle.Fill, BackColor = Ui.Navy };
            Label brand = new Label { Text = "BIBLIOTECA\nACADÉMICA", ForeColor = Color.White, Font = new Font("Segoe UI", 16F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Top, Height = 100, Padding = new Padding(24, 18, 0, 0) };
            FlowLayoutPanel navigation = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(0, 8, 0, 0), BackColor = Ui.Navy };
            string[] labels = { "Inicio", "Libros", "Autores", "Categorías", "Usuarios", "Préstamos", "Devoluciones", "Consultas" };
            foreach (string label in labels)
            {
                Button button = new Button { Text = "  " + label, Tag = label, Width = 230, Height = 52, Margin = Padding.Empty, FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, BackColor = Ui.Navy, ForeColor = Color.White, Font = new Font("Segoe UI", 10F), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(20, 0, 0, 0), Cursor = Cursors.Hand };
                button.Click += Navigate;
                navigation.Controls.Add(button);
            }
            sidebar.Controls.Add(navigation); sidebar.Controls.Add(brand);

            Panel top = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(26, 18, 20, 0) };
            _viewTitle = Ui.Label("Resumen general", 18F, FontStyle.Bold); _viewTitle.ForeColor = Ui.Navy; top.Controls.Add(_viewTitle);
            Label status = Ui.Label("SQL Server | Arquitectura por capas", 9F, FontStyle.Regular); status.ForeColor = Color.Gray; status.Location = new Point(710, 26); top.Controls.Add(status);

            _content = new Panel { Dock = DockStyle.Fill, BackColor = Ui.Surface, Padding = new Padding(26) };
            TableLayoutPanel mainArea = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Margin = Padding.Empty, Padding = Padding.Empty };
            mainArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F)); mainArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); mainArea.Controls.Add(top, 0, 0); mainArea.Controls.Add(_content, 0, 1);
            TableLayoutPanel shell = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1, ColumnCount = 2, Margin = Padding.Empty, Padding = Padding.Empty };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230F)); shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); shell.Controls.Add(sidebar, 0, 0); shell.Controls.Add(mainArea, 1, 0); Controls.Add(shell);
            Shown += delegate { ShowDashboard(); };
        }

        private void Navigate(object sender, EventArgs e)
        {
            string name = Convert.ToString(((Button)sender).Tag);
            if (name == "Inicio") { ShowDashboard(); return; }
            Form module = name == "Libros" ? (Form)new LibrosForm(_demo) : name == "Autores" ? new AutoresForm(_demo) : name == "Categorías" ? new CategoriasForm(_demo) : name == "Usuarios" ? new UsuariosForm(_demo) : name == "Préstamos" ? new PrestamosForm(_demo) : name == "Devoluciones" ? new DevolucionesForm(_demo) : (Form)new ConsultasForm(_demo);
            ShowModule(name, module);
        }

        private void ShowModule(string title, Form module)
        {
            foreach (Control control in _content.Controls) control.Dispose(); _content.Controls.Clear();
            _viewTitle.Text = title; module.TopLevel = false; module.FormBorderStyle = FormBorderStyle.None; module.Dock = DockStyle.Fill; _content.Padding = new Padding(0); _content.Controls.Add(module); module.Show();
        }

        private void ShowDashboard()
        {
            foreach (Control control in _content.Controls) control.Dispose(); _content.Controls.Clear(); _content.Padding = new Padding(26); _viewTitle.Text = "Resumen general";
            TableLayoutPanel cards = new TableLayoutPanel { Dock = DockStyle.Top, Height = 145, ColumnCount = 4, RowCount = 1, BackColor = Ui.Surface }; cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            string[] values = { "4", "10", "3", "2" };
            if (!_demo)
            {
                try { DataTable t = new ConsultaServicio().Resumen(); if (t.Rows.Count > 0) values = new[] { Convert.ToString(t.Rows[0]["Titulos"]), Convert.ToString(t.Rows[0]["Disponibles"]), Convert.ToString(t.Rows[0]["Usuarios"]), Convert.ToString(t.Rows[0]["PrestamosActivos"]) }; }
                catch (Exception ex) { Ui.Error(ex); }
            }
            string[] names = { "Títulos registrados", "Ejemplares disponibles", "Usuarios", "Préstamos activos" };
            Color[] colors = { Ui.Teal, Color.FromArgb(43, 121, 181), Color.FromArgb(138, 99, 210), Color.FromArgb(239, 142, 53) };
            for (int i = 0; i < 4; i++) cards.Controls.Add(Card(names[i], values[i], colors[i]), i, 0);
            _content.Controls.Add(cards);

            Panel welcome = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(34), Margin = new Padding(0, 18, 0, 0) };
            Label heading = Ui.Label("Administración centralizada de la biblioteca", 18F, FontStyle.Bold); heading.ForeColor = Ui.Navy; heading.Location = new Point(34, 40); welcome.Controls.Add(heading);
            Label text = Ui.Label("Gestione el catálogo, los usuarios, los préstamos y las devoluciones desde un solo lugar.\nLas operaciones críticas validan existencias y se ejecutan de forma transaccional.", 11F, FontStyle.Regular); text.ForeColor = Color.DimGray; text.Location = new Point(36, 86); text.MaximumSize = new Size(750, 0); welcome.Controls.Add(text);
            Panel note = new Panel { BackColor = Color.FromArgb(232, 247, 244), Location = new Point(36, 165), Size = new Size(760, 105), Padding = new Padding(22) }; Label noteText = Ui.Label("Estado del sistema\nMódulos disponibles: libros, autores, categorías, usuarios, préstamos, devoluciones y seis consultas operativas.", 10F, FontStyle.Regular); noteText.ForeColor = Ui.Text; note.Controls.Add(noteText); welcome.Controls.Add(note); _content.Controls.Add(welcome); welcome.BringToFront();
        }

        private Panel Card(string name, string value, Color accent)
        {
            Panel card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0, 0, 16, 16), Padding = new Padding(20) }; Panel stripe = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent }; card.Controls.Add(stripe); Label number = Ui.Label(value, 24F, FontStyle.Bold); number.ForeColor = Ui.Navy; number.Location = new Point(24, 22); card.Controls.Add(number); Label label = Ui.Label(name, 9F, FontStyle.Regular); label.ForeColor = Color.DimGray; label.Location = new Point(26, 73); card.Controls.Add(label); return card;
        }
    }
}
