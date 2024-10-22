using Lab_3.Infrastructure;
using Lab_3.Models;
using Lab_3.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FuelStation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".."));
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string? connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;

            var builderWeb = WebApplication.CreateBuilder(args);
            var services = builderWeb.Services;

            // добавление кэширования
            services.AddMemoryCache();

            // добавление поддержки сессии
            services.AddDistributedMemoryCache();
            services.AddSession();

            // внедрение зависимости CachedTanksService
            services.AddScoped<ICachedGenreService, CachedGenreService>();
            services.AddScoped<ICachedTvshowService, CachedTvshowService>();
            services.AddScoped<ICachedEmployeeService, CachedEmployeeService>();

            // Использование MVC - отключено
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddControllersWithViews();

            var app = builderWeb.Build();

            // добавляем поддержку статических файлов
            app.UseStaticFiles();

            // добавляем поддержку сессий
            app.UseSession();

            // Запоминание в Session значений, введенных в форме
            app.Map("/form", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Считывание из Session объекта User
                    User user = context.Session.Get<User>("user") ?? new User();

                    // Формирование строки для вывода динамической HTML формы
                    string strResponse = "<HTML><HEAD><TITLE>Пользователь</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><FORM action ='/form' / >" +
                    "Имя:<BR><INPUT type = 'text' name = 'FirstName' value = " + user.FirstName + ">" +
                    "<BR>Фамилия:<BR><INPUT type = 'text' name = 'LastName' value = " + user.LastName + " >" +
                    "<BR><BR><INPUT type ='submit' value='Сохранить в Session'><INPUT type ='submit' value='Показать'></FORM>";
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";

                    // Запись в Session данных объекта User
                    user.FirstName = context.Request.Query["FirstName"];
                    user.LastName = context.Request.Query["LastName"];
                    context.Session.Set<User>("user", user);

                    // Асинхронный вывод динамической HTML формы
                    await context.Response.WriteAsync(strResponse);
                });
            });

            // Вывод информации о клиенте
            app.Map("/info", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Формирование строки для вывода 
                    string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Информация:</H1>";
                    strResponse += "<BR> Сервер: " + context.Request.Host;
                    strResponse += "<BR> Путь: " + context.Request.PathBase;
                    strResponse += "<BR> Протокол: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
                    // Вывод данных
                    await context.Response.WriteAsync(strResponse);
                });
            });

            // Вывод кэшированной информации из таблицы базы данных
            app.Map("/genre", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedGenreService cachedGenreService = context.RequestServices.GetService<ICachedGenreService>();
                    IEnumerable<Genre> genres = cachedGenreService.GetGenre();
                    string HtmlString = "<HTML><HEAD><TITLE>Жанры</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список жанрой</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>Жанр</TH>";
                    HtmlString += "<TH>Описание</TH>";
                    HtmlString += "</TR>";
                    foreach (var genre in genres)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + genre.GenreId + "</TD>";
                        HtmlString += "<TD>" + genre.GenreName + "</TD>";
                        HtmlString += "<TD>" + genre.GenreDescription + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/genre'>Жанры</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            // Вывод кэшированной информации из таблицы базы данных
            app.Map("/tvshow", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedTvshowService cachedTvshowService = context.RequestServices.GetService<ICachedTvshowService>();
                    IEnumerable<Tvshow> tvshows = cachedTvshowService.GetTvshow();
                    string HtmlString = "<HTML><HEAD><TITLE>Телепередачи</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список телепередач</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>Название передачи</TH>";
                    HtmlString += "<TH>Длительность</TH>";
                    HtmlString += "<TH>Рейтинг</TH>";
                    HtmlString += "<TH>Описание</TH>";
                    HtmlString += "</TR>";
                    foreach (var tvshow in tvshows)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + tvshow.ShowId + "</TD>";
                        HtmlString += "<TD>" + tvshow.ShowName + "</TD>";
                        HtmlString += "<TD>" + tvshow.Duration + "</TD>";
                        HtmlString += "<TD>" + tvshow.Rating + "</TD>";
                        HtmlString += "<TD>" + tvshow.Description + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/genre'>Жанры</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            // Вывод кэшированной информации из таблицы базы данных
            app.Map("/employee", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedEmployeeService cachedEmployeeService = context.RequestServices.GetService<ICachedEmployeeService>();
                    IEnumerable<Employee> employees = cachedEmployeeService.GetEmployee();
                    string HtmlString = "<HTML><HEAD><TITLE>Сотрудники</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список сотрудников</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>ФИО сотрудника</TH>";
                    HtmlString += "<TH>Должность</TH>";
                    HtmlString += "</TR>";
                    foreach (var employee in employees)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + employee.EmployeeId + "</TD>";
                        HtmlString += "<TD>" + employee.FullName + "</TD>";
                        HtmlString += "<TD>" + employee.Position + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/genre'>Жанры</A></BR>";
                    HtmlString += "<BR><A href='/tvshow'>Телепередачи</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });
            // Вывод кэшированной информации из таблицы базы данных
            app.Map("/searchform1", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Получаем объект Search из сессии или создаём новый
                    Search search = context.Session.Get<Search>("search") ?? new Search();

                    // Проверяем, есть ли параметр 'query' в запросе
                    if (!string.IsNullOrEmpty(context.Request.Query["query"]))
                    {
                        // Получаем значение запроса
                        search.SearchRequest = context.Request.Query["query"];

                        // Получаем результаты поиска
                        ICachedGenreService cachedGenreService = context.RequestServices.GetService<ICachedGenreService>();
                        search.AnswerGenre = cachedGenreService.FindGenre(search.SearchRequest);
                    }

                    // Генерируем HTML-страницу
                    string responseHtml = "<HTML><HEAD><TITLE>Поиск</TITLE></HEAD>" +
                                          "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                                          "<BODY><H1>Форма поиска</H1>" +
                                          "<FORM method='GET' action='/searchform1'>" +
                                          "<LABEL for='query'>Введите запрос:</LABEL><br>" +
                                          "<INPUT type='text' id='query' name='query' value='" +
                                          System.Net.WebUtility.HtmlEncode(search.SearchRequest) + "'><br><br>" +
                                          "<INPUT type='submit' value='Поиск'>" +
                                          "</FORM>";

                    // Проверяем, есть ли результаты поиска
                    if (search.AnswerGenre != null && search.AnswerGenre.Any())
                    {
                        responseHtml += "<H2>Результаты поиска:</H2><UL>";
                        foreach (var genre in search.AnswerGenre)
                        {
                            responseHtml += "<LI>" + System.Net.WebUtility.HtmlEncode(genre.GenreName) + " | " +
                            System.Net.WebUtility.HtmlEncode(genre.GenreDescription) + "</LI>";
                        }
                        responseHtml += "</UL>";
                    }
                    else if (!string.IsNullOrEmpty(search.SearchRequest))
                    {
                        responseHtml += "<H2>Нет результатов для: " +
                                        System.Net.WebUtility.HtmlEncode(search.SearchRequest) + "</H2>";
                    }

                    responseHtml += "<BR><A href='/'>Главная</A></BR>" +
                                    "<BR><A href='/genre'>Жанры</A></BR>" +
                                    "<BR><A href='/form'>Данные пользователя</A></BR>" +
                                    "</BODY></HTML>";

                    // Сохраняем объект Search в сессии
                    context.Session.Set("search", search);

                    // Отправляем ответ
                    await context.Response.WriteAsync(responseHtml);
                });
            });
            // Вывод кэшированной информации из таблицы базы данных
            app.Map("/searchform2", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Получаем объект Search из сессии или создаём новый
                    Search search = context.Session.Get<Search>("search") ?? new Search();

                    // Проверяем, есть ли параметр 'query' в запросе
                    if (!string.IsNullOrEmpty(context.Request.Query["query"]))
                    {
                        // Получаем значение запроса
                        search.SearchRequest = context.Request.Query["query"];

                        // Получаем результаты поиска
                        ICachedTvshowService cachedGenreService = context.RequestServices.GetService<ICachedTvshowService>();
                        search.AnswerTvshow = cachedGenreService.FindTvshow(search.SearchRequest);
                    }

                    // Генерируем HTML-страницу
                    string responseHtml = "<HTML><HEAD><TITLE>Поиск</TITLE></HEAD>" +
                                          "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                                          "<BODY><H1>Форма поиска</H1>" +
                                          "<FORM method='GET' action='/searchform2'>" +
                                          "<LABEL for='query'>Введите запрос:</LABEL><br>" +
                                          "<INPUT type='text' id='query' name='query' value='" +
                                          System.Net.WebUtility.HtmlEncode(search.SearchRequest) + "'><br><br>" +
                                          "<INPUT type='submit' value='Поиск'>" +
                                          "</FORM>";

                    // Проверяем, есть ли результаты поиска
                    if (search.AnswerGenre != null && search.AnswerTvshow.Any())
                    {
                        responseHtml += "<H2>Результаты поиска:</H2><UL>";
                        foreach (var tvshow in search.AnswerTvshow)
                        {
                            responseHtml += "<LI>" + System.Net.WebUtility.HtmlEncode(tvshow.ShowName) + " | " +
                            System.Net.WebUtility.HtmlEncode(tvshow.Duration.ToString()) + " | " +
                            System.Net.WebUtility.HtmlEncode(tvshow.Rating.ToString()) + "</LI>";
                        }
                        responseHtml += "</UL>";
                    }
                    else if (!string.IsNullOrEmpty(search.SearchRequest))
                    {
                        responseHtml += "<H2>Нет результатов для: " +
                                        System.Net.WebUtility.HtmlEncode(search.SearchRequest) + "</H2>";
                    }

                    responseHtml += "<BR><A href='/'>Главная</A></BR>" +
                                    "<BR><A href='/genre'>Жанры</A></BR>" +
                                    "<BR><A href='/form'>Данные пользователя</A></BR>" +
                                    "</BODY></HTML>";

                    // Сохраняем объект Search в сессии
                    context.Session.Set("search", search);

                    // Отправляем ответ
                    await context.Response.WriteAsync(responseHtml);
                });
            });

            // Стартовая страница и кэширование данных таблицы на web-сервере
            app.Run((context) =>
            {
                string HtmlString = "<HTML><HEAD><TITLE>Емкости</TITLE></HEAD>" +
                "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                "<BODY><H1>Главная</H1>";
                HtmlString += "<H2>Данные записаны в кэш сервера</H2>";
                HtmlString += "<BR><A href='/'>Главная</A></BR>";
                HtmlString += "<BR><A href='/genre'>Жанры</A></BR>";
                HtmlString += "<BR><A href='/tvshow'>Телепередачи</A></BR>";
                HtmlString += "<BR><A href='/employee'>Сотрудники</A></BR>";
                HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                HtmlString += "<BR><A href='/info'>Информация</A></BR>";
                HtmlString += "<BR><A href='/searchform1'>Поиск 1</A></BR>";
                HtmlString += "<BR><A href='/searchform2'>Поиск 2</A></BR>";
                HtmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(HtmlString);
            });

            // Использование MVC - отключено
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}