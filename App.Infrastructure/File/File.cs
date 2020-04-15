using System.Drawing;
using System.IO;
using System.Linq;
using App.Core.Interfaces;
using QRCoder;
using TikaOnDotNet.TextExtraction;
using Zen.Barcode;

namespace App.Infrastructure.File
{
    public class File: IFile
    {
        public Model.DTO.DTOFileMetadata BynaryToText(byte[] content)
        {
            var data = new Model.DTO.DTOFileMetadata();
            var textExtractor = new TextExtractor();
            var extract = textExtractor.Extract(content);

            data.Text = !string.IsNullOrWhiteSpace(extract.Text) ? extract.Text.Trim() : null;
            data.Metadata = extract.Metadata.Any() ? string.Join(";", extract.Metadata) : null;
            data.Type = extract.ContentType;

            return data;
        }

        public byte[] CreateQR(string id)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(id, QRCodeGenerator.ECCLevel.Q);
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
            var image = barcode39.Draw(code, 80);

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imagebyte = ms.ToArray();
            }

            return imagebyte;
        }
    }
}
