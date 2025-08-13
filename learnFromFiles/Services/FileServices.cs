using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml;

namespace learnFromFiles.Services
{
    public class FileService
    {
        private readonly string _uploadDir;
        private readonly string _logDir;
        private static readonly object _historyLock = new();

        public sealed record DownloadPayload(byte[] Content, string ContentType, string DownloadName);

        public FileService(IWebHostEnvironment env)
        {
            _uploadDir = System.IO.Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists(_uploadDir))
                Directory.CreateDirectory(_uploadDir);
            _logDir = System.IO.Path.Combine(env.WebRootPath, "Log");
            if (!Directory.Exists(_logDir))
                Directory.CreateDirectory(_logDir);
        }



        // takes file from  GUI as IFOrmFIle and gets the file stream to copy to a file with the same name  
        // needs to add to avoid having the same  Name 
        //
        public async Task SaveFileAsync(IFormFile file)
        {
            var filePath = System.IO.Path.Combine(_uploadDir, System.IO.Path.GetFileName(file.FileName));//make path for file 
            using var stream = new FileStream(filePath, FileMode.Create); // create file
            await file.CopyToAsync(stream); // copy strema form file
        }

        // Lista los nombres de los archivos
        public IEnumerable<string> GetAllFileNames()
            => Directory.EnumerateFiles(_uploadDir, "*", SearchOption.AllDirectories)
         //Directory.EnumerateFiles(_uploadDir)
         .Select(p => System.IO.Path.GetFileName(p));// lambda for every file get the correct filename 


        public IDictionary<string, string> SearchWithLine(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return null;
            AddToSearchHistory(keyword);
            var results = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var filePath in Directory.EnumerateFiles(_uploadDir, "*", SearchOption.AllDirectories))
            {
                var text = ExtractTextFromFile(filePath);
                //var content = File.ReadAllText(filePath);
                var ocurrencesLines = GetLineFromSearch(text, keyword);
                if (ocurrencesLines.Count > 0 && !(ocurrencesLines == null))
                {
                    results[System.IO.Path.GetFileName(filePath)] =
              string.Join(Environment.NewLine, ocurrencesLines); // <- join lines
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
                ".txt" => File.ReadAllText(path),
                ".pdf" => ExtractTextFromPdf(path),
                ".docx" => ExtractTextFromDocx(path),
                ".pptx" => ExtractTextFromPptx(path),
                ".xlsx" => ExtractTextFromXlsx(path),
                _ => string.Empty
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
        private List<string> GetLineFromSearch(string fileContent, string keyword)
        {
            var keyowrdLines = new List<string>();
            if (string.IsNullOrEmpty(fileContent) || string.IsNullOrEmpty(keyword))
                return keyowrdLines;

            var unformatLines = fileContent.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            foreach (var line in unformatLines)
            {
                if (line.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //Console.WriteLine(line.ToString());
                    keyowrdLines.Add(line.Trim());

                }
            }
            return keyowrdLines;
        }
        public DownloadPayload ExportSearchToFile(IDictionary<string, string> d, string fileName)
        {
            var csv = ToCsv(d);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            var safe = EnsureCsv(SafeFileName(fileName));
            return new DownloadPayload(bytes, "text/csv", safe);
        }

        private static string ToCsv(IDictionary<string, string> d) =>
            string.Join(Environment.NewLine, d.Select(kv => $"{Csv(kv.Key)},{Csv(kv.Value)}"));

        private static string Csv(string s)
        {
            if (s is null) return "";
            var mustQuote = s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0;
            return mustQuote ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
        }

        private static string SafeFileName(string name)
        {
            Console.WriteLine($"name in file class to var to export is : {name}");
            if (string.IsNullOrWhiteSpace(name)) return "export";
            foreach (var c in System.IO.Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            return name.Trim();
        }

        private static string EnsureCsv(string name) =>
            name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ? name : name + ".csv";
        private void AddToSearchHistory(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return;

            var filePath = Path.Combine(_logDir, "History.txt");

            // Ensure the file exists and is closed immediately
            if (!File.Exists(filePath))
                using (File.Create(filePath)) { }

            // Serialize writers inside this process and allow cross-process sharing
            lock (_historyLock)
            {
                using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using var sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(keyword);
            }
        }
        public IEnumerable<string> GetSearchHistory()
        {
            var filePath = Path.Combine(_logDir, "History.txt");
            if (!File.Exists(filePath))
                return Array.Empty<string>();

            lock (_historyLock)
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                var lines = new List<string>();
                string? line;
                while ((line = sr.ReadLine()) != null)
                    lines.Add(line);
                return lines;
            }
        }
    }
}

