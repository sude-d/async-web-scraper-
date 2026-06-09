# async-web-scraper-
Async &amp; paralel web scraper: link sayısı + yanıt süresi → CSV | Multi-language implementation in Python (aiohttp) and C# (HttpClient + Task.WhenAll)
🇹🇷 Türkçe
Proje Hakkında
Bu proje, verilen bir URL listesindeki web sitelerini tamamen asenkron ve paralel biçimde tarayan, her sitedeki link sayısını ve yanıt süresini ölçen, sonuçları CSV dosyasına kaydeden bir araçtır. Aynı çözüm hem Python hem de C# ile implement edilmiştir.
Nasıl Çalışır?

urls.txt dosyasından URL listesi asenkron olarak okunur.
Tüm sitelere aynı anda istek atılır — Python'da asyncio.gather, C#'ta Task.WhenAll.
Her site için yanıt süresi (Stopwatch / perf_counter) ve HTML içindeki link sayısı (href regex) ölçülür.
Sonuçlar scraped_results.csv dosyasına Excel uyumlu (virgülle ayrılmış, UTF-8 BOM) formatında yazılır.

Kullanılan Kavramlar
KonuPythonC#Async HTTPaiohttp.ClientSessionHttpClientParalel çalışmaasyncio.gatherTask.WhenAllLink çıkarmare.findallRegex.MatchesSüre ölçümütime.perf_counterStopwatchCSV yazmacsv.DictWriterStringBuilder + File.WriteAllTextAsync
About
A multi-language async web scraper that fetches a list of URLs concurrently, counts hyperlinks per page, measures response times, and writes everything to a CSV report. Implemented in both Python (aiohttp) and C# (HttpClient).
How It Works

URLs are read asynchronously from urls.txt.
All sites are requested simultaneously — asyncio.gather in Python, Task.WhenAll in C#.
For each site: response time and the number of href links in the HTML are recorded.
Results are saved to scraped_results.csv in Excel-compatible UTF-8 CSV format.

Concepts Covered
TopicPythonC#Async HTTPaiohttp.ClientSessionHttpClientConcurrencyasyncio.gatherTask.WhenAllLink extractionre.findallRegex.MatchesTimingtime.perf_counterStopwatchCSV outputcsv.DictWriterStringBuilder + WriteAllTextAsync
