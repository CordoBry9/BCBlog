﻿using BCBlog.Models;
using BCBlog.Client.Helpers;
using System.Text.RegularExpressions;

namespace BCBlog.Helpers
{
    public static class UploadHelper
    {
        public static readonly string DefaultProfilePicture = ImageHelper.DefaultProfilePicture;
        public static readonly string DefaultBlogPicture = ImageHelper.DefaultBlogPicture;
        public static readonly string DefaultCategoryPicture = ImageHelper.DefaultCategoryPicture;
        public static readonly int MaxFileSize = ImageHelper.MaxFileSize;

        public static async Task<ImageUpload> GetImageUploadAsync(IFormFile file)
        {
            using var ms = new MemoryStream();

            await file.CopyToAsync(ms);
            byte[] data = ms.ToArray();

            if (ms.Length > 5 * 1024 * 1024)
            {
                throw new IOException("Images must be 5MB or less");
            }

            ImageUpload upload = new ImageUpload()
            {

                Id = Guid.NewGuid(),
                Data = data,
                Type = file.ContentType,
            };

            return upload;
        }

        public static ImageUpload GetImageUpload(string dataUrl) 
        {
            GroupCollection matchGroups = Regex.Match(dataUrl, @"data:(?<type>.+?);base64,(?<data>.+)").Groups;

            if (matchGroups.ContainsKey("type") && matchGroups.ContainsKey("data"))
            {
                string contentType = matchGroups["type"].Value;

                byte[] data = Convert.FromBase64String(matchGroups["data"].Value);

                if (data.Length <= MaxFileSize)
                {
                    ImageUpload upload = new ImageUpload()
                    {
                        Id = Guid.NewGuid(),
                        Data = data,
                        Type = contentType,
                    };

                    return upload;
                }
            }

            throw new IOException("Data URL was either invalid or too large");
        }
    }
}
