from pathlib import Path
import subprocess

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "docs" / "capturas" / "06-historial-git.png"
log = subprocess.check_output(
    ["git", "log", "--oneline", "--decorate", "--reverse"],
    cwd=str(ROOT),
    text=True,
    encoding="utf-8",
).strip().splitlines()

image = Image.new("RGB", (1600, 880), "#F4F7FA")
draw = ImageDraw.Draw(image)
regular = ImageFont.truetype(r"C:\Windows\Fonts\consola.ttf", 28)
bold = ImageFont.truetype(r"C:\Windows\Fonts\consolab.ttf", 34)
title = ImageFont.truetype(r"C:\Windows\Fonts\segoeuib.ttf", 46)

draw.rounded_rectangle((70, 55, 1530, 825), radius=18, fill="#12202F")
draw.rectangle((70, 55, 1530, 135), fill="#1C3044")
draw.text((110, 78), "Historial Git - SistemaGestionBiblioteca", fill="white", font=title)
draw.text((110, 175), "> git log --oneline --decorate --reverse", fill="#00D4B8", font=bold)
y = 245
colors = ["#8BE9FD", "#50FA7B", "#FFB86C", "#BD93F9", "#F1FA8C", "#FF79C6"]
for index, line in enumerate(log):
    commit, message = line.split(" ", 1)
    draw.text((120, y), commit, fill=colors[index % len(colors)], font=bold)
    draw.text((315, y + 3), message, fill="#F8F8F2", font=regular)
    y += 78
draw.text((110, 740), "Rama: main   |   Estado: documentación y entregables versionados", fill="#B9C4CE", font=regular)
image.save(OUT)
print(OUT)
