using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Andloe.Data.Fiscal
{
    public sealed class AlanubeClient
    {
        private readonly SistemaConfigRepository _configRepository;


        public AlanubeClient()
        {
            _configRepository = new SistemaConfigRepository();
        }

        public AlanubeConfigDto GetConfig()
        {
            var cfg = new AlanubeConfigDto
            {
                BaseUrl = (_configRepository.GetValor("ALANUBE_BASE_URL") ?? "").Trim(),
                Token = (_configRepository.GetValor("ALANUBE_TOKEN") ?? "").Trim(),
                Ambiente = (_configRepository.GetValor("ALANUBE_AMBIENTE") ?? "sandbox").Trim(),
                TimeoutSegundos = ParseInt(_configRepository.GetValor("ALANUBE_TIMEOUT_SEGUNDOS"), 60)
            };

            return cfg;
        }

        public AlanubeEmitResponseDto EmitirFactura31(string requestJson)
    => Post("fiscal-invoices", requestJson);

        public AlanubeEmitResponseDto EmitirFactura32(string requestJson)
            => Post("invoices", requestJson);

        public AlanubeEmitResponseDto EmitirFactura45(string requestJson)
            => Post("gubernamentals", requestJson);

        public AlanubeStatusResponseDto ConsultarFactura31(string trackOrId)
            => Get($"fiscal-invoices/{Uri.EscapeDataString((trackOrId ?? "").Trim())}");

        public AlanubeStatusResponseDto ConsultarFactura32(string trackOrId)
            => Get($"invoices/{Uri.EscapeDataString((trackOrId ?? "").Trim())}");

        public AlanubeStatusResponseDto ConsultarFactura45(string trackOrId)
            => Get($"gubernamentals/{Uri.EscapeDataString((trackOrId ?? "").Trim())}");

       
        private AlanubeEmitResponseDto Post(string endpoint, string requestJson)
        {
            var cfg = GetConfig();
            ValidarConfig(cfg);

            using var client = BuildClient(cfg);
            using var content = new StringContent(requestJson ?? "{}", Encoding.UTF8, "application/json");
            using var response = client.PostAsync(endpoint, content).GetAwaiter().GetResult();

            var raw = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Alanube POST {endpoint} devolvió {(int)response.StatusCode}: {raw}");
            }

            return ParseEmitResponse(raw);
        }

        private AlanubeStatusResponseDto Get(string endpoint)
        {
            var cfg = GetConfig();
            ValidarConfig(cfg);

            using var client = BuildClient(cfg);
            using var response = client.GetAsync(endpoint).GetAwaiter().GetResult();

            var raw = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Alanube GET {endpoint} devolvió {(int)response.StatusCode}: {raw}");
            }

            return ParseStatusResponse(raw);
        }

        private static HttpClient BuildClient(AlanubeConfigDto cfg)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(NormalizaBaseUrl(cfg.BaseUrl)),
                Timeout = TimeSpan.FromSeconds(cfg.TimeoutSegundos <= 0 ? 60 : cfg.TimeoutSegundos)
            };

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", cfg.Token);

            return client;
        }

        private static string NormalizaBaseUrl(string baseUrl)
        {
            var txt = (baseUrl ?? "").Trim();
            if (txt.Length == 0)
                throw new InvalidOperationException("ALANUBE_BASE_URL no está configurado.");

            return txt.EndsWith("/") ? txt : txt + "/";
        }
               
       

        private static void ValidarConfig(AlanubeConfigDto cfg)
        {
            if (!cfg.IsValid)
                throw new InvalidOperationException(
                    "Configuración de Alanube incompleta. Verifica ALANUBE_BASE_URL y ALANUBE_TOKEN.");
        }

        private static int ParseInt(string? value, int defaultValue)
            => int.TryParse((value ?? "").Trim(), out var n) ? n : defaultValue;

        private static AlanubeEmitResponseDto ParseEmitResponse(string raw)
        {
            var dto = new AlanubeEmitResponseDto { RawJson = raw };

            if (string.IsNullOrWhiteSpace(raw))
                return dto;

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            dto.Id = TryGetString(root, "id");
            dto.TrackId = TryGetString(root, "trackId");
            dto.Status = TryGetString(root, "status");
            dto.LegalStatus = TryGetString(root, "legalStatus");
            dto.Message = TryGetString(root, "message")
                       ?? TryGetString(root, "detail")
                       ?? TryGetString(root, "error");

            return dto;
        }

        private static AlanubeStatusResponseDto ParseStatusResponse(string raw)
        {
            var dto = new AlanubeStatusResponseDto { RawJson = raw };

            if (string.IsNullOrWhiteSpace(raw))
                return dto;

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            dto.Id = TryGetString(root, "id");
            dto.TrackId = TryGetString(root, "trackId");
            dto.Status = TryGetString(root, "status");
            dto.LegalStatus = TryGetString(root, "legalStatus");
            dto.Code = TryGetString(root, "code");
            dto.Message = TryGetString(root, "message")
                       ?? TryGetString(root, "detail")
                       ?? TryGetString(root, "error");

            return dto;
        }

        private static string? TryGetString(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var value))
                return null;

            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => value.ToString()
            };
        }
    }
}