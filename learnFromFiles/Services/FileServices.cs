using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;

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
        // make so they also accept pdf and also works with IfromFi
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
                var text = ExtractTextFromFile(filePath);
                //var content = File.ReadAllText(filePath);
                var line = GetLineFromSearch(text, keyword);
                if (!string.IsNullOrEmpty(line))
                {
                    results[Path.GetFileName(filePath)] = line;
                }
            }

            return results;
        }

        //using bib pigpdf to seac h i ndiff types of files 
      private string ExtractTextFromFile(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".txt"  => File.ReadAllText(path),
                ".pdf"  => ExtractTextFromPdf(path),
                ".docx" => ExtractTextFromDocx(path),
                ".pptx" => ExtractTextFromPptx(path),
                ".xlsx" => ExtractTextFromXlsx(path),
                _       => string.Empty
            };
        }
                private string ExtractTextFromPdf(string path)
        {
            using var doc = PdfDocument.Open(path);
            return string.Join("\n", doc.GetPages().Select(p => p.Text));
        }

        private string ExtractTextFromDocx(string path)
        {
            using var doc = WordprocessingDocument.Open(path, false);
            return doc.MainDocumentPart.Document.Body.InnerText;
        }

        private string ExtractTextFromPptx(string path)
        {
            using var ppt = PresentationDocument.Open(path, false);
            var texts = ppt.PresentationPart.SlideParts
                .SelectMany(s => s.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                .Select(t => t.Text);
            return string.Join("\n", texts);
        }

        private string ExtractTextFromXlsx(string path)
        {
            using var xls = SpreadsheetDocument.Open(path, false);
            var texts = xls.WorkbookPart.WorksheetParts
                .SelectMany(ws => ws.Worksheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>())
                .Select(c => c.InnerText)
                .Where(t => !string.IsNullOrWhiteSpace(t));
            return string.Join("\n", texts);
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

