﻿
namespace BlogProject.Services
{
    public class BasicImageService : IImageService
    {
        public string ContentType(IFormFile file)
        {
            return file?.ContentType;
        }

        public string DecodeImage(byte[] data, string type)
        {
            if (data == null || type is null) return null;
            return $"data:image/{type};base64,{Convert.ToBase64String(data)}";
            
        }

        public async Task<byte[]> EncodeImageAsync(IFormFile file)
        {
            if(file == null) return null;
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        // When dealing with a path
        public async Task<byte[]> EncodeImageAsync(string fileName)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/assets/img/{fileName}";
            return await File.ReadAllBytesAsync(file);
        }



        public int Size(IFormFile file)
        {
            return Convert.ToInt32(file?.Length);
        }
    }
}