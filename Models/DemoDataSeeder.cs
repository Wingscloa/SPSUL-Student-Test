using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Data;
using System.Text.Json;

namespace SPSUL.Models
{
    public static class DemoDataSeeder
    {
        public const string DemoLoginCode = "DEMO2025";

        public static async Task SeedAsync(SpsulContext ctx)
        {
            var existing = await ctx.StudentTests
                .Include(st => st.Test)
                .FirstOrDefaultAsync(st => st.LoginId == DemoLoginCode);

            // Pokud demo pøiøazení existuje a ještì nebylo zahájeno — nic nedìlej
            if (existing != null && existing.StartedAt == DateTime.MinValue)
                return;

            // Pokud bylo zahájeno/dokonèeno — resetuj ho pro dalšího studenta
            if (existing != null)
            {
                existing.StartedAt = DateTime.MinValue;
                existing.FinishedAt = DateTime.MinValue;
                existing.ResultSnapshot = "{}";
                existing.ShuffleOrder = null;

                // Ujisti se, že test je aktivní
                if (existing.Test != null)
                    existing.Test.IsActive = true;

                await ctx.SaveChangesAsync();
                return;
            }

            // 1) Zajisti, že existuje alespoò jeden uèitel (CreatorId)
            var teacher = await ctx.Teachers.FirstOrDefaultAsync();
            if (teacher == null)
            {
                teacher = new Teacher
                {
                    FirstName = "Demo",
                    LastName = "Uèitel",
                    NickName = "demo",
                    PasswordHash = "not-a-real-hash",
                    IsActive = true
                };
                ctx.Teachers.Add(teacher);
                await ctx.SaveChangesAsync();
            }

            // 2) Zajisti pøedmìt
            var field = await ctx.StudentFields.FirstOrDefaultAsync(f => f.IsActive);
            if (field == null)
            {
                field = new StudentField { Name = "Obecné", IsActive = true };
                ctx.StudentFields.Add(field);
                await ctx.SaveChangesAsync();
            }

            // 3) Zajisti demo studenta
            var student = await ctx.Students.FirstOrDefaultAsync(s => s.FirstName == "Testovací" && s.LastName == "Student");
            if (student == null)
            {
                student = new Student
                {
                    FirstName = "Testovací",
                    LastName = "Student",
                    IsActive = true
                };
                ctx.Students.Add(student);
                await ctx.SaveChangesAsync();
            }

            // 4) Vytvoø snapshot otázek (5 otázek, každá 4 možnosti, 1 správná)
            var questions = new[]
            {
                new {
                    QuestionId = 1,
                    Header = "Hlavní mìsto Èeské republiky?",
                    Description = "Vyberte správnou odpovìï.",
                    QuestionType = "Výbìr z možností",
                    Options = new[] {
                        new { Text = "Praha", ImageKey = (string?)null, IsCorrect = true },
                        new { Text = "Brno", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "Ostrava", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "Plzeò", ImageKey = (string?)null, IsCorrect = false }
                    }
                },
                new {
                    QuestionId = 2,
                    Header = "Kolik je 7 × 8?",
                    Description = "Základní násobení.",
                    QuestionType = "Výbìr z možností",
                    Options = new[] {
                        new { Text = "54", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "56", ImageKey = (string?)null, IsCorrect = true },
                        new { Text = "58", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "64", ImageKey = (string?)null, IsCorrect = false }
                    }
                },
                new {
                    QuestionId = 3,
                    Header = "Který prvek má chemickou znaèku O?",
                    Description = "Periodická tabulka prvkù.",
                    QuestionType = "Výbìr z možností",
                    Options = new[] {
                        new { Text = "Zlato", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "Kyslík", ImageKey = (string?)null, IsCorrect = true },
                        new { Text = "Osmium", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "Olovo", ImageKey = (string?)null, IsCorrect = false }
                    }
                },
                new {
                    QuestionId = 4,
                    Header = "Ve kterém roce zaèala 2. svìtová válka?",
                    Description = "Historická otázka.",
                    QuestionType = "Výbìr z možností",
                    Options = new[] {
                        new { Text = "1936", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "1938", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "1939", ImageKey = (string?)null, IsCorrect = true },
                        new { Text = "1941", ImageKey = (string?)null, IsCorrect = false }
                    }
                },
                new {
                    QuestionId = 5,
                    Header = "Jak se jmenuje nejdelší øeka v ÈR?",
                    Description = "Geografie Èeské republiky.",
                    QuestionType = "Výbìr z možností",
                    Options = new[] {
                        new { Text = "Labe", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "Morava", ImageKey = (string?)null, IsCorrect = false },
                        new { Text = "Vltava", ImageKey = (string?)null, IsCorrect = true },
                        new { Text = "Odra", ImageKey = (string?)null, IsCorrect = false }
                    }
                }
            };

            string snapshotJson = JsonSerializer.Serialize(questions);

            // 5) Vytvoø test
            var test = new Test
            {
                Name = "Demo test – Vyzkoušej si to!",
                CreatorId = teacher.TeacherId,
                StudentFieldId = field.StudentFieldId,
                QuestionSnapshot = snapshotJson,
                TimeLimitMinutes = 10,
                ShuffleQuestions = true,
                IsActive = true
            };
            ctx.Tests.Add(test);
            await ctx.SaveChangesAsync();

            // 6) Pøiøaï test studentovi s demo kódem
            var assignment = new StudentTest
            {
                StudentId = student.StudentId,
                TestId = test.TestId,
                LoginId = DemoLoginCode,
                StartedAt = DateTime.MinValue,
                FinishedAt = DateTime.MinValue,
                ResultSnapshot = "{}"
            };
            ctx.StudentTests.Add(assignment);
            await ctx.SaveChangesAsync();
        }
    }
}
