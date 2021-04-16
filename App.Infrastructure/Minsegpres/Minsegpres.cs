using System;
using System.Linq;
using App.Core.Interfaces;
using System.Text;
using System.Security.Cryptography;
using RestSharp;
using App.Model.Core;
using System.Net;
using App.Model.FirmaDocumentoGenerico;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Image = iTextSharp.text.Image;

namespace App.Infrastructure.Minsegpres
{
    public class Minsegpres : IMinsegpres
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IFolio _folio;
        protected readonly IFile _file;

        public Minsegpres(IGestionProcesos repository, IFolio folio, IFile file)
        {
            _repository = repository;
            _folio = folio;
            _file = file;

        }

        public byte[] SignConOtp(byte[] documento, string OTP, int id, string Run, string Nombre, bool TipoDocumento, int DocumentoId)
        {
            byte[] binario = null;

            string fileContent = Convert.ToBase64String(documento);

            DateTime issuedAt = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(30);

            var headerNew = new
            {
                alg = "HS256",
                typ = "JWT"
            };

            var headerPart = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(headerNew));

            var payloadNew = new
            {
                entity = "Subsecretaría de Economía y Empresas de Menor Tamaño",
                run = Run,
                expiration = expires,
                purpose = "Propósito General"
            };

            var payloadPart = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(payloadNew));

            var secret = "8d7a6d0fea8541b99b0dce110fd0d077";
            var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{headerPart}.{payloadPart}"));
            var hash = Base64UrlEncoder.Encode(hashBytes);

            var jwt = $"{headerPart}.{payloadPart}.{hash}";


            //Testing
            var client = new RestClient("https://api.firma.cert.digital.gob.cl/firma/v2/files/tickets");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("OTP", OTP);
            request.AddHeader("Content-Type", "application/json");
            //Testing
            request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"012edbcc-7eb7-4043-8c16-a7702f1ffc40\",\r\n\"token\":\r\n\"" + jwt + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + fileContent + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            Root obj = JsonConvert.DeserializeObject<Root>(response.Content);


            if (response.StatusCode == HttpStatusCode.OK)
                foreach (var file in obj.files)
                    binario = Convert.FromBase64String(file.content);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(string.Join(",", obj.files.Select(q=>q.status)));

            return binario;
        }

        public byte[] SignSinOtp(byte[] documento, int id, string Run, string Nombre, bool TipoDocumento, int DocumentoId)
        {

            byte[] binario = null;

            string fileContent = Convert.ToBase64String(documento);

            DateTime issuedAt = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(30);

            var headerNew = new
            {
                alg = "HS256",
                typ = "JWT"
            };

            var headerPart = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(headerNew));

            var payloadNew = new
            {
                entity = "Subsecretaría de Economía y Empresas de Menor Tamaño",
                run = Run,
                expiration = expires,
                purpose = "Desatendido"
            };

            var payloadPart = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(payloadNew));

            var secret = "8d7a6d0fea8541b99b0dce110fd0d077";
            var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{headerPart}.{payloadPart}"));
            var hash = Base64UrlEncoder.Encode(hashBytes);

            var jwt = $"{headerPart}.{payloadPart}.{hash}";

            //Testing
            var client = new RestClient("https://api.firma.cert.digital.gob.cl/firma/v2/files/tickets");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            //request.AddHeader("OTP", OTP);
            request.AddHeader("Content-Type", "application/json");
            //Testing
            request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"012edbcc-7eb7-4043-8c16-a7702f1ffc40\",\r\n\"token\":\r\n\"" + jwt + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + fileContent + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            Root obj = JsonConvert.DeserializeObject<Root>(response.Content);


            if (response.StatusCode == HttpStatusCode.OK)
                foreach (var file in obj.files)
                    binario = Convert.FromBase64String(file.content);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(string.Join(",", obj.files.Select(q => q.status)));

            return binario;
        }
    }
}
