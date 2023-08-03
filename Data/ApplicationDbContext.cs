using Common.Utilities;
using Entities.Common;
using Entities.Models.Advertises;
using Entities.Models.User;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        #region Override OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // we need assembly of Entities Class Library, IEntity exists in that class library and we get assembly from that
            var entitiesAssembly = typeof(IEntity).Assembly;

            // register all entities to database automaticilly by Reflection
            //*** they should inherit IEntity ***///
            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);

            // this method is used for when we have fluent api in classes and we wanna push themm into database operations
            //*** they should inherit IEntity ***///
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);

            // for cascade handlling
            modelBuilder.AddRestrictDeleteBehaviorConvention();

            // it submits SequentialGuid  for classes,those inherits IEntity<Guid> 
            modelBuilder.AddSequentialGuidForIdConvention();

            // When it creates tables , it pluralize name   example =>  Class name : User , TableName : Users
            modelBuilder.AddPluralizingTableNameConvention();


            modelBuilder.Entity<User>()
               .HasIndex(x => x.UserName)
               .IsUnique();

            modelBuilder.Entity<User>()
               .HasIndex(x => x.PhoneNumber)
               .IsUnique();
           
            modelBuilder.Entity<User>()
               .HasIndex(x => x.Email)
            .IsUnique();

            
        }
        #endregion 

        #region Override SaveChanges

        // this code is used for correct datas  before SaveChanges in DB
        public override int SaveChanges()
        {
            CleanString._cleanString(this);
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            CleanString._cleanString(this);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            CleanString._cleanString(this);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            CleanString._cleanString(this);
            return base.SaveChangesAsync(cancellationToken);
        }
        // ***
        #endregion

    }
}
