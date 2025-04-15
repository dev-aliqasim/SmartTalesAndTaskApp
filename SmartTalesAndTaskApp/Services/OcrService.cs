using Plugin.Maui.Audio;
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

        private readonly string _baseApiUrl;
        private readonly string _apiUrl; // Store in appsettings.json for better practice

        private readonly IAudioManager _audio;
        public OcrService(IAudioManager audio)
        {
            _audio = audio;
            _client = new HttpClient();

            // Detect platform and set Base API URL
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                _baseApiUrl = "http://10.0.2.2:5296"; // Android Emulator (unsafe due to http)
                //_baseApiUrl = "http://192.168.1.8:5296"; // (ToDo:) Android Device on Same Internet Connection then use computer ipv4 address
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                _baseApiUrl = "https://localhost:7079"; // iOS Simulator
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                _baseApiUrl = "https://localhost:7079"; // Windows
            }
            else
            {
                _baseApiUrl = "https://192.168.1.100:7079"; // Physical Devices (LAN IP)
            }

            //_apiUrl = _baseApiUrl + "/api/OCR/extract-text";
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


                var endpoint = _baseApiUrl + "/api/OCR/extract-text";

                // Call .NET Web API for OCR
                var response = await _client.PostAsync(endpoint, content);

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

        public async Task ConvertImageToAudio(string imagePath)
        {
            try
            {
                var file = File.OpenRead(imagePath);
                if (file == null || file.Length == 0)
                {
                    //return "❌ Failed to read the image.";
                }

                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);

                using var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(memoryStream.ToArray());
                content.Add(imageContent, "image", "image.jpg");


                var endpoint = _baseApiUrl + "/api/OCR/image-to-speech";

                // Call .NET Web API for OCR
                var response = await _client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                {
                    //return $"❌ Error: {response.ReasonPhrase}";

                }


                var audioStream = await response.Content.ReadAsStreamAsync();
                var player = _audio.CreatePlayer(audioStream);
                player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ OCR Error: {ex.Message}");
                //return $"Error: {ex.Message}";
            }
        }



    }
}
