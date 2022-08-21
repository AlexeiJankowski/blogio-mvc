using Blogio.Models;

namespace Blogio
{
    public class PaginatedList
    {
        public List<Post> Posts { get; set; } = new List<Post>();
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int MaxPage => Convert.ToInt32(Math.Ceiling((double)this.Posts.Count() / PageSize));
        public int MinPage => 1;

        public string DisablePrevButton => CurrentPage <= MinPage ? "disabled" : "";
        public string DisableNextButton => CurrentPage >= MaxPage ? "disabled" : "";

        public int PageStartNumber()
        {
            if (MaxPage < 5)
            {
                return MinPage;
            }
            else if (CurrentPage <= 3)
            {
                return MinPage;
            }
            else if ((CurrentPage > MinPage + 2) && (CurrentPage <= MaxPage - 2))
            {
                return CurrentPage - 2;
            }
            else if ((CurrentPage > MinPage + 2) && (CurrentPage > MaxPage - 2))
            {
                return MaxPage - 4;
            }
            return 1;
        }
        public int PageEndNumber()
        {
            if (MaxPage < 5)
            {
                return MaxPage;
            }
            else if (CurrentPage <= 3)
            {
                return 5;
            }
            else if ((CurrentPage > MinPage + 2) && (CurrentPage <= MaxPage - 2))
            {
                return CurrentPage + 2;
            }
            else if ((CurrentPage > MinPage + 2) && (CurrentPage > MaxPage - 2))
            {
                return MaxPage;
            }
            return 1;
        }


        public static List<Post> UsePagination(List<Post> post, int pageSize, int pageNumber)
        {
            return post.OrderByDescending(p => p.Date).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList() ?? post.Skip(pageNumber - 1).ToList();
        }

        //Write an algorithm for the TopRatedPosts method
        //Placeholder
        public static List<Post> TopRatedPosts(List<Post> post, int pageSize, int pageNumber)
        {
            return post.OrderByDescending(r => r.Rating).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList() ?? post.Skip(pageNumber - 1).ToList();
        }
    }
}