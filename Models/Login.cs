#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HobbyHub.Models;
public class Login
{    

   
    [Required]


    public string LoginUsername{ get; set; }    
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string LoginPassword { get; set; } 
}