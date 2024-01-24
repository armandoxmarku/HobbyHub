using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HobbyHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Controllers;
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if (userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Auth", "Home", null);
        }
    }
}
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }
    [HttpGet("Auth")]
    public IActionResult Auth()
    {
        return View("Auth");

    }
    public IActionResult Register(User useriNgaForma)
    {

        if (ModelState.IsValid)
        {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            useriNgaForma.Password = Hasher.HashPassword(useriNgaForma, useriNgaForma.Password);
            _context.Add(useriNgaForma);
            _context.SaveChanges();
            return RedirectToAction("Auth");
        }
        return View("Auth");

    }
    [HttpPost("Login")]
    public IActionResult Login(Login useriNgaForma)
    {

        if (ModelState.IsValid)
        {

            User useriNgaDB = _context.Users
            .FirstOrDefault(e => e.Username == useriNgaForma.LoginUsername);
            if (useriNgaDB == null)
            {
                ModelState.AddModelError("LoginUsername", "Invalid Username");
                return View("Auth");
            }

            PasswordHasher<Login> hasher = new PasswordHasher<Login>();
            var result = hasher.VerifyHashedPassword(useriNgaForma, useriNgaDB.Password, useriNgaForma.LoginPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LoginPassword", "Invalid Password");
                return View("Auth");
            }
            HttpContext.Session.SetInt32("UserId", useriNgaDB.UserId);
            return RedirectToAction("Index");

        }
        return View("Auth");

    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Auth");
    }
    [SessionCheck]

    public IActionResult Index()
    {
        List<Hobby> AllHobbies = _context.Hobbies
                .OrderBy(i => i.Description)
                .Include(i => i.Creator)
                .Include(i => i.associations)
                .ThenInclude(i => i.User)
                .OrderByDescending(i => i.associations.Count())
                .ToList();
        ViewBag.AllHobbies = AllHobbies;

        Func<string, List<string>> GetTopHobbiesByProficiency = (proficiency) =>
        {
            var userHobbiesByProficiency = _context.Associations
                .Include(Associations => Associations.Hobby)
                .Where(Associations => Associations.Proficiency == proficiency);

            var frequencyTable = new Dictionary<string, int>();

            foreach (var userHobby in userHobbiesByProficiency)
            {
                if (!frequencyTable.ContainsKey(userHobby.Hobby.Name))
                {
                    frequencyTable[userHobby.Hobby.Name] = 0;
                }
                frequencyTable[userHobby.Hobby.Name]++;
            }

            var result = new List<string>();

            foreach (var entry in frequencyTable)
            {
                if (result.Count == 0 || entry.Value == frequencyTable[result[0]])
                {
                    result.Add(entry.Key);
                }
                else if (entry.Value > frequencyTable[result[0]])
                {
                    result = new List<string>
                    {
                    entry.Key
                    };
                }
            }


            return result;
        };
        ViewBag.topNoviceHobbies = GetTopHobbiesByProficiency("Novice");
        ViewBag.topIntermediateHobbies = GetTopHobbiesByProficiency("Intermediate");
        ViewBag.topExpertHobbies = GetTopHobbiesByProficiency("Expert");

        return View();
    }
    [SessionCheck]

    [HttpGet("AddHobby")]
    public IActionResult AddHobby()
    {
        return View("AddHobby");
    }
    [SessionCheck]

    [HttpPost("Createhobby")]
    public IActionResult Createhobby(Hobby HobbyForm)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        User? currentUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
        if (ModelState.IsValid)
        {
            HobbyForm.Creator = currentUser;
            _context.Add(HobbyForm);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View("AddHobby");
    }
    [SessionCheck]

    [HttpGet("HobbyDetails/{hobbyId}")]
    public IActionResult HobbyDetails(int HobbyId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        User? currentUser = _context.Users.FirstOrDefault(u => u.UserId == userId);

        ViewBag.CurrentUser = currentUser;

        Hobby theHobby = _context.Hobbies
            .Include(u => u.Creator)
            .Include(u => u.associations)
            .ThenInclude(u => u.User)
            .FirstOrDefault(w => w.HobbyId == HobbyId);

        return View("HobbyDetails", theHobby);
    }
    [SessionCheck]

    [HttpGet("Edit/{hobbyId}")]
    public IActionResult Edit(int hobbyId)
    {
        Hobby theHobby = _context.Hobbies.FirstOrDefault(h => h.HobbyId == hobbyId);


        return View("Edit", theHobby);
    }
    [SessionCheck]

    [HttpPost("HobbyUpdate/{hobbyId}")]
    public IActionResult HobbyUpdate(int hobbyId, Hobby formHobby)
    {

        int? userId = HttpContext.Session.GetInt32("UserId");
        User? currentUser = _context.Users.FirstOrDefault(u => u.UserId == userId);

        Hobby theHobby = _context.Hobbies.FirstOrDefault(h => h.HobbyId == hobbyId);
        if (ModelState.IsValid)
        {
            theHobby.Name = formHobby.Name;
            theHobby.Description = formHobby.Description;
            theHobby.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View("Edit", formHobby);

    }
    [SessionCheck]
    [HttpPost]
    public IActionResult CreateUserHobby(int hobbyId, int userId, string proficiency)
    {
        var association = new Association
        {
            HobbyID = hobbyId,
            UserId = userId,
            Proficiency = proficiency
        };

        _context.Associations.Add(association);
        _context.SaveChanges();


        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
