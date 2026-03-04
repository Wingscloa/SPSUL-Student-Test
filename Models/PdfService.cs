using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SPSUL.Models.Data;
using SPSUL.Models.ViewModels;

namespace SPSUL.Models
{
    public class PdfService
    {
        private readonly IWebHostEnvironment _env;

        public PdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        // ============================================
        // SHARED HEADER
        // ============================================
        private void ComposeHeader(IContainer container)
        {
            var logoSchoolPath = Path.Combine(_env.WebRootPath, "Image", "logo", "SPSUL.webp");
            var logoUkPath = Path.Combine(_env.WebRootPath, "Image", "logo", "spsul-logo-uk.webp");

            container.Row(row =>
            {
                if (File.Exists(logoUkPath))
                {
                    row.ConstantItem(60).AlignMiddle().Image(logoUkPath).FitArea();
                }

                row.RelativeItem().PaddingLeft(8).AlignMiddle().Column(col =>
                {
                    col.Item().Text("SPŠ").Bold().FontSize(14).FontColor("#2D8C3C");
                    col.Item().Text("Ústí nad Labem").Bold().FontSize(10).FontColor("#2D8C3C");
                });

                row.RelativeItem().AlignMiddle().AlignRight().Column(col =>
                {
                    col.Item().Text("Elektrotechnika").FontSize(7).FontColor("#333");
                    col.Item().Text("Informaèní technologie").FontSize(7).FontColor("#333");
                    col.Item().Text("Strojírenství").FontSize(7).FontColor("#333");
                    col.Item().Text("Doprava a logistika").FontSize(7).FontColor("#333");
                });

                if (File.Exists(logoSchoolPath))
                {
                    row.ConstantItem(80).AlignMiddle().PaddingLeft(10).Image(logoSchoolPath).FitArea();
                }
            });
        }

