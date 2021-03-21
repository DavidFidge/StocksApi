using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper;

using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

using Serilog;

using StocksApi.Core;
using StocksApi.Data;
using StocksApi.Extensions;
using StocksApi.Service.Companies;
using StocksApi.Service.EndOfDayData;

namespace StocksApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            services.AddSingleton(Log.Logger);
            services.AddSingleton<IEndOfDayUpdate, EndOfDayUpdate>();
            services.AddSingleton<ICompanyInformation, CompanyInformation>();
            services.AddSingleton<ICompanyInformationStore, AsxCompanyInformationStore>();
            services.AddSingleton<IEndOfDayUpdate, EndOfDayUpdate>();
            services.AddSingleton<IEndOfDayStore, FileEndOfDayStore>();

            var origin = Configuration.GetValueOrEnvironmentVariable(Constants.StocksApiClientApplicationOrigin);

            // CORS config for Azure is done in the portal, not in code
            if (!String.IsNullOrEmpty(origin))
            {
                services.AddCors(
                    options =>
                    {
                        options.AddDefaultPolicy(
                            builder =>
                            {
                                builder.WithOrigins(origin);
                                builder.AllowAnyMethod();
                                builder.AllowAnyHeader();
                            });
                    });
            }

            services.AddOData();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddMvcCore(options =>
            {
                options.EnableEndpointRouting = false;

                foreach (var outputFormatter in options.OutputFormatters.OfType<OutputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }

                foreach (var inputFormatter in options.InputFormatters.OfType<InputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });

            var connectionString = Configuration.GetConnectionString(Constants.StocksApiStocksDbConnectionString);

            if (String.IsNullOrEmpty(connectionString))
                connectionString = Environment.GetEnvironmentVariable(Constants.StocksApiStocksDbConnectionString);

            if (Environment.GetEnvironmentVariable(Constants.AspNetCoreEnvironment) == Constants.AspNetCoreEnvironmentDevelopment)
            {
                services.AddDbContext<StocksContext>(o =>
                    {
                        o.UseSqlite(connectionString);

                        if (Constants.TrueOrOne.Contains(Environment.GetEnvironmentVariable(Constants.StocksApiSensitiveLogging)?.ToLower()))
                            o.EnableSensitiveDataLogging();
                    }
                );
            }
            else
            {
                services.AddDbContext<StocksContext>(o =>
                    {
                        o.UseSqlServer(connectionString);

                        if (Constants.TrueOrOne.Contains(Environment.GetEnvironmentVariable(Constants.StocksApiSensitiveLogging)?.ToLower()))
                            o.EnableSensitiveDataLogging();
                    }
                );
            }

            services.AddAutoMapper(mc => mc.AddProfile(new StockProfile()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            if (env.IsDevelopment())
                app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Filter().OrderBy().Count().MaxTop(10);
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
