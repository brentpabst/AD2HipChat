namespace ActiveDirectory2HipChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncedOnNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "SyncedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "SyncedOn", c => c.DateTime(nullable: false));
        }
    }
}
