using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fitnesclubplus.Controllers
{
    public class AiController : Controller
    {
        private readonly HttpClient _httpClient;

        public AiController()
        {
            _httpClient = new HttpClient();
            // Hem Gemini hem de kendi bilgisayarındaki yapay zeka çalışacağı için süreyi uzun tutalım.
            _httpClient.Timeout = TimeSpan.FromSeconds(180);
        }

        [HttpPost]
        public async Task<IActionResult> GetPlan(string age, string weight, string height, string goal, IFormFile? userPhoto)
        {
            // --- AYARLAR ---
            string googleApiKey = "AIzaSyCCfq3CfdPM5PYXlPcA93ub42xlJSXV89g";

            // DÜZELTME BURADA: Senin çalışan modelin 'gemini-2.5-flash'
            string geminiModel = "gemini-2.5-flash";

            // Stability Matrix Adresi (Localhost)
            string localSdUrl = "http://127.0.0.1:7860/sdapi/v1/txt2img";


            // ==================================================================================
            // ADIM 1: GOOGLE GEMINI İLE METİN PLANI OLUŞTURMA
            // ==================================================================================
            string geminiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{geminiModel}:generateContent?key={googleApiKey}";

            string base64ImageForGemini = "";
            if (userPhoto != null && userPhoto.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await userPhoto.CopyToAsync(ms);
                    base64ImageForGemini = Convert.ToBase64String(ms.ToArray());
                }
            }

            var geminiParts = new List<object>();
            string promptText = $"Ben {age} yaşında, {weight} kilo ve {height} cm boyunda biriyim. " +
                                $"Hedefim: {goal}. " +
                                (string.IsNullOrEmpty(base64ImageForGemini) ? "" : "Eklediğim vücut fotoğrafımı da analiz ederek vücut tipimi belirle. ") +
                                $"Bana maddeler halinde kısa, uygulanabilir bir beslenme ve egzersiz programı yaz. " +
                                $"Cevabı HTML formatında (<ul>, <li>, <b> etiketleri ile) ver.";

            geminiParts.Add(new { text = promptText });

            if (!string.IsNullOrEmpty(base64ImageForGemini))
            {
                geminiParts.Add(new { inline_data = new { mime_type = userPhoto.ContentType, data = base64ImageForGemini } });
            }

            var geminiBody = new { contents = new[] { new { parts = geminiParts } } };
            string aiText = "";

            try
            {
                var response = await _httpClient.PostAsync(geminiUrl, new StringContent(JsonSerializer.Serialize(geminiBody), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var geminiResp = JsonSerializer.Deserialize<GeminiResponse>(await response.Content.ReadAsStringAsync());
                    aiText = geminiResp?.Candidates?[0]?.Content?.Parts?[0]?.Text;
                }
                else
                {
                    // Hata olursa sebebini görelim
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Google Hatası: {errorMsg}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bağlantı Hatası: " + ex.Message });
            }

            if (string.IsNullOrEmpty(aiText)) return Json(new { success = false, message = "Diyet planı oluşturulamadı (Cevap boş)." });


            // ==================================================================================
            // ADIM 2: STABILITY MATRIX İLE GÖRSEL OLUŞTURMA
            // ==================================================================================
            string generatedImageUrl = "";
            string imageTitle = goal.Contains("Kilo") ? "Hedeflenen Fit Görünüm" : "Hedeflenen Atletik Yapı";
            string sdBadge = "<span class='badge bg-success ms-2'>Yapay Zeka (Local)</span>";

            try
            {
                // Prompt Hazırlığı (İngilizce olmak zorunda)
                string bodyType = goal.Contains("Kilo") ? "lean, toned body, fit, abs" : "muscular, athletic build, strong, big arms";
                string genderHint = "person"; // Fotoğraf analizi olmadığı için genel kişi diyoruz

                string sdPrompt = $"A high quality professional photo of a {age} year old {genderHint} with a {bodyType}, working out in a gym, cinematic lighting, 8k, detailed.";
                string negativePrompt = "ugly, deformed, blurry, bad anatomy, extra limbs, watermark, text";

                var sdBody = new
                {
                    prompt = sdPrompt,
                    negative_prompt = negativePrompt,
                    steps = 20,
                    width = 512,
                    height = 512, // Kare format daha hızlı üretilir
                    cfg_scale = 7,
                    sampler_name = "Euler a",
                    batch_size = 1
                };

                var sdResponse = await _httpClient.PostAsync(localSdUrl, new StringContent(JsonSerializer.Serialize(sdBody), Encoding.UTF8, "application/json"));

                if (sdResponse.IsSuccessStatusCode)
                {
                    var sdRespString = await sdResponse.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(sdRespString))
                    {
                        // Stability Matrix çıktısını alıyoruz
                        if (doc.RootElement.TryGetProperty("images", out var imagesElement))
                        {
                            string base64SdImage = imagesElement[0].GetString();
                            generatedImageUrl = $"data:image/png;base64,{base64SdImage}";
                        }
                    }
                }
                else
                {
                    // Local AI kapalıysa veya hata verdiyse stok foto
                    imageTitle += " (Stok)";
                    sdBadge = "<span class='badge bg-secondary ms-2'>AI Kapalı/Hata</span>";
                }
            }
            catch
            {
                // Bağlantı yoksa stok foto
                imageTitle += " (Stok)";
                sdBadge = "<span class='badge bg-secondary ms-2'>Bağlantı Yok</span>";
            }

            // Görsel oluşmadıysa stok görsel kullan
            if (string.IsNullOrEmpty(generatedImageUrl))
            {
                generatedImageUrl = goal.Contains("Kilo") || goal.Contains("Yağ")
                   ? "https://images.unsplash.com/photo-1571019614242-c5c5dee9f50b?w=600&h=400&fit=crop"
                   : "https://images.unsplash.com/photo-1581009146145-b5ef050c2e1e?w=600&h=400&fit=crop";
            }

            string photoMessage = string.IsNullOrEmpty(base64ImageForGemini) ? "" : "<div class='alert alert-info small py-2 mb-2'><i class='bi bi-camera-fill'></i> Fotoğrafın analiz edildi.</div>";

            string finalHtml = $@"
                {photoMessage}
                <div class='card mb-4 border-0 shadow-sm'>
                    <div class='row g-0'>
                        <div class='col-md-5 bg-light d-flex align-items-center justify-content-center'>
                            <img src='{generatedImageUrl}' class='img-fluid rounded-start w-100' style='object-fit: cover; max-height: 400px;' alt='Hedef Vücut'>
                        </div>
                        <div class='col-md-7 d-flex align-items-center'>
                            <div class='card-body'>
                                <h5 class='card-title text-success fw-bold'>
                                    <i class='bi bi-trophy-fill'></i> {imageTitle} {sdBadge}
                                </h5>
                                <p class='card-text text-muted small mb-1'>Senin için özel olarak oluşturulan tahmini hedef formu.</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class='ai-content p-3 bg-white rounded border'>
                    {aiText}
                </div>";

            return Json(new { success = true, data = finalHtml });
        }

        // Modeller
        public class GeminiResponse { [JsonPropertyName("candidates")] public Candidate[] Candidates { get; set; } }
        public class Candidate { [JsonPropertyName("content")] public Content Content { get; set; } }
        public class Content { [JsonPropertyName("parts")] public Part[] Parts { get; set; } }
        public class Part { [JsonPropertyName("text")] public string Text { get; set; } }
    }
}