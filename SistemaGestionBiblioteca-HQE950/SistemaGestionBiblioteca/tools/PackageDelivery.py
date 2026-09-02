from pathlib import Path
from zipfile import ZIP_DEFLATED, ZipFile


ROOT = Path(__file__).resolve().parents[1]
OUTPUT = ROOT.parent / "SistemaGestionBiblioteca-Entrega.zip"


def include(path: Path) -> bool:
    relative = path.relative_to(ROOT)
    parts = relative.parts
    if "__pycache__" in parts:
        return False
    if parts and parts[0] == "build":
        return len(parts) >= 2 and parts[1] == "app"
    if path.suffix.lower() in {".tmp", ".log"}:
        return False
    return True


with ZipFile(OUTPUT, "w", compression=ZIP_DEFLATED, compresslevel=9) as archive:
    for path in sorted(ROOT.rglob("*")):
        if path.is_file() and include(path):
            archive.write(path, Path(ROOT.name) / path.relative_to(ROOT))

with ZipFile(OUTPUT) as archive:
    problem = archive.testzip()
    required = {
        f"{ROOT.name}/SistemaGestionBiblioteca.sln",
        f"{ROOT.name}/README.md",
        f"{ROOT.name}/docs/INFORME.md",
        f"{ROOT.name}/docs/INFORME.pdf",
        f"{ROOT.name}/sql/01_Crear_BibliotecaDB.sql",
        f"{ROOT.name}/.git/HEAD",
    }
    names = set(archive.namelist())
    missing = required - names
    if problem:
        raise RuntimeError(f"Archivo dañado dentro del ZIP: {problem}")
    if missing:
        raise RuntimeError(f"Faltan entregables en el ZIP: {sorted(missing)}")
    print(f"ZIP verificado: {OUTPUT} ({len(names)} archivos)")
