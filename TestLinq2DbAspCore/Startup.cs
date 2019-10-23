using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestLinq2DbAspCore.Models;

namespace TestLinq2DbAspCore
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;

            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (s, s1) => { Console.WriteLine(s); };
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = configuration.GetConnectionString("DefaultConnection");
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<IssueContext>(options =>
                    options.UseNpgsql(connection)
                );
            
            LinqToDBForEFTools.Initialize();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IssueContext db)
        {

            TstLeftJoin(db);

            Console.WriteLine("hello");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private static BulkCopyOptions _bulkCopyOptions = new BulkCopyOptions { KeepIdentity = true, BulkCopyType = BulkCopyType.MultipleRows, MaxBatchSize = 10 };

        public async Task TstLeftJoin(IssueContext db) 
        { 
            //Int Ids
            try
            {
                using (var l2db = db.CreateLinqToDbConnection())
                {

                    LinqToDB.Common.Configuration.Linq.GenerateExpressionTest = true;
                    var test =
                        from dt in l2db.GetTable<Devtypes>()
                        from d in l2db.GetTable<Devices>().LeftJoin(pr => pr.Devtypeid == dt.Devtypeid)
                        select new { dt, d }
                           ;

                    var x0 = test.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(JsonConvert.SerializeObject(e).ToString());
                //throw;
            }

            //Guid Ids
            try
            {
                using (var l2db = db.CreateLinqToDbConnection())
                {

                    LinqToDB.Common.Configuration.Linq.GenerateExpressionTest = true;
                    var test =
                        from g in l2db.GetTable<DAL_Gateway>()
                        from gs in l2db.GetTable<DAL_GatewaySettings>().LeftJoin(pr => pr.GatewayId == g.Id)
                        select new { g, gs }
                           ;

                    var x0 = test.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(JsonConvert.SerializeObject(e).ToString());
                //throw;
            }
        }
    }

    public static class L2dbExtensions
    {
        private static BulkCopyOptions _bulkCopyOptions = new BulkCopyOptions { KeepIdentity = true };
        public static async Task<int> TestInsertAsync<TEntry>(this DataConnection l2db, TEntry entry) where TEntry : BTest, new()
            => await l2db.InsertAsync(entry);        
        
        public static async Task<int> TestBulkCopy<TEntry>(this DataConnection l2db, TEntry entry) where TEntry : BTest, new()
            => (int) l2db.BulkCopy(_bulkCopyOptions, new TEntry[] { entry }).RowsCopied;
    }
}
