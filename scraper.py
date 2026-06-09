
import asyncio
import aiohttp
import re
import csv
import time

URLS_FILE = "urls.txt"
OUTPUT_FILE = "scraped_results.csv"


SAMPLE_URLS = [
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
]

def create_urls_file():
    with open(URLS_FILE, "w", encoding="utf-8") as f:
        for url in SAMPLE_URLS:
            f.write(url + "\n")
    print(f"[INFO] {URLS_FILE} oluşturuldu ({len(SAMPLE_URLS)} URL).")

async def read_urls() -> list[str]:
    """urls.txt dosyasını asenkron olarak oku."""
    loop = asyncio.get_event_loop()
    def _read():
        with open(URLS_FILE, "r", encoding="utf-8") as f:
            return [line.strip() for line in f if line.strip()]
    return await loop.run_in_executor(None, _read)

async def fetch_site(session: aiohttp.ClientSession, url: str) -> dict:
   
    start = time.perf_counter()
    status = "ERROR"
    link_count = 0
    response_time = 0.0

    try:
        async with session.get(url, timeout=aiohttp.ClientTimeout(total=10),
                               ssl=False) as resp:
            status = str(resp.status)
            html = await resp.text(errors="replace")
            
            links = re.findall(r'href=["\']?(https?://[^"\'> ]+)', html)
            link_count = len(links)
    except Exception as e:
        status = f"ERROR: {type(e).__name__}"
    finally:
        response_time = round(time.perf_counter() - start, 3)

    print(f"  [{status}] {url}  |  {link_count} link  |  {response_time}s")
    return {
        "url":           url,
        "status":        status,
        "link_count":    link_count,
        "response_time": response_time,
    }

async def scrape_all(urls: list[str]) -> list[dict]:
   
    connector = aiohttp.TCPConnector(limit=20)
    async with aiohttp.ClientSession(connector=connector) as session:
        tasks = [fetch_site(session, url) for url in urls]
        results = await asyncio.gather(*tasks)
    return list(results)

def write_csv(results: list[dict]):
    """Sonuçları virgülle ayrılmış CSV'ye yaz (Excel uyumlu)."""
    fieldnames = ["url", "status", "link_count", "response_time"]
    with open(OUTPUT_FILE, "w", newline="", encoding="utf-8-sig") as f:
        writer = csv.DictWriter(f, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(results)
    print(f"\n[INFO] Sonuçlar '{OUTPUT_FILE}' dosyasına kaydedildi.")

async def main():
    print("=== Ödev 10: Async Web Scraper ===\n")

    
    import os
    if not os.path.exists(URLS_FILE):
        create_urls_file()

   
    urls = await read_urls()
    print(f"[INFO] {len(urls)} URL okundu. Tarama başlıyor...\n")

    t0 = time.perf_counter()
    results = await scrape_all(urls)
    elapsed = round(time.perf_counter() - t0, 2)

    print(f"\n[INFO] Toplam süre: {elapsed}s")
    write_csv(results)

if __name__ == "__main__":
    asyncio.run(main())
