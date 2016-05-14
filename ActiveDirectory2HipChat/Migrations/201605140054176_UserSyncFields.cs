namespace ActiveDirectory2HipChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSyncFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AddedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "UpdatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "SyncedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "IsSynced", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Users", "Principal", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "Principal", c => c.String());
            DropColumn("dbo.Users", "IsSynced");
            DropColumn("dbo.Users", "SyncedOn");
            DropColumn("dbo.Users", "UpdatedOn");
            DropColumn("dbo.Users", "AddedOn");
        }
    }
}
