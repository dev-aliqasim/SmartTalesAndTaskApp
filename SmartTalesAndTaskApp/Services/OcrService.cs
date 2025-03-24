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
        //private readonly string _tessDataPath;
        //public  OcrService()
        //{
        //    _tessDataPath = Path.Combine(FileSystem.AppDataDirectory, "tessdata");
        //    CopyTessdataIfNotExists();
        //}

        //private void CopyTessdataIfNotExists()
        //{
        //    var tessFile = Path.Combine(_tessDataPath, "eng.traineddata");
        //    if (!File.Exists(tessFile))
        //    {
        //        Directory.CreateDirectory(_tessDataPath);
        //        var source = FileSystem.OpenAppPackageFileAsync("eng.traineddata").Result;
        //        using var stream = File.Create(tessFile);
        //        source.CopyTo(stream);
        //    }
        //}

        //public async Task<string> ExtractTextFromImageAsync(string imagePath)
        //{
        //    try
        //    {
        //        using var engine = new TesseractEngine(_tessDataPath, "eng", EngineMode.Default);
        //        using var image = Pix.LoadFromFile(imagePath);
        //        using var page = engine.Process(image);
        //        return page.GetText();
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Error: {ex.Message}";
        //    }
        //}

        private readonly string _tessDataPath;

        public OcrService()
        {
            _tessDataPath = Path.Combine(FileSystem.AppDataDirectory, "tessdata");
            Task.Run(CopyTessdataIfNotExists).Wait(); // Ensure tessdata exists before using OCR
        }

        private async Task CopyTessdataIfNotExists()
        {
            try
            {
                string tessFile = Path.Combine(_tessDataPath, "eng.traineddata");

                if (!File.Exists(tessFile))
                {
                    Directory.CreateDirectory(_tessDataPath); // Ensure directory exists

                    using var sourceStream = await FileSystem.OpenAppPackageFileAsync("eng.traineddata");
                    using var destinationStream = File.Create(tessFile);
                    await sourceStream.CopyToAsync(destinationStream);

                    //Console.WriteLine($"✅ Tesseract data copied to: {tessFile}");
                }
                else
                {
                    //Console.WriteLine("✅ Tesseract data already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error copying Tesseract data: {ex.Message}");
            }
        }

        public async Task<string> ExtractTextFromImageAsync(string imagePath)
        {
            try
            {
                using var engine = new TesseractEngine(_tessDataPath, "eng", EngineMode.Default);
                using var image = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(image);

                string extractedText = page.GetText();
                Console.WriteLine($"✅ Extracted Text: {extractedText}");

                return extractedText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ OCR Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }



    }
}
