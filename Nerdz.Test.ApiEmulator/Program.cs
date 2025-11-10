using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace Nerdz.Test.ApiEmulator
{
    public class Program
    {
        private static IConfiguration _config;
        private static Process _emulatorProcess;

        public static async Task Main(string[] args)
        {
            Console.Title = "Nerdz - Orquestrador de Dev";

            try
            {
                // PASSO 1: INICIAR O EMULADOR
                if (!await StartFirebaseEmulator())
                {
                    Console.ReadLine();
                    return;
                }

                // PASSO 2: CARREGAR CONFIG
                _config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                // PASSO 3: LÓGICA DE LOGIN OU CRIAÇÃO
                await GetOrCreateUserToken();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nOcorreu um erro inesperado: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                Console.WriteLine("\n=======================================================");
                Console.WriteLine("✅ Os emuladores (Auth, Firestore) estão rodando.");
                Console.WriteLine("✅ A API Nerdz.Api está (ou deveria estar) rodando.");
                Console.WriteLine("Pressione Enter neste console para FECHAR e DERRUBAR OS EMULADORES.");
                Console.ReadLine();

                StopFirebaseEmulator();
            }
        }

        /// <summary>
        /// Tenta logar. Se o usuário não existir, tenta criá-lo.
        /// </summary>
        private static async Task GetOrCreateUserToken()
        {
            // Busca as URLs e o usuário do appsettings
            var signInUrl = _config["TestConfig:AuthEmulatorSignInUrl"];
            var signUpUrl = _config["TestConfig:AuthEmulatorSignUpUrl"];
            var loginPayload = new FirebaseAuthRequest
            {
                Email = _config["TestConfig:TestUser:email"],
                Password = _config["TestConfig:TestUser:password"],
                ReturnSecureToken = _config.GetValue<bool>("TestConfig:TestUser:returnSecureToken")
            };

            using (var httpClient = new HttpClient())
            {
                Console.WriteLine($"Tentando login como: {loginPayload.Email}...");
                var response = await httpClient.PostAsJsonAsync(signInUrl, loginPayload);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Login realizado com sucesso!");
                    await PrintTokenFromResponse(response);
                    return;
                }
                var errorJson = await response.Content.ReadAsStringAsync();

                if (errorJson.Contains("EMAIL_NOT_FOUND"))
                {
                    Console.WriteLine("Usuário não encontrado. Tentando criar a conta...");
                    var signUpResponse = await httpClient.PostAsJsonAsync(signUpUrl, loginPayload);

                    if (signUpResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Conta criada com sucesso!");
                        await PrintTokenFromResponse(signUpResponse);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Falha ao TENTAR CRIAR a conta:");
                        Console.WriteLine(await signUpResponse.Content.ReadAsStringAsync());
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Falha ao TENTAR LOGAR (Usuário existe, mas algo está errado):");
                    Console.WriteLine(errorJson);
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Imprime o token de uma resposta de sucesso.
        /// </summary>
        private static async Task PrintTokenFromResponse(HttpResponseMessage response)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
            if (authResponse == null || string.IsNullOrEmpty(authResponse.IdToken))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("A resposta não continha um IdToken (JWT)!");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SEU TOKEN JWT (Copie para o Postman)");
            Console.ResetColor();
            Console.WriteLine(authResponse.IdToken);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nFIM DO TOKEN");
            Console.WriteLine($"\nUID do usuário: {authResponse.LocalId}");
            Console.ResetColor();
        }

        // MÉTODOS DE CONTROLE DO EMULADOR

        private static async Task<bool> StartFirebaseEmulator()
        {
            Console.WriteLine("Iniciando emuladores do Firebase (auth, firestore)...");
            try
            {
                _emulatorProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/C firebase emulators:start --only auth,firestore",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                _emulatorProcess.Start();
                Console.WriteLine("Emuladores iniciados em segundo plano.");

                await WaitForEmulatorAsync(9099, timeoutSeconds: 20);
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERRO!");
                Console.WriteLine("Não foi possível iniciar ou conectar aos emuladores.");
                Console.WriteLine($"Detalhe: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }

        private static async Task WaitForEmulatorAsync(int port, int timeoutSeconds)
        {
            Console.WriteLine($"Aguardando emulador na porta {port} ficar online (timeout: {timeoutSeconds}s)...");
            var sw = Stopwatch.StartNew();

            while (sw.Elapsed.TotalSeconds < timeoutSeconds)
            {
                try
                {
                    using (var tcpClient = new TcpClient())
                    {
                        await tcpClient.ConnectAsync("localhost", port);
                    }
                    sw.Stop();
                    Console.WriteLine($"Emulador na porta {port} está online! (Levou {sw.Elapsed.TotalSeconds:F1}s)");
                    return;
                }
                catch (SocketException) { await Task.Delay(1000); }
            }
            throw new TimeoutException($"Timeout: Emulador na porta {port} não respondeu após {timeoutSeconds} segundos.");
        }

        private static void StopFirebaseEmulator()
        {
            if (_emulatorProcess != null && !_emulatorProcess.HasExited)
            {
                Console.WriteLine("Parando os emuladores do Firebase...");
                try
                {
                    _emulatorProcess.Kill(true);
                    _emulatorProcess.Dispose();
                    Console.WriteLine("Emuladores parados.");
                }
                catch (Exception ex) { Console.WriteLine($"Erro ao parar emuladores: {ex.Message}"); }
            }
        }
    }

    // Classes DTO (Modelos) (DTO significa Data Transfer Object)
    internal class FirebaseAuthRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("returnSecureToken")]
        public bool ReturnSecureToken { get; set; }
    }

    internal class FirebaseAuthResponse
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }
        [JsonPropertyName("localId")]
        public string LocalId { get; set; }
    }
}