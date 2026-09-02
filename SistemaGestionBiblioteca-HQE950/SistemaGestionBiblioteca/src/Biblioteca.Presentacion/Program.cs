using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Biblioteca.Presentacion
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Any(delegate(string value) { return value.Equals("--capture", StringComparison.OrdinalIgnoreCase); }))
            {
                string output = args.Length > 1 ? args[1] : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "capturas");
                CaptureAll(output);
                return;
            }
            Application.Run(new MainForm(false));
        }

        private static void CaptureAll(string output)
        {
            Directory.CreateDirectory(output);
            Capture(new MainForm(false), Path.Combine(output, "01-inicio.png"));
            Capture(new LibrosForm(false), Path.Combine(output, "02-libros.png"));
            Capture(new AutoresForm(false), Path.Combine(output, "03-autores.png"));
            Capture(new CategoriasForm(false), Path.Combine(output, "04-categorias.png"));
            Capture(new UsuariosForm(false), Path.Combine(output, "05-usuarios.png"));
            Capture(new PrestamosForm(false), Path.Combine(output, "06-prestamos.png"));
            Capture(new DevolucionesForm(false), Path.Combine(output, "07-devoluciones.png"));
            Capture(new ConsultasForm(false), Path.Combine(output, "08-consultas.png"));
        }

        private static void Capture(Form form, string path)
        {
            using (form)
            {
                form.Show();
                Application.DoEvents();
                using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
                {
                    form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                }
                form.Hide();
            }
        }
    }
}
