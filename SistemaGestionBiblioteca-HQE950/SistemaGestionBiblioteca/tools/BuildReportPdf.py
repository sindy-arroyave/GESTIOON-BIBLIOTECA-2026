from __future__ import annotations

import html
import hashlib
import re
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_JUSTIFY, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import cm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    BaseDocTemplate,
    Frame,
    Image,
    KeepTogether,
    ListFlowable,
    ListItem,
    PageBreak,
    PageTemplate,
    Paragraph,
    Preformatted,
    Spacer,
    Table,
    TableStyle,
)
from reportlab.platypus.tableofcontents import TableOfContents


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "docs" / "INFORME.md"
OUTPUT = ROOT / "docs" / "INFORME.pdf"
NAVY = colors.HexColor("#12202F")
TEAL = colors.HexColor("#00A896")
BLUE = colors.HexColor("#2B79B5")
SURFACE = colors.HexColor("#F4F7FA")
TEXT = colors.HexColor("#222F3E")
GRAY = colors.HexColor("#687580")


pdfmetrics.registerFont(TTFont("SegoeUI", r"C:\Windows\Fonts\segoeui.ttf"))
pdfmetrics.registerFont(TTFont("SegoeUI-Bold", r"C:\Windows\Fonts\segoeuib.ttf"))
pdfmetrics.registerFontFamily("SegoeUI", normal="SegoeUI", bold="SegoeUI-Bold")


class ReportDocTemplate(BaseDocTemplate):
    def __init__(self, filename: str):
        super().__init__(
            filename,
            pagesize=A4,
            rightMargin=1.8 * cm,
            leftMargin=1.8 * cm,
            topMargin=2.0 * cm,
            bottomMargin=1.8 * cm,
            title="Sistema de Gestión de Biblioteca",
            author="Proyecto de Programación Avanzada",
            subject="Informe técnico y funcional",
        )
        frame = Frame(self.leftMargin, self.bottomMargin, self.width, self.height, id="body")
        self.addPageTemplates(PageTemplate(id="report", frames=[frame], onPage=self._page))

    def _page(self, canvas, doc):
        canvas.saveState()
        if doc.page > 1:
            canvas.setFont("SegoeUI", 8)
            canvas.setFillColor(GRAY)
            canvas.drawString(self.leftMargin, A4[1] - 1.1 * cm, "Sistema de Gestión de Biblioteca")
            canvas.setStrokeColor(colors.HexColor("#D7DFE6"))
            canvas.line(self.leftMargin, A4[1] - 1.3 * cm, A4[0] - self.rightMargin, A4[1] - 1.3 * cm)
        canvas.setFont("SegoeUI", 8)
        canvas.setFillColor(GRAY)
        canvas.drawRightString(A4[0] - self.rightMargin, 0.9 * cm, f"Página {doc.page}")
        canvas.restoreState()

    def afterFlowable(self, flowable):
        if not isinstance(flowable, Paragraph):
            return
        style = flowable.style.name
        if style not in ("Heading1", "Heading2"):
            return
        level = 0 if style == "Heading1" else 1
        text = flowable.getPlainText()
        key = "heading-" + hashlib.sha1(text.encode("utf-8")).hexdigest()[:16]
        self.canv.bookmarkPage(key)
        self.canv.addOutlineEntry(text, key, level=level, closed=False)
        self.notify("TOCEntry", (level, text, self.page, key))


styles = getSampleStyleSheet()
for style in styles.byName.values():
    style.fontName = "SegoeUI"
    style.textColor = TEXT

