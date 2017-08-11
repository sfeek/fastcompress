using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System;

namespace FASTCompress
{
    public class ImageFunctions
    {
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public void ResampleImage(string inputFileName, string outputFileName, long percent, long minsize)
        {
            try
            {
                // Check to make sure this file is big enough to be worth compressing
                long length = new System.IO.FileInfo(inputFileName).Length;

                if (length < minsize)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFileName));
                    if (File.Exists(inputFileName)) File.Copy(inputFileName, outputFileName);
                    return;
                }

                // Get a bitmap. The using statement ensures objects  
                // are automatically disposed from memory after use.  
                using (Bitmap bmp = new Bitmap(inputFileName))
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID  
                    // for the Quality parameter category.  
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object.  
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, percent);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    Directory.CreateDirectory(Path.GetDirectoryName(outputFileName));

                    bmp.Save(outputFileName, jpgEncoder, myEncoderParameters);
                }
            }
            catch  //(Exception ex)
            {
                Console.WriteLine("\nCompression failed, skipping!...\n");
            }
        }

    }
}
