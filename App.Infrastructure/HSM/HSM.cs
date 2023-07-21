using App.Core.Interfaces;
using App.Infrastructure.FirmaElock;
using App.Util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace App.Infrastructure.HSM
{
    public class DTOFirmaRequest
    {
        public string inputfolder { get; set; }
        public string page_number { get; set; } = "0";
        public string sign_location { get; set; } = "BOTTOM_EDGE_CENTER";
        public string username { get; set; } = "sachin";
        public string password { get; set; } = "sachin@123";
    }

    public class DTOFirmaResponse
    {
        public string Status { get; set; }
        public string SignedBase64EncodedString { get; set; }
    }

    public class Hsm : IHsm
    {
        public byte[] SignWSDL(byte[] contenido, List<string> firmantes, int documentoId, string folio, string urlVerificacion, byte[] QR)
        {
            //validaciones
            if (documentoId == 0)
                throw new System.Exception("No se especificó el código de verificación del documento.");
            if (contenido == null)
                throw new System.Exception("No se especificó el contenido del documento.");
            if (!firmantes.Any())
                throw new System.Exception("Debe especificar al menos un firmante.");
            if (urlVerificacion.IsNullOrWhiteSpace())
                throw new System.Exception("No se especificó la url de verificación del documento.");
            if (QR == null)
                throw new System.Exception("No se especificó el código QR.");

            using (MemoryStream ms = new MemoryStream())
            using (var reader = new PdfReader(contenido))
            using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
            {
                //agregar folio
                if (!folio.IsNullOrWhiteSpace())
                {
                    try
                    {
                        //obtener informacion de la primera pagina
                        var pagesize = reader.GetPageSize(1);
                        var pdfContentFirstPage = stamper.GetOverContent(1);

                        //estampa de folio
                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);

                        //estampa de fecha
                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 182, 0);
                    }
                    catch (System.Exception ex)
                    {
                        throw new System.Exception("Error al insertar folio en el documento: " + ex.Message);
                    }
                }

                //agregar tabla de verificacion
                try
                {
                    var img = Image.GetInstance(QR);
                    var fontStandard = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.DARK_GRAY);
                    var fontBold = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY);
                    var pdfContentLastPage = stamper.GetOverContent(reader.NumberOfPages);
                    var table = new PdfPTable(3) { HorizontalAlignment = Element.ALIGN_CENTER, WidthPercentage = 85 };

                    table.TotalWidth = 480f;
                    table.SetWidths(new[] { 8f, 25f, 6f });
                    table.AddCell(new PdfPCell(new Phrase("Información de firma electrónica:", fontBold)) { Colspan = 2, BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell() { Rowspan = 5 }).AddElement(img);
                    table.AddCell(new PdfPCell(new Phrase("Firmantes", fontBold)));
                    table.AddCell(new PdfPCell(new Phrase(string.Join(", ", firmantes), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Fecha de firma", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Código de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase(documentoId.ToString(), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("URL de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase(urlVerificacion, fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                    table.WriteSelectedRows(0, -1, 43, 100, pdfContentLastPage);
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Error al insertar tabla de validación de firma electrónica: " + ex.Message);
                }

                stamper.Close();
                contenido = ms.ToArray();
            }

            //firma documento
            var documentoParaFirmar = contenido;
            using (var ws = new SignFileImplClient())
            {
                foreach (var firmante in firmantes)
                {
                    try
                    {
                        //ejecutar llamada a servicio
                        var respuesta = ws.SignFile(documentoParaFirmar, firmante.Trim(), "BOTTOM_EDGE_CENTER", "0", null, null, 0, 0, 0, 0);

                        //sin respuesta 
                        if (respuesta == null)
                            throw new System.Exception("El servicio externo de firma electrónica WSDL no retornó respuesta");

                        //respuesta con error
                        if (respuesta != null && respuesta.status.Contains("FAIL"))
                            throw new System.Exception("El servicio externo de firma electrónica WSDL retornó falla");

                        //firma ok
                        documentoParaFirmar = respuesta.message;

                    }
                    catch (System.Exception ex)
                    {
                        throw new System.Exception("Error al firmar documento: " + ex.Message);
                    }
                }
            }

            return documentoParaFirmar;
        }
        public byte[] SignREST(byte[] contenido, List<string> firmantes, int documentoId, string folio, string urlVerificacion, byte[] QR)
        {
            //validaciones
            if (documentoId == 0)
                throw new System.Exception("No se especificó el código de verificación del documento.");
            if (contenido == null)
                throw new System.Exception("No se especificó el contenido del documento.");
            if (!firmantes.Any())
                throw new System.Exception("Debe especificar al menos un firmante.");
            if (urlVerificacion.IsNullOrWhiteSpace())
                throw new System.Exception("No se especificó la url de verificación del documento.");
            if (QR == null)
                throw new System.Exception("No se especificó el código QR.");

            using (MemoryStream ms = new MemoryStream())
            using (var reader = new PdfReader(contenido))
            using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
            {
                //agregar folio
                if (!folio.IsNullOrWhiteSpace())
                {
                    try
                    {
                        //obtener informacion de la primera pagina
                        var pagesize = reader.GetPageSize(1);
                        var pdfContentFirstPage = stamper.GetOverContent(1);

                        //estampa de folio
                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);

                        //estampa de fecha
                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 182, 0);
                    }
                    catch (System.Exception ex)
                    {
                        throw new System.Exception("Error al insertar folio en el documento: " + ex.Message);
                    }
                }

                //agregar tabla de verificacion
                try
                {
                    var img = Image.GetInstance(QR);
                    var fontStandard = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.DARK_GRAY);
                    var fontBold = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY);
                    var pdfContentLastPage = stamper.GetOverContent(reader.NumberOfPages);
                    var table = new PdfPTable(3) { HorizontalAlignment = Element.ALIGN_CENTER, WidthPercentage = 100 };

                    table.TotalWidth = 520f;
                    table.SetWidths(new[] { 8f, 25f, 6f });
                    table.AddCell(new PdfPCell(new Phrase("Información de firma electrónica:", fontBold)) { Colspan = 2, BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell() { Rowspan = 5 }).AddElement(img);
                    table.AddCell(new PdfPCell(new Phrase("Firmantes", fontBold)));
                    table.AddCell(new PdfPCell(new Phrase(string.Join(", ", firmantes), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Fecha de firma", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Código de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase(documentoId.ToString(), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("URL de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                    table.AddCell(new PdfPCell(new Phrase(urlVerificacion, fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                    table.WriteSelectedRows(0, -1, 43, 100, pdfContentLastPage);
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Error al insertar tabla de validación de firma electrónica: " + ex.Message);
                }

                stamper.Close();
                contenido = ms.ToArray();
            }

            //firma documento
            var documentoParaFirmar = contenido;

            var client = new RestClient("https://sdf.economia.cl/digitalsign2/elock/sign/signbase64");
            using (var ws = new SignFileImplClient())
            {
                foreach (var firmante in firmantes)
                {
                    var firmaRequest = new DTOFirmaRequest
                    {
                        inputfolder = Convert.ToBase64String(documentoParaFirmar)
                    };

                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("inputfolder", firmaRequest.inputfolder);
                    request.AddParameter("page_number", firmaRequest.page_number);
                    request.AddParameter("sign_location", firmaRequest.sign_location);
                    request.AddParameter("signer_name", firmante);
                    request.AddParameter("username", firmaRequest.username);
                    request.AddParameter("password", firmaRequest.password);

                    var response = client.Execute(request);
                    if (!response.IsSuccessful)
                        throw new System.Exception("El servicio externo de firma electrónica REST retornó falla:" + response.ErrorMessage);

                    var dtoResponse = JsonConvert.DeserializeObject<DTOFirmaResponse>(response.Content);
                    if (dtoResponse == null)
                        throw new System.Exception("El servicio externo de firma electrónica REST no retornó respuesta");

                    documentoParaFirmar = Convert.FromBase64String(dtoResponse.SignedBase64EncodedString);
                }
            }

            return documentoParaFirmar;
        }
    }
}