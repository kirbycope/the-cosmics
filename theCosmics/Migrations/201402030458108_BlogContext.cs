namespace theCosmics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BlogContext : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blogs", "Author", c => c.String(nullable: false));
            DropColumn("dbo.Blogs", "Aurthor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Blogs", "Aurthor", c => c.String(nullable: false));
            DropColumn("dbo.Blogs", "Author");
        }
    }
}
