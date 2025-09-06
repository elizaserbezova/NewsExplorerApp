# NewsExplorerApp

ASP.NET Core MVC приложение, което показва последни новини в реално време чрез интеграция с **NewsAPI.org**. Проектът е изграден за стажантско портфолио и демонстрира работа с външен API, Razor изгледи, филтри/търсене, сортиране и тестове (unit + „controller-level“ integration).

---

## Съдържание
- [Какво представлява](#какво-представлява)
- [Функционалности](#функционалности)
- [Технологии](#технологии)
- [Архитектура и структури](#архитектура-и-структури)
  - [Структура на проекта](#структура-на-проекта)
  - [Модели](#модели)
  - [Контролер](#контролер)
  - [Изглед](#изглед)
- [Настройка](#настройка)
  - [API ключ чрез User Secrets (без да се комитва)](#api-ключ-чрез-user-secrets-без-да-се-комитва)
  - [Конфигурация](#конфигурация)
- [Стартиране](#стартиране)
- [Тестове](#тестове)
  - [Създаване на тестов проект (без терминал)](#създаване-на-тестов-проект-без-терминал)
  - [Мини промяна за тестируемост](#мини-промяна-за-тестируемост)
  - [Какво покриват тестовете](#какво-покриват-тестовете)
  - [Стартиране на тестовете](#стартиране-на-тестовете)
- [Правила за параметри и валидация](#правила-за-параметри-и-валидация)
- [UX детайли](#ux-детайли)
- [Обработка на грешки](#обработка-на-грешки)
- [Сигурност на ключове](#сигурност-на-ключове)
- [Известни ограничения](#известни-ограничения)
- [Идеи за бъдещо развитие](#идеи-за-бъдещо-развитие)
- [Лиценз](#лиценз)

---

## Какво представлява
**NewsExplorerApp** е малко, но завършено MVC приложение, което:
- Изпраща заявки към `https://newsapi.org/v2/top-headlines`.
- Позволява филтриране по държава/категория или конкретни източници (sources).
- Има поле за търсене по ключова дума и локално сортиране по дата.
- Включва тестове, които покриват основната логика в контролера.

---

## Функционалности
- Преглед на **Top Headlines** по **държава** (`country`) и **категория** (`category`).
- **Търсене** по ключова дума (`searchQuery` → `q`).
- Филтър по **източници** (`sources`) от NewsAPI.
- **Сортиране** по дата (`sortOrder`): най-нови (`desc`) / най-стари (`asc`).
- Картички с **изображение**, **заглавие**, **описание**, **дата** и **линк** към статията.
- UI логика: при избран `sources`, контролите за `country`/`category` са **disabled** и не се изпращат към API.

---

## Технологии
- **.NET 8** (препоръчително)
- ASP.NET Core MVC (Controllers, ViewModels, Razor Views)
- Bootstrap 5 за UI
- `System.Text.Json` за десериализация
- **xUnit** + **FluentAssertions** за тестове

---

## Архитектура и структури

### Структура на проекта
```
NewsExplorerApp/
├─ Controllers/
│  └─ NewsController.cs
├─ Models/
│  ├─ NewsArticle.cs
│  ├─ NewsApiResponse.cs
│  ├─ NewsApiSourcesResponse.cs
│  └─ NewsApiSource.cs
├─ ViewModels/
│  └─ NewsViewModel.cs
├─ Views/
│  └─ News/
│     └─ Index.cshtml
├─ wwwroot/
│  └─ images/ (по избор: placeholder.jpg)
├─ appsettings.json
└─ NewsExplorerApp.csproj

NewsExplorerApp.Tests/
├─ Fixtures/
│  └─ NewsJson.cs
├─ Fakes/
│  └─ TestableNewsController.cs
├─ Http/
│  └─ FakeHttpMessageHandler.cs
└─ NewsControllerUnitTests.cs
```

### Модели
**`Models/NewsArticle.cs`**
```csharp
namespace NewsExplorerApp.Models
{
    public class NewsArticle
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string SourceName { get; set; } // показва източника като име
    }
}
```

**`Models/NewsApiResponse.cs`**
```csharp
using System.Collections.Generic;

namespace NewsExplorerApp.Models
{
    public class NewsApiResponse
    {
        public List<NewsArticle> Articles { get; set; }
    }
}
```

**`Models/NewsApiSourcesResponse.cs` + `Models/NewsApiSource.cs`**
```csharp
using System.Collections.Generic;

namespace NewsExplorerApp.Models
{
    public class NewsApiSourcesResponse
    {
        public List<NewsApiSource> Sources { get; set; }
    }

    public class NewsApiSource
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
```

**`ViewModels/NewsViewModel.cs`**
```csharp
using NewsExplorerApp.Models;
using System.Collections.Generic;

namespace NewsExplorerApp.ViewModels
{
    public class NewsViewModel
    {
        public List<NewsArticle> Articles { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedCountry { get; set; }

        public string SearchQuery { get; set; }
        public string SelectedSources { get; set; }
        public string SortOrder { get; set; } // "desc" | "asc"

        public DateTime? PublishedAt { get; set; }

        public List<string> Categories { get; set; } = new()
        {
            "business", "entertainment", "general", "health", "science", "sports", "technology"
        };

        public List<string> Countries { get; set; } = new()
        {
            "us", "gb", "de", "fr", "it", "bg"
        };
    }
}
```

### Контролер
**`Controllers/NewsController.cs` (основни акценти)**

- Чете API ключ от `IConfiguration["NewsApi:ApiKey"]`.
- Сглобява URL чрез `UriBuilder` + `System.Web.HttpUtility.ParseQueryString`.
- Правила за параметри:
  - Ако има `sources`, **не** изпраща `country` и `category`.
  - Ако няма `sources`, изпраща `country` и `category`.
  - Ако има `searchQuery`, подава `q` към API (внимавай за двойно енкодване).
- Зарежда списъци в `ViewBag`:
  - `ViewBag.Countries`, `ViewBag.Categories` (статични списъци).
  - `ViewBag.Sources` — динамично от `/v2/sources` на NewsAPI.
- **Сортиране**: локално по `PublishedAt` (desc/asc).

> **Бележка за енкодване:** избягвай двойно енкодване на `q`. Вместо `UrlEncode`, задай `query["q"] = searchQuery;` и остави `UriBuilder`/`ParseQueryString` да енкоднат.

### Изглед
**`Views/News/Index.cshtml`**:
- Форма с полета: Country, Category, Sources, Search by keyword и бутони за Sort (Newest/Oldest) + Search.
- Картова мрежа на статии (изображение, заглавие, описание, дата `dd MMM yyyy HH:mm`, бутон „Прочети“).
- Малък скрипт, който **disable-ва** Country/Category, ако е избран конкретен Source.

---

## Настройка

### API ключ чрез User Secrets (без да се комитва)
1. Вземи **API Key** от https://newsapi.org.
2. В **Visual Studio**:  
   - Right-click върху проекта **NewsExplorerApp** → **Manage User Secrets**.  
   - В отворения `secrets.json` добави:
     ```json
     {
       "NewsApi": {
         "ApiKey": "YOUR_NEWSAPI_KEY"
       }
     }
     ```
   - Запази файла (той е извън репозитория и не се комитва).

> Алтернатива: environment variable `NewsApi__ApiKey`.

### Конфигурация
`appsettings.json` (пример):
```json
{
  "NewsApi": {
    "ApiKey": "",
    "DefaultCountry": "us",
    "DefaultCategory": "general"
  }
}
```

---

## Стартиране
- **Visual Studio**: Set as Startup Project → **F5**.  
- Начална страница: `/News/Index`.  
- Използвай формата за избор на **Country**, **Category**, **Sources**, търсене по ключова дума и сортиране.

---

## Тестове

### Създаване на тестов проект (без терминал)
1. **Solution Explorer** → Right-click върху **Solution** → **Add** → **New Project…**  
2. Избери **xUnit Test Project (.NET Core)** → **Next**.  
3. Име: **NewsExplorerApp.Tests** → **Create**.  
4. Добави референция: Right-click върху **NewsExplorerApp.Tests** → **Add** → **Project Reference…** → избери **NewsExplorerApp** → **OK**.  
5. NuGet пакети: Right-click **NewsExplorerApp.Tests** → **Manage NuGet Packages…** → **Browse** → инсталирай **FluentAssertions** (последна stable).  
6. Създай папки: **Fixtures**, **Fakes**, **Http** и добави файловете по-долу.

### Мини промяна за тестируемост
За да подменяме `HttpClient` в тестовете без DI/сервизи, добави в `NewsController` два метода и ги използвай вместо `new HttpClient()`:

```csharp
// вътре в класа NewsController
protected virtual HttpClient CreateHttpClient() => new HttpClient();
protected virtual HttpClient CreateHttpClientForSources() => new HttpClient();
```

В `Index(...)` замени:
```csharp
using HttpClient httpClient = new HttpClient();
```
с:
```csharp
using HttpClient httpClient = CreateHttpClient();
```

В `GetSourcesAsync()` замени:
```csharp
using HttpClient httpClient = new HttpClient();
```
с:
```csharp
using HttpClient httpClient = CreateHttpClientForSources();
```

### Какво покриват тестовете
- Сортиране по дата (desc/asc).
- Правила за параметри:
  - При подаден `sources` → **игнорира** `country` и `category`.
  - При липса на `sources` → използва `country` и `category`.
- Коректно енкодване на `searchQuery` (`q`) — без **двойно енкодване**.
- Обработка на грешки при non-2xx отговор (напр. 429) → показва се съобщение и моделът е празен.
- Пълнене на `ViewBag.Sources` от `/v2/sources`.

**Помощни класове за тестове** (в `NewsExplorerApp.Tests`):

`Http/FakeHttpMessageHandler.cs` — фалшив HTTP handler за фиктивни JSON отговори.  
`Fakes/TestableNewsController.cs` — наследник, който override-ва `CreateHttpClient*()`.  
`Fixtures/NewsJson.cs` — примерни JSON отговори.

**Примерни JSON фикстури:**
```json
{
  "status": "ok",
  "totalResults": 2,
  "articles": [
    {
      "title": "B",
      "description": "desc B",
      "url": "https://example.com/b",
      "urlToImage": "https://example.com/b.jpg",
      "publishedAt": "2025-09-05T12:00:00Z",
      "source": { "id": "src-b", "name": "Source B" }
    },
    {
      "title": "A",
      "description": "desc A",
      "url": "https://example.com/a",
      "urlToImage": "https://example.com/a.jpg",
      "publishedAt": "2025-09-04T12:00:00Z",
      "source": { "id": "src-a", "name": "Source A" }
    }
  ]
}
```

**Стартиране на тестовете (Visual Studio UI):**
- Меню **Test** → **Test Explorer** → **Run All Tests**.

---

## Правила за параметри и валидация
- `sources` **заменя** `country` и `category` (специфика на `top-headlines` в NewsAPI).
- `searchQuery` → параметър `q`. Внимавай за двойно енкодване (остави `UriBuilder` да енкодва).
- `sortOrder` ∈ {`desc`,`asc`} и се прилага **локално** по `PublishedAt`.

---

## UX детайли
- Когато е избран `sources`, полетата **Country**/**Category** са **disabled** (и се игнорират на сървър).
- В картите на статии: ако `UrlToImage` липсва, можеш да сложиш `placeholder.jpg` (по избор).
- Датата се визуализира във формат `dd MMM yyyy HH:mm`.

---

## Обработка на грешки
- При неуспешен отговор от NewsAPI (напр. 429/500) се показва съобщение в UI („Error loading news.“) и се връща празен списък.
- По желание може да се добавят retry/timeout, но в базовия вариант не се изискват.

---

## Сигурност на ключове
- **Не комитвай** API ключа. Използвай **User Secrets** локално или environment variables в CI.
- Пример за `secrets.json`:
```json
{
  "NewsApi": {
    "ApiKey": "YOUR_NEWSAPI_KEY"
  }
}
```

---

## Известни ограничения
- Използва се безплатният `v2/top-headlines` endpoint (ограничен достъп спрямо „everything“).
- Възможен е **rate limit (429)**.
- Сортирането е клиентско (след като получим данните).
- При избран `sources`, `country/category` не могат да се ползват едновременно.

---

## Идеи за бъдещо развитие
- Dark Mode (Bootstrap 5.3 color modes) + запомняне в `localStorage`.
- Favorites (локално или с Identity + EF Core) + страница „My Favorites“.
- Пагинация (`page`, `pageSize`) според NewsAPI.
- Краткотрайно кеширане и по-добра обработка на грешки.
- CI (GitHub Actions): build, test, format.

---

## Лиценз
Проект с образователна цел. Свободен за използване в портфолио и стажантски кандидатури. Проверявай лицензи при добавяне на външни пакети/теми.
