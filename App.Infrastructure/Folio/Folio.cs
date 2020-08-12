using App.Core.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace App.Infrastructure.Folio
{
    public class Folio : IFolio
    {
        public List<Model.FirmaDocumento.DTOTipoDocumento> GetTipoDocumento()
        {
            var url = "http://wsfolio.test.economia.cl/api/tipodocumento";
            var client = new RestClient(url);
            var response = client.Execute(new RestRequest());
            var returnValue = JsonConvert.DeserializeObject<List<Model.FirmaDocumento.DTOTipoDocumento>>(response.Content);
            return returnValue;
        }
        public Model.FirmaDocumento.DTOSolicitud GetFolio(string solicitante, string tipoDocumento)
        {
            var solicitud = new Model.FirmaDocumento.DTOSolicitud() { 
                periodo = DateTime.Now.Year.ToString(),
                solicitante = solicitante,
                tipodocumento = tipoDocumento
            };

            var url = "http://wsfolio.test.economia.cl/api/folio";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };

            request.AddJsonBody(solicitud);
            IRestResponse response = client.Execute(request);
            var returnValue = JsonConvert.DeserializeObject<Model.FirmaDocumento.DTOSolicitud>(response.Content);
            return returnValue;
        }
        public Model.FirmaDocumento.DTOSolicitud GetFolioSubsecretaria(string solicitante, string tipoDocumento, string subSecretaria)
        {
            var solicitud = new Model.FirmaDocumento.DTOSolicitud()
            {
                periodo = DateTime.Now.Year.ToString(),
                solicitante = solicitante,
                tipodocumento = tipoDocumento,
                Subsecretaria = subSecretaria
            };

            var url = "http://wsfolio.test.economia.cl/api/folio";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };

            request.AddJsonBody(solicitud);
            IRestResponse response = client.Execute(request);
            var returnValue = JsonConvert.DeserializeObject<Model.FirmaDocumento.DTOSolicitud>(response.Content);
            return returnValue;
        }

    }
}
