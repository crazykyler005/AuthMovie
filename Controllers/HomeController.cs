using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthMovie.Models;
using AuthMovie.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace AuthMovie.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        //private int userId;

        public HomeController(AppDbContext context) {
            _context = context;
            var cookie = HttpContext.Request.Cookies["user_id"];
            HttpContext.Response.Cookies.Append("user_id", "0");
        }

        public async Task<IActionResult> Index() {

            try{
                ViewData["userId"] = Convert.ToInt16(HttpContext.Request.Cookies["user_id"]);
                return View(await _context.Movies.ToListAsync());
            }catch{
                return View("CreateMovie");
            }
            //return View();
        }
        public IActionResult CreateUser(){
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user) {
            if (ModelState.IsValid) {
                // Manual Auth is custom class to hold hash methods
                user.Password = ManualAuth.Sha256(user.Password);
                // Add user and save changes to database.
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Welcome));
            }
            return View(user);
        }

        public IActionResult Welcome() {
            return View();
        }

        public ViewResult Login() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user) {
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddDays(7));

            if (ModelState.IsValid) {
                // attempt to get a user with the matching username from DB.
                User GetUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == user.UserName);
                // if no match on username skip password check.
                if (GetUser != null) {
                    // compare hashed passwords.
                    if (ManualAuth.Sha256Check(user.Password, GetUser.Password)) {
                        // if password match is true return treats.

                        HttpContext.Response.Cookies.Append("user_id", user.Id.ToString(), cookieOptions);

                        return View(nameof(Index));
                    }
                }
            }
            return View("LoginFail");
        }

        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult CreateMovie(){
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMovie([Bind("Id,Name,Director,YearReleased")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nameof(Index));
        }
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("Id,Name,Director,YearReleased")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Playlist(int playlistId){
            List<Playlist> playlists = new List<Playlist>();
            playlists =  await _context.Playlists.ToListAsync();
            foreach(Playlist playlist in playlists){
                if(playlist.Id == playlistId){
                    return View((object)playlist.MovieNames);
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Playlists(){
            return View(await _context.Playlists.ToListAsync());
        }
        [HttpPut]
        public async Task<IActionResult> RemoveMovie(Movie movie){
            List<Playlist> playlists = new List<Playlist>();
            playlists =  await _context.Playlists.ToListAsync();
            int id = Convert.ToInt32(ViewData["currentUser"]);
            Console.WriteLine(id);
            foreach(Playlist playlist in playlists){
                if(id == playlist.UserId){
                    playlist.MovieNames.ToList().Remove(movie.Name);
                    playlist.MovieNames.ToArray();
                    _context.Playlists.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                return View("Playlist",(object)playlist.Id);
            }
            return View("Playlist", (object)id);
        }
    }
}