using System;
using System.Drawing;
using System.IO;
using System.Linq;
using App.Core.Interfaces;
using iTextSharp.text.pdf;
using QRCoder;
using TikaOnDotNet.TextExtraction;
using Zen.Barcode;

namespace App.Infrastructure.File
{
    public class File : IFile
    {
        public Model.DTO.DTOFileMetadata BynaryToText(byte[] content)
        {
            if (content == null)
                return null;

            var textExtractor = new TextExtractor();
            var data = new Model.DTO.DTOFileMetadata();

            try
            {
                var extract = textExtractor.Extract(content);
                data.Text = !string.IsNullOrWhiteSpace(extract.Text) ? extract.Text.Trim() : null;
                data.Metadata = extract.Metadata.Any() ? string.Join(";", extract.Metadata) : null;
                data.Type = extract.ContentType;
            }
            catch (Exception)
            {
                return null;
            }

            return data;
        }

        public byte[] CreateQR(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    return ImageToByte(qrCodeImage);
                }
            }
        }

        public static byte[] ImageToByte(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public byte[] CreateBarCode(string code)
        {
            byte[] imagebyte;

            var barcode39 = BarcodeDrawFactory.Code39WithoutChecksum;
            var image = barcode39.Draw(code, 35);

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imagebyte = ms.ToArray();
            }

            return imagebyte;
        }

        public byte[] EstamparCodigoEnDocumento(byte[] documento, string text)
        {
            byte[] returnValue;

            if (documento == null)
                throw new System.Exception("Debe especificar el documento");

            using (MemoryStream ms = new MemoryStream())
            using (var reader = new PdfReader(documento))
            using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
            {
                try
                {
                    var pdfContent = stamper.GetOverContent(1);
                    var pagesize = reader.GetPageSize(1);
                    ColumnText.ShowTextAligned(pdfContent, iTextSharp.text.Element.ALIGN_MIDDLE,new iTextSharp.text.Phrase("ID " + text, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE)),10, pagesize.Height - 20, 0);
                    stamper.Close();
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Error al insertar código en documento:" + ex.Message);
                }

                stamper.Close();
                returnValue = ms.ToArray();
            }

            return returnValue;
        }
    }
}