styles.add(ParagraphStyle(name="CoverTitle", parent=styles["Title"], fontName="SegoeUI-Bold", fontSize=29, leading=35, textColor=NAVY, alignment=TA_CENTER, spaceAfter=22))
styles.add(ParagraphStyle(name="CoverSubtitle", parent=styles["Heading2"], fontName="SegoeUI-Bold", fontSize=17, leading=23, textColor=TEAL, alignment=TA_CENTER, spaceAfter=18))
styles["Heading1"].fontName = "SegoeUI-Bold"
styles["Heading1"].fontSize = 20
styles["Heading1"].leading = 25
styles["Heading1"].textColor = NAVY
styles["Heading1"].spaceBefore = 7
styles["Heading1"].spaceAfter = 11
styles["Heading1"].keepWithNext = True
styles["Heading2"].fontName = "SegoeUI-Bold"
styles["Heading2"].fontSize = 14
styles["Heading2"].leading = 18
styles["Heading2"].textColor = BLUE
styles["Heading2"].spaceBefore = 8
styles["Heading2"].spaceAfter = 7
styles.add(ParagraphStyle(name="Heading3Custom", parent=styles["Heading3"], fontName="SegoeUI-Bold", fontSize=11.5, leading=15, textColor=TEAL, spaceBefore=6, spaceAfter=5))
styles["BodyText"].fontName = "SegoeUI"
styles["BodyText"].fontSize = 9.4
styles["BodyText"].leading = 13.2
styles["BodyText"].alignment = TA_JUSTIFY
styles["BodyText"].spaceAfter = 6
styles.add(ParagraphStyle(name="BulletText", parent=styles["BodyText"], leftIndent=15, firstLineIndent=0, bulletIndent=3, alignment=TA_LEFT, spaceAfter=3))
styles.add(ParagraphStyle(name="Note", parent=styles["BodyText"], backColor=colors.HexColor("#E8F7F4"), borderColor=TEAL, borderWidth=0.8, borderPadding=8, textColor=TEXT, spaceBefore=5, spaceAfter=8))
styles.add(ParagraphStyle(name="Caption", parent=styles["BodyText"], fontSize=8, leading=10, textColor=GRAY, alignment=TA_CENTER, spaceBefore=3, spaceAfter=9))
styles.add(ParagraphStyle(name="TableCell", parent=styles["BodyText"], fontSize=7.4, leading=9.2, alignment=TA_LEFT, spaceAfter=0))
styles.add(ParagraphStyle(name="TableHeader", parent=styles["TableCell"], fontName="SegoeUI-Bold", textColor=colors.white))
styles.add(ParagraphStyle(name="CodeBlock", parent=styles["Code"], fontName="Courier", fontSize=7.5, leading=9.3, backColor=colors.HexColor("#F1F3F5"), borderPadding=7, spaceBefore=4, spaceAfter=8))


def inline(text: str) -> str:
    safe = html.escape(text.strip())
    safe = re.sub(r"`([^`]+)`", r'<font name="Courier">\1</font>', safe)
    safe = re.sub(r"\*\*([^*]+)\*\*", r"<b>\1</b>", safe)
    safe = re.sub(r"\*([^*]+)\*", r"<i>\1</i>", safe)
    safe = re.sub(r"\[([^\]]+)\]\(([^)]+)\)", r'<link href="\2" color="#2B79B5">\1</link>', safe)
    return safe


def table_flowable(rows):
    data = []
    for index, row in enumerate(rows):
        style = styles["TableHeader"] if index == 0 else styles["TableCell"]
        data.append([Paragraph(inline(cell), style) for cell in row])
    columns = len(rows[0])
    available = A4[0] - 3.6 * cm
    widths = [available / columns] * columns
    table = Table(data, colWidths=widths, repeatRows=1, hAlign="LEFT")
    table.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), NAVY),
        ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
        ("GRID", (0, 0), (-1, -1), 0.45, colors.HexColor("#C6D0D9")),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING", (0, 0), (-1, -1), 5),
        ("RIGHTPADDING", (0, 0), (-1, -1), 5),
        ("TOPPADDING", (0, 0), (-1, -1), 4),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, SURFACE]),
    ]))
    return table


