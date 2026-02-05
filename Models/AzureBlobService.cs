using Azure.Storage.Blobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SPSUL.Models
{
    public enum BlobContainers
    {
        QuestionImage
    }
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public AzureBlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task UploadOptimizedAsync(IFormFile file, string name)
        {
            try
            {
                BlobContainerClient _containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainers.QuestionImage.ToString().ToLowerInvariant());
                
                // Vytvoření kontejneru, pokud neexistuje
                await _containerClient.CreateIfNotExistsAsync();
                
                string fileName = $"{name}.webp"; // WebP je nejmodernější a nejmenší formát
                var blobClient = _containerClient.GetBlobClient(fileName);

                // Načteme obrázek do paměti pomocí ImageSharp
                using var image = await Image.LoadAsync(file.OpenReadStream());

                // 1. Změna velikosti (např. max šířka 1200px při zachování poměru stran)
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(1200, 0),
                    Mode = ResizeMode.Max
                }));

                // 2. Uložení do streamu s kompresí (kvalita 75 % je k nerozeznání, ale ušetří 80 % místa)
                using var outputStream = new MemoryStream();
                await image.SaveAsWebpAsync(outputStream, new SixLabors.ImageSharp.Formats.Webp.WebpEncoder
                {
                    Quality = 75
                });

                outputStream.Position = 0; // Vrátíme se na začátek streamu před nahráním

                // 3. Nahrání optimalizovaných dat do Azurite
                await blobClient.UploadAsync(outputStream, true);
            }
            catch (Exception ex)
            {
                var e = ex;
                throw new ApplicationException("Chyba při nahrávání optimalizovaného obrázku do Azurite.", ex);
            }
        }

        public IFormFile ConvertBase64ToIFormFile(string base64String, string fileName)
        {
            string contentType = "image/jpeg";
            string extension = ".jpg";

            // 1. Detekce MIME type z Base64 hlavičky
            if (base64String.Contains(','))
            {
                var header = base64String.Split(',')[0];
                if (header.Contains("data:"))
                {
                    // Extrakce MIME type z hlavičky (např. "data:image/png;base64")
                    var mimeType = header.Replace("data:", "").Split(';')[0];
                    contentType = mimeType;

                    // Mapování MIME type na příponu
                    extension = mimeType switch
                    {
                        "image/png" => ".png",
                        "image/jpeg" => ".jpg",
                        "image/jpg" => ".jpg",
                        "image/gif" => ".gif",
                        "image/webp" => ".webp",
                        "image/bmp" => ".bmp",
                        "image/svg+xml" => ".svg",
                        _ => ".jpg"
                    };
                }
            }

            // 2. Očištění Base64 od hlavičky
            var base64Parts = base64String.Split(',');
            var pureBase64 = base64Parts.Length > 1 ? base64Parts[1] : base64Parts[0];

            // 3. Převod na byte array
            byte[] fileBytes = Convert.FromBase64String(pureBase64);

            // 4. Fallback detekce z magic bytes (pokud nebyla hlavička)
            if (!base64String.Contains("data:"))
            {
                (contentType, extension) = DetectImageTypeFromBytes(fileBytes);
            }

            // 5. Přidání přípony k názvu souboru (pokud ji nemá)
            if (!fileName.Contains('.'))
            {
                fileName += extension;
            }

            // 6. Vytvoření streamu
            var stream = new MemoryStream(fileBytes);

            // 7. Vytvoření instance FormFile
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return formFile;
        }

        private (string contentType, string extension) DetectImageTypeFromBytes(byte[] bytes)
        {
            // Detekce formátu podle magic bytes (první bajty souboru)
            if (bytes.Length >= 2)
            {
                // PNG: 89 50 4E 47
                if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes.Length >= 4 && 
                    bytes[2] == 0x4E && bytes[3] == 0x47)
                    return ("image/png", ".png");

                // JPEG: FF D8 FF
                if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                    return ("image/jpeg", ".jpg");

                // GIF: 47 49 46
                if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                    return ("image/gif", ".gif");

                // WebP: RIFF ... WEBP
                if (bytes.Length >= 12 && bytes[0] == 0x52 && bytes[1] == 0x49 && 
                    bytes[2] == 0x46 && bytes[3] == 0x46 && 
                    bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
                    return ("image/webp", ".webp");

                // BMP: 42 4D
                if (bytes[0] == 0x42 && bytes[1] == 0x4D)
                    return ("image/bmp", ".bmp");
            }

            // Fallback na JPEG
            return ("image/jpeg", ".jpg");
        }

        /// <summary>
        /// Smaže blob z Azure Blob Storage podle jeho klíče.
        /// </summary>
        /// <param name="blobKey">Klíč/název blobu (např. "q123_opt0_abc123.webp")</param>
        /// <param name="containerName">Název kontejneru (výchozí: QuestionImage)</param>
        /// <returns>True pokud byl blob smazán, false pokud neexistoval</returns>
        public async Task<bool> DeleteBlobAsync(string blobKey, BlobContainers containerName = BlobContainers.QuestionImage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(blobKey))
                {
                    throw new ArgumentException("Klíč blobu nemůže být prázdný.", nameof(blobKey));
                }

                // Přidání přípony .webp pokud chybí (protože UploadOptimizedAsync vždy ukládá jako .webp)
                string fileName = blobKey.EndsWith(".webp") ? blobKey : $"{blobKey}.webp";

                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(
                    containerName.ToString().ToLowerInvariant()
                );

                // Kontrola existence kontejneru
                bool containerExists = await containerClient.ExistsAsync();
                if (!containerExists)
                {
                    return false; // Kontejner neexistuje, blob nemůže existovat
                }

                var blobClient = containerClient.GetBlobClient(fileName);

                // Pokus o smazání blobu
                var response = await blobClient.DeleteIfExistsAsync();
                
                return response.Value; // True pokud byl smazán, False pokud neexistoval
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Chyba při mazání blobu '{blobKey}' z Azurite.", ex);
            }
        }

        /// <summary>
        /// Smaže více blobů najednou.
        /// </summary>
        /// <param name="blobKeys">Seznam klíčů blobů k smazání</param>
        /// <param name="containerName">Název kontejneru (výchozí: QuestionImage)</param>
        /// <returns>Počet úspěšně smazaných blobů</returns>
        public async Task<int> DeleteBlobsAsync(IEnumerable<string> blobKeys, BlobContainers containerName = BlobContainers.QuestionImage)
        {
            int deletedCount = 0;

            foreach (var blobKey in blobKeys)
            {
                try
                {
                    bool deleted = await DeleteBlobAsync(blobKey, containerName);
                    if (deleted)
                    {
                        deletedCount++;
                    }
                }
                catch (Exception ex)
                {
                    // Logování chyby, ale pokračujeme v mazání dalších
                    Console.WriteLine($"Chyba při mazání blobu '{blobKey}': {ex.Message}");
                }
            }

            return deletedCount;
        }

        /// <summary>
        /// Kontrola existence blobu.
        /// </summary>
        /// <param name="blobKey">Klíč/název blobu</param>
        /// <param name="containerName">Název kontejneru (výchozí: QuestionImage)</param>
        /// <returns>True pokud blob existuje</returns>
        public async Task<bool> BlobExistsAsync(string blobKey, BlobContainers containerName = BlobContainers.QuestionImage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(blobKey))
                {
                    return false;
                }

                string fileName = blobKey.EndsWith(".webp") ? blobKey : $"{blobKey}.webp";

                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(
                    containerName.ToString().ToLowerInvariant()
                );

                bool containerExists = await containerClient.ExistsAsync();
                if (!containerExists)
                {
                    return false;
                }

                var blobClient = containerClient.GetBlobClient(fileName);
                return await blobClient.ExistsAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}
