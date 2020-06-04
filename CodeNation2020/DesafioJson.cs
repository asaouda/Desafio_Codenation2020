using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CodeNation2020
{
    public class DesafioJson
    {
        [JsonPropertyName("numero_casas")]
        public int NumeroCasas { get; set; }
        public string token { get; set; }
        public string cifrado { get; set; }
        public string decifrado { get; set; }
        public string resumo_Criptografico { get; set; }

        static char[] _alfabeto = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        static void CreateFileJson(string informacaoArquivo)
        {
            string pathArquivo = @"C:\Users\aline\source\repos\CodeNation2020\";
            string fileName = "answer.json";
            pathArquivo = System.IO.Path.Combine(pathArquivo, fileName);
            System.IO.File.WriteAllText(pathArquivo, informacaoArquivo);
        }

        public void GetDesafioWeb()
        {
            var requisicaoWeb = WebRequest.CreateHttp("https://api.codenation.dev/v1/challenge/dev-ps/generate-data?token=9c439d80a2dd930204abb829b7f46d94a2de3183");
            requisicaoWeb.Method = "GET";

            using (var resposta = requisicaoWeb.GetResponse())
            {
                var streamDados = resposta.GetResponseStream();
                StreamReader reader = new StreamReader(streamDados);

                object response = reader.ReadToEnd();

                Console.WriteLine(response.ToString());
                CreateFileJson(response.ToString());

                streamDados.Close();
                resposta.Close();
            }
        }

        public DesafioJson GetInfoFileJson(string path,DesafioJson desafio)
        { 

            using(StreamReader fileJson=new StreamReader(path))
            {

                string infoJson = fileJson.ReadToEnd ();
                desafio = JsonSerializer.Deserialize<DesafioJson>(infoJson);
                token = desafio.token;
                fileJson.Close();
                fileJson.Dispose();             

                DecifrandoTexto(desafio.cifrado ,desafio.NumeroCasas);

                return desafio;
            }
        }

        public void SetInfoFileJson(string path, DesafioJson  desafio )
        {
            using (StreamWriter fileJson = new StreamWriter(path))
            {
                string infoJson = JsonSerializer.Serialize(desafio);
                fileJson.Write(infoJson);
            }
        }
        public void DecifrandoTexto(string textoCriptografado, int qtdeLetrasVoltar)
        {
            NumeroCasas = qtdeLetrasVoltar;
            cifrado = textoCriptografado;

            string textoDecifrado = "";
            var chars = textoCriptografado.ToCharArray();

            for (int index = 0; index < chars.Length; index++)
            {

                char charDescriptografado =  GetLetraDecifrada(chars[index],qtdeLetrasVoltar);
                textoDecifrado = textoDecifrado + charDescriptografado;
            }
            
            
            decifrado = textoDecifrado;
            Console.WriteLine(decifrado);

            GetResumo(decifrado);


        }

        public char GetLetraDecifrada(char letraCifrada,int qtdeLetrasVoltar)
        {
            for (int i = 0; i < 26; i++)
            {
                if (_alfabeto[i] == letraCifrada)
                {
                    int index = i - 10;
                    if (index < 0)
                    {
                        return  GetLetraNegativo(index);
                    }
                    return _alfabeto[index];
                }
            }

            return letraCifrada;
        }

        static char GetLetraNegativo(int index)
        {
            for (int i =_alfabeto.Length ; i >= 0; i--)
            {
                if (index == i-_alfabeto.Length)
                {
                    return _alfabeto[i];
                }
            }

            return '@';
        }

        public void GetResumo(string textoDescriptografado)
        {
            SHA1  hashing = new SHA1CryptoServiceProvider();
            byte[] tmpTexto= ASCIIEncoding.ASCII.GetBytes(textoDescriptografado);
            byte[] hash;
            string resumo;

            hash = hashing.ComputeHash(tmpTexto);
                
            StringBuilder sOutput = new StringBuilder(hash.Length);
            for (int i = 0; i < hash.Length; i++)
            {
                sOutput.Append(hash[i].ToString("X2"));
            }
            resumo= sOutput.ToString();
            
            resumo_Criptografico = resumo.ToLower();
            Console.WriteLine(resumo_Criptografico);
        }
        public void Enviar()
        {
            string fileName = @"C:\Users\aline\source\repos\CodeNation2020\answer.json";
            string url = "https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token=9c439d80a2dd930204abb829b7f46d94a2de3183";
            //string url = "http://localhost:5000/upload";

            using (HttpClient client = new HttpClient())
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            using (FileStream fileStream = System.IO.File.OpenRead(fileName))
            using (StreamContent fileContent = new StreamContent(fileStream))
            {
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data"); ;

                content.Headers.Add("file", "answer");
                content.Add(fileContent, "answer", Path.GetFileName(fileName));
                var result = client.PostAsync(url, content).Result;

                var corpoResposta = result.Content.ReadAsStringAsync().Result;

                result.EnsureSuccessStatusCode();
            }
        }

        public string Upload()
        {
            string filePath = @"C:\Users\aline\source\repos\CodeNation2020\answer.json";
            string url = "https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token=9c439d80a2dd930204abb829b7f46d94a2de3183";


            System.Net.WebClient Client = new System.Net.WebClient();
            Client.Headers.Add("Content-Type", "multipart/form-data");
            Client.Headers.Add("file", "answer");
            byte[] result = Client.UploadFile(url, "POST", filePath);
            string s = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
            return s;
        }

        //public void UploadMultipart()
        //{
        //    string fileName = @"C:\Users\aline\source\repos\CodeNation2020\answer.json";
        //    string url = "https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token=9c439d80a2dd930204abb829b7f46d94a2de3183";
        //    string contentType = "multipart/form-data";
        //    byte[] file = File.ReadAllBytes(fileName);
        //    var webClient = new WebClient();
        //    string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
        //    webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
        //    var fileData = webClient.Encoding.GetString(file);
        //    var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, contentType, fileData);

        //    var nfile = webClient.Encoding.GetBytes(package);

        //    byte[] resp = webClient.UploadData(url, "POST", nfile);
        //}
    }
}
