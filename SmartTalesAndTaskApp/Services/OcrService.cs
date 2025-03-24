using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace SmartTalesAndTaskApp.Services
{
    public class OcrService
    {
        private readonly HttpClient _client;
        private readonly string _tessDataPath;

        private readonly string _apiUrl = "https://localhost:7079/api/OCR/extract-text"; // Store in appsettings.json for better practice

        public OcrService()
        {
            _client = new HttpClient();
        }

       
        

        public async Task<string> ExtractTextFromImageAsync(string imagePath)
        {
            try
            {
                var file = File.OpenRead(imagePath);
                if (file == null || file.Length == 0)
                {
                    return "❌ Failed to read the image.";
                }

                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);

                using var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(memoryStream.ToArray());
                content.Add(imageContent, "image", "image.jpg");

                // Call .NET Web API for OCR
                var response = await _client.PostAsync(_apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    return $"❌ Error: {response.ReasonPhrase}";
                   
                }

                
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ OCR Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }



    }
}
