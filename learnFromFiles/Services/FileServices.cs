using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace learnFromFiles.Services{
    public class FileService
    {
        private readonly string _uploadDir; //upload direcotrpath

        public FileService(IWebHostEnvironment env)
        {
            // Folder  wwwroot/uploads
            _uploadDir = Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists(_uploadDir))
                Directory.CreateDirectory(_uploadDir);
        }

        // takes some file 
        public async Task SaveFileAsync(IFormFile file)
        {
            var filePath = Path.Combine(_uploadDir, Path.GetFileName(file.FileName));//make path for file 
            using var stream = new FileStream(filePath, FileMode.Create); // create file
            await file.CopyToAsync(stream); // copy strema form file
        }

        // Lista los nombres de los archivos
        public IEnumerable<string> GetAllFileNames()
            => Directory.EnumerateFiles(_uploadDir)
                        .Select(Path.GetFileName);


         public IDictionary<string, string> SearchWithLine(string keyword)
        {
            var results = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var filePath in Directory.EnumerateFiles(_uploadDir))
            {
                var content = File.ReadAllText(filePath);
                var line = GetLineFromSearch(content, keyword);
                if (!string.IsNullOrEmpty(line))
                {
                    results[Path.GetFileName(filePath)] = line;
                }
            }

            return results;
        }
        // gets the line where the word  until  next line comes 
                private string GetLineFromSearch(string fileContent, string keyword)
        {
            // Separa en líneas (Unix/Mac/Windows)
            var lines = fileContent
                .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            // Busca la primera línea que contenga la keyword
            foreach (var line in lines)
            {
                if (line
                    .IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return line.Trim();
                }
            }

            return string.Empty;
        }
    }
}

