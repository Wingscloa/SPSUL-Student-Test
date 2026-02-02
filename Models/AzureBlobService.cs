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

        public async Task<string> UploadOptimizedAsync(IFormFile file)
        {
            BlobContainerClient _containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainers.QuestionImage.ToString());
            string fileName = $"{Guid.NewGuid()}.webp"; // WebP je nejmodernější a nejmenší formát
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
            return fileName;
        }

        public IFormFile ConvertBase64ToIFormFile(string base64String, string fileName)
        {
            // 1. Očištění Base64 od hlavičky (pokud ji obsahuje)
            var base64Parts = base64String.Split(',');
            var pureBase64 = base64Parts.Length > 1 ? base64Parts[1] : base64Parts[0];

            // 2. Převod na byte array
            byte[] fileBytes = Convert.FromBase64String(pureBase64);

            // 3. Vytvoření streamu
            var stream = new MemoryStream(fileBytes);

            // 4. Vytvoření instance FormFile
            // Parametry: stream, offset, délka, název v poli, název souboru
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg" // Můžete detekovat podle přípony
            };

            return formFile;
        }
    }
}
