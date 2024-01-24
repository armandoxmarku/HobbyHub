#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace HobbyHub.Models;

public class MyContext : DbContext 
{     
    public MyContext(DbContextOptions options) : base(options) { }   

   public DbSet<User> Users {get;set;}
    public DbSet<Hobby> Hobbies {get;set;}
    public DbSet<Association> Associations {get;set;}
}