using Domain.IServices;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

namespace Infrastructure.Services
{
    // QR code generator service class that implements the IQRCodeGeneratorService interface
    public sealed class QRCodeGeneratorService : IQRCodeGeneratorService
    {
        // QR code generator
        private readonly QRCodeGenerator _qrCodeGenerator;

        // Constructor
        public QRCodeGeneratorService()
        {
            _qrCodeGenerator = new QRCodeGenerator();
        }

        // Generate a QR code from a text string
        public string GenerateQRCodeFromText(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentNullException(nameof(text), "Text cannot be null or empty.");

                // Create QR code data
                var data = _qrCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

                // Create bitmap from QR code data
                var bitmap = new BitmapByteQRCode(data);
                var qrCodeBytes = bitmap.GetGraphic(20);

                // Convert bitmap to image
                using var ms = new MemoryStream(qrCodeBytes);
                var qrCodeImage = new Bitmap(ms);

                // Convert image to base64 string
                using var msBase64 = new MemoryStream();
                qrCodeImage.Save(msBase64, ImageFormat.Png);
                var base64String = Convert.ToBase64String(msBase64.ToArray());

                return base64String;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
