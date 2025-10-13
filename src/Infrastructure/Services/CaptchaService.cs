using System;
using System.IO;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Application.Dtos.Core;
using Microsoft.AspNetCore.Http;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ContractorBackend.Infrastructure.Services
{
    public class CaptchaService : ICaptchaService
    {
        private readonly ISecurityService _securityService;
        string Key = "77367c445cdb48eb383308d9dbd6a730";

        public CaptchaService(ISecurityService securityService)
        {
            _securityService = securityService;
        }
        public GenerateCaptchaCodeDto GenerateCaptchaCode()
        {
            Random rand = new Random();
            var first = rand.Next(1, 9);
            var second = rand.Next(1, 9);         // rand.Next(10, 99);
            return new GenerateCaptchaCodeDto
            {
                First = first,
                Second = second,
            };
        }
        public CaptchaDto GenerateCaptchaImage(int first, int second)
        {
            try
            {
                byte[] byteArray;
                Random rand = new Random();

                var captcha = $"{first} + {second} = ?";

                string base64String = "";

                var img = new Image<Rgba32>(130, 30);

                img.Mutate(_ => _.BackgroundColor(Color.White));

                var font = SystemFonts.CreateFont("Tahoma", 17, FontStyle.Italic);

                var location = new PointF(10, 5);
                img.Mutate(_ => _.DrawText(captcha, font, Color.DimGray, location));


                int i, r, x, y;
                Color clr;
                clr = Color.Yellow;
                for (i = 1; i <= 3; i++)
                {
                    clr = new Color(new Bgr24(
                            (byte)rand.Next(0, 255),
                            (byte)rand.Next(0, 255),
                            (byte)rand.Next(0, 255)));

                    r = rand.Next(1, 130 / 3);
                    x = rand.Next(0, 130);
                    y = rand.Next(0, 30);
                    IPath polygon = new EllipsePolygon(new PointF(x - 1, y - 1), r);
                    img.Mutate(x => x.Draw(clr, 1f, polygon));
                }

                using (MemoryStream ms = new())
                {
                    img.SaveAsJpeg(ms);
                    byteArray = ms.ToArray();
                }
                base64String = Convert.ToBase64String(byteArray);
                img.Dispose();
                if (!base64String.StartsWith("data:image/jpeg;base64,", StringComparison.OrdinalIgnoreCase))
                {
                    base64String = $"data:image/jpeg;base64,{base64String}";
                }
                var key = _securityService.EncryptString(Key, $"{first}+{second}");
                return new CaptchaDto
                {
                    Key = key,
                    Image = base64String
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public bool ValidateCaptcha(string token, string value)
        //{
        //    var dec = _securityService.DecryptString(Key, token);
        //    dec = Regex.Replace(dec, @"\s+", "");
        //    string operation = dec[1].ToString();

        //    int.TryParse(dec.Split(operation)[0], out int first);
        //    int.TryParse(dec.Split(operation)[1], out int second);

        //    int calculated = 0;

        //    calculated = operation switch
        //    {
        //        "+" => first + second,
        //        _ => throw new NotImplementedException(nameof(operation))
        //    };

        //    return calculated == int.Parse(value)!;
        //}
        public bool ValidateCaptchaCode(string userInputCaptcha, HttpContext httpContext)
        {
            var isValid = userInputCaptcha == httpContext.Session.GetString("CaptchaCode") && userInputCaptcha != null;
            httpContext.Session.Remove("CaptchaCode");

            return isValid;
        }


    }
}
