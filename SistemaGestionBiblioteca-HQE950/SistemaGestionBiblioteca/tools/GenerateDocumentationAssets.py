from pathlib import Path
from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "docs" / "diagramas"
OUT.mkdir(parents=True, exist_ok=True)

FONT = r"C:\Windows\Fonts\segoeui.ttf"
BOLD = r"C:\Windows\Fonts\segoeuib.ttf"
NAVY = (18, 32, 47)
TEAL = (0, 168, 150)
BLUE = (43, 121, 181)
PURPLE = (138, 99, 210)
ORANGE = (239, 142, 53)
SURFACE = (244, 247, 250)
TEXT = (34, 47, 62)
WHITE = (255, 255, 255)
GRAY = (105, 115, 125)


def font(size, bold=False):
    return ImageFont.truetype(BOLD if bold else FONT, size)


def title(draw, text, width):
    draw.text((width // 2, 50), text, fill=NAVY, font=font(42, True), anchor="ma")


def box(draw, xy, heading, lines, accent=TEAL, width=3):
    x1, y1, x2, y2 = xy
    draw.rounded_rectangle(xy, radius=14, fill=WHITE, outline=(215, 223, 230), width=width)
    draw.rounded_rectangle((x1, y1, x2, y1 + 54), radius=14, fill=accent)
    draw.rectangle((x1, y1 + 39, x2, y1 + 54), fill=accent)
    draw.text((x1 + 18, y1 + 27), heading, fill=WHITE, font=font(22, True), anchor="lm")
    y = y1 + 76
    for line in lines:
        draw.text((x1 + 18, y), line, fill=TEXT, font=font(17), anchor="lm")
        y += 29


def arrow(draw, start, end, color=NAVY, width=5):
    x1, y1 = start
    x2, y2 = end
    draw.line((x1, y1, x2, y2), fill=color, width=width)
    if abs(x2 - x1) > abs(y2 - y1):
        sign = 1 if x2 > x1 else -1
        points = [(x2, y2), (x2 - sign * 18, y2 - 10), (x2 - sign * 18, y2 + 10)]
    else:
        sign = 1 if y2 > y1 else -1
        points = [(x2, y2), (x2 - 10, y2 - sign * 18), (x2 + 10, y2 - sign * 18)]
    draw.polygon(points, fill=color)


def architecture():
    image = Image.new("RGB", (1600, 930), SURFACE)
    draw = ImageDraw.Draw(image)
    title(draw, "Arquitectura por capas", image.width)
    layers = [
        ("Presentación", "Windows Forms: formularios, DataGridView, validación visual", TEAL),
        ("Lógica de negocio", "Servicios, reglas, validaciones y manejo de excepciones", BLUE),
        ("Acceso a datos", "Repositorios y ADO.NET con consultas parametrizadas", PURPLE),
        ("Base de datos", "SQL Server: tablas, vistas y procedimientos transaccionales", ORANGE),
    ]
    y = 140
    for index, (name, detail, color) in enumerate(layers):
        draw.rounded_rectangle((260, y, 1340, y + 130), radius=18, fill=WHITE, outline=color, width=4)
        draw.rounded_rectangle((260, y, 520, y + 130), radius=18, fill=color)
        draw.rectangle((500, y, 520, y + 130), fill=color)
        draw.text((390, y + 65), name, fill=WHITE, font=font(25, True), anchor="mm")
        draw.text((560, y + 65), detail, fill=TEXT, font=font(22), anchor="lm")
        if index < len(layers) - 1:
            arrow(draw, (800, y + 130), (800, y + 175), NAVY, 5)
        y += 185
    image.save(OUT / "arquitectura.png")


def er_diagram():
    image = Image.new("RGB", (1800, 1180), SURFACE)
    draw = ImageDraw.Draw(image)
    title(draw, "Modelo entidad-relación", image.width)
    positions = {
        "Autores": (80, 150, 500, 380),
        "Categorías": (80, 700, 500, 930),
        "Libros": (680, 390, 1120, 730),
        "Usuarios": (1300, 120, 1720, 410),
        "Préstamos": (1280, 520, 1720, 840),
        "DetallePrestamos": (680, 850, 1160, 1120),
    }
    # Relations are drawn first so boxes remain legible.
    arrow(draw, (500, 265), (680, 470), TEAL, 4)
    arrow(draw, (500, 815), (680, 650), TEAL, 4)
    arrow(draw, (1300, 300), (1460, 520), BLUE, 4)
    arrow(draw, (1440, 840), (1160, 965), PURPLE, 4)
    arrow(draw, (900, 850), (900, 730), PURPLE, 4)
    draw.text((565, 330), "1 : N", fill=GRAY, font=font(19, True))
    draw.text((565, 745), "1 : N", fill=GRAY, font=font(19, True))
    draw.text((1430, 455), "1 : N", fill=GRAY, font=font(19, True))
    draw.text((1210, 900), "1 : N", fill=GRAY, font=font(19, True))
    draw.text((920, 770), "1 : N", fill=GRAY, font=font(19, True))
    box(draw, positions["Autores"], "Autores", ["PK AutorId", "UQ Código", "Nombre, Apellidos", "Nacionalidad", "FechaNacimiento"], TEAL)
    box(draw, positions["Categorías"], "Categorías", ["PK CategoriaId", "UQ Nombre", "Descripción", "Activa"], TEAL)
    box(draw, positions["Libros"], "Libros", ["PK LibroId", "UQ Código, ISBN", "Título", "FK AutorId", "FK CategoriaId", "CantidadTotal", "CantidadDisponible"], ORANGE)
    box(draw, positions["Usuarios"], "Usuarios", ["PK UsuarioId", "UQ Documento", "Nombre, Apellidos", "Teléfono", "UQ Correo", "ProgramaAcadémico"], BLUE)
    box(draw, positions["Préstamos"], "Préstamos", ["PK PrestamoId", "FK UsuarioId", "FechaPrestamo", "FechaDevoluciónEsperada", "FechaDevoluciónReal", "Estado"], PURPLE)
    box(draw, positions["DetallePrestamos"], "DetallePrestamos", ["PK DetallePrestamoId", "FK PrestamoId", "FK LibroId", "Cantidad", "UQ Préstamo + Libro"], PURPLE)
    image.save(OUT / "modelo-er.png")


def class_diagram():
    image = Image.new("RGB", (1800, 1240), SURFACE)
    draw = ImageDraw.Draw(image)
    title(draw, "Diagrama de clases principal", image.width)
    # Inheritance and associations.
    arrow(draw, (900, 255), (900, 340), NAVY, 4)
    arrow(draw, (780, 500), (420, 600), NAVY, 4)
    arrow(draw, (1020, 500), (1380, 600), NAVY, 4)
    arrow(draw, (1270, 780), (1060, 920), PURPLE, 4)
    arrow(draw, (1110, 1050), (760, 870), ORANGE, 4)
    box(draw, (680, 115, 1120, 255), "EntidadBase", ["+ Id : int", "+ Validar() : IList<string>"], NAVY)
    box(draw, (680, 340, 1120, 500), "Persona", ["+ Nombre : string", "+ Apellidos : string", "+ NombreCompleto : string"], BLUE)
    box(draw, (120, 600, 620, 840), "Autor", ["+ Código : string", "+ Nacionalidad : string", "+ FechaNacimiento : DateTime", "+ Validar()"], TEAL)
    box(draw, (1180, 600, 1680, 870), "Usuario", ["+ Documento : string", "+ Teléfono : string", "+ Correo : string", "+ ProgramaAcadémico : string", "+ Validar()"], TEAL)
    box(draw, (120, 930, 620, 1165), "Categoría", ["+ Nombre : string", "+ Descripción : string", "+ Validar()"], ORANGE)
    box(draw, (650, 800, 1110, 1100), "Libro", ["+ Código, ISBN, Título", "+ AutorId, CategoriaId", "+ CantidadTotal", "+ CantidadDisponible", "+ Disponible : bool", "+ Validar()"], ORANGE)
    box(draw, (1180, 940, 1680, 1190), "Préstamo", ["+ UsuarioId : int", "+ Fechas", "+ Estado", "+ Detalles", "+ Validar()"], PURPLE)
    image.save(OUT / "diagrama-clases.png")


if __name__ == "__main__":
    architecture()
    er_diagram()
    class_diagram()
    print(f"Diagramas generados en {OUT}")
