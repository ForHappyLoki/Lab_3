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

            // ���������� �����������
            services.AddMemoryCache();

            // ���������� ��������� ������
            services.AddDistributedMemoryCache();
            services.AddSession();

            // ��������� ����������� CachedTanksService
            services.AddScoped<ICachedGenreService, CachedGenreService>();
            services.AddScoped<ICachedTvshowService, CachedTvshowService>();
            services.AddScoped<ICachedEmployeeService, CachedEmployeeService>();

            // ������������� MVC - ���������
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddControllersWithViews();

            var app = builderWeb.Build();

            // ��������� ��������� ����������� ������
            app.UseStaticFiles();

            // ��������� ��������� ������
            app.UseSession();

            // ����������� � Session ��������, ��������� � �����
            app.Map("/form", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // ���������� �� Session ������� User
                    User user = context.Session.Get<User>("user") ?? new User();

                    // ������������ ������ ��� ������ ������������ HTML �����
                    string strResponse = "<HTML><HEAD><TITLE>������������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><FORM action ='/form' / >" +
                    "���:<BR><INPUT type = 'text' name = 'FirstName' value = " + user.FirstName + ">" +
                    "<BR>�������:<BR><INPUT type = 'text' name = 'LastName' value = " + user.LastName + " >" +
                    "<BR><BR><INPUT type ='submit' value='��������� � Session'><INPUT type ='submit' value='��������'></FORM>";
                    strResponse += "<BR><A href='/'>�������</A></BODY></HTML>";

                    // ������ � Session ������ ������� User
                    user.FirstName = context.Request.Query["FirstName"];
                    user.LastName = context.Request.Query["LastName"];
                    context.Session.Set<User>("user", user);

                    // ����������� ����� ������������ HTML �����
                    await context.Response.WriteAsync(strResponse);
                });
            });

            // ����� ���������� � �������
            app.Map("/info", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // ������������ ������ ��� ������ 
                    string strResponse = "<HTML><HEAD><TITLE>����������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>����������:</H1>";
                    strResponse += "<BR> ������: " + context.Request.Host;
                    strResponse += "<BR> ����: " + context.Request.PathBase;
                    strResponse += "<BR> ��������: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>�������</A></BODY></HTML>";
                    // ����� ������
                    await context.Response.WriteAsync(strResponse);
                });
            });

            // ����� ������������ ���������� �� ������� ���� ������
            app.Map("/genre", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //��������� � �������
                    ICachedGenreService cachedGenreService = context.RequestServices.GetService<ICachedGenreService>();
                    IEnumerable<Genre> genres = cachedGenreService.GetGenre();
                    string HtmlString = "<HTML><HEAD><TITLE>�����</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>������ ������</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>���</TH>";
                    HtmlString += "<TH>����</TH>";
                    HtmlString += "<TH>��������</TH>";
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
                    HtmlString += "<BR><A href='/'>�������</A></BR>";
                    HtmlString += "<BR><A href='/genre'>�����</A></BR>";
                    HtmlString += "<BR><A href='/form'>������ ������������</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // ����� ������
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            // ����� ������������ ���������� �� ������� ���� ������
            app.Map("/tvshow", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //��������� � �������
                    ICachedTvshowService cachedTvshowService = context.RequestServices.GetService<ICachedTvshowService>();
                    IEnumerable<Tvshow> tvshows = cachedTvshowService.GetTvshow();
                    string HtmlString = "<HTML><HEAD><TITLE>������������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>������ �����������</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>���</TH>";
                    HtmlString += "<TH>�������� ��������</TH>";
                    HtmlString += "<TH>������������</TH>";
                    HtmlString += "<TH>�������</TH>";
                    HtmlString += "<TH>��������</TH>";
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
                    HtmlString += "<BR><A href='/'>�������</A></BR>";
                    HtmlString += "<BR><A href='/genre'>�����</A></BR>";
                    HtmlString += "<BR><A href='/form'>������ ������������</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // ����� ������
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            // ����� ������������ ���������� �� ������� ���� ������
            app.Map("/employee", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //��������� � �������
                    ICachedEmployeeService cachedEmployeeService = context.RequestServices.GetService<ICachedEmployeeService>();
                    IEnumerable<Employee> employees = cachedEmployeeService.GetEmployee();
                    string HtmlString = "<HTML><HEAD><TITLE>����������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>������ �����������</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>���</TH>";
                    HtmlString += "<TH>��� ����������</TH>";
                    HtmlString += "<TH>���������</TH>";
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
                    HtmlString += "<BR><A href='/'>�������</A></BR>";
                    HtmlString += "<BR><A href='/genre'>�����</A></BR>";
                    HtmlString += "<BR><A href='/tvshow'>������������</A></BR>";
                    HtmlString += "<BR><A href='/form'>������ ������������</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // ����� ������
                    await context.Response.WriteAsync(HtmlString);
                });
            });
            // ����� ������������ ���������� �� ������� ���� ������
            app.Map("/searchform1", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // �������� ������ Search �� ������ ��� ������ �����
                    Search search = context.Session.Get<Search>("search") ?? new Search();

                    // ���������, ���� �� �������� 'query' � �������
                    if (!string.IsNullOrEmpty(context.Request.Query["query"]))
                    {
                        // �������� �������� �������
                        search.SearchRequest = context.Request.Query["query"];

                        // �������� ���������� ������
                        ICachedGenreService cachedGenreService = context.RequestServices.GetService<ICachedGenreService>();
                        search.AnswerGenre = cachedGenreService.FindGenre(search.SearchRequest);
                    }

                    // ���������� HTML-��������
                    string responseHtml = "<HTML><HEAD><TITLE>�����</TITLE></HEAD>" +
                                          "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                                          "<BODY><H1>����� ������</H1>" +
                                          "<FORM method='GET' action='/searchform1'>" +
                                          "<LABEL for='query'>������� ������:</LABEL><br>" +
                                          "<INPUT type='text' id='query' name='query' value='" +
                                          System.Net.WebUtility.HtmlEncode(search.SearchRequest) + "'><br><br>" +
                                          "<INPUT type='submit' value='�����'>" +
                                          "</FORM>";

                    // ���������, ���� �� ���������� ������
                    if (search.AnswerGenre != null && search.AnswerGenre.Any())
                    {
                        responseHtml += "<H2>���������� ������:</H2><UL>";
                        foreach (var genre in search.AnswerGenre)
                        {
                            responseHtml += "<LI>" + System.Net.WebUtility.HtmlEncode(genre.GenreName) + " | " +
                            System.Net.WebUtility.HtmlEncode(genre.GenreDescription) + "</LI>";
                        }
                        responseHtml += "</UL>";
                    }
                    else if (!string.IsNullOrEmpty(search.SearchRequest))
                    {
                        responseHtml += "<H2>��� ����������� ���: " +
                                        System.Net.WebUtility.HtmlEncode(search.SearchRequest) + "</H2>";
                    }

                    responseHtml += "<BR><A href='/'>�������</A></BR>" +
                                    "<BR><A href='/genre'>�����</A></BR>" +
                                    "<BR><A href='/form'>������ ������������</A></BR>" +
                                    "</BODY></HTML>";

                    // ��������� ������ Search � ������
                    context.Session.Set("search", search);

                    // ���������� �����
                    await context.Response.WriteAsync(responseHtml);
                });
            });
            // ����� ������������ ���������� �� ������� ���� ������
            app.Map("/searchform2", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // �������� ������ Search �� ������ ��� ������ �����
                    Search search = context.Session.Get<Search>("search") ?? new Search();

                    // ���������, ���� �� �������� 'query' � �������
                    if (!string.IsNullOrEmpty(context.Request.Query["query"]))
                    {
                        // �������� �������� �������
                        search.SearchRequest = context.Request.Query["query"];

                        // �������� ���������� ������
                        ICachedTvshowService cachedGenreService = context.RequestServices.GetService<ICachedTvshowService>();
                        search.AnswerTvshow = cachedGenreService.FindTvshow(search.SearchRequest);
                    }

                    // ���������� HTML-��������
                    string responseHtml = "<HTML><HEAD><TITLE>�����</TITLE></HEAD>" +
                                          "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                                          "<BODY><H1>����� ������</H1>" +
                                          "<FORM method='GET' action='/searchform2'>" +
                                          "<LABEL for='query'>������� ������:</LABEL><br>" +
                                          "<INPUT type='text' id='query' name='query' value='" +
                                          System.Net.WebUtility.HtmlEncode(search.SearchRequest) + "'><br><br>" +
                                          "<INPUT type='submit' value='�����'>" +
                                          "</FORM>";

                    // ���������, ���� �� ���������� ������
                    if (search.AnswerGenre != null && search.AnswerTvshow.Any())
                    {
                        responseHtml += "<H2>���������� ������:</H2><UL>";
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
                        responseHtml += "<H2>��� ����������� ���: " +
                                        System.Net.WebUtility.HtmlEncode(search.SearchRequest) + "</H2>";
                    }

                    responseHtml += "<BR><A href='/'>�������</A></BR>" +
                                    "<BR><A href='/genre'>�����</A></BR>" +
                                    "<BR><A href='/form'>������ ������������</A></BR>" +
                                    "</BODY></HTML>";

                    // ��������� ������ Search � ������
                    context.Session.Set("search", search);

                    // ���������� �����
                    await context.Response.WriteAsync(responseHtml);
                });
            });

            // ��������� �������� � ����������� ������ ������� �� web-�������
            app.Run((context) =>
            {
                string HtmlString = "<HTML><HEAD><TITLE>�������</TITLE></HEAD>" +
                "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                "<BODY><H1>�������</H1>";
                HtmlString += "<H2>������ �������� � ��� �������</H2>";
                HtmlString += "<BR><A href='/'>�������</A></BR>";
                HtmlString += "<BR><A href='/genre'>�����</A></BR>";
                HtmlString += "<BR><A href='/tvshow'>������������</A></BR>";
                HtmlString += "<BR><A href='/employee'>����������</A></BR>";
                HtmlString += "<BR><A href='/form'>������ ������������</A></BR>";
                HtmlString += "<BR><A href='/info'>����������</A></BR>";
                HtmlString += "<BR><A href='/searchform1'>����� 1</A></BR>";
                HtmlString += "<BR><A href='/searchform2'>����� 2</A></BR>";
                HtmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(HtmlString);
            });

            // ������������� MVC - ���������
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