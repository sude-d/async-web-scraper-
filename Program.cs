using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ─── MODEL ────────────────────────────────────────────────────────────────────
public record ScrapeResult(
    string Url,
    string Status,
    int    LinkCount,
    double ResponseTime   // saniye
);

// ─── SCRAPER ──────────────────────────────────────────────────────────────────
public class WebScraper
{
    private static readonly HttpClient _client = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    public async Task<ScrapeResult> FetchAsync(string url)
    {
        var sw     = Stopwatch.StartNew();
        string status   = "ERROR";
        int    linkCount = 0;

        try
        {
            var response = await _client.GetAsync(url);
            status = ((int)response.StatusCode).ToString();
            string html = await response.Content.ReadAsStringAsync();

            // href="https://..." linklerini say
            var matches  = Regex.Matches(html,
                @"href=[""']?(https?://[^""'> ]+)", RegexOptions.IgnoreCase);
            linkCount = matches.Count;
        }
        catch (Exception ex)
        {
            status = $"ERROR:{ex.GetType().Name}";
        }
        finally
        {
            sw.Stop();
        }

        double elapsed = Math.Round(sw.Elapsed.TotalSeconds, 3);
        Console.WriteLine($"  [{status}] {url}  |  {linkCount} link  |  {elapsed}s");

        return new ScrapeResult(url, status, linkCount, elapsed);
    }
}

// ─── CSV WRITER ───────────────────────────────────────────────────────────────
public static class CsvWriter
{
    public static async Task WriteAsync(string path, IEnumerable<ScrapeResult> results)
    {
        var sb = new StringBuilder();
        sb.AppendLine("url,status,link_count,response_time");
        foreach (var r in results)
            sb.AppendLine($"{r.Url},{r.Status},{r.LinkCount},{r.ResponseTime}");

        await File.WriteAllTextAsync(path, sb.ToString(), new UTF8Encoding(true));
        Console.WriteLine($"\n[INFO] Sonuçlar '{path}' dosyasına kaydedildi.");
    }
}

// ─── MAIN ─────────────────────────────────────────────────────────────────────
class Program
{
    // Demo URL listesi — gerçekte urls.txt'den okunur
    static readonly string[] SampleUrls =
    [
        "https://www.python.org",
        "https://www.github.com",
        "https://www.wikipedia.org",
        "https://www.bbc.com",
        "https://www.stackoverflow.com",
        "https://www.medium.com",
        "https://www.reddit.com",
        "https://news.ycombinator.com",
        "https://www.nytimes.com",
        "https://www.nature.com",
    ];

    static async Task Main()
    {
        Console.WriteLine("=== Ödev 10: Async Web Scraper (C#) ===\n");

        const string urlsFile   = "urls.txt";
        const string outputFile = "scraped_results.csv";

        // urls.txt yoksa oluştur
        if (!File.Exists(urlsFile))
        {
            await File.WriteAllLinesAsync(urlsFile, SampleUrls);
            Console.WriteLine($"[INFO] {urlsFile} oluşturuldu.");
        }

        // Asenkron URL okuma
        string[] urls = await File.ReadAllLinesAsync(urlsFile);
        Console.WriteLine($"[INFO] {urls.Length} URL okundu. Tarama başlıyor...\n");

        var scraper = new WebScraper();
        var sw      = Stopwatch.StartNew();

        // Tüm sitelere aynı anda istek at (Task.WhenAll = paralel)
        var tasks   = Array.ConvertAll(urls, url => scraper.FetchAsync(url));
        var results = await Task.WhenAll(tasks);

        sw.Stop();
        Console.WriteLine($"\n[INFO] Toplam süre: {sw.Elapsed.TotalSeconds:F2}s");

        await CsvWriter.WriteAsync(outputFile, results);
    }
}
