using System.Text.RegularExpressions;
using Api.Core.Exceptions;

namespace Api.Core.Helpers;

public static class FileHelper
{
  private static readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
  private const long _maxFileSize = 5 * 1024 * 1024;

  public static void ValidateImage(IFormFile file)
  {
    var extension = Path.GetExtension(file.FileName).ToLower();

    if (!_allowedExtensions.Contains(extension))
    {
      throw new BusinessException($"Geçersiz dosya formatı. İzin verilenler: {string.Join(", ", _allowedExtensions)}");
    }

    if (file.Length > _maxFileSize)
    {
      throw new BusinessException("Dosya boyutu 5MB'dan büyük olamaz.");
    }
  }

  public static async Task<string> SaveImageToDisk(IFormFile file, string subFolder, string name, CancellationToken cancellationToken)
  {
    string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    string targetPath = Path.Combine(wwwrootPath, "images", subFolder);

    if (!Directory.Exists(targetPath))
    {
      Directory.CreateDirectory(targetPath);
    }

    string slug = GetSlug(name);
    string shortId = Guid.NewGuid().ToString().Substring(0, 8);
    string extension = Path.GetExtension(file.FileName).ToLower();

    string fileName = $"{slug}-{shortId}{extension}";
    string fullPath = Path.Combine(targetPath, fileName);

    using (var stream = new FileStream(fullPath, FileMode.Create))
    {
      await file.CopyToAsync(stream, cancellationToken);
    }

    return $"/images/{subFolder}/{fileName}";
  }

  public static void DeleteImageFromDisk(string? relativePath)
  {
    if (string.IsNullOrEmpty(relativePath))
    {
      return;
    }

    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));

    try
    {
      if (File.Exists(fullPath))
      {
        File.Delete(fullPath);
      }
    }
    catch
    {
      /* Fail silently to prevent crashing the main process */
    }
  }

  public static async Task<string?> ReplaceImageOnDisk(
    IFormFile? file,
    string? oldRelativePath,
    string subFolder,
    string name,
    CancellationToken cancellationToken)
  {
    if (file == null || file.Length == 0)
    {
      return oldRelativePath;
    }

    ValidateImage(file);
    string newPath = await SaveImageToDisk(file, subFolder, name, cancellationToken);
    DeleteImageFromDisk(oldRelativePath);

    return newPath;
  }

  private static string GetSlug(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return "image";
    }

    var culture = new System.Globalization.CultureInfo("tr-TR");
    string slug = name.ToLower(culture);

    slug = slug.Replace("ç", "c")
               .Replace("ğ", "g")
               .Replace("ı", "i")
               .Replace("ö", "o")
               .Replace("ş", "s")
               .Replace("ü", "u");

    slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
    slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');

    return slug;
  }
}