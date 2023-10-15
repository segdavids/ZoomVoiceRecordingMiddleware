using Eleveo_EFCX_Connector_API.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Eleveo_EFCX_Connector_API.Data
{
    public class EleveoEFCXDBContext:DbContext 
    {
        public EleveoEFCXDBContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
    }
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<EleveoEFCXDBContext>
    {
        public EleveoEFCXDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EleveoEFCXDBContext>();
            optionsBuilder.UseSqlServer(Helper.GetConnectionString());

            return new EleveoEFCXDBContext(optionsBuilder.Options);
        }
    }


        //public DbSet<CiscoRecordingObject> CiscoRecordingObjects { get; set; }
        public DbSet<ConversationsDto> Conversations { get; set; }
        public DbSet<ConversationDataDto> Conversation_Data { get; set; }


    }
}
