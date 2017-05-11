using System.Data.Entity;

namespace WebService.Data
{
    public class MyDBContext:DbContext
    {
        public MyDBContext():base("DbConnString")
        {
           Database.SetInitializer<MyDBContext>(null);
        }

        //设定单复数形式
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }


        public DbSet<TempInfo> TempInfo { get; set; }

        public DbSet<TestView> TestView { get; set; }
    }
}