using EvoltingStore.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EvoltingStore.Pages
{
    public class DetailsModel : PageModel
    {
        EvoltingStoreContext context = new EvoltingStoreContext();

        public async Task OnGetAsync(int gameId)
        {
            Game game = context.Games.Include(g => g.Genres).Include(g => g.Comments).ThenInclude(c => c.User).Include(g => g.Users).FirstOrDefault(g => g.GameId == gameId);
            Boolean isFavourite = false;
            List<int> gameOrderSuccessList = null;


            String userJSON = HttpContext.Session.GetString("user");
            if(userJSON != null)
            {
                User us = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userJSON);

                User user = context.Users.FirstOrDefault(u => u.UserId == us.UserId);

                gameOrderSuccessList = context.OrderDetails
               .Where(od => od.Order.Status == true && od.Order.UserId == user.UserId)
               .Select(od => od.GameId)
               .Distinct()
               .ToList();

                if (game.Users.Contains(user))
                {
                    isFavourite = true;
                }
            }


            GameRequirement minimum = context.GameRequirements.FirstOrDefault(gr => gr.GameId == gameId && gr.Type.Equals("minimum"));
            GameRequirement recommend = context.GameRequirements.FirstOrDefault(gr => gr.GameId == gameId && gr.Type.Equals("recommend"));

            ViewData["isFavourite"] = isFavourite;
            ViewData["game"] = game;
            ViewData["minimum"] = minimum;
            ViewData["recommend"] = recommend;
            ViewData["gameOrderSuccessList"] = gameOrderSuccessList;

        }

        public async Task OnPostFavouriteAsync(int gameId)
        {
            Game game = context.Games.Include(g => g.Genres).Include(g => g.Comments).ThenInclude(c => c.User).Include(g => g.Users).FirstOrDefault(g => g.GameId == gameId);
            Boolean isFavourite = false;

            String userJSON = HttpContext.Session.GetString("user");
            if(userJSON != null)
            {
                User us = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userJSON);

                User user = context.Users.FirstOrDefault(u => u.UserId == us.UserId);


                if (game.Users.Contains(user))
                {
                    game.Users.Remove(user);
                }
                else
                {
                    game.Users.Add(user);
                    isFavourite = true;
                }
            }

            context.Games.Update(game);
            context.SaveChanges();


            GameRequirement minimum = context.GameRequirements.FirstOrDefault(gr => gr.GameId == gameId && gr.Type.Equals("minimum"));
            GameRequirement recommend = context.GameRequirements.FirstOrDefault(gr => gr.GameId == gameId && gr.Type.Equals("recommend"));

            ViewData["isFavourite"] = isFavourite;
            ViewData["game"] = game;
            ViewData["minimum"] = minimum;
            ViewData["recommend"] = recommend;

        }

        public void OnPostComment(int gameId, string messageInput)
        {
            Comment c = new Comment();
            Game game = context.Games.Include(g => g.Genres).Include(g => g.Comments).ThenInclude(c => c.User).Include(g => g.Users).FirstOrDefault(g => g.GameId == gameId);
            Boolean isFavourite = false;

            String userJSON = HttpContext.Session.GetString("user");
            
            if(userJSON != null)
            {
                User us = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userJSON);

                c.UserId = us.UserId;
                c.GameId = gameId;
                c.Content = messageInput;
                c.PostTime = DateTime.Now;

                context.Comments.Add(c);

                User user = context.Users.FirstOrDefault(u => u.UserId == us.UserId);


                if (game.Users.Contains(user))
                {
                    isFavourite = true;
                }

                context.Games.Update(game);
                context.SaveChanges();
            }


            GameRequirement minimum = context.GameRequirements.FirstOrDefault(gr => gr.GameId == gameId && gr.Type.Equals("minimum"));
            GameRequirement recommend = context.GameRequirements.FirstOrDefault(gr => gr.GameId == gameId && gr.Type.Equals("recommend"));

            ViewData["isFavourite"] = isFavourite;
            ViewData["game"] = game;
            ViewData["minimum"] = minimum;
            ViewData["recommend"] = recommend;
        }
    }
}
