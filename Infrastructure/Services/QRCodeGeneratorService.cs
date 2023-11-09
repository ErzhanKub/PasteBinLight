using Domain.IServices;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;


namespace Infrastructure.Services
{
    public class QRCodeGeneratorService : IQRCodeGeneratorService
    {
        private readonly QRCodeGenerator _qrCodeGenerator;

        public QRCodeGeneratorService()
        {
            _qrCodeGenerator = new QRCodeGenerator();
        }

        public string GeneratorQRCode(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text), "Text cannot be null or empty.");

            var data = _qrCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var bitmap = new BitmapByteQRCode(data);
            var qrCodeBytes = bitmap.GetGraphic(20);

            using var ms = new MemoryStream(qrCodeBytes);
            var qrCodeImage = new Bitmap(ms);

            using var msBase64 = new MemoryStream();
            qrCodeImage.Save(msBase64, ImageFormat.Png);
            var base64String = Convert.ToBase64String(msBase64.ToArray());

            return base64String;
        }

    }
}
