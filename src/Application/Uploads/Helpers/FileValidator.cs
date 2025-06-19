using System.Text;
using Application.UseCases.Uploads.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Uploads.Helpers;

public class FileValidator : IFileValidator
{
    public static readonly Dictionary<string, List<byte[]>> AllowedFiles = new()
    {
        {
            ".jpg",
            new List<byte[]>()
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
            }
        },
        {
            ".png",
            new List<byte[]>() { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } }
        },
        {
            ".mp4",
            new List<byte[]>() { new byte[] { 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D } }
        },
        {
            ".mpg",
            new List<byte[]>() { new byte[] { 0x00, 0x00, 0x01, 0xBA } }
        },
        {
            ".pdf",
            new List<byte[]>() { new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D } }
        },
    };

    private static bool HasValidExtension(string extension)
    {
        return !string.IsNullOrEmpty(extension) && AllowedFiles.TryGetValue(extension, out var _);
    }

    private static bool HasValidSignature(Stream stream, string extension)
    {
        long originalPosition = stream.Position;
        try
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                AllowedFiles.TryGetValue(extension, out var signatures);

                if (signatures == null || signatures.Count == 0)
                    throw new ArgumentException("Signatures list must not be null or empty.");

                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature)
                );
            }
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }

    public bool IsValidFile(IFormFile file)
    {
        string fileName = file.FileName;
        string extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!HasValidExtension(extension))
        {
            return false;
        }

        using var stream = file.OpenReadStream();
        return HasValidSignature(stream, extension);
    }
}