        private Document CreateDocument(string title, Action<IContainer> composeContent)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginTop(25);
                    page.MarginBottom(25);
                    page.MarginHorizontal(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(col =>
                    {
                        col.Item().Element(ComposeHeader);
                        col.Item().PaddingVertical(6).LineHorizontal(1).LineColor("#ccc");
                        col.Item().PaddingBottom(8).AlignCenter().Text(title).Bold().FontSize(13);
                    });

                    page.Content().Element(composeContent);

                    page.Footer().AlignCenter().Text(t =>
                    {
                        t.Span("Strana ");
                        t.CurrentPageNumber();
                        t.Span(" / ");
                        t.TotalPages();
                    });
                });
            });
        }

        // ============================================
        // CODES PDF
        // ============================================
        public byte[] GenerateCodesPdf(Test test, List<StudentTest> assignments)
        {
            return CreateDocument($"Pøihlašovací kódy – {test.Name}", content =>
            {
                content.Column(col =>
                {
                    var chunks = assignments
                        .Select((a, i) => new { a, i })
                        .GroupBy(x => x.i / 3)
                        .Select(g => g.Select(x => x.a).ToList())
                        .ToList();

                    foreach (var chunk in chunks)
                    {
                        col.Item().PaddingBottom(8).Row(row =>
                        {
                            for (int c = 0; c < 3; c++)
                            {
                                if (c > 0)
                                    row.ConstantItem(8);

                                if (c < chunk.Count)
                                {
                                    var a = chunk[c];
                                    row.RelativeItem().Border(1).BorderColor("#333").Padding(8).Column(card =>
                                    {
                                        card.Item().AlignCenter().Text($"{a.Student.FirstName} {a.Student.LastName}")
                                            .Bold().FontSize(10);
                                        card.Item().AlignCenter().Text(test.Name)
                                            .FontSize(7).FontColor("#666");
                                        card.Item().PaddingVertical(6).AlignCenter()
                                            .Background("#f5f5f5").Padding(6)
                                            .Text(a.LoginId)
                                            .Bold().FontSize(16).LetterSpacing(0.15f);
                                        card.Item().AlignCenter().Text("Zadejte tento kód na pøihlašovací stránce testu")
                                            .FontSize(6).FontColor("#888");
                                    });
                                }
                                else
                                {
                                    row.RelativeItem();
                                }
                            }
                        });
                    }
                });
            }).GeneratePdf();
        }

        // ============================================
        // STUDENTS PDF
        // ============================================
        public byte[] GenerateStudentsPdf(List<Student> students)
        {
            return CreateDocument("Seznam studentù", content =>
            {
                content.Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(30);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(1.5f);
                    });

                    table.Header(header =>
                    {
                        HeaderCell(header, "#");
                        HeaderCell(header, "Jméno");
                        HeaderCell(header, "Tøídy");
                        HeaderCell(header, "Stav");
                    });

                    int idx = 1;
                    foreach (var s in students)
                    {
                        var classes = s.ClassesStudents != null
                            ? string.Join(", ", s.ClassesStudents.Select(cs => cs.Classes?.Name))
                            : "";

                        DataCell(table, idx.ToString());
                        DataCell(table, $"{s.FirstName} {s.LastName}");
                        DataCell(table, classes);
                        DataCell(table, s.IsActive ? "Aktivní" : "Neaktivní");
                        idx++;
                    }
                });
            }).GeneratePdf();
        }

        // ============================================
        // TEACHERS PDF
        // ============================================
        public byte[] GenerateTeachersPdf(List<Teacher> teachers)
        {
            return CreateDocument("Seznam uèitelù", content =>
            {
                content.Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(30);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.ConstantColumn(55);
                    });

                    table.Header(header =>
                    {
                        HeaderCell(header, "#");
                        HeaderCell(header, "Jméno");
                        HeaderCell(header, "Tituly");
                        HeaderCell(header, "Pøezdívka");
                        HeaderCell(header, "Stav");
                    });

                    int idx = 1;
                    foreach (var t in teachers)
                    {
                        var titles = t.Titles != null
                            ? string.Join(", ", t.Titles.Select(tt => tt.Title?.Shortcut))
                            : "";

                        DataCell(table, idx.ToString());
                        DataCell(table, $"{t.FirstName} {t.LastName}");
                        DataCell(table, titles);
                        DataCell(table, $"@{t.NickName}");
                        DataCell(table, t.IsActive ? "Aktivní" : "Neaktivní");
                        idx++;
                    }
                });
            }).GeneratePdf();
        }

        // ============================================
        // CLASSES PDF
        // ============================================
        public byte[] GenerateClassesPdf(List<Classes> classes)
        {
            return CreateDocument("Seznam tøíd", content =>
            {
                content.Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(30);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(1.5f);
                        cols.RelativeColumn(1.5f);
                        cols.ConstantColumn(55);
                    });

                    table.Header(header =>
                    {
                        HeaderCell(header, "#");
                        HeaderCell(header, "Název");
                        HeaderCell(header, "Obory");
                        HeaderCell(header, "Od");
                        HeaderCell(header, "Do");
                        HeaderCell(header, "Stav");
                    });

                    int idx = 1;
                    foreach (var c in classes)
                    {
                        var fields = c.ClassesFields != null
                            ? string.Join(", ", c.ClassesFields.Select(cf => cf.StudentField?.Name))
                            : "";

                        DataCell(table, idx.ToString());
                        DataCell(table, c.Name);
                        DataCell(table, fields);
                        DataCell(table, c.StartFrom.ToString());
                        DataCell(table, c.EndTo.ToString());
                        DataCell(table, c.IsActive ? "Aktivní" : "Neaktivní");
                        idx++;
                    }
                });
            }).GeneratePdf();
        }

        // ============================================
        // TESTS PDF
        // ============================================
        public byte[] GenerateTestsPdf(List<Test> tests)
        {
            return CreateDocument("Seznam testù", content =>
            {
                content.Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(30);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.ConstantColumn(55);
                    });

                    table.Header(header =>
                    {
                        HeaderCell(header, "#");
                        HeaderCell(header, "Název");
                        HeaderCell(header, "Obor");
                        HeaderCell(header, "Autor");
                        HeaderCell(header, "Stav");
                    });

                    int idx = 1;
                    foreach (var t in tests)
                    {
                        DataCell(table, idx.ToString());
                        DataCell(table, t.Name);
                        DataCell(table, t.StudentField?.Name ?? "");
                        DataCell(table, t.Creator != null ? $"{t.Creator.FirstName} {t.Creator.LastName}" : "");
                        DataCell(table, t.IsActive ? "Aktivní" : "Neaktivní");
                        idx++;
                    }
                });
            }).GeneratePdf();
        }

        // ============================================
        // QUESTIONS PDF
        // ============================================
        public byte[] GenerateQuestionsPdf(List<Question> questions)
        {
            return CreateDocument("Seznam otázek", content =>
            {
                content.Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(25);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.ConstantColumn(55);
                    });

                    table.Header(header =>
                    {
                        HeaderCell(header, "#");
                        HeaderCell(header, "Název");
                        HeaderCell(header, "Tvùrce");
                        HeaderCell(header, "Typ otázky");
                        HeaderCell(header, "Pøedmìt");
                        HeaderCell(header, "Stav");
                    });

                    int idx = 1;
                    foreach (var q in questions)
                    {
                        DataCell(table, idx.ToString());
                        DataCell(table, q.Header);
                        DataCell(table, q.Creator != null ? $"{q.Creator.FirstName} {q.Creator.LastName}" : "");
                        DataCell(table, q.QuestionType?.Name ?? "");
                        DataCell(table, q.Field?.Name ?? "");
                        DataCell(table, q.IsActive ? "Aktivní" : "Neaktivní");
                        idx++;
                    }
                });
            }).GeneratePdf();
        }

        // ============================================
        // RESULTS PDF (Výsledky)
        // ============================================
        public byte[] GenerateResultsPdf(List<AssignedTestVm> results)
        {
            return CreateDocument("Výsledky testù", content =>
            {
                content.Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(25);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2.5f);
                        cols.RelativeColumn(2.2f);
                        cols.RelativeColumn(2.2f);
                        cols.ConstantColumn(58);
                        cols.ConstantColumn(62);
                    });

                    table.Header(header =>
                    {
                        HeaderCell(header, "#");
                        HeaderCell(header, "Test");
                        HeaderCell(header, "Student");
                        HeaderCell(header, "Zaèal");
                        HeaderCell(header, "Dokonèil");
                        HeaderCell(header, "Úspìšnost");
                        HeaderCell(header, "Výsledek");
                    });

                    int idx = 1;
                    foreach (var r in results)
                    {
                        DataCell(table, idx.ToString());
                        DataCell(table, r.Nazev);
                        DataCell(table, r.Jmeno);
                        DataCell(table, r.ZacalV.ToString("dd.MM.yyyy HH:mm"));
                        DataCell(table, r.DokoncilV.ToString("dd.MM.yyyy HH:mm"));
                        DataCell(table, $"{r.UspechPct} %");
                        DataCell(table, r.Absolvoval ? "Prospìl" : "Neprospìl");
                        idx++;
                    }
                });
            }).GeneratePdf();
        }

        // ============================================
        // TABLE HELPERS
        // ============================================
        private static void HeaderCell(TableCellDescriptor header, string text)
        {
            header.Cell().Background("#ff8a00").Padding(5)
                .Text(text).Bold().FontSize(9).FontColor(Colors.White).ClampLines(1);
        }

        private static void DataCell(TableDescriptor table, string text)
        {
            table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                .Text(text).FontSize(8).ClampLines(1);
        }
    }
}
