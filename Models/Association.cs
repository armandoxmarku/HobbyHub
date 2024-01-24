#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HobbyHub.Models;

    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        public int UserId { get; set; }
        public int HobbyID { get; set; }
        public string Proficiency { get; set; }
        public User User { get; set; }
        public Hobby Hobby { get; set; }
    }