def parse_markdown():
    lines = SOURCE.read_text(encoding="utf-8").splitlines()
    story = []
    paragraph = []
    index = 0
    cover = True
    skip_manual_toc = False
    manual_breaks = 0

    def flush_paragraph():
        if paragraph:
            story.append(Paragraph(inline(" ".join(part.strip() for part in paragraph)), styles["BodyText"]))
            paragraph[:] = []

    while index < len(lines):
        raw = lines[index]
        line = raw.strip()

        if line.startswith("<div align=") or line == "</div>" or (line.startswith("<div") and "Página" in line):
            flush_paragraph(); index += 1; continue
        if "page-break-after" in line:
            flush_paragraph(); manual_breaks += 1
            if manual_breaks <= 3: story.append(PageBreak())
            else: story.append(Spacer(1, 8))
            cover = False; skip_manual_toc = False; index += 1; continue
        if not line:
            flush_paragraph(); index += 1; continue

        if line.startswith("```"):
            flush_paragraph(); code = []; index += 1
            while index < len(lines) and not lines[index].strip().startswith("```"):
                code.append(lines[index]); index += 1
            story.append(KeepTogether([Preformatted("\n".join(code), styles["CodeBlock"])])); index += 1; continue

        image_match = re.match(r"!\[([^\]]*)\]\(([^)]+)\)", line)
        if image_match:
            flush_paragraph(); image_path = (SOURCE.parent / image_match.group(2)).resolve()
            img = Image(str(image_path)); max_w, max_h = 17.2 * cm, 13.5 * cm
            scale = min(max_w / img.imageWidth, max_h / img.imageHeight)
            img.drawWidth = img.imageWidth * scale; img.drawHeight = img.imageHeight * scale
            img.hAlign = "CENTER"
            image_block = [img]
            if image_match.group(1): image_block.append(Paragraph(inline(image_match.group(1)), styles["Caption"]))
            story.append(KeepTogether(image_block))
            index += 1; continue

        if line.startswith("|") and index + 1 < len(lines) and lines[index + 1].strip().startswith("|"):
            flush_paragraph(); table_lines = []
            while index < len(lines) and lines[index].strip().startswith("|"):
                table_lines.append(lines[index].strip()); index += 1
            parsed = [[cell.strip() for cell in value.strip("|").split("|")] for value in table_lines]
            if len(parsed) > 1 and all(re.match(r"^:?-{3,}:?$", cell.replace(" ", "")) for cell in parsed[1]): parsed.pop(1)
            story.append(table_flowable(parsed)); story.append(Spacer(1, 7)); continue

        if line.startswith("# "):
            flush_paragraph(); text = line[2:].strip()
            if text in ("8. Modelo entidad-relación", "10. Arquitectura del sistema"):
                story.append(PageBreak())
            if cover: story.extend([Spacer(1, 4.5 * cm), Paragraph(inline(text), styles["CoverTitle"])])
            else: story.append(Paragraph(inline(text), styles["Heading1"]))
            if text == "Tabla de contenido":
                toc = TableOfContents(); toc.levelStyles = [ParagraphStyle(name="TOC1", fontName="SegoeUI", fontSize=9.5, leading=14, leftIndent=0, firstLineIndent=0, textColor=TEXT), ParagraphStyle(name="TOC2", fontName="SegoeUI", fontSize=8.5, leading=12, leftIndent=14, firstLineIndent=0, textColor=GRAY)]
                story.append(toc); skip_manual_toc = True
            index += 1; continue
        if skip_manual_toc and re.match(r"^\d+\. \[", line):
            index += 1; continue
        if line.startswith("## "):
            flush_paragraph(); text = line[3:].strip()
            if text == "13.2 Gestión de libros": story.append(PageBreak())
            style = styles["CoverSubtitle"] if cover else styles["Heading2"]; story.append(Paragraph(inline(text), style)); index += 1; continue
        if line.startswith("### "):
            flush_paragraph(); story.append(Paragraph(inline(line[4:]), styles["Heading3Custom"])); index += 1; continue

        if line.startswith("> "):
            flush_paragraph(); story.append(Paragraph(inline(line[2:]), styles["Note"])); index += 1; continue
        if re.match(r"^[-*] ", line):
            flush_paragraph(); items = []
            while index < len(lines) and re.match(r"^[-*] ", lines[index].strip()):
                items.append(ListItem(Paragraph(inline(lines[index].strip()[2:]), styles["BulletText"]), leftIndent=12)); index += 1
            story.append(ListFlowable(items, bulletType="bullet", leftIndent=18, bulletFontName="SegoeUI", bulletFontSize=7)); story.append(Spacer(1, 4)); continue
        if re.match(r"^\d+\. ", line):
            flush_paragraph(); items = []
            while index < len(lines) and re.match(r"^\d+\. ", lines[index].strip()):
                items.append(ListItem(Paragraph(inline(re.sub(r"^\d+\. ", "", lines[index].strip())), styles["BulletText"]), leftIndent=12)); index += 1
            story.append(ListFlowable(items, bulletType="1", leftIndent=22, bulletFontName="SegoeUI", bulletFontSize=8)); story.append(Spacer(1, 4)); continue

        paragraph.append(line)
        index += 1

    flush_paragraph()
    return story


if __name__ == "__main__":
    doc = ReportDocTemplate(str(OUTPUT))
    doc.multiBuild(parse_markdown())
    print(f"PDF generado: {OUTPUT}")
